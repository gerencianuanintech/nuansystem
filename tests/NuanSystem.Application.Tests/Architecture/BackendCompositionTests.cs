using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.DependencyInjection;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Infrastructure.DependencyInjection;
using NuanSystem.Persistence.Connections;
using NuanSystem.Persistence.DependencyInjection;
using NuanSystem.SapIntegration.DependencyInjection;

namespace NuanSystem.Application.Tests.Architecture;

public sealed class BackendCompositionTests
{
    [Fact]
    public void Real_backend_registrations_compose_all_application_handlers_without_external_IO()
    {
        var barrier = new ExternalIoBarrier();
        using var host = BuildUnstartedHost(barrier);
        using var scope = host.Services.CreateScope();
        var handlerContracts = typeof(ApplicationServiceRegistration).Assembly.GetTypes()
            .Where(type => !type.IsAbstract && !type.ContainsGenericParameters)
            .SelectMany(type => type.GetInterfaces())
            .Where(BackendDependencyTests.IsRequestHandler).Distinct().ToArray();
        Assert.NotEmpty(handlerContracts);

        foreach (var contract in handlerContracts)
            Assert.NotNull(scope.ServiceProvider.GetRequiredService(contract));

        Assert.NotEmpty(scope.ServiceProvider.GetServices<ISapSyncScheduledExecutionProcessor>());
        Assert.NotEmpty(scope.ServiceProvider.GetServices<ISapSyncExecutionRetryProcessor>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<ISapSyncScheduler>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<MasterConnectionFactory>());
        Assert.Equal(0, barrier.ConnectionAttempts);
        Assert.Equal(0, barrier.HttpSendAttempts);
    }

