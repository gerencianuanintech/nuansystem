using NuanSystem.Application.Features.Sync.EntityDefinitions.Dtos;

namespace NuanSystem.Application.Features.Sync.EntityDefinitions.Services;

public interface ISyncEntityCatalogService
{
    Task<IReadOnlyCollection<SyncEntityDefinitionLookupDto>> GetAsync(
        bool includeInactive,
        int? includeId = null,
        CancellationToken cancellationToken = default);
}
