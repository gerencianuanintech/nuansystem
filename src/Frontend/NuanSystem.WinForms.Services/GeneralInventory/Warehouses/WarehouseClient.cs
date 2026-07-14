using NuanSystem.WinForms.Services.GeneralInventory.Warehouses.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.GeneralInventory.Warehouses;

public sealed class WarehouseClient(INuanApiClient apiClient) : IWarehouseClient
{
    public async Task<IReadOnlyCollection<WarehouseItem>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<WarehouseItem>>("/api/warehouses", cancellationToken);
    }

    public Task<WarehouseItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<WarehouseItem>($"/api/warehouses/{id}", cancellationToken);
    }

    public Task<WarehouseItem> CreateAsync(SaveWarehouseRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveWarehouseRequest, WarehouseItem>("/api/warehouses", request, cancellationToken);
    }

    public Task<WarehouseItem> UpdateAsync(int id, SaveWarehouseRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveWarehouseRequest, WarehouseItem>($"/api/warehouses/{id}", request, cancellationToken);
    }

    public async Task DeactivateAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"/api/warehouses/{id}", cancellationToken);
    }
}
