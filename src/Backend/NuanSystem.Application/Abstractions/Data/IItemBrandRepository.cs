using System.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ItemBrands.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IItemBrandRepository : IRepository
{
    Task<IReadOnlyCollection<ItemBrandDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemBrandLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default);
    Task<ItemBrandDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ItemBrandDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemBrandAuditChangeDto>> GetHistoryAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateItemBrandData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> UpdateAsync(UpdateItemBrandData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> DeleteAsync(int id, int? auditUserId, string? auditUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
}
