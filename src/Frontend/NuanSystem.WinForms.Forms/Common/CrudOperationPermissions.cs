using NuanSystem.Shared.Constants;

namespace NuanSystem.WinForms.Forms.Common;

public sealed record CrudOperationPermissions(
    string Read,
    string Create,
    string Update,
    string Delete)
{
    public static CrudOperationPermissions BusinessPartners { get; } = new(
        PermissionCodes.BusinessPartnersRead,
        PermissionCodes.BusinessPartnersManage,
        PermissionCodes.BusinessPartnersManage,
        PermissionCodes.BusinessPartnersManage);

    public static CrudOperationPermissions Customers { get; } = new(
        PermissionCodes.CustomersRead,
        PermissionCodes.CustomersManage,
        PermissionCodes.CustomersManage,
        PermissionCodes.CustomersManage);

    public static CrudOperationPermissions Items { get; } = new(
        PermissionCodes.ItemsRead,
        PermissionCodes.ItemsManage,
        PermissionCodes.ItemsManage,
        PermissionCodes.ItemsManage);

    public static CrudOperationPermissions PurchaseOrders { get; } = new(
        PermissionCodes.PurchaseOrdersRead,
        PermissionCodes.PurchaseOrdersManage,
        PermissionCodes.PurchaseOrdersManage,
        PermissionCodes.PurchaseOrdersManage);

    public static CrudOperationPermissions SecurityDocumentSeries { get; } = new(
        PermissionCodes.DocumentsSeriesRead,
        PermissionCodes.DocumentsSeriesCreate,
        PermissionCodes.DocumentsSeriesUpdate,
        PermissionCodes.DocumentsSeriesDelete);

    public static CrudOperationPermissions OperationalCatalogs { get; } = new(
        PermissionCodes.OperationalCatalogsRead,
        PermissionCodes.OperationalCatalogsManage,
        PermissionCodes.OperationalCatalogsManage,
        PermissionCodes.OperationalCatalogsManage);

    public static CrudOperationPermissions ItemGroups { get; } = new(
        PermissionCodes.ItemsRead,
        PermissionCodes.ItemsManage,
        PermissionCodes.ItemsManage,
        PermissionCodes.ItemsManage);

    public static CrudOperationPermissions ItemFamilies { get; } = new(
        PermissionCodes.GeneralInventoryItemFamiliesRead,
        PermissionCodes.GeneralInventoryItemFamiliesManage,
        PermissionCodes.GeneralInventoryItemFamiliesManage,
        PermissionCodes.GeneralInventoryItemFamiliesManage);

    public static CrudOperationPermissions ItemSubgroups { get; } = new(
        PermissionCodes.GeneralInventoryItemSubgroupsRead,
        PermissionCodes.GeneralInventoryItemSubgroupsManage,
        PermissionCodes.GeneralInventoryItemSubgroupsManage,
        PermissionCodes.GeneralInventoryItemSubgroupsManage);

    public static CrudOperationPermissions ItemOrigins { get; } = new(
        PermissionCodes.GeneralInventoryItemOriginsRead,
        PermissionCodes.GeneralInventoryItemOriginsManage,
        PermissionCodes.GeneralInventoryItemOriginsManage,
        PermissionCodes.GeneralInventoryItemOriginsManage);

    public static CrudOperationPermissions ReplenishmentMethods { get; } = new(
        PermissionCodes.GeneralInventoryReplenishmentMethodsRead,
        PermissionCodes.GeneralInventoryReplenishmentMethodsManage,
        PermissionCodes.GeneralInventoryReplenishmentMethodsManage,
        PermissionCodes.GeneralInventoryReplenishmentMethodsManage);

    public static CrudOperationPermissions StorageConditions { get; } = new(
        PermissionCodes.GeneralInventoryStorageConditionsRead,
        PermissionCodes.GeneralInventoryStorageConditionsManage,
        PermissionCodes.GeneralInventoryStorageConditionsManage,
        PermissionCodes.GeneralInventoryStorageConditionsManage);

    public static CrudOperationPermissions ItemBrands { get; } = new(
        PermissionCodes.GeneralInventoryItemBrandsRead,
        PermissionCodes.GeneralInventoryItemBrandsManage,
        PermissionCodes.GeneralInventoryItemBrandsManage,
        PermissionCodes.GeneralInventoryItemBrandsManage);

    public static CrudOperationPermissions UnitMeasures { get; } = new(
        PermissionCodes.GeneralInventoryUnitMeasuresRead,
        PermissionCodes.GeneralInventoryUnitMeasuresManage,
        PermissionCodes.GeneralInventoryUnitMeasuresManage,
        PermissionCodes.GeneralInventoryUnitMeasuresManage);

    public static CrudOperationPermissions InventoryWarehouses { get; } = new(
        PermissionCodes.GeneralInventoryWarehousesRead,
        PermissionCodes.GeneralInventoryWarehousesManage,
        PermissionCodes.GeneralInventoryWarehousesManage,
        PermissionCodes.GeneralInventoryWarehousesManage);

    public static CrudOperationPermissions ChartOfAccounts { get; } = new(
        PermissionCodes.AccountingRead,
        PermissionCodes.AccountingManage,
        PermissionCodes.AccountingManage,
        PermissionCodes.AccountingManage);
}
