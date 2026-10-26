using System.Collections;
using System.Diagnostics;
using System.Security;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal sealed record Phase7SqlHostFile(string Path, string Sha256);
internal sealed record Phase7SqlOperationalHostOptions(Phase7SqlHostFile ProvisioningEntry, Phase7SqlHostFile ProvisioningModule,
    Phase7SqlHostFile AutomationAssembly, Phase7SqlHostFile Dotnet, Phase7SqlHostFile TestAssembly,
    Phase7SqlHostFile TestDependencies, Phase7SqlHostFile TestRuntimeConfiguration);
internal sealed record Phase7SqlOperationalHostApproval(Guid RunId, string PlanSha256, string OptionsSha256, string AuthorizationId, bool ExecutionAuthorized);
internal sealed class Phase7SqlProvisioningResources(SqlConnection connection, IDictionary approval, IReadOnlyDictionary<string, SecureString> secrets)
{
    internal SqlConnection Connection { get; } = connection;
    internal IDictionary Approval { get; } = approval;
    internal IReadOnlyDictionary<string, SecureString> Secrets { get; } = secrets;
    public override string ToString() => "Approved provisioning resources (redacted)";
}
internal sealed record Phase7SqlOperationalResources(
    Func<Phase7SqlValidationContext, CancellationToken, Task<Phase7SqlProvisioningResources>> Provisioning,
    Func<Phase7SqlValidationContext, CancellationToken, Task<Phase7SqlApprovedSuite?>> ApproveSuite,
    Func<Phase7SqlValidationContext, Phase7SqlApprovedSuite, CancellationToken, Task<IReadOnlyDictionary<string, string>>> SuiteConnections,
    Func<Phase7SqlValidationContext, Phase7SqlApprovedSuite, Phase7SqlSuiteCompletion, CancellationToken, Task<Phase7SqlApprovedMonitorResources?>> ApproveMonitor);
internal sealed record Phase7SqlPowerShellReply(string Mode, Guid? RunId, string? ManifestSha256, int BatchCount,
    int CompletedBatches, bool SqlExecuted, bool SapExecuted, bool GlobalClosureAccepted, string? JournalPath);
internal interface IPhase7SqlPowerShellEngine
{
    Task<Phase7SqlPowerShellReply> InvokeAsync(Phase7SqlOperationalHostOptions options, IDictionary parameters, CancellationToken token);
    Task StopAsync(CancellationToken token);
}
internal interface IPhase7SqlSuiteChild
{
    Process? Original { get; }
    int Id { get; }
    int ExitCode { get; }
    int RunnerExitCode => ExitCode;
    Task WaitAsync(CancellationToken token);
}
internal interface IPhase7SqlSuiteLauncher
{
    Task<IPhase7SqlSuiteChild> StartAsync(Phase7SqlSuiteLaunch launch, CancellationToken token);
    Task StopAsync(CancellationToken token);
}
internal sealed class Phase7SqlSuiteLaunch(ProcessStartInfo info, Phase7SqlOperationalHostOptions options, IReadOnlyList<Phase7SqlHostFile> inputs)
{
    internal ProcessStartInfo StartInfo { get; } = info;
    internal Phase7SqlOperationalHostOptions Options { get; } = options;
    internal IReadOnlyList<Phase7SqlHostFile> Inputs { get; } = inputs;
    public override string ToString() => "Restricted phase 7 test child (environment redacted)";
}

