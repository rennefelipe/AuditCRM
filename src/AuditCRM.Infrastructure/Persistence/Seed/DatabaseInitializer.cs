using AuditCRM.Application.Interfaces;
using AuditCRM.Domain.Entities;
using AuditCRM.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuditCRM.Infrastructure.Persistence.Seed;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<AuditDbContext>();

        var passwordHasher = scope.ServiceProvider
            .GetRequiredService<IPasswordHasher>();

        var settings = configuration
            .GetSection(InitialAdminSettings.SectionName)
            .Get<InitialAdminSettings>();

        if (settings is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(settings.Email) ||
            string.IsNullOrWhiteSpace(settings.Password))
        {
            return;
        }

        var normalizedEmail = settings.Email
            .Trim()
            .ToLowerInvariant();

        var userExists = await dbContext.Users
            .IgnoreQueryFilters()
            .AnyAsync(
                user => user.Email == normalizedEmail,
                cancellationToken);

        if (userExists)
        {
            return;
        }

        var adminName = string.IsNullOrWhiteSpace(settings.Name)
            ? "Administrador"
            : settings.Name.Trim();

        var passwordHash = passwordHasher.Hash(
            settings.Password);

        var administrator = new User(
            adminName,
            normalizedEmail,
            passwordHash,
            phone: null,
            isAdministrator: true);

        dbContext.Users.Add(administrator);

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }
}