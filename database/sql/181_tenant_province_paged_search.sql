/* Adds server-side paging and global text search for the Provinces master list. Tenant only; requires 172. */
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.Countries', N'U') IS NULL
    OR OBJECT_ID(N'dbo.Provinces', N'U') IS NULL
    OR OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
    THROW 51181, 'Migration 172 is required before 181.', 1;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PROVINCES_BUSCARPAGINADO
    @Search nvarchar(120) = NULL,
    @PageNumber int = 1,
    @PageSize int = 50
AS
BEGIN
    SET NOCOUNT ON;

    IF @PageNumber < 1
        THROW 51181, 'PageNumber must be greater than zero.', 1;

    IF @PageSize < 1 OR @PageSize > 100
        THROW 51181, 'PageSize must be between 1 and 100.', 1;

    DECLARE @NormalizedSearch nvarchar(120) = NULLIF(LTRIM(RTRIM(@Search)), N'');
    DECLARE @Offset bigint = (CONVERT(bigint, @PageNumber) - 1) * @PageSize;

    SELECT
        province.ProvinceId AS Id,
        province.GlobalId,
        province.CountryId,
        country.GlobalId AS CountryGlobalId,
        country.Code AS CountryCode,
        country.Name AS CountryName,
        province.Code,
        province.Name,
        province.ExternalSystem,
        province.ExternalCode,
        province.IsActive,
        province.CreatedAt,
        province.UpdatedAt
    FROM dbo.Provinces AS province
    INNER JOIN dbo.Countries AS country ON country.CountryId = province.CountryId
    WHERE province.IsDeleted = 0
      AND country.IsDeleted = 0
      AND
      (
          @NormalizedSearch IS NULL
          OR country.Code LIKE N'%' + @NormalizedSearch + N'%'
          OR country.Name LIKE N'%' + @NormalizedSearch + N'%'
          OR province.Code LIKE N'%' + @NormalizedSearch + N'%'
          OR province.Name LIKE N'%' + @NormalizedSearch + N'%'
      )
    ORDER BY country.Name, province.Name, province.Code, province.ProvinceId
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(1)
    FROM dbo.Provinces AS province
    INNER JOIN dbo.Countries AS country ON country.CountryId = province.CountryId
    WHERE province.IsDeleted = 0
      AND country.IsDeleted = 0
      AND
      (
          @NormalizedSearch IS NULL
          OR country.Code LIKE N'%' + @NormalizedSearch + N'%'
          OR country.Name LIKE N'%' + @NormalizedSearch + N'%'
          OR province.Code LIKE N'%' + @NormalizedSearch + N'%'
          OR province.Name LIKE N'%' + @NormalizedSearch + N'%'
      );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SchemaHistory WHERE Version = N'20260807.181')
BEGIN
    INSERT dbo.SchemaHistory(Version, Description)
    VALUES (N'20260807.181', N'Provinces server-side paging and global text search');
END;
GO
