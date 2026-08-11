using AuditCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuditCRM.Infrastructure.Persistence.Configurations;

public sealed class ProcessAppointmentConfiguration
    : IEntityTypeConfiguration<ProcessAppointment>
{
    public void Configure(EntityTypeBuilder<ProcessAppointment> builder)
    {
        builder.ToTable("ProcessAppointments");

        builder.HasKey(appointment => appointment.Id);

        builder.Property(appointment => appointment.ScheduledAt)
            .IsRequired();

        builder.Property(appointment => appointment.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(appointment => appointment.Notes)
            .HasMaxLength(4000);

        builder.Property(appointment => appointment.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasOne(appointment => appointment.StoreProcess)
            .WithMany()
            .HasForeignKey(appointment => appointment.StoreProcessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(appointment => appointment.ResponsibleUser)
            .WithMany()
            .HasForeignKey(appointment => appointment.ResponsibleUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(appointment => appointment.StoreContact)
            .WithMany()
            .HasForeignKey(appointment => appointment.StoreContactId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(appointment => appointment.StoreProcessId);

        builder.HasIndex(appointment => appointment.ScheduledAt);

        builder.HasIndex(appointment => appointment.Status);

        builder.HasIndex(appointment => appointment.ResponsibleUserId);

        builder.HasQueryFilter(
            appointment => !appointment.IsDeleted);
    }
}