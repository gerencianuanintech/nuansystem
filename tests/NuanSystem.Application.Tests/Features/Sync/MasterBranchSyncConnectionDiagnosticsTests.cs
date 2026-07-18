using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.DependencyInjection;
using NuanSystem.Infrastructure.DependencyInjection;
using NuanSystem.MasterBranchSyncWorker.Options;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Persistence.Connections;
using NuanSystem.Persistence.DependencyInjection;
using NuanSystem.Persistence.Options;
using NuanSystem.Persistence.Repositories.Sync;
using NuanSystem.Shared.Sync;
using NuanSystem.Application.Tests.Infrastructure;
using Xunit.Abstractions;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class MasterBranchSyncConnectionDiagnosticsTests
{
    private const long DiagnosticOutboxId = 20005;
    private const long DiagnosticTargetId = 20005;
    private static readonly Guid DiagnosticEventId = Guid.Parse("fa452762-910a-40fc-9e79-72fb06a28eea");
    private static readonly Guid DiagnosticGlobalId = Guid.Parse("383f9281-c05c-41ef-a9be-9fb1a57c9bd2");

    private readonly ITestOutputHelper _output;

    public MasterBranchSyncConnectionDiagnosticsTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Diagnostica entorno del testhost sin secretos")]
    public void ProcessEnvironmentComparisonDiagnostics()
    {
        WriteProcessEnvironmentDiagnostics("A_TestHost");
    }

    [Fact]
    public void MasterConnectionFactory_AppliesConfiguredSqlConnectionPolicy()
    {
        using var provider = BuildMasterConnectionFactoryProvider(new Dictionary<string, string?>
        {
            ["ConnectionStrings:SqlServerAdmin"] = BuildAdminConnectionString(multipleActiveResultSets: false),
            ["MasterDatabase:DatabaseName"] = "NuanSystem_Master",
            ["SqlConnectionPolicy:Encrypt"] = "true",
            ["SqlConnectionPolicy:TrustServerCertificate"] = "true"
        });

        var builder = CreateMasterConnectionStringBuilder(provider);

        Assert.True(builder.Encrypt);
        Assert.True(builder.TrustServerCertificate);
        Assert.Equal("NuanSystem_Master", builder.InitialCatalog);
    }

    [Fact]
    public void MasterConnectionFactory_UsesSecureDefaults_WhenPolicyMissing()
    {
        using var provider = BuildMasterConnectionFactoryProvider(new Dictionary<string, string?>
        {
            ["ConnectionStrings:SqlServerAdmin"] = BuildAdminConnectionString(multipleActiveResultSets: false),
            ["MasterDatabase:DatabaseName"] = "NuanSystem_Master"
        });

        var builder = CreateMasterConnectionStringBuilder(provider);

        Assert.True(builder.Encrypt);
        Assert.False(builder.TrustServerCertificate);
    }

    [Fact]
    public void MasterConnectionFactory_PreservesConnectionCredentialsWithoutPrintingSecrets()
    {
        using var provider = BuildMasterConnectionFactoryProvider(new Dictionary<string, string?>
        {
            ["ConnectionStrings:SqlServerAdmin"] = BuildAdminConnectionString(integratedSecurity: false),
            ["MasterDatabase:DatabaseName"] = "NuanSystem_Master",
            ["SqlConnectionPolicy:Encrypt"] = "true",
            ["SqlConnectionPolicy:TrustServerCertificate"] = "true"
        });

        var builder = CreateMasterConnectionStringBuilder(provider);

        Assert.False(string.IsNullOrWhiteSpace(builder.UserID));
        Assert.False(string.IsNullOrWhiteSpace(builder.Password));
        Assert.False(builder.IntegratedSecurity);
    }

    [Fact]
    public void MasterConnectionFactory_DoesNotDependOnSqlServerAdminPolicyText()
    {
        using var provider = BuildMasterConnectionFactoryProvider(new Dictionary<string, string?>
        {
            ["ConnectionStrings:SqlServerAdmin"] = BuildAdminConnectionString(),
            ["MasterDatabase:DatabaseName"] = "NuanSystem_Master",
            ["SqlConnectionPolicy:Encrypt"] = "true",
            ["SqlConnectionPolicy:TrustServerCertificate"] = "true"
        });

        var builder = CreateMasterConnectionStringBuilder(provider);

        Assert.True(builder.Encrypt);
        Assert.True(builder.TrustServerCertificate);
        Assert.Equal("NuanSystem_Master", builder.InitialCatalog);
    }

    [SqlServerIntegrationFact(DisplayName = "Compara conexion MasterConnectionFactory con politica SQL esperada sin secretos")]
    [Trait("Category", "SqlServerIntegration")]
    public async Task CompareMasterConnectionFactoryConnectionPolicyWithoutSecrets()
    {
        await using var provider = BuildWorkerServiceProvider(out var configuration);
        using var scope = provider.CreateScope();

        var masterFactory = scope.ServiceProvider.GetRequiredService<IMasterConnectionFactory>();
        using var masterConnection = masterFactory.CreateConnection();
        var factoryBuilder = new SqlConnectionStringBuilder(masterConnection.ConnectionString);

        var adminConnectionString = configuration.GetConnectionString("SqlServerAdmin");
        var manualBuilder = new SqlConnectionStringBuilder(adminConnectionString)
        {
            InitialCatalog = configuration["MasterDatabase:DatabaseName"]
        };

        var expectedPolicy = scope.ServiceProvider
            .GetRequiredService<IOptions<SqlConnectionPolicyOptions>>()
            .Value;

        WriteLine("ConnectionComparisonMechanism=MasterConnectionFactory.CreateConnection without opening connection");
        WriteLine("ConnectionComparisonEnvironment=Local");
        WriteLine("ConnectionComparisonWorkerLocalExists=True");
        WriteLine($"ConnectionComparisonSqlServerAdminContainsEncrypt={manualBuilder.ContainsKey("Encrypt")}");
        WriteLine($"ConnectionComparisonSqlServerAdminContainsTrustServerCertificate={manualBuilder.ContainsKey("Trust Server Certificate")}");

        WriteSanitizedConnectionProperties("A_MasterConnectionFactory", masterConnection.GetType().FullName, factoryBuilder);
        WriteSanitizedConnectionProperties("B_ManualDiagnostic", "Microsoft.Data.SqlClient.SqlConnectionStringBuilder", manualBuilder);
        WriteLine($"C_SqlConnectionPolicy_Encrypt={expectedPolicy.Encrypt}");
        WriteLine($"C_SqlConnectionPolicy_TrustServerCertificate={expectedPolicy.TrustServerCertificate}");

        WriteLine($"Diff_MasterFactoryEncryptMatchesPolicy={factoryBuilder.Encrypt == expectedPolicy.Encrypt}");
        WriteLine($"Diff_MasterFactoryTrustServerCertificateMatchesPolicy={factoryBuilder.TrustServerCertificate == expectedPolicy.TrustServerCertificate}");
        WriteLine($"Diff_ManualEncryptMatchesFactory={manualBuilder.Encrypt == factoryBuilder.Encrypt}");
        WriteLine($"Diff_ManualTrustServerCertificateMatchesFactory={manualBuilder.TrustServerCertificate == factoryBuilder.TrustServerCertificate}");

        Assert.IsType<SqlConnection>(masterConnection);
        Assert.False(string.IsNullOrWhiteSpace(factoryBuilder.InitialCatalog));
        Assert.Equal(manualBuilder.InitialCatalog, factoryBuilder.InitialCatalog);
    }

    [SqlServerIntegrationFact(DisplayName = "Diagnostica conexiones Master y Sucursal sin procesar eventos")]
    [Trait("Category", "SqlServerIntegration")]
    public async Task DiagnoseMasterAndBranchConnectionsWithoutProcessingEvents()
    {
        var repositoryRoot = FindRepositoryRoot();
        var workerDirectory = Path.Combine(repositoryRoot, "src", "Backend", "NuanSystem.MasterBranchSyncWorker");
        var environmentName = "Local";
        var args = Array.Empty<string>();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(workerDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: false)
            .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

        var adminConnectionString = configuration.GetConnectionString("SqlServerAdmin") ?? string.Empty;
        var adminBuilder = string.IsNullOrWhiteSpace(adminConnectionString)
            ? null
            : new SqlConnectionStringBuilder(adminConnectionString);

        WriteLine("DiagnosticMechanism=tests/NuanSystem.Application.Tests/Features/Sync/MasterBranchSyncConnectionDiagnosticsTests.cs");
        WriteLine("ConfigurationBase=src/Backend/NuanSystem.MasterBranchSyncWorker");
        WriteLine("DOTNET_ENVIRONMENT=Local");
        WriteLine($"WorkerAppsettingsJsonExists={File.Exists(Path.Combine(workerDirectory, "appsettings.json"))}");
        WriteLine($"WorkerAppsettingsLocalExists={File.Exists(Path.Combine(workerDirectory, "appsettings.Local.json"))}");
        WriteLine($"SqlServerAdminPresent={!string.IsNullOrWhiteSpace(adminConnectionString)}");
        WriteLine($"SqlServerAdminContainsEncrypt={adminBuilder?.ContainsKey("Encrypt") ?? false}");
        WriteLine($"SqlServerAdminContainsTrustServerCertificate={adminBuilder?.ContainsKey("Trust Server Certificate") ?? false}");

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services
            .AddApplicationServices()
            .AddInfrastructureServices()
            .AddPersistenceServices(configuration);

        await using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var masterFactory = scope.ServiceProvider.GetRequiredService<MasterConnectionFactory>();
        var companyResolver = scope.ServiceProvider.GetRequiredService<ICompanyResolver>();

        await DiagnoseMasterConnectionAsync(masterFactory);

        CompanyConnectionInfo? branch = null;
        try
        {
            branch = await companyResolver.ResolveByIdAsync(2);
            WriteLine(branch is null
                ? "ResolveBranchCompany=FAILED NullCompany"
                : "ResolveBranchCompany=OK");
        }
        catch (Exception exception)
        {
            WriteException("ResolveBranchCompany", exception);
        }

        if (branch is not null)
        {
            await DiagnoseBranchConnectionAsync(branch.ConnectionString);
        }
        else
        {
            WriteLine("BranchConnection=SKIPPED BranchCompanyNotResolved");
        }
    }

    [SqlServerIntegrationFact(DisplayName = "Diagnostica ruta real del worker sin claim ni apply")]
    [Trait("Category", "SqlServerIntegration")]
    public async Task DiagnoseWorkerRouteWithoutClaimOrApply()
    {
        var repositoryRoot = FindRepositoryRoot();
        var workerDirectory = Path.Combine(repositoryRoot, "src", "Backend", "NuanSystem.MasterBranchSyncWorker");
        var workerOutputDirectory = Path.Combine(workerDirectory, "bin", "Debug", "net9.0");
        var environmentName = "Local";
        var args = Array.Empty<string>();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(workerDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: false)
            .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

        WriteLine("RouteDiagnosticMechanism=tests/NuanSystem.Application.Tests/Features/Sync/MasterBranchSyncConnectionDiagnosticsTests.cs");
        WriteLine("HostedServiceStarted=False");
        WriteLine("ProcessOnceInvoked=False");
        WriteLine("ClaimInvoked=False");
        WriteLine("ApplyInvoked=False");
        WriteLine($"WorkerProjectLocalExists={File.Exists(Path.Combine(workerDirectory, "appsettings.Local.json"))}");
        WriteLine($"WorkerOutputLocalExists={File.Exists(Path.Combine(workerOutputDirectory, "appsettings.Local.json"))}");

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services
            .AddApplicationServices()
            .AddInfrastructureServices()
            .AddPersistenceServices(configuration);

        services.Configure<MasterBranchSyncWorkerOptions>(
            configuration.GetSection(MasterBranchSyncWorkerOptions.SectionName));
        services.AddScoped<ISyncEntityEventApplier, BusinessPartnerSyncEventApplier>();
        services.AddScoped<ISyncEntityEventApplier, ItemSyncEventApplier>();
        services.AddScoped<ISyncEntityEventApplier, WarehouseSyncEventApplier>();
        services.AddScoped<ISyncEventApplier, SyncEventApplierDispatcher>();
        services.AddScoped<IMasterBranchSyncWorkerProcessor, MasterBranchSyncWorkerProcessor>();

        await using var provider = services.BuildServiceProvider();
        WriteLine("ServiceProviderBuild=OK");

        using var scope = provider.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        _ = scopedProvider.GetRequiredService<IMasterBranchSyncWorkerProcessor>();
        WriteLine("ProcessorResolved=OK");

        var outboxRepository = scopedProvider.GetRequiredService<ISyncOutboxRepository>();
        _ = scopedProvider.GetRequiredService<ISyncAuditRepository>();
        WriteLine("SyncRepositoriesResolved=OK");

        _ = scopedProvider.GetRequiredService<MasterConnectionFactory>();
        WriteLine("MasterConnectionFactoryResolved=OK");

        var companyResolver = scopedProvider.GetRequiredService<ICompanyResolver>();
        WriteLine($"CompanyResolverResolved={companyResolver.GetType().Name}");
        WriteLine($"CompanyResolverIsSqlServerCompanyResolver={companyResolver.GetType().Name == "SqlServerCompanyResolver"}");

        var dispatcher = scopedProvider.GetRequiredService<ISyncEventApplier>();
        WriteLine($"SyncEventApplierResolved={dispatcher.GetType().Name}");

        var entityAppliers = scopedProvider.GetServices<ISyncEntityEventApplier>().ToArray();
        WriteLine($"EntityApplierCount={entityAppliers.Length}");
        WriteLine($"WarehouseApplierRegistered={entityAppliers.Any(applier => applier.CanApply("Warehouse"))}");

        var warehouseApplyRepository = scopedProvider.GetRequiredService<IWarehouseSyncApplyRepository>();
        WriteLine($"WarehouseApplyRepositoryResolved={warehouseApplyRepository.GetType().Name}");

        var syncEvent = await outboxRepository.GetByIdAsync(1, DiagnosticOutboxId);
        WriteLine(syncEvent is null
            ? $"ReadOnlyOutbox{DiagnosticOutboxId}=FAILED NotFound"
            : $"ReadOnlyOutbox{DiagnosticOutboxId}=OK");

        if (syncEvent is not null)
        {
            WriteLine($"ReadOnlyOutboxId={syncEvent.Id}");
            WriteLine($"ReadOnlyOutboxEventId={syncEvent.EventId}");
            WriteLine($"ReadOnlyOutboxEntityName={syncEvent.EntityName}");
            WriteLine($"ReadOnlyOutboxEntityCode={syncEvent.EntityCode}");
            WriteLine($"ReadOnlyOutboxStatus={syncEvent.Status}");
            WriteLine($"ReadOnlyOutboxAttemptCount={syncEvent.AttemptCount}");
            WriteLine($"ReadOnlyOutboxLastErrorNull={syncEvent.LastErrorMessage is null}");
        }

        var targets = await outboxRepository.GetTargetsAsync(1, DiagnosticOutboxId);
        WriteLine($"ReadOnlyTargetCount={targets.Count}");
        foreach (var target in targets.Where(target => target.Id == DiagnosticTargetId))
        {
            WriteLine($"ReadOnlyTargetId={target.Id}");
            WriteLine($"ReadOnlyTargetBranchCompanyId={target.BranchCompanyId}");
            WriteLine($"ReadOnlyTargetStatus={target.Status}");
            WriteLine($"ReadOnlyTargetAttemptCount={target.AttemptCount}");
            WriteLine($"ReadOnlyTargetLastErrorNull={target.LastErrorMessage is null}");
        }

        var branch = await companyResolver.ResolveByIdAsync(2);
        WriteLine(branch is null ? "RouteResolveBranch=FAILED NullCompany" : "RouteResolveBranch=OK");

        if (branch is not null)
        {
            var branchBuilder = new SqlConnectionStringBuilder(branch.ConnectionString);
            WriteLine($"RouteBranchPolicyEncrypt={branchBuilder.Encrypt}");
            WriteLine($"RouteBranchPolicyTrustServerCertificate={branchBuilder.TrustServerCertificate}");

            await using var branchConnection = new SqlConnection(branch.ConnectionString);
            await branchConnection.OpenAsync();
            await using var branchCommand = branchConnection.CreateCommand();
            branchCommand.CommandText = "SELECT 1;";
            _ = await branchCommand.ExecuteScalarAsync();
            WriteLine("RouteBranchConnectionSelect1=OK");
        }

        if (syncEvent is not null)
        {
            var exists = await warehouseApplyRepository.ExistsByGlobalIdAsync(2, DiagnosticGlobalId);
            WriteLine($"WarehouseApplyRepositoryExistsByGlobalIdReadOnly={exists}");
        }

        Assert.NotNull(syncEvent);
        Assert.Equal(DiagnosticEventId, syncEvent.EventId);
        Assert.Equal("Warehouse", syncEvent.EntityName);
        Assert.Equal("BOD-SYNC-FINAL-001", syncEvent.EntityCode);
        Assert.Equal(SyncEventStatus.Applied, syncEvent.Status);
        Assert.Equal(1, syncEvent.AttemptCount);
        Assert.Contains(targets, target =>
            target.Id == DiagnosticTargetId &&
            target.BranchCompanyId == 2 &&
            target.Status == SyncEventStatus.Applied);
        Assert.Contains(entityAppliers, applier => applier.CanApply("Warehouse"));
    }

    [SqlServerIntegrationFact(DisplayName = "ReleaseExpiredLocksAsync diagnostico con rollback no persiste cambios")]
    [Trait("Category", "SqlServerIntegration")]
    public async Task ReleaseExpiredLocksAsync_DiagnosticRollback_DoesNotPersistChanges()
    {
        await using var provider = BuildWorkerServiceProvider(out var configuration);
        using var scope = provider.CreateScope();
        var masterFactory = scope.ServiceProvider.GetRequiredService<MasterConnectionFactory>();
        var repository = new SyncOutboxRepository(masterFactory);

        var before = await ReadDiagnosticStateAsync(masterFactory);
        WriteDiagnosticState("Before", before);

        await using var connection = masterFactory.CreateConnection();
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var affectedRows = await repository.ReleaseExpiredLocksAsync(connection, transaction);
            WriteLine($"ReleaseExpiredLocksRollbackResult=OK AffectedRows={affectedRows}");
            await transaction.RollbackAsync();
            WriteLine("ReleaseExpiredLocksRollbackExecuted=True");
        }
        catch (Exception exception)
        {
            WriteException("ReleaseExpiredLocksRollbackResult", exception);
            await transaction.RollbackAsync();
            WriteLine("ReleaseExpiredLocksRollbackExecuted=True");
            throw;
        }

        var after = await ReadDiagnosticStateAsync(masterFactory);
        WriteDiagnosticState("After", after);
        AssertDiagnosticStatePreserved(before, after);
        AssertDiagnosticEventClean(after);
    }

    [SqlServerIntegrationFact(DisplayName = "ClaimPendingAsync diagnostico con rollback puede reclamar sin persistir")]
    [Trait("Category", "SqlServerIntegration")]
    public async Task ClaimPendingAsync_DiagnosticRollback_CanClaimWithoutPersisting()
    {
        await using var provider = BuildWorkerServiceProvider(out var configuration);
        using var scope = provider.CreateScope();
        var masterFactory = scope.ServiceProvider.GetRequiredService<MasterConnectionFactory>();
        var repository = new SyncOutboxRepository(masterFactory);

        var before = await ReadDiagnosticStateAsync(masterFactory);
        WriteDiagnosticState("Before", before);

        await using var connection = masterFactory.CreateConnection();
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var claimed = await repository.ClaimPendingAsync(
                connection,
                transaction,
                "diagnostic-rollback",
                take: 1,
                TimeSpan.FromMinutes(5));
            var claimedList = claimed.ToArray();
            WriteLine($"ClaimPendingRollbackResult=OK ClaimedCount={claimedList.Length}");

            foreach (var item in claimedList)
            {
                WriteLine($"ClaimPendingRollbackClaimedId={item.Id}");
                WriteLine($"ClaimPendingRollbackClaimedEventId={item.EventId}");
                WriteLine($"ClaimPendingRollbackClaimedEntityName={item.EntityName}");
                WriteLine($"ClaimPendingRollbackClaimedEntityCode={item.EntityCode}");
                WriteLine($"ClaimPendingRollbackClaimedStatus={item.Status}");
                WriteLine($"ClaimPendingRollbackClaimedAttemptCount={item.AttemptCount}");
            }

            if (claimedList.Length > 0 && claimedList[0].Id != DiagnosticOutboxId)
            {
                WriteLine("ClaimPendingRollbackClaimedDifferentEvent=True");
                WriteLine("ClaimPendingRollbackDifferentReason=ClaimPendingAsync orders by CreatedAt, Id and does not force a specific event.");
            }

            await transaction.RollbackAsync();
            WriteLine("ClaimPendingRollbackExecuted=True");
        }
        catch (Exception exception)
        {
            WriteException("ClaimPendingRollbackResult", exception);
            await transaction.RollbackAsync();
            WriteLine("ClaimPendingRollbackExecuted=True");
            throw;
        }

        var after = await ReadDiagnosticStateAsync(masterFactory);
        WriteDiagnosticState("After", after);
        AssertDiagnosticStatePreserved(before, after);
        AssertDiagnosticEventClean(after);
    }

    private async Task DiagnoseMasterConnectionAsync(MasterConnectionFactory masterFactory)
    {
        try
        {
            await using var connection = masterFactory.CreateConnection();
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1;";
            _ = await command.ExecuteScalarAsync();
            WriteLine("MasterConnection=OK");
        }
        catch (Exception exception)
        {
            WriteException("MasterConnection", exception);
        }
    }

    private async Task DiagnoseBranchConnectionAsync(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);
        WriteLine($"BranchPolicyEncrypt={builder.Encrypt}");
        WriteLine($"BranchPolicyTrustServerCertificate={builder.TrustServerCertificate}");

        try
        {
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1;";
            _ = await command.ExecuteScalarAsync();
            WriteLine("BranchConnection=OK");
        }
        catch (Exception exception)
        {
            WriteException("BranchConnection", exception);
        }
    }

    private void WriteException(string label, Exception exception)
    {
        WriteLine($"{label}=FAILED");
        WriteLine($"{label}ExceptionType={exception.GetType().FullName}");
        WriteLine($"{label}ExceptionMessage={Sanitize(exception.Message)}");

        var stack = exception.StackTrace?
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Take(6)
            .Select(Sanitize)
            .ToArray() ?? [];

        for (var index = 0; index < stack.Length; index++)
        {
            WriteLine($"{label}Stack{index + 1}={stack[index]}");
        }
    }

    private void WriteSanitizedConnectionProperties(
        string prefix,
        string? concreteConnectionType,
        SqlConnectionStringBuilder builder)
    {
        WriteLine($"{prefix}_ConcreteConnectionType={concreteConnectionType ?? "NULL"}");
        WriteLine($"{prefix}_DataSourcePresent={!string.IsNullOrWhiteSpace(builder.DataSource)}");
        WriteLine($"{prefix}_InitialCatalog={builder.InitialCatalog}");
        WriteLine($"{prefix}_IntegratedSecurity={builder.IntegratedSecurity}");
        WriteLine($"{prefix}_UserIdPresent={!string.IsNullOrWhiteSpace(builder.UserID)}");
        WriteLine($"{prefix}_PasswordPresent={!string.IsNullOrWhiteSpace(builder.Password)}");
        WriteLine($"{prefix}_Encrypt={builder.Encrypt}");
        WriteLine($"{prefix}_TrustServerCertificate={builder.TrustServerCertificate}");
        WriteLine($"{prefix}_MultipleActiveResultSets={builder.MultipleActiveResultSets}");
        WriteLine($"{prefix}_ConnectTimeout={builder.ConnectTimeout}");
        WriteLine($"{prefix}_ApplicationName={SanitizeOptional(builder.ApplicationName)}");
    }

    private static string SanitizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "NULL" : Sanitize(value);
    }

    private void WriteLine(string message)
    {
        _output.WriteLine(message);
        Console.WriteLine(message);
    }

    private void WriteProcessEnvironmentDiagnostics(string prefix)
    {
        var currentProcess = Process.GetCurrentProcess();
        var sqlClientAssembly = typeof(SqlConnection).Assembly;

        WriteLine($"{prefix}_ProcessName={currentProcess.ProcessName}");
        WriteLine($"{prefix}_ProcessPath={Environment.ProcessPath ?? "NULL"}");
        WriteLine($"{prefix}_ProcessId={Environment.ProcessId}");
        WriteLine($"{prefix}_CurrentDirectory={Directory.GetCurrentDirectory()}");
        WriteLine($"{prefix}_AppContextBaseDirectory={AppContext.BaseDirectory}");
        WriteLine($"{prefix}_DOTNET_ROOT={Environment.GetEnvironmentVariable("DOTNET_ROOT") ?? "NULL"}");
        WriteLine($"{prefix}_DOTNET_ENVIRONMENT={Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "NULL"}");
        WriteLine($"{prefix}_ASPNETCORE_ENVIRONMENT={Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "NULL"}");
        WriteLine($"{prefix}_FrameworkDescription={RuntimeInformation.FrameworkDescription}");
        WriteLine($"{prefix}_OSDescription={RuntimeInformation.OSDescription}");
        WriteLine($"{prefix}_ProcessArchitecture={RuntimeInformation.ProcessArchitecture}");
        WriteLine($"{prefix}_Is64BitProcess={Environment.Is64BitProcess}");
        WriteLine($"{prefix}_SqlClientAssemblyLocation={sqlClientAssembly.Location}");
        WriteLine($"{prefix}_SqlClientAssemblyVersion={sqlClientAssembly.GetName().Version?.ToString() ?? "NULL"}");
        WriteLine($"{prefix}_SqlClientPackageVersion={sqlClientAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "NULL"}");
        WriteLine($"{prefix}_SystemSecurityCryptographyAssembly={typeof(System.Security.Cryptography.SHA256).Assembly.Location}");
        WriteLine($"{prefix}_PathLength={Environment.GetEnvironmentVariable("PATH")?.Length ?? 0}");
        WriteLine($"{prefix}_SniNativeDependencyAvailable={HasSniNativeDependency(AppContext.BaseDirectory)}");
        WriteLine($"{prefix}_SniNativeDependencyLoaded={IsSniNativeDependencyLoaded()}");
        WriteLine($"{prefix}_CurrentUserName={SanitizeUserName(Environment.UserName)}");
    }

    private static bool HasSniNativeDependency(string baseDirectory)
    {
        return Directory.EnumerateFiles(baseDirectory, "*SNI*.dll", SearchOption.TopDirectoryOnly).Any();
    }

    private static bool IsSniNativeDependencyLoaded()
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

    private static string SanitizeUserName(string? userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            return "NULL";
        }

        var slashIndex = userName.LastIndexOf('\\');
        return slashIndex >= 0 ? userName[(slashIndex + 1)..] : userName;
    }

    private static string Sanitize(string value)
    {
        return value
            .ReplaceLineEndings(" ")
            .Replace("Password=", "Password=<redacted>", StringComparison.OrdinalIgnoreCase)
            .Replace("User ID=", "User ID=<redacted>", StringComparison.OrdinalIgnoreCase)
            .Replace("User Id=", "User Id=<redacted>", StringComparison.OrdinalIgnoreCase);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("No se encontro NuanSystem.sln desde el directorio de pruebas.");
    }

    private static ServiceProvider BuildMasterConnectionFactoryProvider(
        IReadOnlyDictionary<string, string?> settings)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddPersistenceServices(configuration);

        return services.BuildServiceProvider();
    }

    private static string BuildAdminConnectionString(
        bool? multipleActiveResultSets = null,
        bool? integratedSecurity = null)
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = "localhost",
            InitialCatalog = "master",
            UserID = "test-user",
            Password = "test-password"
        };

        if (multipleActiveResultSets.HasValue)
        {
            builder.MultipleActiveResultSets = multipleActiveResultSets.Value;
        }

        if (integratedSecurity.HasValue)
        {
            builder.IntegratedSecurity = integratedSecurity.Value;
        }

        return builder.ConnectionString;
    }

    private static SqlConnectionStringBuilder CreateMasterConnectionStringBuilder(
        IServiceProvider provider)
    {
        var factory = provider.GetRequiredService<IMasterConnectionFactory>();
        using var connection = factory.CreateConnection();
        return new SqlConnectionStringBuilder(connection.ConnectionString);
    }

    private static ServiceProvider BuildWorkerServiceProvider(out IConfiguration configuration)
    {
        var repositoryRoot = FindRepositoryRoot();
        var workerDirectory = Path.Combine(repositoryRoot, "src", "Backend", "NuanSystem.MasterBranchSyncWorker");
        var args = Array.Empty<string>();

        configuration = new ConfigurationBuilder()
            .SetBasePath(workerDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services
            .AddApplicationServices()
            .AddInfrastructureServices()
            .AddPersistenceServices(configuration);

        services.Configure<MasterBranchSyncWorkerOptions>(
            configuration.GetSection(MasterBranchSyncWorkerOptions.SectionName));
        services.AddScoped<ISyncEntityEventApplier, BusinessPartnerSyncEventApplier>();
        services.AddScoped<ISyncEntityEventApplier, ItemSyncEventApplier>();
        services.AddScoped<ISyncEntityEventApplier, WarehouseSyncEventApplier>();
        services.AddScoped<ISyncEventApplier, SyncEventApplierDispatcher>();
        services.AddScoped<IMasterBranchSyncWorkerProcessor, MasterBranchSyncWorkerProcessor>();

        return services.BuildServiceProvider();
    }

    private static async Task<DiagnosticState> ReadDiagnosticStateAsync(MasterConnectionFactory masterFactory)
    {
        await using var connection = masterFactory.CreateConnection();
        await connection.OpenAsync();

        var outbox = await ReadOutboxStateAsync(connection);
        var target = await ReadTargetStateAsync(connection);
        var branchWarehouseCount = await ExecuteScalarAsync<int>(
            connection,
            "SELECT COUNT(1) FROM NuanSystem_SYNC_WH_BRANCH_TEST.dbo.Warehouses WHERE Code = N'BOD-SYNC-FINAL-001';");
        var inboxCount = await ExecuteScalarAsync<int>(
            connection,
            $"SELECT COUNT(1) FROM NuanSystem_SYNC_WH_BRANCH_TEST.dbo.SyncInbox WHERE EventId = '{DiagnosticEventId}';");

        return new DiagnosticState(outbox, target, branchWarehouseCount, inboxCount);
    }

    private static async Task<OutboxState> ReadOutboxStateAsync(SqlConnection connection)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = $"""
SELECT Id, Status, AttemptCount, LastErrorMessage, LockedBy, LockedAt, LockExpiresAt
FROM dbo.SyncOutbox
WHERE Id = {DiagnosticOutboxId};
""";

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            throw new InvalidOperationException($"SyncOutboxId {DiagnosticOutboxId} no existe.");
        }

        return new OutboxState(
            reader.GetInt64(reader.GetOrdinal("Id")),
            reader.GetString(reader.GetOrdinal("Status")),
            reader.GetInt32(reader.GetOrdinal("AttemptCount")),
            reader.IsDBNull(reader.GetOrdinal("LastErrorMessage")),
            reader.IsDBNull(reader.GetOrdinal("LockedBy")),
            reader.IsDBNull(reader.GetOrdinal("LockedAt")),
            reader.IsDBNull(reader.GetOrdinal("LockExpiresAt")));
    }

    private static async Task<TargetState> ReadTargetStateAsync(SqlConnection connection)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = $"""
SELECT Id, Status, AttemptCount, LastErrorMessage
FROM dbo.SyncOutboxTargets
WHERE Id = {DiagnosticTargetId};
""";

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            throw new InvalidOperationException($"TargetId {DiagnosticTargetId} no existe.");
        }

        return new TargetState(
            reader.GetInt64(reader.GetOrdinal("Id")),
            reader.GetString(reader.GetOrdinal("Status")),
            reader.GetInt32(reader.GetOrdinal("AttemptCount")),
            reader.IsDBNull(reader.GetOrdinal("LastErrorMessage")));
    }

    private static async Task<T> ExecuteScalarAsync<T>(SqlConnection connection, string commandText)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = commandText;
        var value = await command.ExecuteScalarAsync();
        return (T)Convert.ChangeType(value!, typeof(T));
    }

    private void WriteDiagnosticState(string prefix, DiagnosticState state)
    {
        WriteLine($"{prefix}SyncOutboxId={state.Outbox.Id}");
        WriteLine($"{prefix}SyncOutboxStatus={state.Outbox.Status}");
        WriteLine($"{prefix}SyncOutboxAttemptCount={state.Outbox.AttemptCount}");
        WriteLine($"{prefix}SyncOutboxLastErrorNull={state.Outbox.LastErrorNull}");
        WriteLine($"{prefix}SyncOutboxLockedByNull={state.Outbox.LockedByNull}");
        WriteLine($"{prefix}SyncOutboxLockedAtNull={state.Outbox.LockedAtNull}");
        WriteLine($"{prefix}SyncOutboxLockExpiresAtNull={state.Outbox.LockExpiresAtNull}");
        WriteLine($"{prefix}TargetId={state.Target.Id}");
        WriteLine($"{prefix}TargetStatus={state.Target.Status}");
        WriteLine($"{prefix}TargetAttemptCount={state.Target.AttemptCount}");
        WriteLine($"{prefix}TargetLastErrorNull={state.Target.LastErrorNull}");
        WriteLine($"{prefix}BranchWarehouseCodeCount={state.BranchWarehouseCodeCount}");
        WriteLine($"{prefix}BranchSyncInboxEventCount={state.BranchSyncInboxEventCount}");
    }

    private static void AssertDiagnosticStatePreserved(DiagnosticState before, DiagnosticState after)
    {
        Assert.Equal(before, after);
    }

    private static void AssertDiagnosticEventClean(DiagnosticState state)
    {
        Assert.Equal(DiagnosticOutboxId, state.Outbox.Id);
        Assert.Equal("Applied", state.Outbox.Status);
        Assert.Equal(1, state.Outbox.AttemptCount);
        Assert.True(state.Outbox.LastErrorNull);
        Assert.True(state.Outbox.LockedByNull);
        Assert.True(state.Outbox.LockedAtNull);
        Assert.True(state.Outbox.LockExpiresAtNull);
        Assert.Equal(DiagnosticTargetId, state.Target.Id);
        Assert.Equal("Applied", state.Target.Status);
        Assert.Equal(1, state.Target.AttemptCount);
        Assert.True(state.Target.LastErrorNull);
        Assert.Equal(1, state.BranchWarehouseCodeCount);
        Assert.Equal(1, state.BranchSyncInboxEventCount);
    }

    private sealed record DiagnosticState(
        OutboxState Outbox,
        TargetState Target,
        int BranchWarehouseCodeCount,
        int BranchSyncInboxEventCount);

    private sealed record OutboxState(
        long Id,
        string Status,
        int AttemptCount,
        bool LastErrorNull,
        bool LockedByNull,
        bool LockedAtNull,
        bool LockExpiresAtNull);

    private sealed record TargetState(
        long Id,
        string Status,
        int AttemptCount,
        bool LastErrorNull);
}
