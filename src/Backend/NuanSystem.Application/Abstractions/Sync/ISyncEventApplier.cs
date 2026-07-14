using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ISyncEventApplier
{
    Task<SyncEventApplyResult> ApplyAsync(SyncEventApplyContext context, CancellationToken cancellationToken = default);
}
