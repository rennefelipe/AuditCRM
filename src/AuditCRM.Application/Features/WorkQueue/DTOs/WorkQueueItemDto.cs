namespace AuditCRM.Application.Features.WorkQueue.DTOs;

public sealed record WorkQueueItemDto(
    Guid StoreProcessId,
    Guid StoreId,
    string StoreName,
    string? StoreLuc,
    string? StoreDocument,

    Guid ShoppingId,
    string ShoppingName,

    Guid? ShoppingGroupId,
    string? ShoppingGroupName,

    Guid? InstallationTypeId,
    string? InstallationTypeName,

    int Status,
    int Priority,
    int Frequency,

    Guid? ResponsibleUserId,
    string? ResponsibleUserName,

    string? NextAction,
    DateTime? NextActionAt,

    DateTime StartedAt,
    DateTime? CompletedAt,

    bool IsClosed);