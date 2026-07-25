using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ISyncOutboxPromotionRepository
{
    Task<SyncOutboxPromotionResult> PromoteAsync(
        SyncOutboxPromotionData data,
        CancellationToken cancellationToken = default);
}
