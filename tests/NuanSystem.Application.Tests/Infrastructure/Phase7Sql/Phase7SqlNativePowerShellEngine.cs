using System.Collections;
using System.Reflection;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

// Late binding to an explicitly pinned local PowerShell installation; no SDK install or profile/process launch.
internal sealed class Phase7SqlNativePowerShellEngine : IPhase7SqlPowerShellEngine
{
    private const string Script = "param($entry,$parameters) $ErrorActionPreference='Stop'; & $entry @parameters";
    private readonly object sync = new();
    private readonly HashSet<Job> jobs = [];
    private bool stopped;
    private sealed class Job(object shell) { public object Shell { get; } = shell; public Task<Phase7SqlPowerShellReply> Task { get; set; } = null!; }

    public Task<Phase7SqlPowerShellReply> InvokeAsync(Phase7SqlOperationalHostOptions options, IDictionary parameters, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        var leases = new List<IDisposable>(); object? shell = null;
        try
        {
            leases.Add(Phase7SqlOperationalHost.LockFile(options.ProvisioningEntry)); leases.Add(Phase7SqlOperationalHost.LockFile(options.ProvisioningModule));
            leases.Add(Phase7SqlOperationalHost.LockFile(options.AutomationAssembly));
            var assembly = Assembly.LoadFrom(options.AutomationAssembly.Path);
            Phase7SqlOperationalHost.Require(Path.GetFullPath(assembly.Location).Equals(Path.GetFullPath(options.AutomationAssembly.Path), StringComparison.OrdinalIgnoreCase), "RUNTIME_LOCATION");
            var type = assembly.GetType("System.Management.Automation.PowerShell", true)!;
            shell = type.GetMethod("Create", Type.EmptyTypes)!.Invoke(null, null)!;
            type.GetMethod("AddScript", [typeof(string), typeof(bool)])!.Invoke(shell, [Script, true]);
            var args = new Hashtable(StringComparer.Ordinal); foreach (DictionaryEntry p in parameters) args.Add(p.Key, p.Value);
            args["CancellationToken"] = token;
            var add = type.GetMethod("AddParameter", [typeof(string), typeof(object)])!;
            add.Invoke(shell, ["entry", options.ProvisioningEntry.Path]); add.Invoke(shell, ["parameters", args]);
            var job = new Job(shell);
            lock (sync)
            {
                Phase7SqlOperationalHost.Require(!stopped, "POWERSHELL_STOPPED"); jobs.Add(job);
                job.Task = Task.Run(() =>
                {
                    using var cancellation = token.Register(() => { _ = Task.Run(() => RequestStop(job.Shell)); });
                    try
                    {
                        token.ThrowIfCancellationRequested();
                        var invoke = type.GetMethods().Single(m => m.Name == "Invoke" && !m.IsGenericMethod && m.GetParameters().Length == 0);
                        var output = ((IEnumerable)invoke.Invoke(job.Shell, null)!).Cast<object>().ToArray();
                        token.ThrowIfCancellationRequested();
                        Phase7SqlOperationalHost.Require(type.GetProperty("HadErrors")!.GetValue(job.Shell) is false && output.Length == 1, "POWERSHELL_REPLY");
                        var properties = output[0].GetType().GetProperty("Properties")!.GetValue(output[0])!;
                        object? Value(string name)
                        {
                            var property = properties.GetType().GetProperty("Item", [typeof(string)])!.GetValue(properties, [name]);
                            return property?.GetType().GetProperty("Value")!.GetValue(property);
                        }
                        bool Flag(string name) { var v = Value(name); Phase7SqlOperationalHost.Require(v is bool, "POWERSHELL_BOOLEAN"); return (bool)v!; }
                        int Count(string name) { var v = Value(name); if (v is null) return 0; Phase7SqlOperationalHost.Require(v is int or long, "POWERSHELL_COUNT"); return Convert.ToInt32(v); }
                        Guid? run = Value("RunId") is { } raw ? Guid.Parse(raw.ToString()!) : null;
                        return new Phase7SqlPowerShellReply((string)Value("Mode")!, run, (string?)Value("ManifestSha256"), Count("BatchCount"), Count("CompletedBatches"),
                            Flag("SqlExecuted"), Flag("SapExecuted"), Flag("GlobalClosureAccepted"), (string?)Value("JournalPath"));
                    }
                    catch (OperationCanceledException) { throw; }
                    catch (Exception) when (token.IsCancellationRequested) { throw new OperationCanceledException(token); }
                    catch (Phase7SqlGuardException) { throw; }
                    catch { throw Phase7SqlOperationalHost.Guard("POWERSHELL_INVOKE"); }
                    finally
                    {
                        try { ((IDisposable)job.Shell).Dispose(); }
                        finally { foreach (var lease in leases) lease.Dispose(); args.Clear(); lock (sync) jobs.Remove(job); }
                    }
                });
            }
            return job.Task; // Keep locks and SecureString references until the real invocation has actually ended.
        }
        catch (OperationCanceledException) { foreach (var lease in leases) lease.Dispose(); (shell as IDisposable)?.Dispose(); throw; }
        catch { foreach (var lease in leases) lease.Dispose(); (shell as IDisposable)?.Dispose(); throw Phase7SqlOperationalHost.Guard("POWERSHELL_SETUP"); }
    }

    private static async Task RequestStop(object shell)
    {
        try
        {
            var task = (Task)shell.GetType().GetMethod("StopAsync", [typeof(AsyncCallback), typeof(object)])!.Invoke(shell, [null, null])!;
            await task;
        }
        catch { /* Invocation completion and StopAsync ownership remain authoritative; do not log stream/provider errors. */ }
    }
    public async Task StopAsync(CancellationToken token)
    {
        Job[] current; lock (sync) { stopped = true; current = jobs.ToArray(); }
        var stopRequests = current.Select(j => Task.Run(() => RequestStop(j.Shell))).ToArray();
        static async Task Settled(Task task) { try { await task; } catch { } } // Invocation errors are reported to their caller, not reclassified as Stop failures.
        await Task.WhenAll(stopRequests.Concat(current.Select(j => Settled(j.Task)))).WaitAsync(token);
    }
}
