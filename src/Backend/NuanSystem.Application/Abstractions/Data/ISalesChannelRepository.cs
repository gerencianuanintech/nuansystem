using System.Data;
using NuanSystem.Application.Features.Definitions.Inventory.SalesChannels.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ISalesChannelRepository : IRepository
{
    Task<IReadOnlyCollection<SalesChannelDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SalesChannelLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default);
    Task<SalesChannelDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SalesChannelDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SalesChannelAuditChangeDto>> GetHistoryAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateSalesChannelData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> UpdateAsync(UpdateSalesChannelData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> DeleteAsync(int id, int? auditUserId, string? auditUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
}


