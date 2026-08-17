using NuanSystem.WinForms.Services.Definitions.Inventory.ItemCommercialSegments;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemCommercialSegments.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Definitions.Inventory.ItemCommercialSegments;

public sealed class ItemCommercialSegmentsViewModel(IItemCommercialSegmentClient client) : CrudViewModel<ItemCommercialSegmentItem, SaveItemCommercialSegmentRequest>
{
    public override Task LoadAsync(CancellationToken ct = default) => LoadItemsAsync(client.GetAsync, ct);
    public Task<ItemCommercialSegmentItem> GetByIdAsync(int id, CancellationToken ct = default) => client.GetByIdAsync(id, ct);
    public Task<IReadOnlyCollection<ItemCommercialSegmentAuditChange>> GetHistoryAsync(int id, CancellationToken ct = default) => client.GetHistoryAsync(id, ct);
    public override Task CreateAsync(SaveItemCommercialSegmentRequest request, CancellationToken ct = default) => client.CreateAsync(request, ct);
    public override Task UpdateAsync(int id, SaveItemCommercialSegmentRequest request, CancellationToken ct = default) => client.UpdateAsync(id, request, ct);
    public override Task DeleteAsync(int id, CancellationToken ct = default) => client.DeleteAsync(id, ct);
}
