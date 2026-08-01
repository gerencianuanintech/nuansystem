using System.Data;
using NuanSystem.Application.Features.Carriers.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ICarrierRepository : IRepository
{
    Task<IReadOnlyCollection<CarrierListItemDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CarrierLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default);
    Task<CarrierDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CarrierDetailDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CarrierAuditChangeDto>> GetHistoryAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, int? excludingId = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<CreateCarrierResult> CreateAsync(CreateCarrierData data, CancellationToken cancellationToken = default);
    Task<CreateCarrierResult> CreateAsync(CreateCarrierData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<UpdateCarrierResult> UpdateAsync(UpdateCarrierData data, CancellationToken cancellationToken = default);
    Task<UpdateCarrierResult> UpdateAsync(UpdateCarrierData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(DeleteCarrierData data, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(DeleteCarrierData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
}
