namespace NuanSystem.WinForms.Services.FinancialCatalogs.PriceLists;

public interface IPriceListClient
{
    Task<IReadOnlyCollection<PriceListItem>> GetAsync(CancellationToken cancellationToken = default);
    Task<PriceListItem> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PriceListItem> CreateAsync(SavePriceListRequest request, CancellationToken cancellationToken = default);
    Task<PriceListItem> UpdateAsync(int id, SavePriceListRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
