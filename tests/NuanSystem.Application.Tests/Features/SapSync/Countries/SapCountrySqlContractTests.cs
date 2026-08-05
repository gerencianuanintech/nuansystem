using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.SapSync.Countries;

public sealed class SapCountrySqlContractTests
{
    [Fact]
    public void Architecture_RecordsOcryAndOcstSourcesWithoutChangingServiceLayerBoundary()
    {
        var architecture = ReadSource("docs", "architecture", "SAP-COUNTRIES-SYNC.md");

        architecture.Should().Contain("`OCRY`")
            .And.Contain("`OCST`")
            .And.Contain("entity set `Countries`")
            .And.Contain("no consulta `OCRY` ni `OCST` directamente");
    }

    [Fact]
    public void Migration170_RegistersOnlyFullSapToErpCountryCapabilityWithoutActivation()
    {
        var migration = ReadSource("database", "sql", "170_master_sap_country_sync_capability.sql");

        migration.Should().Contain("WHERE EntityCode = N'Countries'")
            .And.Contain("SupportsSapToErp = 1")
            .And.Contain("SupportsErpToSap = 0")
            .And.Contain("SupportsFull = 1")
            .And.Contain("SupportsIncremental = 0")
            .And.Contain("Version = N'20260804.170'")
            .And.NotContain("INSERT dbo.SapSyncProfiles")
            .And.NotContain("INSERT dbo.SapSyncProfileEntities")
            .And.NotContain("INSERT dbo.SapSyncSchedules");
    }

    [Fact]
    public void Migration169_AddsCountryV1WithOnlyApprovedCountryFields()
    {
        var migration = ReadSource("database", "sql", "169_tenant_sap_country_execution_snapshot.sql");

        migration.Should().Contain("'CountryV1'")
            .And.Contain("@ApprovedSnapshotType = 'CountryV1'")
            .And.Contain("'countryCode', 'countryName', 'iso2', 'iso3'")
            .And.Contain("Version = N'20260804.169'");
    }

    [Fact]
    public void DatabaseInitializers_RegisterCountryMigrationsInOrder()
    {
        var tenant = ReadSource(
            "src", "Backend", "NuanSystem.Persistence", "Services",
            "SqlServerTenantDatabaseInitializer.cs");
        var master = ReadSource(
            "src", "Backend", "NuanSystem.Persistence", "Services",
            "SqlServerMasterDatabaseInitializer.cs");

        tenant.IndexOf("169_tenant_sap_country_execution_snapshot.sql", StringComparison.Ordinal)
            .Should().BeGreaterThan(tenant.IndexOf(
                "168_tenant_country_transactional_outbox.sql", StringComparison.Ordinal));
        master.IndexOf("171_master_definitions_general_geography_navigation.sql", StringComparison.Ordinal)
            .Should().BeGreaterThan(master.IndexOf(
                "170_master_sap_country_sync_capability.sql", StringComparison.Ordinal));
    }

    private static string ReadSource(params string[] pathParts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine([directory.FullName, .. pathParts]);
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException(Path.Combine(pathParts));
    }
}
