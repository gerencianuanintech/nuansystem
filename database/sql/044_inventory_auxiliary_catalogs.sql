SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @Catalogs TABLE
(
    Token sysname NOT NULL,
    TableName sysname NOT NULL
);

INSERT INTO @Catalogs (Token, TableName)
VALUES
    (N'UNITMEASURES', N'UnitOfMeasures'),
    (N'WAREHOUSES', N'Warehouses'),
    (N'ITEMBRANDS', N'ItemBrands'),
    (N'ITEMTYPES', N'ItemTypes'),
    (N'PRODUCTTYPES', N'ProductTypes'),
    (N'ITEMLINES', N'ItemLines'),
    (N'ITEMSUBGROUPS', N'ItemSubgroups'),
    (N'SALESCHANNELS', N'SalesChannels'),
    (N'WAREHOUSELOCATIONS', N'WarehouseLocations'),
    (N'STORAGEZONES', N'StorageZones'),
    (N'STORAGECONDITIONS', N'StorageConditions'),
    (N'REPLENISHMENTMETHODS', N'ReplenishmentMethods'),
    (N'VARIANTATTRIBUTES', N'VariantAttributes'),
    (N'ATTACHMENTDOCUMENTTYPES', N'AttachmentDocumentTypes'),
    (N'ATTACHMENTCATEGORIES', N'AttachmentCategories');

DECLARE @Token sysname;
DECLARE @TableName sysname;
DECLARE @Sql nvarchar(max);
DECLARE @QuotedTable nvarchar(260);
DECLARE @FullTable nvarchar(260);

DECLARE catalog_cursor CURSOR LOCAL FAST_FORWARD FOR
    SELECT Token, TableName
    FROM @Catalogs;

OPEN catalog_cursor;
FETCH NEXT FROM catalog_cursor INTO @Token, @TableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @QuotedTable = N'dbo.' + QUOTENAME(@TableName);
    SET @FullTable = N'dbo.' + @TableName;

    IF OBJECT_ID(@FullTable, N'U') IS NULL
    BEGIN
        SET @Sql = N'
CREATE TABLE ' + @QuotedTable + N'
(
    Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_' + @TableName + N' PRIMARY KEY,
    Code nvarchar(50) NOT NULL,
    Name nvarchar(150) NOT NULL,
    Description nvarchar(500) NULL,
    IsActive bit NOT NULL CONSTRAINT DF_' + @TableName + N'_IsActive DEFAULT (1),
    IsDeleted bit NOT NULL CONSTRAINT DF_' + @TableName + N'_IsDeleted DEFAULT (0),
    CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_' + @TableName + N'_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedByUserId int NULL,
    CreatedByUserName nvarchar(120) NULL,
    UpdatedAt datetime2(0) NULL,
    UpdatedByUserId int NULL,
    UpdatedByUserName nvarchar(120) NULL,
    DeletedAt datetime2(0) NULL,
    DeletedByUserId int NULL,
    DeletedByUserName nvarchar(120) NULL
);';
        EXEC sys.sp_executesql @Sql;
    END;

    IF COL_LENGTH(@FullTable, N'Description') IS NULL
    BEGIN
        SET @Sql = N'ALTER TABLE ' + @QuotedTable + N' ADD Description nvarchar(500) NULL;';
        EXEC sys.sp_executesql @Sql;
    END;

    IF COL_LENGTH(@FullTable, N'IsDeleted') IS NULL
    BEGIN
        SET @Sql = N'ALTER TABLE ' + @QuotedTable + N' ADD IsDeleted bit NOT NULL CONSTRAINT DF_' + @TableName + N'_IsDeleted DEFAULT (0);';
        EXEC sys.sp_executesql @Sql;
    END;

    IF COL_LENGTH(@FullTable, N'DeletedAt') IS NULL
    BEGIN
        SET @Sql = N'
ALTER TABLE ' + @QuotedTable + N' ADD
    DeletedAt datetime2(0) NULL,
    DeletedByUserId int NULL,
    DeletedByUserName nvarchar(120) NULL;';
        EXEC sys.sp_executesql @Sql;
    END;

    IF @TableName = N'UnitOfMeasures' AND COL_LENGTH(@FullTable, N'Code') IS NOT NULL
    BEGIN
        IF EXISTS (
            SELECT 1
            FROM sys.columns
            WHERE object_id = OBJECT_ID(@FullTable)
              AND name = N'Code'
              AND max_length < 100
        )
        BEGIN
            SET @Sql = N'ALTER TABLE ' + @QuotedTable + N' ALTER COLUMN Code nvarchar(50) NOT NULL;';
            EXEC sys.sp_executesql @Sql;
        END;
    END;

    IF NOT EXISTS (
        SELECT 1
        FROM sys.indexes
        WHERE object_id = OBJECT_ID(@FullTable)
          AND name = N'UX_' + @TableName + N'_Code_Active'
    )
    BEGIN
        SET @Sql = N'CREATE UNIQUE INDEX UX_' + @TableName + N'_Code_Active ON ' + @QuotedTable + N'(Code) WHERE IsDeleted = 0;';
        EXEC sys.sp_executesql @Sql;
    END;

    IF NOT EXISTS (
        SELECT 1
        FROM sys.indexes
        WHERE object_id = OBJECT_ID(@FullTable)
          AND name = N'IX_' + @TableName + N'_Name_Active'
    )
    BEGIN
        SET @Sql = N'CREATE INDEX IX_' + @TableName + N'_Name_Active ON ' + @QuotedTable + N'(Name, IsActive) INCLUDE (Code) WHERE IsDeleted = 0;';
        EXEC sys.sp_executesql @Sql;
    END;

    SET @Sql = N'
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_' + @Token + N'_LISTAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Code,
        Name,
        Description,
        CAST(IsActive AS bit) AS IsActive,
        CreatedByUserId,
        CreatedByUserName,
        CreatedAt,
        UpdatedByUserId,
        UpdatedByUserName,
        UpdatedAt,
        DeletedByUserId,
        DeletedByUserName,
        DeletedAt
    FROM ' + @QuotedTable + N'
    WHERE IsDeleted = 0
    ORDER BY Name, Code;
