using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Abstractions.Common;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;
using System.Security.Cryptography;

namespace NuanSystem.Application.Features.SapSync.Services;

public sealed class SapSyncLockService(
    ISapSyncLockRepository repository,
    ISystemClock clock) : ISapSyncLockService
{
    public Task<SapSyncLockDto?> TryAcquireAsync(int companyId, string entityCode, SapSyncDirection direction, string workerInstance, string correlationId, TimeSpan timeout, CancellationToken cancellationToken = default)
        => TryAcquireCoreAsync(
            companyId,
            entityCode,
            direction,
            workerInstance,
            correlationId,
            executionUid: null,
            timeout,
            cancellationToken);

    public Task<SapSyncLockDto?> TryAcquireForExecutionAsync(
        int companyId,
        string entityCode,
        SapSyncDirection direction,
        string workerInstance,
        string correlationId,
        Guid executionUid,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
        => TryAcquireCoreAsync(
            companyId,
            entityCode,
            direction,
            workerInstance,
            correlationId,
            executionUid,
            timeout,
            cancellationToken);

    private Task<SapSyncLockDto?> TryAcquireCoreAsync(
        int companyId,
        string entityCode,
        SapSyncDirection direction,
        string workerInstance,
        string correlationId,
        Guid? executionUid,
        TimeSpan timeout,
        CancellationToken cancellationToken)
        => repository.TryAcquireAsync(
            companyId,
            entityCode,
            direction,
            workerInstance,
            correlationId,
            executionUid,
            Convert.ToHexString(RandomNumberGenerator.GetBytes(32)),
            clock.UtcNow.UtcDateTime.Add(timeout),
            cancellationToken);

    public Task<bool> RenewAsync(
        SapSyncLockDto syncLock,
        TimeSpan timeout,
        CancellationToken cancellationToken = default) =>
        repository.RenewAsync(
            syncLock.Id,
            syncLock.OwnerToken,
            clock.UtcNow.UtcDateTime.Add(timeout),
            cancellationToken);

    public Task ReleaseAsync(SapSyncLockDto syncLock, CancellationToken cancellationToken = default)
        => repository.ReleaseAsync(syncLock.Id, syncLock.OwnerToken, cancellationToken);
}
