using System.Security.Cryptography;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.DependencyInjection;
using NuanSystem.Infrastructure.DependencyInjection;
using NuanSystem.MasterBranchSyncWorker.Options;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.MasterBranchSyncWorker.Workers;
using NuanSystem.Persistence.DependencyInjection;
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
            options.ServiceName = "NuanSystem Master Branch Sync Worker";
        })
        .UseSerilog((context, services, configuration) =>
        {
            var logPath = context.Configuration["Serilog:FilePath"] ?? "logs/nuansystem-masterbranch-syncworker-.log";
            var errorLogPath = context.Configuration["Serilog:ErrorFilePath"] ?? "logs/nuansystem-masterbranch-syncworker-errors-.log";
            var retainedFileCount = context.Configuration.GetValue<int?>("Serilog:RetainedFileCountLimit") ?? 30;
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: retainedFileCount, shared: true)
                .WriteTo.File(errorLogPath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: retainedFileCount, shared: true, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error);
        })
        .ConfigureServices((context, services) =>
        {
            services.AddOptions<MasterBranchSyncWorkerOptions>()
                .Bind(context.Configuration.GetSection(MasterBranchSyncWorkerOptions.SectionName))
                .Validate(options => !options.Enabled || options.SkeletonMode || options.EnabledEntityAppliers.Length > 0,
                    "EnabledEntityAppliers es obligatorio cuando el worker opera en modo real.")
                .Validate(options => !context.HostingEnvironment.IsProduction() || options.SkeletonModeBehavior != SkeletonModeBehavior.ClaimAndIgnore,
                    "ClaimAndIgnore no esta permitido en Production.")
                .Validate(options => !options.Enabled || !context.HostingEnvironment.IsProduction() ||
                    (!string.IsNullOrWhiteSpace(context.Configuration.GetConnectionString("SqlServerAdmin")) &&
                     !string.IsNullOrWhiteSpace(context.Configuration["Security:EncryptionKey"])),
                    "ConnectionStrings:SqlServerAdmin y Security:EncryptionKey son obligatorios al habilitar el worker en Production.")
                .ValidateOnStart();

            services
                .AddApplicationServices()
                .AddInfrastructureServices()
                .AddPersistenceServices(context.Configuration);

            services.AddScoped<ISyncEntityEventApplier, CountrySyncEventApplier>();
            services.AddScoped<ISyncEntityEventApplier, ProvinceSyncEventApplier>();
            services.AddScoped<ISyncEntityEventApplier, CitySyncEventApplier>();
            services.AddScoped<ISyncEntityEventApplier, CurrencySyncEventApplier>();
            services.AddScoped<ISyncEntityEventApplier, PriceListSyncEventApplier>();
            services.AddScoped<ISyncEntityEventApplier, TaxSyncEventApplier>();
            services.AddScoped<ISyncEntityEventApplier, ReferenceCatalogSyncEventApplier>();
            services.AddScoped<ISyncEntityEventApplier, UnitMeasureSyncEventApplier>();
            services.AddScoped<ISyncEntityEventApplier, ProductTypeSyncEventApplier>();
            services.AddScoped<ISyncEntityEventApplier, PurchaseOrderSyncEventApplier>();
            services.AddScoped<ISyncEntityEventApplier, BusinessPartnerSyncEventApplier>();
            services.AddScoped<ISyncEntityEventApplier, BusinessPartnerProposalSyncEventApplier>();
            services.AddScoped<ISyncEntityEventApplier, BusinessPartnerProposalResultSyncEventApplier>();
            services.AddScoped<ISyncEntityEventApplier, ItemGroupSyncEventApplier>();
            services.AddScoped<ISyncEntityEventApplier, ItemFamilySyncEventApplier>();
            services.AddScoped<ISyncEntityEventApplier, ItemBrandSyncEventApplier>();
            services.AddScoped<ISyncEntityEventApplier, ItemLineSyncEventApplier>();
            services.AddScoped<ISyncEntityEventApplier, ItemOriginSyncEventApplier>();
            services.AddScoped<ISyncEntityEventApplier, ReplenishmentMethodSyncEventApplier>();
            services.AddScoped<ISyncEntityEventApplier, StorageConditionSyncEventApplier>();
            services.AddScoped<ISyncEntityEventApplier, ItemSubgroupSyncEventApplier>();
            services.AddScoped<ISyncEntityEventApplier, ItemSyncEventApplier>();
            services.AddScoped<ISyncEntityEventApplier, WarehouseSyncEventApplier>();
            services.AddScoped<ISyncEntityEventApplier, CarrierSyncEventApplier>();
            services.AddScoped<ISyncEventApplier, SyncEventApplierDispatcher>();
            services.AddScoped<ILocalSyncOutboxRelay, LocalSyncOutboxRelay>();
            services.AddScoped<IMasterBranchSyncWorkerProcessor, MasterBranchSyncWorkerProcessor>();

            var diagnosticsPath = $"{MasterBranchSyncWorkerOptions.SectionName}:Diagnostics";
            var openMasterConnectionAndExit = context.Configuration.GetValue<bool>($"{diagnosticsPath}:OpenMasterConnectionAndExit");
            var releaseExpiredLocksAndExit = context.Configuration.GetValue<bool>($"{diagnosticsPath}:ReleaseExpiredLocksAndExit");
            if (!openMasterConnectionAndExit && !releaseExpiredLocksAndExit)
            {
                services.AddHostedService<MasterBranchSyncWorker>();
            }
        })
        .Build();

    var currentOptions = host.Services.GetRequiredService<IOptions<MasterBranchSyncWorkerOptions>>().Value;
    if (currentOptions.Diagnostics.OpenMasterConnectionAndExit)
    {
        if (currentOptions.Diagnostics.SqlConnectionDiagnostics)
        {
            LogProcessEnvironmentDiagnostics("B_Worker");
        }

        Environment.ExitCode = await RunOpenMasterConnectionDiagnosticAsync(host.Services);
        return;
    }

    if (currentOptions.Diagnostics.ReleaseExpiredLocksAndExit)
    {
        if (currentOptions.Diagnostics.SqlConnectionDiagnostics)
        {
            LogProcessEnvironmentDiagnostics("B_Worker");
        }

        Environment.ExitCode = await RunReleaseExpiredLocksDiagnosticAsync(host.Services);
        return;
    }

    await host.RunAsync();
}
catch (Exception exception)
{
    Log.Fatal(exception, "NuanSystem Master/Branch Sync Worker finalizo inesperadamente.");
    Environment.ExitCode = 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}

