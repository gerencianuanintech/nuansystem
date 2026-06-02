/*
    Ejecutar este script dentro de la base de datos de una empresa/tenant.
    Crea dimensiones contables transversales usadas por proveedores y procesos financieros.
    SQL Server es el motor principal; otros proveedores deben tener script equivalente.
*/

DECLARE @Catalogs table
(
    TableName sysname NOT NULL,
    IdColumn sysname NOT NULL,
    ProcedureToken sysname NOT NULL
);

INSERT INTO @Catalogs (TableName, IdColumn, ProcedureToken)
VALUES
    (N'Branches', N'BranchId', N'BRANCHES'),
    (N'Departments', N'DepartmentId', N'DEPARTMENTS'),
    (N'BusinessLines', N'BusinessLineId', N'BUSINESSLINES'),
    (N'CostCenters', N'CostCenterId', N'COSTCENTERS'),
    (N'Projects', N'ProjectId', N'PROJECTS');

DECLARE @TableName sysname;
DECLARE @IdColumn sysname;
DECLARE @ProcedureToken sysname;
DECLARE @Sql nvarchar(max);

DECLARE catalog_cursor CURSOR LOCAL FAST_FORWARD FOR
SELECT TableName, IdColumn, ProcedureToken FROM @Catalogs;

OPEN catalog_cursor;
FETCH NEXT FROM catalog_cursor INTO @TableName, @IdColumn, @ProcedureToken;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @Sql = N'
IF OBJECT_ID(N''dbo.' + @TableName + N''', N''U'') IS NULL
BEGIN
    CREATE TABLE dbo.' + QUOTENAME(@TableName) + N'
    (
        ' + QUOTENAME(@IdColumn) + N' int IDENTITY(1,1) NOT NULL CONSTRAINT ' + QUOTENAME(N'PK_' + @TableName) + N' PRIMARY KEY,
        Code nvarchar(30) NOT NULL,
        Name nvarchar(160) NOT NULL,
        Description nvarchar(300) NULL,
        IsActive bit NOT NULL CONSTRAINT ' + QUOTENAME(N'DF_' + @TableName + N'_IsActive') + N' DEFAULT 1,
        IsDeleted bit NOT NULL CONSTRAINT ' + QUOTENAME(N'DF_' + @TableName + N'_IsDeleted') + N' DEFAULT 0,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT ' + QUOTENAME(N'DF_' + @TableName + N'_CreatedAt') + N' DEFAULT SYSUTCDATETIME(),
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(100) NULL,
        UpdatedAt datetime2(0) NULL,
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(100) NULL,
        DeletedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(100) NULL
    );
END;';
    EXEC sys.sp_executesql @Sql;

    SET @Sql = N'
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N''UX_' + @TableName + N'_Code'' AND object_id = OBJECT_ID(N''dbo.' + @TableName + N'''))
    CREATE UNIQUE INDEX ' + QUOTENAME(N'UX_' + @TableName + N'_Code') + N' ON dbo.' + QUOTENAME(@TableName) + N'(Code) WHERE IsDeleted = 0;';
    EXEC sys.sp_executesql @Sql;

    SET @Sql = N'
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_' + @ProcedureToken + N'_LISTAR
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ' + QUOTENAME(@IdColumn) + N' AS Id, Code, Name, Description, IsActive, CreatedAt, CreatedByUserId, CreatedByUserName, UpdatedAt, UpdatedByUserId, UpdatedByUserName
    FROM dbo.' + QUOTENAME(@TableName) + N'
    WHERE IsDeleted = 0
    ORDER BY Name;
END;';
    EXEC sys.sp_executesql @Sql;

    SET @Sql = N'
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_' + @ProcedureToken + N'_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ' + QUOTENAME(@IdColumn) + N' AS Id, Code, Name, Description, IsActive, CreatedAt, CreatedByUserId, CreatedByUserName, UpdatedAt, UpdatedByUserId, UpdatedByUserName
    FROM dbo.' + QUOTENAME(@TableName) + N'
    WHERE ' + QUOTENAME(@IdColumn) + N' = @Id AND IsDeleted = 0;
