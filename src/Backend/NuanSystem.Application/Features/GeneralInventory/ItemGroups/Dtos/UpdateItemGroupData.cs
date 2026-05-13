namespace NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;

public sealed record UpdateItemGroupData(
    int Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    string? InventoryAccountCode,
    string? CostOfSalesAccountCode,
    string? SalesAccountCode,
    string? PurchaseAccountCode,
    string? SapGroupCode,
    string? SapCode,
    int? UpdatedByUserId,
    string? UpdatedByUserName);
