using AuditCRM.Application.Features.Timelines.DTOs;
using AuditCRM.Application.Interfaces;
using AuditCRM.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuditCRM.Infrastructure.Services;

public sealed class ProcessTimelineService
    : IProcessTimelineService
{
    private readonly AuditDbContext _dbContext;

    public ProcessTimelineService(
        AuditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<TimelineItemDto>> GetAsync(
        Guid storeProcessId,
        CancellationToken cancellationToken = default)
    {
        var processExists = await _dbContext.StoreProcesses
            .AnyAsync(
                x => x.Id == storeProcessId,
                cancellationToken);

        if (!processExists)
        {
            throw new KeyNotFoundException(
                "Processo não encontrado.");
        }

        var items = new List<TimelineItemDto>();

        var interactions = await _dbContext.ProcessInteractions
            .AsNoTracking()
            .Where(x => x.StoreProcessId == storeProcessId)
            .Select(x => new
            {
                x.Id,
                x.Channel,
                x.Description,
                x.Result,
                x.OccurredAt,
                x.ResponsibleUserId,
                UserName = x.ResponsibleUser != null
                    ? x.ResponsibleUser.Name
                    : null
            })
            .ToListAsync(cancellationToken);

        foreach (var item in interactions)
        {
            var description = item.Description;

            if (!string.IsNullOrWhiteSpace(item.Result))
            {
                description += $" Resultado: {item.Result}";
            }

            items.Add(
                new TimelineItemDto(
                    item.Id,
                    "interaction",
                    $"Tratativa - {item.Channel}",
                    description,
                    item.OccurredAt,
                    item.ResponsibleUserId,
                    item.UserName));
        }

        var appointments = await _dbContext.ProcessAppointments
            .AsNoTracking()
            .Where(x => x.StoreProcessId == storeProcessId)
            .Select(x => new
            {
                x.Id,
                x.Status,
                x.ScheduledAt,
                x.Notes,
                x.ResponsibleUserId,
                UserName = x.ResponsibleUser != null
                    ? x.ResponsibleUser.Name
                    : null
            })
            .ToListAsync(cancellationToken);

        foreach (var item in appointments)
        {
            items.Add(
                new TimelineItemDto(
                    item.Id,
                    "appointment",
                    $"Agendamento - {item.Status}",
                    item.Notes,
                    item.ScheduledAt,
                    item.ResponsibleUserId,
                    item.UserName));
        }

        var installations = await _dbContext.ProcessInstallations
            .AsNoTracking()
            .Where(x => x.StoreProcessId == storeProcessId)
            .Select(x => new
            {
                x.Id,
                x.Successful,
                x.Result,
                x.Notes,
                x.InstalledAt,
                x.ResponsibleUserId,
                UserName = x.ResponsibleUser != null
                    ? x.ResponsibleUser.Name
                    : null
            })
            .ToListAsync(cancellationToken);

        foreach (var item in installations)
        {
            string? description = item.Result;

            if (!string.IsNullOrWhiteSpace(item.Notes))
            {
                description = string.IsNullOrWhiteSpace(description)
                    ? item.Notes
                    : $"{description} {item.Notes}";
            }

            items.Add(
                new TimelineItemDto(
                    item.Id,
                    "installation",
                    item.Successful
                        ? "Instalação concluída"
                        : "Tentativa de instalação",
                    description,
                    item.InstalledAt,
                    item.ResponsibleUserId,
                    item.UserName));
        }

        var notes = await _dbContext.ProcessNotes
            .AsNoTracking()
            .Where(x => x.StoreProcessId == storeProcessId)
            .Select(x => new
            {
                x.Id,
                x.Text,
                x.CreatedAt,
                x.AuthorUserId,
                UserName = x.AuthorUser != null
                    ? x.AuthorUser.Name
                    : null
            })
            .ToListAsync(cancellationToken);

        foreach (var item in notes)
        {
            items.Add(
                new TimelineItemDto(
                    item.Id,
                    "note",
                    "Anotação adicionada",
                    item.Text,
                    item.CreatedAt,
                    item.AuthorUserId,
                    item.UserName));
        }

        var documents = await _dbContext.ProcessDocuments
            .AsNoTracking()
            .Where(x => x.StoreProcessId == storeProcessId)
            .Select(x => new
            {
                x.Id,
                x.OriginalFileName,
                x.CreatedAt,
                x.UploadedByUserId,
                UserName = x.UploadedByUser != null
                    ? x.UploadedByUser.Name
                    : null
            })
            .ToListAsync(cancellationToken);

        foreach (var item in documents)
        {
            items.Add(
                new TimelineItemDto(
                    item.Id,
                    "document",
                    "Documento anexado",
                    item.OriginalFileName,
                    item.CreatedAt,
                    item.UploadedByUserId,
                    item.UserName));
        }

        return items
            .OrderByDescending(x => x.OccurredAt)
            .ToList();
    }
}