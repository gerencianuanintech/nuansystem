using System.Diagnostics;
using System.IO.Pipes;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal static class Phase7SqlSuiteLaunchBuilder
{
    public static Phase7SqlSuiteLaunch Create(Phase7SqlOperationalHostOptions options, Phase7SqlValidationContext c,
        Phase7SqlManifest manifest, IReadOnlyDictionary<string, string> connections, IReadOnlyList<Phase7SqlHostFile> inputs)
    {
        var expected = new[] { "A", "B" }.SelectMany(role => new[] { $"NUANSYSTEM_PHASE7_TENANT_{role}_CONNECTION_STRING", $"NUANSYSTEM_PHASE7_OBSERVER_{role}_CONNECTION_STRING" }).ToArray();
        Phase7SqlOperationalHost.Require(connections.Count == 4 && expected.All(connections.ContainsKey), "SUITE_SECRETS");
        foreach (var target in new[] { manifest.TenantA, manifest.TenantB }) foreach (var observer in new[] { false, true })
            Phase7SqlFixture.ValidateConnectionString(connections[$"NUANSYSTEM_PHASE7_{(observer ? "OBSERVER" : "TENANT")}_{target.Role}_CONNECTION_STRING"], manifest, target, observer);
        Directory.CreateDirectory(c.SuiteWorkingDirectory);
        var info = new ProcessStartInfo(options.Dotnet.Path) { WorkingDirectory = c.SuiteWorkingDirectory, UseShellExecute = false,
            CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
        foreach (var a in new[] { "vstest", options.TestAssembly.Path, "--TestCaseFilter:" + Phase7SqlExecutionCatalog.RestrictedFilter,
            "--logger:trx;LogFileName=sql.trx", "--ResultsDirectory:" + c.SuiteRoot }) info.ArgumentList.Add(a);
        info.Environment.Clear();
        foreach (var key in new[] { "SystemRoot", "WINDIR", "TEMP", "TMP", "ProgramFiles", "ProgramFiles(x86)", "PROCESSOR_ARCHITECTURE" })
            if (Environment.GetEnvironmentVariable(key) is { } v) info.Environment[key] = v;
        var dotnetRoot = Path.GetDirectoryName(options.Dotnet.Path)!;
        info.Environment["DOTNET_ROOT"] = dotnetRoot; info.Environment["DOTNET_MULTILEVEL_LOOKUP"] = "0";
        info.Environment["DOTNET_NOLOGO"] = "1"; info.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
        info.Environment["DOTNET_SKIP_FIRST_TIME_EXPERIENCE"] = "1"; info.Environment["DOTNET_CLI_WORKLOAD_UPDATE_NOTIFY_DISABLE"] = "1";
        info.Environment["DOTNET_CLI_HOME"] = Path.Combine(c.Plan.OutputDirectory, "dotnet-home");
        info.Environment[SqlServerIntegrationFactAttribute.EnabledEnvironmentVariable] = "1";
        info.Environment[Phase7SqlGates.IsolatedEnvironmentVariable] = "1"; info.Environment["NUAN_RUN_SAP_PO_PILOT"] = "0";
        info.Environment["NUANSYSTEM_PHASE7_RUN_ROOT"] = c.SuiteRoot;
        foreach (var (key, file) in new[] { ("MANIFEST", "manifest.json"), ("APPROVAL", "approval.json"), ("SUITE_INPUTS", "suite-inputs.json"), ("CANDIDATE", "candidate.json"), ("OFFLINE_BASELINE", "offline-baseline.trx") })
            info.Environment["NUANSYSTEM_PHASE7_" + key + "_PATH"] = Path.Combine(c.PreparedDirectory, file);
        foreach (var pair in connections) info.Environment[pair.Key] = pair.Value;
        return new(info, options, inputs);
    }
}

internal sealed class Phase7SqlNativeSuiteLauncher : IPhase7SqlSuiteLauncher
{
    private readonly object sync = new(); private readonly List<Child> owned = []; private bool stopped;
    public Task<IPhase7SqlSuiteChild> StartAsync(Phase7SqlSuiteLaunch launch, CancellationToken token)
    {
        token.ThrowIfCancellationRequested(); var leases = new List<IDisposable>(); Process? process = null;
        try
        {
            var info = launch.StartInfo;
            Phase7SqlOperationalHost.Require(Phase7SqlOperationalHost.SamePath(info.FileName, launch.Options.Dotnet.Path)
                && !info.UseShellExecute && info.CreateNoWindow && info.RedirectStandardError && info.RedirectStandardOutput
                && info.ArgumentList.Count == 5 && info.ArgumentList[0] == "vstest"
                && info.ArgumentList[1] == launch.Options.TestAssembly.Path
                && info.ArgumentList[2] == "--TestCaseFilter:" + Phase7SqlExecutionCatalog.RestrictedFilter
                && info.ArgumentList[3] == "--logger:trx;LogFileName=sql.trx"
                && info.ArgumentList[4] == "--ResultsDirectory:" + info.Environment["NUANSYSTEM_PHASE7_RUN_ROOT"], "CHILD_COMMAND");
            foreach (var file in new[] { launch.Options.Dotnet, launch.Options.TestAssembly, launch.Options.TestDependencies, launch.Options.TestRuntimeConfiguration }.Concat(launch.Inputs))
                leases.Add(Phase7SqlOperationalHost.LockFile(file));
            lock (sync)
            {
                token.ThrowIfCancellationRequested(); Phase7SqlOperationalHost.Require(!stopped, "LAUNCHER_STOPPED");
                var pipeName = "NuanPhase7_" + Guid.NewGuid().ToString("N");
                var nonce = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
                var run = Guid.Parse(Path.GetFileName(launch.StartInfo.Environment["NUANSYSTEM_PHASE7_RUN_ROOT"]!));
                var pipe = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);
                launch.StartInfo.Environment["NUANSYSTEM_PHASE7_HOST_PIPE"] = pipeName; launch.StartInfo.Environment["NUANSYSTEM_PHASE7_HOST_NONCE"] = nonce;
                process = new Process { StartInfo = launch.StartInfo };
                Phase7SqlOperationalHost.Require(process.Start(), "CHILD_START");
                process.StartInfo.Environment.Clear(); // The OS has copied the environment; no parent-side retention in StartInfo.
                var child = new Child(process, leases, pipe, run, nonce, launch.Options.TestAssembly.Sha256, token); owned.Add(child); return Task.FromResult<IPhase7SqlSuiteChild>(child);
            }
        }
        catch (OperationCanceledException) { foreach (var lease in leases) lease.Dispose(); process?.Dispose(); throw; }
        catch { foreach (var lease in leases) lease.Dispose(); process?.Dispose(); throw Phase7SqlOperationalHost.Guard("CHILD_START"); }
    }
    private sealed class Child : IPhase7SqlSuiteChild
    {
        private readonly Process process;
        private readonly IReadOnlyList<IDisposable> leases;
        private readonly NamedPipeServerStream pipe;
        private readonly Task stdout, stderr;
        private readonly Task<Process> identified;
        private readonly CancellationTokenSource lifetime;
        private Process? executor;
        private int released;
        public Child(Process process, IReadOnlyList<IDisposable> leases, NamedPipeServerStream pipe, Guid run, string nonce, string hash, CancellationToken token)
        {
            this.process = process; this.leases = leases; this.pipe = pipe; lifetime = CancellationTokenSource.CreateLinkedTokenSource(token);
            stdout = Drain(process.StandardOutput); stderr = Drain(process.StandardError);
            identified = Phase7SqlSuiteHostHandshake.AcceptAsync(pipe, run, nonce, hash, process, lifetime.Token);
        }
        public Process Original => executor ?? throw Phase7SqlOperationalHost.Guard("NO_TESTHOST");
        public int Id => Original.Id;
        public int ExitCode => Original.ExitCode;
        public int RunnerExitCode => process.ExitCode;
        public async Task WaitAsync(CancellationToken token)
        {
            var exited = process.WaitForExitAsync(token);
            if (await Task.WhenAny(identified, exited) == exited && !identified.IsCompleted)
            { lifetime.Cancel(); throw Phase7SqlOperationalHost.Guard("NO_TESTHOST"); }
            executor = await identified.WaitAsync(token);
            await exited; await executor.WaitForExitAsync(token); await Task.WhenAll(stdout, stderr).WaitAsync(token); Release();
        }
        public async Task StopAsync(CancellationToken token)
        {
            try { lifetime.Cancel(); } catch (ObjectDisposedException) { }
            if (!process.HasExited) try { process.Kill(entireProcessTree: true); } catch (InvalidOperationException) when (process.HasExited) { }
            await process.WaitForExitAsync(token); await Task.WhenAll(stdout, stderr).WaitAsync(token); Release();
        }
        private void Release() { if (Interlocked.Exchange(ref released, 1) == 0) { foreach (var lease in leases) lease.Dispose(); pipe.Dispose(); lifetime.Dispose(); } }
        private static async Task Drain(StreamReader reader)
        { var buffer = new char[4096]; try { while (await reader.ReadAsync(buffer) != 0) { } } finally { Array.Clear(buffer); } }
    }
    public async Task StopAsync(CancellationToken token)
    {
        Child[] children; lock (sync) { stopped = true; children = owned.ToArray(); }
        await Task.WhenAll(children.Select(c => Task.Run(() => c.StopAsync(token)))).WaitAsync(token);
    }
}
