using NuanSystem.Application.Features.GeneralSupplier.Catalogs.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IGeneralSupplierCatalogRepository : IRepository
{
    Task<IReadOnlyCollection<GeneralSupplierCatalogDto>> GetAllAsync(
        string catalogKey,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<GeneralSupplierCatalogLookupDto>> GetLookupAsync(
        string catalogKey,
        CancellationToken cancellationToken = default);

    Task<GeneralSupplierCatalogDto?> GetByIdAsync(
        string catalogKey,
        int id,
        CancellationToken cancellationToken = default);

    Task<int> CreateAsync(
        string catalogKey,
        CreateGeneralSupplierCatalogData catalog,
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
        UpdateGeneralSupplierCatalogData catalog,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        string catalogKey,
        int id,
        int? deletedByUserId,
        string? deletedByUserName,
        CancellationToken cancellationToken = default);
}

