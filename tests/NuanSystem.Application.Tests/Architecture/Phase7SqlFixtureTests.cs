using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

namespace NuanSystem.Application.Tests.Architecture;

[Trait("Category", "OfflineSafety")]
public sealed class Phase7SqlFixtureTests
{
    [Fact]
    public void Suite_continuation_reuses_the_live_fixture_with_remaining_budget_and_original_window()
    {
        var data=new FixtureData();
        using var fixture=data.Create(remainingWindow:TimeSpan.FromMinutes(11));
        var lease=new Phase7SqlSuiteProcessLease(fixture.Manifest.RunId,"candidate",TimeSpan.FromMinutes(10),timeProvider:data.Clock);
        lease.Take(fixture.Manifest.RunId,"candidate","S01");
        data.Clock.Advance(TimeSpan.FromMinutes(2));
        Assert.Throws<Phase7SqlGuardException>(()=>Phase7SqlManifestValidator.Validate(data.ManifestText,data.ApprovalSnapshot,new(true,true),data.Clock.GetUtcNow()));
        var reused=Phase7SqlSuiteLoader.Continue(fixture,lease,fixture.Manifest.RunId,"candidate","S02");
        Assert.Same(fixture,reused);Assert.Equal(TimeSpan.FromMinutes(8),lease.Remaining);
        data.Clock.Advance(TimeSpan.FromMinutes(10));
        Assert.Throws<Phase7SqlGuardException>(()=>Phase7SqlSuiteLoader.Continue(fixture,lease,fixture.Manifest.RunId,"candidate","S03"));
    }
    [Fact]
    public async Task Independent_reliability_scopes_share_only_target_configuration_and_keep_the_verifier()
    {
        var f = new FixtureData();
        using var fixture = f.Create();
        var a = fixture.Tenant("A");
        var next = a.CreateIndependentScope();
        Assert.Same(next,Assert.Single(a.IndependentScopes));
        Assert.Empty(next.IndependentScopes);
        Assert.NotSame(a, next);
        Assert.Equal(a.CompanyId, next.CompanyId);
        var target = fixture.Manifest.TenantA;
        Task<Phase7SqlScopeState> Read(System.Data.IDbConnection c, System.Data.IDbTransaction t, int timeout, CancellationToken ct) =>
            Task.FromResult(new Phase7SqlScopeState(52, target.DatabaseName, target.DatabaseId, 1, 1, false));
        await a.CreateReliabilityScope(readState: Read).Unit.ExecuteAsync((_, _) => Task.FromResult(1));
        await next.CreateReliabilityScope(readState: Read).Unit.ExecuteAsync((_, _) => Task.FromResult(2));
        Assert.Equal(1, a.TotalWriteConnections);
        Assert.Equal(1, next.TotalWriteConnections);
        Assert.Equal(2, f.Verifier.Calls.Count);
        Assert.Single(a.ScopeStates);
        Assert.Single(next.ScopeStates);
        f.WriterA.Received(2).CreateConnection();
        f.WriterB.DidNotReceive().CreateConnection();
    }

    [Fact]
    public async Task Residual_fault_markers_reject_reliability_callback_after_destination_verification()
    {
        var f = new FixtureData();
        using var fixture = f.Create();
        var target = fixture.Manifest.TenantA;
        var scope = fixture.Tenant("A").CreateReliabilityScope(readState: (_, _, _, _) =>
            Task.FromResult(new Phase7SqlScopeState(52, target.DatabaseName, target.DatabaseId, 1, 1, true)));
        var invoked = false;
        var error = await Assert.ThrowsAsync<Phase7SqlGuardException>(() => scope.Unit.ExecuteAsync((_, _) => { invoked = true; return Task.FromResult(1); }));
        Assert.Equal("PHASE7_SQL_SCOPE_NOT_CLEAN", error.ErrorCode);
        Assert.False(invoked);
        Assert.Single(f.Verifier.Calls);
        f.TransactionA.Received(1).Rollback();
        f.TransactionA.DidNotReceive().Commit();
    }

    [Fact]
    public void Recovery_requires_dedicated_session_and_context_invalidation_requires_active_callback()
    {
        var f = new FixtureData();
        using var fixture = f.Create();
        var tenant = fixture.Tenant("A");
        Assert.Throws<Phase7SqlGuardException>(() => tenant.CreateReliabilityScope(pooling: true, closeAfterEnqueue: true));
        var scope = tenant.CreateReliabilityScope();
        Assert.Throws<Phase7SqlGuardException>(() => scope.InvalidateCompany());
        f.WriterA.DidNotReceive().CreateConnection();
    }

