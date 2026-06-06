using NuanSystem.WinForms.Services.Purchasing.PurchaseOrders;
using NuanSystem.WinForms.Services.Purchasing.PurchaseOrders.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Purchasing.PurchaseOrders;

public sealed class PurchaseOrdersViewModel(IPurchaseOrderClient client)
    : CrudViewModel<PurchaseOrderItem, SavePurchaseOrderRequest>
{
    public PurchaseOrderLookups Lookups { get; private set; } = EmptyLookups();

    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(client.GetAsync, cancellationToken);
    }

    public async Task LoadLookupsAsync(string actionKey, CancellationToken cancellationToken = default)
    {
        Lookups = await client.GetLookupsAsync(actionKey, cancellationToken);
    }

    public Task<IReadOnlyCollection<PurchaseOrderFieldAccess>> GetFieldAccessAsync(int seriesId, CancellationToken cancellationToken = default)
    {
        return client.GetFieldAccessAsync(seriesId, cancellationToken);
    }

    public Task<PurchaseOrderDetail> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return client.GetByIdAsync(id, cancellationToken);
    }

    public override Task CreateAsync(SavePurchaseOrderRequest request, CancellationToken cancellationToken = default)
    {
        return client.CreateAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SavePurchaseOrderRequest request, CancellationToken cancellationToken = default)
    {
        return client.UpdateAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return client.DeleteAsync(id, cancellationToken);
    }

    public Task<PurchaseOrderDetail> SyncSapAsync(int id, CancellationToken cancellationToken = default)
    {
        return client.SyncSapAsync(id, cancellationToken);
    }

    private static PurchaseOrderLookups EmptyLookups()
    {
        return new PurchaseOrderLookups([], [], [], [], [], [], [], [], [], [], [], [], []);
    }
}
