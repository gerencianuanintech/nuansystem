using NuanSystem.Application.Features.TenantConfiguration.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ITenantIntegrationRepository : IRepository
{
    Task<IReadOnlyCollection<TenantIntegrationDto>> GetByCompanyIdAsync(
        int companyId,
        CancellationToken cancellationToken = default);
}

