using NuanSystem.WinForms.Services.Definitions.General.Common;
using NuanSystem.WinForms.Services.GeneralInventory.Warehouses;
using NuanSystem.WinForms.Services.GeneralInventory.Warehouses.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.GeneralInventory.Warehouses;

public sealed class WarehousesViewModel(IWarehouseClient warehouseClient, IGeographyClient geographyClient)
    : CrudViewModel<WarehouseItem, SaveWarehouseRequest>
{
    public IReadOnlyCollection<GeographyLookupItem> Countries { get; private set; } = [];

    public override async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        var countriesTask = geographyClient.GetCountryLookupAsync(cancellationToken);
        var warehousesTask = LoadItemsAsync(warehouseClient.GetAsync, cancellationToken);
        await Task.WhenAll(countriesTask, warehousesTask);
        Countries = await countriesTask;
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
