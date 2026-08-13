namespace NuanSystem.WinForms.Services.Definitions.Inventory.ProductTypes.Models;

public sealed class ProductTypeItem
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string NatureCode { get; set; } = string.Empty;
    public string NatureName => NatureCode switch
    {
        "Merchandise" => "Mercadería",
        "FinishedGood" => "Producto terminado",
        "RawMaterial" => "Materia prima",
        "SemiFinished" => "Semielaborado",
        "Supply" => "Insumo",
        "Packaging" => "Empaque",
        "ByProduct" => "Subproducto",
        "Other" => "Otro",
        _ => NatureCode
    };
    public int SortOrder { get; set; }
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
