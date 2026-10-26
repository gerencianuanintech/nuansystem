using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal sealed record Phase7SqlMonitorInputs(string RunRoot, string OutputDirectory, IReadOnlyDictionary<string, string> Sha256);
internal sealed record Phase7SqlControlledMonitorApproval(Phase7SqlMonitorApproval Monitor, string InputsSha256,
    int CommandTimeoutSeconds, int TotalTimeoutSeconds, bool ExecutionAuthorized);
internal sealed record Phase7SqlMonitorIsolation(string Inspector, string AuthorizationReference,
    DateTimeOffset WorkersOffFromUtc, DateTimeOffset WorkersOffThroughUtc, IReadOnlyList<string> DisabledComponents);
internal sealed record Phase7SqlMonitorProcessCapture(Phase7SqlIndependentWitness Witness, DateTimeOffset ProcessStartedAtUtc);
internal sealed record Phase7SqlMonitorCaptureIndex(Guid RunId, Phase7SqlEvidenceOrigin Origin, string InputsSha256,
    IReadOnlyList<Phase7SqlMonitorCaptureFile> Files);
internal sealed record Phase7SqlMonitorCaptureFile(string RelativePath, long Length, string Sha256);
internal sealed record Phase7SqlMonitorExecutionResult(string Mode, Phase7SqlEvidenceOrigin Origin, bool CaptureCompleted,
    bool ObservationsConform, Phase7SqlMonitorObservation? Observation, Phase7SqlIndependentWitness? Witness,
    string? OutputDirectory, string? IndexSha256)
{
    public bool SqlExecuted => CaptureCompleted && Origin == Phase7SqlEvidenceOrigin.SqlServerLaboratory;
    public bool EvidenceAuthenticityVerified => false;
    public bool GlobalPhase7Closed => false;
}

internal sealed class Phase7SqlMonitorPlan(Phase7SqlManifest manifest, Phase7SqlRuntimeStart start, Phase7SqlRuntimeEnd end,
    Phase7SqlTrxInfo trx, IReadOnlyDictionary<string, byte[]> snapshots, byte[] inputIndex, string output)
{
    public Phase7SqlManifest Manifest { get; } = manifest;
    public Phase7SqlRuntimeStart Start { get; } = start;
    public Phase7SqlRuntimeEnd End { get; } = end;
    public Phase7SqlTrxInfo Trx { get; } = trx;
    internal IReadOnlyDictionary<string, byte[]> Snapshots { get; } = snapshots;
    internal byte[] InputIndex { get; } = inputIndex;
    internal string Output { get; } = output;
}

// Only the explicitly named simulation path accepts this port. Real execution constructs a private SqlProbe.
internal interface IPhase7SqlMonitorProbe
{
    Task<Phase7SqlMonitorProcessCapture> WitnessAsync(Phase7SqlMonitorPlan plan, Phase7SqlMonitorIsolation isolation, CancellationToken token);
    Task<Phase7SqlMonitorObservation> ReadAsync(Phase7SqlMonitorPlan plan, Phase7SqlMonitorApproval approval,
        DateTimeOffset now, int timeout, CancellationToken token);
    Task CloseAsync();
}

internal static class Phase7SqlMonitorExecutor
{
    public static IReadOnlyList<string> InputPaths { get; } = Array.AsReadOnly(new[]
        { "manifest.json", "global/start.json", "global/end.json", "sql.trx", "global/final-A.json", "global/final-B.json" });

    public static string InputFingerprint(Phase7SqlMonitorInputs inputs) => Phase7SqlEvidenceFiles.Hash(Index(inputs.Sha256));
    private static byte[] Index(IReadOnlyDictionary<string, string> hashes)
    {
        Require(hashes.Count == InputPaths.Count && InputPaths.All(hashes.ContainsKey)
            && hashes.Values.All(Phase7SqlEvidenceFiles.IsHash), "INPUT_INDEX");
        return Phase7SqlEvidenceFiles.Encode(InputPaths.Select(p => new { Path = p, Sha256 = hashes[p] }).ToArray());
    }

