using NuanSystem.WinForms.Services.GeneralInventory.ItemFamilies.Models;

namespace NuanSystem.WinForms.Services.GeneralInventory.ItemFamilies;

public interface IItemFamilyClient
{
    Task<IReadOnlyCollection<ItemFamilyItem>> GetAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ItemFamilyItem>> GetByGroupAsync(int itemGroupId, CancellationToken cancellationToken = default);

    Task<ItemFamilyItem> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ItemFamilyItem> CreateAsync(SaveItemFamilyRequest request, CancellationToken cancellationToken = default);

    Task<ItemFamilyItem> UpdateAsync(int id, SaveItemFamilyRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
