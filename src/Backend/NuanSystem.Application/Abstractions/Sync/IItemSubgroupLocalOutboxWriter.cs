using System.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ItemSubgroups.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IItemSubgroupLocalOutboxWriter
{
    Task<Guid?> EnqueueAsync(
        ItemSubgroupDto itemSubgroup,
        SyncOperation operation,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);
}
