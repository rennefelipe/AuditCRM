using AuditCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuditCRM.Infrastructure.Persistence.Configurations;

public sealed class ErpConfiguration : IEntityTypeConfiguration<Erp>
{
    public void Configure(EntityTypeBuilder<Erp> builder)
    {
        builder.ToTable("Erps");

        builder.HasKey(erp => erp.Id);

        builder.Property(erp => erp.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(erp => erp.Manufacturer)
            .HasMaxLength(150);

        builder.Property(erp => erp.Notes)
            .HasMaxLength(2000);

        builder.Property(erp => erp.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasIndex(erp => erp.Name)
            .IsUnique();

        builder.HasQueryFilter(erp => !erp.IsDeleted);
    }
}