    [Fact]
    public async Task Failure_unit_requires_verified_destination_then_dedicated_sql_provider_before_callback()
    {
        var f = new FixtureData();
        using var fixture = f.Create();
        var tenant = fixture.Tenant("A");
        var options = new Phase7SqlFaultSessionOptions(fixture.Manifest.RunId, "P7_12345678_S05", Phase7SqlFailureCase.AuditInsert);
        var unit = tenant.CreateFailureUnitOfWork(options, (_, _, _, _) => Task.CompletedTask);
        var invoked = false;
        var error = await Assert.ThrowsAsync<Phase7SqlGuardException>(() => unit.ExecuteAsync((_, _) => { invoked = true; return Task.FromResult(1); }));
        Assert.Equal("PHASE7_SQL_FAULT_SESSION_CONNECTION", error.ErrorCode); // IDbConnection double is deliberately rejected
        Assert.False(invoked);
        Assert.Single(f.Verifier.Calls);
        f.TransactionA.Received(1).Rollback();
        f.TransactionA.DidNotReceive().Commit();
    }

    [Fact]
    public void Failure_unit_rejects_another_run_before_creating_any_connection()
    {
        var f = new FixtureData();
        using var fixture = f.Create();
        var runId = Guid.NewGuid();
        var code = $"P7_{runId:N}"[..11] + "_S05";
        Assert.Throws<Phase7SqlGuardException>(() => fixture.Tenant("A").CreateFailureUnitOfWork(new(runId, code, Phase7SqlFailureCase.AuditInsert), (_, _, _, _) => Task.CompletedTask));
        f.WriterA.DidNotReceive().CreateConnection();
    }

