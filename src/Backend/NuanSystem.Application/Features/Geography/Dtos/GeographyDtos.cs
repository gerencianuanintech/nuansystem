namespace NuanSystem.Application.Features.Geography.Dtos;

public sealed class CountryDto
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Iso2 { get; set; }

    public string? Iso3 { get; set; }

    public string? PhonePrefix { get; set; }

    public bool IsActive { get; set; }
}

public sealed class ProvinceDto
{
    public int Id { get; set; }

    public int CountryId { get; set; }

    public string CountryCode { get; set; } = string.Empty;

    public string CountryName { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}

public sealed class CityDto
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

public sealed class GeographyLookupDto
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}

public sealed class ReverseGeocodeResultDto
{
    public string? Country { get; set; }

    public string? CountryCode { get; set; }

    public string? Province { get; set; }

    public string? City { get; set; }

    public string? PostalCode { get; set; }

    public string? FormattedAddress { get; set; }
}

public sealed class StaticMapResultDto
{
    public string ContentType { get; set; } = "image/png";

    public string ImageBase64 { get; set; } = string.Empty;
}

public sealed record SaveCountryData(
    int? Id,
    string Code,
    string Name,
    string? Iso2,
    string? Iso3,
    string? PhonePrefix,
    bool IsActive,
    int? AuditUserId,
    string? AuditUserName);

public sealed record SaveProvinceData(
    int? Id,
    int CountryId,
    string Code,
    string Name,
    bool IsActive,
    int? AuditUserId,
    string? AuditUserName);

public sealed record SaveCityData(
    int? Id,
    int CountryId,
    int ProvinceId,
    string Code,
    string Name,
    bool IsActive,
    int? AuditUserId,
    string? AuditUserName);
