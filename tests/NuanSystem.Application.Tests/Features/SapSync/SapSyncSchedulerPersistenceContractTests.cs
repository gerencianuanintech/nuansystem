using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapSyncSchedulerPersistenceContractTests
{
    private const string Migration = "155_master_sap_sync_scheduler.sql";
    private const string ContractRepairMigration =
        "156_master_sap_sync_scheduler_dapper_contract.sql";
    private const string Migration155NormalizedSha256 =
        "90F26B0691B3F7362823AD9E77BFA367C304C4CDF7DC218B1672DF3356BC5F41";

    [Fact]
    public void Migration_IsForwardOnlyIdempotentAndOrderedAfter154()
    {
        var sql = Sql();
        var initializer = Read(
            "src", "Backend", "NuanSystem.Persistence", "Services",
            "SqlServerMasterDatabaseInitializer.cs");

        sql.Should().Contain("Version = N'20260730.155'")
            .And.Contain("CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SAPSYNCSCHEDULECANDIDATOSPAGINAR")
            .And.Contain("CREATE OR ALTER PROCEDURE dbo.SP_NA_PATCH_SAPSYNCSCHEDULERESERVAR")
            .And.Contain("CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SAPSYNCENTITYSETTINGSHABILITADOS")
            .And.NotContain("DELETE FROM dbo.MasterSchemaHistory")
            .And.NotContain("UPDATE dbo.MasterSchemaHistory")
            .And.NotContain("DROP TABLE")
            .And.NotContain("ALTER TABLE dbo.SapSyncEntitySettings")
            .And.NotContain("UPDATE dbo.SapSyncEntitySettings")
            .And.NotContain("INSERT dbo.SapSyncEntitySettings")
            .And.NotContain("dbo.SyncProfiles")
            .And.NotContain("dbo.SyncSchedules");

        Regex.Matches(sql, "CREATE OR ALTER PROCEDURE dbo\\.SP_NA_").Count
            .Should().Be(3);
        Regex.Matches(sql, "20260730\\.155").Count.Should().Be(2);
        initializer.IndexOf(Migration, StringComparison.Ordinal)
            .Should().BeGreaterThan(
                initializer.IndexOf(
                    "154_master_sap_sync_profile_api_hardening.sql",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void CandidateProcedure_UsesFairKeysetAndExcludesInactiveOrManualSchedules()
    {
        var sql = Sql();

        sql.Should().Contain("profile.IsActive = 1")
            .And.Contain("entity.IsActive = 1")
            .And.Contain("schedule.IsActive = 1")
            .And.Contain("schedule.ScheduleType <> 'Manual'")
            .And.Contain("schedule.NextExecutionAtUtc IS NULL OR schedule.NextExecutionAtUtc <= @UtcNow")
            .And.Contain("CompanyId > @AfterCompanyId")
            .And.Contain("SortProfileId > @AfterProfileId")
            .And.Contain("ExecutionOrder > @AfterExecutionOrder")
            .And.Contain("SortEntityId > @AfterEntityId")
            .And.Contain("ORDER BY CompanyId, SortProfileId, ExecutionOrder, SortEntityId")
            .And.Contain("@PageSize BETWEEN 1 AND 500");
    }

    [Fact]
    public void Fallback_IsReadOnlyVersionedAndBlockedByAnyNativeProfile()
    {
        var sql = Sql();
        var fallback = Regex.Match(
            sql,
            @"(?s)CAST\('LegacyFallback'.*?FROM dbo\.SapSyncEntitySettings.*?NOT EXISTS.*?\)")
            .Value;

        fallback.Should().Contain("compatibility.LegacyFallbackEnabled = 1")
            .And.Contain("compatibility.CompatibilityVersion")
            .And.Contain("compatibility.RequiredSuccessfulCycles")
            .And.Contain("currentProfile.SourceType = 'Native'")
            .And.Contain("currentProfile.IsDeleted = 0");
        sql.Should().Contain("RequiredSuccessfulCycles")
            .And.Contain("CompatibilityVersion")
            .And.NotContain("SET LegacyFallbackEnabled");
    }

    [Fact]
    public void Reservation_IsAtomicRowVersionGuardedAndRechecksEveryActiveOwner()
    {
        var sql = Sql();
        var procedure = Regex.Match(
            sql,
            @"(?s)CREATE OR ALTER PROCEDURE dbo\.SP_NA_PATCH_SAPSYNCSCHEDULERESERVAR.*?\r?\nGO")
            .Value;

        procedure.Should().Contain("@ExpectedRowVersion varbinary(8)")
            .And.Contain("schedule.RowVersion = @ExpectedRowVersion")
            .And.Contain("schedule.IsActive = 1")
            .And.Contain("entity.IsActive = 1")
            .And.Contain("profile.IsActive = 1")
            .And.Contain("schedule.NextExecutionAtUtc = @ObservedNextExecutionAtUtc")
            .And.Contain("schedule.NextExecutionAtUtc <= @UtcNow")
            .And.Contain("LastScheduledAtUtc = COALESCE(@ScheduledAtUtc")
            .And.Contain("SELECT @@ROWCOUNT");
    }

    [Fact]
    public void Repositories_UseOnlyStoredProceduresParametersAndCancellation()
    {
        var schedulerRepository = Read(
            "src", "Backend", "NuanSystem.Persistence", "Repositories",
            "SapSync", "SapSyncScheduleRepository.cs");
        var legacyRepository = Read(
            "src", "Backend", "NuanSystem.Persistence", "Repositories",
            "SapSync", "SapSyncSettingsRepository.cs");

        foreach (var repository in new[] { schedulerRepository, legacyRepository })
        {
            repository.Should().Contain("CommandDefinition")
                .And.Contain("CommandType.StoredProcedure")
                .And.Contain("cancellationToken: cancellationToken")
                .And.NotMatchRegex(@"(?im)^\s*(SELECT|FROM|JOIN|WHERE)\b")
                .And.NotContain("CommandType.Text");
        }

        schedulerRepository.Should().Contain("SP_NA_GET_SAPSYNCSCHEDULECANDIDATOSPAGINAR")
            .And.Contain("SP_NA_PATCH_SAPSYNCSCHEDULERESERVAR")
            .And.Contain("ReadAsync<SapSyncScheduleCandidateRow>")
            .And.NotContain("ReadAsync<SapSyncScheduleCandidate>");
        legacyRepository.Should().Contain("SP_NA_GET_SAPSYNCENTITYSETTINGSHABILITADOS");
    }

    [Fact]
    public void ContractRepair_IsForwardOnlyIdempotentAndOrderedAfter155()
    {
        var sql = Read("database", "sql", ContractRepairMigration);
        var initializer = Read(
            "src", "Backend", "NuanSystem.Persistence", "Services",
            "SqlServerMasterDatabaseInitializer.cs");

        sql.Should().Contain("Version = N'20260731.156'")
            .And.Contain("CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SAPSYNCSCHEDULECANDIDATOSPAGINAR")
            .And.NotContain("CREATE OR ALTER PROCEDURE dbo.SP_NA_PATCH_SAPSYNCSCHEDULERESERVAR")
            .And.NotContain("ALTER TABLE")
            .And.NotContain("UPDATE dbo.SapSync")
            .And.NotContain("INSERT dbo.SapSync")
            .And.NotContain("DELETE FROM dbo.SapSync");
        Regex.Matches(sql, "20260731\\.156").Count.Should().Be(2);
        Regex.Matches(
                sql,
                "CREATE OR ALTER PROCEDURE dbo\\.SP_NA_GET_SAPSYNCSCHEDULECANDIDATOSPAGINAR")
            .Count.Should().Be(1);
        initializer.IndexOf(ContractRepairMigration, StringComparison.Ordinal)
            .Should().BeGreaterThan(initializer.IndexOf(Migration, StringComparison.Ordinal));
    }

    [Fact]
    public void ContractRepair_UsesHomogeneousUnionTypesForDapperRow()
    {
        var sql = Read("database", "sql", ContractRepairMigration);
        var branches = Regex.Split(sql, @"\r?\n\s*UNION ALL\s*\r?\n");

        branches.Should().HaveCount(2);
        foreach (var branch in branches)
        {
            branch.Should().Contain("AS varchar(30)) AS CandidateSource")
                .And.Contain("AS nvarchar(50)) AS CompanyCode")
                .And.Contain("AS nvarchar(80)) AS ProfileCode")
                .And.Contain("AS nvarchar(160)) AS ProfileName")
                .And.Contain("AS nvarchar(80)) AS EntityCode")
                .And.Contain("AS varchar(20)) AS Direction")
                .And.Contain("AS varchar(20)) AS SyncMode")
                .And.Contain("AS varchar(20)) AS ScheduleType")
                .And.Contain("AS nvarchar(100)) AS TimeZoneId")
                .And.Contain("AS bigint) AS SortProfileId")
                .And.Contain("AS bigint) AS SortEntityId");
        }

        Regex.Matches(sql, @"COALESCE\(CAST\(capability\..*? AS bit\), CAST\(0 AS bit\)\)")
            .Count.Should().Be(6);
        sql.Should().Contain("CAST(NULL AS bigint) AS ProfileId")
            .And.Contain("CAST(NULL AS bigint) AS ProfileEntityId")
            .And.Contain("CAST(NULL AS bigint) AS ScheduleId")
            .And.Contain("CAST(NULL AS int) AS IntervalMinutes")
            .And.Contain("CAST(NULL AS time(0)) AS ExecutionTime")
            .And.Contain("CAST(NULL AS datetime2(0)) AS NextExecutionAtUtc")
            .And.Contain("CAST(NULL AS datetime2(0)) AS LastScheduledAtUtc")
            .And.Contain("CAST(NULL AS datetime2(0)) AS LastExecutionAtUtc")
            .And.Contain("CAST(NULL AS varbinary(8)) AS ScheduleRowVersion");
    }

    [Fact]
    public void Migration155_RemainsByteSemanticallyUnchanged()
    {
        var normalized = Sql().Replace("\r\n", "\n", StringComparison.Ordinal);
        var hash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(normalized)));

        hash.Should().Be(Migration155NormalizedSha256);
    }

    [Fact]
    public void WorkerHost_PreservesSingleProcessAndDoesNotDispatchExternalHandlers()
    {
        var program = Read(
            "src", "Backend", "NuanSystem.SyncWorker", "Program.cs");
        var worker = Read(
            "src", "Backend", "NuanSystem.SyncWorker", "Workers",
            "SapSyncWorker.cs");
        var scheduler = Read(
            "src", "Backend", "NuanSystem.Application", "Features",
            "SapSync", "Services", "SapSyncScheduler.cs");

        Regex.Matches(program, @"AddHostedService<").Count.Should().Be(3);
        program.Should().Contain("AddHostedService<SapSyncWorker>()")
            .And.Contain("AddHostedService<SapRetryWorker>()")
            .And.Contain("AddHostedService<SapOutboxWorker>()")
            .And.NotContain("MasterBranchSyncWorker")
            .And.NotContain("SriWorker");

        (worker + scheduler).Should().NotContain("ImportFromSapAsync(")
            .And.NotContain("ExportToSapAsync(")
            .And.NotContain("ServiceLayer")
            .And.NotContain("ISri")
            .And.NotContain("SapWarehouse");
        worker.Should().Contain("WorkerTypes.SapSync")
            .And.Contain("LoopDelaySeconds")
            .And.Contain("SapSyncWorkerRuntimeState.ResolveVersion")
            .And.NotContain("logger.LogError(exception")
            .And.NotContain("logger.LogCritical(exception");
    }

    private static string Sql() => Read("database", "sql", Migration);

    private static string Read(params string[] parts) =>
        File.ReadAllText(Path.Combine([Root(), .. parts]));

    private static string Root()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null
               && !File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
               ?? throw new DirectoryNotFoundException("Repository root not found.");
    }
}
