using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NuanSystem.Api.Endpoints;
using NuanSystem.Api.Middleware;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Diagnostics;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.DependencyInjection;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Dtos;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Services;
using NuanSystem.Application.Features.SecurityAccess.Dtos;
using NuanSystem.Application.Tests.Infrastructure;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Infrastructure.Diagnostics;
using NuanSystem.Persistence.Diagnostics;
using NuanSystem.Persistence.Tenancy;
using NuanSystem.Shared.Constants;
using NuanSystem.Shared.Contracts.Auth;
using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Purchasing.PurchaseOrders;
using NuanSystem.WinForms.Services.Session;

namespace NuanSystem.Application.Tests.Features.Integration;

internal sealed class BackendApiFixture : IAsyncDisposable
{
    private WebApplication app = null!;
    public string? LastDiagnostic { get; private set; }
    public bool AllowForm { get; set; } = true;
    public bool AllowCompany { get; set; } = true;
    public bool AllowSapPermission { get; set; } = true;
    public IPurchaseOrderRepository Orders { get; } = Substitute.For<IPurchaseOrderRepository>();
    public ISapSupplierReader Reader { get; } = Substitute.For<ISapSupplierReader>();
    public ISapSupplierImportRepository Imports { get; } = Substitute.For<ISapSupplierImportRepository>();
    public IBusinessPartnerRepository Partners { get; } = Substitute.For<IBusinessPartnerRepository>();
    public ISapSyncLogRepository SapLog { get; } = Substitute.For<ISapSyncLogRepository>();
    public ISecurityDocumentNumberingService Numbering { get; } = Substitute.For<ISecurityDocumentNumberingService>();
    public List<int> RepositoryCompanyIds { get; } = [];
    public ApiSession Session { get; } = new();
    public NuanApiClient Api { get; private set; } = null!;
    public PurchaseOrderClient PurchaseOrders { get; private set; } = null!;
    public HttpClient Http { get; private set; } = null!;

    public static async Task<BackendApiFixture> CreateAsync()
    {
        var fixture = new BackendApiFixture();
        await fixture.InitializeAsync();
        return fixture;
    }

