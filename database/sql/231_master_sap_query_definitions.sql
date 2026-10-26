/*
    Migracion 231 - Definiciones aprobadas de consultas SAP en Master.

    Objetos Master:
      - SapQueryDefinitions: versiones de consultas HANA/Service Layer por entidad.
      - parametros, watermark y aprobacion se conservan como configuracion auditable.

    Prerequisitos: 152_master_sap_sync_profiles.sql.
    No contiene secretos, no consulta tenants y no ejecuta SAP.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SapSyncHandlerCapabilities', N'U') IS NULL
    THROW 51231, 'SapSyncHandlerCapabilities is required before migration 231.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL
    THROW 51231, 'MasterSchemaHistory is required before migration 231.', 1;
GO

IF OBJECT_ID(N'dbo.SapQueryDefinitions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SapQueryDefinitions
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SapQueryDefinitions PRIMARY KEY,
        EntityCode nvarchar(80) NOT NULL,
        Transport varchar(20) NOT NULL,
        QueryName nvarchar(160) NOT NULL,
        QueryText nvarchar(max) NOT NULL,
        ParameterSchemaJson nvarchar(max) NOT NULL CONSTRAINT DF_SapQueryDefinitions_ParameterSchema DEFAULT N'{}',
        WatermarkField nvarchar(120) NULL,
        Version int NOT NULL,
        IsApproved bit NOT NULL CONSTRAINT DF_SapQueryDefinitions_IsApproved DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_SapQueryDefinitions_IsActive DEFAULT 0,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SapQueryDefinitions_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SapQueryDefinitions_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        RowVersion rowversion NOT NULL,
        CONSTRAINT FK_SapQueryDefinitions_Capability FOREIGN KEY (EntityCode) REFERENCES dbo.SapSyncHandlerCapabilities(EntityCode),
        CONSTRAINT CK_SapQueryDefinitions_EntityCode_NotBlank CHECK (LEN(LTRIM(RTRIM(EntityCode))) > 0),
        CONSTRAINT CK_SapQueryDefinitions_Transport CHECK (Transport IN ('Hana', 'ServiceLayer')),
        CONSTRAINT CK_SapQueryDefinitions_QueryName_NotBlank CHECK (LEN(LTRIM(RTRIM(QueryName))) > 0),
        CONSTRAINT CK_SapQueryDefinitions_QueryText_NotBlank CHECK (LEN(LTRIM(RTRIM(QueryText))) > 0),
        CONSTRAINT CK_SapQueryDefinitions_ParameterSchemaJson CHECK (ISJSON(ParameterSchemaJson) = 1),
        CONSTRAINT CK_SapQueryDefinitions_Version CHECK (Version BETWEEN 1 AND 1000000),
        CONSTRAINT CK_SapQueryDefinitions_Watermark_NotBlank CHECK (WatermarkField IS NULL OR LEN(LTRIM(RTRIM(WatermarkField))) > 0)
    );

    CREATE UNIQUE INDEX UX_SapQueryDefinitions_Entity_Transport_Version
        ON dbo.SapQueryDefinitions(EntityCode, Transport, Version)
        WHERE IsDeleted = 0;
    CREATE INDEX IX_SapQueryDefinitions_Active
        ON dbo.SapQueryDefinitions(EntityCode, Transport, IsApproved, IsActive, Version DESC)
        WHERE IsDeleted = 0;
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260904.231'
)
BEGIN
    INSERT dbo.MasterSchemaHistory(Version, Description)
    VALUES (N'20260904.231', N'Definiciones versionadas y aprobadas de consultas SAP');
END;
GO
