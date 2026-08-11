namespace AuditCRM.Application.Features.ShoppingGroups.DTOs;

public sealed record ShoppingGroupDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAt);