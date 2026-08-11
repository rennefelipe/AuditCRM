using AuditCRM.Application.Features.Dashboard.DTOs;
using AuditCRM.Application.Interfaces;
using AuditCRM.Domain.Enums;
using AuditCRM.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuditCRM.Infrastructure.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly AuditDbContext _dbContext;

    public DashboardService(
        AuditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(
        Guid? shoppingGroupId,
        Guid? shoppingId,
        Guid? responsibleUserId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var startOfToday = now.Date;
        var startOfTomorrow = startOfToday.AddDays(1);

        var query = _dbContext.StoreProcesses
            .AsNoTracking()
            .AsQueryable();

        if (shoppingGroupId.HasValue)
        {
            query = query.Where(
                x =>
                    x.Store != null &&
                    x.Store.Shopping != null &&
                    x.Store.Shopping.ShoppingGroupId ==
                    shoppingGroupId.Value);
        }

        if (shoppingId.HasValue)
        {
            query = query.Where(
                x =>
                    x.Store != null &&
                    x.Store.ShoppingId ==
                    shoppingId.Value);
        }

        if (responsibleUserId.HasValue)
        {
            query = query.Where(
                x =>
                    x.ResponsibleUserId ==
                    responsibleUserId.Value);
        }

        var totalProcesses = await query
            .CountAsync(cancellationToken);

        var openProcesses = await query
            .CountAsync(
                x => !x.IsClosed,
                cancellationToken);

        var installed = await query
            .CountAsync(
                x => x.Status == StoreProcessStatus.Installed,
                cancellationToken);

        var inNegotiation = await query
            .CountAsync(
                x => x.Status == StoreProcessStatus.InNegotiation,
                cancellationToken);

        var notifiedNoResponse = await query
            .CountAsync(
                x => x.Status == StoreProcessStatus.NotifiedNoResponse,
                cancellationToken);

        var notAuthorized = await query
            .CountAsync(
                x => x.Status == StoreProcessStatus.NotAuthorized,
                cancellationToken);

        var leftShopping = await query
            .CountAsync(
                x => x.Status == StoreProcessStatus.LeftShopping,
                cancellationToken);

        var overdueActions = await query
            .CountAsync(
                x =>
                    !x.IsClosed &&
                    x.NextActionAt.HasValue &&
                    x.NextActionAt.Value < now,
                cancellationToken);

        var dueToday = await query
            .CountAsync(
                x =>
                    !x.IsClosed &&
                    x.NextActionAt.HasValue &&
                    x.NextActionAt.Value >= startOfToday &&
                    x.NextActionAt.Value < startOfTomorrow,
                cancellationToken);

        var withoutNextAction = await query
            .CountAsync(
                x =>
                    !x.IsClosed &&
                    !x.NextActionAt.HasValue,
                cancellationToken);

        var processIds = query.Select(x => x.Id);

        var scheduledAppointments =
            await _dbContext.ProcessAppointments
                .AsNoTracking()
                .CountAsync(
                    x =>
                        processIds.Contains(x.StoreProcessId) &&
                        (
                            x.Status == AppointmentStatus.Scheduled ||
                            x.Status == AppointmentStatus.Confirmed ||
                            x.Status == AppointmentStatus.Rescheduled
                        ),
                    cancellationToken);

        return new DashboardSummaryDto(
            totalProcesses,
            openProcesses,
            installed,
            inNegotiation,
            notifiedNoResponse,
            notAuthorized,
            leftShopping,
            scheduledAppointments,
            overdueActions,
            dueToday,
            withoutNextAction);
    }
}