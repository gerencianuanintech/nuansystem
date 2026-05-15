using NuanSystem.WinForms.Services.GeneralInventory.ItemFamilies;
using NuanSystem.WinForms.Services.GeneralInventory.ItemFamilies.Models;
using NuanSystem.WinForms.Services.InventoryItems;
using NuanSystem.WinForms.Services.InventoryItems.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.GeneralInventory.ItemFamilies;

public sealed class ItemFamiliesViewModel(IItemFamilyClient itemFamilyClient, IItemClient itemClient)
    : CrudViewModel<ItemFamilyItem, SaveItemFamilyRequest>
{
    public ItemLookups Lookups { get; private set; } = new([], [], [], [], []);

    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(itemFamilyClient.GetAsync, cancellationToken);
    }

    public async Task LoadLookupsAsync(CancellationToken cancellationToken = default)
    {
        var lookups = await itemClient.GetLookupsAsync(cancellationToken);
        Lookups = lookups with
        {
            ItemFamilies = []
        };
    }

    public Task<ItemFamilyItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return itemFamilyClient.GetByIdAsync(id, cancellationToken);
    }

    public override Task CreateAsync(SaveItemFamilyRequest request, CancellationToken cancellationToken = default)
    {
        return itemFamilyClient.CreateAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveItemFamilyRequest request, CancellationToken cancellationToken = default)
    {
        return itemFamilyClient.UpdateAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return itemFamilyClient.DeleteAsync(id, cancellationToken);
    }
}
