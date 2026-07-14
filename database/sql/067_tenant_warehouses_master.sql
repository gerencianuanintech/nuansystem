SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF OBJECT_ID(N'dbo.Warehouses', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Warehouses
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Warehouses PRIMARY KEY,
        GlobalId uniqueidentifier NOT NULL CONSTRAINT DF_Warehouses_GlobalId DEFAULT NEWID(),
        Code nvarchar(50) NOT NULL,
        Name nvarchar(150) NOT NULL,
        Description nvarchar(500) NULL,
        BranchCode nvarchar(50) NULL,
        Address nvarchar(250) NULL,
        City nvarchar(100) NULL,
        Province nvarchar(100) NULL,
        Country nvarchar(100) NULL,
        Phone nvarchar(50) NULL,
        Email nvarchar(150) NULL,
        ManagerName nvarchar(150) NULL,
        AllowsSales bit NOT NULL CONSTRAINT DF_Warehouses_AllowsSales DEFAULT 1,
        AllowsPurchases bit NOT NULL CONSTRAINT DF_Warehouses_AllowsPurchases DEFAULT 1,
        AllowsTransfers bit NOT NULL CONSTRAINT DF_Warehouses_AllowsTransfers DEFAULT 1,
        AllowsProduction bit NOT NULL CONSTRAINT DF_Warehouses_AllowsProduction DEFAULT 0,
        IsDefault bit NOT NULL CONSTRAINT DF_Warehouses_IsDefault DEFAULT 0,
        ExternalSystem nvarchar(50) NULL,
        ExternalCode nvarchar(100) NULL,
        SapCode nvarchar(100) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_Warehouses_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_Warehouses_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_Warehouses_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL
    );
END;
GO

IF COL_LENGTH(N'dbo.Warehouses', N'GlobalId') IS NULL
    ALTER TABLE dbo.Warehouses ADD GlobalId uniqueidentifier NOT NULL CONSTRAINT DF_Warehouses_GlobalId DEFAULT NEWID();
IF COL_LENGTH(N'dbo.Warehouses', N'Description') IS NULL
    ALTER TABLE dbo.Warehouses ADD Description nvarchar(500) NULL;
IF COL_LENGTH(N'dbo.Warehouses', N'BranchCode') IS NULL
    ALTER TABLE dbo.Warehouses ADD BranchCode nvarchar(50) NULL;
IF COL_LENGTH(N'dbo.Warehouses', N'Address') IS NULL
    ALTER TABLE dbo.Warehouses ADD Address nvarchar(250) NULL;
IF COL_LENGTH(N'dbo.Warehouses', N'City') IS NULL
    ALTER TABLE dbo.Warehouses ADD City nvarchar(100) NULL;
IF COL_LENGTH(N'dbo.Warehouses', N'Province') IS NULL
    ALTER TABLE dbo.Warehouses ADD Province nvarchar(100) NULL;
IF COL_LENGTH(N'dbo.Warehouses', N'Country') IS NULL
    ALTER TABLE dbo.Warehouses ADD Country nvarchar(100) NULL;
IF COL_LENGTH(N'dbo.Warehouses', N'Phone') IS NULL
    ALTER TABLE dbo.Warehouses ADD Phone nvarchar(50) NULL;
IF COL_LENGTH(N'dbo.Warehouses', N'Email') IS NULL
    ALTER TABLE dbo.Warehouses ADD Email nvarchar(150) NULL;
IF COL_LENGTH(N'dbo.Warehouses', N'ManagerName') IS NULL
    ALTER TABLE dbo.Warehouses ADD ManagerName nvarchar(150) NULL;
IF COL_LENGTH(N'dbo.Warehouses', N'AllowsSales') IS NULL
    ALTER TABLE dbo.Warehouses ADD AllowsSales bit NOT NULL CONSTRAINT DF_Warehouses_AllowsSales DEFAULT 1;
IF COL_LENGTH(N'dbo.Warehouses', N'AllowsPurchases') IS NULL
    ALTER TABLE dbo.Warehouses ADD AllowsPurchases bit NOT NULL CONSTRAINT DF_Warehouses_AllowsPurchases DEFAULT 1;
IF COL_LENGTH(N'dbo.Warehouses', N'AllowsTransfers') IS NULL
    ALTER TABLE dbo.Warehouses ADD AllowsTransfers bit NOT NULL CONSTRAINT DF_Warehouses_AllowsTransfers DEFAULT 1;
IF COL_LENGTH(N'dbo.Warehouses', N'AllowsProduction') IS NULL
    ALTER TABLE dbo.Warehouses ADD AllowsProduction bit NOT NULL CONSTRAINT DF_Warehouses_AllowsProduction DEFAULT 0;
IF COL_LENGTH(N'dbo.Warehouses', N'IsDefault') IS NULL
    ALTER TABLE dbo.Warehouses ADD IsDefault bit NOT NULL CONSTRAINT DF_Warehouses_IsDefault DEFAULT 0;
IF COL_LENGTH(N'dbo.Warehouses', N'ExternalSystem') IS NULL
    ALTER TABLE dbo.Warehouses ADD ExternalSystem nvarchar(50) NULL;
IF COL_LENGTH(N'dbo.Warehouses', N'ExternalCode') IS NULL
    ALTER TABLE dbo.Warehouses ADD ExternalCode nvarchar(100) NULL;
IF COL_LENGTH(N'dbo.Warehouses', N'SapCode') IS NULL
    ALTER TABLE dbo.Warehouses ADD SapCode nvarchar(100) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Warehouses_GlobalId' AND object_id = OBJECT_ID(N'dbo.Warehouses'))
    CREATE UNIQUE INDEX UX_Warehouses_GlobalId ON dbo.Warehouses (GlobalId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Warehouses_ExternalRef' AND object_id = OBJECT_ID(N'dbo.Warehouses'))
    CREATE INDEX IX_Warehouses_ExternalRef ON dbo.Warehouses (ExternalSystem, ExternalCode);
GO

IF OBJECT_ID(N'dbo.SchemaVersions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SchemaVersions
    (
        Version nvarchar(50) NOT NULL CONSTRAINT PK_SchemaVersions PRIMARY KEY,
        Description nvarchar(250) NOT NULL,
        AppliedAt datetime2(0) NOT NULL CONSTRAINT DF_SchemaVersions_AppliedAt DEFAULT SYSUTCDATETIME()
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SchemaVersions WHERE Version = N'20260710.01')
BEGIN
    INSERT INTO dbo.SchemaVersions (Version, Description)
    VALUES (N'20260710.01', N'Maestro ampliado de bodegas tenant');
END;
GO
