namespace NuanSystem.WinForms.Services.Definitions.General.Provinces;

public sealed class ProvinceItem
{
    public int Id { get; set; }

    public int CountryId { get; set; }

    public string CountryCode { get; set; } = string.Empty;

    public string CountryName { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}

public sealed record ProvincePage(
    IReadOnlyCollection<ProvinceItem> Items,
    int TotalCount,
    int PageNumber,
    int PageSize);

public sealed record SaveProvinceRequest(
    int CountryId,
    string Code,
    string Name,
    bool IsActive);
