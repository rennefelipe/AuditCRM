using System.Security.Claims;
using AuditCRM.Application.Features.ProcessInstallations.DTOs;
using AuditCRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuditCRM.API.Controllers;

[ApiController]
[Route("api/process-installations")]
[Authorize]
public sealed class ProcessInstallationsController : ControllerBase
{
    private readonly IProcessInstallationService _service;

    public ProcessInstallationsController(
        IProcessInstallationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProcessInstallationDto>>> GetAll(
        [FromQuery] Guid? storeProcessId,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.GetAllAsync(
            storeProcessId,
            cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProcessInstallationDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(
            id,
            cancellationToken);

        return result is null
            ? NotFound(new { message = "Instalação não encontrada." })
            : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ProcessInstallationDto>> Create(
        [FromBody] CreateProcessInstallationRequestDto request,
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
    public async Task<ActionResult<ProcessInstallationDto>> Update(
        Guid id,
        [FromBody] UpdateProcessInstallationRequestDto request,
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