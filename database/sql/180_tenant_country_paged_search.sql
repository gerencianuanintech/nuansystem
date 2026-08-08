/* Adds server-side paging and global text search for the Countries master list. Tenant only; requires 168. */
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.Countries', N'U') IS NULL
    OR OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
    THROW 51180, 'Migration 168 is required before 180.', 1;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_COUNTRIES_BUSCARPAGINADO
    @Search nvarchar(120) = NULL,
    @PageNumber int = 1,
    @PageSize int = 50
AS
BEGIN
    SET NOCOUNT ON;

    IF @PageNumber < 1
        THROW 51180, 'PageNumber must be greater than zero.', 1;

    IF @PageSize < 1 OR @PageSize > 100
        THROW 51180, 'PageSize must be between 1 and 100.', 1;

    DECLARE @NormalizedSearch nvarchar(120) = NULLIF(LTRIM(RTRIM(@Search)), N'');
    DECLARE @Offset bigint = (CONVERT(bigint, @PageNumber) - 1) * @PageSize;

    SELECT
        CountryId AS Id,
        GlobalId,
        Code,
        Name,
        Iso2,
        Iso3,
        PhonePrefix,
        ExternalSystem,
        ExternalCode,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM dbo.Countries
    WHERE IsDeleted = 0
      AND
      (
          @NormalizedSearch IS NULL
          OR Code LIKE N'%' + @NormalizedSearch + N'%'
          OR Name LIKE N'%' + @NormalizedSearch + N'%'
          OR Iso2 LIKE N'%' + @NormalizedSearch + N'%'
          OR Iso3 LIKE N'%' + @NormalizedSearch + N'%'
          OR PhonePrefix LIKE N'%' + @NormalizedSearch + N'%'
      )
    ORDER BY Name, Code, CountryId
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(1)
    FROM dbo.Countries
    WHERE IsDeleted = 0
      AND
      (
          @NormalizedSearch IS NULL
          OR Code LIKE N'%' + @NormalizedSearch + N'%'
          OR Name LIKE N'%' + @NormalizedSearch + N'%'
          OR Iso2 LIKE N'%' + @NormalizedSearch + N'%'
          OR Iso3 LIKE N'%' + @NormalizedSearch + N'%'
          OR PhonePrefix LIKE N'%' + @NormalizedSearch + N'%'
      );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SchemaHistory WHERE Version = N'20260807.180')
BEGIN
    INSERT dbo.SchemaHistory(Version, Description)
    VALUES (N'20260807.180', N'Countries server-side paging and global text search');
END;
GO
