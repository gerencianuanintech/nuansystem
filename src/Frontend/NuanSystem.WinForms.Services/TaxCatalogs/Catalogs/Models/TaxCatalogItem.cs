namespace NuanSystem.WinForms.Services.TaxCatalogs.Catalogs.Models;

public class TaxCatalogItem
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }
}

public sealed class RetentionConceptItem : TaxCatalogItem
{
    public int? RetentionTypeId { get; set; }

    public string? RetentionTypeName { get; set; }

    public string? SriCode { get; set; }

    public decimal Percent { get; set; }

    public bool AppliesIva { get; set; }

    public bool AppliesIncome { get; set; }
}
