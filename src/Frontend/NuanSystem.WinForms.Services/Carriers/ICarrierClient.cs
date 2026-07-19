using NuanSystem.WinForms.Services.Carriers.Models;

namespace NuanSystem.WinForms.Services.Carriers;

public interface ICarrierClient
{
    Task<IReadOnlyCollection<CarrierItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CarrierLookupItem>> GetLookupAsync(CancellationToken cancellationToken = default);
    Task<CarrierDetail> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CarrierAuditChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default);
    Task<CarrierDetail> CreateAsync(SaveCarrierRequest request, CancellationToken cancellationToken = default);
    Task<CarrierDetail> UpdateAsync(int id, SaveCarrierRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
