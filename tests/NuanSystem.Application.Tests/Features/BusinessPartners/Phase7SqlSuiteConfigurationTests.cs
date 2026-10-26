using Microsoft.Data.SqlClient;
using NuanSystem.Application.Tests.Infrastructure.Phase7Sql;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NSubstitute;
using System.Reflection;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

[Trait("Category","OfflineSafety")]
public sealed class Phase7SqlSuiteConfigurationTests
{
    [Fact]
    public void Bound_inputs_are_valid_without_files_credentials_or_connections()
    {
        var (manifest,inputs)=Data();
        Phase7SqlSuiteLoader.ValidateInputs(inputs,manifest,inputs.ManifestSha256);
    }

    [Theory]
    [InlineData("run")][InlineData("authorization")][InlineData("manifest")][InlineData("catalog")]
    [InlineData("lookupMissing")][InlineData("lookupDifferent")][InlineData("missingPartner")]
    [InlineData("missingType")][InlineData("boundaries")][InlineData("triggersMissing")]
    [InlineData("triggersWrongName")][InlineData("zeroHash")][InlineData("nullHash")]
    public void Drifting_or_incomplete_configuration_fails_closed(string change)
    {
        var (manifest,inputs)=Data();
        inputs=change switch
        {
            "run"=>inputs with{RunId=Guid.NewGuid()},
            "authorization"=>inputs with{ExecutionAuthorizationId="other"},
            "manifest"=>inputs with{ManifestSha256=new('C',64)},
            "catalog"=>inputs with{CatalogSha256=new('C',64)},
            "lookupMissing"=>inputs with{IdentificationTypeIdA=0},
            "lookupDifferent"=>inputs with{IdentificationTypeIdB=2},
            "missingPartner"=>inputs with{MissingPartnerId=0},
            "missingType"=>inputs with{MissingIdentificationTypeId=1},
            "boundaries"=>inputs with{BoundaryAttestation=""},
            "triggersMissing"=>inputs with{FaultTriggerSha256B=new Dictionary<string,string>()},
            "triggersWrongName"=>inputs with{FaultTriggerSha256A=new Dictionary<string,string>{{"other",new('B',64)}}},
            _=>inputs with{FaultTriggerSha256A=inputs.FaultTriggerSha256A.ToDictionary(p=>p.Key,_=>change=="zeroHash"?new string('0',64):null!)}
        };
        Assert.Throws<Phase7SqlGuardException>(()=>Phase7SqlSuiteLoader.ValidateInputs(inputs,manifest,new('A',64)));
    }

    [Theory][InlineData(false)][InlineData(true)]
    public void Factories_use_only_approved_references_and_produce_closed_connections(bool pooling)
    {
        var (manifest,_)=Data();var names=new List<string>();
        string? Resolve(string name)
        {
            names.Add(name);var observer=name.Contains("OBSERVER");
            return Secret(manifest,observer);
        }
        var connections=Phase7SqlSuiteLoader.Connections(manifest,manifest.TenantA,pooling,Resolve);
        using var writer=connections.Writer.CreateConnection();using var observer=connections.Observer.CreateConnection();
        Assert.Equal(System.Data.ConnectionState.Closed,writer.State);Assert.Equal(System.Data.ConnectionState.Closed,observer.State);
        Assert.Equal(pooling,new SqlConnectionStringBuilder(writer.ConnectionString).Pooling);
        Assert.False(new SqlConnectionStringBuilder(observer.ConnectionString).Pooling);
        Assert.Equal(new[]{"NUANSYSTEM_PHASE7_TENANT_A_CONNECTION_STRING","NUANSYSTEM_PHASE7_OBSERVER_A_CONNECTION_STRING"},names);
    }

    [Fact]
    public void Foreign_secret_references_are_rejected_before_resolution()
    {
        var (manifest,_)=Data();var calls=0;
        var target=manifest.TenantA with{ApplicationSecretReference="env:UNRELATED_SECRET"};
        Assert.Throws<Phase7SqlGuardException>(()=>Phase7SqlSuiteLoader.Connections(manifest,target,false,_=>{calls++;return null;}));
        Assert.Equal(0,calls);
    }

    [Fact]
    public void Production_database_in_a_secret_is_rejected_without_opening_it()
    {
        var (manifest,_)=Data();
        var builder=new SqlConnectionStringBuilder(Secret(manifest,false)){InitialCatalog="production"};
        Assert.Throws<Phase7SqlGuardException>(()=>Phase7SqlSuiteLoader.Connections(manifest,manifest.TenantA,false,_=>builder.ConnectionString));
    }

