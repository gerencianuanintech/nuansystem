namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemTypes.Models;

public sealed record SaveItemTypeRequest(
    string Code,
    string Name,
    string? Description,
    string BehaviorCode,
    bool DefaultIsPurchaseItem,
    bool DefaultIsSalesItem,
    bool DefaultIsInventoryItem,
    int SortOrder,
    bool IsActive);
