using System.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ItemFamilies.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IItemFamilyRepository : IRepository
{
    Task<IReadOnlyCollection<ItemFamilyDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ItemFamilyLookupDto>> GetLookupAsync(int? itemGroupId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ItemFamilyAuditChangeDto>> GetHistoryAsync(int id, CancellationToken cancellationToken = default);

    Task<ItemFamilyDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ItemFamilyDto?> GetByIdAsync(
        int id,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateItemFamilyData itemFamily, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(
        CreateItemFamilyData itemFamily,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(int itemGroupId, string code, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(int itemGroupId, string code, int excludingId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        int itemGroupId,
        string code,
        int? excludingId,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UpdateItemFamilyData itemFamily, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        UpdateItemFamilyData itemFamily,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);

    Task<int> UpdateWithResultAsync(
        UpdateItemFamilyData itemFamily,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int id,
        int? deletedByUserId,
        string? deletedByUserName,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);

    Task<int> DeleteWithResultAsync(
        int id,
        int? deletedByUserId,
        string? deletedByUserName,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);
}
