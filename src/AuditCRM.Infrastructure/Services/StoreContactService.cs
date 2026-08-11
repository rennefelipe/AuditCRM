using AuditCRM.Application.Features.StoreContacts.DTOs;
using AuditCRM.Application.Interfaces;
using AuditCRM.Domain.Entities;
using AuditCRM.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuditCRM.Infrastructure.Services;

public sealed class StoreContactService : IStoreContactService
{
    private readonly AuditDbContext _dbContext;

    public StoreContactService(AuditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<StoreContactDto>> GetAllAsync(
        Guid? storeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.StoreContacts
            .AsNoTracking()
            .Include(contact => contact.Store)
            .AsQueryable();

        if (storeId.HasValue)
        {
            query = query.Where(
                contact => contact.StoreId == storeId.Value);
        }

        return await query
            .OrderByDescending(contact => contact.IsMainContact)
            .ThenBy(contact => contact.Name)
            .Select(contact => new StoreContactDto(
                contact.Id,
                contact.StoreId,
                contact.Store != null
                    ? contact.Store.TradeName
                    : string.Empty,
                contact.Name,
                contact.Position,
                contact.Phone1,
                contact.Phone2,
                contact.Phone3,
                contact.Email1,
                contact.Email2,
                contact.Email3,
                contact.IsMainContact,
                contact.Notes,
                contact.IsActive,
                contact.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<StoreContactDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.StoreContacts
            .AsNoTracking()
            .Include(contact => contact.Store)
            .Where(contact => contact.Id == id)
            .Select(contact => new StoreContactDto(
                contact.Id,
                contact.StoreId,
                contact.Store != null
                    ? contact.Store.TradeName
                    : string.Empty,
                contact.Name,
                contact.Position,
                contact.Phone1,
                contact.Phone2,
                contact.Phone3,
                contact.Email1,
                contact.Email2,
                contact.Email3,
                contact.IsMainContact,
                contact.Notes,
                contact.IsActive,
                contact.CreatedAt))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<StoreContactDto> CreateAsync(
        CreateStoreContactRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        ValidateCreateRequest(request);

        var storeExists = await _dbContext.Stores
            .AnyAsync(
                store =>
                    store.Id == request.StoreId &&
                    store.IsActive,
                cancellationToken);

        if (!storeExists)
        {
            throw new InvalidOperationException(
                "A loja informada não existe ou está inativa.");
        }

        if (request.IsMainContact)
        {
            await ClearMainContactAsync(
                request.StoreId,
                null,
                currentUserId,
                cancellationToken);
        }

        var contact = new StoreContact(
            request.StoreId,
            request.Name,
            request.Position,
            request.IsMainContact);

        contact.Update(
            request.Name,
            request.Position,
            request.Phone1,
            request.Phone2,
            request.Phone3,
            request.Email1,
            request.Email2,
            request.Email3,
            request.IsMainContact,
            request.Notes);

        contact.MarkCreatedBy(currentUserId);

        _dbContext.StoreContacts.Add(contact);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetRequiredDtoAsync(
            contact.Id,
            cancellationToken);
    }

    public async Task<StoreContactDto> UpdateAsync(
        Guid id,
        UpdateStoreContactRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        ValidateUpdateRequest(request);

        var contact = await FindRequiredAsync(
            id,
            cancellationToken);

        if (request.IsMainContact)
        {
            await ClearMainContactAsync(
                contact.StoreId,
                contact.Id,
                currentUserId,
                cancellationToken);
        }

        contact.Update(
            request.Name,
            request.Position,
            request.Phone1,
            request.Phone2,
            request.Phone3,
            request.Email1,
            request.Email2,
            request.Email3,
            request.IsMainContact,
            request.Notes);

        contact.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetRequiredDtoAsync(
            contact.Id,
            cancellationToken);
    }

    public async Task ActivateAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var contact = await FindRequiredAsync(
            id,
            cancellationToken);

        contact.Activate();
        contact.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var contact = await FindRequiredAsync(
            id,
            cancellationToken);

        contact.Deactivate();
        contact.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var contact = await FindRequiredAsync(
            id,
            cancellationToken);

        contact.Delete(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task ClearMainContactAsync(
        Guid storeId,
        Guid? currentContactId,
        Guid? currentUserId,
        CancellationToken cancellationToken)
    {
        var mainContacts = await _dbContext.StoreContacts
            .Where(contact =>
                contact.StoreId == storeId &&
                contact.IsMainContact &&
                (!currentContactId.HasValue ||
                 contact.Id != currentContactId.Value))
            .ToListAsync(cancellationToken);

        foreach (var contact in mainContacts)
        {
            contact.Update(
                contact.Name,
                contact.Position,
                contact.Phone1,
                contact.Phone2,
                contact.Phone3,
                contact.Email1,
                contact.Email2,
                contact.Email3,
                false,
                contact.Notes);

            contact.MarkUpdatedBy(currentUserId);
        }
    }

    private async Task<StoreContact> FindRequiredAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var contact = await _dbContext.StoreContacts
            .SingleOrDefaultAsync(
                item => item.Id == id,
                cancellationToken);

        if (contact is null)
        {
            throw new KeyNotFoundException(
                "Contato não encontrado.");
        }

        return contact;
    }

    private async Task<StoreContactDto> GetRequiredDtoAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var contact = await GetByIdAsync(
            id,
            cancellationToken);

        if (contact is null)
        {
            throw new KeyNotFoundException(
                "Contato não encontrado.");
        }

        return contact;
    }

    private static void ValidateCreateRequest(
        CreateStoreContactRequestDto request)
    {
        if (request.StoreId == Guid.Empty)
        {
            throw new ArgumentException(
                "A loja é obrigatória.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException(
                "O nome do contato é obrigatório.");
        }
    }

    private static void ValidateUpdateRequest(
        UpdateStoreContactRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException(
                "O nome do contato é obrigatório.");
        }
    }
}