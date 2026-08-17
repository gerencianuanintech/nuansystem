using System.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ItemOrigins.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IItemOriginRepository : IRepository
{
    Task<IReadOnlyCollection<ItemOriginDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemOriginLookupDto>> GetLookupAsync(string? includeCode = null, CancellationToken cancellationToken = default);
    Task<ItemOriginDto?> GetByCodeAsync(string code, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<ItemOriginDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ItemOriginDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemOriginAuditChangeDto>> GetHistoryAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateItemOriginData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> UpdateAsync(UpdateItemOriginData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> DeleteAsync(int id, int? auditUserId, string? auditUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
}
