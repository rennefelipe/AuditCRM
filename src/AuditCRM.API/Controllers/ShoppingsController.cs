using System.Security.Claims;
using AuditCRM.Application.Features.Shoppings.DTOs;
using AuditCRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuditCRM.API.Controllers;

[ApiController]
[Route("api/shoppings")]
[Authorize]
public sealed class ShoppingsController : ControllerBase
{
    private readonly IShoppingService _shoppingService;

    public ShoppingsController(
        IShoppingService shoppingService)
    {
        _shoppingService = shoppingService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ShoppingDto>>> GetAll(
        [FromQuery] Guid? shoppingGroupId,
        CancellationToken cancellationToken)
    {
        var shoppings = await _shoppingService.GetAllAsync(
            shoppingGroupId,
            cancellationToken);

        return Ok(shoppings);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ShoppingDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var shopping = await _shoppingService.GetByIdAsync(
            id,
            cancellationToken);

        if (shopping is null)
        {
            return NotFound(new
            {
                message = "Shopping não encontrado."
            });
        }

        return Ok(shopping);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<ActionResult<ShoppingDto>> Create(
        [FromBody] CreateShoppingRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var shopping = await _shoppingService.CreateAsync(
                request,
                GetCurrentUserId(),
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = shopping.Id },
                shopping);
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
    public async Task<ActionResult<ShoppingDto>> Update(
        Guid id,
        [FromBody] UpdateShoppingRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var shopping = await _shoppingService.UpdateAsync(
                id,
                request,
                GetCurrentUserId(),
                cancellationToken);

            return Ok(shopping);
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
            await _shoppingService.ActivateAsync(
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
            await _shoppingService.DeactivateAsync(
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
            await _shoppingService.DeleteAsync(
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