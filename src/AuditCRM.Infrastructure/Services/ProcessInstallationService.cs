using AuditCRM.Application.Features.ProcessInstallations.DTOs;
using AuditCRM.Application.Interfaces;
using AuditCRM.Domain.Entities;
using AuditCRM.Domain.Enums;
using AuditCRM.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuditCRM.Infrastructure.Services;

public sealed class ProcessInstallationService
    : IProcessInstallationService
{
    private readonly AuditDbContext _dbContext;

    public ProcessInstallationService(
        AuditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ProcessInstallationDto>> GetAllAsync(
        Guid? storeProcessId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ProcessInstallations
            .AsNoTracking()
            .Include(x => x.ResponsibleUser)
            .Include(x => x.Erp)
            .AsQueryable();

        if (storeProcessId.HasValue)
        {
            query = query.Where(
                x => x.StoreProcessId == storeProcessId.Value);
        }

        return await query
            .OrderByDescending(x => x.InstalledAt)
            .Select(x => new ProcessInstallationDto(
                x.Id,
                x.StoreProcessId,
                x.ResponsibleUserId,
                x.ResponsibleUser != null
                    ? x.ResponsibleUser.Name
                    : null,
                x.ErpId,
                x.Erp != null
                    ? x.Erp.Name
                    : null,
                x.InstalledAt,
                x.Successful,
                x.XmlLocationType,
                x.NumberOfRegisters,
                x.Result,
                x.Notes,
                x.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<ProcessInstallationDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProcessInstallations
            .AsNoTracking()
            .Include(x => x.ResponsibleUser)
            .Include(x => x.Erp)
            .Where(x => x.Id == id)
            .Select(x => new ProcessInstallationDto(
                x.Id,
                x.StoreProcessId,
                x.ResponsibleUserId,
                x.ResponsibleUser != null
                    ? x.ResponsibleUser.Name
                    : null,
                x.ErpId,
                x.Erp != null
                    ? x.Erp.Name
                    : null,
                x.InstalledAt,
                x.Successful,
                x.XmlLocationType,
                x.NumberOfRegisters,
                x.Result,
                x.Notes,
                x.CreatedAt))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<ProcessInstallationDto> CreateAsync(
        CreateProcessInstallationRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        await ValidateProcessAsync(
            request.StoreProcessId,
            cancellationToken);

        await ValidateResponsibleUserAsync(
            request.ResponsibleUserId,
            cancellationToken);

        await ValidateErpAsync(
            request.ErpId,
            cancellationToken);

        ValidateRegisters(request.NumberOfRegisters);

        var installation = new ProcessInstallation(
            request.StoreProcessId,
            request.InstalledAt,
            request.Successful,
            request.ResponsibleUserId,
            request.ErpId,
            request.XmlLocationType);

        installation.Update(
            request.InstalledAt,
            request.Successful,
            request.ResponsibleUserId,
            request.ErpId,
            request.XmlLocationType,
            request.NumberOfRegisters,
            request.Result,
            request.Notes);

        installation.MarkCreatedBy(currentUserId);

        _dbContext.ProcessInstallations.Add(installation);

        var process = await _dbContext.StoreProcesses
            .SingleAsync(
                x => x.Id == request.StoreProcessId,
                cancellationToken);

        if (request.Successful)
        {
            process.ChangeStatus(
                StoreProcessStatus.Installed);

            process.SetNextAction(
                null,
                null);
        }

        process.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetRequiredDtoAsync(
            installation.Id,
            cancellationToken);
    }

    public async Task<ProcessInstallationDto> UpdateAsync(
        Guid id,
        UpdateProcessInstallationRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var installation = await FindRequiredAsync(
            id,
            cancellationToken);

        await ValidateResponsibleUserAsync(
            request.ResponsibleUserId,
            cancellationToken);

        await ValidateErpAsync(
            request.ErpId,
            cancellationToken);

        ValidateRegisters(request.NumberOfRegisters);

        installation.Update(
            request.InstalledAt,
            request.Successful,
            request.ResponsibleUserId,
            request.ErpId,
            request.XmlLocationType,
            request.NumberOfRegisters,
            request.Result,
            request.Notes);

        installation.MarkUpdatedBy(currentUserId);

        var process = await _dbContext.StoreProcesses
            .SingleAsync(
                x => x.Id == installation.StoreProcessId,
                cancellationToken);

        if (request.Successful)
        {
            process.ChangeStatus(
                StoreProcessStatus.Installed);

            process.SetNextAction(
                null,
                null);
        }

        process.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetRequiredDtoAsync(
            installation.Id,
            cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var installation = await FindRequiredAsync(
            id,
            cancellationToken);

        installation.Delete(currentUserId);

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

    private async Task ValidateErpAsync(
        Guid? erpId,
        CancellationToken cancellationToken)
    {
        if (!erpId.HasValue)
            return;

        var exists = await _dbContext.Erps
            .AnyAsync(
                x =>
                    x.Id == erpId.Value &&
                    x.IsActive,
                cancellationToken);

        if (!exists)
        {
            throw new InvalidOperationException(
                "O ERP informado não existe ou está inativo.");
        }
    }

    private async Task<ProcessInstallation> FindRequiredAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var installation = await _dbContext.ProcessInstallations
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (installation is null)
        {
            throw new KeyNotFoundException(
                "Instalação não encontrada.");
        }

        return installation;
    }

    private async Task<ProcessInstallationDto> GetRequiredDtoAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var installation = await GetByIdAsync(
            id,
            cancellationToken);

        if (installation is null)
        {
            throw new KeyNotFoundException(
                "Instalação não encontrada.");
        }

        return installation;
    }

    private static void ValidateRegisters(
        int? numberOfRegisters)
    {
        if (numberOfRegisters < 0)
        {
            throw new ArgumentException(
                "A quantidade de caixas não pode ser negativa.");
        }
    }
}