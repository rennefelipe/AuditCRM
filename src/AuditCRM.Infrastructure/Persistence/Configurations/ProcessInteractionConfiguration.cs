using AuditCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuditCRM.Infrastructure.Persistence.Configurations;

public sealed class ProcessInteractionConfiguration
    : IEntityTypeConfiguration<ProcessInteraction>
{
    public void Configure(EntityTypeBuilder<ProcessInteraction> builder)
    {
        builder.ToTable("ProcessInteractions");

        builder.HasKey(interaction => interaction.Id);

        builder.Property(interaction => interaction.Channel)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(interaction => interaction.OccurredAt)
            .IsRequired();

        builder.Property(interaction => interaction.Description)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(interaction => interaction.Result)
            .HasMaxLength(1000);

        builder.Property(interaction => interaction.NextAction)
            .HasMaxLength(500);

        builder.Property(interaction => interaction.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasOne(interaction => interaction.StoreProcess)
            .WithMany()
            .HasForeignKey(interaction => interaction.StoreProcessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(interaction => interaction.ResponsibleUser)
            .WithMany()
            .HasForeignKey(interaction => interaction.ResponsibleUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(interaction => interaction.StoreContact)
            .WithMany()
            .HasForeignKey(interaction => interaction.StoreContactId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(interaction => interaction.StoreProcessId);

        builder.HasIndex(interaction => interaction.ResponsibleUserId);

        builder.HasIndex(interaction => interaction.StoreContactId);

        builder.HasIndex(interaction => interaction.OccurredAt);

        builder.HasIndex(interaction => interaction.Channel);

        builder.HasQueryFilter(interaction => !interaction.IsDeleted);
    }
}