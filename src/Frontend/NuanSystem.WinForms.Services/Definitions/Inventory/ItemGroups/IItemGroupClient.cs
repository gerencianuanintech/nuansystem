using NuanSystem.WinForms.Services.Definitions.Inventory.ItemGroups.Models;

namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemGroups;

public interface IItemGroupClient
{
    Task<IReadOnlyCollection<ItemGroupItem>> GetAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemGroupLookupItem>> GetLookupAsync(CancellationToken cancellationToken = default);
    Task<ItemGroupItem> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ItemGroupItem> CreateAsync(SaveItemGroupRequest request, CancellationToken cancellationToken = default);
    Task<ItemGroupItem> UpdateAsync(int id, SaveItemGroupRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
