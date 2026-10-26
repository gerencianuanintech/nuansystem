using System.Data;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal delegate Task<string> Phase7SqlSnapshotRead(IDbConnection connection, int timeoutSeconds, CancellationToken token);

// Observer only. Full physical evidence is restricted to the explicitly approved, disposable laboratory DB.
// Business DTOs project stored values; lookup display labels are outside this evidence contract.
internal sealed class Phase7SqlSnapshotReaders
{
    private static readonly string[] Scalars = ["BusinessPartnerFiscalData", "BusinessPartnerAccountingSettings",
        "BusinessPartnerCreditSettings", "BusinessPartnerPurchaseSettings", "BusinessPartnerSapMapping"];
    private static readonly JsonSerializerOptions Json = new() { Converters = { new JsonStringEnumConverter() } };
    private readonly Phase7SqlManifest manifest;
    private readonly Phase7SqlTarget target;
    private readonly string code;
    private readonly IPhase7SqlDestinationVerifier verifier;
    private readonly Phase7SqlSnapshotRead read;

    public Phase7SqlSnapshotReaders(Phase7SqlManifest manifest, Phase7SqlTarget target, string caseCode,
        IPhase7SqlDestinationVerifier verifier, Phase7SqlSnapshotRead? read = null)
    {
        if (!(ReferenceEquals(target, manifest.TenantA) || ReferenceEquals(target, manifest.TenantB))
            || manifest.RunId == Guid.Empty || target.RunMarker != manifest.RunId || target.DatabaseId <= 4
            || target.DatabaseName != $"NuanSystem_Phase7_{manifest.RunId:N}_{target.Role}" || target.CompanyId <= 0
            || target.Role is not ("A" or "B") || target.Purpose != "BackendTransactionValidation"
            || !Regex.IsMatch(caseCode ?? "", "\\AP7_" + manifest.RunId.ToString("N")[..8] + "_S[0-9]{2}[A-Za-z0-9]{0,4}\\z",RegexOptions.IgnoreCase|RegexOptions.CultureInvariant))
            throw new Phase7SqlGuardException("PHASE7_SQL_SNAPSHOT_SCOPE");
        ArgumentNullException.ThrowIfNull(verifier);
        this.manifest=manifest; this.target=target; code=caseCode!.ToUpperInvariant(); this.verifier=verifier;
        this.read=read ?? ((c,t,ct)=>Phase7SqlSnapshotSqlReader.ReadAsync(c,manifest,target,t,ct));
    }

    public async Task<Phase7SqlRollbackSnapshot> ReadRollbackAsync(IDbConnection connection, int timeoutSeconds, CancellationToken token) =>
        new(await ReadTablesAsync(connection,timeoutSeconds,token));

    public async Task<Phase7NominalSnapshot> ReadNominalAsync(IDbConnection connection, int id, int timeoutSeconds, CancellationToken token)
    {
        if(id<=0)throw new Phase7SqlGuardException("PHASE7_SQL_SNAPSHOT_ID");
        var tables=await ReadTablesAsync(connection,timeoutSeconds,token);
        var result=Projection(()=>
        {
            var partners=Rows(tables,"BusinessPartners").Where(p=>Text(p,"Code")==code && Number(p,"Id")==id).ToArray();
            Require(partners.Length==1,"PARTNER_MISSING_OR_AMBIGUOUS");
            var p=partners[0]; var state=Project(tables,[p]);
            return new Phase7NominalSnapshot(state.Partners.Single(),Boolean(p,"IsDeleted"),state.Audits,state.Events,state.SyncAudits);
        });
        token.ThrowIfCancellationRequested();
        return result;
    }

    public async Task<Phase7SqlReliabilitySnapshot> ReadReliabilityAsync(IDbConnection connection,int timeoutSeconds,CancellationToken token)
    {
        var tables=await ReadTablesAsync(connection,timeoutSeconds,token);
        var result=Projection(()=>Project(tables,Rows(tables,"BusinessPartners").Where(p=>Text(p,"Code")==code).ToArray()));
        token.ThrowIfCancellationRequested();
        return result;
    }

