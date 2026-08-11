namespace AuditCRM.Application.Features.ProcessDocuments.DTOs;

public sealed record CreateProcessDocumentRequestDto(
    Guid StoreProcessId);