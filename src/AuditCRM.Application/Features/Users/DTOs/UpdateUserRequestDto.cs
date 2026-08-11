namespace AuditCRM.Application.Features.Users.DTOs;

public sealed record UpdateUserRequestDto(
    string Name,
    string Email,
    string? Phone,
    bool IsAdministrator);