using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface ISapSyncLockRepository
{
    Task<SapSyncLockDto?> TryAcquireAsync(
        int companyId,
        string entityCode,
        SapSyncDirection direction,
        string workerInstance,
        string correlationId,
        Guid? executionUid,
        string ownerToken,
        DateTime lockExpiresAtUtc,
        CancellationToken cancellationToken = default);

    Task<bool> RenewAsync(
        long id,
        string ownerToken,
        DateTime lockExpiresAtUtc,
        CancellationToken cancellationToken = default);

    Task ReleaseAsync(
        long id,
        string ownerToken,
        CancellationToken cancellationToken = default);

    Task<bool> ReleaseExpiredAsync(
        long id,
        string reason,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default);
}
