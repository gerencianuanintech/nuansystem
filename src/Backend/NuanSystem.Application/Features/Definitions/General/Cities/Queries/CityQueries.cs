using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.General.Cities.Dtos;
using NuanSystem.Application.Features.Definitions.General.Common.Dtos;

namespace NuanSystem.Application.Features.Definitions.General.Cities.Queries;

public sealed record GetCitiesQuery : IQuery<IReadOnlyCollection<CityDto>>;
public sealed record SearchCitiesQuery(string? Search, int PageNumber = 1, int PageSize = 50) : IQuery<CityPageDto>;
public sealed record GetCityLookupQuery(string? CountryCode, string? ProvinceCode) : IQuery<IReadOnlyCollection<GeographyLookupDto>>;
public sealed record GetCityByIdQuery(int Id) : IQuery<CityDto>;
