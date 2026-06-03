using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NuanSystem.Application.DependencyInjection;
using NuanSystem.Infrastructure.DependencyInjection;
using NuanSystem.Persistence.DependencyInjection;
using NuanSystem.SapIntegration.DependencyInjection;
using NuanSystem.SyncWorker.Options;
using NuanSystem.SyncWorker.Workers;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var host = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((_, configuration) =>
        {
            configuration
                .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args);
        })
        .UseWindowsService(options =>
        {
            options.ServiceName = "NuanSystem SAP Sync Worker";
        })
        .UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/nuansystem-syncworker-.log", rollingInterval: RollingInterval.Day)
                .WriteTo.File("logs/nuansystem-syncworker-errors-.log", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error);
        })
        .ConfigureServices((context, services) =>
        {
            ValidateConfiguration(context);

            services.Configure<WorkerOptions>(context.Configuration.GetSection(WorkerOptions.SectionName));
            services.Configure<SapSyncOptions>(context.Configuration.GetSection(SapSyncOptions.SectionName));
            services.Configure<RetryOptions>(context.Configuration.GetSection(RetryOptions.SectionName));
            services.Configure<ServiceLayerWorkerOptions>(context.Configuration.GetSection(ServiceLayerWorkerOptions.SectionName));
            services.Configure<HostOptions>(options =>
            {
                var configuredSeconds = context.Configuration.GetValue<int?>($"{WorkerOptions.SectionName}:ShutdownTimeoutSeconds");
                options.ShutdownTimeout = TimeSpan.FromSeconds(configuredSeconds is > 0 ? configuredSeconds.Value : 30);
            });

            services
                .AddApplicationServices()
                .AddInfrastructureServices()
                .AddPersistenceServices(context.Configuration)
                .AddSapIntegrationServices();

            services.AddHostedService<SapSyncWorker>();
            services.AddHostedService<SapRetryWorker>();
            services.AddHostedService<SapOutboxWorker>();
        })
        .Build();

    await host.RunAsync();
}
catch (Exception exception)
{
    Log.Fatal(exception, "NuanSystem SAP Sync Worker finalizo inesperadamente.");
    Environment.ExitCode = 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}

static void ValidateConfiguration(HostBuilderContext context)
{
    var environment = context.HostingEnvironment.EnvironmentName;
    var ignoreSslErrors = context.Configuration.GetValue<bool>($"{ServiceLayerWorkerOptions.SectionName}:IgnoreSslErrors");

    if (ignoreSslErrors && !string.Equals(environment, Environments.Development, StringComparison.OrdinalIgnoreCase))
    {
        throw new InvalidOperationException("ServiceLayer:IgnoreSslErrors solo puede activarse en Development.");
    }
}
