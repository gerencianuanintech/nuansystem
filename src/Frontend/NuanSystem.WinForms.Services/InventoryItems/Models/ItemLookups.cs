using NuanSystem.WinForms.Services.Accounting.ChartOfAccounts.Models;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.ReplenishmentMethods.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.StorageConditions.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemCommercialSegments.Models;

namespace NuanSystem.WinForms.Services.InventoryItems.Models;

public sealed record ItemGroupLookupItem(int Id, string Code, string Name)
{
    public string DisplayText => $"{Code} - {Name}";
}

public sealed record ItemFamilyLookupItem(int Id, int ItemGroupId, string Code, string Name)
{
    public string DisplayText => $"{Code} - {Name}";
}

public sealed record ItemSubgroupLookupItem(int Id, int ItemFamilyId, string Code, string Name)
{
    public string DisplayText => $"{Code} - {Name}";
}

public sealed record ItemOriginLookupItem(int Id, string Code, string Name, bool IsActive)
{
    public string DisplayText => $"{Code} - {Name}";
}

public sealed record UnitOfMeasureLookupItem(int Id, string Code, string Name)
{
    public string DisplayText => $"{Code} - {Name}";
}

public sealed record TaxLookupItem(int Id, string Code, string Name, decimal Rate)
{
    public string DisplayText => $"{Code} - {Name}";
}

public sealed record WarehouseLookupItem(int Id, string Code, string Name)
{
    public string DisplayText => $"{Code} - {Name}";
}

public sealed record ItemLookups(
    IReadOnlyCollection<ItemGroupLookupItem> ItemGroups,
    IReadOnlyCollection<ItemFamilyLookupItem> ItemFamilies,
    IReadOnlyCollection<UnitOfMeasureLookupItem> UnitOfMeasures,
    IReadOnlyCollection<TaxLookupItem> Taxes,
    IReadOnlyCollection<WarehouseLookupItem> Warehouses)
{
    public IReadOnlyCollection<ChartOfAccountLookupItem> Accounts { get; init; } = [];

    public IReadOnlyCollection<GeneralInventoryCatalogLookupItem> Brands { get; init; } = [];

    public IReadOnlyCollection<GeneralInventoryCatalogLookupItem> ItemTypes { get; init; } = [];

    public IReadOnlyCollection<GeneralInventoryCatalogLookupItem> ProductTypes { get; init; } = [];

    public IReadOnlyCollection<GeneralInventoryCatalogLookupItem> ItemLines { get; init; } = [];

    public IReadOnlyCollection<ItemOriginLookupItem> ItemOrigins { get; init; } = [];

    public IReadOnlyCollection<ItemSubgroupLookupItem> ItemSubgroups { get; init; } = [];

    public IReadOnlyCollection<GeneralInventoryCatalogLookupItem> SalesChannels { get; init; } = [];

    public IReadOnlyCollection<GeneralInventoryCatalogLookupItem> WarehouseLocations { get; init; } = [];

    public IReadOnlyCollection<GeneralInventoryCatalogLookupItem> StorageZones { get; init; } = [];

    public IReadOnlyCollection<StorageConditionLookupItem> StorageConditions { get; init; } = [];

    public IReadOnlyCollection<ReplenishmentMethodLookupItem> ReplenishmentMethods { get; init; } = [];

    public IReadOnlyCollection<ItemCommercialSegmentItem> ItemCommercialSegments { get; init; } = [];

    public IReadOnlyCollection<GeneralInventoryCatalogLookupItem> VariantAttributes { get; init; } = [];

    public IReadOnlyCollection<GeneralInventoryCatalogLookupItem> AttachmentDocumentTypes { get; init; } = [];

    public IReadOnlyCollection<GeneralInventoryCatalogLookupItem> AttachmentCategories { get; init; } = [];
}
