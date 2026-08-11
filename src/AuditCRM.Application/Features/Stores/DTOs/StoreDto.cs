using AuditCRM.Domain.Enums;

namespace AuditCRM.Application.Features.Stores.DTOs;

public sealed record StoreDto(
    Guid Id,
    Guid ShoppingId,
    string ShoppingName,
    Guid? ErpId,
    string? ErpName,
    string? Luc,
    string TradeName,
    string? CorporateName,
    string? Document,
    string? StateRegistration,
    int? NumberOfRegisters,
    MonitoringStatus MonitoringStatus,
    string? BusinessType,
    string? Notes,
    bool IsActive,
    DateTime CreatedAt);