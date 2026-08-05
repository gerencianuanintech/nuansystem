using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Features.SapSync.Provinces.Contracts;

public sealed record SapProvinceSnapshot(
    string CountryCode,
    string ProvinceCode,
    string ProvinceName)
{
    public string ExternalCode => BuildExternalCode(CountryCode, ProvinceCode);

    public static SapProvinceSnapshot FromRecord(SapProvinceRecord record) =>
        new(record.CountryCode, record.ProvinceCode, record.ProvinceName);

    public static string BuildExternalCode(string? countryCode, string? provinceCode) =>
        $"{Normalize(countryCode)}|{Normalize(provinceCode)}";

    private static string Normalize(string? value) =>
        value?.Trim().ToUpperInvariant() ?? string.Empty;
}
