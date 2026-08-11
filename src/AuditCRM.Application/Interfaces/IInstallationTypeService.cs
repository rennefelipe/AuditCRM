using AuditCRM.Application.Features.InstallationTypes.DTOs;

namespace AuditCRM.Application.Interfaces;

public interface IInstallationTypeService
{
    Task<IReadOnlyList<InstallationTypeDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<InstallationTypeDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<InstallationTypeDto> CreateAsync(
        CreateInstallationTypeRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task<InstallationTypeDto> UpdateAsync(
        Guid id,
        UpdateInstallationTypeRequestDto request,
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