using AuditCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuditCRM.Infrastructure.Persistence.Configurations;

public sealed class InstallationTypeConfiguration
    : IEntityTypeConfiguration<InstallationType>
{
    public void Configure(EntityTypeBuilder<InstallationType> builder)
    {
        builder.ToTable("InstallationTypes");

        builder.HasKey(type => type.Id);

        builder.Property(type => type.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(type => type.Description)
            .HasMaxLength(2000);

        builder.Property(type => type.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasIndex(type => type.Name)
            .IsUnique();

        builder.HasQueryFilter(type => !type.IsDeleted);
    }
}