using AuditCRM.Domain.Enums;

namespace AuditCRM.Application.Features.StoreProcesses.DTOs;

public sealed record StoreProcessDto(
    Guid Id,
    Guid StoreId,
    string StoreName,
    Guid? InstallationTypeId,
    string? InstallationTypeName,
    Guid? ResponsibleUserId,
    string? ResponsibleUserName,
    StoreProcessStatus Status,
    ProcessPriority Priority,
    string? NextAction,
    DateTime? NextActionAt,
    DateTime StartedAt,
    DateTime? CompletedAt,
    string? Notes,
    bool IsClosed,
    DateTime CreatedAt);