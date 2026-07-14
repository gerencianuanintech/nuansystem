using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Application.Features.TenantConfiguration.Dtos;

public sealed record TenantConfigurationSummaryDto(
    int CompanyId,
    string CompanyCode,
    CompanyOperationMode OperationMode,
    SapIntegrationMode SapIntegrationMode,
    bool IsMaster,
    int? ParentCompanyId,
    string? BranchCode,
    bool SyncEnabled);

