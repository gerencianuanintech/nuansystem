using NuanSystem.WinForms.Services.OperationalCatalogs.Models;

namespace NuanSystem.WinForms.Services.OperationalCatalogs;

public interface IOperationalCatalogClient
{
    Task<IReadOnlyCollection<OperationalCatalogItem>> GetAsync(
        string catalogKey,
        string? search = null,
        string? parentCatalogKey = null,
        string? parentCode = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<OperationalCatalogLookupItem>> GetLookupAsync(
        string catalogKey,
        string? parentCatalogKey = null,
        string? parentCode = null,
        bool activeOnly = true,
        CancellationToken cancellationToken = default);

    Task<OperationalCatalogItem> GetByIdAsync(string catalogKey, int id, CancellationToken cancellationToken = default);

    Task<OperationalCatalogItem> CreateAsync(string catalogKey, SaveOperationalCatalogRequest request, CancellationToken cancellationToken = default);

    Task<OperationalCatalogItem> UpdateAsync(string catalogKey, int id, SaveOperationalCatalogRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(string catalogKey, int id, CancellationToken cancellationToken = default);
}
