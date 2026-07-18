using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IPurchaseOrderSyncApplyRepository
{
    Task<PurchaseOrderSyncApplyResult> ApplyAsync(int branchCompanyId,SyncEventApplyContext context,PurchaseOrderSyncPayload payload,SyncOperation operation,CancellationToken cancellationToken=default);
}
