namespace AuditCRM.Application.Features.Users.DTOs;

public sealed record UserDto(
    Guid Id,
    string Name,
    string Email,
    string? Phone,
    bool IsAdministrator,
    bool IsActive,
    DateTime? LastLoginAt,
    DateTime CreatedAt);