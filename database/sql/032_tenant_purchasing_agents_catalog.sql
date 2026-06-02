-- NuanSystem tenant purchasing agents catalog.
-- SQL Server is the primary provider. Keep provider-specific syntax isolated in SQL Server scripts.

IF OBJECT_ID(N'dbo.PurchasingAgents', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PurchasingAgents
    (
        PurchasingAgentId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PurchasingAgents PRIMARY KEY,
        Code nvarchar(30) NOT NULL,
        Name nvarchar(160) NOT NULL,
        Description nvarchar(300) NULL,
        Email nvarchar(256) NULL,
        Phone nvarchar(50) NULL,
        UserId int NULL,
        IsActive bit NOT NULL CONSTRAINT DF_PurchasingAgents_IsActive DEFAULT (1),
        IsDeleted bit NOT NULL CONSTRAINT DF_PurchasingAgents_IsDeleted DEFAULT (0),
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_PurchasingAgents_CreatedAt DEFAULT (SYSUTCDATETIME()),
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

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_PurchasingAgents_Code' AND object_id = OBJECT_ID(N'dbo.PurchasingAgents'))
    CREATE UNIQUE INDEX UX_PurchasingAgents_Code ON dbo.PurchasingAgents(Code) WHERE IsDeleted = 0;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PurchasingAgents WHERE Code = N'MFORTIZ')
    INSERT INTO dbo.PurchasingAgents (Code, Name, Email, CreatedByUserName)
    VALUES (N'MFORTIZ', N'Maria Fernandez Ortiz', N'maria.fernandez@nuansystem.local', N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.PurchasingAgents WHERE Code = N'ALPEREZ')
    INSERT INTO dbo.PurchasingAgents (Code, Name, Email, CreatedByUserName)
    VALUES (N'ALPEREZ', N'Ana Lucia Perez', N'ana.perez@nuansystem.local', N'Sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.PurchasingAgents WHERE Code = N'CPEREZ')
    INSERT INTO dbo.PurchasingAgents (Code, Name, Email, CreatedByUserName)
    VALUES (N'CPEREZ', N'Carlos Perez', N'carlos.perez@nuansystem.local', N'Sistema');
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PURCHASINGAGENTS_LISTAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        PurchasingAgentId AS Id,
        Code,
        Name,
        Description,
        Email,
        Phone,
        UserId,
        IsActive,
        CreatedAt,
        CreatedByUserId,
        CreatedByUserName,
        UpdatedAt,
        UpdatedByUserId,
        UpdatedByUserName
    FROM dbo.PurchasingAgents
    WHERE IsDeleted = 0
    ORDER BY Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PURCHASINGAGENTS_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        PurchasingAgentId AS Id,
        Code,
        Name,
        Description,
        Email,
        Phone,
        UserId,
        IsActive,
        CreatedAt,
        CreatedByUserId,
        CreatedByUserName,
        UpdatedAt,
        UpdatedByUserId,
        UpdatedByUserName
    FROM dbo.PurchasingAgents
    WHERE PurchasingAgentId = @Id
      AND IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PURCHASINGAGENTS_LOOKUP
AS
BEGIN
    SET NOCOUNT ON;

    SELECT PurchasingAgentId AS Id, Code, Name, IsActive
    FROM dbo.PurchasingAgents
    WHERE IsDeleted = 0 AND IsActive = 1
    ORDER BY Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PURCHASINGAGENTS_BUSCARPORCODIGO
    @Code nvarchar(30),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM dbo.PurchasingAgents
    WHERE IsDeleted = 0
      AND Code = @Code
      AND (@ExcluirId IS NULL OR PurchasingAgentId <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_PURCHASINGAGENTS_CREAR
    @Code nvarchar(30),
    @Name nvarchar(160),
    @Description nvarchar(300) = NULL,
    @Email nvarchar(256) = NULL,
    @Phone nvarchar(50) = NULL,
    @UserId int = NULL,
    @IsActive bit = 1,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.PurchasingAgents (Code, Name, Description, Email, Phone, UserId, IsActive, CreatedByUserId, CreatedByUserName)
    VALUES (@Code, @Name, @Description, @Email, @Phone, @UserId, @IsActive, @CreatedByUserId, @CreatedByUserName);

    SELECT CONVERT(int, SCOPE_IDENTITY());
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_PURCHASINGAGENTS_ACTUALIZAR
    @Id int,
    @Code nvarchar(30),
    @Name nvarchar(160),
    @Description nvarchar(300) = NULL,
    @Email nvarchar(256) = NULL,
    @Phone nvarchar(50) = NULL,
    @UserId int = NULL,
    @IsActive bit = 1,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.PurchasingAgents
    SET Code = @Code,
        Name = @Name,
        Description = @Description,
        Email = @Email,
        Phone = @Phone,
        UserId = @UserId,
        IsActive = @IsActive,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName
    WHERE PurchasingAgentId = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_PURCHASINGAGENTS_ELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.PurchasingAgents
    SET IsDeleted = 1,
        IsActive = 0,
        DeletedAt = SYSUTCDATETIME(),
        DeletedByUserId = @DeletedByUserId,
        DeletedByUserName = @DeletedByUserName
    WHERE PurchasingAgentId = @Id
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
    SELECT CurrencyId AS Id, Code, Name, IsActive FROM dbo.Currencies WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Code;
    SELECT PriceListId AS Id, Code, Name, IsActive FROM dbo.PriceLists WHERE IsDeleted = 0 AND IsActive = 1 AND AppliesTo IN (N'Purchasing', N'Both') ORDER BY IsDefault DESC, Name;
    SELECT PurchasingAgentId AS Id, Code, Name, IsActive FROM dbo.PurchasingAgents WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
END;
GO
