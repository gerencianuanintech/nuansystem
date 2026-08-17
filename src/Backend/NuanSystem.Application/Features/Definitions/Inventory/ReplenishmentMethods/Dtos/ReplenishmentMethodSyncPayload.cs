namespace NuanSystem.Application.Features.Definitions.Inventory.ReplenishmentMethods.Dtos;

public sealed record ReplenishmentMethodSyncPayload(
    Guid GlobalId,
    string Code,
    string Name,
    string? Description,
    int SortOrder,
    bool IsActive,
    bool IsDeleted,
    DateTime UpdatedAt);
