using AuditCRM.Application.Features.ProcessDocuments.DTOs;
using AuditCRM.Application.Interfaces;
using AuditCRM.Domain.Entities;
using AuditCRM.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuditCRM.Infrastructure.Services;

public sealed class ProcessDocumentService
    : IProcessDocumentService
{
    private readonly AuditDbContext _dbContext;

    public ProcessDocumentService(
        AuditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ProcessDocumentDto>> GetAllAsync(
        Guid storeProcessId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProcessDocuments
            .AsNoTracking()
            .Where(x => x.StoreProcessId == storeProcessId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ProcessDocumentDto(
                x.Id,
                x.StoreProcessId,
                x.UploadedByUserId,
                x.UploadedByUser != null
                    ? x.UploadedByUser.Name
                    : null,
                x.OriginalFileName,
                x.ContentType,
                x.FileSize,
                x.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<ProcessDocumentDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProcessDocuments
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new ProcessDocumentDto(
                x.Id,
                x.StoreProcessId,
                x.UploadedByUserId,
                x.UploadedByUser != null
                    ? x.UploadedByUser.Name
                    : null,
                x.OriginalFileName,
                x.ContentType,
                x.FileSize,
                x.CreatedAt))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<ProcessDocumentDto> CreateAsync(
        Guid storeProcessId,
        Guid? uploadedByUserId,
        string originalFileName,
        string storedFileName,
        string relativePath,
        string contentType,
        long fileSize,
        CancellationToken cancellationToken = default)
    {
        var processExists = await _dbContext.StoreProcesses
            .AnyAsync(
                x => x.Id == storeProcessId,
                cancellationToken);

        if (!processExists)
        {
            throw new InvalidOperationException(
                "O processo informado não existe.");
        }

        var document = new ProcessDocument(
            storeProcessId,
            originalFileName,
            storedFileName,
            relativePath,
            contentType,
            fileSize,
            uploadedByUserId);

        document.MarkCreatedBy(uploadedByUserId);

        _dbContext.ProcessDocuments.Add(document);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return await GetRequiredDtoAsync(
            document.Id,
            cancellationToken);
    }

    public async Task<ProcessDocument?> GetEntityByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProcessDocuments
            .AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var document = await _dbContext.ProcessDocuments
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (document is null)
        {
            throw new KeyNotFoundException(
                "Documento não encontrado.");
        }

        document.Delete(currentUserId);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    private async Task<ProcessDocumentDto> GetRequiredDtoAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var document = await GetByIdAsync(
            id,
            cancellationToken);

        return document
            ?? throw new KeyNotFoundException(
                "Documento não encontrado.");
    }
}