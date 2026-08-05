using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Features.SapSync.Countries.Contracts;

public sealed record SapCountrySnapshot(
    string CountryCode,
    string CountryName,
    string? Iso2,
    string? Iso3)
{
    public static SapCountrySnapshot FromRecord(SapCountryRecord record) =>
        new(record.CountryCode, record.CountryName, record.Iso2, record.Iso3);
}
