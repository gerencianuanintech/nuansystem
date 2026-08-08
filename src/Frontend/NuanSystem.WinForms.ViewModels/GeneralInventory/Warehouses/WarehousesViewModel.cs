using NuanSystem.WinForms.Services.Definitions.General.Common;
using NuanSystem.WinForms.Services.Definitions.General.Cities;
using NuanSystem.WinForms.Services.Definitions.General.Countries;
using NuanSystem.WinForms.Services.Definitions.General.Provinces;
using NuanSystem.WinForms.Services.GeneralInventory.Warehouses;
using NuanSystem.WinForms.Services.GeneralInventory.Warehouses.Models;
using NuanSystem.WinForms.Services.Security.Access;
using NuanSystem.WinForms.ViewModels.Common;
using NuanSystem.WinForms.ViewModels.Definitions.General.Common;

namespace NuanSystem.WinForms.ViewModels.GeneralInventory.Warehouses;

public sealed class WarehousesViewModel(
    IWarehouseClient warehouseClient,
    IGeographyClient geographyClient,
    ISecurityAccessClient securityAccessClient)
    : CrudViewModel<WarehouseItem, SaveWarehouseRequest>
{
    public IReadOnlyCollection<GeographyLookupItem> Countries { get; private set; } = [];

    public bool CanCreateCountries { get; private set; }
    public bool CanUpdateCountries { get; private set; }
    public bool CanCreateProvinces { get; private set; }
    public bool CanUpdateProvinces { get; private set; }
    public bool CanCreateCities { get; private set; }
    public bool CanUpdateCities { get; private set; }

    public override async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        var countriesTask = geographyClient.GetCountryLookupAsync(cancellationToken);
        var warehousesTask = LoadItemsAsync(warehouseClient.GetAsync, cancellationToken);
        var countryAccessTask = GeographyRelatedFormAccess.LoadAsync(securityAccessClient, "countries", cancellationToken);
        var provinceAccessTask = GeographyRelatedFormAccess.LoadAsync(securityAccessClient, "provinces", cancellationToken);
        var cityAccessTask = GeographyRelatedFormAccess.LoadAsync(securityAccessClient, "cities", cancellationToken);
        await Task.WhenAll(countriesTask, warehousesTask, countryAccessTask, provinceAccessTask, cityAccessTask);
        Countries = await countriesTask;
        var countryAccess = await countryAccessTask;
        var provinceAccess = await provinceAccessTask;
        var cityAccess = await cityAccessTask;
        CanCreateCountries = countryAccess.CanCreate;
        CanUpdateCountries = countryAccess.CanUpdate;
        CanCreateProvinces = provinceAccess.CanCreate;
        CanUpdateProvinces = provinceAccess.CanUpdate;
        CanCreateCities = cityAccess.CanCreate;
        CanUpdateCities = cityAccess.CanUpdate;
    }

    public Task<IReadOnlyCollection<GeographyLookupItem>> LoadProvincesAsync(
        string countryCode,
        CancellationToken cancellationToken = default) =>
        geographyClient.GetProvinceLookupAsync(countryCode, cancellationToken);

    public Task<IReadOnlyCollection<GeographyLookupItem>> LoadCitiesAsync(
        string countryCode,
        string provinceCode,
        CancellationToken cancellationToken = default) =>
        geographyClient.GetCityLookupAsync(countryCode, provinceCode, cancellationToken);

    public Task<CountryItem> GetCountryByIdAsync(int id, CancellationToken cancellationToken = default) =>
        geographyClient.GetCountryByIdAsync(id, cancellationToken);

    public Task<ProvinceItem> GetProvinceByIdAsync(int id, CancellationToken cancellationToken = default) =>
        geographyClient.GetProvinceByIdAsync(id, cancellationToken);

    public Task<CityItem> GetCityByIdAsync(int id, CancellationToken cancellationToken = default) =>
        geographyClient.GetCityByIdAsync(id, cancellationToken);

    public async Task<CountryItem> CreateCountryAsync(SaveCountryRequest request, CancellationToken cancellationToken = default)
    {
        var saved = await geographyClient.CreateCountryAsync(request, cancellationToken);
        UpdateCountryLookup(saved);
        return saved;
    }

    public async Task<CountryItem> UpdateCountryAsync(int id, SaveCountryRequest request, CancellationToken cancellationToken = default)
    {
        var saved = await geographyClient.UpdateCountryAsync(id, request, cancellationToken);
        UpdateCountryLookup(saved);
        return saved;
    }

    public Task<ProvinceItem> CreateProvinceAsync(SaveProvinceRequest request, CancellationToken cancellationToken = default) =>
        geographyClient.CreateProvinceAsync(request, cancellationToken);

    public Task<ProvinceItem> UpdateProvinceAsync(int id, SaveProvinceRequest request, CancellationToken cancellationToken = default) =>
        geographyClient.UpdateProvinceAsync(id, request, cancellationToken);

    public Task<CityItem> CreateCityAsync(SaveCityRequest request, CancellationToken cancellationToken = default) =>
        geographyClient.CreateCityAsync(request, cancellationToken);

    public Task<CityItem> UpdateCityAsync(int id, SaveCityRequest request, CancellationToken cancellationToken = default) =>
        geographyClient.UpdateCityAsync(id, request, cancellationToken);

    private void UpdateCountryLookup(CountryItem country)
    {
        Countries = Countries
            .Where(item => item.Id != country.Id)
            .Concat(country.IsActive
                ? [ToLookup(country)]
                : [])
            .OrderBy(item => item.Name)
            .ToArray();
    }

    private static GeographyLookupItem ToLookup(CountryItem country) =>
        new()
        {
            Id = country.Id,
            Code = country.Code,
            Name = country.Name,
            IsActive = country.IsActive
        };

    public Task<WarehouseItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return warehouseClient.GetByIdAsync(id, cancellationToken);
    }

    public override Task CreateAsync(SaveWarehouseRequest request, CancellationToken cancellationToken = default)
    {
        return warehouseClient.CreateAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveWarehouseRequest request, CancellationToken cancellationToken = default)
    {
        return warehouseClient.UpdateAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return warehouseClient.DeleteAsync(id, cancellationToken);
    }
}
