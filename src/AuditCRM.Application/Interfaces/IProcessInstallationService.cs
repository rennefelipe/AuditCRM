using AuditCRM.Application.Features.ProcessInstallations.DTOs;

namespace AuditCRM.Application.Interfaces;

public interface IProcessInstallationService
{
    Task<IReadOnlyList<ProcessInstallationDto>> GetAllAsync(
        Guid? storeProcessId = null,
        CancellationToken cancellationToken = default);

    Task<ProcessInstallationDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ProcessInstallationDto> CreateAsync(
        CreateProcessInstallationRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task<ProcessInstallationDto> UpdateAsync(
        Guid id,
        UpdateProcessInstallationRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);
}