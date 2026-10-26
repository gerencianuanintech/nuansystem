using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal sealed record Phase7SqlEvidenceBundleFile(string RelativePath,byte[] Bytes);
internal sealed record Phase7SqlPackageResult(string Directory,string ZipPath,string ArchiveSha256,Phase7SqlRunEvidence Report,
    bool IntegrityVerified,IReadOnlyList<string> ReviewBlockers)
{
    public bool ReadyForIndependentRunReview=>IntegrityVerified && ReviewBlockers.Count==0;
    public bool EvidenceAuthenticityVerified=>false;
    public bool GlobalPhase7Closed=>false;
}
internal sealed record Phase7SqlPackageVerification(bool IntegrityVerified,IReadOnlyList<string> Reasons);

// Local, explicit offline packager. No SQL/SAP, process launch, credential resolution, extraction or deletion.
internal static class Phase7SqlEvidencePackager
{
    public static Phase7SqlPackageResult BuildFromMonitorCapture(string runRoot,string destination,
        Phase7SqlApproval trustedApproval,Phase7SqlCandidate candidate,string captureRoot,string expectedIndexSha256,
        Phase7SqlControlledMonitorApproval monitorApproval,CancellationToken token=default)
    {
        token.ThrowIfCancellationRequested();
        try
        {
            var capturePath=Path.GetFullPath(captureRoot);var output=Path.GetFullPath(destination);var source=Path.GetFullPath(runRoot);
            Phase7SqlEvidenceFiles.Require(!Inside(output,capturePath) && !Inside(capturePath,output)
                && !Inside(source,capturePath) && !Inside(capturePath,source),"MONITOR_CAPTURE_OUTPUT");
            var capture=Phase7SqlMonitorCaptureReader.Load(captureRoot,runRoot,expectedIndexSha256,monitorApproval,trustedApproval,token);
            return BuildCore(runRoot,destination,"sql.trx",trustedApproval,candidate,capture.Observation,capture.Witness,monitorApproval.Monitor,token,capture,monitorApproval);
        }
        catch(OperationCanceledException) when(token.IsCancellationRequested){throw;}
        catch(Phase7SqlGuardException){throw;}
        catch{throw new Phase7SqlGuardException("PHASE7_SQL_EVIDENCE_MONITOR_PACKAGING_FAILED");}
    }
    public static Phase7SqlPackageResult Build(string runRoot,string destination,string trxRelativePath,
        Phase7SqlApproval trustedApproval,Phase7SqlCandidate candidate,Phase7SqlMonitorObservation? monitor,
        Phase7SqlIndependentWitness? witness,Phase7SqlMonitorApproval? monitorApproval=null,CancellationToken token=default)
    {
        try{return BuildCore(runRoot,destination,trxRelativePath,trustedApproval,candidate,monitor,witness,monitorApproval,token);}
        catch(OperationCanceledException) when(token.IsCancellationRequested){throw;}
        catch(Phase7SqlGuardException){throw;}
        catch{throw new Phase7SqlGuardException("PHASE7_SQL_EVIDENCE_PACKAGING_FAILED");}
    }
    private static Phase7SqlPackageResult BuildCore(string runRoot,string destination,string trxRelativePath,
        Phase7SqlApproval trustedApproval,Phase7SqlCandidate candidate,Phase7SqlMonitorObservation? monitor,
        Phase7SqlIndependentWitness? witness,Phase7SqlMonitorApproval? monitorApproval,CancellationToken token,
        Phase7SqlVerifiedMonitorCapture? capture=null,Phase7SqlControlledMonitorApproval? captureApproval=null)
    {
        token.ThrowIfCancellationRequested();candidate.Verify(token);
        var sourceRoot=Path.GetFullPath(runRoot);var output=Path.GetFullPath(destination);
        Phase7SqlEvidenceFiles.CheckNoLinks(sourceRoot);Phase7SqlEvidenceFiles.CheckNoLinks(output);
        Phase7SqlEvidenceFiles.Require(!Directory.Exists(output) && !File.Exists(output) && !File.Exists(output+".zip")
            && !Inside(output,sourceRoot) && !Inside(sourceRoot,output),"OUTPUT");
        var payload=new Dictionary<string,byte[]>(StringComparer.OrdinalIgnoreCase);long total=0;
        void Add(string path,byte[] bytes)
        {
            Phase7SqlEvidenceFiles.SafeRelative(path);total+=bytes.Length;
            Phase7SqlEvidenceFiles.Require(total<=Phase7SqlEvidenceFiles.MaximumPackageBytes && payload.Count<4094 && !payload.ContainsKey(path),"TOTAL_OR_DUPLICATE");
            payload.Add(path,bytes.ToArray());
        }
        byte[] Read(string path,int maximum=Phase7SqlEvidenceFiles.MaximumFileBytes)
        {
            token.ThrowIfCancellationRequested();
            var bytes=capture is not null && capture.Inputs.TryGetValue(path,out var frozen)?frozen:Phase7SqlEvidenceFiles.Read(sourceRoot,path,maximum,token);
            Phase7SqlEvidenceFiles.Require(bytes.Length<=maximum,"SIZE");Add("evidence/"+path,bytes);return bytes;
        }
        if(capture is not null)
        {
            foreach(var file in capture.Files)Add(Phase7SqlMonitorCaptureReader.Prefix+file.Key,file.Value);
            Add(Phase7SqlMonitorCaptureReader.Prefix+"reference.json",Phase7SqlEvidenceFiles.Encode(new Phase7SqlMonitorCaptureReference(capture.IndexHash,captureApproval!)));
        }
        byte[]? Optional(string path,int maximum=Phase7SqlEvidenceFiles.MaximumFileBytes)=>
            capture?.Inputs.ContainsKey(path)==true || File.Exists(Phase7SqlEvidenceFiles.Resolve(sourceRoot,path))?Read(path,maximum):null;
        T? OptionalJson<T>(string path) where T:class
        {var bytes=Optional(path);return bytes is null?null:Phase7SqlEvidenceFiles.Decode<T>(bytes);}
        var manifestBytes=Read("manifest.json",65536);
        var manifestText=new UTF8Encoding(false,true).GetString(manifestBytes);
        var start=Phase7SqlEvidenceFiles.Decode<Phase7SqlRuntimeStart>(Read("global/start.json"));
        var manifest=Phase7SqlManifestValidator.Validate(manifestText,trustedApproval,new(true,true),start.StartedAtUtc).Manifest;
        Phase7SqlEvidenceFiles.Require(start.RunId==manifest.RunId && start.ProcessId>0,"START_BINDING");
        var web=new JsonSerializerOptions(JsonSerializerDefaults.Web){AllowDuplicateProperties=false,UnmappedMemberHandling=System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow,RespectRequiredConstructorParameters=true};
        var inputs=JsonSerializer.Deserialize<Phase7SqlSuiteInputs>(Read("suite-inputs.json",65536),web)!;
        Phase7SqlSuiteLoader.ValidateInputs(inputs,manifest,Phase7SqlEvidenceFiles.Hash(manifestBytes));
        var archivedApproval=JsonSerializer.Deserialize<Phase7SqlApproval>(Read("approval.json",65536),web);
        Phase7SqlEvidenceFiles.Require(archivedApproval==trustedApproval,"APPROVAL_ARCHIVE");
        var sourceIndex=Read("candidate/source-index.json");var binaryIndex=Read("candidate/binary-index.json");
        Phase7SqlEvidenceFiles.Require(sourceIndex.SequenceEqual(candidate.SourceIndex) && binaryIndex.SequenceEqual(candidate.BinaryIndex)
            && start.SourceSha256==candidate.SourceSha256 && start.BinarySha256==candidate.BinarySha256
            && inputs.SourceInventorySha256==candidate.SourceSha256 && inputs.BinaryInventorySha256==candidate.BinarySha256
            && start.CatalogSha256==Phase7SqlExecutionCatalog.Sha256
            && candidate.BinaryFiles.Any(f=>f.Sha256==start.ExecutingAssemblySha256 && Path.GetFileName(f.RelativePath)=="NuanSystem.Application.Tests.dll"),"CANDIDATE_BINDING");
        foreach(var file in candidate.SourceFiles.Concat(candidate.BinaryFiles))
        {
            var bytes=Phase7SqlEvidenceFiles.Read(candidate.RootPath,file.RelativePath,Phase7SqlEvidenceFiles.MaximumFileBytes,token);
            Phase7SqlEvidenceFiles.Require(file.Length==bytes.Length && file.Sha256==Phase7SqlEvidenceFiles.Hash(bytes),"CANDIDATE_CHANGED");
            Add("candidate-files/"+file.RelativePath,bytes);
        }
        var trxBytes=Read(trxRelativePath,1024*1024);var trx=Phase7SqlEvidenceTrx.Inspect(trxBytes,false);
        var baseline=Optional("offline-baseline.trx",16*1024*1024);
        var end=OptionalJson<Phase7SqlRuntimeEnd>("global/end.json");
        var failureLatch=Optional("global/failure-latch.json");
        var captures=new List<Phase7SqlDestinationCapture>();
        foreach(var role in new[]{"A","B"})foreach(var kind in new[]{"O","W"})
        {var captured=OptionalJson<Phase7SqlDestinationCapture>($"preflight/destination-{role}{kind}.json");if(captured is not null)captures.Add(captured);}
        var cases=new List<Phase7SqlCaseEvidence>();var snapshots=new List<Phase7SqlEvidenceBundleFile>();
        var traces=new List<Phase7SqlEvidenceBundleFile>();var errors=new List<Phase7SqlEvidenceBundleFile>();
        var timingFiles=new List<Phase7SqlEvidenceBundleFile>();var timings=new List<Phase7SqlCaseTiming>();
        var prerequisites=new List<Phase7SqlPrerequisiteCapture>();var unexpectedFailure=false;
        foreach(var spec in Phase7SqlExecutionCatalog.Cases)
        {
            var dir=spec.Method;
            var caseBytes=Optional($"{dir}/case.json");
            if(caseBytes is null)cases.Add(new(spec.Id,spec.Checks.ToDictionary(c=>c,_=>Phase7SqlEvidenceStatus.NotRun)));
            else
            {
                var item=Phase7SqlEvidenceFiles.Decode<Phase7SqlCaseEvidence>(caseBytes);
                Phase7SqlEvidenceFiles.Require(item.Id==spec.Id,"CASE_BINDING");cases.Add(item);
            }
            var directory=Phase7SqlEvidenceFiles.Resolve(sourceRoot,dir);
            if(!Directory.Exists(directory))continue;
            Phase7SqlEvidenceFiles.Require(System.IO.Directory.GetDirectories(directory).Length==0,"UNEXPECTED_CASE_DIRECTORY");
            var files=System.IO.Directory.GetFiles(directory,"*",SearchOption.TopDirectoryOnly);
            Phase7SqlEvidenceFiles.Require(files.Length<=1024,"FILE_COUNT");
            foreach(var file in files.Order(StringComparer.Ordinal))
            {
                var name=Path.GetFileName(file);var relative=dir+"/"+name;
                if(name=="case.json")continue;
                if(name.StartsWith("snapshot-",StringComparison.Ordinal))
                {Phase7SqlEvidenceFiles.Require(System.Text.RegularExpressions.Regex.IsMatch(name,"\\Asnapshot-[AB]-[0-9]{4,8}\\.json\\z"),"SNAPSHOT_NAME");snapshots.Add(new(relative,Read(relative)));}
                else if(name=="sessions.json")traces.Add(new(relative,Read(relative)));
                else if(name=="case-timing.json")
                {var bytes=Read(relative);timingFiles.Add(new(relative,bytes));var time=Phase7SqlEvidenceFiles.Decode<Phase7SqlCaseTiming>(bytes);Phase7SqlEvidenceFiles.Require(time.Id==spec.Id,"TIMING_CASE");timings.Add(time);}
                else if(name is "preflight-A.json" or "preflight-B.json")prerequisites.Add(Phase7SqlEvidenceFiles.Decode<Phase7SqlPrerequisiteCapture>(Read(relative)));
                else if(name is "failure.json" or "fault-C.json" or "fault-U.json" or "fault-D.json" or "internal-rollback.json" or "owner-close.json" or "repetition.json" or "concurrent.json")
                {var bytes=Read(relative);Phase7SqlGlobalControls.ValidateDiagnosticExport(bytes);errors.Add(new(relative,bytes));if(name=="failure.json")unexpectedFailure=true;}
                else throw new Phase7SqlGuardException("PHASE7_SQL_EVIDENCE_UNEXPECTED_CASE_FILE");
            }
        }
        var final=new Dictionary<string,byte[]>();
        foreach(var role in new[]{"A","B"}){var bytes=Optional($"global/final-{role}.json");if(bytes is not null)final.Add(role,bytes);}
        if(monitor is not null)Add("independent/monitor.json",Phase7SqlEvidenceFiles.Encode(monitor));
        if(witness is not null)Add("independent/witness.json",Phase7SqlEvidenceFiles.Encode(witness));
        if(monitorApproval is not null)Add("independent/monitor-approval.json",Phase7SqlEvidenceFiles.Encode(monitorApproval));
        var checks=Phase7SqlExecutionCatalog.RunChecks.ToDictionary(k=>k,_=>Phase7SqlEvidenceStatus.NotRun);
        void Check(string name,bool available,Action validate)
        {
            if(!available)return;
            try{token.ThrowIfCancellationRequested();validate();checks[name]=Phase7SqlEvidenceStatus.Passed;}
            catch(OperationCanceledException) when(token.IsCancellationRequested){throw;}
            catch{checks[name]=Phase7SqlEvidenceStatus.Failed;}
        }
        void Require(bool ok)=>Phase7SqlEvidenceFiles.Require(ok,"CONTROL");
        Check("ApprovedManifest",true,()=>Require(manifest.RunId==start.RunId && start.StartedAtUtc>=trx.Start && start.StartedAtUtc<trx.Finish));
        Check("DestinationPreflight",captures.Count==4,()=>
        {
            Require(captures.Select(c=>c.Role+(c.Observer?"O":"W")).Distinct().Count()==4);
            foreach(var cap in captures){var target=cap.Role=="A"?manifest.TenantA:manifest.TenantB;Require(cap.Role is "A" or "B" && cap.CapturedAtUtc>=trx.Start && cap.CapturedAtUtc<=trx.Finish);Phase7SqlDestinationVerifier.ValidateObservation(manifest,target,cap.Observer,cap.Observation);}
        });
        Check("EffectiveSchemaAndProcedures",captures.Count==4,()=>
        {
            foreach(var cap in captures)
            {
                var target=cap.Role=="A"?manifest.TenantA:manifest.TenantB;
                Require(Phase7SqlDestinationSqlReader.Fingerprint(cap.SchemaInventory)==target.SchemaSha256.ToUpperInvariant());
                Require(target.ProcedureSha256.All(p=>cap.ProcedureDefinitions.TryGetValue(p.Key,out var text) && Phase7SqlDestinationSqlReader.Fingerprint(text)==p.Value.ToUpperInvariant()));
            }
        });
        Check("FaultTriggerDefinitions",prerequisites.Count==42,()=>
        {
            foreach(var spec in Phase7SqlExecutionCatalog.Cases)foreach(var role in new[]{"A","B"})
            {
                var found=prerequisites.Where(p=>p.CaseId==spec.Id && p.Role==role).ToArray();Require(found.Length==1);
                var expected=role=="A"?inputs.FaultTriggerSha256A:inputs.FaultTriggerSha256B;
                Require(found[0].CapturedAtUtc>=trx.Start && found[0].CapturedAtUtc<=trx.Finish && found[0].Triggers.Count==3
                    && expected.All(e=>found[0].Triggers.Count(t=>t.Name==e.Key && !t.IsDisabled && t.Definition is not null && Phase7SqlDestinationSqlReader.Fingerprint(t.Definition)==e.Value.ToUpperInvariant())==1));
            }
        });
        Check("LookupSeeds",prerequisites.Count==42,()=>Require(prerequisites.All(p=>p.MissingIdsFound==0 && p.LookupId==(p.Role=="A"?inputs.IdentificationTypeIdA:inputs.IdentificationTypeIdB))));
        Check("OfflineBaseline",baseline is not null,()=>
        {
            var info=Phase7SqlEvidenceTrx.Inspect(baseline!,true);
            Require(Phase7SqlEvidenceFiles.Hash(baseline!)==inputs.OfflineBaselineSha256 && start.BaselineSha256==inputs.OfflineBaselineSha256
                && candidate.CapturedAtUtc<=info.Start && info.Finish<=start.StartedAtUtc);
        });
        Check("ArtifactHashes",true,()=>Require(payload.Count>0)); // bytes were read and hashed; output and seal are verified again below
        Check("DurableSnapshots",Phase7SqlExecutionCatalog.Cases.All(c=>snapshots.Count(s=>s.RelativePath.StartsWith(c.Method+"/",StringComparison.Ordinal))>=2) && final.Count==2,()=>
        {foreach(var snapshot in snapshots)Phase7SqlGlobalControls.ValidatePhysicalSnapshot(snapshot.Bytes);foreach(var bytes in final.Values)Phase7SqlGlobalControls.ValidatePhysicalSnapshot(bytes);});
        void TraceChecks()=>Phase7SqlTraceCoverage.Validate(traces.ToDictionary(f=>Path.GetDirectoryName(f.RelativePath)!.Replace('_','.'),f=>Phase7SqlEvidenceFiles.Decode<Phase7SqlTracePair>(f.Bytes)),
            timings,snapshots,manifest,trx,final.Count==2);
        Check("SessionTraces",traces.Count==21 && timings.Count==21,TraceChecks);
        Check("ConnectionCounts",traces.Count==21 && timings.Count==21,TraceChecks);
        Check("ErrorDiagnostics",cases.Count==21,()=>
        {
            Require(!unexpectedFailure && failureLatch is null);
            foreach(var spec in Phase7SqlExecutionCatalog.Cases.Where(c=>c.Id is "S05" or "S06" or "S07" or "S08" or "S09" or "S10" or "S11"))
                foreach(var op in new[]{"C","U","D"})Require(errors.Any(e=>e.RelativePath==$"{spec.Method}/fault-{op}.json"));
            foreach(var id in new[]{"R01_Legacy","R01_Pilot"})Require(errors.Any(e=>e.RelativePath==id+"/internal-rollback.json"));
            Require(errors.Any(e=>e.RelativePath=="S17/owner-close.json"));
            Phase7SqlGlobalControls.ErrorDiagnostics(errors);
        });
        bool MonitorAuthorized()=>monitorApproval is not null && monitor is not null && monitorApproval.RunId==manifest.RunId
            && monitorApproval.Principal==monitor.Principal && monitorApproval.AuthorizationReference==monitor.AuthorizationReference
            && monitorApproval.NotBeforeUtc.Offset==TimeSpan.Zero && monitorApproval.ExpiresAtUtc.Offset==TimeSpan.Zero
            && monitor.ObservedAtUtc>=monitorApproval.NotBeforeUtc && monitor.ObservedAtUtc<monitorApproval.ExpiresAtUtc;
        Check("NoOrphanSessions",monitor is not null && witness is not null && end is not null && monitorApproval is not null,()=>Require(MonitorAuthorized() && Phase7SqlGlobalControls.Termination(monitor!,witness!,manifest,start,end!,trx)));
        Check("NoExternalCalls",end is not null,()=>Require(end!.RunId==manifest.RunId && end.ProcessId==start.ProcessId && end.ExternalBoundaryAttempts==0));
        Check("SapDisabled",end is not null,()=>Require(!end!.SapExecuted));
        Check("WorkersAndRelayDisabled",witness is not null && end is not null,()=>Require(end!.WorkerExecutions==0 && Phase7SqlGlobalControls.WorkerWitness(witness!,start,trx)));
        Check("GatesDisabledAtEnd",end is not null,()=>Require(end!.RunId==manifest.RunId && end.ProcessId==start.ProcessId && end.Completed
            && !end.StandardGateEnabled && !end.IsolatedGateEnabled && end.StoppedAtUtc>=start.StartedAtUtc && end.StoppedAtUtc<=trx.Finish
            && end.CompletedCases.Count==21 && end.CompletedCases.ToHashSet(StringComparer.Ordinal).SetEquals(Phase7SqlExecutionCatalog.Cases.Select(c=>c.Id))));
        Check("DataPreserved",monitor is not null && monitorApproval is not null && final.Count==2,()=>Require(MonitorAuthorized() && witness is not null && end is not null
            && Phase7SqlGlobalControls.Termination(monitor!,witness!,manifest,start,end!,trx) && Phase7SqlGlobalControls.Retained(monitor!,manifest,final)));

        var artifacts=new List<Phase7SqlArtifactEvidence>();
        void Artifact(string kind,byte[] bytes,string extension="json")
        {
            var relative="artifacts/"+kind+"."+extension;Add(relative,bytes);artifacts.Add(new(kind,relative,Phase7SqlEvidenceFiles.Hash(bytes)));
        }
        Artifact("Manifest",manifestBytes);Artifact("SourceInventory",sourceIndex);Artifact("Binaries",binaryIndex);
        var catalog=Encoding.UTF8.GetBytes(string.Join("\n",Phase7SqlExecutionCatalog.Cases.Select(c=>$"{c.Id}|{c.TestName}|{string.Join(',',c.Checks)}")));
        Artifact("CaseCatalog",catalog,"txt");Artifact("Trx",trxBytes,"trx");
        Artifact("Schema",Phase7SqlEvidenceFiles.Encode(captures.Select(c=>new{c.Role,c.Observer,c.SchemaInventory}).ToArray()));
        Artifact("Procedures",Phase7SqlEvidenceFiles.Encode(new{Destinations=captures.Select(c=>new{c.Role,c.Observer,c.ProcedureDefinitions}),Prerequisites=prerequisites}));
        Artifact("Snapshots",Bundle(snapshots.Concat(final.Select(p=>new Phase7SqlEvidenceBundleFile("global/final-"+p.Key+".json",p.Value)))));
        Artifact("Sessions",Bundle(traces.Concat(timingFiles)));Artifact("Connections",Bundle(traces));Artifact("Errors",Bundle(errors));
        Artifact("Boundaries",Phase7SqlEvidenceFiles.Encode(new{Start=start,End=end,Witness=witness}));
        Artifact("Environment",capture is null
            ?Phase7SqlEvidenceFiles.Encode(new{Start=start,End=end,Monitor=monitor,Witness=witness,RunChecks=checks})
            :Phase7SqlEvidenceFiles.Encode(new{Start=start,End=end,Monitor=monitor,Witness=witness,RunChecks=checks,MonitorCaptureIndexSha256=capture.IndexHash}));
        Artifact("SessionTermination",Phase7SqlEvidenceFiles.Encode(new{Monitor=monitor,Witness=witness}));
        var report=new Phase7SqlRunEvidence(manifest.RunId,manifest.TenantA.DatabaseName,manifest.TenantB.DatabaseName,
            Phase7SqlEvidenceFiles.Hash(manifestBytes),candidate.SourceSha256,candidate.BinarySha256,Phase7SqlExecutionCatalog.Sha256,
            trx.Start,trx.Finish,witness?.Origin??Phase7SqlEvidenceOrigin.Offline,checks,cases,artifacts,trxBytes,end?.ExternalBoundaryAttempts??-1,end?.SapExecuted??false,(end?.WorkerExecutions??0)>0);
        var blockers=Phase7SqlClosureEvaluator.ValidateSingleRun(report);
        Add("run-report.json",Phase7SqlEvidenceFiles.Encode(report));
        token.ThrowIfCancellationRequested();Directory.CreateDirectory(output);
        foreach(var file in payload)Phase7SqlEvidenceFiles.Write(output,file.Key,file.Value,token);
        foreach(var file in payload)Phase7SqlEvidenceFiles.Require(Phase7SqlEvidenceFiles.Hash(Phase7SqlEvidenceFiles.Read(output,file.Key,Phase7SqlEvidenceFiles.MaximumFileBytes,token))==Phase7SqlEvidenceFiles.Hash(file.Value),"WRITE_HASH");
        var ledger=payload.OrderBy(p=>p.Key,StringComparer.Ordinal).Select(p=>new Phase7SqlEvidenceFile(p.Key,p.Value.Length,Phase7SqlEvidenceFiles.Hash(p.Value))).ToArray();
        Phase7SqlEvidenceFiles.Write(output,"seal-index.json",Phase7SqlEvidenceFiles.Encode(ledger),token);
        var zip=output+".zip";
        using(var stream=new FileStream(zip,FileMode.CreateNew,FileAccess.Write,FileShare.None))
        using(var archive=new ZipArchive(stream,ZipArchiveMode.Create))
            foreach(var file in ledger.Select(f=>f.RelativePath).Append("seal-index.json"))
            {token.ThrowIfCancellationRequested();var entry=archive.CreateEntry(file,CompressionLevel.Optimal);using var write=entry.Open();write.Write(Phase7SqlEvidenceFiles.Read(output,file,Phase7SqlEvidenceFiles.MaximumFileBytes,token));}
        var verified=VerifyZip(zip,token);Phase7SqlEvidenceFiles.Require(verified.IntegrityVerified,"ZIP_VERIFY");
        using var zipped=new FileStream(zip,FileMode.Open,FileAccess.Read,FileShare.Read);
        var archiveHash=Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(zipped));
        return new(output,zip,archiveHash,report,true,blockers);
    }

    private static byte[] Bundle(IEnumerable<Phase7SqlEvidenceBundleFile> files)=>Phase7SqlEvidenceFiles.Encode(files.OrderBy(f=>f.RelativePath,StringComparer.Ordinal)
        .Select(f=>new Phase7SqlEvidenceFile("evidence/"+f.RelativePath,f.Bytes.Length,Phase7SqlEvidenceFiles.Hash(f.Bytes))).ToArray());
    private static bool Inside(string path,string root)=>path.Equals(root,StringComparison.OrdinalIgnoreCase)
        || path.StartsWith(Path.TrimEndingDirectorySeparator(root)+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase);

    public static Phase7SqlPackageVerification VerifyZip(string path,CancellationToken token=default,string? expectedSha256=null)
    {
        token.ThrowIfCancellationRequested();
        try
        {
            Phase7SqlEvidenceFiles.CheckNoLinks(path);using var stream=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.Read);
            Phase7SqlEvidenceFiles.Require(stream.Length<=Phase7SqlEvidenceFiles.MaximumPackageBytes,"ZIP_SIZE");
            if(expectedSha256 is not null)
            {
                Phase7SqlEvidenceFiles.Require(Phase7SqlEvidenceFiles.IsHash(expectedSha256)
                    && string.Equals(Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(stream)),expectedSha256,StringComparison.OrdinalIgnoreCase),"ARCHIVE_BINDING");
                stream.Position=0;
            }
            ValidateZipDirectory(stream);
            using var zip=new ZipArchive(stream,ZipArchiveMode.Read);
            Phase7SqlEvidenceFiles.Require(zip.Entries.Count is >1 and <=4096 && zip.Entries.Select(e=>e.FullName).Distinct(StringComparer.OrdinalIgnoreCase).Count()==zip.Entries.Count,"ZIP_ENTRIES");
            var bytes=new Dictionary<string,byte[]>(StringComparer.OrdinalIgnoreCase);long total=0;
            foreach(var entry in zip.Entries)
            {
                token.ThrowIfCancellationRequested();Phase7SqlEvidenceFiles.SafeRelative(entry.FullName);
                var kind=((uint)entry.ExternalAttributes>>16)&0xF000;
                Phase7SqlEvidenceFiles.Require(kind is 0 or 0x8000 && (entry.ExternalAttributes&(int)FileAttributes.ReparsePoint)==0,"ZIP_LINK");
                total+=entry.Length;Phase7SqlEvidenceFiles.Require(entry.Length<=Phase7SqlEvidenceFiles.MaximumFileBytes && total<=Phase7SqlEvidenceFiles.MaximumPackageBytes,"ZIP_SIZE");
                using var input=entry.Open();var content=new byte[checked((int)entry.Length)];input.ReadExactly(content);
                Phase7SqlEvidenceFiles.Require(input.ReadByte()==-1,"ZIP_SIZE");bytes.Add(entry.FullName,content);
            }
            Phase7SqlEvidenceFiles.Require(bytes.ContainsKey("seal-index.json"),"SEAL");
            var ledger=Phase7SqlEvidenceFiles.Decode<Phase7SqlEvidenceFile[]>(bytes["seal-index.json"]);
            Phase7SqlEvidenceFiles.Require(ledger.Length==bytes.Count-1 && ledger.Select(f=>f.RelativePath).Distinct(StringComparer.OrdinalIgnoreCase).Count()==ledger.Length,"SEAL");
            foreach(var file in ledger)
                Phase7SqlEvidenceFiles.Require(file.RelativePath!="seal-index.json" && bytes.TryGetValue(Phase7SqlEvidenceFiles.SafeRelative(file.RelativePath),out var content)
                    && content.Length==file.Length && Phase7SqlEvidenceFiles.Hash(content)==file.Sha256,"HASH");
            var report=Phase7SqlEvidenceFiles.Decode<Phase7SqlRunEvidence>(bytes["run-report.json"]);
            Phase7SqlEvidenceFiles.Require(report.Artifacts.Count==14 && report.Artifacts.All(a=>bytes.TryGetValue(Phase7SqlEvidenceFiles.SafeRelative(a.RelativePath),out var content)
                && Phase7SqlEvidenceFiles.Hash(content)==a.Sha256),"ARTIFACT_HASH");
            Phase7SqlEvidenceFiles.Require(report.Artifacts.Select(a=>a.Kind).ToHashSet(StringComparer.Ordinal).SetEquals(Phase7SqlExecutionCatalog.ArtifactKinds)
                && report.Artifacts.Select(a=>a.RelativePath).Distinct(StringComparer.OrdinalIgnoreCase).Count()==14,"ARTIFACT_SET");
            Phase7SqlArtifactEvidence Artifact(string kind)=>report.Artifacts.Single(a=>a.Kind==kind);
            Phase7SqlEvidenceFiles.Require(Artifact("Manifest").Sha256==report.ManifestSha256 && Artifact("SourceInventory").Sha256==report.SourceSha256
                && Artifact("Binaries").Sha256==report.BinarySha256 && Artifact("CaseCatalog").Sha256==Phase7SqlExecutionCatalog.Sha256
                && report.CatalogSha256==Phase7SqlExecutionCatalog.Sha256 && bytes[Artifact("Trx").RelativePath].SequenceEqual(report.TrxBytes),"REPORT_HASH_BINDING");
            foreach(var kind in new[]{"SourceInventory","Binaries","Snapshots","Sessions","Connections","Errors"})
            {
                var files=Phase7SqlEvidenceFiles.Decode<Phase7SqlEvidenceFile[]>(bytes[Artifact(kind).RelativePath]);
                Phase7SqlEvidenceFiles.Require(files.Select(f=>f.RelativePath).Distinct(StringComparer.OrdinalIgnoreCase).Count()==files.Length,"BUNDLE_DUPLICATE");
                foreach(var file in files)
                {
                    var relative=kind is "SourceInventory" or "Binaries"?"candidate-files/"+file.RelativePath:file.RelativePath;
                    Phase7SqlEvidenceFiles.Require(bytes.TryGetValue(Phase7SqlEvidenceFiles.SafeRelative(relative),out var content)
                        && content.Length==file.Length && Phase7SqlEvidenceFiles.Hash(content)==file.Sha256,"BUNDLE_HASH");
                }
            }
            Phase7SqlMonitorCaptureReader.VerifyArchived(bytes,report);
            return new(true,[]);
        }
        catch(OperationCanceledException) when(token.IsCancellationRequested){throw;}
        catch(Phase7SqlGuardException error){return new(false,[error.ErrorCode]);}
        catch{return new(false,["PHASE7_SQL_EVIDENCE_ZIP"]);}
    }

    public static Phase7SqlRunEvidence ReadIntegrityVerifiedReport(string zipPath,string expectedSha256,CancellationToken token=default)
    {
        token.ThrowIfCancellationRequested();
        Phase7SqlEvidenceFiles.CheckNoLinks(zipPath);
        using var stream=new FileStream(zipPath,FileMode.Open,FileAccess.Read,FileShare.Read);
        Phase7SqlEvidenceFiles.Require(stream.Length<=Phase7SqlEvidenceFiles.MaximumPackageBytes
            && Phase7SqlEvidenceFiles.IsHash(expectedSha256)
            && string.Equals(Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(stream)),expectedSha256,StringComparison.OrdinalIgnoreCase),"ARCHIVE_BINDING");
        Phase7SqlEvidenceFiles.Require(VerifyZip(zipPath,token,expectedSha256).IntegrityVerified,"ZIP_VERIFY");stream.Position=0;
        using var archive=new ZipArchive(stream,ZipArchiveMode.Read);var entry=archive.GetEntry("run-report.json")!;
        using var input=entry.Open();var bytes=new byte[checked((int)entry.Length)];input.ReadExactly(bytes);token.ThrowIfCancellationRequested();
        return Phase7SqlEvidenceFiles.Decode<Phase7SqlRunEvidence>(bytes);
    }

    private static void ValidateZipDirectory(FileStream stream)
    {
        // Bound central-directory metadata BEFORE ZipArchive materializes attacker-controlled entry arrays.
        Phase7SqlEvidenceFiles.Require(stream.Length is >=22 and <=Phase7SqlEvidenceFiles.MaximumPackageBytes,"ZIP_SIZE");
        var length=(int)Math.Min(stream.Length,65557);var tail=new byte[length];stream.Position=stream.Length-length;stream.ReadExactly(tail);
        var end=-1;
        for(var i=tail.Length-22;i>=0;i--)
            if(BitConverter.ToUInt32(tail,i)==0x06054b50 && i+22+BitConverter.ToUInt16(tail,i+20)==tail.Length){end=i;break;}
        Phase7SqlEvidenceFiles.Require(end>=0,"ZIP_DIRECTORY");
        var count=BitConverter.ToUInt16(tail,end+10);var size=BitConverter.ToUInt32(tail,end+12);var offset=BitConverter.ToUInt32(tail,end+16);
        Phase7SqlEvidenceFiles.Require(count is >1 and <=4096 && count==BitConverter.ToUInt16(tail,end+8)
            && BitConverter.ToUInt16(tail,end+4)==0 && BitConverter.ToUInt16(tail,end+6)==0
            && (long)offset+size==stream.Length-length+end,"ZIP_DIRECTORY");
        stream.Position=offset;long total=0;var header=new byte[46];
        for(var i=0;i<count;i++)
        {
            stream.ReadExactly(header);Phase7SqlEvidenceFiles.Require(BitConverter.ToUInt32(header)==0x02014b50,"ZIP_DIRECTORY");
            var name=BitConverter.ToUInt16(header,28);var extra=BitConverter.ToUInt16(header,30);var comment=BitConverter.ToUInt16(header,32);
            var uncompressed=BitConverter.ToUInt32(header,24);total+=uncompressed;
            Phase7SqlEvidenceFiles.Require(name is >0 and <=160 && extra<=1024 && comment<=1024 && uncompressed<=Phase7SqlEvidenceFiles.MaximumFileBytes
                && total<=Phase7SqlEvidenceFiles.MaximumPackageBytes && stream.Position+name+extra+comment<=(long)offset+size,"ZIP_DIRECTORY");
            stream.Position+=name+extra+comment;
        }
        Phase7SqlEvidenceFiles.Require(stream.Position==(long)offset+size,"ZIP_DIRECTORY");stream.Position=0;
    }
}
