/*
    Relacion geografica opcional y local para Bodegas.
    Conserva City/Province/Country para datos historicos y valores SAP no homologados.
    No modifica el payload corporativo Matriz-Sucursal.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.Warehouses', N'U') IS NULL
   OR OBJECT_ID(N'dbo.Countries', N'U') IS NULL
   OR OBJECT_ID(N'dbo.Provinces', N'U') IS NULL
   OR OBJECT_ID(N'dbo.Cities', N'U') IS NULL
    THROW 51183, 'Warehouses, Countries, Provinces and Cities are required before migration 183.', 1;
GO

IF COL_LENGTH(N'dbo.Warehouses', N'CountryId') IS NULL
    ALTER TABLE dbo.Warehouses ADD CountryId int NULL;
IF COL_LENGTH(N'dbo.Warehouses', N'ProvinceId') IS NULL
    ALTER TABLE dbo.Warehouses ADD ProvinceId int NULL;
IF COL_LENGTH(N'dbo.Warehouses', N'CityId') IS NULL
    ALTER TABLE dbo.Warehouses ADD CityId int NULL;
GO

UPDATE warehouse
SET CountryId = candidate.Id
FROM dbo.Warehouses warehouse
OUTER APPLY
(
    SELECT MIN(country.Id) AS Id
    FROM dbo.Countries country
    WHERE country.IsDeleted = 0
      AND (UPPER(LTRIM(RTRIM(country.Code))) = UPPER(LTRIM(RTRIM(warehouse.Country)))
           OR UPPER(LTRIM(RTRIM(country.Name))) = UPPER(LTRIM(RTRIM(warehouse.Country))))
    HAVING COUNT(*) = 1
) candidate
WHERE warehouse.CountryId IS NULL
  AND NULLIF(LTRIM(RTRIM(warehouse.Country)), N'') IS NOT NULL
  AND candidate.Id IS NOT NULL;
GO

UPDATE warehouse
SET ProvinceId = candidate.Id
FROM dbo.Warehouses warehouse
OUTER APPLY
(
    SELECT MIN(province.Id) AS Id
    FROM dbo.Provinces province
    WHERE province.IsDeleted = 0
      AND province.CountryId = warehouse.CountryId
      AND (UPPER(LTRIM(RTRIM(province.Code))) = UPPER(LTRIM(RTRIM(warehouse.Province)))
           OR UPPER(LTRIM(RTRIM(province.Name))) = UPPER(LTRIM(RTRIM(warehouse.Province))))
    HAVING COUNT(*) = 1
) candidate
WHERE warehouse.ProvinceId IS NULL
  AND warehouse.CountryId IS NOT NULL
  AND NULLIF(LTRIM(RTRIM(warehouse.Province)), N'') IS NOT NULL
  AND candidate.Id IS NOT NULL;
GO

UPDATE warehouse
SET CityId = candidate.Id
FROM dbo.Warehouses warehouse
OUTER APPLY
(
    SELECT MIN(city.Id) AS Id
    FROM dbo.Cities city
    WHERE city.IsDeleted = 0
      AND city.CountryId = warehouse.CountryId
      AND city.ProvinceId = warehouse.ProvinceId
      AND (UPPER(LTRIM(RTRIM(city.Code))) = UPPER(LTRIM(RTRIM(warehouse.City)))
           OR UPPER(LTRIM(RTRIM(city.Name))) = UPPER(LTRIM(RTRIM(warehouse.City))))
    HAVING COUNT(*) = 1
) candidate
WHERE warehouse.CityId IS NULL
  AND warehouse.CountryId IS NOT NULL
  AND warehouse.ProvinceId IS NOT NULL
  AND NULLIF(LTRIM(RTRIM(warehouse.City)), N'') IS NOT NULL
  AND candidate.Id IS NOT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Warehouses') AND name=N'IX_Warehouses_CountryId')
    CREATE INDEX IX_Warehouses_CountryId ON dbo.Warehouses(CountryId) WHERE CountryId IS NOT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Warehouses') AND name=N'IX_Warehouses_ProvinceId')
    CREATE INDEX IX_Warehouses_ProvinceId ON dbo.Warehouses(ProvinceId) WHERE ProvinceId IS NOT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.Warehouses') AND name=N'IX_Warehouses_CityId')
    CREATE INDEX IX_Warehouses_CityId ON dbo.Warehouses(CityId) WHERE CityId IS NOT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_Warehouses_Countries_CountryId')
    ALTER TABLE dbo.Warehouses WITH CHECK ADD CONSTRAINT FK_Warehouses_Countries_CountryId FOREIGN KEY(CountryId) REFERENCES dbo.Countries(Id);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_Warehouses_Provinces_ProvinceId')
    ALTER TABLE dbo.Warehouses WITH CHECK ADD CONSTRAINT FK_Warehouses_Provinces_ProvinceId FOREIGN KEY(ProvinceId) REFERENCES dbo.Provinces(Id);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_Warehouses_Cities_CityId')
    ALTER TABLE dbo.Warehouses WITH CHECK ADD CONSTRAINT FK_Warehouses_Cities_CityId FOREIGN KEY(CityId) REFERENCES dbo.Cities(Id);
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_WAREHOUSES_LISTAR
AS
BEGIN
    SET NOCOUNT ON;
    SELECT warehouse.Id, warehouse.GlobalId, warehouse.Code, warehouse.Name, warehouse.Description,
           warehouse.BranchCode, warehouse.Address,
           warehouse.CityId, city.Code AS CityCode, COALESCE(city.Name, warehouse.City) AS City,
           warehouse.ProvinceId, province.Code AS ProvinceCode, COALESCE(province.Name, warehouse.Province) AS Province,
           warehouse.CountryId, country.Code AS CountryCode, COALESCE(country.Name, warehouse.Country) AS Country,
           warehouse.Phone, warehouse.Email, warehouse.ManagerName, warehouse.AllowsSales,
           warehouse.AllowsPurchases, warehouse.AllowsTransfers, warehouse.AllowsProduction,
           warehouse.IsDefault, warehouse.ExternalSystem, warehouse.ExternalCode, warehouse.SapCode,
           warehouse.IsActive, warehouse.CreatedByUserId, warehouse.CreatedByUserName, warehouse.CreatedAt,
           warehouse.UpdatedByUserId, warehouse.UpdatedByUserName, warehouse.UpdatedAt,
           warehouse.DeletedByUserId, warehouse.DeletedByUserName, warehouse.DeletedAt
    FROM dbo.Warehouses warehouse
    LEFT JOIN dbo.Countries country ON country.Id=warehouse.CountryId
    LEFT JOIN dbo.Provinces province ON province.Id=warehouse.ProvinceId
    LEFT JOIN dbo.Cities city ON city.Id=warehouse.CityId
    WHERE warehouse.IsDeleted=0
    ORDER BY warehouse.IsDefault DESC, warehouse.Name, warehouse.Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_WAREHOUSES_BUSCARPORID @Id int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP (1) warehouse.Id, warehouse.GlobalId, warehouse.Code, warehouse.Name, warehouse.Description,
           warehouse.BranchCode, warehouse.Address,
           warehouse.CityId, city.Code AS CityCode, COALESCE(city.Name, warehouse.City) AS City,
           warehouse.ProvinceId, province.Code AS ProvinceCode, COALESCE(province.Name, warehouse.Province) AS Province,
           warehouse.CountryId, country.Code AS CountryCode, COALESCE(country.Name, warehouse.Country) AS Country,
           warehouse.Phone, warehouse.Email, warehouse.ManagerName, warehouse.AllowsSales,
           warehouse.AllowsPurchases, warehouse.AllowsTransfers, warehouse.AllowsProduction,
           warehouse.IsDefault, warehouse.ExternalSystem, warehouse.ExternalCode, warehouse.SapCode,
           warehouse.IsActive, warehouse.CreatedByUserId, warehouse.CreatedByUserName, warehouse.CreatedAt,
           warehouse.UpdatedByUserId, warehouse.UpdatedByUserName, warehouse.UpdatedAt,
           warehouse.DeletedByUserId, warehouse.DeletedByUserName, warehouse.DeletedAt
    FROM dbo.Warehouses warehouse
    LEFT JOIN dbo.Countries country ON country.Id=warehouse.CountryId
    LEFT JOIN dbo.Provinces province ON province.Id=warehouse.ProvinceId
    LEFT JOIN dbo.Cities city ON city.Id=warehouse.CityId
    WHERE warehouse.Id=@Id AND warehouse.IsDeleted=0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_WAREHOUSES_CREAR
    @GlobalId uniqueidentifier, @Code nvarchar(50), @Name nvarchar(150),
    @Description nvarchar(500)=NULL, @BranchCode nvarchar(50)=NULL, @Address nvarchar(250)=NULL,
    @CityId int=NULL, @City nvarchar(100)=NULL, @ProvinceId int=NULL, @Province nvarchar(100)=NULL,
    @CountryId int=NULL, @Country nvarchar(100)=NULL, @Phone nvarchar(50)=NULL,
    @Email nvarchar(150)=NULL, @ManagerName nvarchar(150)=NULL,
    @AllowsSales bit, @AllowsPurchases bit, @AllowsTransfers bit, @AllowsProduction bit,
    @IsDefault bit, @ExternalSystem nvarchar(50)=NULL, @ExternalCode nvarchar(100)=NULL,
    @SapCode nvarchar(100)=NULL, @IsActive bit, @CreatedByUserId int=NULL,
    @CreatedByUserName nvarchar(120)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @ProvinceId IS NOT NULL AND @CountryId IS NULL THROW 51183, 'CountryId is required when ProvinceId is supplied.', 1;
    IF @CityId IS NOT NULL AND (@CountryId IS NULL OR @ProvinceId IS NULL) THROW 51183, 'CountryId and ProvinceId are required when CityId is supplied.', 1;
    IF @CountryId IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.Countries WHERE Id=@CountryId AND IsDeleted=0) THROW 51183, 'CountryId does not exist.', 1;
    IF @ProvinceId IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.Provinces WHERE Id=@ProvinceId AND CountryId=@CountryId AND IsDeleted=0) THROW 51183, 'ProvinceId does not belong to CountryId.', 1;
    IF @CityId IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.Cities WHERE Id=@CityId AND CountryId=@CountryId AND ProvinceId=@ProvinceId AND IsDeleted=0) THROW 51183, 'CityId does not belong to CountryId and ProvinceId.', 1;

    INSERT dbo.Warehouses
    (GlobalId,Code,Name,Description,BranchCode,Address,CityId,City,ProvinceId,Province,CountryId,Country,
     Phone,Email,ManagerName,AllowsSales,AllowsPurchases,AllowsTransfers,AllowsProduction,IsDefault,
     ExternalSystem,ExternalCode,SapCode,IsActive,IsDeleted,CreatedAt,CreatedByUserId,CreatedByUserName)
    VALUES
    (@GlobalId,@Code,@Name,@Description,@BranchCode,@Address,@CityId,@City,@ProvinceId,@Province,@CountryId,@Country,
     @Phone,@Email,@ManagerName,@AllowsSales,@AllowsPurchases,@AllowsTransfers,@AllowsProduction,@IsDefault,
     @ExternalSystem,@ExternalCode,@SapCode,@IsActive,0,SYSUTCDATETIME(),@CreatedByUserId,@CreatedByUserName);
    SELECT CONVERT(int,SCOPE_IDENTITY());
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_WAREHOUSES_ACTUALIZAR
    @Id int, @GlobalId uniqueidentifier, @Code nvarchar(50), @Name nvarchar(150),
    @Description nvarchar(500)=NULL, @BranchCode nvarchar(50)=NULL, @Address nvarchar(250)=NULL,
    @CityId int=NULL, @City nvarchar(100)=NULL, @ProvinceId int=NULL, @Province nvarchar(100)=NULL,
    @CountryId int=NULL, @Country nvarchar(100)=NULL, @Phone nvarchar(50)=NULL,
    @Email nvarchar(150)=NULL, @ManagerName nvarchar(150)=NULL,
    @AllowsSales bit, @AllowsPurchases bit, @AllowsTransfers bit, @AllowsProduction bit,
    @IsDefault bit, @ExternalSystem nvarchar(50)=NULL, @ExternalCode nvarchar(100)=NULL,
    @SapCode nvarchar(100)=NULL, @IsActive bit, @UpdatedByUserId int=NULL,
    @UpdatedByUserName nvarchar(120)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @ProvinceId IS NOT NULL AND @CountryId IS NULL THROW 51183, 'CountryId is required when ProvinceId is supplied.', 1;
    IF @CityId IS NOT NULL AND (@CountryId IS NULL OR @ProvinceId IS NULL) THROW 51183, 'CountryId and ProvinceId are required when CityId is supplied.', 1;
    IF @CountryId IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.Countries WHERE Id=@CountryId AND IsDeleted=0) THROW 51183, 'CountryId does not exist.', 1;
    IF @ProvinceId IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.Provinces WHERE Id=@ProvinceId AND CountryId=@CountryId AND IsDeleted=0) THROW 51183, 'ProvinceId does not belong to CountryId.', 1;
    IF @CityId IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.Cities WHERE Id=@CityId AND CountryId=@CountryId AND ProvinceId=@ProvinceId AND IsDeleted=0) THROW 51183, 'CityId does not belong to CountryId and ProvinceId.', 1;

    UPDATE dbo.Warehouses SET GlobalId=@GlobalId,Code=@Code,Name=@Name,Description=@Description,
        BranchCode=@BranchCode,Address=@Address,CityId=@CityId,City=@City,ProvinceId=@ProvinceId,
        Province=@Province,CountryId=@CountryId,Country=@Country,Phone=@Phone,Email=@Email,
        ManagerName=@ManagerName,AllowsSales=@AllowsSales,AllowsPurchases=@AllowsPurchases,
        AllowsTransfers=@AllowsTransfers,AllowsProduction=@AllowsProduction,IsDefault=@IsDefault,
        ExternalSystem=@ExternalSystem,ExternalCode=@ExternalCode,SapCode=@SapCode,IsActive=@IsActive,
        UpdatedAt=SYSUTCDATETIME(),UpdatedByUserId=@UpdatedByUserId,UpdatedByUserName=@UpdatedByUserName
    WHERE Id=@Id AND IsDeleted=0;
    SELECT @@ROWCOUNT;
END;
GO

IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
    THROW 51183, 'SchemaHistory is required before recording migration 183.', 1;
IF NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260808.183')
    INSERT dbo.SchemaHistory(Version,Description)
    VALUES(N'20260808.183',N'Relaciones geograficas opcionales y locales para Bodegas');
GO
