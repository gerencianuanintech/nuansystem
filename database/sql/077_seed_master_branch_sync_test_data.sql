/*
    Datos de prueba para configuracion y monitoreo Sync Maestro/Sucursal.

    Ejecutar en NuanSystem_Master.

    Reglas:
    - Idempotente.
    - Usa la empresa maestra activa y la primera sucursal activa con SyncEnabled = 1.
    - No deja ejecuciones Pending/Running para evitar que el worker procese datos de prueba automaticamente.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SyncProfiles', N'U') IS NULL
    THROW 51077, 'No existe dbo.SyncProfiles. Ejecute primero los scripts 069 a 073.', 1;
IF OBJECT_ID(N'dbo.SyncProfileBranches', N'U') IS NULL
    THROW 51077, 'No existe dbo.SyncProfileBranches. Ejecute primero los scripts 069 a 073.', 1;
IF OBJECT_ID(N'dbo.SyncProfileEntities', N'U') IS NULL
    THROW 51077, 'No existe dbo.SyncProfileEntities. Ejecute primero los scripts 069 a 073.', 1;
IF OBJECT_ID(N'dbo.SyncProfileEntityBranches', N'U') IS NULL
    THROW 51077, 'No existe dbo.SyncProfileEntityBranches. Ejecute primero los scripts 069 a 073.', 1;
IF OBJECT_ID(N'dbo.SyncSchedules', N'U') IS NULL
    THROW 51077, 'No existe dbo.SyncSchedules. Ejecute primero los scripts 069 a 073.', 1;
IF OBJECT_ID(N'dbo.SyncProfileExecutions', N'U') IS NULL
    THROW 51077, 'No existe dbo.SyncProfileExecutions. Ejecute primero los scripts 069 a 073.', 1;
IF OBJECT_ID(N'dbo.SyncProfileExecutionDetails', N'U') IS NULL
    THROW 51077, 'No existe dbo.SyncProfileExecutionDetails. Ejecute primero los scripts 069 a 073.', 1;
GO

DECLARE @MasterCompanyId int =
(
    SELECT TOP (1) Id
    FROM dbo.Companies
    WHERE IsMaster = 1
      AND SyncEnabled = 1
      AND IsActive = 1
      AND IsDeleted = 0
    ORDER BY IsDefault DESC, Id
);

IF @MasterCompanyId IS NULL
    THROW 51077, 'No existe una empresa maestra activa con SyncEnabled = 1.', 1;

DECLARE @BranchCompanyId int =
(
    SELECT TOP (1) Id
    FROM dbo.Companies
    WHERE IsMaster = 0
      AND ParentCompanyId = @MasterCompanyId
      AND SyncEnabled = 1
      AND IsActive = 1
      AND IsDeleted = 0
    ORDER BY Id
);

IF @BranchCompanyId IS NULL
    THROW 51077, 'No existe una sucursal activa con SyncEnabled = 1 para la empresa maestra.', 1;

DECLARE @SeedUser nvarchar(120) = N'Seed Sync Test';
DECLARE @ProfileCode nvarchar(50) = N'TEST-MB-CATALOGS-FULL';
DECLARE @ProfileId int;
DECLARE @BranchProfileId int;

BEGIN TRANSACTION;

MERGE dbo.SyncProfiles AS target
USING
(
    SELECT
        @MasterCompanyId AS CompanyId,
        @ProfileCode AS Code,
        N'Prueba catalogos maestro a sucursal' AS Name,
        N'Perfil de prueba para validar configuracion, ejecuciones y monitoreo Sync Maestro/Sucursal.' AS Description,
        N'MasterToBranch' AS Direction,
        N'Full' AS ExecutionMode,
        N'MasterWins' AS ConflictStrategy,
        100 AS BatchSize,
        2 AS MaxRetries,
        30 AS RetryDelaySeconds,
        15 AS TimeoutMinutes,
        1 AS IsActive
) AS source
ON target.CompanyId = source.CompanyId
   AND target.Code = source.Code
   AND target.IsDeleted = 0
WHEN MATCHED THEN
    UPDATE SET
        Name = source.Name,
        Description = source.Description,
        Direction = source.Direction,
        ExecutionMode = source.ExecutionMode,
        ConflictStrategy = source.ConflictStrategy,
        BatchSize = source.BatchSize,
        MaxRetries = source.MaxRetries,
        RetryDelaySeconds = source.RetryDelaySeconds,
        TimeoutMinutes = source.TimeoutMinutes,
        IsActive = source.IsActive,
        UpdatedByUserName = @SeedUser,
        UpdatedAt = SYSUTCDATETIME()
WHEN NOT MATCHED THEN
    INSERT
    (
        CompanyId, Code, Name, Description, Direction, ExecutionMode, ConflictStrategy,
        BatchSize, MaxRetries, RetryDelaySeconds, TimeoutMinutes, IsActive,
        CreatedByUserName, CreatedAt, IsDeleted
    )
    VALUES
    (
        source.CompanyId, source.Code, source.Name, source.Description, source.Direction, source.ExecutionMode, source.ConflictStrategy,
        source.BatchSize, source.MaxRetries, source.RetryDelaySeconds, source.TimeoutMinutes, source.IsActive,
        @SeedUser, SYSUTCDATETIME(), 0
    );

SET @ProfileId =
(
    SELECT Id
    FROM dbo.SyncProfiles
    WHERE CompanyId = @MasterCompanyId
      AND Code = @ProfileCode
      AND IsDeleted = 0
);

MERGE dbo.SyncProfileBranches AS target
USING
(
    SELECT @ProfileId AS SyncProfileId, @BranchCompanyId AS BranchCompanyId, 100 AS BatchSize, 2 AS MaxRetries, 1 AS IsActive
) AS source
ON target.SyncProfileId = source.SyncProfileId
   AND target.BranchCompanyId = source.BranchCompanyId
   AND target.IsDeleted = 0
WHEN MATCHED THEN
    UPDATE SET
        BatchSize = source.BatchSize,
        MaxRetries = source.MaxRetries,
        IsActive = source.IsActive,
        UpdatedByUserName = @SeedUser,
        UpdatedAt = SYSUTCDATETIME()
WHEN NOT MATCHED THEN
    INSERT
    (
        SyncProfileId, BranchCompanyId, BatchSize, MaxRetries, IsActive,
        CreatedByUserName, CreatedAt, IsDeleted
    )
    VALUES
    (
        source.SyncProfileId, source.BranchCompanyId, source.BatchSize, source.MaxRetries, source.IsActive,
        @SeedUser, SYSUTCDATETIME(), 0
    );

SET @BranchProfileId =
(
    SELECT Id
    FROM dbo.SyncProfileBranches
    WHERE SyncProfileId = @ProfileId
      AND BranchCompanyId = @BranchCompanyId
      AND IsDeleted = 0
);

DECLARE @Entities table
(
    EntityCode nvarchar(80) NOT NULL PRIMARY KEY,
    EntityName nvarchar(120) NOT NULL,
    ExecutionOrder int NOT NULL,
    KeyField nvarchar(100) NULL,
    ModifiedAtField nvarchar(100) NULL,
    ActiveField nvarchar(100) NULL,
    BatchSize int NULL,
    ContinueOnError bit NOT NULL
);

INSERT INTO @Entities
(
    EntityCode, EntityName, ExecutionOrder, KeyField, ModifiedAtField, ActiveField, BatchSize, ContinueOnError
)
VALUES
    (N'Countries', N'Paises', 10, N'Code', N'UpdatedAt', N'IsActive', 100, 0),
    (N'Provinces', N'Provincias', 20, N'Code', N'UpdatedAt', N'IsActive', 100, 0),
    (N'Cities', N'Ciudades', 30, N'Code', N'UpdatedAt', N'IsActive', 100, 1),
    (N'Currencies', N'Monedas', 40, N'Code', N'UpdatedAt', N'IsActive', 50, 0),
    (N'SupplierGroups', N'Grupos de proveedores', 50, N'Code', N'UpdatedAt', N'IsActive', 50, 1);

MERGE dbo.SyncProfileEntities AS target
USING
(
    SELECT
        @ProfileId AS SyncProfileId,
        EntityCode,
        EntityName,
        ExecutionOrder,
        N'Full' AS SyncMode,
        KeyField,
        ModifiedAtField,
        CAST(NULL AS nvarchar(100)) AS VersionField,
        ActiveField,
        CAST(1 AS bit) AS AllowInsert,
        CAST(1 AS bit) AS AllowUpdate,
        CAST(1 AS bit) AS AllowDeactivate,
        ContinueOnError,
        BatchSize,
        CAST(1 AS bit) AS IsActive
    FROM @Entities
) AS source
ON target.SyncProfileId = source.SyncProfileId
   AND target.EntityCode = source.EntityCode
   AND target.IsDeleted = 0
WHEN MATCHED THEN
    UPDATE SET
        EntityName = source.EntityName,
        ExecutionOrder = source.ExecutionOrder,
        SyncMode = source.SyncMode,
        KeyField = source.KeyField,
        ModifiedAtField = source.ModifiedAtField,
        VersionField = source.VersionField,
        ActiveField = source.ActiveField,
        AllowInsert = source.AllowInsert,
        AllowUpdate = source.AllowUpdate,
        AllowDeactivate = source.AllowDeactivate,
        ContinueOnError = source.ContinueOnError,
        BatchSize = source.BatchSize,
        IsActive = source.IsActive,
        UpdatedByUserName = @SeedUser,
        UpdatedAt = SYSUTCDATETIME()
WHEN NOT MATCHED THEN
    INSERT
    (
        SyncProfileId, EntityCode, EntityName, ExecutionOrder, SyncMode, KeyField,
        ModifiedAtField, VersionField, ActiveField, AllowInsert, AllowUpdate,
        AllowDeactivate, ContinueOnError, BatchSize, IsActive,
        CreatedByUserName, CreatedAt, IsDeleted
    )
    VALUES
    (
        source.SyncProfileId, source.EntityCode, source.EntityName, source.ExecutionOrder, source.SyncMode, source.KeyField,
        source.ModifiedAtField, source.VersionField, source.ActiveField, source.AllowInsert, source.AllowUpdate,
        source.AllowDeactivate, source.ContinueOnError, source.BatchSize, source.IsActive,
        @SeedUser, SYSUTCDATETIME(), 0
    );

MERGE dbo.SyncProfileEntityBranches AS target
USING
(
    SELECT
        @ProfileId AS SyncProfileId,
        entity.Id AS SyncProfileEntityId,
        @BranchProfileId AS SyncProfileBranchId,
        CAST(1 AS bit) AS IsEnabled,
        entity.BatchSize
    FROM dbo.SyncProfileEntities entity
    WHERE entity.SyncProfileId = @ProfileId
      AND entity.EntityCode IN (SELECT EntityCode FROM @Entities)
      AND entity.IsDeleted = 0
) AS source
ON target.SyncProfileId = source.SyncProfileId
   AND target.SyncProfileEntityId = source.SyncProfileEntityId
   AND target.SyncProfileBranchId = source.SyncProfileBranchId
   AND target.IsDeleted = 0
WHEN MATCHED THEN
    UPDATE SET
        IsEnabled = source.IsEnabled,
        BatchSize = source.BatchSize,
        UpdatedByUserName = @SeedUser,
        UpdatedAt = SYSUTCDATETIME()
WHEN NOT MATCHED THEN
    INSERT
    (
        SyncProfileId, SyncProfileEntityId, SyncProfileBranchId, IsEnabled, BatchSize,
        CreatedByUserName, CreatedAt, IsDeleted
    )
    VALUES
    (
        source.SyncProfileId, source.SyncProfileEntityId, source.SyncProfileBranchId, source.IsEnabled, source.BatchSize,
        @SeedUser, SYSUTCDATETIME(), 0
    );

MERGE dbo.SyncSchedules AS target
USING
(
    SELECT
        @ProfileId AS SyncProfileId,
        N'Interval' AS ScheduleType,
        60 AS IntervalMinutes,
        CAST(NULL AS time(0)) AS ExecutionTime,
        N'America/Guayaquil' AS TimeZoneId,
        CAST(1 AS bit) AS PreventConcurrentExecutions,
        CAST(1 AS bit) AS IsActive,
        DATEADD(day, -1, SYSUTCDATETIME()) AS LastSuccessfulScheduledExecutionAt,
        DATEADD(hour, 1, SYSUTCDATETIME()) AS NextExecutionAt
) AS source
ON target.SyncProfileId = source.SyncProfileId
   AND target.IsDeleted = 0
WHEN MATCHED THEN
    UPDATE SET
        ScheduleType = source.ScheduleType,
        IntervalMinutes = source.IntervalMinutes,
        ExecutionTime = source.ExecutionTime,
        TimeZoneId = source.TimeZoneId,
        PreventConcurrentExecutions = source.PreventConcurrentExecutions,
        IsActive = source.IsActive,
        LastSuccessfulScheduledExecutionAt = source.LastSuccessfulScheduledExecutionAt,
        NextExecutionAt = source.NextExecutionAt,
        UpdatedByUserName = @SeedUser,
        UpdatedAt = SYSUTCDATETIME()
WHEN NOT MATCHED THEN
    INSERT
    (
        SyncProfileId, ScheduleType, IntervalMinutes, ExecutionTime, TimeZoneId,
        PreventConcurrentExecutions, IsActive, LastSuccessfulScheduledExecutionAt, NextExecutionAt,
        CreatedByUserName, CreatedAt, IsDeleted
    )
    VALUES
    (
        source.SyncProfileId, source.ScheduleType, source.IntervalMinutes, source.ExecutionTime, source.TimeZoneId,
        source.PreventConcurrentExecutions, source.IsActive, source.LastSuccessfulScheduledExecutionAt, source.NextExecutionAt,
        @SeedUser, SYSUTCDATETIME(), 0
    );

DECLARE @Executions table
(
    CorrelationId nvarchar(100) NOT NULL PRIMARY KEY,
    ExecutionType nvarchar(20) NOT NULL,
    Status nvarchar(30) NOT NULL,
    RequestedBy nvarchar(120) NULL,
    RequestedAt datetime2(0) NOT NULL,
    StartedAt datetime2(0) NULL,
    FinishedAt datetime2(0) NULL,
    CancelledAt datetime2(0) NULL,
    CancelledBy nvarchar(120) NULL,
    TotalRecordsRead int NOT NULL,
    TotalEventsPublished int NOT NULL,
    TotalSkipped int NOT NULL,
    TotalErrors int NOT NULL,
    Message nvarchar(1000) NULL
);

INSERT INTO @Executions
(
    CorrelationId, ExecutionType, Status, RequestedBy, RequestedAt, StartedAt, FinishedAt,
    CancelledAt, CancelledBy, TotalRecordsRead, TotalEventsPublished, TotalSkipped, TotalErrors, Message
)
VALUES
    (N'TEST-MB-CATALOGS-FULL-001', N'Manual', N'Completed', @SeedUser, DATEADD(hour, -8, SYSUTCDATETIME()), DATEADD(hour, -8, DATEADD(minute, 1, SYSUTCDATETIME())), DATEADD(hour, -8, DATEADD(minute, 5, SYSUTCDATETIME())), NULL, NULL, 245, 245, 0, 0, N'Ejecucion manual completada para datos de prueba.'),
    (N'TEST-MB-CATALOGS-FULL-002', N'Scheduled', N'CompletedWithErrors', N'Scheduler', DATEADD(hour, -5, SYSUTCDATETIME()), DATEADD(hour, -5, DATEADD(minute, 1, SYSUTCDATETIME())), DATEADD(hour, -5, DATEADD(minute, 6, SYSUTCDATETIME())), NULL, NULL, 221, 218, 1, 2, N'Ejecucion con errores controlados para validar monitoreo.'),
    (N'TEST-MB-CATALOGS-FULL-003', N'Manual', N'Failed', @SeedUser, DATEADD(hour, -3, SYSUTCDATETIME()), DATEADD(hour, -3, DATEADD(minute, 1, SYSUTCDATETIME())), DATEADD(hour, -3, DATEADD(minute, 2, SYSUTCDATETIME())), NULL, NULL, 35, 30, 0, 5, N'Fallo simulado: sucursal temporalmente no disponible.'),
    (N'TEST-MB-CATALOGS-FULL-004', N'Manual', N'Cancelled', @SeedUser, DATEADD(hour, -2, SYSUTCDATETIME()), DATEADD(hour, -2, DATEADD(minute, 1, SYSUTCDATETIME())), DATEADD(hour, -2, DATEADD(minute, 3, SYSUTCDATETIME())), DATEADD(hour, -2, DATEADD(minute, 3, SYSUTCDATETIME())), @SeedUser, 80, 80, 0, 0, N'Ejecucion cancelada por usuario para validar estado.'),
    (N'TEST-MB-CATALOGS-FULL-005', N'Retry', N'Completed', @SeedUser, DATEADD(hour, -1, SYSUTCDATETIME()), DATEADD(hour, -1, DATEADD(minute, 1, SYSUTCDATETIME())), DATEADD(hour, -1, DATEADD(minute, 4, SYSUTCDATETIME())), NULL, NULL, 35, 35, 0, 0, N'Reintento completado despues de una falla simulada.');

MERGE dbo.SyncProfileExecutions AS target
USING
(
    SELECT
        @ProfileId AS SyncProfileId,
        ExecutionType,
        Status,
        CorrelationId,
        RequestedBy,
        RequestedAt,
        StartedAt,
        FinishedAt,
        CancelledAt,
        CancelledBy,
        (
            SELECT COUNT(1)
            FROM dbo.SyncProfileEntities entity
            WHERE entity.SyncProfileId = @ProfileId
              AND entity.EntityCode IN (SELECT EntityCode FROM @Entities)
              AND entity.IsDeleted = 0
        ) AS TotalEntities,
        TotalRecordsRead,
        TotalEventsPublished,
        TotalSkipped,
        TotalErrors,
        Message
    FROM @Executions
) AS source
ON target.CorrelationId = source.CorrelationId
WHEN MATCHED THEN
    UPDATE SET
        SyncProfileId = source.SyncProfileId,
        ExecutionType = source.ExecutionType,
        Status = source.Status,
        RequestedBy = source.RequestedBy,
        RequestedAt = source.RequestedAt,
        StartedAt = source.StartedAt,
        FinishedAt = source.FinishedAt,
        CancelledAt = source.CancelledAt,
        CancelledBy = source.CancelledBy,
        TotalEntities = source.TotalEntities,
        TotalRecordsRead = source.TotalRecordsRead,
        TotalEventsPublished = source.TotalEventsPublished,
        TotalSkipped = source.TotalSkipped,
        TotalErrors = source.TotalErrors,
        Message = source.Message,
        UpdatedAt = SYSUTCDATETIME()
WHEN NOT MATCHED THEN
    INSERT
    (
        SyncProfileId, ExecutionType, Status, CorrelationId, RequestedBy, RequestedAt,
        StartedAt, FinishedAt, CancelledAt, CancelledBy, EntityCodesJson, FromKey, MaxRecords,
        TotalEntities, TotalRecordsRead, TotalEventsPublished, TotalSkipped, TotalErrors,
        Message, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        source.SyncProfileId, source.ExecutionType, source.Status, source.CorrelationId, source.RequestedBy, source.RequestedAt,
        source.StartedAt, source.FinishedAt, source.CancelledAt, source.CancelledBy, NULL, NULL, NULL,
        source.TotalEntities, source.TotalRecordsRead, source.TotalEventsPublished, source.TotalSkipped, source.TotalErrors,
        source.Message, @SeedUser, SYSUTCDATETIME()
    );

DECLARE @ExecutionDetails table
(
    CorrelationId nvarchar(100) NOT NULL,
    EntityCode nvarchar(80) NOT NULL,
    Status nvarchar(30) NOT NULL,
    TotalRecordsRead int NOT NULL,
    TotalEventsPublished int NOT NULL,
    TotalSkipped int NOT NULL,
    TotalErrors int NOT NULL,
    LastProcessedKey nvarchar(200) NULL,
    Message nvarchar(1000) NULL,
    PRIMARY KEY (CorrelationId, EntityCode)
);

INSERT INTO @ExecutionDetails
(
    CorrelationId, EntityCode, Status, TotalRecordsRead, TotalEventsPublished, TotalSkipped, TotalErrors, LastProcessedKey, Message
)
VALUES
    (N'TEST-MB-CATALOGS-FULL-001', N'Countries', N'Completed', 25, 25, 0, 0, N'COUNTRY-025', N'Paises sincronizados.'),
    (N'TEST-MB-CATALOGS-FULL-001', N'Provinces', N'Completed', 80, 80, 0, 0, N'PROVINCE-080', N'Provincias sincronizadas.'),
    (N'TEST-MB-CATALOGS-FULL-001', N'Cities', N'Completed', 100, 100, 0, 0, N'CITY-100', N'Ciudades sincronizadas.'),
    (N'TEST-MB-CATALOGS-FULL-001', N'Currencies', N'Completed', 10, 10, 0, 0, N'USD', N'Monedas sincronizadas.'),
    (N'TEST-MB-CATALOGS-FULL-001', N'SupplierGroups', N'Completed', 30, 30, 0, 0, N'SUP-GRP-030', N'Grupos sincronizados.'),
    (N'TEST-MB-CATALOGS-FULL-002', N'Countries', N'Completed', 25, 25, 0, 0, N'COUNTRY-025', NULL),
    (N'TEST-MB-CATALOGS-FULL-002', N'Provinces', N'Completed', 80, 80, 0, 0, N'PROVINCE-080', NULL),
    (N'TEST-MB-CATALOGS-FULL-002', N'Cities', N'CompletedWithErrors', 100, 98, 1, 1, N'CITY-100', N'Una ciudad omitida por validacion.'),
    (N'TEST-MB-CATALOGS-FULL-002', N'Currencies', N'Completed', 10, 10, 0, 0, N'USD', NULL),
    (N'TEST-MB-CATALOGS-FULL-002', N'SupplierGroups', N'Failed', 6, 5, 0, 1, N'SUP-GRP-006', N'Fallo simulado en grupo de proveedor.'),
    (N'TEST-MB-CATALOGS-FULL-003', N'Countries', N'Completed', 25, 25, 0, 0, N'COUNTRY-025', NULL),
    (N'TEST-MB-CATALOGS-FULL-003', N'Provinces', N'Failed', 10, 5, 0, 5, N'PROVINCE-010', N'Fallo simulado de conexion.'),
    (N'TEST-MB-CATALOGS-FULL-004', N'Countries', N'Completed', 25, 25, 0, 0, N'COUNTRY-025', NULL),
    (N'TEST-MB-CATALOGS-FULL-004', N'Provinces', N'Cancelled', 55, 55, 0, 0, N'PROVINCE-055', N'Cancelado por usuario.'),
    (N'TEST-MB-CATALOGS-FULL-005', N'Provinces', N'Completed', 35, 35, 0, 0, N'PROVINCE-035', N'Reintento completado.');

MERGE dbo.SyncProfileExecutionDetails AS target
USING
(
    SELECT
        execution.Id AS SyncProfileExecutionId,
        entity.Id AS SyncProfileEntityId,
        source.EntityCode,
        source.Status,
        DATEADD(minute, 1, execution.RequestedAt) AS StartedAt,
        DATEADD(minute, 3, execution.RequestedAt) AS FinishedAt,
        source.TotalRecordsRead,
        source.TotalEventsPublished,
        source.TotalSkipped,
        source.TotalErrors,
        source.LastProcessedKey,
        source.Message
    FROM @ExecutionDetails source
    INNER JOIN dbo.SyncProfileExecutions execution
        ON execution.CorrelationId = source.CorrelationId
    INNER JOIN dbo.SyncProfileEntities entity
        ON entity.SyncProfileId = execution.SyncProfileId
       AND entity.EntityCode = source.EntityCode
       AND entity.IsDeleted = 0
) AS source
ON target.SyncProfileExecutionId = source.SyncProfileExecutionId
   AND target.SyncProfileEntityId = source.SyncProfileEntityId
WHEN MATCHED THEN
    UPDATE SET
        EntityCode = source.EntityCode,
        Status = source.Status,
        StartedAt = source.StartedAt,
        FinishedAt = source.FinishedAt,
        TotalRecordsRead = source.TotalRecordsRead,
        TotalEventsPublished = source.TotalEventsPublished,
        TotalSkipped = source.TotalSkipped,
        TotalErrors = source.TotalErrors,
        LastProcessedKey = source.LastProcessedKey,
        Message = source.Message,
        UpdatedAt = SYSUTCDATETIME()
WHEN NOT MATCHED THEN
    INSERT
    (
        SyncProfileExecutionId, SyncProfileEntityId, EntityCode, Status,
        StartedAt, FinishedAt, TotalRecordsRead, TotalEventsPublished, TotalSkipped,
        TotalErrors, LastProcessedKey, Message, CreatedAt
    )
    VALUES
    (
        source.SyncProfileExecutionId, source.SyncProfileEntityId, source.EntityCode, source.Status,
        source.StartedAt, source.FinishedAt, source.TotalRecordsRead, source.TotalEventsPublished, source.TotalSkipped,
        source.TotalErrors, source.LastProcessedKey, source.Message, SYSUTCDATETIME()
    );

COMMIT TRANSACTION;

SELECT
    @ProfileId AS SyncProfileId,
    @ProfileCode AS SyncProfileCode,
    @MasterCompanyId AS MasterCompanyId,
    @BranchCompanyId AS BranchCompanyId,
    (SELECT COUNT(1) FROM dbo.SyncProfileEntities WHERE SyncProfileId = @ProfileId AND IsDeleted = 0) AS EntityCount,
    (SELECT COUNT(1) FROM dbo.SyncProfileExecutions WHERE SyncProfileId = @ProfileId) AS ExecutionCount;
GO
