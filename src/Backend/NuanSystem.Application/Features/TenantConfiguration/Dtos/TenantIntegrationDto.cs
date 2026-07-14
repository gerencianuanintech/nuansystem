namespace NuanSystem.Application.Features.TenantConfiguration.Dtos;

public sealed record TenantIntegrationDto(
    string IntegrationCode,
    bool IsEnabled,
    string? ConfigurationJson,
    DateTime? CreatedAt,
    DateTime? UpdatedAt);

