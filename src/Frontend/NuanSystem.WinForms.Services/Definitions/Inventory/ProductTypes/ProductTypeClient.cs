using NuanSystem.WinForms.Services.Definitions.Inventory.ProductTypes.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.Definitions.Inventory.ProductTypes;

public sealed class ProductTypeClient(INuanApiClient apiClient) : IProductTypeClient
{
    private const string Route = "/api/definitions/inventory/product-types";

    public async Task<IReadOnlyCollection<ProductTypeItem>> GetAsync(CancellationToken cancellationToken = default) =>
        await apiClient.GetAsync<List<ProductTypeItem>>(Route, cancellationToken);

    public async Task<IReadOnlyCollection<ProductTypeLookupItem>> GetLookupAsync(CancellationToken cancellationToken = default) =>
        await apiClient.GetAsync<List<ProductTypeLookupItem>>($"{Route}/lookup", cancellationToken);

    public Task<ProductTypeItem> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        apiClient.GetAsync<ProductTypeItem>($"{Route}/{id}", cancellationToken);

    public async Task<IReadOnlyCollection<ProductTypeAuditChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default) =>
        await apiClient.GetAsync<List<ProductTypeAuditChange>>($"{Route}/{id}/history", cancellationToken);

    public Task<ProductTypeItem> CreateAsync(SaveProductTypeRequest request, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync<SaveProductTypeRequest, ProductTypeItem>(Route, request, cancellationToken);

    public Task<ProductTypeItem> UpdateAsync(int id, SaveProductTypeRequest request, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync<SaveProductTypeRequest, ProductTypeItem>($"{Route}/{id}", request, cancellationToken);

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        await apiClient.DeleteAsync<object>($"{Route}/{id}", cancellationToken);
}
