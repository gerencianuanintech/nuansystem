/*
    Ejecutar este script dentro de la base tenant.
*/

IF OBJECT_ID(N'dbo.SapSyncWatermark', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SapSyncWatermark
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SapSyncWatermark PRIMARY KEY,
        CompanyId int NOT NULL,
        EntityCode nvarchar(80) NOT NULL,
        Direction nvarchar(20) NOT NULL,
        LastSuccessfulSyncAtUtc datetime2(0) NULL,
        LastSapKey nvarchar(120) NULL,
        LastLocalKey nvarchar(120) NULL,
        MetadataJson nvarchar(max) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SapSyncWatermark_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT UQ_SapSyncWatermark_Company_Entity_Direction UNIQUE (CompanyId, EntityCode, Direction)
    );
END;
GO

IF OBJECT_ID(N'dbo.SapSyncInbox', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SapSyncInbox
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SapSyncInbox PRIMARY KEY,
        CompanyId int NOT NULL,
        EntityCode nvarchar(80) NOT NULL,
        SapEntityId nvarchar(120) NOT NULL,
        PayloadJson nvarchar(max) NULL,
        Status nvarchar(30) NOT NULL CONSTRAINT DF_SapSyncInbox_Status DEFAULT N'Pending',
        AttemptCount int NOT NULL CONSTRAINT DF_SapSyncInbox_AttemptCount DEFAULT 0,
        MaxRetryCount int NOT NULL CONSTRAINT DF_SapSyncInbox_MaxRetryCount DEFAULT 3,
        NextAttemptAtUtc datetime2(0) NULL,
        WorkerInstance nvarchar(120) NULL,
        CorrelationId nvarchar(80) NULL,
        LocalEntityId nvarchar(120) NULL,
        ErrorCode nvarchar(120) NULL,
        ErrorMessage nvarchar(max) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SapSyncInbox_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        LockedAt datetime2(0) NULL,
        ExpiresAt datetime2(0) NULL,
        CONSTRAINT UQ_SapSyncInbox_Company_Entity_SapEntity UNIQUE (CompanyId, EntityCode, SapEntityId)
    );
END;
GO

IF OBJECT_ID(N'dbo.SapSyncOutbox', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SapSyncOutbox
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SapSyncOutbox PRIMARY KEY,
        CompanyId int NOT NULL,
        EntityCode nvarchar(80) NOT NULL,
        OperationCode nvarchar(80) NOT NULL,
        LocalEntityId nvarchar(120) NOT NULL,
        PayloadJson nvarchar(max) NULL,
        Status nvarchar(30) NOT NULL CONSTRAINT DF_SapSyncOutbox_Status DEFAULT N'Pending',
        AttemptCount int NOT NULL CONSTRAINT DF_SapSyncOutbox_AttemptCount DEFAULT 0,
        MaxRetryCount int NOT NULL CONSTRAINT DF_SapSyncOutbox_MaxRetryCount DEFAULT 3,
        NextAttemptAtUtc datetime2(0) NULL,
        WorkerInstance nvarchar(120) NULL,
        CorrelationId nvarchar(80) NULL,
        ResponseJson nvarchar(max) NULL,
        ErrorCode nvarchar(120) NULL,
        ErrorMessage nvarchar(max) NULL,
        SapDocEntry int NULL,
        SapDocNum int NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SapSyncOutbox_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        LockedAt datetime2(0) NULL,
        ExpiresAt datetime2(0) NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.SapSyncLock', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SapSyncLock
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SapSyncLock PRIMARY KEY,
        CompanyId int NOT NULL,
        EntityCode nvarchar(80) NOT NULL,
        Direction nvarchar(20) NOT NULL,
        WorkerInstance nvarchar(120) NOT NULL,
        CorrelationId nvarchar(80) NOT NULL,
        LockedAt datetime2(0) NOT NULL CONSTRAINT DF_SapSyncLock_LockedAt DEFAULT SYSUTCDATETIME(),
        ExpiresAt datetime2(0) NOT NULL,
        CONSTRAINT UQ_SapSyncLock_Company_Entity_Direction UNIQUE (CompanyId, EntityCode, Direction)
    );
END;
GO

IF OBJECT_ID(N'dbo.SapSyncConflict', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SapSyncConflict
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SapSyncConflict PRIMARY KEY,
        CompanyId int NOT NULL,
        EntityCode nvarchar(80) NOT NULL,
        Direction nvarchar(20) NOT NULL,
        LocalEntityId nvarchar(120) NULL,
        SapEntityId nvarchar(120) NULL,
        CorrelationId nvarchar(80) NULL,
        Message nvarchar(max) NOT NULL,
        PayloadJson nvarchar(max) NULL,
        IsResolved bit NOT NULL CONSTRAINT DF_SapSyncConflict_IsResolved DEFAULT 0,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SapSyncConflict_CreatedAt DEFAULT SYSUTCDATETIME(),
        ResolvedAt datetime2(0) NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.SapSyncTechnicalLog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SapSyncTechnicalLog
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SapSyncTechnicalLog PRIMARY KEY,
        CompanyId int NOT NULL,
        CompanyCode nvarchar(50) NOT NULL,
        EntityCode nvarchar(80) NOT NULL,
        Direction nvarchar(20) NOT NULL,
        Operation nvarchar(40) NOT NULL,
        Status nvarchar(30) NOT NULL,
        CorrelationId nvarchar(80) NOT NULL,
        WorkerInstance nvarchar(120) NOT NULL,
        AttemptCount int NOT NULL,
        QueueItemId bigint NULL,
        LocalEntityId nvarchar(120) NULL,
        SapEntityId nvarchar(120) NULL,
        SapDocEntry int NULL,
        SapDocNum int NULL,
        RequestJson nvarchar(max) NULL,
        ResponseJson nvarchar(max) NULL,
        ErrorCode nvarchar(120) NULL,
        ErrorMessage nvarchar(max) NULL,
        DurationMs bigint NOT NULL,
        StartedAtUtc datetime2(0) NOT NULL,
        FinishedAtUtc datetime2(0) NOT NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SapSyncTechnicalLog_CreatedAt DEFAULT SYSUTCDATETIME()
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SapSyncTechnicalLog_CorrelationId' AND object_id = OBJECT_ID(N'dbo.SapSyncTechnicalLog'))
    CREATE INDEX IX_SapSyncTechnicalLog_CorrelationId ON dbo.SapSyncTechnicalLog (CorrelationId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SapSyncInbox_Claim' AND object_id = OBJECT_ID(N'dbo.SapSyncInbox'))
    CREATE INDEX IX_SapSyncInbox_Claim ON dbo.SapSyncInbox (CompanyId, EntityCode, Status, NextAttemptAtUtc, CreatedAt);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SapSyncOutbox_Claim' AND object_id = OBJECT_ID(N'dbo.SapSyncOutbox'))
    CREATE INDEX IX_SapSyncOutbox_Claim ON dbo.SapSyncOutbox (CompanyId, Status, NextAttemptAtUtc, CreatedAt);
GO
