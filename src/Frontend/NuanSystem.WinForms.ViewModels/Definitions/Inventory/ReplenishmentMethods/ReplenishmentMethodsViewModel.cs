using NuanSystem.WinForms.Services.Definitions.Inventory.ReplenishmentMethods;
using NuanSystem.WinForms.Services.Definitions.Inventory.ReplenishmentMethods.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Definitions.Inventory.ReplenishmentMethods;

public sealed class ReplenishmentMethodsViewModel(IReplenishmentMethodClient client)
    : CrudViewModel<ReplenishmentMethodItem, SaveReplenishmentMethodRequest>
{
    public override Task LoadAsync(CancellationToken ct = default) => LoadItemsAsync(client.GetAsync, ct);
    public Task<ReplenishmentMethodItem> GetByIdAsync(int id, CancellationToken ct = default) => client.GetByIdAsync(id, ct);
    public Task<IReadOnlyCollection<ReplenishmentMethodAuditChange>> GetHistoryAsync(int id, CancellationToken ct = default) => client.GetHistoryAsync(id, ct);
    public Task<ReplenishmentMethodItem> CreateAndReturnAsync(SaveReplenishmentMethodRequest request, CancellationToken ct = default) => client.CreateAsync(request, ct);
    public Task<ReplenishmentMethodItem> UpdateAndReturnAsync(int id, SaveReplenishmentMethodRequest request, CancellationToken ct = default) => client.UpdateAsync(id, request, ct);
    public override async Task CreateAsync(SaveReplenishmentMethodRequest request, CancellationToken ct = default) => await CreateAndReturnAsync(request, ct);
    public override async Task UpdateAsync(int id, SaveReplenishmentMethodRequest request, CancellationToken ct = default) => await UpdateAndReturnAsync(id, request, ct);
    public override Task DeleteAsync(int id, CancellationToken ct = default) => client.DeleteAsync(id, ct);
}
