namespace NuanSystem.Application.Features.Sync.Configuration;

public sealed record SyncMasterBranchEntityCatalogItem(
    string EntityCode,
    string EntityName,
    string DisplayName,
    bool ExistsInModel,
    bool HasMasterBranchProducer,
    bool HasMasterBranchApplier,
    bool SupportsInsert,
    bool SupportsUpdate,
    bool SupportsDeactivate,
    string Notes,
    int DefaultExecutionOrder = 100,
    bool SupportsIncremental = true,
    string? DefaultKeyField = "Code",
    string? DefaultModifiedAtField = "UpdatedAt",
    IReadOnlyCollection<string>? Dependencies = null)
{
    public bool HasProducer => HasMasterBranchProducer;
    public bool HasApplier => HasMasterBranchApplier;
    public bool IsOperative => HasMasterBranchProducer && HasMasterBranchApplier;
}

public static class SyncMasterBranchEntityCodes
{
    public const string Countries = "Countries";
    public const string Provinces = "Provinces";
    public const string Cities = "Cities";
    public const string Currencies = "Currencies";
    public const string Taxes = "Tax";
    public const string UnitOfMeasures = "UnitOfMeasure";
    public const string ProductTypes = "ProductType";
    public const string PriceLists = "PriceList";
    public const string PurchaseOrder = "PurchaseOrder";
    public const string BusinessPartnerPaymentTerms = "BusinessPartnerPaymentTerms";
    public const string SupplierGroups = "SupplierGroups";
    public const string SupplierClasses = "SupplierClasses";
    public const string EconomicActivities = "EconomicActivities";
    public const string Zones = "Zones";
    public const string SupplyMethods = "SupplyMethods";
    public const string BusinessPartner = "BusinessPartner";
    public const string ItemGroups = "ItemGroups";
    public const string ItemFamilies = "ItemFamilies";
    public const string ItemSubgroups = "ItemSubgroups";
    public const string ItemBrands = "ItemBrands";
    public const string ItemLines = "ItemLine";
    public const string ItemOrigins = "ItemOrigin";
    public const string ReplenishmentMethods = "ReplenishmentMethod";
    public const string StorageConditions = "StorageCondition";
    public const string Item = "Item";
    public const string Warehouse = "Warehouse";
    public const string Carrier = "Carrier";

