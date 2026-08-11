namespace AuditCRM.Application.Features.Erps.DTOs;

public sealed record ErpDto(
    Guid Id,
    string Name,
    string? Manufacturer,
    string? Notes,
    bool IsActive,
    DateTime CreatedAt);