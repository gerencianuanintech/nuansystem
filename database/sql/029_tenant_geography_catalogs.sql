-- NuanSystem tenant geography catalogs.
-- SQL Server is the primary provider. Keep provider-specific syntax isolated in SQL Server scripts.

IF OBJECT_ID(N'dbo.Countries', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Countries
    (
        CountryId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Countries PRIMARY KEY,
        Code nvarchar(10) NOT NULL,
        Name nvarchar(120) NOT NULL,
        Iso2 nvarchar(2) NULL,
        Iso3 nvarchar(3) NULL,
        PhonePrefix nvarchar(10) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_Countries_IsActive DEFAULT (1),
        IsDeleted bit NOT NULL CONSTRAINT DF_Countries_IsDeleted DEFAULT (0),
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_Countries_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(100) NULL,
        UpdatedAt datetime2(0) NULL,
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(100) NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.Provinces', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Provinces
    (
        ProvinceId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Provinces PRIMARY KEY,
        CountryId int NOT NULL,
        Code nvarchar(20) NOT NULL,
        Name nvarchar(120) NOT NULL,
        IsActive bit NOT NULL CONSTRAINT DF_Provinces_IsActive DEFAULT (1),
        IsDeleted bit NOT NULL CONSTRAINT DF_Provinces_IsDeleted DEFAULT (0),
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_Provinces_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(100) NULL,
        UpdatedAt datetime2(0) NULL,
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(100) NULL,
        CONSTRAINT FK_Provinces_Countries FOREIGN KEY (CountryId) REFERENCES dbo.Countries(CountryId)
    );
END;
GO

IF OBJECT_ID(N'dbo.Cities', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Cities
    (
        CityId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Cities PRIMARY KEY,
        CountryId int NOT NULL,
        ProvinceId int NOT NULL,
        Code nvarchar(20) NOT NULL,
        Name nvarchar(120) NOT NULL,
        IsActive bit NOT NULL CONSTRAINT DF_Cities_IsActive DEFAULT (1),
        IsDeleted bit NOT NULL CONSTRAINT DF_Cities_IsDeleted DEFAULT (0),
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_Cities_CreatedAt DEFAULT (SYSUTCDATETIME()),
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

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Countries_Code' AND object_id = OBJECT_ID(N'dbo.Countries'))
    CREATE UNIQUE INDEX UX_Countries_Code ON dbo.Countries(Code) WHERE IsDeleted = 0;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Provinces_Country_Code' AND object_id = OBJECT_ID(N'dbo.Provinces'))
    CREATE UNIQUE INDEX UX_Provinces_Country_Code ON dbo.Provinces(CountryId, Code) WHERE IsDeleted = 0;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Cities_Province_Code' AND object_id = OBJECT_ID(N'dbo.Cities'))
    CREATE UNIQUE INDEX UX_Cities_Province_Code ON dbo.Cities(ProvinceId, Code) WHERE IsDeleted = 0;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Countries WHERE Code = N'EC')
    INSERT INTO dbo.Countries (Code, Name, Iso2, Iso3, PhonePrefix, CreatedByUserName) VALUES (N'EC', N'Ecuador', N'EC', N'ECU', N'+593', N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.Countries WHERE Code = N'PE')
    INSERT INTO dbo.Countries (Code, Name, Iso2, Iso3, PhonePrefix, CreatedByUserName) VALUES (N'PE', N'Peru', N'PE', N'PER', N'+51', N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.Countries WHERE Code = N'CO')
    INSERT INTO dbo.Countries (Code, Name, Iso2, Iso3, PhonePrefix, CreatedByUserName) VALUES (N'CO', N'Colombia', N'CO', N'COL', N'+57', N'Sistema');
GO

DECLARE @EcuadorId int = (SELECT CountryId FROM dbo.Countries WHERE Code = N'EC');

IF @EcuadorId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Provinces WHERE CountryId = @EcuadorId AND Code = N'PIC')
    INSERT INTO dbo.Provinces (CountryId, Code, Name, CreatedByUserName) VALUES (@EcuadorId, N'PIC', N'Pichincha', N'Sistema');
IF @EcuadorId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Provinces WHERE CountryId = @EcuadorId AND Code = N'GYE')
    INSERT INTO dbo.Provinces (CountryId, Code, Name, CreatedByUserName) VALUES (@EcuadorId, N'GYE', N'Guayas', N'Sistema');
IF @EcuadorId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Provinces WHERE CountryId = @EcuadorId AND Code = N'AZU')
    INSERT INTO dbo.Provinces (CountryId, Code, Name, CreatedByUserName) VALUES (@EcuadorId, N'AZU', N'Azuay', N'Sistema');
GO

DECLARE @EcuadorId int = (SELECT CountryId FROM dbo.Countries WHERE Code = N'EC');
DECLARE @PichinchaId int = (SELECT ProvinceId FROM dbo.Provinces WHERE CountryId = @EcuadorId AND Code = N'PIC');
DECLARE @GuayasId int = (SELECT ProvinceId FROM dbo.Provinces WHERE CountryId = @EcuadorId AND Code = N'GYE');
DECLARE @AzuayId int = (SELECT ProvinceId FROM dbo.Provinces WHERE CountryId = @EcuadorId AND Code = N'AZU');

