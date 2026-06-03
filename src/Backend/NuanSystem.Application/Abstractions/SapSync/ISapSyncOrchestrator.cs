using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface ISapSyncOrchestrator
{
    Task<SapSyncExecutionResult> ExecuteAsync(
        SapSyncCompanyDto company,
        SapSyncEntitySettingsDto settings,
        SapSyncDirection direction,
        string workerInstance,
        TimeSpan lockTimeout,
        CancellationToken cancellationToken = default);
}
