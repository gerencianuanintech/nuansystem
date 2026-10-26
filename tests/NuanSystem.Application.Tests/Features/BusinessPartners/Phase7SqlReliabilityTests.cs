using NuanSystem.Application.Tests.Infrastructure.Phase7Sql;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.BusinessPartners.Dtos;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

[Trait("Category", "OfflineSafety")]
public sealed class Phase7SqlReliabilityTests
{
    [Fact]
    public async Task S12_two_companies_keep_the_same_business_key_isolated()
    {
        var a = new Phase7ReliabilityOfflineHarness(11);
        var b = new Phase7ReliabilityOfflineHarness(12);
        await Phase7SqlReliabilityScenarios.TenantIsolationAsync(a.Scope().Unit, b.Scope().Unit,
            a.Command("Empresa A"), b.Command("Empresa B"), a.ObserveAsync, b.ObserveAsync, 11, 12);
        Assert.Equal((await a.ObserveAsync(default)).Partners[0].Id, (await b.ObserveAsync(default)).Partners[0].Id); // local ids may coincide
    }

    [Fact]
    public async Task S13_reused_physical_session_has_new_scope_and_clean_transaction_state()
    {
        var engine = new Phase7ReliabilityOfflineHarness(11);
        var first = engine.Scope();
        await first.Unit.ExecuteAsync((_, _) => Task.FromResult(1));
        var next = engine.Scope();
        await next.Unit.ExecuteAsync((_, _) => Task.FromResult(2));
        Assert.Equal(2, engine.Connections);
        Assert.Single(engine.Sessions.Select(s => s.Spid).Distinct());
        Assert.All(engine.Sessions, s => Assert.False(s.HasFaultMarkers));
        Phase7SqlScopeHygiene.AssertReuse(engine.Sessions, "Offline_11", 111);
    }

    [Fact]
    public async Task S14_repeated_create_keeps_one_identity_and_one_created_event()
    {
        var engine = new Phase7ReliabilityOfflineHarness(11);
        await Phase7SqlReliabilityScenarios.RepeatedCreateAsync(engine.Scope().Unit, engine.Command(), engine.ObserveAsync, 11);
    }

    [Fact]
    public async Task S15_independent_units_race_and_only_one_create_survives()
    {
        var engine = new Phase7ReliabilityOfflineHarness(11);
        var outcomes = await Phase7SqlReliabilityScenarios.ConcurrentCreateAsync(engine.Scope().Unit, engine.Scope().Unit,
            engine.Command(), engine.ObserveAsync, 11, engine.IsExpectedConflict, TimeSpan.FromSeconds(5));
        Assert.Single(outcomes, o => o.Result?.IsSuccess == true);
        Assert.Equal(2, engine.Connections);
    }

    [Fact]
    public async Task S16_repeated_update_records_an_intention_per_successful_operation()
    {
        var engine = new Phase7ReliabilityOfflineHarness(11);
        var evidence = await Phase7SqlReliabilityScenarios.RepeatedUpdateAsync(engine.Scope().Unit, engine.Command(), engine.ObserveAsync, 11);
        Assert.Equal(1, evidence.EventDelta);
        Assert.Equal(1, evidence.SyncAuditDelta);
        Assert.Equal(0, evidence.AuditDelta);
        var state = await engine.ObserveAsync(default);
        Assert.Equal(3, state.Events.Count);
        Assert.Equal(2, state.Audits.Count); // unchanged repeat adds intention but no fake field-change audit
    }

    [Fact]
    public async Task S17_closing_only_owner_before_commit_rolls_back_without_retry()
    {
        var engine = new Phase7ReliabilityOfflineHarness(11);
        var evidence = await Phase7SqlRecoveryScenarios.CloseBeforeCommitAsync(engine.Scope(closeAfterEnqueue: true).Unit,
            engine.Command(), engine.RollbackSnapshotAsync);
        Assert.IsType<Phase7SqlOwnerClosedException>(evidence);
        Assert.NotNull(evidence.Data["NuanSystem.Transaction.RollbackException"]);
        Assert.Equal(1, engine.Connections);
        Assert.Equal(1, engine.OwnedClosures);
        Assert.Empty((await engine.ObserveAsync(default)).Events);
    }

    [Fact]
    public async Task S18_context_lifetime_company_and_overlapping_execution_are_enforced()
    {
        var engine = new Phase7ReliabilityOfflineHarness(11);
        var scope = engine.Scope();
        await Phase7SqlRecoveryScenarios.InvalidContextsAsync(scope, () => engine.Connections);
        Assert.Empty((await engine.ObserveAsync(default)).Events);
        Assert.Equal(0, engine.RepositoryReads);
        Assert.Equal(6, engine.Connections);
    }

