using NuanSystem.Application.Features.SapSync.Executions;
using NuanSystem.Application.Features.SapSync.Profiles;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface ISapSyncExecutionRepository
{
    Task<SapSyncExecutionWriteResult> CreateAsync(
        SapSyncExecutionCreateData data,
        CancellationToken cancellationToken = default);

    Task<SapSyncPagedResult<SapSyncExecutionListItemDto>> SearchAsync(
        SapSyncExecutionFilter filter,
        CancellationToken cancellationToken = default);

    Task<SapSyncExecutionDto?> GetByExecutionUidAsync(
        Guid executionUid,
        CancellationToken cancellationToken = default);

    Task<SapSyncPagedResult<SapSyncExecutionDetailListItemDto>> SearchDetailsAsync(
        SapSyncExecutionDetailFilter filter,
        CancellationToken cancellationToken = default);

    Task<SapSyncExecutionWriteResult> UpsertDetailAsync(
        SapSyncExecutionDetailData detail,
        CancellationToken cancellationToken = default);

    Task<SapSyncExecutionWriteResult> TransitionAsync(
        SapSyncExecutionStateData state,
        CancellationToken cancellationToken = default);

    Task<SapSyncExecutionWriteResult> RequestCancellationAsync(
        Guid executionUid,
        int? requestedByUserId,
        string? requestedByUserName,
        byte[] expectedRowVersion,
        CancellationToken cancellationToken = default);

    Task<SapSyncExecutionDetailClaim?> TryClaimDueDetailAsync(
        string workerInstance,
        string ownerToken,
        DateTime lockExpiresAtUtc,
        IReadOnlyCollection<string> approvedSnapshotTypes,
        CancellationToken cancellationToken = default);

    Task<SapSyncExecutionRetryResult> CreateManualRetryAsync(
        SapSyncExecutionRetryRequest request,
        CancellationToken cancellationToken = default);

    Task<SapSyncExecutionWriteResult> CompleteClaimedDetailAsync(
        SapSyncExecutionDetailCompletion completion,
        CancellationToken cancellationToken = default);

    Task<int> RecoverExpiredDetailLocksAsync(
        DateTime utcNow,
        CancellationToken cancellationToken = default);

    Task<SapSyncExecutionWriteResult> ReleaseExpiredDetailLockAsync(
        long detailId,
        string reason,
        int? requestedByUserId,
        string? requestedByUserName,
        byte[] expectedRowVersion,
        CancellationToken cancellationToken = default);

    Task<bool> RenewDetailLockAsync(
        long detailId,
        string ownerToken,
        DateTime lockExpiresAtUtc,
        CancellationToken cancellationToken = default);

    Task<bool> ReleaseDetailLockAsync(
        long detailId,
        string ownerToken,
        CancellationToken cancellationToken = default);
}
