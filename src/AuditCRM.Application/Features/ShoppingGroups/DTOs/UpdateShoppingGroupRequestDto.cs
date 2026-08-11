namespace AuditCRM.Application.Features.ShoppingGroups.DTOs;

public sealed record UpdateShoppingGroupRequestDto(
    string Name,
    string? Description);