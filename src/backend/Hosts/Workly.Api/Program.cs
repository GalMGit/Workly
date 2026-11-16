using JasperFx.CodeGeneration.Model;
using Serilog;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Persistence.Durability;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;
using Modules.Identity.Di;
using Workly.Api.Di;
using Scalar.AspNetCore;
using Workly.Shared.Endpoint;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((
    context, 
    services, 
    configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Host.UseWolverine(opt =>
{
    opt.UseRabbitMq(
        builder.Configuration.GetConnectionString(
            "RabbitMq")!);
    
    opt.UseEntityFrameworkCoreTransactions();
    opt.Policies.UseDurableOutboxOnAllSendingEndpoints();
    opt.Policies.UseDurableInboxOnAllListeners();
    
    // opt.PersistMessagesWithPostgresql(
    //     builder.Configuration.GetConnectionString(
    //         "WolverineDatabase")!,
    //     role: MessageStoreRole.Main);
    
    opt.ServiceLocationPolicy = ServiceLocationPolicy.AlwaysAllowed;
    
    opt.AddIdentityMessaging(builder.Configuration);
});

builder.Services.AddConfiguration(builder.Configuration);

var app = builder.Build();

await app.Services.InitializeIdentityAsync();

var api = app.MapGroup("/api");
app.MapEndpoints(api);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    app.MapScalarApiReference("/docs",options =>
    {
        options.WithTitle("Workly API")
            .AddPreferredSecuritySchemes("Bearer")
            .AddHttpAuthentication("Bearer", auth =>
            {
                auth.Token = string.Empty;
                auth.Description = "Bearer Token";
            });
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.Run();