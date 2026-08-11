namespace AuditCRM.Application.Features.Timelines.DTOs;

public sealed record TimelineItemDto(
    Guid Id,
    string Type,
    string Title,
    string? Description,
    DateTime OccurredAt,
    Guid? UserId,
    string? UserName);