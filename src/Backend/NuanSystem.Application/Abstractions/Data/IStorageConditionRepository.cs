using System.Data;
using NuanSystem.Application.Features.Definitions.Inventory.StorageConditions.Dtos;
namespace NuanSystem.Application.Abstractions.Data;
public interface IStorageConditionRepository : IRepository
{
    Task<IReadOnlyCollection<StorageConditionDto>> GetAllAsync(CancellationToken ct=default);
    Task<IReadOnlyCollection<StorageConditionLookupDto>> GetLookupAsync(string? includeCode=null,CancellationToken ct=default);
    Task<StorageConditionDto?> GetByCodeAsync(string code,IDbConnection connection,IDbTransaction transaction,CancellationToken ct=default);
    Task<StorageConditionDto?> GetByIdAsync(int id,CancellationToken ct=default);
    Task<StorageConditionDto?> GetByIdAsync(int id,IDbConnection connection,IDbTransaction transaction,CancellationToken ct=default);
    Task<IReadOnlyCollection<StorageConditionAuditChangeDto>> GetHistoryAsync(int id,CancellationToken ct=default);
    Task<bool> ExistsByCodeAsync(string code,int? excludingId,IDbConnection connection,IDbTransaction transaction,CancellationToken ct=default);
    Task<int> CreateAsync(CreateStorageConditionData data,IDbConnection connection,IDbTransaction transaction,CancellationToken ct=default);
    Task<int> UpdateAsync(UpdateStorageConditionData data,IDbConnection connection,IDbTransaction transaction,CancellationToken ct=default);
    Task<int> DeleteAsync(int id,int? auditUserId,string? auditUserName,IDbConnection connection,IDbTransaction transaction,CancellationToken ct=default);
}
