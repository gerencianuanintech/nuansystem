/*
    Currencies como entidad operativa de sincronizacion Maestro-Sucursal.

    CurrencyId sigue siendo identidad local. GlobalId es la identidad estable
    entre bases y Code permite adoptar registros historicos.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF OBJECT_ID(N'dbo.Currencies', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Currencies
    (
        CurrencyId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Currencies PRIMARY KEY,
        GlobalId uniqueidentifier NOT NULL CONSTRAINT DF_Currencies_GlobalId DEFAULT NEWID(),
        Code nvarchar(3) NOT NULL,
        Name nvarchar(120) NOT NULL,
        Symbol nvarchar(10) NULL,
        Description nvarchar(300) NULL,
        IsBaseCurrency bit NOT NULL CONSTRAINT DF_Currencies_IsBaseCurrency DEFAULT (0),
        IsActive bit NOT NULL CONSTRAINT DF_Currencies_IsActive DEFAULT (1),
        IsDeleted bit NOT NULL CONSTRAINT DF_Currencies_IsDeleted DEFAULT (0),
        ExternalSystem nvarchar(50) NULL,
        ExternalCode nvarchar(100) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_Currencies_CreatedAt DEFAULT SYSUTCDATETIME(),
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(100) NULL,
        UpdatedAt datetime2(0) NULL,
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(100) NULL,
        DeletedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(100) NULL
    );
END;
GO

IF COL_LENGTH(N'dbo.Currencies', N'GlobalId') IS NULL
BEGIN
    ALTER TABLE dbo.Currencies ADD GlobalId uniqueidentifier NULL;
END;
GO

IF COL_LENGTH(N'dbo.Currencies', N'ExternalSystem') IS NULL
BEGIN
    ALTER TABLE dbo.Currencies ADD ExternalSystem nvarchar(50) NULL;
END;
GO

IF COL_LENGTH(N'dbo.Currencies', N'ExternalCode') IS NULL
BEGIN
    ALTER TABLE dbo.Currencies ADD ExternalCode nvarchar(100) NULL;
END;
GO

UPDATE dbo.Currencies
SET GlobalId = NEWID()
WHERE GlobalId IS NULL;
GO

IF EXISTS
(
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Currencies')
      AND name = N'GlobalId'
      AND is_nullable = 1
)
BEGIN
    ALTER TABLE dbo.Currencies ALTER COLUMN GlobalId uniqueidentifier NOT NULL;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.default_constraints defaultConstraint
    INNER JOIN sys.columns columnDefinition
        ON columnDefinition.object_id = defaultConstraint.parent_object_id
       AND columnDefinition.column_id = defaultConstraint.parent_column_id
    WHERE defaultConstraint.parent_object_id = OBJECT_ID(N'dbo.Currencies')
      AND columnDefinition.name = N'GlobalId'
)
BEGIN
    ALTER TABLE dbo.Currencies
    ADD CONSTRAINT DF_Currencies_GlobalId DEFAULT NEWID() FOR GlobalId;
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'UX_Currencies_Code'
      AND object_id = OBJECT_ID(N'dbo.Currencies')
)
BEGIN
    CREATE UNIQUE INDEX UX_Currencies_Code
        ON dbo.Currencies (Code)
        WHERE IsDeleted = 0;
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'UX_Currencies_GlobalId'
      AND object_id = OBJECT_ID(N'dbo.Currencies')
)
BEGIN
    CREATE UNIQUE INDEX UX_Currencies_GlobalId ON dbo.Currencies (GlobalId);
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Currencies_ExternalRef'
      AND object_id = OBJECT_ID(N'dbo.Currencies')
)
BEGIN
    CREATE INDEX IX_Currencies_ExternalRef
        ON dbo.Currencies (ExternalSystem, ExternalCode)
        WHERE ExternalSystem IS NOT NULL AND ExternalCode IS NOT NULL;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Currencies WHERE Code = N'USD' AND IsDeleted = 0)
BEGIN
    INSERT INTO dbo.Currencies
    (
        Code, Name, Symbol, Description, IsBaseCurrency, CreatedByUserName
    )
    VALUES
    (
        N'USD', N'USD - Dolar Americano', N'$', N'Moneda base por defecto.', 1, N'Sistema'
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Currencies WHERE Code = N'EUR' AND IsDeleted = 0)
BEGIN
    INSERT INTO dbo.Currencies
    (
        Code, Name, Symbol, Description, IsBaseCurrency, CreatedByUserName
    )
    VALUES
    (
        N'EUR', N'EUR - Euro', N'EUR', N'Moneda extranjera.', 0, N'Sistema'
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CURRENCIES_LISTAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CurrencyId AS Id, GlobalId, Code, Name, Symbol, Description,
           IsBaseCurrency, IsActive, ExternalSystem, ExternalCode,
           CreatedAt, CreatedByUserId, CreatedByUserName,
           UpdatedAt, UpdatedByUserId, UpdatedByUserName
    FROM dbo.Currencies
    WHERE IsDeleted = 0
    ORDER BY Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CURRENCIES_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CurrencyId AS Id, GlobalId, Code, Name, Symbol, Description,
           IsBaseCurrency, IsActive, ExternalSystem, ExternalCode,
           CreatedAt, CreatedByUserId, CreatedByUserName,
           UpdatedAt, UpdatedByUserId, UpdatedByUserName
    FROM dbo.Currencies
    WHERE CurrencyId = @Id
      AND IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CURRENCIES_LOOKUP
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CurrencyId AS Id, Code, Name, IsActive
    FROM dbo.Currencies
    WHERE IsDeleted = 0
      AND IsActive = 1
    ORDER BY Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CURRENCIES_BUSCARPORCODIGO
    @Code nvarchar(3),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM dbo.Currencies
    WHERE IsDeleted = 0
      AND Code = @Code
      AND (@ExcluirId IS NULL OR CurrencyId <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_CURRENCIES_CREAR
    @Code nvarchar(3),
    @Name nvarchar(120),
    @Description nvarchar(300) = NULL,
    @IsActive bit = 1,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Currencies
    (
        Code, Name, Description, IsActive, CreatedByUserId, CreatedByUserName
    )
    VALUES
    (
        @Code, @Name, @Description, @IsActive, @CreatedByUserId, @CreatedByUserName
    );

    SELECT CONVERT(int, SCOPE_IDENTITY());
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_CURRENCIES_ACTUALIZAR
    @Id int,
    @Code nvarchar(3),
    @Name nvarchar(120),
    @Description nvarchar(300) = NULL,
    @IsActive bit = 1,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Currencies
    SET Code = @Code,
        Name = @Name,
        Description = @Description,
        IsActive = @IsActive,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName
    WHERE CurrencyId = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_CURRENCIES_ELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Currencies
    SET IsDeleted = 1,
        IsActive = 0,
        DeletedAt = SYSUTCDATETIME(),
        DeletedByUserId = @DeletedByUserId,
        DeletedByUserName = @DeletedByUserName
    WHERE CurrencyId = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

IF OBJECT_ID(N'dbo.SchemaVersions', N'U') IS NOT NULL
    AND NOT EXISTS (SELECT 1 FROM dbo.SchemaVersions WHERE Version = N'20260716.090')
BEGIN
    INSERT INTO dbo.SchemaVersions (Version, Description)
    VALUES (N'20260716.090', N'Currencies operativo para sincronizacion Maestro-Sucursal');
END;
GO
