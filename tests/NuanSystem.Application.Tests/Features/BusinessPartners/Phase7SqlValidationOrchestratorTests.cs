using System.Text;
using System.Text.Json;
using NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

[Trait("Category", "OfflineSafety")]
public sealed class Phase7SqlValidationOrchestratorTests
{
    [Fact]
    public async Task Default_review_never_calls_a_driver_or_creates_output()
    {
        var f = new Fixture();
        var result = await Phase7SqlValidationOrchestrator.ExecuteAsync(f.Plan, f.Approval);
        Assert.Equal("Review", result.State); Assert.False(result.SqlExecuted);
        Assert.False(Directory.Exists(f.Plan.OutputDirectory)); Assert.Empty(f.Driver.Calls);
    }

    [Fact]
    public async Task Full_offline_pipeline_integrates_capture_and_real_offline_packager_without_promotion()
    {
        var f = new Fixture(); var result = await f.Run();
        Assert.Equal(new[] { "ReviewProvisioning", "Provision", "ApproveSuite", "Suite", "Monitor", "Stop" }, f.Driver.Calls);
        Assert.Equal("CompletedWithBlockers", result.State);
        Assert.Equal(Phase7SqlEvidenceOrigin.Offline, result.Origin);
        Assert.False(result.SqlExecuted); Assert.False(result.EvidenceAuthenticityVerified); Assert.False(result.GlobalPhase7Closed);
        Assert.NotNull(result.Package);
        Assert.True(Phase7SqlEvidencePackager.VerifyZip(result.Package.ZipPath, expectedSha256: result.Package.ArchiveSha256).IntegrityVerified);
        Assert.True(File.Exists(Path.Combine(f.Plan.OutputDirectory, "orchestration-result.json")));
    }

    [Theory]
    [InlineData("ReviewProvisioning")][InlineData("Provision")][InlineData("ApproveSuite")][InlineData("Suite")][InlineData("Monitor")]
    public async Task Failure_stops_later_stages_closes_driver_and_redacts_details(string stage)
    {
        var f = new Fixture(); f.Driver.FailStage = stage;
        var error = await Assert.ThrowsAsync<Phase7SqlGuardException>(() => f.Run());
        Assert.DoesNotContain("SENSITIVE", error.ToString()); Assert.Equal("Stop", f.Driver.Calls.Last());
        Assert.Equal(1, f.Driver.Calls.Count(c => c == stage));
        Assert.False(File.Exists(Path.Combine(f.Plan.OutputDirectory, "orchestration-result.json")));
        Assert.False(Directory.Exists(Path.Combine(f.Plan.OutputDirectory, "package")));
    }

