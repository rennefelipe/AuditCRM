namespace AuditCRM.Application.Features.ProcessAppointments.DTOs;

public sealed record CreateProcessAppointmentRequestDto(
    Guid StoreProcessId,
    DateTime ScheduledAt,
    Guid? ResponsibleUserId,
    Guid? StoreContactId,
    string? Notes);