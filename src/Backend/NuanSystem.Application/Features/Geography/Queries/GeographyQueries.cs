using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Geography.Dtos;

namespace NuanSystem.Application.Features.Geography.Queries;

public sealed record GetCountriesQuery : IQuery<IReadOnlyCollection<CountryDto>>;

public sealed record GetProvincesQuery : IQuery<IReadOnlyCollection<ProvinceDto>>;

public sealed record GetCitiesQuery : IQuery<IReadOnlyCollection<CityDto>>;

public sealed record GetCountryLookupQuery : IQuery<IReadOnlyCollection<GeographyLookupDto>>;

public sealed record GetProvinceLookupQuery(string? CountryCode) : IQuery<IReadOnlyCollection<GeographyLookupDto>>;

public sealed record GetCityLookupQuery(string? CountryCode, string? ProvinceCode) : IQuery<IReadOnlyCollection<GeographyLookupDto>>;

public sealed record ReverseGeocodeQuery(decimal Latitude, decimal Longitude) : IQuery<ReverseGeocodeResultDto>;

public sealed record GetStaticMapQuery(decimal Latitude, decimal Longitude) : IQuery<StaticMapResultDto>;

public sealed record GetCountryByIdQuery(int Id) : IQuery<CountryDto>;

public sealed record GetProvinceByIdQuery(int Id) : IQuery<ProvinceDto>;

public sealed record GetCityByIdQuery(int Id) : IQuery<CityDto>;
