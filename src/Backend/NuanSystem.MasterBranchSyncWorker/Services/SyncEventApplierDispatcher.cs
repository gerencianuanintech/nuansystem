using Microsoft.Extensions.Options;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Options;

namespace NuanSystem.MasterBranchSyncWorker.Services;

public sealed class SyncEventApplierDispatcher(
    IOptionsMonitor<MasterBranchSyncWorkerOptions> options,
    IEnumerable<ISyncEntityEventApplier> entityAppliers) : ISyncEventApplier
{
    public async Task<SyncEventApplyResult> ApplyAsync(
        SyncEventApplyContext context,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var currentOptions = options.CurrentValue;
        if (currentOptions.SkeletonMode)
        {
            return new SyncEventApplyResult(
                Applied: false,
                Message: $"SkeletonMode activo: evento {context.EventId} inspeccionado sin modificar entidades.");
        }

        if (!currentOptions.IsEntityApplierEnabled(context.EntityName))
        {
            return new SyncEventApplyResult(
                Applied: false,
                Message: $"Aplicador no habilitado para entidad {context.EntityName}.",
                ErrorCode: "SYNC_ENTITY_APPLIER_DISABLED");
        }

        var applier = entityAppliers.FirstOrDefault(candidate => candidate.CanApply(context.EntityName));
        if (applier is null)
        {
            return new SyncEventApplyResult(
                Applied: false,
                Message: $"No existe aplicador registrado para entidad {context.EntityName}.",
                ErrorCode: "SYNC_ENTITY_APPLIER_NOT_FOUND");
        }

        return await applier.ApplyAsync(context, cancellationToken);
    }
}
