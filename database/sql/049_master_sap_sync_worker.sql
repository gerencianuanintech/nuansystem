USE [NuanSystem_Master];
GO

IF OBJECT_ID(N'dbo.SapSyncEntitySettings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SapSyncEntitySettings
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SapSyncEntitySettings PRIMARY KEY,
        CompanyId int NOT NULL,
        EntityCode nvarchar(80) NOT NULL,
        Direction nvarchar(20) NOT NULL,
        IsEnabled bit NOT NULL CONSTRAINT DF_SapSyncEntitySettings_IsEnabled DEFAULT 1,
        BatchSize int NOT NULL CONSTRAINT DF_SapSyncEntitySettings_BatchSize DEFAULT 100,
        MaxRetryCount int NOT NULL CONSTRAINT DF_SapSyncEntitySettings_MaxRetryCount DEFAULT 3,
        ExecutionOrder int NOT NULL CONSTRAINT DF_SapSyncEntitySettings_ExecutionOrder DEFAULT 0,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SapSyncEntitySettings_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT FK_SapSyncEntitySettings_Companies FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(Id),
        CONSTRAINT UQ_SapSyncEntitySettings_Company_Entity_Direction UNIQUE (CompanyId, EntityCode, Direction)
    );
END;
GO

IF OBJECT_ID(N'dbo.WorkerHeartbeat', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.WorkerHeartbeat
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_WorkerHeartbeat PRIMARY KEY,
        InstanceName nvarchar(120) NOT NULL,
        CompanyId int NULL,
        CompanyCode nvarchar(50) NULL,
        LastBeatAt datetime2(0) NOT NULL,
        Status nvarchar(30) NOT NULL,
        CurrentJob nvarchar(300) NULL,
        WorkerVersion nvarchar(80) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_WorkerHeartbeat_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT UQ_WorkerHeartbeat_InstanceName UNIQUE (InstanceName)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SapSyncEntitySettings_Company_Enabled' AND object_id = OBJECT_ID(N'dbo.SapSyncEntitySettings'))
    CREATE INDEX IX_SapSyncEntitySettings_Company_Enabled ON dbo.SapSyncEntitySettings (CompanyId, IsEnabled, ExecutionOrder);
GO

DECLARE @Defaults TABLE (EntityCode nvarchar(80), Direction nvarchar(20), ExecutionOrder int);
INSERT INTO @Defaults (EntityCode, Direction, ExecutionOrder)
VALUES (N'Suppliers', N'SapToErp', 10), (N'Items', N'SapToErp', 20), (N'PurchaseOrders', N'Both', 30);

INSERT INTO dbo.SapSyncEntitySettings (CompanyId, EntityCode, Direction, IsEnabled, BatchSize, MaxRetryCount, ExecutionOrder)
SELECT c.Id, d.EntityCode, d.Direction, 1, 100, COALESCE(s.MaxRetryCount, 3), d.ExecutionOrder
FROM dbo.Companies c
INNER JOIN dbo.SapCompanySettings s ON s.CompanyId = c.Id
CROSS JOIN @Defaults d
WHERE c.IsActive = 1
  AND c.SapIntegrationMode <> 0
  AND s.IsEnabled = 1
  AND s.IntegrationMode <> 0
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.SapSyncEntitySettings existing
      WHERE existing.CompanyId = c.Id
        AND existing.EntityCode = d.EntityCode
        AND existing.Direction = d.Direction
  );
GO
