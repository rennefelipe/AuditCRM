namespace AuditCRM.Application.Features.ProcessNotes.DTOs;

public sealed record CreateProcessNoteRequestDto(
    Guid StoreProcessId,
    string Text);