static async Task<int> RunOpenMasterConnectionDiagnosticAsync(IServiceProvider serviceProvider)
{
    await using var scope = serviceProvider.CreateAsyncScope();
    var services = scope.ServiceProvider;
    var configuration = services.GetRequiredService<IConfiguration>();
    var options = services.GetRequiredService<IOptions<MasterBranchSyncWorkerOptions>>().Value;
    var connectionFactory = services.GetRequiredService<IMasterConnectionFactory>();

    try
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        if (options.Diagnostics.SqlConnectionDiagnostics)
        {
            LogSqlConnectionDiagnostics(configuration, options, connection);
        }

        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1;";
        _ = await command.ExecuteScalarAsync();

        Log.Information("OpenMasterConnectionAndExit=OK");
        return 0;
    }
    catch (Exception exception)
    {
        Log.Error(exception, "OpenMasterConnectionAndExit=FAILED");
        return 2;
    }
}

static async Task<int> RunReleaseExpiredLocksDiagnosticAsync(IServiceProvider serviceProvider)
{
    await using var scope = serviceProvider.CreateAsyncScope();
    var services = scope.ServiceProvider;
    var configuration = services.GetRequiredService<IConfiguration>();
    var options = services.GetRequiredService<IOptions<MasterBranchSyncWorkerOptions>>().Value;
    var connectionFactory = services.GetRequiredService<IMasterConnectionFactory>();
    var outboxRepository = services.GetRequiredService<ISyncOutboxRepository>();

    try
    {
        var expiredInProcessCandidateCount = await CountExpiredInProcessCandidatesAsync(connectionFactory, configuration, options);
        Log.Information("ExpiredInProcessCandidateCount={ExpiredInProcessCandidateCount}", expiredInProcessCandidateCount);
        if (expiredInProcessCandidateCount > 0)
        {
            Log.Warning("ReleaseExpiredLocksAndExit=SKIPPED because expired candidates exist.");
            return 3;
        }

        var affectedRows = await outboxRepository.ReleaseExpiredLocksAsync();
        Log.Information("ReleaseExpiredLocksAndExit=OK AffectedRows={AffectedRows}", affectedRows);
        return 0;
    }
    catch (Exception exception)
    {
        Log.Error(exception, "ReleaseExpiredLocksAndExit=FAILED");
        return 2;
    }
}

