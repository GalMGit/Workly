using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Profiles.Contracts.Events;
using Modules.Profiles.Infrastructure.Persistence.Context;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Persistence.Durability;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;
using Workly.Shared.Endpoint;

namespace Modules.Profiles.Di;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddProfilesModule(
            IConfiguration configuration)
        {
            services.AddDbContextWithWolverineIntegration<
                ProfilesDbContext>(
                options =>
                {
                    options.UseNpgsql(
                        configuration.GetConnectionString(
                            "ProfilesDatabase"));
                });
            
            services.AddEndpoints(
                Assembly.GetExecutingAssembly());

            return services;
        }
    }
    
    extension(WolverineOptions options)
    {
        public void AddProfilesMessaging(
            IConfiguration configuration)
        {
            options.Discovery.IncludeAssembly(
                Assembly.GetExecutingAssembly());

            options.ListenToRabbitQueue("workly-profiles")
                .ConfigureQueue(q =>
                {
                    q.IsDurable = true;
                    q.AutoDelete = false;
                    q.IsExclusive = false;
                });
            
            options.PublishMessage<UserProfileCreated>()
                .ToRabbitExchange("profiles-events");
            
            options.PersistMessagesWithPostgresql(
                    configuration.GetConnectionString(
                        "ProfilesDatabase")!,
                    role: MessageStoreRole.Ancillary)
                .Enroll<ProfilesDbContext>();
        }
    }
    
    extension(IServiceProvider services)
    {
        public async Task InitializeProfilesAsync()
        {
            using var scope = services.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<ProfilesDbContext>();

            await db.Database.MigrateAsync();
        }
    }
}