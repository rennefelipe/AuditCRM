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
        [FromQuery] Guid? responsibleUserId,
        [FromQuery] bool? overdueOnly,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(
            shoppingGroupId,
            shoppingId,
            responsibleUserId,
            overdueOnly,
            cancellationToken);

        return Ok(result);
    }
}