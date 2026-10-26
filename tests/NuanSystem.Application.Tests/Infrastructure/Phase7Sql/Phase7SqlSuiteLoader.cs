using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Data;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal sealed record Phase7SqlSuiteInputs(Guid RunId,string ExecutionAuthorizationId,string ManifestSha256,string CatalogSha256,
    int IdentificationTypeIdA,int IdentificationTypeIdB,int MissingPartnerId,int MissingIdentificationTypeId,
    IReadOnlyDictionary<string,string> FaultTriggerSha256A,IReadOnlyDictionary<string,string> FaultTriggerSha256B,
    string BoundaryAttestation,string SourceInventorySha256,string BinaryInventorySha256,string OfflineBaselineSha256);

// This loader is reachable only from an explicitly enabled SQL Fact body, never test discovery/offline regression.
internal static class Phase7SqlSuiteLoader
{
    private static readonly JsonSerializerOptions Json=new(JsonSerializerDefaults.Web)
    {
        UnmappedMemberHandling=JsonUnmappedMemberHandling.Disallow,AllowDuplicateProperties=false,
        RespectRequiredConstructorParameters=true,RespectNullableAnnotations=true,MaxDepth=12
    };
    private static readonly object Sync=new();
    private static Phase7SqlSuiteProcessLease? lease;
    private static Phase7SqlFixture? preparedFixture;
    private static Phase7SqlSuiteInputs? preparedInputs;
    private static Phase7SqlSuiteEvidenceCapture? capture;
    private static bool stopped;

