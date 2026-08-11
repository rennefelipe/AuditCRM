using AuditCRM.Domain.Enums;

namespace AuditCRM.Application.Features.ProcessInstallations.DTOs;

public sealed record CreateProcessInstallationRequestDto(
    Guid StoreProcessId,
    DateTime InstalledAt,
    bool Successful,
    Guid? ResponsibleUserId,
    Guid? ErpId,
    XmlLocationType XmlLocationType,
    int? NumberOfRegisters,
    string? Result,
    string? Notes);