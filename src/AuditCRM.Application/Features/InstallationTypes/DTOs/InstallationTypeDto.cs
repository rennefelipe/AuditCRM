namespace AuditCRM.Application.Features.InstallationTypes.DTOs;

public sealed record InstallationTypeDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAt);