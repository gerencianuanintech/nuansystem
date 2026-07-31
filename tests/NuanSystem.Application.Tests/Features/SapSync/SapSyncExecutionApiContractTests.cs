using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapSyncExecutionApiContractTests
{
    [Fact]
    public void Endpoints_AreSeparatedProtectedAndRegistered()
    {
        var endpoints=Read("src","Backend","NuanSystem.Api","Endpoints","SapSyncExecutionEndpoints.cs");
        var program=Read("src","Backend","NuanSystem.Api","Program.cs");
        endpoints.Should().Contain("/api/sap/sync-executions")
            .And.Contain("SapSyncExecutionsView")
            .And.Contain("SapSyncExecutionsViewDetail")
            .And.Contain("SapSyncExecutionsRetry")
            .And.Contain("SapSyncExecutionsCancel")
            .And.Contain("SapSyncExecutionsReleaseExpiredLock")
            .And.Contain("GetSapSyncExecutionDetailsQuery")
            .And.NotContain("ProfileSnapshotJson")
            .And.NotContain("ApprovedSnapshotJson")
            .And.NotContain("SnapshotHash");
        program.Should().Contain("app.MapSapSyncExecutionEndpoints()");
    }

    [Fact]
    public void Migration_IsForwardOnlyAndExposesOnlySafeDetailProjection()
    {
        var sql=Read("database","sql","158_tenant_sap_sync_execution_operations.sql");
        sql.Should().Contain("20260731.158")
            .And.Contain("SP_NA_POST_SAPSYNCEXECUTIONREINTENTOMANUAL")
            .And.Contain("SP_NA_POST_SAPSYNCEXECUTIONDETALLERECUPERARVENCIDOS")
            .And.Contain("@ApprovedSnapshotTypesCsv")
            .And.NotContain("DROP TABLE")
            .And.NotContain("DELETE FROM dbo.SchemaHistory");
        var safePage=sql[..sql.IndexOf("CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SAPSYNCEXECUTIONDETALLECLAIM",StringComparison.Ordinal)];
        safePage.Should().NotContain("ApprovedSnapshotJson").And.NotContain("SnapshotHash");
    }

    private static string Read(params string[] parts) => File.ReadAllText(Path.Combine(WorkspaceRoot(),Path.Combine(parts)));
    private static string WorkspaceRoot()
    {
        var dir=new DirectoryInfo(AppContext.BaseDirectory);
        while(dir is not null && !File.Exists(Path.Combine(dir.FullName,"NuanSystem.sln"))) dir=dir.Parent;
        return dir?.FullName ?? throw new DirectoryNotFoundException("Workspace root not found.");
    }
}
