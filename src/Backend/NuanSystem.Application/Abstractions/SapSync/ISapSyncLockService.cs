using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface ISapSyncLockService
{
    Task<SapSyncLockDto?> TryAcquireAsync(int companyId, string entityCode, SapSyncDirection direction, string workerInstance, string correlationId, TimeSpan timeout, CancellationToken cancellationToken = default);
    Task<SapSyncLockDto?> TryAcquireForExecutionAsync(int companyId, string entityCode, SapSyncDirection direction, string workerInstance, string correlationId, Guid executionUid, TimeSpan timeout, CancellationToken cancellationToken = default);
    Task<bool> RenewAsync(SapSyncLockDto syncLock, TimeSpan timeout, CancellationToken cancellationToken = default);
    Task ReleaseAsync(SapSyncLockDto syncLock, CancellationToken cancellationToken = default);
}
