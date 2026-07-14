using NuanSystem.WinForms.Services.GeneralInventory.Warehouses;
using NuanSystem.WinForms.Services.GeneralInventory.Warehouses.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.GeneralInventory.Warehouses;

public sealed class WarehousesViewModel(IWarehouseClient warehouseClient)
    : CrudViewModel<WarehouseItem, SaveWarehouseRequest>
{
    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(warehouseClient.GetAsync, cancellationToken);
    }

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
        return warehouseClient.DeactivateAsync(id, cancellationToken);
    }
}
