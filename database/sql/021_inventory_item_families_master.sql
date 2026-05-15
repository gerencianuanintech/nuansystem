/*
    Maestro de Lineas/Familias de Articulos.
    Cada linea/familia pertenece obligatoriamente a un grupo de articulos.
*/

IF OBJECT_ID(N'dbo.ItemFamilies', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ItemFamilies
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ItemFamilies PRIMARY KEY,
        ItemGroupId int NOT NULL,
        Code nvarchar(50) NOT NULL,
        Name nvarchar(150) NOT NULL,
        Description nvarchar(500) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_ItemFamilies_IsActive DEFAULT 1,
        SapFamilyCode nvarchar(100) NULL,
        SapCode nvarchar(50) NULL,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_ItemFamilies_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_ItemFamilies_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT FK_ItemFamilies_ItemGroups FOREIGN KEY (ItemGroupId) REFERENCES dbo.ItemGroups(Id)
    );
END;
GO

IF COL_LENGTH('dbo.ItemFamilies', 'ItemGroupId') IS NULL
    ALTER TABLE dbo.ItemFamilies ADD ItemGroupId int NOT NULL CONSTRAINT DF_ItemFamilies_ItemGroupId DEFAULT 1;
GO

IF COL_LENGTH('dbo.ItemFamilies', 'SapFamilyCode') IS NULL
    ALTER TABLE dbo.ItemFamilies ADD SapFamilyCode nvarchar(100) NULL;
GO

IF COL_LENGTH('dbo.ItemFamilies', 'SapCode') IS NULL
    ALTER TABLE dbo.ItemFamilies ADD SapCode nvarchar(50) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_ItemFamilies_ItemGroups')
BEGIN
    ALTER TABLE dbo.ItemFamilies ADD CONSTRAINT FK_ItemFamilies_ItemGroups FOREIGN KEY (ItemGroupId) REFERENCES dbo.ItemGroups(Id);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_ItemFamilies_ItemGroup_Code_Active' AND object_id = OBJECT_ID(N'dbo.ItemFamilies'))
BEGIN
    CREATE UNIQUE INDEX UX_ItemFamilies_ItemGroup_Code_Active ON dbo.ItemFamilies (ItemGroupId, Code) WHERE IsDeleted = 0;
END;
GO

IF COL_LENGTH('dbo.Items', 'ItemFamilyId') IS NULL
    ALTER TABLE dbo.Items ADD ItemFamilyId int NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Items_ItemFamilies')
