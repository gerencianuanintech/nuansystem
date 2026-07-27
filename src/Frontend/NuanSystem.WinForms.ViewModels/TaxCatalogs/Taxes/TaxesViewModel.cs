using NuanSystem.WinForms.Services.TaxCatalogs.Taxes;
using NuanSystem.WinForms.Services.Audit;
using NuanSystem.WinForms.Services.Audit.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.TaxCatalogs.Taxes;

public sealed class TaxesViewModel(ITaxClient client, IAuditClient auditClient) : CrudViewModel<TaxItem, SaveTaxRequest>
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
    public Task<IReadOnlyCollection<SecurityChangeItem>> GetHistoryAsync(int id, CancellationToken cancellationToken = default) =>
        auditClient.GetInventoryChangesAsync("Taxes", id.ToString(), cancellationToken: cancellationToken);
}
