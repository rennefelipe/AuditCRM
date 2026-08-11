using AuditCRM.Application.Features.WorkQueue.DTOs;

namespace AuditCRM.Application.Interfaces;

public interface IWorkQueueService
{
    Task<IReadOnlyList<WorkQueueItemDto>> GetAsync(
        Guid? shoppingGroupId,
        Guid? shoppingId,
        Guid? responsibleUserId,
        bool? overdueOnly,
        CancellationToken cancellationToken = default);
}