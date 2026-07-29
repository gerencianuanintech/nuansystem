using NuanSystem.Application.Abstractions.Sri;
using NuanSystem.Infrastructure.DependencyInjection;
using NuanSystem.Application.DependencyInjection;
using NuanSystem.Persistence.DependencyInjection;
using NuanSystem.SriWorker.Options;
using NuanSystem.SriWorker.Services;
using NuanSystem.SriWorker.Workers;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
try
{
    var host = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((_, configuration) => configuration
            .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
            .AddJsonFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "NuanSystem", "SriWorker", "config", "appsettings.Operations.json"), optional: true, reloadOnChange: true)
            .AddEnvironmentVariables().AddCommandLine(args))
        .UseWindowsService(options => options.ServiceName = "NuanSystem SRI Worker")
        .UseSerilog((context, services, configuration) =>
        {
            var configuredLogDirectory=context.Configuration["Operations:LogDirectory"];
            var logDirectory=string.IsNullOrWhiteSpace(configuredLogDirectory)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),"NuanSystem","SriWorker","logs")
                : Path.GetFullPath(configuredLogDirectory);
            configuration.ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services).Enrich.FromLogContext().WriteTo.Console()
                .WriteTo.File(Path.Combine(logDirectory,"nuansystem-sriworker-.log"),rollingInterval:RollingInterval.Day,retainedFileCountLimit:30,shared:true)
                .WriteTo.File(Path.Combine(logDirectory,"nuansystem-sriworker-errors-.log"),rollingInterval:RollingInterval.Day,retainedFileCountLimit:30,shared:true,
                    restrictedToMinimumLevel:Serilog.Events.LogEventLevel.Error);
        })
        .ConfigureServices((context, services) =>
        {
            services.AddOptions<SriWorkerOptions>().Bind(context.Configuration.GetSection(SriWorkerOptions.SectionName))
                .Validate(o => o.BatchSize is >= 1 and <= 100 && o.MaxConcurrency is >= 1 and <= 16, "BatchSize o MaxConcurrency fuera de rango.")
                .Validate(o => o.LeaseSeconds is >= 30 and <= 3600 && o.MaxAttempts is >= 1 and <= 20, "LeaseSeconds o MaxAttempts fuera de rango.")
                .Validate(o => o.NotFoundMaxAttempts >= 1 && o.NotFoundMaxAttempts <= o.MaxAttempts, "NotFoundMaxAttempts debe estar entre 1 y MaxAttempts.")
                .Validate(o => o.RetryJitterRatio is >= 0 and <= 0.5, "RetryJitterRatio debe estar entre 0 y 0.5.").ValidateOnStart();
            services.AddOptions<WorkerEventLogOptions>().Bind(context.Configuration.GetSection(WorkerEventLogOptions.SectionName))
                .Validate(o => !string.IsNullOrWhiteSpace(o.SourceName) && o.CriticalEventId is >= 1 and <= 65535, "Windows Event Log invalido.").ValidateOnStart();
            services.AddOptions<SriWorkerOptions>().Validate(o => o.HeartbeatSeconds is >= 10 and <= 300 && o.NormalizedWorkerInstance.Length <= 120,
                "HeartbeatSeconds o WorkerInstance fuera de rango.").ValidateOnStart();
            services.AddOptions<SriProviderOptions>().Bind(context.Configuration.GetSection(SriProviderOptions.SectionName))
                .Validate(o => SriProviderOptions.IsOfficialEndpoint(o.TestAuthorizationUrl, "celcer.sri.gob.ec"), "Endpoint SRI Test no oficial.")
                .Validate(o => SriProviderOptions.IsOfficialEndpoint(o.ProductionAuthorizationUrl, "cel.sri.gob.ec"), "Endpoint SRI Production no oficial.")
                .Validate(o => o.TimeoutSeconds is >= 5 and <= 120 && o.MaxXmlBytes is >= 1024 and <= 5 * 1024 * 1024, "Timeout o MaxXmlBytes fuera de rango.").ValidateOnStart();
            services.Configure<HostOptions>(o => o.ShutdownTimeout = TimeSpan.FromSeconds(30));
            services.AddApplicationServices().AddInfrastructureServices().AddPersistenceServices(context.Configuration);
            services.AddHttpClient<ISriAuthorizationProvider, SriAuthorizationProvider>((provider, client) =>
            {
                var configured = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<SriProviderOptions>>().Value;
                client.Timeout = TimeSpan.FromSeconds(configured.TimeoutSeconds);
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false });
            services.AddScoped<ISriWorkerProcessor, SriWorkerProcessor>();
            services.AddSingleton<SriWorkerRuntimeState>();
            services.AddSingleton<ISriWorkerExecutionGate>(provider => provider.GetRequiredService<SriWorkerRuntimeState>());
            services.AddSingleton<SriSingleInstanceGuard>();
            services.AddSingleton<IWorkerOperationalEventPublisher, WorkerOperationalEventPublisher>();
            services.AddSingleton<SriHeartbeatWorker>();
            services.AddHostedService(provider => provider.GetRequiredService<SriHeartbeatWorker>());
            services.AddHostedService<SriBackgroundWorker>();
        }).Build();
    await host.RunAsync();
}
catch (Exception exception)
{
    Log.Fatal("NuanSystem SRI Worker finalizo inesperadamente. ErrorType={ErrorType}", exception.GetType().Name);
    Environment.ExitCode = 1;
}
finally { await Log.CloseAndFlushAsync(); }
