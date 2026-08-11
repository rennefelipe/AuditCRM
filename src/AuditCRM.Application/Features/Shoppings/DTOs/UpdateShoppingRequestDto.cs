namespace AuditCRM.Application.Features.Shoppings.DTOs;

public sealed record UpdateShoppingRequestDto(
    Guid ShoppingGroupId,
    string Name,
    string? CorporateName,
    string? Document,
    string? Address,
    string? Number,
    string? District,
    string? City,
    string? State,
    string? ZipCode,
    string? ContactName,
    string? ContactEmail,
    string? ContactPhone,
    bool PaysInstallation,
    string? Notes);