using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Data.SqlClient;
using NuanSystem.Api.Endpoints;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Shared.Responses;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

    var diagnosticsAndExit = args.Contains("--diagnostics-and-exit", StringComparer.OrdinalIgnoreCase) ||
        builder.Configuration.GetValue<bool>("Diagnostics:ProcessEnvironmentAndExit");

    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/nuansystem-api-.log", rollingInterval: RollingInterval.Day);
    });

    if (diagnosticsAndExit)
    {
        LogProcessEnvironmentDiagnostics("C_Api");
        return;
    }

    builder.Services.AddNuanSystemServices(builder.Configuration);

    var app = builder.Build();

    var initOnly = args.Contains("--init-only", StringComparer.OrdinalIgnoreCase);
    var initializeMasterOnStartup = builder.Configuration.GetValue<bool>("DatabaseInitialization:InitializeMasterOnStartup");
    if (initOnly || initializeMasterOnStartup)
    {
        await InitializeMasterDatabaseAsync(app.Services, app.Lifetime.ApplicationStopping);
    }

    if (initOnly)
    {
        Log.Information("Inicializacion de base master completada. Finalizando por --init-only.");
        return;
    }

    app.UseGlobalExceptionHandling();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "NuanSystem API v1");
            options.RoutePrefix = "swagger";
        });
    }

    app.UseHttpsRedirection();
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseRequiredPasswordChange();
    app.UseCompanyContext();
    app.UseAuthorization();
    app.UseAuditLogging();

    app.MapHealthChecks("/health");
    app.MapGet("/", () => ApiResponse<object>.Ok(new
    {
        Service = "NuanSystem.Api",
        Status = "Running",
        Swagger = "/swagger"
    }));

    app.MapAuthEndpoints();
    app.MapCompanyEndpoints();
    app.MapConfigurationCompanyEndpoints();
    app.MapConfigurationSettingEndpoints();
    app.MapTenantConfigurationEndpoints();
    app.MapTenancyEndpoints();
    app.MapSapEndpoints();
    app.MapSettingsEndpoints();
    app.MapUserEndpoints();
    app.MapRoleEndpoints();
    app.MapSecurityOperationEndpoints();
    app.MapSecurityMenuEndpoints();
    app.MapSecurityFormEndpoints();
    app.MapSecurityFieldEndpoints();
    app.MapSecurityAccessEndpoints();
    app.MapSecurityRoleFormAccessEndpoints();
    app.MapSecurityRoleFormFieldAccessEndpoints();
    app.MapSecurityDocumentSeriesAccessEndpoints();
    app.MapGridColumnSettingsEndpoints();
    app.MapAuditEndpoints();
    app.MapSyncEndpoints();
    app.MapSyncConfigurationEndpoints();

    app.MapAccountingEndpoints();
    app.MapBusinessPartnerEndpoints();
    app.MapFinancialCatalogEndpoints();
    app.MapGeneralSupplierEndpoints();
    app.MapGeographyEndpoints();
    app.MapInventoryCatalogEndpoints();
    app.MapOperationalCatalogEndpoints();
    app.MapPurchaseOrderEndpoints();
    app.MapSecurityDocumentSeriesEndpoints();
    app.MapTaxCatalogEndpoints();

    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "NuanSystem API finalizo inesperadamente.");
    Environment.ExitCode = 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}

static async Task InitializeMasterDatabaseAsync(IServiceProvider services, CancellationToken cancellationToken)
{
    using var scope = services.CreateScope();
    var initializer = scope.ServiceProvider.GetRequiredService<IMasterDatabaseInitializer>();
    await initializer.InitializeAsync(cancellationToken);
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
        typeof(System.Security.Cryptography.SHA256).Assembly.Location,
        Environment.GetEnvironmentVariable("PATH")?.Length ?? 0,
        HasSniNativeDependency(AppContext.BaseDirectory),
        IsSniNativeDependencyLoaded(),
        SanitizeUserName(Environment.UserName));
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
