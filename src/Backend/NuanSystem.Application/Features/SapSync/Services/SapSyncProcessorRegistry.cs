using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.Application.Features.SapSync.Profiles;

namespace NuanSystem.Application.Features.SapSync.Services;

/// <summary>Executable support only. Persisted company capabilities can further restrict it.</summary>
public sealed class SapSyncProcessorRegistry : ISapSyncProcessorRegistry
{
    private readonly IReadOnlyDictionary<string, ISapSyncImporter> importers;
    private readonly IReadOnlyDictionary<string, ISapSyncExporter> exporters;
    private readonly IReadOnlyDictionary<string, ISapSyncScheduledExecutionProcessor> scheduled;

    public SapSyncProcessorRegistry(
        IEnumerable<ISapSyncImporter> importers,
        IEnumerable<ISapSyncExporter> exporters,
        IEnumerable<ISapSyncScheduledExecutionProcessor> scheduled,
        IEnumerable<ISapSyncExecutionRetryProcessor> retries)
    {
        this.importers = Unique(importers, item => item.EntityCode);
        this.exporters = Unique(exporters, item => item.EntityCode);
        this.scheduled = Unique(scheduled, item => item.EntityCode);
        foreach (var importer in this.importers.Values) ValidateModes(importer.SupportedModes);
        foreach (var exporter in this.exporters.Values) ValidateModes(exporter.SupportedModes);
        foreach (var processor in this.scheduled.Values)
        {
            ValidateModes(processor.SupportedModes);
            var importer = FindImporter(processor.EntityCode);
            if (importer is null || processor.SupportedModes.Any(mode => !Contains(importer.SupportedModes, mode)))
                throw new InvalidOperationException($"SAP_SYNC_INCOMPATIBLE_SCHEDULED_PROCESSOR: {processor.EntityCode}");
        }

        var retryByType = Unique(retries, item => item.ApprovedSnapshotType);
        _ = Unique(retryByType.Values, item => item.EntityCode);
        foreach (var processor in retryByType.Values)
            if (!this.scheduled.ContainsKey(processor.EntityCode))
                throw new InvalidOperationException($"SAP_SYNC_INCOMPATIBLE_RETRY_PROCESSOR: {processor.EntityCode}");
    }

    public ISapSyncImporter? FindImporter(string entityCode) => importers.GetValueOrDefault(entityCode);
    public ISapSyncExporter? FindExporter(string entityCode) => exporters.GetValueOrDefault(entityCode);
    public ISapSyncScheduledExecutionProcessor? FindScheduledProcessor(string entityCode) => scheduled.GetValueOrDefault(entityCode);

    // Current durable processors implement SAP-to-ERP only; Both is never an alias for Import.
    public bool SupportsScheduled(string entityCode, SapSyncDirection direction, string mode) =>
        direction == SapSyncDirection.SapToErp
        && FindImporter(entityCode) is { } importer && Contains(importer.SupportedModes, mode)
        && FindScheduledProcessor(entityCode) is { } processor && Contains(processor.SupportedModes, mode);

    public SapSyncHandlerCapabilityDto Intersect(SapSyncHandlerCapabilityDto configured)
    {
        var full = SupportsScheduled(configured.EntityCode, SapSyncDirection.SapToErp, SapSyncModes.Full);
        var incremental = SupportsScheduled(configured.EntityCode, SapSyncDirection.SapToErp, SapSyncModes.Incremental);
        var executable = configured.IsActive && configured.IsImplemented && configured.SupportsSapToErp;
        return configured with
        {
            SupportsSapToErp = executable && (full || incremental),
            SupportsErpToSap = false,
            SupportsFull = executable && configured.SupportsFull && full,
            SupportsIncremental = executable && configured.SupportsIncremental && incremental,
            IsImplemented = executable && (configured.SupportsFull && full || configured.SupportsIncremental && incremental)
        };
    }

    private static bool Contains(IReadOnlyCollection<string> modes, string mode) => modes.Contains(mode, StringComparer.OrdinalIgnoreCase);

    private static void ValidateModes(IReadOnlyCollection<string> modes)
    {
        if (modes.Count == 0 || modes.Any(mode => !string.Equals(mode, SapSyncModes.Full, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(mode, SapSyncModes.Incremental, StringComparison.OrdinalIgnoreCase))
            || modes.Distinct(StringComparer.OrdinalIgnoreCase).Count() != modes.Count)
            throw new InvalidOperationException("SAP_SYNC_INVALID_PROCESSOR_MODES");
    }

    private static IReadOnlyDictionary<string, T> Unique<T>(IEnumerable<T> items, Func<T, string> key)
    {
        var result = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in items)
        {
            var code = key(item);
            if (string.IsNullOrWhiteSpace(code) || code != code.Trim() || !result.TryAdd(code, item))
                throw new InvalidOperationException($"SAP_SYNC_INVALID_OR_DUPLICATE_PROCESSOR: {typeof(T).Name}");
        }
        return result;
    }
}
