using NuanSystem.Application.Features.Sync.Execution.Dtos;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ISyncFullEntitySource
{
    string EntityCode { get; }

    Task<SyncSourcePage> ReadPageAsync(
        SyncSourceReadContext context,
        CancellationToken cancellationToken = default);
}
