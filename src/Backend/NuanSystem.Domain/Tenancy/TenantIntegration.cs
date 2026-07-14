namespace NuanSystem.Domain.Tenancy;

public sealed class TenantIntegration
{
    public int Id { get; init; }
    public int CompanyId { get; init; }
    public string IntegrationCode { get; init; } = string.Empty;
    public bool IsEnabled { get; init; }
    public string? ConfigurationJson { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

