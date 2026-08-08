using System.Globalization;
using NuanSystem.WinForms.Services.Definitions.General.Cities;
using NuanSystem.WinForms.Services.Definitions.General.Countries;
using NuanSystem.WinForms.Services.Definitions.General.Provinces;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.Definitions.General.Common;

public sealed class GeographyClient(INuanApiClient apiClient) : IGeographyClient
{
    public Task<IReadOnlyCollection<CountryItem>> GetCountriesAsync(CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<IReadOnlyCollection<CountryItem>>("/api/geography/countries", cancellationToken);
    }

    public Task<CountryPage> SearchCountriesAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var parameters = new List<string>
        {
            $"pageNumber={pageNumber}",
            $"pageSize={pageSize}"
        };
        if (!string.IsNullOrWhiteSpace(search))
        {
            parameters.Add($"search={Uri.EscapeDataString(search.Trim())}");
        }

        return apiClient.GetAsync<CountryPage>(
            $"/api/geography/countries/page?{string.Join("&", parameters)}",
            cancellationToken);
    }

    public Task<IReadOnlyCollection<ProvinceItem>> GetProvincesAsync(CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<IReadOnlyCollection<ProvinceItem>>("/api/geography/provinces", cancellationToken);
    }

    public Task<ProvincePage> SearchProvincesAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<ProvincePage>(
            BuildPagedSearchRoute("/api/geography/provinces/page", search, pageNumber, pageSize),
            cancellationToken);
    }

    public Task<IReadOnlyCollection<CityItem>> GetCitiesAsync(CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<IReadOnlyCollection<CityItem>>("/api/geography/cities", cancellationToken);
    }

    public Task<CityPage> SearchCitiesAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<CityPage>(
            BuildPagedSearchRoute("/api/geography/cities/page", search, pageNumber, pageSize),
            cancellationToken);
    }

    public Task<IReadOnlyCollection<GeographyLookupItem>> GetCountryLookupAsync(CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<IReadOnlyCollection<GeographyLookupItem>>("/api/geography/countries/lookup", cancellationToken);
    }

    public Task<IReadOnlyCollection<GeographyLookupItem>> GetProvinceLookupAsync(string? countryCode = null, CancellationToken cancellationToken = default)
    {
        var route = string.IsNullOrWhiteSpace(countryCode)
            ? "/api/geography/provinces/lookup"
            : $"/api/geography/provinces/lookup?countryCode={Uri.EscapeDataString(countryCode)}";

        return apiClient.GetAsync<IReadOnlyCollection<GeographyLookupItem>>(route, cancellationToken);
    }

    public Task<IReadOnlyCollection<GeographyLookupItem>> GetCityLookupAsync(string? countryCode = null, string? provinceCode = null, CancellationToken cancellationToken = default)
    {
        var query = BuildCityLookupQuery(countryCode, provinceCode);
        return apiClient.GetAsync<IReadOnlyCollection<GeographyLookupItem>>($"/api/geography/cities/lookup{query}", cancellationToken);
    }

    public Task<ReverseGeocodeResult> ReverseGeocodeAsync(decimal latitude, decimal longitude, CancellationToken cancellationToken = default)
    {
        var lat = latitude.ToString(CultureInfo.InvariantCulture);
        var lon = longitude.ToString(CultureInfo.InvariantCulture);
        return apiClient.GetAsync<ReverseGeocodeResult>($"/api/geography/reverse-geocode?latitude={lat}&longitude={lon}", cancellationToken);
    }

    public Task<StaticMapResult> GetStaticMapAsync(decimal latitude, decimal longitude, CancellationToken cancellationToken = default)
    {
        var lat = latitude.ToString(CultureInfo.InvariantCulture);
        var lon = longitude.ToString(CultureInfo.InvariantCulture);
        return apiClient.GetAsync<StaticMapResult>($"/api/geography/static-map?latitude={lat}&longitude={lon}", cancellationToken);
    }

    public Task<CountryItem> GetCountryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<CountryItem>($"/api/geography/countries/{id}", cancellationToken);
    }

    public Task<ProvinceItem> GetProvinceByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<ProvinceItem>($"/api/geography/provinces/{id}", cancellationToken);
    }

    public Task<CityItem> GetCityByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<CityItem>($"/api/geography/cities/{id}", cancellationToken);
    }

    public Task<CountryItem> CreateCountryAsync(SaveCountryRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveCountryRequest, CountryItem>("/api/geography/countries", request, cancellationToken);
    }

    public Task<ProvinceItem> CreateProvinceAsync(SaveProvinceRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveProvinceRequest, ProvinceItem>("/api/geography/provinces", request, cancellationToken);
    }

    public Task<CityItem> CreateCityAsync(SaveCityRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveCityRequest, CityItem>("/api/geography/cities", request, cancellationToken);
    }

    public Task<CountryItem> UpdateCountryAsync(int id, SaveCountryRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveCountryRequest, CountryItem>($"/api/geography/countries/{id}", request, cancellationToken);
    }

    public Task<ProvinceItem> UpdateProvinceAsync(int id, SaveProvinceRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveProvinceRequest, ProvinceItem>($"/api/geography/provinces/{id}", request, cancellationToken);
    }

    public Task<CityItem> UpdateCityAsync(int id, SaveCityRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveCityRequest, CityItem>($"/api/geography/cities/{id}", request, cancellationToken);
    }

    public async Task DeleteCountryAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"/api/geography/countries/{id}", cancellationToken);
    }

    public async Task DeleteProvinceAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"/api/geography/provinces/{id}", cancellationToken);
    }

    public async Task DeleteCityAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"/api/geography/cities/{id}", cancellationToken);
    }

    private static string BuildCityLookupQuery(string? countryCode, string? provinceCode)
    {
        var parameters = new List<string>();
        if (!string.IsNullOrWhiteSpace(countryCode))
        {
            parameters.Add($"countryCode={Uri.EscapeDataString(countryCode)}");
        }

        if (!string.IsNullOrWhiteSpace(provinceCode))
        {
            parameters.Add($"provinceCode={Uri.EscapeDataString(provinceCode)}");
        }

        return parameters.Count == 0 ? string.Empty : $"?{string.Join("&", parameters)}";
    }

    private static string BuildPagedSearchRoute(
        string route,
        string? search,
        int pageNumber,
        int pageSize)
    {
        var parameters = new List<string>
        {
            $"pageNumber={pageNumber}",
            $"pageSize={pageSize}"
        };
        if (!string.IsNullOrWhiteSpace(search))
        {
            parameters.Add($"search={Uri.EscapeDataString(search.Trim())}");
        }

        return $"{route}?{string.Join("&", parameters)}";
    }
}
