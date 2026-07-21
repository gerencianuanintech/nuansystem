/* Iteracion 6: evolucion forward-only del heartbeat compartido. NuanSystem_Master only. */
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF COL_LENGTH(N'dbo.WorkerHeartbeat', N'WorkerType') IS NULL ALTER TABLE dbo.WorkerHeartbeat ADD WorkerType nvarchar(40) NULL;
IF COL_LENGTH(N'dbo.WorkerHeartbeat', N'HostName') IS NULL ALTER TABLE dbo.WorkerHeartbeat ADD HostName nvarchar(120) NULL;
IF COL_LENGTH(N'dbo.WorkerHeartbeat', N'WorkerInstance') IS NULL ALTER TABLE dbo.WorkerHeartbeat ADD WorkerInstance nvarchar(120) NULL;
IF COL_LENGTH(N'dbo.WorkerHeartbeat', N'LifecycleState') IS NULL ALTER TABLE dbo.WorkerHeartbeat ADD LifecycleState nvarchar(30) NULL;
IF COL_LENGTH(N'dbo.WorkerHeartbeat', N'IsEnabled') IS NULL ALTER TABLE dbo.WorkerHeartbeat ADD IsEnabled bit NULL;
IF COL_LENGTH(N'dbo.WorkerHeartbeat', N'StartedAt') IS NULL ALTER TABLE dbo.WorkerHeartbeat ADD StartedAt datetime2(0) NULL;
IF COL_LENGTH(N'dbo.WorkerHeartbeat', N'LastCycleStartedAt') IS NULL ALTER TABLE dbo.WorkerHeartbeat ADD LastCycleStartedAt datetime2(0) NULL;
IF COL_LENGTH(N'dbo.WorkerHeartbeat', N'LastCycleCompletedAt') IS NULL ALTER TABLE dbo.WorkerHeartbeat ADD LastCycleCompletedAt datetime2(0) NULL;
IF COL_LENGTH(N'dbo.WorkerHeartbeat', N'LastSuccessfulCycleAt') IS NULL ALTER TABLE dbo.WorkerHeartbeat ADD LastSuccessfulCycleAt datetime2(0) NULL;
IF COL_LENGTH(N'dbo.WorkerHeartbeat', N'LastCycleDurationMs') IS NULL ALTER TABLE dbo.WorkerHeartbeat ADD LastCycleDurationMs int NULL;
IF COL_LENGTH(N'dbo.WorkerHeartbeat', N'LastCycleResult') IS NULL ALTER TABLE dbo.WorkerHeartbeat ADD LastCycleResult nvarchar(40) NULL;
IF COL_LENGTH(N'dbo.WorkerHeartbeat', N'LastSafeErrorCode') IS NULL ALTER TABLE dbo.WorkerHeartbeat ADD LastSafeErrorCode nvarchar(80) NULL;
IF COL_LENGTH(N'dbo.WorkerHeartbeat', N'LastSafeErrorMessage') IS NULL ALTER TABLE dbo.WorkerHeartbeat ADD LastSafeErrorMessage nvarchar(300) NULL;
IF COL_LENGTH(N'dbo.WorkerHeartbeat', N'EnabledCompanyCount') IS NULL ALTER TABLE dbo.WorkerHeartbeat ADD EnabledCompanyCount int NULL;
IF COL_LENGTH(N'dbo.WorkerHeartbeat', N'PendingCount') IS NULL ALTER TABLE dbo.WorkerHeartbeat ADD PendingCount bigint NULL;
IF COL_LENGTH(N'dbo.WorkerHeartbeat', N'RetryScheduledCount') IS NULL ALTER TABLE dbo.WorkerHeartbeat ADD RetryScheduledCount bigint NULL;
IF COL_LENGTH(N'dbo.WorkerHeartbeat', N'DeadLetterCount') IS NULL ALTER TABLE dbo.WorkerHeartbeat ADD DeadLetterCount bigint NULL;
IF COL_LENGTH(N'dbo.WorkerHeartbeat', N'RecentDeadLetterCount') IS NULL ALTER TABLE dbo.WorkerHeartbeat ADD RecentDeadLetterCount bigint NULL;
IF COL_LENGTH(N'dbo.WorkerHeartbeat', N'ActiveLeaseCount') IS NULL ALTER TABLE dbo.WorkerHeartbeat ADD ActiveLeaseCount bigint NULL;
IF COL_LENGTH(N'dbo.WorkerHeartbeat', N'ExpiredLeaseCount') IS NULL ALTER TABLE dbo.WorkerHeartbeat ADD ExpiredLeaseCount bigint NULL;
IF COL_LENGTH(N'dbo.WorkerHeartbeat', N'OldestPendingAt') IS NULL ALTER TABLE dbo.WorkerHeartbeat ADD OldestPendingAt datetime2(0) NULL;
IF COL_LENGTH(N'dbo.WorkerHeartbeat', N'StorageFreePercent') IS NULL ALTER TABLE dbo.WorkerHeartbeat ADD StorageFreePercent decimal(5,2) NULL;
IF COL_LENGTH(N'dbo.WorkerHeartbeat', N'CertificateDaysRemaining') IS NULL ALTER TABLE dbo.WorkerHeartbeat ADD CertificateDaysRemaining int NULL;
GO

