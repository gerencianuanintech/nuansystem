/*
    Provinces como entidad operativa de sincronizacion Maestro-Sucursal.

    CountryId y ProvinceId siguen siendo identidades locales. GlobalId y
    Country.GlobalId forman el contrato estable entre bases.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF OBJECT_ID(N'dbo.Countries', N'U') IS NULL
BEGIN
    THROW 51085, 'No existe dbo.Countries. Ejecute primero 083_tenant_country_master_branch_sync.sql.', 1;
END;
GO

IF OBJECT_ID(N'dbo.Provinces', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Provinces
    (
        ProvinceId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Provinces PRIMARY KEY,
        GlobalId uniqueidentifier NOT NULL CONSTRAINT DF_Provinces_GlobalId DEFAULT NEWID(),
        CountryId int NOT NULL,
        Code nvarchar(20) NOT NULL,
        Name nvarchar(120) NOT NULL,
        IsActive bit NOT NULL CONSTRAINT DF_Provinces_IsActive DEFAULT (1),
        IsDeleted bit NOT NULL CONSTRAINT DF_Provinces_IsDeleted DEFAULT (0),
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_Provinces_CreatedAt DEFAULT SYSUTCDATETIME(),
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(100) NULL,
        UpdatedAt datetime2(0) NULL,
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(100) NULL,
        CONSTRAINT FK_Provinces_Countries FOREIGN KEY (CountryId) REFERENCES dbo.Countries(CountryId)
    );
END;
GO

IF COL_LENGTH(N'dbo.Provinces', N'GlobalId') IS NULL
BEGIN
    ALTER TABLE dbo.Provinces ADD GlobalId uniqueidentifier NULL;
END;
GO

UPDATE dbo.Provinces
SET GlobalId = NEWID()
WHERE GlobalId IS NULL;
GO

IF EXISTS
(
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Provinces')
      AND name = N'GlobalId'
      AND is_nullable = 1
)
BEGIN
    ALTER TABLE dbo.Provinces ALTER COLUMN GlobalId uniqueidentifier NOT NULL;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.default_constraints defaultConstraint
    INNER JOIN sys.columns columnDefinition
        ON columnDefinition.object_id = defaultConstraint.parent_object_id
       AND columnDefinition.column_id = defaultConstraint.parent_column_id
    WHERE defaultConstraint.parent_object_id = OBJECT_ID(N'dbo.Provinces')
      AND columnDefinition.name = N'GlobalId'
)
BEGIN
    ALTER TABLE dbo.Provinces
    ADD CONSTRAINT DF_Provinces_GlobalId DEFAULT NEWID() FOR GlobalId;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Provinces_Country_Code'
      AND object_id = OBJECT_ID(N'dbo.Provinces')
)
BEGIN
    CREATE UNIQUE INDEX UX_Provinces_Country_Code
        ON dbo.Provinces (CountryId, Code)
        WHERE IsDeleted = 0;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Provinces_GlobalId'
      AND object_id = OBJECT_ID(N'dbo.Provinces')
)
BEGIN
    CREATE UNIQUE INDEX UX_Provinces_GlobalId ON dbo.Provinces (GlobalId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PROVINCES_LISTAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        province.ProvinceId AS Id,
        province.GlobalId,
        province.CountryId,
        country.GlobalId AS CountryGlobalId,
        country.Code AS CountryCode,
        country.Name AS CountryName,
        province.Code,
        province.Name,
        province.IsActive,
        province.CreatedAt,
        province.UpdatedAt
    FROM dbo.Provinces AS province
    INNER JOIN dbo.Countries AS country ON country.CountryId = province.CountryId
    WHERE province.IsDeleted = 0
      AND country.IsDeleted = 0
    ORDER BY country.Name, province.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PROVINCES_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        province.ProvinceId AS Id,
        province.GlobalId,
        province.CountryId,
        country.GlobalId AS CountryGlobalId,
        country.Code AS CountryCode,
        country.Name AS CountryName,
        province.Code,
        province.Name,
        province.IsActive,
        province.CreatedAt,
        province.UpdatedAt
    FROM dbo.Provinces AS province
    INNER JOIN dbo.Countries AS country ON country.CountryId = province.CountryId
    WHERE province.ProvinceId = @Id
      AND province.IsDeleted = 0
      AND country.IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PROVINCES_BUSCARPORCODIGO
    @CountryId int,
    @Code nvarchar(20),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM dbo.Provinces
    WHERE CountryId = @CountryId
      AND Code = @Code
      AND IsDeleted = 0
      AND (@ExcluirId IS NULL OR ProvinceId <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PROVINCES_LOOKUP
    @CountryCode nvarchar(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT province.ProvinceId AS Id, province.Code, province.Name, province.IsActive
    FROM dbo.Provinces AS province
    INNER JOIN dbo.Countries AS country ON country.CountryId = province.CountryId
    WHERE province.IsDeleted = 0
      AND province.IsActive = 1
      AND country.IsDeleted = 0
      AND (@CountryCode IS NULL OR country.Code = @CountryCode OR country.Iso2 = @CountryCode OR country.Iso3 = @CountryCode)
    ORDER BY province.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_PROVINCES_CREAR
    @Id int = NULL,
    @GlobalId uniqueidentifier,
    @CountryId int,
    @Code nvarchar(20),
    @Name nvarchar(120),
    @IsActive bit = 1,
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Provinces
    (
        GlobalId, CountryId, Code, Name, IsActive,
        CreatedByUserId, CreatedByUserName
    )
    VALUES
    (
        @GlobalId, @CountryId, @Code, @Name, @IsActive,
        @AuditUserId, @AuditUserName
    );

    SELECT CONVERT(int, SCOPE_IDENTITY());
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_PROVINCES_ACTUALIZAR
    @Id int,
    @GlobalId uniqueidentifier,
    @CountryId int,
    @Code nvarchar(20),
    @Name nvarchar(120),
    @IsActive bit = 1,
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Provinces
    SET GlobalId = @GlobalId,
        CountryId = @CountryId,
        Code = @Code,
        Name = @Name,
        IsActive = @IsActive,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @AuditUserId,
        UpdatedByUserName = @AuditUserName
    WHERE ProvinceId = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_PROVINCES_ELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Provinces
    SET IsDeleted = 1,
        IsActive = 0,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @DeletedByUserId,
        UpdatedByUserName = @DeletedByUserName
    WHERE ProvinceId = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

IF OBJECT_ID(N'dbo.SchemaVersions', N'U') IS NOT NULL
    AND NOT EXISTS (SELECT 1 FROM dbo.SchemaVersions WHERE Version = N'20260716.085')
BEGIN
    INSERT INTO dbo.SchemaVersions (Version, Description)
    VALUES (N'20260716.085', N'Provinces operativo para sincronizacion Maestro-Sucursal');
END;
GO
