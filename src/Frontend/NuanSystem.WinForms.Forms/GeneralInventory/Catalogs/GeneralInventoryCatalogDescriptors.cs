using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs;

namespace NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;

public static class GeneralInventoryCatalogDescriptors
{
    public static GeneralInventoryCatalogDescriptor Warehouses { get; } = new(
        GeneralInventoryCatalogRoutes.Warehouses,
        "inventory-warehouses",
        "Bodegas",
        "bodega",
        "Codigo de bodega",
        "Nombre de bodega",
        Permissions(PermissionCodes.GeneralInventoryWarehousesRead, PermissionCodes.GeneralInventoryWarehousesManage));

    public static GeneralInventoryCatalogDescriptor ItemTypes { get; } = new(
        GeneralInventoryCatalogRoutes.ItemTypes,
        "inventory-item-types",
        "Tipos de item",
        "tipo de item",
        "Codigo de tipo",
        "Nombre de tipo",
        Permissions(PermissionCodes.GeneralInventoryItemTypesRead, PermissionCodes.GeneralInventoryItemTypesManage));

    public static GeneralInventoryCatalogDescriptor ItemSubgroups { get; } = new(
        GeneralInventoryCatalogRoutes.ItemSubgroups,
        "inventory-item-subgroups",
        "Subgrupos de articulos",
        "subgrupo de articulo",
        "Codigo de subgrupo",
        "Nombre de subgrupo",
        Permissions(PermissionCodes.GeneralInventoryItemSubgroupsRead, PermissionCodes.GeneralInventoryItemSubgroupsManage));

    public static GeneralInventoryCatalogDescriptor SalesChannels { get; } = new(
        GeneralInventoryCatalogRoutes.SalesChannels,
        "inventory-sales-channels",
        "Canales de venta",
        "canal de venta",
        "Codigo de canal",
        "Nombre de canal",
        Permissions(PermissionCodes.GeneralInventorySalesChannelsRead, PermissionCodes.GeneralInventorySalesChannelsManage));

    public static GeneralInventoryCatalogDescriptor WarehouseLocations { get; } = new(
        GeneralInventoryCatalogRoutes.WarehouseLocations,
        "inventory-warehouse-locations",
        "Ubicaciones de bodega",
        "ubicacion de bodega",
        "Codigo de ubicacion",
        "Nombre de ubicacion",
        Permissions(PermissionCodes.GeneralInventoryWarehouseLocationsRead, PermissionCodes.GeneralInventoryWarehouseLocationsManage));

    public static GeneralInventoryCatalogDescriptor StorageZones { get; } = new(
        GeneralInventoryCatalogRoutes.StorageZones,
        "inventory-storage-zones",
        "Zonas de almacenamiento",
        "zona de almacenamiento",
        "Codigo de zona",
        "Nombre de zona",
        Permissions(PermissionCodes.GeneralInventoryStorageZonesRead, PermissionCodes.GeneralInventoryStorageZonesManage));

    public static GeneralInventoryCatalogDescriptor StorageConditions { get; } = new(
        GeneralInventoryCatalogRoutes.StorageConditions,
        "inventory-storage-conditions",
        "Condiciones de almacenamiento",
        "condicion de almacenamiento",
        "Codigo de condicion",
        "Nombre de condicion",
        Permissions(PermissionCodes.GeneralInventoryStorageConditionsRead, PermissionCodes.GeneralInventoryStorageConditionsManage));

    public static GeneralInventoryCatalogDescriptor ReplenishmentMethods { get; } = new(
        GeneralInventoryCatalogRoutes.ReplenishmentMethods,
        "inventory-replenishment-methods",
        "Metodos de reposicion",
        "metodo de reposicion",
        "Codigo de metodo",
        "Nombre de metodo",
        Permissions(PermissionCodes.GeneralInventoryReplenishmentMethodsRead, PermissionCodes.GeneralInventoryReplenishmentMethodsManage));

    public static GeneralInventoryCatalogDescriptor VariantAttributes { get; } = new(
        GeneralInventoryCatalogRoutes.VariantAttributes,
        "inventory-variant-attributes",
        "Atributos de variantes",
        "atributo de variante",
        "Codigo de atributo",
        "Nombre de atributo",
        Permissions(PermissionCodes.GeneralInventoryVariantAttributesRead, PermissionCodes.GeneralInventoryVariantAttributesManage));

    public static GeneralInventoryCatalogDescriptor AttachmentDocumentTypes { get; } = new(
        GeneralInventoryCatalogRoutes.AttachmentDocumentTypes,
        "inventory-attachment-document-types",
        "Tipos de documento de anexos",
        "tipo de documento",
        "Codigo de tipo",
        "Nombre de tipo",
        Permissions(PermissionCodes.GeneralInventoryAttachmentDocumentTypesRead, PermissionCodes.GeneralInventoryAttachmentDocumentTypesManage));

    public static GeneralInventoryCatalogDescriptor AttachmentCategories { get; } = new(
        GeneralInventoryCatalogRoutes.AttachmentCategories,
        "inventory-attachment-categories",
        "Categorias de anexos",
        "categoria de anexo",
        "Codigo de categoria",
        "Nombre de categoria",
        Permissions(PermissionCodes.GeneralInventoryAttachmentCategoriesRead, PermissionCodes.GeneralInventoryAttachmentCategoriesManage));

    private static CrudOperationPermissions Permissions(string readPermission, string managePermission)
    {
        return new CrudOperationPermissions(readPermission, managePermission, managePermission, managePermission);
    }
}
