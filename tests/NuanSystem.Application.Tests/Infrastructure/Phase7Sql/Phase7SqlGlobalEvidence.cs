using System.Globalization;
using System.Text.Json.Nodes;
using System.Xml;
using System.Xml.Linq;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal sealed record Phase7SqlDestinationCapture(string Role,bool Observer,DateTimeOffset CapturedAtUtc,
    Phase7SqlDestinationObservation Observation,string SchemaInventory,IReadOnlyDictionary<string,string> ProcedureDefinitions);
internal sealed record Phase7SqlTriggerCapture(string Name,string? Definition,bool IsDisabled);
internal sealed record Phase7SqlPrerequisiteCapture(string CaseId,string Role,DateTimeOffset CapturedAtUtc,int LookupId,
    int MissingIdsFound,IReadOnlyList<Phase7SqlTriggerCapture> Triggers);
internal sealed record Phase7SqlRuntimeStart(Guid RunId,int ProcessId,DateTimeOffset StartedAtUtc,string SourceSha256,
    string BinarySha256,string BaselineSha256,string CatalogSha256,string ExecutingAssemblySha256);
internal sealed record Phase7SqlRuntimeEnd(Guid RunId,int ProcessId,DateTimeOffset StoppedAtUtc,bool Completed,
    IReadOnlyList<string> CompletedCases,bool StandardGateEnabled,bool IsolatedGateEnabled,int ExternalBoundaryAttempts,bool SapExecuted,int WorkerExecutions);
internal sealed record Phase7SqlRetainedDatabase(string Role,string DatabaseName,int DatabaseId,Guid RunMarker,string Purpose,
    IReadOnlyDictionary<string,long> RowCounts,string SnapshotSha256);
internal sealed record Phase7SqlMonitorObservation(Guid RunId,string ServerName,string ProductVersion,string Principal,
    string AuthorizationReference,DateTimeOffset ObservedAtUtc,int RemainingSessions,long TransactionsA,long TransactionsB,
    IReadOnlyList<Phase7SqlRetainedDatabase> Databases);
internal sealed record Phase7SqlIndependentWitness(Guid RunId,int ProcessId,bool HasExited,int? ExitCode,
    DateTimeOffset? ProcessExitObservedAtUtc,string Inspector,string AuthorizationReference,
    DateTimeOffset WorkersOffFromUtc,DateTimeOffset WorkersOffThroughUtc,IReadOnlyList<string> DisabledComponents,
    Phase7SqlEvidenceOrigin Origin);
internal sealed record Phase7SqlTraceNode(Guid EvidenceScopeId,IReadOnlyList<Phase7SqlSample> Samples,IReadOnlyList<Phase7SqlScopeState> ScopeStates,
    int TotalWriteConnections,int ObserverConnectionCount,IReadOnlyList<Phase7SqlTraceNode> Independent);
internal sealed record Phase7SqlTracePair(Phase7SqlTraceNode A,Phase7SqlTraceNode B);
internal sealed record Phase7SqlCaseTiming(string Id,int Sequence,DateTimeOffset StartedAtUtc,DateTimeOffset FinishedAtUtc,
    Phase7SqlTracePair Before,bool FinalSnapshotsCaptured);
internal sealed record Phase7SqlAttemptEvidence(bool? Success,IReadOnlyList<string> ErrorCodes,string? ErrorType,IReadOnlyList<int> SqlNumbers);
internal sealed record Phase7SqlTrxInfo(DateTimeOffset Start,DateTimeOffset Finish,int Tests);

