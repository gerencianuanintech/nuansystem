using System.Text.Json.Nodes;
using NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

[Trait("Category", "OfflineSafety")]
public sealed class Phase7SqlFailureTests
{
    [Theory]
    [InlineData(Phase7SqlFailureCase.AuditInsert)]
    [InlineData(Phase7SqlFailureCase.OutboxInsert)]
    [InlineData(Phase7SqlFailureCase.SyncAuditInsert)]
    [InlineData(Phase7SqlFailureCase.AfterProcedure)]
    [InlineData(Phase7SqlFailureCase.ProcedureError)]
    public async Task S05_S09_original_failure_and_complete_rollback_are_required(Phase7SqlFailureCase fault)
    {
        var harness = new Phase7FailureOfflineHarness(fault);
        var result = await harness.RunAsync();
        Assert.Same(harness.ExpectedFailure, result.Failure);
        Assert.Equal(0, harness.Commits);
        Assert.Equal(1, harness.Rollbacks);
        Assert.Equal(2, harness.Observations);
        Assert.Equal(1, harness.HandlerCalls);
    }

    [Theory]
    [InlineData(Phase7SqlFailureCase.CancelAfterWrites)]
    [InlineData(Phase7SqlFailureCase.CancelDuringCommand)]
    public async Task S10_S11_cancel_only_after_acknowledged_barrier_then_observe_rollback(Phase7SqlFailureCase fault)
    {
        var harness = new Phase7FailureOfflineHarness(fault);
        var result = await harness.RunAsync();
        Assert.IsAssignableFrom<OperationCanceledException>(result.Failure);
        Assert.True(result.BarrierAcknowledged);
        Assert.Equal(0, harness.Commits);
        Assert.Equal(1, harness.Rollbacks);
        Assert.Equal(2, harness.Observations);
    }

