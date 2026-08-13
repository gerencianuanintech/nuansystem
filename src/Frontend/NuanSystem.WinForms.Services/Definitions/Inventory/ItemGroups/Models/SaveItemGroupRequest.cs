namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemGroups.Models;

public sealed record SaveItemGroupRequest(
    string Code,
    string Name,
    string? Description,
    string? InventoryAccountCode,
    string? IncomeAccountCode,
    string? CostOfSalesAccountCode,
    string? SalesReturnAccountCode,
    string? PurchaseReturnAccountCode,
    string? CostVarianceAccountCode,
    string? InventoryAdjustmentAccountCode,
    string? PurchaseExpenseAccountCode,
    int SortOrder,
    bool IsActive,
    string? ExternalSystem,
    string? ExternalCode,
    string? SapGroupCode,
    string? SapCode);