internal sealed class Phase7SqlOperationalHost
{
    private readonly Phase7SqlOperationalHostOptions options;
    private readonly Phase7SqlOperationalHostApproval approval;
    private readonly Phase7SqlOperationalResources resources;
    private readonly IPhase7SqlPowerShellEngine engine;
    private readonly IPhase7SqlSuiteLauncher launcher;
    private readonly TimeProvider clock;
    private readonly CancellationTokenSource lifetime = new();
    private readonly List<SqlConnection> connections = [];
    private Phase7SqlProvisioningReview? review;
    private bool provisioned;
    private Task? stop;
    private readonly object sync = new();
    public Phase7SqlValidationHostBindings Bindings { get; }
    public Phase7SqlOperationalHost(Phase7SqlOperationalHostOptions options, Phase7SqlOperationalHostApproval approval,
        Phase7SqlOperationalResources resources, IPhase7SqlPowerShellEngine? engine = null, IPhase7SqlSuiteLauncher? launcher = null, TimeProvider? clock = null)
    {
        this.options = options; this.approval = approval; this.resources = resources;
        this.engine = engine ?? new Phase7SqlNativePowerShellEngine(); this.launcher = launcher ?? new Phase7SqlNativeSuiteLauncher(); this.clock = clock ?? TimeProvider.System;
        Bindings = new(ReviewProvisioningAsync, ProvisionAsync, ApproveSuiteAsync, RunSuiteAsync, ApproveMonitorAsync, StopAsync);
    }
    public static string Fingerprint(Phase7SqlOperationalHostOptions options) => Phase7SqlEvidenceFiles.Hash(Phase7SqlEvidenceFiles.Encode(options));
    public Task<Phase7SqlValidationResult> RunAsync(Phase7SqlValidationPlan plan, Phase7SqlValidationApproval orchestratorApproval,
        bool execute = false, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested(); VerifyOptions(options);
        Require(approval.OptionsSha256 == Fingerprint(options) && approval.RunId == plan.RunId
            && approval.PlanSha256 == Phase7SqlValidationOrchestrator.Fingerprint(plan), "PROFILE_PIN");
        return Phase7SqlValidationOrchestrator.ExecuteAsync(plan, orchestratorApproval, Bindings, execute, token);
    }