internal static class Phase7SqlEvidenceTrx
{
    public const string OfflineFilter="Category!=SqlServerIntegration&FullyQualifiedName!~SapPurchaseOrderPilotIntegrationTests";
    public static Phase7SqlTrxInfo Inspect(byte[] bytes,bool baseline)
    {
        Phase7SqlEvidenceFiles.Require(bytes.Length>0 && bytes.Length<=(baseline?16*1024*1024:1024*1024),"TRX_SIZE");
        try
        {
            using var stream=new MemoryStream(bytes,false);
            using var reader=XmlReader.Create(stream,new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null,MaxCharactersInDocument=16*1024*1024});
            var doc=XDocument.Load(reader);XNamespace ns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010";
            var root=doc.Root!;Phase7SqlEvidenceFiles.Require(root.Name==ns+"TestRun" && root.Elements(ns+"Times").Count()==1,"TRX");
            var times=root.Element(ns+"Times")!;
            DateTimeOffset Time(string name)
            {
                var text=(string?)times.Attribute(name);
                Phase7SqlEvidenceFiles.Require(text is not null && System.Text.RegularExpressions.Regex.IsMatch(text,"(Z|[+-][0-9]{2}:[0-9]{2})\\z")
                    && DateTimeOffset.TryParse(text,CultureInfo.InvariantCulture,DateTimeStyles.None,out _),"TRX_TIME");
                return DateTimeOffset.Parse(text!,CultureInfo.InvariantCulture).ToUniversalTime();
            }
            var start=Time("start");var finish=Time("finish");Phase7SqlEvidenceFiles.Require(start<finish,"TRX_TIME");
            var results=root.Element(ns+"Results")!.Elements().ToArray();
            if(baseline)
            {
                var summary=root.Element(ns+"ResultSummary")!;var counts=summary.Element(ns+"Counters")!;
                int Count(string name)=>int.TryParse((string?)counts.Attribute(name),out var count)?count:-1;
                Phase7SqlEvidenceFiles.Require(results.Length>0 && results.Length<=20000 && (string?)summary.Attribute("outcome")=="Completed"
                    && new[]{"total","executed","passed"}.All(k=>Count(k)==results.Length)
                    && new[]{"failed","error","timeout","aborted","inconclusive","passedButRunAborted","notRunnable","notExecuted","disconnected","warning","completed","inProgress","pending"}.All(k=>Count(k)==0)
                    && !doc.Descendants(ns+"ErrorInfo").Any() && results.All(r=>r.Name==ns+"UnitTestResult" && (string?)r.Attribute("outcome")=="Passed"
                        && !((string?)r.Attribute("testName")??"").StartsWith(Phase7SqlExecutionCatalog.ExpectedTestClass,StringComparison.Ordinal)
                        && !((string?)r.Attribute("testName")??"").Contains("SapPurchaseOrderPilotIntegrationTests",StringComparison.Ordinal)),"OFFLINE_BASELINE");
                Phase7SqlEvidenceFiles.Require(root.Elements(ns+"Results").Count()==1 && root.Elements(ns+"ResultSummary").Count()==1
                    && root.Elements(ns+"TestDefinitions").Count()==1 && doc.Descendants(ns+"RunInfo").All(r=>(string?)r.Attribute("outcome") is "Passed" or "Completed"),"BASELINE_STRUCTURE");
                var definitions=root.Element(ns+"TestDefinitions")!.Elements().ToArray();
                var executions=new HashSet<Guid>();var ids=new HashSet<Guid>();
                foreach(var result in results)
                {
                    Phase7SqlEvidenceFiles.Require(Guid.TryParse((string?)result.Attribute("testId"),out var id) && id!=Guid.Empty
                        && Guid.TryParse((string?)result.Attribute("executionId"),out var execution) && execution!=Guid.Empty && executions.Add(execution),"BASELINE_IDS");
                    ids.Add(id);
                }
                Phase7SqlEvidenceFiles.Require(definitions.Length==ids.Count && definitions.All(d=>d.Name==ns+"UnitTest"),"BASELINE_DEFINITIONS");
                foreach(var id in ids)
                {
                    var found=definitions.Where(d=>Guid.TryParse((string?)d.Attribute("id"),out var value) && value==id).ToArray();
                    Phase7SqlEvidenceFiles.Require(found.Length==1 && found[0].Elements(ns+"TestMethod").Count()==1,"BASELINE_DEFINITIONS");
                    var method=found[0].Element(ns+"TestMethod")!;var type=(string?)method.Attribute("className")??"";var name=(string?)method.Attribute("name")??"";
                    Phase7SqlEvidenceFiles.Require(type.Length>0 && name.Length>0 && !ForbiddenBaselineMethods.Contains(type+"."+name)
                        && type!=Phase7SqlExecutionCatalog.ExpectedTestClass && !type.Contains("SapPurchaseOrderPilotIntegrationTests",StringComparison.Ordinal),"BASELINE_LIVE_TEST");
                }
            }
            return new(start,finish,results.Length);
        }
        catch(Phase7SqlGuardException){throw;}
        catch{throw new Phase7SqlGuardException("PHASE7_SQL_EVIDENCE_TRX");}
    }
    private static readonly HashSet<string> ForbiddenBaselineMethods=typeof(Phase7SqlEvidenceTrx).Assembly.GetTypes()
        .SelectMany(type=>type.GetMethods().Where(method=>
            type.GetCustomAttributesData().Concat(method.GetCustomAttributesData()).Any(a=>
                typeof(SqlServerIntegrationFactAttribute).IsAssignableFrom(a.AttributeType)
                || a.AttributeType==typeof(Phase7IsolatedSqlFactAttribute)
                || a.AttributeType==typeof(TraitAttribute) && a.ConstructorArguments.Count==2
                    && (string?)a.ConstructorArguments[0].Value=="Category" && (string?)a.ConstructorArguments[1].Value=="SqlServerIntegration"))
            .Select(method=>type.FullName+"."+method.Name)).ToHashSet(StringComparer.Ordinal);
}

