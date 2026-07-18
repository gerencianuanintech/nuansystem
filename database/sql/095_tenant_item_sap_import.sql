/* Persistencia idempotente del vinculo SAP Business One para articulos. */

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_ITEMS_CREAR
    @Code nvarchar(50), @Name nvarchar(200), @Description nvarchar(500) = NULL,
    @ItemGroupId int = NULL, @ItemFamilyId int = NULL, @ItemType nvarchar(30),
    @InventoryUnitOfMeasureId int = NULL, @PurchaseUnitOfMeasureId int = NULL, @SalesUnitOfMeasureId int = NULL,
    @IsPurchaseItem bit, @IsSalesItem bit, @IsInventoryItem bit,
    @PurchaseTaxId int = NULL, @SalesTaxId int = NULL,
    @ValuationMethod nvarchar(30), @ManagedBy nvarchar(20), @BatchSerialManagementMethod nvarchar(30),
    @PreferredVendorCode nvarchar(50) = NULL, @VendorCatalogCode nvarchar(80) = NULL,
    @BaseSalesPrice decimal(19,6), @ReferenceCost decimal(19,6),
    @PurchaseFactor decimal(19,6), @SalesFactor decimal(19,6),
    @AllowDiscount bit, @AllowSaleWithoutStock bit, @Remarks nvarchar(1000) = NULL, @IsActive bit,
    @GlobalId uniqueidentifier = NULL, @ExternalSystem nvarchar(50) = NULL,
    @ExternalCode nvarchar(100) = NULL, @SapCode nvarchar(100) = NULL,
    @BarcodesJson nvarchar(max) = NULL, @WarehousesJson nvarchar(max) = NULL,
    @CreatedByUserId int = NULL, @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;

    INSERT INTO dbo.Items
    (
        GlobalId, Code, Name, ExternalSystem, ExternalCode, SapCode, Description,
        ItemGroupId, ItemFamilyId, ItemType,
        InventoryUnitOfMeasureId, PurchaseUnitOfMeasureId, SalesUnitOfMeasureId,
        IsPurchaseItem, IsSalesItem, IsInventoryItem, PurchaseTaxId, SalesTaxId,
        ValuationMethod, ManagedBy, BatchSerialManagementMethod,
        PreferredVendorCode, VendorCatalogCode, BaseSalesPrice, ReferenceCost,
        PurchaseFactor, SalesFactor, AllowDiscount, AllowSaleWithoutStock, Remarks,
        IsActive, CreatedByUserId, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        COALESCE(@GlobalId, NEWID()), @Code, @Name, @ExternalSystem, @ExternalCode, @SapCode, @Description,
        @ItemGroupId, @ItemFamilyId, @ItemType,
        @InventoryUnitOfMeasureId, @PurchaseUnitOfMeasureId, @SalesUnitOfMeasureId,
        @IsPurchaseItem, @IsSalesItem, @IsInventoryItem, @PurchaseTaxId, @SalesTaxId,
        @ValuationMethod, @ManagedBy, @BatchSerialManagementMethod,
        @PreferredVendorCode, @VendorCatalogCode, @BaseSalesPrice, @ReferenceCost,
        @PurchaseFactor, @SalesFactor, @AllowDiscount, @AllowSaleWithoutStock, @Remarks,
        @IsActive, @CreatedByUserId, @CreatedByUserName, SYSUTCDATETIME()
    );

    DECLARE @Id int = CAST(SCOPE_IDENTITY() AS int);

    INSERT INTO dbo.ItemBarcodes (ItemId, Barcode, UnitOfMeasureId, BarcodeType, ConversionFactor, IsMain, IsActive)
    SELECT @Id, Barcode, UnitOfMeasureId, BarcodeType, ConversionFactor, IsMain, IsActive
    FROM OPENJSON(ISNULL(@BarcodesJson, N'[]'))
    WITH
    (
        Barcode nvarchar(120) '$.barcode', UnitOfMeasureId int '$.unitOfMeasureId',
        BarcodeType nvarchar(40) '$.barcodeType', ConversionFactor decimal(19,6) '$.conversionFactor',
        IsMain bit '$.isMain', IsActive bit '$.isActive'
    )
    WHERE NULLIF(Barcode, N'') IS NOT NULL;

    INSERT INTO dbo.ItemWarehouses
        (ItemId, WarehouseId, MinimumStock, MaximumStock, RequiredStock, ReorderPoint,
         DefaultLocationCode, WarehouseCost, IsDefaultWarehouse, IsLocked, IsActive)
    SELECT @Id, WarehouseId, MinimumStock, MaximumStock, RequiredStock, ReorderPoint,
        DefaultLocationCode, WarehouseCost, IsDefaultWarehouse, IsLocked, IsActive
    FROM OPENJSON(ISNULL(@WarehousesJson, N'[]'))
    WITH
    (
        WarehouseId int '$.warehouseId', MinimumStock decimal(19,6) '$.minimumStock',
        MaximumStock decimal(19,6) '$.maximumStock', RequiredStock decimal(19,6) '$.requiredStock',
        ReorderPoint decimal(19,6) '$.reorderPoint', DefaultLocationCode nvarchar(80) '$.defaultLocationCode',
        WarehouseCost decimal(19,6) '$.warehouseCost', IsDefaultWarehouse bit '$.isDefaultWarehouse',
        IsLocked bit '$.isLocked', IsActive bit '$.isActive'
    )
    WHERE WarehouseId IS NOT NULL;

    INSERT INTO dbo.AuditInventoryChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    VALUES (N'Items', CONVERT(nvarchar(80), @Id), N'INSERT', N'Code', NULL, @Code, @CreatedByUserId, @CreatedByUserName);

    COMMIT TRANSACTION;
    SELECT @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_ITEMS_ACTUALIZAR
    @Id int, @Code nvarchar(50), @Name nvarchar(200), @Description nvarchar(500) = NULL,
    @ItemGroupId int = NULL, @ItemFamilyId int = NULL, @ItemType nvarchar(30),
    @InventoryUnitOfMeasureId int = NULL, @PurchaseUnitOfMeasureId int = NULL, @SalesUnitOfMeasureId int = NULL,
    @IsPurchaseItem bit, @IsSalesItem bit, @IsInventoryItem bit,
    @PurchaseTaxId int = NULL, @SalesTaxId int = NULL,
    @ValuationMethod nvarchar(30), @ManagedBy nvarchar(20), @BatchSerialManagementMethod nvarchar(30),
    @PreferredVendorCode nvarchar(50) = NULL, @VendorCatalogCode nvarchar(80) = NULL,
    @BaseSalesPrice decimal(19,6), @ReferenceCost decimal(19,6),
    @PurchaseFactor decimal(19,6), @SalesFactor decimal(19,6),
    @AllowDiscount bit, @AllowSaleWithoutStock bit, @Remarks nvarchar(1000) = NULL, @IsActive bit,
    @ExternalSystem nvarchar(50) = NULL, @ExternalCode nvarchar(100) = NULL, @SapCode nvarchar(100) = NULL,
    @BarcodesJson nvarchar(max) = NULL, @WarehousesJson nvarchar(max) = NULL,
    @UpdatedByUserId int = NULL, @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;

    DECLARE @OldCode nvarchar(50);
    SELECT @OldCode = Code FROM dbo.Items WHERE Id = @Id AND IsDeleted = 0;
    IF @OldCode IS NULL
    BEGIN
        ROLLBACK TRANSACTION;
        SELECT 0;
        RETURN;
    END;

    UPDATE dbo.Items
    SET Code = @Code, Name = @Name, Description = @Description,
        ItemGroupId = @ItemGroupId, ItemFamilyId = @ItemFamilyId, ItemType = @ItemType,
        InventoryUnitOfMeasureId = @InventoryUnitOfMeasureId,
        PurchaseUnitOfMeasureId = @PurchaseUnitOfMeasureId, SalesUnitOfMeasureId = @SalesUnitOfMeasureId,
        IsPurchaseItem = @IsPurchaseItem, IsSalesItem = @IsSalesItem, IsInventoryItem = @IsInventoryItem,
        PurchaseTaxId = @PurchaseTaxId, SalesTaxId = @SalesTaxId,
        ValuationMethod = @ValuationMethod, ManagedBy = @ManagedBy,
        BatchSerialManagementMethod = @BatchSerialManagementMethod,
        PreferredVendorCode = @PreferredVendorCode, VendorCatalogCode = @VendorCatalogCode,
        BaseSalesPrice = @BaseSalesPrice, ReferenceCost = @ReferenceCost,
        PurchaseFactor = @PurchaseFactor, SalesFactor = @SalesFactor,
        AllowDiscount = @AllowDiscount, AllowSaleWithoutStock = @AllowSaleWithoutStock,
        Remarks = @Remarks, IsActive = @IsActive,
        ExternalSystem = COALESCE(@ExternalSystem, ExternalSystem),
        ExternalCode = COALESCE(@ExternalCode, ExternalCode),
        SapCode = COALESCE(@SapCode, SapCode),
        UpdatedByUserId = @UpdatedByUserId, UpdatedByUserName = @UpdatedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id AND IsDeleted = 0;

    UPDATE dbo.ItemBarcodes SET IsDeleted = 1 WHERE ItemId = @Id;
    INSERT INTO dbo.ItemBarcodes (ItemId, Barcode, UnitOfMeasureId, BarcodeType, ConversionFactor, IsMain, IsActive)
    SELECT @Id, Barcode, UnitOfMeasureId, BarcodeType, ConversionFactor, IsMain, IsActive
    FROM OPENJSON(ISNULL(@BarcodesJson, N'[]'))
    WITH
    (
        Barcode nvarchar(120) '$.barcode', UnitOfMeasureId int '$.unitOfMeasureId',
        BarcodeType nvarchar(40) '$.barcodeType', ConversionFactor decimal(19,6) '$.conversionFactor',
        IsMain bit '$.isMain', IsActive bit '$.isActive'
    )
    WHERE NULLIF(Barcode, N'') IS NOT NULL;

    UPDATE dbo.ItemWarehouses SET IsDeleted = 1 WHERE ItemId = @Id;
    INSERT INTO dbo.ItemWarehouses
        (ItemId, WarehouseId, MinimumStock, MaximumStock, RequiredStock, ReorderPoint,
         DefaultLocationCode, WarehouseCost, IsDefaultWarehouse, IsLocked, IsActive)
    SELECT @Id, WarehouseId, MinimumStock, MaximumStock, RequiredStock, ReorderPoint,
        DefaultLocationCode, WarehouseCost, IsDefaultWarehouse, IsLocked, IsActive
    FROM OPENJSON(ISNULL(@WarehousesJson, N'[]'))
    WITH
    (
        WarehouseId int '$.warehouseId', MinimumStock decimal(19,6) '$.minimumStock',
        MaximumStock decimal(19,6) '$.maximumStock', RequiredStock decimal(19,6) '$.requiredStock',
        ReorderPoint decimal(19,6) '$.reorderPoint', DefaultLocationCode nvarchar(80) '$.defaultLocationCode',
        WarehouseCost decimal(19,6) '$.warehouseCost', IsDefaultWarehouse bit '$.isDefaultWarehouse',
        IsLocked bit '$.isLocked', IsActive bit '$.isActive'
    )
    WHERE WarehouseId IS NOT NULL;

    INSERT INTO dbo.AuditInventoryChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    VALUES (N'Items', CONVERT(nvarchar(80), @Id), N'UPDATE', N'Code', @OldCode, @Code, @UpdatedByUserId, @UpdatedByUserName);

    COMMIT TRANSACTION;
    SELECT 1;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SchemaHistory WHERE Version = N'20260717.01')
BEGIN
    INSERT INTO dbo.SchemaHistory (Version, Description)
    VALUES (N'20260717.01', N'Persistencia de referencias SAP para importacion de articulos');
END;
GO
