using AuditCRM.Domain.Common;

namespace AuditCRM.Domain.Entities;

public sealed class Shopping : AuditableEntity
{
    private readonly List<Store> _stores = [];

    private Shopping()
    {
    }

    public Shopping(
        Guid shoppingGroupId,
        string name,
        bool paysInstallation)
    {
        if (shoppingGroupId == Guid.Empty)
        {
            throw new ArgumentException(
                "A rede de shopping é obrigatória.",
                nameof(shoppingGroupId));
        }

        ShoppingGroupId = shoppingGroupId;
        SetName(name);
        PaysInstallation = paysInstallation;
        IsActive = true;
    }

    public Guid ShoppingGroupId { get; private set; }

    public ShoppingGroup? ShoppingGroup { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? CorporateName { get; private set; }

    public string? Document { get; private set; }

    public string? Address { get; private set; }

    public string? Number { get; private set; }

    public string? District { get; private set; }

    public string? City { get; private set; }

    public string? State { get; private set; }

    public string? ZipCode { get; private set; }

    public string? ContactName { get; private set; }

    public string? ContactEmail { get; private set; }

    public string? ContactPhone { get; private set; }

    public bool PaysInstallation { get; private set; }

    public string? Notes { get; private set; }

    public bool IsActive { get; private set; }

    public IReadOnlyCollection<Store> Stores => _stores.AsReadOnly();

    public void Update(
        Guid shoppingGroupId,
        string name,
        string? corporateName,
        string? document,
        string? address,
        string? number,
        string? district,
        string? city,
        string? state,
        string? zipCode,
        string? contactName,
        string? contactEmail,
        string? contactPhone,
        bool paysInstallation,
        string? notes)
    {
        if (shoppingGroupId == Guid.Empty)
        {
            throw new ArgumentException(
                "A rede de shopping é obrigatória.",
                nameof(shoppingGroupId));
        }

        ShoppingGroupId = shoppingGroupId;
        SetName(name);

        CorporateName = NormalizeOptionalText(corporateName);
        Document = NormalizeDocument(document);
        Address = NormalizeOptionalText(address);
        Number = NormalizeOptionalText(number);
        District = NormalizeOptionalText(district);
        City = NormalizeOptionalText(city);
        State = NormalizeState(state);
        ZipCode = NormalizeDocument(zipCode);
        ContactName = NormalizeOptionalText(contactName);
        ContactEmail = NormalizeEmail(contactEmail);
        ContactPhone = NormalizeOptionalText(contactPhone);
        PaysInstallation = paysInstallation;
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
                "O nome do shopping é obrigatório.",
                nameof(name));
        }

        var normalizedName = name.Trim();

        if (normalizedName.Length > 180)
        {
            throw new ArgumentException(
                "O nome do shopping deve possuir no máximo 180 caracteres.",
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

    private static string? NormalizeEmail(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim().ToLowerInvariant();
    }

    private static string? NormalizeState(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim().ToUpperInvariant();
    }

    private static string? NormalizeDocument(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return new string(value.Where(char.IsDigit).ToArray());
    }
}