using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Configuration;
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
        var requiresExplicitTarget = RequiresExplicitTarget(syncEvent.EntityName);
        if (requiresExplicitTarget && syncEvent.TargetCompanyId is not > 0)
        {
            return Deferred("El evento direccional requiere un destino explicito.");
        }

        var routing = await routingService.ResolveTargetsAsync(
            new SyncRoutingContext(
                syncEvent.CompanyId,
                syncEvent.EntityName,
                EntityGlobalId: syncEvent.EntityGlobalId,
                PayloadJson: syncEvent.PayloadJson,
                TargetCompanyId: syncEvent.TargetCompanyId),
            cancellationToken);

        if (requiresExplicitTarget && (!routing.ShouldDistribute || routing.Targets.Count == 0))
        {
            return Deferred(routing.Reason ?? "No existe una ruta activa para el destino solicitado.");
        }

        if (requiresExplicitTarget
            && syncEvent.TargetCompanyId is int targetCompanyId
            && (routing.Targets.Count != 1 || routing.Targets.Single().BranchCompanyId != targetCompanyId))
        {
            return Deferred("La ruta resuelta no coincide de forma unica con el destino solicitado.");
        }

        return await repository.PromoteAsync(
            new SyncOutboxPromotionData(
                syncEvent,
                routing.Targets,
                routing.Decisions ?? [],
                workerInstance),
            cancellationToken);
    }

    private static bool RequiresExplicitTarget(string entityName) =>
        string.Equals(entityName, SyncMasterBranchEntityCodes.BusinessPartnerProposal, StringComparison.OrdinalIgnoreCase)
        || string.Equals(entityName, SyncMasterBranchEntityCodes.BusinessPartnerProposalResult, StringComparison.OrdinalIgnoreCase);

    private static SyncOutboxPromotionResult Deferred(string reason) =>
        new(SyncOutboxPromotionStatus.Deferred, null, reason);
}
