using NuanSystem.WinForms.Services.Definitions.Inventory.ItemOrigins;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemOrigins.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Definitions.Inventory.ItemOrigins;

public sealed class ItemOriginsViewModel(IItemOriginClient client) : CrudViewModel<ItemOriginItem, SaveItemOriginRequest>
{
    public override Task LoadAsync(CancellationToken ct = default) => LoadItemsAsync(client.GetAsync, ct);
    public Task<ItemOriginItem> GetByIdAsync(int id, CancellationToken ct = default) => client.GetByIdAsync(id, ct);
    public Task<IReadOnlyCollection<ItemOriginAuditChange>> GetHistoryAsync(int id, CancellationToken ct = default) => client.GetHistoryAsync(id, ct);
    public Task<ItemOriginItem> CreateAndReturnAsync(SaveItemOriginRequest request, CancellationToken ct = default) => client.CreateAsync(request, ct);
    public Task<ItemOriginItem> UpdateAndReturnAsync(int id, SaveItemOriginRequest request, CancellationToken ct = default) => client.UpdateAsync(id, request, ct);
    public override async Task CreateAsync(SaveItemOriginRequest request, CancellationToken ct = default) => await CreateAndReturnAsync(request, ct);
    public override async Task UpdateAsync(int id, SaveItemOriginRequest request, CancellationToken ct = default) => await UpdateAndReturnAsync(id, request, ct);
    public override Task DeleteAsync(int id, CancellationToken ct = default) => client.DeleteAsync(id, ct);
}