END;';
    EXEC sys.sp_executesql @Sql;

    SET @Sql = N'
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_' + @Token + N'_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        Id,
        Code,
        Name,
        Description,
        CAST(IsActive AS bit) AS IsActive,
        CreatedByUserId,
        CreatedByUserName,
        CreatedAt,
        UpdatedByUserId,
        UpdatedByUserName,
        UpdatedAt,
        DeletedByUserId,
        DeletedByUserName,
        DeletedAt
    FROM ' + @QuotedTable + N'
    WHERE Id = @Id
      AND IsDeleted = 0;
END;';
    EXEC sys.sp_executesql @Sql;

    SET @Sql = N'
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_' + @Token + N'_LOOKUP
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Code,
        Name,
        CAST(IsActive AS bit) AS IsActive
    FROM ' + @QuotedTable + N'
    WHERE IsDeleted = 0
      AND IsActive = 1
    ORDER BY Name, Code;
END;';
    EXEC sys.sp_executesql @Sql;

    SET @Sql = N'
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_GENERAL_INVENTORY_' + @Token + N'_BUSCARPORCODIGO
    @Code nvarchar(50),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM ' + @QuotedTable + N'
    WHERE Code = @Code
      AND IsDeleted = 0
      AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;';
    EXEC sys.sp_executesql @Sql;

    SET @Sql = N'
CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_GENERAL_INVENTORY_' + @Token + N'_CREAR
    @Code nvarchar(50),
    @Name nvarchar(150),
    @Description nvarchar(500) = NULL,
    @IsActive bit = 1,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO ' + @QuotedTable + N'
    (
        Code,
        Name,
        Description,
        IsActive,
        IsDeleted,
        CreatedAt,
        CreatedByUserId,
        CreatedByUserName
    )
    VALUES
    (
        @Code,
        @Name,
        @Description,
        @IsActive,
        0,
        SYSUTCDATETIME(),
        @CreatedByUserId,
        @CreatedByUserName
    );

    SELECT CONVERT(int, SCOPE_IDENTITY());
END;';
    EXEC sys.sp_executesql @Sql;

    SET @Sql = N'
CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_GENERAL_INVENTORY_' + @Token + N'_ACTUALIZAR
    @Id int,
    @Code nvarchar(50),
    @Name nvarchar(150),
    @Description nvarchar(500) = NULL,
    @IsActive bit = 1,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE ' + @QuotedTable + N'
    SET Code = @Code,
        Name = @Name,
        Description = @Description,
        IsActive = @IsActive,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName
    WHERE Id = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;';
    EXEC sys.sp_executesql @Sql;

    SET @Sql = N'
CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_GENERAL_INVENTORY_' + @Token + N'_ELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE ' + @QuotedTable + N'
    SET IsDeleted = 1,
        IsActive = 0,
        DeletedAt = SYSUTCDATETIME(),
        DeletedByUserId = @DeletedByUserId,
        DeletedByUserName = @DeletedByUserName
    WHERE Id = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;';
    EXEC sys.sp_executesql @Sql;

    FETCH NEXT FROM catalog_cursor INTO @Token, @TableName;
END;

CLOSE catalog_cursor;
DEALLOCATE catalog_cursor;

DECLARE @Seed TABLE
(
    TableName sysname NOT NULL,
    Code nvarchar(50) NOT NULL,
    Name nvarchar(150) NOT NULL,
    Description nvarchar(500) NULL
);

