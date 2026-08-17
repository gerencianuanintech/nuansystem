using NuanSystem.WinForms.Services.Definitions.Inventory.StorageConditions;
using NuanSystem.WinForms.Services.Definitions.Inventory.StorageConditions.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Definitions.Inventory.StorageConditions;

public sealed class StorageConditionsViewModel(IStorageConditionClient client) : CrudViewModel<StorageConditionItem, SaveStorageConditionRequest>
{
    public override Task LoadAsync(CancellationToken ct = default) => LoadItemsAsync(client.GetAsync, ct);
    public Task<StorageConditionItem> GetByIdAsync(int id, CancellationToken ct = default) => client.GetByIdAsync(id, ct);
    public Task<IReadOnlyCollection<StorageConditionAuditChange>> GetHistoryAsync(int id, CancellationToken ct = default) => client.GetHistoryAsync(id, ct);
    public Task<StorageConditionItem> CreateAndReturnAsync(SaveStorageConditionRequest request, CancellationToken ct = default) => client.CreateAsync(request, ct);
    public Task<StorageConditionItem> UpdateAndReturnAsync(int id, SaveStorageConditionRequest request, CancellationToken ct = default) => client.UpdateAsync(id, request, ct);
    public override async Task CreateAsync(SaveStorageConditionRequest request, CancellationToken ct = default) => await CreateAndReturnAsync(request, ct);
    public override async Task UpdateAsync(int id, SaveStorageConditionRequest request, CancellationToken ct = default) => await UpdateAndReturnAsync(id, request, ct);
    public override Task DeleteAsync(int id, CancellationToken ct = default) => client.DeleteAsync(id, ct);
}
