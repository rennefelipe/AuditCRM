using AuditCRM.Domain.Common;
using AuditCRM.Domain.Enums;

namespace AuditCRM.Domain.Entities;

public sealed class StoreProcess : AuditableEntity
{
    private StoreProcess()
    {
    }

    public StoreProcess(
        Guid storeId,
        Guid? installationTypeId = null,
        Guid? responsibleUserId = null)
    {
        if (storeId == Guid.Empty)
        {
            throw new ArgumentException(
                "A loja é obrigatória.",
                nameof(storeId));
        }

        StoreId = storeId;
        InstallationTypeId = installationTypeId;
        ResponsibleUserId = responsibleUserId;

        Status = StoreProcessStatus.Service;
        Priority = ProcessPriority.Normal;

        StartedAt = DateTime.UtcNow;
        IsClosed = false;
    }

    public Guid StoreId { get; private set; }

    public Store? Store { get; private set; }

    public Guid? InstallationTypeId { get; private set; }

    public InstallationType? InstallationType { get; private set; }

    public Guid? ResponsibleUserId { get; private set; }

    public User? ResponsibleUser { get; private set; }

    public StoreProcessStatus Status { get; private set; }

    public ProcessPriority Priority { get; private set; }

    public string? NextAction { get; private set; }

    public DateTime? NextActionAt { get; private set; }

    public DateTime StartedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public string? Notes { get; private set; }

    public bool IsClosed { get; private set; }

    public void Update(
        Guid? installationTypeId,
        Guid? responsibleUserId,
        StoreProcessStatus status,
        ProcessPriority priority,
        string? nextAction,
        DateTime? nextActionAt,
        string? notes)
    {
        InstallationTypeId = installationTypeId;
        ResponsibleUserId = responsibleUserId;
        Status = status;
        Priority = priority;

        NextAction = NormalizeOptionalText(nextAction);
        NextActionAt = nextActionAt;
        Notes = NormalizeOptionalText(notes);

        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeStatus(StoreProcessStatus status)
    {
        Status = status;

        if (status == StoreProcessStatus.Installed)
        {
            CompletedAt ??= DateTime.UtcNow;
        }
        else
        {
            CompletedAt = null;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void SetNextAction(
        string? nextAction,
        DateTime? nextActionAt)
    {
        NextAction = NormalizeOptionalText(nextAction);
        NextActionAt = nextActionAt;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Close()
    {
        IsClosed = true;
        CompletedAt ??= DateTime.UtcNow;
        NextAction = null;
        NextActionAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reopen()
    {
        IsClosed = false;
        CompletedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}