BEGIN
    ALTER TABLE dbo.Items ADD CONSTRAINT FK_Items_ItemFamilies FOREIGN KEY (ItemFamilyId) REFERENCES dbo.ItemFamilies(Id);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_FAMILIES_LISTAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        family.Id,
        family.ItemGroupId,
        itemGroup.Code AS ItemGroupCode,
        itemGroup.Name AS ItemGroupName,
        family.Code,
        family.Name,
        family.Description,
        family.IsActive,
        family.SapFamilyCode,
        family.SapCode,
        family.CreatedByUserId,
        family.CreatedByUserName,
        family.CreatedAt,
        family.UpdatedByUserId,
        family.UpdatedByUserName,
        family.UpdatedAt,
        family.DeletedByUserId,
        family.DeletedByUserName,
        family.DeletedAt
    FROM dbo.ItemFamilies family
    INNER JOIN dbo.ItemGroups itemGroup ON itemGroup.Id = family.ItemGroupId
    WHERE family.IsDeleted = 0
    ORDER BY itemGroup.Name, family.Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_FAMILIES_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        family.Id,
        family.ItemGroupId,
        itemGroup.Code AS ItemGroupCode,
        itemGroup.Name AS ItemGroupName,
        family.Code,
        family.Name,
        family.Description,
        family.IsActive,
        family.SapFamilyCode,
        family.SapCode,
        family.CreatedByUserId,
        family.CreatedByUserName,
        family.CreatedAt,
        family.UpdatedByUserId,
        family.UpdatedByUserName,
        family.UpdatedAt,
        family.DeletedByUserId,
        family.DeletedByUserName,
        family.DeletedAt
    FROM dbo.ItemFamilies family
    INNER JOIN dbo.ItemGroups itemGroup ON itemGroup.Id = family.ItemGroupId
    WHERE family.Id = @Id
      AND family.IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_FAMILIES_BUSCARPORGRUPO
    @ItemGroupId int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        family.Id,
        family.ItemGroupId,
        itemGroup.Code AS ItemGroupCode,
        itemGroup.Name AS ItemGroupName,
        family.Code,
        family.Name,
        family.Description,
        family.IsActive,
        family.SapFamilyCode,
        family.SapCode,
        family.CreatedByUserId,
        family.CreatedByUserName,
        family.CreatedAt,
        family.UpdatedByUserId,
        family.UpdatedByUserName,
        family.UpdatedAt,
        family.DeletedByUserId,
        family.DeletedByUserName,
        family.DeletedAt
    FROM dbo.ItemFamilies family
    INNER JOIN dbo.ItemGroups itemGroup ON itemGroup.Id = family.ItemGroupId
    WHERE family.ItemGroupId = @ItemGroupId
      AND family.IsDeleted = 0
      AND family.IsActive = 1
    ORDER BY family.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_FAMILIESBUSCARPORCODIGO
    @ItemGroupId int,
    @Code nvarchar(50),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM dbo.ItemFamilies
    WHERE ItemGroupId = @ItemGroupId
      AND Code = @Code
      AND IsDeleted = 0
      AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_ITEM_FAMILIES_CREAR
    @ItemGroupId int,
    @Code nvarchar(50),
    @Name nvarchar(150),
    @Description nvarchar(500) = NULL,
    @IsActive bit,
    @SapFamilyCode nvarchar(100) = NULL,
    @SapCode nvarchar(50) = NULL,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.ItemFamilies
    (
        ItemGroupId,
        Code,
        Name,
        Description,
        IsActive,
        SapFamilyCode,
        SapCode,
        CreatedByUserId,
        CreatedByUserName
    )
    VALUES
    (
        @ItemGroupId,
        @Code,
        @Name,
        @Description,
        @IsActive,
        @SapFamilyCode,
        @SapCode,
        @CreatedByUserId,
        @CreatedByUserName
    );

    SELECT CONVERT(int, SCOPE_IDENTITY());
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_ITEM_FAMILIES_ACTUALIZAR
    @Id int,
    @ItemGroupId int,
    @Code nvarchar(50),
    @Name nvarchar(150),
    @Description nvarchar(500) = NULL,
    @IsActive bit,
    @SapFamilyCode nvarchar(100) = NULL,
    @SapCode nvarchar(50) = NULL,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.ItemFamilies
    SET
        ItemGroupId = @ItemGroupId,
        Code = @Code,
        Name = @Name,
        Description = @Description,
        IsActive = @IsActive,
        SapFamilyCode = @SapFamilyCode,
        SapCode = @SapCode,
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_ITEM_FAMILIES_ELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.ItemFamilies
    SET
        IsDeleted = 1,
        DeletedByUserId = @DeletedByUserId,
        DeletedByUserName = @DeletedByUserName,
        DeletedAt = SYSUTCDATETIME()
    WHERE Id = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEMS_LOOKUPS
AS
BEGIN
    SELECT Id, Code, Name FROM dbo.ItemGroups WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT Id, ItemGroupId, Code, Name FROM dbo.ItemFamilies WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT Id, Code, Name FROM dbo.UnitOfMeasures WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT Id, Code, Name, Rate FROM dbo.Taxes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT Id, Code, Name FROM dbo.Warehouses WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEMS_LISTAR
