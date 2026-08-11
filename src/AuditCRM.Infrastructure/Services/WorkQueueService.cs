using AuditCRM.Application.Features.WorkQueue.DTOs;
using AuditCRM.Application.Interfaces;
using AuditCRM.Domain.Enums;
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
        Guid? storeId,
        string? search,
        Guid? installationTypeId,
        int? status,
        Guid? responsibleUserId,
        int? frequency,
        int? priority,
        bool? overdueOnly,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var query = _dbContext.StoreProcesses
            .AsNoTracking()
            .Where(process => !process.IsClosed)
            .AsQueryable();

        //
        // GRUPO DE SHOPPING
        //
        if (shoppingGroupId.HasValue)
        {
            query = query.Where(
                process =>
                    process.Store != null &&
                    process.Store.Shopping != null &&
                    process.Store.Shopping.ShoppingGroupId ==
                    shoppingGroupId.Value);
        }

        //
        // SHOPPING
        //
        if (shoppingId.HasValue)
        {
            query = query.Where(
                process =>
                    process.Store != null &&
                    process.Store.ShoppingId ==
                    shoppingId.Value);
        }

        //
        // LOJA
        //
        if (storeId.HasValue)
        {
            query = query.Where(
                process =>
                    process.StoreId ==
                    storeId.Value);
        }

        //
        // BUSCA LIVRE
        //
        // Pesquisa por:
        // - nome da loja
        // - LUC
        // - CNPJ/documento
        // - nome do contato
        // - e-mail do contato
        // - telefone do contato
        //
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.Trim();

            query = query.Where(
                process =>
                    (
                        process.Store != null &&
                        (
                            process.Store.TradeName.Contains(searchTerm) ||
                            (
                                process.Store.Luc != null &&
                                process.Store.Luc.Contains(searchTerm)
                            ) ||
                            (
                                process.Store.Document != null &&
                                process.Store.Document.Contains(searchTerm)
                            )
                        )
                    ) ||
                    _dbContext.StoreContacts.Any(
                        contact =>
                            contact.StoreId == process.StoreId &&
                            contact.IsActive &&
                            (
                                contact.Name.Contains(searchTerm) ||
                                (
                                    contact.Email1 != null &&
                                    contact.Email1.Contains(searchTerm)
                                ) ||
                                (
                                    contact.Email2 != null &&
                                    contact.Email2.Contains(searchTerm)
                                ) ||
                                (
                                    contact.Email3 != null &&
                                    contact.Email3.Contains(searchTerm)
                                ) ||
                                (
                                    contact.Phone1 != null &&
                                    contact.Phone1.Contains(searchTerm)
                                ) ||
                                (
                                    contact.Phone2 != null &&
                                    contact.Phone2.Contains(searchTerm)
                                ) ||
                                (
                                    contact.Phone3 != null &&
                                    contact.Phone3.Contains(searchTerm)
                                )
                            )
                    ));
        }

        //
        // TIPO DE INSTALAÇÃO
        //
        if (installationTypeId.HasValue)
        {
            query = query.Where(
                process =>
                    process.InstallationTypeId ==
                    installationTypeId.Value);
        }

        //
        // STATUS
        //
        if (status.HasValue &&
            Enum.IsDefined(
                typeof(StoreProcessStatus),
                status.Value))
        {
            var statusValue =
                (StoreProcessStatus)status.Value;

            query = query.Where(
                process =>
                    process.Status == statusValue);
        }

        //
        // TÉCNICO / RESPONSÁVEL
        //
        if (responsibleUserId.HasValue)
        {
            query = query.Where(
                process =>
                    process.ResponsibleUserId ==
                    responsibleUserId.Value);
        }

        //
        // PERIODICIDADE
        //
        if (frequency.HasValue &&
            Enum.IsDefined(
                typeof(ProcessFrequency),
                frequency.Value))
        {
            var frequencyValue =
                (ProcessFrequency)frequency.Value;

            query = query.Where(
                process =>
                    process.Frequency ==
                    frequencyValue);
        }

        //
        // PRIORIDADE
        //
        if (priority.HasValue &&
            Enum.IsDefined(
                typeof(ProcessPriority),
                priority.Value))
        {
            var priorityValue =
                (ProcessPriority)priority.Value;

            query = query.Where(
                process =>
                    process.Priority ==
                    priorityValue);
        }

        //
        // SOMENTE ATRASADAS
        //
        if (overdueOnly == true)
        {
            query = query.Where(
                process =>
                    process.NextActionAt.HasValue &&
                    process.NextActionAt.Value < now);
        }

        return await query
            .OrderByDescending(
                process => process.Priority)
            .ThenBy(
                process => process.NextActionAt)
            .ThenBy(
                process =>
                    process.Store != null
                        ? process.Store.TradeName
                        : string.Empty)
            .Select(
                process =>
                    new WorkQueueItemDto(
                        process.Id,

                        process.StoreId,

                        process.Store != null
                            ? process.Store.TradeName
                            : string.Empty,

                        process.Store != null
                            ? process.Store.Luc
                            : null,

                        process.Store != null
                            ? process.Store.Document
                            : null,

                        process.Store != null
                            ? process.Store.ShoppingId
                            : Guid.Empty,

                        process.Store != null &&
                        process.Store.Shopping != null
                            ? process.Store.Shopping.Name
                            : string.Empty,

                        process.Store != null &&
                        process.Store.Shopping != null
                            ? process.Store.Shopping.ShoppingGroupId
                            : null,

                        process.Store != null &&
                        process.Store.Shopping != null &&
                        process.Store.Shopping.ShoppingGroup != null
                            ? process.Store.Shopping.ShoppingGroup.Name
                            : null,

                        process.InstallationTypeId,

                        process.InstallationType != null
                            ? process.InstallationType.Name
                            : null,

                        (int)process.Status,

                        (int)process.Priority,

                        (int)process.Frequency,

                        process.ResponsibleUserId,

                        process.ResponsibleUser != null
                            ? process.ResponsibleUser.Name
                            : null,

                        process.NextAction,

                        process.NextActionAt,

                        process.StartedAt,

                        process.CompletedAt,

                        process.IsClosed))
            .ToListAsync(cancellationToken);
    }
}