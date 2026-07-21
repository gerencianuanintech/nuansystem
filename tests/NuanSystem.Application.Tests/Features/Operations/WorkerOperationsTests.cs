using FluentAssertions;
using NuanSystem.Application.Features.Operations;
using NuanSystem.SriWorker.Services;

namespace NuanSystem.Application.Tests.Features.Operations;

public sealed class WorkerOperationsTests
{
    private static readonly DateTime Now = new(2026,7,21,12,0,0,DateTimeKind.Utc);

    [Fact]
    public void Health_TransitionsAtApprovedHeartbeatThresholds()
    {
        WorkerHealthEvaluator.Evaluate([Snapshot(lastBeat:Now.AddSeconds(-91))],new(),Now).OverallHealth.Should().Be("Degraded");
        WorkerHealthEvaluator.Evaluate([Snapshot(lastBeat:Now.AddSeconds(-181))],new(),Now).OverallHealth.Should().Be("Unhealthy");
    }

    [Fact]
    public void HealthThresholds_RejectInvertedConfiguration()
    {
        var thresholds=new WorkerHealthThresholds(HeartbeatDegradedSeconds:180,HeartbeatUnhealthySeconds:90);
        Action action=thresholds.Validate;
        action.Should().Throw<InvalidOperationException>().WithMessage("*umbrales*");
    }

    [Fact]
    public void Health_UsesQueueDeadLetterLeaseCertificateAndStorageThresholds()
    {
        var value=Snapshot(lastBeat:Now,retries:20,deadLetters:1,recentDeadLetters:5,expiredLeases:1,
            oldestPending:Now.AddMinutes(-31),storage:9m,certificateDays:14);
        var result=WorkerHealthEvaluator.Evaluate([value],new(),Now).Instances.Single();
        result.Health.Should().Be("Unhealthy");
        result.ReasonCodes.Should().Contain(["OLDEST_PENDING_CRITICAL","RETRY_SCHEDULED_CRITICAL","DEADLETTER_RATE_CRITICAL","EXPIRED_LEASE_PRESENT","CERTIFICATE_CRITICAL","STORAGE_CRITICAL"]);
    }

    [Fact]
    public void Health_ReturnsDisabledUnknownAndDetectsSecondActiveInstance()
    {
        WorkerHealthEvaluator.Evaluate([],new(),Now).OverallHealth.Should().Be("Unknown");
        WorkerHealthEvaluator.Evaluate([Snapshot(lastBeat:Now,enabled:false,lifecycle:WorkerLifecycleStates.Disabled)],new(),Now).OverallHealth.Should().Be("Disabled");
        var report=WorkerHealthEvaluator.Evaluate([Snapshot(lastBeat:Now,instance:"one"),Snapshot(lastBeat:Now,instance:"two")],new(),Now);
        report.OverallHealth.Should().Be("Unhealthy");
        report.Instances.Should().OnlyContain(x=>x.ReasonCodes.Contains("UNAUTHORIZED_SECOND_INSTANCE"));
    }

    [Fact]
    public void Health_DoesNotReportEnabledStoppedOrFailedCycleAsHealthy()
    {
        WorkerHealthEvaluator.Evaluate([Snapshot(lastBeat:Now,lifecycle:WorkerLifecycleStates.Stopped)],new(),Now).OverallHealth.Should().Be("Unhealthy");
        var failed=Snapshot(lastBeat:Now) with { LastCycleResult="Failed" };
        WorkerHealthEvaluator.Evaluate([failed],new(),Now).OverallHealth.Should().Be("Degraded");
    }

    [Fact]
    public void RuntimeGate_StopsClaimsAndPublishesOnlySafeErrorFields()
    {
        var state=new SriWorkerRuntimeState();
        state.MarkStarted(Now);
        state.MarkCycleStarted(Now);
        state.MarkCycleCompleted(Now.AddSeconds(1),1000,false);
        state.StopClaims();
        var snapshot=state.Snapshot();
        state.CanClaim.Should().BeFalse();
        snapshot.LifecycleState.Should().Be(WorkerLifecycleStates.Stopping);
        snapshot.LastSafeErrorCode.Should().Be("SRI_WORKER_CYCLE_FAILED");
        snapshot.LastSafeErrorMessage.Should().NotContain("Exception").And.NotContain("Password").And.NotContain("Connection String");
    }

    [Fact]
    public void SriIdentity_IsStableByHostAndInstance()
    {
        var first=SriWorkerRuntimeState.StorageKey("HOST","pilot-01");
        first.Should().Be(SriWorkerRuntimeState.StorageKey("HOST","pilot-01"));
        first.Should().NotBe(SriWorkerRuntimeState.StorageKey("HOST","pilot-02"));
        first.Should().StartWith("SRI-").And.HaveLength(36);
    }

