using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ILocalSyncOutboxPromotionService
{
    Task<SyncOutboxPromotionResult> PromoteAsync(
        LocalSyncOutboxDto syncEvent,
        string workerInstance,
        CancellationToken cancellationToken = default);
}
