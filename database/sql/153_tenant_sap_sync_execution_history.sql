/*
    Migracion 153 - Historial tenant de ejecuciones SAP y leases recuperables.

    Objetos tenant:
      - SapSyncExecutions y SapSyncExecutionDetails;
      - resultados seguros por SourceRecordKey y snapshot allowlist con SHA-256;
      - claims por detalle para retry futuro;
      - evolucion compatible de SapSyncLock con ExecutionUid, owner token,
        renovacion, vencimiento, recuperacion y auditoria.

    No contiene FK hacia NuanSystem_Master, no crea purga y no ejecuta SAP.
    La retencion de desarrollo es indefinida.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
    THROW 51153, 'SchemaHistory is required before migration 153.', 1;
IF OBJECT_ID(N'dbo.SapSyncLock', N'U') IS NULL
    THROW 51153, 'SapSyncLock is required before migration 153.', 1;
GO

IF OBJECT_ID(N'dbo.SapSyncExecutions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SapSyncExecutions
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SapSyncExecutions PRIMARY KEY,
        ExecutionUid uniqueidentifier NOT NULL,
        RunGroupId uniqueidentifier NOT NULL,
        CorrelationId uniqueidentifier NOT NULL,
        SapSyncProfileId bigint NULL,
        SapSyncProfileEntityId bigint NULL,
        ProfileCode nvarchar(80) NOT NULL,
        ProfileName nvarchar(160) NOT NULL,
        CompanyId int NOT NULL,
        CompanyCode nvarchar(50) NOT NULL,
        EntityCode nvarchar(80) NOT NULL,
        Direction varchar(20) NOT NULL,
        TriggerType varchar(20) NOT NULL,
        ParentExecutionId bigint NULL,
        Status varchar(30) NOT NULL CONSTRAINT DF_SapSyncExecutions_Status DEFAULT 'Pending',
        BatchSize int NOT NULL,
        MaxAttempts int NOT NULL,
        ExecutionOrder int NOT NULL,
        TimeoutMinutes int NOT NULL,
        ScheduleType varchar(20) NULL,
        TimeZoneId nvarchar(100) NULL,
        ProfileSnapshotJson nvarchar(max) NOT NULL,
        EffectiveParametersJson nvarchar(max) NOT NULL,
        RequestedByUserId int NULL,
        RequestedByUserName nvarchar(120) NULL,
        RequestedAtUtc datetime2(0) NOT NULL CONSTRAINT DF_SapSyncExecutions_RequestedAt DEFAULT SYSUTCDATETIME(),
        WorkerInstance nvarchar(120) NULL,
        StartedAtUtc datetime2(0) NULL,
        LastProgressAtUtc datetime2(0) NULL,
        FinishedAtUtc datetime2(0) NULL,
        NextAttemptAtUtc datetime2(0) NULL,
        CancellationRequestedAtUtc datetime2(0) NULL,
        CancellationRequestedByUserId int NULL,
        CancellationRequestedByUserName nvarchar(120) NULL,
        TotalRecords int NOT NULL CONSTRAINT DF_SapSyncExecutions_TotalRecords DEFAULT 0,
        CreatedRecords int NOT NULL CONSTRAINT DF_SapSyncExecutions_CreatedRecords DEFAULT 0,
        UpdatedRecords int NOT NULL CONSTRAINT DF_SapSyncExecutions_UpdatedRecords DEFAULT 0,
        UnchangedRecords int NOT NULL CONSTRAINT DF_SapSyncExecutions_UnchangedRecords DEFAULT 0,
        ApprovalRequiredRecords int NOT NULL CONSTRAINT DF_SapSyncExecutions_ApprovalRecords DEFAULT 0,
        ConflictRecords int NOT NULL CONSTRAINT DF_SapSyncExecutions_ConflictRecords DEFAULT 0,
        SkippedRecords int NOT NULL CONSTRAINT DF_SapSyncExecutions_SkippedRecords DEFAULT 0,
        RetryScheduledRecords int NOT NULL CONSTRAINT DF_SapSyncExecutions_RetryRecords DEFAULT 0,
        FailedRecords int NOT NULL CONSTRAINT DF_SapSyncExecutions_FailedRecords DEFAULT 0,
        DeadLetterRecords int NOT NULL CONSTRAINT DF_SapSyncExecutions_DeadLetterRecords DEFAULT 0,
        LastSafeErrorCode nvarchar(120) NULL,
        LastSafeErrorMessage nvarchar(1000) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SapSyncExecutions_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        RowVersion rowversion NOT NULL,
        CONSTRAINT UQ_SapSyncExecutions_ExecutionUid UNIQUE (ExecutionUid),
        CONSTRAINT FK_SapSyncExecutions_ParentExecution FOREIGN KEY (ParentExecutionId) REFERENCES dbo.SapSyncExecutions(Id),
        CONSTRAINT CK_SapSyncExecutions_ProfileCode_NotBlank CHECK (LEN(LTRIM(RTRIM(ProfileCode))) > 0),
        CONSTRAINT CK_SapSyncExecutions_CompanyCode_NotBlank CHECK (LEN(LTRIM(RTRIM(CompanyCode))) > 0),
        CONSTRAINT CK_SapSyncExecutions_EntityCode_NotBlank CHECK (LEN(LTRIM(RTRIM(EntityCode))) > 0),
        CONSTRAINT CK_SapSyncExecutions_Direction CHECK (Direction IN ('SapToErp', 'ErpToSap', 'Both')),
        CONSTRAINT CK_SapSyncExecutions_TriggerType CHECK (TriggerType IN ('Manual', 'Scheduled', 'Retry')),
        CONSTRAINT CK_SapSyncExecutions_Status CHECK
        (
            Status IN
            (
                'Pending', 'Running', 'Cancelling', 'Cancelled', 'RetryScheduled',
                'SkippedConcurrent', 'Completed', 'CompletedWithWarnings',
                'CompletedWithErrors', 'Failed'
            )
        ),
        CONSTRAINT CK_SapSyncExecutions_ScheduleType CHECK
        (
            ScheduleType IS NULL OR ScheduleType IN ('Manual', 'Interval', 'Daily')
        ),
        CONSTRAINT CK_SapSyncExecutions_Limits CHECK
        (
            BatchSize BETWEEN 1 AND 10000
            AND MaxAttempts BETWEEN 1 AND 20
            AND ExecutionOrder BETWEEN 0 AND 100000
            AND TimeoutMinutes BETWEEN 1 AND 1440
        ),
        CONSTRAINT CK_SapSyncExecutions_Counts CHECK
        (
            TotalRecords >= 0
            AND CreatedRecords >= 0
            AND UpdatedRecords >= 0
            AND UnchangedRecords >= 0
            AND ApprovalRequiredRecords >= 0
            AND ConflictRecords >= 0
            AND SkippedRecords >= 0
            AND RetryScheduledRecords >= 0
            AND FailedRecords >= 0
            AND DeadLetterRecords >= 0
        ),
        CONSTRAINT CK_SapSyncExecutions_ProfileSnapshotJson CHECK (ISJSON(ProfileSnapshotJson) = 1),
        CONSTRAINT CK_SapSyncExecutions_EffectiveParametersJson CHECK (ISJSON(EffectiveParametersJson) = 1),
        CONSTRAINT CK_SapSyncExecutions_Snapshots_NoSecrets CHECK
        (
            LOWER(ProfileSnapshotJson) NOT LIKE N'%password%'
            AND LOWER(ProfileSnapshotJson) NOT LIKE N'%token%'
            AND LOWER(ProfileSnapshotJson) NOT LIKE N'%cookie%'
            AND LOWER(ProfileSnapshotJson) NOT LIKE N'%authorization%'
            AND LOWER(ProfileSnapshotJson) NOT LIKE N'%connectionstring%'
            AND LOWER(EffectiveParametersJson) NOT LIKE N'%password%'
            AND LOWER(EffectiveParametersJson) NOT LIKE N'%token%'
            AND LOWER(EffectiveParametersJson) NOT LIKE N'%cookie%'
            AND LOWER(EffectiveParametersJson) NOT LIKE N'%authorization%'
            AND LOWER(EffectiveParametersJson) NOT LIKE N'%connectionstring%'
        )
    );

    CREATE INDEX IX_SapSyncExecutions_List
        ON dbo.SapSyncExecutions(RequestedAtUtc DESC, Id DESC);
    CREATE INDEX IX_SapSyncExecutions_Profile
        ON dbo.SapSyncExecutions(SapSyncProfileId, RequestedAtUtc DESC, Id DESC);
    CREATE INDEX IX_SapSyncExecutions_Entity
        ON dbo.SapSyncExecutions(EntityCode, Direction, RequestedAtUtc DESC, Id DESC);
    CREATE INDEX IX_SapSyncExecutions_Status_Date
        ON dbo.SapSyncExecutions(Status, NextAttemptAtUtc, RequestedAtUtc, Id);
    CREATE INDEX IX_SapSyncExecutions_RunGroup
        ON dbo.SapSyncExecutions(RunGroupId, ExecutionOrder, Id);
    CREATE INDEX IX_SapSyncExecutions_Correlation
        ON dbo.SapSyncExecutions(CorrelationId, Id);
END;
GO

IF OBJECT_ID(N'dbo.SapSyncExecutionDetails', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SapSyncExecutionDetails
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SapSyncExecutionDetails PRIMARY KEY,
        SapSyncExecutionId bigint NOT NULL,
        SourceRecordKey nvarchar(120) NOT NULL,
        SourceVersion nvarchar(120) NULL,
        LocalEntityId bigint NULL,
        LocalGlobalId uniqueidentifier NULL,
        Action varchar(20) NOT NULL,
        Status varchar(30) NOT NULL CONSTRAINT DF_SapSyncExecutionDetails_Status DEFAULT 'Pending',
        AttemptCount int NOT NULL CONSTRAINT DF_SapSyncExecutionDetails_AttemptCount DEFAULT 0,
        MaxAttempts int NOT NULL,
        NextAttemptAtUtc datetime2(0) NULL,
        ErrorClass varchar(20) NULL,
        ResultCode nvarchar(120) NULL,
        SafeMessage nvarchar(1000) NULL,
        ApprovedSnapshotType varchar(40) NULL,
        ApprovedSnapshotJson nvarchar(max) NULL,
        SnapshotHash binary(32) NULL,
        WorkerInstance nvarchar(120) NULL,
        OwnerToken char(64) NULL,
        LockedAtUtc datetime2(0) NULL,
        RenewedAtUtc datetime2(0) NULL,
        LockExpiresAtUtc datetime2(0) NULL,
        StartedAtUtc datetime2(0) NULL,
        FinishedAtUtc datetime2(0) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SapSyncExecutionDetails_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        RowVersion rowversion NOT NULL,
        CONSTRAINT FK_SapSyncExecutionDetails_Execution FOREIGN KEY (SapSyncExecutionId) REFERENCES dbo.SapSyncExecutions(Id),
        CONSTRAINT UQ_SapSyncExecutionDetails_Record UNIQUE (SapSyncExecutionId, SourceRecordKey),
        CONSTRAINT CK_SapSyncExecutionDetails_SourceRecordKey_NotBlank CHECK (LEN(LTRIM(RTRIM(SourceRecordKey))) > 0),
        CONSTRAINT CK_SapSyncExecutionDetails_Action CHECK
        (
            Action IN ('Create', 'Update', 'NoChange', 'Approval', 'Conflict', 'Skip')
        ),
        CONSTRAINT CK_SapSyncExecutionDetails_Status CHECK
        (
            Status IN
            (
                'Pending', 'Processing', 'Created', 'Updated', 'Unchanged',
                'ApprovalRequired', 'Conflict', 'Skipped', 'RetryScheduled',
                'Failed', 'DeadLetter'
            )
        ),
        CONSTRAINT CK_SapSyncExecutionDetails_Attempts CHECK
        (
            MaxAttempts BETWEEN 1 AND 20
            AND AttemptCount BETWEEN 0 AND MaxAttempts
        ),
        CONSTRAINT CK_SapSyncExecutionDetails_ErrorClass CHECK
        (
            ErrorClass IS NULL OR ErrorClass IN ('Transient', 'Terminal', 'Conflict', 'Approval')
        ),
        CONSTRAINT CK_SapSyncExecutionDetails_SnapshotPair CHECK
        (
            (ApprovedSnapshotType IS NULL AND ApprovedSnapshotJson IS NULL AND SnapshotHash IS NULL)
            OR
            (ApprovedSnapshotType IS NOT NULL AND ApprovedSnapshotJson IS NOT NULL AND SnapshotHash IS NOT NULL)
        ),
        CONSTRAINT CK_SapSyncExecutionDetails_ApprovedSnapshotType CHECK
        (
            ApprovedSnapshotType IS NULL
            OR ApprovedSnapshotType IN ('SupplierV1', 'ItemV1', 'PaymentTermV1', 'WarehouseV1')
        ),
        CONSTRAINT CK_SapSyncExecutionDetails_ApprovedSnapshotJson CHECK
        (
            ApprovedSnapshotJson IS NULL OR ISJSON(ApprovedSnapshotJson) = 1
        ),
        CONSTRAINT CK_SapSyncExecutionDetails_ApprovedSnapshot_NoSecrets CHECK
        (
            ApprovedSnapshotJson IS NULL
            OR
            (
                LOWER(ApprovedSnapshotJson) NOT LIKE N'%password%'
                AND LOWER(ApprovedSnapshotJson) NOT LIKE N'%token%'
                AND LOWER(ApprovedSnapshotJson) NOT LIKE N'%cookie%'
                AND LOWER(ApprovedSnapshotJson) NOT LIKE N'%authorization%'
                AND LOWER(ApprovedSnapshotJson) NOT LIKE N'%connectionstring%'
                AND LOWER(ApprovedSnapshotJson) NOT LIKE N'%b1session%'
                AND LOWER(ApprovedSnapshotJson) NOT LIKE N'%routeid%'
                AND LOWER(ApprovedSnapshotJson) NOT LIKE N'%login%'
            )
        ),
        CONSTRAINT CK_SapSyncExecutionDetails_LockShape CHECK
        (
            (OwnerToken IS NULL AND WorkerInstance IS NULL AND LockedAtUtc IS NULL AND RenewedAtUtc IS NULL AND LockExpiresAtUtc IS NULL)
            OR
            (OwnerToken IS NOT NULL AND WorkerInstance IS NOT NULL AND LockedAtUtc IS NOT NULL AND LockExpiresAtUtc IS NOT NULL)
        ),
        CONSTRAINT CK_SapSyncExecutionDetails_OwnerToken CHECK
        (
            OwnerToken IS NULL OR LEN(OwnerToken) = 64
        )
    );

    CREATE INDEX IX_SapSyncExecutionDetails_List
        ON dbo.SapSyncExecutionDetails(SapSyncExecutionId, Status, SourceRecordKey, Id);
    CREATE INDEX IX_SapSyncExecutionDetails_Claim
        ON dbo.SapSyncExecutionDetails(Status, NextAttemptAtUtc, LockExpiresAtUtc, Id)
        INCLUDE (SapSyncExecutionId, AttemptCount, MaxAttempts);
    CREATE INDEX IX_SapSyncExecutionDetails_Result
        ON dbo.SapSyncExecutionDetails(ResultCode, CreatedAt DESC, Id);
END;
GO

IF OBJECT_ID(N'dbo.AuditSapSyncExecutionChanges', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuditSapSyncExecutionChanges
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditSapSyncExecutionChanges PRIMARY KEY,
        SapSyncExecutionId bigint NOT NULL,
        SapSyncExecutionDetailId bigint NULL,
        Action varchar(40) NOT NULL,
        PreviousStatus varchar(30) NULL,
        NewStatus varchar(30) NULL,
        Reason nvarchar(500) NULL,
        UserId int NULL,
        UserName nvarchar(120) NULL,
        WorkerInstance nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_AuditSapSyncExecutionChanges_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_AuditSapSyncExecutionChanges_Execution FOREIGN KEY (SapSyncExecutionId) REFERENCES dbo.SapSyncExecutions(Id),
        CONSTRAINT FK_AuditSapSyncExecutionChanges_Detail FOREIGN KEY (SapSyncExecutionDetailId) REFERENCES dbo.SapSyncExecutionDetails(Id),
        CONSTRAINT CK_AuditSapSyncExecutionChanges_Action CHECK
        (
            Action IN
            (
                'Created', 'Transitioned', 'CancellationRequested',
                'DetailCreated', 'DetailUpdated', 'DetailClaimed',
                'DetailLockRenewed', 'DetailLockReleased'
            )
        )
    );

    CREATE INDEX IX_AuditSapSyncExecutionChanges_Execution_CreatedAt
        ON dbo.AuditSapSyncExecutionChanges(SapSyncExecutionId, CreatedAt DESC, Id DESC);
    CREATE INDEX IX_AuditSapSyncExecutionChanges_Detail_CreatedAt
        ON dbo.AuditSapSyncExecutionChanges(SapSyncExecutionDetailId, CreatedAt DESC, Id DESC)
        WHERE SapSyncExecutionDetailId IS NOT NULL;
END;
GO

IF COL_LENGTH(N'dbo.SapSyncLock', N'ExecutionUid') IS NULL
    ALTER TABLE dbo.SapSyncLock ADD ExecutionUid uniqueidentifier NULL;
IF COL_LENGTH(N'dbo.SapSyncLock', N'OwnerToken') IS NULL
    ALTER TABLE dbo.SapSyncLock ADD OwnerToken char(64) NULL;
IF COL_LENGTH(N'dbo.SapSyncLock', N'RenewedAtUtc') IS NULL
    ALTER TABLE dbo.SapSyncLock ADD RenewedAtUtc datetime2(0) NULL;
IF COL_LENGTH(N'dbo.SapSyncLock', N'LockExpiresAtUtc') IS NULL
    ALTER TABLE dbo.SapSyncLock ADD LockExpiresAtUtc datetime2(0) NULL;
GO

UPDATE dbo.SapSyncLock
SET OwnerToken = CONVERT(varchar(64), HASHBYTES('SHA2_256', CONCAT(NEWID(), N':', Id, N':', SYSUTCDATETIME())), 2)
WHERE OwnerToken IS NULL;

UPDATE dbo.SapSyncLock
SET LockExpiresAtUtc = ExpiresAt
WHERE LockExpiresAtUtc IS NULL;
GO

IF EXISTS
(
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.SapSyncLock')
      AND name = N'OwnerToken'
      AND is_nullable = 1
)
    ALTER TABLE dbo.SapSyncLock ALTER COLUMN OwnerToken char(64) NOT NULL;

IF EXISTS
(
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.SapSyncLock')
      AND name = N'LockExpiresAtUtc'
      AND is_nullable = 1
)
    ALTER TABLE dbo.SapSyncLock ALTER COLUMN LockExpiresAtUtc datetime2(0) NOT NULL;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.check_constraints
    WHERE parent_object_id = OBJECT_ID(N'dbo.SapSyncLock')
      AND name = N'CK_SapSyncLock_OwnerToken'
)
BEGIN
    ALTER TABLE dbo.SapSyncLock WITH CHECK
    ADD CONSTRAINT CK_SapSyncLock_OwnerToken CHECK (LEN(OwnerToken) = 64);
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.SapSyncLock')
      AND name = N'IX_SapSyncLock_ExecutionUid'
)
    CREATE INDEX IX_SapSyncLock_ExecutionUid ON dbo.SapSyncLock(ExecutionUid) WHERE ExecutionUid IS NOT NULL;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.SapSyncLock')
      AND name = N'IX_SapSyncLock_Expiry'
)
    CREATE INDEX IX_SapSyncLock_Expiry ON dbo.SapSyncLock(LockExpiresAtUtc, Id);
GO

IF OBJECT_ID(N'dbo.AuditSapSyncLockChanges', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuditSapSyncLockChanges
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditSapSyncLockChanges PRIMARY KEY,
        SapSyncLockId bigint NULL,
        CompanyId int NOT NULL,
        EntityCode nvarchar(80) NOT NULL,
        Direction varchar(20) NOT NULL,
        ExecutionUid uniqueidentifier NULL,
        Action varchar(30) NOT NULL,
        PreviousOwnerHash char(64) NULL,
        NewOwnerHash char(64) NULL,
        Reason nvarchar(500) NULL,
        UserId int NULL,
        UserName nvarchar(120) NULL,
        WorkerInstance nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_AuditSapSyncLockChanges_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT CK_AuditSapSyncLockChanges_Action CHECK
        (
            Action IN ('Acquired', 'Recovered', 'Renewed', 'Released', 'ExpiredReleased')
        )
    );

    CREATE INDEX IX_AuditSapSyncLockChanges_Key_CreatedAt
        ON dbo.AuditSapSyncLockChanges(CompanyId, EntityCode, Direction, CreatedAt DESC, Id DESC);
    CREATE INDEX IX_AuditSapSyncLockChanges_ExecutionUid
        ON dbo.AuditSapSyncLockChanges(ExecutionUid, CreatedAt DESC, Id DESC)
        WHERE ExecutionUid IS NOT NULL;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SAPSYNCLOCKADQUIRIR
    @CompanyId int,
    @EntityCode nvarchar(80),
    @Direction varchar(20),
    @WorkerInstance nvarchar(120),
    @CorrelationId nvarchar(80),
    @ExecutionUid uniqueidentifier = NULL,
    @OwnerToken char(64),
    @LockExpiresAtUtc datetime2(0)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @EntityCode = NULLIF(LTRIM(RTRIM(@EntityCode)), N'');
    SET @WorkerInstance = NULLIF(LTRIM(RTRIM(@WorkerInstance)), N'');
    SET @CorrelationId = NULLIF(LTRIM(RTRIM(@CorrelationId)), N'');

    IF @EntityCode IS NULL OR @WorkerInstance IS NULL OR @CorrelationId IS NULL
       OR @Direction NOT IN ('SapToErp', 'ErpToSap', 'Both')
       OR LEN(@OwnerToken) <> 64
       OR @LockExpiresAtUtc <= SYSUTCDATETIME()
        THROW 51153, 'Invalid SAP lock acquisition contract.', 1;

    DECLARE @Now datetime2(0) = SYSUTCDATETIME();
    DECLARE @LockId bigint;
    DECLARE @PreviousOwnerHash char(64);
    DECLARE @Action varchar(30);

    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
    BEGIN TRANSACTION;

    SELECT
        @LockId = Id,
        @PreviousOwnerHash = CONVERT(char(64), HASHBYTES('SHA2_256', OwnerToken), 2)
    FROM dbo.SapSyncLock WITH (UPDLOCK, HOLDLOCK)
    WHERE CompanyId = @CompanyId
      AND EntityCode = @EntityCode
      AND Direction = @Direction;

    IF @LockId IS NOT NULL
    BEGIN
        IF EXISTS
        (
            SELECT 1
            FROM dbo.SapSyncLock
            WHERE Id = @LockId
              AND LockExpiresAtUtc > @Now
        )
        BEGIN
            COMMIT;
            RETURN;
        END;

        UPDATE dbo.SapSyncLock
        SET WorkerInstance = @WorkerInstance,
            CorrelationId = @CorrelationId,
            ExecutionUid = @ExecutionUid,
            OwnerToken = @OwnerToken,
            LockedAt = @Now,
            RenewedAtUtc = NULL,
            LockExpiresAtUtc = @LockExpiresAtUtc,
            ExpiresAt = @LockExpiresAtUtc
        WHERE Id = @LockId;

        SET @Action = 'Recovered';
    END
    ELSE
    BEGIN
        INSERT dbo.SapSyncLock
        (
            CompanyId, EntityCode, Direction, WorkerInstance, CorrelationId,
            ExecutionUid, OwnerToken, LockedAt, RenewedAtUtc,
            LockExpiresAtUtc, ExpiresAt
        )
        VALUES
        (
            @CompanyId, @EntityCode, @Direction, @WorkerInstance, @CorrelationId,
            @ExecutionUid, @OwnerToken, @Now, NULL,
            @LockExpiresAtUtc, @LockExpiresAtUtc
        );

        SET @LockId = SCOPE_IDENTITY();
        SET @Action = 'Acquired';
    END;

    INSERT dbo.AuditSapSyncLockChanges
    (
        SapSyncLockId, CompanyId, EntityCode, Direction, ExecutionUid,
        Action, PreviousOwnerHash, NewOwnerHash, WorkerInstance
    )
    VALUES
    (
        @LockId, @CompanyId, @EntityCode, @Direction, @ExecutionUid,
        @Action, @PreviousOwnerHash,
        CONVERT(char(64), HASHBYTES('SHA2_256', @OwnerToken), 2),
        @WorkerInstance
    );

    COMMIT;

    SELECT
        Id, CompanyId, EntityCode, Direction, WorkerInstance, CorrelationId,
        ExecutionUid, OwnerToken, LockedAt AS LockedAtUtc,
        RenewedAtUtc, LockExpiresAtUtc
    FROM dbo.SapSyncLock
    WHERE Id = @LockId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PATCH_SAPSYNCLOCKRENOVAR
    @Id bigint,
    @OwnerToken char(64),
    @LockExpiresAtUtc datetime2(0)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Now datetime2(0) = SYSUTCDATETIME();
    IF LEN(@OwnerToken) <> 64 OR @LockExpiresAtUtc <= @Now
        THROW 51153, 'Invalid SAP lock renewal contract.', 1;

    BEGIN TRANSACTION;

    UPDATE dbo.SapSyncLock WITH (UPDLOCK)
    SET RenewedAtUtc = @Now,
        LockExpiresAtUtc = @LockExpiresAtUtc,
        ExpiresAt = @LockExpiresAtUtc
    WHERE Id = @Id
      AND OwnerToken = @OwnerToken
      AND LockExpiresAtUtc > @Now;

    DECLARE @Affected int = @@ROWCOUNT;

    IF @Affected = 1
    BEGIN
        INSERT dbo.AuditSapSyncLockChanges
        (
            SapSyncLockId, CompanyId, EntityCode, Direction, ExecutionUid,
            Action, NewOwnerHash, WorkerInstance
        )
        SELECT
            Id, CompanyId, EntityCode, Direction, ExecutionUid,
            'Renewed', CONVERT(char(64), HASHBYTES('SHA2_256', OwnerToken), 2),
            WorkerInstance
        FROM dbo.SapSyncLock
        WHERE Id = @Id;
    END;

    COMMIT;
    SELECT @Affected;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_SAPSYNCLOCKLIBERAR
    @Id bigint,
    @OwnerToken char(64)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    DECLARE @Released table
    (
        Id bigint,
        CompanyId int,
        EntityCode nvarchar(80),
        Direction varchar(20),
        ExecutionUid uniqueidentifier,
        WorkerInstance nvarchar(120),
        OwnerHash char(64)
    );

    DELETE FROM dbo.SapSyncLock
    OUTPUT
        DELETED.Id, DELETED.CompanyId, DELETED.EntityCode, DELETED.Direction,
        DELETED.ExecutionUid, DELETED.WorkerInstance,
        CONVERT(char(64), HASHBYTES('SHA2_256', DELETED.OwnerToken), 2)
    INTO @Released
    WHERE Id = @Id
      AND OwnerToken = @OwnerToken;

    INSERT dbo.AuditSapSyncLockChanges
    (
        SapSyncLockId, CompanyId, EntityCode, Direction, ExecutionUid,
        Action, PreviousOwnerHash, WorkerInstance
    )
    SELECT
        Id, CompanyId, EntityCode, Direction, ExecutionUid,
        'Released', OwnerHash, WorkerInstance
    FROM @Released;

    DECLARE @Affected int = (SELECT COUNT(1) FROM @Released);
    COMMIT;
    SELECT @Affected;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_SAPSYNCLOCKLIBERARVENCIDO
    @Id bigint,
    @Reason nvarchar(500),
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @Reason = NULLIF(LTRIM(RTRIM(@Reason)), N'');
    IF @Reason IS NULL
        THROW 51153, 'Reason is required to release an expired SAP lock.', 1;

    BEGIN TRANSACTION;

    DECLARE @Released table
    (
        Id bigint,
        CompanyId int,
        EntityCode nvarchar(80),
        Direction varchar(20),
        ExecutionUid uniqueidentifier,
        WorkerInstance nvarchar(120),
        OwnerHash char(64)
    );

    DELETE FROM dbo.SapSyncLock
    OUTPUT
        DELETED.Id, DELETED.CompanyId, DELETED.EntityCode, DELETED.Direction,
        DELETED.ExecutionUid, DELETED.WorkerInstance,
        CONVERT(char(64), HASHBYTES('SHA2_256', DELETED.OwnerToken), 2)
    INTO @Released
    WHERE Id = @Id
      AND LockExpiresAtUtc <= SYSUTCDATETIME();

    INSERT dbo.AuditSapSyncLockChanges
    (
        SapSyncLockId, CompanyId, EntityCode, Direction, ExecutionUid,
        Action, PreviousOwnerHash, Reason, UserId, UserName, WorkerInstance
    )
    SELECT
        Id, CompanyId, EntityCode, Direction, ExecutionUid,
        'ExpiredReleased', OwnerHash, @Reason, @AuditUserId,
        NULLIF(LTRIM(RTRIM(@AuditUserName)), N''), WorkerInstance
    FROM @Released;

    DECLARE @Affected int = (SELECT COUNT(1) FROM @Released);
    COMMIT;
    SELECT @Affected;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SAPSYNCEXECUTIONCREAR
    @ExecutionUid uniqueidentifier,
    @RunGroupId uniqueidentifier,
    @CorrelationId uniqueidentifier,
    @SapSyncProfileId bigint = NULL,
    @SapSyncProfileEntityId bigint = NULL,
    @ProfileCode nvarchar(80),
    @ProfileName nvarchar(160),
    @CompanyId int,
    @CompanyCode nvarchar(50),
    @EntityCode nvarchar(80),
    @Direction varchar(20),
    @TriggerType varchar(20),
    @ParentExecutionId bigint = NULL,
    @BatchSize int,
    @MaxAttempts int,
    @ExecutionOrder int,
    @TimeoutMinutes int,
    @ScheduleType varchar(20) = NULL,
    @TimeZoneId nvarchar(100) = NULL,
    @ProfileSnapshotJson nvarchar(max),
    @EffectiveParametersJson nvarchar(max),
    @RequestedByUserId int = NULL,
    @RequestedByUserName nvarchar(120) = NULL,
    @WorkerInstance nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @ExecutionUid IS NULL OR @RunGroupId IS NULL OR @CorrelationId IS NULL
       OR @Direction NOT IN ('SapToErp', 'ErpToSap')
       OR @TriggerType NOT IN ('Manual', 'Scheduled', 'Retry')
       OR ISJSON(@ProfileSnapshotJson) <> 1
       OR ISJSON(@EffectiveParametersJson) <> 1
       OR @BatchSize NOT BETWEEN 1 AND 10000
       OR @MaxAttempts NOT BETWEEN 1 AND 20
       OR @ExecutionOrder NOT BETWEEN 0 AND 100000
       OR @TimeoutMinutes NOT BETWEEN 1 AND 1440
        THROW 51153, 'Invalid SAP execution creation contract.', 1;

    IF LOWER(@ProfileSnapshotJson) LIKE N'%password%'
       OR LOWER(@ProfileSnapshotJson) LIKE N'%token%'
       OR LOWER(@ProfileSnapshotJson) LIKE N'%cookie%'
       OR LOWER(@ProfileSnapshotJson) LIKE N'%authorization%'
       OR LOWER(@ProfileSnapshotJson) LIKE N'%connectionstring%'
       OR LOWER(@EffectiveParametersJson) LIKE N'%password%'
       OR LOWER(@EffectiveParametersJson) LIKE N'%token%'
       OR LOWER(@EffectiveParametersJson) LIKE N'%cookie%'
       OR LOWER(@EffectiveParametersJson) LIKE N'%authorization%'
       OR LOWER(@EffectiveParametersJson) LIKE N'%connectionstring%'
        THROW 51153, 'Sensitive keys are forbidden in SAP execution snapshots.', 1;

    IF EXISTS (SELECT 1 FROM dbo.SapSyncExecutions WHERE ExecutionUid = @ExecutionUid)
    BEGIN
        SELECT Id, N'Existing' AS ResultCode, RowVersion
        FROM dbo.SapSyncExecutions
        WHERE ExecutionUid = @ExecutionUid;
        RETURN;
    END;

    BEGIN TRANSACTION;

    INSERT dbo.SapSyncExecutions
    (
        ExecutionUid, RunGroupId, CorrelationId,
        SapSyncProfileId, SapSyncProfileEntityId,
        ProfileCode, ProfileName, CompanyId, CompanyCode,
        EntityCode, Direction, TriggerType, ParentExecutionId, Status,
        BatchSize, MaxAttempts, ExecutionOrder, TimeoutMinutes,
        ScheduleType, TimeZoneId, ProfileSnapshotJson, EffectiveParametersJson,
        RequestedByUserId, RequestedByUserName, WorkerInstance
    )
    VALUES
    (
        @ExecutionUid, @RunGroupId, @CorrelationId,
        @SapSyncProfileId, @SapSyncProfileEntityId,
        LTRIM(RTRIM(@ProfileCode)), LTRIM(RTRIM(@ProfileName)),
        @CompanyId, LTRIM(RTRIM(@CompanyCode)),
        LTRIM(RTRIM(@EntityCode)), @Direction, @TriggerType,
        @ParentExecutionId, 'Pending',
        @BatchSize, @MaxAttempts, @ExecutionOrder, @TimeoutMinutes,
        @ScheduleType, COALESCE(NULLIF(LTRIM(RTRIM(@TimeZoneId)), N''), N'America/Guayaquil'),
        @ProfileSnapshotJson, @EffectiveParametersJson,
        @RequestedByUserId, NULLIF(LTRIM(RTRIM(@RequestedByUserName)), N''),
        NULLIF(LTRIM(RTRIM(@WorkerInstance)), N'')
    );

    DECLARE @ExecutionId bigint = SCOPE_IDENTITY();

    INSERT dbo.AuditSapSyncExecutionChanges
    (
        SapSyncExecutionId, Action, NewStatus, UserId, UserName, WorkerInstance
    )
    VALUES
    (
        @ExecutionId, 'Created', 'Pending',
        @RequestedByUserId, NULLIF(LTRIM(RTRIM(@RequestedByUserName)), N''),
        NULLIF(LTRIM(RTRIM(@WorkerInstance)), N'')
    );

    COMMIT;

    SELECT Id, N'Created' AS ResultCode, RowVersion
    FROM dbo.SapSyncExecutions
    WHERE Id = @ExecutionId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SAPSYNCEXECUTIONPAGINAR
    @SapSyncProfileId bigint = NULL,
    @EntityCode nvarchar(80) = NULL,
    @Direction varchar(20) = NULL,
    @Status varchar(30) = NULL,
    @TriggerType varchar(20) = NULL,
    @DateFromUtc datetime2(0) = NULL,
    @DateToUtc datetime2(0) = NULL,
    @PageNumber int = 1,
    @PageSize int = 50
AS
BEGIN
    SET NOCOUNT ON;

    SET @PageNumber = CASE WHEN @PageNumber < 1 THEN 1 ELSE @PageNumber END;
    SET @PageSize = CASE WHEN @PageSize < 1 OR @PageSize > 500 THEN 50 ELSE @PageSize END;

    ;WITH Filtered AS
    (
        SELECT
            execution.Id, execution.ExecutionUid, execution.RunGroupId,
            execution.CorrelationId, execution.SapSyncProfileId,
            execution.ProfileCode, execution.ProfileName, execution.EntityCode,
            execution.Direction, execution.TriggerType, execution.Status,
            execution.RequestedAtUtc, execution.StartedAtUtc, execution.FinishedAtUtc,
            execution.TotalRecords,
            execution.CreatedRecords + execution.UpdatedRecords + execution.UnchangedRecords AS SucceededRecords,
            execution.ApprovalRequiredRecords + execution.ConflictRecords + execution.SkippedRecords AS WarningRecords,
            execution.FailedRecords + execution.DeadLetterRecords AS FailedRecords
        FROM dbo.SapSyncExecutions execution
        WHERE (@SapSyncProfileId IS NULL OR execution.SapSyncProfileId = @SapSyncProfileId)
          AND (@EntityCode IS NULL OR execution.EntityCode = @EntityCode)
          AND (@Direction IS NULL OR execution.Direction = @Direction)
          AND (@Status IS NULL OR execution.Status = @Status)
          AND (@TriggerType IS NULL OR execution.TriggerType = @TriggerType)
          AND (@DateFromUtc IS NULL OR execution.RequestedAtUtc >= @DateFromUtc)
          AND (@DateToUtc IS NULL OR execution.RequestedAtUtc < @DateToUtc)
    )
    SELECT *
    FROM Filtered
    ORDER BY RequestedAtUtc DESC, Id DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(1)
    FROM dbo.SapSyncExecutions execution
    WHERE (@SapSyncProfileId IS NULL OR execution.SapSyncProfileId = @SapSyncProfileId)
      AND (@EntityCode IS NULL OR execution.EntityCode = @EntityCode)
      AND (@Direction IS NULL OR execution.Direction = @Direction)
      AND (@Status IS NULL OR execution.Status = @Status)
      AND (@TriggerType IS NULL OR execution.TriggerType = @TriggerType)
      AND (@DateFromUtc IS NULL OR execution.RequestedAtUtc >= @DateFromUtc)
      AND (@DateToUtc IS NULL OR execution.RequestedAtUtc < @DateToUtc);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SAPSYNCEXECUTIONBUSCARPORUID
    @ExecutionUid uniqueidentifier
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id, ExecutionUid, RunGroupId, CorrelationId,
        SapSyncProfileId, SapSyncProfileEntityId,
        ProfileCode, ProfileName, CompanyId, CompanyCode,
        EntityCode, Direction, TriggerType, ParentExecutionId, Status,
        BatchSize, MaxAttempts, ExecutionOrder, TimeoutMinutes,
        ScheduleType, TimeZoneId, ProfileSnapshotJson, EffectiveParametersJson,
        RequestedByUserId, RequestedByUserName, RequestedAtUtc,
        WorkerInstance, StartedAtUtc, LastProgressAtUtc, FinishedAtUtc,
        CancellationRequestedAtUtc,
        TotalRecords, CreatedRecords, UpdatedRecords, UnchangedRecords,
        ApprovalRequiredRecords, ConflictRecords, SkippedRecords,
        RetryScheduledRecords, FailedRecords, DeadLetterRecords,
        LastSafeErrorCode, LastSafeErrorMessage, RowVersion
    FROM dbo.SapSyncExecutions
    WHERE ExecutionUid = @ExecutionUid;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SAPSYNCEXECUTIONDETALLEPAGINAR
    @ExecutionUid uniqueidentifier,
    @Status varchar(30) = NULL,
    @SourceRecordKey nvarchar(120) = NULL,
    @PageNumber int = 1,
    @PageSize int = 100
AS
BEGIN
    SET NOCOUNT ON;

    SET @PageNumber = CASE WHEN @PageNumber < 1 THEN 1 ELSE @PageNumber END;
    SET @PageSize = CASE WHEN @PageSize < 1 OR @PageSize > 500 THEN 100 ELSE @PageSize END;
    SET @SourceRecordKey = NULLIF(LTRIM(RTRIM(@SourceRecordKey)), N'');

    DECLARE @ExecutionId bigint =
    (
        SELECT Id FROM dbo.SapSyncExecutions WHERE ExecutionUid = @ExecutionUid
    );

    SELECT
        detail.Id, @ExecutionUid AS ExecutionUid,
        detail.SourceRecordKey, detail.SourceVersion,
        detail.LocalEntityId, detail.LocalGlobalId,
        detail.Action, detail.Status, detail.AttemptCount, detail.MaxAttempts,
        detail.NextAttemptAtUtc, detail.ErrorClass, detail.ResultCode,
        detail.SafeMessage, detail.ApprovedSnapshotType,
        detail.ApprovedSnapshotJson, detail.SnapshotHash,
        detail.StartedAtUtc, detail.FinishedAtUtc, detail.RowVersion
    FROM dbo.SapSyncExecutionDetails detail
    WHERE detail.SapSyncExecutionId = @ExecutionId
      AND (@Status IS NULL OR detail.Status = @Status)
      AND (@SourceRecordKey IS NULL OR detail.SourceRecordKey LIKE N'%' + @SourceRecordKey + N'%')
    ORDER BY detail.SourceRecordKey, detail.Id
    OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(1)
    FROM dbo.SapSyncExecutionDetails detail
    WHERE detail.SapSyncExecutionId = @ExecutionId
      AND (@Status IS NULL OR detail.Status = @Status)
      AND (@SourceRecordKey IS NULL OR detail.SourceRecordKey LIKE N'%' + @SourceRecordKey + N'%');
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SAPSYNCEXECUTIONDETALLEGUARDAR
    @Id bigint = NULL,
    @ExecutionUid uniqueidentifier,
    @SourceRecordKey nvarchar(120),
    @SourceVersion nvarchar(120) = NULL,
    @LocalEntityId bigint = NULL,
    @LocalGlobalId uniqueidentifier = NULL,
    @Action varchar(20),
    @Status varchar(30),
    @AttemptCount int,
    @MaxAttempts int,
    @NextAttemptAtUtc datetime2(0) = NULL,
    @ErrorClass varchar(20) = NULL,
    @ResultCode nvarchar(120) = NULL,
    @SafeMessage nvarchar(1000) = NULL,
    @ApprovedSnapshotType varchar(40) = NULL,
    @ApprovedSnapshotJson nvarchar(max) = NULL,
    @SnapshotHash binary(32) = NULL,
    @StartedAtUtc datetime2(0) = NULL,
    @FinishedAtUtc datetime2(0) = NULL,
    @RowVersion varbinary(8) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @SourceRecordKey = NULLIF(LTRIM(RTRIM(@SourceRecordKey)), N'');
    IF @SourceRecordKey IS NULL
       OR (@ApprovedSnapshotType IS NULL AND (@ApprovedSnapshotJson IS NOT NULL OR @SnapshotHash IS NOT NULL))
       OR (@ApprovedSnapshotType IS NOT NULL AND (@ApprovedSnapshotJson IS NULL OR @SnapshotHash IS NULL))
       OR (@ApprovedSnapshotJson IS NOT NULL AND ISJSON(@ApprovedSnapshotJson) <> 1)
        THROW 51153, 'Invalid SAP execution detail snapshot contract.', 1;

    IF @ApprovedSnapshotJson IS NOT NULL
       AND
       (
           LOWER(@ApprovedSnapshotJson) LIKE N'%password%'
           OR LOWER(@ApprovedSnapshotJson) LIKE N'%token%'
           OR LOWER(@ApprovedSnapshotJson) LIKE N'%cookie%'
           OR LOWER(@ApprovedSnapshotJson) LIKE N'%authorization%'
           OR LOWER(@ApprovedSnapshotJson) LIKE N'%connectionstring%'
           OR LOWER(@ApprovedSnapshotJson) LIKE N'%b1session%'
           OR LOWER(@ApprovedSnapshotJson) LIKE N'%routeid%'
           OR LOWER(@ApprovedSnapshotJson) LIKE N'%login%'
       )
        THROW 51153, 'Sensitive keys are forbidden in ApprovedSnapshotJson.', 1;

    IF @ApprovedSnapshotJson IS NOT NULL
       AND
       (
           @ApprovedSnapshotType NOT IN ('SupplierV1', 'ItemV1', 'PaymentTermV1', 'WarehouseV1')
           OR EXISTS
              (
                  SELECT 1
                  FROM OPENJSON(@ApprovedSnapshotJson) property
                  WHERE
                      (@ApprovedSnapshotType = 'WarehouseV1' AND property.[key] NOT IN ('warehouseCode', 'warehouseName', 'street', 'city', 'province', 'country', 'isActive'))
                      OR
                      (@ApprovedSnapshotType = 'SupplierV1' AND property.[key] NOT IN ('cardCode', 'cardName', 'taxIdentification', 'cardType', 'groupCode', 'phone', 'email', 'currency', 'isActive', 'createdAt', 'updatedAt'))
                      OR
                      (@ApprovedSnapshotType = 'ItemV1' AND property.[key] NOT IN ('itemCode', 'itemName', 'itemGroupCode', 'inventoryUnitCode', 'purchaseUnitCode', 'salesUnitCode', 'barcode', 'purchaseTaxCode', 'salesTaxCode', 'isPurchaseItem', 'isSalesItem', 'isInventoryItem', 'manageSerialNumbers', 'manageBatchNumbers', 'itemType', 'isActive'))
                      OR
                      (@ApprovedSnapshotType = 'PaymentTermV1' AND property.[key] NOT IN ('groupNumber', 'name', 'additionalDays', 'additionalMonths', 'numberOfInstallments'))
              )
       )
        THROW 51153, 'ApprovedSnapshotJson contains fields outside its typed allowlist.', 1;

    DECLARE @ExecutionId bigint =
    (
        SELECT Id FROM dbo.SapSyncExecutions WHERE ExecutionUid = @ExecutionUid
    );
    IF @ExecutionId IS NULL
    BEGIN
        SELECT CAST(NULL AS bigint) AS Id, N'ExecutionNotFound' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
        RETURN;
    END;

    DECLARE @DetailId bigint =
    (
        SELECT Id
        FROM dbo.SapSyncExecutionDetails
        WHERE SapSyncExecutionId = @ExecutionId
          AND SourceRecordKey = @SourceRecordKey
    );
    DECLARE @WasCreated bit = 0;

    BEGIN TRANSACTION;

    IF @DetailId IS NULL
    BEGIN
        INSERT dbo.SapSyncExecutionDetails
        (
            SapSyncExecutionId, SourceRecordKey, SourceVersion,
            LocalEntityId, LocalGlobalId, Action, Status,
            AttemptCount, MaxAttempts, NextAttemptAtUtc, ErrorClass,
            ResultCode, SafeMessage, ApprovedSnapshotType, ApprovedSnapshotJson, SnapshotHash,
            StartedAtUtc, FinishedAtUtc
        )
        VALUES
        (
            @ExecutionId, @SourceRecordKey, NULLIF(LTRIM(RTRIM(@SourceVersion)), N''),
            @LocalEntityId, @LocalGlobalId, @Action, @Status,
            @AttemptCount, @MaxAttempts, @NextAttemptAtUtc, @ErrorClass,
            NULLIF(LTRIM(RTRIM(@ResultCode)), N''), NULLIF(LTRIM(RTRIM(@SafeMessage)), N''),
            @ApprovedSnapshotType, @ApprovedSnapshotJson, @SnapshotHash, @StartedAtUtc, @FinishedAtUtc
        );
        SET @DetailId = SCOPE_IDENTITY();
        SET @WasCreated = 1;

        INSERT dbo.AuditSapSyncExecutionChanges
        (
            SapSyncExecutionId, SapSyncExecutionDetailId, Action, NewStatus
        )
        VALUES(@ExecutionId, @DetailId, 'DetailCreated', @Status);
    END
    ELSE
    BEGIN
        DECLARE @PreviousStatus varchar(30) =
        (
            SELECT Status FROM dbo.SapSyncExecutionDetails WHERE Id = @DetailId
        );

        UPDATE dbo.SapSyncExecutionDetails WITH (UPDLOCK)
        SET SourceVersion = NULLIF(LTRIM(RTRIM(@SourceVersion)), N''),
            LocalEntityId = @LocalEntityId,
            LocalGlobalId = @LocalGlobalId,
            Action = @Action,
            Status = @Status,
            AttemptCount = @AttemptCount,
            MaxAttempts = @MaxAttempts,
            NextAttemptAtUtc = @NextAttemptAtUtc,
            ErrorClass = @ErrorClass,
            ResultCode = NULLIF(LTRIM(RTRIM(@ResultCode)), N''),
            SafeMessage = NULLIF(LTRIM(RTRIM(@SafeMessage)), N''),
            ApprovedSnapshotType = @ApprovedSnapshotType,
            ApprovedSnapshotJson = @ApprovedSnapshotJson,
            SnapshotHash = @SnapshotHash,
            StartedAtUtc = @StartedAtUtc,
            FinishedAtUtc = @FinishedAtUtc,
            WorkerInstance = CASE WHEN @Status IN ('Pending', 'Processing', 'RetryScheduled') THEN WorkerInstance ELSE NULL END,
            OwnerToken = CASE WHEN @Status IN ('Pending', 'Processing', 'RetryScheduled') THEN OwnerToken ELSE NULL END,
            LockedAtUtc = CASE WHEN @Status IN ('Pending', 'Processing', 'RetryScheduled') THEN LockedAtUtc ELSE NULL END,
            RenewedAtUtc = CASE WHEN @Status IN ('Pending', 'Processing', 'RetryScheduled') THEN RenewedAtUtc ELSE NULL END,
            LockExpiresAtUtc = CASE WHEN @Status IN ('Pending', 'Processing', 'RetryScheduled') THEN LockExpiresAtUtc ELSE NULL END,
            UpdatedAt = SYSUTCDATETIME()
        WHERE Id = @DetailId
          AND (@RowVersion IS NULL OR RowVersion = @RowVersion);

        IF @@ROWCOUNT = 0
        BEGIN
            ROLLBACK;
            SELECT @DetailId AS Id, N'ConcurrencyConflict' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
            RETURN;
        END;

        INSERT dbo.AuditSapSyncExecutionChanges
        (
            SapSyncExecutionId, SapSyncExecutionDetailId, Action,
            PreviousStatus, NewStatus
        )
        VALUES(@ExecutionId, @DetailId, 'DetailUpdated', @PreviousStatus, @Status);
    END;

    UPDATE dbo.SapSyncExecutions
    SET LastProgressAtUtc = SYSUTCDATETIME(), UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @ExecutionId;

    COMMIT;

    SELECT Id,
           CASE WHEN @WasCreated = 1 THEN N'Created' ELSE N'Updated' END AS ResultCode,
           RowVersion
    FROM dbo.SapSyncExecutionDetails
    WHERE Id = @DetailId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PATCH_SAPSYNCEXECUTIONTRANSICIONAR
    @ExecutionUid uniqueidentifier,
    @ExpectedStatus varchar(30),
    @NewStatus varchar(30),
    @TotalRecords int,
    @CreatedRecords int,
    @UpdatedRecords int,
    @UnchangedRecords int,
    @ApprovalRequiredRecords int,
    @ConflictRecords int,
    @SkippedRecords int,
    @RetryScheduledRecords int,
    @FailedRecords int,
    @DeadLetterRecords int,
    @LastSafeErrorCode nvarchar(120) = NULL,
    @LastSafeErrorMessage nvarchar(1000) = NULL,
    @NextAttemptAtUtc datetime2(0) = NULL,
    @ExpectedRowVersion varbinary(8)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT
    (
        (@ExpectedStatus = 'Pending' AND @NewStatus IN ('Running', 'Cancelled', 'SkippedConcurrent'))
        OR (@ExpectedStatus = 'Running' AND @NewStatus IN ('Cancelling', 'RetryScheduled', 'Completed', 'CompletedWithWarnings', 'CompletedWithErrors', 'Failed'))
        OR (@ExpectedStatus = 'RetryScheduled' AND @NewStatus IN ('Running', 'Cancelled', 'Failed'))
        OR (@ExpectedStatus = 'Cancelling' AND @NewStatus = 'Cancelled')
    )
        THROW 51153, 'Illegal SAP execution state transition.', 1;

    BEGIN TRANSACTION;

    DECLARE @ExecutionId bigint =
    (
        SELECT Id
        FROM dbo.SapSyncExecutions WITH (UPDLOCK)
        WHERE ExecutionUid = @ExecutionUid
          AND Status = @ExpectedStatus
          AND RowVersion = @ExpectedRowVersion
    );

    IF @ExecutionId IS NULL
    BEGIN
        ROLLBACK;
        SELECT CAST(NULL AS bigint) AS Id, N'ConcurrencyConflict' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
        RETURN;
    END;

    UPDATE dbo.SapSyncExecutions
    SET Status = @NewStatus,
        StartedAtUtc = CASE WHEN @NewStatus = 'Running' AND StartedAtUtc IS NULL THEN SYSUTCDATETIME() ELSE StartedAtUtc END,
        LastProgressAtUtc = SYSUTCDATETIME(),
        FinishedAtUtc = CASE
                            WHEN @NewStatus IN ('Cancelled', 'SkippedConcurrent', 'Completed', 'CompletedWithWarnings', 'CompletedWithErrors', 'Failed')
                            THEN SYSUTCDATETIME()
                            ELSE NULL
                        END,
        NextAttemptAtUtc = CASE WHEN @NewStatus = 'RetryScheduled' THEN @NextAttemptAtUtc ELSE NULL END,
        TotalRecords = @TotalRecords,
        CreatedRecords = @CreatedRecords,
        UpdatedRecords = @UpdatedRecords,
        UnchangedRecords = @UnchangedRecords,
        ApprovalRequiredRecords = @ApprovalRequiredRecords,
        ConflictRecords = @ConflictRecords,
        SkippedRecords = @SkippedRecords,
        RetryScheduledRecords = @RetryScheduledRecords,
        FailedRecords = @FailedRecords,
        DeadLetterRecords = @DeadLetterRecords,
        LastSafeErrorCode = NULLIF(LTRIM(RTRIM(@LastSafeErrorCode)), N''),
        LastSafeErrorMessage = NULLIF(LTRIM(RTRIM(@LastSafeErrorMessage)), N''),
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @ExecutionId;

    INSERT dbo.AuditSapSyncExecutionChanges
    (
        SapSyncExecutionId, Action, PreviousStatus, NewStatus, Reason
    )
    VALUES
    (
        @ExecutionId, 'Transitioned', @ExpectedStatus, @NewStatus,
        NULLIF(LTRIM(RTRIM(@LastSafeErrorCode)), N'')
    );

    COMMIT;

    SELECT Id, N'Updated' AS ResultCode, RowVersion
    FROM dbo.SapSyncExecutions
    WHERE Id = @ExecutionId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PATCH_SAPSYNCEXECUTIONCANCELARSOLICITAR
    @ExecutionUid uniqueidentifier,
    @RequestedByUserId int = NULL,
    @RequestedByUserName nvarchar(120) = NULL,
    @ExpectedRowVersion varbinary(8)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    DECLARE @ExecutionId bigint;
    DECLARE @PreviousStatus varchar(30);

    SELECT @ExecutionId = Id, @PreviousStatus = Status
    FROM dbo.SapSyncExecutions WITH (UPDLOCK)
    WHERE ExecutionUid = @ExecutionUid
      AND RowVersion = @ExpectedRowVersion
      AND Status IN ('Pending', 'Running', 'RetryScheduled');

    IF @ExecutionId IS NULL
    BEGIN
        ROLLBACK;
        SELECT CAST(NULL AS bigint) AS Id, N'ConcurrencyConflict' AS ResultCode, CAST(NULL AS varbinary(8)) AS RowVersion;
        RETURN;
    END;

    DECLARE @NewStatus varchar(30) =
        CASE WHEN @PreviousStatus = 'Running' THEN 'Cancelling' ELSE 'Cancelled' END;

    UPDATE dbo.SapSyncExecutions
    SET Status = @NewStatus,
        CancellationRequestedAtUtc = SYSUTCDATETIME(),
        CancellationRequestedByUserId = @RequestedByUserId,
        CancellationRequestedByUserName = NULLIF(LTRIM(RTRIM(@RequestedByUserName)), N''),
        FinishedAtUtc = CASE WHEN @NewStatus = 'Cancelled' THEN SYSUTCDATETIME() ELSE FinishedAtUtc END,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @ExecutionId;

    INSERT dbo.AuditSapSyncExecutionChanges
    (
        SapSyncExecutionId, Action, PreviousStatus, NewStatus,
        UserId, UserName
    )
    VALUES
    (
        @ExecutionId, 'CancellationRequested', @PreviousStatus, @NewStatus,
        @RequestedByUserId, NULLIF(LTRIM(RTRIM(@RequestedByUserName)), N'')
    );

    COMMIT;

    SELECT Id, N'CancellationRequested' AS ResultCode, RowVersion
    FROM dbo.SapSyncExecutions
    WHERE Id = @ExecutionId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SAPSYNCEXECUTIONDETALLECLAIM
    @WorkerInstance nvarchar(120),
    @OwnerToken char(64),
    @LockExpiresAtUtc datetime2(0)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @WorkerInstance = NULLIF(LTRIM(RTRIM(@WorkerInstance)), N'');
    IF @WorkerInstance IS NULL OR LEN(@OwnerToken) <> 64 OR @LockExpiresAtUtc <= SYSUTCDATETIME()
        THROW 51153, 'Invalid SAP execution detail claim contract.', 1;

    DECLARE @Now datetime2(0) = SYSUTCDATETIME();
    DECLARE @DetailId bigint;
    DECLARE @PreviousStatus varchar(30);

    BEGIN TRANSACTION;

    SELECT TOP (1) @DetailId = detail.Id, @PreviousStatus = detail.Status
    FROM dbo.SapSyncExecutionDetails detail WITH (UPDLOCK, READPAST, ROWLOCK)
    INNER JOIN dbo.SapSyncExecutions execution ON execution.Id = detail.SapSyncExecutionId
    WHERE detail.Status IN ('Pending', 'RetryScheduled')
      AND (detail.NextAttemptAtUtc IS NULL OR detail.NextAttemptAtUtc <= @Now)
      AND (detail.LockExpiresAtUtc IS NULL OR detail.LockExpiresAtUtc <= @Now)
      AND detail.AttemptCount < detail.MaxAttempts
      AND execution.Status IN ('Running', 'RetryScheduled')
    ORDER BY COALESCE(detail.NextAttemptAtUtc, detail.CreatedAt), detail.Id;

    IF @DetailId IS NULL
    BEGIN
        COMMIT;
        RETURN;
    END;

    UPDATE dbo.SapSyncExecutionDetails
    SET Status = 'Processing',
        AttemptCount = AttemptCount + 1,
        WorkerInstance = @WorkerInstance,
        OwnerToken = @OwnerToken,
        LockedAtUtc = @Now,
        RenewedAtUtc = NULL,
        LockExpiresAtUtc = @LockExpiresAtUtc,
        StartedAtUtc = COALESCE(StartedAtUtc, @Now),
        UpdatedAt = @Now
    WHERE Id = @DetailId;

    INSERT dbo.AuditSapSyncExecutionChanges
    (
        SapSyncExecutionId, SapSyncExecutionDetailId, Action,
        PreviousStatus, NewStatus, WorkerInstance
    )
    SELECT
        SapSyncExecutionId, Id, 'DetailClaimed',
        @PreviousStatus, 'Processing', @WorkerInstance
    FROM dbo.SapSyncExecutionDetails
    WHERE Id = @DetailId;

    COMMIT;

    SELECT
        detail.Id, execution.ExecutionUid, detail.SourceRecordKey,
        detail.Status, detail.AttemptCount, detail.MaxAttempts,
        detail.ApprovedSnapshotType, detail.ApprovedSnapshotJson, detail.SnapshotHash,
        detail.OwnerToken, detail.LockedAtUtc, detail.LockExpiresAtUtc
    FROM dbo.SapSyncExecutionDetails detail
    INNER JOIN dbo.SapSyncExecutions execution ON execution.Id = detail.SapSyncExecutionId
    WHERE detail.Id = @DetailId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PATCH_SAPSYNCEXECUTIONDETALLERENOVAR
    @DetailId bigint,
    @OwnerToken char(64),
    @LockExpiresAtUtc datetime2(0)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Now datetime2(0) = SYSUTCDATETIME();
    IF LEN(@OwnerToken) <> 64 OR @LockExpiresAtUtc <= @Now
        THROW 51153, 'Invalid SAP execution detail lock renewal.', 1;

    BEGIN TRANSACTION;

    UPDATE dbo.SapSyncExecutionDetails WITH (UPDLOCK)
    SET RenewedAtUtc = @Now,
        LockExpiresAtUtc = @LockExpiresAtUtc,
        UpdatedAt = @Now
    WHERE Id = @DetailId
      AND OwnerToken = @OwnerToken
      AND Status = 'Processing'
      AND LockExpiresAtUtc > @Now;

    DECLARE @Affected int = @@ROWCOUNT;

    IF @Affected = 1
    BEGIN
        INSERT dbo.AuditSapSyncExecutionChanges
        (
            SapSyncExecutionId, SapSyncExecutionDetailId,
            Action, NewStatus, WorkerInstance
        )
        SELECT
            SapSyncExecutionId, Id, 'DetailLockRenewed', Status, WorkerInstance
        FROM dbo.SapSyncExecutionDetails
        WHERE Id = @DetailId;
    END;

    COMMIT;
    SELECT @Affected;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PATCH_SAPSYNCEXECUTIONDETALLELIBERAR
    @DetailId bigint,
    @OwnerToken char(64)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    DECLARE @ExecutionId bigint;
    DECLARE @WorkerInstance nvarchar(120);

    SELECT
        @ExecutionId = SapSyncExecutionId,
        @WorkerInstance = WorkerInstance
    FROM dbo.SapSyncExecutionDetails WITH (UPDLOCK)
    WHERE Id = @DetailId
      AND OwnerToken = @OwnerToken;

    IF @ExecutionId IS NULL
    BEGIN
        ROLLBACK;
        SELECT 0;
        RETURN;
    END;

    UPDATE dbo.SapSyncExecutionDetails
    SET WorkerInstance = NULL,
        OwnerToken = NULL,
        LockedAtUtc = NULL,
        RenewedAtUtc = NULL,
        LockExpiresAtUtc = NULL,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @DetailId
      AND OwnerToken = @OwnerToken;

    INSERT dbo.AuditSapSyncExecutionChanges
    (
        SapSyncExecutionId, SapSyncExecutionDetailId,
        Action, WorkerInstance
    )
    VALUES(@ExecutionId, @DetailId, 'DetailLockReleased', @WorkerInstance);

    COMMIT;
    SELECT 1;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.SchemaHistory
    WHERE Version = N'20260730.153'
)
BEGIN
    INSERT dbo.SchemaHistory(Version, Description)
    VALUES
    (
        N'20260730.153',
        N'Historial SAP tenant, resultados por registro y locks renovables auditados'
    );
END;
GO
