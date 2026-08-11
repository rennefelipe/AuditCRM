using AuditCRM.Domain.Enums;

namespace AuditCRM.Application.Features.ProcessAppointments.DTOs;

public sealed record ProcessAppointmentDto(
    Guid Id,
    Guid StoreProcessId,
    Guid? ResponsibleUserId,
    string? ResponsibleUserName,
    Guid? StoreContactId,
    string? StoreContactName,
    DateTime ScheduledAt,
    AppointmentStatus Status,
    string? Notes,
    DateTime? CompletedAt,
    DateTime? CancelledAt,
    DateTime CreatedAt);