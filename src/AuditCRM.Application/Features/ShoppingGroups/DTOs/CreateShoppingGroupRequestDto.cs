namespace AuditCRM.Application.Features.ShoppingGroups.DTOs;

public sealed record CreateShoppingGroupRequestDto(
    string Name,
    string? Description);