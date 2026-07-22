using System.Text.RegularExpressions;
using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.Operations;

public sealed class WorkerHeartbeatSqlMigrationTests
{
    private static readonly string[] RequiredAlteredColumns =
    [
        "WorkerType", "HostName", "WorkerInstance", "LifecycleState", "IsEnabled", "EnabledCompanyCount",
        "PendingCount", "RetryScheduledCount", "DeadLetterCount", "RecentDeadLetterCount", "ActiveLeaseCount",
        "ExpiredLeaseCount"
    ];

    [Theory]
    [InlineData("120_master_worker_heartbeat_operations.sql")]
    [InlineData("122_master_worker_heartbeat_operations_idempotency_fix.sql")]
    public void EveryAlterColumn_IsGuardedByExactCatalogMetadata(string fileName)
    {
        var sql = Read("database", "sql", fileName);
        var alterLines = Regex.Matches(sql,
                @"(?im)^\s*(?<line>IF NOT EXISTS\(SELECT 1 FROM sys\.columns.*?ALTER TABLE dbo\.WorkerHeartbeat ALTER COLUMN (?<column>\w+).*?;)\s*$")
            .Cast<Match>().ToArray();

        alterLines.Select(x => x.Groups["column"].Value).Should().BeEquivalentTo(RequiredAlteredColumns);
        alterLines.Should().OnlyContain(x =>
            x.Value.Contains("JOIN sys.types", StringComparison.Ordinal)
            && x.Value.Contains("c.max_length=", StringComparison.Ordinal)
            && x.Value.Contains("c.is_nullable=", StringComparison.Ordinal));

        Regex.Matches(sql, @"(?im)^\s*ALTER TABLE dbo\.WorkerHeartbeat ALTER COLUMN").Count
            .Should().Be(0, "un ALTER COLUMN incondicional rompe la segunda y tercera ejecucion");
    }

    [Theory]
    [InlineData("120_master_worker_heartbeat_operations.sql")]
    [InlineData("122_master_worker_heartbeat_operations_idempotency_fix.sql")]
    public void LogicalIdentityRepair_IsTransactionalConditionalAndRestoresExactFilteredUniqueIndex(string fileName)
    {
        var sql = Read("database", "sql", fileName);
        var repairStart = sql.IndexOf("BEGIN TRY", StringComparison.Ordinal);
        var transaction = sql.IndexOf("BEGIN TRANSACTION", repairStart, StringComparison.Ordinal);
        var drop = sql.IndexOf("DROP INDEX UX_WorkerHeartbeat_LogicalIdentity", transaction, StringComparison.Ordinal);
        var firstAlter = sql.IndexOf("ALTER TABLE dbo.WorkerHeartbeat ALTER COLUMN WorkerType", drop, StringComparison.Ordinal);
        var create = sql.IndexOf("CREATE UNIQUE INDEX UX_WorkerHeartbeat_LogicalIdentity ON dbo.WorkerHeartbeat(WorkerType,HostName,WorkerInstance) WHERE WorkerInstance IS NOT NULL", firstAlter, StringComparison.Ordinal);
        var commit = sql.IndexOf("COMMIT;", create, StringComparison.Ordinal);

        repairStart.Should().BeGreaterThanOrEqualTo(0);
        transaction.Should().BeGreaterThan(repairStart);
        drop.Should().BeGreaterThan(transaction);
        firstAlter.Should().BeGreaterThan(drop);
        create.Should().BeGreaterThan(firstAlter);
        commit.Should().BeGreaterThan(create);
        sql.Should().Contain("@IdentityRepair=1 OR @LogicalIndexRepair=1")
            .And.Contain("i.is_unique=1 AND i.type=2 AND i.has_filter=1")
            .And.Contain("ic.key_ordinal=1 AND c.name=N'WorkerType'")
            .And.Contain("ic.key_ordinal=2 AND c.name=N'HostName'")
            .And.Contain("ic.key_ordinal=3 AND c.name=N'WorkerInstance'")
            .And.Contain("BEGIN CATCH")
            .And.Contain("IF XACT_STATE()<>0 ROLLBACK");
    }

