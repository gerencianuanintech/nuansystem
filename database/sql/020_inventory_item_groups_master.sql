/*
    Maestro de Grupos de Artículos
    Extiende dbo.ItemGroups sin romper los lookups actuales usados por Items.
*/

IF OBJECT_ID(N'dbo.ItemGroups', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ItemGroups
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ItemGroups PRIMARY KEY,
        Code nvarchar(50) NOT NULL,
        Name nvarchar(150) NOT NULL,
        Description nvarchar(500) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_ItemGroups_IsActive DEFAULT 1,
        InventoryAccountCode nvarchar(120) NULL,
        CostOfSalesAccountCode nvarchar(120) NULL,
        SalesAccountCode nvarchar(120) NULL,
        PurchaseAccountCode nvarchar(120) NULL,
        SapGroupCode nvarchar(100) NULL,
        SapCode nvarchar(50) NULL,
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

IF COL_LENGTH('dbo.ItemGroups', 'Description') IS NULL
    ALTER TABLE dbo.ItemGroups ADD Description nvarchar(500) NULL;
GO

IF COL_LENGTH('dbo.ItemGroups', 'InventoryAccountCode') IS NULL
    ALTER TABLE dbo.ItemGroups ADD InventoryAccountCode nvarchar(120) NULL;
GO

IF COL_LENGTH('dbo.ItemGroups', 'CostOfSalesAccountCode') IS NULL
    ALTER TABLE dbo.ItemGroups ADD CostOfSalesAccountCode nvarchar(120) NULL;
GO

IF COL_LENGTH('dbo.ItemGroups', 'SalesAccountCode') IS NULL
    ALTER TABLE dbo.ItemGroups ADD SalesAccountCode nvarchar(120) NULL;
GO

IF COL_LENGTH('dbo.ItemGroups', 'PurchaseAccountCode') IS NULL
    ALTER TABLE dbo.ItemGroups ADD PurchaseAccountCode nvarchar(120) NULL;
GO

IF COL_LENGTH('dbo.ItemGroups', 'SapGroupCode') IS NULL
    ALTER TABLE dbo.ItemGroups ADD SapGroupCode nvarchar(100) NULL;
GO

IF COL_LENGTH('dbo.ItemGroups', 'SapCode') IS NULL
    ALTER TABLE dbo.ItemGroups ADD SapCode nvarchar(50) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_ItemGroups_Code_Active' AND object_id = OBJECT_ID(N'dbo.ItemGroups'))
BEGIN
    CREATE UNIQUE INDEX UX_ItemGroups_Code_Active ON dbo.ItemGroups (Code) WHERE IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_GROUPS_LISTAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Code,
        Name,
        Description,
        IsActive,
        InventoryAccountCode,
        CostOfSalesAccountCode,
        SalesAccountCode,
        PurchaseAccountCode,
        SapGroupCode,
        SapCode,
        CreatedByUserId,
        CreatedByUserName,
        CreatedAt,
        UpdatedByUserId,
        UpdatedByUserName,
        UpdatedAt,
        DeletedByUserId,
        DeletedByUserName,
        DeletedAt
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

    SELECT
        Id,
        Code,
        Name,
        Description,
        IsActive,
        InventoryAccountCode,
        CostOfSalesAccountCode,
        SalesAccountCode,
        PurchaseAccountCode,
        SapGroupCode,
        SapCode,
        CreatedByUserId,
        CreatedByUserName,
        CreatedAt,
        UpdatedByUserId,
        UpdatedByUserName,
        UpdatedAt,
        DeletedByUserId,
        DeletedByUserName,
        DeletedAt
    FROM dbo.ItemGroups
    WHERE Id = @Id AND IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEM_GROUPSBUSCARPORCODIGO
    @Code nvarchar(50),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM dbo.ItemGroups
    WHERE Code = @Code
      AND IsDeleted = 0
      AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_ITEM_GROUPS_CREAR
    @Code nvarchar(50),
    @Name nvarchar(150),
    @Description nvarchar(500) = NULL,
    @IsActive bit,
    @InventoryAccountCode nvarchar(120) = NULL,
    @CostOfSalesAccountCode nvarchar(120) = NULL,
    @SalesAccountCode nvarchar(120) = NULL,
    @PurchaseAccountCode nvarchar(120) = NULL,
    @SapGroupCode nvarchar(100) = NULL,
    @SapCode nvarchar(50) = NULL,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.ItemGroups
    (
        Code,
        Name,
        Description,
        IsActive,
        InventoryAccountCode,
        CostOfSalesAccountCode,
        SalesAccountCode,
        PurchaseAccountCode,
        SapGroupCode,
        SapCode,
        CreatedByUserId,
        CreatedByUserName
    )
    VALUES
    (
        @Code,
        @Name,
        @Description,
        @IsActive,
        @InventoryAccountCode,
        @CostOfSalesAccountCode,
        @SalesAccountCode,
        @PurchaseAccountCode,
        @SapGroupCode,
        @SapCode,
        @CreatedByUserId,
        @CreatedByUserName
    );

    SELECT CONVERT(int, SCOPE_IDENTITY());
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_ITEM_GROUPS_ACTUALIZAR
    @Id int,
    @Code nvarchar(50),
    @Name nvarchar(150),
    @Description nvarchar(500) = NULL,
    @IsActive bit,
    @InventoryAccountCode nvarchar(120) = NULL,
    @CostOfSalesAccountCode nvarchar(120) = NULL,
    @SalesAccountCode nvarchar(120) = NULL,
    @PurchaseAccountCode nvarchar(120) = NULL,
    @SapGroupCode nvarchar(100) = NULL,
    @SapCode nvarchar(50) = NULL,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.ItemGroups
    SET
        Code = @Code,
        Name = @Name,
        Description = @Description,
        IsActive = @IsActive,
        InventoryAccountCode = @InventoryAccountCode,
        CostOfSalesAccountCode = @CostOfSalesAccountCode,
        SalesAccountCode = @SalesAccountCode,
        PurchaseAccountCode = @PurchaseAccountCode,
        SapGroupCode = @SapGroupCode,
        SapCode = @SapCode,
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_ITEM_GROUPS_ELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.ItemGroups
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
