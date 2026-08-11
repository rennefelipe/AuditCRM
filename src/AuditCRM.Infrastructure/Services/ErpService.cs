using AuditCRM.Application.Features.Erps.DTOs;
using AuditCRM.Application.Interfaces;
using AuditCRM.Domain.Entities;
using AuditCRM.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuditCRM.Infrastructure.Services;

public sealed class ErpService : IErpService
{
    private readonly AuditDbContext _dbContext;

    public ErpService(AuditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ErpDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Erps
            .AsNoTracking()
            .OrderBy(erp => erp.Name)
            .Select(erp => new ErpDto(
                erp.Id,
                erp.Name,
                erp.Manufacturer,
                erp.Notes,
                erp.IsActive,
                erp.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<ErpDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Erps
            .AsNoTracking()
            .Where(erp => erp.Id == id)
            .Select(erp => new ErpDto(
                erp.Id,
                erp.Name,
                erp.Manufacturer,
                erp.Notes,
                erp.IsActive,
                erp.CreatedAt))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<ErpDto> CreateAsync(
        CreateErpRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        ValidateName(request.Name);

        var normalizedName = request.Name.Trim();

        var exists = await _dbContext.Erps
            .IgnoreQueryFilters()
            .AnyAsync(
                erp => erp.Name == normalizedName,
                cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException(
                "Já existe um ERP cadastrado com este nome.");
        }

        var erp = new Erp(
            normalizedName,
            request.Manufacturer,
            request.Notes);

        erp.MarkCreatedBy(currentUserId);

        _dbContext.Erps.Add(erp);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(erp);
    }

    public async Task<ErpDto> UpdateAsync(
        Guid id,
        UpdateErpRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        ValidateName(request.Name);

        var erp = await FindRequiredAsync(
            id,
            cancellationToken);

        var normalizedName = request.Name.Trim();

        var exists = await _dbContext.Erps
            .IgnoreQueryFilters()
            .AnyAsync(
                item =>
                    item.Name == normalizedName &&
                    item.Id != id,
                cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException(
                "Já existe outro ERP cadastrado com este nome.");
        }

        erp.Update(
            normalizedName,
            request.Manufacturer,
            request.Notes);

        erp.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(erp);
    }

    public async Task ActivateAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var erp = await FindRequiredAsync(id, cancellationToken);

        erp.Activate();
        erp.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var erp = await FindRequiredAsync(id, cancellationToken);

        erp.Deactivate();
        erp.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var erp = await FindRequiredAsync(id, cancellationToken);

        var hasStores = await _dbContext.Stores
            .AnyAsync(
                store => store.ErpId == id,
                cancellationToken);

        if (hasStores)
        {
            throw new InvalidOperationException(
                "Não é possível excluir o ERP porque existem lojas vinculadas.");
        }

        erp.Delete(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<Erp> FindRequiredAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var erp = await _dbContext.Erps
            .SingleOrDefaultAsync(
                item => item.Id == id,
                cancellationToken);

        if (erp is null)
        {
            throw new KeyNotFoundException(
                "ERP não encontrado.");
        }

        return erp;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "O nome do ERP é obrigatório.");
        }
    }

    private static ErpDto Map(Erp erp)
    {
        return new ErpDto(
            erp.Id,
            erp.Name,
            erp.Manufacturer,
            erp.Notes,
            erp.IsActive,
            erp.CreatedAt);
    }
}