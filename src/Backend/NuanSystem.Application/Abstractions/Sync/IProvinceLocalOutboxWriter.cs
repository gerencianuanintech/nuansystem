using System.Data;
using NuanSystem.Application.Features.Definitions.General.Provinces.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IProvinceLocalOutboxWriter
{
    Task<Guid?> EnqueueAsync(ProvinceDto province, SyncOperation operation, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
}
