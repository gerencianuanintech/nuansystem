using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Dtos;

namespace NuanSystem.Application.Features.Sync.EntityDefinitions.Services;

public sealed class SyncEntityCatalogService(ISyncEntityDefinitionRepository repository) : ISyncEntityCatalogService
{
    public async Task<IReadOnlyCollection<SyncEntityDefinitionLookupDto>> GetAsync(
        bool includeInactive,
        int? includeId = null,
        CancellationToken cancellationToken = default)
    {
        var definitions = await repository.GetLookupAsync(includeId, includeInactive, cancellationToken);
        return definitions.Select(SyncEntityDefinitionMapper.ToLookupDto).ToArray();
    }
}
