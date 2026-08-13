namespace NuanSystem.WinForms.Services.Definitions.Inventory.UnitMeasures.Models;

public sealed class UnitMeasureItem
{
    public int Id { get; set; }
    public Guid? GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Symbol { get; set; }
    public string MagnitudeCode { get; set; } = string.Empty;
    public string MagnitudeName => MagnitudeCode switch
    {
        "Quantity" => "Cantidad",
        "Packaging" => "Empaque",
        "Mass" => "Masa",
        "Volume" => "Volumen",
        "Length" => "Longitud",
        "Area" => "Área",
        "Time" => "Tiempo",
        _ => "Otro"
    };
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public string? ExternalSystem { get; set; }
    public string? ExternalCode { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    public string? DeletedByUserName { get; set; }
    public DateTime? DeletedAt { get; set; }
}
