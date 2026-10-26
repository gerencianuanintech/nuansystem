using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.Application.Features.SapSync.Profiles;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface ISapSyncProcessorRegistry
{
    ISapSyncImporter? FindImporter(string entityCode);
    ISapSyncExporter? FindExporter(string entityCode);
    ISapSyncScheduledExecutionProcessor? FindScheduledProcessor(string entityCode);
    bool SupportsScheduled(string entityCode, SapSyncDirection direction, string mode);
    SapSyncHandlerCapabilityDto Intersect(SapSyncHandlerCapabilityDto configured);
}
