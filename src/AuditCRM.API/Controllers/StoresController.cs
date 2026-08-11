using System.Security.Claims;
using AuditCRM.Application.Features.Stores.DTOs;
using AuditCRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuditCRM.API.Controllers;

[ApiController]
[Route("api/stores")]
[Authorize]
public sealed class StoresController : ControllerBase
{
    private readonly IStoreService _storeService;

    public StoresController(IStoreService storeService)
    {
        _storeService = storeService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StoreDto>>> GetAll(
        [FromQuery] Guid? shoppingId,
        [FromQuery] Guid? shoppingGroupId,
        CancellationToken cancellationToken)
    {
        var stores = await _storeService.GetAllAsync(
            shoppingId,
            shoppingGroupId,
            cancellationToken);

        return Ok(stores);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StoreDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var store = await _storeService.GetByIdAsync(
            id,
            cancellationToken);

        if (store is null)
        {
            return NotFound(new
            {
                message = "Loja não encontrada."
            });
        }

        return Ok(store);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<ActionResult<StoreDto>> Create(
        [FromBody] CreateStoreRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var store = await _storeService.CreateAsync(
                request,
                GetCurrentUserId(),
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = store.Id },
                store);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
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

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<StoreDto>> Update(
        Guid id,
        [FromBody] UpdateStoreRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var store = await _storeService.UpdateAsync(
                id,
                request,
                GetCurrentUserId(),
                cancellationToken);

            return Ok(store);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
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
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    [Authorize(Roles = "Administrator")]
    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult> Activate(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _storeService.ActivateAsync(
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
    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _storeService.DeactivateAsync(
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
            await _storeService.DeleteAsync(
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
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
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