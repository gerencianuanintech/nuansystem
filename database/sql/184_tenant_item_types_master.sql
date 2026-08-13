/*
    Evoluciona dbo.ItemTypes como maestro tenant independiente.

    - Ejecutar solo en bases tenant, nunca en NuanSystem_Master.
    - Prerrequisitos: 044_inventory_auxiliary_catalogs.sql,
      106_tenant_catalog_audit_foundation.sql y SchemaHistory.
    - No activa sincronizacion Matriz/Sucursal ni integra SAP.
    - Los codigos existentes se conservan; BehaviorCode gobierna el
      comportamiento cerrado del ERP.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME() = N'NuanSystem_Master'
    THROW 51184, 'Migration 184 must run only in tenant databases.', 1;
IF OBJECT_ID(N'dbo.ItemTypes', N'U') IS NULL
    THROW 51184, 'ItemTypes from migration 044 is required before migration 184.', 1;
IF OBJECT_ID(N'dbo.AuditCatalogChanges', N'U') IS NULL
    THROW 51184, 'AuditCatalogChanges from migration 106 is required before migration 184.', 1;
IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
    THROW 51184, 'SchemaHistory is required before migration 184.', 1;
GO

/*
    No se asigna Product silenciosamente a tipos personalizados. Toda fila
    previa debe pertenecer al conjunto conocido o recibir un mapeo explicito
    en una migracion de preparacion anterior a esta.
*/
DECLARE @HasUnmappedCustomTypes bit = 0;

IF COL_LENGTH(N'dbo.ItemTypes', N'BehaviorCode') IS NULL
   OR COL_LENGTH(N'dbo.ItemTypes', N'DefaultIsPurchaseItem') IS NULL
   OR COL_LENGTH(N'dbo.ItemTypes', N'DefaultIsSalesItem') IS NULL
   OR COL_LENGTH(N'dbo.ItemTypes', N'DefaultIsInventoryItem') IS NULL
   OR COL_LENGTH(N'dbo.ItemTypes', N'SortOrder') IS NULL
   OR COL_LENGTH(N'dbo.ItemTypes', N'IsSystem') IS NULL
BEGIN
    IF EXISTS
    (
        SELECT 1
        FROM dbo.ItemTypes
        WHERE UPPER(LTRIM(RTRIM(Code))) NOT IN
        (
            N'PRODUCTO', N'PRODUCT',
            N'SERVICIO', N'SERVICE',
            N'INSUMO', N'SUPPLY',
            N'ACTIVO', N'ASSET',
            N'KIT'
        )
    )
        SET @HasUnmappedCustomTypes = 1;
END;
ELSE
BEGIN
    EXEC sys.sp_executesql
        N'SELECT @HasUnmapped = CASE WHEN EXISTS
          (
              SELECT 1
              FROM dbo.ItemTypes
              WHERE UPPER(LTRIM(RTRIM(Code))) NOT IN
              (
                  N''PRODUCTO'', N''PRODUCT'',
                  N''SERVICIO'', N''SERVICE'',
                  N''INSUMO'', N''SUPPLY'',
                  N''ACTIVO'', N''ASSET'',
                  N''KIT''
              )
              AND
              (
                  BehaviorCode IS NULL
                  OR BehaviorCode NOT IN (N''Product'', N''Service'', N''Supply'', N''Asset'', N''Kit'')
                  OR DefaultIsPurchaseItem IS NULL
                  OR DefaultIsSalesItem IS NULL
                  OR DefaultIsInventoryItem IS NULL
                  OR SortOrder IS NULL
                  OR IsSystem IS NULL
              )
          ) THEN 1 ELSE 0 END;',
        N'@HasUnmapped bit OUTPUT',
        @HasUnmapped = @HasUnmappedCustomTypes OUTPUT;
END;

IF @HasUnmappedCustomTypes = 1
    THROW 51184, 'ItemTypes contains custom codes without a complete explicit behavior mapping.', 1;

IF EXISTS
(
    SELECT Code
    FROM dbo.ItemTypes
    GROUP BY Code
    HAVING COUNT_BIG(1) > 1
)
    THROW 51184, 'ItemTypes contains duplicate codes, including deleted records.', 1;

