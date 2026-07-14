IF COL_LENGTH('dbo.Companies', 'OperationMode') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD OperationMode int NOT NULL CONSTRAINT DF_Companies_OperationMode DEFAULT 0;
END;
GO

IF COL_LENGTH('dbo.Companies', 'IsMaster') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD IsMaster bit NOT NULL CONSTRAINT DF_Companies_IsMaster DEFAULT 1;
END;
GO

IF COL_LENGTH('dbo.Companies', 'ParentCompanyId') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD ParentCompanyId int NULL;
END;
GO

IF COL_LENGTH('dbo.Companies', 'BranchCode') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD BranchCode nvarchar(50) NULL;
END;
GO

IF COL_LENGTH('dbo.Companies', 'SyncEnabled') IS NULL
BEGIN
    ALTER TABLE dbo.Companies ADD SyncEnabled bit NOT NULL CONSTRAINT DF_Companies_SyncEnabled DEFAULT 0;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_Companies_OperationMode' AND parent_object_id = OBJECT_ID(N'dbo.Companies'))
BEGIN
    ALTER TABLE dbo.Companies ADD CONSTRAINT CK_Companies_OperationMode CHECK (OperationMode IN (0, 1, 2));
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Companies_ParentCompany')
BEGIN
    ALTER TABLE dbo.Companies ADD CONSTRAINT FK_Companies_ParentCompany FOREIGN KEY (ParentCompanyId) REFERENCES dbo.Companies(Id);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Companies_ParentCompanyId' AND object_id = OBJECT_ID(N'dbo.Companies'))
BEGIN
    CREATE INDEX IX_Companies_ParentCompanyId ON dbo.Companies (ParentCompanyId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Companies_BranchCode' AND object_id = OBJECT_ID(N'dbo.Companies'))
BEGIN
    CREATE INDEX IX_Companies_BranchCode ON dbo.Companies (BranchCode) WHERE BranchCode IS NOT NULL;
END;
GO

IF OBJECT_ID(N'dbo.TenantFeatures', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TenantFeatures
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_TenantFeatures PRIMARY KEY,
        CompanyId int NOT NULL,
        FeatureCode nvarchar(80) NOT NULL,
        IsEnabled bit NOT NULL CONSTRAINT DF_TenantFeatures_IsEnabled DEFAULT 0,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_TenantFeatures_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT FK_TenantFeatures_Companies FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(Id),
        CONSTRAINT UQ_TenantFeatures_Company_Feature UNIQUE (CompanyId, FeatureCode),
        CONSTRAINT CK_TenantFeatures_FeatureCode CHECK (FeatureCode IN
        (
            N'SAP_B1_INTEGRATION',
            N'SRI_DOCUMENTS',
            N'MULTI_BRANCH_SYNC',
            N'INVENTORY_MODULE',
            N'PURCHASES_MODULE',
            N'SALES_MODULE',
            N'ACCOUNTING_MODULE',
            N'OFFLINE_BRANCH_MODE'
        ))
    );
END;
GO

IF OBJECT_ID(N'dbo.TenantIntegrations', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TenantIntegrations
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_TenantIntegrations PRIMARY KEY,
        CompanyId int NOT NULL,
        IntegrationCode nvarchar(80) NOT NULL,
        IsEnabled bit NOT NULL CONSTRAINT DF_TenantIntegrations_IsEnabled DEFAULT 0,
        ConfigurationJson nvarchar(max) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_TenantIntegrations_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT FK_TenantIntegrations_Companies FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(Id),
        CONSTRAINT UQ_TenantIntegrations_Company_Integration UNIQUE (CompanyId, IntegrationCode),
        CONSTRAINT CK_TenantIntegrations_IntegrationCode CHECK (IntegrationCode IN
        (
            N'SAP_B1',
            N'SRI',
            N'QLIK',
            N'EXTERNAL_API'
        )),
        CONSTRAINT CK_TenantIntegrations_ConfigurationJson CHECK (ConfigurationJson IS NULL OR ISJSON(ConfigurationJson) = 1)
    );
END;
GO

IF OBJECT_ID(N'dbo.EntityOwnershipConfigurations', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.EntityOwnershipConfigurations
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_EntityOwnershipConfigurations PRIMARY KEY,
        CompanyId int NOT NULL,
        EntityName nvarchar(120) NOT NULL,
        SourceOfTruth int NOT NULL,
        SyncDirection int NOT NULL,
        IsEnabled bit NOT NULL CONSTRAINT DF_EntityOwnershipConfigurations_IsEnabled DEFAULT 1,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_EntityOwnershipConfigurations_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT FK_EntityOwnershipConfigurations_Companies FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(Id),
        CONSTRAINT UQ_EntityOwnershipConfigurations_Company_Entity UNIQUE (CompanyId, EntityName),
        CONSTRAINT CK_EntityOwnershipConfigurations_SourceOfTruth CHECK (SourceOfTruth IN (0, 1, 2)),
        CONSTRAINT CK_EntityOwnershipConfigurations_SyncDirection CHECK (SyncDirection IN (0, 1, 2, 3, 4, 5))
    );
END;
GO

INSERT INTO dbo.TenantFeatures (CompanyId, FeatureCode, IsEnabled)
SELECT c.Id, feature.FeatureCode, feature.IsEnabled
FROM dbo.Companies c
CROSS APPLY
(
    VALUES
        (N'SAP_B1_INTEGRATION', CASE WHEN c.SapIntegrationMode <> 0 THEN CONVERT(bit, 1) ELSE CONVERT(bit, 0) END),
        (N'SRI_DOCUMENTS', CONVERT(bit, 0)),
        (N'MULTI_BRANCH_SYNC', c.SyncEnabled),
        (N'INVENTORY_MODULE', CONVERT(bit, 1)),
        (N'PURCHASES_MODULE', CONVERT(bit, 1)),
        (N'SALES_MODULE', CONVERT(bit, 1)),
        (N'ACCOUNTING_MODULE', CONVERT(bit, 0)),
        (N'OFFLINE_BRANCH_MODE', CASE WHEN c.IsMaster = 0 AND c.SyncEnabled = 1 THEN CONVERT(bit, 1) ELSE CONVERT(bit, 0) END)
) feature(FeatureCode, IsEnabled)
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.TenantFeatures existing
    WHERE existing.CompanyId = c.Id
      AND existing.FeatureCode = feature.FeatureCode
);
GO

INSERT INTO dbo.TenantIntegrations (CompanyId, IntegrationCode, IsEnabled, ConfigurationJson)
SELECT c.Id, integration.IntegrationCode, integration.IsEnabled, integration.ConfigurationJson
FROM dbo.Companies c
CROSS APPLY
(
    VALUES
        (N'SAP_B1', CASE WHEN c.SapIntegrationMode <> 0 THEN CONVERT(bit, 1) ELSE CONVERT(bit, 0) END, CONVERT(nvarchar(max), NULL)),
        (N'SRI', CONVERT(bit, 0), CONVERT(nvarchar(max), NULL)),
        (N'QLIK', CONVERT(bit, 0), CONVERT(nvarchar(max), NULL)),
        (N'EXTERNAL_API', CONVERT(bit, 0), CONVERT(nvarchar(max), NULL))
) integration(IntegrationCode, IsEnabled, ConfigurationJson)
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.TenantIntegrations existing
    WHERE existing.CompanyId = c.Id
      AND existing.IntegrationCode = integration.IntegrationCode
);
GO

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NOT NULL
    AND NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260709.02')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description)
    VALUES (N'20260709.02', N'Configuracion tenant por empresa: modo operacion, features, integraciones y ownership');
END;
GO

