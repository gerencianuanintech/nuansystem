using NuanSystem.WinForms.Services.Definitions.Inventory.ItemTypes;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemTypes.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Definitions.Inventory.ItemTypes;

public sealed class ItemTypesViewModel : CrudViewModel<ItemTypeItem, SaveItemTypeRequest>
{
    private readonly IItemTypeClient client;

    public ItemTypesViewModel(IItemTypeClient client) => this.client = client;

    public override Task LoadAsync(CancellationToken cancellationToken = default) =>
        LoadItemsAsync(client.GetAsync, cancellationToken);

    public Task<ItemTypeItem> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        client.GetByIdAsync(id, cancellationToken);

    public Task<IReadOnlyCollection<ItemTypeAuditChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default) =>
        client.GetHistoryAsync(id, cancellationToken);

    public override Task CreateAsync(SaveItemTypeRequest request, CancellationToken cancellationToken = default) =>
        client.CreateAsync(request, cancellationToken);

    public override Task UpdateAsync(int id, SaveItemTypeRequest request, CancellationToken cancellationToken = default) =>
        client.UpdateAsync(id, request, cancellationToken);

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        client.DeleteAsync(id, cancellationToken);
}
