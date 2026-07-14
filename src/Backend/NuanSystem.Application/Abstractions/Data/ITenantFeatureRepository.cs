using NuanSystem.Application.Features.TenantConfiguration.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ITenantFeatureRepository : IRepository
{
    Task<IReadOnlyCollection<TenantFeatureDto>> GetByCompanyIdAsync(
        int companyId,
        CancellationToken cancellationToken = default);
}

