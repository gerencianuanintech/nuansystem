using System.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ReplenishmentMethods.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IReplenishmentMethodRepository : IRepository
{
    Task<IReadOnlyCollection<ReplenishmentMethodDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ReplenishmentMethodLookupDto>> GetLookupAsync(string? includeCode = null, CancellationToken cancellationToken = default);
    Task<ReplenishmentMethodDto?> GetByCodeAsync(string code, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<ReplenishmentMethodDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ReplenishmentMethodDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ReplenishmentMethodAuditChangeDto>> GetHistoryAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateReplenishmentMethodData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> UpdateAsync(UpdateReplenishmentMethodData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> DeleteAsync(int id, int? auditUserId, string? auditUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
}