    [Fact]
    public void Shared_factories_apply_the_active_case_pooling_policy_without_reresolving_credentials()
    {
        var (manifest,_)=Data();var pooling=false;var resolutions=0;
        var connections=Phase7SqlSuiteLoader.Connections(manifest,manifest.TenantA,false,name=>
        {resolutions++;return Secret(manifest,name.Contains("OBSERVER"));},()=>pooling);
        using var dedicated=connections.Writer.CreateConnection();
        Assert.False(new SqlConnectionStringBuilder(dedicated.ConnectionString).Pooling);
        pooling=true;
        using var pooled=connections.Writer.CreateConnection();using var observer=connections.Observer.CreateConnection();
        Assert.True(new SqlConnectionStringBuilder(pooled.ConnectionString).Pooling);
        Assert.False(new SqlConnectionStringBuilder(observer.ConnectionString).Pooling);
        Assert.Equal(2,resolutions);
        Assert.All(new[]{dedicated,pooled,observer},c=>Assert.Equal(System.Data.ConnectionState.Closed,c.State));
    }

    [Fact]
    public void Process_lease_rejects_replay_configuration_drift_and_run_changes()
    {
        var run=Guid.NewGuid();var lease=new Phase7SqlSuiteProcessLease(run,"hash",TimeSpan.FromMinutes(1));
        lease.Take(run,"hash","S01");
        Assert.Throws<Phase7SqlGuardException>(()=>lease.Take(run,"hash","S01"));
        Assert.Throws<Phase7SqlGuardException>(()=>lease.Take(run,"other","S02"));
        Assert.Throws<Phase7SqlGuardException>(()=>lease.Take(Guid.NewGuid(),"hash","S02"));
    }

    [Fact]
    public void Failed_or_expired_lease_cannot_start_another_case()
    {
        var run=Guid.NewGuid();var stops=0;var lease=new Phase7SqlSuiteProcessLease(run,"hash",TimeSpan.FromMinutes(1),()=>stops++);
        lease.Fail();Assert.Equal(1,stops);
        Assert.Throws<Phase7SqlGuardException>(()=>lease.Take(run,"hash","S01"));
        lease=new(run,"hash",TimeSpan.Zero);
        Assert.Throws<Phase7SqlGuardException>(()=>lease.Take(run,"hash","S01"));
    }

    [Fact]
    public void Only_completion_of_all_catalog_cases_revokes_the_process_gates()
    {
        var run=Guid.NewGuid();var stops=0;var lease=new Phase7SqlSuiteProcessLease(run,"hash",TimeSpan.FromMinutes(1),()=>stops++);
        Assert.Throws<Phase7SqlGuardException>(()=>lease.Complete("S01"));
        foreach(var spec in Phase7SqlExecutionCatalog.Cases){lease.Take(run,"hash",spec.Id);lease.Complete(spec.Id);}
        Assert.Equal(1,stops);
        Assert.Throws<Phase7SqlGuardException>(()=>lease.Complete("S01"));
    }

    [Fact]
    public void Internal_rollback_requires_a_characterized_failure_and_never_accepts_silent_success()
    {
        Assert.False(Phase7SqlR01Outcome.IsExpected(null,true));
        Assert.False(Phase7SqlR01Outcome.IsExpected(new Exception(),true));
        Assert.False(Phase7SqlR01Outcome.IsExpected(new OperationCanceledException(),true));
        Assert.False(Phase7SqlR01Outcome.IsExpected(new Phase7SqlGuardException("drift"),true));
        Assert.False(Phase7SqlR01Outcome.IsExpected(new InvalidOperationException(),false));
        Assert.True(Phase7SqlR01Outcome.IsExpected(new InvalidOperationException(),true));
    }

    [Fact]
    public void Missing_row_update_uses_the_real_data_contract_and_explicit_audit_identity()
    {
        var command=Phase7NominalCommands.CreateCommand(7,"ident","P7_ABCDEF12_S20") with{Name="Synthetic",AuditUserId=42,AuditUserName="lab"};
        var data=Phase7SqlR01Data.Map(command,int.MaxValue);
        Assert.Equal(int.MaxValue,data.Id);Assert.Equal(command.Code,data.Code);Assert.Equal("Synthetic",data.Name);
        Assert.Equal(7,data.IdentificationTypeId);Assert.Equal(42,data.UpdatedByUserId);Assert.Equal("lab",data.UpdatedByUserName);
        Assert.Throws<Phase7SqlGuardException>(()=>Phase7SqlR01Data.Map(command,0));
    }

