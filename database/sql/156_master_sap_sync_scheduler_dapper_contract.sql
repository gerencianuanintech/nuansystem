/*
    Fase 10.4 - Reparacion forward-only del contrato Dapper del scheduler SAP.
    Prerrequisito: 155_master_sap_sync_scheduler.sql.
    Alcance: homogeneiza exclusivamente el result set de candidatos; no modifica datos.
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
            CAST(profile.CompanyId AS int) AS CompanyId,
            CAST(company.Code AS nvarchar(50)) AS CompanyCode,
            CAST(profile.Id AS bigint) AS ProfileId,
            CAST(profile.Code AS nvarchar(80)) AS ProfileCode,
            CAST(profile.Name AS nvarchar(160)) AS ProfileName,
            CAST(profile.IsActive AS bit) AS ProfileIsActive,
            CAST(entity.Id AS bigint) AS ProfileEntityId,
            CAST(entity.EntityCode AS nvarchar(80)) AS EntityCode,
            CAST(entity.Direction AS varchar(20)) AS Direction,
            CAST(entity.SyncMode AS varchar(20)) AS SyncMode,
            CAST(entity.BatchSize AS int) AS BatchSize,
            CAST(entity.MaxAttempts AS int) AS MaxAttempts,
            CAST(entity.ExecutionOrder AS int) AS ExecutionOrder,
            CAST(entity.ContinueOnError AS bit) AS ContinueOnError,
            CAST(entity.ExecutionTimeoutMinutes AS int) AS ExecutionTimeoutMinutes,
            CAST(entity.IsActive AS bit) AS EntityIsActive,
            CAST(schedule.Id AS bigint) AS ScheduleId,
            CAST(schedule.ScheduleType AS varchar(20)) AS ScheduleType,
            CAST(schedule.IntervalMinutes AS int) AS IntervalMinutes,
            CAST(schedule.ExecutionTime AS time(0)) AS ExecutionTime,
            CAST(schedule.TimeZoneId AS nvarchar(100)) AS TimeZoneId,
            CAST(schedule.PreventConcurrentExecutions AS bit) AS PreventConcurrentExecutions,
            CAST(schedule.NextExecutionAtUtc AS datetime2(0)) AS NextExecutionAtUtc,
            CAST(schedule.LastScheduledAtUtc AS datetime2(0)) AS LastScheduledAtUtc,
            CAST(schedule.LastExecutionAtUtc AS datetime2(0)) AS LastExecutionAtUtc,
            CAST(schedule.IsActive AS bit) AS ScheduleIsActive,
            CAST(CONVERT(varbinary(8), schedule.RowVersion) AS varbinary(8)) AS ScheduleRowVersion,
            CAST(capability.SupportsSapToErp AS bit) AS SupportsSapToErp,
            CAST(capability.SupportsErpToSap AS bit) AS SupportsErpToSap,
            CAST(capability.SupportsFull AS bit) AS SupportsFull,
            CAST(capability.SupportsIncremental AS bit) AS SupportsIncremental,
            CAST(capability.IsImplemented AS bit) AS CapabilityIsImplemented,
            CAST(capability.IsActive AS bit) AS CapabilityIsActive,
            CAST(0 AS bit) AS LegacyFallbackEnabled,
            CAST(NULL AS nvarchar(40)) AS CompatibilityVersion,
            CAST(0 AS int) AS RequiredSuccessfulCycles,
            CAST(profile.Id AS bigint) AS SortProfileId,
            CAST(entity.Id AS bigint) AS SortEntityId
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
            CAST(legacy.CompanyId AS int) AS CompanyId,
            CAST(company.Code AS nvarchar(50)) AS CompanyCode,
            CAST(NULL AS bigint) AS ProfileId,
            CAST('LEGACY-SAP-SETTINGS' AS nvarchar(80)) AS ProfileCode,
            CAST(N'Compatibilidad SAP legado' AS nvarchar(160)) AS ProfileName,
            CAST(1 AS bit) AS ProfileIsActive,
            CAST(NULL AS bigint) AS ProfileEntityId,
            CAST(legacy.EntityCode AS nvarchar(80)) AS EntityCode,
            CAST(legacy.Direction AS varchar(20)) AS Direction,
            CAST('Full' AS varchar(20)) AS SyncMode,
            CAST(
                CASE WHEN legacy.BatchSize BETWEEN 1 AND 10000 THEN legacy.BatchSize ELSE 100 END
                AS int) AS BatchSize,
            CAST(
                CASE
                    WHEN legacy.MaxRetryCount < 0 THEN 1
                    WHEN legacy.MaxRetryCount >= 19 THEN 20
                    ELSE legacy.MaxRetryCount + 1
                END
                AS int) AS MaxAttempts,
            CAST(legacy.ExecutionOrder AS int) AS ExecutionOrder,
            CAST(1 AS bit) AS ContinueOnError,
            CAST(15 AS int) AS ExecutionTimeoutMinutes,
            CAST(legacy.IsEnabled AS bit) AS EntityIsActive,
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
            COALESCE(CAST(capability.SupportsSapToErp AS bit), CAST(0 AS bit)) AS SupportsSapToErp,
            COALESCE(CAST(capability.SupportsErpToSap AS bit), CAST(0 AS bit)) AS SupportsErpToSap,
            COALESCE(CAST(capability.SupportsFull AS bit), CAST(0 AS bit)) AS SupportsFull,
            COALESCE(CAST(capability.SupportsIncremental AS bit), CAST(0 AS bit)) AS SupportsIncremental,
            COALESCE(CAST(capability.IsImplemented AS bit), CAST(0 AS bit)) AS CapabilityIsImplemented,
            COALESCE(CAST(capability.IsActive AS bit), CAST(0 AS bit)) AS CapabilityIsActive,
            CAST(compatibility.LegacyFallbackEnabled AS bit) AS LegacyFallbackEnabled,
            CAST(compatibility.CompatibilityVersion AS nvarchar(40)) AS CompatibilityVersion,
            CAST(compatibility.RequiredSuccessfulCycles AS int) AS RequiredSuccessfulCycles,
            CAST(0 AS bigint) AS SortProfileId,
            CAST(legacy.Id AS bigint) AS SortEntityId
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

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.MasterSchemaHistory
    WHERE Version = N'20260731.156'
)
BEGIN
    INSERT dbo.MasterSchemaHistory(Version, Description)
    VALUES
    (
        N'20260731.156',
        N'Repara tipos del contrato Dapper de candidatos del scheduler SAP'
    );
END;
GO
