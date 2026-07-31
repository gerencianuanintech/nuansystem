/*
    Fase 10.4 - Scheduler SAP en NuanSystem_Master.
    Prerrequisitos: 152_master_sap_sync_profiles.sql y
                    154_master_sap_sync_profile_api_hardening.sql.
    Alcance: lectura paginada justa, reserva atomica de agenda y fallback legado
             exclusivamente de lectura. No ejecuta SAP ni modifica settings legado.
*/
USE [NuanSystem_Master];
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SAPSYNCSCHEDULECANDIDATOSPAGINAR
    @UtcNow datetime2(0),
    @PageSize int = 50,
    @AfterCompanyId int = 0,
    @AfterProfileId bigint = 0,
    @AfterExecutionOrder int = -1,
    @AfterEntityId bigint = 0
AS
BEGIN
    SET NOCOUNT ON;

    SET @PageSize = CASE WHEN @PageSize BETWEEN 1 AND 500 THEN @PageSize ELSE 50 END;

    ;WITH Candidates AS
    (
        SELECT
            CAST('Profile' AS varchar(30)) AS CandidateSource,
            profile.CompanyId,
            company.Code AS CompanyCode,
            profile.Id AS ProfileId,
            profile.Code AS ProfileCode,
            profile.Name AS ProfileName,
            profile.IsActive AS ProfileIsActive,
            entity.Id AS ProfileEntityId,
            entity.EntityCode,
            entity.Direction,
            entity.SyncMode,
            entity.BatchSize,
            entity.MaxAttempts,
            entity.ExecutionOrder,
            entity.ContinueOnError,
            entity.ExecutionTimeoutMinutes,
            entity.IsActive AS EntityIsActive,
            schedule.Id AS ScheduleId,
            schedule.ScheduleType,
            schedule.IntervalMinutes,
            schedule.ExecutionTime,
            schedule.TimeZoneId,
            schedule.PreventConcurrentExecutions,
            schedule.NextExecutionAtUtc,
            schedule.LastScheduledAtUtc,
            schedule.LastExecutionAtUtc,
            schedule.IsActive AS ScheduleIsActive,
            CONVERT(varbinary(8), schedule.RowVersion) AS ScheduleRowVersion,
            capability.SupportsSapToErp,
            capability.SupportsErpToSap,
            capability.SupportsFull,
            capability.SupportsIncremental,
            capability.IsImplemented AS CapabilityIsImplemented,
            capability.IsActive AS CapabilityIsActive,
            CAST(0 AS bit) AS LegacyFallbackEnabled,
            CAST(NULL AS nvarchar(40)) AS CompatibilityVersion,
            CAST(0 AS int) AS RequiredSuccessfulCycles,
            profile.Id AS SortProfileId,
            entity.Id AS SortEntityId
        FROM dbo.SapSyncProfiles profile
        INNER JOIN dbo.Companies company
            ON company.Id = profile.CompanyId
        INNER JOIN dbo.SapCompanySettings sapSettings
            ON sapSettings.CompanyId = profile.CompanyId
        INNER JOIN dbo.SapSyncProfileEntities entity
            ON entity.SapSyncProfileId = profile.Id
        INNER JOIN dbo.SapSyncSchedules schedule
            ON schedule.SapSyncProfileEntityId = entity.Id
        INNER JOIN dbo.SapSyncHandlerCapabilities capability
            ON capability.EntityCode = entity.EntityCode
        WHERE profile.IsDeleted = 0
          AND profile.IsActive = 1
          AND entity.IsDeleted = 0
          AND entity.IsActive = 1
          AND schedule.IsDeleted = 0
          AND schedule.IsActive = 1
          AND schedule.ScheduleType <> 'Manual'
          AND (schedule.NextExecutionAtUtc IS NULL OR schedule.NextExecutionAtUtc <= @UtcNow)
          AND company.IsActive = 1
          AND company.SapIntegrationMode <> 0
          AND sapSettings.IsEnabled = 1
          AND sapSettings.IntegrationMode <> 0

        UNION ALL

        SELECT
            CAST('LegacyFallback' AS varchar(30)) AS CandidateSource,
            legacy.CompanyId,
            company.Code AS CompanyCode,
            CAST(NULL AS bigint) AS ProfileId,
            CAST('LEGACY-SAP-SETTINGS' AS nvarchar(80)) AS ProfileCode,
            CAST(N'Compatibilidad SAP legado' AS nvarchar(160)) AS ProfileName,
            CAST(1 AS bit) AS ProfileIsActive,
            CAST(NULL AS bigint) AS ProfileEntityId,
            legacy.EntityCode,
            CONVERT(varchar(20), legacy.Direction) AS Direction,
            CAST('Full' AS varchar(20)) AS SyncMode,
            CASE WHEN legacy.BatchSize BETWEEN 1 AND 10000 THEN legacy.BatchSize ELSE 100 END AS BatchSize,
            CASE
                WHEN legacy.MaxRetryCount < 0 THEN 1
                WHEN legacy.MaxRetryCount >= 19 THEN 20
                ELSE legacy.MaxRetryCount + 1
            END AS MaxAttempts,
            legacy.ExecutionOrder,
            CAST(1 AS bit) AS ContinueOnError,
            CAST(15 AS int) AS ExecutionTimeoutMinutes,
            legacy.IsEnabled AS EntityIsActive,
            CAST(NULL AS bigint) AS ScheduleId,
            CAST('LegacyFallback' AS varchar(20)) AS ScheduleType,
            CAST(NULL AS int) AS IntervalMinutes,
            CAST(NULL AS time(0)) AS ExecutionTime,
            CAST(N'America/Guayaquil' AS nvarchar(100)) AS TimeZoneId,
            CAST(1 AS bit) AS PreventConcurrentExecutions,
            CAST(NULL AS datetime2(0)) AS NextExecutionAtUtc,
            CAST(NULL AS datetime2(0)) AS LastScheduledAtUtc,
            CAST(NULL AS datetime2(0)) AS LastExecutionAtUtc,
            CAST(1 AS bit) AS ScheduleIsActive,
            CAST(NULL AS varbinary(8)) AS ScheduleRowVersion,
            COALESCE(capability.SupportsSapToErp, 0) AS SupportsSapToErp,
            COALESCE(capability.SupportsErpToSap, 0) AS SupportsErpToSap,
            COALESCE(capability.SupportsFull, 0) AS SupportsFull,
            COALESCE(capability.SupportsIncremental, 0) AS SupportsIncremental,
            COALESCE(capability.IsImplemented, 0) AS CapabilityIsImplemented,
            COALESCE(capability.IsActive, 0) AS CapabilityIsActive,
            compatibility.LegacyFallbackEnabled,
            compatibility.CompatibilityVersion,
            compatibility.RequiredSuccessfulCycles,
            CAST(0 AS bigint) AS SortProfileId,
            legacy.Id AS SortEntityId
        FROM dbo.SapSyncEntitySettings legacy
        INNER JOIN dbo.Companies company
            ON company.Id = legacy.CompanyId
        INNER JOIN dbo.SapCompanySettings sapSettings
            ON sapSettings.CompanyId = legacy.CompanyId
        INNER JOIN dbo.SapSyncProfileCompatibilitySettings compatibility
            ON compatibility.CompanyId = legacy.CompanyId
        LEFT JOIN dbo.SapSyncHandlerCapabilities capability
            ON capability.EntityCode = legacy.EntityCode
        WHERE legacy.IsEnabled = 1
          AND compatibility.LegacyFallbackEnabled = 1
          AND company.IsActive = 1
          AND company.SapIntegrationMode <> 0
          AND sapSettings.IsEnabled = 1
          AND sapSettings.IntegrationMode <> 0
          AND NOT EXISTS
              (
                  SELECT 1
                  FROM dbo.SapSyncProfiles currentProfile
                  WHERE currentProfile.CompanyId = legacy.CompanyId
                    AND currentProfile.SourceType = 'Native'
                    AND currentProfile.IsDeleted = 0
              )
    )
    SELECT TOP (@PageSize)
        CandidateSource,
        CompanyId,
        CompanyCode,
        ProfileId,
        ProfileCode,
        ProfileName,
        ProfileIsActive,
        ProfileEntityId,
        EntityCode,
        Direction,
        SyncMode,
        BatchSize,
        MaxAttempts,
        ExecutionOrder,
        ContinueOnError,
        ExecutionTimeoutMinutes,
        EntityIsActive,
        ScheduleId,
        ScheduleType,
        IntervalMinutes,
        ExecutionTime,
        TimeZoneId,
        PreventConcurrentExecutions,
        NextExecutionAtUtc,
        LastScheduledAtUtc,
        LastExecutionAtUtc,
        ScheduleIsActive,
        ScheduleRowVersion,
        SupportsSapToErp,
        SupportsErpToSap,
        SupportsFull,
        SupportsIncremental,
        CapabilityIsImplemented,
        CapabilityIsActive,
        LegacyFallbackEnabled,
        CompatibilityVersion,
        RequiredSuccessfulCycles,
        SortProfileId,
        SortEntityId
    FROM Candidates
    WHERE CompanyId > @AfterCompanyId
       OR
       (
           CompanyId = @AfterCompanyId
           AND
           (
               SortProfileId > @AfterProfileId
               OR
               (
                   SortProfileId = @AfterProfileId
                   AND
                   (
                       ExecutionOrder > @AfterExecutionOrder
                       OR
                       (
                           ExecutionOrder = @AfterExecutionOrder
                           AND SortEntityId > @AfterEntityId
                       )
                   )
               )
           )
       )
    ORDER BY CompanyId, SortProfileId, ExecutionOrder, SortEntityId;

    SELECT COUNT(DISTINCT company.Id)
    FROM dbo.Companies company
    INNER JOIN dbo.SapCompanySettings sapSettings
        ON sapSettings.CompanyId = company.Id
    WHERE company.IsActive = 1
      AND company.SapIntegrationMode <> 0
      AND sapSettings.IsEnabled = 1
      AND sapSettings.IntegrationMode <> 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PATCH_SAPSYNCSCHEDULERESERVAR
    @ScheduleId bigint,
    @ExpectedRowVersion varbinary(8),
    @UtcNow datetime2(0),
    @ObservedNextExecutionAtUtc datetime2(0) = NULL,
    @ScheduledAtUtc datetime2(0) = NULL,
    @NextExecutionAtUtc datetime2(0)
