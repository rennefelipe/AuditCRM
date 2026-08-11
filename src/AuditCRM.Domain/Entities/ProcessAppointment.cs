using AuditCRM.Domain.Common;
using AuditCRM.Domain.Enums;

namespace AuditCRM.Domain.Entities;

public sealed class ProcessAppointment : AuditableEntity
{
    private ProcessAppointment()
    {
    }

    public ProcessAppointment(
        Guid storeProcessId,
        DateTime scheduledAt,
        Guid? responsibleUserId = null,
        Guid? storeContactId = null,
        string? notes = null)
    {
        if (storeProcessId == Guid.Empty)
        {
            throw new ArgumentException(
                "O processo da loja é obrigatório.",
                nameof(storeProcessId));
        }

        StoreProcessId = storeProcessId;
        ScheduledAt = scheduledAt;
        ResponsibleUserId = responsibleUserId;
        StoreContactId = storeContactId;
        Notes = NormalizeOptionalText(notes);

        Status = AppointmentStatus.Scheduled;
    }

    public Guid StoreProcessId { get; private set; }

    public StoreProcess? StoreProcess { get; private set; }

    public Guid? ResponsibleUserId { get; private set; }

    public User? ResponsibleUser { get; private set; }

    public Guid? StoreContactId { get; private set; }

    public StoreContact? StoreContact { get; private set; }

    public DateTime ScheduledAt { get; private set; }

    public AppointmentStatus Status { get; private set; }

    public string? Notes { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public DateTime? CancelledAt { get; private set; }

    public void Update(
        DateTime scheduledAt,
        Guid? responsibleUserId,
        Guid? storeContactId,
        string? notes)
    {
        ScheduledAt = scheduledAt;
        ResponsibleUserId = responsibleUserId;
        StoreContactId = storeContactId;
        Notes = NormalizeOptionalText(notes);

        UpdatedAt = DateTime.UtcNow;
    }

    public void Confirm()
    {
        Status = AppointmentStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        Status = AppointmentStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        CancelledAt = null;

        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = AppointmentStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CompletedAt = null;

        UpdatedAt = DateTime.UtcNow;
    }

    public void Reschedule(DateTime scheduledAt)
    {
        ScheduledAt = scheduledAt;
        Status = AppointmentStatus.Rescheduled;
        CompletedAt = null;
        CancelledAt = null;

        UpdatedAt = DateTime.UtcNow;
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}