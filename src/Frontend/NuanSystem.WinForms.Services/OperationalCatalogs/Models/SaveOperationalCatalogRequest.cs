namespace NuanSystem.WinForms.Services.OperationalCatalogs.Models;

public sealed record SaveOperationalCatalogRequest(
    string Code,
    string Name,
    string? Description,
    string? ParentCatalogKey,
    string? ParentCode,
    int DisplayOrder,
    bool IsDefault,
    bool IsActive);
