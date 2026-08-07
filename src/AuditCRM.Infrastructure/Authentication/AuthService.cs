using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuditCRM.Application.Features.Auth.DTOs;
using AuditCRM.Application.Interfaces;
using AuditCRM.Domain.Entities;
using AuditCRM.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuditCRM.Infrastructure.Authentication;

public sealed class AuthService : IAuthService
{
    private readonly AuditDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        AuditDbContext dbContext,
        IPasswordHasher passwordHasher,
        IOptions<JwtSettings> jwtOptions)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<AuthResponseDto> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            throw new UnauthorizedAccessException("E-mail ou senha inválidos.");
        }

        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _dbContext.Users
            .SingleOrDefaultAsync(
                x => x.Email == email,
                cancellationToken);

        if (user is null ||
            !user.IsActive ||
            !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("E-mail ou senha inválidos.");
        }

        user.RegisterLogin();

        var tokenResult = CreateAccessToken(user);
        var refreshTokenValue = GenerateRefreshToken();
        var refreshTokenHash = HashRefreshToken(refreshTokenValue);

        var refreshToken = new RefreshToken(
            user.Id,
            refreshTokenHash,
            DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays));

        _dbContext.RefreshTokens.Add(refreshToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto(
            tokenResult.Token,
            refreshTokenValue,
            tokenResult.ExpiresAt,
            user.Id,
            user.Name,
            user.Email,
            user.IsAdministrator);
    }

    public async Task<AuthResponseDto> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new UnauthorizedAccessException("Refresh token inválido.");

        var tokenHash = HashRefreshToken(refreshToken);

        var storedToken = await _dbContext.RefreshTokens
            .Include(x => x.User)
            .SingleOrDefaultAsync(
                x => x.TokenHash == tokenHash,
                cancellationToken);

        if (storedToken is null ||
            !storedToken.IsActive ||
            storedToken.User is null ||
            !storedToken.User.IsActive)
        {
            throw new UnauthorizedAccessException("Refresh token inválido ou expirado.");
        }

        var newRefreshTokenValue = GenerateRefreshToken();
        var newRefreshTokenHash = HashRefreshToken(newRefreshTokenValue);

        storedToken.Revoke(newRefreshTokenHash);

        var newRefreshToken = new RefreshToken(
            storedToken.UserId,
            newRefreshTokenHash,
            DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays));

        _dbContext.RefreshTokens.Add(newRefreshToken);

        var accessToken = CreateAccessToken(storedToken.User);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto(
            accessToken.Token,
            newRefreshTokenValue,
            accessToken.ExpiresAt,
            storedToken.User.Id,
            storedToken.User.Name,
            storedToken.User.Email,
            storedToken.User.IsAdministrator);
    }

    public async Task LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return;

        var tokenHash = HashRefreshToken(refreshToken);

        var storedToken = await _dbContext.RefreshTokens
            .SingleOrDefaultAsync(
                x => x.TokenHash == tokenHash,
                cancellationToken);

        if (storedToken is null)
            return;

        storedToken.Revoke();

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private (string Token, DateTime ExpiresAt) CreateAccessToken(User user)
    {
        if (string.IsNullOrWhiteSpace(_jwtSettings.Key))
        {
            throw new InvalidOperationException(
                "A chave JWT não foi configurada.");
        }

        var expiresAt = DateTime.UtcNow
            .AddMinutes(_jwtSettings.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),

            new(
                JwtRegisteredClaimNames.Email,
                user.Email),

            new(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new(
                ClaimTypes.Name,
                user.Name),

            new(
                ClaimTypes.Email,
                user.Email),

            new(
                ClaimTypes.Role,
                user.IsAdministrator
                    ? "Administrator"
                    : "User")
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Key));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt,
            signingCredentials: credentials);

        return (
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt);
    }

    private static string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);

        return Convert
            .ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string HashRefreshToken(string token)
    {
        var bytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(bytes);
    }
}