    [Fact]
    public async Task Procedure_error_is_injected_after_the_requested_callback_on_the_same_context()
    {
        var inner=Substitute.For<IBusinessPartnerUnitOfWork>();var context=Substitute.For<IBusinessPartnerTransaction>();
        var order=new List<string>();var primary=new InvalidOperationException("simulated FK");
        inner.ExecuteAsync(Arg.Any<Func<IBusinessPartnerTransaction,CancellationToken,Task<int>>>(),Arg.Any<CancellationToken>())
            .Returns(call=>call.Arg<Func<IBusinessPartnerTransaction,CancellationToken,Task<int>>>()(context,call.Arg<CancellationToken>()));
        context.CreateAsync(Arg.Any<CreateBusinessPartnerData>(),Arg.Any<CancellationToken>()).Returns(call=>
        {
            order.Add("SQL procedure");var data=call.Arg<CreateBusinessPartnerData>();Assert.Equal(-1,data.IdentificationTypeId);
            Assert.EndsWith("E",data.Code);return Task.FromException<int>(primary);
        });
        var command=Phase7NominalCommands.CreateCommand(7,"ident","P7_ABCDEF12_S09C");
        var unit=new Phase7SqlProcedureErrorUnit(inner,command,-1);
        var actual=await Assert.ThrowsAsync<InvalidOperationException>(()=>unit.ExecuteAsync((c,_)=>
        {Assert.Same(context,c);order.Add("CRUD and enqueue");return Task.FromResult(1);}));
        Assert.Same(primary,actual);Assert.Equal(new[]{"CRUD and enqueue","SQL procedure"},order);
    }

    [Fact]
    public void Insert_exec_rollback_rejection_is_characterized_from_all_provider_errors()
    {
        var error=SqlErrors(50000,3915,266);
        Assert.Equal(50000,error.Number); // Number is just the first error, not necessarily the diagnostic cause
        Assert.True(Phase7SqlR01Outcome.IsExpected(error,false));
        Assert.False(Phase7SqlR01Outcome.IsExpected(SqlErrors(-2,3915),false));
        Assert.False(Phase7SqlR01Outcome.IsExpected(SqlErrors(547),false));
    }

    private static SqlException SqlErrors(params int[] numbers)
    {
        // Construct provider exception data only. No connection, command or TDS session is involved.
        var collection=(SqlErrorCollection)Activator.CreateInstance(typeof(SqlErrorCollection),true)!;
        var constructor=typeof(SqlError).GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance)
            .First(c=>c.GetParameters().Length==9);
        var add=typeof(SqlErrorCollection).GetMethod("Add",BindingFlags.NonPublic|BindingFlags.Instance)!;
        foreach(var number in numbers)
        {
            var nativeCode=constructor.GetParameters()[7].ParameterType==typeof(uint)?(object)(uint)0:0;
            var error=constructor.Invoke(new object?[]{number,(byte)1,(byte)16,"offline","synthetic","offline",1,nativeCode,null});
            add.Invoke(collection,[error]);
        }
        var create=typeof(SqlException).GetMethods(BindingFlags.NonPublic|BindingFlags.Static)
            .First(m=>m.Name=="CreateException" && m.GetParameters().Length==2 && m.GetParameters()[0].ParameterType==typeof(SqlErrorCollection));
        return (SqlException)create.Invoke(null,[collection,"16.0.0.0"])!;
    }

    private static string Secret(Phase7SqlManifest manifest,bool observer)=>new SqlConnectionStringBuilder {
        DataSource=manifest.Server.DataSource,InitialCatalog=manifest.TenantA.DatabaseName,UserID=observer?"observer":"app",
        Password="offline-synthetic-value",Encrypt=SqlConnectionEncryptOption.Mandatory,TrustServerCertificate=false,
        Enlist=false,ConnectTimeout=10 }.ConnectionString;

    private static (Phase7SqlManifest,Phase7SqlSuiteInputs) Data()
    {
        var run=Guid.Parse("abcdef12-1234-1234-1234-123456789abc");
        Phase7SqlTarget Target(string role,int id)=>new(role,$"NuanSystem_Phase7_{run:N}_{role}",id,id,run,"BackendTransactionValidation",160,"test",false,new('A',64),
            new Dictionary<string,string>(),"app","observer",["SELECT","EXECUTE","INSERT"],["SELECT"],
            $"env:NUANSYSTEM_PHASE7_TENANT_{role}_CONNECTION_STRING",$"env:NUANSYSTEM_PHASE7_OBSERVER_{role}_CONNECTION_STRING");
        var manifest=new Phase7SqlManifest(1,run,"operator","exec","prep",DateTimeOffset.UtcNow.AddHours(-1),DateTimeOffset.UtcNow.AddHours(1),600,30,new("tcp:lab.example.test,1433","LAB","16.0.1.1"),Target("A",101),Target("B",102));
        var hashes=new[]{"Audit","Outbox","SyncAudit"}.ToDictionary(s=>$"TR_P7_{run:N}_{s}",_=>new string('B',64));
        return(manifest,new(run,"exec",new('A',64),Phase7SqlExecutionCatalog.Sha256,1,1,int.MaxValue,-1,hashes,new Dictionary<string,string>(hashes),"SYNTHETIC_LAB_ONLY_SAP_NONE_WORKERS_OFF_RELAY_OFF",new('B',64),new('C',64),new('D',64)));
    }
}
