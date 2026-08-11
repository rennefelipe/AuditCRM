using AuditCRM.Domain.Enums;

namespace AuditCRM.Application.Features.StoreProcesses.DTOs;

public sealed record CreateStoreProcessRequestDto(
    Guid StoreId,
    Guid? InstallationTypeId,
    Guid? ResponsibleUserId,
    StoreProcessStatus Status,
    ProcessPriority Priority,
    ProcessFrequency Frequency,
    string? NextAction,
    DateTime? NextActionAt,
    string? Notes);