using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.TenantConfiguration;
using NuanSystem.Application.Features.TenantConfiguration.Dtos;
using NuanSystem.Application.Features.TenantConfiguration.Services;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Application.Tests.Features.TenantConfiguration;

public sealed class TenantConfigurationServiceTests
{
    private const int CompanyId = 10;
    private const string CompanyCode = "EMPRESA01";

    [Fact]
    public async Task TenantFeatures_ReturnsAllKnownFeaturesWithStandaloneSapDisabledByDefault()
    {
        var repository = Substitute.For<ITenantFeatureRepository>();
        repository.GetByCompanyIdAsync(CompanyId, Arg.Any<CancellationToken>())
            .Returns([
                new TenantFeatureDto(TenantFeatureCodes.InventoryModule, true, DateTime.UtcNow, null),
                new TenantFeatureDto(TenantFeatureCodes.PurchasesModule, true, DateTime.UtcNow, null)
            ]);
        var service = new TenantFeatureService(repository, CreateCompanyContext());

        var result = await service.GetActiveCompanyFeaturesAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Select(feature => feature.FeatureCode).Should().BeEquivalentTo(TenantFeatureCodes.All);
        result.Value!.Single(feature => feature.FeatureCode == TenantFeatureCodes.SapB1Integration).IsEnabled.Should().BeFalse();
        result.Value!.Single(feature => feature.FeatureCode == TenantFeatureCodes.InventoryModule).IsEnabled.Should().BeTrue();
        await repository.Received(1).GetByCompanyIdAsync(CompanyId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task TenantIntegrations_ReturnsSapEnabled_WhenSapIntegrationIsConfigured()
    {
        var repository = Substitute.For<ITenantIntegrationRepository>();
        repository.GetByCompanyIdAsync(CompanyId, Arg.Any<CancellationToken>())
            .Returns([
                new TenantIntegrationDto(TenantIntegrationCodes.SapB1, true, """{"mode":"ServiceLayer","password":"plain-secret"}""", DateTime.UtcNow, null)
            ]);
        var service = new TenantIntegrationService(repository, CreateCompanyContext(CompanyOperationMode.SapIntegrated, SapIntegrationMode.ServiceLayer));

        var result = await service.GetActiveCompanyIntegrationsAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Select(integration => integration.IntegrationCode).Should().BeEquivalentTo(TenantIntegrationCodes.All);
        var sapIntegration = result.Value!.Single(integration => integration.IntegrationCode == TenantIntegrationCodes.SapB1);
        sapIntegration.IsEnabled.Should().BeTrue();
        sapIntegration.ConfigurationJson.Should().Contain("\"password\":\"********\"");
        sapIntegration.ConfigurationJson.Should().NotContain("plain-secret");
        result.Value!.Single(integration => integration.IntegrationCode == TenantIntegrationCodes.Sri).IsEnabled.Should().BeFalse();
    }

    [Fact]
    public async Task EntityOwnership_ReturnsConfigurationForActiveCompanyAndEntity()
    {
        var repository = Substitute.For<IEntityOwnershipRepository>();
        repository.GetByCompanyIdAndEntityAsync(CompanyId, "Suppliers", Arg.Any<CancellationToken>())
            .Returns(new EntityOwnershipConfigurationDto(
                "Suppliers",
                EntitySourceOfTruth.SapBusinessOne,
                EntitySyncDirection.SapToNuan,
                true,
                DateTime.UtcNow,
                null));
        var service = new EntityOwnershipService(repository, CreateCompanyContext(CompanyOperationMode.Hybrid));

        var result = await service.GetActiveCompanyOwnershipAsync("Suppliers", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.EntityName.Should().Be("Suppliers");
        result.Value.SourceOfTruth.Should().Be(EntitySourceOfTruth.SapBusinessOne);
        result.Value.SyncDirection.Should().Be(EntitySyncDirection.SapToNuan);
    }

    [Fact]
    public async Task EntityOwnership_ReturnsFailure_WhenEntityIsNotConfigured()
    {
        var repository = Substitute.For<IEntityOwnershipRepository>();
        var service = new EntityOwnershipService(repository, CreateCompanyContext());

        var result = await service.GetActiveCompanyOwnershipAsync("Items", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("No existe configuracion de ownership para la entidad indicada.");
    }

    private static ICompanyContext CreateCompanyContext(
        CompanyOperationMode operationMode = CompanyOperationMode.Standalone,
        SapIntegrationMode sapIntegrationMode = SapIntegrationMode.None)
    {
        var companyContext = Substitute.For<ICompanyContext>();
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(new CompanyConnectionInfo(
            CompanyId,
            CompanyCode,
            "Empresa de prueba",
            DatabaseEngine.SqlServer,
            "Server=(local);Database=NuanSystem_Test;Trusted_Connection=True;",
            sapIntegrationMode,
            operationMode,
            IsMaster: true,
            ParentCompanyId: null,
            BranchCode: null,
            SyncEnabled: false));

        return companyContext;
    }
}
