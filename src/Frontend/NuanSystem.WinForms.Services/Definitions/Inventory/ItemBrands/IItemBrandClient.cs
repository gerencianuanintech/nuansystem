using NuanSystem.WinForms.Services.Definitions.Inventory.ItemBrands.Models;

namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemBrands;

public interface IItemBrandClient
{
    Task<IReadOnlyCollection<ItemBrandItem>> GetAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemBrandLookupItem>> GetLookupAsync(CancellationToken cancellationToken = default);
    Task<ItemBrandItem> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemBrandAuditChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default);
    Task<ItemBrandItem> CreateAsync(SaveItemBrandRequest request, CancellationToken cancellationToken = default);
    Task<ItemBrandItem> UpdateAsync(int id, SaveItemBrandRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
