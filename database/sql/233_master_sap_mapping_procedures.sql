/*
    Migracion 233 - Procedimientos Master para consultas y mapeos SAP.

    Prerequisitos: 231_master_sap_query_definitions.sql y 232_master_sap_field_mappings.sql.
    Procedimientos parametrizados, auditables e idempotentes. No consulta tenants ni SAP.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SapQueryDefinitions', N'U') IS NULL
    THROW 51233, 'SapQueryDefinitions is required before migration 233.', 1;
IF OBJECT_ID(N'dbo.SapFieldMappings', N'U') IS NULL
    THROW 51233, 'SapFieldMappings is required before migration 233.', 1;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SAPQUERYDEFINITIONS_LISTAR
    @EntityCode nvarchar(80) = NULL,
    @Transport varchar(20) = NULL,
    @ActiveOnly bit = 1
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, EntityCode, Transport, QueryName, QueryText, ParameterSchemaJson,
           WatermarkField, Version, IsApproved, IsActive, CreatedAt AS CreatedAtUtc,
           UpdatedAt AS UpdatedAtUtc, CONVERT(varbinary(8), RowVersion) AS RowVersion
    FROM dbo.SapQueryDefinitions
    WHERE IsDeleted = 0
      AND (@EntityCode IS NULL OR EntityCode = @EntityCode)
      AND (@Transport IS NULL OR Transport = @Transport)
      AND (@ActiveOnly = 0 OR (IsApproved = 1 AND IsActive = 1))
    ORDER BY EntityCode, Transport, Version DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SAPFIELDMAPPINGS_LISTAR
    @SapSyncProfileEntityId bigint,
    @Direction varchar(20) = NULL,
    @ActiveOnly bit = 1
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, SapSyncProfileEntityId, SapField, NuanField, Direction, DataType,
           IsRequired, IsReadOnly, TransformationKey, DefaultValue, ValidationRuleKey,
           ExecutionOrder, IsActive, CreatedAt AS CreatedAtUtc,
           UpdatedAt AS UpdatedAtUtc, CONVERT(varbinary(8), RowVersion) AS RowVersion
    FROM dbo.SapFieldMappings
    WHERE IsDeleted = 0
      AND SapSyncProfileEntityId = @SapSyncProfileEntityId
      AND (@Direction IS NULL OR Direction = @Direction)
      AND (@ActiveOnly = 0 OR IsActive = 1)
    ORDER BY ExecutionOrder, Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SAPQUERYDEFINITIONCREAR
    @EntityCode nvarchar(80),
    @Transport varchar(20),
    @QueryName nvarchar(160),
    @QueryText nvarchar(max),
    @ParameterSchemaJson nvarchar(max),
    @WatermarkField nvarchar(120) = NULL,
    @Version int,
    @IsApproved bit,
    @IsActive bit,
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF EXISTS (SELECT 1 FROM dbo.SapQueryDefinitions WHERE IsDeleted = 0 AND EntityCode = @EntityCode AND Transport = @Transport AND Version = @Version)
        SELECT CAST(NULL AS bigint) AS Id, 'DuplicateVersion' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
    ELSE
    BEGIN
        INSERT dbo.SapQueryDefinitions
        (EntityCode, Transport, QueryName, QueryText, ParameterSchemaJson, WatermarkField, Version, IsApproved, IsActive, CreatedByUserId, CreatedByUserName)
        VALUES
        (@EntityCode, @Transport, @QueryName, @QueryText, @ParameterSchemaJson, @WatermarkField, @Version, @IsApproved, @IsActive, @AuditUserId, @AuditUserName);
        DECLARE @Id bigint = SCOPE_IDENTITY();
        SELECT @Id AS Id, 'Created' AS ResultCode,
               CONVERT(varbinary(8), RowVersion) AS RowVersion
        FROM dbo.SapQueryDefinitions WHERE Id = @Id;
    END;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_SAPFIELDMAPPINGSREEMPLAZAR
    @SapSyncProfileEntityId bigint,
    @MappingsJson nvarchar(max),
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF ISJSON(@MappingsJson) <> 1
        THROW 51234, 'MappingsJson must be valid JSON.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.SapSyncProfileEntities WHERE Id = @SapSyncProfileEntityId AND IsDeleted = 0)
    BEGIN
        SELECT CAST(NULL AS bigint) AS Id, 'ProfileEntityNotFound' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
        RETURN;
    END;
    BEGIN TRANSACTION;
    UPDATE dbo.SapFieldMappings
    SET IsDeleted = 1, IsActive = 0, DeletedByUserId = @AuditUserId, DeletedByUserName = @AuditUserName, DeletedAt = SYSUTCDATETIME(), UpdatedAt = SYSUTCDATETIME(), UpdatedByUserId = @AuditUserId, UpdatedByUserName = @AuditUserName
    WHERE SapSyncProfileEntityId = @SapSyncProfileEntityId AND IsDeleted = 0;
    INSERT dbo.SapFieldMappings
    (SapSyncProfileEntityId, SapField, NuanField, Direction, DataType, IsRequired, IsReadOnly, TransformationKey, DefaultValue, ValidationRuleKey, ExecutionOrder, IsActive, CreatedByUserId, CreatedByUserName)
    SELECT @SapSyncProfileEntityId, SapField, NuanField, Direction, DataType, IsRequired, IsReadOnly, TransformationKey, DefaultValue, ValidationRuleKey, ExecutionOrder, IsActive, @AuditUserId, @AuditUserName
    FROM OPENJSON(@MappingsJson)
    WITH
    (
        SapField nvarchar(120) '$.sapField', NuanField nvarchar(160) '$.nuanField', Direction varchar(20) '$.direction', DataType varchar(20) '$.dataType', IsRequired bit '$.isRequired', IsReadOnly bit '$.isReadOnly', TransformationKey nvarchar(80) '$.transformationKey', DefaultValue nvarchar(4000) '$.defaultValue', ValidationRuleKey nvarchar(80) '$.validationRuleKey', ExecutionOrder int '$.executionOrder', IsActive bit '$.isActive'
    );
    COMMIT TRANSACTION;
    SELECT @SapSyncProfileEntityId AS Id, 'Replaced' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PATCH_SAPQUERYDEFINITIONDESACTIVAR
    @Id bigint,
    @ExpectedRowVersion varbinary(8),
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.SapQueryDefinitions
    SET IsActive = 0, UpdatedByUserId = @AuditUserId, UpdatedByUserName = @AuditUserName, UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id AND IsDeleted = 0 AND (@ExpectedRowVersion IS NULL OR RowVersion = @ExpectedRowVersion);
    IF @@ROWCOUNT = 0
        SELECT CAST(NULL AS bigint) AS Id, 'NotFoundOrConcurrencyConflict' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
    ELSE
        SELECT Id, 'Deactivated' AS ResultCode, CONVERT(varbinary(8), RowVersion) AS RowVersion FROM dbo.SapQueryDefinitions WHERE Id = @Id;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260904.233')
    INSERT dbo.MasterSchemaHistory(Version, Description) VALUES (N'20260904.233', N'Procedimientos de consultas y mapeos SAP');
GO
