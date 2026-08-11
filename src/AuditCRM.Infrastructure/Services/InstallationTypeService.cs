using AuditCRM.Application.Features.InstallationTypes.DTOs;
using AuditCRM.Application.Interfaces;
using AuditCRM.Domain.Entities;
using AuditCRM.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuditCRM.Infrastructure.Services;

public sealed class InstallationTypeService : IInstallationTypeService
{
    private readonly AuditDbContext _dbContext;

    public InstallationTypeService(AuditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<InstallationTypeDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.InstallationTypes
            .AsNoTracking()
            .OrderBy(type => type.Name)
            .Select(type => new InstallationTypeDto(
                type.Id,
                type.Name,
                type.Description,
                type.IsActive,
                type.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<InstallationTypeDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.InstallationTypes
            .AsNoTracking()
            .Where(type => type.Id == id)
            .Select(type => new InstallationTypeDto(
                type.Id,
                type.Name,
                type.Description,
                type.IsActive,
                type.CreatedAt))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<InstallationTypeDto> CreateAsync(
        CreateInstallationTypeRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        ValidateName(request.Name);

        var normalizedName = request.Name.Trim();

        var exists = await _dbContext.InstallationTypes
            .IgnoreQueryFilters()
            .AnyAsync(
                type => type.Name == normalizedName,
                cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException(
                "Já existe um tipo de instalação cadastrado com este nome.");
        }

        var type = new InstallationType(
            normalizedName,
            request.Description);

        type.MarkCreatedBy(currentUserId);

        _dbContext.InstallationTypes.Add(type);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(type);
    }

    public async Task<InstallationTypeDto> UpdateAsync(
        Guid id,
        UpdateInstallationTypeRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        ValidateName(request.Name);

        var type = await FindRequiredAsync(id, cancellationToken);

        var normalizedName = request.Name.Trim();

        var exists = await _dbContext.InstallationTypes
            .IgnoreQueryFilters()
            .AnyAsync(
                item =>
                    item.Name == normalizedName &&
                    item.Id != id,
                cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException(
                "Já existe outro tipo de instalação cadastrado com este nome.");
        }

        type.Update(
            normalizedName,
            request.Description);

        type.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(type);
    }

    public async Task ActivateAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var type = await FindRequiredAsync(id, cancellationToken);

        type.Activate();
        type.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var type = await FindRequiredAsync(id, cancellationToken);

        type.Deactivate();
        type.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var type = await FindRequiredAsync(id, cancellationToken);

        type.Delete(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<InstallationType> FindRequiredAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var type = await _dbContext.InstallationTypes
            .SingleOrDefaultAsync(
                item => item.Id == id,
                cancellationToken);

        if (type is null)
        {
            throw new KeyNotFoundException(
                "Tipo de instalação não encontrado.");
        }

        return type;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "O nome do tipo de instalação é obrigatório.");
        }
    }

    private static InstallationTypeDto Map(
        InstallationType type)
    {
        return new InstallationTypeDto(
            type.Id,
            type.Name,
            type.Description,
            type.IsActive,
            type.CreatedAt);
    }
}