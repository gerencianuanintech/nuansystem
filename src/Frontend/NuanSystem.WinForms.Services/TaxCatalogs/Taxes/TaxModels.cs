namespace NuanSystem.WinForms.Services.TaxCatalogs.Taxes;

public sealed class TaxItem
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Rate { get; set; }
    public bool IsActive { get; set; }
    public string? ExternalSystem { get; set; }
    public string? ExternalCode { get; set; }
}

public sealed record SaveTaxRequest(string Code, string Name, string? Description, decimal Rate, bool IsActive);
