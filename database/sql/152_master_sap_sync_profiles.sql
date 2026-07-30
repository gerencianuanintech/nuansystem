/*
    Migracion 152 - Perfiles SAP independientes en NuanSystem_Master.

    Objetos:
      - catalogo verificable de handlers/capacidades SAP;
      - SapSyncProfiles, SapSyncProfileEntities y SapSyncSchedules;
      - auditoria y compatibilidad temporal con SapSyncEntitySettings;
      - permisos/operaciones SAP propios, concedidos solo a ADMIN;
      - migracion legado inactiva, Manual, idempotente y sin dual-write.

    No crea formularios ni menus, no contiene secretos y no ejecuta SAP.
    SapSyncEntitySettings se conserva sin cambios como fallback de solo lectura.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.Companies', N'U') IS NULL
    THROW 51152, 'Companies is required before migration 152.', 1;
IF OBJECT_ID(N'dbo.SapCompanySettings', N'U') IS NULL
    THROW 51152, 'SapCompanySettings is required before migration 152.', 1;
IF OBJECT_ID(N'dbo.SapSyncEntitySettings', N'U') IS NULL
    THROW 51152, 'SapSyncEntitySettings is required before migration 152.', 1;
IF OBJECT_ID(N'dbo.Modules', N'U') IS NULL
    THROW 51152, 'Modules is required before migration 152.', 1;
IF OBJECT_ID(N'dbo.Permissions', N'U') IS NULL
    THROW 51152, 'Permissions is required before migration 152.', 1;
IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
    THROW 51152, 'Roles is required before migration 152.', 1;
IF OBJECT_ID(N'dbo.RolePermissions', N'U') IS NULL
    THROW 51152, 'RolePermissions is required before migration 152.', 1;
IF OBJECT_ID(N'dbo.SecurityOperations', N'U') IS NULL
    THROW 51152, 'SecurityOperations is required before migration 152.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL
    THROW 51152, 'MasterSchemaHistory is required before migration 152.', 1;
GO

IF EXISTS
(
    SELECT 1
    FROM dbo.SapSyncEntitySettings
    WHERE Direction NOT IN (N'SapToErp', N'ErpToSap', N'Both')
)
    THROW 51152, 'SapSyncEntitySettings contains an unsupported Direction.', 1;
GO

IF OBJECT_ID(N'dbo.SapSyncHandlerCapabilities', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SapSyncHandlerCapabilities
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SapSyncHandlerCapabilities PRIMARY KEY,
        EntityCode nvarchar(80) NOT NULL,
        DisplayName nvarchar(160) NOT NULL,
        SupportsSapToErp bit NOT NULL,
        SupportsErpToSap bit NOT NULL,
        SupportsFull bit NOT NULL,
        SupportsIncremental bit NOT NULL,
        IsImplemented bit NOT NULL,
        IsActive bit NOT NULL CONSTRAINT DF_SapSyncHandlerCapabilities_IsActive DEFAULT 1,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SapSyncHandlerCapabilities_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        RowVersion rowversion NOT NULL,
        CONSTRAINT UQ_SapSyncHandlerCapabilities_EntityCode UNIQUE (EntityCode),
        CONSTRAINT CK_SapSyncHandlerCapabilities_EntityCode_NotBlank CHECK (LEN(LTRIM(RTRIM(EntityCode))) > 0),
        CONSTRAINT CK_SapSyncHandlerCapabilities_DisplayName_NotBlank CHECK (LEN(LTRIM(RTRIM(DisplayName))) > 0)
    );
END;
GO

IF OBJECT_ID(N'dbo.SapSyncProfiles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SapSyncProfiles
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SapSyncProfiles PRIMARY KEY,
        CompanyId int NOT NULL,
        Code nvarchar(80) NOT NULL,
        Name nvarchar(160) NOT NULL,
        Description nvarchar(500) NULL,
        SourceType varchar(30) NOT NULL CONSTRAINT DF_SapSyncProfiles_SourceType DEFAULT 'Native',
        IsActive bit NOT NULL CONSTRAINT DF_SapSyncProfiles_IsActive DEFAULT 0,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SapSyncProfiles_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SapSyncProfiles_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        RowVersion rowversion NOT NULL,
        CONSTRAINT FK_SapSyncProfiles_Companies FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(Id),
        CONSTRAINT CK_SapSyncProfiles_Code_NotBlank CHECK (LEN(LTRIM(RTRIM(Code))) > 0),
        CONSTRAINT CK_SapSyncProfiles_Name_NotBlank CHECK (LEN(LTRIM(RTRIM(Name))) > 0),
        CONSTRAINT CK_SapSyncProfiles_SourceType CHECK (SourceType IN ('Native', 'LegacyMigration'))
    );

    CREATE UNIQUE INDEX UX_SapSyncProfiles_Company_Code_Current
        ON dbo.SapSyncProfiles(CompanyId, Code)
        WHERE IsDeleted = 0;
    CREATE INDEX IX_SapSyncProfiles_Company_Active
        ON dbo.SapSyncProfiles(CompanyId, IsActive, Id)
        WHERE IsDeleted = 0;
END;
GO

IF OBJECT_ID(N'dbo.SapSyncProfileEntities', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SapSyncProfileEntities
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SapSyncProfileEntities PRIMARY KEY,
        SapSyncProfileId bigint NOT NULL,
        EntityCode nvarchar(80) NOT NULL,
        Direction varchar(20) NOT NULL,
        SyncMode varchar(20) NOT NULL,
        BatchSize int NOT NULL,
        MaxAttempts int NOT NULL,
        ExecutionOrder int NOT NULL,
        ContinueOnError bit NOT NULL CONSTRAINT DF_SapSyncProfileEntities_ContinueOnError DEFAULT 1,
        ExecutionTimeoutMinutes int NOT NULL,
        IsActive bit NOT NULL CONSTRAINT DF_SapSyncProfileEntities_IsActive DEFAULT 0,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SapSyncProfileEntities_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SapSyncProfileEntities_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        RowVersion rowversion NOT NULL,
        CONSTRAINT FK_SapSyncProfileEntities_Profile FOREIGN KEY (SapSyncProfileId) REFERENCES dbo.SapSyncProfiles(Id),
        CONSTRAINT FK_SapSyncProfileEntities_Capability FOREIGN KEY (EntityCode) REFERENCES dbo.SapSyncHandlerCapabilities(EntityCode),
        CONSTRAINT CK_SapSyncProfileEntities_EntityCode_NotBlank CHECK (LEN(LTRIM(RTRIM(EntityCode))) > 0),
        CONSTRAINT CK_SapSyncProfileEntities_Direction CHECK (Direction IN ('SapToErp', 'ErpToSap', 'Both')),
        CONSTRAINT CK_SapSyncProfileEntities_SyncMode CHECK (SyncMode IN ('Full', 'Incremental')),
        CONSTRAINT CK_SapSyncProfileEntities_BatchSize CHECK (BatchSize BETWEEN 1 AND 10000),
        CONSTRAINT CK_SapSyncProfileEntities_MaxAttempts CHECK (MaxAttempts BETWEEN 1 AND 20),
        CONSTRAINT CK_SapSyncProfileEntities_ExecutionOrder CHECK (ExecutionOrder BETWEEN 0 AND 100000),
        CONSTRAINT CK_SapSyncProfileEntities_Timeout CHECK (ExecutionTimeoutMinutes BETWEEN 1 AND 1440)
    );

    CREATE UNIQUE INDEX UX_SapSyncProfileEntities_Current
        ON dbo.SapSyncProfileEntities(SapSyncProfileId, EntityCode, Direction)
        WHERE IsDeleted = 0;
    CREATE INDEX IX_SapSyncProfileEntities_Profile_Order
        ON dbo.SapSyncProfileEntities(SapSyncProfileId, IsActive, ExecutionOrder, Id)
        WHERE IsDeleted = 0;
END;
GO

IF OBJECT_ID(N'dbo.SapSyncSchedules', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SapSyncSchedules
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SapSyncSchedules PRIMARY KEY,
        SapSyncProfileEntityId bigint NOT NULL,
        ScheduleType varchar(20) NOT NULL,
        IntervalMinutes int NULL,
        ExecutionTime time(0) NULL,
        TimeZoneId nvarchar(100) NOT NULL CONSTRAINT DF_SapSyncSchedules_TimeZoneId DEFAULT N'America/Guayaquil',
        PreventConcurrentExecutions bit NOT NULL CONSTRAINT DF_SapSyncSchedules_PreventConcurrent DEFAULT 1,
        NextExecutionAtUtc datetime2(0) NULL,
        LastScheduledAtUtc datetime2(0) NULL,
        LastExecutionAtUtc datetime2(0) NULL,
        LastSuccessfulExecutionAtUtc datetime2(0) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_SapSyncSchedules_IsActive DEFAULT 0,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SapSyncSchedules_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SapSyncSchedules_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        RowVersion rowversion NOT NULL,
        CONSTRAINT FK_SapSyncSchedules_ProfileEntity FOREIGN KEY (SapSyncProfileEntityId) REFERENCES dbo.SapSyncProfileEntities(Id),
        CONSTRAINT CK_SapSyncSchedules_TimeZoneId_NotBlank CHECK (LEN(LTRIM(RTRIM(TimeZoneId))) > 0),
        CONSTRAINT CK_SapSyncSchedules_Interval CHECK (IntervalMinutes IS NULL OR IntervalMinutes BETWEEN 1 AND 525600),
        CONSTRAINT CK_SapSyncSchedules_Shape CHECK
        (
            (ScheduleType = 'Manual' AND IntervalMinutes IS NULL AND ExecutionTime IS NULL)
            OR
            (ScheduleType = 'Interval' AND IntervalMinutes IS NOT NULL AND ExecutionTime IS NULL)
            OR
            (ScheduleType = 'Daily' AND IntervalMinutes IS NULL AND ExecutionTime IS NOT NULL)
        )
    );

    CREATE UNIQUE INDEX UX_SapSyncSchedules_ProfileEntity_Current
        ON dbo.SapSyncSchedules(SapSyncProfileEntityId)
        WHERE IsDeleted = 0;
    CREATE INDEX IX_SapSyncSchedules_Due
        ON dbo.SapSyncSchedules(IsActive, NextExecutionAtUtc, SapSyncProfileEntityId)
        WHERE IsDeleted = 0 AND IsActive = 1;
END;
GO

IF OBJECT_ID(N'dbo.AuditSapSyncProfileChanges', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuditSapSyncProfileChanges
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditSapSyncProfileChanges PRIMARY KEY,
        SapSyncProfileId bigint NULL,
        EntityName nvarchar(120) NOT NULL,
        RecordId nvarchar(80) NOT NULL,
        Action varchar(30) NOT NULL,
        SafeDataJson nvarchar(max) NULL,
        UserId int NULL,
        UserName nvarchar(120) NULL,
        Source varchar(30) NOT NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_AuditSapSyncProfileChanges_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_AuditSapSyncProfileChanges_Profile FOREIGN KEY (SapSyncProfileId) REFERENCES dbo.SapSyncProfiles(Id),
        CONSTRAINT CK_AuditSapSyncProfileChanges_Action CHECK
        (
            Action IN ('Created', 'Updated', 'Activated', 'Deactivated', 'Deleted', 'LegacyMigrated')
        ),
        CONSTRAINT CK_AuditSapSyncProfileChanges_Source CHECK (Source IN ('API', 'Migration', 'System')),
        CONSTRAINT CK_AuditSapSyncProfileChanges_SafeDataJson CHECK (SafeDataJson IS NULL OR ISJSON(SafeDataJson) = 1)
    );

    CREATE INDEX IX_AuditSapSyncProfileChanges_Profile_CreatedAt
        ON dbo.AuditSapSyncProfileChanges(SapSyncProfileId, CreatedAt DESC);
    CREATE INDEX IX_AuditSapSyncProfileChanges_Entity_Record_CreatedAt
        ON dbo.AuditSapSyncProfileChanges(EntityName, RecordId, CreatedAt DESC);
END;
GO

IF OBJECT_ID(N'dbo.SapSyncProfileCompatibilitySettings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SapSyncProfileCompatibilitySettings
    (
        CompanyId int NOT NULL CONSTRAINT PK_SapSyncProfileCompatibilitySettings PRIMARY KEY,
        LegacyFallbackEnabled bit NOT NULL CONSTRAINT DF_SapSyncProfileCompatibilitySettings_Fallback DEFAULT 1,
        CompatibilityVersion nvarchar(40) NOT NULL,
        RequiredSuccessfulCycles int NOT NULL CONSTRAINT DF_SapSyncProfileCompatibilitySettings_Cycles DEFAULT 2,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SapSyncProfileCompatibilitySettings_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        DisabledAt datetime2(0) NULL,
        RowVersion rowversion NOT NULL,
        CONSTRAINT FK_SapSyncProfileCompatibilitySettings_Companies FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(Id),
        CONSTRAINT CK_SapSyncProfileCompatibilitySettings_Cycles CHECK (RequiredSuccessfulCycles = 2)
    );
END;
GO

DECLARE @Capabilities table
(
    EntityCode nvarchar(80) PRIMARY KEY,
    DisplayName nvarchar(160) NOT NULL,
    SupportsSapToErp bit NOT NULL,
    SupportsErpToSap bit NOT NULL,
    SupportsFull bit NOT NULL,
    SupportsIncremental bit NOT NULL,
    IsImplemented bit NOT NULL
);

INSERT @Capabilities
(
    EntityCode, DisplayName, SupportsSapToErp, SupportsErpToSap,
    SupportsFull, SupportsIncremental, IsImplemented
)
VALUES
    (N'Suppliers', N'Proveedores', 1, 0, 1, 1, 1),
    (N'Items', N'Articulos', 1, 0, 1, 0, 1),
    (N'PurchaseOrders', N'Ordenes de compra', 0, 0, 0, 0, 0),
    (N'PaymentTerms', N'Condiciones de pago', 1, 0, 1, 0, 1);

UPDATE target
SET DisplayName = source.DisplayName,
    SupportsSapToErp = source.SupportsSapToErp,
    SupportsErpToSap = source.SupportsErpToSap,
    SupportsFull = source.SupportsFull,
    SupportsIncremental = source.SupportsIncremental,
    IsImplemented = source.IsImplemented,
    IsActive = 1,
    UpdatedByUserName = N'Sistema',
    UpdatedAt = SYSUTCDATETIME()
FROM dbo.SapSyncHandlerCapabilities target
INNER JOIN @Capabilities source ON source.EntityCode = target.EntityCode;

INSERT dbo.SapSyncHandlerCapabilities
(
    EntityCode, DisplayName, SupportsSapToErp, SupportsErpToSap,
    SupportsFull, SupportsIncremental, IsImplemented, IsActive,
    CreatedByUserName
)
SELECT
    source.EntityCode, source.DisplayName, source.SupportsSapToErp, source.SupportsErpToSap,
    source.SupportsFull, source.SupportsIncremental, source.IsImplemented, 1, N'Sistema'
FROM @Capabilities source
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.SapSyncHandlerCapabilities target
    WHERE target.EntityCode = source.EntityCode
);

INSERT dbo.SapSyncHandlerCapabilities
(
    EntityCode, DisplayName, SupportsSapToErp, SupportsErpToSap,
    SupportsFull, SupportsIncremental, IsImplemented, IsActive,
    CreatedByUserName
)
SELECT DISTINCT
    legacy.EntityCode, legacy.EntityCode, 0, 0, 0, 0, 0, 1, N'Migracion 152'
FROM dbo.SapSyncEntitySettings legacy
WHERE legacy.EntityCode <> N'Warehouses'
  AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SapSyncHandlerCapabilities target
          WHERE target.EntityCode = legacy.EntityCode
      );
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SAPSYNCHANDLERCAPABILITYLISTAR
    @ActiveOnly bit = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT EntityCode, DisplayName, SupportsSapToErp, SupportsErpToSap,
           SupportsFull, SupportsIncremental, IsImplemented, IsActive
    FROM dbo.SapSyncHandlerCapabilities
    WHERE @ActiveOnly = 0 OR IsActive = 1
    ORDER BY DisplayName, EntityCode;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SAPSYNCPROFILEPAGINAR
    @CompanyId int = NULL,
    @Search nvarchar(160) = NULL,
    @IsActive bit = NULL,
    @EntityCode nvarchar(80) = NULL,
    @PageNumber int = 1,
    @PageSize int = 50
AS
BEGIN
    SET NOCOUNT ON;

    SET @PageNumber = CASE WHEN @PageNumber < 1 THEN 1 ELSE @PageNumber END;
    SET @PageSize = CASE WHEN @PageSize < 1 OR @PageSize > 500 THEN 50 ELSE @PageSize END;
    SET @Search = NULLIF(LTRIM(RTRIM(@Search)), N'');
    SET @EntityCode = NULLIF(LTRIM(RTRIM(@EntityCode)), N'');

    ;WITH Filtered AS
    (
        SELECT
            profile.Id, profile.CompanyId, company.Code AS CompanyCode,
            COALESCE(company.CommercialName, company.Code) AS CompanyName,
            profile.Code, profile.Name, profile.Description, profile.IsActive,
            COUNT(CASE WHEN entity.IsDeleted = 0 AND entity.IsActive = 1 THEN 1 END) AS ActiveEntityCount,
            profile.CreatedAt AS CreatedAtUtc, profile.UpdatedAt AS UpdatedAtUtc,
            profile.RowVersion
        FROM dbo.SapSyncProfiles profile
        INNER JOIN dbo.Companies company ON company.Id = profile.CompanyId
        LEFT JOIN dbo.SapSyncProfileEntities entity ON entity.SapSyncProfileId = profile.Id
        WHERE profile.IsDeleted = 0
          AND (@CompanyId IS NULL OR profile.CompanyId = @CompanyId)
          AND (@IsActive IS NULL OR profile.IsActive = @IsActive)
          AND
          (
              @Search IS NULL
              OR profile.Code LIKE N'%' + @Search + N'%'
              OR profile.Name LIKE N'%' + @Search + N'%'
          )
          AND
          (
              @EntityCode IS NULL
              OR EXISTS
                 (
                     SELECT 1
                     FROM dbo.SapSyncProfileEntities matchEntity
                     WHERE matchEntity.SapSyncProfileId = profile.Id
                       AND matchEntity.EntityCode = @EntityCode
                       AND matchEntity.IsDeleted = 0
                 )
          )
        GROUP BY
            profile.Id, profile.CompanyId, company.Code, company.CommercialName,
            profile.Code, profile.Name, profile.Description, profile.IsActive,
            profile.CreatedAt, profile.UpdatedAt, profile.RowVersion
    )
    SELECT *
    FROM Filtered
    ORDER BY CompanyCode, Code, Id
    OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(1)
    FROM dbo.SapSyncProfiles profile
    WHERE profile.IsDeleted = 0
      AND (@CompanyId IS NULL OR profile.CompanyId = @CompanyId)
      AND (@IsActive IS NULL OR profile.IsActive = @IsActive)
      AND
      (
          @Search IS NULL
          OR profile.Code LIKE N'%' + @Search + N'%'
          OR profile.Name LIKE N'%' + @Search + N'%'
      )
      AND
      (
          @EntityCode IS NULL
          OR EXISTS
             (
                 SELECT 1
                 FROM dbo.SapSyncProfileEntities matchEntity
                 WHERE matchEntity.SapSyncProfileId = profile.Id
                   AND matchEntity.EntityCode = @EntityCode
                   AND matchEntity.IsDeleted = 0
             )
      );
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SAPSYNCPROFILEBUSCARPORID
    @Id bigint
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        profile.Id, profile.CompanyId, company.Code AS CompanyCode,
        COALESCE(company.CommercialName, company.Code) AS CompanyName,
        profile.Code, profile.Name, profile.Description, profile.IsActive,
        profile.CreatedByUserId, profile.CreatedByUserName,
        profile.CreatedAt AS CreatedAtUtc,
        profile.UpdatedByUserId, profile.UpdatedByUserName,
        profile.UpdatedAt AS UpdatedAtUtc,
        profile.RowVersion
    FROM dbo.SapSyncProfiles profile
    INNER JOIN dbo.Companies company ON company.Id = profile.CompanyId
    WHERE profile.Id = @Id AND profile.IsDeleted = 0;

    SELECT
        entity.Id, entity.EntityCode, entity.Direction, entity.SyncMode,
        entity.BatchSize, entity.MaxAttempts, entity.ExecutionOrder,
        entity.ContinueOnError, entity.ExecutionTimeoutMinutes,
        entity.IsActive, entity.RowVersion,
        schedule.Id AS ScheduleId, schedule.ScheduleType, schedule.IntervalMinutes,
        schedule.ExecutionTime, schedule.TimeZoneId,
        schedule.PreventConcurrentExecutions, schedule.NextExecutionAtUtc,
        schedule.LastScheduledAtUtc, schedule.LastExecutionAtUtc,
        schedule.LastSuccessfulExecutionAtUtc,
        schedule.IsActive AS ScheduleIsActive,
        schedule.RowVersion AS ScheduleRowVersion
    FROM dbo.SapSyncProfileEntities entity
    INNER JOIN dbo.SapSyncSchedules schedule
        ON schedule.SapSyncProfileEntityId = entity.Id
       AND schedule.IsDeleted = 0
    WHERE entity.SapSyncProfileId = @Id
      AND entity.IsDeleted = 0
    ORDER BY entity.ExecutionOrder, entity.EntityCode, entity.Direction;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SAPSYNCPROFILECREAR
    @CompanyId int,
    @Code nvarchar(80),
    @Name nvarchar(160),
    @Description nvarchar(500) = NULL,
    @IsActive bit = 0,
    @EntitiesJson nvarchar(max),
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

    IF @Code IS NULL OR @Name IS NULL OR ISJSON(@EntitiesJson) <> 1
    BEGIN
        SELECT CAST(NULL AS bigint) AS Id, N'InvalidProfile' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
        RETURN;
    END;
    IF @IsActive = 1
    BEGIN
        SELECT CAST(NULL AS bigint) AS Id, N'InitialStateMustBeInactive' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
        RETURN;
    END;
    IF NOT EXISTS (SELECT 1 FROM dbo.Companies WHERE Id = @CompanyId AND IsActive = 1)
    BEGIN
        SELECT CAST(NULL AS bigint) AS Id, N'CompanyNotAvailable' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
        RETURN;
    END;
    IF EXISTS (SELECT 1 FROM dbo.SapSyncProfiles WHERE CompanyId = @CompanyId AND Code = @Code AND IsDeleted = 0)
    BEGIN
        SELECT CAST(NULL AS bigint) AS Id, N'DuplicateCode' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
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
        ScheduleType varchar(20) NOT NULL,
        IntervalMinutes int NULL,
        ExecutionTime time(0) NULL,
        TimeZoneId nvarchar(100) NOT NULL,
        PreventConcurrentExecutions bit NOT NULL,
        PRIMARY KEY(EntityCode, Direction)
    );

    INSERT @Entities
    (
        EntityCode, Direction, SyncMode, BatchSize, MaxAttempts, ExecutionOrder,
        ContinueOnError, ExecutionTimeoutMinutes, ScheduleType, IntervalMinutes,
        ExecutionTime, TimeZoneId, PreventConcurrentExecutions
    )
    SELECT
        LTRIM(RTRIM(EntityCode)), Direction, SyncMode, BatchSize, MaxAttempts,
        ExecutionOrder, ContinueOnError, ExecutionTimeoutMinutes,
        ScheduleType, IntervalMinutes, ExecutionTime,
        COALESCE(NULLIF(LTRIM(RTRIM(TimeZoneId)), N''), N'America/Guayaquil'),
        PreventConcurrentExecutions
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
        ScheduleType varchar(20) '$.schedule.scheduleType',
        IntervalMinutes int '$.schedule.intervalMinutes',
        ExecutionTime time(0) '$.schedule.executionTime',
        TimeZoneId nvarchar(100) '$.schedule.timeZoneId',
        PreventConcurrentExecutions bit '$.schedule.preventConcurrentExecutions'
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
          )
    BEGIN
        SELECT CAST(NULL AS bigint) AS Id, N'UnsupportedCapability' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
        RETURN;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM @Entities
        WHERE NOT
        (
            (ScheduleType = 'Manual' AND IntervalMinutes IS NULL AND ExecutionTime IS NULL)
            OR
            (ScheduleType = 'Interval' AND IntervalMinutes BETWEEN 1 AND 525600 AND ExecutionTime IS NULL)
            OR
            (ScheduleType = 'Daily' AND IntervalMinutes IS NULL AND ExecutionTime IS NOT NULL)
        )
    )
    BEGIN
        SELECT CAST(NULL AS bigint) AS Id, N'InvalidSchedule' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT dbo.SapSyncProfiles
        (
            CompanyId, Code, Name, Description, SourceType, IsActive,
            CreatedByUserId, CreatedByUserName
        )
        VALUES
        (
            @CompanyId, @Code, @Name, @Description, 'Native', 0,
            @AuditUserId, @AuditUserName
        );

        DECLARE @ProfileId bigint = SCOPE_IDENTITY();

        INSERT dbo.SapSyncProfileEntities
        (
            SapSyncProfileId, EntityCode, Direction, SyncMode, BatchSize,
            MaxAttempts, ExecutionOrder, ContinueOnError, ExecutionTimeoutMinutes,
            IsActive, CreatedByUserId, CreatedByUserName
        )
        SELECT
            @ProfileId, EntityCode, Direction, SyncMode, BatchSize,
            MaxAttempts, ExecutionOrder, ContinueOnError, ExecutionTimeoutMinutes,
            0, @AuditUserId, @AuditUserName
        FROM @Entities;

        INSERT dbo.SapSyncSchedules
        (
            SapSyncProfileEntityId, ScheduleType, IntervalMinutes, ExecutionTime,
            TimeZoneId, PreventConcurrentExecutions, IsActive,
            CreatedByUserId, CreatedByUserName
        )
        SELECT
            profileEntity.Id, entity.ScheduleType, entity.IntervalMinutes,
            entity.ExecutionTime, entity.TimeZoneId,
            entity.PreventConcurrentExecutions, 0,
            @AuditUserId, @AuditUserName
        FROM @Entities entity
        INNER JOIN dbo.SapSyncProfileEntities profileEntity
            ON profileEntity.SapSyncProfileId = @ProfileId
           AND profileEntity.EntityCode = entity.EntityCode
           AND profileEntity.Direction = entity.Direction
           AND profileEntity.IsDeleted = 0;

        INSERT dbo.AuditSapSyncProfileChanges
        (
            SapSyncProfileId, EntityName, RecordId, Action, SafeDataJson,
            UserId, UserName, Source
        )
        VALUES
        (
            @ProfileId, N'SapSyncProfile', CONVERT(nvarchar(80), @ProfileId),
            'Created',
            (SELECT @CompanyId AS CompanyId, @Code AS Code, 0 AS IsActive FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
            @AuditUserId, @AuditUserName, 'API'
        );

        COMMIT;

        SELECT Id, N'Created' AS ResultCode, RowVersion
        FROM dbo.SapSyncProfiles
        WHERE Id = @ProfileId;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK;
        THROW;
    END CATCH;
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

    IF @Code IS NULL OR @Name IS NULL OR ISJSON(@EntitiesJson) <> 1
    BEGIN
        SELECT @Id AS Id, N'InvalidProfile' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
        RETURN;
    END;
    IF NOT EXISTS (SELECT 1 FROM dbo.SapSyncProfiles WHERE Id = @Id AND IsDeleted = 0)
    BEGIN
        SELECT @Id AS Id, N'NotFound' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
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
        SET CompanyId = @CompanyId,
            Code = @Code,
            Name = @Name,
            Description = @Description,
            UpdatedByUserId = @AuditUserId,
            UpdatedByUserName = @AuditUserName,
            UpdatedAt = SYSUTCDATETIME()
        WHERE Id = @Id
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

CREATE OR ALTER PROCEDURE dbo.SP_NA_PATCH_SAPSYNCPROFILEACTIVAR
    @Id bigint,
    @IsActive bit,
    @ExpectedRowVersion varbinary(8),
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.SapSyncProfiles WHERE Id = @Id AND IsDeleted = 0)
    BEGIN
        SELECT @Id AS Id, N'NotFound' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
        RETURN;
    END;
    IF @IsActive = 1
       AND NOT EXISTS
           (
               SELECT 1
               FROM dbo.SapSyncProfileEntities entity
               INNER JOIN dbo.SapSyncHandlerCapabilities capability ON capability.EntityCode = entity.EntityCode
               WHERE entity.SapSyncProfileId = @Id
                 AND entity.IsDeleted = 0
                 AND entity.IsActive = 1
                 AND entity.Direction <> 'Both'
                 AND capability.IsActive = 1
                 AND capability.IsImplemented = 1
                 AND
                 (
                     (entity.Direction = 'SapToErp' AND capability.SupportsSapToErp = 1)
                     OR
                     (entity.Direction = 'ErpToSap' AND capability.SupportsErpToSap = 1)
                 )
           )
    BEGIN
        SELECT @Id AS Id, N'NoActiveSupportedEntities' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
        RETURN;
    END;

    BEGIN TRANSACTION;

    UPDATE dbo.SapSyncProfiles WITH (UPDLOCK)
    SET IsActive = @IsActive,
        UpdatedByUserId = @AuditUserId,
        UpdatedByUserName = NULLIF(LTRIM(RTRIM(@AuditUserName)), N''),
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id
      AND RowVersion = @ExpectedRowVersion
      AND IsDeleted = 0;

    IF @@ROWCOUNT = 0
    BEGIN
        ROLLBACK;
        SELECT @Id AS Id, N'ConcurrencyConflict' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
        RETURN;
    END;

    INSERT dbo.AuditSapSyncProfileChanges
    (
        SapSyncProfileId, EntityName, RecordId, Action, SafeDataJson,
        UserId, UserName, Source
    )
    VALUES
    (
        @Id, N'SapSyncProfile', CONVERT(nvarchar(80), @Id),
        CASE WHEN @IsActive = 1 THEN 'Activated' ELSE 'Deactivated' END,
        (SELECT @IsActive AS IsActive FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
        @AuditUserId, NULLIF(LTRIM(RTRIM(@AuditUserName)), N''), 'API'
    );

    COMMIT;

    SELECT Id,
           CASE WHEN @IsActive = 1 THEN N'Activated' ELSE N'Deactivated' END AS ResultCode,
           RowVersion
    FROM dbo.SapSyncProfiles
    WHERE Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_SAPSYNCPROFILEELIMINAR
    @Id bigint,
    @ExpectedRowVersion varbinary(8),
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.SapSyncProfiles WHERE Id = @Id AND IsDeleted = 0)
    BEGIN
        SELECT @Id AS Id, N'NotFound' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
        RETURN;
    END;

    BEGIN TRANSACTION;

    UPDATE schedule
    SET IsDeleted = 1, IsActive = 0,
        DeletedByUserId = @AuditUserId,
        DeletedByUserName = NULLIF(LTRIM(RTRIM(@AuditUserName)), N''),
        DeletedAt = SYSUTCDATETIME()
    FROM dbo.SapSyncSchedules schedule
    INNER JOIN dbo.SapSyncProfileEntities entity ON entity.Id = schedule.SapSyncProfileEntityId
    WHERE entity.SapSyncProfileId = @Id AND schedule.IsDeleted = 0;

    UPDATE dbo.SapSyncProfileEntities
    SET IsDeleted = 1, IsActive = 0,
        DeletedByUserId = @AuditUserId,
        DeletedByUserName = NULLIF(LTRIM(RTRIM(@AuditUserName)), N''),
        DeletedAt = SYSUTCDATETIME()
    WHERE SapSyncProfileId = @Id AND IsDeleted = 0;

    UPDATE dbo.SapSyncProfiles WITH (UPDLOCK)
    SET IsDeleted = 1, IsActive = 0,
        DeletedByUserId = @AuditUserId,
        DeletedByUserName = NULLIF(LTRIM(RTRIM(@AuditUserName)), N''),
        DeletedAt = SYSUTCDATETIME()
    WHERE Id = @Id
      AND RowVersion = @ExpectedRowVersion
      AND IsDeleted = 0;

    IF @@ROWCOUNT = 0
    BEGIN
        ROLLBACK;
        SELECT @Id AS Id, N'ConcurrencyConflict' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
        RETURN;
    END;

    INSERT dbo.AuditSapSyncProfileChanges
    (
        SapSyncProfileId, EntityName, RecordId, Action,
        UserId, UserName, Source
    )
    VALUES
    (
        @Id, N'SapSyncProfile', CONVERT(nvarchar(80), @Id), 'Deleted',
        @AuditUserId, NULLIF(LTRIM(RTRIM(@AuditUserName)), N''), 'API'
    );

    COMMIT;
    SELECT @Id AS Id, N'Deleted' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
END;
GO

BEGIN TRY
BEGIN TRANSACTION;

INSERT dbo.SapSyncProfiles
(
    CompanyId, Code, Name, Description, SourceType, IsActive,
    CreatedByUserName
)
SELECT
    legacy.CompanyId,
    CONCAT(N'LEGACY-SAP-', legacy.CompanyId),
    CONCAT(N'Perfil SAP legado ', company.Code),
    N'Migrado desde SapSyncEntitySettings; requiere validacion y activacion explicitas.',
    'LegacyMigration',
    0,
    N'Migracion 152'
FROM (SELECT DISTINCT CompanyId FROM dbo.SapSyncEntitySettings) legacy
INNER JOIN dbo.Companies company ON company.Id = legacy.CompanyId
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.SapSyncProfiles profile
    WHERE profile.CompanyId = legacy.CompanyId
      AND profile.Code = CONCAT(N'LEGACY-SAP-', legacy.CompanyId)
      AND profile.IsDeleted = 0
);

INSERT dbo.SapSyncProfileEntities
(
    SapSyncProfileId, EntityCode, Direction, SyncMode, BatchSize,
    MaxAttempts, ExecutionOrder, ContinueOnError, ExecutionTimeoutMinutes,
    IsActive, CreatedByUserName
)
SELECT
    profile.Id,
    legacy.EntityCode,
    legacy.Direction,
    CASE WHEN legacy.EntityCode = N'Suppliers' THEN 'Incremental' ELSE 'Full' END,
    CASE WHEN legacy.BatchSize BETWEEN 1 AND 10000 THEN legacy.BatchSize ELSE 100 END,
    CASE WHEN legacy.MaxRetryCount BETWEEN 0 AND 19 THEN legacy.MaxRetryCount + 1 ELSE 3 END,
    CASE WHEN legacy.ExecutionOrder BETWEEN 0 AND 100000 THEN legacy.ExecutionOrder ELSE 0 END,
    1,
    30,
    0,
    N'Migracion 152'
FROM dbo.SapSyncEntitySettings legacy
INNER JOIN dbo.SapSyncProfiles profile
    ON profile.CompanyId = legacy.CompanyId
   AND profile.Code = CONCAT(N'LEGACY-SAP-', legacy.CompanyId)
   AND profile.IsDeleted = 0
INNER JOIN dbo.SapSyncHandlerCapabilities capability ON capability.EntityCode = legacy.EntityCode
WHERE legacy.EntityCode <> N'Warehouses'
  AND NOT EXISTS
(
    SELECT 1
    FROM dbo.SapSyncProfileEntities currentEntity
    WHERE currentEntity.SapSyncProfileId = profile.Id
      AND currentEntity.EntityCode = legacy.EntityCode
      AND currentEntity.Direction = legacy.Direction
      AND currentEntity.IsDeleted = 0
);

INSERT dbo.SapSyncSchedules
(
    SapSyncProfileEntityId, ScheduleType, IntervalMinutes, ExecutionTime,
    TimeZoneId, PreventConcurrentExecutions, IsActive, CreatedByUserName
)
SELECT
    entity.Id, 'Manual', NULL, NULL,
    N'America/Guayaquil', 1, 0, N'Migracion 152'
FROM dbo.SapSyncProfileEntities entity
INNER JOIN dbo.SapSyncProfiles profile ON profile.Id = entity.SapSyncProfileId
WHERE profile.SourceType = 'LegacyMigration'
  AND entity.IsDeleted = 0
  AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SapSyncSchedules schedule
          WHERE schedule.SapSyncProfileEntityId = entity.Id
            AND schedule.IsDeleted = 0
      );

INSERT dbo.SapSyncProfileCompatibilitySettings
(
    CompanyId, LegacyFallbackEnabled, CompatibilityVersion,
    RequiredSuccessfulCycles
)
SELECT DISTINCT
    legacy.CompanyId, 1, N'Fase10.2-v1', 2
FROM dbo.SapSyncEntitySettings legacy
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.SapSyncProfileCompatibilitySettings currentSetting
    WHERE currentSetting.CompanyId = legacy.CompanyId
);

INSERT dbo.AuditSapSyncProfileChanges
(
    SapSyncProfileId, EntityName, RecordId, Action, SafeDataJson,
    UserName, Source
)
SELECT
    profile.Id,
    N'SapSyncProfile',
    CONVERT(nvarchar(80), profile.Id),
    'LegacyMigrated',
    (
        SELECT profile.CompanyId AS CompanyId,
               profile.Code AS Code,
               0 AS IsActive,
               N'Manual' AS ScheduleType
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    ),
    N'Migracion 152',
    'Migration'
FROM dbo.SapSyncProfiles profile
WHERE profile.SourceType = 'LegacyMigration'
  AND profile.IsDeleted = 0
  AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.AuditSapSyncProfileChanges audit
          WHERE audit.SapSyncProfileId = profile.Id
            AND audit.Action = 'LegacyMigrated'
      );

COMMIT;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK;
    THROW;
END CATCH;
GO

DECLARE @SapModuleId int = (SELECT TOP (1) Id FROM dbo.Modules WHERE Code = N'SAP');
IF @SapModuleId IS NULL
BEGIN
    INSERT dbo.Modules(Code, Name, DisplayOrder)
    VALUES(N'SAP', N'SAP Business One', 75);
    SET @SapModuleId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Modules
    SET Name = N'SAP Business One', DisplayOrder = 75,
        IsActive = 1, UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @SapModuleId;
END;

DECLARE @Permissions table
(
    Code nvarchar(120) PRIMARY KEY,
    Name nvarchar(160) NOT NULL,
    Description nvarchar(300) NOT NULL
);

INSERT @Permissions(Code, Name, Description)
VALUES
    (N'SAP.SYNC.PROFILES.VIEW', N'Ver perfiles SAP', N'Consultar perfiles SAP independientes.'),
    (N'SAP.SYNC.PROFILES.CREATE', N'Crear perfiles SAP', N'Crear perfiles SAP inicialmente inactivos.'),
    (N'SAP.SYNC.PROFILES.EDIT', N'Editar perfiles SAP', N'Editar entidades y agendas SAP.'),
    (N'SAP.SYNC.PROFILES.ACTIVATE', N'Activar perfiles SAP', N'Activar o desactivar perfiles SAP validados.'),
    (N'SAP.SYNC.PROFILES.DELETE', N'Eliminar perfiles SAP', N'Eliminar logicamente perfiles SAP.'),
    (N'SAP.SYNC.PROFILES.VALIDATE', N'Validar perfiles SAP', N'Validar contratos y capacidades sin llamar SAP.'),
    (N'SAP.SYNC.PROFILES.EXECUTE', N'Ejecutar perfiles SAP', N'Solicitar ejecucion manual autorizada.'),
    (N'SAP.SYNC.EXECUTIONS.VIEW', N'Ver ejecuciones SAP', N'Consultar historial SAP seguro.'),
    (N'SAP.SYNC.EXECUTIONS.VIEW_DETAIL', N'Ver detalle de ejecuciones SAP', N'Consultar resultados SAP por registro.'),
    (N'SAP.SYNC.EXECUTIONS.RETRY', N'Reintentar ejecuciones SAP', N'Reintentar resultados elegibles con motivo.'),
    (N'SAP.SYNC.EXECUTIONS.CANCEL', N'Cancelar ejecuciones SAP', N'Solicitar cancelacion cooperativa.'),
    (N'SAP.SYNC.EXECUTIONS.RELEASE_EXPIRED_LOCK', N'Liberar lock SAP vencido', N'Recuperar exclusivamente locks SAP vencidos.');

UPDATE target
SET ModuleId = @SapModuleId,
    Name = source.Name,
    Description = source.Description,
    IsActive = 1,
    UpdatedAt = SYSUTCDATETIME()
FROM dbo.Permissions target
INNER JOIN @Permissions source ON source.Code = target.Code;

INSERT dbo.Permissions(ModuleId, Code, Name, Description)
SELECT @SapModuleId, source.Code, source.Name, source.Description
FROM @Permissions source
WHERE NOT EXISTS (SELECT 1 FROM dbo.Permissions target WHERE target.Code = source.Code);

DECLARE @AdminRoleId int =
(
    SELECT TOP (1) Id
    FROM dbo.Roles
    WHERE Code = N'ADMIN' AND IsDeleted = 0
);

IF @AdminRoleId IS NULL
    THROW 51152, 'ADMIN role is required before granting SAP profile permissions.', 1;

INSERT dbo.RolePermissions(RoleId, PermissionId)
SELECT @AdminRoleId, permission.Id
FROM dbo.Permissions permission
INNER JOIN @Permissions source ON source.Code = permission.Code
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.RolePermissions existing
    WHERE existing.RoleId = @AdminRoleId
      AND existing.PermissionId = permission.Id
);
GO

DECLARE @Operations table
(
    Code nvarchar(80) PRIMARY KEY,
    Name nvarchar(120) NOT NULL,
    Description nvarchar(300) NOT NULL,
    ActionKey nvarchar(120) NOT NULL,
    DisplayOrder int NOT NULL
);

INSERT @Operations(Code, Name, Description, ActionKey, DisplayOrder)
VALUES
    (N'ACTION.SAP_SYNC_PROFILES.VIEW', N'Consultar perfil SAP', N'Consultar un perfil SAP.', N'consult', 10),
    (N'ACTION.SAP_SYNC_PROFILES.CREATE', N'Crear perfil SAP', N'Crear un perfil SAP inactivo.', N'create', 20),
    (N'ACTION.SAP_SYNC_PROFILES.EDIT', N'Editar perfil SAP', N'Editar un perfil SAP.', N'edit', 30),
    (N'ACTION.SAP_SYNC_PROFILES.ACTIVATE', N'Activar perfil SAP', N'Activar un perfil SAP validado.', N'activate', 40),
    (N'ACTION.SAP_SYNC_PROFILES.DEACTIVATE', N'Desactivar perfil SAP', N'Desactivar un perfil SAP.', N'deactivate', 50),
    (N'ACTION.SAP_SYNC_PROFILES.DELETE', N'Eliminar perfil SAP', N'Eliminar logicamente un perfil SAP.', N'delete', 60),
    (N'ACTION.SAP_SYNC_PROFILES.VALIDATE', N'Validar perfil SAP', N'Validar configuracion sin llamada externa.', N'validate', 70),
    (N'ACTION.SAP_SYNC_PROFILES.EXECUTE', N'Ejecutar perfil SAP', N'Solicitar ejecucion manual.', N'execute', 80),
    (N'ACTION.SAP_SYNC_EXECUTIONS.VIEW', N'Consultar ejecucion SAP', N'Consultar una ejecucion SAP.', N'consult', 90),
    (N'ACTION.SAP_SYNC_EXECUTIONS.VIEW_DETAIL', N'Ver detalle SAP', N'Consultar resultados por registro.', N'view-detail', 100),
    (N'ACTION.SAP_SYNC_EXECUTIONS.RETRY', N'Reintentar ejecucion SAP', N'Reintentar resultados elegibles.', N'retry', 110),
    (N'ACTION.SAP_SYNC_EXECUTIONS.CANCEL', N'Cancelar ejecucion SAP', N'Solicitar cancelacion cooperativa.', N'cancel', 120),
    (N'ACTION.SAP_SYNC_EXECUTIONS.RELEASE_EXPIRED_LOCK', N'Liberar lock SAP vencido', N'Liberar exclusivamente un lock vencido.', N'release-expired-lock', 130);

UPDATE target
SET Name = source.Name,
    Description = source.Description,
    ActionKey = source.ActionKey,
    RibbonPageName = N'Inicio',
    RibbonGroupName = N'Acciones',
    DisplayOrder = source.DisplayOrder,
    IsActive = 1,
    IsDeleted = 0,
    DeletedByUserId = NULL,
    DeletedByUserName = NULL,
    DeletedAt = NULL,
    UpdatedByUserName = N'Sistema',
    UpdatedAt = SYSUTCDATETIME()
FROM dbo.SecurityOperations target
INNER JOIN @Operations source ON source.Code = target.Code;

INSERT dbo.SecurityOperations
(
    Code, Name, Description, ActionKey, RibbonPageName, RibbonGroupName,
    DisplayOrder, IsActive, CreatedByUserName, IsDeleted
)
SELECT
    source.Code, source.Name, source.Description, source.ActionKey,
    N'Inicio', N'Acciones', source.DisplayOrder, 1, N'Sistema', 0
FROM @Operations source
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.SecurityOperations target
    WHERE target.Code = source.Code
);
GO

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.MasterSchemaHistory
    WHERE Version = N'20260730.152'
)
BEGIN
    INSERT dbo.MasterSchemaHistory(Version, Description)
    VALUES
    (
        N'20260730.152',
        N'Perfiles SAP independientes, capacidades, seguridad ADMIN y migracion legado inactiva'
    );
END;
GO
