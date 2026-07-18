/*
    ItemGroups como entidad operativa de sincronizacion Maestro-Sucursal.

    Id sigue siendo identidad local. GlobalId identifica el mismo grupo entre
    Master y sucursales; Code se usa solamente para adoptar datos heredados.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF OBJECT_ID(N'dbo.ItemGroups', N'U') IS NULL
BEGIN
    THROW 51097, 'No existe dbo.ItemGroups. Ejecute primero 020_inventory_item_groups_master.sql.', 1;
END;
GO

/*
    Algunas sucursales fueron creadas con la version minima de ItemGroups.
    La migracion debe poder evolucionarlas sin exigir que se vuelvan a crear.
*/
IF COL_LENGTH(N'dbo.ItemGroups', N'Description') IS NULL
    ALTER TABLE dbo.ItemGroups ADD Description nvarchar(500) NULL;
GO

IF COL_LENGTH(N'dbo.ItemGroups', N'InventoryAccountCode') IS NULL
    ALTER TABLE dbo.ItemGroups ADD InventoryAccountCode nvarchar(120) NULL;
GO

IF COL_LENGTH(N'dbo.ItemGroups', N'CostOfSalesAccountCode') IS NULL
    ALTER TABLE dbo.ItemGroups ADD CostOfSalesAccountCode nvarchar(120) NULL;
GO

IF COL_LENGTH(N'dbo.ItemGroups', N'SalesAccountCode') IS NULL
    ALTER TABLE dbo.ItemGroups ADD SalesAccountCode nvarchar(120) NULL;
GO

IF COL_LENGTH(N'dbo.ItemGroups', N'PurchaseAccountCode') IS NULL
    ALTER TABLE dbo.ItemGroups ADD PurchaseAccountCode nvarchar(120) NULL;
GO

IF COL_LENGTH(N'dbo.ItemGroups', N'SapGroupCode') IS NULL
    ALTER TABLE dbo.ItemGroups ADD SapGroupCode nvarchar(100) NULL;
GO

IF COL_LENGTH(N'dbo.ItemGroups', N'SapCode') IS NULL
    ALTER TABLE dbo.ItemGroups ADD SapCode nvarchar(100) NULL;
GO

IF COL_LENGTH(N'dbo.ItemGroups', N'GlobalId') IS NULL
    ALTER TABLE dbo.ItemGroups ADD GlobalId uniqueidentifier NULL;
GO

IF COL_LENGTH(N'dbo.ItemGroups', N'ExternalSystem') IS NULL
    ALTER TABLE dbo.ItemGroups ADD ExternalSystem nvarchar(50) NULL;
GO

IF COL_LENGTH(N'dbo.ItemGroups', N'ExternalCode') IS NULL
    ALTER TABLE dbo.ItemGroups ADD ExternalCode nvarchar(100) NULL;
GO

UPDATE dbo.ItemGroups SET GlobalId = NEWID() WHERE GlobalId IS NULL;
GO