    private async Task<IReadOnlyDictionary<string,JsonArray>> ReadTablesAsync(IDbConnection connection,int timeout,CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        Require(connection.State==ConnectionState.Open && timeout is >=1 and <=120 && timeout<=manifest.CommandTimeoutSeconds,"HANDLES_OR_TIMEOUT");
        await verifier.VerifyAsync(manifest,target,true,connection,null,token);
        try
        {
            var text=await read(connection,timeout,token);
            token.ThrowIfCancellationRequested();
            Require(connection.State==ConnectionState.Open && text is not null && text.Length<=8*1024*1024,"SIZE_OR_HANDLES");
            using var document=JsonDocument.Parse(text!,new JsonDocumentOptions { MaxDepth=64 });
            RejectDuplicates(document.RootElement);
            var root=JsonNode.Parse(text!)!.AsObject();
            Require(root.Count==Phase7SqlRollbackSnapshot.RequiredTables.Count,"TABLES");
            var result=new Dictionary<string,JsonArray>(StringComparer.Ordinal);
            foreach(var table in Phase7SqlRollbackSnapshot.RequiredTables)
            {
                Require(root[table] is JsonArray,"TABLES");
                var rows=root[table]!.AsArray();
                Require(rows.All(r=>r is JsonObject),"ROWS");
                var key=Phase7SqlSnapshotSqlReader.PrimaryKey(table);
                var ordered=rows.Select(r=>r!.AsObject()).OrderBy(r=>Number(r,key)).ToArray();
                Require(ordered.All(r=>Number(r,key)>0) && ordered.Select(r=>Number(r,key)).Distinct().Count()==ordered.Length,"ROW_IDENTITY");
                result.Add(table,new JsonArray(ordered.Select(r=>r.DeepClone()).ToArray()));
            }
            token.ThrowIfCancellationRequested();
            return result;
        }
        catch(OperationCanceledException) when(token.IsCancellationRequested) { throw; }
        catch(Phase7SqlGuardException) { throw; }
        catch { throw new Phase7SqlGuardException("PHASE7_SQL_SNAPSHOT_READ"); }
    }

    private Phase7SqlReliabilitySnapshot Project(IReadOnlyDictionary<string,JsonArray> tables,JsonObject[] roots)
    {
        try
        {
            var partners=roots.Select(root=>Partner(tables,root)).ToArray();
            var ids=partners.Select(p=>p.Id).ToHashSet(); var globals=partners.Select(p=>p.GlobalId).ToHashSet();
            var events=Rows(tables,"LocalOutbox").Where(r=>Text(r,"EntityCode")==code || globals.Contains(GuidValue(r,"EntityGlobalId")))
                .Select(r=>{Require(Number(r,"CompanyId")==target.CompanyId && Text(r,"EntityName")=="BusinessPartner","EVENT_SCOPE");
                    return Deserialize<CreateLocalSyncOutboxData>(r);}).ToArray();
            var eventIds=events.Select(e=>e.EventId).ToHashSet();
            var audits=Rows(tables,"AuditCatalogChanges").Where(r=>Text(r,"EntityName")=="BusinessPartners" && ids.Contains(ParseRecordId(r)))
                .Select(r=>new Phase7NominalAudit(Text(r,"EntityName")!,ParseRecordId(r),Text(r,"Action")!,Text(r,"FieldName")!,
                    Text(r,"OldValue"),Text(r,"NewValue"),r["UserId"] is null?null:checked((int)Number(r,"UserId")),Text(r,"UserName"))).ToArray();
            var sync=Rows(tables,"SyncAudit").Where(r=>eventIds.Contains(GuidValue(r,"EventId")) || globals.Contains(GuidValue(r,"EntityGlobalId")))
                .Select(r=>{Require(Number(r,"CompanyId")==target.CompanyId && Text(r,"EntityName")=="BusinessPartner","SYNC_SCOPE");
                    return new Phase7NominalSyncAudit(GuidValue(r,"EventId"),Text(r,"Action")!,Text(r,"NewStatus")!);}).ToArray();
            return new(partners,events,sync,audits);
        }
        catch(Phase7SqlGuardException) { throw; }
        catch { throw new Phase7SqlGuardException("PHASE7_SQL_SNAPSHOT_PROJECTION"); }
    }

