using AuditCRM.Application.Features.ProcessAppointments.DTOs;
using AuditCRM.Application.Interfaces;
using AuditCRM.Domain.Entities;
using AuditCRM.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuditCRM.Infrastructure.Services;

public sealed class ProcessAppointmentService
    : IProcessAppointmentService
{
    private readonly AuditDbContext _dbContext;

    public ProcessAppointmentService(
        AuditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ProcessAppointmentDto>> GetAllAsync(
        Guid? storeProcessId = null,
        Guid? responsibleUserId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ProcessAppointments
            .AsNoTracking()
            .Include(x => x.ResponsibleUser)
            .Include(x => x.StoreContact)
            .AsQueryable();

        if (storeProcessId.HasValue)
        {
            query = query.Where(
                x => x.StoreProcessId == storeProcessId.Value);
        }

        if (responsibleUserId.HasValue)
        {
            query = query.Where(
                x => x.ResponsibleUserId == responsibleUserId.Value);
        }

        return await query
            .OrderBy(x => x.ScheduledAt)
            .Select(x => new ProcessAppointmentDto(
                x.Id,
                x.StoreProcessId,
                x.ResponsibleUserId,
                x.ResponsibleUser != null
                    ? x.ResponsibleUser.Name
                    : null,
                x.StoreContactId,
                x.StoreContact != null
                    ? x.StoreContact.Name
                    : null,
                x.ScheduledAt,
                x.Status,
                x.Notes,
                x.CompletedAt,
                x.CancelledAt,
                x.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<ProcessAppointmentDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProcessAppointments
            .AsNoTracking()
            .Include(x => x.ResponsibleUser)
            .Include(x => x.StoreContact)
            .Where(x => x.Id == id)
            .Select(x => new ProcessAppointmentDto(
                x.Id,
                x.StoreProcessId,
                x.ResponsibleUserId,
                x.ResponsibleUser != null
                    ? x.ResponsibleUser.Name
                    : null,
                x.StoreContactId,
                x.StoreContact != null
                    ? x.StoreContact.Name
                    : null,
                x.ScheduledAt,
                x.Status,
                x.Notes,
                x.CompletedAt,
                x.CancelledAt,
                x.CreatedAt))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<ProcessAppointmentDto> CreateAsync(
        CreateProcessAppointmentRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        await ValidateProcessAsync(
            request.StoreProcessId,
            cancellationToken);

        await ValidateResponsibleUserAsync(
            request.ResponsibleUserId,
            cancellationToken);

        await ValidateStoreContactAsync(
            request.StoreProcessId,
            request.StoreContactId,
            cancellationToken);

        var appointment = new ProcessAppointment(
            request.StoreProcessId,
            request.ScheduledAt,
            request.ResponsibleUserId,
            request.StoreContactId,
            request.Notes);

        appointment.MarkCreatedBy(currentUserId);

        _dbContext.ProcessAppointments.Add(appointment);

        await UpdateProcessNextActionAsync(
            request.StoreProcessId,
            "Agendamento de implantação",
            request.ScheduledAt,
            currentUserId,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetRequiredDtoAsync(
            appointment.Id,
            cancellationToken);
    }

    public async Task<ProcessAppointmentDto> UpdateAsync(
        Guid id,
        UpdateProcessAppointmentRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var appointment = await FindRequiredAsync(
            id,
            cancellationToken);

        await ValidateResponsibleUserAsync(
            request.ResponsibleUserId,
            cancellationToken);

        await ValidateStoreContactAsync(
            appointment.StoreProcessId,
            request.StoreContactId,
            cancellationToken);

        appointment.Update(
            request.ScheduledAt,
            request.ResponsibleUserId,
            request.StoreContactId,
            request.Notes);

        appointment.MarkUpdatedBy(currentUserId);

        await UpdateProcessNextActionAsync(
            appointment.StoreProcessId,
            "Agendamento de implantação",
            request.ScheduledAt,
            currentUserId,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetRequiredDtoAsync(
            appointment.Id,
            cancellationToken);
    }

    public async Task ConfirmAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var appointment = await FindRequiredAsync(
            id,
            cancellationToken);

        appointment.Confirm();
        appointment.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CompleteAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var appointment = await FindRequiredAsync(
            id,
            cancellationToken);

        appointment.Complete();
        appointment.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CancelAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var appointment = await FindRequiredAsync(
            id,
            cancellationToken);

        appointment.Cancel();
        appointment.MarkUpdatedBy(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RescheduleAsync(
        Guid id,
        RescheduleProcessAppointmentRequestDto request,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var appointment = await FindRequiredAsync(
            id,
            cancellationToken);

        appointment.Reschedule(
            request.ScheduledAt);

        appointment.MarkUpdatedBy(currentUserId);

        await UpdateProcessNextActionAsync(
            appointment.StoreProcessId,
            "Agendamento de implantação",
            request.ScheduledAt,
            currentUserId,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var appointment = await FindRequiredAsync(
            id,
            cancellationToken);

        appointment.Delete(currentUserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateProcessAsync(
        Guid storeProcessId,
        CancellationToken cancellationToken)
    {
        var exists = await _dbContext.StoreProcesses
            .AnyAsync(
                x => x.Id == storeProcessId,
                cancellationToken);

        if (!exists)
        {
            throw new InvalidOperationException(
                "O processo informado não existe.");
        }
    }

    private async Task ValidateResponsibleUserAsync(
        Guid? responsibleUserId,
        CancellationToken cancellationToken)
    {
        if (!responsibleUserId.HasValue)
            return;

        var exists = await _dbContext.Users
            .AnyAsync(
                x =>
                    x.Id == responsibleUserId.Value &&
                    x.IsActive,
                cancellationToken);

        if (!exists)
        {
            throw new InvalidOperationException(
                "O usuário responsável informado não existe ou está inativo.");
        }
    }

    private async Task ValidateStoreContactAsync(
        Guid storeProcessId,
        Guid? storeContactId,
        CancellationToken cancellationToken)
    {
        if (!storeContactId.HasValue)
            return;

        var storeId = await _dbContext.StoreProcesses
            .Where(x => x.Id == storeProcessId)
            .Select(x => x.StoreId)
            .SingleOrDefaultAsync(cancellationToken);

        var exists = await _dbContext.StoreContacts
            .AnyAsync(
                x =>
                    x.Id == storeContactId.Value &&
                    x.StoreId == storeId &&
                    x.IsActive,
                cancellationToken);

        if (!exists)
        {
            throw new InvalidOperationException(
                "O contato informado não pertence à loja deste processo ou está inativo.");
        }
    }

    private async Task UpdateProcessNextActionAsync(
        Guid storeProcessId,
        string nextAction,
        DateTime nextActionAt,
        Guid? currentUserId,
        CancellationToken cancellationToken)
    {
        var process = await _dbContext.StoreProcesses
            .SingleAsync(
                x => x.Id == storeProcessId,
                cancellationToken);

        process.SetNextAction(
            nextAction,
            nextActionAt);

        process.MarkUpdatedBy(currentUserId);
    }

    private async Task<ProcessAppointment> FindRequiredAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var appointment = await _dbContext.ProcessAppointments
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (appointment is null)
        {
            throw new KeyNotFoundException(
                "Agendamento não encontrado.");
        }

        return appointment;
    }

    private async Task<ProcessAppointmentDto> GetRequiredDtoAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var appointment = await GetByIdAsync(
            id,
            cancellationToken);

        if (appointment is null)
        {
            throw new KeyNotFoundException(
                "Agendamento não encontrado.");
        }

        return appointment;
    }
}