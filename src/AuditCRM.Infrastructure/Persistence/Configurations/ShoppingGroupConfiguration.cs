using AuditCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuditCRM.Infrastructure.Persistence.Configurations;

public sealed class ShoppingGroupConfiguration
    : IEntityTypeConfiguration<ShoppingGroup>
{
    public void Configure(EntityTypeBuilder<ShoppingGroup> builder)
    {
        builder.ToTable("ShoppingGroups");

        builder.HasKey(group => group.Id);

        builder.Property(group => group.Name)
            .HasMaxLength(180)
            .IsRequired();

        builder.Property(group => group.Description)
            .HasMaxLength(1000);

        builder.Property(group => group.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasIndex(group => group.Name)
            .IsUnique();

        builder.HasMany(group => group.Shoppings)
            .WithOne(shopping => shopping.ShoppingGroup)
            .HasForeignKey(shopping => shopping.ShoppingGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(group => !group.IsDeleted);
    }
}