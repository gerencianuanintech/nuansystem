using NuanSystem.Application.Abstractions.Geography;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Geography.Common.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Geography.Common.Queries;

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
