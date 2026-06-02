using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Geography;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Geography.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Geography.Queries;

public sealed class GetCountriesQueryHandler(IGeographyRepository repository)
    : IQueryHandler<GetCountriesQuery, IReadOnlyCollection<CountryDto>>
{
    public async Task<Result<IReadOnlyCollection<CountryDto>>> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
    {
        return Result<IReadOnlyCollection<CountryDto>>.Success(await repository.GetCountriesAsync(cancellationToken));
    }
}

public sealed class GetProvincesQueryHandler(IGeographyRepository repository)
    : IQueryHandler<GetProvincesQuery, IReadOnlyCollection<ProvinceDto>>
{
    public async Task<Result<IReadOnlyCollection<ProvinceDto>>> Handle(GetProvincesQuery request, CancellationToken cancellationToken)
    {
        return Result<IReadOnlyCollection<ProvinceDto>>.Success(await repository.GetProvincesAsync(cancellationToken));
    }
}

public sealed class GetCitiesQueryHandler(IGeographyRepository repository)
    : IQueryHandler<GetCitiesQuery, IReadOnlyCollection<CityDto>>
{
    public async Task<Result<IReadOnlyCollection<CityDto>>> Handle(GetCitiesQuery request, CancellationToken cancellationToken)
    {
        return Result<IReadOnlyCollection<CityDto>>.Success(await repository.GetCitiesAsync(cancellationToken));
    }
}

public sealed class GetCountryLookupQueryHandler(IGeographyRepository repository)
    : IQueryHandler<GetCountryLookupQuery, IReadOnlyCollection<GeographyLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<GeographyLookupDto>>> Handle(GetCountryLookupQuery request, CancellationToken cancellationToken)
    {
        return Result<IReadOnlyCollection<GeographyLookupDto>>.Success(await repository.GetCountryLookupAsync(cancellationToken));
    }
}

public sealed class GetProvinceLookupQueryHandler(IGeographyRepository repository)
    : IQueryHandler<GetProvinceLookupQuery, IReadOnlyCollection<GeographyLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<GeographyLookupDto>>> Handle(GetProvinceLookupQuery request, CancellationToken cancellationToken)
    {
        return Result<IReadOnlyCollection<GeographyLookupDto>>.Success(await repository.GetProvinceLookupAsync(request.CountryCode, cancellationToken));
    }
}

public sealed class GetCityLookupQueryHandler(IGeographyRepository repository)
    : IQueryHandler<GetCityLookupQuery, IReadOnlyCollection<GeographyLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<GeographyLookupDto>>> Handle(GetCityLookupQuery request, CancellationToken cancellationToken)
    {
        return Result<IReadOnlyCollection<GeographyLookupDto>>.Success(await repository.GetCityLookupAsync(request.CountryCode, request.ProvinceCode, cancellationToken));
    }
}

public sealed class ReverseGeocodeQueryHandler(IReverseGeocodingService reverseGeocodingService)
    : IQueryHandler<ReverseGeocodeQuery, ReverseGeocodeResultDto>
{
    public async Task<Result<ReverseGeocodeResultDto>> Handle(ReverseGeocodeQuery request, CancellationToken cancellationToken)
    {
        if (request.Latitude < -90 || request.Latitude > 90)
        {
            return Result<ReverseGeocodeResultDto>.Failure(
                "La latitud debe estar entre -90 y 90.",
                [new ApiError("GEOGRAPHY_INVALID_LATITUDE", "La latitud debe estar entre -90 y 90.", nameof(request.Latitude))]);
        }

        if (request.Longitude < -180 || request.Longitude > 180)
        {
            return Result<ReverseGeocodeResultDto>.Failure(
                "La longitud debe estar entre -180 y 180.",
                [new ApiError("GEOGRAPHY_INVALID_LONGITUDE", "La longitud debe estar entre -180 y 180.", nameof(request.Longitude))]);
        }

        if (!reverseGeocodingService.IsConfigured)
        {
            return Result<ReverseGeocodeResultDto>.Failure(
                "El proveedor Google Maps no esta configurado. Configure Geocoding:Google:ApiKey en el backend.",
                [new ApiError("GEOGRAPHY_PROVIDER_NOT_CONFIGURED", "Falta configurar la clave de Google Maps en el backend.", "Geocoding.Google.ApiKey")]);
        }

        var result = await reverseGeocodingService.ReverseGeocodeAsync(request.Latitude, request.Longitude, cancellationToken);
        if (result is null)
        {
            return Result<ReverseGeocodeResultDto>.Failure(
                "No fue posible obtener una direccion para las coordenadas ingresadas.",
                [new ApiError("GEOGRAPHY_REVERSE_GEOCODE_NOT_FOUND", "No se encontraron datos geograficos para esas coordenadas.", nameof(request.Latitude))]);
        }

        return Result<ReverseGeocodeResultDto>.Success(result);
    }
}

