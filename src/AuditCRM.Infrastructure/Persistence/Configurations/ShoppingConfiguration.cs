using AuditCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuditCRM.Infrastructure.Persistence.Configurations;

public sealed class ShoppingConfiguration
    : IEntityTypeConfiguration<Shopping>
{
    public void Configure(
        EntityTypeBuilder<Shopping> builder)
    {
        builder.ToTable("Shoppings");

        builder.HasKey(shopping => shopping.Id);

        builder.Property(shopping => shopping.Name)
            .HasMaxLength(180)
            .IsRequired();

        builder.Property(shopping => shopping.CorporateName)
            .HasMaxLength(250);

        builder.Property(shopping => shopping.Document)
            .HasMaxLength(20);

        builder.Property(shopping => shopping.Address)
            .HasMaxLength(250);

        builder.Property(shopping => shopping.Number)
            .HasMaxLength(30);

        builder.Property(shopping => shopping.District)
            .HasMaxLength(120);

        builder.Property(shopping => shopping.City)
            .HasMaxLength(120);

        builder.Property(shopping => shopping.State)
            .HasMaxLength(2);

        builder.Property(shopping => shopping.ZipCode)
            .HasMaxLength(10);

        builder.Property(shopping => shopping.ContactName)
            .HasMaxLength(150);

        builder.Property(shopping => shopping.ContactEmail)
            .HasMaxLength(200);

        builder.Property(shopping => shopping.ContactPhone)
            .HasMaxLength(30);

        builder.Property(shopping => shopping.WebsiteUrl)
            .HasMaxLength(500);

        builder.Property(shopping => shopping.ControlShopUrl)
            .HasMaxLength(500);

        builder.Property(shopping => shopping.PortalUrl)
            .HasMaxLength(500);

        builder.Property(shopping => shopping.ApiName)
            .HasMaxLength(200);

        builder.Property(shopping => shopping.XmlReadingEmail)
            .HasMaxLength(200);

        builder.Property(shopping => shopping.RegistrationEmail)
            .HasMaxLength(200);

        builder.Property(shopping => shopping.Notes)
            .HasMaxLength(2000);

        builder.Property(shopping => shopping.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasIndex(shopping => new
        {
            shopping.ShoppingGroupId,
            shopping.Name
        }).IsUnique();

        builder.HasQueryFilter(
            shopping => !shopping.IsDeleted);
    }
}