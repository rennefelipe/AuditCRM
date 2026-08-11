using AuditCRM.Application.Features.ProcessAppointments.DTOs;

namespace AuditCRM.Application.Interfaces;

public interface IProcessAppointmentService
{
    Task<IReadOnlyList<ProcessAppointmentDto>> GetAllAsync(
        Guid? storeProcessId = null,
        Guid? responsibleUserId = null,
        CancellationToken cancellationToken = default);

    Task<ProcessAppointmentDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ProcessAppointmentDto> CreateAsync(
        CreateProcessAppointmentRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task<ProcessAppointmentDto> UpdateAsync(
        Guid id,
        UpdateProcessAppointmentRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task ConfirmAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task CompleteAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task CancelAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task RescheduleAsync(
        Guid id,
        RescheduleProcessAppointmentRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);
}