/* Infraestructura compartida de auditoria detallada para catalogos tenant. */
IF OBJECT_ID(N'dbo.AuditCatalogChanges', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuditCatalogChanges
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditCatalogChanges PRIMARY KEY,
        EntityName nvarchar(120) NOT NULL,
        RecordId nvarchar(80) NOT NULL,
        [Action] nvarchar(20) NOT NULL,
        FieldName nvarchar(120) NOT NULL,
        OldValue nvarchar(max) NULL,
        NewValue nvarchar(max) NULL,
        UserId int NULL,
        UserName nvarchar(120) NULL,
        [Source] nvarchar(60) NOT NULL CONSTRAINT DF_AuditCatalogChanges_Source DEFAULT N'API',
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_AuditCatalogChanges_CreatedAt DEFAULT SYSUTCDATETIME()
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.AuditCatalogChanges') AND name = N'IX_AuditCatalogChanges_Entity_Record_CreatedAt')
    CREATE INDEX IX_AuditCatalogChanges_Entity_Record_CreatedAt ON dbo.AuditCatalogChanges (EntityName, RecordId, CreatedAt DESC);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.AuditCatalogChanges') AND name = N'IX_AuditCatalogChanges_User_CreatedAt')
    CREATE INDEX IX_AuditCatalogChanges_User_CreatedAt ON dbo.AuditCatalogChanges (UserId, CreatedAt DESC);
GO
