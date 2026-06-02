namespace NuanSystem.WinForms.Services.Geography.Models;

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

public sealed class GeographyLookupItem
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}

public sealed class ReverseGeocodeResult
{
    public string? Country { get; set; }

    public string? CountryCode { get; set; }

    public string? Province { get; set; }

    public string? City { get; set; }

    public string? PostalCode { get; set; }

    public string? FormattedAddress { get; set; }
}

public sealed class StaticMapResult
{
    public string ContentType { get; set; } = "image/png";

    public string ImageBase64 { get; set; } = string.Empty;
}

public sealed record SaveCountryRequest(
    string Code,
    string Name,
    string? Iso2,
    string? Iso3,
    string? PhonePrefix,
    bool IsActive);

public sealed record SaveProvinceRequest(
    int CountryId,
    string Code,
    string Name,
    bool IsActive);

public sealed record SaveCityRequest(
    int CountryId,
    int ProvinceId,
    string Code,
    string Name,
    bool IsActive);
