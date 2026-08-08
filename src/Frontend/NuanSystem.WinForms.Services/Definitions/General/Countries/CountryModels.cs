namespace NuanSystem.WinForms.Services.Definitions.General.Countries;

public sealed class CountryItem
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Iso2 { get; set; }

    public string? Iso3 { get; set; }

    public string? PhonePrefix { get; set; }

    public bool IsActive { get; set; }
}

public sealed record CountryPage(
    IReadOnlyCollection<CountryItem> Items,
    int TotalCount,
    int PageNumber,
    int PageSize);

public sealed record SaveCountryRequest(
    string Code,
    string Name,
    string? Iso2,
    string? Iso3,
    string? PhonePrefix,
    bool IsActive);
