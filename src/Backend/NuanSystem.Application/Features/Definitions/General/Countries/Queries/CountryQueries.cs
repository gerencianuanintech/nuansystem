using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.General.Common.Dtos;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;

namespace NuanSystem.Application.Features.Definitions.General.Countries.Queries;

public sealed record GetCountriesQuery : IQuery<IReadOnlyCollection<CountryDto>>;
public sealed record SearchCountriesQuery(string? Search, int PageNumber = 1, int PageSize = 50) : IQuery<CountryPageDto>;
public sealed record GetCountryLookupQuery : IQuery<IReadOnlyCollection<GeographyLookupDto>>;
public sealed record GetCountryByIdQuery(int Id) : IQuery<CountryDto>;
