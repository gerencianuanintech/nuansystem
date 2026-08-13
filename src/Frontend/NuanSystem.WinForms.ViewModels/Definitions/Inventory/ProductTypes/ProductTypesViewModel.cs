using NuanSystem.WinForms.Services.Definitions.Inventory.ProductTypes;
using NuanSystem.WinForms.Services.Definitions.Inventory.ProductTypes.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Definitions.Inventory.ProductTypes;

public sealed class ProductTypesViewModel(IProductTypeClient client)
    : CrudViewModel<ProductTypeItem, SaveProductTypeRequest>
{
    public override Task LoadAsync(CancellationToken cancellationToken = default) =>
        LoadItemsAsync(client.GetAsync, cancellationToken);

    public Task<ProductTypeItem> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        client.GetByIdAsync(id, cancellationToken);

    public Task<IReadOnlyCollection<ProductTypeAuditChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default) =>
        client.GetHistoryAsync(id, cancellationToken);

    public override Task CreateAsync(SaveProductTypeRequest request, CancellationToken cancellationToken = default) =>
        client.CreateAsync(request, cancellationToken);

    public override Task UpdateAsync(int id, SaveProductTypeRequest request, CancellationToken cancellationToken = default) =>
        client.UpdateAsync(id, request, cancellationToken);

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        client.DeleteAsync(id, cancellationToken);
}
