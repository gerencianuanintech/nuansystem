using System.Text;
using System.Xml.Linq;
using NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

[Trait("Category","OfflineSafety")]
public sealed class Phase7SqlEvidenceCoherenceTests
{
    private static readonly Lazy<Phase7SqlSyntheticPackage> Data=new(()=>new(Phase7SqlEvidenceFilesTests.Root()));
    private static Phase7SqlSyntheticPackage Fixture=>Data.Value;
    private static List<Phase7SqlEvidenceBundleFile> Diagnostics()=>Directory.GetFiles(Fixture.Root,"*.json",SearchOption.AllDirectories)
        .Where(p=>Path.GetFileName(p) is "concurrent.json" or "repetition.json" or "owner-close.json" or "internal-rollback.json" or "fault-C.json" or "fault-U.json" or "fault-D.json")
        .Select(p=>new Phase7SqlEvidenceBundleFile(Path.GetRelativePath(Fixture.Root,p).Replace('\\','/'),File.ReadAllBytes(p))).ToList();

    [Theory]
    [InlineData("timeout")][InlineData("nativeTimeout")][InlineData("unknownBusiness")][InlineData("noCode")][InlineData("emptyObject")][InlineData("twoWinners")]
    public void Concurrency_diagnostics_require_a_classified_conflict(string change)
    {
        var files=Diagnostics();var bytes=Phase7SqlEvidenceFiles.Encode(new[]{new Phase7SqlAttemptEvidence(true,[],null,[]),change switch{
            "timeout"=>new Phase7SqlAttemptEvidence(null,[],"TimeoutException",[]),
            "nativeTimeout"=>new(null,[],"SqlException",[-2,2601]),
            "unknownBusiness"=>new(false,["UnexpectedFailure"],null,[]),
            "noCode"=>new(false,[],null,[]),
            _=>new(true,[],null,[])} });
        if(change=="emptyObject")bytes=Encoding.UTF8.GetBytes("[{\"Success\":true,\"ErrorCodes\":[],\"ErrorType\":null,\"SqlNumbers\":[]},{}]");
        files[files.FindIndex(f=>f.RelativePath=="S15/concurrent.json")]=new("S15/concurrent.json",bytes);
        Assert.Throws<Phase7SqlGuardException>(()=>Phase7SqlGlobalControls.ErrorDiagnostics(files));
    }

    [Theory][InlineData(2601)][InlineData(2627)][InlineData(1205)]
    public void Concurrency_accepts_only_the_documented_native_conflicts(int number)
    {
        var files=Diagnostics();files[files.FindIndex(f=>f.RelativePath=="S15/concurrent.json")]=new("S15/concurrent.json",
            Phase7SqlEvidenceFiles.Encode(new[]{new Phase7SqlAttemptEvidence(true,[],null,[]),new Phase7SqlAttemptEvidence(null,[],"SqlException",[number])}));
        Phase7SqlGlobalControls.ErrorDiagnostics(files);
    }

    [Theory][InlineData("BusinessPartnerCodeAlreadyExists")][InlineData("BusinessPartnerIdentificationAlreadyExists")]
    public void Concurrency_preserves_business_conflict_codes(string code)
    {
        var files=Diagnostics();files[files.FindIndex(f=>f.RelativePath=="S15/concurrent.json")]=new("S15/concurrent.json",
            Phase7SqlEvidenceFiles.Encode(new[]{new Phase7SqlAttemptEvidence(true,[],null,[]),new Phase7SqlAttemptEvidence(false,[code],null,[])}));
        Phase7SqlGlobalControls.ErrorDiagnostics(files);
    }

