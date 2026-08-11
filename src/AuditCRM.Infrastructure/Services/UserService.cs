using AuditCRM.Application.Features.Users.DTOs;
using AuditCRM.Application.Interfaces;
using AuditCRM.Domain.Entities;
using AuditCRM.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuditCRM.Infrastructure.Services;

public sealed class UserService : IUserService
{
    private readonly AuditDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(
        AuditDbContext dbContext,
        IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.Name)
            .Select(user => new UserDto(
                user.Id,
                user.Name,
                user.Email,
                user.Phone,
                user.IsAdministrator,
                user.IsActive,
                user.LastLoginAt,
                user.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<UserDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Where(user => user.Id == id)
            .Select(user => new UserDto(
                user.Id,
                user.Name,
                user.Email,
                user.Phone,
                user.IsAdministrator,
                user.IsActive,
                user.LastLoginAt,
                user.CreatedAt))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<UserDto> CreateAsync(
        CreateUserRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        ValidateCreateRequest(request);

        var email = request.Email.Trim().ToLowerInvariant();

        var emailExists = await _dbContext.Users
            .IgnoreQueryFilters()
            .AnyAsync(
                user => user.Email == email,
                cancellationToken);

        if (emailExists)
        {
            throw new InvalidOperationException(
                "Já existe um usuário cadastrado com este e-mail.");
        }

        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = new User(
            request.Name,
            email,
            passwordHash,
            request.Phone,
            request.IsAdministrator);

        user.MarkCreatedBy(currentUserId);

        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(user);
    }

    public async Task<UserDto> UpdateAsync(
        Guid id,
        UpdateUserRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException(
                "O nome do usuário é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException(
                "O e-mail do usuário é obrigatório.");
        }

        var user = await FindRequiredAsync(
            id,
            cancellationToken);

        var email = request.Email.Trim().ToLowerInvariant();

        var emailExists = await _dbContext.Users
            .IgnoreQueryFilters()
            .AnyAsync(
                item =>
                    item.Email == email &&
                    item.Id != id,
                cancellationToken);

        if (emailExists)
        {
            throw new InvalidOperationException(
                "Já existe outro usuário cadastrado com este e-mail.");
        }

        user.UpdateProfile(
            request.Name,
            email,
            request.Phone,
            request.IsAdministrator);

        user.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(user);
    }

    public async Task ChangePasswordAsync(
        Guid id,
        ChangeUserPasswordRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.NewPassword))
        {
            throw new ArgumentException(
                "A nova senha é obrigatória.");
        }

        if (request.NewPassword.Length < 8)
        {
            throw new ArgumentException(
                "A senha deve possuir pelo menos 8 caracteres.");
        }

        var user = await FindRequiredAsync(
            id,
            cancellationToken);

        var passwordHash =
            _passwordHasher.Hash(request.NewPassword);

        user.ChangePassword(passwordHash);
        user.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ActivateAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var user = await FindRequiredAsync(
            id,
            cancellationToken);

        user.Activate();
        user.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var user = await FindRequiredAsync(
            id,
            cancellationToken);

        user.Deactivate();
        user.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var user = await FindRequiredAsync(
            id,
            cancellationToken);

        user.Delete(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<User> FindRequiredAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .SingleOrDefaultAsync(
                item => item.Id == id,
                cancellationToken);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "Usuário não encontrado.");
        }

        return user;
    }

    private static void ValidateCreateRequest(
        CreateUserRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException(
                "O nome do usuário é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException(
                "O e-mail do usuário é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException(
                "A senha é obrigatória.");
        }

        if (request.Password.Length < 8)
        {
            throw new ArgumentException(
                "A senha deve possuir pelo menos 8 caracteres.");
        }
    }

    private static UserDto Map(User user)
    {
        return new UserDto(
            user.Id,
            user.Name,
            user.Email,
            user.Phone,
            user.IsAdministrator,
            user.IsActive,
            user.LastLoginAt,
            user.CreatedAt);
    }
}