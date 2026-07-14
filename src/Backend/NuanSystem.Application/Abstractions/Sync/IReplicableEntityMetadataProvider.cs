using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IReplicableEntityMetadataProvider
{
    Task<ReplicableEntityMetadata> GetAsync(
        int companyId,
        string entityName,
        CancellationToken cancellationToken = default);
}
