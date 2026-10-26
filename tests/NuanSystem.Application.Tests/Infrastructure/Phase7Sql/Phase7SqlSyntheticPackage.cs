using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

// Fabricated complete protocol, never live SQL evidence. Positive tests only prove coherence/integrity.
internal sealed class Phase7SqlSyntheticPackage
{
    public string Root {get;}
    public Phase7SqlManifest Manifest {get;}
    public Phase7SqlApproval Approval {get;}
    public Phase7SqlCandidate Candidate {get;}
    public Phase7SqlMonitorObservation Monitor {get;set;}
    public Phase7SqlIndependentWitness Witness {get;set;}
    public Phase7SqlRuntimeStart Start {get;}
    public Phase7SqlRuntimeEnd End {get;set;}
    public Phase7SqlSuiteInputs Inputs {get;}
    public Phase7SqlMonitorApproval MonitorApproval {get;}
    public DateTimeOffset Started {get;}
    public DateTimeOffset Finished {get;}

    public Phase7SqlSyntheticPackage(string directory,int runIndex=0,IReadOnlyList<string>? executionOrder=null)
    {
        Root=Path.Combine(directory,"run");System.IO.Directory.CreateDirectory(Root);
        var sample=Phase7SqlSyntheticEvidence.Pair()[runIndex];Started=sample.StartedAtUtc;Finished=sample.FinishedAtUtc;
        var candidateRoot=Path.Combine(directory,"candidate");System.IO.Directory.CreateDirectory(Path.Combine(candidateRoot,"src"));System.IO.Directory.CreateDirectory(Path.Combine(candidateRoot,"bin"));
        File.WriteAllText(Path.Combine(candidateRoot,"src","Pilot.cs"),"// synthetic protocol source");File.WriteAllBytes(Path.Combine(candidateRoot,"bin","NuanSystem.Application.Tests.dll"),[1,2,3]);
        Candidate=Phase7SqlCandidate.Freeze(candidateRoot,["src/Pilot.cs"],["bin/NuanSystem.Application.Tests.dll"]) with{CapturedAtUtc=Started.AddMinutes(-20)};
        var procedures=new[]{"SP_NA_POST_BUSINESSPARTNERS_CREAR","SP_NA_PUT_BUSINESSPARTNERS_ACTUALIZAR","SP_NA_DELETE_BUSINESSPARTNERS_ELIMINAR",
            "SP_NA_GET_BUSINESSPARTNERS_BUSCARPORID","SP_NA_GET_BUSINESSPARTNERS_BUSCARPORCODIGO","SP_NA_GET_BUSINESSPARTNERS_BUSCARPORIDENTIFICACION"}
            .ToDictionary(n=>"dbo."+n,n=>"CREATE PROCEDURE dbo."+n+" AS SELECT 1;");
        Phase7SqlTarget Target(string role,int id)=>new(role,$"NuanSystem_Phase7_{sample.RunId:N}_{role}",id,id,sample.RunId,"BackendTransactionValidation",160,"SQL_Latin1_General_CP1_CI_AS",false,
            Phase7SqlDestinationSqlReader.Fingerprint("synthetic-schema-"+role),procedures.ToDictionary(p=>p.Key,p=>Phase7SqlDestinationSqlReader.Fingerprint(p.Value)),
            "p7_app_"+role,"p7_observer_"+role,["SELECT","EXECUTE","INSERT"],["SELECT"],$"env:NUANSYSTEM_PHASE7_TENANT_{role}_CONNECTION_STRING",$"env:NUANSYSTEM_PHASE7_OBSERVER_{role}_CONNECTION_STRING");
        Manifest=new(1,sample.RunId,"operator","EXEC-AUTH","PREP-AUTH",Started.AddHours(-1),Started.AddHours(1),600,30,
            new("tcp:lab.example.test,1433","LAB","16.0.1000.6"),Target("A",101),Target("B",102));
        var manifestBytes=JsonSerializer.SerializeToUtf8Bytes(Manifest,new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Approval=new("EXEC-AUTH","PREP-AUTH","operator",Manifest.RunId,Phase7SqlEvidenceFiles.Hash(manifestBytes),Manifest.Server.DataSource,Manifest.Server.ExpectedServerName,
            new("A",Manifest.TenantA.DatabaseName,101,101),new("B",Manifest.TenantB.DatabaseName,102,102),Manifest.NotBeforeUtc,Manifest.ExpiresAtUtc);
        Write("manifest.json",manifestBytes);Write("approval.json",JsonSerializer.SerializeToUtf8Bytes(Approval,new JsonSerializerOptions(JsonSerializerDefaults.Web)));
        var baseline=Baseline(sample.TrxBytes,Started.AddMinutes(-10),Started.AddMinutes(-9));
        var triggers=new[]{"Audit","Outbox","SyncAudit"}.Select(s=>new Phase7SqlTriggerCapture($"TR_P7_{Manifest.RunId:N}_{s}","synthetic trigger "+s,false)).ToArray();
        var hashes=triggers.ToDictionary(t=>t.Name,t=>Phase7SqlDestinationSqlReader.Fingerprint(t.Definition!));
        Inputs=new(Manifest.RunId,Manifest.ExecutionAuthorizationId,Approval.ManifestSha256,Phase7SqlExecutionCatalog.Sha256,7,7,int.MaxValue,-1,hashes,hashes,
            "SYNTHETIC_LAB_ONLY_SAP_NONE_WORKERS_OFF_RELAY_OFF",Candidate.SourceSha256,Candidate.BinarySha256,Phase7SqlEvidenceFiles.Hash(baseline));
        Write("suite-inputs.json",JsonSerializer.SerializeToUtf8Bytes(Inputs,new JsonSerializerOptions(JsonSerializerDefaults.Web)));
        Write("candidate/source-index.json",Candidate.SourceIndex);Write("candidate/binary-index.json",Candidate.BinaryIndex);
        Write("offline-baseline.trx",baseline);Write("sql.trx",sample.TrxBytes);
        Start=new(Manifest.RunId,12345,Started.AddSeconds(1),Candidate.SourceSha256,Candidate.BinarySha256,Inputs.OfflineBaselineSha256,
            Phase7SqlExecutionCatalog.Sha256,Candidate.BinaryFiles[0].Sha256);
        End=new(Manifest.RunId,12345,Finished.AddSeconds(-1),true,Phase7SqlExecutionCatalog.Cases.Select(c=>c.Id).ToArray(),false,false,0,false,0);
        Write("global/start.json",Phase7SqlEvidenceFiles.Encode(Start));WriteEnd();
        foreach(var target in new[]{Manifest.TenantA,Manifest.TenantB})foreach(var observer in new[]{false,true})
        {
            var principal=observer?target.ObserverPrincipal:target.ApplicationPrincipal;
            var observation=new Phase7SqlDestinationObservation(Manifest.Server.ExpectedServerName,Manifest.Server.ProductVersion,target.DatabaseName,target.DatabaseId,target.CompanyId,
                Manifest.RunId,target.Purpose,160,target.Collation,false,principal,principal,principal,false,true,
                observer?["SELECT","CONNECT","VIEW DEFINITION"]:["SELECT","INSERT","EXECUTE","CONNECT","VIEW DEFINITION"],
                ["CONNECT SQL","VIEW ANY DATABASE"],["SELECT"],target.SchemaSha256,target.ProcedureSha256,50,observer?0:1,observer?0:1,false);
            Write($"preflight/destination-{target.Role}{(observer?"O":"W")}.json",Phase7SqlEvidenceFiles.Encode(new Phase7SqlDestinationCapture(target.Role,observer,Started.AddSeconds(2),observation,"synthetic-schema-"+target.Role,procedures)));
        }
        var empty=Phase7SqlEvidenceFiles.Encode(Phase7SqlRollbackSnapshot.RequiredTables.ToDictionary(t=>t,_=>new JsonArray()));
        var treeA=new TraceBuilder(Manifest.TenantA);var treeB=new TraceBuilder(Manifest.TenantB);var sequence=0;
        foreach(var spec in executionOrder is null?Phase7SqlExecutionCatalog.Cases:executionOrder.Select(id=>Phase7SqlExecutionCatalog.Cases.Single(c=>c.Id==id)))
        {
            sequence++;var before=new Phase7SqlTracePair(treeA.Capture(),treeB.Capture());var rule=Phase7SqlTraceCoverage.Required(spec.Id);
            treeA.Apply(rule.WritesA,rule.StatesA,rule.NewScopesA);treeB.Apply(rule.WritesB,rule.StatesB,rule.NewScopesB);
            treeA.Observers+=2+(sequence==21?1:0);treeB.Observers+=2+(sequence==21?1:0);
            var caseStart=Started.AddSeconds(sequence*2);var caseFinish=caseStart.AddSeconds(1);
            Write(spec.Method+"/case.json",Phase7SqlEvidenceFiles.Encode(new Phase7SqlCaseEvidence(spec.Id,spec.Checks.ToDictionary(c=>c,_=>Phase7SqlEvidenceStatus.Passed))));
            Write(spec.Method+"/snapshot-A-0001.json",empty);Write(spec.Method+"/snapshot-B-0002.json",empty);
            Write(spec.Method+"/sessions.json",Phase7SqlEvidenceFiles.Encode(new Phase7SqlTracePair(treeA.Capture(),treeB.Capture())));
            Write(spec.Method+"/case-timing.json",Phase7SqlEvidenceFiles.Encode(new Phase7SqlCaseTiming(spec.Id,sequence,caseStart,caseFinish,before,sequence==21)));
            foreach(var role in new[]{"A","B"})Write(spec.Method+"/preflight-"+role+".json",Phase7SqlEvidenceFiles.Encode(new Phase7SqlPrerequisiteCapture(spec.Id,role,caseStart.AddMilliseconds(100),7,0,triggers)));
            if(spec.Id is "S05" or "S06" or "S07" or "S08" or "S09" or "S10" or "S11")
            {
                var number=int.Parse(spec.Id[1..]);var fault=(Phase7SqlFailureCase)number;
                foreach(var operation in new[]{"C","U","D"})Write(spec.Method+"/fault-"+operation+".json",Phase7SqlEvidenceFiles.Encode(new{
                    Case=fault.ToString(),BarrierAcknowledged=number>=10,Error=Diagnostic(number is 5 or 6 or 7 or 9?"SqlException":number==8?nameof(Phase7SqlAfterProcedureException):"OperationCanceledException",
                        number is 5 or 6 or 7?[51500+number]:number==9?[547]:[])}));
            }
        }
        foreach(var method in new[]{"R01_Legacy","R01_Pilot"})Write(method+"/internal-rollback.json",Phase7SqlEvidenceFiles.Encode(new{
            attempted=true,returnedFalse=false,Disposition="InsertExecRollbackRejected",Error=Diagnostic("SqlException",[3915,266])}));
        Write("S17/owner-close.json",Phase7SqlEvidenceFiles.Encode(Diagnostic(nameof(Phase7SqlOwnerClosedException),[])));
        Write("S15/concurrent.json",Phase7SqlEvidenceFiles.Encode(new[]{new Phase7SqlAttemptEvidence(true,[],null,[]),new Phase7SqlAttemptEvidence(null,[],"SqlException",[2601])}));
        Write("S16/repetition.json",Phase7SqlEvidenceFiles.Encode(new{EventDelta=1,SyncAuditDelta=1,AuditDelta=0}));
        Write("global/final-A.json",empty);Write("global/final-B.json",empty);
        Monitor=new(Manifest.RunId,Manifest.Server.ExpectedServerName,Manifest.Server.ProductVersion,"independent_monitor","MONITOR-AUTH",Finished.AddSeconds(2),0,0,0,
            new[]{Manifest.TenantA,Manifest.TenantB}.Select(t=>new Phase7SqlRetainedDatabase(t.Role,t.DatabaseName,t.DatabaseId,Manifest.RunId,t.Purpose,
                Phase7SqlRollbackSnapshot.RequiredTables.ToDictionary(table=>table,_=>0L),Phase7SqlEvidenceFiles.Hash(empty))).ToArray());
        MonitorApproval=new(Manifest.RunId,Monitor.AuthorizationReference,Monitor.Principal,Finished.AddMinutes(-1),Finished.AddMinutes(10));
        Witness=new(Manifest.RunId,Start.ProcessId,true,0,Finished.AddSeconds(1),"independent-reviewer","MONITOR-AUTH",Started.AddMinutes(-1),Finished.AddMinutes(1),
            ["SyncWorker","MasterBranchSyncWorker","SriWorker","Relay"],Phase7SqlEvidenceOrigin.SqlServerLaboratory);
    }
    public Phase7SqlPackageResult Build(string destination,Phase7SqlMonitorObservation? monitor=null,Phase7SqlIndependentWitness? witness=null,bool omitIndependent=false)=>
        Phase7SqlEvidencePackager.Build(Root,destination,"sql.trx",Approval,Candidate,omitIndependent?null:monitor??Monitor,omitIndependent?null:witness??Witness,omitIndependent?null:MonitorApproval);
    public void WriteEnd()=>Replace("global/end.json",Phase7SqlEvidenceFiles.Encode(End));
    public void Replace(string path,byte[] bytes)
    {var full=Phase7SqlEvidenceFiles.Resolve(Root,path);System.IO.Directory.CreateDirectory(Path.GetDirectoryName(full)!);File.WriteAllBytes(full,bytes);}
    private void Write(string path,byte[] bytes)=>Phase7SqlEvidenceFiles.Write(Root,path,bytes);
    private static object Diagnostic(string type,int[] numbers)=>new{Type=type,SqlNumber=numbers.Length>0?(int?)numbers[0]:null,SqlNumbers=numbers,
        GuardCode=(string?)null,SecondaryType=(string?)null,CleanupType=(string?)null,PrimaryType=(string?)null,PrimarySqlNumbers=Array.Empty<int>()};
    private static byte[] Baseline(byte[] sql,DateTimeOffset start,DateTimeOffset finish)
    {
        var doc=XDocument.Parse(Encoding.UTF8.GetString(sql));XNamespace ns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010";
        var times=doc.Root!.Element(ns+"Times")!;times.SetAttributeValue("start",start.ToString("O"));times.SetAttributeValue("finish",finish.ToString("O"));
        foreach(var result in doc.Descendants(ns+"UnitTestResult"))result.SetAttributeValue("testName","Offline.Synthetic."+Guid.NewGuid().ToString("N"));
        foreach(var method in doc.Descendants(ns+"TestMethod"))method.SetAttributeValue("className","Offline.Synthetic");
        return Encoding.UTF8.GetBytes(doc.ToString());
    }
    private sealed class TraceBuilder(Phase7SqlTarget target)
    {
        private readonly Guid id=Guid.NewGuid();private readonly List<Phase7SqlSample> samples=[];
        private readonly List<Phase7SqlScopeState> states=[];private readonly List<TraceBuilder> children=[];
        public int Observers;
        public void Apply(int writes,int stateCount,int scopes)
        {
            if(scopes>0)for(var i=0;i<scopes;i++){var child=new TraceBuilder(target);child.Apply(writes/scopes,stateCount/scopes,0);children.Add(child);}
            else for(var i=0;i<writes;i++)
            {
                var execution=Guid.NewGuid();var session=new Phase7SqlSession(50,target.DatabaseName,1,1,target.DatabaseId);
                samples.Add(new(execution,"Entry",session));samples.Add(new(execution,"BeforeCommit",session));
                if(stateCount>0)states.Add(new(50,target.DatabaseName,target.DatabaseId,1,1,false));
            }
        }
        public Phase7SqlTraceNode Capture()=>new(id,samples.ToArray(),states.ToArray(),samples.Select(s=>s.ExecutionId).Distinct().Count(),Observers,children.Select(c=>c.Capture()).ToArray());
    }
}
