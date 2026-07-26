/*
    ItemFamilies como entidad operativa de sincronizacion Matriz-Sucursal.

    - GlobalId es la unica identidad entre tenants.
    - ItemGroup se resuelve por ItemGroupGlobalId.
    - Una colision de (ItemGroupId, Code) con otro GlobalId es terminal.
    - No existe adopcion automatica por codigo.

    Ejecutar solo en bases tenant. Nunca en NuanSystem_Master.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.ItemFamilies', N'U') IS NULL
    THROW 51127, 'ItemFamilies is required before migration 127.', 1;

IF OBJECT_ID(N'dbo.ItemGroups', N'U') IS NULL
    THROW 51127, 'ItemGroups is required before migration 127.', 1;

IF OBJECT_ID(N'dbo.SyncInbox', N'U') IS NULL
    THROW 51127, 'SyncInbox is required before migration 127.', 1;
GO

IF COL_LENGTH(N'dbo.ItemFamilies', N'GlobalId') IS NULL
    ALTER TABLE dbo.ItemFamilies ADD GlobalId uniqueidentifier NULL;
GO

IF COL_LENGTH(N'dbo.ItemFamilies', N'ExternalSystem') IS NULL
    ALTER TABLE dbo.ItemFamilies ADD ExternalSystem nvarchar(50) NULL;
GO

IF COL_LENGTH(N'dbo.ItemFamilies', N'ExternalCode') IS NULL
    ALTER TABLE dbo.ItemFamilies ADD ExternalCode nvarchar(100) NULL;
GO

UPDATE dbo.ItemFamilies
SET GlobalId = NEWID()
WHERE GlobalId IS NULL;
GO

IF EXISTS
(
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.ItemFamilies')
      AND name = N'GlobalId'
      AND is_nullable = 1
)
    ALTER TABLE dbo.ItemFamilies ALTER COLUMN GlobalId uniqueidentifier NOT NULL;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.default_constraints AS defaultConstraint
    INNER JOIN sys.columns AS columnDefinition
        ON columnDefinition.object_id = defaultConstraint.parent_object_id
       AND columnDefinition.column_id = defaultConstraint.parent_column_id
    WHERE defaultConstraint.parent_object_id = OBJECT_ID(N'dbo.ItemFamilies')
      AND columnDefinition.name = N'GlobalId'
)
    ALTER TABLE dbo.ItemFamilies
        ADD CONSTRAINT DF_ItemFamilies_GlobalId DEFAULT NEWID() FOR GlobalId;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_ItemFamilies_GlobalId'
      AND object_id = OBJECT_ID(N'dbo.ItemFamilies')
)
    CREATE UNIQUE INDEX UX_ItemFamilies_GlobalId
        ON dbo.ItemFamilies (GlobalId);
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_ItemFamilies_ExternalRef'
      AND object_id = OBJECT_ID(N'dbo.ItemFamilies')
)
    CREATE INDEX IX_ItemFamilies_ExternalRef
        ON dbo.ItemFamilies (ExternalSystem, ExternalCode)
        WHERE ExternalSystem IS NOT NULL AND ExternalCode IS NOT NULL;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_FAMILIES_LISTAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        family.Id,
        family.GlobalId,
        family.ItemGroupId,
        itemGroup.GlobalId AS ItemGroupGlobalId,
        itemGroup.Code AS ItemGroupCode,
        itemGroup.Name AS ItemGroupName,
        family.Code,
        family.Name,
        family.ExternalSystem,
        family.ExternalCode,
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
    FROM dbo.ItemFamilies AS family
    INNER JOIN dbo.ItemGroups AS itemGroup ON itemGroup.Id = family.ItemGroupId
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
        family.GlobalId,
        family.ItemGroupId,
        itemGroup.GlobalId AS ItemGroupGlobalId,
        itemGroup.Code AS ItemGroupCode,
        itemGroup.Name AS ItemGroupName,
        family.Code,
        family.Name,
        family.ExternalSystem,
        family.ExternalCode,
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
    FROM dbo.ItemFamilies AS family
    INNER JOIN dbo.ItemGroups AS itemGroup ON itemGroup.Id = family.ItemGroupId
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
        family.GlobalId,
        family.ItemGroupId,
        itemGroup.GlobalId AS ItemGroupGlobalId,
        itemGroup.Code AS ItemGroupCode,
        itemGroup.Name AS ItemGroupName,
        family.Code,
        family.Name,
        family.ExternalSystem,
        family.ExternalCode,
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
    FROM dbo.ItemFamilies AS family
    INNER JOIN dbo.ItemGroups AS itemGroup ON itemGroup.Id = family.ItemGroupId
    WHERE family.ItemGroupId = @ItemGroupId
      AND family.IsDeleted = 0
      AND family.IsActive = 1
    ORDER BY family.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_ITEM_FAMILIES_CREAR
    @GlobalId uniqueidentifier,
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
        GlobalId,
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
        @GlobalId,
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

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_ITEM_FAMILY_SYNC_APPLY
    @GlobalId uniqueidentifier,
    @ItemGroupGlobalId uniqueidentifier,
    @Code nvarchar(50),
    @Name nvarchar(150),
    @Description nvarchar(500) = NULL,
    @IsActive bit,
    @IsDeleted bit,
    @SapFamilyCode nvarchar(100) = NULL,
    @SapCode nvarchar(50) = NULL,
    @ExternalSystem nvarchar(50) = NULL,
    @ExternalCode nvarchar(100) = NULL,
    @CreatedAt datetime2(0),
    @UpdatedAt datetime2(0)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ItemGroupId int;
    DECLARE @ItemFamilyId int;
    DECLARE @ConflictingItemFamilyId int;

    SELECT @ItemGroupId = Id
    FROM dbo.ItemGroups WITH (UPDLOCK, HOLDLOCK)
    WHERE GlobalId = @ItemGroupGlobalId;

    IF @ItemGroupId IS NULL
        THROW 51127, 'ItemGroup dependency was not found for ItemFamily sync.', 1;

    SELECT @ItemFamilyId = Id
    FROM dbo.ItemFamilies WITH (UPDLOCK, HOLDLOCK)
    WHERE GlobalId = @GlobalId;

    SELECT @ConflictingItemFamilyId = Id
    FROM dbo.ItemFamilies WITH (UPDLOCK, HOLDLOCK)
    WHERE ItemGroupId = @ItemGroupId
      AND Code = @Code
      AND IsDeleted = 0
      AND (@ItemFamilyId IS NULL OR Id <> @ItemFamilyId);

    IF @ConflictingItemFamilyId IS NOT NULL
    BEGIN
        SELECT CONVERT(int, -2) AS ResultCode,
               CONVERT(int, NULL) AS ItemFamilyId;
        RETURN;
    END;

    IF @ItemFamilyId IS NULL
    BEGIN
        INSERT INTO dbo.ItemFamilies
        (
            GlobalId,
            ItemGroupId,
            Code,
            Name,
            Description,
            IsActive,
            SapFamilyCode,
            SapCode,
            ExternalSystem,
            ExternalCode,
            CreatedAt,
            CreatedByUserName,
            IsDeleted,
            DeletedAt,
            DeletedByUserName
        )
        VALUES
        (
            @GlobalId,
            @ItemGroupId,
            @Code,
            @Name,
            @Description,
            @IsActive,
            @SapFamilyCode,
            @SapCode,
            @ExternalSystem,
            @ExternalCode,
            @CreatedAt,
            N'MasterBranchSyncWorker',
            @IsDeleted,
            CASE WHEN @IsDeleted = 1 THEN @UpdatedAt ELSE NULL END,
            CASE WHEN @IsDeleted = 1 THEN N'MasterBranchSyncWorker' ELSE NULL END
        );

        SET @ItemFamilyId = CONVERT(int, SCOPE_IDENTITY());
    END
    ELSE
    BEGIN
        UPDATE dbo.ItemFamilies
        SET ItemGroupId = @ItemGroupId,
            Code = @Code,
            Name = @Name,
            Description = @Description,
            IsActive = @IsActive,
            SapFamilyCode = @SapFamilyCode,
            SapCode = @SapCode,
            ExternalSystem = @ExternalSystem,
            ExternalCode = @ExternalCode,
            UpdatedAt = @UpdatedAt,
            UpdatedByUserName = N'MasterBranchSyncWorker',
            IsDeleted = @IsDeleted,
            DeletedAt = CASE WHEN @IsDeleted = 1 THEN @UpdatedAt ELSE NULL END,
            DeletedByUserName = CASE WHEN @IsDeleted = 1 THEN N'MasterBranchSyncWorker' ELSE NULL END
        WHERE Id = @ItemFamilyId;
    END;

    SELECT CONVERT(int, 1) AS ResultCode,
           @ItemFamilyId AS ItemFamilyId;
END;
GO

IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
    THROW 51127, 'SchemaHistory is required before recording migration 127.', 1;

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.SchemaHistory
    WHERE Version = N'20260725.127'
)
BEGIN
    INSERT INTO dbo.SchemaHistory (Version, Description)
    VALUES
    (
        N'20260725.127',
        N'ItemFamilies transaccional y aplicador Matriz-Sucursal sin adopcion por codigo'
    );
END;
GO
