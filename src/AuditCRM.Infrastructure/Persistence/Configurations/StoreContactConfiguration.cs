using AuditCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuditCRM.Infrastructure.Persistence.Configurations;

public sealed class StoreContactConfiguration
    : IEntityTypeConfiguration<StoreContact>
{
    public void Configure(EntityTypeBuilder<StoreContact> builder)
    {
        builder.ToTable("StoreContacts");

        builder.HasKey(contact => contact.Id);

        builder.Property(contact => contact.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(contact => contact.Position)
            .HasMaxLength(100);

        builder.Property(contact => contact.Phone1)
            .HasMaxLength(30);

        builder.Property(contact => contact.Phone2)
            .HasMaxLength(30);

        builder.Property(contact => contact.Phone3)
            .HasMaxLength(30);

        builder.Property(contact => contact.Email1)
            .HasMaxLength(200);

        builder.Property(contact => contact.Email2)
            .HasMaxLength(200);

        builder.Property(contact => contact.Email3)
            .HasMaxLength(200);

        builder.Property(contact => contact.Notes)
            .HasMaxLength(2000);

        builder.Property(contact => contact.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasIndex(contact => contact.StoreId);

        builder.HasQueryFilter(contact => !contact.IsDeleted);
    }
}