namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemBrands.Models;

public sealed record SaveItemBrandRequest(
    string Code,
    string Name,
    string? Description,
    int SortOrder,
    bool IsActive,
    string? ExternalSystem,
    string? ExternalCode,
    string? SapManufacturerCode,
    string? SapCode);
