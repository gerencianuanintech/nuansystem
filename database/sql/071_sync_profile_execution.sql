SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SyncProfileExecutions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SyncProfileExecutions
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SyncProfileExecutions PRIMARY KEY,
        SyncProfileId int NOT NULL,
        ExecutionType nvarchar(20) NOT NULL,
        Status nvarchar(30) NOT NULL CONSTRAINT DF_SyncProfileExecutions_Status DEFAULT N'Pending',
        CorrelationId nvarchar(100) NOT NULL,
        RequestedBy nvarchar(120) NULL,
        RequestedAt datetime2(0) NOT NULL CONSTRAINT DF_SyncProfileExecutions_RequestedAt DEFAULT SYSUTCDATETIME(),
        StartedAt datetime2(0) NULL,
        FinishedAt datetime2(0) NULL,
        CancelledAt datetime2(0) NULL,
        CancelledBy nvarchar(120) NULL,
        EntityCodesJson nvarchar(max) NULL,
        FromKey nvarchar(200) NULL,
        MaxRecords int NULL,
        TotalEntities int NOT NULL CONSTRAINT DF_SyncProfileExecutions_TotalEntities DEFAULT 0,
        TotalRecordsRead int NOT NULL CONSTRAINT DF_SyncProfileExecutions_TotalRecordsRead DEFAULT 0,
        TotalEventsPublished int NOT NULL CONSTRAINT DF_SyncProfileExecutions_TotalEventsPublished DEFAULT 0,
        TotalSkipped int NOT NULL CONSTRAINT DF_SyncProfileExecutions_TotalSkipped DEFAULT 0,
        TotalErrors int NOT NULL CONSTRAINT DF_SyncProfileExecutions_TotalErrors DEFAULT 0,
        Message nvarchar(1000) NULL,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SyncProfileExecutions_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT FK_SyncProfileExecutions_Profile FOREIGN KEY (SyncProfileId) REFERENCES dbo.SyncProfiles(Id),
        CONSTRAINT CK_SyncProfileExecutions_Type CHECK (ExecutionType IN (N'Manual', N'Scheduled', N'Retry')),
        CONSTRAINT CK_SyncProfileExecutions_Status CHECK (Status IN (N'Pending', N'Running', N'Cancelling', N'Cancelled', N'Completed', N'CompletedWithErrors', N'Failed')),
        CONSTRAINT CK_SyncProfileExecutions_MaxRecords CHECK (MaxRecords IS NULL OR MaxRecords > 0),
        CONSTRAINT CK_SyncProfileExecutions_EntityCodesJson CHECK (EntityCodesJson IS NULL OR ISJSON(EntityCodesJson) = 1)
    );
END;
GO

IF OBJECT_ID(N'dbo.SyncProfileExecutionDetails', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SyncProfileExecutionDetails
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SyncProfileExecutionDetails PRIMARY KEY,
        SyncProfileExecutionId int NOT NULL,
        SyncProfileEntityId int NOT NULL,
        EntityCode nvarchar(80) NOT NULL,
        Status nvarchar(30) NOT NULL CONSTRAINT DF_SyncProfileExecutionDetails_Status DEFAULT N'Pending',
        StartedAt datetime2(0) NULL,
        FinishedAt datetime2(0) NULL,
        TotalRecordsRead int NOT NULL CONSTRAINT DF_SyncProfileExecutionDetails_TotalRecordsRead DEFAULT 0,
        TotalEventsPublished int NOT NULL CONSTRAINT DF_SyncProfileExecutionDetails_TotalEventsPublished DEFAULT 0,
        TotalSkipped int NOT NULL CONSTRAINT DF_SyncProfileExecutionDetails_TotalSkipped DEFAULT 0,
        TotalErrors int NOT NULL CONSTRAINT DF_SyncProfileExecutionDetails_TotalErrors DEFAULT 0,
        LastProcessedKey nvarchar(200) NULL,
        Message nvarchar(1000) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SyncProfileExecutionDetails_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT FK_SyncProfileExecutionDetails_Execution FOREIGN KEY (SyncProfileExecutionId) REFERENCES dbo.SyncProfileExecutions(Id),
        CONSTRAINT FK_SyncProfileExecutionDetails_Entity FOREIGN KEY (SyncProfileEntityId) REFERENCES dbo.SyncProfileEntities(Id),
        CONSTRAINT CK_SyncProfileExecutionDetails_Status CHECK (Status IN (N'Pending', N'Running', N'Skipped', N'Cancelled', N'Completed', N'CompletedWithErrors', N'Failed'))
    );
