using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal sealed record Phase7SqlMonitorCaptureReference(string IndexSha256, Phase7SqlControlledMonitorApproval Approval);
internal sealed record Phase7SqlMonitorCaptureIntent(Guid RunId, string InputsSha256, string AuthorizationReference, Phase7SqlEvidenceOrigin Origin);
internal sealed record Phase7SqlMonitorCaptureReport(Guid RunId, Phase7SqlEvidenceOrigin Origin, bool ObservationsConform,
    bool EvidenceAuthenticityVerified, bool GlobalPhase7Closed);
internal sealed record Phase7SqlMonitorCaptureInput(string Path, string Sha256);
internal sealed record Phase7SqlCapturedTermination(Phase7SqlMonitorObservation Monitor, Phase7SqlIndependentWitness Witness);
internal sealed record Phase7SqlVerifiedMonitorCapture(string IndexHash, Phase7SqlMonitorCaptureIndex Index,
    Phase7SqlMonitorObservation Observation, Phase7SqlIndependentWitness Witness,
    IReadOnlyDictionary<string, byte[]> Files, IReadOnlyDictionary<string, byte[]> Inputs);

// Offline format/integrity verification only. A pin supplied by a trusted caller is not a human signature.
internal static class Phase7SqlMonitorCaptureReader
{
    internal const string Prefix = "monitor-capture/";
    internal static IReadOnlyList<string> ContentPaths { get; } = Array.AsReadOnly(new[]
        { "intent.json", "observation.json", "witness.json", "inputs-index.json", "report.json" });

