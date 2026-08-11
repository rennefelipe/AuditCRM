using AuditCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuditCRM.Infrastructure.Persistence.Configurations;

public sealed class StoreProcessConfiguration
    : IEntityTypeConfiguration<StoreProcess>
{
    public void Configure(EntityTypeBuilder<StoreProcess> builder)
    {
        builder.ToTable("StoreProcesses");

        builder.HasKey(process => process.Id);

        builder.Property(process => process.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(process => process.Priority)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(process => process.NextAction)
            .HasMaxLength(500);

        builder.Property(process => process.Notes)
            .HasMaxLength(4000);

        builder.Property(process => process.StartedAt)
            .IsRequired();

        builder.Property(process => process.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasOne(process => process.Store)
            .WithMany()
            .HasForeignKey(process => process.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(process => process.InstallationType)
            .WithMany()
            .HasForeignKey(process => process.InstallationTypeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(process => process.ResponsibleUser)
            .WithMany()
            .HasForeignKey(process => process.ResponsibleUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(process => process.StoreId);

        builder.HasIndex(process => process.ResponsibleUserId);

        builder.HasIndex(process => process.Status);

        builder.HasIndex(process => process.NextActionAt);

        builder.HasQueryFilter(process => !process.IsDeleted);
    }
}