internal static class Phase7SqlGlobalControls
{
    public static bool WorkerWitness(Phase7SqlIndependentWitness witness,Phase7SqlRuntimeStart start,Phase7SqlTrxInfo trx)=>
        witness.RunId==start.RunId && witness.ProcessId==start.ProcessId && witness.Origin==Phase7SqlEvidenceOrigin.SqlServerLaboratory
        && !string.IsNullOrWhiteSpace(witness.Inspector) && !string.IsNullOrWhiteSpace(witness.AuthorizationReference)
        && witness.WorkersOffFromUtc.Offset==TimeSpan.Zero && witness.WorkersOffThroughUtc.Offset==TimeSpan.Zero
        && witness.WorkersOffFromUtc<=trx.Start && witness.WorkersOffThroughUtc>=trx.Finish
        && witness.DisabledComponents.Count==4 && witness.DisabledComponents.ToHashSet(StringComparer.Ordinal)
            .SetEquals(["SyncWorker","MasterBranchSyncWorker","SriWorker","Relay"]);

    public static bool Termination(Phase7SqlMonitorObservation monitor,Phase7SqlIndependentWitness witness,
        Phase7SqlManifest manifest,Phase7SqlRuntimeStart start,Phase7SqlRuntimeEnd end,Phase7SqlTrxInfo trx)=>
        WorkerWitness(witness,start,trx) && witness.HasExited && witness.ExitCode==0 && witness.ProcessExitObservedAtUtc is {Offset.Ticks:0} exited
        && exited>=end.StoppedAtUtc && monitor.RunId==manifest.RunId && monitor.ServerName==manifest.Server.ExpectedServerName
        && monitor.ProductVersion==manifest.Server.ProductVersion && !string.IsNullOrWhiteSpace(monitor.Principal)
        && !new[]{"sa","dbo","guest",manifest.TenantA.ApplicationPrincipal,manifest.TenantB.ApplicationPrincipal,
            manifest.TenantA.ObserverPrincipal,manifest.TenantB.ObserverPrincipal}.Contains(monitor.Principal,StringComparer.OrdinalIgnoreCase)
        && !string.IsNullOrWhiteSpace(monitor.AuthorizationReference) && monitor.ObservedAtUtc.Offset==TimeSpan.Zero
        && monitor.ObservedAtUtc>=exited && monitor.ObservedAtUtc>=trx.Finish
        && monitor.RemainingSessions==0 && monitor.TransactionsA==0 && monitor.TransactionsB==0;

