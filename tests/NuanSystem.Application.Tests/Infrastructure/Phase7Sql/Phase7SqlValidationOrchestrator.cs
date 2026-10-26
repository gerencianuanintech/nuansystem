using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal sealed record Phase7SqlValidationPlan(Guid RunId, string OutputDirectory, string ProvisioningBundleDirectory,
    string ProvisioningBundleSha256, string PreparationAuthorizationId, string ProvisioningAuthorizationId,
    Phase7SqlCandidate Candidate, DateTimeOffset NotBeforeUtc, DateTimeOffset ExpiresAtUtc, int TotalTimeoutSeconds, int StageTimeoutSeconds);
internal sealed record Phase7SqlValidationApproval(Guid RunId, string PlanSha256, string AuthorizationId, bool ExecutionAuthorized);
internal sealed record Phase7SqlProvisioningReview(Guid RunId, string ManifestSha256, int BatchCount, bool SqlExecuted);
internal sealed record Phase7SqlProvisioningCompletion(Guid RunId, string JournalSha256, bool SqlExecuted);
internal sealed record Phase7SqlApprovedSuite(string ManifestJson, Phase7SqlApproval Approval, Phase7SqlSuiteInputs Inputs, byte[] OfflineBaseline);
internal sealed record Phase7SqlSuiteCompletion(int ProcessId, int ExitCode, [property: System.Text.Json.Serialization.JsonIgnore] Process? Child = null, int? RunnerExitCode = null);
internal sealed record Phase7SqlMonitoredRun(string IndexSha256, Phase7SqlControlledMonitorApproval Approval);
internal sealed record Phase7SqlProvisioningTarget(string ServerName, string ProductVersion, int CompanyA, int CompanyB,
    int Compatibility, string Collation, bool ReadCommittedSnapshot);
internal sealed record Phase7SqlValidationResult(string State, string Mode, Phase7SqlEvidenceOrigin Origin, Phase7SqlPackageResult? Package)
{
    // Every returned execution state has already completed and verified provisioning, including pending approval states.
    public bool SqlExecuted => Mode == "Execution" && State != "Review";
    public bool EvidenceAuthenticityVerified => false;
    public bool GlobalPhase7Closed => false;
}
internal sealed class Phase7SqlValidationContext(Phase7SqlValidationPlan plan, Phase7SqlValidationApproval approval, Phase7SqlProvisioningTarget? target = null)
{
    public Phase7SqlValidationPlan Plan { get; } = plan;
    public Phase7SqlValidationApproval Approval { get; } = approval;
    public Phase7SqlProvisioningTarget? ProvisioningTarget { get; } = target;
    public string ProvisioningJournalPath => Path.Combine(Plan.OutputDirectory, "provisioning.jsonl");
    public string SuiteWorkingDirectory => Path.Combine(Plan.OutputDirectory, "suite-work");
    public string SuiteRoot => Path.Combine(SuiteWorkingDirectory, "artifacts", "phase7-sql-executions", Plan.RunId.ToString("N"));
    public string MonitorDirectory => Path.Combine(Plan.OutputDirectory, "monitor-capture");
    public string PackageDirectory => Path.Combine(Plan.OutputDirectory, "package");
    public string PreparedDirectory => Path.Combine(Plan.OutputDirectory, "approved-suite");
}
internal interface IPhase7SqlValidationDriver
{
    Task<Phase7SqlProvisioningReview> ReviewProvisioningAsync(Phase7SqlValidationContext context, CancellationToken token);
    Task<Phase7SqlProvisioningCompletion> ProvisionAsync(Phase7SqlValidationContext context, CancellationToken token);
    Task<Phase7SqlApprovedSuite?> ApproveSuiteAsync(Phase7SqlValidationContext context, CancellationToken token);
    Task<Phase7SqlSuiteCompletion> RunSuiteAsync(Phase7SqlValidationContext context, Phase7SqlApprovedSuite suite, CancellationToken token);
    Task<Phase7SqlMonitoredRun?> MonitorAsync(Phase7SqlValidationContext context, Phase7SqlApprovedSuite suite, Phase7SqlSuiteCompletion completion, CancellationToken token);
    Task StopAsync(CancellationToken token);
}

