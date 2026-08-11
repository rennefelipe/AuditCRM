using AuditCRM.Domain.Common;
using AuditCRM.Domain.Enums;

namespace AuditCRM.Domain.Entities;

public sealed class ProcessInteraction : AuditableEntity
{
    private ProcessInteraction()
    {
    }

    public ProcessInteraction(
        Guid storeProcessId,
        InteractionChannel channel,
        DateTime occurredAt,
        string description,
        Guid? responsibleUserId = null,
        Guid? storeContactId = null)
    {
        if (storeProcessId == Guid.Empty)
        {
            throw new ArgumentException(
                "O processo da loja é obrigatório.",
                nameof(storeProcessId));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException(
                "A descrição da tratativa é obrigatória.",
                nameof(description));
        }

        StoreProcessId = storeProcessId;
        Channel = channel;
        OccurredAt = occurredAt;
        ResponsibleUserId = responsibleUserId;
        StoreContactId = storeContactId;

        Description = description.Trim();
    }

    public Guid StoreProcessId { get; private set; }

    public StoreProcess? StoreProcess { get; private set; }

    public Guid? ResponsibleUserId { get; private set; }

    public User? ResponsibleUser { get; private set; }

    public Guid? StoreContactId { get; private set; }

    public StoreContact? StoreContact { get; private set; }

    public InteractionChannel Channel { get; private set; }

    public DateTime OccurredAt { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public string? Result { get; private set; }

    public string? NextAction { get; private set; }

    public DateTime? NextActionAt { get; private set; }

    public void Update(
        InteractionChannel channel,
        DateTime occurredAt,
        Guid? responsibleUserId,
        Guid? storeContactId,
        string description,
        string? result,
        string? nextAction,
        DateTime? nextActionAt)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException(
                "A descrição da tratativa é obrigatória.",
                nameof(description));
        }

        Channel = channel;
        OccurredAt = occurredAt;
        ResponsibleUserId = responsibleUserId;
        StoreContactId = storeContactId;
        Description = description.Trim();
        Result = NormalizeOptionalText(result);
        NextAction = NormalizeOptionalText(nextAction);
        NextActionAt = nextActionAt;

        UpdatedAt = DateTime.UtcNow;
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}