namespace NuanSystem.Application.Features.Definitions.Inventory.ItemOrigins.Dtos;

public sealed record ItemOriginSyncPayload(
    Guid GlobalId,
    string Code,
    string Name,
    string? Description,
    int SortOrder,
    bool IsActive,
    bool IsDeleted,
    DateTime UpdatedAt);
