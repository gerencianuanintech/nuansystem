using System.IO.Compression;
using System.Text;
using System.Text.Json.Nodes;
using NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

[Trait("Category", "OfflineSafety")]
public sealed class Phase7SqlMonitorPackagingTests
{
    [Fact]
    public async Task Executor_capture_is_verified_archived_and_remains_offline()
    {
        var f = new Fixture(false);
        var result = await Phase7SqlMonitorExecutor.SimulateAsync(f.Inputs, f.Sample.Approval, f.Approval, f.Isolation,
            new Probe(f), new Clock(f.Sample.Finished.AddSeconds(3)));
        f.IndexHash = result.IndexSha256!;
        var package = f.Build();
        Assert.True(package.IntegrityVerified);
        Assert.Equal(Phase7SqlEvidenceOrigin.Offline, package.Report.Origin);
        Assert.False(package.ReadyForIndependentRunReview);
        Assert.False(package.EvidenceAuthenticityVerified);
        Assert.False(package.GlobalPhase7Closed);
        using var zip = ZipFile.OpenRead(package.ZipPath);
        foreach (var path in Fixture.CaptureFiles.Append("capture-index.json"))
        {
            using var input = zip.GetEntry("monitor-capture/" + path)!.Open(); using var bytes = new MemoryStream(); input.CopyTo(bytes);
            Assert.Equal(File.ReadAllBytes(Path.Combine(f.CaptureRoot, path)), bytes.ToArray());
        }
        Assert.True(Phase7SqlEvidencePackager.VerifyZip(package.ZipPath, expectedSha256: package.ArchiveSha256).IntegrityVerified);
    }

    [Theory]
    [InlineData("intent.json")][InlineData("observation.json")][InlineData("witness.json")]
    [InlineData("inputs-index.json")][InlineData("report.json")][InlineData("capture-index.json")]
    public void Any_capture_file_tamper_fails_before_package_creation(string path)
    {
        var f = new Fixture(); File.AppendAllText(Path.Combine(f.CaptureRoot, path), " ");
        Assert.Throws<Phase7SqlGuardException>(() => f.Build());
        Assert.False(Directory.Exists(f.Destination)); Assert.False(File.Exists(f.Destination + ".zip"));
    }

    [Theory]
    [InlineData("Missing")][InlineData("Extra")][InlineData("Duplicate")][InlineData("Traversal")]
    [InlineData("Origin")][InlineData("Run")][InlineData("Principal")][InlineData("Authorization")]
    [InlineData("Report")][InlineData("InputHash")][InlineData("UnknownField")][InlineData("DuplicateJson")]
    public void Rehashed_but_incoherent_capture_is_rejected(string change)
    {
        var f = new Fixture();
        switch (change)
        {
            case "Missing": File.Move(Path.Combine(f.CaptureRoot, "report.json"), Path.Combine(f.Root, "kept-report.json")); break;
            case "Extra": File.WriteAllText(Path.Combine(f.CaptureRoot, "unapproved.json"), "{}"); break;
            case "Duplicate": f.IndexChange = entries => entries.Append(entries[0]).ToArray(); break;
            case "Traversal": f.IndexChange = entries => entries.Select((e, i) => i == 0 ? e with { RelativePath = "../intent.json" } : e).ToArray(); break;
            case "Origin": f.Change("witness.json", n => n["Origin"] = (int)Phase7SqlEvidenceOrigin.SqlServerLaboratory); break;
            case "Run": f.Change("observation.json", n => n["RunId"] = Guid.NewGuid().ToString()); break;
            case "Principal": f.Change("observation.json", n => n["Principal"] = "wrong"); break;
            case "Authorization": f.Change("intent.json", n => n["AuthorizationReference"] = "another-approval"); break;
            case "Report": f.Change("report.json", n => n["ObservationsConform"] = false); break;
            case "InputHash": f.Change("inputs-index.json", n => n[0]!["Sha256"] = new string('D', 64)); break;
            case "UnknownField": f.Change("report.json", n => n["Unapproved"] = true); break;
            default: File.WriteAllText(Path.Combine(f.CaptureRoot, "report.json"), "{\"RunId\":1,\"RunId\":2}"); break;
        }
        f.Reindex();
        Assert.Throws<Phase7SqlGuardException>(() => f.Build());
        Assert.False(Directory.Exists(f.Destination));
    }

