using AuditCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuditCRM.Infrastructure.Persistence.Context;

public sealed class AuditDbContext : DbContext
{
    public AuditDbContext(DbContextOptions<AuditDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<ShoppingGroup> ShoppingGroups => Set<ShoppingGroup>();

    public DbSet<Shopping> Shoppings => Set<Shopping>();

    public DbSet<Erp> Erps => Set<Erp>();

    public DbSet<InstallationType> InstallationTypes => Set<InstallationType>();

    public DbSet<Store> Stores => Set<Store>();

    public DbSet<StoreContact> StoreContacts => Set<StoreContact>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AuditDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}