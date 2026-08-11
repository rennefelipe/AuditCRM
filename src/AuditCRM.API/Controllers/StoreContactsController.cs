using System.Security.Claims;
using AuditCRM.Application.Features.StoreContacts.DTOs;
using AuditCRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuditCRM.API.Controllers;

[ApiController]
[Route("api/store-contacts")]
[Authorize]
public sealed class StoreContactsController : ControllerBase
{
    private readonly IStoreContactService _storeContactService;

    public StoreContactsController(
        IStoreContactService storeContactService)
    {
        _storeContactService = storeContactService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StoreContactDto>>> GetAll(
        [FromQuery] Guid? storeId,
        CancellationToken cancellationToken)
    {
        var contacts = await _storeContactService.GetAllAsync(
            storeId,
            cancellationToken);

        return Ok(contacts);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StoreContactDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var contact = await _storeContactService.GetByIdAsync(
            id,
            cancellationToken);

        if (contact is null)
        {
            return NotFound(new
            {
                message = "Contato não encontrado."
            });
        }

        return Ok(contact);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<ActionResult<StoreContactDto>> Create(
        [FromBody] CreateStoreContactRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _storeContactService.CreateAsync(
                request,
                GetCurrentUserId(),
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = contact.Id },
                contact);
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
    public async Task<ActionResult<StoreContactDto>> Update(
        Guid id,
        [FromBody] UpdateStoreContactRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _storeContactService.UpdateAsync(
                id,
                request,
                GetCurrentUserId(),
                cancellationToken);

            return Ok(contact);
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
    }

    [Authorize(Roles = "Administrator")]
    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult> Activate(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _storeContactService.ActivateAsync(
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
            await _storeContactService.DeactivateAsync(
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
            await _storeContactService.DeleteAsync(
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

        return Guid.TryParse(value, out var userId)
            ? userId
            : null;
    }
}