using System.Data;
using System.Text.Json.Nodes;
using NSubstitute;
using NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

[Trait("Category", "OfflineSafety")]
public sealed class Phase7SqlSnapshotReaderTests
{
    [Fact]
    public async Task Physical_snapshot_keeps_deleted_children_orphans_and_all_columns()
    {
        var d = new Data();
        var first = await d.Reader.ReadRollbackAsync(d.Connection, 30, default);
        d.Tables["BusinessPartnerContacts"]!.AsArray().Add(new JsonObject { ["Id"] = 99, ["BusinessPartnerId"] = 777, ["IsDeleted"] = true, ["UnknownFutureColumn"] = "partial" });
        var after = await d.Reader.ReadRollbackAsync(d.Connection, 30, default);
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlRollbackSnapshot.AssertUnchanged(first, after));
        Assert.Equal(2, d.Verifier.Calls);
    }

    [Fact]
    public async Task Nominal_projection_reads_physical_deleted_parent_and_only_current_children()
    {
        var d = new Data();
        d.Tables["BusinessPartners"]![0]!["IsDeleted"] = true;
        d.Tables["BusinessPartners"]![0]!["IsActive"] = false;
        d.Tables["BusinessPartnerAddresses"]!.AsArray().Add(new JsonObject { ["Id"] = 1, ["BusinessPartnerId"] = 7, ["IsDeleted"] = true, ["Line1"] = "old" });
        d.Tables["BusinessPartnerAddresses"]!.AsArray().Add(new JsonObject { ["Id"] = 2, ["BusinessPartnerId"] = 7, ["IsDeleted"] = false, ["Line1"] = "current" });
        var snapshot = await d.Reader.ReadNominalAsync(d.Connection, 7, 30, default);
        Assert.True(snapshot.IsDeleted); Assert.False(snapshot.Partner.IsActive);
        Assert.Equal("current", Assert.Single(snapshot.Partner.Addresses).Line1);
        Assert.Equal(7, snapshot.Partner.Id); Assert.Equal(d.Global, snapshot.Partner.GlobalId);
        Assert.Equal(12.345678m, snapshot.Partner.MinimumOrderQuantity);
        Assert.Equal("2026-10-25T01:02:03", snapshot.Partner.CreatedAt.ToString("s"));
    }

    [Theory]
    [InlineData("missingTable")][InlineData("extraTable")][InlineData("notArray")]
    [InlineData("nullRow")][InlineData("badJson")][InlineData("duplicateKey")][InlineData("oversize")]
    public async Task Incomplete_or_malformed_evidence_is_rejected(string mutation)
    {
        var d = new Data();
        if (mutation == "missingTable") d.Tables.Remove("SyncAudit");
        if (mutation == "extraTable") d.Tables["Unexpected"] = new JsonArray();
        if (mutation == "notArray") d.Tables["SyncAudit"] = new JsonObject();
        if (mutation == "nullRow") d.Tables["SyncAudit"]!.AsArray().Add((JsonNode?)null);
        if (mutation == "badJson") d.Raw = "{";
        if (mutation == "duplicateKey") d.Raw = "{\"BusinessPartners\":[],\"BusinessPartners\":[]}";
        if (mutation == "oversize") d.Raw = new string(' ', 8 * 1024 * 1024 + 1);
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => d.Reader.ReadRollbackAsync(d.Connection, 30, default));
    }

    [Fact]
    public async Task Case_selection_keeps_partial_outbox_and_audit_instead_of_reconstructing_from_commands()
    {
        var d = new Data();
        d.Tables["LocalOutbox"]!.AsArray().Add(new JsonObject { ["Id"]=1, ["EventId"]=Guid.NewGuid(), ["CompanyId"]=101,
            ["EntityName"]="BusinessPartner", ["EntityGlobalId"]=d.Global, ["EntityCode"]=d.Code,
            ["Operation"]="Created", ["PayloadJson"]="{\"partial\":true}", ["MaxAttempts"]=3 });
        d.Tables["AuditCatalogChanges"]!.AsArray().Add(new JsonObject { ["Id"]=1,["EntityName"]="BusinessPartners",["RecordId"]="7",
            ["Action"]="INSERT",["FieldName"]="Name",["OldValue"]=null,["NewValue"]="observed",["UserId"]=4,["UserName"]="test" });
        var result = await d.Reader.ReadReliabilityAsync(d.Connection,30,default);
        Assert.Equal("{\"partial\":true}", Assert.Single(result.Events).PayloadJson);
        Assert.Empty(result.SyncAudits); Assert.Equal("observed", Assert.Single(result.Audits).NewValue);
    }

    [Fact]
    public async Task Empty_case_is_valid_for_reliability_but_not_nominal_and_other_cases_are_excluded()
    {
        var d = new Data(); d.Tables["BusinessPartners"]![0]!["Code"]="different";
        Assert.Empty((await d.Reader.ReadReliabilityAsync(d.Connection,30,default)).Partners);
        await Assert.ThrowsAsync<Phase7SqlGuardException>(()=>d.Reader.ReadNominalAsync(d.Connection,7,30,default));
    }

    [Fact]
    public async Task Cancellation_closed_handles_and_failed_preflight_never_read()
    {
        var d = new Data(); using var cancel = new CancellationTokenSource(); cancel.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(()=>d.Reader.ReadRollbackAsync(d.Connection,30,cancel.Token));
        d.Connection.State.Returns(ConnectionState.Closed);
        await Assert.ThrowsAsync<Phase7SqlGuardException>(()=>d.Reader.ReadRollbackAsync(d.Connection,30,default));
        d.Connection.State.Returns(ConnectionState.Open); d.Verifier.Fail=true;
        await Assert.ThrowsAsync<Phase7SqlGuardException>(()=>d.Reader.ReadRollbackAsync(d.Connection,30,default));
        Assert.Equal(0,d.Reads);
    }

    [Fact]
    public async Task Reader_errors_are_safe_and_use_exact_observer_handle_timeout_and_token()
    {
        var d=new Data(); d.Failure=new InvalidOperationException("Password=secret");
        var error=await Assert.ThrowsAsync<Phase7SqlGuardException>(()=>d.Reader.ReadRollbackAsync(d.Connection,30,default));
        Assert.DoesNotContain("secret",error.ToString()); Assert.Null(error.InnerException);
        Assert.Equal(1,d.Reads); d.Connection.DidNotReceive().Open(); d.Connection.DidNotReceive().Close();
        d.Connection.DidNotReceive().BeginTransaction();
    }

    [Fact]
    public async Task Physical_child_primary_keys_are_preserved_and_aliased_only_in_DTOs()
    {
        var d = new Data();
        var keys = new Dictionary<string,string> {
            ["BusinessPartnerBankAccounts"]="BusinessPartnerBankAccountId",
            ["BusinessPartnerRetentionSettings"]="BusinessPartnerRetentionSettingId",
            ["BusinessPartnerSapFieldMappings"]="BusinessPartnerSapFieldMappingId",
            ["BusinessPartnerAttachmentsMetadata"]="BusinessPartnerAttachmentId" };
        foreach(var (table,key) in keys)
            d.Tables[table]!.AsArray().Add(new JsonObject { [key]=19,["BusinessPartnerId"]=7,["IsDeleted"]=false });
        var snapshot=await d.Reader.ReadNominalAsync(d.Connection,7,30,default);
        Assert.Equal(19,Assert.Single(snapshot.Partner.BankAccounts).Id);
        Assert.Equal(19,Assert.Single(snapshot.Partner.RetentionSettings).Id);
        Assert.Equal(19,Assert.Single(snapshot.Partner.SapFieldMappings).Id);
        Assert.Equal(19,Assert.Single(snapshot.Partner.Attachments).Id);
        var before=await d.Reader.ReadRollbackAsync(d.Connection,30,default);
        d.Tables["BusinessPartnerBankAccounts"]![0]!["BusinessPartnerBankAccountId"]=20;
        var after=await d.Reader.ReadRollbackAsync(d.Connection,30,default);
        Assert.Throws<Phase7SqlGuardException>(()=>Phase7SqlRollbackSnapshot.AssertUnchanged(before,after));
    }

    [Fact]
    public async Task Physical_order_is_deterministic_and_snapshot_is_detached()
    {
        var d=new Data();
        var rows=d.Tables["BusinessPartnerContacts"]!.AsArray();
        rows.Add(new JsonObject { ["Id"]=12,["BusinessPartnerId"]=7,["IsDeleted"]=true });
        rows.Add(new JsonObject { ["Id"]=2,["BusinessPartnerId"]=7,["IsDeleted"]=false });
        var first=await d.Reader.ReadRollbackAsync(d.Connection,30,default);
        var last=rows[1]!.DeepClone(); rows.RemoveAt(1); rows.Insert(0,last);
        var reordered=await d.Reader.ReadRollbackAsync(d.Connection,30,default);
        Phase7SqlRollbackSnapshot.AssertUnchanged(first,reordered);
        rows[0]!["IsDeleted"]=true;
        var changed=await d.Reader.ReadRollbackAsync(d.Connection,30,default);
        Assert.Throws<Phase7SqlGuardException>(()=>Phase7SqlRollbackSnapshot.AssertUnchanged(first,changed));
    }

    [Theory]
    [InlineData("duplicateId")][InlineData("missingId")][InlineData("negativeId")]
    [InlineData("missingScalar")][InlineData("wrongParent")][InlineData("emptyGlobal")]
    public async Task Invalid_identity_and_missing_relations_are_never_filled_with_command_defaults(string mutation)
    {
        var d=new Data();
        switch(mutation)
        {
            case "duplicateId":d.Tables["BusinessPartners"]!.AsArray().Add(d.Tables["BusinessPartners"]![0]!.DeepClone());break;
            case "missingId":d.Tables["BusinessPartners"]![0]!.AsObject().Remove("Id");break;
            case "negativeId":d.Tables["BusinessPartners"]![0]!["Id"]=-1;break;
            case "missingScalar":d.Tables["BusinessPartnerPurchaseSettings"]!.AsArray().Clear();break;
            case "wrongParent":d.Tables["BusinessPartnerPurchaseSettings"]![0]!["BusinessPartnerId"]=999;break;
            default:d.Tables["BusinessPartners"]![0]!["GlobalId"]=Guid.Empty;break;
        }
        await Assert.ThrowsAsync<Phase7SqlGuardException>(()=>d.Reader.ReadNominalAsync(d.Connection,7,30,default));
    }

    [Fact]
    public async Task Cross_company_events_are_rejected_and_orphan_sync_audits_remain_physical_evidence()
    {
        var d=new Data();
        d.Tables["LocalOutbox"]!.AsArray().Add(new JsonObject { ["Id"]=1,["EventId"]=Guid.NewGuid(),["CompanyId"]=102,
            ["EntityName"]="BusinessPartner",["EntityGlobalId"]=d.Global,["EntityCode"]=d.Code,["Operation"]="Created",["PayloadJson"]="{}" });
        await Assert.ThrowsAsync<Phase7SqlGuardException>(()=>d.Reader.ReadReliabilityAsync(d.Connection,30,default));
        d.Tables["LocalOutbox"]!.AsArray().Clear();
        var before=await d.Reader.ReadRollbackAsync(d.Connection,30,default);
        d.Tables["SyncAudit"]!.AsArray().Add(new JsonObject { ["Id"]=1,["CompanyId"]=101,["EventId"]=Guid.NewGuid(),
            ["EntityGlobalId"]=Guid.NewGuid(),["EntityName"]="BusinessPartner",["Action"]="Created",["NewStatus"]="Pending" });
        var after=await d.Reader.ReadRollbackAsync(d.Connection,30,default);
        Assert.Throws<Phase7SqlGuardException>(()=>Phase7SqlRollbackSnapshot.AssertUnchanged(before,after));
    }

    [Fact]
    public async Task Notes_and_nulls_come_from_storage_and_parent_timestamps_are_not_overwritten()
    {
        var d=new Data();
        d.Tables["BusinessPartnerNotes"]!.AsArray().Add(new JsonObject { ["BusinessPartnerId"]=7,["InternalNotes"]="persisted",["PaymentNotes"]=null });
        d.Tables["BusinessPartnerPurchaseSettings"]![0]!["CreatedAt"]="2000-01-01T00:00:00";
        d.Tables["BusinessPartnerPurchaseSettings"]![0]!["PurchaseCurrencyCode"]=null;
        var snapshot=await d.Reader.ReadNominalAsync(d.Connection,7,30,default);
        Assert.Equal("persisted",snapshot.Partner.Notes!.InternalNotes);
        Assert.Null(snapshot.Partner.Notes.PaymentNotes);Assert.Null(snapshot.Partner.PurchaseCurrencyCode);
        Assert.Equal(2026,snapshot.Partner.CreatedAt.Year);
    }

    [Theory]
    [InlineData(0)][InlineData(31)][InlineData(121)]
    public async Task Out_of_budget_timeouts_are_rejected_before_any_read(int timeout)
    {
        var d=new Data();
        await Assert.ThrowsAsync<Phase7SqlGuardException>(()=>d.Reader.ReadRollbackAsync(d.Connection,timeout,default));
        Assert.Equal(0,d.Reads);Assert.Equal(0,d.Verifier.Calls);
    }

    [Fact]
    public async Task Default_sql_adapter_rejects_fake_connections_without_commands()
    {
        var d=new Data();
        await Assert.ThrowsAsync<Phase7SqlGuardException>(()=>Phase7SqlSnapshotSqlReader.ReadAsync(d.Connection,null!,null!,30,default));
        d.Connection.DidNotReceive().CreateCommand();
    }

    [Fact]
    public async Task Cancellation_during_read_is_preserved_and_cancellation_after_read_prevents_acceptance()
    {
        var d=new Data();using var cancel=new CancellationTokenSource();
        d.AfterRead=()=>{cancel.Cancel();throw new OperationCanceledException(cancel.Token);};
        var error=await Assert.ThrowsAnyAsync<OperationCanceledException>(()=>d.Reader.ReadRollbackAsync(d.Connection,30,cancel.Token));
        Assert.Equal(cancel.Token,error.CancellationToken);
        d=new Data();using var after=new CancellationTokenSource();d.AfterRead=()=>after.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(()=>d.Reader.ReadRollbackAsync(d.Connection,30,after.Token));
    }

    [Fact]
    public async Task A_closed_observer_after_read_is_never_accepted()
    {
        var d=new Data();d.AfterRead=()=>d.Connection.State.Returns(ConnectionState.Closed);
        await Assert.ThrowsAsync<Phase7SqlGuardException>(()=>d.Reader.ReadRollbackAsync(d.Connection,30,default));
    }

    [Theory]
    [InlineData("Code")][InlineData("IsDeleted")]
    public async Task Maltyped_projection_fields_are_sanitized(string field)
    {
        var d=new Data();d.Tables["BusinessPartners"]![0]![field]=123;
        var error=await Assert.ThrowsAsync<Phase7SqlGuardException>(()=>d.Reader.ReadNominalAsync(d.Connection,7,30,default));
        Assert.Equal("PHASE7_SQL_SNAPSHOT_PROJECTION",error.ErrorCode);Assert.Null(error.InnerException);
        if(field=="Code")await Assert.ThrowsAsync<Phase7SqlGuardException>(()=>d.Reader.ReadReliabilityAsync(d.Connection,30,default));
    }

    [Fact]
    public async Task Tenant_B_reads_its_own_company_events_with_the_same_business_key_as_A()
    {
        var d=new Data("B");
        d.Tables["LocalOutbox"]!.AsArray().Add(new JsonObject { ["Id"]=1,["EventId"]=Guid.NewGuid(),["CompanyId"]=102,
            ["EntityName"]="BusinessPartner",["EntityGlobalId"]=d.Global,["EntityCode"]=d.Code,["Operation"]="Created",["PayloadJson"]="{}" });
        var result=await d.Reader.ReadReliabilityAsync(d.Connection,30,default);
        Assert.Equal(102,Assert.Single(result.Events).CompanyId);
        Assert.Equal(d.Code,Assert.Single(result.Partners).Code);
    }

    [Theory]
    [InlineData("P7_87654321_S01")][InlineData("P7_12345678_S01'; DROP TABLE x;--")][InlineData("")]
    public void Case_keys_outside_the_run_are_rejected_before_any_SQL(string code)
    {
        Assert.Throws<Phase7SqlGuardException>(()=>new Data(caseCode:code));
    }

    [Fact]
    public async Task Alphabetic_run_prefixes_follow_the_uppercase_normalization_of_real_handlers()
    {
        var d=new Data(runId:Guid.Parse("abcdef12-1234-1234-1234-123456789abc"));
        Assert.Equal("P7_ABCDEF12_S01",(await d.Reader.ReadNominalAsync(d.Connection,7,30,default)).Partner.Code);
    }

    private sealed class Verifier : IPhase7SqlDestinationVerifier
    {
        public int Calls; public bool Fail;
        public Task VerifyAsync(Phase7SqlManifest m,Phase7SqlTarget t,bool observer,IDbConnection c,IDbTransaction? tx,CancellationToken ct)
        { Calls++; Assert.True(observer); Assert.Null(tx); if(Fail)throw new Phase7SqlGuardException("PREFLIGHT");return Task.CompletedTask; }
    }
    private sealed class Data
    {
        public readonly IDbConnection Connection=Substitute.For<IDbConnection>();
        public readonly Verifier Verifier=new(); public readonly Guid Global=Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
        public string Code="P7_12345678_S01"; public readonly JsonObject Tables=new();
        public string? Raw; public Exception? Failure; public Action? AfterRead; public int Reads;
        public readonly Phase7SqlSnapshotReaders Reader;
        public Data(string role="A",string? caseCode=null,Guid? runId=null)
        {
            Connection.State.Returns(ConnectionState.Open);
            var run=runId??Guid.Parse("12345678-1234-1234-1234-123456789abc");
            Code=$"P7_{run.ToString("N")[..8].ToUpperInvariant()}_S01";
            Phase7SqlTarget Target(string role,int id)=>new(role,$"NuanSystem_Phase7_{run:N}_{role}",id,id,run,"BackendTransactionValidation",160,"test",false,new('A',64),
                new Dictionary<string,string>(),"app","observer",["SELECT","EXECUTE","INSERT"],["SELECT"],"unused","unused");
            var a=Target("A",101);var b=Target("B",102);
            var manifest=new Phase7SqlManifest(1,run,"operator","exec","prep",DateTimeOffset.UtcNow.AddHours(-1),DateTimeOffset.UtcNow.AddHours(1),600,30,new("lab","lab","16.0.1.1"),a,b);
            foreach(var table in Phase7SqlRollbackSnapshot.RequiredTables)Tables[table]=new JsonArray();
            Tables["BusinessPartners"]!.AsArray().Add(new JsonObject { ["Id"]=7,["GlobalId"]=Global,["Code"]=Code,["Name"]="observed",["IsActive"]=true,["IsDeleted"]=false,["CreatedAt"]="2026-10-25T01:02:03" });
            foreach(var table in new[]{"BusinessPartnerFiscalData","BusinessPartnerAccountingSettings","BusinessPartnerCreditSettings","BusinessPartnerPurchaseSettings","BusinessPartnerSapMapping"})
                Tables[table]!.AsArray().Add(new JsonObject { ["BusinessPartnerId"]=7 });
            Tables["BusinessPartnerPurchaseSettings"]![0]!["MinimumOrderQuantity"]=12.345678m;
            Reader=new(manifest,role=="A"?a:b,caseCode??Code,Verifier,(c,timeout,ct)=>
            {Assert.Same(Connection,c);Assert.Equal(30,timeout);ct.ThrowIfCancellationRequested();Reads++;if(Failure is not null)throw Failure;AfterRead?.Invoke();return Task.FromResult(Raw??Tables.ToJsonString());});
        }
    }
}
