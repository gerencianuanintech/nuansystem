using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IPurchaseOrderRoutingRepository
{
    Task<PurchaseOrderRoutingCandidate?> GetCandidateAsync(int purchaseOrderId,CancellationToken cancellationToken=default);
    Task<IReadOnlyCollection<PurchaseOrderRouteTarget>> ResolveTargetsAsync(int sourceCompanyId,IReadOnlyCollection<string> warehouseCodes,CancellationToken cancellationToken=default);
    Task MarkDecisionAsync(int purchaseOrderId,string status,int? branchCompanyId,string reason,int? userId,string? userName,CancellationToken cancellationToken=default);
}
