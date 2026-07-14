/*
    Fase 4: infraestructura base Master para sincronizacion Master/Sucursal.

    Reglas:
    - Usa GlobalId para identificar entidades entre nodos.
    - No implementa workers ni aplicacion de eventos de negocio.
    - No depende de SAP, SRI ni replicacion SQL directa.
*/

IF OBJECT_ID(N'dbo.Modules', N'U') IS NOT NULL
    AND OBJECT_ID(N'dbo.Permissions', N'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Code = N'SYNC')
    BEGIN
        INSERT INTO dbo.Modules (Code, Name, DisplayOrder)
        VALUES (N'SYNC', N'Sincronizacion Master/Sucursal', 70);
    END;

    DECLARE @SyncModuleId int = (SELECT TOP (1) Id FROM dbo.Modules WHERE Code = N'SYNC');
    DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN');

    DECLARE @SyncPermissions table
    (
        Code nvarchar(120) NOT NULL,
        Name nvarchar(160) NOT NULL,
        Description nvarchar(300) NOT NULL
    );

    INSERT INTO @SyncPermissions (Code, Name, Description)
    VALUES
        (N'SYNC.OUTBOX.VIEW', N'Ver Sync Outbox', N'Consultar eventos y destinos de sincronizacion Master/Sucursal.'),
        (N'SYNC.AUDIT.VIEW', N'Ver auditoria Sync', N'Consultar auditoria tecnica de sincronizacion Master/Sucursal.'),
        (N'SYNC.OUTBOX.RETRY', N'Reintentar Sync Error', N'Reintentar manualmente eventos SyncOutbox en estado Error.'),
        (N'SYNC.OUTBOX.RETRY_DEADLETTER', N'Reintentar Sync DeadLetter', N'Reintentar manualmente eventos SyncOutbox en DeadLetter con motivo obligatorio.'),
        (N'SYNC.OUTBOX.RELEASE_LOCK', N'Liberar lock Sync', N'Liberar manualmente locks vencidos de eventos SyncOutbox.');

    INSERT INTO dbo.Permissions (ModuleId, Code, Name, Description)
    SELECT @SyncModuleId, source.Code, source.Name, source.Description
    FROM @SyncPermissions source
    WHERE @SyncModuleId IS NOT NULL
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.Permissions existing
          WHERE existing.Code = source.Code
      );

    IF @AdminRoleId IS NOT NULL
    BEGIN
        INSERT INTO dbo.RolePermissions (RoleId, PermissionId)
        SELECT @AdminRoleId, permission.Id
        FROM dbo.Permissions permission
        WHERE permission.Code IN (SELECT Code FROM @SyncPermissions)
          AND NOT EXISTS
          (
              SELECT 1
              FROM dbo.RolePermissions existing
              WHERE existing.RoleId = @AdminRoleId
                AND existing.PermissionId = permission.Id
          );
    END;
END;
GO

IF OBJECT_ID(N'dbo.SyncEntityConfigurations', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SyncEntityConfigurations
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SyncEntityConfigurations PRIMARY KEY,
        CompanyId int NOT NULL,
        EntityName nvarchar(120) NOT NULL,
        IsEnabled bit NOT NULL CONSTRAINT DF_SyncEntityConfigurations_IsEnabled DEFAULT 0,
        Direction nvarchar(30) NOT NULL,
        ConflictPolicy nvarchar(30) NOT NULL,
        BatchSize int NOT NULL CONSTRAINT DF_SyncEntityConfigurations_BatchSize DEFAULT 100,
        MaxAttempts int NOT NULL CONSTRAINT DF_SyncEntityConfigurations_MaxAttempts DEFAULT 3,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SyncEntityConfigurations_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT FK_SyncEntityConfigurations_Companies FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(Id),
        CONSTRAINT UQ_SyncEntityConfigurations_Company_Entity UNIQUE (CompanyId, EntityName),
        CONSTRAINT CK_SyncEntityConfigurations_Direction CHECK (Direction IN (N'MasterToBranch', N'BranchToMaster', N'Bidirectional')),
        CONSTRAINT CK_SyncEntityConfigurations_ConflictPolicy CHECK (ConflictPolicy IN (N'MasterWins', N'BranchWins', N'RejectOnConflict', N'ManualReview')),
        CONSTRAINT CK_SyncEntityConfigurations_BatchSize CHECK (BatchSize > 0),
        CONSTRAINT CK_SyncEntityConfigurations_MaxAttempts CHECK (MaxAttempts > 0)
    );
END;
GO

