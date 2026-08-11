using AuditCRM.Application.Features.Stores.DTOs;
using AuditCRM.Application.Interfaces;
using AuditCRM.Domain.Entities;
using AuditCRM.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuditCRM.Infrastructure.Services;

public sealed class StoreService : IStoreService
{
    private readonly AuditDbContext _dbContext;

    public StoreService(AuditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<StoreDto>> GetAllAsync(
        Guid? shoppingId = null,
        Guid? shoppingGroupId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Stores
            .AsNoTracking()
            .Include(store => store.Shopping)
            .ThenInclude(shopping => shopping!.ShoppingGroup)
            .Include(store => store.Erp)
            .AsQueryable();

        if (shoppingId.HasValue)
        {
            query = query.Where(
                store => store.ShoppingId == shoppingId.Value);
        }

        if (shoppingGroupId.HasValue)
        {
            query = query.Where(
                store =>
                    store.Shopping != null &&
                    store.Shopping.ShoppingGroupId == shoppingGroupId.Value);
        }

        return await query
            .OrderBy(store => store.TradeName)
            .Select(store => new StoreDto(
                store.Id,
                store.ShoppingId,
                store.Shopping != null
                    ? store.Shopping.Name
                    : string.Empty,
                store.ErpId,
                store.Erp != null
                    ? store.Erp.Name
                    : null,
                store.Luc,
                store.TradeName,
                store.CorporateName,
                store.Document,
                store.StateRegistration,
                store.NumberOfRegisters,
                store.MonitoringStatus,
                store.BusinessType,
                store.Notes,
                store.IsActive,
                store.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<StoreDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Stores
            .AsNoTracking()
            .Include(store => store.Shopping)
            .Include(store => store.Erp)
            .Where(store => store.Id == id)
            .Select(store => new StoreDto(
                store.Id,
                store.ShoppingId,
                store.Shopping != null
                    ? store.Shopping.Name
                    : string.Empty,
                store.ErpId,
                store.Erp != null
                    ? store.Erp.Name
                    : null,
                store.Luc,
                store.TradeName,
                store.CorporateName,
                store.Document,
                store.StateRegistration,
                store.NumberOfRegisters,
                store.MonitoringStatus,
                store.BusinessType,
                store.Notes,
                store.IsActive,
                store.CreatedAt))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<StoreDto> CreateAsync(
        CreateStoreRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);

        await ValidateShoppingAsync(
            request.ShoppingId,
            cancellationToken);

        await ValidateErpAsync(
            request.ErpId,
            cancellationToken);

        await ValidateDuplicatesAsync(
            request.ShoppingId,
            request.Luc,
            request.Document,
            null,
            cancellationToken);

        var store = new Store(
            request.ShoppingId,
            request.TradeName,
            request.Luc,
            request.Document);

        store.Update(
            request.ShoppingId,
            request.ErpId,
            request.TradeName,
            request.Luc,
            request.CorporateName,
            request.Document,
            request.StateRegistration,
            request.NumberOfRegisters,
            request.MonitoringStatus,
            request.BusinessType,
            request.Notes);

        store.MarkCreatedBy(currentUserId);

        _dbContext.Stores.Add(store);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetRequiredDtoAsync(
            store.Id,
            cancellationToken);
    }

    public async Task<StoreDto> UpdateAsync(
        Guid id,
        UpdateStoreRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);

        await ValidateShoppingAsync(
            request.ShoppingId,
            cancellationToken);

        await ValidateErpAsync(
            request.ErpId,
            cancellationToken);

        var store = await FindRequiredAsync(
            id,
            cancellationToken);

        await ValidateDuplicatesAsync(
            request.ShoppingId,
            request.Luc,
            request.Document,
            id,
            cancellationToken);

        store.Update(
            request.ShoppingId,
            request.ErpId,
            request.TradeName,
            request.Luc,
            request.CorporateName,
            request.Document,
            request.StateRegistration,
            request.NumberOfRegisters,
            request.MonitoringStatus,
            request.BusinessType,
            request.Notes);

        store.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetRequiredDtoAsync(
            store.Id,
            cancellationToken);
    }

    public async Task ActivateAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var store = await FindRequiredAsync(
            id,
            cancellationToken);

        store.Activate();
        store.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var store = await FindRequiredAsync(
            id,
            cancellationToken);

        store.Deactivate();
        store.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var store = await FindRequiredAsync(
            id,
            cancellationToken);

        var hasContacts = await _dbContext.StoreContacts
            .AnyAsync(
                contact => contact.StoreId == id,
                cancellationToken);

        if (hasContacts)
        {
            throw new InvalidOperationException(
                "Não é possível excluir a loja porque existem contatos vinculados.");
        }

        store.Delete(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<Store> FindRequiredAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var store = await _dbContext.Stores
            .SingleOrDefaultAsync(
                item => item.Id == id,
                cancellationToken);

        if (store is null)
        {
            throw new KeyNotFoundException(
                "Loja não encontrada.");
        }

        return store;
    }

    private async Task ValidateShoppingAsync(
        Guid shoppingId,
        CancellationToken cancellationToken)
    {
        var exists = await _dbContext.Shoppings
            .AnyAsync(
                shopping =>
                    shopping.Id == shoppingId &&
                    shopping.IsActive,
                cancellationToken);

        if (!exists)
        {
            throw new InvalidOperationException(
                "O shopping informado não existe ou está inativo.");
        }
    }

    private async Task ValidateErpAsync(
        Guid? erpId,
        CancellationToken cancellationToken)
    {
        if (!erpId.HasValue)
        {
            return;
        }

        var exists = await _dbContext.Erps
            .AnyAsync(
                erp =>
                    erp.Id == erpId.Value &&
                    erp.IsActive,
                cancellationToken);

        if (!exists)
        {
            throw new InvalidOperationException(
                "O ERP informado não existe ou está inativo.");
        }
    }

    private async Task ValidateDuplicatesAsync(
        Guid shoppingId,
        string? luc,
        string? document,
        Guid? storeId,
        CancellationToken cancellationToken)
    {
        var normalizedLuc = string.IsNullOrWhiteSpace(luc)
            ? null
            : luc.Trim();

        var normalizedDocument = NormalizeDocument(document);

        if (!string.IsNullOrWhiteSpace(normalizedLuc))
        {
            var lucExists = await _dbContext.Stores
                .IgnoreQueryFilters()
                .AnyAsync(
                    store =>
                        store.ShoppingId == shoppingId &&
                        store.Luc == normalizedLuc &&
                        (!storeId.HasValue ||
                         store.Id != storeId.Value),
                    cancellationToken);

            if (lucExists)
            {
                throw new InvalidOperationException(
                    "Já existe uma loja com este LUC neste shopping.");
            }
        }

        if (!string.IsNullOrWhiteSpace(normalizedDocument))
        {
            var documentExists = await _dbContext.Stores
                .IgnoreQueryFilters()
                .AnyAsync(
                    store =>
                        store.ShoppingId == shoppingId &&
                        store.Document == normalizedDocument &&
                        (!storeId.HasValue ||
                         store.Id != storeId.Value),
                    cancellationToken);

            if (documentExists)
            {
                throw new InvalidOperationException(
                    "Já existe uma loja com este CNPJ neste shopping.");
            }
        }
    }

    private async Task<StoreDto> GetRequiredDtoAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var store = await GetByIdAsync(
            id,
            cancellationToken);

        if (store is null)
        {
            throw new KeyNotFoundException(
                "Loja não encontrada.");
        }

        return store;
    }

    private static void ValidateRequest(
        CreateStoreRequestDto request)
    {
        ValidateCommon(
            request.ShoppingId,
            request.TradeName,
            request.NumberOfRegisters);
    }

    private static void ValidateRequest(
        UpdateStoreRequestDto request)
    {
        ValidateCommon(
            request.ShoppingId,
            request.TradeName,
            request.NumberOfRegisters);
    }

    private static void ValidateCommon(
        Guid shoppingId,
        string tradeName,
        int? numberOfRegisters)
    {
        if (shoppingId == Guid.Empty)
        {
            throw new ArgumentException(
                "O shopping é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(tradeName))
        {
            throw new ArgumentException(
                "O nome da loja é obrigatório.");
        }

        if (numberOfRegisters < 0)
        {
            throw new ArgumentException(
                "A quantidade de caixas não pode ser negativa.");
        }
    }

    private static string? NormalizeDocument(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return new string(value.Where(char.IsDigit).ToArray());
    }
}