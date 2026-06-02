using NuanSystem.WinForms.Services.Accounting.ChartOfAccounts;
using NuanSystem.WinForms.Services.Accounting.ChartOfAccounts.Models;
using NuanSystem.WinForms.Services.GeneralInventory.ItemGroups;
using NuanSystem.WinForms.Services.GeneralInventory.ItemGroups.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.GeneralInventory.ItemGroups;

public sealed class ItemGroupsViewModel : CrudViewModel<ItemGroupItem, SaveItemGroupRequest>
{
    private readonly IItemGroupClient itemGroupClient;
    private readonly IChartOfAccountClient chartOfAccountClient;

    public ItemGroupsViewModel(IItemGroupClient itemGroupClient, IChartOfAccountClient chartOfAccountClient)
    {
        this.itemGroupClient = itemGroupClient;
        this.chartOfAccountClient = chartOfAccountClient;
    }

    public IReadOnlyCollection<ChartOfAccountLookupItem> AccountLookups { get; private set; } = Array.Empty<ChartOfAccountLookupItem>();

    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(itemGroupClient.GetAsync, cancellationToken);
    }

    public Task<ItemGroupItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return itemGroupClient.GetByIdAsync(id, cancellationToken);
    }

    public async Task LoadAccountLookupsAsync(CancellationToken cancellationToken = default)
    {
        var accounts = await chartOfAccountClient.GetLookupAsync(cancellationToken);
        AccountLookups = accounts
            .Where(account => account.IsActive)
            .OrderBy(account => account.Code)
            .ToArray();
    }

    public override Task CreateAsync(SaveItemGroupRequest request, CancellationToken cancellationToken = default)
    {
        return itemGroupClient.CreateAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveItemGroupRequest request, CancellationToken cancellationToken = default)
    {
        return itemGroupClient.UpdateAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return itemGroupClient.DeleteAsync(id, cancellationToken);
    }
}
