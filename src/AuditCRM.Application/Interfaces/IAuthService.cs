using AuditCRM.Application.Features.Auth.DTOs;

namespace AuditCRM.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default);

    Task<AuthResponseDto> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    Task LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);
}