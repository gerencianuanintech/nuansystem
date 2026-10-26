using System.Data;
using System.Text.Json.Nodes;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

// Explicitly enabled suite only. Writes immutable receipts; does not open connections or resolve secrets.
internal sealed class Phase7SqlSuiteEvidenceCapture(string root,Phase7SqlManifest manifest,Phase7SqlCandidate candidate,
    Phase7SqlSuiteInputs inputs,byte[] offlineBaseline)
{
    private readonly object sync=new();
    private readonly HashSet<string> destinations=new(StringComparer.Ordinal);
    public void Start()
    {
        var assembly=typeof(Phase7SqlSuiteEvidenceCapture).Assembly.Location;
        var executing=Phase7SqlEvidenceFiles.Hash(File.ReadAllBytes(assembly));
        Phase7SqlEvidenceFiles.Require(candidate.BinaryFiles.Any(f=>f.Sha256==executing
            && Path.GetFullPath(Path.Combine(candidate.RootPath,f.RelativePath))==Path.GetFullPath(assembly)),"EXECUTING_BINARY");
        var now=DateTimeOffset.UtcNow;var baseline=Phase7SqlEvidenceTrx.Inspect(offlineBaseline,true);
        Phase7SqlEvidenceFiles.Require(candidate.CapturedAtUtc<=baseline.Start && baseline.Finish<=now,"BASELINE_ORDER");
        Write("candidate/source-index.json",candidate.SourceIndex);Write("candidate/binary-index.json",candidate.BinaryIndex);
        Write("offline-baseline.trx",offlineBaseline);
        Write("global/start.json",Phase7SqlEvidenceFiles.Encode(new Phase7SqlRuntimeStart(manifest.RunId,Environment.ProcessId,now,
            candidate.SourceSha256,candidate.BinarySha256,inputs.OfflineBaselineSha256,Phase7SqlExecutionCatalog.Sha256,executing)));
    }
    public IPhase7SqlDestinationVerifier Verifier()=>new Phase7SqlDestinationVerifier(async(c,t,timeout,ct)=>
        await Phase7SqlDestinationSqlReader.ReadWithEvidenceAsync(c,t,timeout,ct,(value,schema,definitions)=>
        {
            var target=new[]{manifest.TenantA,manifest.TenantB}.SingleOrDefault(x=>x.DatabaseName==value.DatabaseName && x.DatabaseId==value.DatabaseId)
                ??throw new Phase7SqlGuardException("PHASE7_SQL_EVIDENCE_DESTINATION");
            var observer=t is null;
            Phase7SqlDestinationVerifier.ValidateObservation(manifest,target,observer,value);
            var key=target.Role+(observer?"O":"W");
            lock(sync)
            {
                if(!destinations.Add(key))return;
                Write($"preflight/destination-{key}.json",Phase7SqlEvidenceFiles.Encode(new Phase7SqlDestinationCapture(
                    target.Role,observer,DateTimeOffset.UtcNow,value,schema,definitions)));
            }
        }));

    public async Task FinalSnapshotsAsync(Phase7SqlFixture fixture,CancellationToken token)
    {
        foreach(var target in new[]{manifest.TenantA,manifest.TenantB})
        {
            var raw=await fixture.Tenant(target.Role).ObserveAsync((c,timeout,ct)=>Phase7SqlSnapshotSqlReader.ReadAsync(c,manifest,target,timeout,ct),token);
            Phase7SqlEvidenceFiles.Require(raw.Length<=8*1024*1024,"SNAPSHOT_SIZE");
            var bytes=System.Text.Encoding.UTF8.GetBytes(raw);Phase7SqlGlobalControls.ValidatePhysicalSnapshot(bytes);
            Write($"global/final-{target.Role}.json",bytes);
        }
    }
    public void Stop(Phase7SqlFixture? fixture,Phase7SqlSuiteProcessLease? lease)
    {
        if(fixture is null || lease is null)return;
        lock(sync)
        {
            if(lease.Failed && !File.Exists(Phase7SqlEvidenceFiles.Resolve(root,"global/failure-latch.json")))
                Write("global/failure-latch.json",Phase7SqlEvidenceFiles.Encode(new{manifest.RunId,Failed=true}));
            if(File.Exists(Phase7SqlEvidenceFiles.Resolve(root,"global/end.json")))return;
            Write("global/end.json",Phase7SqlEvidenceFiles.Encode(new Phase7SqlRuntimeEnd(manifest.RunId,Environment.ProcessId,DateTimeOffset.UtcNow,
                !lease.Failed && lease.CompletedCases.Count==21,lease.CompletedCases,
                Phase7SqlGates.ReadEnvironment().StandardEnabled,Phase7SqlGates.ReadEnvironment().IsolatedEnabled,
                fixture.ExternalBoundaryAttempts,false,0)));
        }
    }
    private void Write(string path,byte[] bytes)=>Phase7SqlEvidenceFiles.Write(root,path,bytes);
}
