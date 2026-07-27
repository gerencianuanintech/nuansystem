using System.Data;
using NuanSystem.Application.Features.FinancialCatalogs.PriceLists.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IPriceListRepository : IRepository
{
    Task<IReadOnlyCollection<PriceListDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<PriceListLookupDto>> GetLookupAsync(string? appliesTo = null, CancellationToken cancellationToken = default);
    Task<PriceListDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PriceListDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<PriceListCurrencyDto?> GetCurrencyAsync(string currencyCode, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<bool> HasDefaultConflictAsync(string appliesTo, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<bool> HasActiveReferencesAsync(int id, string code, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreatePriceListData priceList, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(UpdatePriceListData priceList, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
}
