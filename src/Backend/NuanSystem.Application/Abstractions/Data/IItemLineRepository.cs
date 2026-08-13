using System.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ItemLines.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IItemLineRepository : IRepository
{
    Task<IReadOnlyCollection<ItemLineDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemLineLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default);
    Task<ItemLineDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ItemLineDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemLineAuditChangeDto>> GetHistoryAsync(int id,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, int? excludingId, IDbConnection connection,
        IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateItemLineData data, IDbConnection connection, IDbTransaction transaction,
        CancellationToken cancellationToken = default);
    Task<int> UpdateAsync(UpdateItemLineData data, IDbConnection connection, IDbTransaction transaction,
        CancellationToken cancellationToken = default);
    Task<int> DeleteAsync(int id, int? auditUserId, string? auditUserName, IDbConnection connection,
        IDbTransaction transaction, CancellationToken cancellationToken = default);
}
