using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Features.SapSync.Cities.Contracts;

public sealed record SapCitySnapshot(
    string CountryCode,
    string ProvinceCode,
    string CityCode,
    string CityName)
{
    public string ProvinceExternalCode => BuildProvinceExternalCode(CountryCode, ProvinceCode);
    public string ExternalCode => BuildExternalCode(CountryCode, ProvinceCode, CityCode);

    public static SapCitySnapshot FromRecord(SapCityRecord record) =>
        new(record.CountryCode, record.ProvinceCode, record.CityCode, record.CityName);

    public static string BuildProvinceExternalCode(string? countryCode, string? provinceCode) =>
        $"{Normalize(countryCode)}|{Normalize(provinceCode)}";

    public static string BuildExternalCode(string? countryCode, string? provinceCode, string? cityCode) =>
        $"{Normalize(countryCode)}|{Normalize(provinceCode)}|{Normalize(cityCode)}";

    private static string Normalize(string? value) =>
        value?.Trim().ToUpperInvariant() ?? string.Empty;
}
