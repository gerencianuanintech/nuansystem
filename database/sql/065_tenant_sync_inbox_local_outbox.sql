/*
    Fase 4: infraestructura base tenant/sucursal para sincronizacion Master/Sucursal.

    Reglas:
    - SyncInbox registra mensajes recibidos antes de aplicar.
    - LocalOutbox registra eventos locales pendientes de publicar hacia Master.
    - No implementa workers ni aplicacion real de entidades.
*/

IF OBJECT_ID(N'dbo.SyncInbox', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SyncInbox
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SyncInbox PRIMARY KEY,
        EventId uniqueidentifier NOT NULL,
        SourceCompanyId int NOT NULL,
        EntityName nvarchar(120) NOT NULL,
        EntityGlobalId uniqueidentifier NOT NULL,
        Operation nvarchar(30) NOT NULL,
        PayloadJson nvarchar(max) NOT NULL,
        Status nvarchar(30) NOT NULL CONSTRAINT DF_SyncInbox_Status DEFAULT N'Pending',
        AttemptCount int NOT NULL CONSTRAINT DF_SyncInbox_AttemptCount DEFAULT 0,
        MaxAttempts int NOT NULL CONSTRAINT DF_SyncInbox_MaxAttempts DEFAULT 3,
        NextRetryAt datetime2(0) NULL,
        ReceivedAt datetime2(0) NOT NULL CONSTRAINT DF_SyncInbox_ReceivedAt DEFAULT SYSUTCDATETIME(),
        AppliedAt datetime2(0) NULL,
        ErrorMessage nvarchar(max) NULL,
        LastErrorMessage nvarchar(max) NULL,
        CONSTRAINT CK_SyncInbox_Operation CHECK (Operation IN (N'Created', N'Updated', N'Deleted', N'Disabled')),
        CONSTRAINT CK_SyncInbox_Status CHECK (Status IN (N'Pending', N'InProcess', N'Applied', N'Error', N'Ignored', N'DeadLetter')),
        CONSTRAINT CK_SyncInbox_PayloadJson CHECK (ISJSON(PayloadJson) = 1),
        CONSTRAINT CK_SyncInbox_MaxAttempts CHECK (MaxAttempts > 0),
        CONSTRAINT CK_SyncInbox_AttemptCount CHECK (AttemptCount >= 0)
    );
END;
GO

