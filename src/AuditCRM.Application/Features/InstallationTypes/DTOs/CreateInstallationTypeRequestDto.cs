namespace AuditCRM.Application.Features.InstallationTypes.DTOs;

public sealed record CreateInstallationTypeRequestDto(
    string Name,
    string? Description);