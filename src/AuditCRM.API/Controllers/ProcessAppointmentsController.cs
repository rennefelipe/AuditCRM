using System.Security.Claims;
using AuditCRM.Application.Features.ProcessAppointments.DTOs;
using AuditCRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuditCRM.API.Controllers;

[ApiController]
[Route("api/process-appointments")]
[Authorize]
public sealed class ProcessAppointmentsController : ControllerBase
{
    private readonly IProcessAppointmentService _service;

    public ProcessAppointmentsController(
        IProcessAppointmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProcessAppointmentDto>>> GetAll(
        [FromQuery] Guid? storeProcessId,
        [FromQuery] Guid? responsibleUserId,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.GetAllAsync(
            storeProcessId,
            responsibleUserId,
            cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProcessAppointmentDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(
            id,
            cancellationToken);

        return result is null
            ? NotFound(new { message = "Agendamento não encontrado." })
            : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ProcessAppointmentDto>> Create(
        [FromBody] CreateProcessAppointmentRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.CreateAsync(
                request,
                GetCurrentUserId(),
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProcessAppointmentDto>> Update(
        Guid id,
        [FromBody] UpdateProcessAppointmentRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _service.UpdateAsync(
                id,
                request,
                GetCurrentUserId(),
                cancellationToken));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _service.ConfirmAsync(
                id,
                GetCurrentUserId(),
                cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/complete")]
    public async Task<IActionResult> Complete(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _service.CompleteAsync(
                id,
                GetCurrentUserId(),
                cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _service.CancelAsync(
                id,
                GetCurrentUserId(),
                cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/reschedule")]
    public async Task<IActionResult> Reschedule(
        Guid id,
        [FromBody] RescheduleProcessAppointmentRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _service.RescheduleAsync(
                id,
                request,
                GetCurrentUserId(),
                cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _service.DeleteAsync(
                id,
                GetCurrentUserId(),
                cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    private Guid? GetCurrentUserId()
    {
        var value = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        return Guid.TryParse(value, out var userId)
            ? userId
            : null;
    }
}