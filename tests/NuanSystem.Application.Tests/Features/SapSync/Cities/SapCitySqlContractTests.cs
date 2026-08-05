using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.SapSync.Cities;

public sealed class SapCitySqlContractTests
{
    [Fact]
    public void MasterMigration_AddsQueryAndFullCapabilityWithoutAutomaticProfile()
    {
        var sql = Read("database", "sql", "176_master_sap_city_select_query.sql");
        sql.Should().Contain("CitiesSelectQuery nvarchar(max)")
            .And.Contain("SP_NA_PUT_SAPCOMPANYSETTINGS_CITIESQUERY")
            .And.Contain("EntityCode = N'Cities'")
            .And.Contain("SupportsSapToErp = 1")
            .And.Contain("SupportsErpToSap = 0")
            .And.NotContain("INSERT dbo.SapSyncProfiles")
            .And.NotContain("INSERT dbo.SapSyncSchedules");
    }

    [Fact]
    public void TenantMigration_AddsAtomicStrictCitySnapshotContract()
    {
        var sql = Read("database", "sql", "177_tenant_sap_city_execution_snapshot.sql");
        sql.Should().Contain("'CityV1'")
            .And.Contain("@ApprovedSnapshotType = ''CityV1''")
            .And.Contain("''countryCode'', ''provinceCode'', ''cityCode'', ''cityName''")
            .And.Contain("BEGIN TRY")
            .And.Contain("BEGIN TRANSACTION")
            .And.Contain("IF XACT_STATE() <> 0 ROLLBACK")
            .And.Contain("Version=N'20260805.177'");
    }

    [Fact]
    public void Initializers_RegisterCityMigrationsAfterDependencies()
    {
        var tenant = Read("src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerTenantDatabaseInitializer.cs");
        var master = Read("src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerMasterDatabaseInitializer.cs");
        tenant.IndexOf("177_tenant_sap_city_execution_snapshot.sql", StringComparison.Ordinal)
            .Should().BeGreaterThan(tenant.IndexOf("175_tenant_city_transactional_outbox.sql", StringComparison.Ordinal));
        master.IndexOf("176_master_sap_city_select_query.sql", StringComparison.Ordinal)
            .Should().BeGreaterThan(master.IndexOf("174_master_sap_province_sync_capability.sql", StringComparison.Ordinal));
    }

    private static string Read(params string[] parts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine([directory.FullName, .. parts]);
            if (File.Exists(path)) return File.ReadAllText(path);
            directory = directory.Parent;
        }
        throw new FileNotFoundException();
    }
}
