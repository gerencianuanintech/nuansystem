namespace NuanSystem.Application.Features.Items.Dtos;

public sealed record ItemWarehouseDto(
    int Id,
    int ItemId,
    int WarehouseId,
    string? WarehouseCode,
    string? WarehouseName,
    decimal MinimumStock,
    decimal MaximumStock,
    decimal RequiredStock,
    decimal ReorderPoint,
    string? DefaultLocationCode,
    decimal WarehouseCost,
    bool IsDefaultWarehouse,
    bool IsLocked,
    bool IsActive);
