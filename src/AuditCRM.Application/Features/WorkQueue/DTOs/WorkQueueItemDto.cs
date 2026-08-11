namespace AuditCRM.Application.Features.WorkQueue.DTOs;

public sealed record WorkQueueItemDto(
    Guid StoreProcessId,
    Guid StoreId,
    string StoreName,
    Guid ShoppingId,
    string ShoppingName,
    Guid? ShoppingGroupId,
    string? ShoppingGroupName,
    int Status,
    int Priority,
    Guid? ResponsibleUserId,
    string? ResponsibleUserName,
    string? NextAction,
    DateTime? NextActionAt,
    DateTime StartedAt,
    bool IsClosed);