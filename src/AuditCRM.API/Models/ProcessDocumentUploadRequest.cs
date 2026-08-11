using Microsoft.AspNetCore.Http;

namespace AuditCRM.API.Models;

public sealed class ProcessDocumentUploadRequest
{
    public Guid StoreProcessId { get; set; }

    public IFormFile File { get; set; } = default!;
}