    [Theory]
    [InlineData("Data Source=tcp:other.test,1433")]
    [InlineData("Initial Catalog=Production")]
    [InlineData("User ID=sa")]
    [InlineData("TrustServerCertificate=true")]
    [InlineData("Encrypt=false")]
    [InlineData("Enlist=true")]
    [InlineData("MultipleActiveResultSets=true")]
    [InlineData("Connect Timeout=121")]
    [InlineData("AttachDbFilename=C:\\data.mdf")]
    public void Resolved_connection_is_checked_without_opening_it(string change)
    {
        var f = new FixtureData();
        using var fixture = f.Create();
        var manifest = fixture.Manifest;
        var target = manifest.TenantA;
        var baseline = $"Data Source={manifest.Server.DataSource};Initial Catalog={target.DatabaseName};User ID=p7_app;Password=offline-test-only;Encrypt=true;TrustServerCertificate=false;Enlist=false;Connect Timeout=15";
        Phase7SqlFixture.ValidateConnectionString(baseline, manifest, target, false);
        var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(baseline);
        var separator = change.IndexOf('=');
        builder[change[..separator]] = change[(separator + 1)..];
        var error = Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlFixture.ValidateConnectionString(builder.ConnectionString, manifest, target, false));
        Assert.DoesNotContain("offline-test-only", error.ToString());
        Assert.Null(error.InnerException);
        f.WriterA.DidNotReceive().CreateConnection();
    }

    [Fact]
    public async Task Observer_cannot_run_inside_the_business_transaction()
    {
        var f = new FixtureData();
        using var fixture = f.Create();
        var tenant = fixture.Tenant("A");
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => tenant.ExecuteAsync((_, _, _) =>
            tenant.ObserveAsync((_, _, _) => Task.FromResult(1))));
        f.ObserverA.DidNotReceive().CreateConnection();
        f.TransactionA.Received(1).Rollback();
    }

    [Fact]
    public async Task Real_unit_of_work_cannot_bypass_destination_verification()
    {
        var f = new FixtureData();
        using var fixture = f.Create();
        var tenant = fixture.Tenant("A");
        Assert.DoesNotContain(tenant.GetType().GetProperties(), p => typeof(NuanSystem.Application.Abstractions.Data.ITransactionRunner).IsAssignableFrom(p.PropertyType));
        Assert.Equal(42, await tenant.CreateUnitOfWork().ExecuteAsync((_, _) => Task.FromResult(42)));
        Assert.Single(f.Verifier.Calls);
        Assert.Equal("A", f.Verifier.Calls[0].Role);
        Assert.False(f.Verifier.Calls[0].Observer);
        f.TransactionA.Received(1).Commit();
    }

    [Fact]
    public async Task Revoking_gate_after_business_callback_causes_rollback()
    {
        var f = new FixtureData();
        var enabled = true;
        using var fixture = f.Create(() => new(enabled, enabled));
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => fixture.Tenant("A").ExecuteAsync((_, _, _) =>
        {
            enabled = false;
            return Task.FromResult(1);
        }));
        f.TransactionA.Received(1).Rollback();
        f.TransactionA.DidNotReceive().Commit();
    }

    [Fact]
    public async Task Expired_approval_is_rejected_before_connection_creation()
    {
        var f = new FixtureData();
        using var fixture = f.Create();
        f.Clock.Advance(TimeSpan.FromHours(1));
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => fixture.Tenant("A").ExecuteAsync((_, _, _) => Task.FromResult(1)));
        f.WriterA.DidNotReceive().CreateConnection();
    }

    [Fact]
    public async Task Caller_cancellation_after_business_callback_causes_rollback()
    {
        var f = new FixtureData();
        using var fixture = f.Create();
        using var cancel = new CancellationTokenSource();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => fixture.Tenant("A").ExecuteAsync((_, _, _) =>
        {
            cancel.Cancel();
            return Task.FromResult(1);
        }, cancel.Token));
        f.TransactionA.Received(1).Rollback();
        f.TransactionA.DidNotReceive().Commit();
    }

    [Fact]
    public void Lookup_selection_requires_one_positive_candidate_instead_of_assuming_seed_ids()
    {
        Assert.Equal(104, Phase7SqlLedger.ResolveLookup([104]));
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlLedger.ResolveLookup([]));
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlLedger.ResolveLookup([1, 2]));
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlLedger.ResolveLookup([0]));
    }

    [Fact]
    public void Disabled_gates_reject_fixture_before_touching_any_connection()
    {
        var f = new FixtureData();
        Assert.Throws<Phase7SqlGuardException>(() => f.Create(() => new(false, false)));
        f.WriterA.DidNotReceive().CreateConnection();
        f.ObserverA.DidNotReceive().CreateConnection();
    }

    [Fact]
    public async Task Revocation_before_execution_does_not_create_a_connection()
    {
        var f = new FixtureData();
        var enabled = true;
        using var fixture = f.Create(() => new(enabled, enabled));
        enabled = false;
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => fixture.Tenant("A").ExecuteAsync((_, _, _) => Task.FromResult(1)));
        f.WriterA.DidNotReceive().CreateConnection();
    }

    [Fact]
    public async Task Preflight_failure_prevents_business_callback_and_rolls_back()
    {
        var f = new FixtureData();
        f.Verifier.Failure = new Phase7SqlGuardException("MISMATCH");
        using var fixture = f.Create();
        var invoked = false;
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => fixture.Tenant("A").ExecuteAsync((_, _, _) => { invoked = true; return Task.FromResult(1); }));
        Assert.False(invoked);
        f.TransactionA.Received(1).Rollback();
        f.TransactionA.DidNotReceive().Commit();
    }

    [Fact]
    public async Task Separate_observer_is_verified_and_counted_after_commit()
    {
        var f = new FixtureData();
        using var fixture = f.Create();
        var tenant = fixture.Tenant("A");
        await tenant.ExecuteAsync((_, _, _) => Task.FromResult(1));
        var snapshot = await tenant.ObserveAsync((connection, timeout, token) =>
        {
            f.TransactionA.Received(1).Commit();
            Assert.Same(f.ObserverConnectionA, connection);
            Assert.Equal(30, timeout);
            token.ThrowIfCancellationRequested();
            return Task.FromResult(42);
        });
        Assert.Equal(42, snapshot);
        Assert.Equal(1, tenant.WriteConnectionCount);
        Assert.Equal(1, tenant.ObserverConnectionCount);
        Assert.Contains(f.Verifier.Calls, x => x.Role == "A" && x.Observer && ReferenceEquals(x.Connection, f.ObserverConnectionA) && x.Transaction is null);
        f.ObserverConnectionA.Received(1).Dispose();
        f.WriterB.DidNotReceive().CreateConnection();
    }

    [Fact]
    public async Task Each_tenant_uses_its_own_factory_and_verification_target()
    {
        var f = new FixtureData();
        using var fixture = f.Create();
        await fixture.Tenant("A").ExecuteAsync((c, _, _) => { Assert.Same(f.ConnectionA, c); return Task.FromResult(1); });
        await fixture.Tenant("B").ExecuteAsync((c, _, _) => { Assert.Same(f.ConnectionB, c); return Task.FromResult(2); });
        f.WriterA.Received(1).CreateConnection();
        f.WriterB.Received(1).CreateConnection();
        Assert.Contains(f.Verifier.Calls, x => x.Role == "B" && !x.Observer && ReferenceEquals(x.Connection, f.ConnectionB));
    }

    [Fact]
    public async Task Disposed_scope_fails_before_creating_a_connection()
    {
        var f = new FixtureData();
        using var fixture = f.Create();
        fixture.Dispose();
        await Assert.ThrowsAsync<ObjectDisposedException>(() => fixture.Tenant("B").ExecuteAsync((_, _, _) => Task.FromResult(1)));
        f.WriterB.DidNotReceive().CreateConnection();
    }

    [Fact]
    public void External_barriers_always_fail_and_real_pilot_can_be_composed_without_external_access()
    {
        var f = new FixtureData();
        using var fixture = f.Create();
        Assert.IsType<NuanSystem.Persistence.Transactions.BusinessPartnerUnitOfWork>(fixture.Tenant("A").CreateUnitOfWork());
        f.WriterA.DidNotReceive().CreateConnection();
        var barrier = new Phase7SqlFixture.ExternalBarrier();
        Assert.Throws<Phase7SqlGuardException>(() => barrier.CreateConnection());
        Assert.Throws<Phase7SqlGuardException>(() => { _ = barrier.ResolveByIdAsync(1); });
        Assert.Throws<Phase7SqlGuardException>(() => { _ = barrier.ResolveByCodeAsync("A"); });
        Assert.Throws<Phase7SqlGuardException>(() => { _ = barrier.ResolveByCodeForUserAsync("A", 1); });
    }

    [Fact]
    public async Task Probe_uses_real_runner_and_same_handles_for_each_checkpoint()
    {
        var (_, connection, transaction, factory) = SqlTransactionRunnerTests.Create();
        var handles = new List<(IDbConnection, IDbTransaction?)>();
        var probe = new Phase7SqlSessionProbe(factory, "LabA", 30, (_, _, _) => { },
            (c, t, _, _) => { handles.Add((c, t)); return Task.FromResult(new Phase7SqlSession(52, "LabA", 1, 1)); });
        Assert.Equal(42, await probe.ExecuteInTenantTransactionAsync(async (_, _, token) =>
        {
            await probe.CheckpointAsync("AfterBusinessPartner", token);
            return 42;
        }));
        Assert.Equal(new[] { "Entry", "AfterBusinessPartner", "BeforeCommit" }, probe.Samples.Select(x => x.Checkpoint));
        Assert.All(handles, x => { Assert.Same(connection, x.Item1); Assert.Same(transaction, x.Item2); });
        Assert.Equal(1, probe.WriteConnectionCount);
        transaction.Received(1).Commit();
        connection.Received(1).Dispose();
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => probe.CheckpointAsync("Late"));
    }

    [Theory]
    [InlineData(53, "LabA", 1, 1)]
    [InlineData(52, "Other", 1, 1)]
    [InlineData(52, "LabA", 0, 0)]
    [InlineData(52, "LabA", 2, 1)]
    [InlineData(52, "LabA", 1, -1)]
    public async Task Session_drift_rolls_back_before_commit(int spid, string database, int count, int state)
    {
        var (_, _, transaction, factory) = SqlTransactionRunnerTests.Create();
        var calls = 0;
        var probe = new Phase7SqlSessionProbe(factory, "LabA", 30, (_, _, _) => { },
            (_, _, _, _) => Task.FromResult(++calls == 1 ? new Phase7SqlSession(52, "LabA", 1, 1) : new(spid, database, count, state)));
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => probe.ExecuteInTenantTransactionAsync((_, _, _) => Task.FromResult(1)));
        transaction.DidNotReceive().Commit();
        transaction.Received(1).Rollback();
    }

    [Fact]
    public async Task Original_failure_is_preserved_and_next_execution_is_allowed()
    {
        var (_, _, transaction, factory) = SqlTransactionRunnerTests.Create();
        var probe = new Phase7SqlSessionProbe(factory, "LabA", 30, (_, _, _) => { },
            (_, _, _, _) => Task.FromResult(new Phase7SqlSession(52, "LabA", 1, 1)));
        var original = new InvalidOperationException("business failure");
        Assert.Same(original, await Assert.ThrowsAsync<InvalidOperationException>(() => probe.ExecuteInTenantTransactionAsync<int>((_, _, _) => throw original)));
        Assert.Equal(7, await probe.ExecuteInTenantTransactionAsync((_, _, _) => Task.FromResult(7)));
        transaction.Received(1).Rollback();
        transaction.Received(1).Commit();
    }

    [Fact]
    public async Task Nested_execution_is_rejected_without_second_connection()
    {
        var (_, _, _, factory) = SqlTransactionRunnerTests.Create();
        var probe = new Phase7SqlSessionProbe(factory, "LabA", 30, (_, _, _) => { },
            (_, _, _, _) => Task.FromResult(new Phase7SqlSession(52, "LabA", 1, 1)));
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => probe.ExecuteInTenantTransactionAsync((_, _, _) =>
            probe.ExecuteInTenantTransactionAsync((_, _, _) => Task.FromResult(1))));
        factory.Received(1).CreateConnection();
    }

    [Fact]
    public async Task Caught_session_failure_still_prevents_commit()
    {
        var (_, _, transaction, factory) = SqlTransactionRunnerTests.Create();
        var calls = 0;
        var probe = new Phase7SqlSessionProbe(factory, "LabA", 30, (_, _, _) => { },
            (_, _, _, _) => Task.FromResult(new Phase7SqlSession(52, "LabA", ++calls == 2 ? 0 : 1, 1)));
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => probe.ExecuteInTenantTransactionAsync(async (_, _, token) =>
        {
            await Assert.ThrowsAsync<Phase7SqlGuardException>(() => probe.CheckpointAsync("Failure", token));
            return 1;
        }));
        transaction.DidNotReceive().Commit();
        transaction.Received(1).Rollback();
    }

    [Fact]
    public async Task Second_write_connection_request_is_rejected_even_if_callback_catches_it()
    {
        var (_, _, transaction, factory) = SqlTransactionRunnerTests.Create();
        var probe = new Phase7SqlSessionProbe(factory, "LabA", 30, (_, _, _) => { },
            (_, _, _, _) => Task.FromResult(new Phase7SqlSession(52, "LabA", 1, 1)));
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => probe.ExecuteInTenantTransactionAsync((_, _, _) =>
        {
            Assert.Throws<Phase7SqlGuardException>(() => probe.ConnectionFactory.CreateConnection());
            return Task.FromResult(1);
        }));
        factory.Received(1).CreateConnection();
        transaction.DidNotReceive().Commit();
    }

    [Fact]
    public void Ledger_rejects_duplicate_cases_and_keeps_identifiers_without_assuming_identity_sequences()
    {
        var ledger = new Phase7SqlLedger(Guid.Parse("12345678-1234-1234-1234-123456789abc"));
        var entry = ledger.Add("S01", "A");
        Assert.Equal("P7_12345678_S01", entry.Code);
        ledger.Record("S01", "A", 104, Guid.NewGuid(), Guid.NewGuid());
        Assert.Equal(104, Assert.Single(ledger.Entries).Id);
        Assert.Throws<Phase7SqlGuardException>(() => ledger.Add("S01", "A"));
        Assert.Throws<Phase7SqlGuardException>(() => ledger.Add("bad;case", "A"));
        Assert.Throws<Phase7SqlGuardException>(() => ledger.Add("S02", "Master"));
        Assert.Equal(entry.Code, ledger.Add("S01", "B").Code);
    }

    private sealed class FixtureData
    {
        public string ManifestText {get;private set;}="";
        public Phase7SqlApproval ApprovalSnapshot {get;private set;}=null!;
        public readonly IDbConnection ConnectionA = Substitute.For<IDbConnection>();
        public readonly IDbConnection ConnectionB = Substitute.For<IDbConnection>();
        public readonly IDbConnection ObserverConnectionA = Substitute.For<IDbConnection>();
        public readonly IDbTransaction TransactionA = Substitute.For<IDbTransaction>();
        public readonly ITenantConnectionFactory WriterA = Substitute.For<ITenantConnectionFactory>();
        public readonly ITenantConnectionFactory WriterB = Substitute.For<ITenantConnectionFactory>();
        public readonly ITenantConnectionFactory ObserverA = Substitute.For<ITenantConnectionFactory>();
        public readonly FakeVerifier Verifier = new();
        private static readonly DateTimeOffset Now = new(2026, 10, 24, 12, 30, 0, TimeSpan.Zero);
        private static readonly Guid RunId = Guid.Parse("12345678-1234-1234-1234-123456789abc");
        public readonly FixedClock Clock = new(Now);
        private static string Database(string role) => $"NuanSystem_Phase7_{RunId:N}_{role}";

        public FixtureData()
        {
            ConnectionA.BeginTransaction().Returns(TransactionA);
            ConnectionB.BeginTransaction().Returns(Substitute.For<IDbTransaction>());
            WriterA.CreateConnection().Returns(ConnectionA);
            WriterB.CreateConnection().Returns(ConnectionB);
            ObserverA.CreateConnection().Returns(ObserverConnectionA);
        }

        public Phase7SqlFixture Create(Func<Phase7SqlGates>? gates = null,TimeSpan? remainingWindow = null)
        {
            Phase7SqlTarget Target(string role, int id) => new(role, Database(role), id, id, RunId,
                "BackendTransactionValidation", 160, "SQL_Latin1_General_CP1_CI_AS", false, new('A', 64),
                new[] { "SP_NA_POST_BUSINESSPARTNERS_CREAR", "SP_NA_PUT_BUSINESSPARTNERS_ACTUALIZAR", "SP_NA_DELETE_BUSINESSPARTNERS_ELIMINAR",
                    "SP_NA_GET_BUSINESSPARTNERS_BUSCARPORID", "SP_NA_GET_BUSINESSPARTNERS_BUSCARPORCODIGO", "SP_NA_GET_BUSINESSPARTNERS_BUSCARPORIDENTIFICACION" }
                    .ToDictionary(x => $"dbo.{x}", _ => new string('B', 64)), "p7_app", "p7_observer", ["SELECT", "EXECUTE", "INSERT"], ["SELECT"],
                $"env:NUANSYSTEM_PHASE7_TENANT_{role}_CONNECTION_STRING", $"env:NUANSYSTEM_PHASE7_OBSERVER_{role}_CONNECTION_STRING");
            var manifest = new Phase7SqlManifest(1, RunId, "test-operator", "AUTH-EXEC", "AUTH-PREP", Now.AddMinutes(-30), Now.Add(remainingWindow??TimeSpan.FromMinutes(30)), 600, 30,
                new("tcp:phase7-lab.example.test,1433", "PHASE7-LAB", "16.0.1000.6"), Target("A", 101), Target("B", 102));
            var json = JsonSerializer.Serialize(manifest, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            var approval = new Phase7SqlApproval("AUTH-EXEC", "AUTH-PREP", "test-operator", RunId, Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json))),
                manifest.Server.DataSource, manifest.Server.ExpectedServerName, new("A", Database("A"), 101, 101), new("B", Database("B"), 102, 102),
                manifest.NotBeforeUtc, manifest.ExpiresAtUtc);
            ManifestText=json;ApprovalSnapshot=approval;
            return new(json, approval, new(WriterA, ObserverA), new(WriterB, Substitute.For<ITenantConnectionFactory>()), Verifier,
                Clock, gates ?? (() => new(true, true)), (c, t, _, _) => Task.FromResult(new Phase7SqlSession(
                    ReferenceEquals(c, ObserverConnectionA) ? 53 : 52, ReferenceEquals(c, ConnectionB) ? Database("B") : Database("A"),
                    t is null ? 0 : 1, t is null ? 0 : 1, ReferenceEquals(c, ConnectionB) ? 102 : 101)));
        }
    }

    private sealed class FixedClock(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
        public override long GetTimestamp()=>now.UtcTicks;
        public override long TimestampFrequency=>TimeSpan.TicksPerSecond;
        public void Advance(TimeSpan elapsed) => now += elapsed;
    }

    private sealed class FakeVerifier : IPhase7SqlDestinationVerifier
    {
        public Exception? Failure;
        public List<(string Role, bool Observer, IDbConnection Connection, IDbTransaction? Transaction)> Calls = [];
        public Task VerifyAsync(Phase7SqlManifest manifest, Phase7SqlTarget target, bool observer, IDbConnection connection, IDbTransaction? transaction, CancellationToken token)
        {
            Calls.Add((target.Role, observer, connection, transaction));
            return Failure is null ? Task.CompletedTask : Task.FromException(Failure);
        }
    }
}
