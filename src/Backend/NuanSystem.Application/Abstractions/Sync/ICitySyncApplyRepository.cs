using NuanSystem.Application.Features.Definitions.General.Cities.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ICitySyncApplyRepository
{
    Task<CitySyncApplyResult> UpsertFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        CitySyncPayload payload,
        SyncOperation operation,
        CancellationToken cancellationToken = default);

    Task<CitySyncApplyResult> DisableFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        CitySyncPayload payload,
        bool markDeleted,
        CancellationToken cancellationToken = default);
}

public sealed record CitySyncApplyResult(
    bool Applied,
    bool AlreadyApplied,
    bool TerminalConflict,
    int? CityId,
    string Message,
    string? ErrorCode = null);
