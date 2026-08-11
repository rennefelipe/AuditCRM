using AuditCRM.Application.Features.StoreProcesses.DTOs;
using AuditCRM.Application.Interfaces;
using AuditCRM.Domain.Entities;
using AuditCRM.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuditCRM.Infrastructure.Services;

public sealed class StoreProcessService : IStoreProcessService
{
    private readonly AuditDbContext _dbContext;

    public StoreProcessService(
        AuditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<StoreProcessDto>> GetAllAsync(
        Guid? storeId = null,
        Guid? shoppingId = null,
        Guid? responsibleUserId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.StoreProcesses
            .AsNoTracking()
            .Include(process => process.Store)
            .Include(process => process.InstallationType)
            .Include(process => process.ResponsibleUser)
            .AsQueryable();

        if (storeId.HasValue)
        {
            query = query.Where(
                process =>
                    process.StoreId == storeId.Value);
        }

        if (shoppingId.HasValue)
        {
            query = query.Where(
                process =>
                    process.Store != null &&
                    process.Store.ShoppingId ==
                    shoppingId.Value);
        }

        if (responsibleUserId.HasValue)
        {
            query = query.Where(
                process =>
                    process.ResponsibleUserId ==
                    responsibleUserId.Value);
        }

        return await query
            .OrderBy(process => process.IsClosed)
            .ThenBy(process => process.NextActionAt)
            .ThenByDescending(process => process.Priority)
            .Select(process => new StoreProcessDto(
                process.Id,
                process.StoreId,
                process.Store != null
                    ? process.Store.TradeName
                    : string.Empty,
                process.InstallationTypeId,
                process.InstallationType != null
                    ? process.InstallationType.Name
                    : null,
                process.ResponsibleUserId,
                process.ResponsibleUser != null
                    ? process.ResponsibleUser.Name
                    : null,
                process.Status,
                process.Priority,
                process.Frequency,
                process.NextAction,
                process.NextActionAt,
                process.StartedAt,
                process.CompletedAt,
                process.Notes,
                process.IsClosed,
                process.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<StoreProcessDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.StoreProcesses
            .AsNoTracking()
            .Include(process => process.Store)
            .Include(process => process.InstallationType)
            .Include(process => process.ResponsibleUser)
            .Where(process => process.Id == id)
            .Select(process => new StoreProcessDto(
                process.Id,
                process.StoreId,
                process.Store != null
                    ? process.Store.TradeName
                    : string.Empty,
                process.InstallationTypeId,
                process.InstallationType != null
                    ? process.InstallationType.Name
                    : null,
                process.ResponsibleUserId,
                process.ResponsibleUser != null
                    ? process.ResponsibleUser.Name
                    : null,
                process.Status,
                process.Priority,
                process.Frequency,
                process.NextAction,
                process.NextActionAt,
                process.StartedAt,
                process.CompletedAt,
                process.Notes,
                process.IsClosed,
                process.CreatedAt))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<StoreProcessDto> CreateAsync(
        CreateStoreProcessRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        await ValidateStoreAsync(
            request.StoreId,
            cancellationToken);

        await ValidateInstallationTypeAsync(
            request.InstallationTypeId,
            cancellationToken);

        await ValidateResponsibleUserAsync(
            request.ResponsibleUserId,
            cancellationToken);

        var process = new StoreProcess(
            request.StoreId,
            request.InstallationTypeId,
            request.ResponsibleUserId);

        process.Update(
            request.InstallationTypeId,
            request.ResponsibleUserId,
            request.Status,
            request.Priority,
            request.Frequency,
            request.NextAction,
            request.NextActionAt,
            request.Notes);

        process.MarkCreatedBy(currentUserId);

        _dbContext.StoreProcesses.Add(process);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return await GetRequiredDtoAsync(
            process.Id,
            cancellationToken);
    }

    public async Task<StoreProcessDto> UpdateAsync(
        Guid id,
        UpdateStoreProcessRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        await ValidateInstallationTypeAsync(
            request.InstallationTypeId,
            cancellationToken);

        await ValidateResponsibleUserAsync(
            request.ResponsibleUserId,
            cancellationToken);

        var process = await FindRequiredAsync(
            id,
            cancellationToken);

        process.Update(
            request.InstallationTypeId,
            request.ResponsibleUserId,
            request.Status,
            request.Priority,
            request.Frequency,
            request.NextAction,
            request.NextActionAt,
            request.Notes);

        process.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return await GetRequiredDtoAsync(
            process.Id,
            cancellationToken);
    }

    public async Task CloseAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var process = await FindRequiredAsync(
            id,
            cancellationToken);

        process.Close();
        process.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task ReopenAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var process = await FindRequiredAsync(
            id,
            cancellationToken);

        process.Reopen();
        process.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var process = await FindRequiredAsync(
            id,
            cancellationToken);

        process.Delete(currentUserId);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    private async Task ValidateStoreAsync(
        Guid storeId,
        CancellationToken cancellationToken)
    {
        var exists = await _dbContext.Stores
            .AnyAsync(
                store =>
                    store.Id == storeId &&
                    store.IsActive,
                cancellationToken);

        if (!exists)
        {
            throw new InvalidOperationException(
                "A loja informada não existe ou está inativa.");
        }
    }

    private async Task ValidateInstallationTypeAsync(
        Guid? installationTypeId,
        CancellationToken cancellationToken)
    {
        if (!installationTypeId.HasValue)
        {
            return;
        }

        var exists = await _dbContext.InstallationTypes
            .AnyAsync(
                type =>
                    type.Id == installationTypeId.Value &&
                    type.IsActive,
                cancellationToken);

        if (!exists)
        {
            throw new InvalidOperationException(
                "O tipo de instalação informado não existe ou está inativo.");
        }
    }

    private async Task ValidateResponsibleUserAsync(
        Guid? responsibleUserId,
        CancellationToken cancellationToken)
    {
        if (!responsibleUserId.HasValue)
        {
            return;
        }

        var exists = await _dbContext.Users
            .AnyAsync(
                user =>
                    user.Id == responsibleUserId.Value &&
                    user.IsActive,
                cancellationToken);

        if (!exists)
        {
            throw new InvalidOperationException(
                "O usuário responsável informado não existe ou está inativo.");
        }
    }

    private async Task<StoreProcess> FindRequiredAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var process = await _dbContext.StoreProcesses
            .SingleOrDefaultAsync(
                item => item.Id == id,
                cancellationToken);

        if (process is null)
        {
            throw new KeyNotFoundException(
                "Processo não encontrado.");
        }

        return process;
    }

    private async Task<StoreProcessDto> GetRequiredDtoAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var process = await GetByIdAsync(
            id,
            cancellationToken);

        if (process is null)
        {
            throw new KeyNotFoundException(
                "Processo não encontrado.");
        }

        return process;
    }
}