    [Fact]
    public void Rollback_comparison_does_not_normalize_ids_dates_or_missing_tables()
    {
        var before = EmptyTables();
        var after = EmptyTables();
        after["BusinessPartners"].Add(new JsonObject { ["Id"] = 9, ["IsDeleted"] = true });
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlRollbackSnapshot.AssertUnchanged(new(before), new(after)));
        before.Remove("SyncAudit");
        Assert.Throws<Phase7SqlGuardException>(() => new Phase7SqlRollbackSnapshot(before));
    }

    internal static Dictionary<string, JsonArray> EmptyTables() => Phase7SqlRollbackSnapshot.RequiredTables.ToDictionary(x => x, _ => new JsonArray(), StringComparer.Ordinal);

    [Theory]
    [InlineData(Phase7FailureOperation.Update)]
    [InlineData(Phase7FailureOperation.Delete)]
    public async Task All_failure_points_preserve_existing_business_and_child_rows(Phase7FailureOperation operation)
    {
        foreach (var fault in Enum.GetValues<Phase7SqlFailureCase>())
        {
            var harness = new Phase7FailureOfflineHarness(fault, operation);
            await harness.RunAsync();
            Assert.Equal(0, harness.Commits);
            Assert.Equal(1, harness.Rollbacks);
        }
    }

    [Theory]
    [InlineData(Phase7FailureOperation.Create)]
    [InlineData(Phase7FailureOperation.Update)]
    [InlineData(Phase7FailureOperation.Delete)]
    public async Task Any_durable_leak_is_rejected_even_when_the_expected_failure_was_raised(Phase7FailureOperation operation)
    {
        var harness = new Phase7FailureOfflineHarness(Phase7SqlFailureCase.SyncAuditInsert, operation, leakWrites: true);
        var error = await Assert.ThrowsAsync<Phase7SqlGuardException>(() => harness.RunAsync());
        Assert.Equal("PHASE7_SQL_ROLLBACK_STATE_CHANGED", error.ErrorCode);
        Assert.Same(harness.ExpectedFailure, error.Data["Phase7Sql.PrimaryFailure"]);
    }

    [Fact]
    public async Task Secondary_rollback_error_does_not_replace_the_original_failure()
    {
        var harness = new Phase7FailureOfflineHarness(Phase7SqlFailureCase.OutboxInsert) { RollbackFailure = new InvalidOperationException("rollback") };
        var evidence = await harness.RunAsync();
        Assert.Same(harness.ExpectedFailure, evidence.Failure);
        Assert.Same(harness.RollbackFailure, evidence.Failure.Data["NuanSystem.Transaction.RollbackException"]);
    }

    [Fact]
    public void Fault_script_is_run_scoped_inert_without_markers_and_never_rewrites_existing_triggers()
    {
        var runId = Guid.Parse("12345678-1234-1234-1234-123456789abc");
        var script = Phase7SqlFaultScript.Render(runId, "A", 11);
        Assert.Contains($"NuanSystem_Phase7_{runId:N}_A", script.Sql);
        Assert.Contains("SESSION_CONTEXT(N''P7_RunId'')", script.Sql);
        Assert.Contains("SESSION_CONTEXT(N''P7_CaseCode'')", script.Sql);
        Assert.Contains("Phase7.Purpose", script.Sql);
        Assert.Contains("P7_12345678_", script.Sql);
        Assert.Contains("THROW 51505", script.Sql);
        Assert.Contains("THROW 51506", script.Sql);
        Assert.Contains("THROW 51507", script.Sql);
        Assert.DoesNotContain("DISABLE TRIGGER", script.Sql);
        Assert.DoesNotContain("CREATE OR ALTER", script.Sql);
        Assert.Equal(64, script.Sha256.Length);
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlFaultScript.Render(Guid.Empty, "A", 11));
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlFaultScript.Render(runId, "Master", 11));
    }

    [Theory]
    [InlineData("Pooling=true")]
    [InlineData("Pooling=false;MultipleActiveResultSets=true")]
    public void Marker_sessions_reject_pooling_and_parallel_batches_without_opening_a_connection(string settings)
    {
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlFaultSession.RequireDedicatedConnection(settings));
        Phase7SqlFaultSession.RequireDedicatedConnection("Pooling=false;MultipleActiveResultSets=false;Enlist=false");
    }

    [Fact]
    public async Task Marker_cleanup_uses_its_own_budget_and_preserves_primary_exception()
    {
        var secondary = new InvalidOperationException("cleanup");
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var original = new OperationCanceledException(cancellation.Token);
        var actual = await Assert.ThrowsAsync<OperationCanceledException>(() => Phase7SqlFaultSession.RunWithCleanupAsync<int>(
            () => Task.FromException<int>(original), token =>
            {
                Assert.False(token.IsCancellationRequested);
                return Task.FromException(secondary);
            }));
        Assert.Same(original, actual);
        Assert.Same(secondary, actual.Data["Phase7Sql.SessionCleanupException"]);
    }

    [Fact]
    public async Task Unexpected_exception_is_rethrown_unchanged()
    {
        var unexpected = new InvalidOperationException("unexpected");
        var actual = await Assert.ThrowsAsync<InvalidOperationException>(() => Phase7SqlFailureScenarios.RunAsync(Phase7SqlFailureCase.AuditInsert,
            (_, _) => Task.FromException(unexpected), _ => Task.FromResult(new Phase7SqlRollbackSnapshot(EmptyTables())),
            _ => false, TimeSpan.FromSeconds(5)));
        Assert.Same(unexpected, actual);
    }

    [Fact]
    public async Task Successful_return_is_not_accepted_as_a_failure_scenario()
    {
        var error = await Assert.ThrowsAsync<Phase7SqlGuardException>(() => Phase7SqlFailureScenarios.RunAsync(Phase7SqlFailureCase.AuditInsert,
            (_, _) => Task.CompletedTask, _ => Task.FromResult(new Phase7SqlRollbackSnapshot(EmptyTables())),
            _ => true, TimeSpan.FromSeconds(5)));
        Assert.Equal("PHASE7_SQL_EXPECTED_FAILURE_NOT_RAISED", error.ErrorCode);
    }

    [Fact]
    public async Task Unacknowledged_cancellation_is_rejected()
    {
        var error = await Assert.ThrowsAsync<Phase7SqlGuardException>(() => Phase7SqlFailureScenarios.RunAsync(Phase7SqlFailureCase.CancelDuringCommand,
            (_, token) => Task.FromException(new OperationCanceledException(token)), _ => Task.FromResult(new Phase7SqlRollbackSnapshot(EmptyTables())),
            _ => true, TimeSpan.FromSeconds(5)));
        Assert.Equal("PHASE7_SQL_CANCELLATION_NOT_ACKNOWLEDGED", error.ErrorCode);
    }

    [Fact]
    public async Task Linked_inner_operation_token_is_accepted_after_its_barrier()
    {
        var evidence = await Phase7SqlFailureScenarios.RunAsync(Phase7SqlFailureCase.CancelAfterWrites, async (barrier, token) =>
        {
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(token);
            await barrier.WaitForCancellationAsync(Phase7SqlFailureCase.CancelAfterWrites, linked.Token);
        }, _ => Task.FromResult(new Phase7SqlRollbackSnapshot(EmptyTables())), _ => false, TimeSpan.FromSeconds(5));
        Assert.True(evidence.BarrierAcknowledged);
    }

    [Fact]
    public async Task Caller_cancellation_cannot_pass_as_the_expected_test_cancellation()
    {
        using var caller = new CancellationTokenSource();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Phase7SqlFailureScenarios.RunAsync(Phase7SqlFailureCase.CancelAfterWrites,
            async (barrier, token) =>
            {
                barrier.Acknowledge(Phase7SqlFailureCase.CancelAfterWrites, token);
                caller.Cancel();
                await Task.Delay(Timeout.InfiniteTimeSpan, token);
            }, _ => Task.FromResult(new Phase7SqlRollbackSnapshot(EmptyTables())), _ => true, TimeSpan.FromSeconds(5), caller.Token));
    }

    [Fact]
    public async Task Deadline_before_barrier_fails_and_does_not_observe_during_unwind()
    {
        var observations = 0;
        var stopped = false;
        var error = await Assert.ThrowsAsync<Phase7SqlGuardException>(() => Phase7SqlFailureScenarios.RunAsync(Phase7SqlFailureCase.CancelDuringCommand,
            async (_, token) =>
            {
                try { await Task.Delay(Timeout.InfiniteTimeSpan, token); }
                finally { stopped = true; }
            }, _ =>
            {
                observations++;
                return Task.FromResult(new Phase7SqlRollbackSnapshot(EmptyTables()));
            }, _ => true, TimeSpan.FromSeconds(1)));
        Assert.Equal("PHASE7_SQL_FAILURE_CASE_DEADLINE", error.ErrorCode);
        Assert.True(stopped);
        Assert.Equal(1, observations);
    }

    [Fact]
    public void Snapshots_are_detached_and_compare_exact_generated_values()
    {
        var tables = EmptyTables();
        tables["AuditCatalogChanges"].Add(new JsonObject { ["Id"] = 4, ["CreatedAt"] = "before" });
        var frozen = new Phase7SqlRollbackSnapshot(tables);
        tables["AuditCatalogChanges"][0]!["CreatedAt"] = "after";
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlRollbackSnapshot.AssertUnchanged(frozen, new(tables)));
    }

    [Theory]
    [InlineData("P7_12345678_S05", Phase7SqlFailureCase.OutboxInsert)]
    [InlineData("P7_00000000_S05", Phase7SqlFailureCase.AuditInsert)]
    [InlineData("P7_12345678_S05;DROP", Phase7SqlFailureCase.AuditInsert)]
    public void Fault_options_must_match_the_case_and_run(string code, Phase7SqlFailureCase fault)
    {
        var options = new Phase7SqlFaultSessionOptions(Guid.Parse("12345678-1234-1234-1234-123456789abc"), code, fault);
        Assert.Throws<Phase7SqlGuardException>(() => options.Validate());
    }

    [Fact]
    public void Hexadecimal_run_prefixes_follow_handler_uppercase_normalization()
    {
        var run = Guid.Parse("abcdef12-1234-1234-1234-123456789abc");
        var options = new Phase7SqlFaultSessionOptions(run, "P7_abcdef12_S05a", Phase7SqlFailureCase.AuditInsert);
        options.Validate();
        Assert.Equal("P7_ABCDEF12_S05A", options.CanonicalCaseCode);
        Assert.Contains("P7_ABCDEF12_", Phase7SqlFaultScript.Render(run, "B", 12).Sql);
    }

    [Fact]
    public async Task Observer_failure_is_inconclusive_and_keeps_the_primary_operation_error()
    {
        var operationFailure = new InvalidOperationException("operation");
        var observerFailure = new InvalidOperationException("observer");
        var reads = 0;
        var actual = await Assert.ThrowsAsync<InvalidOperationException>(() => Phase7SqlFailureScenarios.RunAsync(Phase7SqlFailureCase.OutboxInsert,
            (_, _) => Task.FromException(operationFailure), _ => ++reads == 1 ? Task.FromResult(new Phase7SqlRollbackSnapshot(EmptyTables()))
                : Task.FromException<Phase7SqlRollbackSnapshot>(observerFailure), _ => true, TimeSpan.FromSeconds(5)));
        Assert.Same(observerFailure, actual);
        Assert.Same(operationFailure, actual.Data["Phase7Sql.PrimaryFailure"]);
    }
}