    [Theory]
    [InlineData("manifest.json")][InlineData("global/start.json")][InlineData("global/end.json")]
    [InlineData("sql.trx")][InlineData("global/final-A.json")][InlineData("global/final-B.json")]
    public void Capture_cannot_be_used_with_a_changed_source_run(string path)
    {
        var f = new Fixture(); File.AppendAllText(Path.Combine(f.Sample.Root, path), " ");
        Assert.Throws<Phase7SqlGuardException>(() => f.Build());
        Assert.False(Directory.Exists(f.Destination));
    }

    [Fact]
    public void Negative_capture_is_packaged_with_blockers_instead_of_upgrading_it()
    {
        var f = new Fixture();
        f.Change("observation.json", n => n["RemainingSessions"] = 1);
        f.Change("report.json", n => n["ObservationsConform"] = false); f.Reindex();
        var result = f.Build();
        Assert.True(result.IntegrityVerified); Assert.False(result.ReadyForIndependentRunReview);
        Assert.Equal(Phase7SqlEvidenceStatus.Failed, result.Report.RunChecks["NoOrphanSessions"]);
    }

    [Fact]
    public void Fabricated_lab_labeled_format_can_pass_integrity_but_never_proves_authenticity()
    {
        var f = new Fixture();
        foreach (var path in new[] { "intent.json", "witness.json", "report.json" })
            f.Change(path, n => n["Origin"] = (int)Phase7SqlEvidenceOrigin.SqlServerLaboratory);
        f.Origin = Phase7SqlEvidenceOrigin.SqlServerLaboratory; f.Reindex();
        var package = f.Build();
        Assert.True(package.IntegrityVerified); Assert.True(package.ReadyForIndependentRunReview);
        Assert.False(package.EvidenceAuthenticityVerified); Assert.False(package.GlobalPhase7Closed);
    }

    [Fact]
    public void Archive_reverification_rejects_semantic_tamper_even_after_outer_resealing()
    {
        var f = new Fixture(); var package = f.Build();
        using (var zip = ZipFile.Open(package.ZipPath, ZipArchiveMode.Update))
        {
            var file = zip.GetEntry("monitor-capture/report.json")!;
            byte[] changed;
            using (var read = file.Open()) { using var buffer = new MemoryStream(); read.CopyTo(buffer); var node = JsonNode.Parse(buffer.ToArray())!; node["ObservationsConform"] = false; changed = Encoding.UTF8.GetBytes(node.ToJsonString()); }
            using (var write = file.Open()) { write.SetLength(0); write.Write(changed); }
            var ledgerEntry = zip.GetEntry("seal-index.json")!; Phase7SqlEvidenceFile[] ledger;
            using (var read = ledgerEntry.Open()) { using var b = new MemoryStream(); read.CopyTo(b); ledger = Phase7SqlEvidenceFiles.Decode<Phase7SqlEvidenceFile[]>(b.ToArray()); }
            ledger = ledger.Select(e => e.RelativePath == file.FullName ? e with { Length = changed.Length, Sha256 = Phase7SqlEvidenceFiles.Hash(changed) } : e).ToArray();
            using var writeLedger = ledgerEntry.Open(); writeLedger.SetLength(0); writeLedger.Write(Phase7SqlEvidenceFiles.Encode(ledger));
        }
        Assert.False(Phase7SqlEvidencePackager.VerifyZip(package.ZipPath).IntegrityVerified);
    }

    [Fact]
    public void Wrong_external_pin_and_cancelled_requests_preserve_capture_and_existing_output()
    {
        var f = new Fixture(); var original = File.ReadAllBytes(Path.Combine(f.CaptureRoot, "capture-index.json"));
        Assert.Throws<Phase7SqlGuardException>(() => f.Build(new string('A', 64)));
        using var cancelled = new CancellationTokenSource(); cancelled.Cancel();
        Assert.ThrowsAny<OperationCanceledException>(() => f.Build(token: cancelled.Token));
        Assert.Equal(original, File.ReadAllBytes(Path.Combine(f.CaptureRoot, "capture-index.json")));
        Directory.CreateDirectory(f.Destination); File.WriteAllText(Path.Combine(f.Destination, "keep.txt"), "preserved");
        Assert.Throws<Phase7SqlGuardException>(() => f.Build());
        Assert.Equal("preserved", File.ReadAllText(Path.Combine(f.Destination, "keep.txt")));
    }

