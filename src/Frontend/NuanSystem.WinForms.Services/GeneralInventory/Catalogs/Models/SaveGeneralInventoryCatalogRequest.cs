namespace NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;

public sealed record SaveGeneralInventoryCatalogRequest(
    string Code,
    string Name,
    string? Description,
    bool IsActive);