IF OBJECT_ID(N'dbo.SyncDistributionRules', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SyncDistributionRules
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SyncDistributionRules PRIMARY KEY,
        CompanyId int NOT NULL,
        EntityName nvarchar(120) NOT NULL,
        BranchCompanyId int NOT NULL,
        RuleType nvarchar(30) NOT NULL,
        RuleValue nvarchar(200) NULL,
        IsEnabled bit NOT NULL CONSTRAINT DF_SyncDistributionRules_IsEnabled DEFAULT 1,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SyncDistributionRules_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT FK_SyncDistributionRules_Companies FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(Id),
        CONSTRAINT FK_SyncDistributionRules_BranchCompanies FOREIGN KEY (BranchCompanyId) REFERENCES dbo.Companies(Id),
        CONSTRAINT CK_SyncDistributionRules_RuleType CHECK (RuleType IN (N'All', N'ByEntityCode', N'ByBranch', N'ByCustomValue'))
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SyncDistributionRules_Company_Entity' AND object_id = OBJECT_ID(N'dbo.SyncDistributionRules'))
BEGIN
    CREATE INDEX IX_SyncDistributionRules_Company_Entity
    ON dbo.SyncDistributionRules (CompanyId, EntityName, IsEnabled);
END;
GO

IF OBJECT_ID(N'dbo.SyncOutbox', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SyncOutbox
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SyncOutbox PRIMARY KEY,
        EventId uniqueidentifier NOT NULL CONSTRAINT DF_SyncOutbox_EventId DEFAULT NEWID(),
        CompanyId int NOT NULL,
        EntityName nvarchar(120) NOT NULL,
        EntityGlobalId uniqueidentifier NOT NULL,
        EntityCode nvarchar(100) NULL,
        Operation nvarchar(30) NOT NULL,
        PayloadJson nvarchar(max) NOT NULL,
        SourceSystem nvarchar(80) NULL,
        SourceReference nvarchar(120) NULL,
        Status nvarchar(30) NOT NULL CONSTRAINT DF_SyncOutbox_Status DEFAULT N'Pending',
        AttemptCount int NOT NULL CONSTRAINT DF_SyncOutbox_AttemptCount DEFAULT 0,
        MaxAttempts int NOT NULL CONSTRAINT DF_SyncOutbox_MaxAttempts DEFAULT 3,
        NextRetryAt datetime2(0) NULL,
        LockedBy nvarchar(120) NULL,
        LockedAt datetime2(0) NULL,
        LockExpiresAt datetime2(0) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SyncOutbox_CreatedAt DEFAULT SYSUTCDATETIME(),
        ProcessedAt datetime2(0) NULL,
        LastErrorMessage nvarchar(max) NULL,
        CONSTRAINT FK_SyncOutbox_Companies FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(Id),
        CONSTRAINT CK_SyncOutbox_Operation CHECK (Operation IN (N'Created', N'Updated', N'Deleted', N'Disabled')),
        CONSTRAINT CK_SyncOutbox_Status CHECK (Status IN (N'Pending', N'InProcess', N'Applied', N'Error', N'Ignored', N'DeadLetter')),
        CONSTRAINT CK_SyncOutbox_PayloadJson CHECK (ISJSON(PayloadJson) = 1),
        CONSTRAINT CK_SyncOutbox_MaxAttempts CHECK (MaxAttempts > 0),
        CONSTRAINT CK_SyncOutbox_AttemptCount CHECK (AttemptCount >= 0)
    );
END;
GO

IF OBJECT_ID(N'dbo.SyncOutbox', N'U') IS NOT NULL
BEGIN
    IF EXISTS
    (
        SELECT 1
        FROM sys.check_constraints
        WHERE name = N'CK_SyncOutbox_Status'
          AND parent_object_id = OBJECT_ID(N'dbo.SyncOutbox')
    )
    BEGIN
        ALTER TABLE dbo.SyncOutbox DROP CONSTRAINT CK_SyncOutbox_Status;
    END;

    ALTER TABLE dbo.SyncOutbox WITH CHECK
    ADD CONSTRAINT CK_SyncOutbox_Status CHECK (Status IN (N'Pending', N'InProcess', N'Applied', N'Error', N'Ignored', N'DeadLetter'));
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SyncOutbox_EventId' AND object_id = OBJECT_ID(N'dbo.SyncOutbox'))
BEGIN
    CREATE UNIQUE INDEX UX_SyncOutbox_EventId ON dbo.SyncOutbox (EventId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SyncOutbox_Status_NextRetryAt' AND object_id = OBJECT_ID(N'dbo.SyncOutbox'))
BEGIN
    CREATE INDEX IX_SyncOutbox_Status_NextRetryAt ON dbo.SyncOutbox (Status, NextRetryAt, CreatedAt);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SyncOutbox_Entity_GlobalId' AND object_id = OBJECT_ID(N'dbo.SyncOutbox'))
BEGIN
    CREATE INDEX IX_SyncOutbox_Entity_GlobalId ON dbo.SyncOutbox (EntityName, EntityGlobalId);
END;
GO

IF OBJECT_ID(N'dbo.SyncOutboxTargets', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SyncOutboxTargets
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SyncOutboxTargets PRIMARY KEY,
        OutboxId bigint NOT NULL,
        BranchCompanyId int NOT NULL,
        Status nvarchar(30) NOT NULL CONSTRAINT DF_SyncOutboxTargets_Status DEFAULT N'Pending',
        AttemptCount int NOT NULL CONSTRAINT DF_SyncOutboxTargets_AttemptCount DEFAULT 0,
        MaxAttempts int NOT NULL CONSTRAINT DF_SyncOutboxTargets_MaxAttempts DEFAULT 3,
        NextRetryAt datetime2(0) NULL,
        AppliedAt datetime2(0) NULL,
        LastErrorMessage nvarchar(max) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SyncOutboxTargets_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT FK_SyncOutboxTargets_SyncOutbox FOREIGN KEY (OutboxId) REFERENCES dbo.SyncOutbox(Id),
        CONSTRAINT FK_SyncOutboxTargets_BranchCompanies FOREIGN KEY (BranchCompanyId) REFERENCES dbo.Companies(Id),
        CONSTRAINT UQ_SyncOutboxTargets_Outbox_Branch UNIQUE (OutboxId, BranchCompanyId),
        CONSTRAINT CK_SyncOutboxTargets_Status CHECK (Status IN (N'Pending', N'InProcess', N'Applied', N'Error', N'Ignored', N'DeadLetter')),
        CONSTRAINT CK_SyncOutboxTargets_MaxAttempts CHECK (MaxAttempts > 0),
        CONSTRAINT CK_SyncOutboxTargets_AttemptCount CHECK (AttemptCount >= 0)
    );
END;
GO

IF OBJECT_ID(N'dbo.SyncOutboxTargets', N'U') IS NOT NULL
BEGIN
    IF EXISTS
    (
        SELECT 1
        FROM sys.check_constraints
        WHERE name = N'CK_SyncOutboxTargets_Status'
          AND parent_object_id = OBJECT_ID(N'dbo.SyncOutboxTargets')
    )
    BEGIN
        ALTER TABLE dbo.SyncOutboxTargets DROP CONSTRAINT CK_SyncOutboxTargets_Status;
    END;

    ALTER TABLE dbo.SyncOutboxTargets WITH CHECK
    ADD CONSTRAINT CK_SyncOutboxTargets_Status CHECK (Status IN (N'Pending', N'InProcess', N'Applied', N'Error', N'Ignored', N'DeadLetter'));
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SyncOutboxTargets_Outbox_Branch' AND object_id = OBJECT_ID(N'dbo.SyncOutboxTargets'))
BEGIN
    CREATE INDEX IX_SyncOutboxTargets_Outbox_Branch ON dbo.SyncOutboxTargets (OutboxId, BranchCompanyId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SyncOutboxTargets_Status_NextRetryAt' AND object_id = OBJECT_ID(N'dbo.SyncOutboxTargets'))
BEGIN
    CREATE INDEX IX_SyncOutboxTargets_Status_NextRetryAt ON dbo.SyncOutboxTargets (Status, NextRetryAt, CreatedAt);
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
        CONSTRAINT FK_SyncAudit_Companies FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(Id),
        CONSTRAINT FK_SyncAudit_BranchCompanies FOREIGN KEY (BranchCompanyId) REFERENCES dbo.Companies(Id),
        CONSTRAINT CK_SyncAudit_Action CHECK ([Action] IN (N'Created', N'TargetCreated', N'Claimed', N'Applied', N'Failed', N'Ignored', N'Retried', N'DeadLetter', N'DryRun', N'RetriedFromDeadLetter', N'LockReleased'))
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
    ADD CONSTRAINT CK_SyncAudit_Action CHECK ([Action] IN (N'Created', N'TargetCreated', N'Claimed', N'Applied', N'Failed', N'Ignored', N'Retried', N'DeadLetter', N'DryRun', N'RetriedFromDeadLetter', N'LockReleased'));
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

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NOT NULL
    AND NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260709.04')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description)
    VALUES (N'20260709.04', N'Fase 4: infraestructura Master SyncOutbox, targets y auditoria');
END;
GO
