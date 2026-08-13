using System.Data;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IItemGroupRepository : IRepository
{
    Task<IReadOnlyCollection<ItemGroupDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemGroupLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemGroupAuditChangeDto>> GetHistoryAsync(int id, CancellationToken cancellationToken = default);

    Task<ItemGroupDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ItemGroupDto?> GetByIdAsync(
        int id,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateItemGroupData itemGroup, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(
        CreateItemGroupData itemGroup,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, int excludingId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string code,
        int? excludingId,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UpdateItemGroupData itemGroup, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        UpdateItemGroupData itemGroup,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);
    Task<int> UpdateWithResultAsync(UpdateItemGroupData itemGroup, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int id,
        int? deletedByUserId,
        string? deletedByUserName,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);
    Task<int> DeleteWithResultAsync(int id, int? deletedByUserId, string? deletedByUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
}
