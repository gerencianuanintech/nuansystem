using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.General.Common.Dtos;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Definitions.General.Countries.Queries;

public sealed class GetCountriesQueryHandler(IGeographyRepository repository) : IQueryHandler<GetCountriesQuery, IReadOnlyCollection<CountryDto>>
{
    public async Task<Result<IReadOnlyCollection<CountryDto>>> Handle(GetCountriesQuery request, CancellationToken cancellationToken) => Result<IReadOnlyCollection<CountryDto>>.Success(await repository.GetCountriesAsync(cancellationToken));
}

public sealed class GetCountryLookupQueryHandler(IGeographyRepository repository) : IQueryHandler<GetCountryLookupQuery, IReadOnlyCollection<GeographyLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<GeographyLookupDto>>> Handle(GetCountryLookupQuery request, CancellationToken cancellationToken) => Result<IReadOnlyCollection<GeographyLookupDto>>.Success(await repository.GetCountryLookupAsync(cancellationToken));
}

public sealed class GetCountryByIdQueryHandler(IGeographyRepository repository) : IQueryHandler<GetCountryByIdQuery, CountryDto>
{
    public async Task<Result<CountryDto>> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await repository.GetCountryByIdAsync(request.Id, cancellationToken);
        return item is null ? Result<CountryDto>.Failure("No se encontro el pais.", [new ApiError("GEOGRAPHY_COUNTRY_NOT_FOUND", "El pais no existe.", nameof(request.Id))]) : Result<CountryDto>.Success(item);
    }
}
