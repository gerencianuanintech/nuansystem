using NuanSystem.WinForms.Services.Definitions.General.Cities;
using NuanSystem.WinForms.Services.Definitions.General.Common;
using NuanSystem.WinForms.Services.Definitions.General.Countries;
using NuanSystem.WinForms.Services.Definitions.General.Provinces;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Definitions.General.Cities;

public sealed class CitiesViewModel(IGeographyClient geographyClient)
    : CrudViewModel<CityItem, SaveCityRequest>
{
    public IReadOnlyCollection<GeographyLookupItem> Countries { get; private set; } = Array.Empty<GeographyLookupItem>();

    public IReadOnlyCollection<GeographyLookupItem> Provinces { get; private set; } = Array.Empty<GeographyLookupItem>();

    public override async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Countries = await geographyClient.GetCountryLookupAsync(cancellationToken);
        Provinces = Array.Empty<GeographyLookupItem>();
        await LoadItemsAsync(geographyClient.GetCitiesAsync, cancellationToken);
    }

    public async Task<IReadOnlyCollection<GeographyLookupItem>> LoadProvincesAsync(
        string countryCode,
        CancellationToken cancellationToken = default)
    {
        Provinces = await geographyClient.GetProvinceLookupAsync(countryCode, cancellationToken);
        return Provinces;
    }

    public Task<CityItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return geographyClient.GetCityByIdAsync(id, cancellationToken);
    }

    public Task<CountryItem> GetCountryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return geographyClient.GetCountryByIdAsync(id, cancellationToken);
    }

    public Task<ProvinceItem> GetProvinceByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return geographyClient.GetProvinceByIdAsync(id, cancellationToken);
    }

    public async Task<CountryItem> CreateCountryAsync(SaveCountryRequest request, CancellationToken cancellationToken = default)
    {
        var saved = await geographyClient.CreateCountryAsync(request, cancellationToken);
        UpdateCountryLookup(saved);
        return saved;
    }

    public Task<ProvinceItem> CreateProvinceAsync(SaveProvinceRequest request, CancellationToken cancellationToken = default)
    {
        return geographyClient.CreateProvinceAsync(request, cancellationToken);
    }

    public async Task<CountryItem> UpdateCountryAsync(int id, SaveCountryRequest request, CancellationToken cancellationToken = default)
    {
        var saved = await geographyClient.UpdateCountryAsync(id, request, cancellationToken);
        UpdateCountryLookup(saved);
        return saved;
    }

    public Task<ProvinceItem> UpdateProvinceAsync(int id, SaveProvinceRequest request, CancellationToken cancellationToken = default)
    {
        return geographyClient.UpdateProvinceAsync(id, request, cancellationToken);
    }

    private void UpdateCountryLookup(CountryItem country)
    {
        Countries = Countries
            .Where(item => item.Id != country.Id)
            .Concat(country.IsActive
                ? [new GeographyLookupItem
                {
                    Id = country.Id,
                    Code = country.Code,
                    Name = country.Name,
                    IsActive = true
                }]
                : [])
            .OrderBy(item => item.Name)
            .ToArray();
    }

    public override Task CreateAsync(SaveCityRequest request, CancellationToken cancellationToken = default)
    {
        return geographyClient.CreateCityAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveCityRequest request, CancellationToken cancellationToken = default)
    {
        return geographyClient.UpdateCityAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return geographyClient.DeleteCityAsync(id, cancellationToken);
    }
}
