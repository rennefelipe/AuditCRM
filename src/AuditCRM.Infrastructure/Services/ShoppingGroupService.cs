using AuditCRM.Application.Features.ShoppingGroups.DTOs;
using AuditCRM.Application.Interfaces;
using AuditCRM.Domain.Entities;
using AuditCRM.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuditCRM.Infrastructure.Services;

public sealed class ShoppingGroupService : IShoppingGroupService
{
    private readonly AuditDbContext _dbContext;

    public ShoppingGroupService(AuditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ShoppingGroupDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ShoppingGroups
            .AsNoTracking()
            .OrderBy(group => group.Name)
            .Select(group => new ShoppingGroupDto(
                group.Id,
                group.Name,
                group.Description,
                group.IsActive,
                group.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<ShoppingGroupDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ShoppingGroups
            .AsNoTracking()
            .Where(group => group.Id == id)
            .Select(group => new ShoppingGroupDto(
                group.Id,
                group.Name,
                group.Description,
                group.IsActive,
                group.CreatedAt))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<ShoppingGroupDto> CreateAsync(
        CreateShoppingGroupRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        ValidateName(request.Name);

        var normalizedName = request.Name.Trim();

        var exists = await _dbContext.ShoppingGroups
            .IgnoreQueryFilters()
            .AnyAsync(
                group => group.Name == normalizedName,
                cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException(
                "Já existe uma rede de shopping cadastrada com este nome.");
        }

        var group = new ShoppingGroup(
            normalizedName,
            request.Description);

        group.MarkCreatedBy(currentUserId);

        _dbContext.ShoppingGroups.Add(group);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(group);
    }

    public async Task<ShoppingGroupDto> UpdateAsync(
        Guid id,
        UpdateShoppingGroupRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        ValidateName(request.Name);

        var group = await FindRequiredAsync(
            id,
            cancellationToken);

        var normalizedName = request.Name.Trim();

        var exists = await _dbContext.ShoppingGroups
            .IgnoreQueryFilters()
            .AnyAsync(
                item =>
                    item.Name == normalizedName &&
                    item.Id != id,
                cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException(
                "Já existe outra rede de shopping cadastrada com este nome.");
        }

        group.Update(
            normalizedName,
            request.Description);

        group.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(group);
    }

    public async Task ActivateAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var group = await FindRequiredAsync(
            id,
            cancellationToken);

        group.Activate();
        group.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var group = await FindRequiredAsync(
            id,
            cancellationToken);

        group.Deactivate();
        group.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var group = await FindRequiredAsync(
            id,
            cancellationToken);

        var hasShoppings = await _dbContext.Shoppings
            .AnyAsync(
                shopping => shopping.ShoppingGroupId == id,
                cancellationToken);

        if (hasShoppings)
        {
            throw new InvalidOperationException(
                "Não é possível excluir a rede porque existem shoppings vinculados.");
        }

        group.Delete(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<ShoppingGroup> FindRequiredAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var group = await _dbContext.ShoppingGroups
            .SingleOrDefaultAsync(
                item => item.Id == id,
                cancellationToken);

        if (group is null)
        {
            throw new KeyNotFoundException(
                "Rede de shopping não encontrada.");
        }

        return group;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "O nome da rede de shopping é obrigatório.");
        }
    }

    private static ShoppingGroupDto Map(ShoppingGroup group)
    {
        return new ShoppingGroupDto(
            group.Id,
            group.Name,
            group.Description,
            group.IsActive,
            group.CreatedAt);
    }
}