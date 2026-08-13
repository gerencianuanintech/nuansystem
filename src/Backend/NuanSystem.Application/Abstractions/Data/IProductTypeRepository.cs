using System.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IProductTypeRepository : IRepository
{
    Task<IReadOnlyCollection<ProductTypeDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ProductTypeLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default);
    Task<ProductTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProductTypeDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ProductTypeAuditChangeDto>> GetHistoryAsync(int id,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, int? excludingId, IDbConnection connection,
        IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateProductTypeData data, IDbConnection connection, IDbTransaction transaction,
        CancellationToken cancellationToken = default);
    Task<int> UpdateAsync(UpdateProductTypeData data, IDbConnection connection, IDbTransaction transaction,
        CancellationToken cancellationToken = default);
    Task<int> DeleteAsync(int id, int? auditUserId, string? auditUserName, IDbConnection connection,
        IDbTransaction transaction, CancellationToken cancellationToken = default);
}