    // Review by default. No connection creation/opening, environment reads or process launch.
    public static async Task<Phase7SqlMonitorExecutionResult> ExecuteAsync(Phase7SqlMonitorInputs inputs,
        Phase7SqlApproval suiteApproval, Phase7SqlControlledMonitorApproval approval, Phase7SqlMonitorIsolation isolation,
        SqlConnection? connection = null, Process? child = null, bool execute = false, CancellationToken token = default)
    {
        var clock = TimeProvider.System;
        var plan = Prepare(inputs, suiteApproval, approval, isolation, clock.GetUtcNow(), execute, token);
        if (!execute) return new("Review", Phase7SqlEvidenceOrigin.Offline, false, false, null, null, null, null);
        Require(connection is not null && child is not null, "LIVE_RESOURCES");
        ValidateConnection(connection!, plan.Manifest);
        var probe = new SqlProbe(connection!, child!);
        Exception? primary = null;
        try { return await RunAsync(plan, approval, Freeze(isolation), probe, clock, Phase7SqlEvidenceOrigin.SqlServerLaboratory, token); }
        catch (Exception error) { primary = error; throw; }
        finally
        {
            if (!probe.CloseScheduled && connection!.State != System.Data.ConnectionState.Closed)
                try { await CloseBoundedAsync(probe); }
                catch when (primary is not null) { primary.Data["MonitorCleanupFailed"] = true; }
                catch { throw Guard("CLOSE"); }
        }
    }

    public static Task<Phase7SqlMonitorExecutionResult> SimulateAsync(Phase7SqlMonitorInputs inputs, Phase7SqlApproval suiteApproval,
        Phase7SqlControlledMonitorApproval approval, Phase7SqlMonitorIsolation isolation, IPhase7SqlMonitorProbe probe,
        TimeProvider clock, CancellationToken token = default)
    {
        var plan = Prepare(inputs, suiteApproval, approval, isolation, clock.GetUtcNow(), true, token);
        return RunAsync(plan, approval, Freeze(isolation), probe, clock, Phase7SqlEvidenceOrigin.Offline, token);
    }