AS
BEGIN
    SELECT
        item.Id, item.Code, item.Name, item.Description,
        item.ItemGroupId, itemGroup.Code AS ItemGroupCode, itemGroup.Name AS ItemGroupName,
        item.ItemFamilyId, itemFamily.Code AS ItemFamilyCode, itemFamily.Name AS ItemFamilyName,
        item.ItemType,
        item.InventoryUnitOfMeasureId, inventoryUom.Code AS InventoryUnitOfMeasureCode, inventoryUom.Name AS InventoryUnitOfMeasureName,
        item.PurchaseUnitOfMeasureId, purchaseUom.Code AS PurchaseUnitOfMeasureCode, purchaseUom.Name AS PurchaseUnitOfMeasureName,
        item.SalesUnitOfMeasureId, salesUom.Code AS SalesUnitOfMeasureCode, salesUom.Name AS SalesUnitOfMeasureName,
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
    SELECT
        item.Id, item.Code, item.Name, item.Description,
        item.ItemGroupId, itemGroup.Code AS ItemGroupCode, itemGroup.Name AS ItemGroupName,
        item.ItemFamilyId, itemFamily.Code AS ItemFamilyCode, itemFamily.Name AS ItemFamilyName,
        item.ItemType,
        item.InventoryUnitOfMeasureId, inventoryUom.Code AS InventoryUnitOfMeasureCode, inventoryUom.Name AS InventoryUnitOfMeasureName,
        item.PurchaseUnitOfMeasureId, purchaseUom.Code AS PurchaseUnitOfMeasureCode, purchaseUom.Name AS PurchaseUnitOfMeasureName,
        item.SalesUnitOfMeasureId, salesUom.Code AS SalesUnitOfMeasureCode, salesUom.Name AS SalesUnitOfMeasureName,
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

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_ITEMS_CREAR
    @Code nvarchar(50),
    @Name nvarchar(200),
    @Description nvarchar(500) = NULL,
    @ItemGroupId int = NULL,
    @ItemFamilyId int = NULL,
    @ItemType nvarchar(30),
    @InventoryUnitOfMeasureId int = NULL,
    @PurchaseUnitOfMeasureId int = NULL,
    @SalesUnitOfMeasureId int = NULL,
    @IsPurchaseItem bit,
    @IsSalesItem bit,
    @IsInventoryItem bit,
    @PurchaseTaxId int = NULL,
    @SalesTaxId int = NULL,
    @ValuationMethod nvarchar(30),
    @ManagedBy nvarchar(20),
    @BatchSerialManagementMethod nvarchar(30),
    @PreferredVendorCode nvarchar(50) = NULL,
    @VendorCatalogCode nvarchar(80) = NULL,
    @BaseSalesPrice decimal(19,6),
    @ReferenceCost decimal(19,6),
    @PurchaseFactor decimal(19,6),
    @SalesFactor decimal(19,6),
    @AllowDiscount bit,
    @AllowSaleWithoutStock bit,
    @Remarks nvarchar(1000) = NULL,
    @IsActive bit,
    @BarcodesJson nvarchar(max) = NULL,
    @WarehousesJson nvarchar(max) = NULL,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;

    INSERT INTO dbo.Items
    (
        Code, Name, Description, ItemGroupId, ItemFamilyId, ItemType,
        InventoryUnitOfMeasureId, PurchaseUnitOfMeasureId, SalesUnitOfMeasureId,
        IsPurchaseItem, IsSalesItem, IsInventoryItem, PurchaseTaxId, SalesTaxId,
        ValuationMethod, ManagedBy, BatchSerialManagementMethod,
        PreferredVendorCode, VendorCatalogCode, BaseSalesPrice, ReferenceCost,
        PurchaseFactor, SalesFactor, AllowDiscount, AllowSaleWithoutStock, Remarks,
        IsActive, CreatedByUserId, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        @Code, @Name, @Description, @ItemGroupId, @ItemFamilyId, @ItemType,
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
        Barcode nvarchar(120) '$.barcode',
        UnitOfMeasureId int '$.unitOfMeasureId',
        BarcodeType nvarchar(40) '$.barcodeType',
        ConversionFactor decimal(19,6) '$.conversionFactor',
        IsMain bit '$.isMain',
        IsActive bit '$.isActive'
    )
    WHERE NULLIF(Barcode, N'') IS NOT NULL;

    INSERT INTO dbo.ItemWarehouses (ItemId, WarehouseId, MinimumStock, MaximumStock, RequiredStock, ReorderPoint, DefaultLocationCode, WarehouseCost, IsDefaultWarehouse, IsLocked, IsActive)
    SELECT @Id, WarehouseId, MinimumStock, MaximumStock, RequiredStock, ReorderPoint, DefaultLocationCode, WarehouseCost, IsDefaultWarehouse, IsLocked, IsActive
    FROM OPENJSON(ISNULL(@WarehousesJson, N'[]'))
    WITH
    (
        WarehouseId int '$.warehouseId',
        MinimumStock decimal(19,6) '$.minimumStock',
        MaximumStock decimal(19,6) '$.maximumStock',
        RequiredStock decimal(19,6) '$.requiredStock',
        ReorderPoint decimal(19,6) '$.reorderPoint',
        DefaultLocationCode nvarchar(80) '$.defaultLocationCode',
        WarehouseCost decimal(19,6) '$.warehouseCost',
        IsDefaultWarehouse bit '$.isDefaultWarehouse',
        IsLocked bit '$.isLocked',
        IsActive bit '$.isActive'
    )
    WHERE WarehouseId IS NOT NULL;

    INSERT INTO dbo.AuditInventoryChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    VALUES (N'Items', CONVERT(nvarchar(80), @Id), N'INSERT', N'Code', NULL, @Code, @CreatedByUserId, @CreatedByUserName);

    COMMIT TRANSACTION;
    SELECT @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_ITEMS_ACTUALIZAR
    @Id int,
    @Code nvarchar(50),
    @Name nvarchar(200),
    @Description nvarchar(500) = NULL,
    @ItemGroupId int = NULL,
    @ItemFamilyId int = NULL,
    @ItemType nvarchar(30),
    @InventoryUnitOfMeasureId int = NULL,
    @PurchaseUnitOfMeasureId int = NULL,
    @SalesUnitOfMeasureId int = NULL,
    @IsPurchaseItem bit,
    @IsSalesItem bit,
    @IsInventoryItem bit,
    @PurchaseTaxId int = NULL,
    @SalesTaxId int = NULL,
    @ValuationMethod nvarchar(30),
    @ManagedBy nvarchar(20),
    @BatchSerialManagementMethod nvarchar(30),
    @PreferredVendorCode nvarchar(50) = NULL,
    @VendorCatalogCode nvarchar(80) = NULL,
    @BaseSalesPrice decimal(19,6),
    @ReferenceCost decimal(19,6),
    @PurchaseFactor decimal(19,6),
    @SalesFactor decimal(19,6),
    @AllowDiscount bit,
    @AllowSaleWithoutStock bit,
    @Remarks nvarchar(1000) = NULL,
    @IsActive bit,
    @BarcodesJson nvarchar(max) = NULL,
    @WarehousesJson nvarchar(max) = NULL,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;

    DECLARE @OldCode nvarchar(50);
    SELECT @OldCode = Code
    FROM dbo.Items
    WHERE Id = @Id AND IsDeleted = 0;

    IF @OldCode IS NULL
    BEGIN
        ROLLBACK TRANSACTION;
        SELECT 0;
        RETURN;
    END;

    UPDATE dbo.Items
    SET
        Code = @Code,
        Name = @Name,
        Description = @Description,
        ItemGroupId = @ItemGroupId,
        ItemFamilyId = @ItemFamilyId,
        ItemType = @ItemType,
        InventoryUnitOfMeasureId = @InventoryUnitOfMeasureId,
        PurchaseUnitOfMeasureId = @PurchaseUnitOfMeasureId,
        SalesUnitOfMeasureId = @SalesUnitOfMeasureId,
        IsPurchaseItem = @IsPurchaseItem,
        IsSalesItem = @IsSalesItem,
        IsInventoryItem = @IsInventoryItem,
        PurchaseTaxId = @PurchaseTaxId,
        SalesTaxId = @SalesTaxId,
        ValuationMethod = @ValuationMethod,
        ManagedBy = @ManagedBy,
        BatchSerialManagementMethod = @BatchSerialManagementMethod,
        PreferredVendorCode = @PreferredVendorCode,
        VendorCatalogCode = @VendorCatalogCode,
        BaseSalesPrice = @BaseSalesPrice,
        ReferenceCost = @ReferenceCost,
        PurchaseFactor = @PurchaseFactor,
        SalesFactor = @SalesFactor,
        AllowDiscount = @AllowDiscount,
        AllowSaleWithoutStock = @AllowSaleWithoutStock,
        Remarks = @Remarks,
        IsActive = @IsActive,
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id
      AND IsDeleted = 0;

    UPDATE dbo.ItemBarcodes SET IsDeleted = 1 WHERE ItemId = @Id;
    INSERT INTO dbo.ItemBarcodes (ItemId, Barcode, UnitOfMeasureId, BarcodeType, ConversionFactor, IsMain, IsActive)
    SELECT @Id, Barcode, UnitOfMeasureId, BarcodeType, ConversionFactor, IsMain, IsActive
    FROM OPENJSON(ISNULL(@BarcodesJson, N'[]'))
    WITH
    (
        Barcode nvarchar(120) '$.barcode',
        UnitOfMeasureId int '$.unitOfMeasureId',
        BarcodeType nvarchar(40) '$.barcodeType',
        ConversionFactor decimal(19,6) '$.conversionFactor',
        IsMain bit '$.isMain',
        IsActive bit '$.isActive'
    )
    WHERE NULLIF(Barcode, N'') IS NOT NULL;

    UPDATE dbo.ItemWarehouses SET IsDeleted = 1 WHERE ItemId = @Id;
    INSERT INTO dbo.ItemWarehouses (ItemId, WarehouseId, MinimumStock, MaximumStock, RequiredStock, ReorderPoint, DefaultLocationCode, WarehouseCost, IsDefaultWarehouse, IsLocked, IsActive)
    SELECT @Id, WarehouseId, MinimumStock, MaximumStock, RequiredStock, ReorderPoint, DefaultLocationCode, WarehouseCost, IsDefaultWarehouse, IsLocked, IsActive
    FROM OPENJSON(ISNULL(@WarehousesJson, N'[]'))
    WITH
    (
        WarehouseId int '$.warehouseId',
        MinimumStock decimal(19,6) '$.minimumStock',
        MaximumStock decimal(19,6) '$.maximumStock',
        RequiredStock decimal(19,6) '$.requiredStock',
        ReorderPoint decimal(19,6) '$.reorderPoint',
        DefaultLocationCode nvarchar(80) '$.defaultLocationCode',
        WarehouseCost decimal(19,6) '$.warehouseCost',
        IsDefaultWarehouse bit '$.isDefaultWarehouse',
        IsLocked bit '$.isLocked',
        IsActive bit '$.isActive'
    )
    WHERE WarehouseId IS NOT NULL;

    INSERT INTO dbo.AuditInventoryChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    VALUES (N'Items', CONVERT(nvarchar(80), @Id), N'UPDATE', N'Code', @OldCode, @Code, @UpdatedByUserId, @UpdatedByUserName);

    COMMIT TRANSACTION;
    SELECT 1;
END;
GO
