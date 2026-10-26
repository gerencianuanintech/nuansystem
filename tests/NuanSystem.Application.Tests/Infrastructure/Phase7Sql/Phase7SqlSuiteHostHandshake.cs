using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal sealed record Phase7SqlTestHostAnnouncement(Guid RunId, string Nonce, int ProcessId, DateTimeOffset ProcessStartedAtUtc, string AssemblySha256);

// Local parent/testhost identity handoff, before fixture connections. Optional for legacy manually hosted runs.
internal static class Phase7SqlSuiteHostHandshake
{
    private static Guid? announced;
    public static void Notify(Guid runId, int timeoutSeconds)
    {
        var pipeName = Environment.GetEnvironmentVariable("NUANSYSTEM_PHASE7_HOST_PIPE");
        var nonce = Environment.GetEnvironmentVariable("NUANSYSTEM_PHASE7_HOST_NONCE");
        if (string.IsNullOrEmpty(pipeName) && string.IsNullOrEmpty(nonce)) return;
        if (announced == runId) return;
        Phase7SqlOperationalHost.Require(pipeName is not null && pipeName.StartsWith("NuanPhase7_", StringComparison.Ordinal)
            && nonce is { Length: 64 } && nonce.All(Uri.IsHexDigit), "HANDSHAKE_CONFIG");
        using var current = Process.GetCurrentProcess();
        using var binary = new FileStream(typeof(Phase7SqlSuiteHostHandshake).Assembly.Location, FileMode.Open, FileAccess.Read, FileShare.Read);
        var announcement = new Phase7SqlTestHostAnnouncement(runId, nonce!, current.Id, new(current.StartTime.ToUniversalTime()),
            Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(binary)));
        using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(Math.Clamp(timeoutSeconds, 1, 120)));
        using var pipe = new NamedPipeClientStream(".", pipeName!, PipeDirection.InOut, PipeOptions.Asynchronous);
        pipe.ConnectAsync(deadline.Token).GetAwaiter().GetResult();
        using var writer = new StreamWriter(pipe, new UTF8Encoding(false), 4096, true) { AutoFlush = true };
        using var reader = new StreamReader(pipe, Encoding.UTF8, false, 4096, true);
        writer.WriteLine(Encoding.UTF8.GetString(Phase7SqlEvidenceFiles.Encode(announcement)));
        Phase7SqlOperationalHost.Require(reader.ReadLineAsync(deadline.Token).AsTask().GetAwaiter().GetResult() == "ACK", "HANDSHAKE_ACK");
        announced = runId;
    }

    public static void Validate(Phase7SqlTestHostAnnouncement value, Guid run, string nonce, string assemblyHash,
        int runnerId, int attachedId, DateTimeOffset attachedStarted, bool descendant)
    {
        Phase7SqlOperationalHost.Require(value.RunId == run && value.Nonce == nonce && value.AssemblySha256 == assemblyHash
            && value.ProcessId > 0 && value.ProcessId != runnerId && value.ProcessId == attachedId
            && value.ProcessStartedAtUtc.Offset == TimeSpan.Zero && value.ProcessStartedAtUtc == attachedStarted && descendant, "HANDSHAKE_IDENTITY");
    }

    public static async Task<Process> AcceptAsync(NamedPipeServerStream pipe, Guid run, string nonce, string assemblyHash, Process runner, CancellationToken token)
    {
        await pipe.WaitForConnectionAsync(token);
        var bytes = new List<byte>(); var one = new byte[1];
        while (bytes.Count < 4096)
        {
            if (await pipe.ReadAsync(one, token) == 0) throw Phase7SqlOperationalHost.Guard("HANDSHAKE_EOF");
            if (one[0] == (byte)'\n') break; bytes.Add(one[0]);
        }
        Phase7SqlOperationalHost.Require(bytes.Count < 4096, "HANDSHAKE_SIZE");
        var value = Phase7SqlEvidenceFiles.Decode<Phase7SqlTestHostAnnouncement>(bytes.ToArray());
        Phase7SqlOperationalHost.Require(value.RunId == run && value.Nonce == nonce && value.AssemblySha256 == assemblyHash
            && value.ProcessId > 0 && value.ProcessId != runner.Id, "HANDSHAKE_CHALLENGE");
        // Attach once, while the child is blocked awaiting ACK, never a later lookup to replace an exited process.
        var executor = Process.GetProcessById(value.ProcessId);
        try
        {
            Validate(value, run, nonce, assemblyHash, runner.Id, executor.Id, new(executor.StartTime.ToUniversalTime()), IsDescendant(executor, runner));
            Phase7SqlOperationalHost.Require(!executor.HasExited && executor.SessionId == runner.SessionId, "HANDSHAKE_SESSION");
            await pipe.WriteAsync("ACK\n"u8.ToArray(), token); await pipe.FlushAsync(token); return executor;
        }
        catch { executor.Dispose(); throw; }
    }
    private static bool IsDescendant(Process executor, Process runner)
    {
        if (!OperatingSystem.IsWindows()) return false;
        var next = executor.Id;
        for (var depth = 0; depth < 12; depth++)
        {
            using var process = Process.GetProcessById(next);
            next = ParentId(process);
            if (next == runner.Id) return true;
            if (next <= 0 || next == process.Id) return false;
        }
        return false;
    }
    internal static int ParentId(Process process)
    {
        Phase7SqlOperationalHost.Require(OperatingSystem.IsWindows(), "PROCESS_PARENT" );

        if (NtQueryInformationProcess(process.Handle, 0, out var result, Marshal.SizeOf<ProcessInfo>(), out _) != 0)
            throw Phase7SqlOperationalHost.Guard("PROCESS_PARENT");
        return checked((int)result.ParentPid);
    }
    [StructLayout(LayoutKind.Sequential)] private struct ProcessInfo { public IntPtr Reserved1, Peb, Reserved2A, Reserved2B, Pid, ParentPid; }
    [DllImport("ntdll.dll")] private static extern int NtQueryInformationProcess(IntPtr handle, int informationClass, out ProcessInfo information, int length, out int returned);
}
