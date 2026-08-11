namespace AuditCRM.Application.Features.Erps.DTOs;

public sealed record UpdateErpRequestDto(
    string Name,
    string? Manufacturer,
    string? Notes);