    private async Task InitializeAsync()
    {
        var builder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions
        { EnvironmentName = "Phase9", ApplicationName = typeof(PurchaseOrderEndpoints).Assembly.FullName });
        builder.Configuration.Sources.Clear();
        builder.Logging.ClearProviders();
        var server = new InProcessApiServer();
        builder.Services.AddSingleton<IServer>(server);
        builder.Services.AddRouting();
        builder.Services.AddLogging();
        builder.Services.AddSingleton(this);
        builder.Services.AddApplicationServices();
        builder.Services.AddSingleton<IErrorSanitizer, ErrorSanitizer>();
        builder.Services.AddSingleton<IDependencyFailureClassifier, DependencyFailureClassifier>();
        builder.Services.AddScoped<IExceptionResponseMapper, ExceptionResponseMapper>();
        builder.Services.AddScoped<IExceptionAuditWriter, ExceptionAuditWriter>();
        builder.Services.AddScoped<ICompanyContext, CompanyContext>();
        var resolver = Substitute.For<ICompanyResolver>();
        resolver.ResolveByCodeForUserAsync(Arg.Any<string>(), 42, Arg.Any<CancellationToken>()).Returns(call =>
            AllowCompany && call.Arg<string>() is "TEST-1" or "TEST-2"
                ? new CompanyConnectionInfo(call.Arg<string>() == "TEST-1" ? 1 : 2, call.Arg<string>(), "Test",
                    DatabaseEngine.SqlServer, "UNUSED-SECRET-CONNECTION", SapIntegrationMode.None) : null);
        builder.Services.AddSingleton(resolver);
        builder.Services.AddScoped<IPurchaseOrderRepository>(provider =>
        {
            RepositoryCompanyIds.Add(provider.GetRequiredService<ICompanyContext>().CurrentCompany!.CompanyId);
            return Orders;
        });
        var access = Substitute.For<ISecurityAccessRepository>();
        access.GetFormOperationsAsync(42, Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(_ => AllowForm
            ? new[] { "consult", "create", "refresh", "update" }.Select(action => new FormOperationAccessDto(
                1, action, action, null, action, null, null, null, null, 0, true)).ToArray() : []);
        builder.Services.AddSingleton(access);
        var audit = Substitute.For<IAuditLogRepository>();
        audit.AddErrorAsync(Arg.Any<NuanSystem.Application.Features.Audit.Dtos.CreateAuditErrorLogData>(), Arg.Any<CancellationToken>())
            .Returns(call => { LastDiagnostic = call.Arg<NuanSystem.Application.Features.Audit.Dtos.CreateAuditErrorLogData>().ErrorMessage; return Task.CompletedTask; });
        builder.Services.AddSingleton(audit);
        var seriesAccess = Substitute.For<ISecurityDocumentSeriesAccessRepository>();
        seriesAccess.ValidateUserOperationAsync(42, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), 3,
            Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);
        builder.Services.AddSingleton(seriesAccess);
        var series = Substitute.For<ISecurityDocumentSeriesRepository>();
        series.GetByIdAsync(3, Arg.Any<CancellationToken>()).Returns(new SecurityDocumentSeriesDto
        { Id = 3, Code = "AUTHORIZED", DocumentType = NuanSystem.Application.Features.Purchasing.PurchaseOrders.PurchaseOrderSecurity.DocumentType, IsActive = true });
        builder.Services.AddSingleton(series);
        Numbering.ReserveNumberAsync(3, 42, "phase9-user", Arg.Any<CancellationToken>()).Returns(new ReserveSecurityDocumentNumberResult(true, 42, "RESERVED-42", "42", "Reserved"));
        builder.Services.AddSingleton(Numbering);
        builder.Services.AddSingleton(Reader).AddSingleton(Imports).AddSingleton(Partners).AddSingleton(SapLog);
        builder.Services.AddScoped<ISapSupplierImportService, SapSupplierImportService>();
        builder.Services.AddSingleton(Substitute.For<NuanSystem.Application.Abstractions.SapSync.ISapSyncWatermarkRepository>());
        Partners.GetAllAsync(null, Arg.Any<CancellationToken>()).Returns(Array.Empty<BusinessPartnerDto>());
        Reader.GetSuppliersAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(new[]
        { new SapSupplierRecord("EXT-SAME", "Test supplier", "099001", "S", null, null, null, "USD", true, null, null) });
        Imports.ImportSupplierFromSapAsync(Arg.Any<BusinessPartnerSapImportData>(), Arg.Any<CancellationToken>())
            .Returns(new BusinessPartnerSapImportResultData(17, "Created", "Created"));
        builder.Services.AddAuthentication("Phase9").AddScheme<AuthenticationSchemeOptions, ControlledAuthentication>("Phase9", _ => { });
        builder.Services.AddAuthorization(options =>
        {
            foreach (var permission in PermissionCodes.All)
                options.AddPolicy(permission, policy => policy.RequireAuthenticatedUser().RequireClaim(AuthClaimNames.Permission, permission));
        });
        app = builder.Build();
        app.UseMiddleware<GlobalExceptionMiddleware>();
        app.UseAuthentication();
        app.UseMiddleware<CompanyContextMiddleware>();
        app.UseAuthorization();
        app.MapPurchaseOrderEndpoints();
        app.MapSapEndpoints();
        await app.StartAsync();
        Http = new HttpClient(server.CreateHandler()) { BaseAddress = new Uri("http://phase9.invalid") };
        Session.SetUser(new LoginResponse(42, "phase9-user", "Phase9", "phase9-token", DateTime.UtcNow.AddHours(1), false, [], PermissionCodes.All.ToArray(), []));
        Session.SelectCompany(new UserCompanyResponse(1, "TEST-1", "Test", null, null, null));
        Api = new NuanApiClient(Http, Session);
        PurchaseOrders = new PurchaseOrderClient(Api);
    }

    public async ValueTask DisposeAsync()
    {
        Http?.Dispose();
        if (app is not null) { await app.StopAsync(); await app.DisposeAsync(); }
    }

    private sealed class ControlledAuthentication(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, BackendApiFixture fixture) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Headers.Authorization != "Bearer phase9-token") return Task.FromResult(AuthenticateResult.NoResult());
            var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, "42"), new(ClaimTypes.Name, "phase9-user") };
            claims.AddRange(PermissionCodes.All.Where(permission => fixture.AllowSapPermission || permission != PermissionCodes.SapManage)
                .Select(permission => new Claim(AuthClaimNames.Permission, permission)));
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name)), Scheme.Name)));
        }
    }
}
