using NuanSystem.WinForms.Services.Accounting.ChartOfAccounts;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;
using NuanSystem.WinForms.Services.GeneralInventory.ItemGroups;
using NuanSystem.WinForms.Services.GeneralInventory.ItemGroups.Models;
using NuanSystem.WinForms.Services.GeneralInventory.ItemFamilies;
using NuanSystem.WinForms.Services.GeneralInventory.ItemFamilies.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemBrands;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemLines;
using NuanSystem.WinForms.Services.Definitions.Inventory.ProductTypes;
using NuanSystem.WinForms.Services.Definitions.Inventory.UnitMeasures;
using DefinitionUnitMeasureLookupItem = NuanSystem.WinForms.Services.Definitions.Inventory.UnitMeasures.Models.UnitMeasureLookupItem;
using NuanSystem.WinForms.Services.InventoryItems;
using NuanSystem.WinForms.Services.InventoryItems.Models;
using NuanSystem.WinForms.Services.Security.Access;
using NuanSystem.WinForms.Services.Security.Access.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.InventoryItems;

public sealed class ItemsViewModel : CrudViewModel<ItemItem, SaveItemRequest>
{
    private readonly IItemClient itemClient;
    private readonly IItemGroupClient itemGroupClient;
    private readonly IItemFamilyClient itemFamilyClient;
    private readonly IItemBrandClient itemBrandClient;
    private readonly IItemLineClient itemLineClient;
    private readonly IProductTypeClient productTypeClient;
    private readonly IUnitMeasureClient unitMeasureClient;
    private readonly IGeneralInventoryCatalogClient generalInventoryCatalogClient;
    private readonly IChartOfAccountClient chartOfAccountClient;
    private readonly ISecurityAccessClient securityAccessClient;
    private IReadOnlyDictionary<string, bool> relatedCatalogCreateAccess = new Dictionary<string, bool>();

    private static readonly string[] RelatedCatalogFormKeys =
    {
        "unit-measures",
        "inventory-warehouses",
        "item-brands",
        "inventory-item-types",
        "product-types",
        "item-lines",
        "inventory-item-subgroups",
        "inventory-sales-channels",
        "inventory-warehouse-locations",
        "inventory-storage-zones",
        "inventory-storage-conditions",
        "inventory-replenishment-methods",
        "inventory-variant-attributes",
        "inventory-attachment-document-types",
        "inventory-attachment-categories"
    };

    public ItemsViewModel(
        IItemClient itemClient,
        IItemGroupClient itemGroupClient,
        IItemFamilyClient itemFamilyClient,
        IItemBrandClient itemBrandClient,
        IItemLineClient itemLineClient,
        IProductTypeClient productTypeClient,
        IUnitMeasureClient unitMeasureClient,
        IGeneralInventoryCatalogClient generalInventoryCatalogClient,
        IChartOfAccountClient chartOfAccountClient,
        ISecurityAccessClient securityAccessClient)
    {
        this.itemClient = itemClient;
        this.itemGroupClient = itemGroupClient;
        this.itemFamilyClient = itemFamilyClient;
        this.itemBrandClient = itemBrandClient;
        this.itemLineClient = itemLineClient;
        this.productTypeClient = productTypeClient;
        this.unitMeasureClient = unitMeasureClient;
        this.generalInventoryCatalogClient = generalInventoryCatalogClient;
        this.chartOfAccountClient = chartOfAccountClient;
        this.securityAccessClient = securityAccessClient;
    }

    public ItemLookups Lookups { get; private set; } = new([], [], [], [], []);

    public bool CanCreateItemGroups { get; private set; }

    public bool CanCreateItemFamilies { get; private set; }

