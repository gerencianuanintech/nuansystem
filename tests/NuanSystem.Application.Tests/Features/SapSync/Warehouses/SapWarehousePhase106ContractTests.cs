using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.SapSync.Warehouses;

public sealed class SapWarehousePhase106ContractTests
{
    [Fact]
    public void Migration159_RegistersOnlyFullSapToErpWarehouseCapability()
    {
        var migration = ReadSource("database", "sql", "159_master_sap_warehouse_sync_capability.sql");

        migration.Should().Contain("WHERE Version = N'20260731.157'");
        migration.Should().Contain("WHERE EntityCode = N'Warehouses'");
        migration.Should().Contain("SupportsSapToErp = 1");
        migration.Should().Contain("SupportsErpToSap = 0");
        migration.Should().Contain("SupportsFull = 1");
        migration.Should().Contain("SupportsIncremental = 0");
        migration.Should().Contain("Version = N'20260731.159'");
        migration.Should().NotContain("INSERT dbo.SapSyncProfiles");
        migration.Should().NotContain("INSERT dbo.SapSyncProfileEntities");
        migration.Should().NotContain("INSERT dbo.SapSyncSchedules");
    }

    [Fact]
    public void MasterInitializer_RegistersMigration159()
    {
        var initializer = ReadSource(
            "src", "Backend", "NuanSystem.Persistence", "Services",
            "SqlServerMasterDatabaseInitializer.cs");

        initializer.Should().Contain("159_master_sap_warehouse_sync_capability.sql");
    }

    [Fact]
    public void ApplicationRegistration_RegistersWarehouseHandlerExecutionAndRetryProcessors()
    {
        var registration = ReadSource(
            "src", "Backend", "NuanSystem.Application", "DependencyInjection",
            "ApplicationServiceRegistration.cs");

        registration.Should().Contain("ISapSyncEntityHandler, SapWarehouseSyncHandler");
        registration.Should().Contain("ISapSyncScheduledExecutionProcessor, SapWarehouseExecutionProcessor");
        registration.Should().Contain("ISapSyncExecutionRetryProcessor, SapWarehouseExecutionRetryProcessor");
    }

    [Fact]
    public void WarehouseSapWrites_KeepTransactionalLocalOutboxPath()
    {
        var recordProcessor = ReadSource(
            "src", "Backend", "NuanSystem.Application", "Features", "SapSync",
            "Warehouses", "Services", "SapWarehouseRecordProcessor.cs");
        var createHandler = ReadSource(
            "src", "Backend", "NuanSystem.Application", "Features", "GeneralInventory",
            "Warehouses", "Commands", "CreateWarehouseCommandHandler.cs");
        var updateHandler = ReadSource(
            "src", "Backend", "NuanSystem.Application", "Features", "GeneralInventory",
            "Warehouses", "Commands", "UpdateWarehouseCommandHandler.cs");

        recordProcessor.Should().Contain("sender.Send(new CreateWarehouseCommand(");
        recordProcessor.Should().Contain("sender.Send(new UpdateWarehouseCommand(");
        createHandler.Should().Contain("ExecuteInTenantTransactionAsync");
        createHandler.Should().Contain("localOutboxWriter.EnqueueAsync(warehouse, SyncOperation.Created");
        updateHandler.Should().Contain("ExecuteInTenantTransactionAsync");
        updateHandler.Should().Contain("localOutboxWriter.EnqueueAsync(");
        updateHandler.Should().Contain("warehouse.IsActive ? SyncOperation.Updated : SyncOperation.Disabled");
    }

    [Fact]
    public void WarehouseSnapshot_UsesApprovedWarehouseV1Allowlist()
    {
        var contracts = ReadSource(
            "src", "Backend", "NuanSystem.Application", "Features", "SapSync",
            "Executions", "SapSyncExecutionContracts.cs");
        var migration = ReadSource("database", "sql", "153_tenant_sap_sync_execution_history.sql");

        contracts.Should().Contain("public const string WarehouseV1 = \"WarehouseV1\"");
        migration.Should().Contain("@ApprovedSnapshotType = 'WarehouseV1'");
        migration.Should().Contain("'warehouseCode', 'warehouseName', 'street', 'city', 'province', 'country', 'isActive'");
    }

    [Fact]
    public void Migration166_AddsTypedProfileWarehouseFilterWithoutHardcodedNames()
    {
        var migration = ReadSource(
            "database", "sql", "166_master_sap_warehouse_profile_filter.sql");

        migration.Should().Contain("CREATE TABLE dbo.SapSyncWarehouseProfileFilters")
            .And.Contain("SP_NA_PUT_SAPWAREHOUSEPROFILEFILTERCONFIGURAR")
            .And.Contain("NameContains")
            .And.Contain("ExactName")
            .And.Contain("LEFT JOIN dbo.SapSyncWarehouseProfileFilters filter")
            .And.Contain("Version = N'20260804.166'")
            .And.NotContain("MEGA")
            .And.NotContain("FERIA LIBRE");
    }

    [Fact]
    public void MasterInitializer_RegistersMigration166After165()
    {
        var initializer = ReadSource(
            "src", "Backend", "NuanSystem.Persistence", "Services",
            "SqlServerMasterDatabaseInitializer.cs");

        initializer.IndexOf(
                "166_master_sap_warehouse_profile_filter.sql",
                StringComparison.Ordinal)
            .Should().BeGreaterThan(initializer.IndexOf(
                "165_master_sap_sync_profile_inactive_capabilities.sql",
                StringComparison.Ordinal));
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
