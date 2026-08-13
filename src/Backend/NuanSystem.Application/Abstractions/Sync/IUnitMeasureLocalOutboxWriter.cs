using System.Data;
using NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IUnitMeasureLocalOutboxWriter
{
    Task<Guid?> EnqueueAsync(UnitMeasureDto unitMeasure, SyncOperation operation,
        IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
}
