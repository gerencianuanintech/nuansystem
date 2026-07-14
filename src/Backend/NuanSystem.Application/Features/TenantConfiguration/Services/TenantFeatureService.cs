using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.TenantConfiguration.Dtos;

namespace NuanSystem.Application.Features.TenantConfiguration.Services;

public sealed class TenantFeatureService(
    ITenantFeatureRepository repository,
    ICompanyContext companyContext) : ITenantFeatureService
{
    public async Task<Result<IReadOnlyCollection<TenantFeatureDto>>> GetActiveCompanyFeaturesAsync(
        CancellationToken cancellationToken = default)
    {
        if (companyContext.CurrentCompany is null)
        {
            return Result<IReadOnlyCollection<TenantFeatureDto>>.Failure("Debe seleccionar una empresa.");
        }

        var configuredFeatures = await repository.GetByCompanyIdAsync(
            companyContext.CurrentCompany.CompanyId,
            cancellationToken);
        var configuredByCode = configuredFeatures.ToDictionary(x => x.FeatureCode, StringComparer.OrdinalIgnoreCase);

        var features = TenantFeatureCodes.All
            .Select(code => configuredByCode.TryGetValue(code, out var feature)
                ? feature
                : new TenantFeatureDto(code, false, null, null))
            .ToArray();

        return Result<IReadOnlyCollection<TenantFeatureDto>>.Success(features);
    }
}

