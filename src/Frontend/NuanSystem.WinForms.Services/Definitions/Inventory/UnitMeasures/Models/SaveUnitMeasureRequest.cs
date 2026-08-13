namespace NuanSystem.WinForms.Services.Definitions.Inventory.UnitMeasures.Models;

public sealed record SaveUnitMeasureRequest(
    string Code,
    string Name,
    string? Description,
    string? Symbol,
    string MagnitudeCode,
    int SortOrder,
    bool IsActive,
    string? ExternalSystem,
    string? ExternalCode);
