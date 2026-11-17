using Wolverine;
using Wolverine.RabbitMQ;
using Workly.EmailWorker.Di;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddConfiguration(builder.Configuration);

builder.UseWolverine(opt =>
{
    opt.UseRabbitMq(
            builder.Configuration.GetConnectionString("RabbitMq")!)
        .AutoProvision();

    opt.ListenToRabbitQueue("workly-email")
        .ConfigureQueue(q =>
        {
            q.IsDurable = true;
            q.AutoDelete = false;
            q.IsExclusive = false;
        });

    opt.Discovery.IncludeAssembly(
        typeof(EmailWorkerMarker).Assembly);
});

var host = builder.Build();

host.Run();

public sealed class EmailWorkerMarker;
