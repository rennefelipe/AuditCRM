namespace AuditCRM.Application.Features.Users.DTOs;

public sealed record CreateUserRequestDto(
    string Name,
    string Email,
    string Password,
    string? Phone,
    bool IsAdministrator);