using NuanSystem.Application.Tests.Infrastructure.Phase7Sql;
using System.Xml.Linq;
using System.Text;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

[Trait("Category", "OfflineSafety")]
public sealed class Phase7SqlClosureTests
{
    [Fact]
    public void Complete_synthetic_reports_only_qualify_for_independent_review_never_close_the_phase()
    {
        var assessment = Phase7SqlClosureEvaluator.Assess(Phase7SqlSyntheticEvidence.Pair());
        Assert.True(assessment.ReadyForIndependentPilotReview);
        Assert.Empty(assessment.Reasons);
        Assert.False(assessment.GlobalPhase7Closed);
        Assert.False(assessment.EvidenceAuthenticityVerified);
    }

    [Fact]
    public void Preparation_without_real_run_evidence_remains_not_ready()
    {
        var assessment = Phase7SqlClosureEvaluator.Assess([]);
        Assert.False(assessment.ReadyForIndependentPilotReview);
        Assert.Contains("TWO_RUNS_REQUIRED", assessment.Reasons);
    }

    [Fact]
    public void The_same_trx_cannot_be_relabelled_as_a_second_run()
    {
        var runs = Phase7SqlSyntheticEvidence.Pair().ToArray();
        runs[1] = Phase7SqlSyntheticEvidence.WithBytes(runs[1], runs[0].TrxBytes);
        Assert.False(Phase7SqlClosureEvaluator.Assess(runs).ReadyForIndependentPilotReview);
    }

    [Fact]
    public void Reported_times_must_match_the_trx_execution()
    {
        var runs = Phase7SqlSyntheticEvidence.Pair().ToArray();
        runs[0] = runs[0] with { FinishedAtUtc = runs[0].FinishedAtUtc.AddSeconds(1) };
        Assert.False(Phase7SqlClosureEvaluator.Assess(runs).ReadyForIndependentPilotReview);
    }

    [Theory]
    [InlineData("zero")]
    [InlineData("missing")]
    [InlineData("extra")]
    [InlineData("skipped")]
    [InlineData("failed")]
    [InlineData("inconclusive")]
    [InlineData("duplicateName")]
    [InlineData("duplicateTestId")]
    [InlineData("duplicateExecutionId")]
    [InlineData("unknownClass")]
    [InlineData("wrongMethod")]
    [InlineData("wrongCounter")]
    [InlineData("failedSummary")]
    [InlineData("wrongNamespace")]
    [InlineData("errorInfo")]
    [InlineData("truncated")]
    [InlineData("dtd")]
    [InlineData("oversized")]
    public void Incomplete_or_inconsistent_trx_cannot_qualify_even_with_reported_passed_checks(string mutation)
    {
        var runs = Phase7SqlSyntheticEvidence.Pair().ToArray();
        XNamespace ns = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";
        using var input = new MemoryStream(runs[0].TrxBytes);
        var doc = XDocument.Load(input);
        var root = doc.Root!;
        var results = root.Element(ns + "Results")!;
        var first = results.Elements().First();
        var second = results.Elements().Skip(1).First();
        var summary = root.Element(ns + "ResultSummary")!;
        switch (mutation)
        {
            case "zero": results.RemoveNodes(); foreach (var key in new[] { "total", "executed", "passed" }) summary.Element(ns + "Counters")!.SetAttributeValue(key, 0); break;
            case "missing": results.Elements().Last().Remove(); break;
            case "extra": results.Add(new XElement(first)); break;
            case "skipped": first.SetAttributeValue("outcome", "NotExecuted"); break;
            case "failed": first.SetAttributeValue("outcome", "Failed"); break;
            case "inconclusive": first.SetAttributeValue("outcome", "Inconclusive"); break;
            case "duplicateName": second.SetAttributeValue("testName", first.Attribute("testName")!.Value); break;
            case "duplicateTestId": second.SetAttributeValue("testId", first.Attribute("testId")!.Value); break;
            case "duplicateExecutionId": second.SetAttributeValue("executionId", first.Attribute("executionId")!.Value); break;
            case "unknownClass": root.Element(ns + "TestDefinitions")!.Elements().First().Element(ns + "TestMethod")!.SetAttributeValue("className", "Unrelated.PassingTests"); break;
            case "wrongMethod": root.Element(ns + "TestDefinitions")!.Elements().First().Element(ns + "TestMethod")!.SetAttributeValue("name", "Different"); break;
            case "wrongCounter": summary.Element(ns + "Counters")!.SetAttributeValue("passed", 0); break;
            case "failedSummary": summary.SetAttributeValue("outcome", "Failed"); break;
            case "wrongNamespace": break;
            case "errorInfo": first.Add(new XElement(ns + "Output", new XElement(ns + "ErrorInfo", new XElement(ns + "Message", "s3cr3t-sample")))); break;
        }
        var xml = mutation switch { "truncated" => "<TestRun", "dtd" => "<!DOCTYPE TestRun [<!ENTITY test 'ignored'>]>" + doc,
            "wrongNamespace" => doc.ToString().Replace(ns.NamespaceName, "urn:unrelated", StringComparison.Ordinal), _ => doc.ToString() };
        runs[0] = mutation == "oversized" ? Phase7SqlSyntheticEvidence.WithBytes(runs[0], new byte[1_048_577]) : Phase7SqlSyntheticEvidence.WithTrx(runs[0], xml);
        var assessment = Phase7SqlClosureEvaluator.Assess(runs);
        Assert.False(assessment.ReadyForIndependentPilotReview);
        Assert.DoesNotContain("s3cr3t-sample", string.Join(',', assessment.Reasons));
    }

