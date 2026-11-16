using System.Security.Claims;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Modules.Identity.Application.Abstractions.Auth;
using Modules.Identity.Application.Abstractions.Cache;
using Modules.Identity.Infrastructure.Auth;
using Modules.Identity.Infrastructure.Cache;
using Modules.Identity.Infrastructure.Persistence;
using Modules.Identity.Infrastructure.Persistence.Context;
using Modules.Identity.Infrastructure.Persistence.Seed;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Persistence.Durability;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;
using Workly.Shared.Endpoint;

namespace Modules.Identity.Di;

public sealed class IdentityModuleMarker;

public static class DependencyInjection
{
    extension(WolverineOptions options)
    {
        public void AddIdentityMessaging(
            IConfiguration configuration)
        {
            options.Discovery.IncludeAssembly(
                typeof(IdentityModuleMarker).Assembly);
            
            options.PersistMessagesWithPostgresql(
                    configuration.GetConnectionString(
                        "IdentityDatabase")!,
                    role: MessageStoreRole.Main)
                .Enroll<IdentityDbContext>();
        }
    }

    extension(IServiceProvider services)
    {
        public async Task InitializeIdentityAsync()
        {
            using var scope = services.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<IdentityDbContext>();

            var adminOptions = scope.ServiceProvider
                .GetRequiredService<IOptions<AdminOptions>>();

            var passwordHasher = scope.ServiceProvider
                .GetRequiredService<IPasswordHasher>();

            var logger = scope.ServiceProvider
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("Modules.Identity.Seeder");

            await db.Database.MigrateAsync();

            await IdentitySeeder.SeedAsync(
                db,
                adminOptions.Value,
                passwordHasher,
                logger);
        }
    }
    
    extension(IServiceCollection services)
    {
        public IServiceCollection AddIdentityModule(
            IConfiguration configuration)
        {
            services.AddDbContextWithWolverineIntegration<IdentityDbContext>(
                options =>
                {
                    options.UseNpgsql(
                        configuration.GetConnectionString(
                            "IdentityDatabase"));
                });
            
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "Workly_";
            });
            
            services.AddAuth(configuration);
            
            services.Configure<AdminOptions>(
                configuration.GetSection("Identity:Admin"));
            
            services.AddEndpoints(
                typeof(IdentityModuleMarker).Assembly);
            
            services.AddValidatorsFromAssembly(
                typeof(IdentityModuleMarker).Assembly);
            
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<IJwtProvider, JwtProvider>();
            services.AddSingleton<ICacheService, RedisCacheService>();
            
            return services;
        }
        
        private void AddAuth(IConfiguration configuration)
        {
            services.Configure<JwtOptions>(
                configuration.GetSection(nameof(JwtOptions)));

            services.AddOptions<JwtOptions>()
                .Validate(o =>
                    !string.IsNullOrEmpty(o.SecretKey) &&
                    Encoding.UTF8.GetByteCount(o.SecretKey) >= 32,
                    "SecretKey is required and must be at least 32 bytes long")
                .Validate(o =>
                    o.Expires > 0,
                    "Expires (minutes) must be greater than zero")
                .ValidateOnStart();

            var jwtOptions = configuration
                .GetSection(nameof(JwtOptions))
                .Get<JwtOptions>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
                {
                    o.TokenValidationParameters = new()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtOptions!.SecretKey)),
                        ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                        NameClaimType = ClaimTypes.NameIdentifier,
                        RoleClaimType = ClaimTypes.Role,
                    };
                    
                    o.MapInboundClaims = true;

                    o.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var request = context.HttpContext.Request;
                            
                            var authHeader = request.Headers.Authorization.FirstOrDefault();
                            if (!string.IsNullOrEmpty(authHeader) &&
                                authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                            {
                                context.Token = authHeader["Bearer ".Length..].Trim();
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization();
        }
    }
}
