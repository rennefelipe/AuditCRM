using AuditCRM.Domain.Enums;

namespace AuditCRM.Application.Features.ProcessInteractions.DTOs;

public sealed record UpdateProcessInteractionRequestDto(
    Guid? ResponsibleUserId,
    Guid? StoreContactId,
    InteractionChannel Channel,
    DateTime OccurredAt,
    string Description,
    string? Result,
    string? NextAction,
    DateTime? NextActionAt);