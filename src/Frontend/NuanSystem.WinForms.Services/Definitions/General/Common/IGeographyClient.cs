using NuanSystem.WinForms.Services.Definitions.General.Cities;
using NuanSystem.WinForms.Services.Definitions.General.Countries;
using NuanSystem.WinForms.Services.Definitions.General.Provinces;

namespace NuanSystem.WinForms.Services.Definitions.General.Common;

public interface IGeographyClient
{
    Task<IReadOnlyCollection<CountryItem>> GetCountriesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ProvinceItem>> GetProvincesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CityItem>> GetCitiesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<GeographyLookupItem>> GetCountryLookupAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<GeographyLookupItem>> GetProvinceLookupAsync(string? countryCode = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<GeographyLookupItem>> GetCityLookupAsync(string? countryCode = null, string? provinceCode = null, CancellationToken cancellationToken = default);

    Task<ReverseGeocodeResult> ReverseGeocodeAsync(decimal latitude, decimal longitude, CancellationToken cancellationToken = default);

    Task<StaticMapResult> GetStaticMapAsync(decimal latitude, decimal longitude, CancellationToken cancellationToken = default);

    Task<CountryItem> GetCountryByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ProvinceItem> GetProvinceByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<CityItem> GetCityByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<CountryItem> CreateCountryAsync(SaveCountryRequest request, CancellationToken cancellationToken = default);

    Task<ProvinceItem> CreateProvinceAsync(SaveProvinceRequest request, CancellationToken cancellationToken = default);

    Task<CityItem> CreateCityAsync(SaveCityRequest request, CancellationToken cancellationToken = default);

    Task<CountryItem> UpdateCountryAsync(int id, SaveCountryRequest request, CancellationToken cancellationToken = default);

    Task<ProvinceItem> UpdateProvinceAsync(int id, SaveProvinceRequest request, CancellationToken cancellationToken = default);

    Task<CityItem> UpdateCityAsync(int id, SaveCityRequest request, CancellationToken cancellationToken = default);

    Task DeleteCountryAsync(int id, CancellationToken cancellationToken = default);

    Task DeleteProvinceAsync(int id, CancellationToken cancellationToken = default);

    Task DeleteCityAsync(int id, CancellationToken cancellationToken = default);
}
