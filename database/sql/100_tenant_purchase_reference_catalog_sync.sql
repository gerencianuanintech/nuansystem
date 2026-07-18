SET NOCOUNT ON;
SET XACT_ABORT ON;

IF OBJECT_ID(N'dbo.Taxes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Taxes
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Taxes PRIMARY KEY,
        GlobalId uniqueidentifier NOT NULL CONSTRAINT DF_Taxes_GlobalId DEFAULT NEWSEQUENTIALID(),
        Code nvarchar(50) NOT NULL,
        Name nvarchar(200) NOT NULL,
        Description nvarchar(500) NULL,
        Rate decimal(18,6) NOT NULL CONSTRAINT DF_Taxes_Rate DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_Taxes_IsActive DEFAULT 1,
        IsDeleted bit NOT NULL CONSTRAINT DF_Taxes_IsDeleted DEFAULT 0,
        ExternalSystem nvarchar(50) NULL,
        ExternalCode nvarchar(100) NULL,
        CreatedByUserId int NULL, CreatedByUserName nvarchar(150) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_Taxes_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL, UpdatedByUserName nvarchar(150) NULL, UpdatedAt datetime2(0) NULL,
        DeletedByUserId int NULL, DeletedByUserName nvarchar(150) NULL, DeletedAt datetime2(0) NULL,
        CONSTRAINT UQ_Taxes_Code UNIQUE(Code), CONSTRAINT UQ_Taxes_GlobalId UNIQUE(GlobalId)
    );
END;

IF COL_LENGTH(N'dbo.Taxes', N'Description') IS NULL ALTER TABLE dbo.Taxes ADD Description nvarchar(500) NULL;
IF COL_LENGTH(N'dbo.Taxes', N'GlobalId') IS NULL ALTER TABLE dbo.Taxes ADD GlobalId uniqueidentifier NULL;
IF COL_LENGTH(N'dbo.Taxes', N'ExternalSystem') IS NULL ALTER TABLE dbo.Taxes ADD ExternalSystem nvarchar(50) NULL;
IF COL_LENGTH(N'dbo.Taxes', N'ExternalCode') IS NULL ALTER TABLE dbo.Taxes ADD ExternalCode nvarchar(100) NULL;
IF COL_LENGTH(N'dbo.Taxes', N'IsDeleted') IS NULL ALTER TABLE dbo.Taxes ADD IsDeleted bit NOT NULL CONSTRAINT DF_Taxes_IsDeleted_Sync DEFAULT 0;
UPDATE dbo.Taxes SET GlobalId=NEWID() WHERE GlobalId IS NULL;
IF EXISTS(SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID(N'dbo.Taxes') AND name=N'GlobalId' AND is_nullable=1)
    ALTER TABLE dbo.Taxes ALTER COLUMN GlobalId uniqueidentifier NOT NULL;

IF OBJECT_ID(N'dbo.UnitOfMeasures', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UnitOfMeasures
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_UnitOfMeasures PRIMARY KEY,
        GlobalId uniqueidentifier NOT NULL CONSTRAINT DF_UnitOfMeasures_GlobalId DEFAULT NEWSEQUENTIALID(),
        Code nvarchar(50) NOT NULL, Name nvarchar(200) NOT NULL, Description nvarchar(500) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_UnitOfMeasures_IsActive DEFAULT 1,
        IsDeleted bit NOT NULL CONSTRAINT DF_UnitOfMeasures_IsDeleted DEFAULT 0,
        ExternalSystem nvarchar(50) NULL, ExternalCode nvarchar(100) NULL,
        CreatedByUserId int NULL, CreatedByUserName nvarchar(150) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_UnitOfMeasures_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL, UpdatedByUserName nvarchar(150) NULL, UpdatedAt datetime2(0) NULL,
        DeletedByUserId int NULL, DeletedByUserName nvarchar(150) NULL, DeletedAt datetime2(0) NULL,
        CONSTRAINT UQ_UnitOfMeasures_Code UNIQUE(Code), CONSTRAINT UQ_UnitOfMeasures_GlobalId UNIQUE(GlobalId)
    );
END;

IF OBJECT_ID(N'dbo.PriceLists', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PriceLists
    (
        PriceListId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PriceLists PRIMARY KEY,
        GlobalId uniqueidentifier NOT NULL CONSTRAINT DF_PriceLists_GlobalId DEFAULT NEWSEQUENTIALID(),
        Code nvarchar(50) NOT NULL, Name nvarchar(200) NOT NULL, Description nvarchar(500) NULL,
        CurrencyCode nvarchar(10) NOT NULL CONSTRAINT DF_PriceLists_CurrencyCode DEFAULT N'USD',
        AppliesTo nvarchar(30) NOT NULL CONSTRAINT DF_PriceLists_AppliesTo DEFAULT N'All',
        IsDefault bit NOT NULL CONSTRAINT DF_PriceLists_IsDefault DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_PriceLists_IsActive DEFAULT 1,
        IsDeleted bit NOT NULL CONSTRAINT DF_PriceLists_IsDeleted DEFAULT 0,
        ExternalSystem nvarchar(50) NULL, ExternalCode nvarchar(100) NULL, SapCode nvarchar(100) NULL,
        CreatedByUserId int NULL, CreatedByUserName nvarchar(150) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_PriceLists_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL, UpdatedByUserName nvarchar(150) NULL, UpdatedAt datetime2(0) NULL,
        DeletedByUserId int NULL, DeletedByUserName nvarchar(150) NULL, DeletedAt datetime2(0) NULL,
        CONSTRAINT UQ_PriceLists_Code UNIQUE(Code), CONSTRAINT UQ_PriceLists_GlobalId UNIQUE(GlobalId)
    );
END;

IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NOT NULL
AND NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260718.100')
    INSERT dbo.SchemaHistory(Version,Description) VALUES(N'20260718.100',N'Catalogos previos a ordenes: impuestos, unidades y listas sincronizables');