    [Theory]
    [InlineData(0, "Lab", 101, 1, 1, false)]
    [InlineData(52, "Other", 101, 1, 1, false)]
    [InlineData(52, "Lab", 102, 1, 1, false)]
    [InlineData(52, "Lab", 101, 2, 1, false)]
    [InlineData(52, "Lab", 101, 1, -1, false)]
    [InlineData(52, "Lab", 101, 1, 1, true)]
    public void Session_hygiene_rejects_wrong_target_or_residual_state(int spid, string database, int id, int count, int state, bool markers)
    {
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlScopeHygiene.AssertClean(new(spid, database, id, count, state, markers), "Lab", 101));
    }

    [Fact]
    public void Pooling_flag_alone_does_not_prove_physical_session_reuse()
    {
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlScopeHygiene.AssertReuse([new(52, "Lab", 101, 1, 1, false), new(53, "Lab", 101, 1, 1, false)], "Lab", 101));
    }

    [Fact]
    public async Task Dirty_reused_sessions_fail_before_business_access()
    {
        var engine = new Phase7ReliabilityOfflineHarness(11) { DirtySessions = true };
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => engine.Scope().Unit.ExecuteAsync((_, _) => Task.FromResult(1)));
        engine.DirtySessions = false;
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => engine.Scope().Unit.ExecuteAsync((_, _) => Task.FromResult(1)));
        Assert.Equal(0, engine.RepositoryReads);
        Assert.Empty((await engine.ObserveAsync(default)).Partners);
    }

    [Fact]
    public async Task Wrong_company_in_event_evidence_is_rejected()
    {
        var a = new Phase7ReliabilityOfflineHarness(11);
        var b = new Phase7ReliabilityOfflineHarness(12);
        var error = await Assert.ThrowsAsync<Phase7SqlGuardException>(() => Phase7SqlReliabilityScenarios.TenantIsolationAsync(a.Scope().Unit, b.Scope().Unit,
            a.Command("Empresa A"), b.Command("Empresa B"), a.ObserveAsync, async ct =>
            {
                var state = await b.ObserveAsync(ct);
                return state with { Events = state.Events.Select(e => e with { CompanyId = 11 }).ToArray() };
            }, 11, 12));
        Assert.Equal("PHASE7_SQL_RELIABILITY_EVENT_SCOPE", error.ErrorCode);
    }

    [Fact]
    public async Task Missing_uniqueness_cannot_pass_the_concurrent_case()
    {
        var engine = new Phase7ReliabilityOfflineHarness(11) { EnforceUnique = false };
        var error = await Assert.ThrowsAsync<Phase7SqlGuardException>(() => Phase7SqlReliabilityScenarios.ConcurrentCreateAsync(engine.Scope().Unit,
            engine.Scope().Unit, engine.Command(), engine.ObserveAsync, 11, engine.IsExpectedConflict, TimeSpan.FromSeconds(5)));
        Assert.Equal("PHASE7_SQL_CONCURRENT_WINNER_COUNT", error.ErrorCode);
        Assert.Equal(2, (await engine.ObserveAsync(default)).Partners.Count);
    }

    [Fact]
    public async Task Unexpected_loser_error_is_not_treated_as_a_duplicate_or_deadlock()
    {
        var original = new NullReferenceException("unexpected");
        var engine = new Phase7ReliabilityOfflineHarness(11) { ConflictFailure = original };
        var error = await Assert.ThrowsAsync<Phase7SqlGuardException>(() => Phase7SqlReliabilityScenarios.ConcurrentCreateAsync(engine.Scope().Unit,
            engine.Scope().Unit, engine.Command(), engine.ObserveAsync, 11, engine.IsExpectedConflict, TimeSpan.FromSeconds(5)));
        Assert.Equal("PHASE7_SQL_CONCURRENT_UNEXPECTED_FAILURE", error.ErrorCode);
        Assert.Same(original, error.Data["Phase7Sql.ConcurrentFailure"]);
    }

    [Fact]
    public async Task Deadlock_victim_is_recorded_without_replaying_the_callback()
    {
        var engine = new Phase7ReliabilityOfflineHarness(11) { SimulateDeadlock = true };
        var attempts = await Phase7SqlReliabilityScenarios.ConcurrentCreateAsync(engine.Scope().Unit, engine.Scope().Unit,
            engine.Command(), engine.ObserveAsync, 11, engine.IsExpectedConflict, TimeSpan.FromSeconds(5));
        Assert.Contains(attempts, a => a.Error?.Message == "Offline deadlock victim.");
        Assert.Equal(2, engine.Connections);
    }

    [Fact]
    public async Task Barrier_deadline_is_a_failure_and_waits_for_the_active_contender_to_stop()
    {
        var engine = new Phase7ReliabilityOfflineHarness(11);
        var observations = 0;
        var error = await Assert.ThrowsAsync<Phase7SqlGuardException>(() => Phase7SqlReliabilityScenarios.ConcurrentCreateAsync(engine.Scope().Unit,
            new RejectingUnit(), engine.Command(), ct => { observations++; return engine.ObserveAsync(ct); }, 11, _ => true, TimeSpan.FromSeconds(1)));
        Assert.Equal("PHASE7_SQL_CONCURRENT_CASE_DEADLINE", error.ErrorCode);
        Assert.Equal(1, observations);
        Assert.Empty((await engine.ObserveAsync(default)).Partners);
    }

    [Fact]
    public async Task Dropping_the_second_update_intention_is_rejected()
    {
        var engine = new Phase7ReliabilityOfflineHarness(11);
        var error = await Assert.ThrowsAsync<Phase7SqlGuardException>(() => Phase7SqlReliabilityScenarios.RepeatedUpdateAsync(engine.Scope().Unit,
            engine.Command(), async ct =>
            {
                var state = await engine.ObserveAsync(ct);
                return state.Events.Count == 3 ? state with { Events = state.Events.Take(2).ToArray() } : state;
            }, 11));
        Assert.Equal("PHASE7_SQL_RELIABILITY_EVENT_COUNT", error.ErrorCode);
    }

    [Fact]
    public async Task Unit_remains_usable_after_an_owned_session_closes_without_replaying_create()
    {
        var engine = new Phase7ReliabilityOfflineHarness(11);
        var scope = engine.Scope(closeAfterEnqueue: true);
        await Phase7SqlRecoveryScenarios.CloseBeforeCommitAsync(scope.Unit, engine.Command(), engine.RollbackSnapshotAsync);
        Assert.Equal(1, engine.Connections);
        Assert.Equal(42, await scope.Unit.ExecuteAsync((_, _) => Task.FromResult(42)));
        Assert.Equal(2, engine.Connections);
        Assert.Empty((await engine.ObserveAsync(default)).Partners);
    }

    private sealed class RejectingUnit : IBusinessPartnerUnitOfWork
    {
        public Task<T> ExecuteAsync<T>(Func<IBusinessPartnerTransaction, CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default) =>
            Task.FromException<T>(new InvalidOperationException("Rejected before transaction."));
    }

    [Fact]
    public async Task Owner_close_scenario_rejects_an_adapter_that_replays_the_callback()
    {
        var engine = new Phase7ReliabilityOfflineHarness(11);
        var unit = new ReplayingUnit(engine.Scope(closeAfterEnqueue: true).Unit);
        var error = await Assert.ThrowsAsync<Phase7SqlGuardException>(() => Phase7SqlRecoveryScenarios.CloseBeforeCommitAsync(unit, engine.Command(), engine.RollbackSnapshotAsync));
        Assert.Equal("PHASE7_SQL_OWNER_CALLBACK_REPLAYED", error.ErrorCode);
        Assert.IsType<Phase7SqlOwnerClosedException>(error.Data["Phase7Sql.PrimaryFailure"]);
        Assert.Equal(2, engine.OwnedClosures);
    }

    private sealed class ReplayingUnit(IBusinessPartnerUnitOfWork inner) : IBusinessPartnerUnitOfWork
    {
        public async Task<T> ExecuteAsync<T>(Func<IBusinessPartnerTransaction, CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default)
        {
            try { return await inner.ExecuteAsync(operation, cancellationToken); }
            catch (Phase7SqlOwnerClosedException) { return await inner.ExecuteAsync(operation, cancellationToken); }
        }
    }

    [Fact]
    public async Task Repeated_update_allows_child_row_replacement_while_preserving_parent_and_business_values()
    {
        var engine = new Phase7ReliabilityOfflineHarness(11);
        var command = engine.Command() with
        {
            Addresses = [new(null, null, null, "Main", "Direccion sintetica", null, "EC", null, null, null, null, null, true, true)]
        };
        var ids = new List<int>();
        await Phase7SqlReliabilityScenarios.RepeatedUpdateAsync(engine.Scope().Unit, command, async ct =>
        {
            var state = await engine.ObserveAsync(ct);
            if (state.Events.Count >= 2)
            {
                state.Partners[0].Addresses = [new(state.Events.Count * 100, state.Partners[0].Id, null, null, null, "Main", "Direccion sintetica", null, "EC", null, null, null, null, null, true, true)];
                ids.Add(state.Partners[0].Addresses.Single().Id);
            }
            return state;
        }, 11);
        Assert.Equal(2, ids.Count);
        Assert.NotEqual(ids[0], ids[1]);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Child_id_normalization_cannot_hide_changed_business_values_or_parent(bool changeParent)
    {
        var engine = new Phase7ReliabilityOfflineHarness(11);
        var command = engine.Command() with
        {
            Addresses = [new(null, null, null, "Main", "Direccion sintetica", null, "EC", null, null, null, null, null, true, true)]
        };
        var error = await Assert.ThrowsAsync<Phase7SqlGuardException>(() => Phase7SqlReliabilityScenarios.RepeatedUpdateAsync(engine.Scope().Unit, command,
            async ct =>
            {
                var state = await engine.ObserveAsync(ct);
                if (state.Events.Count == 3)
                {
                    var address = state.Partners[0].Addresses.Single();
                    state.Partners[0].Addresses = [changeParent ? address with { BusinessPartnerId = 999 } : address with { Line1 = "Incorrecta" }];
                }
                return state;
            }, 11));
        Assert.Equal(changeParent ? "PHASE7_SQL_RELIABILITY_CHILD_SCOPE" : "PHASE7_SQL_REPEATED_UPDATE_CHANGED_IDENTITY_OR_BUSINESS", error.ErrorCode);
    }
}
