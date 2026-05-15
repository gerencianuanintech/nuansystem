using NuanSystem.WinForms.Services.Accounting.ChartOfAccounts.Models;

namespace NuanSystem.WinForms.Services.Accounting.ChartOfAccounts;

public interface IChartOfAccountClient
{
    Task<IReadOnlyCollection<ChartOfAccountItem>> GetAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ChartOfAccountLookupItem>> GetLookupAsync(CancellationToken cancellationToken = default);

    Task<ChartOfAccountItem> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ChartOfAccountItem> CreateAsync(SaveChartOfAccountRequest request, CancellationToken cancellationToken = default);

    Task<ChartOfAccountItem> UpdateAsync(int id, SaveChartOfAccountRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