    [Theory]
    [InlineData("PlanPin")][InlineData("MissingAuthorization")][InlineData("Expired")][InlineData("SharedAuthorization")]
    [InlineData("BundlePin")][InlineData("Candidate")][InlineData("Budget")]
    public async Task Admission_errors_do_not_touch_driver_or_create_output(string change)
    {
        var f = new Fixture();
        if (change == "PlanPin") f.Approval = f.Approval with { PlanSha256 = new('F', 64) };
        if (change == "MissingAuthorization") f.Approval = f.Approval with { ExecutionAuthorized = false };
        if (change == "SharedAuthorization") f.Approval = f.Approval with { AuthorizationId = f.Plan.ProvisioningAuthorizationId };
        if (change == "Candidate") File.AppendAllText(Path.Combine(f.Sample.Candidate.RootPath, "src/Pilot.cs"), "tamper");
        if (change == "BundlePin") File.AppendAllText(Path.Combine(f.Plan.ProvisioningBundleDirectory, "provisioning-spec.json"), "tamper");
        if (change == "Expired") f.Plan = f.Plan with { ExpiresAtUtc = f.Now.AddMinutes(-1) };
        if (change == "Budget") f.Plan = f.Plan with { StageTimeoutSeconds = 0 };
        if (change is "Expired" or "Budget") f.Reapprove();
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => f.Run());
        Assert.Empty(f.Driver.Calls); Assert.False(Directory.Exists(f.Plan.OutputDirectory));
    }

    [Theory]
    [InlineData("Incomplete")][InlineData("WrongRun")][InlineData("WrongOrigin")]
    public async Task Invalid_provisioning_journal_never_reaches_suite(string change)
    {
        var f = new Fixture(); f.Driver.BadJournal = change;
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => f.Run());
        Assert.DoesNotContain("ApproveSuite", f.Driver.Calls); Assert.Equal("Stop", f.Driver.Calls.Last());
    }

    [Theory]
    [InlineData("Suite")][InlineData("Monitor")]
    public async Task Missing_stage_approval_returns_pending_without_approving_on_behalf_of_operator(string stage)
    {
        var f = new Fixture(); f.Driver.PendingStage = stage;
        var result = await f.Run();
        Assert.Equal("Awaiting" + stage + "Approval", result.State); Assert.Null(result.Package);
        Assert.Equal("Stop", f.Driver.Calls.Last());
        if (stage == "Suite") Assert.DoesNotContain("Suite", f.Driver.Calls);
    }

    [Fact]
    public async Task Cancellation_preserves_primary_even_when_cleanup_fails()
    {
        var f = new Fixture(); using var cts = new CancellationTokenSource();
        f.Driver.Cancel = cts; f.Driver.FailCleanup = true;
        var error = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => f.Run(cts.Token));
        Assert.Equal(true, error.Data["OrchestrationCleanupFailed"]);
        Assert.DoesNotContain("Suite", f.Driver.Calls); Assert.Equal("Stop", f.Driver.Calls.Last());
    }

    [Fact]
    public async Task Stage_timeout_stops_cooperation_and_never_starts_a_later_stage()
    {
        var f = new Fixture(); f.Plan = f.Plan with { StageTimeoutSeconds = 1 }; f.Reapprove(); f.Driver.DelayReview = true;
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => f.Run());
        Assert.Equal(new[] { "ReviewProvisioning", "Stop" }, f.Driver.Calls);
    }

    [Fact]
    public async Task Existing_output_is_preserved_and_replay_is_rejected()
    {
        var f = new Fixture(); Directory.CreateDirectory(f.Plan.OutputDirectory);
        File.WriteAllText(Path.Combine(f.Plan.OutputDirectory, "keep.txt"), "preserved");
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => f.Run()); Assert.Empty(f.Driver.Calls);
        Assert.Equal("preserved", File.ReadAllText(Path.Combine(f.Plan.OutputDirectory, "keep.txt")));
    }

    [Theory]
    [InlineData("AwaitingSuiteApproval")][InlineData("AwaitingMonitorApproval")][InlineData("CompletedWithBlockers")]
    public void Pending_execution_reports_prior_sql_provisioning_but_simulation_never_does(string state)
    {
        Assert.True(new Phase7SqlValidationResult(state, "Execution", Phase7SqlEvidenceOrigin.SqlServerLaboratory, null).SqlExecuted);
        Assert.False(new Phase7SqlValidationResult(state, "Simulation", Phase7SqlEvidenceOrigin.Offline, null).SqlExecuted);
    }

    [Fact]
    public async Task Native_bridge_rejects_missing_original_child_without_calling_monitor()
    {
        var f = new Fixture(); var context = new Phase7SqlValidationContext(f.Plan, f.Approval);
        var binding = new Phase7SqlValidationHostBindings(f.Driver.ReviewProvisioningAsync, f.Driver.ProvisionAsync,
            f.Driver.ApproveSuiteAsync, f.Driver.RunSuiteAsync, (_, _, _, _) => Task.FromResult<Phase7SqlApprovedMonitorResources?>(null), f.Driver.StopAsync);
        var suite = (await f.Driver.ApproveSuiteAsync(context, default))!;
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => new Phase7SqlNativeValidationDriver(binding).RunSuiteAsync(context, suite, default));
        Assert.DoesNotContain("Monitor", f.Driver.Calls);
        Assert.Throws<Phase7SqlGuardException>(() => new Phase7SqlNativeValidationDriver(binding with { Stop = null! }));
    }

    [Fact]
    public async Task Archived_plan_preserves_the_external_pin_even_with_a_trailing_separator()
    {
        var f = new Fixture(); f.Plan = f.Plan with { OutputDirectory = f.Plan.OutputDirectory + Path.DirectorySeparatorChar }; f.Reapprove();
        f.Driver.PendingStage = "Suite"; await f.Run();
        var bytes = File.ReadAllBytes(Path.Combine(f.Plan.OutputDirectory, "plan.json"));
        Assert.Equal(f.Approval.PlanSha256, Phase7SqlEvidenceFiles.Hash(bytes));
    }

    [Theory]
    [InlineData("Server")][InlineData("Company")]
    public async Task Independently_approved_suite_still_cannot_target_a_different_provisioned_lab(string variant)
    {
        var f = new Fixture(); f.Driver.SuiteVariant = variant;
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => f.Run());
        Assert.DoesNotContain("Suite", f.Driver.Calls); Assert.Equal("Stop", f.Driver.Calls.Last());
    }

    [Fact]
    public async Task Nonzero_suite_exit_with_complete_evidence_is_monitored_and_packaged_with_blockers()
    {
        var f = new Fixture(); f.Driver.ExitCode = 1;
        var result = await f.Run();
        Assert.Contains("Monitor", f.Driver.Calls); Assert.NotNull(result.Package);
        Assert.Equal("CompletedWithBlockers", result.State);
        Assert.Equal(Phase7SqlEvidenceStatus.Failed, result.Package.Report.RunChecks["NoOrphanSessions"]);
    }

    [Fact]
    public async Task Cleanup_only_failure_is_persisted_before_it_is_reported()
    {
        var f = new Fixture(); f.Driver.PendingStage = "Suite"; f.Driver.FailCleanup = true;
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => f.Run());
        var failure = System.Text.Json.Nodes.JsonNode.Parse(File.ReadAllBytes(Path.Combine(f.Plan.OutputDirectory, "failure.json")))!;
        Assert.Equal("CleanupFailed", failure["Code"]!.GetValue<string>());
        Assert.True(failure["CleanupFailed"]!.GetValue<bool>());
        Assert.False(File.Exists(Path.Combine(f.Plan.OutputDirectory, "orchestration-result.json")));
    }

    [Fact]
    public async Task Cleanup_deadline_is_propagated_to_cooperative_host()
    {
        var f = new Fixture(); f.Driver.PendingStage = "Suite"; f.Driver.DelayStop = true;
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => f.Run());
        await f.Driver.StopCancellationObserved.Task.WaitAsync(TimeSpan.FromSeconds(2));
        Assert.DoesNotContain("Suite", f.Driver.Calls);
        Assert.True(File.Exists(Path.Combine(f.Plan.OutputDirectory, "failure.json")));
    }

    private sealed class Fixture
    {
        public string Root { get; } = Phase7SqlEvidenceFilesTests.Root();
        public Phase7SqlSyntheticPackage Sample { get; }
        public DateTimeOffset Now { get; set; }
        public Phase7SqlValidationPlan Plan { get; set; }
        public Phase7SqlValidationApproval Approval { get; set; } = null!;
        public Driver Driver { get; }
        public Fixture()
        {
            Sample = new(Path.Combine(Root, "sample")); Now = Sample.Started.AddMinutes(-8);
            var bundle = Path.Combine(Root, "bundle"); Directory.CreateDirectory(bundle);
            var payload = Phase7SqlEvidenceFiles.Encode(new { Sample.Manifest.RunId, PreparationAuthorizationId = "PREP-AUTH",
                ExpectedServerName = Sample.Manifest.Server.ExpectedServerName, Sample.Manifest.Server.ProductVersion,
                CompanyIdA = Sample.Manifest.TenantA.CompanyId, CompanyIdB = Sample.Manifest.TenantB.CompanyId,
                Sample.Manifest.TenantA.CompatibilityLevel, Sample.Manifest.TenantA.Collation, Sample.Manifest.TenantA.ReadCommittedSnapshot });
            File.WriteAllBytes(Path.Combine(bundle, "provisioning-spec.json"), payload);
            var manifest = Phase7SqlEvidenceFiles.Encode(new { Sample.Manifest.RunId, PreparationAuthorizationId = "PREP-AUTH", ExecutionAuthorized = false,
                SpecSha256 = Phase7SqlEvidenceFiles.Hash(payload), SqlExecuted = false, SapExecuted = false,
                Files = new[] { new { Path = "provisioning-spec.json", Bytes = payload.Length, Sha256 = Phase7SqlEvidenceFiles.Hash(payload) } } });
            File.WriteAllBytes(Path.Combine(bundle, "bundle-manifest.json"), manifest);
            Plan = new(Sample.Manifest.RunId, Path.Combine(Root, "orchestration"), bundle, Phase7SqlEvidenceFiles.Hash(manifest),
                "PREP-AUTH", "PROVISION-EXEC", Sample.Candidate, Sample.Started.AddMinutes(-30), Sample.Started.AddHours(1), 3600, 120);
            Reapprove(); Driver = new(this);
        }
        public void Reapprove() => Approval = new(Plan.RunId, Phase7SqlValidationOrchestrator.Fingerprint(Plan), "ORCHESTRATE-AUTH", true);
        public Task<Phase7SqlValidationResult> Run(CancellationToken token = default) => Phase7SqlValidationOrchestrator.SimulateAsync(Plan, Approval, Driver, new Clock(this), token);
        private sealed class Clock(Fixture f) : TimeProvider { public override DateTimeOffset GetUtcNow() => f.Now; }
    }

    private sealed class Driver(Fixture f) : IPhase7SqlValidationDriver
    {
        public List<string> Calls { get; } = [];
        public string? FailStage { get; set; }
        public string? PendingStage { get; set; }
        public string? BadJournal { get; set; }
        public bool FailCleanup { get; set; }
        public bool DelayReview { get; set; }
        public string? SuiteVariant { get; set; }
        public int ExitCode { get; set; }
        public bool DelayStop { get; set; }
        public TaskCompletionSource StopCancellationObserved { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public CancellationTokenSource? Cancel { get; set; }
        private void Call(string stage) { Calls.Add(stage); if (FailStage == stage) throw new IOException("SENSITIVE provider information"); }
        public async Task<Phase7SqlProvisioningReview> ReviewProvisioningAsync(Phase7SqlValidationContext context, CancellationToken token)
        { Call("ReviewProvisioning"); if (DelayReview) await Task.Delay(5000, token); return new(f.Plan.RunId, f.Plan.ProvisioningBundleSha256, 9, false); }
        public Task<Phase7SqlProvisioningCompletion> ProvisionAsync(Phase7SqlValidationContext context, CancellationToken token)
        {
            Call("Provision"); var rows = new List<object>();
            void Row(string status, string stage, int batch) => rows.Add(new { RunId = BadJournal == "WrongRun" ? Guid.NewGuid() : f.Plan.RunId,
                ManifestSha256 = f.Plan.ProvisioningBundleSha256, AuthorizationId = f.Plan.ProvisioningAuthorizationId,
                Origin = BadJournal == "WrongOrigin" ? "SqlServerProvisioning" : "OfflineSimulation", TimestampUtc = f.Now,
                Status = status, Stage = stage, Batch = batch, Sha256 = batch > 0 ? new string('A', 64) : "" });
            Row("Admitted", "Admission", 0);
            foreach (var stage in Phase7SqlValidationOrchestrator.ProvisioningStages) { Row("Started", stage, 1); Row("Completed", stage, 1); }
            if (BadJournal != "Incomplete") Row("Finished", "Session", 0);
            var bytes = Encoding.UTF8.GetBytes(string.Join("\n", rows.Select(r => JsonSerializer.Serialize(r, Phase7SqlEvidenceFiles.Json))) + "\n");
            File.WriteAllBytes(context.ProvisioningJournalPath, bytes); Cancel?.Cancel();
            return Task.FromResult(new Phase7SqlProvisioningCompletion(f.Plan.RunId, Phase7SqlEvidenceFiles.Hash(bytes), false));
        }
        public Task<Phase7SqlApprovedSuite?> ApproveSuiteAsync(Phase7SqlValidationContext context, CancellationToken token)
        {
            Call("ApproveSuite");
            var manifest = f.Sample.Manifest; var approval = f.Sample.Approval;
            if (SuiteVariant == "Server") { manifest = manifest with { Server = manifest.Server with { ExpectedServerName = "OTHER" } }; approval = approval with { ExpectedServerName = "OTHER" }; }
            if (SuiteVariant == "Company") { manifest = manifest with { TenantA = manifest.TenantA with { CompanyId = 999 } }; approval = approval with { TenantA = approval.TenantA with { CompanyId = 999 } }; }
            var json = JsonSerializer.Serialize(manifest, Phase7SqlValidationOrchestrator.Web);
            var hash = Phase7SqlEvidenceFiles.Hash(Encoding.UTF8.GetBytes(json)); approval = approval with { ManifestSha256 = hash };
            return Task.FromResult(PendingStage == "Suite" ? null : new Phase7SqlApprovedSuite(json, approval,
                f.Sample.Inputs with { ManifestSha256 = hash }, File.ReadAllBytes(Path.Combine(f.Sample.Root, "offline-baseline.trx"))));
        }
        public Task<Phase7SqlSuiteCompletion> RunSuiteAsync(Phase7SqlValidationContext context, Phase7SqlApprovedSuite suite, CancellationToken token)
        {
            Call("Suite");
            foreach (var file in Directory.EnumerateFiles(f.Sample.Root, "*", SearchOption.AllDirectories))
                Phase7SqlEvidenceFiles.Write(context.SuiteRoot, Path.GetRelativePath(f.Sample.Root, file).Replace('\\', '/'), File.ReadAllBytes(file));
            f.Now = f.Sample.Finished.AddSeconds(3);
            return Task.FromResult(new Phase7SqlSuiteCompletion(f.Sample.Start.ProcessId, ExitCode));
        }
        public async Task<Phase7SqlMonitoredRun?> MonitorAsync(Phase7SqlValidationContext context, Phase7SqlApprovedSuite suite, Phase7SqlSuiteCompletion completion, CancellationToken token)
        {
            Call("Monitor"); if (PendingStage == "Monitor") return null;
            var inputs = new Phase7SqlMonitorInputs(context.SuiteRoot, context.MonitorDirectory,
                Phase7SqlMonitorExecutor.InputPaths.ToDictionary(p => p, p => Phase7SqlEvidenceFiles.Hash(File.ReadAllBytes(Path.Combine(context.SuiteRoot, p)))));
            var approval = new Phase7SqlControlledMonitorApproval(f.Sample.MonitorApproval, Phase7SqlMonitorExecutor.InputFingerprint(inputs), 30, 60, true);
            var isolation = new Phase7SqlMonitorIsolation(f.Sample.Witness.Inspector, f.Sample.Witness.AuthorizationReference,
                f.Sample.Witness.WorkersOffFromUtc, f.Sample.Witness.WorkersOffThroughUtc, f.Sample.Witness.DisabledComponents);
            var result = await Phase7SqlMonitorExecutor.SimulateAsync(inputs, f.Sample.Approval, approval, isolation, new MonitorProbe(f, ExitCode), new MonitorClock(f), token);
            return new(result.IndexSha256!, approval);
        }
        public async Task StopAsync(CancellationToken token)
        {
            Calls.Add("Stop"); if (FailCleanup) throw new IOException("SENSITIVE cleanup");
            if (DelayStop) try { await Task.Delay(Timeout.InfiniteTimeSpan, token); }
                finally { if (token.IsCancellationRequested) StopCancellationObserved.TrySetResult(); }
        }
        private sealed class MonitorClock(Fixture fixture) : TimeProvider { public override DateTimeOffset GetUtcNow() => fixture.Now; }
        private sealed class MonitorProbe(Fixture fixture, int exitCode) : IPhase7SqlMonitorProbe
        {
            public Task<Phase7SqlMonitorProcessCapture> WitnessAsync(Phase7SqlMonitorPlan plan, Phase7SqlMonitorIsolation isolation, CancellationToken token) => Task.FromResult(new Phase7SqlMonitorProcessCapture(fixture.Sample.Witness with { ExitCode = exitCode }, fixture.Sample.Start.StartedAtUtc.AddSeconds(-1)));
            public Task<Phase7SqlMonitorObservation> ReadAsync(Phase7SqlMonitorPlan plan, Phase7SqlMonitorApproval approval, DateTimeOffset now, int timeout, CancellationToken token) => Task.FromResult(fixture.Sample.Monitor);
            public Task CloseAsync() => Task.CompletedTask;
        }
    }
}
