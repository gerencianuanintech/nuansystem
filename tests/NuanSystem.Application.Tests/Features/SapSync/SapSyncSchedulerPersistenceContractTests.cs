using System.Text.RegularExpressions;
using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapSyncSchedulerPersistenceContractTests
{
    private const string Migration = "155_master_sap_sync_scheduler.sql";

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
            .And.Contain("SP_NA_PATCH_SAPSYNCSCHEDULERESERVAR");
        legacyRepository.Should().Contain("SP_NA_GET_SAPSYNCENTITYSETTINGSHABILITADOS");
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
