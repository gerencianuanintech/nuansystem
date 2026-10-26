using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Data;

public interface IBusinessPartnerUnitOfWork
{
    Task<T> ExecuteAsync<T>(Func<IBusinessPartnerTransaction, CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Valid only during its unit-of-work callback, for the current tenant.
/// Await operations sequentially; do not retain the context or start detached work.
/// </summary>
public interface IBusinessPartnerTransaction
{
    Task<BusinessPartnerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, int? excludingId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdentificationAsync(int typeId, string number, int? excludingId, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateBusinessPartnerData partner, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(UpdateBusinessPartnerData partner, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int? userId, string? userName, CancellationToken cancellationToken = default);
    Task<Guid?> EnqueueAsync(BusinessPartnerDto partner, SyncOperation operation, CancellationToken cancellationToken = default);
}