IF EXISTS
(
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.ItemGroups')
      AND name = N'GlobalId'
      AND is_nullable = 1
)
    ALTER TABLE dbo.ItemGroups ALTER COLUMN GlobalId uniqueidentifier NOT NULL;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.default_constraints defaultConstraint
    INNER JOIN sys.columns columnDefinition
        ON columnDefinition.object_id = defaultConstraint.parent_object_id
       AND columnDefinition.column_id = defaultConstraint.parent_column_id
    WHERE defaultConstraint.parent_object_id = OBJECT_ID(N'dbo.ItemGroups')
      AND columnDefinition.name = N'GlobalId'
)
    ALTER TABLE dbo.ItemGroups ADD CONSTRAINT DF_ItemGroups_GlobalId DEFAULT NEWID() FOR GlobalId;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'UX_ItemGroups_GlobalId'
      AND object_id = OBJECT_ID(N'dbo.ItemGroups')
)
    CREATE UNIQUE INDEX UX_ItemGroups_GlobalId ON dbo.ItemGroups (GlobalId);
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_ItemGroups_ExternalRef'
      AND object_id = OBJECT_ID(N'dbo.ItemGroups')
)
    CREATE INDEX IX_ItemGroups_ExternalRef
        ON dbo.ItemGroups (ExternalSystem, ExternalCode)
        WHERE ExternalSystem IS NOT NULL AND ExternalCode IS NOT NULL;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_GROUPS_LISTAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, GlobalId, Code, Name, Description, IsActive,
           InventoryAccountCode, CostOfSalesAccountCode, SalesAccountCode, PurchaseAccountCode,
           SapGroupCode, SapCode, ExternalSystem, ExternalCode,
           CreatedByUserId, CreatedByUserName, CreatedAt,
           UpdatedByUserId, UpdatedByUserName, UpdatedAt,
           DeletedByUserId, DeletedByUserName, DeletedAt
    FROM dbo.ItemGroups
    WHERE IsDeleted = 0
    ORDER BY Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_GROUPS_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, GlobalId, Code, Name, Description, IsActive,
           InventoryAccountCode, CostOfSalesAccountCode, SalesAccountCode, PurchaseAccountCode,
           SapGroupCode, SapCode, ExternalSystem, ExternalCode,
           CreatedByUserId, CreatedByUserName, CreatedAt,
           UpdatedByUserId, UpdatedByUserName, UpdatedAt,
           DeletedByUserId, DeletedByUserName, DeletedAt
    FROM dbo.ItemGroups
    WHERE Id = @Id AND IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_GROUP_SYNC_EXISTS_BY_GLOBAL_ID
    @GlobalId uniqueidentifier
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1) FROM dbo.ItemGroups WHERE GlobalId = @GlobalId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_ITEM_GROUP_SYNC_APPLY
    @GlobalId uniqueidentifier,
    @Code nvarchar(50),
    @Name nvarchar(150),
    @Description nvarchar(500) = NULL,
    @InventoryAccountCode nvarchar(120) = NULL,
    @CostOfSalesAccountCode nvarchar(120) = NULL,
    @SalesAccountCode nvarchar(120) = NULL,
    @PurchaseAccountCode nvarchar(120) = NULL,
    @SapGroupCode nvarchar(100) = NULL,
    @SapCode nvarchar(50) = NULL,
    @IsActive bit,
    @IsDeleted bit,
    @ExternalSystem nvarchar(50) = NULL,
    @ExternalCode nvarchar(100) = NULL,
    @CreatedAt datetime2(0),
    @UpdatedAt datetime2(0)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ItemGroupId int;

    SELECT @ItemGroupId = Id
    FROM dbo.ItemGroups WITH (UPDLOCK, HOLDLOCK)
    WHERE GlobalId = @GlobalId;

    IF @ItemGroupId IS NULL
    BEGIN
        SELECT @ItemGroupId = Id
        FROM dbo.ItemGroups WITH (UPDLOCK, HOLDLOCK)
        WHERE Code = @Code;
    END;

    IF @ItemGroupId IS NULL
    BEGIN
        INSERT INTO dbo.ItemGroups
        (
            GlobalId, Code, Name, Description,
            InventoryAccountCode, CostOfSalesAccountCode, SalesAccountCode, PurchaseAccountCode,
            SapGroupCode, SapCode, IsActive, IsDeleted, ExternalSystem, ExternalCode,
            CreatedAt, CreatedByUserName
        )
        VALUES
        (
            @GlobalId, @Code, @Name, @Description,
            @InventoryAccountCode, @CostOfSalesAccountCode, @SalesAccountCode, @PurchaseAccountCode,
            @SapGroupCode, @SapCode, @IsActive, @IsDeleted, @ExternalSystem, @ExternalCode,
            @CreatedAt, N'MasterBranchSyncWorker'
        );

        SET @ItemGroupId = CONVERT(int, SCOPE_IDENTITY());
    END
    ELSE
    BEGIN
        UPDATE dbo.ItemGroups
        SET GlobalId = @GlobalId,
            Code = @Code,
            Name = @Name,
            Description = @Description,
            InventoryAccountCode = @InventoryAccountCode,
            CostOfSalesAccountCode = @CostOfSalesAccountCode,
            SalesAccountCode = @SalesAccountCode,
            PurchaseAccountCode = @PurchaseAccountCode,
            SapGroupCode = @SapGroupCode,
            SapCode = @SapCode,
            IsActive = @IsActive,
            IsDeleted = @IsDeleted,
            ExternalSystem = @ExternalSystem,
            ExternalCode = @ExternalCode,
            UpdatedAt = @UpdatedAt,
            UpdatedByUserName = N'MasterBranchSyncWorker',
            DeletedAt = CASE WHEN @IsDeleted = 1 THEN @UpdatedAt ELSE NULL END,
            DeletedByUserName = CASE WHEN @IsDeleted = 1 THEN N'MasterBranchSyncWorker' ELSE NULL END
        WHERE Id = @ItemGroupId;
    END;

    SELECT @ItemGroupId;
END;
GO

IF OBJECT_ID(N'dbo.SchemaVersions', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.SchemaVersions WHERE Version = N'20260717.097')
BEGIN
    INSERT INTO dbo.SchemaVersions (Version, Description)
    VALUES (N'20260717.097', N'ItemGroups operativo para sincronizacion Maestro-Sucursal');
END;
GO