IF @PichinchaId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Cities WHERE ProvinceId = @PichinchaId AND Code = N'UIO')
    INSERT INTO dbo.Cities (CountryId, ProvinceId, Code, Name, CreatedByUserName) VALUES (@EcuadorId, @PichinchaId, N'UIO', N'Quito', N'Sistema');
IF @GuayasId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Cities WHERE ProvinceId = @GuayasId AND Code = N'GYE')
    INSERT INTO dbo.Cities (CountryId, ProvinceId, Code, Name, CreatedByUserName) VALUES (@EcuadorId, @GuayasId, N'GYE', N'Guayaquil', N'Sistema');
IF @AzuayId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Cities WHERE ProvinceId = @AzuayId AND Code = N'CUE')
    INSERT INTO dbo.Cities (CountryId, ProvinceId, Code, Name, CreatedByUserName) VALUES (@EcuadorId, @AzuayId, N'CUE', N'Cuenca', N'Sistema');
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_COUNTRIES_LOOKUP
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CountryId AS Id, Code, Name, IsActive
    FROM dbo.Countries
    WHERE IsDeleted = 0 AND IsActive = 1
    ORDER BY Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PROVINCES_LOOKUP
    @CountryCode nvarchar(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT p.ProvinceId AS Id, p.Code, p.Name, p.IsActive
    FROM dbo.Provinces p
    INNER JOIN dbo.Countries c ON c.CountryId = p.CountryId
    WHERE p.IsDeleted = 0
      AND p.IsActive = 1
      AND c.IsDeleted = 0
      AND (@CountryCode IS NULL OR c.Code = @CountryCode OR c.Iso2 = @CountryCode OR c.Iso3 = @CountryCode)
    ORDER BY p.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CITIES_LOOKUP
    @CountryCode nvarchar(10) = NULL,
    @ProvinceCode nvarchar(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ci.CityId AS Id, ci.Code, ci.Name, ci.IsActive
    FROM dbo.Cities ci
    INNER JOIN dbo.Countries c ON c.CountryId = ci.CountryId
    INNER JOIN dbo.Provinces p ON p.ProvinceId = ci.ProvinceId
    WHERE ci.IsDeleted = 0
      AND ci.IsActive = 1
      AND c.IsDeleted = 0
      AND p.IsDeleted = 0
      AND (@CountryCode IS NULL OR c.Code = @CountryCode OR c.Iso2 = @CountryCode OR c.Iso3 = @CountryCode)
      AND (@ProvinceCode IS NULL OR p.Code = @ProvinceCode)
    ORDER BY ci.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_BUSINESSPARTNERS_LOOKUPS
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Code, Name, CountryCode FROM dbo.BusinessPartnerIdentificationTypes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT Id, Code, Name, Days, IsCredit FROM dbo.BusinessPartnerPaymentTerms WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Days, Name;
    SELECT Id, Code, Name, IsActive FROM dbo.ChartOfAccounts WHERE IsDeleted = 0 AND IsActive = 1 AND AllowsMovement = 1 ORDER BY Code;
    SELECT N'Customer' AS Code, N'Cliente' AS Name UNION ALL SELECT N'Supplier', N'Proveedor' UNION ALL SELECT N'Both', N'Cliente y proveedor';
    SELECT N'Active' AS Code, N'Activo' AS Name UNION ALL SELECT N'Inactive', N'Inactivo';
    SELECT N'Pending' AS Code, N'Pendiente' AS Name UNION ALL SELECT N'Synced', N'Sincronizado' UNION ALL SELECT N'Error', N'Error';
    SELECT SupplierGroupId AS Id, Code, Name, IsActive FROM dbo.SupplierGroups WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT SupplierClassId AS Id, Code, Name, IsActive FROM dbo.SupplierClasses WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT EconomicActivityId AS Id, Code, Name, IsActive FROM dbo.EconomicActivities WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT ZoneId AS Id, Code, Name, IsActive FROM dbo.Zones WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT SupplyMethodId AS Id, Code, Name, IsActive FROM dbo.SupplyMethods WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT ContactTypeId AS Id, Code, Name, IsActive FROM dbo.ContactTypes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT ContactChannelId AS Id, Code, Name, IsActive FROM dbo.ContactChannels WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT CountryId AS Id, Code, Name, IsActive FROM dbo.Countries WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT ProvinceId AS Id, Code, Name, IsActive, CountryId, CAST(NULL AS int) AS ProvinceId, CAST(NULL AS nvarchar(30)) AS PostalCode
    FROM dbo.Provinces
    WHERE IsDeleted = 0 AND IsActive = 1
    ORDER BY Name;
    SELECT CityId AS Id, Code, Name, IsActive, CountryId, ProvinceId, CAST(NULL AS nvarchar(30)) AS PostalCode
    FROM dbo.Cities
    WHERE IsDeleted = 0 AND IsActive = 1
    ORDER BY Name;
END;
GO
