using System.IO.Compression;
using System.Text;
using System.Text.Json.Nodes;
using NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

[Trait("Category","OfflineSafety")]
public sealed class Phase7SqlEvidencePackageTests
{
    [Fact]
    public void Complete_fabricated_packages_verify_integrity_but_never_authenticate_or_close_the_phase()
    {
        var root=Phase7SqlEvidenceFilesTests.Root();var fixture=new Phase7SqlSyntheticPackage(root);
        var package=fixture.Build(Path.Combine(root,"package"));
        Assert.True(package.IntegrityVerified);Assert.Empty(package.ReviewBlockers);Assert.True(package.ReadyForIndependentRunReview);
        Assert.False(package.EvidenceAuthenticityVerified);Assert.False(package.GlobalPhase7Closed);
        Assert.Equal(14,package.Report.Artifacts.Count);Assert.Equal(17,package.Report.RunChecks.Count);
        Assert.All(package.Report.RunChecks.Values,status=>Assert.Equal(Phase7SqlEvidenceStatus.Passed,status));
        Assert.True(Phase7SqlEvidencePackager.VerifyZip(package.ZipPath).IntegrityVerified);
        var loaded=Phase7SqlEvidencePackager.ReadIntegrityVerifiedReport(package.ZipPath,package.ArchiveSha256);
        Assert.Equal(package.Report.RunId,loaded.RunId);
        Assert.False(Phase7SqlEvidencePackager.VerifyZip(package.ZipPath,expectedSha256:new('A',64)).IntegrityVerified);
    }

    [Fact]
    public void Missing_independent_monitoring_stays_pending_even_when_every_SQL_case_claims_passed()
    {
        var root=Phase7SqlEvidenceFilesTests.Root();var fixture=new Phase7SqlSyntheticPackage(root);
        var package=fixture.Build(Path.Combine(root,"package"),omitIndependent:true);
        Assert.True(package.IntegrityVerified);Assert.False(package.ReadyForIndependentRunReview);
        Assert.Equal(Phase7SqlEvidenceStatus.NotRun,package.Report.RunChecks["NoOrphanSessions"]);
        Assert.Equal(Phase7SqlEvidenceStatus.NotRun,package.Report.RunChecks["DataPreserved"]);
        Assert.Equal(Phase7SqlEvidenceOrigin.Offline,package.Report.Origin);
    }

    [Theory]
    [InlineData("sessions","NoOrphanSessions")][InlineData("transactions","NoOrphanSessions")]
    [InlineData("processAlive","NoOrphanSessions")][InlineData("nonzeroExit","NoOrphanSessions")]
    [InlineData("wrongRun","NoOrphanSessions")][InlineData("earlyMonitor","NoOrphanSessions")]
    [InlineData("workers","WorkersAndRelayDisabled")][InlineData("retention","DataPreserved")]
    [InlineData("gates","GatesDisabledAtEnd")][InlineData("external","NoExternalCalls")]
    [InlineData("sap","SapDisabled")][InlineData("failedRun","GatesDisabledAtEnd")]
    public void Contradictory_global_observations_cannot_become_passed(string change,string control)
    {
        var root=Phase7SqlEvidenceFilesTests.Root();var f=new Phase7SqlSyntheticPackage(root);
        switch(change)
        {
            case "sessions":f.Monitor=f.Monitor with{RemainingSessions=1};break;
            case "transactions":f.Monitor=f.Monitor with{TransactionsB=1};break;
            case "processAlive":f.Witness=f.Witness with{HasExited=false};break;
            case "nonzeroExit":f.Witness=f.Witness with{ExitCode=1};break;
            case "wrongRun":f.Monitor=f.Monitor with{RunId=Guid.NewGuid()};break;
            case "earlyMonitor":f.Monitor=f.Monitor with{ObservedAtUtc=f.Started};break;
            case "workers":f.Witness=f.Witness with{DisabledComponents=["SyncWorker"]};break;
            case "retention":f.Monitor=f.Monitor with{Databases=[]};break;
            case "gates":f.End=f.End with{StandardGateEnabled=true};break;
            case "external":f.End=f.End with{ExternalBoundaryAttempts=1};break;
            case "sap":f.End=f.End with{SapExecuted=true};break;
            default:f.End=f.End with{Completed=false};break;
        }
        f.WriteEnd();var package=f.Build(Path.Combine(root,"package"));
        Assert.False(package.ReadyForIndependentRunReview);Assert.Equal(Phase7SqlEvidenceStatus.Failed,package.Report.RunChecks[control]);
    }

    [Fact]
    public void A_diagnostic_file_with_unexpected_content_does_not_pass_by_presence()
    {
        var root=Phase7SqlEvidenceFilesTests.Root();var f=new Phase7SqlSyntheticPackage(root);
        f.Replace("S05/fault-C.json",Encoding.UTF8.GetBytes("{}"));
        var package=f.Build(Path.Combine(root,"package"));
        Assert.Equal(Phase7SqlEvidenceStatus.Failed,package.Report.RunChecks["ErrorDiagnostics"]);
    }

