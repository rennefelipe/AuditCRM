namespace AuditCRM.Application.Features.ProcessAppointments.DTOs;

public sealed record UpdateProcessAppointmentRequestDto(
    DateTime ScheduledAt,
    Guid? ResponsibleUserId,
    Guid? StoreContactId,
    string? Notes);