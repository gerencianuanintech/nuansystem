using NuanSystem.Application.Features.TaxCatalogs.Catalogs.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ITaxCatalogRepository : IRepository
{
    Task<IReadOnlyCollection<TaxCatalogDto>> GetAllAsync(string catalogKey, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<TaxCatalogLookupDto>> GetLookupAsync(string catalogKey, CancellationToken cancellationToken = default);

    Task<TaxCatalogDto?> GetByIdAsync(string catalogKey, int id, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(string catalogKey, CreateTaxCatalogData catalog, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string catalogKey, string code, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string catalogKey, string code, int excludingId, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(string catalogKey, UpdateTaxCatalogData catalog, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string catalogKey, int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RetentionConceptDto>> GetRetentionConceptsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RetentionConceptLookupDto>> GetRetentionConceptLookupAsync(CancellationToken cancellationToken = default);

    Task<RetentionConceptDto?> GetRetentionConceptByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> CreateRetentionConceptAsync(SaveRetentionConceptData concept, CancellationToken cancellationToken = default);

    Task<bool> UpdateRetentionConceptAsync(SaveRetentionConceptData concept, CancellationToken cancellationToken = default);
}
