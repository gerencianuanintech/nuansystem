using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Security;
using System.Text;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

[Trait("Category", "OfflineSafety")]
public sealed class Phase7SqlOperationalHostTests
{
    [Fact]
    public async Task Default_review_never_loads_powershell_starts_process_or_requests_secrets()
    {
        var f = new Fixture(); var result = await f.Host.RunAsync(f.Context.Plan, f.Context.Approval);
        Assert.Equal("Review", result.State); Assert.Equal(0, f.Engine.Calls); Assert.Equal(0, f.Launcher.Starts);
        Assert.Equal(0, f.ResourceCalls); Assert.False(Directory.Exists(f.Context.Plan.OutputDirectory));
    }

    [Fact]
    public async Task Provisioning_adapter_uses_fixed_entry_and_object_parameters_without_opening_connection()
    {
        var f = new Fixture(); await f.ReviewAndProvision();
        Assert.Equal(2, f.Engine.Calls); Assert.Equal(f.Options.ProvisioningEntry.Path, f.Engine.LastEntry);
        Assert.Same(f.Connection, f.Engine.LastParameters!["Connection"]);
        Assert.IsAssignableFrom<IDictionary>(f.Engine.LastParameters["Secrets"]);
        Assert.Equal(f.Context.ProvisioningJournalPath, f.Engine.LastParameters["JournalPath"]);
        Assert.Equal(ConnectionState.Closed, f.Connection.State); // Fake engine accepted protocol; real PowerShell rejects closed SQL.
        Assert.DoesNotContain("SYNTHETIC-SECRET", File.ReadAllText(f.Context.ProvisioningJournalPath));
        await f.Host.StopAsync(default);
    }

    [Fact]
    public async Task Suite_adapter_uses_restricted_filter_pinned_binary_and_child_only_secrets()
    {
        var f = new Fixture(); await f.ReviewAndProvision();
        var parent = Environment.GetEnvironmentVariable(Phase7SqlGates.IsolatedEnvironmentVariable);
        var result = await f.Host.Bindings.RunSuite(f.Context, f.Suite, default);
        Assert.Equal(1, f.Launcher.Starts); var info = f.Launcher.Last!.StartInfo;
        Assert.False(info.UseShellExecute); Assert.True(info.CreateNoWindow);
        Assert.Equal(f.Options.Dotnet.Path, info.FileName);
        Assert.Equal("vstest", info.ArgumentList[0]); Assert.Contains(f.Options.TestAssembly.Path, info.ArgumentList);
        Assert.Contains("--TestCaseFilter:" + Phase7SqlExecutionCatalog.RestrictedFilter, info.ArgumentList);
        Assert.DoesNotContain(info.ArgumentList, a => a.Contains("SYNTHETIC-SECRET", StringComparison.Ordinal));
        Assert.Equal("1", info.Environment[Phase7SqlGates.IsolatedEnvironmentVariable]); Assert.Equal("0", info.Environment["NUAN_RUN_SAP_PO_PILOT"]);
        Assert.Equal(f.Context.SuiteRoot, info.Environment["NUANSYSTEM_PHASE7_RUN_ROOT"]);
        Assert.Equal(parent, Environment.GetEnvironmentVariable(Phase7SqlGates.IsolatedEnvironmentVariable));
        Assert.Equal(4, info.Environment.Keys.Count(k => k.StartsWith("NUANSYSTEM_PHASE7_", StringComparison.Ordinal) && k.EndsWith("_CONNECTION_STRING", StringComparison.Ordinal)));
        Assert.Equal(12345, result.ProcessId); Assert.Equal(0, result.ExitCode);
        Assert.DoesNotContain("SYNTHETIC-SECRET", f.Launcher.Last.ToString());
        await f.Host.StopAsync(default);
    }

