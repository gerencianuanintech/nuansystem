using NuanSystem.Application.Features.Geography.Common.Dtos;

namespace NuanSystem.Application.Abstractions.Geography;

public interface IReverseGeocodingService
{
    bool IsConfigured { get; }

    Task<ReverseGeocodeResultDto?> ReverseGeocodeAsync(
        decimal latitude,
        decimal longitude,
        CancellationToken cancellationToken = default);
}

public interface IStaticMapService
{
    bool IsConfigured { get; }

    Task<StaticMapResultDto?> GetStaticMapAsync(
        decimal latitude,
        decimal longitude,
        CancellationToken cancellationToken = default);
}