INSERT INTO @Seed (TableName, Code, Name, Description)
VALUES
    (N'UnitOfMeasures', N'UND', N'Unidad', N'Unidad base de inventario.'),
    (N'UnitOfMeasures', N'CAJ', N'Caja', N'Presentacion de caja para compra o venta.'),
    (N'UnitOfMeasures', N'PAQ', N'Paquete', N'Presentacion de paquete.'),
    (N'UnitOfMeasures', N'BUL', N'Bulto', N'Presentacion de bulto.'),
    (N'UnitOfMeasures', N'KG', N'Kilogramo', N'Unidad de peso.'),
    (N'UnitOfMeasures', N'G', N'Gramo', N'Unidad de peso menor.'),
    (N'UnitOfMeasures', N'L', N'Litro', N'Unidad de volumen.'),
    (N'Warehouses', N'MATRIZ', N'Matriz', N'Bodega principal.'),
    (N'Warehouses', N'TRANSITO', N'Transito', N'Bodega de transito.'),
    (N'ItemBrands', N'GEN', N'Generica', N'Marca generica.'),
    (N'ItemTypes', N'PRODUCTO', N'Producto', N'Articulo inventariable o vendible.'),
    (N'ItemTypes', N'SERVICIO', N'Servicio', N'Articulo sin stock fisico.'),
    (N'ItemTypes', N'INSUMO', N'Insumo', N'Articulo usado como materia prima o consumo interno.'),
    (N'ItemTypes', N'KIT', N'Kit', N'Articulo compuesto por otros articulos.'),
    (N'ProductTypes', N'MERCADERIA', N'Mercaderia', N'Producto comercial para compra y venta.'),
    (N'ProductTypes', N'PROD_TERM', N'Producto terminado', N'Producto final listo para venta.'),
    (N'ProductTypes', N'MAT_PRIMA', N'Materia prima', N'Insumo para transformacion.'),
    (N'ItemLines', N'GENERAL', N'General', N'Linea general.'),
    (N'ItemSubgroups', N'GENERAL', N'General', N'Subgrupo general.'),
    (N'SalesChannels', N'LOCAL', N'Local', N'Venta en punto fisico.'),
    (N'SalesChannels', N'ECOMMERCE', N'E-commerce', N'Venta por canal digital.'),
    (N'SalesChannels', N'MAYORISTA', N'Mayorista', N'Canal de venta mayorista.'),
    (N'WarehouseLocations', N'A1-01-01', N'A1-01-01', N'Ubicacion inicial de bodega.'),
    (N'StorageZones', N'SECO', N'Seco', N'Zona seca.'),
    (N'StorageZones', N'FRIO', N'Frio', N'Zona fria.'),
    (N'StorageConditions', N'AMBIENTE', N'Ambiente', N'Condicion ambiental normal.'),
    (N'StorageConditions', N'REFRIGERADO', N'Refrigerado', N'Condicion de refrigeracion.'),
    (N'ReplenishmentMethods', N'COMPRAR', N'Comprar', N'Reposicion por compra.'),
    (N'ReplenishmentMethods', N'FABRICAR', N'Fabricar', N'Reposicion por produccion.'),
    (N'ReplenishmentMethods', N'TRANSFERIR', N'Transferir', N'Reposicion por transferencia entre bodegas.'),
    (N'VariantAttributes', N'PRESENTACION', N'Presentacion', N'Atributo para variantes por presentacion.'),
    (N'VariantAttributes', N'COLOR', N'Color', N'Atributo para variantes por color.'),
    (N'VariantAttributes', N'TALLA', N'Talla', N'Atributo para variantes por talla.'),
    (N'AttachmentDocumentTypes', N'IMG_PRODUCTO', N'Imagen producto', N'Imagen comercial del articulo.'),
    (N'AttachmentDocumentTypes', N'FICHA_TEC', N'Ficha tecnica', N'Documento tecnico del articulo.'),
    (N'AttachmentDocumentTypes', N'REG_SAN', N'Registro sanitario', N'Documento sanitario o regulatorio.'),
    (N'AttachmentCategories', N'COMERCIAL', N'Comercial', N'Archivo visible en procesos comerciales.'),
    (N'AttachmentCategories', N'CALIDAD', N'Calidad', N'Archivo de calidad o cumplimiento.'),
    (N'AttachmentCategories', N'LOGISTICA', N'Logistica', N'Archivo logistico.');

DECLARE seed_cursor CURSOR LOCAL FAST_FORWARD FOR
    SELECT TableName, Code, Name, Description
    FROM @Seed;

DECLARE @Code nvarchar(50);
DECLARE @Name nvarchar(150);
DECLARE @Description nvarchar(500);

OPEN seed_cursor;
FETCH NEXT FROM seed_cursor INTO @TableName, @Code, @Name, @Description;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @QuotedTable = N'dbo.' + QUOTENAME(@TableName);
    SET @Sql = N'
IF NOT EXISTS (SELECT 1 FROM ' + @QuotedTable + N' WHERE Code = @Code AND IsDeleted = 0)
BEGIN
    INSERT INTO ' + @QuotedTable + N' (Code, Name, Description, IsActive, IsDeleted, CreatedAt, CreatedByUserName)
    VALUES (@Code, @Name, @Description, 1, 0, SYSUTCDATETIME(), N''Sistema'');
END;';

    EXEC sys.sp_executesql
        @Sql,
        N'@Code nvarchar(50), @Name nvarchar(150), @Description nvarchar(500)',
        @Code = @Code,
        @Name = @Name,
        @Description = @Description;

    FETCH NEXT FROM seed_cursor INTO @TableName, @Code, @Name, @Description;
END;

CLOSE seed_cursor;
DEALLOCATE seed_cursor;
