/*
    Ejecutar este script en cada base tenant.
    Crea el maestro generico de catalogos operativos por empresa.
*/

IF OBJECT_ID(N'dbo.OperationalCatalog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.OperationalCatalog
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_OperationalCatalog PRIMARY KEY,
        CatalogKey nvarchar(80) NOT NULL,
        Code nvarchar(40) NOT NULL,
        Name nvarchar(150) NOT NULL,
        Description nvarchar(500) NULL,
        ParentCatalogKey nvarchar(80) NULL,
        ParentCode nvarchar(40) NULL,
        DisplayOrder int NOT NULL CONSTRAINT DF_OperationalCatalog_DisplayOrder DEFAULT 0,
        IsDefault bit NOT NULL CONSTRAINT DF_OperationalCatalog_IsDefault DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_OperationalCatalog_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_OperationalCatalog_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_OperationalCatalog_IsDeleted DEFAULT 0
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_OperationalCatalog_CatalogKey_Code' AND object_id = OBJECT_ID(N'dbo.OperationalCatalog'))
BEGIN
    CREATE UNIQUE INDEX UX_OperationalCatalog_CatalogKey_Code ON dbo.OperationalCatalog(CatalogKey, Code) WHERE IsDeleted = 0;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OperationalCatalog_CatalogKey_Parent' AND object_id = OBJECT_ID(N'dbo.OperationalCatalog'))
BEGIN
    CREATE INDEX IX_OperationalCatalog_CatalogKey_Parent ON dbo.OperationalCatalog(CatalogKey, ParentCatalogKey, ParentCode, DisplayOrder, Name) WHERE IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_OPERATIONALCATALOG_LISTAR
    @CatalogKey nvarchar(80),
    @Search nvarchar(150) = NULL,
    @ParentCatalogKey nvarchar(80) = NULL,
    @ParentCode nvarchar(40) = NULL,
    @IsActive bit = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id, CatalogKey, Code, Name, Description, ParentCatalogKey, ParentCode,
        DisplayOrder, IsDefault, IsActive,
        CreatedByUserId, CreatedByUserName, CreatedAt,
        UpdatedByUserId, UpdatedByUserName, UpdatedAt,
        DeletedByUserId, DeletedByUserName, DeletedAt
    FROM dbo.OperationalCatalog
    WHERE IsDeleted = 0
      AND CatalogKey = @CatalogKey
      AND (@IsActive IS NULL OR IsActive = @IsActive)
      AND (@ParentCatalogKey IS NULL OR ParentCatalogKey = @ParentCatalogKey)
      AND (@ParentCode IS NULL OR ParentCode = @ParentCode)
      AND
      (
          @Search IS NULL
          OR Code LIKE N'%' + @Search + N'%'
          OR Name LIKE N'%' + @Search + N'%'
          OR ISNULL(Description, N'') LIKE N'%' + @Search + N'%'
      )
    ORDER BY DisplayOrder, Name, Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_OPERATIONALCATALOG_BUSCARPORID
    @CatalogKey nvarchar(80),
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id, CatalogKey, Code, Name, Description, ParentCatalogKey, ParentCode,
        DisplayOrder, IsDefault, IsActive,
        CreatedByUserId, CreatedByUserName, CreatedAt,
        UpdatedByUserId, UpdatedByUserName, UpdatedAt,
        DeletedByUserId, DeletedByUserName, DeletedAt
    FROM dbo.OperationalCatalog
    WHERE Id = @Id
      AND CatalogKey = @CatalogKey
      AND IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_OPERATIONALCATALOG_LOOKUP
    @CatalogKey nvarchar(80),
    @ParentCatalogKey nvarchar(80) = NULL,
    @ParentCode nvarchar(40) = NULL,
    @ActiveOnly bit = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id, CatalogKey, Code, Name, Description, ParentCatalogKey, ParentCode,
        DisplayOrder, IsDefault, IsActive
    FROM dbo.OperationalCatalog
    WHERE IsDeleted = 0
      AND CatalogKey = @CatalogKey
      AND (@ActiveOnly = 0 OR IsActive = 1)
      AND (@ParentCatalogKey IS NULL OR ParentCatalogKey = @ParentCatalogKey)
      AND (@ParentCode IS NULL OR ParentCode = @ParentCode)
    ORDER BY DisplayOrder, Name, Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_OPERATIONALCATALOG_BUSCARPORCODIGO
    @CatalogKey nvarchar(80),
    @Code nvarchar(40),
    @ExcludedId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CASE WHEN EXISTS
    (
        SELECT 1
        FROM dbo.OperationalCatalog
        WHERE CatalogKey = @CatalogKey
          AND Code = @Code
          AND IsDeleted = 0
          AND (@ExcludedId IS NULL OR Id <> @ExcludedId)
    ) THEN 1 ELSE 0 END;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_OPERATIONALCATALOG_CREAR
    @CatalogKey nvarchar(80),
    @Code nvarchar(40),
    @Name nvarchar(150),
    @Description nvarchar(500) = NULL,
    @ParentCatalogKey nvarchar(80) = NULL,
    @ParentCode nvarchar(40) = NULL,
    @DisplayOrder int = 0,
    @IsDefault bit = 0,
    @IsActive bit = 1,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @IsDefault = 1
    BEGIN
        UPDATE dbo.OperationalCatalog
        SET IsDefault = 0,
            UpdatedByUserId = @CreatedByUserId,
            UpdatedByUserName = @CreatedByUserName,
            UpdatedAt = SYSUTCDATETIME()
        WHERE CatalogKey = @CatalogKey
          AND IsDeleted = 0;
    END;

    INSERT INTO dbo.OperationalCatalog
    (
        CatalogKey, Code, Name, Description, ParentCatalogKey, ParentCode,
        DisplayOrder, IsDefault, IsActive,
        CreatedByUserId, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        @CatalogKey, @Code, @Name, @Description, @ParentCatalogKey, @ParentCode,
        @DisplayOrder, @IsDefault, @IsActive,
        @CreatedByUserId, @CreatedByUserName, SYSUTCDATETIME()
    );

    SELECT CONVERT(int, SCOPE_IDENTITY());
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_OPERATIONALCATALOG_ACTUALIZAR
    @Id int,
    @CatalogKey nvarchar(80),
    @Code nvarchar(40),
    @Name nvarchar(150),
    @Description nvarchar(500) = NULL,
    @ParentCatalogKey nvarchar(80) = NULL,
    @ParentCode nvarchar(40) = NULL,
    @DisplayOrder int = 0,
    @IsDefault bit = 0,
    @IsActive bit = 1,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @IsDefault = 1
    BEGIN
        UPDATE dbo.OperationalCatalog
        SET IsDefault = 0,
            UpdatedByUserId = @UpdatedByUserId,
            UpdatedByUserName = @UpdatedByUserName,
            UpdatedAt = SYSUTCDATETIME()
        WHERE CatalogKey = @CatalogKey
          AND IsDeleted = 0
          AND Id <> @Id;
    END;

    UPDATE dbo.OperationalCatalog
    SET Code = @Code,
        Name = @Name,
        Description = @Description,
        ParentCatalogKey = @ParentCatalogKey,
        ParentCode = @ParentCode,
        DisplayOrder = @DisplayOrder,
        IsDefault = @IsDefault,
        IsActive = @IsActive,
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id
      AND CatalogKey = @CatalogKey
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_OPERATIONALCATALOG_ELIMINAR
    @CatalogKey nvarchar(80),
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.OperationalCatalog
    SET IsDeleted = 1,
        IsActive = 0,
        DeletedByUserId = @DeletedByUserId,
        DeletedByUserName = @DeletedByUserName,
        DeletedAt = SYSUTCDATETIME()
    WHERE Id = @Id
      AND CatalogKey = @CatalogKey
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

DECLARE @Seed table
(
    CatalogKey nvarchar(80),
    Code nvarchar(40),
    Name nvarchar(150),
    Description nvarchar(500),
    ParentCatalogKey nvarchar(80),
    ParentCode nvarchar(40),
    DisplayOrder int,
    IsDefault bit,
    IsActive bit
);

INSERT INTO @Seed (CatalogKey, Code, Name, Description, ParentCatalogKey, ParentCode, DisplayOrder, IsDefault, IsActive)
VALUES
    (N'DOCUMENT_ESTABLISHMENT', N'001', N'Casa Matriz', N'Establecimiento principal.', NULL, NULL, 10, 1, 1),
    (N'DOCUMENT_EMISSION_POINT', N'001', N'Principal', N'Punto de emision principal.', N'DOCUMENT_ESTABLISHMENT', N'001', 10, 1, 1),
    (N'SAP_OBJECT_TYPE', N'22', N'Orden de Compra', N'Objeto SAP Business One.', NULL, NULL, 10, 1, 1),
    (N'SAP_OBJECT_TYPE', N'13', N'Factura de Venta', N'Objeto SAP Business One.', NULL, NULL, 20, 0, 1),
    (N'SAP_OBJECT_TYPE', N'15', N'Entrega', N'Objeto SAP Business One.', NULL, NULL, 30, 0, 1),
    (N'SAP_OBJECT_TYPE', N'20', N'Entrada de Mercancias', N'Objeto SAP Business One.', NULL, NULL, 40, 0, 1),
    (N'SAP_OBJECT_TYPE', N'67', N'Transferencia de Stock', N'Objeto SAP Business One.', NULL, NULL, 50, 0, 1),
    (N'DOCUMENT_TYPE', N'PURCHASE_ORDER', N'Orden de Compra', N'Tipo de documento operativo.', NULL, NULL, 10, 1, 1),
    (N'DOCUMENT_TYPE', N'SALES_INVOICE', N'Factura de Venta', N'Tipo de documento operativo.', NULL, NULL, 20, 0, 1),
    (N'DOCUMENT_TYPE', N'SALES_RECEIPT', N'Boleta de Venta', N'Tipo de documento operativo.', NULL, NULL, 30, 0, 1),
    (N'DOCUMENT_TYPE', N'DELIVERY_NOTE', N'Guia de Remision', N'Tipo de documento operativo.', NULL, NULL, 40, 0, 1),
    (N'DOCUMENT_TYPE', N'PURCHASE_RECEIPT', N'Ingreso por Compras', N'Tipo de documento operativo.', NULL, NULL, 50, 0, 1),
    (N'DOCUMENT_TYPE', N'WAREHOUSE_TRANSFER', N'Transferencia', N'Tipo de documento operativo.', NULL, NULL, 60, 0, 1),
    (N'DOCUMENT_TYPE', N'DEBIT_NOTE', N'Nota de Debito', N'Tipo de documento operativo.', NULL, NULL, 70, 0, 1),
    (N'DOCUMENT_TYPE', N'CREDIT_NOTE', N'Nota de Credito', N'Tipo de documento operativo.', NULL, NULL, 80, 0, 1);

INSERT INTO dbo.OperationalCatalog
(
    CatalogKey, Code, Name, Description, ParentCatalogKey, ParentCode,
    DisplayOrder, IsDefault, IsActive, CreatedByUserName, CreatedAt
)
SELECT
    seed.CatalogKey, seed.Code, seed.Name, seed.Description, seed.ParentCatalogKey, seed.ParentCode,
    seed.DisplayOrder, seed.IsDefault, seed.IsActive, N'Sistema', SYSUTCDATETIME()
FROM @Seed seed
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.OperationalCatalog existing
    WHERE existing.CatalogKey = seed.CatalogKey
      AND existing.Code = seed.Code
      AND existing.IsDeleted = 0
);
GO
