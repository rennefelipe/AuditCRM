using AuditCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuditCRM.Infrastructure.Persistence.Configurations;

public sealed class ProcessInstallationConfiguration
    : IEntityTypeConfiguration<ProcessInstallation>
{
    public void Configure(
        EntityTypeBuilder<ProcessInstallation> builder)
    {
        builder.ToTable("ProcessInstallations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.InstalledAt)
            .IsRequired();

        builder.Property(x => x.Successful)
            .IsRequired();

        builder.Property(x => x.XmlLocationType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.NumberOfRegisters);

        builder.Property(x => x.Result)
            .HasMaxLength(1000);

        builder.Property(x => x.Notes)
            .HasMaxLength(4000);

        builder.Property(x => x.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasOne(x => x.StoreProcess)
            .WithMany()
            .HasForeignKey(x => x.StoreProcessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ResponsibleUser)
            .WithMany()
            .HasForeignKey(x => x.ResponsibleUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Erp)
            .WithMany()
            .HasForeignKey(x => x.ErpId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.StoreProcessId);

        builder.HasIndex(x => x.ResponsibleUserId);

        builder.HasIndex(x => x.ErpId);

        builder.HasIndex(x => x.InstalledAt);

        builder.HasIndex(x => x.Successful);

        builder.HasIndex(x => x.XmlLocationType);

        builder.HasQueryFilter(
            installation =>
                !installation.IsDeleted &&
                installation.StoreProcess != null &&
                !installation.StoreProcess.IsDeleted);
    }
}