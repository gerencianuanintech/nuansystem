namespace NuanSystem.Application.Features.TenantConfiguration.Dtos;

public sealed record TenantFeatureDto(
    string FeatureCode,
    bool IsEnabled,
    DateTime? CreatedAt,
    DateTime? UpdatedAt);