    [Theory][InlineData("state")][InlineData("scope")][InlineData("history")][InlineData("sequence")]
    public void Trace_coverage_rejects_missing_states_scopes_history_and_case_order(string mutation)
    {
        var f=Fixture;var after=Phase7SqlExecutionCatalog.Cases.ToDictionary(c=>c.Id,c=>Phase7SqlEvidenceFiles.Decode<Phase7SqlTracePair>(File.ReadAllBytes(Path.Combine(f.Root,c.Method,"sessions.json"))));
        var timings=Phase7SqlExecutionCatalog.Cases.Select(c=>Phase7SqlEvidenceFiles.Decode<Phase7SqlCaseTiming>(File.ReadAllBytes(Path.Combine(f.Root,c.Method,"case-timing.json")))).ToArray();
        var snapshots=Phase7SqlExecutionCatalog.Cases.SelectMany(c=>new[]{"A","B"}.Select((role,index)=>new Phase7SqlEvidenceBundleFile($"{c.Method}/snapshot-{role}-{index+1:0000}.json",[]))).ToArray();
        if(mutation=="state")after["S12"]=after["S12"] with{A=after["S12"].A with{ScopeStates=[]}};
        if(mutation=="scope")after["S15"]=after["S15"] with{A=after["S15"].A with{Independent=[]}};
        if(mutation=="history")
        {
            var time=timings.Single(t=>t.Id=="S03");timings[Array.IndexOf(timings,time)]=time with{Before=time.Before with{A=time.Before.A with{Samples=[] ,TotalWriteConnections=0}}};
        }
        if(mutation=="sequence")timings[1]=timings[1] with{Sequence=timings[0].Sequence};
        Assert.Throws<Phase7SqlGuardException>(()=>Phase7SqlTraceCoverage.Validate(after,timings,snapshots,f.Manifest,new(f.Started,f.Finished,21),true));
    }

    [Fact]
    public void Trace_lineage_follows_execution_sequence_instead_of_catalog_sorting()
    {
        var f=new Phase7SqlSyntheticPackage(Phase7SqlEvidenceFilesTests.Root(),executionOrder:Phase7SqlExecutionCatalog.Cases.Reverse().Select(c=>c.Id).ToArray());
        var after=Phase7SqlExecutionCatalog.Cases.ToDictionary(c=>c.Id,c=>Phase7SqlEvidenceFiles.Decode<Phase7SqlTracePair>(File.ReadAllBytes(Path.Combine(f.Root,c.Method,"sessions.json"))));
        var timings=Phase7SqlExecutionCatalog.Cases.Select(c=>Phase7SqlEvidenceFiles.Decode<Phase7SqlCaseTiming>(File.ReadAllBytes(Path.Combine(f.Root,c.Method,"case-timing.json")))).ToArray();
        var snapshots=Phase7SqlExecutionCatalog.Cases.SelectMany(c=>new[]{"A","B"}.Select((role,index)=>new Phase7SqlEvidenceBundleFile($"{c.Method}/snapshot-{role}-{index+1:0000}.json",[]))).ToArray();
        Phase7SqlTraceCoverage.Validate(after,timings,snapshots,f.Manifest,new(f.Started,f.Finished,21),true);
    }

    [Fact]
    public void Baseline_supports_repeated_test_ids_but_requires_distinct_execution_ids()
    {
        var doc=XDocument.Parse(Encoding.UTF8.GetString(File.ReadAllBytes(Path.Combine(Fixture.Root,"offline-baseline.trx"))));
        XNamespace ns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010";var results=doc.Root!.Element(ns+"Results")!;
        var clone=new XElement(results.Elements().First());clone.SetAttributeValue("executionId",Guid.NewGuid());results.Add(clone);
        var count=results.Elements().Count();var counters=doc.Root.Element(ns+"ResultSummary")!.Element(ns+"Counters")!;
        foreach(var key in new[]{"total","executed","passed"})counters.SetAttributeValue(key,count);
        Assert.Equal(count,Phase7SqlEvidenceTrx.Inspect(Encoding.UTF8.GetBytes(doc.ToString()),true).Tests);
        results.Add(new XElement(clone));foreach(var key in new[]{"total","executed","passed"})counters.SetAttributeValue(key,count+1);
        Assert.Throws<Phase7SqlGuardException>(()=>Phase7SqlEvidenceTrx.Inspect(Encoding.UTF8.GetBytes(doc.ToString()),true));
    }

    [Fact]
    public void Baseline_cannot_hide_SQL_execution_by_relabelling_only_result_names()
    {
        var doc=XDocument.Parse(Encoding.UTF8.GetString(File.ReadAllBytes(Path.Combine(Fixture.Root,"offline-baseline.trx"))));
        XNamespace ns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010";
        doc.Descendants(ns+"TestMethod").First().SetAttributeValue("className",Phase7SqlExecutionCatalog.ExpectedTestClass);
        Assert.Throws<Phase7SqlGuardException>(()=>Phase7SqlEvidenceTrx.Inspect(Encoding.UTF8.GetBytes(doc.ToString()),true));
    }

    [Fact]
    public void Diagnostics_with_raw_credentials_are_rejected_for_export()
    {
        Assert.Throws<Phase7SqlGuardException>(()=>Phase7SqlGlobalControls.ValidateDiagnosticExport(Encoding.UTF8.GetBytes("{\"Message\":\"Password=synthetic\"}")));
    }
}
