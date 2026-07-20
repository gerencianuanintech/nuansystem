/*
    Fase 5.2 - Cola durable para consulta y descarga de documentos autorizados del SRI.
    Ejecutar en cada base tenant. Este script no consulta al SRI ni almacena XML.
*/
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SriDocumentQueue', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SriDocumentQueue
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SriDocumentQueue PRIMARY KEY,
        Environment nvarchar(20) NOT NULL,
        AccessKey char(49) NOT NULL,
        DocumentTypeCode char(2) NOT NULL,
        SourceType nvarchar(30) NOT NULL,
        SourceReference nvarchar(200) NOT NULL,
        BranchCode nvarchar(50) NULL,
        Status nvarchar(30) NOT NULL CONSTRAINT DF_SriDocumentQueue_Status DEFAULT N'Pending',
        Priority tinyint NOT NULL CONSTRAINT DF_SriDocumentQueue_Priority DEFAULT 5,
        AttemptCount int NOT NULL CONSTRAINT DF_SriDocumentQueue_AttemptCount DEFAULT 0,
        MaxAttempts int NULL,
        NextAttemptAt datetime2(0) NULL,
        TraceId uniqueidentifier NOT NULL,
        LastErrorCode nvarchar(100) NULL,
        LastErrorMessage nvarchar(2000) NULL,
        LockedBy nvarchar(200) NULL,
        LockedAt datetime2(0) NULL,
        LockExpiresAt datetime2(0) NULL,
        CompletedAt datetime2(0) NULL,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(150) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SriDocumentQueue_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(150) NULL,
        UpdatedAt datetime2(0) NULL,
        RowVersion rowversion NOT NULL,
        CONSTRAINT CK_SriDocumentQueue_Environment CHECK (Environment IN (N'Test', N'Production')),
        CONSTRAINT CK_SriDocumentQueue_DocumentType CHECK (DocumentTypeCode IN ('01', '04', '07')),
        CONSTRAINT CK_SriDocumentQueue_SourceType CHECK (SourceType IN (N'NuanSystem', N'Txt', N'SapAddOn', N'Manual', N'ExternalApi')),
        CONSTRAINT CK_SriDocumentQueue_Status CHECK (Status IN (N'Pending', N'Querying', N'RetryScheduled', N'Authorized', N'NotFound', N'Failed', N'DeadLetter', N'Cancelled')),
        CONSTRAINT CK_SriDocumentQueue_Priority CHECK (Priority BETWEEN 1 AND 9),
        CONSTRAINT CK_SriDocumentQueue_Attempts CHECK (AttemptCount >= 0 AND (MaxAttempts IS NULL OR MaxAttempts > 0))
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.SriDocumentQueue') AND name = N'UX_SriDocumentQueue_Environment_AccessKey')
    CREATE UNIQUE INDEX UX_SriDocumentQueue_Environment_AccessKey ON dbo.SriDocumentQueue(Environment, AccessKey);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.SriDocumentQueue') AND name = N'IX_SriDocumentQueue_Claim')
    CREATE INDEX IX_SriDocumentQueue_Claim ON dbo.SriDocumentQueue(Status, NextAttemptAt, Priority, CreatedAt) INCLUDE (Environment, AccessKey, AttemptCount, MaxAttempts, LockExpiresAt);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.SriDocumentQueue') AND name = N'IX_SriDocumentQueue_Source')
    CREATE INDEX IX_SriDocumentQueue_Source ON dbo.SriDocumentQueue(SourceType, SourceReference, CreatedAt DESC);
GO

IF OBJECT_ID(N'dbo.SriDocumentAttempts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SriDocumentAttempts
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SriDocumentAttempts PRIMARY KEY,
        QueueId bigint NOT NULL,
        AttemptNumber int NOT NULL,
        Action nvarchar(50) NOT NULL,
        ResultStatus nvarchar(30) NOT NULL,
        ErrorCategory nvarchar(100) NULL,
        ErrorCode nvarchar(100) NULL,
        ErrorMessage nvarchar(2000) NULL,
        RemoteCorrelationId nvarchar(200) NULL,
        StartedAt datetime2(0) NOT NULL,
        CompletedAt datetime2(0) NULL,
        DurationMs int NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SriDocumentAttempts_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_SriDocumentAttempts_Queue FOREIGN KEY (QueueId) REFERENCES dbo.SriDocumentQueue(Id),
        CONSTRAINT UQ_SriDocumentAttempts_Queue_Attempt UNIQUE (QueueId, AttemptNumber),
        CONSTRAINT CK_SriDocumentAttempts_Attempt CHECK (AttemptNumber > 0),
        CONSTRAINT CK_SriDocumentAttempts_Duration CHECK (DurationMs IS NULL OR DurationMs >= 0)
    );