    public static IPhase7SqlExecutableRun FromEnvironment(string caseId)
    {
        try
        {
            lock(Sync)if(stopped)throw new Phase7SqlGuardException("PHASE7_SQL_SUITE_STOPPED");
            return Load(caseId);
        }
        catch
        {
            lock(Sync){stopped=true;lease?.Fail();DisableGates();}
            throw;
        }
    }
    private static void DisableGates()
    {
        Environment.SetEnvironmentVariable(SqlServerIntegrationFactAttribute.EnabledEnvironmentVariable,"0");
        Environment.SetEnvironmentVariable(Phase7SqlGates.IsolatedEnvironmentVariable,"0");
        try{capture?.Stop(preparedFixture,lease);}finally{preparedFixture?.Dispose();}
    }
    private static IPhase7SqlExecutableRun Load(string caseId)
    {
        var gates=Phase7SqlGates.ReadEnvironment();
        if(!gates.StandardEnabled || !gates.IsolatedEnabled)throw new Phase7SqlGuardException("PHASE7_SQL_GATES_DISABLED");
        if(SqlServerIntegrationFactAttribute.IsEnabled(Environment.GetEnvironmentVariable("NUAN_RUN_SAP_PO_PILOT")))
            throw new Phase7SqlGuardException("PHASE7_SQL_SUITE_SAP_GATE");
        // Paths are explicit operator inputs, with no appsettings, discovery or connection-string fallback.
        var manifestJson=ReadRequired("NUANSYSTEM_PHASE7_MANIFEST_PATH");
        var approvalJson=ReadRequired("NUANSYSTEM_PHASE7_APPROVAL_PATH");
        var approval=Parse<Phase7SqlApproval>(approvalJson);
        var inputsJson=ReadRequired("NUANSYSTEM_PHASE7_SUITE_INPUTS_PATH");
        var inputs=Parse<Phase7SqlSuiteInputs>(inputsJson);
        var candidateJson=new UTF8Encoding(false,true).GetString(ReadBytesRequired("NUANSYSTEM_PHASE7_CANDIDATE_PATH",1024*1024));
        var candidate=Phase7SqlEvidenceFiles.Decode<Phase7SqlCandidate>(Encoding.UTF8.GetBytes(candidateJson));
        var hash=Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(manifestJson+"\n"+approvalJson+"\n"+inputsJson+"\n"+candidateJson)));
        var root=ResolveRunRoot(inputs.RunId,Environment.GetEnvironmentVariable("NUANSYSTEM_PHASE7_RUN_ROOT"),Environment.CurrentDirectory);
        Phase7SqlSuiteProcessLease current;
        lock(Sync)
        {
            if(lease is null)
            {
                var validated=Phase7SqlManifestValidator.Validate(manifestJson,approval,gates,DateTimeOffset.UtcNow);
                ValidateInputs(inputs,validated.Manifest,validated.ManifestSha256);
                candidate.Verify();
                Phase7SqlEvidenceFiles.Require(candidate.SourceSha256==inputs.SourceInventorySha256 && candidate.BinarySha256==inputs.BinaryInventorySha256,"CANDIDATE_BINDING");
                Phase7SqlSuiteHostHandshake.Notify(inputs.RunId,validated.Manifest.CommandTimeoutSeconds);
                var baseline=ReadBytesRequired("NUANSYSTEM_PHASE7_OFFLINE_BASELINE_PATH",16*1024*1024);
                Phase7SqlEvidenceFiles.Require(Phase7SqlEvidenceFiles.Hash(baseline)==inputs.OfflineBaselineSha256,"BASELINE_HASH");
                Directory.CreateDirectory(root);
                using var claim=new FileStream(Path.Combine(root,"execution.claim"),FileMode.CreateNew,FileAccess.Write,FileShare.None);
                lease=new(inputs.RunId,hash,TimeSpan.FromSeconds(validated.Manifest.RunTimeoutSeconds),DisableGates);
                SaveText(Path.Combine(root,"manifest.json"),manifestJson);
                SaveText(Path.Combine(root,"approval.json"),approvalJson);
                SaveText(Path.Combine(root,"suite-inputs.json"),inputsJson);
                capture=new(root,validated.Manifest,candidate,inputs,baseline);capture.Start();
                lease.Take(inputs.RunId,hash,caseId);
                var manifest=validated.Manifest;
                var a=Connections(manifest,manifest.TenantA,false,Environment.GetEnvironmentVariable,()=>lease.Pooling);
                var b=Connections(manifest,manifest.TenantB,false,Environment.GetEnvironmentVariable,()=>lease.Pooling);
                preparedFixture=new Phase7SqlFixture(manifestJson,approval,a,b,capture.Verifier());
                preparedInputs=inputs;
            }
            else Continue(preparedFixture!,lease,inputs.RunId,hash,caseId);
            current=lease;
        }
        var directory=Path.Combine(root,caseId.Replace('.','_'));
        Directory.CreateDirectory(directory);
        try
        {
            return new Phase7SqlSuiteRuntime(preparedFixture!,preparedInputs!,current,directory,capture!);
        }
        catch{current.Fail();throw;}
    }

    internal static Phase7SqlFixture Continue(Phase7SqlFixture fixture,Phase7SqlSuiteProcessLease current,
        Guid runId,string configurationHash,string caseId)
    {
        current.Take(runId,configurationHash,caseId);
        // Real clock/gates/approval window and the original fixture lifetime remain active.
        // Do not repeat the initial full-run-budget admission test during a valid continuation.
        fixture.EnsureAuthorized(default);
        return fixture;
    }

    internal static string ResolveRunRoot(Guid runId,string? explicitRoot,string workingDirectory)
    {
        if(string.IsNullOrEmpty(explicitRoot))return Path.GetFullPath(Path.Combine(workingDirectory,"artifacts","phase7-sql-executions",runId.ToString("N")));
        Phase7SqlEvidenceFiles.Require(runId!=Guid.Empty && Path.IsPathFullyQualified(explicitRoot),"SUITE_ROOT");
        var root=Path.TrimEndingDirectorySeparator(Path.GetFullPath(explicitRoot));
        Phase7SqlEvidenceFiles.Require(Path.GetFileName(root)==runId.ToString("N")
            && Path.GetFileName(Path.GetDirectoryName(root))=="phase7-sql-executions"
            && Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(root)))=="artifacts","SUITE_ROOT");
        Phase7SqlEvidenceFiles.CheckNoLinks(root);return root;
    }

    internal static void ValidateInputs(Phase7SqlSuiteInputs inputs,Phase7SqlManifest manifest,string manifestHash)
    {
        if(inputs.RunId!=manifest.RunId || inputs.ExecutionAuthorizationId!=manifest.ExecutionAuthorizationId
            || !string.Equals(inputs.ManifestSha256,manifestHash,StringComparison.OrdinalIgnoreCase)
            || inputs.CatalogSha256!=Phase7SqlExecutionCatalog.Sha256 || inputs.IdentificationTypeIdA<=0
            || !Phase7SqlEvidenceFiles.IsHash(inputs.SourceInventorySha256) || !Phase7SqlEvidenceFiles.IsHash(inputs.BinaryInventorySha256)
            || !Phase7SqlEvidenceFiles.IsHash(inputs.OfflineBaselineSha256)
            || inputs.IdentificationTypeIdB!=inputs.IdentificationTypeIdA || inputs.MissingPartnerId<=0 || inputs.MissingIdentificationTypeId>=0
            || inputs.BoundaryAttestation!="SYNTHETIC_LAB_ONLY_SAP_NONE_WORKERS_OFF_RELAY_OFF")
            throw new Phase7SqlGuardException("PHASE7_SQL_SUITE_INPUTS");
        CheckTriggers(inputs.FaultTriggerSha256A,manifest.RunId);CheckTriggers(inputs.FaultTriggerSha256B,manifest.RunId);
    }
    private static void CheckTriggers(IReadOnlyDictionary<string,string>? hashes,Guid run)
    {
        var names=new[]{"Audit","Outbox","SyncAudit"}.Select(s=>$"TR_P7_{run:N}_{s}").ToArray();
        if(hashes is null || hashes.Count!=3 || names.Any(n=>!hashes.TryGetValue(n,out var hash) || hash is not {Length:64}
            || !hash.All(Uri.IsHexDigit) || hash.All(c=>c=='0')))
            throw new Phase7SqlGuardException("PHASE7_SQL_SUITE_TRIGGER_INPUTS");
    }

    internal static Phase7SqlConnections Connections(Phase7SqlManifest manifest,Phase7SqlTarget target,bool pooling,Func<string,string?> resolve,Func<bool>? poolingPolicy=null)
    {
        string Connection(bool observer)
        {
            var reference=observer?target.ObserverSecretReference:target.ApplicationSecretReference;
            var expected=$"env:NUANSYSTEM_PHASE7_{(observer?"OBSERVER":"TENANT")}_{target.Role}_CONNECTION_STRING";
            if(reference!=expected)throw new Phase7SqlGuardException("PHASE7_SQL_SUITE_SECRET_REFERENCE");
            var secret=resolve(expected[4..]);
            if(string.IsNullOrEmpty(secret))throw new Phase7SqlGuardException("PHASE7_SQL_SUITE_SECRET_MISSING");
            Phase7SqlFixture.ValidateConnectionString(secret,manifest,target,observer);
            return new SqlConnectionStringBuilder(secret){Pooling=!observer && pooling,
                ApplicationName=Phase7SqlMonitorReader.ApplicationName(manifest.RunId,target.Role,observer)}.ConnectionString;
        }
        var writer=Connection(false);var observer=Connection(true);
        return new(new Factory(()=>new SqlConnectionStringBuilder(writer){Pooling=poolingPolicy?.Invoke()??pooling}.ConnectionString),new Factory(()=>observer));
    }
    private sealed class Factory(Func<string> connectionString):ITenantConnectionFactory
    {
        public IDbConnection CreateConnection()=>new SqlConnection(connectionString());
    }
    private static T Parse<T>(string text)
    {
        try{return JsonSerializer.Deserialize<T>(text,Json)??throw new JsonException();}
        catch{throw new Phase7SqlGuardException("PHASE7_SQL_SUITE_JSON");}
    }
    private static void SaveText(string path,string text)
    {
        using var stream=new FileStream(path,FileMode.CreateNew,FileAccess.Write,FileShare.None);
        using var writer=new StreamWriter(stream,new UTF8Encoding(false));writer.Write(text);
    }
    private static string ReadRequired(string variable)
    {
        try
        {
            var path=Environment.GetEnvironmentVariable(variable);
            if(string.IsNullOrEmpty(path) || !Path.IsPathFullyQualified(path))throw new IOException();
            using var stream=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.Read);
            if(stream.Length>65_536)throw new IOException();
            using var reader=new StreamReader(stream,new UTF8Encoding(false,true));
            var text=reader.ReadToEnd();if(text.Length>65_536)throw new IOException();return text;
        }
        catch{throw new Phase7SqlGuardException("PHASE7_SQL_SUITE_FILE");}
    }
    private static byte[] ReadBytesRequired(string variable,int maximum)
    {
        var path=Environment.GetEnvironmentVariable(variable);
        Phase7SqlEvidenceFiles.Require(path is not null && Path.IsPathFullyQualified(path),"INPUT_PATH");
        return Phase7SqlEvidenceFiles.Read(Path.GetDirectoryName(path)!,Path.GetFileName(path),maximum,default);
    }
}

