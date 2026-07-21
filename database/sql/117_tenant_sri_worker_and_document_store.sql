/* Fase 5.3: claim/lease, intentos y almacenamiento inmutable de XML autorizado SRI. Tenant only. */
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SriAuthorizedDocuments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SriAuthorizedDocuments
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SriAuthorizedDocuments PRIMARY KEY,
        QueueId bigint NOT NULL,
        Environment nvarchar(20) NOT NULL,
        AccessKey char(49) NOT NULL,
        AuthorizationNumber nvarchar(100) NOT NULL,
        AuthorizationAt datetimeoffset(0) NOT NULL,
        ProviderEnvironment nvarchar(50) NOT NULL,
        IssuerRuc char(13) NOT NULL,
        DocumentTypeCode char(2) NOT NULL,
        XmlContent varbinary(max) NOT NULL,
        Sha256 binary(32) NOT NULL,
        ContentType nvarchar(100) NOT NULL,
        SizeBytes int NOT NULL,
        AttemptId bigint NOT NULL,
        StoredAt datetime2(0) NOT NULL CONSTRAINT DF_SriAuthorizedDocuments_StoredAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_SriAuthorizedDocuments_Queue FOREIGN KEY(QueueId) REFERENCES dbo.SriDocumentQueue(Id),
        CONSTRAINT FK_SriAuthorizedDocuments_Attempt FOREIGN KEY(AttemptId) REFERENCES dbo.SriDocumentAttempts(Id),
        CONSTRAINT UQ_SriAuthorizedDocuments_Queue UNIQUE(QueueId),
        CONSTRAINT UQ_SriAuthorizedDocuments_Identity UNIQUE(Environment, AccessKey),
        CONSTRAINT CK_SriAuthorizedDocuments_Size CHECK(SizeBytes > 0 AND SizeBytes <= 5242880 AND DATALENGTH(XmlContent)=SizeBytes),
        CONSTRAINT CK_SriAuthorizedDocuments_Hash CHECK(DATALENGTH(Sha256)=32)
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SRIDOCUMENTQUEUE_LIBERARLEASESVENCIDOS
    @WorkerInstance nvarchar(200), @MaxAttempts int
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @Released table(QueueId bigint, AttemptNumber int, TraceId uniqueidentifier, PreviousStatus nvarchar(30), NewStatus nvarchar(30));
    BEGIN TRANSACTION;
    UPDATE q WITH (UPDLOCK, READPAST, ROWLOCK)
       SET Status=CASE WHEN AttemptCount >= COALESCE(MaxAttempts,@MaxAttempts) THEN N'DeadLetter' ELSE N'RetryScheduled' END,
           NextAttemptAt=CASE WHEN AttemptCount >= COALESCE(MaxAttempts,@MaxAttempts) THEN NULL ELSE SYSUTCDATETIME() END,
           CompletedAt=CASE WHEN AttemptCount >= COALESCE(MaxAttempts,@MaxAttempts) THEN SYSUTCDATETIME() ELSE NULL END,
           LastErrorCode=N'SRI_LEASE_EXPIRED', LastErrorMessage=N'El lease del worker vencio antes de completar el intento.',
           LockedBy=NULL, LockedAt=NULL, LockExpiresAt=NULL, UpdatedByUserName=@WorkerInstance, UpdatedAt=SYSUTCDATETIME()
    OUTPUT inserted.Id, inserted.AttemptCount, inserted.TraceId, deleted.Status, inserted.Status INTO @Released
    FROM dbo.SriDocumentQueue q
    WHERE q.Status=N'Querying' AND q.LockExpiresAt <= SYSUTCDATETIME();

    UPDATE a SET ResultStatus=N'LeaseExpired', ErrorCategory=N'Lease', ErrorCode=N'SRI_LEASE_EXPIRED',
        ErrorMessage=N'El lease vencio antes de completar el intento.', CompletedAt=SYSUTCDATETIME(),
        DurationMs=DATEDIFF(millisecond,a.StartedAt,SYSUTCDATETIME())
    FROM dbo.SriDocumentAttempts a INNER JOIN @Released r ON r.QueueId=a.QueueId AND r.AttemptNumber=a.AttemptNumber
    WHERE a.CompletedAt IS NULL;

    INSERT dbo.AuditSriDocumentChanges(QueueId,Action,PreviousStatus,NewStatus,Reason,UserName,TraceId)
    SELECT QueueId,N'LeaseExpired',PreviousStatus,NewStatus,N'Lease vencido recuperado atomicamente.',@WorkerInstance,TraceId FROM @Released;
    DECLARE @Count int=(SELECT COUNT(1) FROM @Released); COMMIT; SELECT @Count;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SRIDOCUMENTQUEUE_RECLAMAR
    @Environment nvarchar(20), @WorkerInstance nvarchar(200), @BatchSize int, @LeaseSeconds int, @MaxAttempts int
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @Claimed table(Id bigint,Environment nvarchar(20),AccessKey char(49),DocumentTypeCode char(2),SourceType nvarchar(30),
        SourceReference nvarchar(200),BranchCode nvarchar(50),AttemptCount int,MaxAttempts int,TraceId uniqueidentifier,
        CreatedAt datetime2(0),LockedBy nvarchar(200),LockExpiresAt datetime2(0),PreviousStatus nvarchar(30));
    BEGIN TRANSACTION;
    ;WITH candidates AS
    (
        SELECT TOP (@BatchSize) * FROM dbo.SriDocumentQueue WITH (UPDLOCK,READPAST,ROWLOCK)
        WHERE Environment=@Environment AND Status IN (N'Pending',N'RetryScheduled') AND (NextAttemptAt IS NULL OR NextAttemptAt <= SYSUTCDATETIME())
          AND AttemptCount < COALESCE(MaxAttempts,@MaxAttempts)
        ORDER BY Priority ASC, CreatedAt ASC, Id ASC
    )
    UPDATE candidates SET Status=N'Querying', AttemptCount=AttemptCount+1, MaxAttempts=COALESCE(MaxAttempts,@MaxAttempts),
        LockedBy=@WorkerInstance, LockedAt=SYSUTCDATETIME(), LockExpiresAt=DATEADD(second,@LeaseSeconds,SYSUTCDATETIME()),
        NextAttemptAt=NULL, UpdatedByUserName=@WorkerInstance, UpdatedAt=SYSUTCDATETIME()
    OUTPUT inserted.Id,inserted.Environment,inserted.AccessKey,inserted.DocumentTypeCode,inserted.SourceType,inserted.SourceReference,
        inserted.BranchCode,inserted.AttemptCount,inserted.MaxAttempts,inserted.TraceId,inserted.CreatedAt,inserted.LockedBy,inserted.LockExpiresAt,
        deleted.Status
    INTO @Claimed;

    INSERT dbo.SriDocumentAttempts(QueueId,AttemptNumber,Action,ResultStatus,StartedAt)
    SELECT Id,AttemptCount,N'AuthorizationLookup',N'InProgress',SYSUTCDATETIME() FROM @Claimed;
    INSERT dbo.AuditSriDocumentChanges(QueueId,Action,PreviousStatus,NewStatus,Reason,UserName,TraceId)
    SELECT Id,N'Claim',PreviousStatus,N'Querying',N'Lease adquirido por SRI Worker.',@WorkerInstance,TraceId FROM @Claimed;
    COMMIT;
    SELECT Id,Environment,AccessKey,DocumentTypeCode,SourceType,SourceReference,BranchCode,AttemptCount,MaxAttempts,
           TraceId,CreatedAt,LockedBy,LockExpiresAt
    FROM @Claimed ORDER BY Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SRIDOCUMENTQUEUE_COMPLETARAUTORIZADO
    @QueueId bigint,@WorkerInstance nvarchar(200),@AttemptNumber int,@AuthorizationNumber nvarchar(100),
    @AuthorizationAt datetimeoffset(0),@ProviderEnvironment nvarchar(50),@IssuerRuc char(13),@DocumentTypeCode char(2),
    @XmlContent varbinary(max),@Sha256 binary(32),@ContentType nvarchar(100),@RemoteCorrelationId nvarchar(200)=NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    BEGIN TRANSACTION;
    DECLARE @Status nvarchar(30),@LockedBy nvarchar(200),@Environment nvarchar(20),@AccessKey char(49),@TraceId uniqueidentifier,@AttemptId bigint;
    SELECT @Status=Status,@LockedBy=LockedBy,@Environment=Environment,@AccessKey=AccessKey,@TraceId=TraceId
    FROM dbo.SriDocumentQueue WITH(UPDLOCK) WHERE Id=@QueueId;
    IF @Status IS NULL BEGIN ROLLBACK; SELECT 0; RETURN; END;
    IF @Status<>N'Querying' BEGIN ROLLBACK; SELECT -3; RETURN; END;
    IF @LockedBy<>@WorkerInstance BEGIN ROLLBACK; SELECT -2; RETURN; END;
    SELECT @AttemptId=Id FROM dbo.SriDocumentAttempts WITH(UPDLOCK) WHERE QueueId=@QueueId AND AttemptNumber=@AttemptNumber AND CompletedAt IS NULL;
    IF @AttemptId IS NULL BEGIN ROLLBACK; SELECT -2; RETURN; END;
    IF EXISTS(SELECT 1 FROM dbo.SriAuthorizedDocuments WHERE QueueId=@QueueId AND Sha256<>@Sha256) BEGIN ROLLBACK; SELECT -4; RETURN; END;
    IF NOT EXISTS(SELECT 1 FROM dbo.SriAuthorizedDocuments WHERE QueueId=@QueueId)
        INSERT dbo.SriAuthorizedDocuments(QueueId,Environment,AccessKey,AuthorizationNumber,AuthorizationAt,ProviderEnvironment,IssuerRuc,
            DocumentTypeCode,XmlContent,Sha256,ContentType,SizeBytes,AttemptId)
        VALUES(@QueueId,@Environment,@AccessKey,@AuthorizationNumber,@AuthorizationAt,@ProviderEnvironment,@IssuerRuc,
            @DocumentTypeCode,@XmlContent,@Sha256,@ContentType,DATALENGTH(@XmlContent),@AttemptId);
    UPDATE dbo.SriDocumentAttempts SET ResultStatus=N'Authorized',RemoteCorrelationId=@RemoteCorrelationId,CompletedAt=SYSUTCDATETIME(),
        DurationMs=DATEDIFF(millisecond,StartedAt,SYSUTCDATETIME()) WHERE Id=@AttemptId;
    UPDATE dbo.SriDocumentQueue SET Status=N'Authorized',CompletedAt=SYSUTCDATETIME(),LastErrorCode=NULL,LastErrorMessage=NULL,
        LockedBy=NULL,LockedAt=NULL,LockExpiresAt=NULL,UpdatedByUserName=@WorkerInstance,UpdatedAt=SYSUTCDATETIME() WHERE Id=@QueueId;
    INSERT dbo.AuditSriDocumentChanges(QueueId,Action,PreviousStatus,NewStatus,Reason,UserName,TraceId)
    VALUES(@QueueId,N'Authorized',N'Querying',N'Authorized',N'XML autorizado almacenado con integridad SHA-256.',@WorkerInstance,@TraceId);
    COMMIT; SELECT 1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SRIDOCUMENTQUEUE_COMPLETARINTENTO
    @QueueId bigint,@WorkerInstance nvarchar(200),@AttemptNumber int,@Outcome nvarchar(20),
    @ErrorCategory nvarchar(100)=NULL,@ErrorCode nvarchar(100)=NULL,@ErrorMessage nvarchar(2000)=NULL,
    @RemoteCorrelationId nvarchar(200)=NULL,@NextAttemptAt datetime2(0)=NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    IF @Outcome NOT IN(N'Retry',N'NotFound',N'Failed') BEGIN SELECT -3; RETURN; END;
    BEGIN TRANSACTION;
    DECLARE @Status nvarchar(30),@LockedBy nvarchar(200),@TraceId uniqueidentifier,@Attempts int,@Maximum int,@AttemptId bigint,@NewStatus nvarchar(30);
    SELECT @Status=Status,@LockedBy=LockedBy,@TraceId=TraceId,@Attempts=AttemptCount,@Maximum=MaxAttempts
    FROM dbo.SriDocumentQueue WITH(UPDLOCK) WHERE Id=@QueueId;
    IF @Status IS NULL BEGIN ROLLBACK; SELECT 0; RETURN; END;
    IF @Status<>N'Querying' BEGIN ROLLBACK; SELECT -3; RETURN; END;
    IF @LockedBy<>@WorkerInstance BEGIN ROLLBACK; SELECT -2; RETURN; END;
    SELECT @AttemptId=Id FROM dbo.SriDocumentAttempts WITH(UPDLOCK) WHERE QueueId=@QueueId AND AttemptNumber=@AttemptNumber AND CompletedAt IS NULL;
    IF @AttemptId IS NULL BEGIN ROLLBACK; SELECT -2; RETURN; END;
    SET @NewStatus=CASE WHEN @Outcome=N'Retry' AND @Attempts>=@Maximum THEN N'DeadLetter'
        WHEN @Outcome=N'Retry' THEN N'RetryScheduled' WHEN @Outcome=N'NotFound' THEN N'NotFound' ELSE N'Failed' END;
    UPDATE dbo.SriDocumentAttempts SET ResultStatus=@NewStatus,ErrorCategory=@ErrorCategory,ErrorCode=@ErrorCode,ErrorMessage=@ErrorMessage,
        RemoteCorrelationId=@RemoteCorrelationId,CompletedAt=SYSUTCDATETIME(),DurationMs=DATEDIFF(millisecond,StartedAt,SYSUTCDATETIME()) WHERE Id=@AttemptId;
    UPDATE dbo.SriDocumentQueue SET Status=@NewStatus,NextAttemptAt=CASE WHEN @NewStatus=N'RetryScheduled' THEN @NextAttemptAt ELSE NULL END,
        CompletedAt=CASE WHEN @NewStatus IN(N'NotFound',N'Failed',N'DeadLetter') THEN SYSUTCDATETIME() ELSE NULL END,
        LastErrorCode=@ErrorCode,LastErrorMessage=@ErrorMessage,LockedBy=NULL,LockedAt=NULL,LockExpiresAt=NULL,
        UpdatedByUserName=@WorkerInstance,UpdatedAt=SYSUTCDATETIME() WHERE Id=@QueueId;
    INSERT dbo.AuditSriDocumentChanges(QueueId,Action,PreviousStatus,NewStatus,Reason,UserName,TraceId)
    VALUES(@QueueId,CASE WHEN @NewStatus=N'RetryScheduled' THEN N'RetryScheduled' ELSE @NewStatus END,N'Querying',@NewStatus,@ErrorMessage,@WorkerInstance,@TraceId);
    COMMIT; SELECT 1;
END;
GO

IF OBJECT_ID(N'dbo.SchemaHistory',N'U') IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260720.117')
    INSERT dbo.SchemaHistory(Version,Description) VALUES(N'20260720.117',N'SRI Worker claim lease retries attempts and authorized XML store');
GO