END;';
    EXEC sys.sp_executesql @Sql;

    SET @Sql = N'
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_' + @ProcedureToken + N'_LOOKUP
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ' + QUOTENAME(@IdColumn) + N' AS Id, Code, Name, IsActive
    FROM dbo.' + QUOTENAME(@TableName) + N'
    WHERE IsDeleted = 0 AND IsActive = 1
    ORDER BY Name;
END;';
    EXEC sys.sp_executesql @Sql;

    SET @Sql = N'
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_' + @ProcedureToken + N'_BUSCARPORCODIGO
    @Code nvarchar(30),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1)
    FROM dbo.' + QUOTENAME(@TableName) + N'
    WHERE IsDeleted = 0
      AND Code = @Code
      AND (@ExcluirId IS NULL OR ' + QUOTENAME(@IdColumn) + N' <> @ExcluirId);
END;';
    EXEC sys.sp_executesql @Sql;

    SET @Sql = N'
CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_' + @ProcedureToken + N'_CREAR
    @Code nvarchar(30),
    @Name nvarchar(160),
    @Description nvarchar(300) = NULL,
    @IsActive bit = 1,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.' + QUOTENAME(@TableName) + N' (Code, Name, Description, IsActive, CreatedByUserId, CreatedByUserName)
    VALUES (@Code, @Name, @Description, @IsActive, @CreatedByUserId, @CreatedByUserName);
    SELECT CONVERT(int, SCOPE_IDENTITY());
END;';
    EXEC sys.sp_executesql @Sql;

    SET @Sql = N'
CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_' + @ProcedureToken + N'_ACTUALIZAR
    @Id int,
    @Code nvarchar(30),
    @Name nvarchar(160),
    @Description nvarchar(300) = NULL,
    @IsActive bit = 1,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.' + QUOTENAME(@TableName) + N'
    SET Code = @Code,
        Name = @Name,
        Description = @Description,
        IsActive = @IsActive,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName
    WHERE ' + QUOTENAME(@IdColumn) + N' = @Id
      AND IsDeleted = 0;
    SELECT @@ROWCOUNT;
END;';
    EXEC sys.sp_executesql @Sql;

    SET @Sql = N'
CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_' + @ProcedureToken + N'_ELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.' + QUOTENAME(@TableName) + N'
    SET IsDeleted = 1,
        IsActive = 0,
        DeletedAt = SYSUTCDATETIME(),
        DeletedByUserId = @DeletedByUserId,
        DeletedByUserName = @DeletedByUserName
    WHERE ' + QUOTENAME(@IdColumn) + N' = @Id
      AND IsDeleted = 0;
    SELECT @@ROWCOUNT;
END;';
    EXEC sys.sp_executesql @Sql;

    FETCH NEXT FROM catalog_cursor INTO @TableName, @IdColumn, @ProcedureToken;
END;

CLOSE catalog_cursor;
DEALLOCATE catalog_cursor;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Branches WHERE Code = N'01')
    INSERT INTO dbo.Branches (Code, Name, Description, CreatedByUserName) VALUES (N'01', N'Matriz', N'Sucursal matriz de la empresa.', N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.Departments WHERE Code = N'ADM')
    INSERT INTO dbo.Departments (Code, Name, Description, CreatedByUserName) VALUES (N'ADM', N'Administracion', N'Departamento administrativo.', N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.BusinessLines WHERE Code = N'COM')
    INSERT INTO dbo.BusinessLines (Code, Name, Description, CreatedByUserName) VALUES (N'COM', N'Comercializacion', N'Linea de negocio comercial.', N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.CostCenters WHERE Code = N'CC-ADM-001')
    INSERT INTO dbo.CostCenters (Code, Name, Description, CreatedByUserName) VALUES (N'CC-ADM-001', N'Administracion general', N'Centro de costo administrativo principal.', N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.Projects WHERE Code = N'SINPROY')
    INSERT INTO dbo.Projects (Code, Name, Description, CreatedByUserName) VALUES (N'SINPROY', N'Sin Proyecto', N'Opcion predeterminada cuando no aplica proyecto.', N'Sistema');
GO
