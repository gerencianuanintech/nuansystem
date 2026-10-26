using System.Globalization;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

// Pure assessment of reported evidence, NOT proof of SQL execution or an authorization capability.
// No file access, environment reads, secret resolution, process launches or connections.
internal static class Phase7SqlClosureEvaluator
{
    internal static IReadOnlyList<string> ValidateSingleRun(Phase7SqlRunEvidence run)
    {
        var reasons=new List<string>();ValidateRun(run,"",reasons);return reasons.AsReadOnly();
    }
    public static Phase7SqlClosureAssessment Assess(IReadOnlyList<Phase7SqlRunEvidence> runs)
    {
        var reasons = new List<string>();
        if (runs.Count != 2) return new(["TWO_RUNS_REQUIRED"]);
        if (runs.Any(r => r is null)) return new(["REPORT_INVALID"]);
        var trxIds = new Guid[2];
        for (var i = 0; i < 2; i++) trxIds[i] = ValidateRun(runs[i], $"Run{i + 1}.", reasons);
        var first = runs[0];
        var second = runs[1];
        if (first.RunId == second.RunId || new[] { first.TenantADatabase, first.TenantBDatabase, second.TenantADatabase, second.TenantBDatabase }.Distinct(StringComparer.OrdinalIgnoreCase).Count() != 4)
            reasons.Add("FRESH_RUNS_AND_DESTINATIONS_REQUIRED");
        if (!Equal(first.SourceSha256, second.SourceSha256) || !Equal(first.BinarySha256, second.BinarySha256)) reasons.Add("SAME_CANDIDATE_REQUIRED");
        if (Equal(first.ManifestSha256, second.ManifestSha256)) reasons.Add("INDEPENDENT_MANIFESTS_REQUIRED");
        if (trxIds[0] != Guid.Empty && trxIds[0] == trxIds[1]) reasons.Add("INDEPENDENT_TRX_RUNS_REQUIRED");
        if (second.StartedAtUtc < first.FinishedAtUtc) reasons.Add("ORDERED_RUNS_REQUIRED");
        return new(reasons);
    }

    private static Guid ValidateRun(Phase7SqlRunEvidence run, string prefix, List<string> reasons)
    {
        var trxBytes = run.TrxBytes is { Length: > 0 and <= 1_048_576 } ? run.TrxBytes.ToArray() : [];
        void Require(bool condition, string code) { if (!condition) reasons.Add(prefix + code); }
        Require(run.RunId != Guid.Empty && run.TenantADatabase == $"NuanSystem_Phase7_{run.RunId:N}_A"
            && run.TenantBDatabase == $"NuanSystem_Phase7_{run.RunId:N}_B", "DESTINATIONS_INVALID");
        Require(run.StartedAtUtc.Offset == TimeSpan.Zero && run.FinishedAtUtc.Offset == TimeSpan.Zero
            && run.StartedAtUtc != default && run.FinishedAtUtc > run.StartedAtUtc, "TIMES_INVALID");
        Require(run.Origin == Phase7SqlEvidenceOrigin.SqlServerLaboratory, "SQL_LAB_EVIDENCE_REQUIRED");
        Require(run.ExternalBoundaryAttempts == 0 && !run.SapExecuted && !run.WorkersExecuted, "EXTERNAL_ACTIVITY_REJECTED");
        Require(Hash(run.ManifestSha256) && Hash(run.SourceSha256) && Hash(run.BinarySha256) && Equal(run.CatalogSha256, Phase7SqlExecutionCatalog.Sha256), "FINGERPRINT_INVALID");
        Require(CompleteChecks(run.RunChecks, Phase7SqlExecutionCatalog.RunChecks), "RUN_CHECKS_INCOMPLETE");
        Require(run.Cases is not null && run.Cases.All(c => c is not null) && run.Cases.Count == Phase7SqlExecutionCatalog.Cases.Count
            && run.Cases.Select(c => c.Id).Distinct(StringComparer.Ordinal).Count() == run.Cases.Count
            && Phase7SqlExecutionCatalog.Cases.All(spec => run.Cases.Count(c => c.Id == spec.Id && CompleteChecks(c.Checks, spec.Checks)) == 1), "CASE_COVERAGE_INCOMPLETE");
        if (run.Artifacts is null || run.Artifacts.Any(a => a is null)) Require(false, "ARTIFACTS_INCOMPLETE");
        else
        {
            Require(run.Artifacts.Count == Phase7SqlExecutionCatalog.ArtifactKinds.Count
                && run.Artifacts.Select(a => a.RelativePath).Distinct(StringComparer.OrdinalIgnoreCase).Count() == run.Artifacts.Count
                && Phase7SqlExecutionCatalog.ArtifactKinds.All(kind => run.Artifacts.Count(a => a.Kind == kind && Hash(a.Sha256) && SafePath(a.RelativePath)) == 1), "ARTIFACTS_INCOMPLETE");
            bool Matches(string kind, string hash) => run.Artifacts.Count(a => a.Kind == kind && Equal(a.Sha256, hash)) == 1;
            Require(Matches("Manifest", run.ManifestSha256) && Matches("SourceInventory", run.SourceSha256)
                && Matches("Binaries", run.BinarySha256) && Matches("CaseCatalog", run.CatalogSha256)
                && Matches("Trx", Convert.ToHexString(SHA256.HashData(trxBytes))), "ARTIFACT_BINDING_INVALID");
        }
        var validTrx = ValidTrx(trxBytes, run.StartedAtUtc, run.FinishedAtUtc, out var trxId);
        Require(validTrx, "TRX_REJECTED");
        return validTrx ? trxId : Guid.Empty;
    }

