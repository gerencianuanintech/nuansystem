using NuanSystem.WinForms.Services.Definitions.Inventory.ItemTypes.Models;

namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemTypes;

public interface IItemTypeClient
{
    Task<IReadOnlyCollection<ItemTypeItem>> GetAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemTypeLookupItem>> GetLookupAsync(CancellationToken cancellationToken = default);
    Task<ItemTypeItem> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemTypeAuditChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default);
    Task<ItemTypeItem> CreateAsync(SaveItemTypeRequest request, CancellationToken cancellationToken = default);
    Task<ItemTypeItem> UpdateAsync(int id, SaveItemTypeRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
