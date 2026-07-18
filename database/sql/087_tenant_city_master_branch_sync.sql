/*
    Cities como entidad operativa de sincronizacion Maestro-Sucursal.

    Las identidades locales se conservan. El contrato entre bases utiliza
    City.GlobalId, Province.GlobalId y Country.GlobalId.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF OBJECT_ID(N'dbo.Countries', N'U') IS NULL OR OBJECT_ID(N'dbo.Provinces', N'U') IS NULL
BEGIN
    THROW 51087, 'No existen Countries/Provinces. Ejecute primero los scripts 083 y 085.', 1;
END;
GO

IF OBJECT_ID(N'dbo.Cities', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Cities
    (
        CityId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Cities PRIMARY KEY,
        GlobalId uniqueidentifier NOT NULL CONSTRAINT DF_Cities_GlobalId DEFAULT NEWID(),
        CountryId int NOT NULL,
        ProvinceId int NOT NULL,
        Code nvarchar(20) NOT NULL,
        Name nvarchar(120) NOT NULL,
        IsActive bit NOT NULL CONSTRAINT DF_Cities_IsActive DEFAULT (1),
        IsDeleted bit NOT NULL CONSTRAINT DF_Cities_IsDeleted DEFAULT (0),
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_Cities_CreatedAt DEFAULT SYSUTCDATETIME(),
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(100) NULL,
        UpdatedAt datetime2(0) NULL,
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(100) NULL,
        CONSTRAINT FK_Cities_Countries FOREIGN KEY (CountryId) REFERENCES dbo.Countries(CountryId),
        CONSTRAINT FK_Cities_Provinces FOREIGN KEY (ProvinceId) REFERENCES dbo.Provinces(ProvinceId)
    );
END;
GO

IF COL_LENGTH(N'dbo.Cities', N'GlobalId') IS NULL
BEGIN
    ALTER TABLE dbo.Cities ADD GlobalId uniqueidentifier NULL;
END;
GO

UPDATE dbo.Cities
SET GlobalId = NEWID()
WHERE GlobalId IS NULL;
GO

IF EXISTS
(
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Cities')
      AND name = N'GlobalId'
      AND is_nullable = 1
)
BEGIN
    ALTER TABLE dbo.Cities ALTER COLUMN GlobalId uniqueidentifier NOT NULL;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.default_constraints defaultConstraint
    INNER JOIN sys.columns columnDefinition
        ON columnDefinition.object_id = defaultConstraint.parent_object_id
       AND columnDefinition.column_id = defaultConstraint.parent_column_id
    WHERE defaultConstraint.parent_object_id = OBJECT_ID(N'dbo.Cities')
      AND columnDefinition.name = N'GlobalId'
)
BEGIN
    ALTER TABLE dbo.Cities
    ADD CONSTRAINT DF_Cities_GlobalId DEFAULT NEWID() FOR GlobalId;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Cities_Province_Code'
      AND object_id = OBJECT_ID(N'dbo.Cities')
)
BEGIN
    CREATE UNIQUE INDEX UX_Cities_Province_Code
        ON dbo.Cities (ProvinceId, Code)
        WHERE IsDeleted = 0;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Cities_GlobalId'
      AND object_id = OBJECT_ID(N'dbo.Cities')
)
BEGIN
    CREATE UNIQUE INDEX UX_Cities_GlobalId ON dbo.Cities (GlobalId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CITIES_LISTAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        city.CityId AS Id,
        city.GlobalId,
        city.CountryId,
        country.GlobalId AS CountryGlobalId,
        country.Code AS CountryCode,
        country.Name AS CountryName,
        city.ProvinceId,
        province.GlobalId AS ProvinceGlobalId,
        province.Code AS ProvinceCode,
        province.Name AS ProvinceName,
        city.Code,
        city.Name,
        city.IsActive,
        city.CreatedAt,
        city.UpdatedAt
    FROM dbo.Cities AS city
    INNER JOIN dbo.Countries AS country ON country.CountryId = city.CountryId
    INNER JOIN dbo.Provinces AS province ON province.ProvinceId = city.ProvinceId
    WHERE city.IsDeleted = 0
      AND country.IsDeleted = 0
      AND province.IsDeleted = 0
      AND province.CountryId = country.CountryId
    ORDER BY country.Name, province.Name, city.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CITIES_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        city.CityId AS Id,
        city.GlobalId,
        city.CountryId,
        country.GlobalId AS CountryGlobalId,
        country.Code AS CountryCode,
        country.Name AS CountryName,
        city.ProvinceId,
        province.GlobalId AS ProvinceGlobalId,
        province.Code AS ProvinceCode,
        province.Name AS ProvinceName,
        city.Code,
        city.Name,
        city.IsActive,
        city.CreatedAt,
        city.UpdatedAt
    FROM dbo.Cities AS city
    INNER JOIN dbo.Countries AS country ON country.CountryId = city.CountryId
    INNER JOIN dbo.Provinces AS province ON province.ProvinceId = city.ProvinceId
    WHERE city.CityId = @Id
      AND city.IsDeleted = 0
      AND country.IsDeleted = 0
      AND province.IsDeleted = 0
      AND province.CountryId = country.CountryId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CITIES_BUSCARPORCODIGO
    @ProvinceId int,
    @Code nvarchar(20),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM dbo.Cities
    WHERE ProvinceId = @ProvinceId
      AND Code = @Code
      AND IsDeleted = 0
      AND (@ExcluirId IS NULL OR CityId <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CITIES_LOOKUP
    @CountryCode nvarchar(10) = NULL,
    @ProvinceCode nvarchar(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT city.CityId AS Id, city.Code, city.Name, city.IsActive
    FROM dbo.Cities AS city
    INNER JOIN dbo.Countries AS country ON country.CountryId = city.CountryId
    INNER JOIN dbo.Provinces AS province ON province.ProvinceId = city.ProvinceId
    WHERE city.IsDeleted = 0
      AND city.IsActive = 1
      AND country.IsDeleted = 0
      AND province.IsDeleted = 0
      AND province.CountryId = country.CountryId
      AND (@CountryCode IS NULL OR country.Code = @CountryCode OR country.Iso2 = @CountryCode OR country.Iso3 = @CountryCode)
      AND (@ProvinceCode IS NULL OR province.Code = @ProvinceCode)
    ORDER BY city.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_CITIES_CREAR
    @Id int = NULL,
    @GlobalId uniqueidentifier,
    @CountryId int,
    @ProvinceId int,
    @Code nvarchar(20),
    @Name nvarchar(120),
    @IsActive bit = 1,
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Provinces
        WHERE ProvinceId = @ProvinceId
          AND CountryId = @CountryId
          AND IsDeleted = 0
    )
    BEGIN
        THROW 51090, 'La provincia no pertenece al pais indicado.', 1;
    END;

    INSERT INTO dbo.Cities
    (
        GlobalId, CountryId, ProvinceId, Code, Name, IsActive,
        CreatedByUserId, CreatedByUserName
    )
    VALUES
    (
        @GlobalId, @CountryId, @ProvinceId, @Code, @Name, @IsActive,
        @AuditUserId, @AuditUserName
    );

    SELECT CONVERT(int, SCOPE_IDENTITY());
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_CITIES_ACTUALIZAR
    @Id int,
    @GlobalId uniqueidentifier,
    @CountryId int,
    @ProvinceId int,
    @Code nvarchar(20),
    @Name nvarchar(120),
    @IsActive bit = 1,
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Provinces
        WHERE ProvinceId = @ProvinceId
          AND CountryId = @CountryId
          AND IsDeleted = 0
    )
    BEGIN
        THROW 51090, 'La provincia no pertenece al pais indicado.', 1;
    END;

    UPDATE dbo.Cities
    SET GlobalId = @GlobalId,
        CountryId = @CountryId,
        ProvinceId = @ProvinceId,
        Code = @Code,
        Name = @Name,
        IsActive = @IsActive,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @AuditUserId,
        UpdatedByUserName = @AuditUserName
    WHERE CityId = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_CITIES_ELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Cities
    SET IsDeleted = 1,
        IsActive = 0,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @DeletedByUserId,
        UpdatedByUserName = @DeletedByUserName
    WHERE CityId = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

IF OBJECT_ID(N'dbo.SchemaVersions', N'U') IS NOT NULL
    AND NOT EXISTS (SELECT 1 FROM dbo.SchemaVersions WHERE Version = N'20260716.087')
BEGIN
    INSERT INTO dbo.SchemaVersions (Version, Description)
    VALUES (N'20260716.087', N'Cities operativo para sincronizacion Maestro-Sucursal');
END;
GO
