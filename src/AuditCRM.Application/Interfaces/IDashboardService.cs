using AuditCRM.Application.Features.Dashboard.DTOs;

namespace AuditCRM.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(
        Guid? shoppingGroupId,
        Guid? shoppingId,
        Guid? responsibleUserId,
        CancellationToken cancellationToken = default);
}