    [Theory]
    [InlineData("StripCapture")][InlineData("Termination")][InlineData("Environment")][InlineData("UpgradeOfflineChecks")]
    public void Fully_resealed_package_cannot_contradict_its_embedded_capture(string change)
    {
        var f = new Fixture(); var package = f.Build(); var payload = new Dictionary<string, byte[]>(StringComparer.Ordinal);
        using (var zip = ZipFile.OpenRead(package.ZipPath)) foreach (var entry in zip.Entries)
        { using var read = entry.Open(); using var memory = new MemoryStream(); read.CopyTo(memory); payload.Add(entry.FullName, memory.ToArray()); }
        var report = Phase7SqlEvidenceFiles.Decode<Phase7SqlRunEvidence>(payload["run-report.json"]);
        void ChangeArtifact(string kind, Action<JsonNode> action)
        {
            var path = report.Artifacts.Single(a => a.Kind == kind).RelativePath;
            var node = JsonNode.Parse(payload[path])!; action(node); payload[path] = Encoding.UTF8.GetBytes(node.ToJsonString());
        }
        if (change == "StripCapture") foreach (var path in payload.Keys.Where(p => p.StartsWith("monitor-capture/", StringComparison.Ordinal)).ToArray()) payload.Remove(path);
        if (change == "Termination") ChangeArtifact("SessionTermination", n => n["Monitor"]!["Principal"] = "contradiction");
        if (change == "Environment") ChangeArtifact("Environment", n => n["Monitor"]!["Principal"] = "contradiction");
        if (change == "UpgradeOfflineChecks")
        {
            report = report with { RunChecks = report.RunChecks.ToDictionary(p => p.Key, p => p.Key == "NoOrphanSessions" ? Phase7SqlEvidenceStatus.Passed : p.Value) };
            ChangeArtifact("Environment", n => n["RunChecks"]!["NoOrphanSessions"] = (int)Phase7SqlEvidenceStatus.Passed);
        }
        report = report with { Artifacts = report.Artifacts.Select(a => a with { Sha256 = Phase7SqlEvidenceFiles.Hash(payload[a.RelativePath]) }).ToArray() };
        payload["run-report.json"] = Phase7SqlEvidenceFiles.Encode(report);
        payload["seal-index.json"] = Phase7SqlEvidenceFiles.Encode(payload.Where(p => p.Key != "seal-index.json")
            .Select(p => new Phase7SqlEvidenceFile(p.Key, p.Value.LongLength, Phase7SqlEvidenceFiles.Hash(p.Value))).ToArray());
        var modified = package.ZipPath + ".resealed.zip";
        using (var stream = new FileStream(modified, FileMode.CreateNew, FileAccess.Write))
        using (var zip = new ZipArchive(stream, ZipArchiveMode.Create)) foreach (var file in payload)
        { var entry = zip.CreateEntry(file.Key); using var write = entry.Open(); write.Write(file.Value); }
        Assert.False(Phase7SqlEvidencePackager.VerifyZip(modified).IntegrityVerified);
    }

