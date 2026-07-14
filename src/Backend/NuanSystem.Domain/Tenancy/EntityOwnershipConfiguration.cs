namespace NuanSystem.Domain.Tenancy;

public sealed class EntityOwnershipConfiguration
{
    public int Id { get; init; }
    public int CompanyId { get; init; }
    public string EntityName { get; init; } = string.Empty;
    public EntitySourceOfTruth SourceOfTruth { get; init; }
    public EntitySyncDirection SyncDirection { get; init; }
    public bool IsEnabled { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

