namespace NuanSystem.Application.Features.GeneralInventory.Catalogs.Dtos;

public sealed class GeneralInventoryCatalogLookupDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string DisplayText => string.IsNullOrWhiteSpace(Code) ? Name : $"{Code} - {Name}";
}
