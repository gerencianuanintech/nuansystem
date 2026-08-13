namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemFamilies.Models;

public sealed record SaveItemFamilyRequest(
    int ItemGroupId,
    string Code,
    string Name,
    string? Description,
    int SortOrder,
    bool IsActive,
    string? ExternalSystem,
    string? ExternalCode,
    string? SapFamilyCode,
    string? SapCode);
