using System.Reflection;

namespace NuanSystem.SriWorker.Services;

internal static class WorkerVersionResolver
{
    public static string Resolve(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        var informationalVersion = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;
        return Resolve(informationalVersion, assembly.GetName().Version);
    }

    internal static string Resolve(string? informationalVersion, Version? assemblyVersion)
    {
        if (!string.IsNullOrWhiteSpace(informationalVersion))
        {
            return informationalVersion.Trim();
        }

        return assemblyVersion?.ToString() ?? "unknown";
    }
}
