-- NuanSystem tenant financial bank catalogs.
-- SQL Server remains the primary provider; future providers must use equivalent provider-specific scripts.

IF OBJECT_ID(N'dbo.Banks', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Banks
    (
        BankId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Banks PRIMARY KEY,
        Code nvarchar(30) NOT NULL,
        Name nvarchar(160) NOT NULL,
        Description nvarchar(300) NULL,
        SwiftCode nvarchar(30) NULL,
        CountryId int NULL,
        IsActive bit NOT NULL CONSTRAINT DF_Banks_IsActive DEFAULT (1),
        IsDeleted bit NOT NULL CONSTRAINT DF_Banks_IsDeleted DEFAULT (0),
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_Banks_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(100) NULL,
        UpdatedAt datetime2(0) NULL,
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(100) NULL,
        DeletedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(100) NULL,
        CONSTRAINT FK_Banks_Countries FOREIGN KEY (CountryId) REFERENCES dbo.Countries(CountryId)
    );
END;
GO

IF OBJECT_ID(N'dbo.BankAccountTypes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BankAccountTypes
    (
        BankAccountTypeId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_BankAccountTypes PRIMARY KEY,
        Code nvarchar(30) NOT NULL,
        Name nvarchar(120) NOT NULL,
        Description nvarchar(300) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_BankAccountTypes_IsActive DEFAULT (1),
        IsDeleted bit NOT NULL CONSTRAINT DF_BankAccountTypes_IsDeleted DEFAULT (0),
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_BankAccountTypes_CreatedAt DEFAULT (SYSUTCDATETIME()),
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

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Banks_Code' AND object_id = OBJECT_ID(N'dbo.Banks'))
    CREATE UNIQUE INDEX UX_Banks_Code ON dbo.Banks(Code) WHERE IsDeleted = 0;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_BankAccountTypes_Code' AND object_id = OBJECT_ID(N'dbo.BankAccountTypes'))
    CREATE UNIQUE INDEX UX_BankAccountTypes_Code ON dbo.BankAccountTypes(Code) WHERE IsDeleted = 0;
GO

DECLARE @EcuadorId int = (SELECT CountryId FROM dbo.Countries WHERE Code = N'EC');

IF NOT EXISTS (SELECT 1 FROM dbo.Banks WHERE Code = N'PICHINCHA')
    INSERT INTO dbo.Banks (Code, Name, Description, CountryId, CreatedByUserName)
    VALUES (N'PICHINCHA', N'Banco Pichincha', N'Banco comercial local.', @EcuadorId, N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.Banks WHERE Code = N'GUAYAQUIL')
    INSERT INTO dbo.Banks (Code, Name, Description, CountryId, CreatedByUserName)
    VALUES (N'GUAYAQUIL', N'Banco de Guayaquil', N'Banco comercial local.', @EcuadorId, N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.Banks WHERE Code = N'PACIFICO')
    INSERT INTO dbo.Banks (Code, Name, Description, CountryId, CreatedByUserName)
    VALUES (N'PACIFICO', N'Banco del Pacifico', N'Banco comercial local.', @EcuadorId, N'Sistema');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.BankAccountTypes WHERE Code = N'CORRIENTE')
    INSERT INTO dbo.BankAccountTypes (Code, Name, Description, CreatedByUserName)
    VALUES (N'CORRIENTE', N'Corriente', N'Cuenta corriente bancaria.', N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.BankAccountTypes WHERE Code = N'AHORROS')
    INSERT INTO dbo.BankAccountTypes (Code, Name, Description, CreatedByUserName)
    VALUES (N'AHORROS', N'Ahorros', N'Cuenta de ahorros bancaria.', N'Sistema');
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_BANKS_LISTAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        b.BankId AS Id,
        b.Code,
        b.Name,
        b.Description,
        b.SwiftCode,
        b.CountryId,
        c.Code AS CountryCode,
        c.Name AS CountryName,
        b.IsActive,
        b.CreatedAt,
        b.CreatedByUserId,
        b.CreatedByUserName,
        b.UpdatedAt,
        b.UpdatedByUserId,
        b.UpdatedByUserName
    FROM dbo.Banks b
    LEFT JOIN dbo.Countries c ON c.CountryId = b.CountryId
    WHERE b.IsDeleted = 0
    ORDER BY b.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_BANKS_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        b.BankId AS Id,
        b.Code,
        b.Name,
        b.Description,
        b.SwiftCode,
        b.CountryId,
        c.Code AS CountryCode,
        c.Name AS CountryName,
        b.IsActive,
        b.CreatedAt,
        b.CreatedByUserId,
        b.CreatedByUserName,
        b.UpdatedAt,
        b.UpdatedByUserId,
        b.UpdatedByUserName
    FROM dbo.Banks b
    LEFT JOIN dbo.Countries c ON c.CountryId = b.CountryId
    WHERE b.BankId = @Id
      AND b.IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_BANKS_LOOKUP
AS
BEGIN
    SET NOCOUNT ON;

    SELECT BankId AS Id, Code, Name, IsActive
    FROM dbo.Banks
    WHERE IsDeleted = 0 AND IsActive = 1
    ORDER BY Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_BANKS_BUSCARPORCODIGO
    @Code nvarchar(30),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM dbo.Banks
    WHERE IsDeleted = 0
      AND Code = @Code
      AND (@ExcluirId IS NULL OR BankId <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_BANKS_CREAR
    @Code nvarchar(30),
    @Name nvarchar(160),
    @Description nvarchar(300) = NULL,
    @SwiftCode nvarchar(30) = NULL,
    @CountryId int = NULL,
    @IsActive bit = 1,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Banks (Code, Name, Description, SwiftCode, CountryId, IsActive, CreatedByUserId, CreatedByUserName)
    VALUES (@Code, @Name, @Description, @SwiftCode, @CountryId, @IsActive, @CreatedByUserId, @CreatedByUserName);

    SELECT CONVERT(int, SCOPE_IDENTITY());
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_BANKS_ACTUALIZAR
    @Id int,
    @Code nvarchar(30),
    @Name nvarchar(160),
    @Description nvarchar(300) = NULL,
    @SwiftCode nvarchar(30) = NULL,
    @CountryId int = NULL,
    @IsActive bit = 1,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Banks
    SET Code = @Code,
        Name = @Name,
        Description = @Description,
        SwiftCode = @SwiftCode,
        CountryId = @CountryId,
        IsActive = @IsActive,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName
    WHERE BankId = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_BANKS_ELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Banks
    SET IsDeleted = 1,
        IsActive = 0,
        DeletedAt = SYSUTCDATETIME(),
        DeletedByUserId = @DeletedByUserId,
        DeletedByUserName = @DeletedByUserName
    WHERE BankId = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_BANKACCOUNTTYPES_LISTAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        BankAccountTypeId AS Id,
        Code,
        Name,
        Description,
        IsActive,
        CreatedAt,
        CreatedByUserId,
        CreatedByUserName,
        UpdatedAt,
        UpdatedByUserId,
        UpdatedByUserName
    FROM dbo.BankAccountTypes
    WHERE IsDeleted = 0
    ORDER BY Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_BANKACCOUNTTYPES_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        BankAccountTypeId AS Id,
        Code,
        Name,
        Description,
        IsActive,
        CreatedAt,
        CreatedByUserId,
        CreatedByUserName,
        UpdatedAt,
        UpdatedByUserId,
        UpdatedByUserName
    FROM dbo.BankAccountTypes
    WHERE BankAccountTypeId = @Id
      AND IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_BANKACCOUNTTYPES_LOOKUP
AS
BEGIN
    SET NOCOUNT ON;

    SELECT BankAccountTypeId AS Id, Code, Name, IsActive
    FROM dbo.BankAccountTypes
    WHERE IsDeleted = 0 AND IsActive = 1
    ORDER BY Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_BANKACCOUNTTYPES_BUSCARPORCODIGO
    @Code nvarchar(30),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM dbo.BankAccountTypes
    WHERE IsDeleted = 0
      AND Code = @Code
      AND (@ExcluirId IS NULL OR BankAccountTypeId <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_BANKACCOUNTTYPES_CREAR
    @Code nvarchar(30),
    @Name nvarchar(120),
    @Description nvarchar(300) = NULL,
    @IsActive bit = 1,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.BankAccountTypes (Code, Name, Description, IsActive, CreatedByUserId, CreatedByUserName)
    VALUES (@Code, @Name, @Description, @IsActive, @CreatedByUserId, @CreatedByUserName);

    SELECT CONVERT(int, SCOPE_IDENTITY());
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_BANKACCOUNTTYPES_ACTUALIZAR
    @Id int,
    @Code nvarchar(30),
    @Name nvarchar(120),
    @Description nvarchar(300) = NULL,
    @IsActive bit = 1,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.BankAccountTypes
    SET Code = @Code,
        Name = @Name,
        Description = @Description,
        IsActive = @IsActive,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName
    WHERE BankAccountTypeId = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_BANKACCOUNTTYPES_ELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.BankAccountTypes
    SET IsDeleted = 1,
        IsActive = 0,
        DeletedAt = SYSUTCDATETIME(),
        DeletedByUserId = @DeletedByUserId,
        DeletedByUserName = @DeletedByUserName
    WHERE BankAccountTypeId = @Id
      AND IsDeleted = 0;

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
END;
GO