    [Theory]
    [InlineData("offline")]
    [InlineData("sameRun")]
    [InlineData("sameDatabase")]
    [InlineData("sourceChanged")]
    [InlineData("binaryChanged")]
    [InlineData("sameManifest")]
    [InlineData("missingCases")]
    [InlineData("duplicateCases")]
    [InlineData("subcaseSkipped")]
    [InlineData("subcaseMissing")]
    [InlineData("runCheckNotRun")]
    [InlineData("missingArtifact")]
    [InlineData("pathTraversal")]
    [InlineData("wrongArtifactHash")]
    [InlineData("zeroHash")]
    [InlineData("wrongCatalog")]
    [InlineData("externalAttempt")]
    [InlineData("sapExecuted")]
    [InlineData("workersExecuted")]
    [InlineData("notUtc")]
    [InlineData("overlappingRuns")]
    [InlineData("negativeAttempts")]
    public void Closure_requires_complete_consistent_independent_lab_reports(string mutation)
    {
        var runs = Phase7SqlSyntheticEvidence.Pair().ToArray();
        var run = runs[1];
        switch (mutation)
        {
            case "offline": run = run with { Origin = Phase7SqlEvidenceOrigin.Offline }; break;
            case "sameRun": run = run with { RunId = runs[0].RunId }; break;
            case "sameDatabase": run = run with { TenantADatabase = runs[0].TenantADatabase }; break;
            case "sourceChanged": run = run with { SourceSha256 = new('D', 64) }; break;
            case "binaryChanged": run = run with { BinarySha256 = new('D', 64) }; break;
            case "sameManifest": run = run with { ManifestSha256 = runs[0].ManifestSha256 }; break;
            case "missingCases": run = run with { Cases = run.Cases.Skip(1).ToArray() }; break;
            case "duplicateCases": run = run with { Cases = run.Cases.Take(run.Cases.Count - 1).Append(run.Cases[0]).ToArray() }; break;
            case "subcaseSkipped": case "subcaseMissing":
                var cases = run.Cases.ToArray();
                var checks = cases[0].Checks.ToDictionary(p => p.Key, p => p.Value);
                if (mutation == "subcaseSkipped") checks["Pilot"] = Phase7SqlEvidenceStatus.Skipped; else checks.Remove("Pilot");
                cases[0] = cases[0] with { Checks = checks };
                run = run with { Cases = cases }; break;
            case "runCheckNotRun": var runChecks = run.RunChecks.ToDictionary(p => p.Key, p => p.Value); runChecks["NoOrphanSessions"] = Phase7SqlEvidenceStatus.NotRun; run = run with { RunChecks = runChecks }; break;
            case "missingArtifact": run = run with { Artifacts = run.Artifacts.Skip(1).ToArray() }; break;
            case "pathTraversal": run = run with { Artifacts = run.Artifacts.Select(a => a.Kind == "Trx" ? a with { RelativePath = "../outside.trx" } : a).ToArray() }; break;
            case "wrongArtifactHash": run = run with { Artifacts = run.Artifacts.Select(a => a.Kind == "Trx" ? a with { Sha256 = new('E', 64) } : a).ToArray() }; break;
            case "zeroHash": run = run with { Artifacts = run.Artifacts.Select(a => a.Kind == "Sessions" ? a with { Sha256 = new('0', 64) } : a).ToArray() }; break;
            case "wrongCatalog": run = run with { CatalogSha256 = new('F', 64) }; break;
            case "externalAttempt": run = run with { ExternalBoundaryAttempts = 1 }; break;
            case "negativeAttempts": run = run with { ExternalBoundaryAttempts = -1 }; break;
            case "sapExecuted": run = run with { SapExecuted = true }; break;
            case "workersExecuted": run = run with { WorkersExecuted = true }; break;
            case "notUtc": run = run with { StartedAtUtc = run.StartedAtUtc.ToOffset(TimeSpan.FromHours(-5)) }; break;
            case "overlappingRuns": run = run with { StartedAtUtc = runs[0].StartedAtUtc }; break;
        }
        runs[1] = run;
        Assert.False(Phase7SqlClosureEvaluator.Assess(runs).ReadyForIndependentPilotReview);
    }

    [Fact]
    public void Raw_trx_bom_is_preserved_in_the_artifact_digest()
    {
        var runs = Phase7SqlSyntheticEvidence.Pair().ToArray();
        var bytes = new byte[] { 0xEF, 0xBB, 0xBF }.Concat(runs[0].TrxBytes).ToArray();
        runs[0] = Phase7SqlSyntheticEvidence.WithBytes(runs[0], bytes);
        Assert.True(Phase7SqlClosureEvaluator.Assess(runs).ReadyForIndependentPilotReview);
    }

    [Fact]
    public void Distinct_files_with_the_same_trx_run_id_are_not_independent_executions()
    {
        var runs = Phase7SqlSyntheticEvidence.Pair().ToArray();
        var first = XDocument.Parse(Encoding.UTF8.GetString(runs[0].TrxBytes));
        var second = XDocument.Parse(Encoding.UTF8.GetString(runs[1].TrxBytes));
        second.Root!.SetAttributeValue("id", first.Root!.Attribute("id")!.Value);
        runs[1] = Phase7SqlSyntheticEvidence.WithTrx(runs[1], second.ToString());
        Assert.Contains("INDEPENDENT_TRX_RUNS_REQUIRED", Phase7SqlClosureEvaluator.Assess(runs).Reasons);
    }
}
