namespace AuditCRM.Application.Features.StoreContacts.DTOs;

public sealed record StoreContactDto(
    Guid Id,
    Guid StoreId,
    string StoreName,
    string Name,
    string? Position,
    string? Phone1,
    string? Phone2,
    string? Phone3,
    string? Email1,
    string? Email2,
    string? Email3,
    bool IsMainContact,
    string? Notes,
    bool IsActive,
    DateTime CreatedAt);