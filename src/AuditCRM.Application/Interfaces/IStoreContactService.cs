using AuditCRM.Application.Features.StoreContacts.DTOs;

namespace AuditCRM.Application.Interfaces;

public interface IStoreContactService
{
    Task<IReadOnlyList<StoreContactDto>> GetAllAsync(
        Guid? storeId = null,
        CancellationToken cancellationToken = default);

    Task<StoreContactDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<StoreContactDto> CreateAsync(
        CreateStoreContactRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task<StoreContactDto> UpdateAsync(
        Guid id,
        UpdateStoreContactRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task ActivateAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task DeactivateAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);
}