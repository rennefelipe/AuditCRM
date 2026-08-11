using AuditCRM.Application.Features.Dashboard.DTOs;
using AuditCRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuditCRM.API.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(
        IDashboardService service)
    {
        _service = service;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary(
        [FromQuery] Guid? shoppingGroupId,
        [FromQuery] Guid? shoppingId,
        [FromQuery] Guid? responsibleUserId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetSummaryAsync(
            shoppingGroupId,
            shoppingId,
            responsibleUserId,
            cancellationToken);

        return Ok(result);
    }
}