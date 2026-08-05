using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using NuanSystem.Application.Abstractions.Geography;
using NuanSystem.Application.Features.Geography.Common.Dtos;

namespace NuanSystem.Infrastructure.Geography;

public sealed class GoogleMapsGeographyService(
    HttpClient httpClient,
    IConfiguration configuration) : IReverseGeocodingService, IStaticMapService
{
    private const string DefaultGeocodingBaseUrl = "https://maps.googleapis.com/maps/api/geocode/json";
    private const string DefaultStaticMapBaseUrl = "https://maps.googleapis.com/maps/api/staticmap";

    public bool IsConfigured => !string.IsNullOrWhiteSpace(configuration["Geocoding:Google:ApiKey"]);

    public async Task<ReverseGeocodeResultDto?> ReverseGeocodeAsync(
        decimal latitude,
        decimal longitude,
        CancellationToken cancellationToken = default)
    {
        var apiKey = configuration["Geocoding:Google:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return null;
        }

        var url = BuildReverseGeocodeUrl(latitude, longitude, apiKey);
        var result = await httpClient.GetFromJsonAsync<GoogleGeocodeResponse>(url, cancellationToken);
        var firstResult = result?.Results.FirstOrDefault();
        if (!string.Equals(result?.Status, "OK", StringComparison.OrdinalIgnoreCase) || firstResult is null)
        {
            return null;
        }

        return new ReverseGeocodeResultDto
        {
            Country = Component(firstResult, "country", preferShortName: false),
            CountryCode = Component(firstResult, "country", preferShortName: true)?.ToUpperInvariant(),
            Province = Component(firstResult, "administrative_area_level_1", preferShortName: false),
            City = FirstComponent(firstResult, "locality", "administrative_area_level_2", "postal_town", "sublocality", "sublocality_level_1"),
            PostalCode = Component(firstResult, "postal_code", preferShortName: false),
            FormattedAddress = NullIfWhiteSpace(firstResult.FormattedAddress)
        };
    }

    public async Task<StaticMapResultDto?> GetStaticMapAsync(
        decimal latitude,
        decimal longitude,
        CancellationToken cancellationToken = default)
    {
        var apiKey = configuration["Geocoding:Google:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return null;
        }

        var url = BuildStaticMapUrl(latitude, longitude, apiKey);
        using var response = await httpClient.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        if (bytes.Length == 0)
        {
            return null;
        }

        return new StaticMapResultDto
        {
            ContentType = response.Content.Headers.ContentType?.MediaType ?? "image/png",
            ImageBase64 = Convert.ToBase64String(bytes)
        };
    }

    private string BuildReverseGeocodeUrl(decimal latitude, decimal longitude, string apiKey)
    {
        var baseUrl = configuration["Geocoding:Google:GeocodingBaseUrl"] ?? DefaultGeocodingBaseUrl;
        var language = configuration["Geocoding:Google:Language"] ?? "es";
        var region = configuration["Geocoding:Google:Region"] ?? "ec";
        var lat = latitude.ToString(CultureInfo.InvariantCulture);
        var lon = longitude.ToString(CultureInfo.InvariantCulture);

        return $"{baseUrl}?latlng={lat},{lon}&language={Uri.EscapeDataString(language)}&region={Uri.EscapeDataString(region)}&key={Uri.EscapeDataString(apiKey)}";
    }

    private string BuildStaticMapUrl(decimal latitude, decimal longitude, string apiKey)
    {
        var baseUrl = configuration["Geocoding:Google:StaticMapBaseUrl"] ?? DefaultStaticMapBaseUrl;
        var size = configuration["Geocoding:Google:StaticMapSize"] ?? "600x350";
        var zoom = configuration.GetValue("Geocoding:Google:StaticMapZoom", 16);
        var scale = configuration.GetValue("Geocoding:Google:StaticMapScale", 2);
        var lat = latitude.ToString(CultureInfo.InvariantCulture);
        var lon = longitude.ToString(CultureInfo.InvariantCulture);
        var marker = Uri.EscapeDataString($"color:red|{lat},{lon}");

        return $"{baseUrl}?center={lat},{lon}&zoom={zoom}&size={Uri.EscapeDataString(size)}&scale={scale}&markers={marker}&key={Uri.EscapeDataString(apiKey)}";
    }

    private static string? FirstComponent(GoogleGeocodeResult result, params string[] types)
    {
        foreach (var type in types)
        {
            var value = Component(result, type, preferShortName: false);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }

    private static string? Component(GoogleGeocodeResult result, string type, bool preferShortName)
    {
        var component = result.AddressComponents.FirstOrDefault(item => item.Types.Contains(type));
        return NullIfWhiteSpace(preferShortName ? component?.ShortName : component?.LongName);
    }

    private static string? NullIfWhiteSpace(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private sealed class GoogleGeocodeResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("results")]
        public List<GoogleGeocodeResult> Results { get; set; } = [];
    }

    private sealed class GoogleGeocodeResult
    {
        [JsonPropertyName("formatted_address")]
        public string? FormattedAddress { get; set; }

        [JsonPropertyName("address_components")]
        public List<GoogleAddressComponent> AddressComponents { get; set; } = [];
    }

    private sealed class GoogleAddressComponent
    {
        [JsonPropertyName("long_name")]
        public string? LongName { get; set; }

        [JsonPropertyName("short_name")]
        public string? ShortName { get; set; }

        [JsonPropertyName("types")]
        public List<string> Types { get; set; } = [];
    }
}