    public static readonly IReadOnlyCollection<SyncMasterBranchEntityCatalogItem> InitialCatalog =
    [
        new(Countries, Countries, "Paises", true, true, true, true, true, true, "Catalogo tenant con publicacion incremental, fuente Full y aplicador idempotente por GlobalId.", 10),
        new(Provinces, Provinces, "Provincias", true, true, true, true, true, true, "Catalogo tenant dependiente de Countries, con publicacion incremental, fuente Full y aplicador idempotente por GlobalId.", 20, Dependencies: [Countries]),
        new(Cities, Cities, "Ciudades", true, true, true, true, true, true, "Catalogo tenant dependiente de Countries y Provinces, con publicacion incremental, fuente Full y aplicador idempotente por GlobalId.", 30, Dependencies: [Countries, Provinces]),
        new(Currencies, Currencies, "Monedas", true, true, true, true, true, true, "Catalogo tenant con publicacion incremental, fuente Full y aplicador idempotente por GlobalId.", 40),
        new(Taxes, Taxes, "Impuestos", true, true, true, true, true, true, "Catalogo tributario con fuente Full y aplicador idempotente por GlobalId.", 45),
        new(UnitOfMeasures, UnitOfMeasures, "Unidades de medida", true, true, true, true, true, true, "Catalogo maestro con LocalOutbox transaccional, fuente Full dedicada y aplicador por GlobalId que preserva referencias externas locales.", 50),
        new(ProductTypes, ProductTypes, "Tipos de producto", true, true, true, true, true, true, "Catalogo maestro con naturaleza ERP cerrada, LocalOutbox transaccional, fuente Full dedicada y aplicador por GlobalId sin adopcion por codigo.", 55, DefaultKeyField: "GlobalId"),
        new(BusinessPartnerPaymentTerms, BusinessPartnerPaymentTerms, "Condiciones de pago", true, true, true, true, true, true, "Catalogo tenant con importacion SAP Full, fuente Full y aplicador idempotente por GlobalId.", 50),
        new(SupplierGroups, SupplierGroups, "Grupos de proveedor", true, false, false, false, false, false, "Catalogo tenant definido en 026_tenant_general_supplier_catalogs.sql. Sin productor/aplicador Master-Branch operativo.", 60),
        new(SupplierClasses, SupplierClasses, "Clases de proveedor", true, false, false, false, false, false, "Catalogo tenant definido en 026_tenant_general_supplier_catalogs.sql. Sin productor/aplicador Master-Branch operativo.", 70),
        new(EconomicActivities, EconomicActivities, "Actividades economicas", true, false, false, false, false, false, "Catalogo tenant definido en 026_tenant_general_supplier_catalogs.sql. Sin productor/aplicador Master-Branch operativo.", 80),
        new(Zones, Zones, "Zonas", true, false, false, false, false, false, "Catalogo tenant definido en 026_tenant_general_supplier_catalogs.sql. Sin productor/aplicador Master-Branch operativo.", 90),
        new(SupplyMethods, SupplyMethods, "Metodos de abastecimiento", true, false, false, false, false, false, "Catalogo tenant definido en 026_tenant_general_supplier_catalogs.sql. Sin productor/aplicador Master-Branch operativo.", 100),
        new(BusinessPartner, BusinessPartner, "Socios de negocio", true, true, true, true, true, true, "Productor BusinessPartnerSyncPublisher y aplicador BusinessPartnerSyncApplyRepository existentes; alcance limitado, no BusinessPartners completos.", 200),
        new(ItemGroups, ItemGroups, "Grupos de articulos", true, true, true, true, true, true, "Catalogo maestro con LocalOutbox transaccional, fuente Full y conflicto terminal sin adopcion por codigo.", 205),
        new(ItemFamilies, ItemFamilies, "Familias de articulos", true, true, true, true, true, true, "Catalogo maestro dependiente de ItemGroups con LocalOutbox transaccional, fuente Full y aplicador sin adopcion por codigo.", 207, Dependencies: [ItemGroups]),
        new(ItemSubgroups, ItemSubgroups, "Subgrupos de articulos", true, true, true, true, true, true, "Catalogo maestro dependiente de ItemFamilies con LocalOutbox transaccional, fuente Full y aplicador sin adopcion por codigo.", 209, DefaultKeyField: "GlobalId", Dependencies: [ItemFamilies]),
        new(ItemBrands, ItemBrands, "Marcas de articulos", true, true, true, true, true, true, "Catalogo maestro con LocalOutbox transaccional, fuente Full y aplicador por GlobalId que preserva referencias externas locales.", 208),
        new(ItemLines, ItemLines, "Lineas de articulos", true, true, true, true, true, true, "Catalogo maestro independiente con LocalOutbox transaccional, fuente Full y aplicador por GlobalId sin adopcion por codigo.", 209, DefaultKeyField: "GlobalId"),
        new(ItemOrigins, ItemOrigins, "Origenes de articulos", true, true, true, true, true, true, "Catalogo maestro independiente con LocalOutbox transaccional, fuente Full y aplicador por GlobalId sin adopcion por codigo.", 209, DefaultKeyField: "GlobalId"),
        new(ReplenishmentMethods, ReplenishmentMethods, "Metodos de reposicion", true, true, true, true, true, true, "Catalogo maestro independiente con LocalOutbox transaccional, fuente Full y aplicador por GlobalId sin adopcion por codigo.", 209, DefaultKeyField: "GlobalId"),
        new(StorageConditions, StorageConditions, "Condiciones de almacenamiento", true, true, true, true, true, true, "Catalogo maestro independiente con LocalOutbox transaccional, fuente Full y aplicador por GlobalId sin adopcion por codigo.", 209, DefaultKeyField: "GlobalId"),
        new(Item, Item, "Articulos", true, true, true, true, true, true, "LocalOutbox transaccional y payload v2 con dependencias resueltas exclusivamente por GlobalId; ProductType queda fuera de las dependencias hasta incorporarse al payload; sin stock, costos ni precios.", 210, Dependencies: [ItemGroups, ItemFamilies, UnitOfMeasures]),
        new(Warehouse, Warehouse, "Almacenes", true, true, true, true, true, true, "Contrato corporativo minimo con LocalOutbox transaccional, preservacion de campos locales y conflicto terminal sin adopcion.", 220),
        new(PriceLists, PriceLists, "Listas de precios", true, true, true, true, true, true, "Catalogo comercial con fuente Full y aplicador idempotente por GlobalId.", 230, Dependencies: [Currencies]),
        new(Carrier, Carrier, "Transportistas", true, true, true, true, true, true, "Maestro independiente con LocalOutbox transaccional, fuente Full, tombstone y conflicto terminal sin adopcion por codigo.", 240, DefaultKeyField: "Id"),
        new(PurchaseOrder, PurchaseOrder, "Ordenes de compra", true, true, true, true, true, true, "Documento operativo con enrutamiento por bodega, Outbox/Inbox y aplicacion transaccional.", 300, Dependencies: [Currencies, Taxes, UnitOfMeasures, BusinessPartner, Item, Warehouse, PriceLists])
    ];

    private static readonly IReadOnlyDictionary<string, SyncMasterBranchEntityCatalogItem> CatalogByCode =
        InitialCatalog.ToDictionary(item => item.EntityCode, StringComparer.OrdinalIgnoreCase);

    public static SyncMasterBranchEntityCatalogItem? Find(string? entityCode)
    {
        return !string.IsNullOrWhiteSpace(entityCode)
               && CatalogByCode.TryGetValue(entityCode.Trim(), out var item)
            ? item
            : null;
    }

    public static bool IsKnown(string entityCode)
    {
        return Find(entityCode) is not null;
    }

    public static bool IsOperative(string? entityCode)
    {
        return Find(entityCode)?.IsOperative == true;
    }
}
