using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed record SapSyncCompanyDto(
    int CompanyId,
    string CompanyCode,
    string CompanyName,
    SapIntegrationMode IntegrationMode,
    bool IsSapEnabled);
