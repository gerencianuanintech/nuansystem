namespace NuanSystem.WinForms.Services.GeneralInventory.ItemGroups.Models;

public sealed record SaveItemGroupRequest(
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    string? InventoryAccountCode,
    string? CostOfSalesAccountCode,
    string? SalesAccountCode,
    string? PurchaseAccountCode,
    string? SapGroupCode,
    string? SapCode);