    public static void ValidatePhysicalSnapshot(byte[] bytes)
    {
        Phase7SqlEvidenceFiles.ValidateJson(bytes);
        var root=JsonNode.Parse(bytes)!.AsObject();
        Phase7SqlEvidenceFiles.Require(root.Count==Phase7SqlRollbackSnapshot.RequiredTables.Count
            && Phase7SqlRollbackSnapshot.RequiredTables.All(t=>root[t] is JsonArray a && a.All(r=>r is JsonObject)),"SNAPSHOT");
        _=new Phase7SqlRollbackSnapshot(Phase7SqlRollbackSnapshot.RequiredTables.ToDictionary(t=>t,t=>root[t]!.AsArray()));
    }
    public static void ValidateTrace(Phase7SqlTraceNode node,Phase7SqlTarget target)
    {
        Phase7SqlEvidenceFiles.Require(node.TotalWriteConnections>=0 && node.ObserverConnectionCount>=0
            && node.Samples.All(s=>s.ExecutionId!=Guid.Empty && s.Session.Spid>0 && s.Session.DatabaseName==target.DatabaseName
                && s.Session.DatabaseId==target.DatabaseId && s.Session.TransactionCount==1 && s.Session.TransactionState==1)
            && node.TotalWriteConnections==node.Samples.Select(s=>s.ExecutionId).Distinct().Count(),"CONNECTION_TRACE");
        foreach(var execution in node.Samples.GroupBy(s=>s.ExecutionId))
            Phase7SqlEvidenceFiles.Require(execution.Count(s=>s.Checkpoint=="Entry")==1 && execution.Select(s=>s.Session.Spid).Distinct().Count()==1,"SESSION_TRACE");
        foreach(var state in node.ScopeStates)Phase7SqlScopeHygiene.AssertClean(state,target.DatabaseName,target.DatabaseId);
        foreach(var child in node.Independent)ValidateTrace(child,target);
    }
    public static bool Retained(Phase7SqlMonitorObservation monitor,Phase7SqlManifest manifest,IReadOnlyDictionary<string,byte[]> finalSnapshots)
    {
        if(monitor.Databases.Count!=2)return false;
        foreach(var target in new[]{manifest.TenantA,manifest.TenantB})
        {
            var rows=monitor.Databases.Where(d=>d.Role==target.Role).ToArray();
            if(rows.Length!=1 || rows[0].DatabaseName!=target.DatabaseName || rows[0].DatabaseId!=target.DatabaseId
                || rows[0].RunMarker!=manifest.RunId || rows[0].Purpose!=target.Purpose
                || !finalSnapshots.TryGetValue(target.Role,out var bytes))return false;
            ValidatePhysicalSnapshot(bytes);var snapshot=JsonNode.Parse(bytes)!.AsObject();
            if(rows[0].SnapshotSha256!=Phase7SqlEvidenceFiles.Hash(bytes))return false;
            if(rows[0].RowCounts.Count!=16 || Phase7SqlRollbackSnapshot.RequiredTables.Any(t=>
                !rows[0].RowCounts.TryGetValue(t,out var count) || count!=snapshot[t]!.AsArray().Count))return false;
        }
        return true;
    }

