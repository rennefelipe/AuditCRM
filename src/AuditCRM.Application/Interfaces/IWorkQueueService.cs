using AuditCRM.Application.Features.WorkQueue.DTOs;

namespace AuditCRM.Application.Interfaces;

public interface IWorkQueueService
{
    Task<IReadOnlyList<WorkQueueItemDto>> GetAsync(
        Guid? shoppingGroupId,
        Guid? shoppingId,
        Guid? storeId,
        string? search,
        Guid? installationTypeId,
        int? status,
        Guid? responsibleUserId,
        int? frequency,
        int? priority,
        bool? overdueOnly,
        CancellationToken cancellationToken = default);
}