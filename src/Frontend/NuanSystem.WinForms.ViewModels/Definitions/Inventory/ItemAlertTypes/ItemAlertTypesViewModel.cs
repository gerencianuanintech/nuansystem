using NuanSystem.WinForms.Services.Definitions.Inventory.ItemAlertTypes;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemAlertTypes.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Definitions.Inventory.ItemAlertTypes;

public sealed class ItemAlertTypesViewModel(IItemAlertTypeClient client) : CrudViewModel<ItemAlertTypeItem, SaveItemAlertTypeRequest>
{
    public override Task LoadAsync(CancellationToken ct = default) => LoadItemsAsync(client.GetAsync, ct);
    public Task<ItemAlertTypeItem> GetByIdAsync(int id, CancellationToken ct = default) => client.GetByIdAsync(id, ct);
    public Task<IReadOnlyCollection<ItemAlertTypeAuditChange>> GetHistoryAsync(int id, CancellationToken ct = default) => client.GetHistoryAsync(id, ct);
    public override Task CreateAsync(SaveItemAlertTypeRequest request, CancellationToken ct = default) => client.CreateAsync(request, ct);
    public override Task UpdateAsync(int id, SaveItemAlertTypeRequest request, CancellationToken ct = default) => client.UpdateAsync(id, request, ct);
    public override Task DeleteAsync(int id, CancellationToken ct = default) => client.DeleteAsync(id, ct);
}

