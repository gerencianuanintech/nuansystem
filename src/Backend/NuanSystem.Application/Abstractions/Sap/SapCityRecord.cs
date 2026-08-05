namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed record SapCityRecord(
    string CountryCode,
    string ProvinceCode,
    string CityCode,
    string CityName);