END;
GO

IF OBJECT_ID(N'dbo.AuditSriDocumentChanges', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuditSriDocumentChanges
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditSriDocumentChanges PRIMARY KEY,
        QueueId bigint NOT NULL,
        Action nvarchar(50) NOT NULL,
        PreviousStatus nvarchar(30) NULL,
        NewStatus nvarchar(30) NOT NULL,
        Reason nvarchar(500) NULL,
        UserId int NULL,
        UserName nvarchar(150) NULL,
        TraceId uniqueidentifier NOT NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_AuditSriDocumentChanges_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_AuditSriDocumentChanges_Queue FOREIGN KEY (QueueId) REFERENCES dbo.SriDocumentQueue(Id)
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SRIDOCUMENTQUEUE_ENCOLAR
    @Environment nvarchar(20), @AccessKey char(49), @DocumentTypeCode char(2),
    @SourceType nvarchar(30), @SourceReference nvarchar(200), @BranchCode nvarchar(50) = NULL,
    @Priority int = 5, @TraceId uniqueidentifier, @AuditUserId int = NULL, @AuditUserName nvarchar(150) = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
    BEGIN TRANSACTION;
    DECLARE @Id bigint;
    SELECT @Id = Id FROM dbo.SriDocumentQueue WITH (UPDLOCK, HOLDLOCK)
    WHERE Environment = @Environment AND AccessKey = @AccessKey;

    IF @Id IS NULL
    BEGIN
        INSERT dbo.SriDocumentQueue(Environment, AccessKey, DocumentTypeCode, SourceType, SourceReference, BranchCode, Status, Priority, TraceId, CreatedByUserId, CreatedByUserName)
        VALUES(@Environment, @AccessKey, @DocumentTypeCode, @SourceType, @SourceReference, @BranchCode, N'Pending', CONVERT(tinyint, @Priority), @TraceId, @AuditUserId, @AuditUserName);
        SET @Id = SCOPE_IDENTITY();
        INSERT dbo.AuditSriDocumentChanges(QueueId, Action, PreviousStatus, NewStatus, UserId, UserName, TraceId)
        VALUES(@Id, N'Enqueue', NULL, N'Pending', @AuditUserId, @AuditUserName, @TraceId);
        COMMIT;
        SELECT q.*, CAST(1 AS bit) AS IsCreated FROM dbo.SriDocumentQueue q WHERE q.Id = @Id;
        RETURN;
    END;

    COMMIT;
    SELECT q.*, CAST(0 AS bit) AS IsCreated FROM dbo.SriDocumentQueue q WHERE q.Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SRIDOCUMENTQUEUE_LISTAR
    @Environment nvarchar(20) = NULL, @Status nvarchar(30) = NULL, @SourceType nvarchar(30) = NULL,
    @AccessKey varchar(49) = NULL, @CreatedFrom datetime2(0) = NULL, @CreatedTo datetime2(0) = NULL,
    @Page int = 1, @PageSize int = 100
AS
BEGIN
    SET NOCOUNT ON;
    SET @Page = CASE WHEN @Page < 1 THEN 1 ELSE @Page END;
    SET @PageSize = CASE WHEN @PageSize < 1 THEN 100 WHEN @PageSize > 500 THEN 500 ELSE @PageSize END;
    SELECT Id, Environment, AccessKey, DocumentTypeCode, SourceType, SourceReference, BranchCode, Status, Priority,
           AttemptCount, MaxAttempts, NextAttemptAt, CreatedAt, UpdatedAt, CompletedAt, LastErrorCode
    FROM dbo.SriDocumentQueue
    WHERE (@Environment IS NULL OR Environment = @Environment)
      AND (@Status IS NULL OR Status = @Status)
      AND (@SourceType IS NULL OR SourceType = @SourceType)
      AND (@AccessKey IS NULL OR AccessKey = @AccessKey)
      AND (@CreatedFrom IS NULL OR CreatedAt >= @CreatedFrom)
      AND (@CreatedTo IS NULL OR CreatedAt < DATEADD(day, 1, @CreatedTo))
    ORDER BY CreatedAt DESC, Id DESC
    OFFSET (@Page - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SRIDOCUMENTQUEUE_BUSCARPORID @Id bigint
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Environment, AccessKey, DocumentTypeCode, SourceType, SourceReference, BranchCode, Status, Priority,
           AttemptCount, MaxAttempts, NextAttemptAt, TraceId, LastErrorCode, LastErrorMessage, LockedBy, LockedAt,
           LockExpiresAt, CompletedAt, CreatedByUserId, CreatedByUserName, CreatedAt, UpdatedByUserId,
           UpdatedByUserName, UpdatedAt, RowVersion
    FROM dbo.SriDocumentQueue WHERE Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SRIDOCUMENTQUEUE_INTENTOS @QueueId bigint
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, QueueId, AttemptNumber, Action, ResultStatus, ErrorCategory, ErrorCode, ErrorMessage,
           RemoteCorrelationId, StartedAt, CompletedAt, DurationMs, CreatedAt
    FROM dbo.SriDocumentAttempts WHERE QueueId = @QueueId ORDER BY AttemptNumber DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PATCH_SRIDOCUMENTQUEUE_CANCELAR
    @Id bigint, @RowVersion binary(8), @Reason nvarchar(500) = NULL, @AuditUserId int = NULL, @AuditUserName nvarchar(150) = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    BEGIN TRANSACTION;
    DECLARE @CurrentStatus nvarchar(30), @TraceId uniqueidentifier;
    SELECT @CurrentStatus = Status, @TraceId = TraceId FROM dbo.SriDocumentQueue WITH (UPDLOCK) WHERE Id = @Id;
    IF @CurrentStatus IS NULL BEGIN ROLLBACK; SELECT 0; RETURN; END;
    IF NOT EXISTS(SELECT 1 FROM dbo.SriDocumentQueue WHERE Id = @Id AND RowVersion = @RowVersion) BEGIN ROLLBACK; SELECT -2; RETURN; END;
    IF @CurrentStatus NOT IN (N'Pending', N'RetryScheduled') BEGIN ROLLBACK; SELECT -3; RETURN; END;
    UPDATE dbo.SriDocumentQueue SET Status=N'Cancelled', CompletedAt=SYSUTCDATETIME(), NextAttemptAt=NULL,
        LockedBy=NULL, LockedAt=NULL, LockExpiresAt=NULL, UpdatedByUserId=@AuditUserId, UpdatedByUserName=@AuditUserName, UpdatedAt=SYSUTCDATETIME()
    WHERE Id=@Id AND RowVersion=@RowVersion;
    IF @@ROWCOUNT = 0 BEGIN ROLLBACK; SELECT -2; RETURN; END;
    INSERT dbo.AuditSriDocumentChanges(QueueId, Action, PreviousStatus, NewStatus, Reason, UserId, UserName, TraceId)
    VALUES(@Id, N'Cancel', @CurrentStatus, N'Cancelled', @Reason, @AuditUserId, @AuditUserName, @TraceId);
    COMMIT; SELECT 1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PATCH_SRIDOCUMENTQUEUE_REPROCESAR
    @Id bigint, @RowVersion binary(8), @Reason nvarchar(500), @AuditUserId int = NULL, @AuditUserName nvarchar(150) = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    BEGIN TRANSACTION;
    DECLARE @CurrentStatus nvarchar(30), @TraceId uniqueidentifier;
    SELECT @CurrentStatus = Status, @TraceId = TraceId FROM dbo.SriDocumentQueue WITH (UPDLOCK) WHERE Id = @Id;
    IF @CurrentStatus IS NULL BEGIN ROLLBACK; SELECT 0; RETURN; END;
    IF NOT EXISTS(SELECT 1 FROM dbo.SriDocumentQueue WHERE Id = @Id AND RowVersion = @RowVersion) BEGIN ROLLBACK; SELECT -2; RETURN; END;
    IF @CurrentStatus NOT IN (N'Failed', N'DeadLetter') BEGIN ROLLBACK; SELECT -3; RETURN; END;
    UPDATE dbo.SriDocumentQueue SET Status=N'Pending', AttemptCount=0, NextAttemptAt=NULL, CompletedAt=NULL,
        LastErrorCode=NULL, LastErrorMessage=NULL, LockedBy=NULL, LockedAt=NULL, LockExpiresAt=NULL,
        UpdatedByUserId=@AuditUserId, UpdatedByUserName=@AuditUserName, UpdatedAt=SYSUTCDATETIME()
    WHERE Id=@Id AND RowVersion=@RowVersion;
    IF @@ROWCOUNT = 0 BEGIN ROLLBACK; SELECT -2; RETURN; END;
    INSERT dbo.AuditSriDocumentChanges(QueueId, Action, PreviousStatus, NewStatus, Reason, UserId, UserName, TraceId)
    VALUES(@Id, N'Reprocess', @CurrentStatus, N'Pending', @Reason, @AuditUserId, @AuditUserName, @TraceId);
    COMMIT; SELECT 1;
END;
GO

IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.SchemaHistory WHERE Version = N'20260720.115')
    INSERT dbo.SchemaHistory(Version, Description) VALUES(N'20260720.115', N'Cola durable y auditoria de consultas de documentos autorizados SRI');
GO