    private static BusinessPartnerDto Partner(IReadOnlyDictionary<string,JsonArray> tables,JsonObject root)
    {
        var merged=(JsonObject)root.DeepClone(); var id=Number(root,"Id");
        Require(GuidValue(root,"GlobalId")!=Guid.Empty,"GLOBAL_ID");
        foreach(var table in Scalars)
        {
            var rows=Rows(tables,table).Where(r=>Number(r,"BusinessPartnerId")==id).ToArray();
            Require(rows.Length==1,"SCALAR_MISSING_OR_AMBIGUOUS");
            foreach(var field in rows[0])
                if(field.Key is not ("Id" or "BusinessPartnerId" or "CreatedAt" or "UpdatedAt"))merged[field.Key]=field.Value?.DeepClone();
        }
        JsonArray Children(string table)=>new(Rows(tables,table).Where(r=>Number(r,"BusinessPartnerId")==id && !Boolean(r,"IsDeleted"))
            .Select(r=>
            {
                var child=(JsonObject)r.DeepClone();
                child["Id"]=Number(r,Phase7SqlSnapshotSqlReader.PrimaryKey(table));
                return (JsonNode)child;
            }).ToArray());
        merged["Addresses"]=Children("BusinessPartnerAddresses"); merged["Contacts"]=Children("BusinessPartnerContacts");
        merged["BankAccounts"]=Children("BusinessPartnerBankAccounts"); merged["RetentionSettings"]=Children("BusinessPartnerRetentionSettings");
        merged["SapFieldMappings"]=Children("BusinessPartnerSapFieldMappings"); merged["Attachments"]=Children("BusinessPartnerAttachmentsMetadata");
        var notes=Rows(tables,"BusinessPartnerNotes").Where(r=>Number(r,"BusinessPartnerId")==id).ToArray();
        Require(notes.Length<=1,"NOTES_AMBIGUOUS"); merged["Notes"]=notes.SingleOrDefault()?.DeepClone();
        return Deserialize<BusinessPartnerDto>(merged);
    }

    private static T Deserialize<T>(JsonObject row)=>row.Deserialize<T>(Json) ?? throw new Phase7SqlGuardException("PHASE7_SQL_SNAPSHOT_PROJECTION");
    private static T Projection<T>(Func<T> project)
    {
        try { return project(); }
        catch(Phase7SqlGuardException) { throw; }
        catch { throw new Phase7SqlGuardException("PHASE7_SQL_SNAPSHOT_PROJECTION"); }
    }
    private static IEnumerable<JsonObject> Rows(IReadOnlyDictionary<string,JsonArray> tables,string table)=>tables[table].Select(r=>r!.AsObject());
    private static long Number(JsonObject row,string key)=>row[key]!.GetValue<long>();
    private static string? Text(JsonObject row,string key)=>row[key]?.GetValue<string>();
    private static bool Boolean(JsonObject row,string key)=>row[key]!.GetValue<bool>();
    private static Guid GuidValue(JsonObject row,string key)=>row[key] is null ? Guid.Empty : Guid.Parse(row[key]!.GetValue<string>());
    private static int ParseRecordId(JsonObject row)=>int.TryParse(Text(row,"RecordId"),NumberStyles.None,CultureInfo.InvariantCulture,out var id)?id:0;
    private static void RejectDuplicates(JsonElement element)
    {
        if(element.ValueKind==JsonValueKind.Object)
        {
            var names=new HashSet<string>(StringComparer.Ordinal);
            foreach(var property in element.EnumerateObject()){Require(names.Add(property.Name),"DUPLICATE_JSON_KEY");RejectDuplicates(property.Value);}
        }
        else if(element.ValueKind==JsonValueKind.Array)foreach(var item in element.EnumerateArray())RejectDuplicates(item);
    }
    private static void Require(bool valid,string code)
    {if(!valid)throw new Phase7SqlGuardException("PHASE7_SQL_SNAPSHOT_"+code);}
}
