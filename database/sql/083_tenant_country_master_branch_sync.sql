/*
    Countries como entidad operativa de sincronizacion Maestro-Sucursal.

    Ejecutar en cada base tenant que pueda actuar como maestra o sucursal.
    El script conserva CountryId como identidad local y agrega GlobalId como
    identidad estable de replicacion.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF OBJECT_ID(N'dbo.Countries', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Countries
    (
        CountryId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Countries PRIMARY KEY,
        GlobalId uniqueidentifier NOT NULL CONSTRAINT DF_Countries_GlobalId DEFAULT NEWID(),
        Code nvarchar(10) NOT NULL,
        Name nvarchar(120) NOT NULL,
        Iso2 nvarchar(2) NULL,
        Iso3 nvarchar(3) NULL,
        PhonePrefix nvarchar(10) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_Countries_IsActive DEFAULT (1),
        IsDeleted bit NOT NULL CONSTRAINT DF_Countries_IsDeleted DEFAULT (0),
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_Countries_CreatedAt DEFAULT SYSUTCDATETIME(),
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(100) NULL,
        UpdatedAt datetime2(0) NULL,
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(100) NULL
    );
END;
GO

IF COL_LENGTH(N'dbo.Countries', N'GlobalId') IS NULL
BEGIN
    ALTER TABLE dbo.Countries ADD GlobalId uniqueidentifier NULL;
END;
GO

UPDATE dbo.Countries
SET GlobalId = NEWID()
WHERE GlobalId IS NULL;
GO

IF EXISTS
(
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Countries')
      AND name = N'GlobalId'
      AND is_nullable = 1
)
BEGIN
    ALTER TABLE dbo.Countries ALTER COLUMN GlobalId uniqueidentifier NOT NULL;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.default_constraints defaultConstraint
    INNER JOIN sys.columns columnDefinition
        ON columnDefinition.object_id = defaultConstraint.parent_object_id
       AND columnDefinition.column_id = defaultConstraint.parent_column_id
    WHERE defaultConstraint.parent_object_id = OBJECT_ID(N'dbo.Countries')
      AND columnDefinition.name = N'GlobalId'
)
BEGIN
    ALTER TABLE dbo.Countries
    ADD CONSTRAINT DF_Countries_GlobalId DEFAULT NEWID() FOR GlobalId;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Countries_Code'
      AND object_id = OBJECT_ID(N'dbo.Countries')
)
BEGIN
    CREATE UNIQUE INDEX UX_Countries_Code ON dbo.Countries (Code) WHERE IsDeleted = 0;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Countries_GlobalId'
      AND object_id = OBJECT_ID(N'dbo.Countries')
)
BEGIN
    CREATE UNIQUE INDEX UX_Countries_GlobalId ON dbo.Countries (GlobalId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_COUNTRIES_LISTAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CountryId AS Id,
        GlobalId,
        Code,
        Name,
        Iso2,
        Iso3,
        PhonePrefix,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM dbo.Countries
    WHERE IsDeleted = 0
    ORDER BY Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_COUNTRIES_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CountryId AS Id,
        GlobalId,
        Code,
        Name,
        Iso2,
        Iso3,
        PhonePrefix,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM dbo.Countries
    WHERE CountryId = @Id
      AND IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_COUNTRIES_BUSCARPORCODIGO
    @Code nvarchar(10),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM dbo.Countries
    WHERE Code = @Code
      AND IsDeleted = 0
      AND (@ExcluirId IS NULL OR CountryId <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_COUNTRIES_LOOKUP
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CountryId AS Id, Code, Name, IsActive
    FROM dbo.Countries
    WHERE IsDeleted = 0
      AND IsActive = 1
    ORDER BY Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_COUNTRIES_CREAR
    @Id int = NULL,
    @GlobalId uniqueidentifier,
    @Code nvarchar(10),
    @Name nvarchar(120),
    @Iso2 nvarchar(2) = NULL,
    @Iso3 nvarchar(3) = NULL,
    @PhonePrefix nvarchar(10) = NULL,
    @IsActive bit = 1,
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Countries
    (
        GlobalId, Code, Name, Iso2, Iso3, PhonePrefix, IsActive,
        CreatedByUserId, CreatedByUserName
    )
    VALUES
    (
        @GlobalId, @Code, @Name, @Iso2, @Iso3, @PhonePrefix, @IsActive,
        @AuditUserId, @AuditUserName
    );

    SELECT CONVERT(int, SCOPE_IDENTITY());
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_COUNTRIES_ACTUALIZAR
    @Id int,
    @GlobalId uniqueidentifier,
    @Code nvarchar(10),
    @Name nvarchar(120),
    @Iso2 nvarchar(2) = NULL,
    @Iso3 nvarchar(3) = NULL,
    @PhonePrefix nvarchar(10) = NULL,
    @IsActive bit = 1,
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Countries
    SET GlobalId = @GlobalId,
        Code = @Code,
        Name = @Name,
        Iso2 = @Iso2,
        Iso3 = @Iso3,
        PhonePrefix = @PhonePrefix,
        IsActive = @IsActive,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @AuditUserId,
        UpdatedByUserName = @AuditUserName
    WHERE CountryId = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_COUNTRIES_ELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Countries
    SET IsDeleted = 1,
        IsActive = 0,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @DeletedByUserId,
        UpdatedByUserName = @DeletedByUserName
    WHERE CountryId = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

IF OBJECT_ID(N'dbo.SchemaVersions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SchemaVersions
    (
        Version nvarchar(50) NOT NULL CONSTRAINT PK_SchemaVersions PRIMARY KEY,
        Description nvarchar(250) NOT NULL,
        AppliedAt datetime2(0) NOT NULL CONSTRAINT DF_SchemaVersions_AppliedAt DEFAULT SYSUTCDATETIME()
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SchemaVersions WHERE Version = N'20260716.083')
BEGIN
    INSERT INTO dbo.SchemaVersions (Version, Description)
    VALUES (N'20260716.083', N'Countries operativo para sincronizacion Maestro-Sucursal');
END;
GO
