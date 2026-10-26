using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

// Fabricated protocol examples for unit tests only. Origin labels are not proof of live execution.
internal static class Phase7SqlSyntheticEvidence
{
    public static IReadOnlyList<Phase7SqlRunEvidence> Pair() =>
    [
        Run(Guid.Parse("12345678-1234-1234-1234-123456789abc"), new(2026, 10, 24, 12, 0, 0, TimeSpan.Zero)),
        Run(Guid.Parse("abcdef12-1234-1234-1234-123456789abc"), new(2026, 10, 24, 13, 0, 0, TimeSpan.Zero))
    ];

    private static Phase7SqlRunEvidence Run(Guid run, DateTimeOffset start)
    {
        XNamespace ns = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";
        var results = new XElement(ns + "Results");
        var definitions = new XElement(ns + "TestDefinitions");
        foreach (var spec in Phase7SqlExecutionCatalog.Cases)
        {
            var testId = Guid.NewGuid().ToString("D");
            var executionId = Guid.NewGuid().ToString("D");
            results.Add(new XElement(ns + "UnitTestResult", new XAttribute("testId", testId), new XAttribute("executionId", executionId),
                new XAttribute("testName", spec.TestName), new XAttribute("outcome", "Passed")));
            definitions.Add(new XElement(ns + "UnitTest", new XAttribute("id", testId), new XAttribute("name", spec.TestName),
                new XElement(ns + "Execution", new XAttribute("id", executionId)),
                new XElement(ns + "TestMethod", new XAttribute("className", Phase7SqlExecutionCatalog.ExpectedTestClass), new XAttribute("name", spec.Method))));
        }
        var counters = new XElement(ns + "Counters");
        foreach (var key in new[] { "total", "executed", "passed" }) counters.Add(new XAttribute(key, Phase7SqlExecutionCatalog.Cases.Count));
        foreach (var key in new[] { "failed", "error", "timeout", "aborted", "inconclusive", "passedButRunAborted", "notRunnable", "notExecuted", "disconnected", "warning", "completed", "inProgress", "pending" }) counters.Add(new XAttribute(key, 0));
        var xml = new XDocument(new XElement(ns + "TestRun", new XAttribute("id", Guid.NewGuid()),
            new XElement(ns + "Times", new XAttribute("start", start.ToString("O")), new XAttribute("finish", start.AddMinutes(1).ToString("O"))),
            results, definitions, new XElement(ns + "ResultSummary", new XAttribute("outcome", "Completed"), counters))).ToString();
        var manifest = Digest(run.ToString("N"));
        var artifacts = Phase7SqlExecutionCatalog.ArtifactKinds.Select(kind => new Phase7SqlArtifactEvidence(kind, kind.ToLowerInvariant() + (kind == "Trx" ? ".trx" : ".json"), kind switch
        {
            "Manifest" => manifest, "SourceInventory" => new('A', 64), "Binaries" => new('B', 64),
            "CaseCatalog" => Phase7SqlExecutionCatalog.Sha256, "Trx" => Digest(xml), _ => new('C', 64)
        })).ToArray();
        return new(run, $"NuanSystem_Phase7_{run:N}_A", $"NuanSystem_Phase7_{run:N}_B", manifest, new('A', 64), new('B', 64),
            Phase7SqlExecutionCatalog.Sha256, start, start.AddMinutes(1), Phase7SqlEvidenceOrigin.SqlServerLaboratory,
            Phase7SqlExecutionCatalog.RunChecks.ToDictionary(k => k, _ => Phase7SqlEvidenceStatus.Passed),
            Phase7SqlExecutionCatalog.Cases.Select(c => new Phase7SqlCaseEvidence(c.Id, c.Checks.ToDictionary(k => k, _ => Phase7SqlEvidenceStatus.Passed))).ToArray(), artifacts, Encoding.UTF8.GetBytes(xml), 0, false, false);
    }

    public static Phase7SqlRunEvidence WithTrx(Phase7SqlRunEvidence run, string xml) => run with
    {
        TrxBytes = Encoding.UTF8.GetBytes(xml), Artifacts = run.Artifacts.Select(a => a.Kind == "Trx" ? a with { Sha256 = Digest(xml) } : a).ToArray()
    };
    public static Phase7SqlRunEvidence WithBytes(Phase7SqlRunEvidence run, byte[] bytes) => run with
    {
        TrxBytes = bytes, Artifacts = run.Artifacts.Select(a => a.Kind == "Trx" ? a with { Sha256 = Convert.ToHexString(SHA256.HashData(bytes)) } : a).ToArray()
    };
    private static string Digest(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
