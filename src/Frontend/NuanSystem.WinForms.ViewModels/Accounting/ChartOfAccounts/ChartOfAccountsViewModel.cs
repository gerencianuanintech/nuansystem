using NuanSystem.WinForms.Services.Accounting.ChartOfAccounts;
using NuanSystem.WinForms.Services.Accounting.ChartOfAccounts.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Accounting.ChartOfAccounts;

public sealed class ChartOfAccountsViewModel : CrudViewModel<ChartOfAccountItem, SaveChartOfAccountRequest>
{
    private readonly IChartOfAccountClient accountClient;

    public ChartOfAccountsViewModel(IChartOfAccountClient accountClient)
    {
        this.accountClient = accountClient;
    }

    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(accountClient.GetAsync, cancellationToken);
    }

    public Task<IReadOnlyCollection<ChartOfAccountLookupItem>> GetLookupAsync(CancellationToken cancellationToken = default)
    {
        return accountClient.GetLookupAsync(cancellationToken);
    }

    public Task<ChartOfAccountItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return accountClient.GetByIdAsync(id, cancellationToken);
    }

    public override Task CreateAsync(SaveChartOfAccountRequest request, CancellationToken cancellationToken = default)
    {
        return accountClient.CreateAsync(request, cancellationToken);
    }

    public async Task<ChartOfAccountItem> CreateAndReturnAsync(SaveChartOfAccountRequest request, CancellationToken cancellationToken = default)
    {
        return await accountClient.CreateAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveChartOfAccountRequest request, CancellationToken cancellationToken = default)
    {
        return accountClient.UpdateAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return accountClient.DeleteAsync(id, cancellationToken);
    }
}
