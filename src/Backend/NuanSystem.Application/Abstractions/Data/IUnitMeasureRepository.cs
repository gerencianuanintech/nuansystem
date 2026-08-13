using System.Data;
using NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IUnitMeasureRepository : IRepository
{
    Task<IReadOnlyCollection<UnitMeasureDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<UnitMeasureLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default);
    Task<UnitMeasureDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UnitMeasureDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<UnitMeasureAuditChangeDto>> GetHistoryAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateUnitMeasureData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> UpdateAsync(UpdateUnitMeasureData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> DeleteAsync(int id, int? auditUserId, string? auditUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
}
