using System.Text;
using AuditCRM.Application.Interfaces;
using AuditCRM.Infrastructure.Authentication;
using AuditCRM.Infrastructure.Persistence.Context;
using AuditCRM.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AuditCRM.Infrastructure;



public static class DependencyInjection
{

    
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "A string de conexão 'DefaultConnection' não foi configurada.");

        services.AddDbContext<AuditDbContext>(options =>
            options.UseSqlServer(connectionString));
        

        services.Configure<JwtSettings>(
            configuration.GetSection(JwtSettings.SectionName));

        var jwtSettings = configuration
            .GetSection(JwtSettings.SectionName)
            .Get<JwtSettings>()
            ?? throw new InvalidOperationException(
                "As configurações JWT não foram encontradas.");

        if (string.IsNullOrWhiteSpace(jwtSettings.Key))
        {
            throw new InvalidOperationException(
                "A chave JWT não foi configurada.");
        }

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    jwtSettings.Key)),

                        ClockSkew = TimeSpan.Zero
                    };
            });

        services.AddAuthorization();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IShoppingGroupService, ShoppingGroupService>();
        services.AddScoped<IShoppingService, ShoppingService>();
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<IStoreContactService, StoreContactService>();
        services.AddScoped<IErpService, ErpService>();
        services.AddScoped<IInstallationTypeService, InstallationTypeService>();
        services.AddScoped<IStoreProcessService, StoreProcessService>();
        services.AddScoped<IProcessInteractionService,ProcessInteractionService>();
        services.AddScoped<IProcessAppointmentService,ProcessAppointmentService>();
        services.AddScoped<IProcessInstallationService,ProcessInstallationService>();
        services.AddScoped<IProcessNoteService, ProcessNoteService>();
        services.AddScoped<IProcessDocumentService,ProcessDocumentService>();
        services.AddScoped<IProcessTimelineService,ProcessTimelineService>();
        services.AddScoped<IWorkQueueService,WorkQueueService>();
        services.AddScoped<IDashboardService,DashboardService>();

        return services;
    }
}