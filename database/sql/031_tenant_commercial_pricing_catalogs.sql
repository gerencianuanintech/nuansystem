-- NuanSystem tenant commercial pricing catalogs.
-- SQL Server is the primary provider. Keep provider-specific syntax isolated in SQL Server scripts.

IF OBJECT_ID(N'dbo.Currencies', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Currencies
    (
        CurrencyId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Currencies PRIMARY KEY,
        Code nvarchar(3) NOT NULL,
        Name nvarchar(120) NOT NULL,
        Symbol nvarchar(10) NULL,
        Description nvarchar(300) NULL,
        IsBaseCurrency bit NOT NULL CONSTRAINT DF_Currencies_IsBaseCurrency DEFAULT (0),
        IsActive bit NOT NULL CONSTRAINT DF_Currencies_IsActive DEFAULT (1),
        IsDeleted bit NOT NULL CONSTRAINT DF_Currencies_IsDeleted DEFAULT (0),
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_Currencies_CreatedAt DEFAULT (SYSUTCDATETIME()),
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

IF OBJECT_ID(N'dbo.PriceLists', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PriceLists
    (
        PriceListId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PriceLists PRIMARY KEY,
        Code nvarchar(30) NOT NULL,
        Name nvarchar(120) NOT NULL,
        Description nvarchar(300) NULL,
        CurrencyCode nvarchar(3) NOT NULL CONSTRAINT DF_PriceLists_CurrencyCode DEFAULT (N'USD'),
        AppliesTo nvarchar(20) NOT NULL CONSTRAINT DF_PriceLists_AppliesTo DEFAULT (N'Both'),
        IsDefault bit NOT NULL CONSTRAINT DF_PriceLists_IsDefault DEFAULT (0),
        IsActive bit NOT NULL CONSTRAINT DF_PriceLists_IsActive DEFAULT (1),
        IsDeleted bit NOT NULL CONSTRAINT DF_PriceLists_IsDeleted DEFAULT (0),
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_PriceLists_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(100) NULL,
        UpdatedAt datetime2(0) NULL,
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(100) NULL,
        DeletedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(100) NULL,
        CONSTRAINT CK_PriceLists_AppliesTo CHECK (AppliesTo IN (N'Sales', N'Purchasing', N'Both'))
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Currencies_Code' AND object_id = OBJECT_ID(N'dbo.Currencies'))
    CREATE UNIQUE INDEX UX_Currencies_Code ON dbo.Currencies(Code) WHERE IsDeleted = 0;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_PriceLists_Code' AND object_id = OBJECT_ID(N'dbo.PriceLists'))
    CREATE UNIQUE INDEX UX_PriceLists_Code ON dbo.PriceLists(Code) WHERE IsDeleted = 0;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Currencies WHERE Code = N'USD')
    INSERT INTO dbo.Currencies (Code, Name, Symbol, Description, IsBaseCurrency, CreatedByUserName)
    VALUES (N'USD', N'USD - Dolar Americano', N'$', N'Moneda base por defecto.', 1, N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.Currencies WHERE Code = N'EUR')
    INSERT INTO dbo.Currencies (Code, Name, Symbol, Description, IsBaseCurrency, CreatedByUserName)
    VALUES (N'EUR', N'EUR - Euro', N'EUR', N'Moneda extranjera.', 0, N'Sistema');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PriceLists WHERE Code = N'LP1')
    INSERT INTO dbo.PriceLists (Code, Name, Description, CurrencyCode, AppliesTo, IsDefault, CreatedByUserName)
    VALUES (N'LP1', N'Lista de precios 1', N'Lista comercial principal.', N'USD', N'Both', 1, N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.PriceLists WHERE Code = N'LP2')
    INSERT INTO dbo.PriceLists (Code, Name, Description, CurrencyCode, AppliesTo, IsDefault, CreatedByUserName)
    VALUES (N'LP2', N'Lista de precios 2', N'Lista comercial secundaria.', N'USD', N'Both', 0, N'Sistema');
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CURRENCIES_LISTAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CurrencyId AS Id, Code, Name, Symbol, Description, IsBaseCurrency, IsActive,
           CreatedAt, CreatedByUserId, CreatedByUserName, UpdatedAt, UpdatedByUserId, UpdatedByUserName
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

    SELECT CurrencyId AS Id, Code, Name, Symbol, Description, IsBaseCurrency, IsActive,
           CreatedAt, CreatedByUserId, CreatedByUserName, UpdatedAt, UpdatedByUserId, UpdatedByUserName
    FROM dbo.Currencies
    WHERE CurrencyId = @Id AND IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CURRENCIES_LOOKUP
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CurrencyId AS Id, Code, Name, IsActive
    FROM dbo.Currencies
    WHERE IsDeleted = 0 AND IsActive = 1
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
    @Symbol nvarchar(10) = NULL,
    @Description nvarchar(300) = NULL,
    @IsBaseCurrency bit = 0,
    @IsActive bit = 1,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Currencies (Code, Name, Symbol, Description, IsBaseCurrency, IsActive, CreatedByUserId, CreatedByUserName)
    VALUES (@Code, @Name, @Symbol, @Description, @IsBaseCurrency, @IsActive, @CreatedByUserId, @CreatedByUserName);

    SELECT CONVERT(int, SCOPE_IDENTITY());
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_CURRENCIES_ACTUALIZAR
    @Id int,
    @Code nvarchar(3),
    @Name nvarchar(120),
    @Symbol nvarchar(10) = NULL,
    @Description nvarchar(300) = NULL,
    @IsBaseCurrency bit = 0,
    @IsActive bit = 1,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Currencies
    SET Code = @Code,
        Name = @Name,
        Symbol = @Symbol,
        Description = @Description,
        IsBaseCurrency = @IsBaseCurrency,
        IsActive = @IsActive,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName
    WHERE CurrencyId = @Id AND IsDeleted = 0;

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
    WHERE CurrencyId = @Id AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PRICELISTS_LISTAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT PriceListId AS Id, Code, Name, Description, CurrencyCode, AppliesTo, IsDefault, IsActive,
           CreatedAt, CreatedByUserId, CreatedByUserName, UpdatedAt, UpdatedByUserId, UpdatedByUserName
    FROM dbo.PriceLists
    WHERE IsDeleted = 0
    ORDER BY Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PRICELISTS_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT PriceListId AS Id, Code, Name, Description, CurrencyCode, AppliesTo, IsDefault, IsActive,
           CreatedAt, CreatedByUserId, CreatedByUserName, UpdatedAt, UpdatedByUserId, UpdatedByUserName
    FROM dbo.PriceLists
    WHERE PriceListId = @Id AND IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PRICELISTS_LOOKUP
    @AppliesTo nvarchar(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT PriceListId AS Id, Code, Name, IsActive
    FROM dbo.PriceLists
    WHERE IsDeleted = 0
      AND IsActive = 1
      AND (@AppliesTo IS NULL OR AppliesTo = @AppliesTo OR AppliesTo = N'Both')
    ORDER BY IsDefault DESC, Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PRICELISTS_BUSCARPORCODIGO
    @Code nvarchar(30),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM dbo.PriceLists
    WHERE IsDeleted = 0
      AND Code = @Code
      AND (@ExcluirId IS NULL OR PriceListId <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_PRICELISTS_CREAR
    @Code nvarchar(30),
    @Name nvarchar(120),
    @Description nvarchar(300) = NULL,
    @CurrencyCode nvarchar(3) = N'USD',
    @AppliesTo nvarchar(20) = N'Both',
    @IsDefault bit = 0,
    @IsActive bit = 1,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.PriceLists (Code, Name, Description, CurrencyCode, AppliesTo, IsDefault, IsActive, CreatedByUserId, CreatedByUserName)
    VALUES (@Code, @Name, @Description, @CurrencyCode, @AppliesTo, @IsDefault, @IsActive, @CreatedByUserId, @CreatedByUserName);

    SELECT CONVERT(int, SCOPE_IDENTITY());
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_PRICELISTS_ACTUALIZAR
    @Id int,
    @Code nvarchar(30),
    @Name nvarchar(120),
    @Description nvarchar(300) = NULL,
    @CurrencyCode nvarchar(3) = N'USD',
    @AppliesTo nvarchar(20) = N'Both',
    @IsDefault bit = 0,
    @IsActive bit = 1,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.PriceLists
    SET Code = @Code,
        Name = @Name,
        Description = @Description,
        CurrencyCode = @CurrencyCode,
        AppliesTo = @AppliesTo,
        IsDefault = @IsDefault,
        IsActive = @IsActive,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName
    WHERE PriceListId = @Id AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_PRICELISTS_ELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.PriceLists
    SET IsDeleted = 1,
        IsActive = 0,
        DeletedAt = SYSUTCDATETIME(),
        DeletedByUserId = @DeletedByUserId,
        DeletedByUserName = @DeletedByUserName
    WHERE PriceListId = @Id AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
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
    SELECT ProvinceId AS Id, Code, Name, IsActive FROM dbo.Provinces WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT CityId AS Id, Code, Name, IsActive FROM dbo.Cities WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT BankId AS Id, Code, Name, IsActive FROM dbo.Banks WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT BankAccountTypeId AS Id, Code, Name, IsActive FROM dbo.BankAccountTypes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT CurrencyId AS Id, Code, Name, IsActive FROM dbo.Currencies WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Code;
    SELECT PriceListId AS Id, Code, Name, IsActive FROM dbo.PriceLists WHERE IsDeleted = 0 AND IsActive = 1 AND AppliesTo IN (N'Purchasing', N'Both') ORDER BY IsDefault DESC, Name;
END;
GO
