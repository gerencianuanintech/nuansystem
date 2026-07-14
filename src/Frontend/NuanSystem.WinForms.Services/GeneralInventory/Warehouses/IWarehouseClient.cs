using NuanSystem.WinForms.Services.GeneralInventory.Warehouses.Models;

namespace NuanSystem.WinForms.Services.GeneralInventory.Warehouses;

public interface IWarehouseClient
{
    Task<IReadOnlyCollection<WarehouseItem>> GetAsync(CancellationToken cancellationToken = default);

    Task<WarehouseItem> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<WarehouseItem> CreateAsync(SaveWarehouseRequest request, CancellationToken cancellationToken = default);

    Task<WarehouseItem> UpdateAsync(int id, SaveWarehouseRequest request, CancellationToken cancellationToken = default);

    Task DeactivateAsync(int id, CancellationToken cancellationToken = default);
}
