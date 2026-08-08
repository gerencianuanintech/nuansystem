/* Adds server-side paging and global text search for the Cities master list. Tenant only; requires 175. */
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.Countries', N'U') IS NULL
    OR OBJECT_ID(N'dbo.Provinces', N'U') IS NULL
    OR OBJECT_ID(N'dbo.Cities', N'U') IS NULL
    OR OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
    THROW 51182, 'Migration 175 is required before 182.', 1;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CITIES_BUSCARPAGINADO
    @Search nvarchar(120) = NULL,
    @PageNumber int = 1,
    @PageSize int = 50
AS
BEGIN
    SET NOCOUNT ON;

    IF @PageNumber < 1
        THROW 51182, 'PageNumber must be greater than zero.', 1;

    IF @PageSize < 1 OR @PageSize > 100
        THROW 51182, 'PageSize must be between 1 and 100.', 1;

    DECLARE @NormalizedSearch nvarchar(120) = NULLIF(LTRIM(RTRIM(@Search)), N'');
    DECLARE @Offset bigint = (CONVERT(bigint, @PageNumber) - 1) * @PageSize;

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
        city.ExternalSystem,
        city.ExternalCode,
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
      AND
      (
          @NormalizedSearch IS NULL
          OR country.Code LIKE N'%' + @NormalizedSearch + N'%'
          OR country.Name LIKE N'%' + @NormalizedSearch + N'%'
          OR province.Code LIKE N'%' + @NormalizedSearch + N'%'
          OR province.Name LIKE N'%' + @NormalizedSearch + N'%'
          OR city.Code LIKE N'%' + @NormalizedSearch + N'%'
          OR city.Name LIKE N'%' + @NormalizedSearch + N'%'
      )
    ORDER BY country.Name, province.Name, city.Name, city.Code, city.CityId
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(1)
    FROM dbo.Cities AS city
    INNER JOIN dbo.Countries AS country ON country.CountryId = city.CountryId
    INNER JOIN dbo.Provinces AS province ON province.ProvinceId = city.ProvinceId
    WHERE city.IsDeleted = 0
      AND country.IsDeleted = 0
      AND province.IsDeleted = 0
      AND province.CountryId = country.CountryId
      AND
      (
          @NormalizedSearch IS NULL
          OR country.Code LIKE N'%' + @NormalizedSearch + N'%'
          OR country.Name LIKE N'%' + @NormalizedSearch + N'%'
          OR province.Code LIKE N'%' + @NormalizedSearch + N'%'
          OR province.Name LIKE N'%' + @NormalizedSearch + N'%'
          OR city.Code LIKE N'%' + @NormalizedSearch + N'%'
          OR city.Name LIKE N'%' + @NormalizedSearch + N'%'
      );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SchemaHistory WHERE Version = N'20260807.182')
BEGIN
    INSERT dbo.SchemaHistory(Version, Description)
    VALUES (N'20260807.182', N'Cities server-side paging and global text search');
END;
GO
