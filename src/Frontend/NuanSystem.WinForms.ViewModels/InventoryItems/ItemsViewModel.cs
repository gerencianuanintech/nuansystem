using NuanSystem.WinForms.Services.GeneralInventory.ItemGroups;
using NuanSystem.WinForms.Services.GeneralInventory.ItemGroups.Models;
using NuanSystem.WinForms.Services.GeneralInventory.ItemFamilies;
using NuanSystem.WinForms.Services.GeneralInventory.ItemFamilies.Models;
using NuanSystem.WinForms.Services.InventoryItems;
using NuanSystem.WinForms.Services.InventoryItems.Models;
using NuanSystem.WinForms.Services.SecurityAccess;
using NuanSystem.WinForms.Services.SecurityAccess.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.InventoryItems;

public sealed class ItemsViewModel : CrudViewModel<ItemItem, SaveItemRequest>
{
    private readonly IItemClient itemClient;
    private readonly IItemGroupClient itemGroupClient;
    private readonly IItemFamilyClient itemFamilyClient;
    private readonly ISecurityAccessClient securityAccessClient;

    public ItemsViewModel(
        IItemClient itemClient,
        IItemGroupClient itemGroupClient,
        IItemFamilyClient itemFamilyClient,
        ISecurityAccessClient securityAccessClient)
    {
        this.itemClient = itemClient;
        this.itemGroupClient = itemGroupClient;
        this.itemFamilyClient = itemFamilyClient;
        this.securityAccessClient = securityAccessClient;
    }

    public ItemLookups Lookups { get; private set; } = new([], [], [], [], []);

    public bool CanCreateItemGroups { get; private set; }

    public bool CanCreateItemFamilies { get; private set; }

    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(itemClient.GetAsync, cancellationToken);
    }

    public async Task LoadLookupsAsync(CancellationToken cancellationToken = default)
    {
        var lookups = await itemClient.GetLookupsAsync(cancellationToken);
        var itemFamilies = await itemFamilyClient.GetAsync(cancellationToken);

        Lookups = lookups with
        {
            ItemFamilies = itemFamilies
                .Where(family => family.IsActive)
                .Select(family => new ItemFamilyLookupItem(family.Id, family.ItemGroupId, family.Code, family.Name))
                .OrderBy(family => family.DisplayText)
                .ToArray()
        };
    }

    public async Task LoadItemGroupCreateAccessAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var operations = await securityAccessClient.GetFormOperationsAsync("item-groups", cancellationToken);
            CanCreateItemGroups = operations.Any(IsCreateOperation);
            var familyOperations = await securityAccessClient.GetFormOperationsAsync("item-families", cancellationToken);
            CanCreateItemFamilies = familyOperations.Any(IsCreateOperation);
        }
        catch
        {
            CanCreateItemGroups = false;
            CanCreateItemFamilies = false;
        }
    }

    public async Task<ItemGroupLookupItem> CreateItemGroupAsync(
        SaveItemGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        var itemGroup = await itemGroupClient.CreateAsync(request, cancellationToken);
        await LoadLookupsAsync(cancellationToken);
        return new ItemGroupLookupItem(itemGroup.Id, itemGroup.Code, itemGroup.Name);
    }

    public async Task<ItemFamilyLookupItem> CreateItemFamilyAsync(
        SaveItemFamilyRequest request,
        CancellationToken cancellationToken = default)
    {
        var itemFamily = await itemFamilyClient.CreateAsync(request, cancellationToken);
        await LoadLookupsAsync(cancellationToken);
        return new ItemFamilyLookupItem(itemFamily.Id, itemFamily.ItemGroupId, itemFamily.Code, itemFamily.Name);
    }

    public Task<ItemItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return itemClient.GetByIdAsync(id, cancellationToken);
    }

    public override Task CreateAsync(SaveItemRequest request, CancellationToken cancellationToken = default)
    {
        return itemClient.CreateAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveItemRequest request, CancellationToken cancellationToken = default)
    {
        return itemClient.UpdateAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return itemClient.DeleteAsync(id, cancellationToken);
    }

    private static bool IsCreateOperation(FormOperationAccessItem operation)
    {
        return operation.IsAllowed
            && (MatchesOperation(operation.ActionKey, "create", "new", "nuevo", "crear", "post")
                || MatchesOperation(operation.Code, "create", "new", "nuevo", "crear", "post")
                || MatchesOperation(operation.Name, "create", "new", "nuevo", "crear", "post"));
    }

    private static bool MatchesOperation(string? value, params string[] keys)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = value
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace("_", string.Empty, StringComparison.Ordinal)
            .Trim();

        return keys.Any(key => string.Equals(normalized, key, StringComparison.OrdinalIgnoreCase));
    }
}