IF OBJECT_ID(N'dbo.SyncInbox', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.SyncInbox', N'AttemptCount') IS NULL
BEGIN
    ALTER TABLE dbo.SyncInbox
    ADD AttemptCount int NOT NULL CONSTRAINT DF_SyncInbox_AttemptCount DEFAULT 0;
END;
GO

IF OBJECT_ID(N'dbo.SyncInbox', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.SyncInbox', N'MaxAttempts') IS NULL
BEGIN
    ALTER TABLE dbo.SyncInbox
    ADD MaxAttempts int NOT NULL CONSTRAINT DF_SyncInbox_MaxAttempts DEFAULT 3;
END;
GO

IF OBJECT_ID(N'dbo.SyncInbox', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.SyncInbox', N'NextRetryAt') IS NULL
BEGIN
    ALTER TABLE dbo.SyncInbox
    ADD NextRetryAt datetime2(0) NULL;
END;
GO

IF OBJECT_ID(N'dbo.SyncInbox', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.SyncInbox', N'LastErrorMessage') IS NULL
BEGIN
    ALTER TABLE dbo.SyncInbox
    ADD LastErrorMessage nvarchar(max) NULL;
END;
GO

IF OBJECT_ID(N'dbo.SyncInbox', N'U') IS NOT NULL
BEGIN
    IF EXISTS
    (
        SELECT 1
        FROM sys.check_constraints
        WHERE name = N'CK_SyncInbox_Status'
          AND parent_object_id = OBJECT_ID(N'dbo.SyncInbox')
    )
    BEGIN
        ALTER TABLE dbo.SyncInbox DROP CONSTRAINT CK_SyncInbox_Status;
    END;

    ALTER TABLE dbo.SyncInbox WITH CHECK
    ADD CONSTRAINT CK_SyncInbox_Status CHECK (Status IN (N'Pending', N'InProcess', N'Applied', N'Error', N'Ignored', N'DeadLetter'));
END;
GO

IF OBJECT_ID(N'dbo.SyncInbox', N'U') IS NOT NULL
    AND NOT EXISTS
    (
        SELECT 1
        FROM sys.check_constraints
        WHERE name = N'CK_SyncInbox_MaxAttempts'
          AND parent_object_id = OBJECT_ID(N'dbo.SyncInbox')
    )
BEGIN
    ALTER TABLE dbo.SyncInbox WITH CHECK
    ADD CONSTRAINT CK_SyncInbox_MaxAttempts CHECK (MaxAttempts > 0);
END;
GO

IF OBJECT_ID(N'dbo.SyncInbox', N'U') IS NOT NULL
    AND NOT EXISTS
    (
        SELECT 1
        FROM sys.check_constraints
        WHERE name = N'CK_SyncInbox_AttemptCount'
          AND parent_object_id = OBJECT_ID(N'dbo.SyncInbox')
    )
BEGIN
    ALTER TABLE dbo.SyncInbox WITH CHECK
    ADD CONSTRAINT CK_SyncInbox_AttemptCount CHECK (AttemptCount >= 0);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SyncInbox_EventId' AND object_id = OBJECT_ID(N'dbo.SyncInbox'))
BEGIN
    CREATE UNIQUE INDEX UX_SyncInbox_EventId ON dbo.SyncInbox (EventId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SyncInbox_Status' AND object_id = OBJECT_ID(N'dbo.SyncInbox'))
BEGIN
    CREATE INDEX IX_SyncInbox_Status ON dbo.SyncInbox (Status, ReceivedAt);
END;
GO

IF OBJECT_ID(N'dbo.LocalOutbox', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LocalOutbox
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_LocalOutbox PRIMARY KEY,
        EventId uniqueidentifier NOT NULL CONSTRAINT DF_LocalOutbox_EventId DEFAULT NEWID(),
        CompanyId int NOT NULL,
        EntityName nvarchar(120) NOT NULL,
        EntityGlobalId uniqueidentifier NOT NULL,
        EntityCode nvarchar(100) NULL,
        Operation nvarchar(30) NOT NULL,
        PayloadJson nvarchar(max) NOT NULL,
        Status nvarchar(30) NOT NULL CONSTRAINT DF_LocalOutbox_Status DEFAULT N'Pending',
        AttemptCount int NOT NULL CONSTRAINT DF_LocalOutbox_AttemptCount DEFAULT 0,
        MaxAttempts int NOT NULL CONSTRAINT DF_LocalOutbox_MaxAttempts DEFAULT 3,
        NextRetryAt datetime2(0) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_LocalOutbox_CreatedAt DEFAULT SYSUTCDATETIME(),
        ProcessedAt datetime2(0) NULL,
        LastErrorMessage nvarchar(max) NULL,
        CONSTRAINT CK_LocalOutbox_Operation CHECK (Operation IN (N'Created', N'Updated', N'Deleted', N'Disabled')),
        CONSTRAINT CK_LocalOutbox_Status CHECK (Status IN (N'Pending', N'InProcess', N'Applied', N'Error', N'Ignored', N'DeadLetter')),
        CONSTRAINT CK_LocalOutbox_PayloadJson CHECK (ISJSON(PayloadJson) = 1),
        CONSTRAINT CK_LocalOutbox_MaxAttempts CHECK (MaxAttempts > 0),
        CONSTRAINT CK_LocalOutbox_AttemptCount CHECK (AttemptCount >= 0)
    );
END;
GO

IF OBJECT_ID(N'dbo.LocalOutbox', N'U') IS NOT NULL
BEGIN
    IF EXISTS
    (
        SELECT 1
        FROM sys.check_constraints
        WHERE name = N'CK_LocalOutbox_Status'
          AND parent_object_id = OBJECT_ID(N'dbo.LocalOutbox')
    )
    BEGIN
        ALTER TABLE dbo.LocalOutbox DROP CONSTRAINT CK_LocalOutbox_Status;
    END;

    ALTER TABLE dbo.LocalOutbox WITH CHECK
    ADD CONSTRAINT CK_LocalOutbox_Status CHECK (Status IN (N'Pending', N'InProcess', N'Applied', N'Error', N'Ignored', N'DeadLetter'));
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_LocalOutbox_EventId' AND object_id = OBJECT_ID(N'dbo.LocalOutbox'))
BEGIN
    CREATE UNIQUE INDEX UX_LocalOutbox_EventId ON dbo.LocalOutbox (EventId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_LocalOutbox_Status_NextRetryAt' AND object_id = OBJECT_ID(N'dbo.LocalOutbox'))
BEGIN
    CREATE INDEX IX_LocalOutbox_Status_NextRetryAt ON dbo.LocalOutbox (Status, NextRetryAt, CreatedAt);
END;
GO

IF OBJECT_ID(N'dbo.SyncAudit', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SyncAudit
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SyncAudit PRIMARY KEY,
        CompanyId int NOT NULL,
        BranchCompanyId int NULL,
        EventId uniqueidentifier NULL,
        EntityName nvarchar(120) NOT NULL,
        EntityGlobalId uniqueidentifier NULL,
        [Action] nvarchar(30) NOT NULL,
        PreviousStatus nvarchar(30) NULL,
        NewStatus nvarchar(30) NULL,
        [Message] nvarchar(500) NULL,
        ErrorCode nvarchar(80) NULL,
        ErrorDetail nvarchar(max) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SyncAudit_CreatedAt DEFAULT SYSUTCDATETIME(),
        CreatedBy nvarchar(120) NULL,
        CONSTRAINT CK_SyncAudit_Action CHECK ([Action] IN (N'Created', N'TargetCreated', N'Claimed', N'Applied', N'Failed', N'Ignored', N'Retried', N'DeadLetter', N'DryRun'))
    );
END;
GO

IF OBJECT_ID(N'dbo.SyncAudit', N'U') IS NOT NULL
BEGIN
    IF EXISTS
    (
        SELECT 1
        FROM sys.check_constraints
        WHERE name = N'CK_SyncAudit_Action'
          AND parent_object_id = OBJECT_ID(N'dbo.SyncAudit')
    )
    BEGIN
        ALTER TABLE dbo.SyncAudit DROP CONSTRAINT CK_SyncAudit_Action;
    END;

    ALTER TABLE dbo.SyncAudit WITH CHECK
    ADD CONSTRAINT CK_SyncAudit_Action CHECK ([Action] IN (N'Created', N'TargetCreated', N'Claimed', N'Applied', N'Failed', N'Ignored', N'Retried', N'DeadLetter', N'DryRun'));
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SyncAudit_EventId' AND object_id = OBJECT_ID(N'dbo.SyncAudit'))
BEGIN
    CREATE INDEX IX_SyncAudit_EventId ON dbo.SyncAudit (EventId) WHERE EventId IS NOT NULL;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SyncAudit_Entity_GlobalId' AND object_id = OBJECT_ID(N'dbo.SyncAudit'))
BEGIN
    CREATE INDEX IX_SyncAudit_Entity_GlobalId ON dbo.SyncAudit (EntityName, EntityGlobalId) WHERE EntityGlobalId IS NOT NULL;
END;
GO

IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NOT NULL
    AND NOT EXISTS (SELECT 1 FROM dbo.SchemaHistory WHERE Version = N'20260709.05')
BEGIN
    INSERT INTO dbo.SchemaHistory (Version, Description)
    VALUES (N'20260709.05', N'Fase 4: infraestructura tenant SyncInbox, LocalOutbox y auditoria');
END;
GO
