using AuditCRM.Domain.Common;
using AuditCRM.Domain.Enums;

namespace AuditCRM.Domain.Entities;

public sealed class Store : AuditableEntity
{
    private readonly List<StoreContact> _contacts = [];

    private Store()
    {
    }

    public Store(
        Guid shoppingId,
        string tradeName,
        string? luc = null,
        string? document = null)
    {
        if (shoppingId == Guid.Empty)
        {
            throw new ArgumentException(
                "O shopping é obrigatório.",
                nameof(shoppingId));
        }

        ShoppingId = shoppingId;
        SetTradeName(tradeName);

        Luc = NormalizeOptionalText(luc);
        Document = NormalizeDocument(document);
        MonitoringStatus = MonitoringStatus.NotRegistered;
        IsActive = true;
    }

    public Guid ShoppingId { get; private set; }

    public Shopping? Shopping { get; private set; }

    public Guid? ErpId { get; private set; }

    public Erp? Erp { get; private set; }

    public string? Luc { get; private set; }

    public string TradeName { get; private set; } = string.Empty;

    public string? CorporateName { get; private set; }

    public string? Document { get; private set; }

    public string? StateRegistration { get; private set; }

    public int? NumberOfRegisters { get; private set; }

    public MonitoringStatus MonitoringStatus { get; private set; }

    public string? BusinessType { get; private set; }

    public string? Notes { get; private set; }

    public bool IsActive { get; private set; }

    public IReadOnlyCollection<StoreContact> Contacts => _contacts.AsReadOnly();

    public void Update(
        Guid shoppingId,
        Guid? erpId,
        string tradeName,
        string? luc,
        string? corporateName,
        string? document,
        string? stateRegistration,
        int? numberOfRegisters,
        MonitoringStatus monitoringStatus,
        string? businessType,
        string? notes)
    {
        if (shoppingId == Guid.Empty)
        {
            throw new ArgumentException(
                "O shopping é obrigatório.",
                nameof(shoppingId));
        }

        if (numberOfRegisters < 0)
        {
            throw new ArgumentException(
                "A quantidade de caixas não pode ser negativa.",
                nameof(numberOfRegisters));
        }

        ShoppingId = shoppingId;
        ErpId = erpId;

        SetTradeName(tradeName);

        Luc = NormalizeOptionalText(luc);
        CorporateName = NormalizeOptionalText(corporateName);
        Document = NormalizeDocument(document);
        StateRegistration = NormalizeOptionalText(stateRegistration);
        NumberOfRegisters = numberOfRegisters;
        MonitoringStatus = monitoringStatus;
        BusinessType = NormalizeOptionalText(businessType);
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

    private void SetTradeName(string tradeName)
    {
        if (string.IsNullOrWhiteSpace(tradeName))
        {
            throw new ArgumentException(
                "O nome da loja é obrigatório.",
                nameof(tradeName));
        }

        var normalizedName = tradeName.Trim();

        if (normalizedName.Length > 200)
        {
            throw new ArgumentException(
                "O nome da loja deve possuir no máximo 200 caracteres.",
                nameof(tradeName));
        }

        TradeName = normalizedName;
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
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