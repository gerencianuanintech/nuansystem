using Microsoft.Data.SqlClient;
using NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

[Trait("Category", "OfflineSafety")]
public sealed class Phase7SqlMonitorExecutorTests
{
    [Fact]
    public async Task Default_mode_reviews_inputs_without_connection_or_output()
    {
        var f = new Fixture();
        var result = await Phase7SqlMonitorExecutor.ExecuteAsync(f.Inputs, f.Sample.Approval, f.Approval, f.Isolation);
        Assert.Equal("Review", result.Mode);
        Assert.False(result.SqlExecuted);
        Assert.False(result.CaptureCompleted);
        Assert.False(Directory.Exists(f.Inputs.OutputDirectory));
    }

    [Fact]
    public async Task Simulation_preserves_protocol_but_cannot_become_real_sql_evidence()
    {
        var f = new Fixture(); var probe = new Probe(f);
        var result = await f.Run(probe);
        Assert.Equal(new[] { "Witness", "Read", "Close" }, probe.Calls);
        Assert.True(result.CaptureCompleted);
        Assert.True(result.ObservationsConform);
        Assert.Equal(Phase7SqlEvidenceOrigin.Offline, result.Origin);
        Assert.Equal(Phase7SqlEvidenceOrigin.Offline, result.Witness!.Origin);
        Assert.False(result.SqlExecuted);
        Assert.False(result.EvidenceAuthenticityVerified);
        Assert.False(result.GlobalPhase7Closed);
        var index = Phase7SqlEvidenceFiles.Decode<Phase7SqlMonitorCaptureIndex>(File.ReadAllBytes(Path.Combine(f.Inputs.OutputDirectory, "capture-index.json")));
        foreach (var file in index.Files)
            Assert.Equal(file.Sha256, Phase7SqlEvidenceFiles.Hash(File.ReadAllBytes(Path.Combine(f.Inputs.OutputDirectory, file.RelativePath))));
        Assert.DoesNotContain("Snapshot", File.ReadAllText(Path.Combine(f.Inputs.OutputDirectory, "observation.json")).Replace("SnapshotSha256", ""));
    }