END;
GO

IF COL_LENGTH(N'dbo.SyncSchedules', N'NextExecutionAt') IS NULL
    ALTER TABLE dbo.SyncSchedules ADD NextExecutionAt datetime2(0) NULL;
GO

IF COL_LENGTH(N'dbo.SyncSchedules', N'LastSuccessfulScheduledExecutionAt') IS NULL
    ALTER TABLE dbo.SyncSchedules ADD LastSuccessfulScheduledExecutionAt datetime2(0) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SyncProfileExecutions_CorrelationId' AND object_id = OBJECT_ID(N'dbo.SyncProfileExecutions'))
    CREATE UNIQUE INDEX UX_SyncProfileExecutions_CorrelationId ON dbo.SyncProfileExecutions (CorrelationId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SyncProfileExecutions_Active' AND object_id = OBJECT_ID(N'dbo.SyncProfileExecutions'))
    CREATE INDEX IX_SyncProfileExecutions_Active ON dbo.SyncProfileExecutions (SyncProfileId, Status, RequestedAt);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SyncProfileExecutionDetails_ExecutionEntity' AND object_id = OBJECT_ID(N'dbo.SyncProfileExecutionDetails'))
    CREATE UNIQUE INDEX UX_SyncProfileExecutionDetails_ExecutionEntity ON dbo.SyncProfileExecutionDetails (SyncProfileExecutionId, SyncProfileEntityId);
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_CREATE_SYNCPROFILEEXECUTION
    @SyncProfileId int,
    @ExecutionType nvarchar(20),
    @RequestedBy nvarchar(120) = NULL,
    @CorrelationId nvarchar(100),
    @EntityCodesJson nvarchar(max) = NULL,
    @FromKey nvarchar(200) = NULL,
    @MaxRecords int = NULL,
    @TotalEntities int = 0,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @PreventConcurrent bit = 1;
    SELECT @PreventConcurrent = ISNULL(schedule.PreventConcurrentExecutions, 1)
    FROM dbo.SyncSchedules schedule
    WHERE schedule.SyncProfileId = @SyncProfileId
      AND schedule.IsDeleted = 0;

    IF @PreventConcurrent = 1
       AND EXISTS (SELECT 1 FROM dbo.SyncProfileExecutions WHERE SyncProfileId = @SyncProfileId AND Status IN (N'Pending', N'Running', N'Cancelling'))
    BEGIN
        THROW 51071, 'Ya existe una ejecucion activa para el perfil.', 1;
    END;

    INSERT INTO dbo.SyncProfileExecutions
    (
        SyncProfileId, ExecutionType, Status, CorrelationId, RequestedBy,
        EntityCodesJson, FromKey, MaxRecords, TotalEntities,
        CreatedByUserId, CreatedByUserName
    )
    VALUES
    (
        @SyncProfileId, @ExecutionType, N'Pending', @CorrelationId, @RequestedBy,
        @EntityCodesJson, @FromKey, @MaxRecords, ISNULL(@TotalEntities, 0),
        @CreatedByUserId, @CreatedByUserName
    );

    SELECT CONVERT(int, SCOPE_IDENTITY());
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_START_SYNCPROFILEEXECUTION
    @ExecutionId int
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.SyncProfileExecutions
       SET Status = N'Running',
           StartedAt = COALESCE(StartedAt, SYSUTCDATETIME()),
           UpdatedAt = SYSUTCDATETIME()
     WHERE Id = @ExecutionId
       AND Status = N'Pending';

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_COMPLETE_SYNCPROFILEEXECUTION
    @ExecutionId int,
    @Status nvarchar(30),
    @TotalRecordsRead int,
    @TotalEventsPublished int,
    @TotalSkipped int,
    @TotalErrors int,
    @Message nvarchar(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE execution
       SET Status = @Status,
           FinishedAt = SYSUTCDATETIME(),
           TotalRecordsRead = @TotalRecordsRead,
           TotalEventsPublished = @TotalEventsPublished,
           TotalSkipped = @TotalSkipped,
           TotalErrors = @TotalErrors,
           Message = @Message,
           UpdatedAt = SYSUTCDATETIME(),
           CancelledAt = CASE WHEN @Status = N'Cancelled' THEN COALESCE(CancelledAt, SYSUTCDATETIME()) ELSE CancelledAt END
      FROM dbo.SyncProfileExecutions execution
     WHERE execution.Id = @ExecutionId
       AND execution.Status IN (N'Running', N'Cancelling');

    IF @Status = N'Completed'
    BEGIN
        UPDATE schedule
           SET LastSuccessfulScheduledExecutionAt = SYSUTCDATETIME(),
               UpdatedAt = SYSUTCDATETIME()
          FROM dbo.SyncSchedules schedule
          INNER JOIN dbo.SyncProfileExecutions execution ON execution.SyncProfileId = schedule.SyncProfileId
         WHERE execution.Id = @ExecutionId
           AND execution.ExecutionType = N'Scheduled'
           AND schedule.IsDeleted = 0;
    END;

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_CANCEL_SYNCPROFILEEXECUTION
    @ExecutionId int,
    @CancelledBy nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.SyncProfileExecutions
       SET Status = CASE WHEN Status = N'Pending' THEN N'Cancelled' ELSE N'Cancelling' END,
           CancelledAt = SYSUTCDATETIME(),
           CancelledBy = @CancelledBy,
           UpdatedAt = SYSUTCDATETIME()
     WHERE Id = @ExecutionId
       AND Status IN (N'Pending', N'Running');

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SYNCPROFILEEXECUTION_BYID
    @ExecutionId int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        execution.Id,
        execution.SyncProfileId,
        profile.Code AS ProfileCode,
        profile.Name AS ProfileName,
        profile.CompanyId,
        company.CommercialName AS CompanyName,
        execution.ExecutionType,
        execution.Status,
        execution.CorrelationId,
        execution.RequestedBy,
        execution.RequestedAt,
        execution.StartedAt,
        execution.FinishedAt,
        execution.CancelledAt,
        execution.CancelledBy,
        execution.EntityCodesJson,
        execution.FromKey,
        execution.MaxRecords,
        execution.TotalEntities,
        execution.TotalRecordsRead,
        execution.TotalEventsPublished,
        execution.TotalSkipped,
        execution.TotalErrors,
        execution.Message
    FROM dbo.SyncProfileExecutions execution
    INNER JOIN dbo.SyncProfiles profile ON profile.Id = execution.SyncProfileId
    INNER JOIN dbo.Companies company ON company.Id = profile.CompanyId
    WHERE execution.Id = @ExecutionId;

    SELECT
        detail.Id,
        detail.SyncProfileExecutionId,
        detail.SyncProfileEntityId,
        detail.EntityCode,
        detail.Status,
        detail.StartedAt,
        detail.FinishedAt,
        detail.TotalRecordsRead,
        detail.TotalEventsPublished,
        detail.TotalSkipped,
        detail.TotalErrors,
        detail.LastProcessedKey,
        detail.Message
    FROM dbo.SyncProfileExecutionDetails detail
    WHERE detail.SyncProfileExecutionId = @ExecutionId
    ORDER BY detail.Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_SEARCH_SYNCPROFILEEXECUTIONS
    @ProfileId int = NULL,
    @Status nvarchar(30) = NULL,
    @ExecutionType nvarchar(20) = NULL,
    @DateFrom datetime2(0) = NULL,
    @DateTo datetime2(0) = NULL,
    @PageNumber int = 1,
    @PageSize int = 50
AS
BEGIN
    SET NOCOUNT ON;

    SET @PageNumber = CASE WHEN @PageNumber < 1 THEN 1 ELSE @PageNumber END;
    SET @PageSize = CASE WHEN @PageSize < 1 THEN 50 WHEN @PageSize > 200 THEN 200 ELSE @PageSize END;

    ;WITH Filtered AS
    (
        SELECT
            execution.Id,
            execution.SyncProfileId,
            profile.Code AS ProfileCode,
            profile.Name AS ProfileName,
            profile.CompanyId,
            company.CommercialName AS CompanyName,
            execution.ExecutionType,
            execution.Status,
            execution.CorrelationId,
            execution.RequestedBy,
            execution.RequestedAt,
            execution.StartedAt,
            execution.FinishedAt,
            execution.TotalEntities,
            execution.TotalRecordsRead,
            execution.TotalEventsPublished,
            execution.TotalSkipped,
            execution.TotalErrors,
            execution.Message
        FROM dbo.SyncProfileExecutions execution
        INNER JOIN dbo.SyncProfiles profile ON profile.Id = execution.SyncProfileId
        INNER JOIN dbo.Companies company ON company.Id = profile.CompanyId
        WHERE (@ProfileId IS NULL OR execution.SyncProfileId = @ProfileId)
          AND (@Status IS NULL OR execution.Status = @Status)
          AND (@ExecutionType IS NULL OR execution.ExecutionType = @ExecutionType)
          AND (@DateFrom IS NULL OR execution.RequestedAt >= @DateFrom)
          AND (@DateTo IS NULL OR execution.RequestedAt < DATEADD(day, 1, @DateTo))
    )
    SELECT *
    FROM Filtered
    ORDER BY RequestedAt DESC, Id DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(1)
    FROM dbo.SyncProfileExecutions execution
    WHERE (@ProfileId IS NULL OR execution.SyncProfileId = @ProfileId)
      AND (@Status IS NULL OR execution.Status = @Status)
      AND (@ExecutionType IS NULL OR execution.ExecutionType = @ExecutionType)
      AND (@DateFrom IS NULL OR execution.RequestedAt >= @DateFrom)
      AND (@DateTo IS NULL OR execution.RequestedAt < DATEADD(day, 1, @DateTo));
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ACTIVE_SYNCPROFILEEXECUTION
    @SyncProfileId int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1) Id
    FROM dbo.SyncProfileExecutions
    WHERE SyncProfileId = @SyncProfileId
      AND Status IN (N'Pending', N'Running', N'Cancelling')
    ORDER BY RequestedAt, Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_PENDING_SYNCPROFILEEXECUTIONS
    @Take int = 5
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (CASE WHEN @Take BETWEEN 1 AND 50 THEN @Take ELSE 5 END)
        execution.Id,
        execution.SyncProfileId,
        profile.Code AS ProfileCode,
        profile.Name AS ProfileName,
        profile.CompanyId,
        company.CommercialName AS CompanyName,
        execution.ExecutionType,
        execution.Status,
        execution.CorrelationId,
        execution.RequestedBy,
        execution.RequestedAt,
        execution.StartedAt,
        execution.FinishedAt,
        execution.CancelledAt,
        execution.CancelledBy,
        execution.EntityCodesJson,
        execution.FromKey,
        execution.MaxRecords,
        execution.TotalEntities,
        execution.TotalRecordsRead,
        execution.TotalEventsPublished,
        execution.TotalSkipped,
        execution.TotalErrors,
        execution.Message
    FROM dbo.SyncProfileExecutions execution
    INNER JOIN dbo.SyncProfiles profile ON profile.Id = execution.SyncProfileId
    INNER JOIN dbo.Companies company ON company.Id = profile.CompanyId
    WHERE execution.Status = N'Pending'
    ORDER BY execution.RequestedAt, execution.Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_UPSERT_SYNCPROFILEEXECUTIONDETAIL
    @SyncProfileExecutionId int,
    @SyncProfileEntityId int,
    @EntityCode nvarchar(80),
    @Status nvarchar(30),
    @TotalRecordsRead int,
    @TotalEventsPublished int,
    @TotalSkipped int,
    @TotalErrors int,
    @LastProcessedKey nvarchar(200) = NULL,
    @Message nvarchar(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    MERGE dbo.SyncProfileExecutionDetails AS target
    USING (SELECT @SyncProfileExecutionId AS SyncProfileExecutionId, @SyncProfileEntityId AS SyncProfileEntityId) AS source
       ON target.SyncProfileExecutionId = source.SyncProfileExecutionId
      AND target.SyncProfileEntityId = source.SyncProfileEntityId
    WHEN MATCHED THEN
        UPDATE SET
            Status = @Status,
            StartedAt = COALESCE(target.StartedAt, SYSUTCDATETIME()),
            FinishedAt = CASE WHEN @Status IN (N'Skipped', N'Cancelled', N'Completed', N'CompletedWithErrors', N'Failed') THEN SYSUTCDATETIME() ELSE target.FinishedAt END,
            TotalRecordsRead = @TotalRecordsRead,
            TotalEventsPublished = @TotalEventsPublished,
            TotalSkipped = @TotalSkipped,
            TotalErrors = @TotalErrors,
            LastProcessedKey = @LastProcessedKey,
            Message = @Message,
            UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (SyncProfileExecutionId, SyncProfileEntityId, EntityCode, Status, StartedAt, FinishedAt, TotalRecordsRead, TotalEventsPublished, TotalSkipped, TotalErrors, LastProcessedKey, Message)
        VALUES (@SyncProfileExecutionId, @SyncProfileEntityId, @EntityCode, @Status, SYSUTCDATETIME(), CASE WHEN @Status IN (N'Skipped', N'Cancelled', N'Completed', N'CompletedWithErrors', N'Failed') THEN SYSUTCDATETIME() ELSE NULL END, @TotalRecordsRead, @TotalEventsPublished, @TotalSkipped, @TotalErrors, @LastProcessedKey, @Message);

    SELECT Id
    FROM dbo.SyncProfileExecutionDetails
    WHERE SyncProfileExecutionId = @SyncProfileExecutionId
      AND SyncProfileEntityId = @SyncProfileEntityId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_DUE_SYNCPROFILES
    @UtcNow datetime2(0)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        profile.Id AS SyncProfileId,
        profile.Code AS ProfileCode,
        profile.Name AS ProfileName,
        profile.CompanyId,
        schedule.ScheduleType,
        schedule.IntervalMinutes,
        schedule.ExecutionTime,
        schedule.TimeZoneId,
        schedule.LastSuccessfulScheduledExecutionAt,
        schedule.CreatedAt AS ConfiguredAt,
        schedule.NextExecutionAt
    FROM dbo.SyncProfiles profile
    INNER JOIN dbo.SyncSchedules schedule
        ON schedule.SyncProfileId = profile.Id
       AND schedule.IsDeleted = 0
       AND schedule.IsActive = 1
    WHERE profile.IsDeleted = 0
      AND profile.IsActive = 1
      AND profile.Direction = N'MasterToBranch'
      AND profile.ExecutionMode = N'Full'
      AND profile.ConflictStrategy = N'MasterWins'
      AND schedule.ScheduleType IN (N'Interval', N'Daily')
      AND (schedule.NextExecutionAt IS NULL OR schedule.NextExecutionAt <= @UtcNow)
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SyncProfileExecutions execution
          WHERE execution.SyncProfileId = profile.Id
            AND execution.Status IN (N'Pending', N'Running', N'Cancelling')
      )
    ORDER BY ISNULL(schedule.NextExecutionAt, schedule.CreatedAt), profile.Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SYNCPROFILEPAGINAR
    @Search nvarchar(150) = NULL,
    @CompanyId int = NULL,
    @IsActive bit = NULL,
    @ExecutionMode nvarchar(20) = NULL,
    @PageNumber int = 1,
    @PageSize int = 50,
    @UserId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @PageNumber IS NULL OR @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize IS NULL OR @PageSize < 1 SET @PageSize = 50;
    IF @PageSize > 500 SET @PageSize = 500;

    DECLARE @Offset int = (@PageNumber - 1) * @PageSize;
    DECLARE @SearchPattern nvarchar(160) = CASE WHEN NULLIF(LTRIM(RTRIM(@Search)), N'') IS NULL THEN NULL ELSE N'%' + LTRIM(RTRIM(@Search)) + N'%' END;

    SELECT
        profile.Id,
        profile.Code,
        profile.Name,
        profile.CompanyId,
        company.CommercialName AS CompanyName,
        (SELECT COUNT(1) FROM dbo.SyncProfileBranches branch WHERE branch.SyncProfileId = profile.Id AND branch.IsDeleted = 0) AS BranchCount,
        (SELECT COUNT(1) FROM dbo.SyncProfileEntities entity WHERE entity.SyncProfileId = profile.Id AND entity.IsDeleted = 0) AS EntityCount,
        profile.Direction,
        profile.ExecutionMode,
        profile.ConflictStrategy,
        profile.BatchSize,
        profile.MaxRetries,
        profile.IsActive,
        lastExecution.FinishedAt AS LastExecutionAt,
        schedule.NextExecutionAt,
        profile.CreatedByUserId,
        profile.CreatedByUserName,
        profile.CreatedAt,
        profile.UpdatedByUserId,
        profile.UpdatedByUserName,
        profile.UpdatedAt
    FROM dbo.SyncProfiles profile
    INNER JOIN dbo.Companies company ON company.Id = profile.CompanyId
    OUTER APPLY
    (
        SELECT TOP (1) execution.FinishedAt
        FROM dbo.SyncProfileExecutions execution
        WHERE execution.SyncProfileId = profile.Id
          AND execution.FinishedAt IS NOT NULL
        ORDER BY execution.FinishedAt DESC, execution.Id DESC
    ) lastExecution
    OUTER APPLY
    (
        SELECT TOP (1) syncSchedule.NextExecutionAt
        FROM dbo.SyncSchedules syncSchedule
        WHERE syncSchedule.SyncProfileId = profile.Id
          AND syncSchedule.IsDeleted = 0
          AND syncSchedule.IsActive = 1
        ORDER BY syncSchedule.CreatedAt DESC, syncSchedule.Id DESC
    ) schedule
    WHERE profile.IsDeleted = 0
      AND (@CompanyId IS NULL OR profile.CompanyId = @CompanyId)
      AND (@IsActive IS NULL OR profile.IsActive = @IsActive)
      AND (@ExecutionMode IS NULL OR profile.ExecutionMode = @ExecutionMode)
      AND (@SearchPattern IS NULL OR profile.Code LIKE @SearchPattern OR profile.Name LIKE @SearchPattern OR company.CommercialName LIKE @SearchPattern)
      AND (@UserId IS NULL OR EXISTS (
          SELECT 1 FROM dbo.UserCompanies uc WHERE uc.UserId = @UserId AND uc.CompanyId = profile.CompanyId AND uc.IsActive = 1))
    ORDER BY company.CommercialName, profile.Code
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(1)
    FROM dbo.SyncProfiles profile
    INNER JOIN dbo.Companies company ON company.Id = profile.CompanyId
    WHERE profile.IsDeleted = 0
      AND (@CompanyId IS NULL OR profile.CompanyId = @CompanyId)
      AND (@IsActive IS NULL OR profile.IsActive = @IsActive)
      AND (@ExecutionMode IS NULL OR profile.ExecutionMode = @ExecutionMode)
      AND (@SearchPattern IS NULL OR profile.Code LIKE @SearchPattern OR profile.Name LIKE @SearchPattern OR company.CommercialName LIKE @SearchPattern)
      AND (@UserId IS NULL OR EXISTS (
          SELECT 1 FROM dbo.UserCompanies uc WHERE uc.UserId = @UserId AND uc.CompanyId = profile.CompanyId AND uc.IsActive = 1));
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_MARK_SYNCPROFILE_SCHEDULED
    @SyncProfileId int,
    @NextExecutionAt datetime2(0)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.SyncSchedules
       SET NextExecutionAt = @NextExecutionAt,
           UpdatedAt = SYSUTCDATETIME()
     WHERE SyncProfileId = @SyncProfileId
       AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SYNCROUTINGTARGETS
    @SourceCompanyId int,
    @EntityCode nvarchar(80),
    @SyncProfileId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NormalizedEntityCode nvarchar(80) = LTRIM(RTRIM(@EntityCode));

    SELECT DISTINCT
        profile.Id AS SyncProfileId,
        entity.Id AS SyncProfileEntityId,
        profile.Code AS SyncProfileCode,
        profile.CompanyId AS SourceCompanyId,
        profileBranch.BranchCompanyId,
        entity.EntityCode,
        COALESCE(matrix.BatchSize, entity.BatchSize, profileBranch.BatchSize, profile.BatchSize) AS BatchSize,
        COALESCE(profileBranch.MaxRetries, profile.MaxRetries) AS MaxRetries,
        profile.RetryDelaySeconds,
        profile.TimeoutMinutes,
        entity.AllowInsert,
        entity.AllowUpdate,
        entity.AllowDeactivate,
        entity.ContinueOnError
    FROM dbo.SyncProfiles AS profile
    INNER JOIN dbo.Companies AS sourceCompany
        ON sourceCompany.Id = profile.CompanyId
       AND sourceCompany.IsActive = 1
       AND sourceCompany.IsMaster = 1
       AND sourceCompany.SyncEnabled = 1
       AND sourceCompany.IsDeleted = 0
    INNER JOIN dbo.SyncProfileEntities AS entity
        ON entity.SyncProfileId = profile.Id
       AND entity.IsDeleted = 0
       AND entity.IsActive = 1
       AND entity.EntityCode = @NormalizedEntityCode
       AND (@SyncProfileId IS NOT NULL OR entity.SyncMode = N'Incremental')
    INNER JOIN dbo.SyncProfileEntityBranches AS matrix
        ON matrix.SyncProfileId = profile.Id
       AND matrix.SyncProfileEntityId = entity.Id
       AND matrix.IsDeleted = 0
       AND matrix.IsEnabled = 1
    INNER JOIN dbo.SyncProfileBranches AS profileBranch
        ON profileBranch.Id = matrix.SyncProfileBranchId
       AND profileBranch.SyncProfileId = profile.Id
       AND profileBranch.IsDeleted = 0
       AND profileBranch.IsActive = 1
    INNER JOIN dbo.Companies AS branchCompany
        ON branchCompany.Id = profileBranch.BranchCompanyId
       AND branchCompany.IsActive = 1
       AND branchCompany.IsMaster = 0
       AND branchCompany.SyncEnabled = 1
       AND branchCompany.ParentCompanyId = profile.CompanyId
       AND branchCompany.IsDeleted = 0
    WHERE profile.CompanyId = @SourceCompanyId
      AND profile.IsDeleted = 0
      AND profile.IsActive = 1
      AND profile.Direction = N'MasterToBranch'
      AND profile.ConflictStrategy = N'MasterWins'
      AND (
            (@SyncProfileId IS NULL AND profile.ExecutionMode = N'Incremental')
         OR (@SyncProfileId IS NOT NULL AND profile.Id = @SyncProfileId AND profile.ExecutionMode IN (N'Incremental', N'Full', N'Manual'))
      )
    ORDER BY profileBranch.BranchCompanyId, profile.Id, entity.Id;
END;
GO

IF OBJECT_ID(N'dbo.Modules', N'U') IS NOT NULL
    AND OBJECT_ID(N'dbo.Permissions', N'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'SYNC')
    BEGIN
        INSERT INTO dbo.Modules (Code, Name, DisplayOrder)
        VALUES (N'SYNC', N'Sincronizacion Master/Sucursal', 70);
    END;

    DECLARE @SyncModuleId int = (SELECT TOP (1) Id FROM dbo.Modules WHERE Code = N'SYNC');

    MERGE dbo.Permissions AS target
    USING
    (
        VALUES
            (@SyncModuleId, N'SYNC.CONFIGURATION.EXECUTE', N'Sync Configuration Execute', N'Ejecutar perfiles Maestro-Sucursal manualmente.'),
            (@SyncModuleId, N'SYNC.CONFIGURATION.VIEWEXECUTIONS', N'Sync Configuration View Executions', N'Consultar ejecuciones administrativas Maestro-Sucursal.'),
            (@SyncModuleId, N'SYNC.CONFIGURATION.CANCEL', N'Sync Configuration Cancel', N'Cancelar ejecuciones administrativas Maestro-Sucursal.'),
            (@SyncModuleId, N'SYNC.CONFIGURATION.RETRY', N'Sync Configuration Retry', N'Reintentar ejecuciones administrativas Maestro-Sucursal.')
    ) AS source (ModuleId, Code, Name, Description)
       ON target.Code = source.Code
    WHEN MATCHED THEN
        UPDATE SET ModuleId = source.ModuleId, Name = source.Name, Description = source.Description
    WHEN NOT MATCHED THEN
        INSERT (ModuleId, Code, Name, Description)
        VALUES (source.ModuleId, source.Code, source.Name, source.Description);
END;
GO

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260711.071')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description, AppliedAt)
    VALUES (N'20260711.071', N'Ejecucion administrativa de perfiles Sync Master-Branch', SYSUTCDATETIME());
END;
GO