static async Task<int> CountExpiredInProcessCandidatesAsync(
    IMasterConnectionFactory connectionFactory,
    IConfiguration configuration,
    MasterBranchSyncWorkerOptions options)
{
    await using var connection = (SqlConnection)connectionFactory.CreateConnection();
    if (options.Diagnostics.SqlConnectionDiagnostics)
    {
        LogSqlConnectionDiagnostics(configuration, options, connection);
    }

    await connection.OpenAsync();
    await using var command = connection.CreateCommand();
    command.CommandText = """
SELECT COUNT(1)
FROM dbo.SyncOutbox
WHERE Status = N'InProcess'
  AND LockExpiresAt IS NOT NULL
  AND LockExpiresAt <= SYSUTCDATETIME();
""";
    var value = await command.ExecuteScalarAsync();
    return Convert.ToInt32(value);
}

static void LogSqlConnectionDiagnostics(
    IConfiguration configuration,
    MasterBranchSyncWorkerOptions options,
    SqlConnection connection)
{
    var builder = new SqlConnectionStringBuilder(connection.ConnectionString);
    var encryptionKey = configuration["Security:EncryptionKey"];
    var currentDirectory = Directory.GetCurrentDirectory();
    var baseDirectory = AppContext.BaseDirectory;

    Log.Information(
        "SqlConnectionDiagnostics ProcessId={ProcessId} CurrentDirectory={CurrentDirectory} AppContextBaseDirectory={AppContextBaseDirectory} DOTNET_ENVIRONMENT={DotnetEnvironment} ProcessPath={ProcessPath} FrameworkDescription={FrameworkDescription} OSDescription={OSDescription} ProcessArchitecture={ProcessArchitecture} SqlClientAssemblyVersion={SqlClientAssemblyVersion} CurrentDirectoryLocalAppsettingsExists={CurrentDirectoryLocalAppsettingsExists} BaseDirectoryLocalAppsettingsExists={BaseDirectoryLocalAppsettingsExists} SqlServerAdminPresent={SqlServerAdminPresent} EncryptionKeyPresent={EncryptionKeyPresent} EncryptionKeyLengthGreaterThanZero={EncryptionKeyLengthGreaterThanZero} PolicyEncrypt={PolicyEncrypt} PolicyTrustServerCertificate={PolicyTrustServerCertificate} BuilderEncrypt={BuilderEncrypt} BuilderTrustServerCertificate={BuilderTrustServerCertificate} InitialCatalog={InitialCatalog} IntegratedSecurity={IntegratedSecurity} MultipleActiveResultSets={MultipleActiveResultSets} ConnectTimeout={ConnectTimeout} ApplicationName={ApplicationName} DataSourceLength={DataSourceLength} DataSourceHash={DataSourceHash} UserIdPresent={UserIdPresent} PasswordPresent={PasswordPresent}",
        Environment.ProcessId,
        currentDirectory,
        baseDirectory,
        Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT"),
        Environment.ProcessPath,
        RuntimeInformation.FrameworkDescription,
        RuntimeInformation.OSDescription,
        RuntimeInformation.ProcessArchitecture,
        typeof(SqlConnection).Assembly.GetName().Version?.ToString() ?? "NULL",
        File.Exists(Path.Combine(currentDirectory, "appsettings.Local.json")),
        File.Exists(Path.Combine(baseDirectory, "appsettings.Local.json")),
        !string.IsNullOrWhiteSpace(configuration.GetConnectionString("SqlServerAdmin")),
        !string.IsNullOrWhiteSpace(encryptionKey),
        encryptionKey?.Length > 0,
        configuration["SqlConnectionPolicy:Encrypt"],
        configuration["SqlConnectionPolicy:TrustServerCertificate"],
        builder.Encrypt,
        builder.TrustServerCertificate,
        builder.InitialCatalog,
        builder.IntegratedSecurity,
        builder.MultipleActiveResultSets,
        builder.ConnectTimeout,
        builder.ApplicationName,
        builder.DataSource?.Length ?? 0,
        HashDataSource(builder.DataSource),
        !string.IsNullOrWhiteSpace(builder.UserID),
        !string.IsNullOrWhiteSpace(builder.Password));
}

