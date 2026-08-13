using NuanSystem.WinForms.Services.Definitions.Inventory.ProductTypes.Models;

namespace NuanSystem.WinForms.Services.Definitions.Inventory.ProductTypes;

public interface IProductTypeClient
{
    Task<IReadOnlyCollection<ProductTypeItem>> GetAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ProductTypeLookupItem>> GetLookupAsync(CancellationToken cancellationToken = default);
    Task<ProductTypeItem> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ProductTypeAuditChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default);
    Task<ProductTypeItem> CreateAsync(SaveProductTypeRequest request, CancellationToken cancellationToken = default);
    Task<ProductTypeItem> UpdateAsync(int id, SaveProductTypeRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
