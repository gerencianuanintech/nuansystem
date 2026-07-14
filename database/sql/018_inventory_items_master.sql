/*
    Ejecutar este script dentro de la base de datos de una empresa/tenant.
    Actualiza el maestro de articulos al estandar de mantenimientos con SPs,
    auditoria basica, auditoria detallada y tablas minimas de inventario.
*/

IF OBJECT_ID(N'dbo.UnitOfMeasures', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UnitOfMeasures
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_UnitOfMeasures PRIMARY KEY,
        Code nvarchar(20) NOT NULL,
        Name nvarchar(120) NOT NULL,
        IsActive bit NOT NULL CONSTRAINT DF_UnitOfMeasures_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_UnitOfMeasures_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_UnitOfMeasures_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_UnitOfMeasures_Code_Active' AND object_id = OBJECT_ID(N'dbo.UnitOfMeasures'))
BEGIN
    CREATE UNIQUE INDEX UX_UnitOfMeasures_Code_Active ON dbo.UnitOfMeasures (Code) WHERE IsDeleted = 0;
END;
GO

IF OBJECT_ID(N'dbo.ItemGroups', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ItemGroups
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ItemGroups PRIMARY KEY,
        Code nvarchar(50) NOT NULL,
        Name nvarchar(150) NOT NULL,
        IsActive bit NOT NULL CONSTRAINT DF_ItemGroups_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_ItemGroups_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_ItemGroups_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_ItemGroups_Code_Active' AND object_id = OBJECT_ID(N'dbo.ItemGroups'))
BEGIN
    CREATE UNIQUE INDEX UX_ItemGroups_Code_Active ON dbo.ItemGroups (Code) WHERE IsDeleted = 0;
END;
GO

IF OBJECT_ID(N'dbo.Taxes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Taxes
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Taxes PRIMARY KEY,
        Code nvarchar(50) NOT NULL,
        Name nvarchar(150) NOT NULL,
        Rate decimal(9,6) NOT NULL CONSTRAINT DF_Taxes_Rate DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_Taxes_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_Taxes_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_Taxes_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT CK_Taxes_Rate CHECK (Rate >= 0)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Taxes_Code_Active' AND object_id = OBJECT_ID(N'dbo.Taxes'))
BEGIN
    CREATE UNIQUE INDEX UX_Taxes_Code_Active ON dbo.Taxes (Code) WHERE IsDeleted = 0;
END;
GO

IF OBJECT_ID(N'dbo.Warehouses', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Warehouses
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Warehouses PRIMARY KEY,
        Code nvarchar(50) NOT NULL,
        Name nvarchar(150) NOT NULL,
        IsActive bit NOT NULL CONSTRAINT DF_Warehouses_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_Warehouses_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_Warehouses_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Warehouses_Code_Active' AND object_id = OBJECT_ID(N'dbo.Warehouses'))
BEGIN
    CREATE UNIQUE INDEX UX_Warehouses_Code_Active ON dbo.Warehouses (Code) WHERE IsDeleted = 0;
END;
GO

IF OBJECT_ID(N'dbo.Items', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Items
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Items PRIMARY KEY,
        GlobalId uniqueidentifier NOT NULL CONSTRAINT DF_Items_GlobalId DEFAULT NEWID(),
        Code nvarchar(50) NOT NULL,
        Name nvarchar(200) NOT NULL,
        ExternalSystem nvarchar(50) NULL,
        ExternalCode nvarchar(100) NULL,
        SapCode nvarchar(100) NULL,
        Description nvarchar(500) NULL,
        ItemGroupId int NULL,
        ItemType nvarchar(30) NOT NULL CONSTRAINT DF_Items_ItemType DEFAULT N'Product',
        InventoryUnitOfMeasureId int NULL,
        PurchaseUnitOfMeasureId int NULL,
        SalesUnitOfMeasureId int NULL,
        IsPurchaseItem bit NOT NULL CONSTRAINT DF_Items_IsPurchaseItem DEFAULT 1,
        IsSalesItem bit NOT NULL CONSTRAINT DF_Items_IsSalesItem DEFAULT 1,
        IsInventoryItem bit NOT NULL CONSTRAINT DF_Items_IsInventoryItem DEFAULT 1,
        PurchaseTaxId int NULL,
        SalesTaxId int NULL,
        ValuationMethod nvarchar(30) NOT NULL CONSTRAINT DF_Items_ValuationMethod DEFAULT N'MovingAverage',
        ManagedBy nvarchar(20) NOT NULL CONSTRAINT DF_Items_ManagedBy DEFAULT N'None',
        BatchSerialManagementMethod nvarchar(30) NOT NULL CONSTRAINT DF_Items_BatchSerialManagementMethod DEFAULT N'EveryTransaction',
        PreferredVendorCode nvarchar(50) NULL,
        VendorCatalogCode nvarchar(80) NULL,
        BaseSalesPrice decimal(19,6) NOT NULL CONSTRAINT DF_Items_BaseSalesPrice DEFAULT 0,
        ReferenceCost decimal(19,6) NOT NULL CONSTRAINT DF_Items_ReferenceCost DEFAULT 0,
        PurchaseFactor decimal(19,6) NOT NULL CONSTRAINT DF_Items_PurchaseFactor DEFAULT 1,
        SalesFactor decimal(19,6) NOT NULL CONSTRAINT DF_Items_SalesFactor DEFAULT 1,
        AllowDiscount bit NOT NULL CONSTRAINT DF_Items_AllowDiscount DEFAULT 1,
        AllowSaleWithoutStock bit NOT NULL CONSTRAINT DF_Items_AllowSaleWithoutStock DEFAULT 0,
        Remarks nvarchar(1000) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_Items_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_Items_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_Items_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT FK_Items_ItemGroups FOREIGN KEY (ItemGroupId) REFERENCES dbo.ItemGroups(Id),
        CONSTRAINT FK_Items_InventoryUnitOfMeasures FOREIGN KEY (InventoryUnitOfMeasureId) REFERENCES dbo.UnitOfMeasures(Id),
        CONSTRAINT FK_Items_PurchaseUnitOfMeasures FOREIGN KEY (PurchaseUnitOfMeasureId) REFERENCES dbo.UnitOfMeasures(Id),
        CONSTRAINT FK_Items_SalesUnitOfMeasures FOREIGN KEY (SalesUnitOfMeasureId) REFERENCES dbo.UnitOfMeasures(Id),
        CONSTRAINT FK_Items_PurchaseTaxes FOREIGN KEY (PurchaseTaxId) REFERENCES dbo.Taxes(Id),
        CONSTRAINT FK_Items_SalesTaxes FOREIGN KEY (SalesTaxId) REFERENCES dbo.Taxes(Id),
        CONSTRAINT CK_Items_ItemType CHECK (ItemType IN (N'Product', N'Service', N'Supply', N'Asset')),
        CONSTRAINT CK_Items_ValuationMethod CHECK (ValuationMethod IN (N'MovingAverage', N'Standard', N'FIFO', N'SerialBatch')),
        CONSTRAINT CK_Items_ManagedBy CHECK (ManagedBy IN (N'None', N'Batch', N'Serial')),
        CONSTRAINT CK_Items_BatchSerialManagementMethod CHECK (BatchSerialManagementMethod IN (N'EveryTransaction', N'IssueOnly')),
        CONSTRAINT CK_Items_Prices CHECK (BaseSalesPrice >= 0 AND ReferenceCost >= 0),
        CONSTRAINT CK_Items_Factors CHECK (PurchaseFactor > 0 AND SalesFactor > 0)
    );
END;
GO

DECLARE @Sql nvarchar(max) = N'';

IF COL_LENGTH('dbo.Items', 'UnitOfMeasure') IS NOT NULL AND COL_LENGTH('dbo.Items', 'InventoryUnitOfMeasureId') IS NULL
BEGIN
    SET @Sql += N'
        ALTER TABLE dbo.Items ADD
            ItemGroupId int NULL,
            ItemType nvarchar(30) NOT NULL CONSTRAINT DF_Items_ItemType DEFAULT N''Product'',
            InventoryUnitOfMeasureId int NULL,
            PurchaseUnitOfMeasureId int NULL,
            SalesUnitOfMeasureId int NULL,
            IsPurchaseItem bit NOT NULL CONSTRAINT DF_Items_IsPurchaseItem DEFAULT 1,
            IsSalesItem bit NOT NULL CONSTRAINT DF_Items_IsSalesItem DEFAULT 1,
            PurchaseTaxId int NULL,
            SalesTaxId int NULL,
            ValuationMethod nvarchar(30) NOT NULL CONSTRAINT DF_Items_ValuationMethod DEFAULT N''MovingAverage'',
            ManagedBy nvarchar(20) NOT NULL CONSTRAINT DF_Items_ManagedBy DEFAULT N''None'',
            BatchSerialManagementMethod nvarchar(30) NOT NULL CONSTRAINT DF_Items_BatchSerialManagementMethod DEFAULT N''EveryTransaction'',
            PreferredVendorCode nvarchar(50) NULL,
            VendorCatalogCode nvarchar(80) NULL,
            BaseSalesPrice decimal(19,6) NOT NULL CONSTRAINT DF_Items_BaseSalesPrice DEFAULT 0,
            ReferenceCost decimal(19,6) NOT NULL CONSTRAINT DF_Items_ReferenceCost DEFAULT 0,
            PurchaseFactor decimal(19,6) NOT NULL CONSTRAINT DF_Items_PurchaseFactor DEFAULT 1,
            SalesFactor decimal(19,6) NOT NULL CONSTRAINT DF_Items_SalesFactor DEFAULT 1,
            AllowDiscount bit NOT NULL CONSTRAINT DF_Items_AllowDiscount DEFAULT 1,
            AllowSaleWithoutStock bit NOT NULL CONSTRAINT DF_Items_AllowSaleWithoutStock DEFAULT 0,
            Remarks nvarchar(1000) NULL,
            CreatedByUserId int NULL,
            CreatedByUserName nvarchar(120) NULL,
            UpdatedByUserId int NULL,
            UpdatedByUserName nvarchar(120) NULL,
            IsDeleted bit NOT NULL CONSTRAINT DF_Items_IsDeleted DEFAULT 0,
            DeletedByUserId int NULL,
            DeletedByUserName nvarchar(120) NULL,
            DeletedAt datetime2(0) NULL;';
END;

IF LEN(@Sql) > 0
BEGIN
    EXEC sys.sp_executesql @Sql;
END;
GO

IF COL_LENGTH(N'dbo.Items', N'GlobalId') IS NULL
    ALTER TABLE dbo.Items ADD GlobalId uniqueidentifier NOT NULL CONSTRAINT DF_Items_GlobalId DEFAULT NEWID();
GO

IF COL_LENGTH(N'dbo.Items', N'ExternalSystem') IS NULL
    ALTER TABLE dbo.Items ADD ExternalSystem nvarchar(50) NULL;
GO

IF COL_LENGTH(N'dbo.Items', N'ExternalCode') IS NULL
    ALTER TABLE dbo.Items ADD ExternalCode nvarchar(100) NULL;
GO

IF COL_LENGTH(N'dbo.Items', N'SapCode') IS NULL
    ALTER TABLE dbo.Items ADD SapCode nvarchar(100) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Items_Code_Active' AND object_id = OBJECT_ID(N'dbo.Items'))
BEGIN
    CREATE UNIQUE INDEX UX_Items_Code_Active ON dbo.Items (Code) WHERE IsDeleted = 0;
END;
GO

IF OBJECT_ID(N'dbo.ItemBarcodes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ItemBarcodes
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ItemBarcodes PRIMARY KEY,
        ItemId int NOT NULL,
        Barcode nvarchar(120) NOT NULL,
        UnitOfMeasureId int NULL,
        BarcodeType nvarchar(40) NOT NULL CONSTRAINT DF_ItemBarcodes_BarcodeType DEFAULT N'Internal',
        ConversionFactor decimal(19,6) NOT NULL CONSTRAINT DF_ItemBarcodes_ConversionFactor DEFAULT 1,
        IsMain bit NOT NULL CONSTRAINT DF_ItemBarcodes_IsMain DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_ItemBarcodes_IsActive DEFAULT 1,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_ItemBarcodes_CreatedAt DEFAULT SYSUTCDATETIME(),
        IsDeleted bit NOT NULL CONSTRAINT DF_ItemBarcodes_IsDeleted DEFAULT 0,
        CONSTRAINT FK_ItemBarcodes_Items FOREIGN KEY (ItemId) REFERENCES dbo.Items(Id),
        CONSTRAINT FK_ItemBarcodes_UnitOfMeasures FOREIGN KEY (UnitOfMeasureId) REFERENCES dbo.UnitOfMeasures(Id),
        CONSTRAINT CK_ItemBarcodes_ConversionFactor CHECK (ConversionFactor > 0)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_ItemBarcodes_Barcode_Active' AND object_id = OBJECT_ID(N'dbo.ItemBarcodes'))
BEGIN
    CREATE UNIQUE INDEX UX_ItemBarcodes_Barcode_Active ON dbo.ItemBarcodes (Barcode) WHERE IsDeleted = 0 AND IsActive = 1;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_ItemBarcodes_Item_Main_Active' AND object_id = OBJECT_ID(N'dbo.ItemBarcodes'))
BEGIN
    CREATE UNIQUE INDEX UX_ItemBarcodes_Item_Main_Active ON dbo.ItemBarcodes (ItemId) WHERE IsDeleted = 0 AND IsActive = 1 AND IsMain = 1;
END;
GO

IF OBJECT_ID(N'dbo.ItemWarehouses', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ItemWarehouses
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ItemWarehouses PRIMARY KEY,
        ItemId int NOT NULL,
        WarehouseId int NOT NULL,
        MinimumStock decimal(19,6) NOT NULL CONSTRAINT DF_ItemWarehouses_MinimumStock DEFAULT 0,
        MaximumStock decimal(19,6) NOT NULL CONSTRAINT DF_ItemWarehouses_MaximumStock DEFAULT 0,
        RequiredStock decimal(19,6) NOT NULL CONSTRAINT DF_ItemWarehouses_RequiredStock DEFAULT 0,
        ReorderPoint decimal(19,6) NOT NULL CONSTRAINT DF_ItemWarehouses_ReorderPoint DEFAULT 0,
        DefaultLocationCode nvarchar(80) NULL,
        WarehouseCost decimal(19,6) NOT NULL CONSTRAINT DF_ItemWarehouses_WarehouseCost DEFAULT 0,
        IsDefaultWarehouse bit NOT NULL CONSTRAINT DF_ItemWarehouses_IsDefaultWarehouse DEFAULT 0,
        IsLocked bit NOT NULL CONSTRAINT DF_ItemWarehouses_IsLocked DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_ItemWarehouses_IsActive DEFAULT 1,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_ItemWarehouses_CreatedAt DEFAULT SYSUTCDATETIME(),
        IsDeleted bit NOT NULL CONSTRAINT DF_ItemWarehouses_IsDeleted DEFAULT 0,
        CONSTRAINT FK_ItemWarehouses_Items FOREIGN KEY (ItemId) REFERENCES dbo.Items(Id),
        CONSTRAINT FK_ItemWarehouses_Warehouses FOREIGN KEY (WarehouseId) REFERENCES dbo.Warehouses(Id),
        CONSTRAINT CK_ItemWarehouses_Stocks CHECK (MinimumStock >= 0 AND MaximumStock >= 0 AND RequiredStock >= 0 AND ReorderPoint >= 0 AND WarehouseCost >= 0),
        CONSTRAINT CK_ItemWarehouses_MinMax CHECK (MaximumStock = 0 OR MinimumStock <= MaximumStock)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_ItemWarehouses_Item_Warehouse_Active' AND object_id = OBJECT_ID(N'dbo.ItemWarehouses'))
BEGIN
    CREATE UNIQUE INDEX UX_ItemWarehouses_Item_Warehouse_Active ON dbo.ItemWarehouses (ItemId, WarehouseId) WHERE IsDeleted = 0;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_ItemWarehouses_Item_Default_Active' AND object_id = OBJECT_ID(N'dbo.ItemWarehouses'))
BEGIN
    CREATE UNIQUE INDEX UX_ItemWarehouses_Item_Default_Active ON dbo.ItemWarehouses (ItemId) WHERE IsDeleted = 0 AND IsActive = 1 AND IsDefaultWarehouse = 1;
END;
GO

IF OBJECT_ID(N'dbo.ItemBatches', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ItemBatches
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_ItemBatches PRIMARY KEY,
        ItemId int NOT NULL,
        BatchNumber nvarchar(120) NOT NULL,
        ManufacturingDate date NULL,
        ExpirationDate date NULL,
        [Status] nvarchar(30) NOT NULL CONSTRAINT DF_ItemBatches_Status DEFAULT N'Available',
        IsActive bit NOT NULL CONSTRAINT DF_ItemBatches_IsActive DEFAULT 1,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_ItemBatches_CreatedAt DEFAULT SYSUTCDATETIME(),
        IsDeleted bit NOT NULL CONSTRAINT DF_ItemBatches_IsDeleted DEFAULT 0,
        CONSTRAINT FK_ItemBatches_Items FOREIGN KEY (ItemId) REFERENCES dbo.Items(Id)
    );
END;
GO

IF OBJECT_ID(N'dbo.ItemSerials', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ItemSerials
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_ItemSerials PRIMARY KEY,
        ItemId int NOT NULL,
        SerialNumber nvarchar(120) NOT NULL,
        ManufacturerSerialNumber nvarchar(120) NULL,
        [Status] nvarchar(30) NOT NULL CONSTRAINT DF_ItemSerials_Status DEFAULT N'Available',
        IsActive bit NOT NULL CONSTRAINT DF_ItemSerials_IsActive DEFAULT 1,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_ItemSerials_CreatedAt DEFAULT SYSUTCDATETIME(),
        IsDeleted bit NOT NULL CONSTRAINT DF_ItemSerials_IsDeleted DEFAULT 0,
        CONSTRAINT FK_ItemSerials_Items FOREIGN KEY (ItemId) REFERENCES dbo.Items(Id)
    );
END;
GO

IF OBJECT_ID(N'dbo.AuditInventoryChanges', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuditInventoryChanges
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditInventoryChanges PRIMARY KEY,
        EntityName nvarchar(120) NOT NULL,
        RecordId nvarchar(80) NOT NULL,
        [Action] nvarchar(30) NOT NULL,
        FieldName nvarchar(120) NOT NULL,
        OldValue nvarchar(max) NULL,
        NewValue nvarchar(max) NULL,
        UserId int NULL,
        UserName nvarchar(120) NULL,
        [Source] nvarchar(60) NOT NULL CONSTRAINT DF_AuditInventoryChanges_Source DEFAULT N'API',
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_AuditInventoryChanges_CreatedAt DEFAULT SYSUTCDATETIME()
    );

    CREATE INDEX IX_AuditInventoryChanges_Entity_Record_CreatedAt ON dbo.AuditInventoryChanges (EntityName, RecordId, CreatedAt DESC);
    CREATE INDEX IX_AuditInventoryChanges_User_CreatedAt ON dbo.AuditInventoryChanges (UserId, CreatedAt DESC);
    CREATE INDEX IX_AuditInventoryChanges_CreatedAt ON dbo.AuditInventoryChanges (CreatedAt DESC);
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.UnitOfMeasures WHERE Code = N'UND' AND IsDeleted = 0)
    INSERT INTO dbo.UnitOfMeasures (Code, Name, CreatedByUserName) VALUES (N'UND', N'Unidad', N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.UnitOfMeasures WHERE Code = N'CAJA' AND IsDeleted = 0)
    INSERT INTO dbo.UnitOfMeasures (Code, Name, CreatedByUserName) VALUES (N'CAJA', N'Caja', N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.ItemGroups WHERE Code = N'GENERAL' AND IsDeleted = 0)
    INSERT INTO dbo.ItemGroups (Code, Name, CreatedByUserName) VALUES (N'GENERAL', N'General', N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.Taxes WHERE Code = N'IVA0' AND IsDeleted = 0)
    INSERT INTO dbo.Taxes (Code, Name, Rate, CreatedByUserName) VALUES (N'IVA0', N'IVA 0%', 0, N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.Taxes WHERE Code = N'IVA15' AND IsDeleted = 0)
    INSERT INTO dbo.Taxes (Code, Name, Rate, CreatedByUserName) VALUES (N'IVA15', N'IVA 15%', 0.150000, N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.Warehouses WHERE Code = N'PRINCIPAL' AND IsDeleted = 0)
    INSERT INTO dbo.Warehouses (Code, Name, CreatedByUserName) VALUES (N'PRINCIPAL', N'Bodega principal', N'Sistema');
GO

DECLARE @DefaultUomId int = (SELECT TOP (1) Id FROM dbo.UnitOfMeasures WHERE Code = N'UND' AND IsDeleted = 0);
DECLARE @DefaultGroupId int = (SELECT TOP (1) Id FROM dbo.ItemGroups WHERE Code = N'GENERAL' AND IsDeleted = 0);
UPDATE dbo.Items
SET
    ItemGroupId = ISNULL(ItemGroupId, @DefaultGroupId),
    InventoryUnitOfMeasureId = ISNULL(InventoryUnitOfMeasureId, @DefaultUomId),
    PurchaseUnitOfMeasureId = ISNULL(PurchaseUnitOfMeasureId, @DefaultUomId),
    SalesUnitOfMeasureId = ISNULL(SalesUnitOfMeasureId, @DefaultUomId),
    IsDeleted = ISNULL(IsDeleted, 0)
WHERE IsDeleted = 0;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEMS_LISTAR
AS
BEGIN
    SELECT
        item.Id, item.GlobalId, item.Code, item.Name, item.ExternalSystem, item.ExternalCode, item.SapCode, item.Description,
        item.ItemGroupId, itemGroup.Code AS ItemGroupCode, itemGroup.Name AS ItemGroupName,
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
        item.Id, item.GlobalId, item.Code, item.Name, item.ExternalSystem, item.ExternalCode, item.SapCode, item.Description,
        item.ItemGroupId, itemGroup.Code AS ItemGroupCode, itemGroup.Name AS ItemGroupName,
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

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEMSBUSCARPORCODIGO
    @Code nvarchar(50),
    @ExcluirId int = NULL
AS
BEGIN
    SELECT COUNT(1)
    FROM dbo.Items
    WHERE Code = @Code
      AND IsDeleted = 0
      AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEMS_LOOKUPS
AS
BEGIN
    SELECT Id, Code, Name FROM dbo.ItemGroups WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT Id, Code, Name FROM dbo.UnitOfMeasures WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT Id, Code, Name, Rate FROM dbo.Taxes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT Id, Code, Name FROM dbo.Warehouses WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_ITEMS_CREAR
    @Code nvarchar(50),
    @Name nvarchar(200),
    @Description nvarchar(500) = NULL,
    @ItemGroupId int = NULL,
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
        Code, Name, Description, ItemGroupId, ItemType,
        InventoryUnitOfMeasureId, PurchaseUnitOfMeasureId, SalesUnitOfMeasureId,
        IsPurchaseItem, IsSalesItem, IsInventoryItem, PurchaseTaxId, SalesTaxId,
        ValuationMethod, ManagedBy, BatchSerialManagementMethod,
        PreferredVendorCode, VendorCatalogCode, BaseSalesPrice, ReferenceCost,
        PurchaseFactor, SalesFactor, AllowDiscount, AllowSaleWithoutStock, Remarks,
        IsActive, CreatedByUserId, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        @Code, @Name, @Description, @ItemGroupId, @ItemType,
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

    DECLARE @OldCode nvarchar(50), @OldName nvarchar(200), @OldIsActive bit;
    SELECT @OldCode = Code, @OldName = Name, @OldIsActive = IsActive
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
    WHERE Id = @Id AND IsDeleted = 0;

    DECLARE @AffectedRows int = @@ROWCOUNT;

    UPDATE dbo.ItemBarcodes SET IsDeleted = 1, IsActive = 0 WHERE ItemId = @Id AND IsDeleted = 0;
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

    UPDATE dbo.ItemWarehouses SET IsDeleted = 1, IsActive = 0 WHERE ItemId = @Id AND IsDeleted = 0;
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
    SELECT N'Items', CONVERT(nvarchar(80), @Id), N'UPDATE', FieldName, OldValue, NewValue, @UpdatedByUserId, @UpdatedByUserName
    FROM
    (
        VALUES
            (N'Code', CONVERT(nvarchar(max), @OldCode), CONVERT(nvarchar(max), @Code)),
            (N'Name', CONVERT(nvarchar(max), @OldName), CONVERT(nvarchar(max), @Name)),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @OldIsActive)), CONVERT(nvarchar(max), CONVERT(int, @IsActive)))
    ) AS Changes(FieldName, OldValue, NewValue)
    WHERE ISNULL(OldValue, N'') <> ISNULL(NewValue, N'');

    COMMIT TRANSACTION;
    SELECT @AffectedRows;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_ITEMS_ELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(120) = NULL
AS
BEGIN
    UPDATE dbo.Items
    SET
        IsDeleted = 1,
        IsActive = 0,
        DeletedByUserId = @DeletedByUserId,
        DeletedByUserName = @DeletedByUserName,
        DeletedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @DeletedByUserId,
        UpdatedByUserName = @DeletedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id
      AND IsDeleted = 0;

    DECLARE @AffectedRows int = @@ROWCOUNT;

    IF @AffectedRows > 0
    BEGIN
        INSERT INTO dbo.AuditInventoryChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
        VALUES (N'Items', CONVERT(nvarchar(80), @Id), N'DELETE', N'IsDeleted', N'0', N'1', @DeletedByUserId, @DeletedByUserName);
    END;

    SELECT @AffectedRows;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_INVENTORYCHANGESLISTAR
    @EntityName nvarchar(120),
    @RecordId nvarchar(80),
    @Take int = 200
AS
BEGIN
    SELECT TOP (@Take)
        Id, EntityName, RecordId, [Action], FieldName, OldValue, NewValue,
        UserId, UserName, [Source], CreatedAt
    FROM dbo.AuditInventoryChanges
    WHERE EntityName = @EntityName
      AND RecordId = @RecordId
    ORDER BY CreatedAt DESC, Id DESC;
END;
GO
