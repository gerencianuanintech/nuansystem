namespace NuanSystem.WinForms.Services.Sync.EntityDefinitions.Models;

public sealed record SyncEntityDefinitionListItem
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DefaultExecutionOrder { get; init; }
    public bool SupportsIncremental { get; init; }
    public bool SupportsInsert { get; init; }
    public bool SupportsUpdate { get; init; }
    public bool SupportsDeactivate { get; init; }
    public string? DefaultKeyField { get; init; }
    public string? DefaultModifiedAtField { get; init; }
    public bool IsSystem { get; init; }
    public bool IsActive { get; init; }
    public int DependencyCount { get; init; }
    public bool IsInUse { get; init; }
    public bool HasProducer { get; init; }
    public bool HasApplier { get; init; }
    public bool IsOperative => HasProducer && HasApplier;
    public int? CreatedByUserId { get; init; }
    public string? CreatedByUserName { get; init; }
    public DateTime CreatedAt { get; init; }
    public int? UpdatedByUserId { get; init; }
    public string? UpdatedByUserName { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string StatusText => IsActive ? "Activo" : "Inactivo";
}

public sealed record SyncEntityDefinitionDetail
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DefaultExecutionOrder { get; init; }
    public bool SupportsIncremental { get; init; }
    public bool SupportsInsert { get; init; }
    public bool SupportsUpdate { get; init; }
    public bool SupportsDeactivate { get; init; }
    public string? DefaultKeyField { get; init; }
    public string? DefaultModifiedAtField { get; init; }
    public bool IsSystem { get; init; }
    public bool IsActive { get; init; }
    public bool HasProducer { get; init; }
    public bool HasApplier { get; init; }
    public bool IsOperative => HasProducer && HasApplier;
    public int? CreatedByUserId { get; init; }
    public string? CreatedByUserName { get; init; }
    public DateTime CreatedAt { get; init; }
    public int? UpdatedByUserId { get; init; }
    public string? UpdatedByUserName { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public IReadOnlyCollection<SyncEntityDefinitionDependency> Dependencies { get; init; } = Array.Empty<SyncEntityDefinitionDependency>();
}

public sealed record SyncEntityDefinitionDependency(
    int Id,
    int DependencyDefinitionId,
    string DependencyCode,
    string DependencyName);

public sealed record SyncEntityDefinitionLookupItem
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DefaultExecutionOrder { get; init; }
    public bool SupportsIncremental { get; init; }
    public bool SupportsInsert { get; init; }
    public bool SupportsUpdate { get; init; }
    public bool SupportsDeactivate { get; init; }
    public string? DefaultKeyField { get; init; }
    public string? DefaultModifiedAtField { get; init; }
    public bool IsSystem { get; init; }
    public bool IsActive { get; init; }
    public bool HasProducer { get; init; }
    public bool HasApplier { get; init; }
    public IReadOnlyCollection<string> Dependencies { get; init; } = Array.Empty<string>();
    public bool IsOperative => HasProducer && HasApplier;
    public string DisplayName => $"{Code} - {Name}";
}

public sealed record CreateSyncEntityDefinitionRequest
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DefaultExecutionOrder { get; init; }
    public bool SupportsIncremental { get; init; }
    public bool SupportsInsert { get; init; }
    public bool SupportsUpdate { get; init; }
    public bool SupportsDeactivate { get; init; }
    public string? DefaultKeyField { get; init; }
    public string? DefaultModifiedAtField { get; init; }
    public bool IsActive { get; init; }
    public IReadOnlyCollection<int> DependencyDefinitionIds { get; init; } = Array.Empty<int>();
}

public sealed record UpdateSyncEntityDefinitionRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DefaultExecutionOrder { get; init; }
    public bool SupportsIncremental { get; init; }
    public bool SupportsInsert { get; init; }
    public bool SupportsUpdate { get; init; }
    public bool SupportsDeactivate { get; init; }
    public string? DefaultKeyField { get; init; }
    public string? DefaultModifiedAtField { get; init; }
    public bool IsActive { get; init; }
    public IReadOnlyCollection<int> DependencyDefinitionIds { get; init; } = Array.Empty<int>();
}