    [Theory]
    [InlineData("manifest.json")][InlineData("global/start.json")][InlineData("global/end.json")]
    [InlineData("sql.trx")][InlineData("global/final-A.json")][InlineData("global/final-B.json")]
    public async Task Every_input_is_pinned_before_any_probe(string path)
    {
        var f = new Fixture(); var probe = new Probe(f);
        File.AppendAllText(Path.Combine(f.Sample.Root, path), " ");
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => f.Run(probe));
        Assert.Empty(probe.Calls);
        Assert.False(Directory.Exists(f.Inputs.OutputDirectory));
    }

    [Theory]
    [InlineData("Pin")][InlineData("Run")][InlineData("Principal")][InlineData("SharedAuthorization")]
    [InlineData("Expired")][InlineData("CommandBudget")][InlineData("TotalBudget")][InlineData("MissingAuthorization")]
    public async Task Invalid_monitor_approval_never_contacts_probe(string change)
    {
        var f = new Fixture(); var probe = new Probe(f);
        f.Approval = change switch
        {
            "Pin" => f.Approval with { InputsSha256 = new('F', 64) },
            "Run" => f.Approval with { Monitor = f.Approval.Monitor with { RunId = Guid.NewGuid() } },
            "Principal" => f.Approval with { Monitor = f.Approval.Monitor with { Principal = f.Sample.Manifest.TenantA.ApplicationPrincipal } },
            "SharedAuthorization" => f.Approval with { Monitor = f.Approval.Monitor with { AuthorizationReference = f.Sample.Approval.AuthorizationId } },
            "Expired" => f.Approval with { Monitor = f.Approval.Monitor with { ExpiresAtUtc = f.Now.AddSeconds(-1) } },
            "CommandBudget" => f.Approval with { CommandTimeoutSeconds = 0 },
            "TotalBudget" => f.Approval with { TotalTimeoutSeconds = 3601 },
            _ => f.Approval with { ExecutionAuthorized = false }
        };
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => f.Run(probe));
        Assert.Empty(probe.Calls);
    }

    [Theory]
    [InlineData("Running")][InlineData("Pid")][InlineData("ReusedPid")][InlineData("WrongRun")]
    public async Task Wrong_process_is_rejected_before_sql_read_and_session_is_closed(string change)
    {
        var f = new Fixture(); var probe = new Probe(f);
        probe.Process = change switch
        {
            "Running" => probe.Process with { Witness = probe.Process.Witness with { HasExited = false } },
            "Pid" => probe.Process with { Witness = probe.Process.Witness with { ProcessId = 999 } },
            "ReusedPid" => probe.Process with { ProcessStartedAtUtc = f.Sample.Start.StartedAtUtc.AddMinutes(1) },
            _ => probe.Process with { Witness = probe.Process.Witness with { RunId = Guid.NewGuid() } }
        };
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => f.Run(probe));
        Assert.Equal(new[] { "Witness", "Close" }, probe.Calls);
        Assert.False(File.Exists(Path.Combine(f.Inputs.OutputDirectory, "capture-index.json")));
    }

    [Theory]
    [InlineData("Sessions")][InlineData("TransactionsA")][InlineData("TransactionsB")][InlineData("Retention")][InlineData("NonzeroExit")]
    public async Task Negative_observations_are_retained_without_accepting_postrun_checks(string change)
    {
        var f = new Fixture(); var probe = new Probe(f);
        if (change == "NonzeroExit") probe.Process = probe.Process with { Witness = probe.Process.Witness with { ExitCode = 1 } };
        else probe.Observation = change switch
        {
            "Sessions" => probe.Observation with { RemainingSessions = 1 },
            "TransactionsA" => probe.Observation with { TransactionsA = 1 },
            "TransactionsB" => probe.Observation with { TransactionsB = 1 },
            _ => probe.Observation with { Databases = probe.Observation.Databases.Select(d => d with { SnapshotSha256 = new('E', 64) }).ToArray() }
        };
        var result = await f.Run(probe);
        Assert.True(result.CaptureCompleted);
        Assert.False(result.ObservationsConform);
        Assert.True(File.Exists(Path.Combine(f.Inputs.OutputDirectory, "capture-index.json")));
    }

    [Theory]
    [InlineData("Identity")][InlineData("Read")][InlineData("Close")][InlineData("Expired")]
    public async Task Failed_or_expired_capture_never_gets_a_completion_index(string change)
    {
        var f = new Fixture(); var probe = new Probe(f);
        if (change == "Identity") probe.Observation = probe.Observation with { Principal = "wrong" };
        if (change == "Read") probe.FailRead = true;
        if (change == "Close") probe.FailClose = true;
        if (change == "Expired") probe.AfterRead = () => f.Now = f.Approval.Monitor.ExpiresAtUtc;
        var error = await Assert.ThrowsAsync<Phase7SqlGuardException>(() => f.Run(probe));
        Assert.DoesNotContain("SENSITIVE", error.ToString());
        Assert.Equal(1, probe.Calls.Count(c => c == "Close"));
        Assert.False(File.Exists(Path.Combine(f.Inputs.OutputDirectory, "capture-index.json")));
    }

    [Fact]
    public async Task Cancellation_after_read_closes_session_and_does_not_publish_capture()
    {
        var f = new Fixture(); var probe = new Probe(f); using var cancellation = new CancellationTokenSource();
        probe.AfterRead = cancellation.Cancel;
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => f.Run(probe, cancellation.Token));
        Assert.Equal("Close", probe.Calls.Last());
        Assert.False(File.Exists(Path.Combine(f.Inputs.OutputDirectory, "capture-index.json")));
    }

    [Fact]
    public async Task Cancellation_remains_primary_when_session_close_also_fails()
    {
        var f = new Fixture(); var probe = new Probe(f) { FailClose = true };
        using var cancellation = new CancellationTokenSource(); probe.AfterRead = cancellation.Cancel;
        var error = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => f.Run(probe, cancellation.Token));
        Assert.Equal(true, error.Data["MonitorCleanupFailed"]);
        Assert.False(File.Exists(Path.Combine(f.Inputs.OutputDirectory, "capture-index.json")));
    }

    [Fact]
    public async Task Total_budget_cancels_inflight_read_and_closes_without_publishing()
    {
        var f = new Fixture(); var probe = new Probe(f) { DelayRead = true };
        f.Approval = f.Approval with { TotalTimeoutSeconds = 1, CommandTimeoutSeconds = 1 };
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => f.Run(probe));
        Assert.Equal("Close", probe.Calls.Last());
        Assert.False(File.Exists(Path.Combine(f.Inputs.OutputDirectory, "capture-index.json")));
    }

    [Fact]
    public async Task Pending_close_has_an_independent_deadline_and_cannot_publish_success()
    {
        var f = new Fixture(); var probe = new Probe(f) { HangClose = true };
        var error = await Assert.ThrowsAsync<Phase7SqlGuardException>(() => f.Run(probe).WaitAsync(TimeSpan.FromSeconds(8)));
        Assert.Contains("CLOSE", error.ErrorCode);
        Assert.False(File.Exists(Path.Combine(f.Inputs.OutputDirectory, "capture-index.json")));
    }

    [Fact]
    public async Task Synchronous_close_prefix_is_also_bounded()
    {
        var f = new Fixture(); var probe = new Probe(f) { BlockClose = true };
        var error = await Assert.ThrowsAsync<Phase7SqlGuardException>(() => f.Run(probe).WaitAsync(TimeSpan.FromSeconds(8)));
        Assert.Contains("CLOSE", error.ErrorCode);
        Assert.False(File.Exists(Path.Combine(f.Inputs.OutputDirectory, "capture-index.json")));
    }

    [Fact]
    public async Task Explicit_live_mode_rejects_closed_connection_without_opening_it()
    {
        var f = new Fixture(); var now = DateTimeOffset.UtcNow;
        f.Approval = f.Approval with { Monitor = f.Approval.Monitor with { NotBeforeUtc = now.AddMinutes(-1), ExpiresAtUtc = now.AddMinutes(2) } };
        var isolation = f.Isolation with { WorkersOffThroughUtc = now.AddMinutes(2) };
        using var connection = new SqlConnection(); using var process = System.Diagnostics.Process.GetCurrentProcess();
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => Phase7SqlMonitorExecutor.ExecuteAsync(f.Inputs, f.Sample.Approval,
            f.Approval, isolation, connection, process, execute: true));
        Assert.Equal(System.Data.ConnectionState.Closed, connection.State);
        Assert.False(Directory.Exists(f.Inputs.OutputDirectory));
    }

    [Fact]
    public async Task Existing_output_and_precancelled_requests_do_not_contact_probe()
    {
        var f = new Fixture(); var probe = new Probe(f);
        Directory.CreateDirectory(f.Inputs.OutputDirectory); File.WriteAllText(Path.Combine(f.Inputs.OutputDirectory, "keep.txt"), "preserved");
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => f.Run(probe));
        Assert.Empty(probe.Calls);
        Assert.Equal("preserved", File.ReadAllText(Path.Combine(f.Inputs.OutputDirectory, "keep.txt")));
        using var cancelled = new CancellationTokenSource(); cancelled.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => f.Run(probe, cancelled.Token));
        Assert.Empty(probe.Calls);
    }

    [Fact]
    public async Task Disk_mutation_after_preparation_cannot_replace_verified_final_snapshot()
    {
        var f = new Fixture(); var probe = new Probe(f);
        probe.AfterRead = () => File.WriteAllText(Path.Combine(f.Sample.Root, "global/final-A.json"), "{}");
        var result = await f.Run(probe);
        Assert.True(result.ObservationsConform);
    }

    private sealed class Fixture
    {
        public Phase7SqlSyntheticPackage Sample { get; } = new(Path.Combine(Phase7SqlEvidenceFilesTests.Root(), "monitor"));
        public DateTimeOffset Now { get; set; }
        public Phase7SqlMonitorInputs Inputs { get; }
        public Phase7SqlControlledMonitorApproval Approval { get; set; }
        public Phase7SqlMonitorIsolation Isolation { get; }
        public Fixture()
        {
            Now = Sample.Finished.AddSeconds(3);
            Inputs = new(Sample.Root, Path.Combine(Path.GetDirectoryName(Sample.Root)!, "capture"),
                Phase7SqlMonitorExecutor.InputPaths.ToDictionary(p => p, p => Phase7SqlEvidenceFiles.Hash(File.ReadAllBytes(Path.Combine(Sample.Root, p)))));
            Approval = new(Sample.MonitorApproval, Phase7SqlMonitorExecutor.InputFingerprint(Inputs), 30, 60, true);
            Isolation = new(Sample.Witness.Inspector, Sample.Witness.AuthorizationReference, Sample.Witness.WorkersOffFromUtc,
                Sample.Witness.WorkersOffThroughUtc, Sample.Witness.DisabledComponents);
        }
        public Task<Phase7SqlMonitorExecutionResult> Run(Probe probe, CancellationToken token = default) =>
            Phase7SqlMonitorExecutor.SimulateAsync(Inputs, Sample.Approval, Approval, Isolation, probe, new Clock(this), token);
        private sealed class Clock(Fixture f) : TimeProvider { public override DateTimeOffset GetUtcNow() => f.Now; }
    }

    private sealed class Probe : IPhase7SqlMonitorProbe
    {
        public List<string> Calls { get; } = [];
        public Phase7SqlMonitorProcessCapture Process { get; set; }
        public Phase7SqlMonitorObservation Observation { get; set; }
        public bool FailRead { get; set; }
        public bool FailClose { get; set; }
        public bool DelayRead { get; set; }
        public bool HangClose { get; set; }
        public bool BlockClose { get; set; }
        public Action? AfterRead { get; set; }
        public Probe(Fixture f) { Process = new(f.Sample.Witness, f.Sample.Start.StartedAtUtc.AddSeconds(-1)); Observation = f.Sample.Monitor; }
        public Task<Phase7SqlMonitorProcessCapture> WitnessAsync(Phase7SqlMonitorPlan plan, Phase7SqlMonitorIsolation isolation, CancellationToken token)
        { Calls.Add("Witness"); return Task.FromResult(Process); }
        public async Task<Phase7SqlMonitorObservation> ReadAsync(Phase7SqlMonitorPlan plan, Phase7SqlMonitorApproval approval, DateTimeOffset now, int timeout, CancellationToken token)
        { Calls.Add("Read"); if (FailRead) throw new IOException("SENSITIVE provider details"); if (DelayRead) await Task.Delay(TimeSpan.FromSeconds(5), token); AfterRead?.Invoke(); return Observation; }
        public Task CloseAsync()
        {
            Calls.Add("Close"); if (FailClose) throw new IOException("SENSITIVE cleanup details");
            if (BlockClose) Thread.Sleep(TimeSpan.FromSeconds(6));
            return HangClose ? new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously).Task : Task.CompletedTask;
        }
    }
}