    public static Phase7SqlVerifiedMonitorCapture Load(string captureRoot, string runRoot, string expectedIndexHash,
        Phase7SqlControlledMonitorApproval approval, Phase7SqlApproval suiteApproval, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        try
        {
            Require(Path.IsPathFullyQualified(captureRoot) && Path.IsPathFullyQualified(runRoot), "ROOT");
            Phase7SqlEvidenceFiles.CheckNoLinks(captureRoot);
            var expected = ContentPaths.Append("capture-index.json").ToHashSet(StringComparer.Ordinal);
            var entries = Directory.EnumerateFileSystemEntries(captureRoot).ToArray();
            Require(entries.Length == expected.Count && entries.All(p => File.Exists(p) && expected.Contains(Path.GetFileName(p))), "FILES");
            var files = expected.ToDictionary(p => p, p => Phase7SqlEvidenceFiles.Read(captureRoot, p, 1024 * 1024, token), StringComparer.Ordinal);
            var inputs = Phase7SqlMonitorExecutor.InputPaths.ToDictionary(p => p,
                p => Phase7SqlEvidenceFiles.Read(runRoot, p, p.StartsWith("global/final-", StringComparison.Ordinal)
                    ? Phase7SqlEvidenceFiles.MaximumFileBytes : p == "manifest.json" ? 65536 : 1024 * 1024, token), StringComparer.Ordinal);
            return Validate(files, inputs, expectedIndexHash, approval, suiteApproval);
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { throw; }
        catch (Phase7SqlGuardException) { throw; }
        catch { throw Guard("READ"); }
    }

    private static Phase7SqlVerifiedMonitorCapture Validate(IReadOnlyDictionary<string, byte[]> files,
        IReadOnlyDictionary<string, byte[]> inputs, string expectedHash, Phase7SqlControlledMonitorApproval approval, Phase7SqlApproval suiteApproval)
    {
        Require(Phase7SqlEvidenceFiles.IsHash(expectedHash) && Phase7SqlEvidenceFiles.Hash(files["capture-index.json"]) == expectedHash, "INDEX_PIN");
        var index = Phase7SqlEvidenceFiles.Decode<Phase7SqlMonitorCaptureIndex>(files["capture-index.json"]);
        Require(index.RunId != Guid.Empty && Enum.IsDefined(index.Origin) && index.Files.Count == ContentPaths.Count
            && index.Files.Select(f => f.RelativePath).Distinct(StringComparer.OrdinalIgnoreCase).Count() == ContentPaths.Count
            && index.Files.Select(f => f.RelativePath).ToHashSet(StringComparer.Ordinal).SetEquals(ContentPaths), "INDEX_FILES");
        foreach (var file in index.Files)
            Require(files.TryGetValue(Phase7SqlEvidenceFiles.SafeRelative(file.RelativePath), out var bytes)
                && bytes.LongLength == file.Length && Phase7SqlEvidenceFiles.Hash(bytes) == file.Sha256, "FILE_HASH");
        Require(index.InputsSha256 == approval.InputsSha256 && index.InputsSha256 == Phase7SqlEvidenceFiles.Hash(files["inputs-index.json"]), "INPUT_INDEX");
        var pins = Phase7SqlEvidenceFiles.Decode<Phase7SqlMonitorCaptureInput[]>(files["inputs-index.json"]);
        Require(pins.Length == Phase7SqlMonitorExecutor.InputPaths.Count
            && pins.Select(p => p.Path).Distinct(StringComparer.OrdinalIgnoreCase).Count() == pins.Length
            && pins.Select(p => p.Path).SequenceEqual(Phase7SqlMonitorExecutor.InputPaths), "INPUT_FILES");
        foreach (var pin in pins)
            Require(inputs.TryGetValue(pin.Path, out var content) && Phase7SqlEvidenceFiles.Hash(content) == pin.Sha256, "SOURCE_HASH");
        var start = Phase7SqlEvidenceFiles.Decode<Phase7SqlRuntimeStart>(inputs["global/start.json"]);
        var end = Phase7SqlEvidenceFiles.Decode<Phase7SqlRuntimeEnd>(inputs["global/end.json"]);
        var manifest = Phase7SqlManifestValidator.Validate(new UTF8Encoding(false, true).GetString(inputs["manifest.json"]),
            suiteApproval, new(true, true), start.StartedAtUtc).Manifest;
        var trx = Phase7SqlEvidenceTrx.Inspect(inputs["sql.trx"], false);
        Require(index.RunId == manifest.RunId && start.RunId == manifest.RunId && end.RunId == manifest.RunId
            && start.ProcessId > 0 && end.ProcessId == start.ProcessId && start.CatalogSha256 == Phase7SqlExecutionCatalog.Sha256
            && start.StartedAtUtc.Offset == TimeSpan.Zero && end.StoppedAtUtc.Offset == TimeSpan.Zero
            && start.StartedAtUtc <= end.StoppedAtUtc && end.StoppedAtUtc <= manifest.ExpiresAtUtc
            && trx.Tests == Phase7SqlExecutionCatalog.Cases.Count && trx.Start >= manifest.NotBeforeUtc && trx.Start <= start.StartedAtUtc
            && trx.Finish >= end.StoppedAtUtc && trx.Finish <= manifest.ExpiresAtUtc
            && !end.StandardGateEnabled && !end.IsolatedGateEnabled && !end.SapExecuted && end.ExternalBoundaryAttempts == 0 && end.WorkerExecutions == 0, "RUN");
        var intent = Phase7SqlEvidenceFiles.Decode<Phase7SqlMonitorCaptureIntent>(files["intent.json"]);
        var observation = Phase7SqlEvidenceFiles.Decode<Phase7SqlMonitorObservation>(files["observation.json"]);
        var witness = Phase7SqlEvidenceFiles.Decode<Phase7SqlIndependentWitness>(files["witness.json"]);
        var report = Phase7SqlEvidenceFiles.Decode<Phase7SqlMonitorCaptureReport>(files["report.json"]);
        var a = approval.Monitor;
        Require(approval.ExecutionAuthorized && approval.CommandTimeoutSeconds is >= 1 and <= 120 && approval.TotalTimeoutSeconds is >= 1 and <= 3600
            && approval.CommandTimeoutSeconds <= approval.TotalTimeoutSeconds && a.RunId == manifest.RunId
            && Token(a.AuthorizationReference) && a.AuthorizationReference != manifest.ExecutionAuthorizationId
            && a.AuthorizationReference != manifest.ProvisioningAuthorizationId && Token(a.Principal)
            && !new[] { "sa", "dbo", "guest", manifest.TenantA.ApplicationPrincipal, manifest.TenantB.ApplicationPrincipal,
                manifest.TenantA.ObserverPrincipal, manifest.TenantB.ObserverPrincipal }.Contains(a.Principal, StringComparer.OrdinalIgnoreCase)
            && a.NotBeforeUtc.Offset == TimeSpan.Zero && a.ExpiresAtUtc.Offset == TimeSpan.Zero && a.NotBeforeUtc < a.ExpiresAtUtc
            && a.ExpiresAtUtc - a.NotBeforeUtc <= TimeSpan.FromHours(24), "APPROVAL");
        Require(intent.RunId == index.RunId && report.RunId == index.RunId && observation.RunId == index.RunId && witness.RunId == index.RunId
            && intent.InputsSha256 == index.InputsSha256 && intent.AuthorizationReference == a.AuthorizationReference
            && intent.Origin == index.Origin && witness.Origin == index.Origin && report.Origin == index.Origin
            && !report.EvidenceAuthenticityVerified && !report.GlobalPhase7Closed, "ORIGIN_OR_INTENT");
        Require(observation.Principal == a.Principal && observation.AuthorizationReference == a.AuthorizationReference
            && observation.ServerName == manifest.Server.ExpectedServerName && observation.ProductVersion == manifest.Server.ProductVersion
            && observation.ObservedAtUtc.Offset == TimeSpan.Zero && observation.ObservedAtUtc >= a.NotBeforeUtc && observation.ObservedAtUtc < a.ExpiresAtUtc
            && observation.RemainingSessions >= 0 && observation.TransactionsA >= 0 && observation.TransactionsB >= 0
            && witness.ProcessId == start.ProcessId && witness.HasExited && witness.ProcessExitObservedAtUtc is { Offset.Ticks: 0 } exited
            && exited >= end.StoppedAtUtc && exited >= trx.Finish && exited <= observation.ObservedAtUtc
            && Token(witness.Inspector) && witness.AuthorizationReference == a.AuthorizationReference, "OBSERVATION");
        Require(Phase7SqlGlobalControls.WorkerWitness(witness with { Origin = Phase7SqlEvidenceOrigin.SqlServerLaboratory }, start, trx)
            && witness.WorkersOffThroughUtc >= observation.ObservedAtUtc, "WORKER_WITNESS");
        var final = new Dictionary<string, byte[]> { ["A"] = inputs["global/final-A.json"], ["B"] = inputs["global/final-B.json"] };
        foreach (var snapshot in final.Values) Phase7SqlGlobalControls.ValidatePhysicalSnapshot(snapshot);
        var conforms = Phase7SqlGlobalControls.Termination(observation, witness with { Origin = Phase7SqlEvidenceOrigin.SqlServerLaboratory }, manifest, start, end, trx)
            && Phase7SqlGlobalControls.Retained(observation, manifest, final)
            && end.Completed && end.CompletedCases.SequenceEqual(Phase7SqlExecutionCatalog.Cases.Select(c => c.Id));
        Require(report.ObservationsConform == conforms, "REPORT");
        return new(expectedHash, index, observation, witness, files, inputs);
    }

    // Re-check the embedded protocol even if the outer ZIP ledger has been recalculated.
    public static void VerifyArchived(IReadOnlyDictionary<string, byte[]> archive, Phase7SqlRunEvidence run)
    {
        var environmentArtifact = run.Artifacts.Single(a => a.Kind == "Environment");
        var environment = JsonNode.Parse(archive[environmentArtifact.RelativePath])!;
        var marker = environment["MonitorCaptureIndexSha256"]?.GetValue<string>();
        var entries = archive.Keys.Where(p => p.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase)).ToArray();
        if (entries.Length == 0 && marker is null) return; // Legacy Build remains supported.
        var expected = ContentPaths.Append("capture-index.json").Append("reference.json").Select(p => Prefix + p).ToHashSet(StringComparer.Ordinal);
        Require(entries.Length == expected.Count && entries.ToHashSet(StringComparer.Ordinal).SetEquals(expected), "ARCHIVED_FILES");
        var reference = Phase7SqlEvidenceFiles.Decode<Phase7SqlMonitorCaptureReference>(archive[Prefix + "reference.json"]);
        Require(marker == reference.IndexSha256, "ARCHIVED_PIN");
        var files = ContentPaths.Append("capture-index.json").ToDictionary(p => p, p => archive[Prefix + p], StringComparer.Ordinal);
        var inputs = Phase7SqlMonitorExecutor.InputPaths.ToDictionary(p => p, p => archive["evidence/" + p], StringComparer.Ordinal);
        var web = new JsonSerializerOptions(Phase7SqlEvidenceFiles.Json) { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        Phase7SqlEvidenceFiles.ValidateJson(archive["evidence/approval.json"]);
        var suiteApproval = JsonSerializer.Deserialize<Phase7SqlApproval>(archive["evidence/approval.json"], web)!;
        var capture = Validate(files, inputs, reference.IndexSha256, reference.Approval, suiteApproval);
        var manifest = JsonSerializer.Deserialize<Phase7SqlManifest>(inputs["manifest.json"], web)!;
        var start = Phase7SqlEvidenceFiles.Decode<Phase7SqlRuntimeStart>(inputs["global/start.json"]);
        var end = Phase7SqlEvidenceFiles.Decode<Phase7SqlRuntimeEnd>(inputs["global/end.json"]);
        var trx = Phase7SqlEvidenceTrx.Inspect(inputs["sql.trx"], false);
        Require(run.RunId == capture.Index.RunId && run.Origin == capture.Index.Origin
            && run.ManifestSha256 == Phase7SqlEvidenceFiles.Hash(inputs["manifest.json"])
            && run.TenantADatabase == manifest.TenantA.DatabaseName && run.TenantBDatabase == manifest.TenantB.DatabaseName
            && run.StartedAtUtc == trx.Start && run.FinishedAtUtc == trx.Finish && run.TrxBytes.SequenceEqual(inputs["sql.trx"]), "ARCHIVED_ORIGIN");
        var termination = Phase7SqlEvidenceFiles.Decode<Phase7SqlCapturedTermination>(archive[run.Artifacts.Single(a => a.Kind == "SessionTermination").RelativePath]);
        Require(Same(termination.Monitor, capture.Observation) && Same(termination.Witness, capture.Witness), "ARCHIVED_OBJECTS");
        Require(JsonNode.DeepEquals(environment["Monitor"], JsonSerializer.SerializeToNode(capture.Observation, Phase7SqlEvidenceFiles.Json))
            && JsonNode.DeepEquals(environment["Witness"], JsonSerializer.SerializeToNode(capture.Witness, Phase7SqlEvidenceFiles.Json))
            && JsonNode.DeepEquals(environment["Start"], JsonNode.Parse(inputs["global/start.json"]))
            && JsonNode.DeepEquals(environment["End"], JsonNode.Parse(inputs["global/end.json"]))
            && JsonNode.DeepEquals(environment["RunChecks"], JsonSerializer.SerializeToNode(run.RunChecks, Phase7SqlEvidenceFiles.Json)), "ARCHIVED_ENVIRONMENT");
        var terminated = Phase7SqlGlobalControls.Termination(capture.Observation, capture.Witness, manifest, start, end, trx);
        var retained = terminated && Phase7SqlGlobalControls.Retained(capture.Observation, manifest,
            new Dictionary<string, byte[]> { ["A"] = inputs["global/final-A.json"], ["B"] = inputs["global/final-B.json"] });
        var workers = end.WorkerExecutions == 0 && Phase7SqlGlobalControls.WorkerWitness(capture.Witness, start, trx);
        foreach (var check in new[] { ("NoOrphanSessions", terminated), ("DataPreserved", retained), ("WorkersAndRelayDisabled", workers) })
            Require(run.RunChecks.TryGetValue(check.Item1, out var status)
                && status == (check.Item2 ? Phase7SqlEvidenceStatus.Passed : Phase7SqlEvidenceStatus.Failed), "ARCHIVED_CHECKS");
    }
    private static bool Same<T>(T a, T b) => JsonNode.DeepEquals(JsonSerializer.SerializeToNode(a, Phase7SqlEvidenceFiles.Json), JsonSerializer.SerializeToNode(b, Phase7SqlEvidenceFiles.Json));
    private static bool Token(string value) => value is { Length: > 0 and <= 128 } && System.Text.RegularExpressions.Regex.IsMatch(value, "\\A[A-Za-z0-9_][A-Za-z0-9_.@\\\\-]*\\z");
    private static void Require(bool valid, string code) { if (!valid) throw Guard(code); }
    private static Phase7SqlGuardException Guard(string code) => new("PHASE7_SQL_MONITOR_CAPTURE_" + code);
}
