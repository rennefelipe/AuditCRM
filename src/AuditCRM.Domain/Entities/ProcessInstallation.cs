using AuditCRM.Domain.Common;
using AuditCRM.Domain.Enums;

namespace AuditCRM.Domain.Entities;

public sealed class ProcessInstallation : AuditableEntity
{
    private ProcessInstallation()
    {
    }

    public ProcessInstallation(
        Guid storeProcessId,
        DateTime installedAt,
        bool successful,
        Guid? responsibleUserId = null,
        Guid? erpId = null,
        XmlLocationType xmlLocationType = XmlLocationType.Unknown)
    {
        if (storeProcessId == Guid.Empty)
        {
            throw new ArgumentException(
                "O processo da loja é obrigatório.",
                nameof(storeProcessId));
        }

        StoreProcessId = storeProcessId;
        InstalledAt = installedAt;
        Successful = successful;
        ResponsibleUserId = responsibleUserId;
        ErpId = erpId;
        XmlLocationType = xmlLocationType;
    }

    public Guid StoreProcessId { get; private set; }

    public StoreProcess? StoreProcess { get; private set; }

    public Guid? ResponsibleUserId { get; private set; }

    public User? ResponsibleUser { get; private set; }

    public Guid? ErpId { get; private set; }

    public Erp? Erp { get; private set; }

    public DateTime InstalledAt { get; private set; }

    public bool Successful { get; private set; }

    public XmlLocationType XmlLocationType { get; private set; }

    public int? NumberOfRegisters { get; private set; }

    public string? Result { get; private set; }

    public string? Notes { get; private set; }

    public void Update(
        DateTime installedAt,
        bool successful,
        Guid? responsibleUserId,
        Guid? erpId,
        XmlLocationType xmlLocationType,
        int? numberOfRegisters,
        string? result,
        string? notes)
    {
        if (numberOfRegisters < 0)
        {
            throw new ArgumentException(
                "A quantidade de caixas não pode ser negativa.",
                nameof(numberOfRegisters));
        }

        InstalledAt = installedAt;
        Successful = successful;
        ResponsibleUserId = responsibleUserId;
        ErpId = erpId;
        XmlLocationType = xmlLocationType;
        NumberOfRegisters = numberOfRegisters;
        Result = NormalizeOptionalText(result);
        Notes = NormalizeOptionalText(notes);

        UpdatedAt = DateTime.UtcNow;
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}