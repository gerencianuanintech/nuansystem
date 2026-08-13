namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemBrands.Models;

public sealed class ItemBrandItem
{
    public int Id { get; set; }
    public Guid? GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public string? ExternalSystem { get; set; }
    public string? ExternalCode { get; set; }
    public string? SapManufacturerCode { get; set; }
    public string? SapCode { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
