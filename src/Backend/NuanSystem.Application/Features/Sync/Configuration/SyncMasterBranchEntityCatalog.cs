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
    public const string BusinessPartnerPaymentTerms = "BusinessPartnerPaymentTerms";
    public const string SupplierGroups = "SupplierGroups";
    public const string SupplierClasses = "SupplierClasses";
    public const string EconomicActivities = "EconomicActivities";
    public const string Zones = "Zones";
    public const string SupplyMethods = "SupplyMethods";
    public const string BusinessPartner = "BusinessPartner";
    public const string Item = "Item";
    public const string Warehouse = "Warehouse";

    public static readonly IReadOnlyCollection<SyncMasterBranchEntityCatalogItem> InitialCatalog =
    [
        new(Countries, Countries, "Paises", true, false, false, false, false, false, "Catalogo tenant definido en 029_tenant_geography_catalogs.sql. Sin productor/aplicador Master-Branch operativo.", 10),
        new(Provinces, Provinces, "Provincias", true, false, false, false, false, false, "Catalogo tenant definido en 029_tenant_geography_catalogs.sql. Sin productor/aplicador Master-Branch operativo.", 20, Dependencies: [Countries]),
        new(Cities, Cities, "Ciudades", true, false, false, false, false, false, "Catalogo tenant definido en 029_tenant_geography_catalogs.sql. Sin productor/aplicador Master-Branch operativo.", 30, Dependencies: [Countries, Provinces]),
        new(Currencies, Currencies, "Monedas", true, false, false, false, false, false, "Catalogo tenant definido en 031_tenant_commercial_pricing_catalogs.sql. Sin productor/aplicador Master-Branch operativo.", 40),
        new(BusinessPartnerPaymentTerms, BusinessPartnerPaymentTerms, "Condiciones de pago", true, false, false, false, false, false, "Catalogo tenant definido en 024_tenant_business_partners.sql. Sin productor/aplicador Master-Branch operativo.", 50),
        new(SupplierGroups, SupplierGroups, "Grupos de proveedor", true, false, false, false, false, false, "Catalogo tenant definido en 026_tenant_general_supplier_catalogs.sql. Sin productor/aplicador Master-Branch operativo.", 60),
        new(SupplierClasses, SupplierClasses, "Clases de proveedor", true, false, false, false, false, false, "Catalogo tenant definido en 026_tenant_general_supplier_catalogs.sql. Sin productor/aplicador Master-Branch operativo.", 70),
        new(EconomicActivities, EconomicActivities, "Actividades economicas", true, false, false, false, false, false, "Catalogo tenant definido en 026_tenant_general_supplier_catalogs.sql. Sin productor/aplicador Master-Branch operativo.", 80),
        new(Zones, Zones, "Zonas", true, false, false, false, false, false, "Catalogo tenant definido en 026_tenant_general_supplier_catalogs.sql. Sin productor/aplicador Master-Branch operativo.", 90),
        new(SupplyMethods, SupplyMethods, "Metodos de abastecimiento", true, false, false, false, false, false, "Catalogo tenant definido en 026_tenant_general_supplier_catalogs.sql. Sin productor/aplicador Master-Branch operativo.", 100),
        new(BusinessPartner, BusinessPartner, "Socios de negocio", true, true, true, true, true, true, "Productor BusinessPartnerSyncPublisher y aplicador BusinessPartnerSyncApplyRepository existentes; alcance limitado, no BusinessPartners completos.", 200),
        new(Item, Item, "Articulos", true, true, true, true, true, true, "Productor ItemSyncPublisher y aplicador ItemSyncApplyRepository existentes; alcance maestro limitado.", 210),
        new(Warehouse, Warehouse, "Almacenes", true, true, true, true, true, true, "Productor WarehouseSyncPublisher y aplicador WarehouseSyncApplyRepository existentes.", 220)
    ];

    public static bool IsKnown(string entityCode)
    {
        return InitialCatalog.Any(item => string.Equals(item.EntityCode, entityCode, StringComparison.OrdinalIgnoreCase));
    }
}
