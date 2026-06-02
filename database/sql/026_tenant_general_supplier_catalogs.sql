/*
    NuanSystem - GeneralSupplier tenant catalogs
    SQL Server remains the primary provider. This script uses SQL Server-specific
    idempotent DDL and keeps provider-specific details inside database scripts.
*/

DECLARE @Catalogs table
(
    TableName sysname NOT NULL,
    IdColumn sysname NOT NULL,
    ProcedureName sysname NOT NULL,
    SeedValues nvarchar(max) NULL
);

INSERT INTO @Catalogs (TableName, IdColumn, ProcedureName, SeedValues)
VALUES
    (N'SupplierGroups', N'SupplierGroupId', N'SUPPLIERGROUPS', N'(N''NAC'', N''Proveedores Nacionales'', N''Proveedor ubicado dentro del pais.''),(N''IMP'', N''Importadores'', N''Proveedor de importacion.''),(N''SER'', N''Servicios'', N''Proveedor de servicios.'')'),
    (N'SupplierClasses', N'SupplierClassId', N'SUPPLIERCLASSES', N'(N''MAT'', N''Materiales e Insumos'', N''Proveedor de materiales e insumos.''),(N''TRA'', N''Transporte'', N''Proveedor logistico o transporte.''),(N''SER'', N''Servicios profesionales'', N''Proveedor de servicios profesionales.'')'),
    (N'EconomicActivities', N'EconomicActivityId', N'ECONOMICACTIVITIES', N'(N''COM'', N''Comercio al por mayor'', N''Actividad comercial mayorista.''),(N''IND'', N''Industria manufacturera'', N''Actividad industrial.''),(N''SER'', N''Servicios prestados'', N''Actividad de servicios.'')'),
    (N'Zones', N'ZoneId', N'ZONES', N'(N''SIE'', N''Zona 1 - Sierra'', N''Zona comercial de la sierra.''),(N''COS'', N''Zona 2 - Costa'', N''Zona comercial de la costa.''),(N''AUS'', N''Zona 3 - Austro'', N''Zona comercial del austro.'')'),
    (N'SupplyMethods', N'SupplyMethodId', N'SUPPLYMETHODS', N'(N''LOC'', N''Compra local'', N''Abastecimiento mediante compra local.''),(N''IMP'', N''Importacion'', N''Abastecimiento por importacion.''),(N''REC'', N''Servicio recurrente'', N''Abastecimiento por servicio recurrente.'')'),
    (N'ContactTypes', N'ContactTypeId', N'CONTACTTYPES', N'(N''COM'', N''Comercial'', N''Contacto comercial.''),(N''FIN'', N''Financiero'', N''Contacto financiero.''),(N''LOG'', N''Logistico'', N''Contacto logistico.'')'),
    (N'ContactChannels', N'ContactChannelId', N'CONTACTCHANNELS', N'(N''EMAIL'', N''Correo electronico'', N''Canal por correo electronico.''),(N''PHONE'', N''Telefono'', N''Canal telefonico.''),(N''WHATS'', N''WhatsApp'', N''Canal por WhatsApp.'')');

DECLARE
    @TableName sysname,
    @IdColumn sysname,
    @ProcedureName sysname,
    @SeedValues nvarchar(max),
    @sql nvarchar(max);

DECLARE catalog_cursor CURSOR LOCAL FAST_FORWARD FOR
SELECT TableName, IdColumn, ProcedureName, SeedValues
FROM @Catalogs;

OPEN catalog_cursor;
FETCH NEXT FROM catalog_cursor INTO @TableName, @IdColumn, @ProcedureName, @SeedValues;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @sql = N'
IF OBJECT_ID(N''dbo.' + @TableName + N''', N''U'') IS NULL
BEGIN
    CREATE TABLE dbo.' + QUOTENAME(@TableName) + N'
    (
        ' + QUOTENAME(@IdColumn) + N' int IDENTITY(1,1) NOT NULL CONSTRAINT PK_' + @TableName + N' PRIMARY KEY,
        Code nvarchar(50) NOT NULL,
        Name nvarchar(150) NOT NULL,
        Description nvarchar(500) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_' + @TableName + N'_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_' + @TableName + N'_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_' + @TableName + N'_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N''UX_' + @TableName + N'_Code_Active'' AND object_id = OBJECT_ID(N''dbo.' + @TableName + N'''))
BEGIN
    CREATE UNIQUE INDEX UX_' + @TableName + N'_Code_Active ON dbo.' + QUOTENAME(@TableName) + N' (Code) WHERE IsDeleted = 0;
END;';
    EXEC sys.sp_executesql @sql;

    IF @SeedValues IS NOT NULL
    BEGIN
        SET @sql = N'
