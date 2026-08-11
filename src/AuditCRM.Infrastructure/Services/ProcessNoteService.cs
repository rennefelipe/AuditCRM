using AuditCRM.Application.Features.ProcessNotes.DTOs;
using AuditCRM.Application.Interfaces;
using AuditCRM.Domain.Entities;
using AuditCRM.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuditCRM.Infrastructure.Services;

public sealed class ProcessNoteService : IProcessNoteService
{
    private readonly AuditDbContext _dbContext;

    public ProcessNoteService(AuditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ProcessNoteDto>> GetAllAsync(
        Guid storeProcessId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProcessNotes
            .AsNoTracking()
            .Where(x => x.StoreProcessId == storeProcessId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ProcessNoteDto(
                x.Id,
                x.StoreProcessId,
                x.AuthorUserId,
                x.AuthorUser != null ? x.AuthorUser.Name : null,
                x.Text,
                x.CreatedAt,
                x.UpdatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<ProcessNoteDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProcessNotes
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new ProcessNoteDto(
                x.Id,
                x.StoreProcessId,
                x.AuthorUserId,
                x.AuthorUser != null ? x.AuthorUser.Name : null,
                x.Text,
                x.CreatedAt,
                x.UpdatedAt))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<ProcessNoteDto> CreateAsync(
        CreateProcessNoteRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var processExists = await _dbContext.StoreProcesses
            .AnyAsync(
                x => x.Id == request.StoreProcessId,
                cancellationToken);

        if (!processExists)
        {
            throw new InvalidOperationException(
                "O processo informado não existe.");
        }

        var note = new ProcessNote(
            request.StoreProcessId,
            request.Text,
            currentUserId);

        note.MarkCreatedBy(currentUserId);

        _dbContext.ProcessNotes.Add(note);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetRequiredAsync(
            note.Id,
            cancellationToken);
    }

    public async Task<ProcessNoteDto> UpdateAsync(
        Guid id,
        UpdateProcessNoteRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var note = await _dbContext.ProcessNotes
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (note is null)
        {
            throw new KeyNotFoundException(
                "Anotação não encontrada.");
        }

        note.Update(request.Text);
        note.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetRequiredAsync(
            note.Id,
            cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var note = await _dbContext.ProcessNotes
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (note is null)
        {
            throw new KeyNotFoundException(
                "Anotação não encontrada.");
        }

        note.Delete(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<ProcessNoteDto> GetRequiredAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await GetByIdAsync(
            id,
            cancellationToken);

        return result
            ?? throw new KeyNotFoundException(
                "Anotação não encontrada.");
    }
}