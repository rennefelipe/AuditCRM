using AuditCRM.Application.Features.StoreProcesses.DTOs;

namespace AuditCRM.Application.Interfaces;

public interface IStoreProcessService
{
    Task<IReadOnlyList<StoreProcessDto>> GetAllAsync(
        Guid? storeId = null,
        Guid? shoppingId = null,
        Guid? responsibleUserId = null,
        CancellationToken cancellationToken = default);

    Task<StoreProcessDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<StoreProcessDto> CreateAsync(
        CreateStoreProcessRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task<StoreProcessDto> UpdateAsync(
        Guid id,
        UpdateStoreProcessRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task CloseAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task ReopenAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);
}