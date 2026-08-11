namespace AuditCRM.Application.Features.InstallationTypes.DTOs;

public sealed record UpdateInstallationTypeRequestDto(
    string Name,
    string? Description);