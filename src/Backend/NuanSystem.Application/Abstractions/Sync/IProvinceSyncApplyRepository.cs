using NuanSystem.Application.Features.Definitions.General.Provinces.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IProvinceSyncApplyRepository
{
    Task<ProvinceSyncApplyResult> UpsertFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        ProvinceSyncPayload payload,
        SyncOperation operation,
        CancellationToken cancellationToken = default);

    Task<ProvinceSyncApplyResult> DisableFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        ProvinceSyncPayload payload,
        bool markDeleted,
        CancellationToken cancellationToken = default);
}

public sealed record ProvinceSyncApplyResult(
    bool Applied,
    bool AlreadyApplied,
    bool TerminalConflict,
    int? ProvinceId,
    string Message,
    string? ErrorCode = null);
