namespace NuanSystem.WinForms.Services.FinancialCatalogs.PriceLists;

public sealed class PriceListItem
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public string CurrencyName { get; set; } = string.Empty;
    public Guid CurrencyGlobalId { get; set; }
    public string AppliesTo { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
}

public sealed record SavePriceListRequest(
    string Code,
    string Name,
    string? Description,
    string CurrencyCode,
    string AppliesTo,
    bool IsDefault,
    bool IsActive);

public sealed record PriceListScopeOption(string Code, string Name);
