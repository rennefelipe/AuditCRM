namespace AuditCRM.Infrastructure.Persistence.Seed;

public sealed class InitialAdminSettings
{
    public const string SectionName = "InitialAdmin";

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}