using AuditCRM.Application.Features.ProcessInteractions.DTOs;
using AuditCRM.Application.Interfaces;
using AuditCRM.Domain.Entities;
using AuditCRM.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuditCRM.Infrastructure.Services;

public sealed class ProcessInteractionService
    : IProcessInteractionService
{
    private readonly AuditDbContext _dbContext;

    public ProcessInteractionService(
        AuditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ProcessInteractionDto>> GetAllAsync(
        Guid? storeProcessId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ProcessInteractions
            .AsNoTracking()
            .Include(x => x.ResponsibleUser)
            .Include(x => x.StoreContact)
            .AsQueryable();

        if (storeProcessId.HasValue)
        {
            query = query.Where(
                x => x.StoreProcessId == storeProcessId.Value);
        }

        return await query
            .OrderByDescending(x => x.OccurredAt)
            .Select(x => new ProcessInteractionDto(
                x.Id,
                x.StoreProcessId,
                x.ResponsibleUserId,
                x.ResponsibleUser != null
                    ? x.ResponsibleUser.Name
                    : null,
                x.StoreContactId,
                x.StoreContact != null
                    ? x.StoreContact.Name
                    : null,
                x.Channel,
                x.OccurredAt,
                x.Description,
                x.Result,
                x.NextAction,
                x.NextActionAt,
                x.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<ProcessInteractionDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProcessInteractions
            .AsNoTracking()
            .Include(x => x.ResponsibleUser)
            .Include(x => x.StoreContact)
            .Where(x => x.Id == id)
            .Select(x => new ProcessInteractionDto(
                x.Id,
                x.StoreProcessId,
                x.ResponsibleUserId,
                x.ResponsibleUser != null
                    ? x.ResponsibleUser.Name
                    : null,
                x.StoreContactId,
                x.StoreContact != null
                    ? x.StoreContact.Name
                    : null,
                x.Channel,
                x.OccurredAt,
                x.Description,
                x.Result,
                x.NextAction,
                x.NextActionAt,
                x.CreatedAt))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<ProcessInteractionDto> CreateAsync(
        CreateProcessInteractionRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        ValidateDescription(request.Description);

        await ValidateProcessAsync(
            request.StoreProcessId,
            cancellationToken);

        await ValidateResponsibleUserAsync(
            request.ResponsibleUserId,
            cancellationToken);

        await ValidateStoreContactAsync(
            request.StoreProcessId,
            request.StoreContactId,
            cancellationToken);

        var interaction = new ProcessInteraction(
            request.StoreProcessId,
            request.Channel,
            request.OccurredAt,
            request.Description,
            request.ResponsibleUserId,
            request.StoreContactId);

        interaction.Update(
            request.Channel,
            request.OccurredAt,
            request.ResponsibleUserId,
            request.StoreContactId,
            request.Description,
            request.Result,
            request.NextAction,
            request.NextActionAt);

        interaction.MarkCreatedBy(currentUserId);

        _dbContext.ProcessInteractions.Add(interaction);

        await UpdateProcessNextActionAsync(
            request.StoreProcessId,
            request.NextAction,
            request.NextActionAt,
            currentUserId,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetRequiredDtoAsync(
            interaction.Id,
            cancellationToken);
    }

    public async Task<ProcessInteractionDto> UpdateAsync(
        Guid id,
        UpdateProcessInteractionRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        ValidateDescription(request.Description);

        var interaction = await FindRequiredAsync(
            id,
            cancellationToken);

        await ValidateResponsibleUserAsync(
            request.ResponsibleUserId,
            cancellationToken);

        await ValidateStoreContactAsync(
            interaction.StoreProcessId,
            request.StoreContactId,
            cancellationToken);

        interaction.Update(
            request.Channel,
            request.OccurredAt,
            request.ResponsibleUserId,
            request.StoreContactId,
            request.Description,
            request.Result,
            request.NextAction,
            request.NextActionAt);

        interaction.MarkUpdatedBy(currentUserId);

        await UpdateProcessNextActionAsync(
            interaction.StoreProcessId,
            request.NextAction,
            request.NextActionAt,
            currentUserId,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetRequiredDtoAsync(
            interaction.Id,
            cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var interaction = await FindRequiredAsync(
            id,
            cancellationToken);

        interaction.Delete(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateProcessAsync(
        Guid storeProcessId,
        CancellationToken cancellationToken)
    {
        var exists = await _dbContext.StoreProcesses
            .AnyAsync(
                x => x.Id == storeProcessId,
                cancellationToken);

        if (!exists)
        {
            throw new InvalidOperationException(
                "O processo informado não existe.");
        }
    }

    private async Task ValidateResponsibleUserAsync(
        Guid? responsibleUserId,
        CancellationToken cancellationToken)
    {
        if (!responsibleUserId.HasValue)
            return;

        var exists = await _dbContext.Users
            .AnyAsync(
                x =>
                    x.Id == responsibleUserId.Value &&
                    x.IsActive,
                cancellationToken);

        if (!exists)
        {
            throw new InvalidOperationException(
                "O usuário responsável informado não existe ou está inativo.");
        }
    }

    private async Task ValidateStoreContactAsync(
        Guid storeProcessId,
        Guid? storeContactId,
        CancellationToken cancellationToken)
    {
        if (!storeContactId.HasValue)
            return;

        var storeId = await _dbContext.StoreProcesses
            .Where(x => x.Id == storeProcessId)
            .Select(x => x.StoreId)
            .SingleOrDefaultAsync(cancellationToken);

        var exists = await _dbContext.StoreContacts
            .AnyAsync(
                x =>
                    x.Id == storeContactId.Value &&
                    x.StoreId == storeId &&
                    x.IsActive,
                cancellationToken);

        if (!exists)
        {
            throw new InvalidOperationException(
                "O contato informado não pertence à loja deste processo ou está inativo.");
        }
    }

    private async Task UpdateProcessNextActionAsync(
        Guid storeProcessId,
        string? nextAction,
        DateTime? nextActionAt,
        Guid? currentUserId,
        CancellationToken cancellationToken)
    {
        var process = await _dbContext.StoreProcesses
            .SingleAsync(
                x => x.Id == storeProcessId,
                cancellationToken);

        process.SetNextAction(
            nextAction,
            nextActionAt);

        process.MarkUpdatedBy(currentUserId);
    }

    private async Task<ProcessInteraction> FindRequiredAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var interaction = await _dbContext.ProcessInteractions
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (interaction is null)
        {
            throw new KeyNotFoundException(
                "Tratativa não encontrada.");
        }

        return interaction;
    }

    private async Task<ProcessInteractionDto> GetRequiredDtoAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var interaction = await GetByIdAsync(
            id,
            cancellationToken);

        if (interaction is null)
        {
            throw new KeyNotFoundException(
                "Tratativa não encontrada.");
        }

        return interaction;
    }

    private static void ValidateDescription(
        string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException(
                "A descrição da tratativa é obrigatória.");
        }
    }
}