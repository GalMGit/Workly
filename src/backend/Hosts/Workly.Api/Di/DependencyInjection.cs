using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Modules.Identity.Di;
using Modules.Companies.Di;
using Modules.Profiles.Di;
using Workly.Api.Infrastructure.Messaging;

namespace Workly.Api.Di;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddConfiguration(
            IConfiguration configuration)
        {
            services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(
                    new JsonStringEnumConverter());
            });

            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((
                    document,
                    context, 
                    cancellationToken) =>
                {
                    var bearerScheme = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header
                    };

                    document.Components ??= new OpenApiComponents();
                    document.AddComponent("Bearer", bearerScheme);

                    var securityRequirement = new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                    };

                    foreach (var operation in document.Paths.Values
                                 .SelectMany(path => path.Operations!))
                    {
                        operation.Value.Security ??= [];
                        operation.Value.Security.Add(securityRequirement);
                    }

                    return Task.CompletedTask;
                });
            });
            
            services.AddIdentityModule(configuration);
            services.AddProfilesModule(configuration);
            services.AddCompaniesModule(configuration);

            services.AddDbContext<MessagingDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString(
                        "MessagingDatabase")));
        }
    }

    extension(IServiceProvider services)
    {
        public async Task InitializeMessagingAsync()
        {
            using var scope = services.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<MessagingDbContext>();

            await db.Database.EnsureCreatedAsync();
        }
    }
}