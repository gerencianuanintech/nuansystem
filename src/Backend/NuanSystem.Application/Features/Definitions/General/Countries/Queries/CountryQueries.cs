using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.General.Common.Dtos;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;

namespace NuanSystem.Application.Features.Definitions.General.Countries.Queries;

public sealed record GetCountriesQuery : IQuery<IReadOnlyCollection<CountryDto>>;
public sealed record GetCountryLookupQuery : IQuery<IReadOnlyCollection<GeographyLookupDto>>;
public sealed record GetCountryByIdQuery(int Id) : IQuery<CountryDto>;
