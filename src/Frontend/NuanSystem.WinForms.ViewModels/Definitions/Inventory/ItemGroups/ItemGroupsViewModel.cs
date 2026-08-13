using NuanSystem.WinForms.Services.Accounting.ChartOfAccounts;
using NuanSystem.WinForms.Services.Accounting.ChartOfAccounts.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemGroups;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemGroups.Models;
using NuanSystem.WinForms.Services.Security.Access;
using NuanSystem.WinForms.Services.Security.Access.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Definitions.Inventory.ItemGroups;

public sealed class ItemGroupsViewModel(
    IItemGroupClient itemGroupClient,
    IChartOfAccountClient chartOfAccountClient,
    ISecurityAccessClient securityAccessClient)
    : CrudViewModel<ItemGroupItem, SaveItemGroupRequest>
{
    public IReadOnlyCollection<ChartOfAccountLookupItem> AccountLookups { get; private set; } = [];
    public bool CanCreateAccounts { get; private set; }
    public bool CanEditAccounts { get; private set; }

    public override Task LoadAsync(CancellationToken cancellationToken = default) =>
        LoadItemsAsync(itemGroupClient.GetAsync, cancellationToken);

    public Task<ItemGroupItem> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        itemGroupClient.GetByIdAsync(id, cancellationToken);

    public async Task LoadEditorContextAsync(CancellationToken cancellationToken = default)
    {
        var accountsTask = chartOfAccountClient.GetLookupAsync(cancellationToken);
        var accessTask = securityAccessClient.GetFormOperationsAsync("chart-of-accounts", cancellationToken);
        AccountLookups = (await accountsTask).OrderBy(account => account.Code).ToArray();

        try
        {
            var operations = await accessTask;
            CanCreateAccounts = operations.Any(operation => IsAllowedOperation(operation, "create", "new", "nuevo", "crear", "post"));
            CanEditAccounts = operations.Any(operation => IsAllowedOperation(operation, "edit", "update", "editar", "actualizar", "put"));
        }
        catch
        {
            CanCreateAccounts = false;
            CanEditAccounts = false;
        }
    }

    public Task<ChartOfAccountItem> GetAccountByIdAsync(int id, CancellationToken cancellationToken = default) =>
        chartOfAccountClient.GetByIdAsync(id, cancellationToken);

    public Task<ChartOfAccountItem> CreateAccountAsync(SaveChartOfAccountRequest request, CancellationToken cancellationToken = default) =>
        chartOfAccountClient.CreateAsync(request, cancellationToken);

    public Task<ChartOfAccountItem> UpdateAccountAsync(int id, SaveChartOfAccountRequest request, CancellationToken cancellationToken = default) =>
        chartOfAccountClient.UpdateAsync(id, request, cancellationToken);

    public override Task CreateAsync(SaveItemGroupRequest request, CancellationToken cancellationToken = default) =>
        itemGroupClient.CreateAsync(request, cancellationToken);

    public override Task UpdateAsync(int id, SaveItemGroupRequest request, CancellationToken cancellationToken = default) =>
        itemGroupClient.UpdateAsync(id, request, cancellationToken);

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        itemGroupClient.DeleteAsync(id, cancellationToken);

    private static bool IsAllowedOperation(FormOperationAccessItem operation, params string[] keys)
    {
        if (!operation.IsAllowed) return false;
        return Matches(operation.ActionKey, keys) || Matches(operation.Code, keys) || Matches(operation.Name, keys);
    }

    private static bool Matches(string? value, IReadOnlyCollection<string> keys)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        var normalized = value.Replace(" ", "", StringComparison.Ordinal)
            .Replace("-", "", StringComparison.Ordinal)
            .Replace("_", "", StringComparison.Ordinal).Trim();
        return keys.Any(key => string.Equals(normalized, key, StringComparison.OrdinalIgnoreCase));
    }
}
