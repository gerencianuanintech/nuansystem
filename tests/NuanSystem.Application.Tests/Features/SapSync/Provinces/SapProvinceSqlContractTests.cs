using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.SapSync.Provinces;

public sealed class SapProvinceSqlContractTests
{
    [Fact]
    public void Migration173_AddsStrictProvinceV1Snapshot()
    {
        var sql=ReadSource("database","sql","173_tenant_sap_province_execution_snapshot.sql");
        sql.Should().Contain("'ProvinceV1'")
            .And.Contain("@ApprovedSnapshotType = ''ProvinceV1''")
            .And.Contain("'countryCode'', ''provinceCode'', ''provinceName'")
            .And.Contain("Version=N'20260804.173'");
    }

    [Fact]
    public void Migration174_RegistersFullSapToErpWithoutProfileOrSchedule()
    {
        var sql=ReadSource("database","sql","174_master_sap_province_sync_capability.sql");
        sql.Should().Contain("EntityCode=N'Provinces'")
            .And.Contain("SupportsSapToErp=1")
            .And.Contain("SupportsErpToSap=0")
            .And.Contain("SupportsFull=1")
            .And.Contain("SupportsIncremental=0")
            .And.NotContain("INSERT dbo.SapSyncProfiles")
            .And.NotContain("INSERT dbo.SapSyncSchedules");
    }

    [Fact]
    public void Initializers_RegisterProvinceMigrationsInOrder()
    {
        var tenant=ReadSource("src","Backend","NuanSystem.Persistence","Services","SqlServerTenantDatabaseInitializer.cs");
        var master=ReadSource("src","Backend","NuanSystem.Persistence","Services","SqlServerMasterDatabaseInitializer.cs");
        tenant.IndexOf("173_tenant_sap_province_execution_snapshot.sql",StringComparison.Ordinal).Should().BeGreaterThan(tenant.IndexOf("172_tenant_province_transactional_outbox.sql",StringComparison.Ordinal));
        master.Should().Contain("174_master_sap_province_sync_capability.sql");
    }

    private static string ReadSource(params string[] parts){var d=new DirectoryInfo(AppContext.BaseDirectory);while(d is not null){var p=Path.Combine([d.FullName,..parts]);if(File.Exists(p))return File.ReadAllText(p);d=d.Parent;}throw new FileNotFoundException();}
}