    [Fact]
    public void SqlEvolution_IsForwardOnlyIdempotentAndKeepsSapCompatibility()
    {
        var master=Read("database","sql","120_master_worker_heartbeat_operations.sql");
        var tenant=Read("database","sql","121_tenant_sri_worker_operational_summary.sql");
        master.Should().Contain("COL_LENGTH").And.Contain("CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_WORKERHEARTBEAT_REGISTRAR")
            .And.Contain("UX_WorkerHeartbeat_LogicalIdentity").And.Contain("WHERE InstanceName=@InstanceName").And.Contain("WorkerType").And.Contain("MasterSchemaHistory")
            .And.NotContain("DROP TABLE").And.NotContain("DROP COLUMN").And.NotContain("sp_rename");
        tenant.Should().Contain("CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SRIWORKER_RESUMENOPERATIVO")
            .And.Contain("SchemaHistory").And.NotContain("NuanSystem_Master").And.NotContain("DROP ");
        Read("src","Backend","NuanSystem.SyncWorker","Workers","SapSyncWorker.cs")
            .Should().Contain("NuanSystem.Application.Features.Operations");
    }

    [Fact]
    public void ApiUiAndTemplates_KeepProtectedSafeBoundaries()
    {
        var endpoint=Read("src","Backend","NuanSystem.Api","Endpoints","SriDocumentEndpoints.cs");
        endpoint.Should().Contain("/monitor/worker-health").And.Contain("PermissionCodes.SriWorkerHealthView");
        var models=Read("src","Backend","NuanSystem.Application","Features","Operations","WorkerHeartbeatModels.cs");
        models.Should().NotContain("ConnectionString").And.NotContain("AccessKey").And.NotContain("XmlContent");
        var form=Read("src","Frontend","NuanSystem.WinForms.Forms","SriDocuments","SriDocumentMonitorForm.cs");
        var designer=Read("src","Frontend","NuanSystem.WinForms.Forms","SriDocuments","SriDocumentMonitorForm.Designer.cs");
        form.Should().Contain("RenderWorkerHealth").And.Contain("lblWorkerHealth");
        designer.Should().Contain("workerTab").And.Contain("DockStyle.Fill").And.Contain("AutoScaleMode=AutoScaleMode.Font")
            .And.Contain("MinimumSize=new Size(980,650)");
        var templates=Directory.GetFiles(Path.Combine(Root(),"docs","operations","templates","sri-worker"),"*.ps1");
        templates.Should().HaveCount(6);
        templates.Select(File.ReadAllText).Should().OnlyContain(x=>!x.Contains("TrustServerCertificate=true",StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ShutdownContract_ClosesGateBeforeBaseStopAndKeepsLeaseRecoveryProcedure()
    {
        var background=Read("src","Backend","NuanSystem.SriWorker","Workers","SriBackgroundWorker.cs");
        var processor=Read("src","Backend","NuanSystem.SriWorker","Services","SriWorkerProcessor.cs");
        var tenantWorker=Read("database","sql","117_tenant_sri_worker_and_document_store.sql");
        background.IndexOf("runtime.StopClaims()",StringComparison.Ordinal).Should().BeLessThan(background.IndexOf("base.StopAsync",StringComparison.Ordinal));
        processor.Should().Contain("!executionGate.CanClaim").And.Contain("cancellationToken.IsCancellationRequested");
        tenantWorker.Should().Contain("SP_NA_POST_SRIDOCUMENTQUEUE_LIBERARLEASESVENCIDOS").And.Contain("LeaseExpired");
    }

    private static WorkerHeartbeatSnapshotDto Snapshot(DateTime lastBeat,string instance="pilot",bool enabled=true,
        string lifecycle=WorkerLifecycleStates.Running,long retries=0,long deadLetters=0,long recentDeadLetters=0,
        long expiredLeases=0,DateTime? oldestPending=null,decimal? storage=null,int? certificateDays=null) =>
        new(WorkerTypes.Sri,"HOST",instance,lifecycle,enabled,"1.0",lastBeat,Now.AddHours(-1),Now,Now,Now,
            100,"Succeeded",null,null,1,0,retries,deadLetters,recentDeadLetters,0,expiredLeases,oldestPending,storage,certificateDays);

    private static string Read(params string[] parts)=>File.ReadAllText(Path.Combine([Root(),..parts]));
    private static string Root()
    {
        var directory=new DirectoryInfo(AppContext.BaseDirectory);
        while(directory is not null && !File.Exists(Path.Combine(directory.FullName,"NuanSystem.sln"))) directory=directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root not found.");
    }
}
