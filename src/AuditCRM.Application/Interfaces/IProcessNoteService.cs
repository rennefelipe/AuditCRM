using AuditCRM.Application.Features.ProcessNotes.DTOs;

namespace AuditCRM.Application.Interfaces;

public interface IProcessNoteService
{
    Task<IReadOnlyList<ProcessNoteDto>> GetAllAsync(
        Guid storeProcessId,
        CancellationToken cancellationToken = default);

    Task<ProcessNoteDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ProcessNoteDto> CreateAsync(
        CreateProcessNoteRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task<ProcessNoteDto> UpdateAsync(
        Guid id,
        UpdateProcessNoteRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);
}