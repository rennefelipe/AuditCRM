using AuditCRM.Domain.Common;

namespace AuditCRM.Domain.Entities;

public sealed class Erp : AuditableEntity
{
    private Erp()
    {
    }

    public Erp(
        string name,
        string? manufacturer = null,
        string? notes = null)
    {
        SetName(name);
        Manufacturer = NormalizeOptionalText(manufacturer);
        Notes = NormalizeOptionalText(notes);
        IsActive = true;
    }

    public string Name { get; private set; } = string.Empty;

    public string? Manufacturer { get; private set; }

    public string? Notes { get; private set; }

    public bool IsActive { get; private set; }

    public void Update(
        string name,
        string? manufacturer,
        string? notes)
    {
        SetName(name);
        Manufacturer = NormalizeOptionalText(manufacturer);
        Notes = NormalizeOptionalText(notes);
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
                "O nome do ERP é obrigatório.",
                nameof(name));
        }

        var normalizedName = name.Trim();

        if (normalizedName.Length > 150)
        {
            throw new ArgumentException(
                "O nome do ERP deve possuir no máximo 150 caracteres.",
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