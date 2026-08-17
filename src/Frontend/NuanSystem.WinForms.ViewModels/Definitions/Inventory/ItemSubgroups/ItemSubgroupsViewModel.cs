using NuanSystem.WinForms.Services.Definitions.Inventory.ItemFamilies;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemFamilies.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemSubgroups;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemSubgroups.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemGroups;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemGroups.Models;
using NuanSystem.WinForms.Services.Security.Access;
using NuanSystem.WinForms.Services.Security.Access.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Definitions.Inventory.ItemSubgroups;

public sealed class ItemSubgroupsViewModel(
    IItemSubgroupClient itemSubgroupClient,
    IItemFamilyClient itemFamilyClient,
    IItemGroupClient itemGroupClient,
    ISecurityAccessClient securityAccessClient)
    : CrudViewModel<ItemSubgroupItem, SaveItemSubgroupRequest>
{
    public IReadOnlyCollection<ItemFamilyItem> ItemFamilyLookups { get; private set; } = [];
    public IReadOnlyCollection<ItemGroupItem> ItemGroupLookups { get; private set; } = [];
    public bool CanCreateItemFamilies { get; private set; }
    public bool CanEditItemFamilies { get; private set; }

    public override Task LoadAsync(CancellationToken ct = default) => LoadItemsAsync(itemSubgroupClient.GetAsync, ct);
    public Task<ItemSubgroupItem> GetByIdAsync(int id, CancellationToken ct = default) => itemSubgroupClient.GetByIdAsync(id, ct);
    public Task<IReadOnlyCollection<ItemSubgroupAuditChange>> GetHistoryAsync(int id, CancellationToken ct = default) => itemSubgroupClient.GetHistoryAsync(id, ct);

    public async Task LoadEditorContextAsync(
        int? selectedFamilyId = null, string? selectedFamilyCode = null, string? selectedFamilyName = null,
        CancellationToken ct = default)
    {
        var familiesTask = itemFamilyClient.GetLookupAsync(null, ct);
        var groupsTask = itemGroupClient.GetLookupAsync(ct);
        var operationsTask = securityAccessClient.GetFormOperationsAsync("item-families", ct);
        var families = (await familiesTask).Select(family => new ItemFamilyItem
        {
            Id = family.Id,
            ItemGroupId = family.ItemGroupId,
            Code = family.Code,
            Name = family.Name,
            IsActive = true
        }).ToList();
        if (selectedFamilyId.HasValue && families.All(family => family.Id != selectedFamilyId.Value))
        {
            families.Add(new ItemFamilyItem
            {
                Id = selectedFamilyId.Value,
                Code = selectedFamilyCode ?? selectedFamilyId.Value.ToString(),
                Name = selectedFamilyName ?? selectedFamilyCode ?? selectedFamilyId.Value.ToString(),
                IsActive = false
            });
        }
        ItemFamilyLookups = families.OrderBy(family => family.Code).ToArray();
        ItemGroupLookups = (await groupsTask).Select(group => new ItemGroupItem
        {
            Id = group.Id,
            GlobalId = group.GlobalId,
            Code = group.Code,
            Name = group.Name,
            SortOrder = group.SortOrder,
            IsSystem = group.IsSystem,
            IsActive = group.IsActive
        }).OrderBy(group => group.Code).ToArray();
        try
        {
            var operations = await operationsTask;
            CanCreateItemFamilies = operations.Any(operation => IsAllowed(operation, "create", "new", "nuevo", "crear", "post"));
            CanEditItemFamilies = operations.Any(operation => IsAllowed(operation, "edit", "update", "editar", "actualizar", "put"));
        }
        catch
        {
            CanCreateItemFamilies = false;
            CanEditItemFamilies = false;
        }
    }

    public Task<ItemFamilyItem> GetItemFamilyByIdAsync(int id, CancellationToken ct = default) => itemFamilyClient.GetByIdAsync(id, ct);
    public Task<ItemFamilyItem> CreateItemFamilyAsync(SaveItemFamilyRequest request, CancellationToken ct = default) => itemFamilyClient.CreateAsync(request, ct);
    public Task<ItemFamilyItem> UpdateItemFamilyAsync(int id, SaveItemFamilyRequest request, CancellationToken ct = default) => itemFamilyClient.UpdateAsync(id, request, ct);
    public override Task CreateAsync(SaveItemSubgroupRequest request, CancellationToken ct = default) => itemSubgroupClient.CreateAsync(request, ct);
    public override Task UpdateAsync(int id, SaveItemSubgroupRequest request, CancellationToken ct = default) => itemSubgroupClient.UpdateAsync(id, request, ct);
    public override Task DeleteAsync(int id, CancellationToken ct = default) => itemSubgroupClient.DeleteAsync(id, ct);

    private static bool IsAllowed(FormOperationAccessItem operation, params string[] keys)
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
