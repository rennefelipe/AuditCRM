namespace AuditCRM.Application.Features.ProcessDocuments.DTOs;

public sealed record ProcessDocumentDto(
    Guid Id,
    Guid StoreProcessId,
    Guid? UploadedByUserId,
    string? UploadedByUserName,
    string OriginalFileName,
    string ContentType,
    long FileSize,
    DateTime CreatedAt);