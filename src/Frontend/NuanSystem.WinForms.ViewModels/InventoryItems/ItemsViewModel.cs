using NuanSystem.WinForms.Services.InventoryItems;
using NuanSystem.WinForms.Services.InventoryItems.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.InventoryItems;

public sealed class ItemsViewModel : CrudViewModel<ItemItem, SaveItemRequest>
{
    private readonly IItemClient itemClient;

    public ItemsViewModel(IItemClient itemClient)
    {
        this.itemClient = itemClient;
    }

    public ItemLookups Lookups { get; private set; } = new([], [], [], []);

    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(itemClient.GetAsync, cancellationToken);
    }

    public async Task LoadLookupsAsync(CancellationToken cancellationToken = default)
    {
        Lookups = await itemClient.GetLookupsAsync(cancellationToken);
    }

    public Task<ItemItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return itemClient.GetByIdAsync(id, cancellationToken);
    }

    public override Task CreateAsync(SaveItemRequest request, CancellationToken cancellationToken = default)
    {
        return itemClient.CreateAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveItemRequest request, CancellationToken cancellationToken = default)
    {
        return itemClient.UpdateAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return itemClient.DeleteAsync(id, cancellationToken);
    }
}
