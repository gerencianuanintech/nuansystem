using NuanSystem.Application.Features.Carriers.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ICarrierSyncApplyRepository
{
    Task<CarrierSyncApplyResult> ApplyAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        CarrierSyncPayloadV1 payload,
        SyncOperation operation,
        CancellationToken cancellationToken = default);
}

public sealed record CarrierSyncApplyResult(
    bool Applied,
    bool AlreadyApplied,
    bool TerminalConflict,
    int? CarrierId,
    string Message,
    string? ErrorCode = null);
