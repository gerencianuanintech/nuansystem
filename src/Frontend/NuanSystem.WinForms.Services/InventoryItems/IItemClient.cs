using NuanSystem.WinForms.Services.InventoryItems.Models;

namespace NuanSystem.WinForms.Services.InventoryItems;

public interface IItemClient
{
    Task<IReadOnlyCollection<ItemItem>> GetAsync(CancellationToken cancellationToken = default);
    Task<ItemItem> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ItemLookups> GetLookupsAsync(CancellationToken cancellationToken = default);
    Task<ItemItem> CreateAsync(SaveItemRequest request, CancellationToken cancellationToken = default);
    Task<ItemItem> UpdateAsync(int id, SaveItemRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
