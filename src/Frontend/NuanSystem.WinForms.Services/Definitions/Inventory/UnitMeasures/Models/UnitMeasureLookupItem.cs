namespace NuanSystem.WinForms.Services.Definitions.Inventory.UnitMeasures.Models;

public sealed class UnitMeasureLookupItem
{
    public int Id { get; set; }
    public Guid? GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Symbol { get; set; }
    public string MagnitudeCode { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public string DisplayText => $"{Code} - {Name}";
}
