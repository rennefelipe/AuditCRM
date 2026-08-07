using AuditCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuditCRM.Infrastructure.Persistence.Configurations;

public sealed class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.ToTable("Stores");

        builder.HasKey(store => store.Id);

        builder.Property(store => store.Luc)
            .HasMaxLength(50);

        builder.Property(store => store.TradeName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(store => store.CorporateName)
            .HasMaxLength(250);

        builder.Property(store => store.Document)
            .HasMaxLength(20);

        builder.Property(store => store.StateRegistration)
            .HasMaxLength(30);

        builder.Property(store => store.BusinessType)
            .HasMaxLength(100);

        builder.Property(store => store.Notes)
            .HasMaxLength(4000);

        builder.Property(store => store.MonitoringStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(store => store.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasOne(store => store.Shopping)
            .WithMany(shopping => shopping.Stores)
            .HasForeignKey(store => store.ShoppingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(store => store.Erp)
            .WithMany()
            .HasForeignKey(store => store.ErpId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(store => store.Contacts)
            .WithOne(contact => contact.Store)
            .HasForeignKey(contact => contact.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(store => new
        {
            store.ShoppingId,
            store.Document
        });

        builder.HasIndex(store => new
        {
            store.ShoppingId,
            store.Luc
        });

        builder.HasIndex(store => store.TradeName);

        builder.HasQueryFilter(store => !store.IsDeleted);
    }
}