    public static void ErrorDiagnostics(IReadOnlyList<Phase7SqlEvidenceBundleFile> files)
    {
        JsonNode Read(string name)
        {
            var matches=files.Where(f=>f.RelativePath==name).ToArray();Phase7SqlEvidenceFiles.Require(matches.Length==1,"DIAGNOSTIC_MISSING");
            Phase7SqlEvidenceFiles.ValidateJson(matches[0].Bytes);return JsonNode.Parse(matches[0].Bytes)!;
        }
        int[] Numbers(JsonNode error)=>error["SqlNumbers"]!.AsArray().Select(n=>n!.GetValue<int>()).ToArray();
        foreach(var number in Enumerable.Range(5,7))foreach(var operation in new[]{"C","U","D"})
        {
            var fault=(Phase7SqlFailureCase)number;var item=Read($"S{number:00}/fault-{operation}.json");var error=item["Error"]!;
            Phase7SqlEvidenceFiles.Require(item["Case"]!.GetValue<string>()==fault.ToString(),"DIAGNOSTIC_CASE");
            var type=error["Type"]!.GetValue<string>();
            var valid=number switch
            {
                5 or 6 or 7=>type=="SqlException" && Numbers(error).Contains(51500+number),
                8=>type==nameof(Phase7SqlAfterProcedureException),
                9=>type=="SqlException" && Numbers(error).Contains(547),
                _=>item["BarrierAcknowledged"]!.GetValue<bool>() && type is "OperationCanceledException" or "TaskCanceledException"
            };
            Phase7SqlEvidenceFiles.Require(valid,"DIAGNOSTIC_CLASSIFICATION");
        }
        foreach(var id in new[]{"R01_Legacy","R01_Pilot"})
        {
            var item=Read(id+"/internal-rollback.json");var error=item["Error"]!;var type=error["Type"]!.GetValue<string>();
            var numbers=Numbers(error);var rejected=numbers.Contains(3915);
            Phase7SqlEvidenceFiles.Require(item["attempted"]!.GetValue<bool>()
                && (type=="SqlException" && numbers.Any(n=>n is 266 or 3902 or 3903 or 3915) && !numbers.Any(n=>n is -2 or 1205 or 1222)
                    || item["returnedFalse"]!.GetValue<bool>() && type=="InvalidOperationException")
                && item["Disposition"]!.GetValue<string>()==(rejected?"InsertExecRollbackRejected":"TransactionInvalidatedOrCompleted"),"R01_DIAGNOSTIC");
        }
        Phase7SqlEvidenceFiles.Require(Read("S17/owner-close.json")["Type"]!.GetValue<string>()==nameof(Phase7SqlOwnerClosedException),"OWNER_DIAGNOSTIC");
        var race=Phase7SqlEvidenceFiles.Decode<Phase7SqlAttemptEvidence[]>(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(Read("S15/concurrent.json")));
        Phase7SqlEvidenceFiles.Require(race.Length==2 && race.Count(r=>r.Success==true)==1,"RACE_DIAGNOSTIC");
        var winner=race.Single(r=>r.Success==true);var loser=race.Single(r=>r.Success!=true);
        Phase7SqlEvidenceFiles.Require(winner.ErrorType is null && winner.ErrorCodes.Count==0 && winner.SqlNumbers.Count==0
            && (loser.Success==false && loser.ErrorType is null && loser.SqlNumbers.Count==0 && loser.ErrorCodes.Count>0
                && loser.ErrorCodes.All(c=>c is "BusinessPartnerCodeAlreadyExists" or "BusinessPartnerIdentificationAlreadyExists")
                || loser.Success is null && loser.ErrorType=="SqlException" && loser.ErrorCodes.Count==0
                    && !loser.SqlNumbers.Contains(-2) && loser.SqlNumbers.Any(n=>n is 2601 or 2627 or 1205)),"RACE_CONFLICT_CLASSIFICATION");
        var repeat=Read("S16/repetition.json");
        Phase7SqlEvidenceFiles.Require(repeat["EventDelta"]!.GetValue<int>()==1 && repeat["SyncAuditDelta"]!.GetValue<int>()==1
            && repeat["AuditDelta"]!.GetValue<int>()>=0,"REPETITION_DIAGNOSTIC");
    }

    public static void ValidateDiagnosticExport(byte[] bytes)
    {
        Phase7SqlEvidenceFiles.ValidateJson(bytes);var node=JsonNode.Parse(bytes)!;
        void Check(JsonNode? value)
        {
            if(value is JsonObject obj)foreach(var (key,child) in obj)
            {
                Phase7SqlEvidenceFiles.Require(!new[]{"password","connectionstring","access_token","apikey","innerexception","stacktrace","message"}
                    .Contains(key,StringComparer.OrdinalIgnoreCase),"UNSAFE_DIAGNOSTIC");
                Check(child);
            }
            else if(value is JsonArray array)foreach(var child in array)Check(child);
            else if(value is JsonValue scalar && scalar.TryGetValue<string>(out var text))
                Phase7SqlEvidenceFiles.Require(!System.Text.RegularExpressions.Regex.IsMatch(text,"(?i)(password|connectionstring|access_token|api[_-]?key)[\"']?\\s*[:=]"),"UNSAFE_DIAGNOSTIC");
        }
        Check(node);
    }
}
