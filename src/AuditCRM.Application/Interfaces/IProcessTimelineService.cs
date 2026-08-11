using AuditCRM.Application.Features.Timelines.DTOs;

namespace AuditCRM.Application.Interfaces;

public interface IProcessTimelineService
{
    Task<IReadOnlyList<TimelineItemDto>> GetAsync(
        Guid storeProcessId,
        CancellationToken cancellationToken = default);
}