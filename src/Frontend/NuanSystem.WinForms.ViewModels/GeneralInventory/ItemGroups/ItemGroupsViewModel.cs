using NuanSystem.WinForms.Services.GeneralInventory.ItemGroups;
using NuanSystem.WinForms.Services.GeneralInventory.ItemGroups.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.GeneralInventory.ItemGroups;

public sealed class ItemGroupsViewModel : CrudViewModel<ItemGroupItem, SaveItemGroupRequest>
{
    private readonly IItemGroupClient itemGroupClient;

    public ItemGroupsViewModel(IItemGroupClient itemGroupClient)
    {
        this.itemGroupClient = itemGroupClient;
    }

    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(itemGroupClient.GetAsync, cancellationToken);
    }

    public Task<ItemGroupItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return itemGroupClient.GetByIdAsync(id, cancellationToken);
    }

    public override Task CreateAsync(SaveItemGroupRequest request, CancellationToken cancellationToken = default)
    {
        return itemGroupClient.CreateAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveItemGroupRequest request, CancellationToken cancellationToken = default)
    {
        return itemGroupClient.UpdateAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return itemGroupClient.DeleteAsync(id, cancellationToken);
    }
}
