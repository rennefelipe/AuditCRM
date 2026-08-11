using AuditCRM.Application.Features.ProcessInteractions.DTOs;

namespace AuditCRM.Application.Interfaces;

public interface IProcessInteractionService
{
    Task<IReadOnlyList<ProcessInteractionDto>> GetAllAsync(
        Guid? storeProcessId = null,
        CancellationToken cancellationToken = default);

    Task<ProcessInteractionDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ProcessInteractionDto> CreateAsync(
        CreateProcessInteractionRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task<ProcessInteractionDto> UpdateAsync(
        Guid id,
        UpdateProcessInteractionRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);
}