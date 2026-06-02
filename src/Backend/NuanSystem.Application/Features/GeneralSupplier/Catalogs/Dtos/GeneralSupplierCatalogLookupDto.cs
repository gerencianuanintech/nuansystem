namespace NuanSystem.Application.Features.GeneralSupplier.Catalogs.Dtos;

public sealed class GeneralSupplierCatalogLookupDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