internal sealed class Phase7SqlSuiteProcessLease(Guid runId,string configurationHash,TimeSpan budget,Action? terminal=null,TimeProvider? timeProvider=null)
{
    private readonly TimeProvider clock=timeProvider??TimeProvider.System;
    private readonly long started=(timeProvider??TimeProvider.System).GetTimestamp();
    private readonly HashSet<string> taken=new(StringComparer.Ordinal);
    private readonly HashSet<string> completed=new(StringComparer.Ordinal);
    private bool failed;
    public bool Failed=>failed;
    public int CurrentSequence=>taken.Count;
    public IReadOnlyList<string> CompletedCases=>completed.Order(StringComparer.Ordinal).ToArray();
    public Exception? TerminalFailure {get;private set;}
    public bool CanFinalize(string id)=>!failed && completed.Count==20 && taken.Contains(id) && !completed.Contains(id);
    private string? currentCase;
    public bool Pooling=>currentCase is "S12" or "S13" or "S14" or "S15" or "S16" or "S18";
    public TimeSpan Remaining
    {
        get
        {
            var left=budget-clock.GetElapsedTime(started);
            if(failed || left<=TimeSpan.Zero)throw new Phase7SqlGuardException("PHASE7_SQL_SUITE_STOPPED");
            return left;
        }
    }
    public void Take(Guid run,string hash,string id)
    {
        _=Remaining;
        if(run!=runId || hash!=configurationHash || !Phase7SqlExecutionCatalog.Cases.Any(c=>c.Id==id) || !taken.Add(id))
            throw new Phase7SqlGuardException("PHASE7_SQL_SUITE_REPLAY_OR_DRIFT");
        currentCase=id;
    }
    public void Complete(string id)
    {
        _=Remaining;
        if(!taken.Contains(id) || !completed.Add(id))throw new Phase7SqlGuardException("PHASE7_SQL_SUITE_COMPLETION");
        if(completed.Count==Phase7SqlExecutionCatalog.Cases.Count)terminal?.Invoke();
    }
    public void Fail(){failed=true;try{terminal?.Invoke();}catch(Exception error){TerminalFailure=error;}}
}
