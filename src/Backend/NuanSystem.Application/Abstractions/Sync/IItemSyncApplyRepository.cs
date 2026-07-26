using NuanSystem.Application.Features.Items.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IItemSyncApplyRepository
{
    Task<bool> ExistsByGlobalIdAsync(
        int branchCompanyId,
        Guid globalId,
        CancellationToken cancellationToken = default);

    Task<ItemSyncDependencyCheckResult> CheckDependenciesAsync(
        int branchCompanyId,
        ItemSyncPayload payload,
        CancellationToken cancellationToken = default);

    Task<ItemSyncApplyResult> UpsertFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        ItemSyncPayload payload,
        SyncOperation operation,
        CancellationToken cancellationToken = default);

    Task<ItemSyncApplyResult> DisableFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        ItemSyncPayload payload,
        bool markDeleted,
        CancellationToken cancellationToken = default);
}

public sealed record ItemSyncApplyResult(
    bool Applied,
    bool AlreadyApplied,
    int? ItemId,
    string Message,
    bool TerminalConflict = false,
    string? ErrorCode = null);

public sealed record ItemSyncDependencyCheckResult(
    bool IsSatisfied,
    string? MissingDependencyCode = null,
    string? Message = null)
{
    public static ItemSyncDependencyCheckResult Satisfied { get; } = new(true);
}
