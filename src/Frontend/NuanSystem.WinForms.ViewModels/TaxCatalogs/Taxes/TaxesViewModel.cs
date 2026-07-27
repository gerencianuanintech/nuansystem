using NuanSystem.WinForms.Services.TaxCatalogs.Taxes;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.TaxCatalogs.Taxes;

public sealed class TaxesViewModel(ITaxClient client) : CrudViewModel<TaxItem, SaveTaxRequest>
{
    public override Task LoadAsync(CancellationToken cancellationToken = default) =>
        LoadItemsAsync(client.GetAsync, cancellationToken);
    public Task<TaxItem> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        client.GetByIdAsync(id, cancellationToken);
    public override Task CreateAsync(SaveTaxRequest request, CancellationToken cancellationToken = default) =>
        client.CreateAsync(request, cancellationToken);
    public override Task UpdateAsync(int id, SaveTaxRequest request, CancellationToken cancellationToken = default) =>
        client.UpdateAsync(id, request, cancellationToken);
    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        client.DeleteAsync(id, cancellationToken);
    public Task<IReadOnlyCollection<TaxAuditChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default) =>
        client.GetHistoryAsync(id, cancellationToken);
}
