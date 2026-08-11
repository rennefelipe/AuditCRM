namespace AuditCRM.Application.Features.ProcessAppointments.DTOs;

public sealed record RescheduleProcessAppointmentRequestDto(
    DateTime ScheduledAt);