    [Theory]
    [InlineData("Suppliers")]
    [InlineData("Items")]
    [InlineData("PaymentTerms")]
    [InlineData("Warehouses")]
    [InlineData("Countries")]
    [InlineData("Provinces")]
    [InlineData("Cities")]
    public async Task Real_importers_honor_cancellation_without_external_IO(string entityCode)
    {
        var barrier = new ExternalIoBarrier();
        using var host = BuildUnstartedHost(barrier);
        using var scope = host.Services.CreateScope();
        var registry = scope.ServiceProvider.GetRequiredService<ISapSyncProcessorRegistry>();
        var importers = scope.ServiceProvider.GetServices<ISapSyncImporter>().ToArray();
        Assert.Equal(7, importers.Length);
        Assert.Empty(scope.ServiceProvider.GetServices<ISapSyncExporter>());
        var importer = Assert.Single(importers, item => item.EntityCode == entityCode);
        Assert.Same(importer, registry.FindImporter(entityCode));
        Assert.False(importer is ISapSyncExporter);
        Assert.NotEmpty(importer.SupportedModes);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var context = new NuanSystem.Application.Features.SapSync.Dtos.SapSyncExecutionContext(
            1, "TEST", entityCode,
            NuanSystem.Application.Features.SapSync.Enums.SapSyncDirection.SapToErp,
            NuanSystem.Application.Features.SapSync.Enums.SapSyncOperation.Import,
            "test-worker", "test-correlation", 0, DateTime.UtcNow);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => importer.ImportFromSapAsync(context, cancellation.Token));
        Assert.Equal(0, barrier.ConnectionAttempts);
        Assert.Equal(0, barrier.HttpSendAttempts);
        var expectedScheduled = new[] { "Warehouses", "Countries", "Provinces", "Cities" }.Contains(entityCode);
        Assert.Equal(expectedScheduled, registry.SupportsScheduled(entityCode, context.Direction, "Full"));
        Assert.False(registry.SupportsScheduled(entityCode, context.Direction, "Incremental"));
    }

    [Fact]
    public void Business_partner_crud_contract_does_not_expose_SAP_import()
    {
        Assert.Null(typeof(IBusinessPartnerRepository).GetMethod("ImportSupplierFromSapAsync"));
    }

    [Fact]
    public void Business_partner_repository_contracts_share_one_instance_per_scope_without_external_IO()
    {
        var barrier = new ExternalIoBarrier();
        using var host = BuildUnstartedHost(barrier);
        Assert.Throws<InvalidOperationException>(() => host.Services.GetRequiredService<NuanSystem.Application.Abstractions.Sap.ISapSupplierImportRepository>());
        using var first = host.Services.CreateScope();
        using var second = host.Services.CreateScope();
        var firstCrud = first.ServiceProvider.GetRequiredService<IBusinessPartnerRepository>();
        var firstImport = first.ServiceProvider.GetRequiredService<NuanSystem.Application.Abstractions.Sap.ISapSupplierImportRepository>();
        Assert.Same(firstCrud, firstImport);
        Assert.Same(firstCrud, first.ServiceProvider.GetRequiredService<NuanSystem.Persistence.Repositories.BusinessPartnerRepository>());
        Assert.Same(firstImport, first.ServiceProvider.GetRequiredService<NuanSystem.Application.Abstractions.Sap.ISapSupplierImportRepository>());
        var secondCrud = second.ServiceProvider.GetRequiredService<IBusinessPartnerRepository>();
        var secondImport = second.ServiceProvider.GetRequiredService<NuanSystem.Application.Abstractions.Sap.ISapSupplierImportRepository>();
        Assert.Same(secondCrud, secondImport);
        Assert.NotSame(firstImport, secondImport);
        Assert.NotSame(firstCrud, secondCrud);
        Assert.Equal(0, barrier.ConnectionAttempts);
        Assert.Equal(0, barrier.HttpSendAttempts);
    }

    [Fact]
    public void Business_partner_unit_of_work_is_scoped_and_handlers_do_not_accept_ADO_contracts()
    {
        var barrier = new ExternalIoBarrier();
        using var host = BuildUnstartedHost(barrier);
        Assert.Throws<InvalidOperationException>(() => host.Services.GetRequiredService<IBusinessPartnerUnitOfWork>());
        using var first = host.Services.CreateScope();
        using var second = host.Services.CreateScope();
        Assert.Same(first.ServiceProvider.GetRequiredService<IBusinessPartnerUnitOfWork>(),
            first.ServiceProvider.GetRequiredService<IBusinessPartnerUnitOfWork>());
        Assert.NotSame(first.ServiceProvider.GetRequiredService<IBusinessPartnerUnitOfWork>(),
            second.ServiceProvider.GetRequiredService<IBusinessPartnerUnitOfWork>());
        foreach (var type in new[] {
            typeof(NuanSystem.Application.Features.BusinessPartners.Commands.CreateBusinessPartnerCommandHandler),
            typeof(NuanSystem.Application.Features.BusinessPartners.Commands.UpdateBusinessPartnerCommandHandler),
            typeof(NuanSystem.Application.Features.BusinessPartners.Commands.DeleteBusinessPartnerCommandHandler) })
            Assert.Equal(typeof(IBusinessPartnerUnitOfWork), Assert.Single(Assert.Single(type.GetConstructors()).GetParameters()).ParameterType);
        Assert.Equal(0, barrier.ConnectionAttempts);
        Assert.Equal(0, barrier.HttpSendAttempts);
    }

    [Fact]
    public void Tenant_context_and_repositories_are_scoped_and_cannot_be_resolved_from_root()
    {
        var barrier = new ExternalIoBarrier();
        using var host = BuildUnstartedHost(barrier);
        Assert.Throws<InvalidOperationException>(() => host.Services.GetRequiredService<ICompanyContext>());
        Assert.Throws<InvalidOperationException>(() => host.Services.GetRequiredService<IBusinessPartnerRepository>());

        using var first = host.Services.CreateScope();
        using var second = host.Services.CreateScope();
        var firstContext = first.ServiceProvider.GetRequiredService<ICompanyContext>();
        var secondContext = second.ServiceProvider.GetRequiredService<ICompanyContext>();
        Assert.Same(firstContext, first.ServiceProvider.GetRequiredService<ICompanyContext>());
        Assert.NotSame(firstContext, secondContext);
        Assert.False(firstContext.HasActiveCompany);
        Assert.False(secondContext.HasActiveCompany);

        firstContext.SetCurrentCompany(new CompanyConnectionInfo(
            1, "PHASE1-A", "Test company A", DatabaseEngine.SqlServer, string.Empty, SapIntegrationMode.None));
        Assert.True(firstContext.HasActiveCompany);
        Assert.False(secondContext.HasActiveCompany);
        secondContext.SetCurrentCompany(new CompanyConnectionInfo(
            2, "PHASE1-B", "Test company B", DatabaseEngine.SqlServer, string.Empty, SapIntegrationMode.None));
        Assert.Equal("PHASE1-A", firstContext.CurrentCompany!.CompanyCode);
        Assert.Equal("PHASE1-B", secondContext.CurrentCompany!.CompanyCode);

        var firstRepository = first.ServiceProvider.GetRequiredService<IBusinessPartnerRepository>();
        Assert.Same(firstRepository, first.ServiceProvider.GetRequiredService<IBusinessPartnerRepository>());
        Assert.NotSame(firstRepository, second.ServiceProvider.GetRequiredService<IBusinessPartnerRepository>());
        Assert.Equal(0, barrier.ConnectionAttempts);
        Assert.Equal(0, barrier.HttpSendAttempts);
    }

    [Fact]
    public void Production_connection_registrations_exist_before_test_IO_boundaries_are_replaced()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        services.AddPersistenceServices(configuration);

        var master = Assert.Single(services, service => service.ServiceType == typeof(IMasterConnectionFactory));
        Assert.Equal(ServiceLifetime.Scoped, master.Lifetime);
        Assert.NotNull(master.ImplementationFactory);
        var tenant = Assert.Single(services, service => service.ServiceType == typeof(ITenantConnectionFactory));
        Assert.Equal(ServiceLifetime.Scoped, tenant.Lifetime);
        Assert.Equal(typeof(TenantConnectionFactory), tenant.ImplementationType);
    }

    [Fact]
    public async Task Test_IO_boundaries_fail_closed_instead_of_contacting_SQL_or_HTTP()
    {
        var barrier = new ExternalIoBarrier();
        Assert.Throws<InvalidOperationException>(() => ((IMasterConnectionFactory)barrier).CreateConnection());
        Assert.Throws<InvalidOperationException>(() => ((ITenantConnectionFactory)barrier).CreateConnection());
        using var client = barrier.CreateClient("SapServiceLayer");
        await Assert.ThrowsAsync<InvalidOperationException>(() => client.GetAsync("https://phase1.invalid/"));
        Assert.Equal(2, barrier.ConnectionAttempts);
        Assert.Equal(1, barrier.HttpSendAttempts);
    }

    private static IHost BuildUnstartedHost(ExternalIoBarrier barrier)
    {
        // Generic Host is built but never started: no API Program, hosted worker,
        // JSON settings, environment configuration, secrets or business methods.
        return new HostBuilder()
            .ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.Sources.Clear();
                configuration.AddInMemoryCollection();
            })
            .UseDefaultServiceProvider(options =>
            {
                options.ValidateOnBuild = true;
                options.ValidateScopes = true;
            })
            .ConfigureServices((context, services) =>
            {
                services.AddApplicationServices();
                services.AddInfrastructureServices();
                services.AddPersistenceServices(context.Configuration);
                services.AddSapIntegrationServices(context.Configuration);
                services.Replace(ServiceDescriptor.Scoped<IMasterConnectionFactory>(_ => barrier));
                services.Replace(ServiceDescriptor.Scoped<ITenantConnectionFactory>(_ => barrier));
                services.Replace(ServiceDescriptor.Singleton<IHttpClientFactory>(barrier));
            })
            .Build();
    }

    private sealed class ExternalIoBarrier : IMasterConnectionFactory, ITenantConnectionFactory, IHttpClientFactory
    {
        private int connectionAttempts;
        private int httpSendAttempts;
        public int ConnectionAttempts => Volatile.Read(ref connectionAttempts);
        public int HttpSendAttempts => Volatile.Read(ref httpSendAttempts);

        public IDbConnection CreateConnection()
        {
            Interlocked.Increment(ref connectionAttempts);
            throw new InvalidOperationException("External SQL access is forbidden in phase 1 composition tests.");
        }

        public HttpClient CreateClient(string name) => new(new BlockedHttpHandler(this));

        private sealed class BlockedHttpHandler(ExternalIoBarrier barrier) : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Interlocked.Increment(ref barrier.httpSendAttempts);
                throw new InvalidOperationException("External HTTP access is forbidden in phase 1 composition tests.");
            }
        }
    }
}
