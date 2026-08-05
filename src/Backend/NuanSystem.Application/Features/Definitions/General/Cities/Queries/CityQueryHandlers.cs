using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.General.Cities.Dtos;
using NuanSystem.Application.Features.Definitions.General.Common.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Definitions.General.Cities.Queries;

public sealed class GetCitiesQueryHandler(IGeographyRepository repository) : IQueryHandler<GetCitiesQuery, IReadOnlyCollection<CityDto>>
{
    public async Task<Result<IReadOnlyCollection<CityDto>>> Handle(GetCitiesQuery request, CancellationToken cancellationToken) => Result<IReadOnlyCollection<CityDto>>.Success(await repository.GetCitiesAsync(cancellationToken));
}

public sealed class GetCityLookupQueryHandler(IGeographyRepository repository) : IQueryHandler<GetCityLookupQuery, IReadOnlyCollection<GeographyLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<GeographyLookupDto>>> Handle(GetCityLookupQuery request, CancellationToken cancellationToken) => Result<IReadOnlyCollection<GeographyLookupDto>>.Success(await repository.GetCityLookupAsync(request.CountryCode, request.ProvinceCode, cancellationToken));
}

public sealed class GetCityByIdQueryHandler(IGeographyRepository repository) : IQueryHandler<GetCityByIdQuery, CityDto>
{
    public async Task<Result<CityDto>> Handle(GetCityByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await repository.GetCityByIdAsync(request.Id, cancellationToken);
        return item is null ? Result<CityDto>.Failure("No se encontro la ciudad.", [new ApiError("GEOGRAPHY_CITY_NOT_FOUND", "La ciudad no existe.", nameof(request.Id))]) : Result<CityDto>.Success(item);
    }
}
