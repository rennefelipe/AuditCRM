using AuditCRM.Domain.Enums;

namespace AuditCRM.Application.Features.ProcessInteractions.DTOs;

public sealed record ProcessInteractionDto(
    Guid Id,
    Guid StoreProcessId,
    Guid? ResponsibleUserId,
    string? ResponsibleUserName,
    Guid? StoreContactId,
    string? StoreContactName,
    InteractionChannel Channel,
    DateTime OccurredAt,
    string Description,
    string? Result,
    string? NextAction,
    DateTime? NextActionAt,
    DateTime CreatedAt);