using AuditCRM.Domain.Enums;

namespace AuditCRM.Application.Features.ProcessInstallations.DTOs;

public sealed record ProcessInstallationDto(
    Guid Id,
    Guid StoreProcessId,
    Guid? ResponsibleUserId,
    string? ResponsibleUserName,
    Guid? ErpId,
    string? ErpName,
    DateTime InstalledAt,
    bool Successful,
    XmlLocationType XmlLocationType,
    int? NumberOfRegisters,
    string? Result,
    string? Notes,
    DateTime CreatedAt);