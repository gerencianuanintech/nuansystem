namespace NuanSystem.Application.Features.SapSync.Mapping;

public enum SapMappingDirection
{
    SapToErp,
    ErpToSap,
    Both,
    Generated
}

public sealed record SapFieldMappingDefinition(
    string SourceField,
    string DestinationField,
    SapMappingDirection Direction,
    bool IsRequired,
    bool IsReadOnly,
    string TransformationKey);

public static class SapMasterDataMappingContract
{
    public static IReadOnlyCollection<string> EntityCodes { get; } =
        ["Suppliers", "Customers", "Items"];

    public static IReadOnlyCollection<SapFieldMappingDefinition> SupplierMappings { get; } =
    [
        new("CardCode", "SapCardCode", SapMappingDirection.SapToErp, true, true, "Trim"),
        new("Code", "Code", SapMappingDirection.Generated, true, false, "GenerateInternalCode"),
        new("CardName", "Name", SapMappingDirection.Both, true, false, "Trim"),
        new("LicTradNum", "IdentificationNumber", SapMappingDirection.SapToErp, true, false, "TaxIdNormalize"),
        new("Phone1", "Phone", SapMappingDirection.Both, false, false, "Trim"),
        new("E_Mail", "Email", SapMappingDirection.Both, false, false, "LowerInvariant"),
        new("Currency", "PreferredCurrencyCode", SapMappingDirection.SapToErp, false, false, "CatalogLookup"),
        new("ValidFor", "IsActive", SapMappingDirection.SapToErp, true, false, "SapBooleanYN")
    ];

    public static IReadOnlyCollection<SapFieldMappingDefinition> CustomerMappings { get; } =
    [
        new("CardCode", "SapCardCode", SapMappingDirection.SapToErp, true, true, "Trim"),
        new("Code", "Code", SapMappingDirection.Generated, true, false, "GenerateInternalCode"),
        new("CardName", "Name", SapMappingDirection.Both, true, false, "Trim"),
        new("LicTradNum", "IdentificationNumber", SapMappingDirection.SapToErp, true, false, "TaxIdNormalize"),
        new("Phone1", "Phone", SapMappingDirection.Both, false, false, "Trim"),
        new("E_Mail", "Email", SapMappingDirection.Both, false, false, "LowerInvariant"),
        new("Currency", "PreferredCurrencyCode", SapMappingDirection.SapToErp, false, false, "CatalogLookup"),
        new("ValidFor", "IsActive", SapMappingDirection.SapToErp, true, false, "SapBooleanYN")
    ];

    public static IReadOnlyCollection<SapFieldMappingDefinition> ItemMappings { get; } =
    [
        new("ItemCode", "SapCode", SapMappingDirection.SapToErp, true, true, "Trim"),
        new("ItemCode", "Code", SapMappingDirection.Generated, true, false, "GenerateInternalCode"),
        new("ItemName", "Name", SapMappingDirection.Both, true, false, "Trim"),
        new("ItemsGroupCode", "SapItemGroupCode", SapMappingDirection.SapToErp, false, true, "CatalogLookup"),
        new("InventoryUOM", "InventoryUnitCode", SapMappingDirection.SapToErp, false, false, "CatalogLookup"),
        new("PurchaseUnit", "PurchaseUnitCode", SapMappingDirection.SapToErp, false, false, "CatalogLookup"),
        new("SalesUnit", "SalesUnitCode", SapMappingDirection.SapToErp, false, false, "CatalogLookup"),
        new("BarCode", "Barcode", SapMappingDirection.Both, false, false, "Trim"),
        new("PurchaseVATGroup", "PurchaseTaxCode", SapMappingDirection.SapToErp, false, false, "CatalogLookup"),
        new("SalesVATGroup", "SalesTaxCode", SapMappingDirection.SapToErp, false, false, "CatalogLookup"),
        new("PurchaseItem", "IsPurchaseItem", SapMappingDirection.SapToErp, true, false, "SapBoolean"),
        new("SalesItem", "IsSalesItem", SapMappingDirection.SapToErp, true, false, "SapBoolean"),
        new("InventoryItem", "IsInventoryItem", SapMappingDirection.SapToErp, true, false, "SapBoolean"),
        new("ManageSerialNumbers", "ManageSerialNumbers", SapMappingDirection.SapToErp, true, false, "SapBoolean"),
        new("ManageBatchNumbers", "ManageBatchNumbers", SapMappingDirection.SapToErp, true, false, "SapBoolean"),
        new("ItemType", "ItemType", SapMappingDirection.SapToErp, true, false, "Trim"),
        new("Valid", "IsActive", SapMappingDirection.SapToErp, true, false, "SapBoolean"),
        new("Frozen", "IsFrozenInSap", SapMappingDirection.SapToErp, true, true, "SapBoolean")
    ];

    public static string ForeignTaxpayerType => "PASAPORTE";

    public static IReadOnlyCollection<string> SupplierNationalPrefixes { get; } = ["PL", "P"];

    public static IReadOnlyCollection<string> SupplierForeignPrefixes { get; } = ["PE"];

    public static bool CanActivateBoth(bool importExecutable, bool exportExecutable)
        => importExecutable && exportExecutable;
}