UPDATE dbo.WorkerHeartbeat
SET WorkerType=COALESCE(NULLIF(WorkerType,N''),N'SapSync'),
    HostName=COALESCE(NULLIF(HostName,N''),N'legacy'),
    WorkerInstance=COALESCE(NULLIF(WorkerInstance,N''),InstanceName),
    LifecycleState=COALESCE(NULLIF(LifecycleState,N''),CASE WHEN Status=N'Running' THEN N'Running' ELSE N'Stopped' END),
    IsEnabled=COALESCE(IsEnabled,1), EnabledCompanyCount=COALESCE(EnabledCompanyCount,0),
    PendingCount=COALESCE(PendingCount,0),RetryScheduledCount=COALESCE(RetryScheduledCount,0),
    DeadLetterCount=COALESCE(DeadLetterCount,0),RecentDeadLetterCount=COALESCE(RecentDeadLetterCount,0),
    ActiveLeaseCount=COALESCE(ActiveLeaseCount,0),ExpiredLeaseCount=COALESCE(ExpiredLeaseCount,0)
WHERE WorkerType IS NULL OR HostName IS NULL OR WorkerInstance IS NULL OR LifecycleState IS NULL OR IsEnabled IS NULL
   OR EnabledCompanyCount IS NULL OR PendingCount IS NULL OR RetryScheduledCount IS NULL OR DeadLetterCount IS NULL
   OR RecentDeadLetterCount IS NULL OR ActiveLeaseCount IS NULL OR ExpiredLeaseCount IS NULL;
GO

ALTER TABLE dbo.WorkerHeartbeat ALTER COLUMN WorkerType nvarchar(40) NOT NULL;
ALTER TABLE dbo.WorkerHeartbeat ALTER COLUMN HostName nvarchar(120) NOT NULL;
ALTER TABLE dbo.WorkerHeartbeat ALTER COLUMN WorkerInstance nvarchar(120) NULL;
ALTER TABLE dbo.WorkerHeartbeat ALTER COLUMN LifecycleState nvarchar(30) NOT NULL;
ALTER TABLE dbo.WorkerHeartbeat ALTER COLUMN IsEnabled bit NOT NULL;
ALTER TABLE dbo.WorkerHeartbeat ALTER COLUMN EnabledCompanyCount int NOT NULL;
ALTER TABLE dbo.WorkerHeartbeat ALTER COLUMN PendingCount bigint NOT NULL;
ALTER TABLE dbo.WorkerHeartbeat ALTER COLUMN RetryScheduledCount bigint NOT NULL;
ALTER TABLE dbo.WorkerHeartbeat ALTER COLUMN DeadLetterCount bigint NOT NULL;
ALTER TABLE dbo.WorkerHeartbeat ALTER COLUMN RecentDeadLetterCount bigint NOT NULL;
ALTER TABLE dbo.WorkerHeartbeat ALTER COLUMN ActiveLeaseCount bigint NOT NULL;
ALTER TABLE dbo.WorkerHeartbeat ALTER COLUMN ExpiredLeaseCount bigint NOT NULL;
GO

