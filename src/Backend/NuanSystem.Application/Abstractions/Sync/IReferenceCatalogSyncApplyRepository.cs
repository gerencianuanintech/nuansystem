using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IReferenceCatalogSyncApplyRepository
{
    Task<bool> ExistsByGlobalIdAsync(
        int branchCompanyId,
        string entityCode,
        Guid globalId,
        CancellationToken cancellationToken = default);

    Task<ReferenceCatalogSyncApplyResult> ApplyAsync(
        int branchCompanyId,
        string entityCode,
        SyncEventApplyContext context,
        ReferenceCatalogSyncPayload payload,
        SyncOperation operation,
        CancellationToken cancellationToken = default);
}
