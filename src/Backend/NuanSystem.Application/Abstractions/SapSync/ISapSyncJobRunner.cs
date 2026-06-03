using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface ISapSyncJobRunner
{
    Task<SapSyncExecutionResult> RunOutboxAsync(SapSyncOutboxItemDto item, string workerInstance, CancellationToken cancellationToken = default);
    Task<SapSyncExecutionResult> RunInboxAsync(SapSyncInboxItemDto item, string workerInstance, CancellationToken cancellationToken = default);
}
