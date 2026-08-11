namespace AuditCRM.Domain.Common;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

    public Guid? CreatedByUserId { get; protected set; }

    public DateTime? UpdatedAt { get; protected set; }

    public Guid? UpdatedByUserId { get; protected set; }

    public bool IsDeleted { get; protected set; }

    public DateTime? DeletedAt { get; protected set; }

    public Guid? DeletedByUserId { get; protected set; }

    public byte[] RowVersion { get; protected set; } = [];

    public void MarkCreatedBy(Guid? userId)
    {
        CreatedByUserId = userId;
    }

    public void MarkUpdatedBy(Guid? userId)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedByUserId = userId;
    }

    public void Delete(Guid? userId)
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedByUserId = userId;
    }
}