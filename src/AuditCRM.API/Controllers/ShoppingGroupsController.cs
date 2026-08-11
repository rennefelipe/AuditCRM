using System.Security.Claims;
using AuditCRM.Application.Features.ShoppingGroups.DTOs;
using AuditCRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuditCRM.API.Controllers;

[ApiController]
[Route("api/shopping-groups")]
[Authorize]
public sealed class ShoppingGroupsController : ControllerBase
{
    private readonly IShoppingGroupService _shoppingGroupService;

    public ShoppingGroupsController(
        IShoppingGroupService shoppingGroupService)
    {
        _shoppingGroupService = shoppingGroupService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ShoppingGroupDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var groups = await _shoppingGroupService.GetAllAsync(
            cancellationToken);

        return Ok(groups);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ShoppingGroupDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var group = await _shoppingGroupService.GetByIdAsync(
            id,
            cancellationToken);

        if (group is null)
        {
            return NotFound(new
            {
                message = "Rede de shopping não encontrada."
            });
        }

        return Ok(group);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<ActionResult<ShoppingGroupDto>> Create(
        [FromBody] CreateShoppingGroupRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var group = await _shoppingGroupService.CreateAsync(
                request,
                GetCurrentUserId(),
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = group.Id },
                group);
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
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ShoppingGroupDto>> Update(
        Guid id,
        [FromBody] UpdateShoppingGroupRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var group = await _shoppingGroupService.UpdateAsync(
                id,
                request,
                GetCurrentUserId(),
                cancellationToken);

            return Ok(group);
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
    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult> Activate(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _shoppingGroupService.ActivateAsync(
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

    [Authorize(Roles = "Administrator")]
    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _shoppingGroupService.DeactivateAsync(
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

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _shoppingGroupService.DeleteAsync(
                id,
                GetCurrentUserId(),
                cancellationToken);

            return NoContent();
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

    private Guid? GetCurrentUserId()
    {
        var value = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        return Guid.TryParse(value, out var userId)
            ? userId
            : null;
    }
}