using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.TenantConfiguration.Dtos;

namespace NuanSystem.Application.Abstractions.Tenancy;

public interface ITenantIntegrationService
{
    Task<Result<IReadOnlyCollection<TenantIntegrationDto>>> GetActiveCompanyIntegrationsAsync(
        CancellationToken cancellationToken = default);
}

