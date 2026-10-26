using System.Data;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.BusinessPartners.Commands;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal sealed class Phase7SqlSuiteRuntime(Phase7SqlFixture fixture,Phase7SqlSuiteInputs inputs,
    Phase7SqlSuiteProcessLease lease,string caseDirectory,Phase7SqlSuiteEvidenceCapture capture):IPhase7SqlExecutableRun
{
    private readonly IPhase7SqlDestinationVerifier verifier=new Phase7SqlDestinationVerifier();
    private int snapshots;

    public async Task<Phase7SqlCaseEvidence> ExecuteCaseAsync(string id,CancellationToken token)
    {
        var started=DateTimeOffset.UtcNow;var before=new Phase7SqlTracePair(Trace(fixture.Tenant("A")),Trace(fixture.Tenant("B")));
        using var budget=CancellationTokenSource.CreateLinkedTokenSource(token);
        var ct=budget.Token;
        try
        {
            budget.CancelAfter(lease.Remaining);
            await PreflightAsync(ct);
            switch(id)
            {
                case "S01":case "S02":case "S03":case "Equivalence":
                    var number=id=="Equivalence"?19:int.Parse(id[1..]);
                    var legacy=await Nominal("A",$"S{number:00}",true,true,true,ct);
                    var pilot=await Nominal("B",$"S{number:00}",false,true,true,ct);
                    Phase7SqlNominalScenarios.AssertEquivalent(legacy,pilot);
                    Require(legacy[0].Partner.GlobalId!=pilot[0].Partner.GlobalId,"GENERATED_IDENTITY_REUSED");
                    break;
                case "S04":
                    await Nominal("A","S04N",false,false,true,ct);
                    await Nominal("A","S04D",false,true,false,ct);
                    await Nominal("A","S04B",false,false,false,ct);
                    await Nominal("A","S04Y",false,true,true,ct); // fixture always SAP=None
                    break;
                case "S05":case "S06":case "S07":case "S08":case "S09":case "S10":case "S11":
                    foreach(var operation in new[]{"C","U","D"})await Failure((Phase7SqlFailureCase)int.Parse(id[1..]),operation,ct);
                    break;
                case "S12":
                    var code=Code("S12"); var a=fixture.Tenant("A");var b=fixture.Tenant("B");
                    var ra=Reader("A",code);var rb=Reader("B",code);
                    await Phase7SqlReliabilityScenarios.TenantIsolationAsync(a.CreateReliabilityScope().Unit,b.CreateReliabilityScope().Unit,
                        Command("A",code) with{Name="Laboratorio A"},Command("B",code) with{Name="Laboratorio B"},
                        Phase7SqlReliabilityScenarios.Observer(a,ra.ReadReliabilityAsync),Phase7SqlReliabilityScenarios.Observer(b,rb.ReadReliabilityAsync),a.CompanyId,b.CompanyId,ct);
                    break;
                case "S13":
                    await Phase7SqlReliabilityScenarios.SessionReuseOnFixtureAsync(fixture.Tenant("A"),Command("A",Code(id)),Reader("A",Code(id)).ReadReliabilityAsync,ct);break;
                case "S14":
                    await Phase7SqlReliabilityScenarios.RepeatedCreateOnFixtureAsync(fixture.Tenant("A"),Command("A",Code(id)),Reader("A",Code(id)).ReadReliabilityAsync,ct);break;
                case "S15":
                    var attempts=await Phase7SqlReliabilityScenarios.ConcurrentCreateOnFixtureAsync(fixture.Tenant("A"),Command("A",Code(id)),Reader("A",Code(id)).ReadReliabilityAsync,TimeSpan.FromSeconds(20),ct);
                    Save("concurrent.json",attempts.Select(x=>new Phase7SqlAttemptEvidence(x.Result?.IsSuccess,
                        x.Result?.Errors.Select(e=>e.Code).ToArray()??[],x.Error?.GetType().Name,
                        x.Error is SqlException sql?sql.Errors.Cast<SqlError>().Select(e=>e.Number).ToArray():[])));break;
                case "S16":
                    var repeated=await Phase7SqlReliabilityScenarios.RepeatedUpdateOnFixtureAsync(fixture.Tenant("A"),Command("A",Code(id)),Reader("A",Code(id)).ReadReliabilityAsync,ct);
                    Require(repeated.AfterRepeat.Partners.Single().Addresses.Count>0,"CHILD_REPLACEMENT_NOT_EXERCISED");
                    Save("repetition.json",repeated);break;
                case "S17":
                    var closed=await Phase7SqlRecoveryScenarios.CloseBeforeCommitOnFixtureAsync(fixture.Tenant("A"),Command("A",Code(id)),Reader("A",Code(id)).ReadRollbackAsync,ct);
                    Save("owner-close.json",Diagnostic(closed));break;
                case "S18":
                    await Phase7SqlRecoveryScenarios.InvalidContextsOnFixtureAsync(fixture.Tenant("A"),Reader("A",Code(id)).ReadRollbackAsync,ct);break;
                case "R01.Legacy":case "R01.Pilot":await InternalRollback(id=="R01.Legacy",ct);break;
                default:throw new Phase7SqlGuardException("PHASE7_SQL_SUITE_CASE");
            }
            ct.ThrowIfCancellationRequested();
            var final=lease.CanFinalize(id);if(final)await capture.FinalSnapshotsAsync(fixture,ct);
            var spec=Phase7SqlExecutionCatalog.Cases.Single(c=>c.Id==id);
            var evidence=new Phase7SqlCaseEvidence(id,spec.Checks.ToDictionary(c=>c,_=>Phase7SqlEvidenceStatus.Passed));
            Save("sessions.json",new { A=Trace(fixture.Tenant("A")),B=Trace(fixture.Tenant("B")) });
            Save("case-timing.json",new Phase7SqlCaseTiming(id,lease.CurrentSequence,started,DateTimeOffset.UtcNow,before,final));
            lease.Complete(id);
            Save("case.json",evidence);
            return evidence;
        }
        catch(Exception error)
        {
            lease.Fail();
            if(lease.TerminalFailure is not null)error.Data["Phase7Sql.TerminationException"]=lease.TerminalFailure;
            try{Save("failure.json",Diagnostic(error));}catch{ /* preserve primary; evidence failure cannot turn into a pass */ }
            throw;
        }
    }

    private async Task PreflightAsync(CancellationToken token)
    {
        foreach(var role in new[]{"A","B"})
        {
            var target=Target(role);var expected=role=="A"?inputs.FaultTriggerSha256A:inputs.FaultTriggerSha256B;
            await fixture.Tenant(role).ObserveAsync(async(c,timeout,ct)=>
            {
                var lookup=await c.QueryAsync<int>(new CommandDefinition("SELECT Id FROM dbo.BusinessPartnerIdentificationTypes WHERE Id=@Id AND IsActive=1 AND IsDeleted=0",new{Id=Lookup(role)},commandTimeout:timeout,cancellationToken:ct));
                Require(Phase7SqlLedger.ResolveLookup(lookup.ToArray())==Lookup(role),"LOOKUP");
                var missing=await c.QuerySingleAsync<int>(new CommandDefinition("SELECT (SELECT COUNT(*) FROM dbo.BusinessPartners WHERE Id=@Id)+(SELECT COUNT(*) FROM dbo.BusinessPartnerIdentificationTypes WHERE Id=@TypeId)",
                    new{Id=inputs.MissingPartnerId,TypeId=inputs.MissingIdentificationTypeId},commandTimeout:timeout,cancellationToken:ct));
                Require(missing==0,"MISSING_IDS_EXIST");
                var triggers=(await c.QueryAsync<TriggerRow>(new CommandDefinition("SELECT t.name Name,m.definition Definition,t.is_disabled IsDisabled FROM sys.triggers t LEFT JOIN sys.sql_modules m ON m.object_id=t.object_id WHERE t.name IN @Names",new{Names=expected.Keys.ToArray()},commandTimeout:timeout,cancellationToken:ct))).ToArray();
                Require(triggers.Length==3 && triggers.All(t=>!t.IsDisabled && t.Definition is not null
                    && expected.TryGetValue(t.Name,out var hash) && Phase7SqlDestinationSqlReader.Fingerprint(t.Definition)==hash.ToUpperInvariant()),"FAULT_TRIGGER_DEFINITIONS");
                Save($"preflight-{role}.json",new Phase7SqlPrerequisiteCapture(Path.GetFileName(caseDirectory).Replace('_','.'),role,
                    DateTimeOffset.UtcNow,Lookup(role),missing,triggers.Select(t=>new Phase7SqlTriggerCapture(t.Name,t.Definition,t.IsDisabled)).ToArray()));
                return true;
            },token);
        }
    }
    private sealed record TriggerRow(string Name,string? Definition,bool IsDisabled);

    private async Task<IReadOnlyList<Phase7NominalSnapshot>> Nominal(string role,string suffix,bool legacy,bool master,bool sync,CancellationToken ct)
    {
        var command=Command(role,Code(suffix));var reader=Reader(role,command.Code);
        await Empty(role,reader,ct);
        return await Phase7SqlNominalScenarios.RunOnFixtureAsync(fixture.Tenant(role),legacy,master,sync,command,
            id=>Phase7NominalCommands.UpdateCommand(id,command) with{Name="Laboratorio editado",Email=null,
                Addresses=[new(null,null,null,"Main","Direccion editada",null,"EC",null,null,null,null,null,true,true)]},reader.ReadNominalAsync,ct);
    }

    private async Task Failure(Phase7SqlFailureCase fault,string operation,CancellationToken ct)
    {
        var tenant=fixture.Tenant("A");var command=Command("A",Code($"S{(int)fault:00}{operation}"));var reader=Reader("A",command.Code);
        await Empty("A",reader,ct);
        var id=0;
        if(operation!="C")
        {
            var created=await new CreateBusinessPartnerCommandHandler(tenant.CreateUnitOfWork()).Handle(command,ct);
            Require(created.IsSuccess && created.Value is not null,"FAILURE_SEED");id=created.Value!.Id;
        }
        var evidence=await Phase7SqlFailureScenarios.RunOnFixtureAsync(tenant,new(fixture.Manifest.RunId,command.Code,fault),async(unit,token)=>
        {
            if(fault==Phase7SqlFailureCase.ProcedureError)unit=new Phase7SqlProcedureErrorUnit(unit,command,inputs.MissingIdentificationTypeId);
            if(operation=="C")Require((await new CreateBusinessPartnerCommandHandler(unit).Handle(command,token)).IsSuccess,"UNEXPECTED_RESULT");
            else if(operation=="U")Require((await new UpdateBusinessPartnerCommandHandler(unit).Handle(Phase7NominalCommands.UpdateCommand(id,command) with{Name="Cambio a revertir"},token)).IsSuccess,"UNEXPECTED_RESULT");
            else Require((await new DeleteBusinessPartnerCommandHandler(unit).Handle(new(id,command.AuditUserId,command.AuditUserName),token)).IsSuccess,"UNEXPECTED_RESULT");
        },reader.ReadRollbackAsync,TimeSpan.FromSeconds(20),e=>e.Number==547,ct);
        Save($"fault-{operation}.json",new { Case=evidence.Case.ToString(),evidence.BarrierAcknowledged,Error=Diagnostic(evidence.Failure) });
    }

    private async Task InternalRollback(bool legacy,CancellationToken ct)
    {
        var suffix=legacy?"S20":"S21";var tenant=fixture.Tenant("A");var command=Command("A",Code(suffix));var reader=Reader("A",command.Code);
        await Empty("A",reader,ct);
        var before=await tenant.ObserveAsync(reader.ReadRollbackAsync,ct);bool attempted=false;bool returnedFalse=false;Exception? failure=null;
        try
        {
            await (legacy?tenant.CreateLegacyReference():tenant.CreateUnitOfWork()).ExecuteAsync(async(context,token)=>
            {
                Require(await context.GetByIdAsync(inputs.MissingPartnerId,token) is null,"MISSING_PARTNER_EXISTS");
                var data=CreateBusinessPartnerCommandHandler.ToCreateData(command,command.Code,command.IdentificationNumber);
                var id=await context.CreateAsync(data,token);
                var partner=await context.GetByIdAsync(id,token)??throw new Phase7SqlGuardException("PHASE7_SQL_R01_SEED");
                await context.EnqueueAsync(partner,SyncOperation.Created,token);
                var update=Phase7SqlR01Data.Map(command,inputs.MissingPartnerId);
                attempted=true;returnedFalse=!await context.UpdateAsync(update,token);
                return 1;
            },ct);
        }
        catch(Exception error) when(error is not OperationCanceledException){failure=error;}
        Save("internal-rollback.json",new { attempted,returnedFalse,
            Disposition=failure is not null && Phase7SqlR01Outcome.IsInsertExecRejection(failure)?"InsertExecRollbackRejected":"TransactionInvalidatedOrCompleted",
            Error=failure is null?null:Diagnostic(failure) });
        Require(attempted && Phase7SqlR01Outcome.IsExpected(failure,returnedFalse),"R01_OUTCOME");
        try { Phase7SqlRollbackSnapshot.AssertUnchanged(before,await tenant.ObserveAsync(reader.ReadRollbackAsync,ct)); }
        catch(Exception error){error.Data["Phase7Sql.PrimaryFailure"]=failure;throw;}
    }

    private async Task Empty(string role,Phase7SqlSnapshotReaders reader,CancellationToken ct)
    {
        var state=await fixture.Tenant(role).ObserveAsync(reader.ReadReliabilityAsync,ct);
        Require(state.Partners.Count==0 && state.Events.Count==0 && state.Audits.Count==0 && state.SyncAudits.Count==0,"CASE_NOT_FRESH");
    }
    private Phase7SqlSnapshotReaders Reader(string role,string code)=>new(fixture.Manifest,Target(role),code,verifier,async(c,timeout,ct)=>
    {
        var raw=await Phase7SqlSnapshotSqlReader.ReadAsync(c,fixture.Manifest,Target(role),timeout,ct);
        Require(raw.Length<=8*1024*1024,"SNAPSHOT_TOO_LARGE");
        Save($"snapshot-{role}-{Interlocked.Increment(ref snapshots):0000}.json",JsonNode.Parse(raw));
        return raw;
    });
    private CreateBusinessPartnerCommand Command(string role,string code)=>Phase7NominalCommands.CreateCommand(Lookup(role),code,code) with
    {
        Name="Socio sintetico",PartnerType="Supplier",
        Addresses=[new(null,null,null,"Main","Direccion sintetica",null,"EC",null,null,null,null,null,true,true)],
        Contacts=[new(null,null,"Contacto sintetico",null,null,null,null,null,"test@example.test",null,false,true,true,null)],
        Notes=new("Nota sintetica",null,null,null)
    };
    private int Lookup(string role)=>role=="A"?inputs.IdentificationTypeIdA:inputs.IdentificationTypeIdB;
    private Phase7SqlTarget Target(string role)=>role=="A"?fixture.Manifest.TenantA:fixture.Manifest.TenantB;
    private string Code(string suffix)=>$"P7_{fixture.Manifest.RunId.ToString("N")[..8].ToUpperInvariant()}_{suffix}";
    private void Save(string name,object? value)
    {
        using var stream=new FileStream(Path.Combine(caseDirectory,name),FileMode.CreateNew,FileAccess.Write,FileShare.None);
        JsonSerializer.Serialize(stream,value,new JsonSerializerOptions {WriteIndented=true});
    }
    private static object Diagnostic(Exception error)=>new {Type=error.GetType().Name,SqlNumber=(error as SqlException)?.Number,
        SqlNumbers=error is SqlException sql?sql.Errors.Cast<SqlError>().Select(e=>e.Number).ToArray():[],
        GuardCode=(error as Phase7SqlGuardException)?.ErrorCode,
        SecondaryType=(error.Data["NuanSystem.Transaction.RollbackException"] as Exception)?.GetType().Name,
        CleanupType=(error.Data["Phase7Sql.SessionCleanupException"] as Exception)?.GetType().Name,
        PrimaryType=(error.Data["Phase7Sql.PrimaryFailure"] as Exception)?.GetType().Name,
        PrimarySqlNumbers=error.Data["Phase7Sql.PrimaryFailure"] is SqlException primary?primary.Errors.Cast<SqlError>().Select(e=>e.Number).ToArray():[]};
    private static Phase7SqlTraceNode Trace(Phase7SqlFixture.Phase7SqlTenant tenant)=>new(tenant.EvidenceScopeId,
        tenant.Samples,tenant.ScopeStates,tenant.TotalWriteConnections,tenant.ObserverConnectionCount,
        tenant.IndependentScopes.Select(Trace).ToArray());
    private static void Require(bool valid,string code){if(!valid)throw new Phase7SqlGuardException("PHASE7_SQL_SUITE_"+code);}
    public void Dispose() { /* The process lease owns the shared fixture; each operation disposes its ADO handles. */ }
}

