using AuditCRM.Domain.Enums;

namespace AuditCRM.Application.Features.Stores.DTOs;

public sealed record CreateStoreRequestDto(
    Guid ShoppingId,
    Guid? ErpId,
    string TradeName,
    string? Luc,
    string? CorporateName,
    string? Document,
    string? StateRegistration,
    int? NumberOfRegisters,
    MonitoringStatus MonitoringStatus,
    string? BusinessType,
    string? Notes);