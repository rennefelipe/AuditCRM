using AuditCRM.Domain.Common;

namespace AuditCRM.Domain.Entities;

public sealed class InstallationType : AuditableEntity
{
    private InstallationType()
    {
    }

    public InstallationType(
        string name,
        string? description = null)
    {
        SetName(name);
        Description = NormalizeOptionalText(description);
        IsActive = true;
    }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public bool IsActive { get; private set; }

    public void Update(
        string name,
        string? description)
    {
        SetName(name);
        Description = NormalizeOptionalText(description);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "O nome do tipo de instalação é obrigatório.",
                nameof(name));
        }

        var normalizedName = name.Trim();

        if (normalizedName.Length > 150)
        {
            throw new ArgumentException(
                "O nome do tipo de instalação deve possuir no máximo 150 caracteres.",
                nameof(name));
        }

        Name = normalizedName;
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}