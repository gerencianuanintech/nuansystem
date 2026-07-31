using System.Text.RegularExpressions;
using FluentAssertions;
using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.Application.Features.SapSync.Executions;
using NuanSystem.Application.Features.SapSync.Profiles;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapSyncProfilePersistenceContractTests
{
    private const string MasterMigration = "152_master_sap_sync_profiles.sql";
    private const string HardeningMigration = "154_master_sap_sync_profile_api_hardening.sql";
    private const string TenantMigration = "153_tenant_sap_sync_execution_history.sql";
    private const string TenantOperationsMigration = "158_tenant_sap_sync_execution_operations.sql";

    [Fact]
    public void Migrations_AreVersionedOrderedAndStructurallyIdempotent()
    {
        var master = MasterSql();
        var hardening = HardeningSql();
        var tenant = TenantSql();
        var masterInitializer = Read(
            "src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerMasterDatabaseInitializer.cs");
        var tenantInitializer = Read(
            "src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerTenantDatabaseInitializer.cs");

        master.Should().Contain("Version = N'20260730.152'")
            .And.Contain("IF OBJECT_ID(N'dbo.SapSyncProfiles', N'U') IS NULL")
            .And.Contain("IF OBJECT_ID(N'dbo.SapSyncProfileEntities', N'U') IS NULL")
            .And.Contain("IF OBJECT_ID(N'dbo.SapSyncSchedules', N'U') IS NULL")
            .And.NotContain("DELETE FROM dbo.MasterSchemaHistory")
            .And.NotContain("UPDATE dbo.MasterSchemaHistory")
            .And.NotContain("DROP TABLE dbo.SapSync");
        tenant.Should().Contain("Version = N'20260730.153'")
            .And.Contain("IF OBJECT_ID(N'dbo.SapSyncExecutions', N'U') IS NULL")
            .And.Contain("IF OBJECT_ID(N'dbo.SapSyncExecutionDetails', N'U') IS NULL")
            .And.Contain("IF COL_LENGTH(N'dbo.SapSyncLock', N'OwnerToken') IS NULL")
            .And.NotContain("DELETE FROM dbo.SchemaHistory")
            .And.NotContain("UPDATE dbo.SchemaHistory")
            .And.NotContain("DROP TABLE dbo.SapSync");
        hardening.Should().Contain("Version = N'20260730.154'")
            .And.Contain("CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SAPSYNCPROFILEEMPRESASACCESIBLES")
            .And.Contain("CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_SAPSYNCPROFILEACTUALIZAR")
            .And.NotContain("DELETE FROM dbo.MasterSchemaHistory")
            .And.NotContain("UPDATE dbo.MasterSchemaHistory")
            .And.NotContain("ALTER TABLE dbo.SapSyncEntitySettings")
            .And.NotContain("UPDATE dbo.SapSyncEntitySettings")
            .And.NotContain("INSERT dbo.Permissions")
            .And.NotContain("INSERT dbo.SecurityOperations")
            .And.NotContain("INSERT dbo.SecurityForms")
            .And.NotContain("INSERT dbo.SecurityMenus");

        Regex.Matches(master, "CREATE OR ALTER PROCEDURE dbo\\.SP_NA_").Count.Should().Be(7);
        Regex.Matches(hardening, "CREATE OR ALTER PROCEDURE dbo\\.SP_NA_").Count.Should().Be(2);
        Regex.Matches(tenant, "CREATE OR ALTER PROCEDURE dbo\\.SP_NA_").Count.Should().Be(21);
        Regex.Matches(master, "20260730\\.152").Count.Should().Be(2);
        Regex.Matches(hardening, "20260730\\.154").Count.Should().Be(2);
        Regex.Matches(tenant, "20260730\\.153").Count.Should().Be(3);
        Regex.Matches(tenant, "20260731\\.158").Count.Should().Be(2);

        masterInitializer.IndexOf(MasterMigration, StringComparison.Ordinal)
            .Should().BeGreaterThan(masterInitializer.IndexOf("151_master_user_profile_avatar.sql", StringComparison.Ordinal));
        masterInitializer.IndexOf(HardeningMigration, StringComparison.Ordinal)
            .Should().BeGreaterThan(masterInitializer.IndexOf(MasterMigration, StringComparison.Ordinal));
        tenantInitializer.IndexOf(TenantMigration, StringComparison.Ordinal)
            .Should().BeGreaterThan(tenantInitializer.IndexOf("150_tenant_sri_document_monitor_import_scope.sql", StringComparison.Ordinal));
        tenantInitializer.IndexOf(TenantOperationsMigration, StringComparison.Ordinal)
            .Should().BeGreaterThan(tenantInitializer.IndexOf(TenantMigration, StringComparison.Ordinal));
    }

    [Fact]
    public void CompanyAccessProcedure_AlignsWithDapperAndPreservesAuthorizationSemantics()
    {
        var sql = HardeningSql();
        var repository = Read(
            "src", "Backend", "NuanSystem.Persistence", "Repositories", "SapSync", "SapSyncProfileRepository.cs");

        sql.Should().Contain("CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SAPSYNCPROFILEEMPRESASACCESIBLES")
            .And.Contain("@UserId int")
            .And.Contain("@CompanyId int = NULL")
            .And.Contain("company.Id AS CompanyId")
            .And.Contain("company.Code AS CompanyCode")
            .And.Contain("company.CommercialName AS CompanyName")
            .And.Contain("company.IsActive AS IsCompanyActive")
            .And.Contain("company.SapIntegrationMode")
            .And.Contain("AS HasSapSettings")
            .And.Contain("AS IsSapEnabled")
            .And.Contain("AS SapSettingsIntegrationMode")
            .And.Contain("AS IsUserAuthorized")
            .And.Contain("(@CompanyId IS NULL AND userCompany.UserId IS NOT NULL)")
            .And.Contain("(@CompanyId IS NOT NULL AND company.Id = @CompanyId)")
            .And.Contain("CASE WHEN userCompany.UserId IS NULL THEN 0 ELSE 1 END")
            .And.NotContain("settings.ServiceLayerUrl")
            .And.NotContain("settings.Password")
            .And.NotContain("settings.SapUser");

        repository.Should().Contain(
                "CompanyAccessProcedure = \"dbo.SP_NA_GET_SAPSYNCPROFILEEMPRESASACCESIBLES\"")
            .And.Contain("new { UserId = userId, CompanyId = companyId }")
            .And.Contain("commandType: CommandType.StoredProcedure")
            .And.Contain("cancellationToken: cancellationToken")
            .And.NotMatchRegex(@"(?im)^\s*(SELECT|FROM|JOIN|WHERE)\b");
    }

    [Fact]
    public void UpdateProcedure_RejectsCompanyChangeBeforeMutationOrAudit()
    {
        var sql = HardeningSql();
        var procedure = Regex.Match(
            sql,
            @"(?s)CREATE OR ALTER PROCEDURE dbo\.SP_NA_PUT_SAPSYNCPROFILEACTUALIZAR.*?\r?\nGO")
            .Value;
        var rejection = procedure.IndexOf("N'CompanyImmutable' AS ResultCode", StringComparison.Ordinal);
        var transaction = procedure.IndexOf("BEGIN TRANSACTION", StringComparison.Ordinal);
        var audit = procedure.IndexOf("INSERT dbo.AuditSapSyncProfileChanges", StringComparison.Ordinal);

        rejection.Should().BeGreaterThan(-1);
        transaction.Should().BeGreaterThan(rejection);
        audit.Should().BeGreaterThan(transaction);
        procedure.Should().Contain("AND CompanyId <> @CompanyId")
            .And.Contain("AND CompanyId = @CompanyId")
            .And.NotContain("SET CompanyId = @CompanyId");
    }

    [Fact]
    public void ProfileSchema_EnforcesScheduleShapesConcurrencyAndCurrentUniqueness()
    {
        var sql = MasterSql();

        sql.Should().Contain("ScheduleType = 'Manual' AND IntervalMinutes IS NULL AND ExecutionTime IS NULL")
            .And.Contain("ScheduleType = 'Interval' AND IntervalMinutes IS NOT NULL AND ExecutionTime IS NULL")
            .And.Contain("ScheduleType = 'Daily' AND IntervalMinutes IS NULL AND ExecutionTime IS NOT NULL")
            .And.Contain("TimeZoneId nvarchar(100) NOT NULL CONSTRAINT DF_SapSyncSchedules_TimeZoneId DEFAULT N'America/Guayaquil'")
            .And.Contain("PreventConcurrentExecutions bit NOT NULL CONSTRAINT DF_SapSyncSchedules_PreventConcurrent DEFAULT 1")
            .And.Contain("UX_SapSyncProfiles_Company_Code_Current")
            .And.Contain("ON dbo.SapSyncProfiles(CompanyId, Code)")
            .And.Contain("UX_SapSyncProfileEntities_Current")
            .And.Contain("ON dbo.SapSyncProfileEntities(SapSyncProfileId, EntityCode, Direction)")
            .And.Contain("UX_SapSyncSchedules_ProfileEntity_Current")
            .And.Contain("WHERE IsDeleted = 0")
            .And.Contain("RowVersion rowversion NOT NULL");

        SapSyncScheduleTypes.All.Should().BeEquivalentTo(
            SapSyncScheduleTypes.Manual,
            SapSyncScheduleTypes.Interval,
            SapSyncScheduleTypes.Daily);
    }

    [Fact]
    public void CapabilityContract_RejectsBothUnlessBothDirectionsExist()
    {
        var oneWay = new SapSyncHandlerCapabilityDto(
            "Suppliers", "Proveedores", true, false, true, true, true, true);
        var bidirectional = oneWay with { SupportsErpToSap = true };

        oneWay.Supports(SapSyncDirection.SapToErp).Should().BeTrue();
        oneWay.Supports(SapSyncDirection.Both).Should().BeFalse();
        bidirectional.Supports(SapSyncDirection.Both).Should().BeTrue();

        MasterSql().Should().Contain("OR entity.Direction = 'Both'")
            .And.Contain("capability.SupportsSapToErp = 0")
            .And.Contain("capability.SupportsErpToSap = 0");
    }

    [Fact]
    public void LegacyMigration_IsInactiveManualAndPreservesReadOnlyFallback()
    {
        var sql = MasterSql();

        sql.Should().Contain("CONCAT(N'LEGACY-SAP-', legacy.CompanyId)")
            .And.Contain("'LegacyMigration',")
            .And.Contain("legacy.MaxRetryCount + 1")
            .And.Contain("legacy.EntityCode <> N'Warehouses'")
            .And.Contain("entity.Id, 'Manual', NULL, NULL")
            .And.Contain("N'America/Guayaquil', 1, 0, N'Migracion 152'")
            .And.Contain("legacy.CompanyId, 1, N'Fase10.2-v1', 2")
            .And.Contain("RequiredSuccessfulCycles = 2")
            .And.Contain("(N'PurchaseOrders', N'Ordenes de compra', 0, 0, 0, 0, 0)")
            .And.Contain("IsActive, CreatedByUserName")
            .And.NotContain("ALTER TABLE dbo.SapSyncEntitySettings")
            .And.NotContain("DROP TABLE dbo.SapSyncEntitySettings")
            .And.NotContain("DELETE FROM dbo.SapSyncEntitySettings")
            .And.NotContain("UPDATE dbo.SapSyncEntitySettings");

        Regex.Matches(
                sql,
                @"(?s)INSERT dbo\.SapSyncProfileEntities.*?SELECT.*?ExecutionTimeoutMinutes,\s+IsActive.*?\s30,\s+0,\s+N'Migracion 152'")
            .Count.Should().Be(1, "toda entidad migrada, incluso PurchaseOrders o Both, debe quedar inactiva");
    }

    [Fact]
    public void SecurityContract_SeedsIndependentPermissionsOnlyForAdmin()
    {
        var sql = MasterSql();
        var permissionCodes = Regex.Matches(sql, @"\(N'(SAP\.SYNC\.(?:PROFILES|EXECUTIONS)\.[A-Z_]+)'")
            .Select(match => match.Groups[1].Value)
            .Distinct()
            .ToArray();

        permissionCodes.Should().HaveCount(12);
        permissionCodes.Should().Contain("SAP.SYNC.PROFILES.EXECUTE")
            .And.Contain("SAP.SYNC.EXECUTIONS.RELEASE_EXPIRED_LOCK");
        sql.Should().Contain("WHERE Code = N'ADMIN' AND IsDeleted = 0")
            .And.Contain("SELECT @AdminRoleId, permission.Id")
            .And.NotContain("SecurityRoleMenus")
            .And.NotContain("SecurityRoleForms")
            .And.NotContain("INSERT dbo.SecurityForms")
            .And.NotContain("INSERT dbo.SecurityMenus");
    }

    [Fact]
    public void ExecutionSchema_EnforcesStatesSnapshotsHashesAndRecordIdentity()
    {
        var sql = TenantSql();
        var executionStatuses = new[]
        {
            SapSyncExecutionStatuses.Pending,
            SapSyncExecutionStatuses.Running,
            SapSyncExecutionStatuses.Cancelling,
            SapSyncExecutionStatuses.Cancelled,
            SapSyncExecutionStatuses.RetryScheduled,
            SapSyncExecutionStatuses.SkippedConcurrent,
            SapSyncExecutionStatuses.Completed,
            SapSyncExecutionStatuses.CompletedWithWarnings,
            SapSyncExecutionStatuses.CompletedWithErrors,
            SapSyncExecutionStatuses.Failed
        };
        var detailStatuses = new[]
        {
            SapSyncExecutionDetailStatuses.Pending,
            SapSyncExecutionDetailStatuses.Processing,
            SapSyncExecutionDetailStatuses.Created,
            SapSyncExecutionDetailStatuses.Updated,
            SapSyncExecutionDetailStatuses.Unchanged,
            SapSyncExecutionDetailStatuses.ApprovalRequired,
            SapSyncExecutionDetailStatuses.Conflict,
            SapSyncExecutionDetailStatuses.Skipped,
            SapSyncExecutionDetailStatuses.RetryScheduled,
            SapSyncExecutionDetailStatuses.Failed,
            SapSyncExecutionDetailStatuses.DeadLetter
        };

        executionStatuses.Should().OnlyContain(status => sql.Contains($"'{status}'", StringComparison.Ordinal));
        detailStatuses.Should().OnlyContain(status => sql.Contains($"'{status}'", StringComparison.Ordinal));
        sql.Should().Contain("SnapshotHash binary(32) NULL")
            .And.Contain("@SnapshotHash binary(32) = NULL")
            .And.Contain("ApprovedSnapshotJson IS NULL OR ISJSON(ApprovedSnapshotJson) = 1")
            .And.Contain("ApprovedSnapshotType IS NULL AND ApprovedSnapshotJson IS NULL AND SnapshotHash IS NULL")
            .And.Contain("ApprovedSnapshotType IN ('SupplierV1', 'ItemV1', 'PaymentTermV1', 'WarehouseV1')")
            .And.Contain("CONSTRAINT UQ_SapSyncExecutions_ExecutionUid UNIQUE (ExecutionUid)")
            .And.Contain("CONSTRAINT UQ_SapSyncExecutionDetails_Record UNIQUE (SapSyncExecutionId, SourceRecordKey)")
            .And.Contain("RowVersion rowversion NOT NULL");
    }

    [Fact]
    public void ExecutionAndLockSchema_AreTenantLocalRecoverableAndAudited()
    {
        var sql = TenantSql();

        sql.Should().Contain("OwnerToken char(64)")
            .And.Contain("LockExpiresAtUtc datetime2(0)")
            .And.Contain("CK_SapSyncLock_OwnerToken")
            .And.Contain("SP_NA_PATCH_SAPSYNCLOCKRENOVAR")
            .And.Contain("SP_NA_DELETE_SAPSYNCLOCKLIBERARVENCIDO")
            .And.Contain("AuditSapSyncExecutionChanges")
            .And.Contain("AuditSapSyncLockChanges")
            .And.NotContain("REFERENCES dbo.Companies")
            .And.NotContain("REFERENCES dbo.SapSyncProfiles")
            .And.NotContain("REFERENCES dbo.SapSyncProfileEntities")
            .And.NotContain("DELETE FROM dbo.SapSyncExecutions")
            .And.NotContain("DELETE FROM dbo.SapSyncExecutionDetails");
    }

    [Fact]
    public void PersistenceRepositories_UseStoredProceduresWithAlignedSqlTypes()
    {
        var masterSql = MasterSql() + Environment.NewLine + HardeningSql();
        var tenantSql = TenantSql();
        var profileRepository = Read(
            "src", "Backend", "NuanSystem.Persistence", "Repositories", "SapSync", "SapSyncProfileRepository.cs");
        var executionRepository = Read(
            "src", "Backend", "NuanSystem.Persistence", "Repositories", "SapSync", "SapSyncExecutionRepository.cs");
        var lockRepository = Read(
            "src", "Backend", "NuanSystem.Persistence", "Repositories", "SapSync", "SapSyncLockRepository.cs");

        AssertRepositoryProceduresExist(profileRepository, masterSql, 8);
        AssertRepositoryProceduresExist(executionRepository, tenantSql, 14);
        AssertRepositoryProceduresExist(lockRepository, tenantSql, 4);

        profileRepository.Should().Contain("commandType: CommandType.StoredProcedure")
            .And.NotContain("CommandType.Text");
        executionRepository.Should().Contain("commandType: CommandType.StoredProcedure")
            .And.NotContain("CommandType.Text");
        lockRepository.Should().Contain("commandType: CommandType.StoredProcedure")
            .And.NotContain("CommandType.Text");

        masterSql.Should().Contain("@Id bigint")
            .And.Contain("@CompanyId int")
            .And.Contain("@ExpectedRowVersion varbinary(8)");
        tenantSql.Should().Contain("@ExecutionUid uniqueidentifier")
            .And.Contain("@SapSyncProfileId bigint = NULL")
            .And.Contain("@SnapshotHash binary(32) = NULL")
            .And.Contain("@ExpectedRowVersion varbinary(8)");
    }

    [Fact]
    public void PersistedJsonAndMessages_ExposeOnlySafeContracts()
    {
        var sql = TenantSql();
        var contracts = Read(
            "src", "Backend", "NuanSystem.Application", "Features", "SapSync", "Executions",
            "SapSyncExecutionContracts.cs");

        foreach (var forbiddenKey in new[]
                 {
                     "password", "token", "cookie", "authorization", "connectionstring",
                     "b1session", "routeid", "login"
                 })
        {
            sql.Should().Contain($"NOT LIKE N'%{forbiddenKey}%'");
        }

        contracts.Should().Contain("LastSafeErrorCode")
            .And.Contain("LastSafeErrorMessage")
            .And.Contain("SafeMessage")
            .And.Contain("ApprovedSnapshotType")
            .And.Contain("ApprovedSnapshotJson")
            .And.Contain("SnapshotHash")
            .And.NotContain("ServiceLayerResponse")
            .And.NotContain("HttpHeaders")
            .And.NotContain("ConnectionString");
    }

    [Fact]
    public void NewContracts_DoNotReuseOrAlterMatrixBranchProfiles()
    {
        var master = MasterSql();
        var tenant = TenantSql();

        master.Should().NotContain("dbo.SyncProfiles")
            .And.NotContain("dbo.SyncProfileEntities")
            .And.NotContain("dbo.SyncSchedules");
        tenant.Should().NotContain("dbo.SyncProfiles")
            .And.NotContain("dbo.SyncProfileEntities")
            .And.NotContain("dbo.SyncSchedules");

        Read("database", "sql", "069_sync_master_branch_configuration.sql")
            .Should().Contain("CREATE TABLE dbo.SyncProfiles");
        Read("src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync", "SyncProfileRepository.cs")
            .Should().Contain("class SyncProfileRepository")
            .And.NotContain("SapSyncProfileRepository");
    }

    [Fact]
    public void CurrentSapHandlers_RemainRegisteredAndCapabilitySeedMatchesImplementedScope()
    {
        var registration = Read(
            "src", "Backend", "NuanSystem.Application", "DependencyInjection",
            "ApplicationServiceRegistration.cs");
        var sql = MasterSql();

        foreach (var handler in new[]
                 {
                     "SapSupplierSyncHandler", "SapItemSyncHandler",
                     "SapPurchaseOrderSyncHandler", "SapPaymentTermSyncHandler"
                 })
        {
            registration.Should().Contain($"AddScoped<ISapSyncEntityHandler, {handler}>()");
        }

        sql.Should().Contain("(N'Suppliers', N'Proveedores', 1, 0, 1, 1, 1)")
            .And.Contain("(N'Items', N'Articulos', 1, 0, 1, 0, 1)")
            .And.Contain("(N'PaymentTerms', N'Condiciones de pago', 1, 0, 1, 0, 1)")
            .And.Contain("(N'PurchaseOrders', N'Ordenes de compra', 0, 0, 0, 0, 0)")
            .And.NotContain("(N'Warehouses',");
    }

    private static void AssertRepositoryProceduresExist(string repository, string sql, int expectedCount)
    {
        var procedures = Regex.Matches(repository, "\"dbo\\.(SP_NA_[A-Z0-9_]+)\"")
            .Select(match => match.Groups[1].Value)
            .Distinct()
            .ToArray();

        procedures.Should().HaveCount(expectedCount);
        procedures.Should().OnlyContain(procedure =>
            sql.Contains($"CREATE OR ALTER PROCEDURE dbo.{procedure}", StringComparison.Ordinal));
    }

    private static string MasterSql() => Read("database", "sql", MasterMigration);
    private static string HardeningSql() => Read("database", "sql", HardeningMigration);
    private static string TenantSql() => Read("database", "sql", TenantMigration)
        + Environment.NewLine + Read("database", "sql", TenantOperationsMigration);

    private static string Read(params string[] parts) =>
        File.ReadAllText(Path.Combine([Root(), .. parts]));

    private static string Root()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root not found.");
    }
}
