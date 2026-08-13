namespace NuanSystem.WinForms.Services.Definitions.Inventory.ProductTypes.Models;

public sealed record SaveProductTypeRequest(
    string Code,
    string Name,
    string? Description,
    string NatureCode,
    int SortOrder,
    bool IsActive);
