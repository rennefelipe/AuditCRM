using AuditCRM.Application.Features.WorkQueue.DTOs;
using AuditCRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuditCRM.API.Controllers;

[ApiController]
[Route("api/work-queue")]
[Authorize]
public sealed class WorkQueueController : ControllerBase
{
    private readonly IWorkQueueService _service;

    public WorkQueueController(
        IWorkQueueService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<WorkQueueItemDto>>> Get(
        [FromQuery] Guid? shoppingGroupId,
        [FromQuery] Guid? shoppingId,
        [FromQuery] Guid? storeId,
        [FromQuery] string? search,
        [FromQuery] Guid? installationTypeId,
        [FromQuery] int? status,
        [FromQuery] Guid? responsibleUserId,
        [FromQuery] int? frequency,
        [FromQuery] int? priority,
        [FromQuery] bool? overdueOnly,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(
            shoppingGroupId,
            shoppingId,
            storeId,
            search,
            installationTypeId,
            status,
            responsibleUserId,
            frequency,
            priority,
            overdueOnly,
            cancellationToken);

        return Ok(result);
    }
}