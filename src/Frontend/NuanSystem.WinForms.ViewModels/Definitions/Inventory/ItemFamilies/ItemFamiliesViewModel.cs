using NuanSystem.WinForms.Services.Definitions.Inventory.ItemFamilies;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemFamilies.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemGroups;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemGroups.Models;
using NuanSystem.WinForms.Services.Accounting.ChartOfAccounts;
using NuanSystem.WinForms.Services.Accounting.ChartOfAccounts.Models;
using NuanSystem.WinForms.Services.Security.Access;
using NuanSystem.WinForms.Services.Security.Access.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Definitions.Inventory.ItemFamilies;

public sealed class ItemFamiliesViewModel(
    IItemFamilyClient itemFamilyClient,
    IItemGroupClient itemGroupClient,
    IChartOfAccountClient chartOfAccountClient,
    ISecurityAccessClient securityAccessClient)
    : CrudViewModel<ItemFamilyItem, SaveItemFamilyRequest>
{
    public IReadOnlyCollection<ItemGroupItem> ItemGroupLookups { get; private set; } = [];
    public IReadOnlyCollection<ChartOfAccountLookupItem> AccountLookups { get; private set; } = [];
    public bool CanCreateItemGroups { get; private set; }
    public bool CanEditItemGroups { get; private set; }

    public override Task LoadAsync(CancellationToken cancellationToken = default) =>
        LoadItemsAsync(itemFamilyClient.GetAsync, cancellationToken);

    public Task<ItemFamilyItem> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        itemFamilyClient.GetByIdAsync(id, cancellationToken);

    public Task<IReadOnlyCollection<ItemFamilyAuditChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default) =>
        itemFamilyClient.GetHistoryAsync(id, cancellationToken);

    public async Task LoadEditorContextAsync(
        int? selectedItemGroupId = null,
        string? selectedItemGroupCode = null,
        string? selectedItemGroupName = null,
        CancellationToken cancellationToken = default)
    {
        var groupsTask = itemGroupClient.GetLookupAsync(cancellationToken);
        var accountsTask = chartOfAccountClient.GetLookupAsync(cancellationToken);
        var operationsTask = securityAccessClient.GetFormOperationsAsync("item-groups", cancellationToken);
        var groups = (await groupsTask)
            .Select(group => new ItemGroupItem
            {
                Id = group.Id,
                GlobalId = group.GlobalId,
                Code = group.Code,
                Name = group.Name,
                SortOrder = group.SortOrder,
                IsSystem = group.IsSystem,
                IsActive = group.IsActive
            })
            .ToList();
        if (selectedItemGroupId.HasValue
            && groups.All(group => group.Id != selectedItemGroupId.Value)
            && !string.IsNullOrWhiteSpace(selectedItemGroupCode))
        {
            groups.Add(new ItemGroupItem
            {
                Id = selectedItemGroupId.Value,
                Code = selectedItemGroupCode,
                Name = selectedItemGroupName ?? selectedItemGroupCode,
                IsActive = false
            });
        }

        ItemGroupLookups = groups
            .OrderBy(group => group.Code)
            .ToArray();
        AccountLookups = (await accountsTask).OrderBy(account => account.Code).ToArray();

        try
        {
            var operations = await operationsTask;
            CanCreateItemGroups = operations.Any(operation => IsAllowedOperation(operation, "create", "new", "nuevo", "crear", "post"));
            CanEditItemGroups = operations.Any(operation => IsAllowedOperation(operation, "edit", "update", "editar", "actualizar", "put"));
        }
        catch
        {
            CanCreateItemGroups = false;
            CanEditItemGroups = false;
        }
    }

    public Task<ItemGroupItem> GetItemGroupByIdAsync(int id, CancellationToken cancellationToken = default) =>
        itemGroupClient.GetByIdAsync(id, cancellationToken);

    public Task<ItemGroupItem> CreateItemGroupAsync(SaveItemGroupRequest request, CancellationToken cancellationToken = default) =>
        itemGroupClient.CreateAsync(request, cancellationToken);

    public Task<ItemGroupItem> UpdateItemGroupAsync(int id, SaveItemGroupRequest request, CancellationToken cancellationToken = default) =>
        itemGroupClient.UpdateAsync(id, request, cancellationToken);

    public override Task CreateAsync(SaveItemFamilyRequest request, CancellationToken cancellationToken = default) =>
        itemFamilyClient.CreateAsync(request, cancellationToken);

    public override Task UpdateAsync(int id, SaveItemFamilyRequest request, CancellationToken cancellationToken = default) =>
        itemFamilyClient.UpdateAsync(id, request, cancellationToken);

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        itemFamilyClient.DeleteAsync(id, cancellationToken);

    private static bool IsAllowedOperation(FormOperationAccessItem operation, params string[] keys)
    {
        if (!operation.IsAllowed) return false;
        return Matches(operation.ActionKey, keys) || Matches(operation.Code, keys) || Matches(operation.Name, keys);
    }

    private static bool Matches(string? value, IReadOnlyCollection<string> keys)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        var normalized = value.Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace("_", string.Empty, StringComparison.Ordinal).Trim();
        return keys.Any(key => string.Equals(normalized, key, StringComparison.OrdinalIgnoreCase));
    }
}
