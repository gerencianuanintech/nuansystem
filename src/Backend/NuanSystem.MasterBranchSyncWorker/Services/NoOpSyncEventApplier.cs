using Microsoft.Extensions.Options;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Options;

namespace NuanSystem.MasterBranchSyncWorker.Services;

public sealed class NoOpSyncEventApplier(IOptions<MasterBranchSyncWorkerOptions> options) : ISyncEventApplier
{
    public Task<SyncEventApplyResult> ApplyAsync(
        SyncEventApplyContext context,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var currentOptions = options.Value;
        return Task.FromResult(currentOptions.SkeletonMode
            ? new SyncEventApplyResult(
                Applied: false,
                Message: $"SkeletonMode activo: evento {context.EventId} inspeccionado sin modificar entidades.")
            : new SyncEventApplyResult(
                Applied: true,
                Message: $"NoOp aplicado tecnicamente para evento {context.EventId}; no se modificaron entidades."));
    }
}