    private sealed class Fixture
    {
        public static readonly string[] CaptureFiles = ["intent.json", "observation.json", "witness.json", "inputs-index.json", "report.json"];
        public string Root { get; } = Phase7SqlEvidenceFilesTests.Root();
        public Phase7SqlSyntheticPackage Sample { get; }
        public string CaptureRoot => Path.Combine(Root, "capture");
        public string Destination => Path.Combine(Root, "package");
        public Phase7SqlMonitorInputs Inputs { get; }
        public Phase7SqlControlledMonitorApproval Approval { get; }
        public Phase7SqlMonitorIsolation Isolation { get; }
        public string IndexHash { get; set; } = "";
        public Phase7SqlEvidenceOrigin Origin { get; set; } = Phase7SqlEvidenceOrigin.Offline;
        public Func<Phase7SqlMonitorCaptureFile[], Phase7SqlMonitorCaptureFile[]>? IndexChange { get; set; }
        public Fixture(bool fabricate = true)
        {
            Sample = new(Path.Combine(Root, "synthetic"));
            Inputs = new(Sample.Root, CaptureRoot, Phase7SqlMonitorExecutor.InputPaths.ToDictionary(p => p, p => Phase7SqlEvidenceFiles.Hash(File.ReadAllBytes(Path.Combine(Sample.Root, p)))));
            Approval = new(Sample.MonitorApproval, Phase7SqlMonitorExecutor.InputFingerprint(Inputs), 30, 60, true);
            Isolation = new(Sample.Witness.Inspector, Sample.Witness.AuthorizationReference, Sample.Witness.WorkersOffFromUtc,
                Sample.Witness.WorkersOffThroughUtc, Sample.Witness.DisabledComponents);
            if (!fabricate) return;
            // Fabricated format only; never real SQL evidence.
            Write("intent.json", new { Sample.Manifest.RunId, Approval.InputsSha256, Approval.Monitor.AuthorizationReference, Origin = Phase7SqlEvidenceOrigin.Offline });
            Write("observation.json", Sample.Monitor); Write("witness.json", Sample.Witness with { Origin = Phase7SqlEvidenceOrigin.Offline });
            Write("inputs-index.json", Phase7SqlMonitorExecutor.InputPaths.Select(p => new { Path = p, Sha256 = Inputs.Sha256[p] }).ToArray());
            Write("report.json", new { Sample.Manifest.RunId, Origin = Phase7SqlEvidenceOrigin.Offline, ObservationsConform = true, EvidenceAuthenticityVerified = false, GlobalPhase7Closed = false });
            Reindex();
        }
        private void Write<T>(string path, T value) => Phase7SqlEvidenceFiles.Write(CaptureRoot, path, Phase7SqlEvidenceFiles.Encode(value));
        public void Change(string path, Action<JsonNode> action) { var full = Path.Combine(CaptureRoot, path); var node = JsonNode.Parse(File.ReadAllBytes(full))!; action(node); File.WriteAllBytes(full, Encoding.UTF8.GetBytes(node.ToJsonString())); }
        public void Reindex()
        {
            var entries = CaptureFiles.Where(p => File.Exists(Path.Combine(CaptureRoot, p))).Select(p => { var b = File.ReadAllBytes(Path.Combine(CaptureRoot, p)); return new Phase7SqlMonitorCaptureFile(p, b.Length, Phase7SqlEvidenceFiles.Hash(b)); }).ToArray();
            var bytes = Phase7SqlEvidenceFiles.Encode(new Phase7SqlMonitorCaptureIndex(Sample.Manifest.RunId, Origin,
                Approval.InputsSha256, IndexChange?.Invoke(entries) ?? entries));
            File.WriteAllBytes(Path.Combine(CaptureRoot, "capture-index.json"), bytes); IndexHash = Phase7SqlEvidenceFiles.Hash(bytes);
        }
        public Phase7SqlPackageResult Build(string? pin = null, CancellationToken token = default) =>
            Phase7SqlEvidencePackager.BuildFromMonitorCapture(Sample.Root, Destination, Sample.Approval, Sample.Candidate, CaptureRoot, pin ?? IndexHash, Approval, token);
    }
    private sealed class Clock(DateTimeOffset now) : TimeProvider { public override DateTimeOffset GetUtcNow() => now; }
    private sealed class Probe(Fixture f) : IPhase7SqlMonitorProbe
    {
        public Task<Phase7SqlMonitorProcessCapture> WitnessAsync(Phase7SqlMonitorPlan plan, Phase7SqlMonitorIsolation isolation, CancellationToken token) => Task.FromResult(new Phase7SqlMonitorProcessCapture(f.Sample.Witness, f.Sample.Start.StartedAtUtc.AddSeconds(-1)));
        public Task<Phase7SqlMonitorObservation> ReadAsync(Phase7SqlMonitorPlan plan, Phase7SqlMonitorApproval approval, DateTimeOffset now, int timeout, CancellationToken token) => Task.FromResult(f.Sample.Monitor);
        public Task CloseAsync() => Task.CompletedTask;
    }
}
