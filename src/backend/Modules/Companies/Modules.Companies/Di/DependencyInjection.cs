using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Companies.Contracts.Events;
using Modules.Companies.Infrastructure.Persistence.Context;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Persistence.Durability;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;
using Workly.Shared.Endpoint;

namespace Modules.Companies.Di;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddCompaniesModule(
            IConfiguration configuration)
        {
            services.AddDbContextWithWolverineIntegration<
                CompaniesDbContext>(
                options =>
                {
                    options.UseNpgsql(
                        configuration.GetConnectionString(
                            "CompaniesDatabase"));
                });

            services.AddEndpoints(
                Assembly.GetExecutingAssembly());

            services.AddValidatorsFromAssembly(
                Assembly.GetExecutingAssembly());

            return services;
        }
    }

    extension(WolverineOptions options)
    {
        public void AddCompaniesMessaging(
            IConfiguration configuration)
        {
            options.Discovery.IncludeAssembly(
                Assembly.GetExecutingAssembly());

            options.ListenToRabbitQueue("workly-companies")
                .ConfigureQueue(q =>
                {
                    q.IsDurable = true;
                    q.AutoDelete = false;
                    q.IsExclusive = false;
                });

            options.PublishMessage<CompanyCreated>()
                .ToRabbitExchange("companies-events");

            options.PublishMessage<CompanyVerified>()
                .ToRabbitExchange("companies-events");

            options.PublishMessage<CompanySuspended>()
                .ToRabbitExchange("companies-events");

            options.PublishMessage<CompanyClosed>()
                .ToRabbitExchange("companies-events");

            options.PersistMessagesWithPostgresql(
                    configuration.GetConnectionString(
                        "CompaniesDatabase")!,
                    role: MessageStoreRole.Ancillary)
                .Enroll<CompaniesDbContext>();
        }
    }

    extension(IServiceProvider services)
    {
        public async Task InitializeCompaniesAsync()
        {
            using var scope = services.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<CompaniesDbContext>();

            await db.Database.MigrateAsync();
        }
    }
}
