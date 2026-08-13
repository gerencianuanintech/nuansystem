using NuanSystem.WinForms.Services.Definitions.Inventory.ItemLines.Models;

namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemLines;

public interface IItemLineClient
{
    Task<IReadOnlyCollection<ItemLineItem>> GetAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemLineLookupItem>> GetLookupAsync(CancellationToken cancellationToken = default);
    Task<ItemLineItem> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemLineAuditChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default);
    Task<ItemLineItem> CreateAsync(SaveItemLineRequest request, CancellationToken cancellationToken = default);
    Task<ItemLineItem> UpdateAsync(int id, SaveItemLineRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
