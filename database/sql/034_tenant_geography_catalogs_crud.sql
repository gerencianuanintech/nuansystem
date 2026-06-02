/*
    Ejecutar este script en base tenant.
    Agrega procedimientos CRUD para catalogos geograficos.
*/

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_COUNTRIES_LISTAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CountryId AS Id,
        Code,
        Name,
        Iso2,
        Iso3,
        PhonePrefix,
        IsActive
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
        Code,
        Name,
        Iso2,
        Iso3,
        PhonePrefix,
        IsActive
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

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_COUNTRIES_CREAR
    @Code nvarchar(10),
    @Name nvarchar(120),
    @Iso2 nvarchar(2) = NULL,
    @Iso3 nvarchar(3) = NULL,
    @PhonePrefix nvarchar(10) = NULL,
    @IsActive bit = 1,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Countries
    (
        Code,
        Name,
        Iso2,
        Iso3,
        PhonePrefix,
        IsActive,
        CreatedByUserId,
        CreatedByUserName
    )
    VALUES
    (
        @Code,
        @Name,
        @Iso2,
        @Iso3,
        @PhonePrefix,
        @IsActive,
        @CreatedByUserId,
        @CreatedByUserName
    );

    SELECT CONVERT(int, SCOPE_IDENTITY());
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_COUNTRIES_ACTUALIZAR
    @Id int,
    @Code nvarchar(10),
    @Name nvarchar(120),
    @Iso2 nvarchar(2) = NULL,
    @Iso3 nvarchar(3) = NULL,
    @PhonePrefix nvarchar(10) = NULL,
    @IsActive bit = 1,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Countries
    SET Code = @Code,
        Name = @Name,
        Iso2 = @Iso2,
        Iso3 = @Iso3,
        PhonePrefix = @PhonePrefix,
        IsActive = @IsActive,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName
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
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @DeletedByUserId,
        UpdatedByUserName = @DeletedByUserName
    WHERE CountryId = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PROVINCES_LISTAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.ProvinceId AS Id,
        p.CountryId,
        c.Code AS CountryCode,
        c.Name AS CountryName,
        p.Code,
        p.Name,
        p.IsActive
    FROM dbo.Provinces p
    INNER JOIN dbo.Countries c ON c.CountryId = p.CountryId
    WHERE p.IsDeleted = 0
      AND c.IsDeleted = 0
    ORDER BY c.Name, p.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PROVINCES_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.ProvinceId AS Id,
        p.CountryId,
        c.Code AS CountryCode,
        c.Name AS CountryName,
        p.Code,
        p.Name,
        p.IsActive
    FROM dbo.Provinces p
    INNER JOIN dbo.Countries c ON c.CountryId = p.CountryId
    WHERE p.ProvinceId = @Id
      AND p.IsDeleted = 0
      AND c.IsDeleted = 0;
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

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_PROVINCES_CREAR
    @CountryId int,
    @Code nvarchar(20),
    @Name nvarchar(120),
    @IsActive bit = 1,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Provinces
    (
        CountryId,
        Code,
        Name,
        IsActive,
        CreatedByUserId,
        CreatedByUserName
    )
    VALUES
    (
        @CountryId,
        @Code,
        @Name,
        @IsActive,
        @CreatedByUserId,
        @CreatedByUserName
    );

    SELECT CONVERT(int, SCOPE_IDENTITY());
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_PROVINCES_ACTUALIZAR
    @Id int,
    @CountryId int,
    @Code nvarchar(20),
    @Name nvarchar(120),
    @IsActive bit = 1,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Provinces
    SET CountryId = @CountryId,
        Code = @Code,
        Name = @Name,
        IsActive = @IsActive,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName
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
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @DeletedByUserId,
        UpdatedByUserName = @DeletedByUserName
    WHERE ProvinceId = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CITIES_LISTAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ci.CityId AS Id,
        ci.CountryId,
        c.Code AS CountryCode,
        c.Name AS CountryName,
        ci.ProvinceId,
        p.Code AS ProvinceCode,
        p.Name AS ProvinceName,
        ci.Code,
        ci.Name,
        ci.IsActive
    FROM dbo.Cities ci
    INNER JOIN dbo.Countries c ON c.CountryId = ci.CountryId
    INNER JOIN dbo.Provinces p ON p.ProvinceId = ci.ProvinceId
    WHERE ci.IsDeleted = 0
      AND c.IsDeleted = 0
      AND p.IsDeleted = 0
    ORDER BY c.Name, p.Name, ci.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CITIES_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ci.CityId AS Id,
        ci.CountryId,
        c.Code AS CountryCode,
        c.Name AS CountryName,
        ci.ProvinceId,
        p.Code AS ProvinceCode,
        p.Name AS ProvinceName,
        ci.Code,
        ci.Name,
        ci.IsActive
    FROM dbo.Cities ci
    INNER JOIN dbo.Countries c ON c.CountryId = ci.CountryId
    INNER JOIN dbo.Provinces p ON p.ProvinceId = ci.ProvinceId
    WHERE ci.CityId = @Id
      AND ci.IsDeleted = 0
      AND c.IsDeleted = 0
      AND p.IsDeleted = 0;
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

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_CITIES_CREAR
    @CountryId int,
    @ProvinceId int,
    @Code nvarchar(20),
    @Name nvarchar(120),
    @IsActive bit = 1,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Cities
    (
        CountryId,
        ProvinceId,
        Code,
        Name,
        IsActive,
        CreatedByUserId,
        CreatedByUserName
    )
    VALUES
    (
        @CountryId,
        @ProvinceId,
        @Code,
        @Name,
        @IsActive,
        @CreatedByUserId,
        @CreatedByUserName
    );

    SELECT CONVERT(int, SCOPE_IDENTITY());
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_CITIES_ACTUALIZAR
    @Id int,
    @CountryId int,
    @ProvinceId int,
    @Code nvarchar(20),
    @Name nvarchar(120),
    @IsActive bit = 1,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Cities
    SET CountryId = @CountryId,
        ProvinceId = @ProvinceId,
        Code = @Code,
        Name = @Name,
        IsActive = @IsActive,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName
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
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @DeletedByUserId,
        UpdatedByUserName = @DeletedByUserName
    WHERE CityId = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO
