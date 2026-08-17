using System.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ItemCommercialSegments.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IItemCommercialSegmentRepository : IRepository
{
    Task<IReadOnlyCollection<ItemCommercialSegmentDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemCommercialSegmentLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default);
    Task<ItemCommercialSegmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ItemCommercialSegmentDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemCommercialSegmentAuditChangeDto>> GetHistoryAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateItemCommercialSegmentData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> UpdateAsync(UpdateItemCommercialSegmentData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> DeleteAsync(int id, int? auditUserId, string? auditUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
}
