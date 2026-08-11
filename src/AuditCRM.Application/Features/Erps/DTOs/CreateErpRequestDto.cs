namespace AuditCRM.Application.Features.Erps.DTOs;

public sealed record CreateErpRequestDto(
    string Name,
    string? Manufacturer,
    string? Notes);