// Coordinator only. No connection opener, secret resolution, process launcher, environment mutation or database cleanup.
internal static class Phase7SqlValidationOrchestrator
{
    public static IReadOnlyList<string> ProvisioningStages { get; } = Array.AsReadOnly(new[] { "00-admission.sql", "10-create-databases.sql",
        "20-schema-A.sql", "20-schema-B.sql", "30-seed-A.sql", "30-seed-B.sql", "40-faults-A.sql", "40-faults-B.sql", "50-principals-and-monitor.sql" });
    internal static JsonSerializerOptions Web { get; } = new(Phase7SqlEvidenceFiles.Json) { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    public static string Fingerprint(Phase7SqlValidationPlan plan) => Phase7SqlEvidenceFiles.Hash(Phase7SqlEvidenceFiles.Encode(plan));

    public static Task<Phase7SqlValidationResult> ExecuteAsync(Phase7SqlValidationPlan plan, Phase7SqlValidationApproval approval,
        Phase7SqlValidationHostBindings? bindings = null, bool execute = false, CancellationToken token = default)
    {
        var context = Prepare(plan, approval, TimeProvider.System.GetUtcNow(), execute, token);
        if (!execute) return Task.FromResult(new Phase7SqlValidationResult("Review", "Review", Phase7SqlEvidenceOrigin.Offline, null));
        Require(bindings is not null, "HOST_REQUIRED");
        return RunAsync(context, new Phase7SqlNativeValidationDriver(bindings!), TimeProvider.System, false, token);
    }
    public static Task<Phase7SqlValidationResult> SimulateAsync(Phase7SqlValidationPlan plan, Phase7SqlValidationApproval approval,
        IPhase7SqlValidationDriver driver, TimeProvider clock, CancellationToken token = default) =>
        RunAsync(Prepare(plan, approval, clock.GetUtcNow(), true, token), driver, clock, true, token);

    private static Phase7SqlValidationContext Prepare(Phase7SqlValidationPlan plan, Phase7SqlValidationApproval approval,
        DateTimeOffset now, bool active, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        try
        {
            var frozen = Phase7SqlEvidenceFiles.Decode<Phase7SqlValidationPlan>(Phase7SqlEvidenceFiles.Encode(plan));
            Require(frozen.RunId != Guid.Empty && approval.RunId == frozen.RunId && approval.PlanSha256 == Fingerprint(frozen)
                && Token(approval.AuthorizationId) && Token(frozen.PreparationAuthorizationId) && Token(frozen.ProvisioningAuthorizationId)
                && new[] { approval.AuthorizationId, frozen.PreparationAuthorizationId, frozen.ProvisioningAuthorizationId }.Distinct(StringComparer.Ordinal).Count() == 3, "APPROVAL");
            Require(frozen.NotBeforeUtc.Offset == TimeSpan.Zero && frozen.ExpiresAtUtc.Offset == TimeSpan.Zero && now.Offset == TimeSpan.Zero
                && frozen.NotBeforeUtc < frozen.ExpiresAtUtc && frozen.ExpiresAtUtc - frozen.NotBeforeUtc <= TimeSpan.FromHours(24)
                && frozen.TotalTimeoutSeconds is >= 1 and <= 3600 && frozen.StageTimeoutSeconds is >= 1 and <= 3600
                && frozen.StageTimeoutSeconds <= frozen.TotalTimeoutSeconds, "BUDGET");
            if (active) Require(approval.ExecutionAuthorized && now >= frozen.NotBeforeUtc && now < frozen.ExpiresAtUtc, "WINDOW");
            Require(Path.IsPathFullyQualified(frozen.OutputDirectory) && Path.IsPathFullyQualified(frozen.ProvisioningBundleDirectory), "PATH");
            var output = Path.TrimEndingDirectorySeparator(Path.GetFullPath(frozen.OutputDirectory));
            foreach (var source in new[] { frozen.ProvisioningBundleDirectory, frozen.Candidate.RootPath })
            {
                Phase7SqlEvidenceFiles.CheckNoLinks(source);
                Require(!Inside(output, Path.GetFullPath(source)) && !Inside(Path.GetFullPath(source), output), "OUTPUT");
            }
            Phase7SqlEvidenceFiles.CheckNoLinks(output);
            Require(!Directory.Exists(output) && !File.Exists(output), "OUTPUT");
            frozen.Candidate.Verify(token);
            var bytes = Phase7SqlEvidenceFiles.Read(frozen.ProvisioningBundleDirectory, "bundle-manifest.json", 1024 * 1024, token);
            Require(Phase7SqlEvidenceFiles.Hash(bytes) == frozen.ProvisioningBundleSha256, "BUNDLE_PIN");
            Phase7SqlEvidenceFiles.ValidateJson(bytes); var node = JsonNode.Parse(bytes)!;
            Require(node["RunId"]!.GetValue<Guid>() == frozen.RunId && node["PreparationAuthorizationId"]!.GetValue<string>() == frozen.PreparationAuthorizationId
                && !node["ExecutionAuthorized"]!.GetValue<bool>() && !node["SqlExecuted"]!.GetValue<bool>() && !node["SapExecuted"]!.GetValue<bool>(), "BUNDLE_SCOPE");
            var files = node["Files"]!.AsArray(); Require(files.Count is > 0 and <= 64, "BUNDLE_FILES");
            var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase); long total = 0;
            byte[]? spec = null;
            foreach (var file in files)
            {
                var path = file!["Path"]!.GetValue<string>(); Require(path != "bundle-manifest.json" && paths.Add(path), "BUNDLE_FILES");
                var value = Phase7SqlEvidenceFiles.Read(frozen.ProvisioningBundleDirectory, path, 16 * 1024 * 1024, token); total += value.Length;
                Require(total <= 64 * 1024 * 1024 && value.LongLength == file["Bytes"]!.GetValue<long>()
                    && Phase7SqlEvidenceFiles.Hash(value) == file["Sha256"]!.GetValue<string>(), "BUNDLE_HASH");
                if (path == "provisioning-spec.json") spec = value;
            }
            Require(spec is not null && Phase7SqlEvidenceFiles.Hash(spec) == node["SpecSha256"]!.GetValue<string>(), "SPEC_PIN");
            Phase7SqlEvidenceFiles.ValidateJson(spec!); var target = JsonNode.Parse(spec!)!;
            Require(target["RunId"]!.GetValue<Guid>() == frozen.RunId
                && target["PreparationAuthorizationId"]!.GetValue<string>() == frozen.PreparationAuthorizationId, "SPEC_SCOPE");
            return new(frozen, approval, new(target["ExpectedServerName"]!.GetValue<string>(), target["ProductVersion"]!.GetValue<string>(),
                target["CompanyIdA"]!.GetValue<int>(), target["CompanyIdB"]!.GetValue<int>(), target["CompatibilityLevel"]!.GetValue<int>(),
                target["Collation"]!.GetValue<string>(), target["ReadCommittedSnapshot"]!.GetValue<bool>()));
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { throw; }
        catch (Phase7SqlGuardException) { throw; }
        catch { throw Guard("ADMISSION"); }
    }

    private static async Task<Phase7SqlValidationResult> RunAsync(Phase7SqlValidationContext context, IPhase7SqlValidationDriver driver,
        TimeProvider clock, bool simulation, CancellationToken token)
    {
        var plan = context.Plan; var mode = simulation ? "Simulation" : "Execution";
        var origin = simulation ? Phase7SqlEvidenceOrigin.Offline : Phase7SqlEvidenceOrigin.SqlServerLaboratory;
        using var budget = CancellationTokenSource.CreateLinkedTokenSource(token);
        var watch = Stopwatch.StartNew(); var started = clock.GetUtcNow(); var touched = false; var claimed = false; int sequence = 0;
        Exception? primary = null; Phase7SqlValidationResult? result = null;
        void Check()
        {
            budget.Token.ThrowIfCancellationRequested(); var now = clock.GetUtcNow();
            Require(now >= plan.NotBeforeUtc && now < plan.ExpiresAtUtc && watch.Elapsed.TotalSeconds < plan.TotalTimeoutSeconds, "DEADLINE");
        }
        void Save<T>(string path, T value, CancellationToken ct = default) => Phase7SqlEvidenceFiles.Write(plan.OutputDirectory, path, Phase7SqlEvidenceFiles.Encode(value), ct);
        async Task<T> Stage<T>(string name, Func<CancellationToken, Task<T>> action)
        {
            Check(); var id = ++sequence;
            Save($"stages/{id:00}-started.json", new { plan.RunId, Stage = name, Status = "Started", Mode = mode, AtUtc = clock.GetUtcNow() }, budget.Token);
            using var stage = CancellationTokenSource.CreateLinkedTokenSource(budget.Token); stage.CancelAfter(TimeSpan.FromSeconds(plan.StageTimeoutSeconds));
            var value = await Task.Run(() => action(stage.Token)).WaitAsync(stage.Token);
            Check(); Save($"stages/{id:00}-completed.json", new { plan.RunId, Stage = name, Status = "Completed", Mode = mode, AtUtc = clock.GetUtcNow() }, budget.Token);
            return value;
        }
        try
        {
            Check(); budget.CancelAfter(TimeSpan.FromSeconds(Math.Min(plan.TotalTimeoutSeconds, (plan.ExpiresAtUtc - started).TotalSeconds)));
            Save("intent.json", new { plan.RunId, context.Approval.PlanSha256, context.Approval.AuthorizationId, Mode = mode, Origin = origin }, budget.Token); claimed = true;
            Save("plan.json", plan, budget.Token); Save("approval.json", context.Approval, budget.Token);
            touched = true;
            var review = await Stage("ReviewProvisioning", ct => driver.ReviewProvisioningAsync(context, ct));
            Require(review.RunId == plan.RunId && review.ManifestSha256 == plan.ProvisioningBundleSha256 && !review.SqlExecuted && review.BatchCount is >= 9 and <= 2200, "NATIVE_REVIEW");
            var provision = await Stage("Provision", ct => driver.ProvisionAsync(context, ct));
            Require(provision.RunId == plan.RunId && provision.SqlExecuted == !simulation, "PROVISION");
            VerifyJournal(context, provision, review.BatchCount, simulation, clock.GetUtcNow(), budget.Token);
            var suite = await Stage("ApproveSuite", ct => driver.ApproveSuiteAsync(context, ct));
            if (suite is null) result = new("AwaitingSuiteApproval", mode, origin, null);
            else
            {
                suite = FreezeSuite(suite); var manifest = ValidateSuite(context, suite, clock.GetUtcNow());
                Require(!Directory.Exists(context.SuiteRoot) && !File.Exists(context.SuiteRoot), "SUITE_REPLAY");
                PrepareSuiteFiles(context, suite, budget.Token);
                var completion = await Stage("Suite", ct => driver.RunSuiteAsync(context, suite, ct));
                ValidateCompletedSuite(context, suite, manifest, completion, started, clock.GetUtcNow(), budget.Token);
                var monitored = await Stage("Monitor", ct => driver.MonitorAsync(context, suite, completion, ct));
                if (monitored is null) result = new("AwaitingMonitorApproval", mode, origin, null);
                else
                {
                    Require(monitored.Approval.Monitor.AuthorizationReference != context.Approval.AuthorizationId
                        && monitored.Approval.Monitor.AuthorizationReference != plan.ProvisioningAuthorizationId, "MONITOR_APPROVAL");
                    var capture = Phase7SqlMonitorCaptureReader.Load(context.MonitorDirectory, context.SuiteRoot, monitored.IndexSha256, monitored.Approval, suite.Approval, budget.Token);
                    Require(capture.Index.Origin == origin && capture.Witness.ProcessId == completion.ProcessId
                        && capture.Witness.ExitCode == completion.ExitCode, "MONITOR_ORIGIN_OR_PROCESS");
                    var package = await Stage("Package", ct => Task.FromResult(Phase7SqlEvidencePackager.BuildFromMonitorCapture(context.SuiteRoot,
                        context.PackageDirectory, suite.Approval, plan.Candidate, context.MonitorDirectory, monitored.IndexSha256, monitored.Approval, ct)));
                    Require(package.Report.RunId == plan.RunId && package.Report.Origin == origin
                        && Phase7SqlEvidencePackager.VerifyZip(package.ZipPath, budget.Token, package.ArchiveSha256).IntegrityVerified, "PACKAGE");
                    result = new(package.ReadyForIndependentRunReview && completion.ExitCode == 0 && completion.RunnerExitCode is null or 0 ? "Completed" : "CompletedWithBlockers", mode, origin, package);
                }
            }
        }
        catch (OperationCanceledException error) { primary = error; throw; }
        catch (Phase7SqlGuardException error) { primary = error; throw; }
        catch { primary = Guard("STAGE_FAILED"); throw primary; }
        finally
        {
            var cleanupOnlyFailure = false;
            if (touched)
            {
                using var cleanup = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                try { await Task.Run(() => driver.StopAsync(cleanup.Token)).WaitAsync(cleanup.Token); }
                catch
                {
                    if (primary is null) { primary = Guard("CLEANUP"); cleanupOnlyFailure = true; }
                    primary.Data["OrchestrationCleanupFailed"] = true;
                }
            }
            if (primary is not null && claimed)
                try { Save("failure.json", new { plan.RunId, Code = cleanupOnlyFailure ? "CleanupFailed" : primary is OperationCanceledException ? "CancelledOrTimedOut" : "StageFailed", CleanupFailed = primary.Data.Contains("OrchestrationCleanupFailed"), Mode = mode }); } catch { }
            if (cleanupOnlyFailure) throw primary!;
        }
        Check();
        Save("orchestration-result.json", new { plan.RunId, result!.State, Mode = mode, Origin = origin, ArchiveSha256 = result.Package?.ArchiveSha256,
            EvidenceAuthenticityVerified = false, GlobalPhase7Closed = false }, budget.Token);
        return result;
    }

    private sealed record JournalRow(Guid RunId, string ManifestSha256, string AuthorizationId, string Origin, DateTimeOffset TimestampUtc,
        string Status, string Stage, int Batch, string Sha256);
    private static void VerifyJournal(Phase7SqlValidationContext context, Phase7SqlProvisioningCompletion receipt, int count, bool simulation,
        DateTimeOffset now, CancellationToken token)
    {
        var bytes = Phase7SqlEvidenceFiles.Read(context.Plan.OutputDirectory, "provisioning.jsonl", 16 * 1024 * 1024, token);
        Require(Phase7SqlEvidenceFiles.Hash(bytes) == receipt.JournalSha256, "PROVISION_JOURNAL_HASH");
        var lines = new UTF8Encoding(false, true).GetString(bytes).Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Require(lines.Length == count * 2 + 2, "PROVISION_JOURNAL_COUNT");
        var rows = lines.Select(s => Phase7SqlEvidenceFiles.Decode<JournalRow>(Encoding.UTF8.GetBytes(s))).ToArray();
        Require(rows.All(r => r.RunId == context.Plan.RunId && r.ManifestSha256 == context.Plan.ProvisioningBundleSha256
            && r.AuthorizationId == context.Plan.ProvisioningAuthorizationId && r.Origin == (simulation ? "OfflineSimulation" : "SqlServerProvisioning")
            && r.TimestampUtc.Offset == TimeSpan.Zero && r.TimestampUtc >= context.Plan.NotBeforeUtc && r.TimestampUtc <= now), "PROVISION_JOURNAL_SCOPE");
        Require(rows[0].Status == "Admitted" && rows[0].Stage == "Admission" && rows[0].Batch == 0
            && rows[^1].Status == "Finished" && rows[^1].Stage == "Session" && rows[^1].Batch == 0, "PROVISION_JOURNAL_END");
        var stages = new List<string>(); string? previous = null; int batch = 0;
        for (var i = 1; i < rows.Length - 1; i += 2)
        {
            var a = rows[i]; var b = rows[i + 1];
            if (a.Stage != previous) { stages.Add(a.Stage); previous = a.Stage; batch = 0; }
            Require(a.Status == "Started" && b.Status == "Completed" && a.Stage == b.Stage && a.Batch == ++batch && b.Batch == a.Batch
                && Phase7SqlEvidenceFiles.IsHash(a.Sha256) && a.Sha256 == b.Sha256 && b.TimestampUtc >= a.TimestampUtc, "PROVISION_JOURNAL_PAIR");
        }
        Require(stages.SequenceEqual(ProvisioningStages), "PROVISION_JOURNAL_STAGES");
    }
    private static Phase7SqlApprovedSuite FreezeSuite(Phase7SqlApprovedSuite suite) => Phase7SqlEvidenceFiles.Decode<Phase7SqlApprovedSuite>(Phase7SqlEvidenceFiles.Encode(suite));
    private static Phase7SqlManifest ValidateSuite(Phase7SqlValidationContext c, Phase7SqlApprovedSuite s, DateTimeOffset now)
    {
        var valid = Phase7SqlManifestValidator.Validate(s.ManifestJson, s.Approval, new(true, true), now);
        var m = valid.Manifest;
        var target = c.ProvisioningTarget;
        Require(target is not null && m.Server.ExpectedServerName == target.ServerName && m.Server.ProductVersion == target.ProductVersion
            && m.TenantA.CompanyId == target.CompanyA && m.TenantB.CompanyId == target.CompanyB
            && new[] { m.TenantA, m.TenantB }.All(t => t.CompatibilityLevel == target.Compatibility && t.Collation == target.Collation
                && t.ReadCommittedSnapshot == target.ReadCommittedSnapshot), "PROVISIONED_TARGET");
        Require(m.RunId == c.Plan.RunId && s.Approval.ProvisioningAuthorizationId == c.Plan.PreparationAuthorizationId
            && m.ExecutionAuthorizationId != c.Approval.AuthorizationId && m.ExecutionAuthorizationId != c.Plan.ProvisioningAuthorizationId
            && s.Inputs.SourceInventorySha256 == c.Plan.Candidate.SourceSha256 && s.Inputs.BinaryInventorySha256 == c.Plan.Candidate.BinarySha256
            && s.Inputs.OfflineBaselineSha256 == Phase7SqlEvidenceFiles.Hash(s.OfflineBaseline), "SUITE_BINDING");
        Phase7SqlSuiteLoader.ValidateInputs(s.Inputs, m, valid.ManifestSha256);
        var baseline = Phase7SqlEvidenceTrx.Inspect(s.OfflineBaseline, true); Require(baseline.Finish <= now, "BASELINE_TIME");
        c.Plan.Candidate.Verify(); return m;
    }
    private static void PrepareSuiteFiles(Phase7SqlValidationContext c, Phase7SqlApprovedSuite s, CancellationToken token)
    {
        Phase7SqlEvidenceFiles.Write(c.PreparedDirectory, "manifest.json", Encoding.UTF8.GetBytes(s.ManifestJson), token);
        Phase7SqlEvidenceFiles.Write(c.PreparedDirectory, "approval.json", JsonSerializer.SerializeToUtf8Bytes(s.Approval, Web), token);
        Phase7SqlEvidenceFiles.Write(c.PreparedDirectory, "suite-inputs.json", JsonSerializer.SerializeToUtf8Bytes(s.Inputs, Web), token);
        Phase7SqlEvidenceFiles.Write(c.PreparedDirectory, "candidate.json", Phase7SqlEvidenceFiles.Encode(c.Plan.Candidate), token);
        Phase7SqlEvidenceFiles.Write(c.PreparedDirectory, "offline-baseline.trx", s.OfflineBaseline, token);
        Directory.CreateDirectory(c.SuiteWorkingDirectory);
    }
    private static void ValidateCompletedSuite(Phase7SqlValidationContext c, Phase7SqlApprovedSuite s, Phase7SqlManifest m,
        Phase7SqlSuiteCompletion completion, DateTimeOffset started, DateTimeOffset now, CancellationToken token)
    {
        byte[] Read(string p) => Phase7SqlEvidenceFiles.Read(c.SuiteRoot, p, 16 * 1024 * 1024, token);
        Require(Read("manifest.json").SequenceEqual(Encoding.UTF8.GetBytes(s.ManifestJson))
            && Read("approval.json").SequenceEqual(JsonSerializer.SerializeToUtf8Bytes(s.Approval, Web))
            && Read("suite-inputs.json").SequenceEqual(JsonSerializer.SerializeToUtf8Bytes(s.Inputs, Web)), "SUITE_ARCHIVE");
        var start = Phase7SqlEvidenceFiles.Decode<Phase7SqlRuntimeStart>(Read("global/start.json"));
        var end = Phase7SqlEvidenceFiles.Decode<Phase7SqlRuntimeEnd>(Read("global/end.json"));
        var trx = Phase7SqlEvidenceTrx.Inspect(Read("sql.trx"), false);
        Require(start.RunId == m.RunId && end.RunId == m.RunId && start.ProcessId == completion.ProcessId && end.ProcessId == completion.ProcessId
            && start.StartedAtUtc >= started && start.StartedAtUtc <= end.StoppedAtUtc && end.StoppedAtUtc <= now && trx.Finish <= now
            && trx.Tests == Phase7SqlExecutionCatalog.Cases.Count && start.SourceSha256 == c.Plan.Candidate.SourceSha256
            && start.BinarySha256 == c.Plan.Candidate.BinarySha256, "SUITE_COMPLETION");
    }
    private static bool Token(string value) => value is { Length: > 0 and <= 128 } && System.Text.RegularExpressions.Regex.IsMatch(value, "\\A[A-Za-z0-9_][A-Za-z0-9_.-]*\\z");
    private static bool Inside(string path, string root) => string.Equals(path, root, StringComparison.OrdinalIgnoreCase)
        || path.StartsWith(Path.TrimEndingDirectorySeparator(root) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
    private static void Require(bool valid, string code) { if (!valid) throw Guard(code); }
    private static Phase7SqlGuardException Guard(string code) => new("PHASE7_SQL_ORCHESTRATION_" + code);
}
