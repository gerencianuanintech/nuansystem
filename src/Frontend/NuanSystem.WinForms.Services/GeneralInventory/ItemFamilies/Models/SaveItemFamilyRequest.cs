namespace NuanSystem.WinForms.Services.GeneralInventory.ItemFamilies.Models;

public sealed record SaveItemFamilyRequest(
    int ItemGroupId,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    string? SapFamilyCode,
    string? SapCode);
