namespace NuanSystem.WinForms.Services.Definitions.General.Cities;

public sealed class CityItem
{
    public int Id { get; set; }

    public int CountryId { get; set; }

    public string CountryCode { get; set; } = string.Empty;

    public string CountryName { get; set; } = string.Empty;

    public int ProvinceId { get; set; }

    public string ProvinceCode { get; set; } = string.Empty;

    public string ProvinceName { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}

public sealed record CityPage(
    IReadOnlyCollection<CityItem> Items,
    int TotalCount,
    int PageNumber,
    int PageSize);

public sealed record SaveCityRequest(
    int CountryId,
    int ProvinceId,
    string Code,
    string Name,
    bool IsActive);