IF EXISTS
(
    SELECT 1
    FROM dbo.ItemTypes
    WHERE NULLIF(LTRIM(RTRIM(Code)), N'') IS NULL
       OR NULLIF(LTRIM(RTRIM(Name)), N'') IS NULL
)
    THROW 51184, 'ItemTypes contains blank codes or names.', 1;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    IF COL_LENGTH(N'dbo.ItemTypes', N'GlobalId') IS NULL
        ALTER TABLE dbo.ItemTypes ADD GlobalId uniqueidentifier NULL;

    IF COL_LENGTH(N'dbo.ItemTypes', N'BehaviorCode') IS NULL
        ALTER TABLE dbo.ItemTypes ADD BehaviorCode nvarchar(30) NULL;

    IF COL_LENGTH(N'dbo.ItemTypes', N'DefaultIsPurchaseItem') IS NULL
        ALTER TABLE dbo.ItemTypes ADD DefaultIsPurchaseItem bit NULL;

    IF COL_LENGTH(N'dbo.ItemTypes', N'DefaultIsSalesItem') IS NULL
        ALTER TABLE dbo.ItemTypes ADD DefaultIsSalesItem bit NULL;

    IF COL_LENGTH(N'dbo.ItemTypes', N'DefaultIsInventoryItem') IS NULL
        ALTER TABLE dbo.ItemTypes ADD DefaultIsInventoryItem bit NULL;

    IF COL_LENGTH(N'dbo.ItemTypes', N'SortOrder') IS NULL
        ALTER TABLE dbo.ItemTypes ADD SortOrder int NULL;

    IF COL_LENGTH(N'dbo.ItemTypes', N'IsSystem') IS NULL
        ALTER TABLE dbo.ItemTypes ADD IsSystem bit NULL;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO

