using AuditCRM.Application.Features.Shoppings.DTOs;
using AuditCRM.Application.Interfaces;
using AuditCRM.Domain.Entities;
using AuditCRM.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuditCRM.Infrastructure.Services;

public sealed class ShoppingService : IShoppingService
{
    private readonly AuditDbContext _dbContext;

    public ShoppingService(AuditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ShoppingDto>> GetAllAsync(
        Guid? shoppingGroupId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Shoppings
            .AsNoTracking()
            .Include(shopping => shopping.ShoppingGroup)
            .AsQueryable();

        if (shoppingGroupId.HasValue)
        {
            query = query.Where(
                shopping => shopping.ShoppingGroupId == shoppingGroupId.Value);
        }

        return await query
            .OrderBy(shopping => shopping.Name)
            .Select(shopping => new ShoppingDto(
                shopping.Id,
                shopping.ShoppingGroupId,
                shopping.ShoppingGroup != null
                    ? shopping.ShoppingGroup.Name
                    : string.Empty,
                shopping.Name,
                shopping.CorporateName,
                shopping.Document,
                shopping.Address,
                shopping.Number,
                shopping.District,
                shopping.City,
                shopping.State,
                shopping.ZipCode,
                shopping.ContactName,
                shopping.ContactEmail,
                shopping.ContactPhone,
                shopping.PaysInstallation,
                shopping.Notes,
                shopping.IsActive,
                shopping.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<ShoppingDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Shoppings
            .AsNoTracking()
            .Include(shopping => shopping.ShoppingGroup)
            .Where(shopping => shopping.Id == id)
            .Select(shopping => new ShoppingDto(
                shopping.Id,
                shopping.ShoppingGroupId,
                shopping.ShoppingGroup != null
                    ? shopping.ShoppingGroup.Name
                    : string.Empty,
                shopping.Name,
                shopping.CorporateName,
                shopping.Document,
                shopping.Address,
                shopping.Number,
                shopping.District,
                shopping.City,
                shopping.State,
                shopping.ZipCode,
                shopping.ContactName,
                shopping.ContactEmail,
                shopping.ContactPhone,
                shopping.PaysInstallation,
                shopping.Notes,
                shopping.IsActive,
                shopping.CreatedAt))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<ShoppingDto> CreateAsync(
        CreateShoppingRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        ValidateRequest(
            request.ShoppingGroupId,
            request.Name);

        await ValidateShoppingGroupAsync(
            request.ShoppingGroupId,
            cancellationToken);

        await ValidateDuplicatedNameAsync(
            request.ShoppingGroupId,
            request.Name,
            null,
            cancellationToken);

        var shopping = new Shopping(
            request.ShoppingGroupId,
            request.Name,
            request.PaysInstallation);

        shopping.Update(
            request.ShoppingGroupId,
            request.Name,
            request.CorporateName,
            request.Document,
            request.Address,
            request.Number,
            request.District,
            request.City,
            request.State,
            request.ZipCode,
            request.ContactName,
            request.ContactEmail,
            request.ContactPhone,
            request.PaysInstallation,
            request.Notes);

        shopping.MarkCreatedBy(currentUserId);

        _dbContext.Shoppings.Add(shopping);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetRequiredDtoAsync(
            shopping.Id,
            cancellationToken);
    }

    public async Task<ShoppingDto> UpdateAsync(
        Guid id,
        UpdateShoppingRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        ValidateRequest(
            request.ShoppingGroupId,
            request.Name);

        await ValidateShoppingGroupAsync(
            request.ShoppingGroupId,
            cancellationToken);

        var shopping = await FindRequiredAsync(
            id,
            cancellationToken);

        await ValidateDuplicatedNameAsync(
            request.ShoppingGroupId,
            request.Name,
            id,
            cancellationToken);

        shopping.Update(
            request.ShoppingGroupId,
            request.Name,
            request.CorporateName,
            request.Document,
            request.Address,
            request.Number,
            request.District,
            request.City,
            request.State,
            request.ZipCode,
            request.ContactName,
            request.ContactEmail,
            request.ContactPhone,
            request.PaysInstallation,
            request.Notes);

        shopping.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetRequiredDtoAsync(
            shopping.Id,
            cancellationToken);
    }

    public async Task ActivateAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var shopping = await FindRequiredAsync(
            id,
            cancellationToken);

        shopping.Activate();
        shopping.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var shopping = await FindRequiredAsync(
            id,
            cancellationToken);

        shopping.Deactivate();
        shopping.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var shopping = await FindRequiredAsync(
            id,
            cancellationToken);

        var hasStores = await _dbContext.Stores
            .AnyAsync(
                store => store.ShoppingId == id,
                cancellationToken);

        if (hasStores)
        {
            throw new InvalidOperationException(
                "Não é possível excluir o shopping porque existem lojas vinculadas.");
        }

        shopping.Delete(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<Shopping> FindRequiredAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var shopping = await _dbContext.Shoppings
            .SingleOrDefaultAsync(
                item => item.Id == id,
                cancellationToken);

        if (shopping is null)
        {
            throw new KeyNotFoundException(
                "Shopping não encontrado.");
        }

        return shopping;
    }

    private async Task ValidateShoppingGroupAsync(
        Guid shoppingGroupId,
        CancellationToken cancellationToken)
    {
        var exists = await _dbContext.ShoppingGroups
            .AnyAsync(
                group =>
                    group.Id == shoppingGroupId &&
                    group.IsActive,
                cancellationToken);

        if (!exists)
        {
            throw new InvalidOperationException(
                "A rede de shopping informada não existe ou está inativa.");
        }
    }

    private async Task ValidateDuplicatedNameAsync(
        Guid shoppingGroupId,
        string name,
        Guid? shoppingId,
        CancellationToken cancellationToken)
    {
        var normalizedName = name.Trim();

        var exists = await _dbContext.Shoppings
            .IgnoreQueryFilters()
            .AnyAsync(
                shopping =>
                    shopping.ShoppingGroupId == shoppingGroupId &&
                    shopping.Name == normalizedName &&
                    (!shoppingId.HasValue ||
                     shopping.Id != shoppingId.Value),
                cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException(
                "Já existe um shopping com este nome nesta rede.");
        }
    }

    private async Task<ShoppingDto> GetRequiredDtoAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var shopping = await GetByIdAsync(
            id,
            cancellationToken);

        if (shopping is null)
        {
            throw new KeyNotFoundException(
                "Shopping não encontrado.");
        }

        return shopping;
    }

    private static void ValidateRequest(
        Guid shoppingGroupId,
        string name)
    {
        if (shoppingGroupId == Guid.Empty)
        {
            throw new ArgumentException(
                "A rede de shopping é obrigatória.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "O nome do shopping é obrigatório.");
        }
    }
}