namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed record SapCountryRecord(
    string CountryCode,
    string CountryName,
    string? Iso2,
    string? Iso3);
