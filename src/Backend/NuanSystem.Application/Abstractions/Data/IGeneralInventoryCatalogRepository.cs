using NuanSystem.Application.Features.GeneralInventory.Catalogs.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IGeneralInventoryCatalogRepository : IRepository
{
    Task<IReadOnlyCollection<GeneralInventoryCatalogDto>> GetAllAsync(
        string catalogKey,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<GeneralInventoryCatalogLookupDto>> GetLookupAsync(
        string catalogKey,
        CancellationToken cancellationToken = default);

    Task<GeneralInventoryCatalogDto?> GetByIdAsync(
        string catalogKey,
        int id,
        CancellationToken cancellationToken = default);

    Task<int> CreateAsync(
        string catalogKey,
        CreateGeneralInventoryCatalogData catalog,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string catalogKey,
        string code,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string catalogKey,
        string code,
        int excludingId,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        string catalogKey,
        UpdateGeneralInventoryCatalogData catalog,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        string catalogKey,
        int id,
        int? deletedByUserId,
        string? deletedByUserName,
        CancellationToken cancellationToken = default);
}
