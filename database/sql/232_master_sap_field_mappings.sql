/*
    Migracion 232 - Mapeos configurables de campos SAP en Master.

    Objetos Master:
      - SapFieldMappings: campos SAP a campos NuanSystem por entidad de perfil.
      - reglas de direccion, tipo, transformacion y validacion.

    Prerequisitos: 152_master_sap_sync_profiles.sql y 231_master_sap_query_definitions.sql.
    No contiene secretos, no consulta tenants y no ejecuta SAP.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SapSyncProfileEntities', N'U') IS NULL
    THROW 51232, 'SapSyncProfileEntities is required before migration 232.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL
    THROW 51232, 'MasterSchemaHistory is required before migration 232.', 1;
GO

IF OBJECT_ID(N'dbo.SapFieldMappings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SapFieldMappings
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SapFieldMappings PRIMARY KEY,
        SapSyncProfileEntityId bigint NOT NULL,
        SapField nvarchar(120) NOT NULL,
        NuanField nvarchar(160) NOT NULL,
        Direction varchar(20) NOT NULL,
        DataType varchar(20) NOT NULL,
        IsRequired bit NOT NULL CONSTRAINT DF_SapFieldMappings_IsRequired DEFAULT 0,
        IsReadOnly bit NOT NULL CONSTRAINT DF_SapFieldMappings_IsReadOnly DEFAULT 0,
        TransformationKey nvarchar(80) NOT NULL,
        DefaultValue nvarchar(4000) NULL,
        ValidationRuleKey nvarchar(80) NULL,
        ExecutionOrder int NOT NULL CONSTRAINT DF_SapFieldMappings_ExecutionOrder DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_SapFieldMappings_IsActive DEFAULT 0,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SapFieldMappings_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SapFieldMappings_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        RowVersion rowversion NOT NULL,
        CONSTRAINT FK_SapFieldMappings_ProfileEntity FOREIGN KEY (SapSyncProfileEntityId) REFERENCES dbo.SapSyncProfileEntities(Id),
        CONSTRAINT CK_SapFieldMappings_SapField_NotBlank CHECK (LEN(LTRIM(RTRIM(SapField))) > 0),
        CONSTRAINT CK_SapFieldMappings_NuanField_NotBlank CHECK (LEN(LTRIM(RTRIM(NuanField))) > 0),
        CONSTRAINT CK_SapFieldMappings_Direction CHECK (Direction IN ('SapToErp', 'ErpToSap', 'Both', 'Generated')),
        CONSTRAINT CK_SapFieldMappings_DataType CHECK (DataType IN ('Text', 'Integer', 'Decimal', 'Boolean', 'Date', 'DateTime', 'Json')),
        CONSTRAINT CK_SapFieldMappings_Transformation_NotBlank CHECK (LEN(LTRIM(RTRIM(TransformationKey))) > 0),
        CONSTRAINT CK_SapFieldMappings_Validation_NotBlank CHECK (ValidationRuleKey IS NULL OR LEN(LTRIM(RTRIM(ValidationRuleKey))) > 0),
        CONSTRAINT CK_SapFieldMappings_ExecutionOrder CHECK (ExecutionOrder BETWEEN 0 AND 100000)
    );

    CREATE UNIQUE INDEX UX_SapFieldMappings_Current
        ON dbo.SapFieldMappings(SapSyncProfileEntityId, Direction, NuanField)
        WHERE IsDeleted = 0;
    CREATE INDEX IX_SapFieldMappings_ProfileEntity_Order
        ON dbo.SapFieldMappings(SapSyncProfileEntityId, IsActive, ExecutionOrder, Id)
        WHERE IsDeleted = 0;
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260904.232'
)
BEGIN
    INSERT dbo.MasterSchemaHistory(Version, Description)
    VALUES (N'20260904.232', N'Mapeos versionados de campos SAP por entidad de perfil');
END;
GO
