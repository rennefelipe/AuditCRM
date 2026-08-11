using AuditCRM.Application.Features.Shoppings.DTOs;

namespace AuditCRM.Application.Interfaces;

public interface IShoppingService
{
    Task<IReadOnlyList<ShoppingDto>> GetAllAsync(
        Guid? shoppingGroupId = null,
        CancellationToken cancellationToken = default);

    Task<ShoppingDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ShoppingDto> CreateAsync(
        CreateShoppingRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task<ShoppingDto> UpdateAsync(
        Guid id,
        UpdateShoppingRequestDto request,
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