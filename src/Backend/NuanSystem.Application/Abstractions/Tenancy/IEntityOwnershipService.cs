using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.TenantConfiguration.Dtos;

namespace NuanSystem.Application.Abstractions.Tenancy;

public interface IEntityOwnershipService
{
    Task<Result<IReadOnlyCollection<EntityOwnershipConfigurationDto>>> GetActiveCompanyOwnershipAsync(
        CancellationToken cancellationToken = default);

    Task<Result<EntityOwnershipConfigurationDto>> GetActiveCompanyOwnershipAsync(
        string entityName,
        CancellationToken cancellationToken = default);
}