static void LogProcessEnvironmentDiagnostics(string prefix)
{
    using var currentProcess = Process.GetCurrentProcess();
    var sqlClientAssembly = typeof(SqlConnection).Assembly;

    Log.Information(
        "{Prefix} ProcessName={ProcessName} ProcessPath={ProcessPath} ProcessId={ProcessId} CurrentDirectory={CurrentDirectory} AppContextBaseDirectory={AppContextBaseDirectory} DOTNET_ROOT={DotnetRoot} DOTNET_ENVIRONMENT={DotnetEnvironment} ASPNETCORE_ENVIRONMENT={AspnetcoreEnvironment} FrameworkDescription={FrameworkDescription} OSDescription={OSDescription} ProcessArchitecture={ProcessArchitecture} Is64BitProcess={Is64BitProcess} SqlClientAssemblyLocation={SqlClientAssemblyLocation} SqlClientAssemblyVersion={SqlClientAssemblyVersion} SqlClientPackageVersion={SqlClientPackageVersion} SystemSecurityCryptographyAssembly={SystemSecurityCryptographyAssembly} PathLength={PathLength} SniNativeDependencyAvailable={SniNativeDependencyAvailable} SniNativeDependencyLoaded={SniNativeDependencyLoaded} CurrentUserName={CurrentUserName}",
        prefix,
        currentProcess.ProcessName,
        Environment.ProcessPath ?? "NULL",
        Environment.ProcessId,
        Directory.GetCurrentDirectory(),
        AppContext.BaseDirectory,
        Environment.GetEnvironmentVariable("DOTNET_ROOT") ?? "NULL",
        Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "NULL",
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "NULL",
        RuntimeInformation.FrameworkDescription,
        RuntimeInformation.OSDescription,
        RuntimeInformation.ProcessArchitecture,
        Environment.Is64BitProcess,
        sqlClientAssembly.Location,
        sqlClientAssembly.GetName().Version?.ToString() ?? "NULL",
        sqlClientAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "NULL",
        typeof(SHA256).Assembly.Location,
        Environment.GetEnvironmentVariable("PATH")?.Length ?? 0,
        HasSniNativeDependency(AppContext.BaseDirectory),
        IsSniNativeDependencyLoaded(),
        SanitizeUserName(Environment.UserName));
}

static string HashDataSource(string? dataSource)
{
    if (string.IsNullOrWhiteSpace(dataSource))
    {
        return "NULL";
    }

    var hash = SHA256.HashData(Encoding.UTF8.GetBytes(dataSource));
    return Convert.ToHexString(hash)[..12];
}

static bool HasSniNativeDependency(string baseDirectory)
{
    return Directory.EnumerateFiles(baseDirectory, "*SNI*.dll", SearchOption.TopDirectoryOnly).Any();
}

static bool IsSniNativeDependencyLoaded()
{
    try
    {
        using var process = Process.GetCurrentProcess();
        return process.Modules
            .Cast<ProcessModule>()
            .Any(module => module.ModuleName.Contains("SNI", StringComparison.OrdinalIgnoreCase));
    }
    catch
    {
        return false;
    }
}

static string SanitizeUserName(string? userName)
{
    if (string.IsNullOrWhiteSpace(userName))
    {
        return "NULL";
    }

    var slashIndex = userName.LastIndexOf('\\');
    return slashIndex >= 0 ? userName[(slashIndex + 1)..] : userName;
}
