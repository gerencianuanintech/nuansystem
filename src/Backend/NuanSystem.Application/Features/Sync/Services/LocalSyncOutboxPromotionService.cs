using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Features.Sync.Services;

public sealed class LocalSyncOutboxPromotionService(
    ISyncRoutingService routingService,
    ISyncOutboxPromotionRepository repository) : ILocalSyncOutboxPromotionService
{
    public async Task<SyncOutboxPromotionResult> PromoteAsync(
        LocalSyncOutboxDto syncEvent,
        string workerInstance,
        CancellationToken cancellationToken = default)
    {
        var routing = await routingService.ResolveTargetsAsync(
            new SyncRoutingContext(
                syncEvent.CompanyId,
                syncEvent.EntityName,
                EntityGlobalId: syncEvent.EntityGlobalId,
                PayloadJson: syncEvent.PayloadJson),
            cancellationToken);

        return await repository.PromoteAsync(
            new SyncOutboxPromotionData(
                syncEvent,
                routing.Targets,
                routing.Decisions ?? [],
                workerInstance),
            cancellationToken);
    }
}
