using AuditCRM.Domain.Common;

namespace AuditCRM.Domain.Entities;

public sealed class ShoppingGroup : AuditableEntity
{
    private readonly List<Shopping> _shoppings = [];

    private ShoppingGroup()
    {
    }

    public ShoppingGroup(
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

    public IReadOnlyCollection<Shopping> Shoppings => _shoppings.AsReadOnly();

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
                "O nome da rede de shopping é obrigatório.",
                nameof(name));
        }

        var normalizedName = name.Trim();

        if (normalizedName.Length > 180)
        {
            throw new ArgumentException(
                "O nome da rede deve possuir no máximo 180 caracteres.",
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