    private static Phase7SqlMonitorPlan Prepare(Phase7SqlMonitorInputs inputs, Phase7SqlApproval suiteApproval,
        Phase7SqlControlledMonitorApproval approval, Phase7SqlMonitorIsolation isolation, DateTimeOffset now, bool active, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        try
        {
            var hashes = new ReadOnlyDictionary<string, string>(inputs.Sha256.ToDictionary(p => p.Key, p => p.Value, StringComparer.Ordinal));
            var index = Index(hashes);
            Require(approval.InputsSha256 == Phase7SqlEvidenceFiles.Hash(index), "INPUT_PIN");
            Require(Path.IsPathFullyQualified(inputs.RunRoot) && Path.IsPathFullyQualified(inputs.OutputDirectory), "PATH");
            var root = Path.TrimEndingDirectorySeparator(Path.GetFullPath(inputs.RunRoot));
            var output = Path.TrimEndingDirectorySeparator(Path.GetFullPath(inputs.OutputDirectory));
            Phase7SqlEvidenceFiles.CheckNoLinks(root); Phase7SqlEvidenceFiles.CheckNoLinks(output);
            Require(!Directory.Exists(output) && !File.Exists(output) && root != output
                && !Inside(root, output) && !Inside(output, root), "OUTPUT");
            var bytes = new Dictionary<string, byte[]>();
            foreach (var path in InputPaths)
            {
                var value = Phase7SqlEvidenceFiles.Read(root, path, path.StartsWith("global/final-", StringComparison.Ordinal) ? 32 * 1024 * 1024 : 1024 * 1024, token);
                Require(Phase7SqlEvidenceFiles.Hash(value) == hashes[path], "INPUT_HASH"); bytes.Add(path, value);
            }
            var start = Phase7SqlEvidenceFiles.Decode<Phase7SqlRuntimeStart>(bytes["global/start.json"]);
            var end = Phase7SqlEvidenceFiles.Decode<Phase7SqlRuntimeEnd>(bytes["global/end.json"]);
            var manifest = Phase7SqlManifestValidator.Validate(new UTF8Encoding(false, true).GetString(bytes["manifest.json"]),
                suiteApproval, new(true, true), start.StartedAtUtc).Manifest;
            Require(start.RunId == manifest.RunId && end.RunId == manifest.RunId && start.ProcessId > 0 && start.ProcessId == end.ProcessId
                && start.StartedAtUtc.Offset == TimeSpan.Zero && end.StoppedAtUtc.Offset == TimeSpan.Zero
                && start.StartedAtUtc <= end.StoppedAtUtc && end.StoppedAtUtc <= manifest.ExpiresAtUtc
                && start.CatalogSha256 == Phase7SqlExecutionCatalog.Sha256
                && !end.StandardGateEnabled && !end.IsolatedGateEnabled && !end.SapExecuted
                && end.WorkerExecutions == 0 && end.ExternalBoundaryAttempts == 0, "RUNTIME");
            var trx = Phase7SqlEvidenceTrx.Inspect(bytes["sql.trx"], false);
            Require(trx.Tests == Phase7SqlExecutionCatalog.Cases.Count && trx.Start <= start.StartedAtUtc
                && trx.Finish >= end.StoppedAtUtc && trx.Start >= manifest.NotBeforeUtc && trx.Finish <= manifest.ExpiresAtUtc, "TRX_SCOPE");
            var snapshots = new ReadOnlyDictionary<string, byte[]>(new Dictionary<string, byte[]>
                { ["A"] = bytes["global/final-A.json"], ["B"] = bytes["global/final-B.json"] });
            foreach (var snapshot in snapshots.Values) Phase7SqlGlobalControls.ValidatePhysicalSnapshot(snapshot);
            ValidateApproval(approval, manifest, isolation, trx, now, active);
            return new(manifest, start, end with { CompletedCases = Array.AsReadOnly(end.CompletedCases.ToArray()) }, trx, snapshots, index, output);
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { throw; }
        catch (Phase7SqlGuardException) { throw; }
        catch { throw Guard("PREPARE"); }
    }

    private static void ValidateApproval(Phase7SqlControlledMonitorApproval a, Phase7SqlManifest m,
        Phase7SqlMonitorIsolation isolation, Phase7SqlTrxInfo trx, DateTimeOffset now, bool active)
    {
        var p = a.Monitor;
        Require(p.RunId == m.RunId && Token(p.AuthorizationReference) && p.AuthorizationReference != m.ExecutionAuthorizationId
            && p.AuthorizationReference != m.ProvisioningAuthorizationId && Token(p.Principal)
            && !new[] { "sa", "dbo", "guest", m.TenantA.ApplicationPrincipal, m.TenantB.ApplicationPrincipal,
                m.TenantA.ObserverPrincipal, m.TenantB.ObserverPrincipal }.Contains(p.Principal, StringComparer.OrdinalIgnoreCase), "APPROVAL_SCOPE");
        Require(p.NotBeforeUtc.Offset == TimeSpan.Zero && p.ExpiresAtUtc.Offset == TimeSpan.Zero && p.NotBeforeUtc < p.ExpiresAtUtc
            && p.ExpiresAtUtc - p.NotBeforeUtc <= TimeSpan.FromHours(24) && now.Offset == TimeSpan.Zero
            && a.CommandTimeoutSeconds is >= 1 and <= 120 && a.TotalTimeoutSeconds is >= 1 and <= 3600
            && a.CommandTimeoutSeconds <= a.TotalTimeoutSeconds, "BUDGET");
        Require(Token(isolation.Inspector) && isolation.AuthorizationReference == p.AuthorizationReference
            && isolation.WorkersOffFromUtc.Offset == TimeSpan.Zero && isolation.WorkersOffThroughUtc.Offset == TimeSpan.Zero
            && isolation.WorkersOffFromUtc <= trx.Start && isolation.WorkersOffThroughUtc >= trx.Finish
            && isolation.DisabledComponents.Count == 4 && isolation.DisabledComponents.ToHashSet(StringComparer.Ordinal)
                .SetEquals(["SyncWorker", "MasterBranchSyncWorker", "SriWorker", "Relay"]), "ISOLATION");
        if (active) Require(a.ExecutionAuthorized && now >= p.NotBeforeUtc && now < p.ExpiresAtUtc
            && now >= trx.Finish && now < isolation.WorkersOffThroughUtc, "APPROVAL_WINDOW");
    }

    private static async Task<Phase7SqlMonitorExecutionResult> RunAsync(Phase7SqlMonitorPlan plan,
        Phase7SqlControlledMonitorApproval approval, Phase7SqlMonitorIsolation isolation, IPhase7SqlMonitorProbe probe,
        TimeProvider clock, Phase7SqlEvidenceOrigin origin, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        using var budget = CancellationTokenSource.CreateLinkedTokenSource(token);
        var until = approval.Monitor.ExpiresAtUtc < isolation.WorkersOffThroughUtc ? approval.Monitor.ExpiresAtUtc : isolation.WorkersOffThroughUtc;
        budget.CancelAfter(TimeSpan.FromSeconds(Math.Min(approval.TotalTimeoutSeconds, (until - clock.GetUtcNow()).TotalSeconds)));
        var watch = Stopwatch.StartNew(); var files = new List<Phase7SqlMonitorCaptureFile>();
        Phase7SqlMonitorObservation? observation = null; Phase7SqlIndependentWitness? witness = null; bool conforms = false;
        Exception? primary = null;
        int Check()
        {
            budget.Token.ThrowIfCancellationRequested();
            ValidateApproval(approval, plan.Manifest, isolation, plan.Trx, clock.GetUtcNow(), true);
            var remaining = Math.Min(approval.TotalTimeoutSeconds - watch.Elapsed.TotalSeconds, (until - clock.GetUtcNow()).TotalSeconds);
            Require(remaining > 0, "DEADLINE");
            return Math.Max(1, (int)Math.Min(approval.CommandTimeoutSeconds, Math.Ceiling(remaining)));
        }
        void Write<T>(string path, T value)
        {
            var bytes = Phase7SqlEvidenceFiles.Encode(value);
            Phase7SqlEvidenceFiles.Write(plan.Output, path, bytes, budget.Token);
            files.Add(new(path, bytes.LongLength, Phase7SqlEvidenceFiles.Hash(bytes)));
        }
        try
        {
            _ = Check();
            // CreateNew claim wins atomically even when two invokers passed the offline output check.
            Write("intent.json", new { plan.Manifest.RunId, approval.InputsSha256, approval.Monitor.AuthorizationReference, Origin = origin });
            var process = await probe.WitnessAsync(plan, isolation, budget.Token).WaitAsync(budget.Token);
            witness = process.Witness with { Origin = origin };
            Require(witness.RunId == plan.Manifest.RunId && witness.ProcessId == plan.Start.ProcessId && witness.HasExited
                && process.ProcessStartedAtUtc.Offset == TimeSpan.Zero && process.ProcessStartedAtUtc <= plan.Start.StartedAtUtc
                && witness.ProcessExitObservedAtUtc is { Offset.Ticks: 0 } exited && exited >= plan.End.StoppedAtUtc
                && exited >= plan.Trx.Finish && exited <= clock.GetUtcNow(), "PROCESS");
            var timeout = Check();
            observation = await probe.ReadAsync(plan, approval.Monitor, clock.GetUtcNow(), timeout, budget.Token).WaitAsync(budget.Token);
            _ = Check();
            Require(observation.RunId == plan.Manifest.RunId && observation.ServerName == plan.Manifest.Server.ExpectedServerName
                && observation.ProductVersion == plan.Manifest.Server.ProductVersion && observation.Principal == approval.Monitor.Principal
                && observation.AuthorizationReference == approval.Monitor.AuthorizationReference && observation.ObservedAtUtc.Offset == TimeSpan.Zero
                && observation.ObservedAtUtc >= witness.ProcessExitObservedAtUtc && observation.ObservedAtUtc >= approval.Monitor.NotBeforeUtc
                && observation.ObservedAtUtc < approval.Monitor.ExpiresAtUtc && observation.ObservedAtUtc <= clock.GetUtcNow()
                && observation.RemainingSessions >= 0 && observation.TransactionsA >= 0 && observation.TransactionsB >= 0, "OBSERVATION_SCOPE");
            Require(witness.Inspector == isolation.Inspector && witness.AuthorizationReference == isolation.AuthorizationReference
                && witness.WorkersOffFromUtc == isolation.WorkersOffFromUtc && witness.WorkersOffThroughUtc == isolation.WorkersOffThroughUtc
                && witness.DisabledComponents.SequenceEqual(isolation.DisabledComponents), "WITNESS_BINDING");
            // Pure coherence check only. Persisted simulations keep Offline origin unconditionally.
            conforms = Phase7SqlGlobalControls.Termination(observation, witness with { Origin = Phase7SqlEvidenceOrigin.SqlServerLaboratory },
                plan.Manifest, plan.Start, plan.End, plan.Trx) && Phase7SqlGlobalControls.Retained(observation, plan.Manifest, plan.Snapshots)
                && plan.End.Completed && plan.End.CompletedCases.SequenceEqual(Phase7SqlExecutionCatalog.Cases.Select(c => c.Id));
            Write("observation.json", observation); Write("witness.json", witness);
        }
        catch (OperationCanceledException error) { primary = error; throw; }
        catch (Phase7SqlGuardException error) { primary = error; throw; }
        catch { primary = Guard("CAPTURE"); throw primary; }
        finally
        {
            try { await CloseBoundedAsync(probe); }
            catch when (primary is not null) { primary.Data["MonitorCleanupFailed"] = true; }
            catch { throw Guard("CLOSE"); }
        }
        _ = Check();
        using var inputDocument = System.Text.Json.JsonDocument.Parse(plan.InputIndex);
        Write("inputs-index.json", inputDocument.RootElement);
        Write("report.json", new { plan.Manifest.RunId, Origin = origin, ObservationsConform = conforms, EvidenceAuthenticityVerified = false, GlobalPhase7Closed = false });
        var index = new Phase7SqlMonitorCaptureIndex(plan.Manifest.RunId, origin, approval.InputsSha256, files.AsReadOnly());
        var indexBytes = Phase7SqlEvidenceFiles.Encode(index);
        Phase7SqlEvidenceFiles.Write(plan.Output, "capture-index.json", indexBytes, budget.Token);
        return new("Capture", origin, true, conforms, observation, witness, plan.Output, Phase7SqlEvidenceFiles.Hash(indexBytes));
    }

    private static void ValidateConnection(SqlConnection connection, Phase7SqlManifest manifest)
    {
        var settings = new SqlConnectionStringBuilder(connection.ConnectionString);
        Require(connection.State == System.Data.ConnectionState.Open && connection.Database == "master"
            && string.Equals(connection.DataSource, manifest.Server.DataSource, StringComparison.OrdinalIgnoreCase)
            && !connection.FireInfoMessageEventOnUserErrors && !settings.Pooling && !settings.Enlist
            && !settings.TrustServerCertificate && settings.Encrypt != SqlConnectionEncryptOption.Optional
            && System.Transactions.Transaction.Current is null, "CONNECTION");
    }
    private sealed class SqlProbe(SqlConnection connection, Process child) : IPhase7SqlMonitorProbe
    {
        public bool CloseScheduled { get; set; }
        public Task<Phase7SqlMonitorProcessCapture> WitnessAsync(Phase7SqlMonitorPlan plan, Phase7SqlMonitorIsolation isolation, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            Require(child.Id != Environment.ProcessId && child.Id == plan.Start.ProcessId, "PROCESS");
            var started = new DateTimeOffset(child.StartTime.ToUniversalTime());
            return Task.FromResult(new Phase7SqlMonitorProcessCapture(Phase7SqlMonitorReader.Witness(child, plan.Manifest.RunId,
                isolation.Inspector, isolation.AuthorizationReference, isolation.WorkersOffFromUtc, isolation.WorkersOffThroughUtc,
                isolation.DisabledComponents), started));
        }
        public async Task<Phase7SqlMonitorObservation> ReadAsync(Phase7SqlMonitorPlan plan, Phase7SqlMonitorApproval approval,
            DateTimeOffset now, int timeout, CancellationToken token)
        {
            ValidateConnection(connection, plan.Manifest);
            var result = await Phase7SqlMonitorReader.ReadAsync(connection, plan.Manifest, approval, now, timeout, token);
            ValidateConnection(connection, plan.Manifest); return result;
        }
        public Task CloseAsync() => connection.CloseAsync();
    }
    private static Task CloseBoundedAsync(IPhase7SqlMonitorProbe probe)
    {
        if (probe is SqlProbe live) live.CloseScheduled = true;
        // SqlClient inherits DbConnection.CloseAsync: its synchronous prefix can block before returning a Task.
        // Offload only cleanup, bound the wait, and never schedule a competing second close.
        return Task.Run(probe.CloseAsync).WaitAsync(TimeSpan.FromSeconds(5));
    }
    private static Phase7SqlMonitorIsolation Freeze(Phase7SqlMonitorIsolation value) => value with { DisabledComponents = Array.AsReadOnly(value.DisabledComponents.ToArray()) };
    private static bool Inside(string path, string parent) => path.StartsWith(parent + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
    private static bool Token(string value) => value is { Length: > 0 and <= 128 } && Regex.IsMatch(value, "\\A[A-Za-z0-9_][A-Za-z0-9_.@\\\\-]*\\z");
    private static void Require(bool condition, string code) { if (!condition) throw Guard(code); }
    private static Phase7SqlGuardException Guard(string code) => new("PHASE7_SQL_MONITOR_EXECUTOR_" + code);
}
