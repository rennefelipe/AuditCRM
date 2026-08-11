using System.Security.Claims;
using AuditCRM.Application.Features.ProcessInteractions.DTOs;
using AuditCRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuditCRM.API.Controllers;

[ApiController]
[Route("api/process-interactions")]
[Authorize]
public sealed class ProcessInteractionsController : ControllerBase
{
    private readonly IProcessInteractionService _service;

    public ProcessInteractionsController(
        IProcessInteractionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProcessInteractionDto>>> GetAll(
        [FromQuery] Guid? storeProcessId,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.GetAllAsync(
            storeProcessId,
            cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProcessInteractionDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(
            id,
            cancellationToken);

        return result is null
            ? NotFound(new { message = "Tratativa não encontrada." })
            : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ProcessInteractionDto>> Create(
        [FromBody] CreateProcessInteractionRequestDto request,
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
    public async Task<ActionResult<ProcessInteractionDto>> Update(
        Guid id,
        [FromBody] UpdateProcessInteractionRequestDto request,
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
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
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