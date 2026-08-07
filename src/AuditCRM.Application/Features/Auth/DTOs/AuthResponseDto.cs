namespace AuditCRM.Application.Features.Auth.DTOs;

public sealed record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    Guid UserId,
    string Name,
    string Email,
    bool IsAdministrator);