namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemLines.Models;

public sealed record SaveItemLineRequest(
    string Code,
    string Name,
    string? Description,
    int SortOrder,
    bool IsActive);
