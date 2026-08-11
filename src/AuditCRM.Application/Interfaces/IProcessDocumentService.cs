using AuditCRM.Application.Features.ProcessDocuments.DTOs;
using AuditCRM.Domain.Entities;

namespace AuditCRM.Application.Interfaces;

public interface IProcessDocumentService
{
    Task<IReadOnlyList<ProcessDocumentDto>> GetAllAsync(
        Guid storeProcessId,
        CancellationToken cancellationToken = default);

    Task<ProcessDocumentDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ProcessDocumentDto> CreateAsync(
        Guid storeProcessId,
        Guid? uploadedByUserId,
        string originalFileName,
        string storedFileName,
        string relativePath,
        string contentType,
        long fileSize,
        CancellationToken cancellationToken = default);

    Task<ProcessDocument?> GetEntityByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);
}