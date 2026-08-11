namespace AuditCRM.Application.Features.Users.DTOs;

public sealed record ChangeUserPasswordRequestDto(
    string NewPassword);