    private static bool CompleteChecks(IReadOnlyDictionary<string, Phase7SqlEvidenceStatus>? checks, IReadOnlyList<string> expected) =>
        checks is not null && checks.Count == expected.Count && expected.All(key => checks.TryGetValue(key, out var status) && status == Phase7SqlEvidenceStatus.Passed);
    private static bool Hash(string? value) => value is not null && Regex.IsMatch(value, "\\A[0-9A-Fa-f]{64}\\z") && value.Any(c => c != '0');
    private static bool Equal(string? a, string? b) => Hash(a) && Hash(b) && string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    private static bool SafePath(string? path) => path is not null && Regex.IsMatch(path, "\\A[A-Za-z0-9][A-Za-z0-9_./-]{0,159}\\z")
        && path.Split('/').All(part => part.Length > 0 && part is not ("." or ".."));

    private static bool ValidTrx(byte[] xml, DateTimeOffset started, DateTimeOffset finished, out Guid trxId)
    {
        trxId = Guid.Empty;
        if (xml.Length == 0 || xml.Length > 1_048_576) return false;
        try
        {
            using var input = new MemoryStream(xml, writable: false);
            using var reader = XmlReader.Create(input, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null, MaxCharactersInDocument = 1_048_576 });
            var doc = XDocument.Load(reader);
            XNamespace ns = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";
            var root = doc.Root;
            if (root?.Name != ns + "TestRun" || root.Elements(ns + "Results").Count() != 1 || root.Elements(ns + "TestDefinitions").Count() != 1
                || root.Elements(ns + "ResultSummary").Count() != 1 || doc.Descendants(ns + "ErrorInfo").Any()) return false;
            if (!Guid.TryParse((string?)root.Attribute("id"), out trxId) || trxId == Guid.Empty || root.Elements(ns + "Times").Count() != 1) return false;
            var times = root.Element(ns + "Times")!;
            bool MatchesTime(string key, DateTimeOffset reported)
            {
                var text = (string?)times.Attribute(key);
                return text is not null && Regex.IsMatch(text, "(Z|[+-][0-9]{2}:[0-9]{2})\\z")
                    && DateTimeOffset.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed) && parsed == reported;
            }
            if (!MatchesTime("start", started) || !MatchesTime("finish", finished)) return false;
            var summary = root.Element(ns + "ResultSummary")!;
            if ((string?)summary.Attribute("outcome") != "Completed" || summary.Elements(ns + "Counters").Count() != 1) return false;
            var counters = summary.Element(ns + "Counters")!;
            int Count(string key) => int.TryParse((string?)counters.Attribute(key), NumberStyles.None, CultureInfo.InvariantCulture, out var value) ? value : -1;
            var expected = Phase7SqlExecutionCatalog.Cases;
            if (new[] { "total", "executed", "passed" }.Any(k => Count(k) != expected.Count)
                || new[] { "failed", "error", "timeout", "aborted", "inconclusive", "passedButRunAborted", "notRunnable", "notExecuted", "disconnected", "warning", "completed", "inProgress", "pending" }.Any(k => Count(k) != 0)) return false;
            if (doc.Descendants(ns + "RunInfo").Any(r => (string?)r.Attribute("outcome") is not ("Passed" or "Completed"))) return false;
            var results = root.Element(ns + "Results")!.Elements().ToArray();
            var definitions = root.Element(ns + "TestDefinitions")!.Elements().ToArray();
            if (results.Length != expected.Count || definitions.Length != expected.Count || results.Any(r => r.Name != ns + "UnitTestResult")
                || definitions.Any(d => d.Name != ns + "UnitTest")) return false;
            var testIds = new HashSet<Guid>();
            var executionIds = new HashSet<Guid>();
            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (var result in results)
            {
                var name = (string?)result.Attribute("testName");
                var spec = expected.SingleOrDefault(c => c.TestName == name);
                if (spec is null || !names.Add(name!) || (string?)result.Attribute("outcome") != "Passed"
                    || !Guid.TryParse((string?)result.Attribute("testId"), out var id) || id == Guid.Empty || !testIds.Add(id)
                    || !Guid.TryParse((string?)result.Attribute("executionId"), out var execution) || execution == Guid.Empty || !executionIds.Add(execution)) return false;
                var matching = definitions.Where(d => Guid.TryParse((string?)d.Attribute("id"), out var definitionId) && definitionId == id).ToArray();
                if (matching.Length != 1) return false;
                var definition = matching[0];
                if (definition.Elements(ns + "Execution").Count() != 1 || definition.Elements(ns + "TestMethod").Count() != 1) return false;
                var method = definition.Element(ns + "TestMethod")!;
                if (!Guid.TryParse((string?)definition.Element(ns + "Execution")!.Attribute("id"), out var linkedExecution) || linkedExecution != execution
                    || (string?)method.Attribute("className") != Phase7SqlExecutionCatalog.ExpectedTestClass
                    || (string?)method.Attribute("name") != spec.Method) return false;
            }
            return true;
        }
        catch (XmlException) { return false; }
    }
}
