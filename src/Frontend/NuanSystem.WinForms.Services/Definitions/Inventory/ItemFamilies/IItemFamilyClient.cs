using NuanSystem.WinForms.Services.Definitions.Inventory.ItemFamilies.Models;

namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemFamilies;

public interface IItemFamilyClient
{
    Task<IReadOnlyCollection<ItemFamilyItem>> GetAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemFamilyLookupItem>> GetLookupAsync(int? itemGroupId = null, CancellationToken cancellationToken = default);
    Task<ItemFamilyItem> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemFamilyAuditChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default);
    Task<ItemFamilyItem> CreateAsync(SaveItemFamilyRequest request, CancellationToken cancellationToken = default);
    Task<ItemFamilyItem> UpdateAsync(int id, SaveItemFamilyRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
