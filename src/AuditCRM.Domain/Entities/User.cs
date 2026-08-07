using AuditCRM.Domain.Common;

namespace AuditCRM.Domain.Entities;

public sealed class User : AuditableEntity
{
    private User()
    {
    }

    public User(
        string name,
        string email,
        string passwordHash,
        string? phone = null,
        bool isAdministrator = false)
    {
        SetName(name);
        SetEmail(email);
        SetPasswordHash(passwordHash);

        Phone = NormalizeOptionalText(phone);
        IsAdministrator = isAdministrator;
        IsActive = true;
    }

    public string Name { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public string? Phone { get; private set; }

    public bool IsAdministrator { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime? LastLoginAt { get; private set; }

    public void UpdateProfile(
        string name,
        string email,
        string? phone,
        bool isAdministrator)
    {
        SetName(name);
        SetEmail(email);

        Phone = NormalizeOptionalText(phone);
        IsAdministrator = isAdministrator;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePassword(string passwordHash)
    {
        SetPasswordHash(passwordHash);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RegisterLogin()
    {
        LastLoginAt = DateTime.UtcNow;
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
                "O nome do usuário é obrigatório.",
                nameof(name));
        }

        var normalizedName = name.Trim();

        if (normalizedName.Length > 150)
        {
            throw new ArgumentException(
                "O nome do usuário deve possuir no máximo 150 caracteres.",
                nameof(name));
        }

        Name = normalizedName;
    }

    private void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(
                "O e-mail do usuário é obrigatório.",
                nameof(email));
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();

        if (normalizedEmail.Length > 200)
        {
            throw new ArgumentException(
                "O e-mail do usuário deve possuir no máximo 200 caracteres.",
                nameof(email));
        }

        Email = normalizedEmail;
    }

    private void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException(
                "O hash da senha é obrigatório.",
                nameof(passwordHash));
        }

        if (passwordHash.Length > 500)
        {
            throw new ArgumentException(
                "O hash da senha deve possuir no máximo 500 caracteres.",
                nameof(passwordHash));
        }

        PasswordHash = passwordHash;
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}