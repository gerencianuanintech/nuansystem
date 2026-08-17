namespace NuanSystem.Application.Features.Definitions.Inventory.StorageConditions.Dtos;

public sealed record StorageConditionSyncPayload(
    Guid GlobalId,
    string Code,
    string Name,
    string? Description,
    int SortOrder,
    bool IsActive,
    bool IsDeleted,
    DateTime UpdatedAt);
