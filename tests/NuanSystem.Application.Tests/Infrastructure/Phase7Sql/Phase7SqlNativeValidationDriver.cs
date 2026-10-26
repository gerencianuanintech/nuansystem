using Microsoft.Data.SqlClient;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal sealed record Phase7SqlApprovedMonitorResources(Phase7SqlMonitorInputs Inputs, Phase7SqlControlledMonitorApproval Approval,
    Phase7SqlMonitorIsolation Isolation, SqlConnection Connection);

// Explicit trusted host bridge. PowerShell runspace/session and the original test child belong to this host.
// No default implementation opens connections, launches a process or retrieves a secret.
internal sealed record Phase7SqlValidationHostBindings(
    Func<Phase7SqlValidationContext, CancellationToken, Task<Phase7SqlProvisioningReview>> ReviewProvisioning,
    Func<Phase7SqlValidationContext, CancellationToken, Task<Phase7SqlProvisioningCompletion>> Provision,
    Func<Phase7SqlValidationContext, CancellationToken, Task<Phase7SqlApprovedSuite?>> ApproveSuite,
    Func<Phase7SqlValidationContext, Phase7SqlApprovedSuite, CancellationToken, Task<Phase7SqlSuiteCompletion>> RunSuite,
    Func<Phase7SqlValidationContext, Phase7SqlApprovedSuite, Phase7SqlSuiteCompletion, CancellationToken, Task<Phase7SqlApprovedMonitorResources?>> ApproveMonitor,
    Func<CancellationToken, Task> Stop);

internal sealed class Phase7SqlNativeValidationDriver : IPhase7SqlValidationDriver
{
    private readonly Phase7SqlValidationHostBindings host;
    public Phase7SqlNativeValidationDriver(Phase7SqlValidationHostBindings host)
    {
        if (host.ReviewProvisioning is null || host.Provision is null || host.ApproveSuite is null || host.RunSuite is null
            || host.ApproveMonitor is null || host.Stop is null)
            throw new Phase7SqlGuardException("PHASE7_SQL_ORCHESTRATION_HOST_BINDINGS");
        this.host = host;
    }
    public Task<Phase7SqlProvisioningReview> ReviewProvisioningAsync(Phase7SqlValidationContext context, CancellationToken token) => host.ReviewProvisioning(context, token);
    public Task<Phase7SqlProvisioningCompletion> ProvisionAsync(Phase7SqlValidationContext context, CancellationToken token) => host.Provision(context, token);
    public Task<Phase7SqlApprovedSuite?> ApproveSuiteAsync(Phase7SqlValidationContext context, CancellationToken token) => host.ApproveSuite(context, token);
    public async Task<Phase7SqlSuiteCompletion> RunSuiteAsync(Phase7SqlValidationContext context, Phase7SqlApprovedSuite suite, CancellationToken token)
    {
        var result = await host.RunSuite(context, suite, token);
        if (result.Child is null || !result.Child.HasExited || result.Child.Id != result.ProcessId || result.Child.ExitCode != result.ExitCode)
            throw new Phase7SqlGuardException("PHASE7_SQL_ORCHESTRATION_CHILD");
        return result;
    }
    public async Task<Phase7SqlMonitoredRun?> MonitorAsync(Phase7SqlValidationContext context, Phase7SqlApprovedSuite suite,
        Phase7SqlSuiteCompletion completion, CancellationToken token)
    {
        var approved = await host.ApproveMonitor(context, suite, completion, token);
        if (approved is null) return null;
        if (approved.Inputs.RunRoot != context.SuiteRoot || approved.Inputs.OutputDirectory != context.MonitorDirectory
            || approved.Approval.Monitor.AuthorizationReference == context.Approval.AuthorizationId
            || approved.Approval.Monitor.AuthorizationReference == context.Plan.ProvisioningAuthorizationId)
            throw new Phase7SqlGuardException("PHASE7_SQL_ORCHESTRATION_MONITOR_BINDING");
        var result = await Phase7SqlMonitorExecutor.ExecuteAsync(approved.Inputs, suite.Approval, approved.Approval, approved.Isolation,
            approved.Connection, completion.Child, execute: true, token: token);
        return new(result.IndexSha256!, approved.Approval);
    }
    public Task StopAsync(CancellationToken token) => host.Stop(token);
}
