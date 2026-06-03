using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface ISapSyncLockRepository
{
    Task<SapSyncLockDto?> TryAcquireAsync(int companyId, string entityCode, SapSyncDirection direction, string workerInstance, string correlationId, DateTime expiresAtUtc, CancellationToken cancellationToken = default);
    Task ReleaseAsync(long id, string workerInstance, string correlationId, CancellationToken cancellationToken = default);
}
