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

    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/nuansystem-api-.log", rollingInterval: RollingInterval.Day);
    });

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
    app.MapGridColumnSettingsEndpoints();
    app.MapAuditEndpoints();

    app.MapAccountingEndpoints();
    app.MapBusinessPartnerEndpoints();
    app.MapFinancialCatalogEndpoints();
    app.MapGeneralSupplierEndpoints();
    app.MapGeographyEndpoints();
    app.MapInventoryCatalogEndpoints();
    app.MapPurchaseOrderEndpoints();
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
