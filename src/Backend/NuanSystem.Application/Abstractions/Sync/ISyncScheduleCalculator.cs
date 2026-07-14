using NuanSystem.Application.Features.Sync.Execution.Dtos;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ISyncScheduleCalculator
{
    DateTimeOffset? CalculateNextExecution(
        SyncScheduleDefinition schedule,
        DateTimeOffset referenceTimeUtc);
}
