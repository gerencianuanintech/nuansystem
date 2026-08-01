using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.Carriers;

public sealed class CarrierTransactionalSyncContractTests
{
    [Fact]
    public void TenantMigration_UsesGlobalIdTransactionalOutboxAndTerminalCodeConflict()
    {
        var tenant = ReadSource("database", "sql", "162_tenant_carrier_transactional_outbox.sql");

        tenant.Should().Contain("ALTER TABLE dbo.Carriers ADD GlobalId uniqueidentifier NULL")
            .And.Contain("CREATE UNIQUE INDEX UQ_Carriers_GlobalId ON dbo.Carriers(GlobalId)")
            .And.Contain("CREATE UNIQUE INDEX UQ_Carriers_Code ON dbo.Carriers(Code)")
            .And.Contain("SP_NA_GET_CARRIER_SYNC_FULL")
            .And.Contain("SP_NA_POST_CARRIER_SYNC_APPLY_EVENT")
            .And.Contain("GlobalId <> @GlobalId")
            .And.Contain("Status = N'DeadLetter'")
            .And.Contain("SYNC_CARRIER_CODE_CONFLICT")
            .And.Contain("dbo.SyncAudit")
            .And.Contain("dbo.AuditCatalogChanges")
            .And.Contain("N'MasterBranchSyncWorker'")
            .And.Contain("20260801.162");
    }

    [Fact]
    public void TenantMigration_PreservesOuterTransactionAndSRIIdentificationContract()
    {
        var tenant = ReadSource("database", "sql", "162_tenant_carrier_transactional_outbox.sql");

        tenant.Should().Contain("DECLARE @OwnTransaction bit = CASE WHEN @@TRANCOUNT = 0 THEN 1 ELSE 0 END")
            .And.Contain("@IdentificationTypeCode NOT IN (N'04', N'05', N'06')")
            .And.NotContain("dbo.BusinessPartners")
            .And.NotContain("ExternalSystem")
            .And.NotContain("ExternalCode")
            .And.NotContain("SapCode");
    }

    [Fact]
    public void MasterMigration_RegistersCarrierDisabledWithoutDependenciesOrGrants()
    {
        var master = ReadSource("database", "sql", "163_master_carrier_transactional_registration.sql");

        master.Should().Contain("N'Carrier'")
            .And.Contain("N'Transportistas'")
            .And.Contain("CONVERT(bit, 0)")
            .And.Contain("N'MasterToBranch'")
            .And.Contain("20260801.163")
            .And.NotContain("RolePermissions")
            .And.NotContain("SecurityRoleMenus")
            .And.NotContain("dbo.BusinessPartners")
            .And.NotContain("SapCode");
    }

    [Fact]
    public void Initializers_RegisterBothForwardOnlyMigrations()
    {
        var tenantInitializer = ReadSource(
            "src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerTenantDatabaseInitializer.cs");
        var masterInitializer = ReadSource(
            "src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerMasterDatabaseInitializer.cs");

        tenantInitializer.Should().Contain("162_tenant_carrier_transactional_outbox.sql");
        masterInitializer.Should().Contain("163_master_carrier_transactional_registration.sql");
    }

    private static string ReadSource(params string[] parts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine([directory.FullName, .. parts]);
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException(Path.Combine(parts));
    }
}
