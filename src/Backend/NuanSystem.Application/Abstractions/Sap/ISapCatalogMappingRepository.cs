using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.Sap;

public interface ISapCatalogMappingRepository
{
    Task<IReadOnlyCollection<SapCatalogMappingDto>> GetByCompanyIdAsync(int companyId, CancellationToken cancellationToken = default);
    Task ReplaceAsync(ReplaceSapCatalogMappingsData data, CancellationToken cancellationToken = default);
}
