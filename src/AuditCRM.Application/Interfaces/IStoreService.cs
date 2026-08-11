using AuditCRM.Application.Features.Stores.DTOs;

namespace AuditCRM.Application.Interfaces;

public interface IStoreService
{
    Task<IReadOnlyList<StoreDto>> GetAllAsync(
        Guid? shoppingId = null,
        Guid? shoppingGroupId = null,
        CancellationToken cancellationToken = default);

    Task<StoreDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<StoreDto> CreateAsync(
        CreateStoreRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task<StoreDto> UpdateAsync(
        Guid id,
        UpdateStoreRequestDto request,
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