using NuanSystem.WinForms.Services.Definitions.Inventory.ItemLines;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemLines.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Definitions.Inventory.ItemLines;

public sealed class ItemLinesViewModel(IItemLineClient client)
    : CrudViewModel<ItemLineItem, SaveItemLineRequest>
{
    public override Task LoadAsync(CancellationToken cancellationToken = default) =>
        LoadItemsAsync(client.GetAsync, cancellationToken);

    public Task<ItemLineItem> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        client.GetByIdAsync(id, cancellationToken);

    public Task<IReadOnlyCollection<ItemLineAuditChange>> GetHistoryAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        client.GetHistoryAsync(id, cancellationToken);

    public override Task CreateAsync(SaveItemLineRequest request, CancellationToken cancellationToken = default) =>
        client.CreateAsync(request, cancellationToken);

    public override Task UpdateAsync(int id, SaveItemLineRequest request, CancellationToken cancellationToken = default) =>
        client.UpdateAsync(id, request, cancellationToken);

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        client.DeleteAsync(id, cancellationToken);
}
