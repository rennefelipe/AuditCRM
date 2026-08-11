using System.Security.Claims;
using AuditCRM.Application.Features.StoreProcesses.DTOs;
using AuditCRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuditCRM.API.Controllers;

[ApiController]
[Route("api/store-processes")]
[Authorize]
public sealed class StoreProcessesController : ControllerBase
{
    private readonly IStoreProcessService _service;
    private readonly IProcessTimelineService _timelineService;

    public StoreProcessesController(
        IStoreProcessService service,
        IProcessTimelineService timelineService)
    {
        _service = service;
        _timelineService = timelineService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StoreProcessDto>>> GetAll(
        [FromQuery] Guid? storeId,
        [FromQuery] Guid? shoppingId,
        [FromQuery] Guid? responsibleUserId,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.GetAllAsync(
            storeId,
            shoppingId,
            responsibleUserId,
            cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StoreProcessDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var process = await _service.GetByIdAsync(
            id,
            cancellationToken);

        return process is null
            ? NotFound(new
            {
                message = "Processo não encontrado."
            })
            : Ok(process);
    }

    [HttpGet("{id:guid}/timeline")]
    public async Task<IActionResult> GetTimeline(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var timeline = await _timelineService.GetAsync(
                id,
                cancellationToken);

            return Ok(timeline);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<StoreProcessDto>> Create(
        [FromBody] CreateStoreProcessRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var process = await _service.CreateAsync(
                request,
                GetCurrentUserId(),
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = process.Id },
                process);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<StoreProcessDto>> Update(
        Guid id,
        [FromBody] UpdateStoreProcessRequestDto request,
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
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPatch("{id:guid}/close")]
    public async Task<IActionResult> Close(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _service.CloseAsync(
                id,
                GetCurrentUserId(),
                cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPatch("{id:guid}/reopen")]
    public async Task<IActionResult> Reopen(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _service.ReopenAsync(
                id,
                GetCurrentUserId(),
                cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
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
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    private Guid? GetCurrentUserId()
    {
        var value = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        return Guid.TryParse(
            value,
            out var userId)
            ? userId
            : null;
    }
}