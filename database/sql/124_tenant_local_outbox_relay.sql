/*
    Iteracion 8.1 - LocalOutbox durable relay.
    Forward-only e idempotente. No habilita ningun worker.
*/
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF COL_LENGTH(N'dbo.LocalOutbox', N'LockedBy') IS NULL
    ALTER TABLE dbo.LocalOutbox ADD LockedBy nvarchar(120) NULL;
IF COL_LENGTH(N'dbo.LocalOutbox', N'LockedAt') IS NULL
    ALTER TABLE dbo.LocalOutbox ADD LockedAt datetime2(0) NULL;
IF COL_LENGTH(N'dbo.LocalOutbox', N'LockExpiresAt') IS NULL
    ALTER TABLE dbo.LocalOutbox ADD LockExpiresAt datetime2(0) NULL;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id=OBJECT_ID(N'dbo.LocalOutbox')
      AND name=N'IX_LocalOutbox_RelayClaim'
)
BEGIN
    CREATE INDEX IX_LocalOutbox_RelayClaim
    ON dbo.LocalOutbox(Status,NextRetryAt,LockExpiresAt,CreatedAt)
    INCLUDE(EventId,CompanyId,AttemptCount,MaxAttempts);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_LOCALOUTBOX_LIBERARLEASESVENCIDOS
    @WorkerInstance nvarchar(120)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Now datetime2(0)=SYSUTCDATETIME();
    DECLARE @Released table(Id bigint PRIMARY KEY);

    UPDATE dbo.LocalOutbox
    SET Status=CASE WHEN AttemptCount>=MaxAttempts THEN N'DeadLetter' ELSE N'Error' END,
        NextRetryAt=CASE WHEN AttemptCount>=MaxAttempts THEN NULL ELSE @Now END,
        LockedBy=NULL,LockedAt=NULL,LockExpiresAt=NULL,
        LastErrorMessage=N'Lease vencido liberado por relay.'
    OUTPUT inserted.Id INTO @Released(Id)
    WHERE Status=N'InProcess' AND LockExpiresAt<@Now;

    INSERT dbo.SyncAudit
        (CompanyId,EventId,EntityName,EntityGlobalId,[Action],PreviousStatus,NewStatus,[Message],CreatedBy)
    SELECT item.CompanyId,item.EventId,item.EntityName,item.EntityGlobalId,N'Failed',N'InProcess',item.Status,
           N'Lease local vencido liberado.',@WorkerInstance
    FROM dbo.LocalOutbox item INNER JOIN @Released released ON released.Id=item.Id;

    SELECT COUNT(1) FROM @Released;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_LOCALOUTBOX_RECLAMAR
    @WorkerInstance nvarchar(120),
    @BatchSize int,
    @LeaseSeconds int
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @Now datetime2(0)=SYSUTCDATETIME();
    DECLARE @Claimed table(Id bigint PRIMARY KEY);

    BEGIN TRANSACTION;
    ;WITH Candidates AS
    (
        SELECT TOP (@BatchSize) Id
        FROM dbo.LocalOutbox WITH (UPDLOCK,READPAST,ROWLOCK)
        WHERE Status IN (N'Pending',N'Error')
          AND (NextRetryAt IS NULL OR NextRetryAt<=@Now)
          AND (LockExpiresAt IS NULL OR LockExpiresAt<=@Now)
          AND AttemptCount<MaxAttempts
        ORDER BY CreatedAt,Id
    )
    UPDATE item
    SET Status=N'InProcess',
        AttemptCount=AttemptCount+1,
        LockedBy=@WorkerInstance,
        LockedAt=@Now,
        LockExpiresAt=DATEADD(SECOND,@LeaseSeconds,@Now),
        LastErrorMessage=NULL
    OUTPUT inserted.Id INTO @Claimed(Id)
    FROM dbo.LocalOutbox item
    INNER JOIN Candidates candidate ON candidate.Id=item.Id;
    INSERT dbo.SyncAudit
        (CompanyId,EventId,EntityName,EntityGlobalId,[Action],PreviousStatus,NewStatus,[Message],CreatedBy)
    SELECT item.CompanyId,item.EventId,item.EntityName,item.EntityGlobalId,N'Claimed',
           CASE WHEN item.AttemptCount=1 THEN N'Pending' ELSE N'Error' END,N'InProcess',
           N'Evento LocalOutbox reclamado por relay.',@WorkerInstance
    FROM dbo.LocalOutbox item INNER JOIN @Claimed claimed ON claimed.Id=item.Id;
    COMMIT TRANSACTION;

    SELECT item.*
    FROM dbo.LocalOutbox item
    INNER JOIN @Claimed claimed ON claimed.Id=item.Id
    ORDER BY item.CreatedAt,item.Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_LOCALOUTBOX_COMPLETARPROMOCION
    @Id bigint,
    @WorkerInstance nvarchar(120)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Completed table(Id bigint PRIMARY KEY);
    UPDATE dbo.LocalOutbox
    SET Status=N'Applied',ProcessedAt=SYSUTCDATETIME(),NextRetryAt=NULL,
        LockedBy=NULL,LockedAt=NULL,LockExpiresAt=NULL,LastErrorMessage=NULL
    OUTPUT inserted.Id INTO @Completed(Id)
    WHERE Id=@Id AND Status=N'InProcess' AND LockedBy=@WorkerInstance;

    INSERT dbo.SyncAudit
        (CompanyId,EventId,EntityName,EntityGlobalId,[Action],PreviousStatus,NewStatus,[Message],CreatedBy)
    SELECT item.CompanyId,item.EventId,item.EntityName,item.EntityGlobalId,N'Applied',N'InProcess',N'Applied',
           N'Evento promovido a SyncOutbox Master.',@WorkerInstance
    FROM dbo.LocalOutbox item INNER JOIN @Completed completed ON completed.Id=item.Id;
    SELECT COUNT(1) FROM @Completed;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_LOCALOUTBOX_PROGRAMARREINTENTO
    @Id bigint,
    @WorkerInstance nvarchar(120),
    @ErrorMessage nvarchar(max),
    @RetrySeconds int
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Retried table(Id bigint PRIMARY KEY);
    UPDATE dbo.LocalOutbox
    SET Status=CASE WHEN AttemptCount>=MaxAttempts THEN N'DeadLetter' ELSE N'Error' END,
        NextRetryAt=CASE WHEN AttemptCount>=MaxAttempts THEN NULL ELSE DATEADD(SECOND,@RetrySeconds,SYSUTCDATETIME()) END,
        ProcessedAt=CASE WHEN AttemptCount>=MaxAttempts THEN SYSUTCDATETIME() ELSE NULL END,
        LockedBy=NULL,LockedAt=NULL,LockExpiresAt=NULL,
        LastErrorMessage=LEFT(@ErrorMessage,4000)
    OUTPUT inserted.Id INTO @Retried(Id)
    WHERE Id=@Id AND Status=N'InProcess' AND LockedBy=@WorkerInstance;

    INSERT dbo.SyncAudit
        (CompanyId,EventId,EntityName,EntityGlobalId,[Action],PreviousStatus,NewStatus,[Message],CreatedBy)
    SELECT item.CompanyId,item.EventId,item.EntityName,item.EntityGlobalId,
           CASE WHEN item.Status=N'DeadLetter' THEN N'DeadLetter' ELSE N'Retried' END,
           N'InProcess',item.Status,N'Promocion LocalOutbox no completada; estado actualizado.',@WorkerInstance
    FROM dbo.LocalOutbox item INNER JOIN @Retried retried ON retried.Id=item.Id;
    SELECT COUNT(1) FROM @Retried;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_LOCALOUTBOX_COMPLETARCONFLICTO
    @Id bigint,
    @WorkerInstance nvarchar(120),
    @ErrorMessage nvarchar(max)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Conflicted table(Id bigint PRIMARY KEY);
    UPDATE dbo.LocalOutbox
    SET Status=N'DeadLetter',ProcessedAt=SYSUTCDATETIME(),NextRetryAt=NULL,
        LockedBy=NULL,LockedAt=NULL,LockExpiresAt=NULL,
        LastErrorMessage=LEFT(@ErrorMessage,4000)
    OUTPUT inserted.Id INTO @Conflicted(Id)
    WHERE Id=@Id AND Status=N'InProcess' AND LockedBy=@WorkerInstance;

    INSERT dbo.SyncAudit
        (CompanyId,EventId,EntityName,EntityGlobalId,[Action],PreviousStatus,NewStatus,[Message],CreatedBy)
    SELECT item.CompanyId,item.EventId,item.EntityName,item.EntityGlobalId,N'DeadLetter',
           N'InProcess',N'DeadLetter',N'Conflicto terminal de EventId durante promocion.',@WorkerInstance
    FROM dbo.LocalOutbox item INNER JOIN @Conflicted conflicted ON conflicted.Id=item.Id;
    SELECT COUNT(1) FROM @Conflicted;
END;
GO

IF NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260725.124')
BEGIN
    INSERT dbo.SchemaHistory(Version,Description)
    VALUES(N'20260725.124',N'Iteracion 8.1: leases y contratos de relay para LocalOutbox');
END;
GO
