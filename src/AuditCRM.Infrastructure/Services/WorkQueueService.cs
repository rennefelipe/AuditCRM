using AuditCRM.Application.Features.WorkQueue.DTOs;
using AuditCRM.Application.Interfaces;
using AuditCRM.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuditCRM.Infrastructure.Services;

public sealed class WorkQueueService : IWorkQueueService
{
    private readonly AuditDbContext _dbContext;

    public WorkQueueService(
        AuditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<WorkQueueItemDto>> GetAsync(
        Guid? shoppingGroupId,
        Guid? shoppingId,
        Guid? responsibleUserId,
        bool? overdueOnly,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var query = _dbContext.StoreProcesses
            .AsNoTracking()
            .Where(x => !x.IsClosed)
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

        if (overdueOnly == true)
        {
            query = query.Where(
                x =>
                    x.NextActionAt.HasValue &&
                    x.NextActionAt.Value < now);
        }

        return await query
            .OrderByDescending(x => x.Priority)
            .ThenBy(x => x.NextActionAt)
            .ThenBy(x => x.Store!.TradeName)
            .Select(x => new WorkQueueItemDto(
                x.Id,
                x.StoreId,
                x.Store != null
                    ? x.Store.TradeName
                    : string.Empty,

                x.Store != null
                    ? x.Store.ShoppingId
                    : Guid.Empty,

                x.Store != null &&
                x.Store.Shopping != null
                    ? x.Store.Shopping.Name
                    : string.Empty,

                x.Store != null &&
                x.Store.Shopping != null
                    ? x.Store.Shopping.ShoppingGroupId
                    : null,

                x.Store != null &&
                x.Store.Shopping != null &&
                x.Store.Shopping.ShoppingGroup != null
                    ? x.Store.Shopping.ShoppingGroup.Name
                    : null,

                (int)x.Status,
                (int)x.Priority,

                x.ResponsibleUserId,

                x.ResponsibleUser != null
                    ? x.ResponsibleUser.Name
                    : null,

                x.NextAction,
                x.NextActionAt,
                x.StartedAt,
                x.IsClosed))
            .ToListAsync(cancellationToken);
    }
}