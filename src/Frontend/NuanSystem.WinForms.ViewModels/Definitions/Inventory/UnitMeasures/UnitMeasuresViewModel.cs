using NuanSystem.WinForms.Services.Definitions.Inventory.UnitMeasures;
using NuanSystem.WinForms.Services.Definitions.Inventory.UnitMeasures.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Definitions.Inventory.UnitMeasures;

public sealed class UnitMeasuresViewModel(IUnitMeasureClient unitMeasureClient)
    : CrudViewModel<UnitMeasureItem, SaveUnitMeasureRequest>
{
    public override Task LoadAsync(CancellationToken cancellationToken = default) =>
        LoadItemsAsync(unitMeasureClient.GetAsync, cancellationToken);

    public Task<UnitMeasureItem> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        unitMeasureClient.GetByIdAsync(id, cancellationToken);

    public Task<IReadOnlyCollection<UnitMeasureAuditChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default) =>
        unitMeasureClient.GetHistoryAsync(id, cancellationToken);

    public override Task CreateAsync(SaveUnitMeasureRequest request, CancellationToken cancellationToken = default) =>
        unitMeasureClient.CreateAsync(request, cancellationToken);

    public override Task UpdateAsync(int id, SaveUnitMeasureRequest request, CancellationToken cancellationToken = default) =>
        unitMeasureClient.UpdateAsync(id, request, cancellationToken);

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        unitMeasureClient.DeleteAsync(id, cancellationToken);
}
