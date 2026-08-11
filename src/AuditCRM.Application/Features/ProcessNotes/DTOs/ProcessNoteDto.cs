namespace AuditCRM.Application.Features.ProcessNotes.DTOs;

public sealed record ProcessNoteDto(
    Guid Id,
    Guid StoreProcessId,
    Guid? AuthorUserId,
    string? AuthorUserName,
    string Text,
    DateTime CreatedAt,
    DateTime? UpdatedAt);