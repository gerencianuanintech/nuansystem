using NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IFinancialCatalogRepository : IRepository
{
    Task<IReadOnlyCollection<FinancialCatalogDto>> GetAllAsync(string catalogKey, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<FinancialCatalogLookupDto>> GetLookupAsync(string catalogKey, CancellationToken cancellationToken = default);

    Task<FinancialCatalogDto?> GetByIdAsync(string catalogKey, int id, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(string catalogKey, CreateFinancialCatalogData catalog, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string catalogKey, string code, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string catalogKey, string code, int excludingId, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(string catalogKey, UpdateFinancialCatalogData catalog, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string catalogKey, int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default);
}
