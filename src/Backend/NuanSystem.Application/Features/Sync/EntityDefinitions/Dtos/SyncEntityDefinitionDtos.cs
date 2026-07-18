namespace NuanSystem.Application.Features.Sync.EntityDefinitions.Dtos;

public sealed record SyncEntityDefinitionListFilter(
    string? Search,
    bool? IsActive,
    int PageNumber = 1,
    int PageSize = 50);

public sealed record CreateSyncEntityDefinitionRequest
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DefaultExecutionOrder { get; init; }
    public bool SupportsIncremental { get; init; }
    public bool SupportsInsert { get; init; } = true;
    public bool SupportsUpdate { get; init; } = true;
    public bool SupportsDeactivate { get; init; } = true;
    public string? DefaultKeyField { get; init; }
    public string? DefaultModifiedAtField { get; init; }
    public bool IsActive { get; init; } = true;
    public IReadOnlyCollection<int> DependencyDefinitionIds { get; init; } = Array.Empty<int>();
}

public sealed record UpdateSyncEntityDefinitionRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DefaultExecutionOrder { get; init; }
    public bool SupportsIncremental { get; init; }
    public bool SupportsInsert { get; init; } = true;
    public bool SupportsUpdate { get; init; } = true;
    public bool SupportsDeactivate { get; init; } = true;
    public string? DefaultKeyField { get; init; }
    public string? DefaultModifiedAtField { get; init; }
    public bool IsActive { get; init; } = true;
    public IReadOnlyCollection<int> DependencyDefinitionIds { get; init; } = Array.Empty<int>();
}

public sealed record SyncEntityDefinitionListItemDto
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
}

public sealed record SyncEntityDefinitionDetailDto
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
    public IReadOnlyCollection<SyncEntityDefinitionDependencyDto> Dependencies { get; init; } = [];
}

public sealed record SyncEntityDefinitionDependencyDto(
    int Id,
    int DependencyDefinitionId,
    string DependencyCode,
    string DependencyName);

public sealed record SyncEntityDefinitionLookupDto(
    int Id,
    string Code,
    string Name,
    string? Description,
    int DefaultExecutionOrder,
    bool SupportsIncremental,
    bool SupportsInsert,
    bool SupportsUpdate,
    bool SupportsDeactivate,
    string? DefaultKeyField,
    string? DefaultModifiedAtField,
    bool IsSystem,
    bool IsActive,
    bool HasProducer,
    bool HasApplier,
    IReadOnlyCollection<string> Dependencies)
{
    public bool IsOperative => HasProducer && HasApplier;
}

public sealed record CreateSyncEntityDefinitionData(
    string Code,
    string Name,
    string? Description,
    int DefaultExecutionOrder,
    bool SupportsIncremental,
    bool SupportsInsert,
    bool SupportsUpdate,
    bool SupportsDeactivate,
    string? DefaultKeyField,
    string? DefaultModifiedAtField,
    bool IsActive,
    int? AuditUserId,
    string? AuditUserName,
    IReadOnlyCollection<int> DependencyDefinitionIds);

public sealed record UpdateSyncEntityDefinitionData(
    int Id,
    string Name,
    string? Description,
    int DefaultExecutionOrder,
    bool SupportsIncremental,
    bool SupportsInsert,
    bool SupportsUpdate,
    bool SupportsDeactivate,
    string? DefaultKeyField,
    string? DefaultModifiedAtField,
    bool IsActive,
    int? AuditUserId,
    string? AuditUserName,
    IReadOnlyCollection<int> DependencyDefinitionIds);

public sealed record SyncEntityDefinitionRecord
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
    public int? CreatedByUserId { get; init; }
    public string? CreatedByUserName { get; init; }
    public DateTime CreatedAt { get; init; }
    public int? UpdatedByUserId { get; init; }
    public string? UpdatedByUserName { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public sealed record SyncEntityDefinitionDependencyRecord
{
    public int Id { get; init; }
    public int EntityDefinitionId { get; init; }
    public int DependencyDefinitionId { get; init; }
    public string DependencyCode { get; init; } = string.Empty;
    public string DependencyName { get; init; } = string.Empty;
}

public sealed record SyncEntityDefinitionDetailRecord(
    SyncEntityDefinitionRecord Definition,
    IReadOnlyCollection<SyncEntityDefinitionDependencyRecord> Dependencies);

public enum SyncEntityDefinitionMutationError
{
    None,
    InvalidData,
    DuplicateCode,
    InvalidDependency,
    DependencyCycle,
    NotFound,
    SystemDefinition,
    ReferencedByProfile,
    RequiredByDefinition
}

public sealed record SyncEntityDefinitionMutationResult(
    bool Succeeded,
    int? Id,
    SyncEntityDefinitionMutationError Error)
{
    public static SyncEntityDefinitionMutationResult Success(int? id = null) => new(true, id, SyncEntityDefinitionMutationError.None);

    public static SyncEntityDefinitionMutationResult Failure(SyncEntityDefinitionMutationError error) => new(false, null, error);
}
