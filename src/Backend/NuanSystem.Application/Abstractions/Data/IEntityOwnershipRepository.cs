using NuanSystem.Application.Features.TenantConfiguration.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IEntityOwnershipRepository : IRepository
{
    Task<IReadOnlyCollection<EntityOwnershipConfigurationDto>> GetByCompanyIdAsync(
        int companyId,
        CancellationToken cancellationToken = default);

    Task<EntityOwnershipConfigurationDto?> GetByCompanyIdAndEntityAsync(
        int companyId,
        string entityName,
        CancellationToken cancellationToken = default);
}