INSERT INTO dbo.' + QUOTENAME(@TableName) + N' (Code, Name, Description, IsActive)
SELECT source.Code, source.Name, source.Description, 1
FROM (VALUES ' + @SeedValues + N') AS source(Code, Name, Description)
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.' + QUOTENAME(@TableName) + N' existing
    WHERE existing.Code = source.Code
      AND existing.IsDeleted = 0
);';
        EXEC sys.sp_executesql @sql;
    END;

    SET @sql = N'
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_SUPPLIER_' + @ProcedureName + N'_LISTAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ' + QUOTENAME(@IdColumn) + N' AS Id,
        Code,
        Name,
        Description,
        IsActive,
        CreatedByUserId,
        CreatedByUserName,
        CreatedAt,
        UpdatedByUserId,
        UpdatedByUserName,
        UpdatedAt,
        DeletedByUserId,
        DeletedByUserName,
        DeletedAt
    FROM dbo.' + QUOTENAME(@TableName) + N'
    WHERE IsDeleted = 0
    ORDER BY Code;
END;';
    EXEC sys.sp_executesql @sql;

    SET @sql = N'
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_SUPPLIER_' + @ProcedureName + N'_LOOKUP
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ' + QUOTENAME(@IdColumn) + N' AS Id,
        Code,
        Name,
        IsActive
    FROM dbo.' + QUOTENAME(@TableName) + N'
    WHERE IsDeleted = 0
      AND IsActive = 1
    ORDER BY Name;
END;';
    EXEC sys.sp_executesql @sql;

    SET @sql = N'
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_SUPPLIER_' + @ProcedureName + N'_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ' + QUOTENAME(@IdColumn) + N' AS Id,
        Code,
        Name,
        Description,
        IsActive,
        CreatedByUserId,
        CreatedByUserName,
        CreatedAt,
        UpdatedByUserId,
        UpdatedByUserName,
        UpdatedAt,
        DeletedByUserId,
        DeletedByUserName,
        DeletedAt
    FROM dbo.' + QUOTENAME(@TableName) + N'
    WHERE ' + QUOTENAME(@IdColumn) + N' = @Id
      AND IsDeleted = 0;
END;';
    EXEC sys.sp_executesql @sql;

    SET @sql = N'
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_SUPPLIER_' + @ProcedureName + N'_BUSCARPORCODIGO
    @Code nvarchar(50),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM dbo.' + QUOTENAME(@TableName) + N'
    WHERE Code = @Code
      AND IsDeleted = 0
      AND (@ExcluirId IS NULL OR ' + QUOTENAME(@IdColumn) + N' <> @ExcluirId);
END;';
    EXEC sys.sp_executesql @sql;

    SET @sql = N'
CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_GENERAL_SUPPLIER_' + @ProcedureName + N'_CREAR
    @Code nvarchar(50),
    @Name nvarchar(150),
    @Description nvarchar(500) = NULL,
    @IsActive bit,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.' + QUOTENAME(@TableName) + N'
    (
        Code,
        Name,
        Description,
        IsActive,
        CreatedByUserId,
        CreatedByUserName
    )
    VALUES
    (
        @Code,
        @Name,
        @Description,
        @IsActive,
        @CreatedByUserId,
        @CreatedByUserName
    );

    SELECT CONVERT(int, SCOPE_IDENTITY());
END;';
    EXEC sys.sp_executesql @sql;

    SET @sql = N'
CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_GENERAL_SUPPLIER_' + @ProcedureName + N'_ACTUALIZAR
    @Id int,
    @Code nvarchar(50),
    @Name nvarchar(150),
    @Description nvarchar(500) = NULL,
    @IsActive bit,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.' + QUOTENAME(@TableName) + N'
    SET
        Code = @Code,
        Name = @Name,
        Description = @Description,
        IsActive = @IsActive,
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE ' + QUOTENAME(@IdColumn) + N' = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;';
    EXEC sys.sp_executesql @sql;

    SET @sql = N'
CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_GENERAL_SUPPLIER_' + @ProcedureName + N'_ELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.' + QUOTENAME(@TableName) + N'
    SET
        IsDeleted = 1,
        DeletedByUserId = @DeletedByUserId,
        DeletedByUserName = @DeletedByUserName,
        DeletedAt = SYSUTCDATETIME()
    WHERE ' + QUOTENAME(@IdColumn) + N' = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;';
    EXEC sys.sp_executesql @sql;

    FETCH NEXT FROM catalog_cursor INTO @TableName, @IdColumn, @ProcedureName, @SeedValues;
END;

CLOSE catalog_cursor;
DEALLOCATE catalog_cursor;

