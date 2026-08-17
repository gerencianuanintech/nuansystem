using NuanSystem.WinForms.Services.Accounting.ChartOfAccounts;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;
using NuanSystem.WinForms.Services.GeneralInventory.ItemGroups;
using NuanSystem.WinForms.Services.GeneralInventory.ItemGroups.Models;
using NuanSystem.WinForms.Services.GeneralInventory.ItemFamilies;
using NuanSystem.WinForms.Services.GeneralInventory.ItemFamilies.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemBrands;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemLines;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemOrigins;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemCommercialSegments;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemSubgroups;
using NuanSystem.WinForms.Services.Definitions.Inventory.ProductTypes;
using NuanSystem.WinForms.Services.Definitions.Inventory.ReplenishmentMethods;
using NuanSystem.WinForms.Services.Definitions.Inventory.SalesChannels;
using NuanSystem.WinForms.Services.Definitions.Inventory.ReplenishmentMethods.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.StorageConditions;
using NuanSystem.WinForms.Services.Definitions.Inventory.StorageConditions.Models;
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
    private readonly IItemOriginClient itemOriginClient;
    private readonly IItemCommercialSegmentClient itemCommercialSegmentClient;
    private readonly IItemSubgroupClient itemSubgroupClient;
    private readonly IProductTypeClient productTypeClient;
    private readonly IReplenishmentMethodClient replenishmentMethodClient;
    private readonly ISalesChannelClient salesChannelClient;
    private readonly IStorageConditionClient storageConditionClient;
    private readonly IUnitMeasureClient unitMeasureClient;
    private readonly IGeneralInventoryCatalogClient generalInventoryCatalogClient;
    private readonly IChartOfAccountClient chartOfAccountClient;
    private readonly ISecurityAccessClient securityAccessClient;
    private IReadOnlyDictionary<string, bool> relatedCatalogCreateAccess = new Dictionary<string, bool>();
    private IReadOnlyDictionary<string, bool> relatedCatalogEditAccess = new Dictionary<string, bool>();

    private static readonly string[] RelatedCatalogFormKeys =
    {
        "unit-measures",
        "inventory-warehouses",
        "item-brands",
        "inventory-item-types",
        "product-types",
        "item-lines",
        "item-origins",
        "item-commercial-segments",
        "item-subgroups",
        "sales-channels",
        "inventory-warehouse-locations",
        "inventory-storage-zones",
        "storage-conditions",
        "replenishment-methods",
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
        IItemOriginClient itemOriginClient,
        IItemCommercialSegmentClient itemCommercialSegmentClient,
        IItemSubgroupClient itemSubgroupClient,
        IProductTypeClient productTypeClient,
        IReplenishmentMethodClient replenishmentMethodClient,
        ISalesChannelClient salesChannelClient,
        IStorageConditionClient storageConditionClient,
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
        this.itemOriginClient = itemOriginClient;
        this.itemCommercialSegmentClient = itemCommercialSegmentClient;
        this.itemSubgroupClient = itemSubgroupClient;
        this.productTypeClient = productTypeClient;
        this.replenishmentMethodClient = replenishmentMethodClient;
        this.salesChannelClient = salesChannelClient;
        this.storageConditionClient = storageConditionClient;
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
        var itemOriginsTask = itemOriginClient.GetLookupAsync(cancellationToken);
        var itemCommercialSegmentsTask = itemCommercialSegmentClient.GetLookupAsync(cancellationToken);
        var itemSubgroupsTask = itemSubgroupClient.GetLookupAsync(null, cancellationToken);
        var salesChannelsTask = salesChannelClient.GetLookupAsync(cancellationToken);
        var warehouseLocationsTask = generalInventoryCatalogClient.GetLookupAsync(GeneralInventoryCatalogRoutes.WarehouseLocations, cancellationToken);
        var storageZonesTask = generalInventoryCatalogClient.GetLookupAsync(GeneralInventoryCatalogRoutes.StorageZones, cancellationToken);
        var storageConditionsTask = storageConditionClient.GetLookupAsync(cancellationToken);
        var replenishmentMethodsTask = replenishmentMethodClient.GetLookupAsync(cancellationToken);
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
            itemOriginsTask,
            itemCommercialSegmentsTask,
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
            ItemOrigins = (await itemOriginsTask)
                .Where(item => item.IsActive)
                .OrderBy(item => item.SortOrder)
                .ThenBy(item => item.Code)
                .ThenBy(item => item.Name)
                .Select(item => new ItemOriginLookupItem(item.Id, item.Code, item.Name, item.IsActive))
                .ToArray(),
            ItemCommercialSegments = (await itemCommercialSegmentsTask)
                .Where(item => item.IsActive)
                .OrderBy(item => item.SortOrder)
                .ThenBy(item => item.Code)
                .ThenBy(item => item.Name)
                .ToArray(),
            ItemSubgroups = (await itemSubgroupsTask)
                .Where(item => item.IsActive)
                .OrderBy(item => item.ItemFamilyName)
                .ThenBy(item => item.SortOrder)
                .ThenBy(item => item.Code)
                .Select(item => new ItemSubgroupLookupItem(item.Id, item.ItemFamilyId, item.Code, item.Name))
                .ToArray(),
            SalesChannels = (await salesChannelsTask)
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
            WarehouseLocations = ActiveCatalog(await warehouseLocationsTask),
            StorageZones = ActiveCatalog(await storageZonesTask),
            StorageConditions = (await storageConditionsTask)
                .Where(item => item.IsActive)
                .OrderBy(item => item.SortOrder)
                .ThenBy(item => item.Code)
                .ThenBy(item => item.Name)
                .ToArray(),
            ReplenishmentMethods = (await replenishmentMethodsTask)
                .Where(item => item.IsActive)
                .OrderBy(item => item.SortOrder)
                .ThenBy(item => item.Code)
                .ThenBy(item => item.Name)
                .ToArray(),
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
            var editAccess = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

            foreach (var formKey in RelatedCatalogFormKeys)
            {
                var operations = await GetFormOperationsAsync(formKey, cancellationToken);
                access[formKey] = operations.Any(IsCreateOperation);
                editAccess[formKey] = operations.Any(IsEditOperation);
            }

            relatedCatalogCreateAccess = access;
            relatedCatalogEditAccess = editAccess;
        }
        catch
        {
            CanCreateItemGroups = false;
            CanCreateItemFamilies = false;
            relatedCatalogCreateAccess = new Dictionary<string, bool>();
            relatedCatalogEditAccess = new Dictionary<string, bool>();
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

    public Task<NuanSystem.WinForms.Services.Definitions.Inventory.ItemOrigins.Models.ItemOriginItem> GetItemOriginByIdAsync(
        int id,
        CancellationToken cancellationToken = default) => itemOriginClient.GetByIdAsync(id, cancellationToken);

    public async Task<NuanSystem.WinForms.Services.Definitions.Inventory.ItemOrigins.Models.ItemOriginItem> CreateItemOriginAsync(
        NuanSystem.WinForms.Services.Definitions.Inventory.ItemOrigins.Models.SaveItemOriginRequest request,
        CancellationToken cancellationToken = default)
    {
        var saved = await itemOriginClient.CreateAsync(request, cancellationToken);
        await LoadLookupsAsync(cancellationToken);
        return saved;
    }

    public async Task<NuanSystem.WinForms.Services.Definitions.Inventory.ItemOrigins.Models.ItemOriginItem> UpdateItemOriginAsync(
        int id,
        NuanSystem.WinForms.Services.Definitions.Inventory.ItemOrigins.Models.SaveItemOriginRequest request,
        CancellationToken cancellationToken = default)
    {
        var saved = await itemOriginClient.UpdateAsync(id, request, cancellationToken);
        await LoadLookupsAsync(cancellationToken);
        return saved;
    }

    public Task<ReplenishmentMethodItem> GetReplenishmentMethodByIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        replenishmentMethodClient.GetByIdAsync(id, cancellationToken);

    public async Task<ReplenishmentMethodItem> CreateReplenishmentMethodAsync(
        SaveReplenishmentMethodRequest request,
        CancellationToken cancellationToken = default)
    {
        var saved = await replenishmentMethodClient.CreateAsync(request, cancellationToken);
        await LoadLookupsAsync(cancellationToken);
        return saved;
    }

    public async Task<ReplenishmentMethodItem> UpdateReplenishmentMethodAsync(
        int id,
        SaveReplenishmentMethodRequest request,
        CancellationToken cancellationToken = default)
    {
        var saved = await replenishmentMethodClient.UpdateAsync(id, request, cancellationToken);
        await LoadLookupsAsync(cancellationToken);
        return saved;
    }

    public Task<StorageConditionItem> GetStorageConditionByIdAsync(int id, CancellationToken cancellationToken = default) =>
        storageConditionClient.GetByIdAsync(id, cancellationToken);

    public async Task<StorageConditionItem> CreateStorageConditionAsync(SaveStorageConditionRequest request, CancellationToken cancellationToken = default)
    {
        var saved = await storageConditionClient.CreateAsync(request, cancellationToken);
        await LoadLookupsAsync(cancellationToken);
        return saved;
    }

    public async Task<StorageConditionItem> UpdateStorageConditionAsync(int id, SaveStorageConditionRequest request, CancellationToken cancellationToken = default)
    {
        var saved = await storageConditionClient.UpdateAsync(id, request, cancellationToken);
        await LoadLookupsAsync(cancellationToken);
        return saved;
    }

    public bool CanEditRelatedCatalog(string formKey) =>
        relatedCatalogEditAccess.TryGetValue(formKey, out var canEdit) && canEdit;

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

    private static bool IsEditOperation(FormOperationAccessItem operation)
    {
        return operation.IsAllowed
            && (MatchesOperation(operation.ActionKey, "edit", "update", "editar", "actualizar", "put")
                || MatchesOperation(operation.Code, "edit", "update", "editar", "actualizar", "put")
                || MatchesOperation(operation.Name, "edit", "update", "editar", "actualizar", "put"));
    }

    private async Task<IReadOnlyCollection<FormOperationAccessItem>> GetFormOperationsAsync(
        string formKey,
        CancellationToken cancellationToken)
    {
        try
        {
            return await securityAccessClient.GetFormOperationsAsync(formKey, cancellationToken);
        }
        catch
        {
            return [];
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
