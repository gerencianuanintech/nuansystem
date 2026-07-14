namespace NuanSystem.Domain.Tenancy;

public sealed class TenantFeature
{
    public int Id { get; init; }
    public int CompanyId { get; init; }
    public string FeatureCode { get; init; } = string.Empty;
    public bool IsEnabled { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

