using NuanSystem.WinForms.Services.Definitions.Inventory.SalesChannels;
using NuanSystem.WinForms.Services.Definitions.Inventory.SalesChannels.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Definitions.Inventory.SalesChannels;

public sealed class SalesChannelsViewModel(ISalesChannelClient client) : CrudViewModel<SalesChannelItem, SaveSalesChannelRequest>
{
    public override Task LoadAsync(CancellationToken ct = default) => LoadItemsAsync(client.GetAsync, ct);
    public Task<SalesChannelItem> GetByIdAsync(int id, CancellationToken ct = default) => client.GetByIdAsync(id, ct);
    public Task<IReadOnlyCollection<SalesChannelAuditChange>> GetHistoryAsync(int id, CancellationToken ct = default) => client.GetHistoryAsync(id, ct);
    public override Task CreateAsync(SaveSalesChannelRequest request, CancellationToken ct = default) => client.CreateAsync(request, ct);
    public override Task UpdateAsync(int id, SaveSalesChannelRequest request, CancellationToken ct = default) => client.UpdateAsync(id, request, ct);
    public override Task DeleteAsync(int id, CancellationToken ct = default) => client.DeleteAsync(id, ct);
}


