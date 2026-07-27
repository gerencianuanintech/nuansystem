using System.Data;
using NuanSystem.Application.Features.TaxCatalogs.Taxes.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ITaxRepository : IRepository
{
    Task<IReadOnlyCollection<TaxDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TaxLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default);
    Task<TaxDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TaxDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<bool> HasActiveItemReferencesAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateTaxData tax, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(UpdateTaxData tax, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
}
