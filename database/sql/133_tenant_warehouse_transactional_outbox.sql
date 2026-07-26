/*
    Contrato tenant Warehouse: CRUD por procedimientos, eliminacion logica,
    identidad GlobalId y aplicacion Matriz-Sucursal con payload corporativo minimo.
    No habilita workers ni entidades.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.Warehouses', N'U') IS NULL OR OBJECT_ID(N'dbo.LocalOutbox', N'U') IS NULL
    THROW 51133, 'Warehouses and LocalOutbox are required before migration 133.', 1;
GO

IF EXISTS
(
    SELECT Code
    FROM dbo.Warehouses
    WHERE IsDeleted = 0
    GROUP BY Code
    HAVING COUNT(*) > 1
)
    THROW 51133, 'Active Warehouse codes must be unique before migration 133.', 1;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.Warehouses')
      AND name = N'UX_Warehouses_Code_Active'
)
    CREATE UNIQUE INDEX UX_Warehouses_Code_Active
        ON dbo.Warehouses(Code) WHERE IsDeleted = 0;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_WAREHOUSES_LISTAR
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, GlobalId, Code, Name, Description, BranchCode, Address, City, Province, Country,
           Phone, Email, ManagerName, AllowsSales, AllowsPurchases, AllowsTransfers, AllowsProduction,
           IsDefault, ExternalSystem, ExternalCode, SapCode, IsActive, CreatedByUserId,
           CreatedByUserName, CreatedAt, UpdatedByUserId, UpdatedByUserName, UpdatedAt,
           DeletedByUserId, DeletedByUserName, DeletedAt
    FROM dbo.Warehouses WHERE IsDeleted = 0
    ORDER BY IsDefault DESC, Name, Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_WAREHOUSES_BUSCARPORID @Id int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP (1) Id, GlobalId, Code, Name, Description, BranchCode, Address, City, Province, Country,
           Phone, Email, ManagerName, AllowsSales, AllowsPurchases, AllowsTransfers, AllowsProduction,
           IsDefault, ExternalSystem, ExternalCode, SapCode, IsActive, CreatedByUserId,
           CreatedByUserName, CreatedAt, UpdatedByUserId, UpdatedByUserName, UpdatedAt,
           DeletedByUserId, DeletedByUserName, DeletedAt
    FROM dbo.Warehouses WHERE Id = @Id AND IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_WAREHOUSESBUSCARPORCODIGO
    @Code nvarchar(50), @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1) FROM dbo.Warehouses
    WHERE Code = @Code AND IsDeleted = 0 AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_WAREHOUSES_CREAR
    @GlobalId uniqueidentifier, @Code nvarchar(50), @Name nvarchar(150),
    @Description nvarchar(500)=NULL, @BranchCode nvarchar(50)=NULL, @Address nvarchar(250)=NULL,
    @City nvarchar(100)=NULL, @Province nvarchar(100)=NULL, @Country nvarchar(100)=NULL,
    @Phone nvarchar(50)=NULL, @Email nvarchar(150)=NULL, @ManagerName nvarchar(150)=NULL,
    @AllowsSales bit, @AllowsPurchases bit, @AllowsTransfers bit, @AllowsProduction bit,
    @IsDefault bit, @ExternalSystem nvarchar(50)=NULL, @ExternalCode nvarchar(100)=NULL,
    @SapCode nvarchar(100)=NULL, @IsActive bit, @CreatedByUserId int=NULL,
    @CreatedByUserName nvarchar(120)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT dbo.Warehouses
    (GlobalId,Code,Name,Description,BranchCode,Address,City,Province,Country,Phone,Email,ManagerName,
     AllowsSales,AllowsPurchases,AllowsTransfers,AllowsProduction,IsDefault,ExternalSystem,ExternalCode,
     SapCode,IsActive,IsDeleted,CreatedAt,CreatedByUserId,CreatedByUserName)
    VALUES
    (@GlobalId,@Code,@Name,@Description,@BranchCode,@Address,@City,@Province,@Country,@Phone,@Email,@ManagerName,
     @AllowsSales,@AllowsPurchases,@AllowsTransfers,@AllowsProduction,@IsDefault,@ExternalSystem,@ExternalCode,
     @SapCode,@IsActive,0,SYSUTCDATETIME(),@CreatedByUserId,@CreatedByUserName);
    SELECT CONVERT(int,SCOPE_IDENTITY());
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_WAREHOUSES_ACTUALIZAR
    @Id int, @GlobalId uniqueidentifier, @Code nvarchar(50), @Name nvarchar(150),
    @Description nvarchar(500)=NULL, @BranchCode nvarchar(50)=NULL, @Address nvarchar(250)=NULL,
    @City nvarchar(100)=NULL, @Province nvarchar(100)=NULL, @Country nvarchar(100)=NULL,
    @Phone nvarchar(50)=NULL, @Email nvarchar(150)=NULL, @ManagerName nvarchar(150)=NULL,
    @AllowsSales bit, @AllowsPurchases bit, @AllowsTransfers bit, @AllowsProduction bit,
    @IsDefault bit, @ExternalSystem nvarchar(50)=NULL, @ExternalCode nvarchar(100)=NULL,
    @SapCode nvarchar(100)=NULL, @IsActive bit, @UpdatedByUserId int=NULL,
    @UpdatedByUserName nvarchar(120)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Warehouses SET GlobalId=@GlobalId,Code=@Code,Name=@Name,Description=@Description,
        BranchCode=@BranchCode,Address=@Address,City=@City,Province=@Province,Country=@Country,
        Phone=@Phone,Email=@Email,ManagerName=@ManagerName,AllowsSales=@AllowsSales,
        AllowsPurchases=@AllowsPurchases,AllowsTransfers=@AllowsTransfers,AllowsProduction=@AllowsProduction,
        IsDefault=@IsDefault,ExternalSystem=@ExternalSystem,ExternalCode=@ExternalCode,SapCode=@SapCode,
        IsActive=@IsActive,UpdatedAt=SYSUTCDATETIME(),UpdatedByUserId=@UpdatedByUserId,
        UpdatedByUserName=@UpdatedByUserName
    WHERE Id=@Id AND IsDeleted=0;
    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PATCH_WAREHOUSES_ESTADO
    @Id int, @IsActive bit, @UpdatedByUserId int=NULL, @UpdatedByUserName nvarchar(120)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Warehouses SET IsActive=@IsActive,UpdatedAt=SYSUTCDATETIME(),
        UpdatedByUserId=@UpdatedByUserId,UpdatedByUserName=@UpdatedByUserName
    WHERE Id=@Id AND IsDeleted=0;
    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_WAREHOUSES_ELIMINAR
    @Id int, @DeletedByUserId int=NULL, @DeletedByUserName nvarchar(120)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Warehouses SET IsDeleted=1,IsActive=0,DeletedAt=SYSUTCDATETIME(),
        DeletedByUserId=@DeletedByUserId,DeletedByUserName=@DeletedByUserName
    WHERE Id=@Id AND IsDeleted=0;
    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_WAREHOUSE_SYNC_APPLY
    @GlobalId uniqueidentifier, @Code nvarchar(50), @Name nvarchar(150), @IsActive bit,
    @ExternalSystem nvarchar(50)=NULL, @ExternalCode nvarchar(100)=NULL,
    @SapCode nvarchar(100)=NULL, @CreatedAt datetime2(0), @UpdatedAt datetime2(0)=NULL,
    @IsDeleted bit
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @WarehouseId int;
    SELECT @WarehouseId=Id FROM dbo.Warehouses WITH(UPDLOCK,HOLDLOCK) WHERE GlobalId=@GlobalId;
    IF EXISTS(SELECT 1 FROM dbo.Warehouses WITH(UPDLOCK,HOLDLOCK)
              WHERE Code=@Code AND GlobalId<>@GlobalId)
    BEGIN
        SELECT -2 AS ResultCode, CONVERT(int,NULL) AS WarehouseId;
        RETURN;
    END;
    IF @WarehouseId IS NULL
    BEGIN
        INSERT dbo.Warehouses
        (GlobalId,Code,Name,ExternalSystem,ExternalCode,SapCode,IsActive,IsDeleted,CreatedAt,
         CreatedByUserName,DeletedAt,DeletedByUserName)
        VALUES(@GlobalId,@Code,@Name,@ExternalSystem,@ExternalCode,@SapCode,@IsActive,@IsDeleted,
               COALESCE(@CreatedAt,SYSUTCDATETIME()),N'MasterBranchSyncWorker',
               CASE WHEN @IsDeleted=1 THEN SYSUTCDATETIME() END,
               CASE WHEN @IsDeleted=1 THEN N'MasterBranchSyncWorker' END);
        SET @WarehouseId=CONVERT(int,SCOPE_IDENTITY());
    END
    ELSE
    BEGIN
        UPDATE dbo.Warehouses
        SET Code=@Code,Name=@Name,ExternalSystem=@ExternalSystem,ExternalCode=@ExternalCode,
            SapCode=@SapCode,IsActive=@IsActive,IsDeleted=@IsDeleted,
            UpdatedAt=COALESCE(@UpdatedAt,SYSUTCDATETIME()),UpdatedByUserName=N'MasterBranchSyncWorker',
            DeletedAt=CASE WHEN @IsDeleted=1 THEN COALESCE(DeletedAt,SYSUTCDATETIME()) END,
            DeletedByUserName=CASE WHEN @IsDeleted=1 THEN N'MasterBranchSyncWorker' END
        WHERE Id=@WarehouseId;
    END;
    SELECT 1 AS ResultCode,@WarehouseId AS WarehouseId;
END;
GO

IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
    THROW 51133, 'SchemaHistory is required before recording migration 133.', 1;
IF NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260726.133')
    INSERT dbo.SchemaHistory(Version,Description)
    VALUES(N'20260726.133',N'Warehouse CRUD transaccional y aplicacion Matriz-Sucursal minima');
GO
