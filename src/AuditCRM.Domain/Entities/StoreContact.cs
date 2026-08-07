using AuditCRM.Domain.Common;

namespace AuditCRM.Domain.Entities;

public sealed class StoreContact : AuditableEntity
{
    private StoreContact()
    {
    }

    public StoreContact(
        Guid storeId,
        string name,
        string? position = null,
        bool isMainContact = false)
    {
        if (storeId == Guid.Empty)
        {
            throw new ArgumentException(
                "A loja é obrigatória.",
                nameof(storeId));
        }

        StoreId = storeId;
        SetName(name);

        Position = NormalizeOptionalText(position);
        IsMainContact = isMainContact;
        IsActive = true;
    }

    public Guid StoreId { get; private set; }

    public Store? Store { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Position { get; private set; }

    public string? Phone1 { get; private set; }

    public string? Phone2 { get; private set; }

    public string? Phone3 { get; private set; }

    public string? Email1 { get; private set; }

    public string? Email2 { get; private set; }

    public string? Email3 { get; private set; }

    public bool IsMainContact { get; private set; }

    public string? Notes { get; private set; }

    public bool IsActive { get; private set; }

    public void Update(
        string name,
        string? position,
        string? phone1,
        string? phone2,
        string? phone3,
        string? email1,
        string? email2,
        string? email3,
        bool isMainContact,
        string? notes)
    {
        SetName(name);

        Position = NormalizeOptionalText(position);
        Phone1 = NormalizeOptionalText(phone1);
        Phone2 = NormalizeOptionalText(phone2);
        Phone3 = NormalizeOptionalText(phone3);
        Email1 = NormalizeEmail(email1);
        Email2 = NormalizeEmail(email2);
        Email3 = NormalizeEmail(email3);
        IsMainContact = isMainContact;
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
                "O nome do contato é obrigatório.",
                nameof(name));
        }

        var normalizedName = name.Trim();

        if (normalizedName.Length > 150)
        {
            throw new ArgumentException(
                "O nome do contato deve possuir no máximo 150 caracteres.",
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
}