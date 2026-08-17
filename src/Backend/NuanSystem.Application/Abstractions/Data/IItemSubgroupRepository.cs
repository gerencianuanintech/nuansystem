using System.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ItemSubgroups.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IItemSubgroupRepository : IRepository
{
    Task<IReadOnlyCollection<ItemSubgroupDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemSubgroupLookupDto>> GetLookupAsync(int? itemFamilyId, CancellationToken cancellationToken = default);
    Task<ItemSubgroupDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ItemSubgroupDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemSubgroupAuditChangeDto>> GetHistoryAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(int itemFamilyId, string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<bool> ExistsActiveByFamilyAndCodeAsync(int itemFamilyId, string code, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateItemSubgroupData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> UpdateWithResultAsync(UpdateItemSubgroupData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> DeleteWithResultAsync(int id, int? auditUserId, string? auditUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
}
