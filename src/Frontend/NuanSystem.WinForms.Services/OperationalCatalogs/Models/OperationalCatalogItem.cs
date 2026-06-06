namespace NuanSystem.WinForms.Services.OperationalCatalogs.Models;

public sealed class OperationalCatalogItem
{
    public int Id { get; set; }
    public string CatalogKey { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ParentCatalogKey { get; set; }
    public string? ParentCode { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
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
