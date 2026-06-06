using NuanSystem.Application.Features.OperationalCatalogs.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IOperationalCatalogRepository
{
    Task<IReadOnlyCollection<OperationalCatalogDto>> GetAllAsync(OperationalCatalogFilterData filter, CancellationToken cancellationToken = default);

    Task<OperationalCatalogDto?> GetByIdAsync(string catalogKey, int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<OperationalCatalogLookupDto>> GetLookupAsync(string catalogKey, string? parentCatalogKey, string? parentCode, bool activeOnly = true, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string catalogKey, string code, int? excludedId = null, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateOperationalCatalogData data, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UpdateOperationalCatalogData data, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string catalogKey, int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default);
}
