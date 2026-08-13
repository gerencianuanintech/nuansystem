using NuanSystem.WinForms.Services.Definitions.Inventory.ItemBrands;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemBrands.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Definitions.Inventory.ItemBrands;

public sealed class ItemBrandsViewModel(IItemBrandClient itemBrandClient)
    : CrudViewModel<ItemBrandItem, SaveItemBrandRequest>
{
    public override Task LoadAsync(CancellationToken cancellationToken = default) =>
        LoadItemsAsync(itemBrandClient.GetAsync, cancellationToken);

    public Task<ItemBrandItem> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        itemBrandClient.GetByIdAsync(id, cancellationToken);

    public Task<IReadOnlyCollection<ItemBrandAuditChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default) =>
        itemBrandClient.GetHistoryAsync(id, cancellationToken);

    public override Task CreateAsync(SaveItemBrandRequest request, CancellationToken cancellationToken = default) =>
        itemBrandClient.CreateAsync(request, cancellationToken);

    public override Task UpdateAsync(int id, SaveItemBrandRequest request, CancellationToken cancellationToken = default) =>
        itemBrandClient.UpdateAsync(id, request, cancellationToken);

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        itemBrandClient.DeleteAsync(id, cancellationToken);
}