    [Theory]
    [InlineData("HostPin")][InlineData("Unauthorized")][InlineData("Script")][InlineData("Module")][InlineData("Assembly")][InlineData("Deps")]
    public async Task Modified_or_unapproved_host_does_not_invoke_platform(string change)
    {
        var f = new Fixture();
        if (change == "HostPin") f.Rebuild(f.Approval with { OptionsSha256 = new('F', 64) });
        if (change == "Unauthorized") f.Rebuild(f.Approval with { ExecutionAuthorized = false });
        if (change == "Script") File.AppendAllText(f.Options.ProvisioningEntry.Path, "tamper");
        if (change == "Module") File.AppendAllText(f.Options.ProvisioningModule.Path, "tamper");
        if (change == "Assembly") File.AppendAllText(f.Options.TestAssembly.Path, "tamper");
        if (change == "Deps") File.AppendAllText(f.Options.TestDependencies.Path, "tamper");
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => f.Host.Bindings.ReviewProvisioning(f.Context, default));
        Assert.Equal(0, f.Engine.Calls); Assert.Equal(0, f.Launcher.Starts); Assert.Equal(0, f.ResourceCalls);
    }

    [Theory]
    [InlineData("SqlReview")][InlineData("Sap")][InlineData("Closure")][InlineData("WrongHash")]
    public async Task Powershell_reply_cannot_activate_or_misrepresent_review(string change)
    {
        var f = new Fixture(); f.Engine.BadReply = change;
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => f.Host.Bindings.ReviewProvisioning(f.Context, default));
        Assert.Equal(0, f.ResourceCalls); Assert.Equal(0, f.Launcher.Starts);
        await f.Host.StopAsync(default);
    }

    [Fact]
    public async Task Unreviewed_provision_and_unprovisioned_suite_are_rejected()
    {
        var f = new Fixture();
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => f.Host.Bindings.Provision(f.Context, default));
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => f.Host.Bindings.RunSuite(f.Context, f.Suite, default));
        Assert.Equal(0, f.Engine.Calls); Assert.Equal(0, f.ResourceCalls); Assert.Equal(0, f.Launcher.Starts);
    }

    [Theory]
    [InlineData("Missing")][InlineData("Extra")][InlineData("WrongDatabase")]
    public async Task Invalid_suite_connections_never_start_child(string change)
    {
        var f = new Fixture(); await f.ReviewAndProvision();
        if (change == "Missing") f.Connections.Remove("NUANSYSTEM_PHASE7_OBSERVER_B_CONNECTION_STRING");
        if (change == "Extra") f.Connections.Add("UNAPPROVED", "SYNTHETIC-SECRET");
        if (change == "WrongDatabase") f.Connections["NUANSYSTEM_PHASE7_TENANT_A_CONNECTION_STRING"] = "Server=wrong;Database=wrong";
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => f.Host.Bindings.RunSuite(f.Context, f.Suite, default));
        Assert.Equal(0, f.Launcher.Starts); await f.Host.StopAsync(default);
    }

    [Fact]
    public async Task Cancellation_stops_owned_child_and_stop_is_idempotent()
    {
        var f = new Fixture(); await f.ReviewAndProvision(); f.Launcher.Delay = true;
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(200));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => f.Host.Bindings.RunSuite(f.Context, f.Suite, cts.Token));
        await f.Host.StopAsync(default); await f.Host.StopAsync(default);
        Assert.True(f.Launcher.Stops > 0); Assert.Equal(1, f.Engine.Stops);
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => f.Host.Bindings.ReviewProvisioning(f.Context, default));
    }

    [Fact]
    public void Explicit_suite_root_is_bound_to_run_without_environment_mutation()
    {
        var f = new Fixture();
        Assert.Equal(f.Context.SuiteRoot, Phase7SqlSuiteLoader.ResolveRunRoot(f.Context.Plan.RunId, f.Context.SuiteRoot, f.Root));
        Assert.Equal(Path.Combine(f.Root, "artifacts", "phase7-sql-executions", f.Context.Plan.RunId.ToString("N")), Phase7SqlSuiteLoader.ResolveRunRoot(f.Context.Plan.RunId, null, f.Root));
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlSuiteLoader.ResolveRunRoot(f.Context.Plan.RunId, "relative", f.Root));
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlSuiteLoader.ResolveRunRoot(Guid.NewGuid(), f.Context.SuiteRoot, f.Root));
    }

    [Theory]
    [InlineData("manifest.json")][InlineData("approval.json")][InlineData("suite-inputs.json")][InlineData("candidate.json")][InlineData("offline-baseline.trx")]
    public async Task Modified_prepared_inputs_do_not_reach_launcher(string path)
    {
        var f = new Fixture(); await f.ReviewAndProvision();
        File.AppendAllText(Path.Combine(f.Context.PreparedDirectory, path), "tamper");
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => f.Host.Bindings.RunSuite(f.Context, f.Suite, default));
        Assert.Equal(0, f.Launcher.Starts); await f.Host.StopAsync(default);
    }

    [Fact]
    public async Task Native_launcher_rejects_arbitrary_command_before_starting_any_process()
    {
        var f = new Fixture(); var native = new Phase7SqlNativeSuiteLauncher();
        var launch = new Phase7SqlSuiteLaunch(new ProcessStartInfo("unapproved.exe"), f.Options, []);
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => native.StartAsync(launch, default));
        using var cancellation = new CancellationTokenSource(); cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => native.StartAsync(launch, cancellation.Token));
        await native.StopAsync(default);
    }

    [Fact]
    public void Runner_and_actual_testhost_are_distinct_and_bound_by_live_announcement()
    {
        var run = Guid.NewGuid(); var started = DateTimeOffset.UtcNow; var nonce = new string('A', 64); var hash = new string('B', 64);
        var value = new Phase7SqlTestHostAnnouncement(run, nonce, 202, started, hash);
        Phase7SqlSuiteHostHandshake.Validate(value, run, nonce, hash, 101, 202, started, true);
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlSuiteHostHandshake.Validate(value with { ProcessId = 101 }, run, nonce, hash, 101, 101, started, true));
    }

    [Theory]
    [InlineData("Nonce")][InlineData("Run")][InlineData("Assembly")][InlineData("Pid")][InlineData("ReusedPid")][InlineData("Unrelated")]
    public void Invalid_testhost_announcement_cannot_reach_sql_fixture(string change)
    {
        var run = Guid.NewGuid(); var started = DateTimeOffset.UtcNow; var nonce = new string('A', 64); var hash = new string('B', 64);
        var value = new Phase7SqlTestHostAnnouncement(run, nonce, 202, started, hash);
        value = change switch { "Nonce" => value with { Nonce = "wrong" }, "Run" => value with { RunId = Guid.NewGuid() }, "Assembly" => value with { AssemblySha256 = new string('C', 64) }, _ => value };
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlSuiteHostHandshake.Validate(value, run, nonce, hash, 101,
            change == "Pid" ? 999 : 202, change == "ReusedPid" ? started.AddSeconds(1) : started, change != "Unrelated"));
    }

    [Fact]
    public async Task Real_local_handshake_identifies_existing_testhost_without_launching_any_process()
    {
        var run = Guid.NewGuid(); var nonce = new string('A', 64); var name = "NuanPhase7_" + Guid.NewGuid().ToString("N");
        using var current = Process.GetCurrentProcess();
        using var runner = Process.GetProcessById(Phase7SqlSuiteHostHandshake.ParentId(current));
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        using var server = new System.IO.Pipes.NamedPipeServerStream(name, System.IO.Pipes.PipeDirection.InOut, 1,
            System.IO.Pipes.PipeTransmissionMode.Byte, System.IO.Pipes.PipeOptions.Asynchronous | System.IO.Pipes.PipeOptions.CurrentUserOnly);
        var hash = Phase7SqlEvidenceFiles.Hash(File.ReadAllBytes(typeof(Phase7SqlSuiteHostHandshake).Assembly.Location));
        var accepted = Phase7SqlSuiteHostHandshake.AcceptAsync(server, run, nonce, hash, runner, timeout.Token);
        using var client = new System.IO.Pipes.NamedPipeClientStream(".", name, System.IO.Pipes.PipeDirection.InOut, System.IO.Pipes.PipeOptions.Asynchronous);
        await client.ConnectAsync(timeout.Token);
        using var writer = new StreamWriter(client, new UTF8Encoding(false), 4096, true) { AutoFlush = true };
        using var reader = new StreamReader(client, Encoding.UTF8, false, 4096, true);
        await writer.WriteLineAsync(Encoding.UTF8.GetString(Phase7SqlEvidenceFiles.Encode(new Phase7SqlTestHostAnnouncement(run, nonce, current.Id, new(current.StartTime.ToUniversalTime()), hash))));
        Assert.Equal("ACK", await reader.ReadLineAsync(timeout.Token));
        using var original = await accepted;
        Assert.Equal(current.Id, original.Id); Assert.NotEqual(runner.Id, original.Id);
    }

    private sealed class Fixture
    {
        public string Root { get; } = Phase7SqlEvidenceFilesTests.Root();
        public Phase7SqlSyntheticPackage Sample { get; }
        public Phase7SqlValidationContext Context { get; }
        public Phase7SqlOperationalHostOptions Options { get; }
        public Phase7SqlOperationalHostApproval Approval { get; }
        public Phase7SqlOperationalHost Host { get; private set; } = null!;
        public Phase7SqlApprovedSuite Suite { get; }
        public SqlConnection Connection { get; } = new();
        public Engine Engine { get; } = new();
        public Launcher Launcher { get; } = new();
        public Dictionary<string, string> Connections { get; } = new();
        public int ResourceCalls;
        public Fixture()
        {
            Sample = new(Path.Combine(Root, "sample")); var bundle = Path.Combine(Root, "bundle"); Directory.CreateDirectory(bundle);
            var spec = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { Sample.Manifest.RunId, PreparationAuthorizationId = "PREP-AUTH", ExpectedServerName = "LAB", ProductVersion = "16.0.1000.6", CompanyIdA = 101, CompanyIdB = 102, CompatibilityLevel = 160, Collation = "SQL_Latin1_General_CP1_CI_AS", ReadCommittedSnapshot = false }));
            File.WriteAllBytes(Path.Combine(bundle, "provisioning-spec.json"), spec);
            var manifest = Phase7SqlEvidenceFiles.Encode(new { Sample.Manifest.RunId, PreparationAuthorizationId = "PREP-AUTH", SpecSha256 = Phase7SqlEvidenceFiles.Hash(spec), ExecutionAuthorized = false, SqlExecuted = false, SapExecuted = false,
                Files = new[] { new { Path = "provisioning-spec.json", Bytes = spec.Length, Sha256 = Phase7SqlEvidenceFiles.Hash(spec) } } });
            File.WriteAllBytes(Path.Combine(bundle, "bundle-manifest.json"), manifest);
            var plan = new Phase7SqlValidationPlan(Sample.Manifest.RunId, Path.Combine(Root, "out"), bundle, Phase7SqlEvidenceFiles.Hash(manifest), "PREP-AUTH", "PROVISION-EXEC", Sample.Candidate, Sample.Started.AddHours(-1), Sample.Started.AddHours(1), 3600, 120);
            Context = new(plan, new(plan.RunId, Phase7SqlValidationOrchestrator.Fingerprint(plan), "COORD-AUTH", true));
            Phase7SqlHostFile Pin(string name, string text) { var path = Path.Combine(Root, name); Directory.CreateDirectory(Path.GetDirectoryName(path)!); File.WriteAllText(path, text); return new(path, Phase7SqlEvidenceFiles.Hash(File.ReadAllBytes(path))); }
            var assembly = Path.Combine(Sample.Candidate.RootPath, "bin/NuanSystem.Application.Tests.dll");
            Options = new(Pin("ps/Invoke-Phase7SqlProvisioning.ps1", "offline fake"), Pin("ps/Phase7SqlProvisioningExecutor.psm1", "offline fake module"), Pin("runtime/System.Management.Automation.dll", "offline fake runtime"),
                Pin("dotnet.exe", "offline fake executable"), new(assembly, Phase7SqlEvidenceFiles.Hash(File.ReadAllBytes(assembly))),
                Pin(Path.GetRelativePath(Root, Path.ChangeExtension(assembly, ".deps.json")), "{}"), Pin(Path.GetRelativePath(Root, Path.ChangeExtension(assembly, ".runtimeconfig.json")), "{}"));
            Approval = new(plan.RunId, Context.Approval.PlanSha256, Phase7SqlOperationalHost.Fingerprint(Options), "HOST-AUTH", true);
            Suite = new(File.ReadAllText(Path.Combine(Sample.Root, "manifest.json")), Sample.Approval, Sample.Inputs, File.ReadAllBytes(Path.Combine(Sample.Root, "offline-baseline.trx")));
            foreach (var target in new[] { Sample.Manifest.TenantA, Sample.Manifest.TenantB }) foreach (var observer in new[] { false, true })
                Connections.Add($"NUANSYSTEM_PHASE7_{(observer ? "OBSERVER" : "TENANT")}_{target.Role}_CONNECTION_STRING", new SqlConnectionStringBuilder { DataSource = Sample.Manifest.Server.DataSource, InitialCatalog = target.DatabaseName, UserID = observer ? target.ObserverPrincipal : target.ApplicationPrincipal,
                    Password = "SYNTHETIC-SECRET-NOT-REAL", Encrypt = SqlConnectionEncryptOption.Mandatory, Enlist = false, TrustServerCertificate = false, ConnectTimeout = 5 }.ConnectionString);
            Rebuild(Approval);
        }
        public void Rebuild(Phase7SqlOperationalHostApproval approval)
        {
            var resources = new Phase7SqlOperationalResources(
                (_, _) => { ResourceCalls++; var secrets = new Dictionary<string, SecureString>(); foreach (var k in new[] { "AppA", "ObserverA", "AppB", "ObserverB", "Monitor" }) { var s = new SecureString(); foreach (var c in "SYNTHETIC-SECRET-NOT-REAL-123456789-" + k) s.AppendChar(c); secrets.Add(k, s); }
                    IDictionary a = new Hashtable { ["RunId"] = Context.Plan.RunId.ToString(), ["ManifestSha256"] = Context.Plan.ProvisioningBundleSha256, ["AuthorizationId"] = Context.Plan.ProvisioningAuthorizationId, ["PreparationAuthorizationId"] = Context.Plan.PreparationAuthorizationId };
                    return Task.FromResult(new Phase7SqlProvisioningResources(Connection, a, secrets)); },
                (_, _) => Task.FromResult<Phase7SqlApprovedSuite?>(Suite),
                (_, _, _) => Task.FromResult<IReadOnlyDictionary<string, string>>(Connections),
                (_, _, _, _) => Task.FromResult<Phase7SqlApprovedMonitorResources?>(null));
            Host = new(Options, approval, resources, Engine, Launcher, new Clock(Sample.Started));
        }
        public async Task ReviewAndProvision()
        {
            Directory.CreateDirectory(Context.Plan.OutputDirectory); await Host.Bindings.ReviewProvisioning(Context, default); await Host.Bindings.Provision(Context, default);
            Phase7SqlEvidenceFiles.Write(Context.PreparedDirectory, "manifest.json", Encoding.UTF8.GetBytes(Suite.ManifestJson));
            Phase7SqlEvidenceFiles.Write(Context.PreparedDirectory, "approval.json", JsonSerializer.SerializeToUtf8Bytes(Suite.Approval, Phase7SqlValidationOrchestrator.Web));
            Phase7SqlEvidenceFiles.Write(Context.PreparedDirectory, "suite-inputs.json", JsonSerializer.SerializeToUtf8Bytes(Suite.Inputs, Phase7SqlValidationOrchestrator.Web));
            Phase7SqlEvidenceFiles.Write(Context.PreparedDirectory, "candidate.json", Phase7SqlEvidenceFiles.Encode(Context.Plan.Candidate));
            Phase7SqlEvidenceFiles.Write(Context.PreparedDirectory, "offline-baseline.trx", Suite.OfflineBaseline);
        }
    }
    private sealed class Clock(DateTimeOffset now) : TimeProvider { public override DateTimeOffset GetUtcNow() => now; }
    private sealed class Engine : IPhase7SqlPowerShellEngine
    {
        public int Calls; public int Stops; public string? LastEntry; public IDictionary? LastParameters; public string? BadReply;
        public Task<Phase7SqlPowerShellReply> InvokeAsync(Phase7SqlOperationalHostOptions options, IDictionary parameters, CancellationToken token)
        {
            Calls++; LastEntry = options.ProvisioningEntry.Path; LastParameters = parameters;
            if (!parameters.Contains("Execute")) return Task.FromResult(new Phase7SqlPowerShellReply("Review", JsonDocument.Parse(File.ReadAllBytes(Path.Combine((string)parameters["BundleDirectory"]!, "bundle-manifest.json"))).RootElement.GetProperty("RunId").GetGuid(),
                BadReply == "WrongHash" ? new string('F', 64) : (string)parameters["ExpectedManifestSha256"]!, 9, 0, BadReply == "SqlReview", BadReply == "Sap", BadReply == "Closure", null));
            var path = (string)parameters["JournalPath"]!; File.WriteAllText(path, "offline fake journal");
            return Task.FromResult(new Phase7SqlPowerShellReply("Executed", null, null, 0, 9, true, false, false, path));
        }
        public Task StopAsync(CancellationToken token) { Stops++; return Task.CompletedTask; }
    }
    private sealed class Launcher : IPhase7SqlSuiteLauncher
    {
        public int Starts; public int Stops; public bool Delay; public Phase7SqlSuiteLaunch? Last;
        public Task<IPhase7SqlSuiteChild> StartAsync(Phase7SqlSuiteLaunch launch, CancellationToken token) { Starts++; Last = launch; return Task.FromResult<IPhase7SqlSuiteChild>(new Child(this)); }
        public Task StopAsync(CancellationToken token) { Stops++; return Task.CompletedTask; }
        private sealed class Child(Launcher owner) : IPhase7SqlSuiteChild
        {
            public Process? Original => null; public int Id => 12345; public int ExitCode => 0;
            public async Task WaitAsync(CancellationToken token) { if (owner.Delay) await Task.Delay(5000, token); }
        }
    }
}