public sealed class GetStaticMapQueryHandler(IStaticMapService staticMapService)
    : IQueryHandler<GetStaticMapQuery, StaticMapResultDto>
{
    public async Task<Result<StaticMapResultDto>> Handle(GetStaticMapQuery request, CancellationToken cancellationToken)
    {
        if (request.Latitude < -90 || request.Latitude > 90)
        {
            return Result<StaticMapResultDto>.Failure(
                "La latitud debe estar entre -90 y 90.",
                [new ApiError("GEOGRAPHY_INVALID_LATITUDE", "La latitud debe estar entre -90 y 90.", nameof(request.Latitude))]);
        }

        if (request.Longitude < -180 || request.Longitude > 180)
        {
            return Result<StaticMapResultDto>.Failure(
                "La longitud debe estar entre -180 y 180.",
                [new ApiError("GEOGRAPHY_INVALID_LONGITUDE", "La longitud debe estar entre -180 y 180.", nameof(request.Longitude))]);
        }

        if (!staticMapService.IsConfigured)
        {
            return Result<StaticMapResultDto>.Failure(
                "El proveedor Google Maps no esta configurado. Configure Geocoding:Google:ApiKey en el backend.",
                [new ApiError("GEOGRAPHY_PROVIDER_NOT_CONFIGURED", "Falta configurar la clave de Google Maps en el backend.", "Geocoding.Google.ApiKey")]);
        }

        var result = await staticMapService.GetStaticMapAsync(request.Latitude, request.Longitude, cancellationToken);
        if (result is null || string.IsNullOrWhiteSpace(result.ImageBase64))
        {
            return Result<StaticMapResultDto>.Failure(
                "No fue posible obtener la vista previa del mapa.",
                [new ApiError("GEOGRAPHY_MAP_NOT_AVAILABLE", "El proveedor de mapas no devolvio una imagen valida.", nameof(request.Latitude))]);
        }

        return Result<StaticMapResultDto>.Success(result);
    }
}

public sealed class GetCountryByIdQueryHandler(IGeographyRepository repository)
    : IQueryHandler<GetCountryByIdQuery, CountryDto>
{
    public async Task<Result<CountryDto>> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await repository.GetCountryByIdAsync(request.Id, cancellationToken);
        return item is null
            ? Result<CountryDto>.Failure("No se encontro el pais.", [new ApiError("GEOGRAPHY_COUNTRY_NOT_FOUND", "El pais no existe.", nameof(request.Id))])
            : Result<CountryDto>.Success(item);
    }
}

public sealed class GetProvinceByIdQueryHandler(IGeographyRepository repository)
    : IQueryHandler<GetProvinceByIdQuery, ProvinceDto>
{
    public async Task<Result<ProvinceDto>> Handle(GetProvinceByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await repository.GetProvinceByIdAsync(request.Id, cancellationToken);
        return item is null
            ? Result<ProvinceDto>.Failure("No se encontro la provincia.", [new ApiError("GEOGRAPHY_PROVINCE_NOT_FOUND", "La provincia no existe.", nameof(request.Id))])
            : Result<ProvinceDto>.Success(item);
    }
}

public sealed class GetCityByIdQueryHandler(IGeographyRepository repository)
    : IQueryHandler<GetCityByIdQuery, CityDto>
{
    public async Task<Result<CityDto>> Handle(GetCityByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await repository.GetCityByIdAsync(request.Id, cancellationToken);
        return item is null
            ? Result<CityDto>.Failure("No se encontro la ciudad.", [new ApiError("GEOGRAPHY_CITY_NOT_FOUND", "La ciudad no existe.", nameof(request.Id))])
            : Result<CityDto>.Success(item);
    }
}
