using AuditCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuditCRM.Infrastructure.Persistence.Configurations;

public sealed class ProcessDocumentConfiguration
    : IEntityTypeConfiguration<ProcessDocument>
{
    public void Configure(
        EntityTypeBuilder<ProcessDocument> builder)
    {
        builder.ToTable("ProcessDocuments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OriginalFileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.StoredFileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.RelativePath)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.ContentType)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.FileSize)
            .IsRequired();

        builder.HasOne(x => x.StoreProcess)
            .WithMany()
            .HasForeignKey(x => x.StoreProcessId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.UploadedByUser)
            .WithMany()
            .HasForeignKey(x => x.UploadedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.StoreProcessId);

        builder.HasIndex(x => x.UploadedByUserId);

        builder.HasIndex(x => x.CreatedAt);

        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}