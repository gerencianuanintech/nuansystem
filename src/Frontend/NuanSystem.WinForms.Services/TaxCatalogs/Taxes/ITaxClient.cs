namespace NuanSystem.WinForms.Services.TaxCatalogs.Taxes;

public interface ITaxClient
{
    Task<IReadOnlyCollection<TaxItem>> GetAsync(CancellationToken cancellationToken = default);
    Task<TaxItem> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TaxAuditChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default);
    Task<TaxItem> CreateAsync(SaveTaxRequest request, CancellationToken cancellationToken = default);
    Task<TaxItem> UpdateAsync(int id, SaveTaxRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