    [Theory]
    [InlineData("120_master_worker_heartbeat_operations.sql")]
    [InlineData("122_master_worker_heartbeat_operations_idempotency_fix.sql")]
    public void DefaultsChecksAndProcedures_AreRepairableWithoutDuplicateDataOrLegacyBreakage(string fileName)
    {
        var sql = Read("database", "sql", fileName);

        sql.Should().Contain("DECLARE ensure_defaults CURSOR LOCAL FAST_FORWARD")
            .And.Contain("sys.default_constraints")
            .And.Contain("CK_WorkerHeartbeat_LifecycleState")
            .And.Contain("CK_WorkerHeartbeat_OperationalCounts")
            .And.Contain("CK_WorkerHeartbeat_StorageFreePercent")
            .And.Contain("CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_WORKERHEARTBEAT_REGISTRAR")
            .And.Contain("CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_WORKERHEARTBEAT_LISTARPORCONFIGURACION")
            .And.Contain("WHERE InstanceName=@InstanceName")
            .And.Contain("@WorkerType nvarchar(40)=N'SapSync'")
            .And.Contain("@WorkerInstance nvarchar(120)=NULL")
            .And.NotContain("DELETE FROM dbo.WorkerHeartbeat")
            .And.NotContain("TRUNCATE TABLE")
            .And.NotContain("DROP TABLE")
            .And.NotContain("DROP COLUMN");

        RequiredAlteredColumns.Should().OnlyContain(column =>
            sql.Contains($"c.name=N'{column}'", StringComparison.Ordinal));
    }

    [Fact]
    public void ForwardRepair_CoversFreshCompleteAndPartial120StatesWithoutRewriting120History()
    {
        var repair = Read("database", "sql", "122_master_worker_heartbeat_operations_idempotency_fix.sql");
        var metadataRepair = repair.IndexOf("DECLARE @IdentityRepair", StringComparison.Ordinal);
        var procedureRepair = repair.IndexOf("CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_WORKERHEARTBEAT_REGISTRAR", StringComparison.Ordinal);
        var history = repair.IndexOf("Version=N'20260722.122'", StringComparison.Ordinal);

        metadataRepair.Should().BeGreaterThanOrEqualTo(0);
        procedureRepair.Should().BeGreaterThan(metadataRepair);
        history.Should().BeGreaterThan(procedureRepair);
        repair.Should().Contain("COL_LENGTH")
            .And.Contain("NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260722.122')")
            .And.Contain("INSERT dbo.MasterSchemaHistory(Version,Description) VALUES(N'20260722.122'")
            .And.NotContain("DELETE FROM dbo.MasterSchemaHistory")
            .And.NotContain("UPDATE dbo.MasterSchemaHistory")
            .And.NotContain("VALUES(N'20260721.120'");
    }

    [Fact]
    public void OriginalAndForwardRepair_RegisterOneVersionAndPreservePermissionOperationSeeds()
    {
        var original = Read("database", "sql", "120_master_worker_heartbeat_operations.sql");
        var repair = Read("database", "sql", "122_master_worker_heartbeat_operations_idempotency_fix.sql");

        original.Should().Contain("NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260721.120')")
            .And.Contain("VALUES(N'20260721.120'");
        repair.Should().Contain("NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260722.122')")
            .And.Contain("VALUES(N'20260722.122'");
        foreach (var sql in new[] { original, repair })
        {
            sql.Should().Contain("SRI.WORKER.HEALTH.VIEW")
                .And.Contain("view-worker-health")
                .And.Contain("NOT EXISTS(SELECT 1 FROM dbo.RolePermissions")
                .And.Contain("NOT EXISTS(SELECT 1 FROM dbo.SecurityRoleFormOperations");
        }
    }

    [Fact]
    public void MasterInitializer_RunsForwardRepairImmediatelyAfter120()
    {
        var initializer = Read("src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerMasterDatabaseInitializer.cs");
        var original = initializer.IndexOf("120_master_worker_heartbeat_operations.sql", StringComparison.Ordinal);
        var repair = initializer.IndexOf("122_master_worker_heartbeat_operations_idempotency_fix.sql", StringComparison.Ordinal);

        original.Should().BeGreaterThanOrEqualTo(0);
        repair.Should().BeGreaterThan(original);
    }

    [Fact]
    public void SapConsumersAndLegacyInstanceIdentity_RemainCompatible()
    {
        foreach (var worker in new[] { "SapSyncWorker.cs", "SapRetryWorker.cs", "SapOutboxWorker.cs" })
        {
            Read("src", "Backend", "NuanSystem.SyncWorker", "Workers", worker)
                .Should().Contain("NuanSystem.Application.Features.Operations")
                .And.Contain("InstanceName");
        }

        var original = Read("database", "sql", "120_master_worker_heartbeat_operations.sql");
        var legacy = Read("database", "sql", "049_master_sap_sync_worker.sql");
        original.Should().Contain("WorkerInstance=COALESCE(NULLIF(WorkerInstance,N''),InstanceName)")
            .And.NotContain("DROP CONSTRAINT UQ_WorkerHeartbeat_InstanceName");
        legacy.Should().Contain("CONSTRAINT UQ_WorkerHeartbeat_InstanceName UNIQUE (InstanceName)");
    }

    private static string Read(params string[] parts) => File.ReadAllText(Path.Combine([Root(), .. parts]));

    private static string Root()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root not found.");
    }
}