AS
BEGIN
    SET NOCOUNT ON;

    IF @ExpectedRowVersion IS NULL
       OR DATALENGTH(@ExpectedRowVersion) <> 8
       OR @NextExecutionAtUtc <= @UtcNow
       OR
       (
           (@ObservedNextExecutionAtUtc IS NULL AND @ScheduledAtUtc IS NOT NULL)
           OR
           (
               @ObservedNextExecutionAtUtc IS NOT NULL
               AND
               (
                   @ScheduledAtUtc IS NULL
                   OR @ScheduledAtUtc <> @ObservedNextExecutionAtUtc
               )
           )
       )
        THROW 51155, 'Invalid SAP schedule reservation contract.', 1;

    UPDATE schedule
    SET NextExecutionAtUtc = @NextExecutionAtUtc,
        LastScheduledAtUtc = COALESCE(@ScheduledAtUtc, schedule.LastScheduledAtUtc),
        UpdatedByUserName = N'NuanSystem.SyncWorker',
        UpdatedAt = @UtcNow
    FROM dbo.SapSyncSchedules schedule
    INNER JOIN dbo.SapSyncProfileEntities entity
        ON entity.Id = schedule.SapSyncProfileEntityId
    INNER JOIN dbo.SapSyncProfiles profile
        ON profile.Id = entity.SapSyncProfileId
    WHERE schedule.Id = @ScheduleId
      AND schedule.RowVersion = @ExpectedRowVersion
      AND schedule.IsDeleted = 0
      AND schedule.IsActive = 1
      AND schedule.ScheduleType <> 'Manual'
      AND entity.IsDeleted = 0
      AND entity.IsActive = 1
      AND profile.IsDeleted = 0
      AND profile.IsActive = 1
      AND
      (
          (@ObservedNextExecutionAtUtc IS NULL AND schedule.NextExecutionAtUtc IS NULL)
          OR
          (
              @ObservedNextExecutionAtUtc IS NOT NULL
              AND schedule.NextExecutionAtUtc = @ObservedNextExecutionAtUtc
              AND schedule.NextExecutionAtUtc <= @UtcNow
          )
      );

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SAPSYNCENTITYSETTINGSHABILITADOS
    @CompanyId int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        setting.Id,
        setting.CompanyId,
        company.Code AS CompanyCode,
        setting.EntityCode,
        setting.Direction,
        setting.IsEnabled,
        setting.BatchSize,
        setting.MaxRetryCount,
        setting.ExecutionOrder,
        setting.CreatedAt,
        setting.UpdatedAt
    FROM dbo.SapSyncEntitySettings setting
    INNER JOIN dbo.Companies company
        ON company.Id = setting.CompanyId
    WHERE setting.CompanyId = @CompanyId
      AND setting.IsEnabled = 1
    ORDER BY setting.ExecutionOrder, setting.EntityCode, setting.Id;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.MasterSchemaHistory
    WHERE Version = N'20260730.155'
)
BEGIN
    INSERT dbo.MasterSchemaHistory(Version, Description)
    VALUES
    (
        N'20260730.155',
        N'Scheduler SAP por perfil con paginacion justa, reserva atomica y fallback legado de solo lectura'
    );
END;
GO
