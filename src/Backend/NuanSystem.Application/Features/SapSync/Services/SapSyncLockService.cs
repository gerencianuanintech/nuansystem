using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Application.Features.SapSync.Services;

public sealed class SapSyncLockService(ISapSyncLockRepository repository) : ISapSyncLockService
{
    public Task<SapSyncLockDto?> TryAcquireAsync(int companyId, string entityCode, SapSyncDirection direction, string workerInstance, string correlationId, TimeSpan timeout, CancellationToken cancellationToken = default)
        => repository.TryAcquireAsync(companyId, entityCode, direction, workerInstance, correlationId, DateTime.UtcNow.Add(timeout), cancellationToken);

    public Task ReleaseAsync(SapSyncLockDto syncLock, CancellationToken cancellationToken = default)
        => repository.ReleaseAsync(syncLock.Id, syncLock.WorkerInstance, syncLock.CorrelationId, cancellationToken);
}
