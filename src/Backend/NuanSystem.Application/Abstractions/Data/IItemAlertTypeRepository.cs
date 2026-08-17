using System.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ItemAlertTypes.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IItemAlertTypeRepository : IRepository
{
    Task<IReadOnlyCollection<ItemAlertTypeDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemAlertTypeLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default);
    Task<ItemAlertTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ItemAlertTypeDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemAlertTypeAuditChangeDto>> GetHistoryAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateItemAlertTypeData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> UpdateAsync(UpdateItemAlertTypeData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> DeleteAsync(int id, int? auditUserId, string? auditUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
}

