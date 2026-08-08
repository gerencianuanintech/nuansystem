using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.General.Common.Dtos;
using NuanSystem.Application.Features.Definitions.General.Provinces.Dtos;

namespace NuanSystem.Application.Features.Definitions.General.Provinces.Queries;

public sealed record GetProvincesQuery : IQuery<IReadOnlyCollection<ProvinceDto>>;
public sealed record SearchProvincesQuery(string? Search, int PageNumber = 1, int PageSize = 50) : IQuery<ProvincePageDto>;
public sealed record GetProvinceLookupQuery(string? CountryCode) : IQuery<IReadOnlyCollection<GeographyLookupDto>>;
public sealed record GetProvinceByIdQuery(int Id) : IQuery<ProvinceDto>;