internal static class Phase7SqlR01Outcome
{
    public static bool IsExpected(Exception? error,bool returnedFalse)
    {
        if(error is SqlException sql)
        {
            var numbers=sql.Errors.Cast<SqlError>().Select(e=>e.Number).ToArray();
            return !numbers.Any(n=>n is -2 or 1205 or 1222) && numbers.Any(n=>n is 266 or 3902 or 3903 or 3915);
        }
        return returnedFalse && error is InvalidOperationException and not Phase7SqlGuardException;
    }
    public static bool IsInsertExecRejection(Exception error)=>error is SqlException sql && sql.Errors.Cast<SqlError>().Any(e=>e.Number==3915);
}

internal sealed class Phase7SqlProcedureErrorUnit(IBusinessPartnerUnitOfWork inner,CreateBusinessPartnerCommand command,int missingType):IBusinessPartnerUnitOfWork
{
    public Task<T> ExecuteAsync<T>(Func<IBusinessPartnerTransaction,CancellationToken,Task<T>> operation,CancellationToken token=default)=>inner.ExecuteAsync<T>(async(context,ct)=>
    {
        await operation(context,ct);
        var data=CreateBusinessPartnerCommandHandler.ToCreateData(command,command.Code+"E",command.IdentificationNumber+"E") with {IdentificationTypeId=missingType};
        await context.CreateAsync(data,ct); // real create procedure must raise FK547 after the requested CRUD writes
        throw new Phase7SqlGuardException("PHASE7_SQL_PROCEDURE_ERROR_NOT_RAISED");
    },token);
}

internal static class Phase7SqlR01Data
{
    public static UpdateBusinessPartnerData Map(CreateBusinessPartnerCommand command,int missingId)
    {
        if(missingId<=0)throw new Phase7SqlGuardException("PHASE7_SQL_R01_ID");
        var data=CreateBusinessPartnerCommandHandler.ToCreateData(command,command.Code.Trim().ToUpperInvariant(),command.IdentificationNumber.Trim());
        var node=JsonSerializer.SerializeToNode(data)!.AsObject();node["Id"]=missingId;
        node["UpdatedByUserId"]=command.AuditUserId;node["UpdatedByUserName"]=command.AuditUserName;
        return node.Deserialize<UpdateBusinessPartnerData>()!;
    }
}
