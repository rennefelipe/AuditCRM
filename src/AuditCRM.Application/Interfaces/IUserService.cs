using AuditCRM.Application.Features.Users.DTOs;

namespace AuditCRM.Application.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<UserDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<UserDto> CreateAsync(
        CreateUserRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task<UserDto> UpdateAsync(
        Guid id,
        UpdateUserRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task ChangePasswordAsync(
        Guid id,
        ChangeUserPasswordRequestDto request,
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