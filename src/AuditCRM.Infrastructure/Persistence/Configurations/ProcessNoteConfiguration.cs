using AuditCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuditCRM.Infrastructure.Persistence.Configurations;

public sealed class ProcessNoteConfiguration
    : IEntityTypeConfiguration<ProcessNote>
{
    public void Configure(
        EntityTypeBuilder<ProcessNote> builder)
    {
        builder.ToTable("ProcessNotes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Text)
            .HasMaxLength(4000)
            .IsRequired();

        builder.HasOne(x => x.StoreProcess)
            .WithMany()
            .HasForeignKey(x => x.StoreProcessId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.AuthorUser)
            .WithMany()
            .HasForeignKey(x => x.AuthorUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.StoreProcessId);

        builder.HasIndex(x => x.AuthorUserId);

        builder.HasIndex(x => x.CreatedAt);

        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}