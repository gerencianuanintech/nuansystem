using System.Data;
using System.Text;
using System.Text.Json.Nodes;
using NSubstitute;
using NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

[Trait("Category","OfflineSafety")]
public sealed class Phase7SqlGlobalControlTests
{
    [Fact]
    public async Task Monitor_rejects_fake_or_closed_providers_before_creating_commands()
    {
        var c=Substitute.For<IDbConnection>();c.State.Returns(ConnectionState.Open);c.Database.Returns("master");
        var root=Phase7SqlEvidenceFilesTests.Root();var f=new Phase7SqlSyntheticPackage(root);
        await Assert.ThrowsAsync<Phase7SqlGuardException>(()=>Phase7SqlMonitorReader.ReadAsync(c,f.Manifest,f.MonitorApproval,f.Finished,30,default));
        c.DidNotReceive().CreateCommand();c.DidNotReceive().Open();c.DidNotReceive().Close();
    }

    [Fact]
    public void Monitor_names_are_unique_for_all_four_approved_streams_and_fresh_runs()
    {
        var run=Guid.NewGuid();var names=new[]{"A","B"}.SelectMany(role=>new[]{Phase7SqlMonitorReader.ApplicationName(run,role,false),Phase7SqlMonitorReader.ApplicationName(run,role,true)}).ToArray();
        Assert.Equal(4,names.Distinct().Count());Assert.All(names,n=>Assert.Contains(run.ToString("N"),n));
        Assert.DoesNotContain(Phase7SqlMonitorReader.ApplicationName(Guid.NewGuid(),"A",false),names);
    }

    [Fact]
    public void Boundary_attempts_are_counted_even_when_the_original_failure_is_caught()
    {
        var attempts=0;var barrier=new Phase7SqlFixture.ExternalBarrier(()=>attempts++);
        Assert.Throws<Phase7SqlGuardException>(()=>barrier.CreateConnection());
        Assert.Equal(1,attempts);
    }

    [Fact]
    public void Terminal_evidence_failure_does_not_replace_the_primary_operation_failure()
    {
        var terminal=new IOException("synthetic evidence error");var lease=new Phase7SqlSuiteProcessLease(Guid.NewGuid(),"candidate",TimeSpan.FromMinutes(1),()=>throw terminal);
        lease.Fail();Assert.True(lease.Failed);Assert.Same(terminal,lease.TerminalFailure);
    }

    [Fact]
    public void Duplicate_JSON_keys_are_rejected_inside_physical_rows()
    {
        var text="{\"Table\":[{\"Id\":1,\"Id\":2}]}";
        Assert.Throws<Phase7SqlGuardException>(()=>Phase7SqlEvidenceFiles.Decode<JsonObject>(Encoding.UTF8.GetBytes(text)));
    }

    [Fact]
    public void Data_retention_compares_exact_snapshot_hashes_not_only_row_counts()
    {
        var root=Phase7SqlEvidenceFilesTests.Root();var f=new Phase7SqlSyntheticPackage(root);
        var final=new Dictionary<string,byte[]> { ["A"]=File.ReadAllBytes(Path.Combine(f.Root,"global","final-A.json")),["B"]=File.ReadAllBytes(Path.Combine(f.Root,"global","final-B.json")) };
        Assert.True(Phase7SqlGlobalControls.Retained(f.Monitor,f.Manifest,final));
        var bad=f.Monitor with{Databases=f.Monitor.Databases.Select(d=>d.Role=="A"?d with{SnapshotSha256=new('A',64)}:d).ToArray()};
        Assert.False(Phase7SqlGlobalControls.Retained(bad,f.Manifest,final));
    }

    [Fact]
    public void Missing_monitor_authorization_never_produces_a_passed_orphan_control()
    {
        var root=Phase7SqlEvidenceFilesTests.Root();var f=new Phase7SqlSyntheticPackage(root);
        var package=Phase7SqlEvidencePackager.Build(f.Root,Path.Combine(root,"package"),"sql.trx",f.Approval,f.Candidate,f.Monitor,f.Witness);
        Assert.Equal(Phase7SqlEvidenceStatus.NotRun,package.Report.RunChecks["NoOrphanSessions"]);
    }

    [Fact]
    public void Cancellation_does_not_create_a_package_or_touch_existing_source_evidence()
    {
        var root=Phase7SqlEvidenceFilesTests.Root();var f=new Phase7SqlSyntheticPackage(root);using var stop=new CancellationTokenSource();stop.Cancel();
        var destination=Path.Combine(root,"package");
        Assert.ThrowsAny<OperationCanceledException>(()=>Phase7SqlEvidencePackager.Build(f.Root,destination,"sql.trx",f.Approval,f.Candidate,f.Monitor,f.Witness,f.MonitorApproval,stop.Token));
        Assert.False(Directory.Exists(destination));Assert.True(File.Exists(Path.Combine(f.Root,"manifest.json")));
    }
}
