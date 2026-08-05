using System.Data;
using NuanSystem.Application.Features.Definitions.General.Cities.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ICityLocalOutboxWriter
{
    Task<Guid?> EnqueueAsync(
        CityDto city,
        SyncOperation operation,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);
}
