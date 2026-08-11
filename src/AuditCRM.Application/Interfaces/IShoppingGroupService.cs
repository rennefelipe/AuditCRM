using AuditCRM.Application.Features.ShoppingGroups.DTOs;

namespace AuditCRM.Application.Interfaces;

public interface IShoppingGroupService
{
    Task<IReadOnlyList<ShoppingGroupDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ShoppingGroupDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ShoppingGroupDto> CreateAsync(
        CreateShoppingGroupRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task<ShoppingGroupDto> UpdateAsync(
        Guid id,
        UpdateShoppingGroupRequestDto request,
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