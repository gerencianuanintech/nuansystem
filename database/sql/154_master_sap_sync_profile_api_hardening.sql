/*
    Migracion 154 - Hardening de la API de perfiles SAP en NuanSystem_Master.

    Prerrequisito: 152_master_sap_sync_profiles.sql.
    Alcance:
      - consulta parametrizada de empresas accesibles para perfiles SAP;
      - sin secretos de SapCompanySettings;
      - reparacion forward-only e idempotente.

    No crea formularios, menus, permisos ni perfiles. No habilita agendas.
    SapSyncEntitySettings se conserva sin cambios.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.Companies', N'U') IS NULL
    THROW 51154, 'Companies is required before migration 154.', 1;
IF OBJECT_ID(N'dbo.UserCompanies', N'U') IS NULL
    THROW 51154, 'UserCompanies is required before migration 154.', 1;
IF OBJECT_ID(N'dbo.SapCompanySettings', N'U') IS NULL
    THROW 51154, 'SapCompanySettings is required before migration 154.', 1;
IF OBJECT_ID(N'dbo.SapSyncProfiles', N'U') IS NULL
    THROW 51154, 'Migration 152 is required before migration 154.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL
    THROW 51154, 'MasterSchemaHistory is required before migration 154.', 1;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SAPSYNCPROFILEEMPRESASACCESIBLES
    @UserId int,
    @CompanyId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        company.Id AS CompanyId,
        company.Code AS CompanyCode,
        company.CommercialName AS CompanyName,
        company.IsActive AS IsCompanyActive,
        company.SapIntegrationMode,
        CONVERT(bit, CASE WHEN settings.Id IS NULL THEN 0 ELSE 1 END) AS HasSapSettings,
        CONVERT(bit, COALESCE(settings.IsEnabled, 0)) AS IsSapEnabled,
        COALESCE(settings.IntegrationMode, 0) AS SapSettingsIntegrationMode,
        CONVERT(bit, CASE WHEN userCompany.UserId IS NULL THEN 0 ELSE 1 END) AS IsUserAuthorized
    FROM dbo.Companies company
    LEFT JOIN dbo.SapCompanySettings settings
        ON settings.CompanyId = company.Id
    LEFT JOIN dbo.UserCompanies userCompany
        ON userCompany.CompanyId = company.Id
       AND userCompany.UserId = @UserId
       AND userCompany.IsActive = 1
    WHERE
        (@CompanyId IS NULL AND userCompany.UserId IS NOT NULL)
        OR
        (@CompanyId IS NOT NULL AND company.Id = @CompanyId)
    ORDER BY company.CommercialName, company.Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_SAPSYNCPROFILEACTUALIZAR
    @Id bigint,
    @CompanyId int,
    @Code nvarchar(80),
    @Name nvarchar(160),
    @Description nvarchar(500) = NULL,
    @IsActive bit,
    @EntitiesJson nvarchar(max),
    @ExpectedRowVersion varbinary(8),
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @Code = NULLIF(LTRIM(RTRIM(@Code)), N'');
    SET @Name = NULLIF(LTRIM(RTRIM(@Name)), N'');
    SET @Description = NULLIF(LTRIM(RTRIM(@Description)), N'');
    SET @AuditUserName = NULLIF(LTRIM(RTRIM(@AuditUserName)), N'');

    IF NOT EXISTS (SELECT 1 FROM dbo.SapSyncProfiles WHERE Id = @Id AND IsDeleted = 0)
    BEGIN
        SELECT @Id AS Id, N'NotFound' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
        RETURN;
    END;
    IF EXISTS
    (
        SELECT 1
        FROM dbo.SapSyncProfiles
        WHERE Id = @Id
          AND CompanyId <> @CompanyId
          AND IsDeleted = 0
    )
    BEGIN
        SELECT @Id AS Id, N'CompanyImmutable' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
        RETURN;
    END;
    IF @Code IS NULL OR @Name IS NULL OR ISJSON(@EntitiesJson) <> 1
    BEGIN
        SELECT @Id AS Id, N'InvalidProfile' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
        RETURN;
    END;
    IF NOT EXISTS (SELECT 1 FROM dbo.SapSyncProfiles WHERE Id = @Id AND RowVersion = @ExpectedRowVersion AND IsDeleted = 0)
    BEGIN
        SELECT @Id AS Id, N'ConcurrencyConflict' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
        RETURN;
    END;
    IF EXISTS (SELECT 1 FROM dbo.SapSyncProfiles WHERE CompanyId = @CompanyId AND Code = @Code AND Id <> @Id AND IsDeleted = 0)
    BEGIN
        SELECT @Id AS Id, N'DuplicateCode' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
        RETURN;
    END;

    DECLARE @Entities table
    (
        EntityCode nvarchar(80) NOT NULL,
        Direction varchar(20) NOT NULL,
        SyncMode varchar(20) NOT NULL,
        BatchSize int NOT NULL,
        MaxAttempts int NOT NULL,
        ExecutionOrder int NOT NULL,
        ContinueOnError bit NOT NULL,
        ExecutionTimeoutMinutes int NOT NULL,
        IsActive bit NOT NULL,
        ScheduleType varchar(20) NOT NULL,
        IntervalMinutes int NULL,
        ExecutionTime time(0) NULL,
        TimeZoneId nvarchar(100) NOT NULL,
        PreventConcurrentExecutions bit NOT NULL,
        ScheduleIsActive bit NOT NULL,
        PRIMARY KEY(EntityCode, Direction)
    );

    INSERT @Entities
    SELECT
        LTRIM(RTRIM(EntityCode)), Direction, SyncMode, BatchSize, MaxAttempts,
        ExecutionOrder, ContinueOnError, ExecutionTimeoutMinutes, IsActive,
        ScheduleType, IntervalMinutes, ExecutionTime,
        COALESCE(NULLIF(LTRIM(RTRIM(TimeZoneId)), N''), N'America/Guayaquil'),
        PreventConcurrentExecutions, ScheduleIsActive
    FROM OPENJSON(@EntitiesJson)
    WITH
    (
        EntityCode nvarchar(80) '$.entityCode',
        Direction varchar(20) '$.direction',
        SyncMode varchar(20) '$.syncMode',
        BatchSize int '$.batchSize',
        MaxAttempts int '$.maxAttempts',
        ExecutionOrder int '$.executionOrder',
        ContinueOnError bit '$.continueOnError',
        ExecutionTimeoutMinutes int '$.executionTimeoutMinutes',
        IsActive bit '$.isActive',
        ScheduleType varchar(20) '$.schedule.scheduleType',
        IntervalMinutes int '$.schedule.intervalMinutes',
        ExecutionTime time(0) '$.schedule.executionTime',
        TimeZoneId nvarchar(100) '$.schedule.timeZoneId',
        PreventConcurrentExecutions bit '$.schedule.preventConcurrentExecutions',
        ScheduleIsActive bit '$.schedule.isActive'
    );

    IF NOT EXISTS (SELECT 1 FROM @Entities)
       OR EXISTS
          (
              SELECT 1
              FROM @Entities entity
              LEFT JOIN dbo.SapSyncHandlerCapabilities capability ON capability.EntityCode = entity.EntityCode
              WHERE capability.Id IS NULL
                 OR capability.IsActive = 0
                 OR capability.IsImplemented = 0
                 OR entity.Direction = 'Both'
                 OR (entity.Direction = 'SapToErp' AND capability.SupportsSapToErp = 0)
                 OR (entity.Direction = 'ErpToSap' AND capability.SupportsErpToSap = 0)
                 OR (entity.SyncMode = 'Full' AND capability.SupportsFull = 0)
                 OR (entity.SyncMode = 'Incremental' AND capability.SupportsIncremental = 0)
                 OR entity.BatchSize NOT BETWEEN 1 AND 10000
                 OR entity.MaxAttempts NOT BETWEEN 1 AND 20
                 OR entity.ExecutionOrder NOT BETWEEN 0 AND 100000
                 OR entity.ExecutionTimeoutMinutes NOT BETWEEN 1 AND 1440
                 OR NOT
                    (
                        (entity.ScheduleType = 'Manual' AND entity.IntervalMinutes IS NULL AND entity.ExecutionTime IS NULL)
                        OR
                        (entity.ScheduleType = 'Interval' AND entity.IntervalMinutes BETWEEN 1 AND 525600 AND entity.ExecutionTime IS NULL)
                        OR
                        (entity.ScheduleType = 'Daily' AND entity.IntervalMinutes IS NULL AND entity.ExecutionTime IS NOT NULL)
                    )
          )
    BEGIN
        SELECT @Id AS Id, N'UnsupportedCapability' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.SapSyncProfiles WITH (UPDLOCK)
        SET Code = @Code,
            Name = @Name,
            Description = @Description,
            UpdatedByUserId = @AuditUserId,
            UpdatedByUserName = @AuditUserName,
            UpdatedAt = SYSUTCDATETIME()
        WHERE Id = @Id
          AND CompanyId = @CompanyId
          AND RowVersion = @ExpectedRowVersion
          AND IsDeleted = 0;

        IF @@ROWCOUNT = 0
        BEGIN
            ROLLBACK;
            SELECT @Id AS Id, N'ConcurrencyConflict' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
            RETURN;
        END;

        UPDATE schedule
        SET IsDeleted = 1,
            IsActive = 0,
            DeletedByUserId = @AuditUserId,
            DeletedByUserName = @AuditUserName,
            DeletedAt = SYSUTCDATETIME(),
            UpdatedByUserId = @AuditUserId,
            UpdatedByUserName = @AuditUserName,
            UpdatedAt = SYSUTCDATETIME()
        FROM dbo.SapSyncSchedules schedule
        INNER JOIN dbo.SapSyncProfileEntities profileEntity ON profileEntity.Id = schedule.SapSyncProfileEntityId
        WHERE profileEntity.SapSyncProfileId = @Id
          AND schedule.IsDeleted = 0
          AND NOT EXISTS
              (
                  SELECT 1
                  FROM @Entities source
                  WHERE source.EntityCode = profileEntity.EntityCode
                    AND source.Direction = profileEntity.Direction
              );

        UPDATE profileEntity
        SET IsDeleted = 1,
            IsActive = 0,
            DeletedByUserId = @AuditUserId,
            DeletedByUserName = @AuditUserName,
            DeletedAt = SYSUTCDATETIME(),
            UpdatedByUserId = @AuditUserId,
            UpdatedByUserName = @AuditUserName,
            UpdatedAt = SYSUTCDATETIME()
        FROM dbo.SapSyncProfileEntities profileEntity
        WHERE profileEntity.SapSyncProfileId = @Id
          AND profileEntity.IsDeleted = 0
          AND NOT EXISTS
              (
                  SELECT 1
                  FROM @Entities source
                  WHERE source.EntityCode = profileEntity.EntityCode
                    AND source.Direction = profileEntity.Direction
              );

        UPDATE target
        SET SyncMode = source.SyncMode,
            BatchSize = source.BatchSize,
            MaxAttempts = source.MaxAttempts,
            ExecutionOrder = source.ExecutionOrder,
            ContinueOnError = source.ContinueOnError,
            ExecutionTimeoutMinutes = source.ExecutionTimeoutMinutes,
            IsActive = source.IsActive,
            UpdatedByUserId = @AuditUserId,
            UpdatedByUserName = @AuditUserName,
            UpdatedAt = SYSUTCDATETIME()
        FROM dbo.SapSyncProfileEntities target
        INNER JOIN @Entities source
            ON source.EntityCode = target.EntityCode
           AND source.Direction = target.Direction
        WHERE target.SapSyncProfileId = @Id
          AND target.IsDeleted = 0;

        INSERT dbo.SapSyncProfileEntities
        (
            SapSyncProfileId, EntityCode, Direction, SyncMode, BatchSize,
            MaxAttempts, ExecutionOrder, ContinueOnError, ExecutionTimeoutMinutes,
            IsActive, CreatedByUserId, CreatedByUserName
        )
        SELECT
            @Id, source.EntityCode, source.Direction, source.SyncMode, source.BatchSize,
            source.MaxAttempts, source.ExecutionOrder, source.ContinueOnError,
            source.ExecutionTimeoutMinutes, source.IsActive,
            @AuditUserId, @AuditUserName
        FROM @Entities source
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM dbo.SapSyncProfileEntities target
            WHERE target.SapSyncProfileId = @Id
              AND target.EntityCode = source.EntityCode
              AND target.Direction = source.Direction
              AND target.IsDeleted = 0
        );

        UPDATE target
        SET ScheduleType = source.ScheduleType,
            IntervalMinutes = source.IntervalMinutes,
            ExecutionTime = source.ExecutionTime,
            TimeZoneId = source.TimeZoneId,
            PreventConcurrentExecutions = source.PreventConcurrentExecutions,
            IsActive = source.ScheduleIsActive,
            NextExecutionAtUtc = CASE WHEN source.ScheduleIsActive = 0 OR source.ScheduleType = 'Manual' THEN NULL ELSE target.NextExecutionAtUtc END,
            UpdatedByUserId = @AuditUserId,
            UpdatedByUserName = @AuditUserName,
            UpdatedAt = SYSUTCDATETIME()
        FROM dbo.SapSyncSchedules target
        INNER JOIN dbo.SapSyncProfileEntities profileEntity
            ON profileEntity.Id = target.SapSyncProfileEntityId
           AND profileEntity.SapSyncProfileId = @Id
           AND profileEntity.IsDeleted = 0
        INNER JOIN @Entities source
            ON source.EntityCode = profileEntity.EntityCode
           AND source.Direction = profileEntity.Direction
        WHERE target.IsDeleted = 0;

        INSERT dbo.SapSyncSchedules
        (
            SapSyncProfileEntityId, ScheduleType, IntervalMinutes, ExecutionTime,
            TimeZoneId, PreventConcurrentExecutions, IsActive,
            CreatedByUserId, CreatedByUserName
        )
        SELECT
            profileEntity.Id, source.ScheduleType, source.IntervalMinutes,
            source.ExecutionTime, source.TimeZoneId,
            source.PreventConcurrentExecutions, source.ScheduleIsActive,
            @AuditUserId, @AuditUserName
        FROM @Entities source
        INNER JOIN dbo.SapSyncProfileEntities profileEntity
            ON profileEntity.SapSyncProfileId = @Id
           AND profileEntity.EntityCode = source.EntityCode
           AND profileEntity.Direction = source.Direction
           AND profileEntity.IsDeleted = 0
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM dbo.SapSyncSchedules target
            WHERE target.SapSyncProfileEntityId = profileEntity.Id
              AND target.IsDeleted = 0
        );

        INSERT dbo.AuditSapSyncProfileChanges
        (
            SapSyncProfileId, EntityName, RecordId, Action, SafeDataJson,
            UserId, UserName, Source
        )
        VALUES
        (
            @Id, N'SapSyncProfile', CONVERT(nvarchar(80), @Id), 'Updated',
            (SELECT @CompanyId AS CompanyId, @Code AS Code FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
            @AuditUserId, @AuditUserName, 'API'
        );

        COMMIT;

        SELECT Id, N'Updated' AS ResultCode, RowVersion
        FROM dbo.SapSyncProfiles
        WHERE Id = @Id;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK;
        THROW;
    END CATCH;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.MasterSchemaHistory
    WHERE Version = N'20260730.154'
)
BEGIN
    INSERT dbo.MasterSchemaHistory(Version, Description)
    VALUES
    (
        N'20260730.154',
        N'Hardening de acceso y propiedad de perfiles SAP'
    );
END;
GO