    [Fact]
    public void Truncated_snapshots_and_inconsistent_connection_counts_are_rejected()
    {
        var root=Phase7SqlEvidenceFilesTests.Root();var f=new Phase7SqlSyntheticPackage(root);
        f.Replace("S01/snapshot-A-0001.json",Encoding.UTF8.GetBytes("{}"));
        var trace=JsonNode.Parse(File.ReadAllBytes(Path.Combine(f.Root,"S01","sessions.json")))!;trace["A"]!["TotalWriteConnections"]=2;
        f.Replace("S01/sessions.json",Encoding.UTF8.GetBytes(trace.ToJsonString()));
        var package=f.Build(Path.Combine(root,"package"));
        Assert.Equal(Phase7SqlEvidenceStatus.Failed,package.Report.RunChecks["DurableSnapshots"]);
        Assert.Equal(Phase7SqlEvidenceStatus.Failed,package.Report.RunChecks["ConnectionCounts"]);
    }

    [Fact]
    public void Packaging_does_not_overwrite_existing_evidence_or_accept_a_changed_candidate()
    {
        var root=Phase7SqlEvidenceFilesTests.Root();var f=new Phase7SqlSyntheticPackage(root);var path=Path.Combine(root,"package");
        f.Build(path);
        Assert.Throws<Phase7SqlGuardException>(()=>f.Build(path));
        File.WriteAllText(Path.Combine(f.Candidate.RootPath,"src","Pilot.cs"),"changed");
        Assert.Throws<Phase7SqlGuardException>(()=>f.Build(Path.Combine(root,"different")));
    }

    [Theory][InlineData("tamper")][InlineData("duplicate")][InlineData("traversal")][InlineData("symlink")]
    public void Altered_or_ambiguous_ZIP_entries_fail_integrity(string change)
    {
        var root=Phase7SqlEvidenceFilesTests.Root();var package=new Phase7SqlSyntheticPackage(root).Build(Path.Combine(root,"package"));
        using(var zip=ZipFile.Open(package.ZipPath,ZipArchiveMode.Update))
        {
            if(change=="tamper"){var entry=zip.GetEntry("artifacts/Schema.json")!;using var write=entry.Open();write.SetLength(0);write.Write(Encoding.UTF8.GetBytes("{}"));}
            else if(change=="symlink")zip.GetEntry("artifacts/Schema.json")!.ExternalAttributes=unchecked((int)0xA1FF0000);
            else{var entry=zip.CreateEntry(change=="duplicate"?"run-report.json":"../outside.json");using var write=entry.Open();write.Write([1]);}
        }
        Assert.False(Phase7SqlEvidencePackager.VerifyZip(package.ZipPath).IntegrityVerified);
    }

    [Fact]
    public void Two_distinct_fabricated_packages_feed_the_existing_closure_protocol_without_closing_it()
    {
        var root=Phase7SqlEvidenceFilesTests.Root();var a=new Phase7SqlSyntheticPackage(Path.Combine(root,"first"),0);
        var b=new Phase7SqlSyntheticPackage(Path.Combine(root,"second"),1);
        var first=a.Build(Path.Combine(root,"package-a"));var second=b.Build(Path.Combine(root,"package-b"));
        var assessment=Phase7SqlClosureEvaluator.Assess([first.Report,second.Report]);
        Assert.True(assessment.ReadyForIndependentPilotReview);Assert.False(assessment.GlobalPhase7Closed);Assert.False(assessment.EvidenceAuthenticityVerified);
    }

    [Fact]
    public void Observer_counts_cannot_be_zero_when_the_case_contains_snapshots_and_preflight()
    {
        var root=Phase7SqlEvidenceFilesTests.Root();var f=new Phase7SqlSyntheticPackage(root);
        var node=JsonNode.Parse(File.ReadAllBytes(Path.Combine(f.Root,"S01","sessions.json")))!;node["A"]!["ObserverConnectionCount"]=0;
        f.Replace("S01/sessions.json",Encoding.UTF8.GetBytes(node.ToJsonString()));
        var package=f.Build(Path.Combine(root,"package"));
        Assert.Equal(Phase7SqlEvidenceStatus.Failed,package.Report.RunChecks["ConnectionCounts"]);
    }

    [Fact]
    public void A_timeout_loser_does_not_satisfy_concurrency_error_diagnostics()
    {
        var root=Phase7SqlEvidenceFilesTests.Root();var f=new Phase7SqlSyntheticPackage(root);
        f.Replace("S15/concurrent.json",Phase7SqlEvidenceFiles.Encode(new[]{new{Success=true,ErrorType=(string?)null,SqlNumber=(int?)null},new{Success=false,ErrorType=(string?)"TimeoutException",SqlNumber=(int?)-2}}));
        var package=f.Build(Path.Combine(root,"package"));
        Assert.Equal(Phase7SqlEvidenceStatus.Failed,package.Report.RunChecks["ErrorDiagnostics"]);
    }
}