IF NOT EXISTS(SELECT 1 FROM sys.default_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.WorkerHeartbeat') AND name=N'DF_WorkerHeartbeat_WorkerType') ALTER TABLE dbo.WorkerHeartbeat ADD CONSTRAINT DF_WorkerHeartbeat_WorkerType DEFAULT N'SapSync' FOR WorkerType;
IF NOT EXISTS(SELECT 1 FROM sys.default_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.WorkerHeartbeat') AND name=N'DF_WorkerHeartbeat_HostName') ALTER TABLE dbo.WorkerHeartbeat ADD CONSTRAINT DF_WorkerHeartbeat_HostName DEFAULT N'legacy' FOR HostName;
IF NOT EXISTS(SELECT 1 FROM sys.default_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.WorkerHeartbeat') AND name=N'DF_WorkerHeartbeat_LifecycleState') ALTER TABLE dbo.WorkerHeartbeat ADD CONSTRAINT DF_WorkerHeartbeat_LifecycleState DEFAULT N'Stopped' FOR LifecycleState;
IF NOT EXISTS(SELECT 1 FROM sys.default_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.WorkerHeartbeat') AND name=N'DF_WorkerHeartbeat_IsEnabled') ALTER TABLE dbo.WorkerHeartbeat ADD CONSTRAINT DF_WorkerHeartbeat_IsEnabled DEFAULT 1 FOR IsEnabled;
IF NOT EXISTS(SELECT 1 FROM sys.default_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.WorkerHeartbeat') AND name=N'DF_WorkerHeartbeat_EnabledCompanyCount') ALTER TABLE dbo.WorkerHeartbeat ADD CONSTRAINT DF_WorkerHeartbeat_EnabledCompanyCount DEFAULT 0 FOR EnabledCompanyCount;
IF NOT EXISTS(SELECT 1 FROM sys.default_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.WorkerHeartbeat') AND name=N'DF_WorkerHeartbeat_PendingCount') ALTER TABLE dbo.WorkerHeartbeat ADD CONSTRAINT DF_WorkerHeartbeat_PendingCount DEFAULT 0 FOR PendingCount;
IF NOT EXISTS(SELECT 1 FROM sys.default_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.WorkerHeartbeat') AND name=N'DF_WorkerHeartbeat_RetryScheduledCount') ALTER TABLE dbo.WorkerHeartbeat ADD CONSTRAINT DF_WorkerHeartbeat_RetryScheduledCount DEFAULT 0 FOR RetryScheduledCount;
IF NOT EXISTS(SELECT 1 FROM sys.default_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.WorkerHeartbeat') AND name=N'DF_WorkerHeartbeat_DeadLetterCount') ALTER TABLE dbo.WorkerHeartbeat ADD CONSTRAINT DF_WorkerHeartbeat_DeadLetterCount DEFAULT 0 FOR DeadLetterCount;
IF NOT EXISTS(SELECT 1 FROM sys.default_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.WorkerHeartbeat') AND name=N'DF_WorkerHeartbeat_RecentDeadLetterCount') ALTER TABLE dbo.WorkerHeartbeat ADD CONSTRAINT DF_WorkerHeartbeat_RecentDeadLetterCount DEFAULT 0 FOR RecentDeadLetterCount;
IF NOT EXISTS(SELECT 1 FROM sys.default_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.WorkerHeartbeat') AND name=N'DF_WorkerHeartbeat_ActiveLeaseCount') ALTER TABLE dbo.WorkerHeartbeat ADD CONSTRAINT DF_WorkerHeartbeat_ActiveLeaseCount DEFAULT 0 FOR ActiveLeaseCount;
IF NOT EXISTS(SELECT 1 FROM sys.default_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.WorkerHeartbeat') AND name=N'DF_WorkerHeartbeat_ExpiredLeaseCount') ALTER TABLE dbo.WorkerHeartbeat ADD CONSTRAINT DF_WorkerHeartbeat_ExpiredLeaseCount DEFAULT 0 FOR ExpiredLeaseCount;
GO

IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.WorkerHeartbeat') AND name=N'CK_WorkerHeartbeat_LifecycleState')
 ALTER TABLE dbo.WorkerHeartbeat ADD CONSTRAINT CK_WorkerHeartbeat_LifecycleState CHECK(LifecycleState IN(N'Starting',N'Running',N'Stopping',N'Stopped',N'Faulted',N'Disabled'));
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.WorkerHeartbeat') AND name=N'CK_WorkerHeartbeat_OperationalCounts')
 ALTER TABLE dbo.WorkerHeartbeat ADD CONSTRAINT CK_WorkerHeartbeat_OperationalCounts CHECK(EnabledCompanyCount>=0 AND PendingCount>=0 AND RetryScheduledCount>=0 AND DeadLetterCount>=0 AND RecentDeadLetterCount>=0 AND ActiveLeaseCount>=0 AND ExpiredLeaseCount>=0);
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.WorkerHeartbeat') AND name=N'CK_WorkerHeartbeat_StorageFreePercent')
 ALTER TABLE dbo.WorkerHeartbeat ADD CONSTRAINT CK_WorkerHeartbeat_StorageFreePercent CHECK(StorageFreePercent IS NULL OR StorageFreePercent BETWEEN 0 AND 100);
GO

IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.WorkerHeartbeat') AND name=N'UX_WorkerHeartbeat_LogicalIdentity')
    CREATE UNIQUE INDEX UX_WorkerHeartbeat_LogicalIdentity ON dbo.WorkerHeartbeat(WorkerType,HostName,WorkerInstance) WHERE WorkerInstance IS NOT NULL;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_WORKERHEARTBEAT_REGISTRAR
    @InstanceName nvarchar(120),@CompanyId int=NULL,@CompanyCode nvarchar(50)=NULL,@Status nvarchar(30),
    @CurrentJob nvarchar(300)=NULL,@WorkerVersion nvarchar(80)=NULL,@LastBeatAtUtc datetime2(0),
    @WorkerType nvarchar(40)=N'SapSync',@HostName nvarchar(120)=NULL,@WorkerInstance nvarchar(120)=NULL,
    @LifecycleState nvarchar(30)=NULL,@IsEnabled bit=1,@StartedAtUtc datetime2(0)=NULL,
    @LastCycleStartedAtUtc datetime2(0)=NULL,@LastCycleCompletedAtUtc datetime2(0)=NULL,
    @LastSuccessfulCycleAtUtc datetime2(0)=NULL,@LastCycleDurationMs int=NULL,@LastCycleResult nvarchar(40)=NULL,
    @LastSafeErrorCode nvarchar(80)=NULL,@LastSafeErrorMessage nvarchar(300)=NULL,@EnabledCompanyCount int=0,
    @PendingCount bigint=0,@RetryScheduledCount bigint=0,@DeadLetterCount bigint=0,@RecentDeadLetterCount bigint=0,
    @ActiveLeaseCount bigint=0,@ExpiredLeaseCount bigint=0,@OldestPendingAtUtc datetime2(0)=NULL,
    @StorageFreePercent decimal(5,2)=NULL,@CertificateDaysRemaining int=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 IF NULLIF(LTRIM(RTRIM(@InstanceName)),N'') IS NULL OR NULLIF(LTRIM(RTRIM(@WorkerType)),N'') IS NULL THROW 50001,N'Identidad de worker invalida.',1;
 SET @HostName=COALESCE(NULLIF(LTRIM(RTRIM(@HostName)),N''),N'legacy');
 SET @WorkerInstance=COALESCE(NULLIF(LTRIM(RTRIM(@WorkerInstance)),N''),@InstanceName);
 SET @LifecycleState=COALESCE(NULLIF(@LifecycleState,N''),CASE WHEN @WorkerType=N'SapSync' AND @Status=N'Running' THEN N'Running' WHEN @WorkerType=N'SapSync' THEN N'Stopped' ELSE @Status END);
 BEGIN TRANSACTION;
 UPDATE dbo.WorkerHeartbeat WITH(UPDLOCK,SERIALIZABLE) SET CompanyId=@CompanyId,CompanyCode=@CompanyCode,LastBeatAt=@LastBeatAtUtc,
  Status=@Status,CurrentJob=@CurrentJob,WorkerVersion=@WorkerVersion,UpdatedAt=SYSUTCDATETIME(),WorkerType=@WorkerType,
  HostName=@HostName,WorkerInstance=@WorkerInstance,LifecycleState=@LifecycleState,IsEnabled=@IsEnabled,
  StartedAt=COALESCE(@StartedAtUtc,StartedAt),LastCycleStartedAt=@LastCycleStartedAtUtc,LastCycleCompletedAt=@LastCycleCompletedAtUtc,
  LastSuccessfulCycleAt=COALESCE(@LastSuccessfulCycleAtUtc,LastSuccessfulCycleAt),LastCycleDurationMs=@LastCycleDurationMs,
  LastCycleResult=@LastCycleResult,LastSafeErrorCode=@LastSafeErrorCode,LastSafeErrorMessage=@LastSafeErrorMessage,
  EnabledCompanyCount=@EnabledCompanyCount,PendingCount=@PendingCount,RetryScheduledCount=@RetryScheduledCount,
  DeadLetterCount=@DeadLetterCount,RecentDeadLetterCount=@RecentDeadLetterCount,ActiveLeaseCount=@ActiveLeaseCount,
  ExpiredLeaseCount=@ExpiredLeaseCount,OldestPendingAt=@OldestPendingAtUtc,StorageFreePercent=@StorageFreePercent,
  CertificateDaysRemaining=@CertificateDaysRemaining WHERE InstanceName=@InstanceName;
 IF @@ROWCOUNT=0 INSERT dbo.WorkerHeartbeat(InstanceName,CompanyId,CompanyCode,LastBeatAt,Status,CurrentJob,WorkerVersion,
  WorkerType,HostName,WorkerInstance,LifecycleState,IsEnabled,StartedAt,LastCycleStartedAt,LastCycleCompletedAt,
  LastSuccessfulCycleAt,LastCycleDurationMs,LastCycleResult,LastSafeErrorCode,LastSafeErrorMessage,EnabledCompanyCount,
  PendingCount,RetryScheduledCount,DeadLetterCount,RecentDeadLetterCount,ActiveLeaseCount,ExpiredLeaseCount,OldestPendingAt,
  StorageFreePercent,CertificateDaysRemaining)
 VALUES(@InstanceName,@CompanyId,@CompanyCode,@LastBeatAtUtc,@Status,@CurrentJob,@WorkerVersion,@WorkerType,@HostName,
  @WorkerInstance,@LifecycleState,@IsEnabled,@StartedAtUtc,@LastCycleStartedAtUtc,@LastCycleCompletedAtUtc,
  @LastSuccessfulCycleAtUtc,@LastCycleDurationMs,@LastCycleResult,@LastSafeErrorCode,@LastSafeErrorMessage,@EnabledCompanyCount,
  @PendingCount,@RetryScheduledCount,@DeadLetterCount,@RecentDeadLetterCount,@ActiveLeaseCount,@ExpiredLeaseCount,
  @OldestPendingAtUtc,@StorageFreePercent,@CertificateDaysRemaining);
 COMMIT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_WORKERHEARTBEAT_LISTARPORCONFIGURACION @WorkerType nvarchar(40)
AS
BEGIN
 SET NOCOUNT ON;
 SELECT WorkerType,HostName,WorkerInstance,LifecycleState,IsEnabled,WorkerVersion,LastBeatAt LastBeatAtUtc,
  StartedAt StartedAtUtc,LastCycleStartedAt LastCycleStartedAtUtc,LastCycleCompletedAt LastCycleCompletedAtUtc,
  LastSuccessfulCycleAt LastSuccessfulCycleAtUtc,LastCycleDurationMs,LastCycleResult,LastSafeErrorCode,LastSafeErrorMessage,
  EnabledCompanyCount,PendingCount,RetryScheduledCount,DeadLetterCount,RecentDeadLetterCount,ActiveLeaseCount,
  ExpiredLeaseCount,OldestPendingAt OldestPendingAtUtc,StorageFreePercent,CertificateDaysRemaining
 FROM dbo.WorkerHeartbeat WHERE WorkerType=@WorkerType ORDER BY HostName,WorkerInstance;
END;
GO

DECLARE @ModuleId int=(SELECT TOP(1) Id FROM dbo.Modules WHERE Code=N'SRI');
DECLARE @AdminRoleId int=(SELECT TOP(1) Id FROM dbo.Roles WHERE Code=N'ADMIN' AND IsDeleted=0);
IF NOT EXISTS(SELECT 1 FROM dbo.Permissions WHERE Code=N'SRI.WORKER.HEALTH.VIEW')
 INSERT dbo.Permissions(ModuleId,Code,Name,Description) VALUES(@ModuleId,N'SRI.WORKER.HEALTH.VIEW',N'Ver salud del SRI Worker',N'Consultar estado operativo seguro del SRI Worker.');
ELSE UPDATE dbo.Permissions SET ModuleId=@ModuleId,Name=N'Ver salud del SRI Worker',Description=N'Consultar estado operativo seguro del SRI Worker.',IsActive=1,UpdatedAt=SYSUTCDATETIME() WHERE Code=N'SRI.WORKER.HEALTH.VIEW';
IF @AdminRoleId IS NOT NULL INSERT dbo.RolePermissions(RoleId,PermissionId)
 SELECT @AdminRoleId,p.Id FROM dbo.Permissions p WHERE p.Code=N'SRI.WORKER.HEALTH.VIEW'
 AND NOT EXISTS(SELECT 1 FROM dbo.RolePermissions rp WHERE rp.RoleId=@AdminRoleId AND rp.PermissionId=p.Id);
DECLARE @FormId int=(SELECT TOP(1) Id FROM dbo.SecurityForms WHERE FormKey=N'sri-document-monitor' AND IsDeleted=0);
DECLARE @OperationId int=(SELECT TOP(1) Id FROM dbo.SecurityOperations WHERE ActionKey=N'view-worker-health' AND IsDeleted=0);
IF @OperationId IS NULL BEGIN
 INSERT dbo.SecurityOperations(Code,Name,Description,ActionKey,DisplayOrder,IsActive,CreatedByUserName,CreatedAt,IsDeleted)
 VALUES(N'ACTION.VIEW_WORKER_HEALTH',N'Ver salud del worker',N'Consultar salud operativa segura.',N'view-worker-health',40,1,N'Sistema',SYSUTCDATETIME(),0);
 SET @OperationId=CONVERT(int,SCOPE_IDENTITY());
END;
IF @AdminRoleId IS NOT NULL AND @FormId IS NOT NULL AND @OperationId IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.SecurityRoleFormOperations WHERE RoleId=@AdminRoleId AND FormId=@FormId AND OperationId=@OperationId)
 INSERT dbo.SecurityRoleFormOperations(RoleId,FormId,OperationId,IsAllowed,CreatedByUserName,CreatedAt) VALUES(@AdminRoleId,@FormId,@OperationId,1,N'Sistema',SYSUTCDATETIME());
GO

IF OBJECT_ID(N'dbo.MasterSchemaHistory',N'U') IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260721.120')
 INSERT dbo.MasterSchemaHistory(Version,Description) VALUES(N'20260721.120',N'Heartbeat operacional compartido y salud del SRI Worker');
GO