/* Nuevo lote: SQL Server ya puede resolver las columnas agregadas. */
BEGIN TRY
    BEGIN TRANSACTION;

    UPDATE dbo.ItemTypes
    SET GlobalId = NEWID()
    WHERE GlobalId IS NULL;

    UPDATE dbo.ItemTypes
    SET BehaviorCode = CASE
            WHEN UPPER(LTRIM(RTRIM(Code))) IN (N'PRODUCTO', N'PRODUCT') THEN N'Product'
            WHEN UPPER(LTRIM(RTRIM(Code))) IN (N'SERVICIO', N'SERVICE') THEN N'Service'
            WHEN UPPER(LTRIM(RTRIM(Code))) IN (N'INSUMO', N'SUPPLY') THEN N'Supply'
            WHEN UPPER(LTRIM(RTRIM(Code))) IN (N'ACTIVO', N'ASSET') THEN N'Asset'
            WHEN UPPER(LTRIM(RTRIM(Code))) = N'KIT' THEN N'Kit'
        END,
        DefaultIsPurchaseItem = CASE
            WHEN UPPER(LTRIM(RTRIM(Code))) IN (N'PRODUCTO', N'PRODUCT', N'SERVICIO', N'SERVICE', N'INSUMO', N'SUPPLY', N'ACTIVO', N'ASSET') THEN 1
            ELSE 0
        END,
        DefaultIsSalesItem = CASE
            WHEN UPPER(LTRIM(RTRIM(Code))) IN (N'PRODUCTO', N'PRODUCT', N'SERVICIO', N'SERVICE', N'KIT') THEN 1
            ELSE 0
        END,
        DefaultIsInventoryItem = CASE
            WHEN UPPER(LTRIM(RTRIM(Code))) IN (N'PRODUCTO', N'PRODUCT', N'INSUMO', N'SUPPLY') THEN 1
            ELSE 0
        END,
        SortOrder = CASE
            WHEN UPPER(LTRIM(RTRIM(Code))) IN (N'PRODUCTO', N'PRODUCT') THEN 10
            WHEN UPPER(LTRIM(RTRIM(Code))) IN (N'SERVICIO', N'SERVICE') THEN 20
            WHEN UPPER(LTRIM(RTRIM(Code))) IN (N'INSUMO', N'SUPPLY') THEN 30
            WHEN UPPER(LTRIM(RTRIM(Code))) IN (N'ACTIVO', N'ASSET') THEN 40
            WHEN UPPER(LTRIM(RTRIM(Code))) = N'KIT' THEN 50
        END,
        IsSystem = 1
    WHERE BehaviorCode IS NULL
       OR DefaultIsPurchaseItem IS NULL
       OR DefaultIsSalesItem IS NULL
       OR DefaultIsInventoryItem IS NULL
       OR SortOrder IS NULL
       OR IsSystem IS NULL;

    IF NOT EXISTS (SELECT 1 FROM dbo.ItemTypes WHERE BehaviorCode = N'Asset')
    BEGIN
        INSERT dbo.ItemTypes
        (
            GlobalId, Code, Name, Description, BehaviorCode,
            DefaultIsPurchaseItem, DefaultIsSalesItem, DefaultIsInventoryItem,
            SortOrder, IsSystem, IsActive, IsDeleted,
            CreatedAt, CreatedByUserName
        )
        VALUES
        (
            NEWID(), N'ACTIVO', N'Activo', N'Articulo registrado como activo de la empresa.', N'Asset',
            1, 0, 0,
            40, 1, 1, 0,
            SYSUTCDATETIME(), N'Sistema'
        );
    END;

    /* Kit permanece definido pero inactivo hasta implementar su operacion. */
    UPDATE dbo.ItemTypes
    SET IsActive = 0,
        UpdatedAt = CASE WHEN IsActive <> 0 THEN SYSUTCDATETIME() ELSE UpdatedAt END,
        UpdatedByUserName = CASE WHEN IsActive <> 0 THEN N'Sistema' ELSE UpdatedByUserName END
    WHERE BehaviorCode = N'Kit'
      AND IsSystem = 1;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.ItemTypes
        WHERE GlobalId IS NULL
           OR BehaviorCode IS NULL
           OR DefaultIsPurchaseItem IS NULL
           OR DefaultIsSalesItem IS NULL
           OR DefaultIsInventoryItem IS NULL
           OR SortOrder IS NULL
           OR IsSystem IS NULL
    )
        THROW 51184, 'ItemTypes backfill did not resolve all required values.', 1;

    ALTER TABLE dbo.ItemTypes ALTER COLUMN GlobalId uniqueidentifier NOT NULL;
    ALTER TABLE dbo.ItemTypes ALTER COLUMN BehaviorCode nvarchar(30) NOT NULL;
    ALTER TABLE dbo.ItemTypes ALTER COLUMN DefaultIsPurchaseItem bit NOT NULL;
    ALTER TABLE dbo.ItemTypes ALTER COLUMN DefaultIsSalesItem bit NOT NULL;
    ALTER TABLE dbo.ItemTypes ALTER COLUMN DefaultIsInventoryItem bit NOT NULL;
    ALTER TABLE dbo.ItemTypes ALTER COLUMN SortOrder int NOT NULL;
    ALTER TABLE dbo.ItemTypes ALTER COLUMN IsSystem bit NOT NULL;

    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.default_constraints d
        INNER JOIN sys.columns c
            ON c.object_id = d.parent_object_id
           AND c.column_id = d.parent_column_id
        WHERE d.parent_object_id = OBJECT_ID(N'dbo.ItemTypes')
          AND c.name = N'GlobalId'
    )
        ALTER TABLE dbo.ItemTypes ADD CONSTRAINT DF_ItemTypes_GlobalId DEFAULT NEWID() FOR GlobalId;

    IF NOT EXISTS
    (
        SELECT 1 FROM sys.default_constraints d
        INNER JOIN sys.columns c ON c.object_id = d.parent_object_id AND c.column_id = d.parent_column_id
        WHERE d.parent_object_id = OBJECT_ID(N'dbo.ItemTypes') AND c.name = N'DefaultIsPurchaseItem'
    )
        ALTER TABLE dbo.ItemTypes ADD CONSTRAINT DF_ItemTypes_DefaultIsPurchaseItem DEFAULT (1) FOR DefaultIsPurchaseItem;

    IF NOT EXISTS
    (
        SELECT 1 FROM sys.default_constraints d
        INNER JOIN sys.columns c ON c.object_id = d.parent_object_id AND c.column_id = d.parent_column_id
        WHERE d.parent_object_id = OBJECT_ID(N'dbo.ItemTypes') AND c.name = N'DefaultIsSalesItem'
    )
        ALTER TABLE dbo.ItemTypes ADD CONSTRAINT DF_ItemTypes_DefaultIsSalesItem DEFAULT (1) FOR DefaultIsSalesItem;

    IF NOT EXISTS
    (
        SELECT 1 FROM sys.default_constraints d
        INNER JOIN sys.columns c ON c.object_id = d.parent_object_id AND c.column_id = d.parent_column_id
        WHERE d.parent_object_id = OBJECT_ID(N'dbo.ItemTypes') AND c.name = N'DefaultIsInventoryItem'
    )
        ALTER TABLE dbo.ItemTypes ADD CONSTRAINT DF_ItemTypes_DefaultIsInventoryItem DEFAULT (1) FOR DefaultIsInventoryItem;

    IF NOT EXISTS
    (
        SELECT 1 FROM sys.default_constraints d
        INNER JOIN sys.columns c ON c.object_id = d.parent_object_id AND c.column_id = d.parent_column_id
        WHERE d.parent_object_id = OBJECT_ID(N'dbo.ItemTypes') AND c.name = N'SortOrder'
    )
        ALTER TABLE dbo.ItemTypes ADD CONSTRAINT DF_ItemTypes_SortOrder DEFAULT (0) FOR SortOrder;

    IF NOT EXISTS
    (
        SELECT 1 FROM sys.default_constraints d
        INNER JOIN sys.columns c ON c.object_id = d.parent_object_id AND c.column_id = d.parent_column_id
        WHERE d.parent_object_id = OBJECT_ID(N'dbo.ItemTypes') AND c.name = N'IsSystem'
    )
        ALTER TABLE dbo.ItemTypes ADD CONSTRAINT DF_ItemTypes_IsSystem DEFAULT (0) FOR IsSystem;

    IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.ItemTypes') AND name = N'CK_ItemTypes_Code_NotBlank')
        ALTER TABLE dbo.ItemTypes ADD CONSTRAINT CK_ItemTypes_Code_NotBlank CHECK (NULLIF(LTRIM(RTRIM(Code)), N'') IS NOT NULL);

    IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.ItemTypes') AND name = N'CK_ItemTypes_Name_NotBlank')
        ALTER TABLE dbo.ItemTypes ADD CONSTRAINT CK_ItemTypes_Name_NotBlank CHECK (NULLIF(LTRIM(RTRIM(Name)), N'') IS NOT NULL);

    IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.ItemTypes') AND name = N'CK_ItemTypes_BehaviorCode')
        ALTER TABLE dbo.ItemTypes ADD CONSTRAINT CK_ItemTypes_BehaviorCode CHECK (BehaviorCode IN (N'Product', N'Service', N'Supply', N'Asset', N'Kit'));

    IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.ItemTypes') AND name = N'CK_ItemTypes_SortOrder')
        ALTER TABLE dbo.ItemTypes ADD CONSTRAINT CK_ItemTypes_SortOrder CHECK (SortOrder >= 0);

    IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.ItemTypes') AND name = N'CK_ItemTypes_ServiceNoInventory')
        ALTER TABLE dbo.ItemTypes ADD CONSTRAINT CK_ItemTypes_ServiceNoInventory CHECK (BehaviorCode <> N'Service' OR DefaultIsInventoryItem = 0);

    IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.ItemTypes') AND name = N'UX_ItemTypes_Code_Active')
        DROP INDEX UX_ItemTypes_Code_Active ON dbo.ItemTypes;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.ItemTypes') AND name = N'UQ_ItemTypes_Code')
        CREATE UNIQUE INDEX UQ_ItemTypes_Code ON dbo.ItemTypes(Code);

    IF EXISTS (SELECT GlobalId FROM dbo.ItemTypes GROUP BY GlobalId HAVING COUNT_BIG(1) > 1)
        THROW 51184, 'ItemTypes contains duplicate GlobalId values.', 1;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.ItemTypes') AND name = N'UQ_ItemTypes_GlobalId')
        CREATE UNIQUE INDEX UQ_ItemTypes_GlobalId ON dbo.ItemTypes(GlobalId);

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.ItemTypes') AND name = N'IX_ItemTypes_Active_SortOrder_Name')
        CREATE INDEX IX_ItemTypes_Active_SortOrder_Name ON dbo.ItemTypes(IsActive, SortOrder, Name) INCLUDE (Code, BehaviorCode) WHERE IsDeleted = 0;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.ItemTypes') AND name = N'IX_ItemTypes_BehaviorCode')
        CREATE INDEX IX_ItemTypes_BehaviorCode ON dbo.ItemTypes(BehaviorCode, IsActive) INCLUDE (Code, Name, SortOrder) WHERE IsDeleted = 0;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ITEMTYPES_LISTAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, GlobalId, Code, Name, Description, BehaviorCode,
           DefaultIsPurchaseItem, DefaultIsSalesItem, DefaultIsInventoryItem,
           SortOrder, IsSystem, IsActive,
           CreatedByUserId, CreatedByUserName, CreatedAt,
           UpdatedByUserId, UpdatedByUserName, UpdatedAt,
           DeletedByUserId, DeletedByUserName, DeletedAt
    FROM dbo.ItemTypes
    WHERE IsDeleted = 0
    ORDER BY SortOrder, Name, Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ITEMTYPES_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
           Id, GlobalId, Code, Name, Description, BehaviorCode,
           DefaultIsPurchaseItem, DefaultIsSalesItem, DefaultIsInventoryItem,
           SortOrder, IsSystem, IsActive,
           CreatedByUserId, CreatedByUserName, CreatedAt,
           UpdatedByUserId, UpdatedByUserName, UpdatedAt,
           DeletedByUserId, DeletedByUserName, DeletedAt
    FROM dbo.ItemTypes
    WHERE Id = @Id AND IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ITEMTYPES_LOOKUP
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, GlobalId, Code, Name, BehaviorCode,
           DefaultIsPurchaseItem, DefaultIsSalesItem, DefaultIsInventoryItem,
           SortOrder, IsSystem, CAST(IsActive AS bit) AS IsActive
    FROM dbo.ItemTypes
    WHERE IsDeleted = 0 AND IsActive = 1
    ORDER BY SortOrder, Name, Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ITEMTYPES_BUSCARPORCODIGO
    @Code nvarchar(50),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM dbo.ItemTypes WITH (UPDLOCK, HOLDLOCK)
    WHERE Code = LTRIM(RTRIM(@Code))
      AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_ITEMTYPES_HISTORIAL
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, EntityName, RecordId, [Action], FieldName, OldValue, NewValue,
           UserId, UserName, [Source], CreatedAt
    FROM dbo.AuditCatalogChanges
    WHERE EntityName = N'ItemType'
      AND RecordId = CONVERT(nvarchar(80), @Id)
    ORDER BY CreatedAt DESC, Id DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_GENERAL_INVENTORY_ITEMTYPES_CREAR
    @Code nvarchar(50),
    @Name nvarchar(150),
    @Description nvarchar(500) = NULL,
    @BehaviorCode nvarchar(30) = NULL,
    @DefaultIsPurchaseItem bit = 1,
    @DefaultIsSalesItem bit = 1,
    @DefaultIsInventoryItem bit = 1,
    @SortOrder int = 0,
    @IsActive bit = 1,
    @GlobalId uniqueidentifier = NULL,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @Code = LTRIM(RTRIM(@Code));
    SET @Name = LTRIM(RTRIM(@Name));
    SET @Description = NULLIF(LTRIM(RTRIM(@Description)), N'');
    SET @BehaviorCode = NULLIF(LTRIM(RTRIM(@BehaviorCode)), N'');
    SET @GlobalId = COALESCE(@GlobalId, NEWID());

    IF NULLIF(@Code, N'') IS NULL THROW 51002, 'El codigo es obligatorio.', 1;
    IF NULLIF(@Name, N'') IS NULL THROW 51003, 'El nombre es obligatorio.', 1;
    IF @BehaviorCode IS NULL THROW 51184, 'BehaviorCode is required for ItemType.', 1;
    IF @BehaviorCode NOT IN (N'Product', N'Service', N'Supply', N'Asset', N'Kit') THROW 51184, 'ItemType BehaviorCode is invalid.', 1;
    IF @BehaviorCode = N'Service' AND @DefaultIsInventoryItem = 1 THROW 51184, 'Service ItemType cannot default to inventory.', 1;
    IF @SortOrder < 0 THROW 51184, 'ItemType SortOrder cannot be negative.', 1;
    IF @GlobalId = '00000000-0000-0000-0000-000000000000' THROW 51184, 'ItemType GlobalId is invalid.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS
        (
            SELECT 1 FROM dbo.ItemTypes WITH (UPDLOCK, HOLDLOCK)
            WHERE Code = @Code OR GlobalId = @GlobalId
        )
        BEGIN
            COMMIT TRANSACTION;
            SELECT -1;
            RETURN;
        END;

        INSERT dbo.ItemTypes
        (
            GlobalId, Code, Name, Description, BehaviorCode,
            DefaultIsPurchaseItem, DefaultIsSalesItem, DefaultIsInventoryItem,
            SortOrder, IsSystem, IsActive, IsDeleted,
            CreatedAt, CreatedByUserId, CreatedByUserName
        )
        VALUES
        (
            @GlobalId, @Code, @Name, @Description, @BehaviorCode,
            @DefaultIsPurchaseItem, @DefaultIsSalesItem, @DefaultIsInventoryItem,
            @SortOrder, 0, @IsActive, 0,
            SYSUTCDATETIME(), @CreatedByUserId, @CreatedByUserName
        );

        DECLARE @Id int = CONVERT(int, SCOPE_IDENTITY());

        INSERT dbo.AuditCatalogChanges
            (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
        SELECT N'ItemType', CONVERT(nvarchar(80), @Id), N'INSERT', FieldName,
               NULL, NewValue, @CreatedByUserId, @CreatedByUserName
        FROM (VALUES
            (N'GlobalId', CONVERT(nvarchar(max), @GlobalId)),
            (N'Code', CONVERT(nvarchar(max), @Code)),
            (N'Name', CONVERT(nvarchar(max), @Name)),
            (N'Description', CONVERT(nvarchar(max), @Description)),
            (N'BehaviorCode', CONVERT(nvarchar(max), @BehaviorCode)),
            (N'DefaultIsPurchaseItem', CONVERT(nvarchar(max), CONVERT(int, @DefaultIsPurchaseItem))),
            (N'DefaultIsSalesItem', CONVERT(nvarchar(max), CONVERT(int, @DefaultIsSalesItem))),
            (N'DefaultIsInventoryItem', CONVERT(nvarchar(max), CONVERT(int, @DefaultIsInventoryItem))),
            (N'SortOrder', CONVERT(nvarchar(max), @SortOrder)),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @IsActive)))) changes(FieldName, NewValue);

        COMMIT TRANSACTION;
        SELECT @Id;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        IF ERROR_NUMBER() IN (2601, 2627)
        BEGIN
            SELECT -1;
            RETURN;
        END;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_GENERAL_INVENTORY_ITEMTYPES_ACTUALIZAR
    @Id int,
    @Code nvarchar(50),
    @Name nvarchar(150),
    @Description nvarchar(500) = NULL,
    @BehaviorCode nvarchar(30) = NULL,
    @DefaultIsPurchaseItem bit = 1,
    @DefaultIsSalesItem bit = 1,
    @DefaultIsInventoryItem bit = 1,
    @SortOrder int = 0,
    @IsActive bit = 1,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @Code = LTRIM(RTRIM(@Code));
    SET @Name = LTRIM(RTRIM(@Name));
    SET @Description = NULLIF(LTRIM(RTRIM(@Description)), N'');
    SET @BehaviorCode = NULLIF(LTRIM(RTRIM(@BehaviorCode)), N'');

    IF @Id <= 0 THROW 51184, 'ItemType Id is invalid.', 1;
    IF NULLIF(@Code, N'') IS NULL THROW 51002, 'El codigo es obligatorio.', 1;
    IF NULLIF(@Name, N'') IS NULL THROW 51003, 'El nombre es obligatorio.', 1;
    IF @BehaviorCode IS NULL THROW 51184, 'BehaviorCode is required for ItemType.', 1;
    IF @BehaviorCode NOT IN (N'Product', N'Service', N'Supply', N'Asset', N'Kit') THROW 51184, 'ItemType BehaviorCode is invalid.', 1;
    IF @BehaviorCode = N'Service' AND @DefaultIsInventoryItem = 1 THROW 51184, 'Service ItemType cannot default to inventory.', 1;
    IF @SortOrder < 0 THROW 51184, 'ItemType SortOrder cannot be negative.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @OldCode nvarchar(50), @OldName nvarchar(150), @OldDescription nvarchar(500),
                @OldBehaviorCode nvarchar(30), @OldDefaultPurchase bit, @OldDefaultSales bit,
                @OldDefaultInventory bit, @OldSortOrder int, @OldIsSystem bit, @OldIsActive bit;

        SELECT @OldCode = Code, @OldName = Name, @OldDescription = Description,
               @OldBehaviorCode = BehaviorCode, @OldDefaultPurchase = DefaultIsPurchaseItem,
               @OldDefaultSales = DefaultIsSalesItem, @OldDefaultInventory = DefaultIsInventoryItem,
               @OldSortOrder = SortOrder, @OldIsSystem = IsSystem, @OldIsActive = IsActive
        FROM dbo.ItemTypes WITH (UPDLOCK, HOLDLOCK)
        WHERE Id = @Id AND IsDeleted = 0;

        IF @OldCode IS NULL
        BEGIN
            COMMIT TRANSACTION;
            SELECT 0;
            RETURN;
        END;

        IF @OldIsSystem = 1 AND (@OldCode <> @Code OR @OldBehaviorCode <> @BehaviorCode)
        BEGIN
            COMMIT TRANSACTION;
            SELECT -2;
            RETURN;
        END;

        IF EXISTS
        (
            SELECT 1 FROM dbo.ItemTypes WITH (UPDLOCK, HOLDLOCK)
            WHERE Code = @Code AND Id <> @Id
        )
        BEGIN
            COMMIT TRANSACTION;
            SELECT -1;
            RETURN;
        END;

        UPDATE dbo.ItemTypes
        SET Code = @Code,
            Name = @Name,
            Description = @Description,
            BehaviorCode = @BehaviorCode,
            DefaultIsPurchaseItem = @DefaultIsPurchaseItem,
            DefaultIsSalesItem = @DefaultIsSalesItem,
            DefaultIsInventoryItem = @DefaultIsInventoryItem,
            SortOrder = @SortOrder,
            IsActive = @IsActive,
            UpdatedAt = SYSUTCDATETIME(),
            UpdatedByUserId = @UpdatedByUserId,
            UpdatedByUserName = @UpdatedByUserName
        WHERE Id = @Id AND IsDeleted = 0;

        IF @@ROWCOUNT = 0
        BEGIN
            COMMIT TRANSACTION;
            SELECT 0;
            RETURN;
        END;

        INSERT dbo.AuditCatalogChanges
            (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
        SELECT N'ItemType', CONVERT(nvarchar(80), @Id), N'UPDATE', FieldName,
               OldValue, NewValue, @UpdatedByUserId, @UpdatedByUserName
        FROM (VALUES
            (N'Code', CONVERT(nvarchar(max), @OldCode), CONVERT(nvarchar(max), @Code)),
            (N'Name', CONVERT(nvarchar(max), @OldName), CONVERT(nvarchar(max), @Name)),
            (N'Description', CONVERT(nvarchar(max), @OldDescription), CONVERT(nvarchar(max), @Description)),
            (N'BehaviorCode', CONVERT(nvarchar(max), @OldBehaviorCode), CONVERT(nvarchar(max), @BehaviorCode)),
            (N'DefaultIsPurchaseItem', CONVERT(nvarchar(max), CONVERT(int, @OldDefaultPurchase)), CONVERT(nvarchar(max), CONVERT(int, @DefaultIsPurchaseItem))),
            (N'DefaultIsSalesItem', CONVERT(nvarchar(max), CONVERT(int, @OldDefaultSales)), CONVERT(nvarchar(max), CONVERT(int, @DefaultIsSalesItem))),
            (N'DefaultIsInventoryItem', CONVERT(nvarchar(max), CONVERT(int, @OldDefaultInventory)), CONVERT(nvarchar(max), CONVERT(int, @DefaultIsInventoryItem))),
            (N'SortOrder', CONVERT(nvarchar(max), @OldSortOrder), CONVERT(nvarchar(max), @SortOrder)),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @OldIsActive)), CONVERT(nvarchar(max), CONVERT(int, @IsActive)))) changes(FieldName, OldValue, NewValue)
        WHERE ISNULL(OldValue, N'') <> ISNULL(NewValue, N'');

        COMMIT TRANSACTION;
        SELECT 1;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        IF ERROR_NUMBER() IN (2601, 2627)
        BEGIN
            SELECT -1;
            RETURN;
        END;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_GENERAL_INVENTORY_ITEMTYPES_ELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @OldIsActive bit, @IsSystem bit, @IsInUse bit = 0;

        SELECT @OldIsActive = IsActive, @IsSystem = IsSystem
        FROM dbo.ItemTypes WITH (UPDLOCK, HOLDLOCK)
        WHERE Id = @Id AND IsDeleted = 0;

        IF @OldIsActive IS NULL
        BEGIN
            COMMIT TRANSACTION;
            SELECT 0;
            RETURN;
        END;

        IF @IsSystem = 1
        BEGIN
            COMMIT TRANSACTION;
            SELECT -2;
            RETURN;
        END;

        IF OBJECT_ID(N'dbo.Items', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.Items', N'ItemTypeId') IS NOT NULL
        BEGIN
            EXEC sys.sp_executesql
                N'SELECT @InUse = CASE WHEN EXISTS (SELECT 1 FROM dbo.Items WHERE ItemTypeId = @ItemTypeId AND IsDeleted = 0) THEN 1 ELSE 0 END;',
                N'@ItemTypeId int, @InUse bit OUTPUT',
                @ItemTypeId = @Id,
                @InUse = @IsInUse OUTPUT;
        END;

        IF @IsInUse = 1
        BEGIN
            COMMIT TRANSACTION;
            SELECT -3;
            RETURN;
        END;

        UPDATE dbo.ItemTypes
        SET IsActive = 0,
            IsDeleted = 1,
            DeletedAt = SYSUTCDATETIME(),
            DeletedByUserId = @DeletedByUserId,
            DeletedByUserName = @DeletedByUserName
        WHERE Id = @Id AND IsDeleted = 0;

        IF @@ROWCOUNT = 0
        BEGIN
            COMMIT TRANSACTION;
            SELECT 0;
            RETURN;
        END;

        INSERT dbo.AuditCatalogChanges
            (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
        VALUES
            (N'ItemType', CONVERT(nvarchar(80), @Id), N'DELETE', N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @OldIsActive)), N'0', @DeletedByUserId, @DeletedByUserName),
            (N'ItemType', CONVERT(nvarchar(80), @Id), N'DELETE', N'IsDeleted', N'0', N'1', @DeletedByUserId, @DeletedByUserName);

        COMMIT TRANSACTION;
        SELECT 1;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SchemaHistory WHERE Version = N'20260811.184')
BEGIN
    INSERT dbo.SchemaHistory(Version, Description)
    VALUES
    (
        N'20260811.184',
        N'Evoluciona Tipos de item con comportamiento ERP, defaults, auditoria y GlobalId'
    );
END;
GO