    private void Admit(Phase7SqlValidationContext context)
    {
        Require(!lifetime.IsCancellationRequested, "STOPPED");
        Require(approval.ExecutionAuthorized && context.Approval.ExecutionAuthorized && approval.RunId == context.Plan.RunId
            && approval.PlanSha256 == context.Approval.PlanSha256 && approval.PlanSha256 == Phase7SqlValidationOrchestrator.Fingerprint(context.Plan)
            && approval.OptionsSha256 == Fingerprint(options) && !string.IsNullOrWhiteSpace(approval.AuthorizationId)
            && !new[] { context.Approval.AuthorizationId, context.Plan.PreparationAuthorizationId, context.Plan.ProvisioningAuthorizationId }.Contains(approval.AuthorizationId, StringComparer.Ordinal)
            && clock.GetUtcNow() >= context.Plan.NotBeforeUtc && clock.GetUtcNow() < context.Plan.ExpiresAtUtc, "APPROVAL");
        VerifyOptions(options);
    }
    internal static void VerifyOptions(Phase7SqlOperationalHostOptions value)
    {
        foreach (var file in Files(value)) using (LockFile(file)) { }
        Require(Path.GetFileName(value.ProvisioningEntry.Path) == "Invoke-Phase7SqlProvisioning.ps1"
            && Path.GetFileName(value.ProvisioningModule.Path) == "Phase7SqlProvisioningExecutor.psm1"
            && SamePath(Path.GetDirectoryName(value.ProvisioningEntry.Path)!, Path.GetDirectoryName(value.ProvisioningModule.Path)!)
            && Path.GetFileName(value.AutomationAssembly.Path) == "System.Management.Automation.dll"
            && Path.GetFileName(value.Dotnet.Path) is "dotnet.exe" or "dotnet"
            && Path.GetFileName(value.TestAssembly.Path) == "NuanSystem.Application.Tests.dll"
            && SamePath(value.TestDependencies.Path, Path.ChangeExtension(value.TestAssembly.Path, ".deps.json"))
            && SamePath(value.TestRuntimeConfiguration.Path, Path.ChangeExtension(value.TestAssembly.Path, ".runtimeconfig.json")), "FILES");
    }
    internal static IEnumerable<Phase7SqlHostFile> Files(Phase7SqlOperationalHostOptions o) =>
        [o.ProvisioningEntry, o.ProvisioningModule, o.AutomationAssembly, o.Dotnet, o.TestAssembly, o.TestDependencies, o.TestRuntimeConfiguration];
    internal static FileStream LockFile(Phase7SqlHostFile file)
    {
        Require(Path.IsPathFullyQualified(file.Path) && Phase7SqlEvidenceFiles.IsHash(file.Sha256), "FILE_PIN"); Phase7SqlEvidenceFiles.CheckNoLinks(file.Path);
        FileStream? stream = null;
        try
        {
            stream = new(file.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            Require(stream.Length <= 32 * 1024 * 1024 && Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(stream)) == file.Sha256, "FILE_HASH");
            stream.Position = 0; return stream;
        }
        catch (Phase7SqlGuardException) { stream?.Dispose(); throw; }
        catch { stream?.Dispose(); throw Guard("FILE_READ"); }
    }
    private async Task<Phase7SqlProvisioningReview> ReviewProvisioningAsync(Phase7SqlValidationContext c, CancellationToken token)
    {
        Admit(c); using var linked = CancellationTokenSource.CreateLinkedTokenSource(token, lifetime.Token);
        var p = Parameters(c); var reply = await engine.InvokeAsync(options, p, linked.Token);
        Require(reply.Mode == "Review" && reply.RunId == c.Plan.RunId && reply.ManifestSha256 == c.Plan.ProvisioningBundleSha256
            && reply.BatchCount is >= 9 and <= 2200 && !reply.SqlExecuted && !reply.SapExecuted && !reply.GlobalClosureAccepted, "REVIEW_REPLY");
        review = new(c.Plan.RunId, reply.ManifestSha256!, reply.BatchCount, false); return review;
    }
    private async Task<Phase7SqlProvisioningCompletion> ProvisionAsync(Phase7SqlValidationContext c, CancellationToken token)
    {
        Admit(c); Require(review is not null && review.RunId == c.Plan.RunId && review.ManifestSha256 == c.Plan.ProvisioningBundleSha256 && !provisioned, "PROVISION_ORDER");
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(token, lifetime.Token);
        var resource = await resources.Provisioning(c, linked.Token);
        await RegisterAsync(resource.Connection);
        Require((string?)resource.Approval["RunId"] == c.Plan.RunId.ToString()
            && (string?)resource.Approval["ManifestSha256"] == c.Plan.ProvisioningBundleSha256
            && (string?)resource.Approval["AuthorizationId"] == c.Plan.ProvisioningAuthorizationId
            && (string?)resource.Approval["PreparationAuthorizationId"] == c.Plan.PreparationAuthorizationId, "PROVISION_APPROVAL");
        Require(resource.Secrets.Count == 5 && new[] { "AppA", "ObserverA", "AppB", "ObserverB", "Monitor" }
            .All(k => resource.Secrets.TryGetValue(k, out var s) && s.Length is >= 32 and <= 128), "SECRETS");
        var copies = new Hashtable(StringComparer.Ordinal); foreach (var pair in resource.Secrets) copies.Add(pair.Key, pair.Value.Copy());
        try
        {
            var p = Parameters(c); p["Execute"] = true; p["Approval"] = resource.Approval; p["Secrets"] = copies;
            p["Connection"] = resource.Connection; p["JournalPath"] = c.ProvisioningJournalPath; p["CancellationToken"] = linked.Token;
            var reply = await engine.InvokeAsync(options, p, linked.Token);
            Require(reply.Mode == "Executed" && reply.SqlExecuted && !reply.SapExecuted && !reply.GlobalClosureAccepted
                && reply.CompletedBatches == review!.BatchCount && reply.JournalPath == c.ProvisioningJournalPath, "PROVISION_REPLY");
            var journal = Phase7SqlEvidenceFiles.Read(c.Plan.OutputDirectory, "provisioning.jsonl", 16 * 1024 * 1024, linked.Token);
            provisioned = true; return new(c.Plan.RunId, Phase7SqlEvidenceFiles.Hash(journal), true);
        }
        finally { foreach (SecureString secret in copies.Values) secret.Dispose(); copies.Clear(); }
    }
    private async Task<Phase7SqlApprovedSuite?> ApproveSuiteAsync(Phase7SqlValidationContext c, CancellationToken token)
    {
        Admit(c); Require(provisioned, "PROVISION_ORDER"); using var linked = CancellationTokenSource.CreateLinkedTokenSource(token, lifetime.Token);
        return await resources.ApproveSuite(c, linked.Token);
    }
    private async Task<Phase7SqlSuiteCompletion> RunSuiteAsync(Phase7SqlValidationContext c, Phase7SqlApprovedSuite suite, CancellationToken token)
    {
        Admit(c); Require(provisioned, "PROVISION_ORDER");
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(token, lifetime.Token);
        var manifest = Phase7SqlManifestValidator.Validate(suite.ManifestJson, suite.Approval, new(true, true), clock.GetUtcNow()).Manifest;
        c.Plan.Candidate.Verify(linked.Token);
        Require(c.Plan.Candidate.BinaryFiles.Any(f => SamePath(Path.Combine(c.Plan.Candidate.RootPath, f.RelativePath), options.TestAssembly.Path) && f.Sha256 == options.TestAssembly.Sha256), "TEST_CANDIDATE");
        var expected = new Dictionary<string, byte[]> { ["manifest.json"] = System.Text.Encoding.UTF8.GetBytes(suite.ManifestJson),
            ["approval.json"] = JsonSerializer.SerializeToUtf8Bytes(suite.Approval, Phase7SqlValidationOrchestrator.Web),
            ["suite-inputs.json"] = JsonSerializer.SerializeToUtf8Bytes(suite.Inputs, Phase7SqlValidationOrchestrator.Web),
            ["candidate.json"] = Phase7SqlEvidenceFiles.Encode(c.Plan.Candidate), ["offline-baseline.trx"] = suite.OfflineBaseline };
        var inputs = new List<Phase7SqlHostFile>();
        foreach (var file in expected)
        {
            Require(Phase7SqlEvidenceFiles.Read(c.PreparedDirectory, file.Key, 16 * 1024 * 1024, linked.Token).SequenceEqual(file.Value), "PREPARED_INPUT");
            inputs.Add(new(Path.Combine(c.PreparedDirectory, file.Key), Phase7SqlEvidenceFiles.Hash(file.Value)));
        }
        inputs.AddRange(c.Plan.Candidate.BinaryFiles.Select(f => new Phase7SqlHostFile(Phase7SqlEvidenceFiles.Resolve(c.Plan.Candidate.RootPath, f.RelativePath), f.Sha256)));
        var secrets = await resources.SuiteConnections(c, suite, linked.Token);
        var launch = Phase7SqlSuiteLaunchBuilder.Create(options, c, manifest, secrets, inputs.AsReadOnly());
        try
        {
            var child = await launcher.StartAsync(launch, linked.Token); await child.WaitAsync(linked.Token);
            return new(child.Id, child.ExitCode, child.Original, child.RunnerExitCode);
        }
        catch (OperationCanceledException) { await StopChildBestEffort(); throw; }
        catch (Phase7SqlGuardException) { await StopChildBestEffort(); throw; }
        catch { await StopChildBestEffort(); throw Guard("SUITE_FAILED"); }
    }
    private async Task StopChildBestEffort()
    { using var stopToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)); try { await Task.Run(() => launcher.StopAsync(stopToken.Token)).WaitAsync(stopToken.Token); } catch { } }
    private async Task<Phase7SqlApprovedMonitorResources?> ApproveMonitorAsync(Phase7SqlValidationContext c, Phase7SqlApprovedSuite suite, Phase7SqlSuiteCompletion completion, CancellationToken token)
    {
        Admit(c); using var linked = CancellationTokenSource.CreateLinkedTokenSource(token, lifetime.Token);
        var value = await resources.ApproveMonitor(c, suite, completion, linked.Token);
        if (value is not null) await RegisterAsync(value.Connection); return value;
    }
    private async Task RegisterAsync(SqlConnection connection)
    {
        lock (sync) { if (!lifetime.IsCancellationRequested) { connections.Add(connection); return; } }
        try { await Task.Run(() => connection.CloseAsync()).WaitAsync(TimeSpan.FromSeconds(5)); }
        catch { throw Guard("LATE_RESOURCE_CLOSE"); }
        throw Guard("STOPPED");
    }
    public Task StopAsync(CancellationToken token)
    {
        lock (sync) return stop ??= StopCore(token);
    }
    private async Task StopCore(CancellationToken token)
    {
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(token); deadline.CancelAfter(TimeSpan.FromSeconds(5)); token = deadline.Token;
        lifetime.Cancel(); SqlConnection[] owned; lock (sync) owned = connections.ToArray();
        var work = new List<Task> { Task.Run(() => engine.StopAsync(token)), Task.Run(() => launcher.StopAsync(token)) };
        work.AddRange(owned.Select(connection => Task.Run(async () => { if (connection.State != System.Data.ConnectionState.Closed) await connection.CloseAsync(); })));
        try { await Task.WhenAll(work).WaitAsync(token); } catch (OperationCanceledException) { throw; } catch { throw Guard("STOP"); }
    }
    private static Hashtable Parameters(Phase7SqlValidationContext c) => new(StringComparer.Ordinal)
        { ["BundleDirectory"] = c.Plan.ProvisioningBundleDirectory, ["ExpectedManifestSha256"] = c.Plan.ProvisioningBundleSha256 };
    internal static bool SamePath(string a, string b) => Path.GetFullPath(a).Equals(Path.GetFullPath(b), StringComparison.OrdinalIgnoreCase);
    internal static void Require(bool valid, string code) { if (!valid) throw Guard(code); }
    internal static Phase7SqlGuardException Guard(string code) => new("PHASE7_SQL_OPERATIONAL_HOST_" + code);
}
