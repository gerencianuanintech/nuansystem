using System.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ReplenishmentMethods.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IReplenishmentMethodLocalOutboxWriter
{
    Task<Guid?> EnqueueAsync(ReplenishmentMethodDto method, SyncOperation operation,
        IDbConnection connection, IDbTransaction transaction,
        CancellationToken cancellationToken = default);
}
