using AuditCRM.Domain.Common;

namespace AuditCRM.Domain.Entities;

public sealed class ProcessNote : AuditableEntity
{
    private ProcessNote()
    {
    }

    public ProcessNote(
        Guid storeProcessId,
        string text,
        Guid? authorUserId = null)
    {
        if (storeProcessId == Guid.Empty)
        {
            throw new ArgumentException(
                "O processo da loja é obrigatório.",
                nameof(storeProcessId));
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException(
                "O texto da anotação é obrigatório.",
                nameof(text));
        }

        StoreProcessId = storeProcessId;
        AuthorUserId = authorUserId;
        Text = text.Trim();
    }

    public Guid StoreProcessId { get; private set; }

    public StoreProcess? StoreProcess { get; private set; }

    public Guid? AuthorUserId { get; private set; }

    public User? AuthorUser { get; private set; }

    public string Text { get; private set; } = string.Empty;

    public void Update(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException(
                "O texto da anotação é obrigatório.",
                nameof(text));
        }

        Text = text.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}