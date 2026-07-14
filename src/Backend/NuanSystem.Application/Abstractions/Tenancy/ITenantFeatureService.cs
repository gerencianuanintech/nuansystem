using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.TenantConfiguration.Dtos;

namespace NuanSystem.Application.Abstractions.Tenancy;

public interface ITenantFeatureService
{
    Task<Result<IReadOnlyCollection<TenantFeatureDto>>> GetActiveCompanyFeaturesAsync(
        CancellationToken cancellationToken = default);
}