    public bool CanCreateRelatedCatalog(string formKey)
    {
        return relatedCatalogCreateAccess.TryGetValue(formKey, out var canCreate) && canCreate;
    }

    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(itemClient.GetAsync, cancellationToken);
    }

    public async Task LoadLookupsAsync(CancellationToken cancellationToken = default)
    {
        var lookupsTask = itemClient.GetLookupsAsync(cancellationToken);
        var itemFamiliesTask = itemFamilyClient.GetAsync(cancellationToken);
        var accountsTask = chartOfAccountClient.GetLookupAsync(cancellationToken);
        var unitMeasuresTask = unitMeasureClient.GetLookupAsync(cancellationToken);
        var warehousesTask = generalInventoryCatalogClient.GetLookupAsync(GeneralInventoryCatalogRoutes.Warehouses, cancellationToken);
        var brandsTask = itemBrandClient.GetLookupAsync(cancellationToken);
        var itemTypesTask = generalInventoryCatalogClient.GetLookupAsync(GeneralInventoryCatalogRoutes.ItemTypes, cancellationToken);
        var productTypesTask = productTypeClient.GetLookupAsync(cancellationToken);
        var itemLinesTask = itemLineClient.GetLookupAsync(cancellationToken);
        var itemSubgroupsTask = generalInventoryCatalogClient.GetLookupAsync(GeneralInventoryCatalogRoutes.ItemSubgroups, cancellationToken);
        var salesChannelsTask = generalInventoryCatalogClient.GetLookupAsync(GeneralInventoryCatalogRoutes.SalesChannels, cancellationToken);
        var warehouseLocationsTask = generalInventoryCatalogClient.GetLookupAsync(GeneralInventoryCatalogRoutes.WarehouseLocations, cancellationToken);
        var storageZonesTask = generalInventoryCatalogClient.GetLookupAsync(GeneralInventoryCatalogRoutes.StorageZones, cancellationToken);
        var storageConditionsTask = generalInventoryCatalogClient.GetLookupAsync(GeneralInventoryCatalogRoutes.StorageConditions, cancellationToken);
        var replenishmentMethodsTask = generalInventoryCatalogClient.GetLookupAsync(GeneralInventoryCatalogRoutes.ReplenishmentMethods, cancellationToken);
        var variantAttributesTask = generalInventoryCatalogClient.GetLookupAsync(GeneralInventoryCatalogRoutes.VariantAttributes, cancellationToken);
        var attachmentDocumentTypesTask = generalInventoryCatalogClient.GetLookupAsync(GeneralInventoryCatalogRoutes.AttachmentDocumentTypes, cancellationToken);
        var attachmentCategoriesTask = generalInventoryCatalogClient.GetLookupAsync(GeneralInventoryCatalogRoutes.AttachmentCategories, cancellationToken);

        await Task.WhenAll(
            lookupsTask,
            itemFamiliesTask,
            accountsTask,
            unitMeasuresTask,
            warehousesTask,
            brandsTask,
            itemTypesTask,
            productTypesTask,
            itemLinesTask,
            itemSubgroupsTask,
            salesChannelsTask,
            warehouseLocationsTask,
            storageZonesTask,
            storageConditionsTask,
            replenishmentMethodsTask,
            variantAttributesTask,
            attachmentDocumentTypesTask,
            attachmentCategoriesTask);

        var lookups = await lookupsTask;
        var itemFamilies = await itemFamiliesTask;
        var accounts = await accountsTask;
        var unitMeasures = ToUnitMeasures(await unitMeasuresTask, lookups.UnitOfMeasures);
        var warehouses = ToWarehouses(await warehousesTask, lookups.Warehouses);

        Lookups = lookups with
        {
            UnitOfMeasures = unitMeasures,
            Warehouses = warehouses,
            ItemFamilies = itemFamilies
                .Where(family => family.IsActive)
                .Select(family => new ItemFamilyLookupItem(family.Id, family.ItemGroupId, family.Code, family.Name))
                .OrderBy(family => family.DisplayText)
                .ToArray(),
            Accounts = accounts
                .Where(account => account.IsActive)
                .OrderBy(account => account.Code)
                .ToArray(),
            Brands = (await brandsTask)
                .Where(brand => brand.IsActive)
                .OrderBy(brand => brand.Code)
                .ThenBy(brand => brand.Name)
                .Select(brand => new GeneralInventoryCatalogLookupItem
                {
                    Id = brand.Id,
                    Code = brand.Code,
                    Name = brand.Name,
                    IsActive = brand.IsActive
                })
                .ToArray(),
            ItemTypes = ActiveCatalog(await itemTypesTask),
            ProductTypes = (await productTypesTask)
                .Where(item => item.IsActive)
                .OrderBy(item => item.SortOrder)
                .ThenBy(item => item.Code)
                .Select(item => new GeneralInventoryCatalogLookupItem
                {
                    Id = item.Id,
                    Code = item.Code,
                    Name = item.Name,
                    IsActive = item.IsActive
                })
                .ToArray(),
            ItemLines = (await itemLinesTask)
                .Where(item => item.IsActive)
                .OrderBy(item => item.SortOrder)
                .ThenBy(item => item.Code)
                .ThenBy(item => item.Name)
                .Select(item => new GeneralInventoryCatalogLookupItem
                {
                    Id = item.Id,
                    Code = item.Code,
                    Name = item.Name,
                    IsActive = item.IsActive
                })
                .ToArray(),
            ItemSubgroups = ActiveCatalog(await itemSubgroupsTask),
            SalesChannels = ActiveCatalog(await salesChannelsTask),
            WarehouseLocations = ActiveCatalog(await warehouseLocationsTask),
            StorageZones = ActiveCatalog(await storageZonesTask),
            StorageConditions = ActiveCatalog(await storageConditionsTask),
            ReplenishmentMethods = ActiveCatalog(await replenishmentMethodsTask),
            VariantAttributes = ActiveCatalog(await variantAttributesTask),
            AttachmentDocumentTypes = ActiveCatalog(await attachmentDocumentTypesTask),
            AttachmentCategories = ActiveCatalog(await attachmentCategoriesTask)
        };
    }

    public async Task<ItemLookups> ReloadLookupsForEditAsync(CancellationToken cancellationToken = default)
    {
        await LoadLookupsAsync(cancellationToken);
        return Lookups;
    }

    public async Task LoadItemGroupCreateAccessAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            CanCreateItemGroups = await HasCreateAccessAsync("item-groups", cancellationToken);
            CanCreateItemFamilies = await HasCreateAccessAsync("item-families", cancellationToken);
            var access = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase)
            {
                ["item-groups"] = CanCreateItemGroups,
                ["item-families"] = CanCreateItemFamilies
            };

            foreach (var formKey in RelatedCatalogFormKeys)
            {
                access[formKey] = await HasCreateAccessAsync(formKey, cancellationToken);
            }

            relatedCatalogCreateAccess = access;
        }
        catch
        {
            CanCreateItemGroups = false;
            CanCreateItemFamilies = false;
            relatedCatalogCreateAccess = new Dictionary<string, bool>();
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

    private async Task<bool> HasCreateAccessAsync(string formKey, CancellationToken cancellationToken)
    {
        try
        {
            var operations = await securityAccessClient.GetFormOperationsAsync(formKey, cancellationToken);
            return operations.Any(IsCreateOperation);
        }
        catch
        {
            return false;
        }
    }

    private static IReadOnlyCollection<GeneralInventoryCatalogLookupItem> ActiveCatalog(
        IReadOnlyCollection<GeneralInventoryCatalogLookupItem> items)
    {
        return items
            .Where(item => item.IsActive)
            .OrderBy(item => item.Code)
            .ThenBy(item => item.Name)
            .ToArray();
    }

    private static IReadOnlyCollection<UnitOfMeasureLookupItem> ToUnitMeasures(
        IReadOnlyCollection<DefinitionUnitMeasureLookupItem> items,
        IReadOnlyCollection<UnitOfMeasureLookupItem> fallback)
    {
        var activeItems = items
            .Where(item => item.IsActive)
            .OrderBy(item => item.SortOrder)
            .ThenBy(item => item.Code)
            .ThenBy(item => item.Name)
            .ToArray();
        if (activeItems.Length == 0)
        {
            return fallback;
        }

        return activeItems
            .Select(item => new UnitOfMeasureLookupItem(item.Id, item.Code, item.Name))
            .ToArray();
    }

    private static IReadOnlyCollection<WarehouseLookupItem> ToWarehouses(
        IReadOnlyCollection<GeneralInventoryCatalogLookupItem> items,
        IReadOnlyCollection<WarehouseLookupItem> fallback)
    {
        var activeItems = ActiveCatalog(items);
        if (activeItems.Count == 0)
        {
            return fallback;
        }

        return activeItems
            .Select(item => new WarehouseLookupItem(item.Id, item.Code, item.Name))
            .ToArray();
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
