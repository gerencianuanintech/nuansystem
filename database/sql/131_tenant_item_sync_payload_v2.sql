/*
    Expone las identidades corporativas de las dependencias de Item.
    Los Id locales y los codigos se conservan para UI y compatibilidad;
    la sincronizacion Matriz-Sucursal usa exclusivamente los GlobalId.

    Ejecutar solo en bases tenant. Script idempotente. No ejecutado por esta rama.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.Items', N'U') IS NULL
   OR OBJECT_ID(N'dbo.ItemGroups', N'U') IS NULL
   OR OBJECT_ID(N'dbo.ItemFamilies', N'U') IS NULL
   OR OBJECT_ID(N'dbo.UnitOfMeasures', N'U') IS NULL
    THROW 51131, 'Items and its reference catalogs are required before migration 131.', 1;
GO

IF COL_LENGTH(N'dbo.Items', N'GlobalId') IS NULL
   OR COL_LENGTH(N'dbo.ItemGroups', N'GlobalId') IS NULL
   OR COL_LENGTH(N'dbo.ItemFamilies', N'GlobalId') IS NULL
   OR COL_LENGTH(N'dbo.UnitOfMeasures', N'GlobalId') IS NULL
    THROW 51131, 'GlobalId is required on Item and all synchronized dependencies.', 1;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEMS_LISTAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        item.Id, item.GlobalId, item.Code, item.Name, item.ExternalSystem, item.ExternalCode, item.SapCode, item.Description,
        item.ItemGroupId, itemGroup.GlobalId AS ItemGroupGlobalId,
        itemGroup.Code AS ItemGroupCode, itemGroup.Name AS ItemGroupName,
        item.ItemFamilyId, itemFamily.GlobalId AS ItemFamilyGlobalId,
        itemFamily.Code AS ItemFamilyCode, itemFamily.Name AS ItemFamilyName,
        item.ItemType,
        item.InventoryUnitOfMeasureId, inventoryUom.GlobalId AS InventoryUnitOfMeasureGlobalId,
        inventoryUom.Code AS InventoryUnitOfMeasureCode, inventoryUom.Name AS InventoryUnitOfMeasureName,
        item.PurchaseUnitOfMeasureId, purchaseUom.GlobalId AS PurchaseUnitOfMeasureGlobalId,
        purchaseUom.Code AS PurchaseUnitOfMeasureCode, purchaseUom.Name AS PurchaseUnitOfMeasureName,
        item.SalesUnitOfMeasureId, salesUom.GlobalId AS SalesUnitOfMeasureGlobalId,
        salesUom.Code AS SalesUnitOfMeasureCode, salesUom.Name AS SalesUnitOfMeasureName,
        item.IsPurchaseItem, item.IsSalesItem, item.IsInventoryItem,
        item.PurchaseTaxId, purchaseTax.Code AS PurchaseTaxCode, purchaseTax.Name AS PurchaseTaxName,
        item.SalesTaxId, salesTax.Code AS SalesTaxCode, salesTax.Name AS SalesTaxName,
        item.ValuationMethod, item.ManagedBy, item.BatchSerialManagementMethod,
        item.PreferredVendorCode, item.VendorCatalogCode,
        item.BaseSalesPrice, item.ReferenceCost, item.PurchaseFactor, item.SalesFactor,
        item.AllowDiscount, item.AllowSaleWithoutStock, item.Remarks, item.IsActive,
        item.CreatedByUserId, item.CreatedByUserName, item.CreatedAt,
        item.UpdatedByUserId, item.UpdatedByUserName, item.UpdatedAt,
        item.DeletedByUserId, item.DeletedByUserName, item.DeletedAt
    FROM dbo.Items item
    LEFT JOIN dbo.ItemGroups itemGroup ON itemGroup.Id = item.ItemGroupId
    LEFT JOIN dbo.ItemFamilies itemFamily ON itemFamily.Id = item.ItemFamilyId
    LEFT JOIN dbo.UnitOfMeasures inventoryUom ON inventoryUom.Id = item.InventoryUnitOfMeasureId
    LEFT JOIN dbo.UnitOfMeasures purchaseUom ON purchaseUom.Id = item.PurchaseUnitOfMeasureId
    LEFT JOIN dbo.UnitOfMeasures salesUom ON salesUom.Id = item.SalesUnitOfMeasureId
    LEFT JOIN dbo.Taxes purchaseTax ON purchaseTax.Id = item.PurchaseTaxId
    LEFT JOIN dbo.Taxes salesTax ON salesTax.Id = item.SalesTaxId
    WHERE item.IsDeleted = 0
    ORDER BY item.Code, item.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEMS_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        item.Id, item.GlobalId, item.Code, item.Name, item.ExternalSystem, item.ExternalCode, item.SapCode, item.Description,
        item.ItemGroupId, itemGroup.GlobalId AS ItemGroupGlobalId,
        itemGroup.Code AS ItemGroupCode, itemGroup.Name AS ItemGroupName,
        item.ItemFamilyId, itemFamily.GlobalId AS ItemFamilyGlobalId,
        itemFamily.Code AS ItemFamilyCode, itemFamily.Name AS ItemFamilyName,
        item.ItemType,
        item.InventoryUnitOfMeasureId, inventoryUom.GlobalId AS InventoryUnitOfMeasureGlobalId,
        inventoryUom.Code AS InventoryUnitOfMeasureCode, inventoryUom.Name AS InventoryUnitOfMeasureName,
        item.PurchaseUnitOfMeasureId, purchaseUom.GlobalId AS PurchaseUnitOfMeasureGlobalId,
        purchaseUom.Code AS PurchaseUnitOfMeasureCode, purchaseUom.Name AS PurchaseUnitOfMeasureName,
        item.SalesUnitOfMeasureId, salesUom.GlobalId AS SalesUnitOfMeasureGlobalId,
        salesUom.Code AS SalesUnitOfMeasureCode, salesUom.Name AS SalesUnitOfMeasureName,
        item.IsPurchaseItem, item.IsSalesItem, item.IsInventoryItem,
        item.PurchaseTaxId, purchaseTax.Code AS PurchaseTaxCode, purchaseTax.Name AS PurchaseTaxName,
        item.SalesTaxId, salesTax.Code AS SalesTaxCode, salesTax.Name AS SalesTaxName,
        item.ValuationMethod, item.ManagedBy, item.BatchSerialManagementMethod,
        item.PreferredVendorCode, item.VendorCatalogCode,
        item.BaseSalesPrice, item.ReferenceCost, item.PurchaseFactor, item.SalesFactor,
        item.AllowDiscount, item.AllowSaleWithoutStock, item.Remarks, item.IsActive,
        item.CreatedByUserId, item.CreatedByUserName, item.CreatedAt,
        item.UpdatedByUserId, item.UpdatedByUserName, item.UpdatedAt,
        item.DeletedByUserId, item.DeletedByUserName, item.DeletedAt
    FROM dbo.Items item
    LEFT JOIN dbo.ItemGroups itemGroup ON itemGroup.Id = item.ItemGroupId
    LEFT JOIN dbo.ItemFamilies itemFamily ON itemFamily.Id = item.ItemFamilyId
    LEFT JOIN dbo.UnitOfMeasures inventoryUom ON inventoryUom.Id = item.InventoryUnitOfMeasureId
    LEFT JOIN dbo.UnitOfMeasures purchaseUom ON purchaseUom.Id = item.PurchaseUnitOfMeasureId
    LEFT JOIN dbo.UnitOfMeasures salesUom ON salesUom.Id = item.SalesUnitOfMeasureId
    LEFT JOIN dbo.Taxes purchaseTax ON purchaseTax.Id = item.PurchaseTaxId
    LEFT JOIN dbo.Taxes salesTax ON salesTax.Id = item.SalesTaxId
    WHERE item.Id = @Id
      AND item.IsDeleted = 0;

    SELECT Id, ItemId, Barcode, UnitOfMeasureId, BarcodeType, ConversionFactor, IsMain, IsActive
    FROM dbo.ItemBarcodes
    WHERE ItemId = @Id AND IsDeleted = 0
    ORDER BY IsMain DESC, Barcode;

    SELECT wh.Id, wh.ItemId, wh.WarehouseId, warehouse.Code AS WarehouseCode, warehouse.Name AS WarehouseName,
        wh.MinimumStock, wh.MaximumStock, wh.RequiredStock, wh.ReorderPoint,
        wh.DefaultLocationCode, wh.WarehouseCost, wh.IsDefaultWarehouse, wh.IsLocked, wh.IsActive
    FROM dbo.ItemWarehouses wh
    INNER JOIN dbo.Warehouses warehouse ON warehouse.Id = wh.WarehouseId
    WHERE wh.ItemId = @Id AND wh.IsDeleted = 0
    ORDER BY wh.IsDefaultWarehouse DESC, warehouse.Name;
END;
GO

IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
    THROW 51131, 'SchemaHistory is required before recording migration 131.', 1;

IF NOT EXISTS (SELECT 1 FROM dbo.SchemaHistory WHERE Version = N'20260726.131')
BEGIN
    INSERT INTO dbo.SchemaHistory (Version, Description)
    VALUES (N'20260726.131', N'Expone GlobalId de dependencias para payload Item v2');
END;
GO
