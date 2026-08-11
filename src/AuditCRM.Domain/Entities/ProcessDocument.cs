using AuditCRM.Domain.Common;

namespace AuditCRM.Domain.Entities;

public sealed class ProcessDocument : AuditableEntity
{
    private ProcessDocument()
    {
    }

    public ProcessDocument(
        Guid storeProcessId,
        string originalFileName,
        string storedFileName,
        string relativePath,
        string contentType,
        long fileSize,
        Guid? uploadedByUserId = null)
    {
        if (storeProcessId == Guid.Empty)
        {
            throw new ArgumentException(
                "O processo da loja é obrigatório.",
                nameof(storeProcessId));
        }

        if (string.IsNullOrWhiteSpace(originalFileName))
        {
            throw new ArgumentException(
                "O nome original do arquivo é obrigatório.",
                nameof(originalFileName));
        }

        if (string.IsNullOrWhiteSpace(storedFileName))
        {
            throw new ArgumentException(
                "O nome interno do arquivo é obrigatório.",
                nameof(storedFileName));
        }

        if (string.IsNullOrWhiteSpace(relativePath))
        {
            throw new ArgumentException(
                "O caminho do arquivo é obrigatório.",
                nameof(relativePath));
        }

        if (fileSize < 0)
        {
            throw new ArgumentException(
                "O tamanho do arquivo não pode ser negativo.",
                nameof(fileSize));
        }

        StoreProcessId = storeProcessId;
        OriginalFileName = originalFileName.Trim();
        StoredFileName = storedFileName.Trim();
        RelativePath = relativePath.Trim();
        ContentType = string.IsNullOrWhiteSpace(contentType)
            ? "application/octet-stream"
            : contentType.Trim();

        FileSize = fileSize;
        UploadedByUserId = uploadedByUserId;
    }

    public Guid StoreProcessId { get; private set; }

    public StoreProcess? StoreProcess { get; private set; }

    public Guid? UploadedByUserId { get; private set; }

    public User? UploadedByUser { get; private set; }

    public string OriginalFileName { get; private set; } = string.Empty;

    public string StoredFileName { get; private set; } = string.Empty;

    public string RelativePath { get; private set; } = string.Empty;

    public string ContentType { get; private set; } = string.Empty;

    public long FileSize { get; private set; }
}