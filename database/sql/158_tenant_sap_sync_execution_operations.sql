/* Fase 10.5: operaciones seguras de historial, reintento y recuperacion SAP. */
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO
IF NOT EXISTS (SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260731.153')
    THROW 51158, 'Migration 153 is required.', 1;
GO
IF EXISTS(SELECT 1 FROM sys.check_constraints WHERE name=N'CK_AuditSapSyncExecutionChanges_Action')
 ALTER TABLE dbo.AuditSapSyncExecutionChanges DROP CONSTRAINT CK_AuditSapSyncExecutionChanges_Action;
ALTER TABLE dbo.AuditSapSyncExecutionChanges WITH CHECK ADD CONSTRAINT CK_AuditSapSyncExecutionChanges_Action CHECK
(Action IN('Created','Transitioned','CancellationRequested','DetailCreated','DetailUpdated','DetailClaimed',
 'DetailLockRenewed','DetailLockReleased','ManualRetryCreated','DetailCompleted','ExpiredLockRecovered','ExpiredLockReleased'));
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SAPSYNCEXECUTIONDETALLEPAGINAR
 @ExecutionUid uniqueidentifier, @Status varchar(30)=NULL, @SourceRecordKey nvarchar(120)=NULL,
 @PageNumber int=1, @PageSize int=100
AS
BEGIN
 SET NOCOUNT ON;
 SET @PageNumber=CASE WHEN @PageNumber<1 THEN 1 ELSE @PageNumber END;
 SET @PageSize=CASE WHEN @PageSize BETWEEN 1 AND 500 THEN @PageSize ELSE 100 END;
 ;WITH Q AS
 (
  SELECT d.Id,e.ExecutionUid,d.SourceRecordKey,d.SourceVersion,d.LocalEntityId,d.LocalGlobalId,
   d.Action,d.Status,d.AttemptCount,d.MaxAttempts,d.NextAttemptAtUtc,d.ErrorClass,d.ResultCode,
   d.SafeMessage,d.StartedAtUtc,d.FinishedAtUtc,d.RowVersion
  FROM dbo.SapSyncExecutionDetails d JOIN dbo.SapSyncExecutions e ON e.Id=d.SapSyncExecutionId
  WHERE e.ExecutionUid=@ExecutionUid AND (@Status IS NULL OR d.Status=@Status)
   AND (@SourceRecordKey IS NULL OR d.SourceRecordKey LIKE N'%'+@SourceRecordKey+N'%')
 )
 SELECT * FROM Q ORDER BY SourceRecordKey,Id OFFSET (@PageNumber-1)*@PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
 SELECT COUNT(1) TotalCount FROM dbo.SapSyncExecutionDetails d JOIN dbo.SapSyncExecutions e ON e.Id=d.SapSyncExecutionId
 WHERE e.ExecutionUid=@ExecutionUid AND (@Status IS NULL OR d.Status=@Status)
  AND (@SourceRecordKey IS NULL OR d.SourceRecordKey LIKE N'%'+@SourceRecordKey+N'%');
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SAPSYNCEXECUTIONDETALLECLAIM
 @WorkerInstance nvarchar(120), @OwnerToken char(64), @LockExpiresAtUtc datetime2(0),
 @ApprovedSnapshotTypesCsv nvarchar(500)
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 DECLARE @Now datetime2(0)=SYSUTCDATETIME(), @Id bigint;
 BEGIN TRAN;
 SELECT TOP(1) @Id=d.Id FROM dbo.SapSyncExecutionDetails d WITH(UPDLOCK,READPAST,ROWLOCK)
 JOIN dbo.SapSyncExecutions e ON e.Id=d.SapSyncExecutionId
 WHERE d.Status='RetryScheduled' AND (d.NextAttemptAtUtc IS NULL OR d.NextAttemptAtUtc<=@Now)
  AND d.OwnerToken IS NULL AND e.Status IN('Running','RetryScheduled')
  AND d.ApprovedSnapshotType IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT(@ApprovedSnapshotTypesCsv,','))
 ORDER BY ISNULL(d.NextAttemptAtUtc,'19000101'),d.Id;
 IF @Id IS NOT NULL UPDATE dbo.SapSyncExecutionDetails SET Status='Processing',AttemptCount=AttemptCount+1,
  WorkerInstance=@WorkerInstance,OwnerToken=@OwnerToken,LockedAtUtc=@Now,LockExpiresAtUtc=@LockExpiresAtUtc,
  StartedAtUtc=COALESCE(StartedAtUtc,@Now),UpdatedAt=@Now WHERE Id=@Id;
 COMMIT;
 SELECT d.Id,e.ExecutionUid,d.SourceRecordKey,d.Status,d.AttemptCount,d.MaxAttempts,d.ApprovedSnapshotType,
  d.ApprovedSnapshotJson,d.SnapshotHash,d.OwnerToken,d.LockedAtUtc,d.LockExpiresAtUtc
 FROM dbo.SapSyncExecutionDetails d JOIN dbo.SapSyncExecutions e ON e.Id=d.SapSyncExecutionId WHERE d.Id=@Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PATCH_SAPSYNCEXECUTIONDETALLECOMPLETAR
 @DetailId bigint,@OwnerToken char(64),@Action varchar(20),@Status varchar(30),@LocalEntityId bigint=NULL,
 @LocalGlobalId uniqueidentifier=NULL,@ErrorClass varchar(20)=NULL,@ResultCode nvarchar(120)=NULL,
 @SafeMessage nvarchar(1000)=NULL,@NextAttemptAtUtc datetime2(0)=NULL
AS
BEGIN
 SET NOCOUNT ON; DECLARE @Changed int=0,@ExecutionId bigint;
 SELECT @ExecutionId=SapSyncExecutionId FROM dbo.SapSyncExecutionDetails WHERE Id=@DetailId;
 UPDATE dbo.SapSyncExecutionDetails SET Action=@Action,Status=@Status,LocalEntityId=@LocalEntityId,
  LocalGlobalId=@LocalGlobalId,ErrorClass=@ErrorClass,ResultCode=@ResultCode,SafeMessage=@SafeMessage,
  NextAttemptAtUtc=@NextAttemptAtUtc,WorkerInstance=NULL,OwnerToken=NULL,LockedAtUtc=NULL,RenewedAtUtc=NULL,
  LockExpiresAtUtc=NULL,FinishedAtUtc=CASE WHEN @Status='RetryScheduled' THEN NULL ELSE SYSUTCDATETIME() END,
  UpdatedAt=SYSUTCDATETIME() WHERE Id=@DetailId AND Status='Processing' AND OwnerToken=@OwnerToken;
 SET @Changed=@@ROWCOUNT;
 IF @Changed=1
 INSERT dbo.AuditSapSyncExecutionChanges(SapSyncExecutionId,SapSyncExecutionDetailId,Action,PreviousStatus,NewStatus,Reason,WorkerInstance)
 VALUES(@ExecutionId,@DetailId,'DetailCompleted','Processing',@Status,@SafeMessage,NULL);
 IF @Changed=1
 UPDATE e SET TotalRecords=x.TotalRecords,CreatedRecords=x.CreatedRecords,UpdatedRecords=x.UpdatedRecords,
  UnchangedRecords=x.UnchangedRecords,ApprovalRequiredRecords=x.ApprovalRequiredRecords,
  ConflictRecords=x.ConflictRecords,SkippedRecords=x.SkippedRecords,RetryScheduledRecords=x.RetryScheduledRecords,
  FailedRecords=x.FailedRecords,DeadLetterRecords=x.DeadLetterRecords,LastProgressAtUtc=SYSUTCDATETIME(),
  Status=CASE WHEN x.OpenRecords>0 THEN 'Running' WHEN x.RetryScheduledRecords>0 THEN 'RetryScheduled'
   WHEN x.FailedRecords+x.DeadLetterRecords=x.TotalRecords THEN 'Failed'
   WHEN x.FailedRecords+x.DeadLetterRecords>0 THEN 'CompletedWithErrors'
   WHEN x.ApprovalRequiredRecords+x.ConflictRecords>0 THEN 'CompletedWithWarnings' ELSE 'Completed' END,
  FinishedAtUtc=CASE WHEN x.OpenRecords+x.RetryScheduledRecords=0 THEN SYSUTCDATETIME() END,
  UpdatedAt=SYSUTCDATETIME()
 FROM dbo.SapSyncExecutions e CROSS APPLY(SELECT COUNT(1) TotalRecords,
  SUM(CASE WHEN Status='Created' THEN 1 ELSE 0 END) CreatedRecords,SUM(CASE WHEN Status='Updated' THEN 1 ELSE 0 END) UpdatedRecords,
  SUM(CASE WHEN Status='Unchanged' THEN 1 ELSE 0 END) UnchangedRecords,SUM(CASE WHEN Status='ApprovalRequired' THEN 1 ELSE 0 END) ApprovalRequiredRecords,
  SUM(CASE WHEN Status='Conflict' THEN 1 ELSE 0 END) ConflictRecords,SUM(CASE WHEN Status='Skipped' THEN 1 ELSE 0 END) SkippedRecords,
  SUM(CASE WHEN Status='RetryScheduled' THEN 1 ELSE 0 END) RetryScheduledRecords,SUM(CASE WHEN Status='Failed' THEN 1 ELSE 0 END) FailedRecords,
  SUM(CASE WHEN Status='DeadLetter' THEN 1 ELSE 0 END) DeadLetterRecords,SUM(CASE WHEN Status IN('Pending','Processing') THEN 1 ELSE 0 END) OpenRecords
  FROM dbo.SapSyncExecutionDetails WHERE SapSyncExecutionId=@ExecutionId)x WHERE e.Id=@ExecutionId;
 SELECT CAST(@DetailId AS bigint) Id,CASE WHEN @Changed=1 THEN 'Updated' ELSE 'LockLost' END ResultCode,RowVersion
 FROM dbo.SapSyncExecutionDetails WHERE Id=@DetailId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SAPSYNCEXECUTIONDETALLERECUPERARVENCIDOS @UtcNow datetime2(0)
AS
BEGIN
 SET NOCOUNT ON; DECLARE @Changed table(Id bigint,SapSyncExecutionId bigint,NewStatus varchar(30));
 UPDATE dbo.SapSyncExecutionDetails SET Status=CASE WHEN AttemptCount>=MaxAttempts THEN 'DeadLetter' ELSE 'RetryScheduled' END,
  ErrorClass='Transient',ResultCode='SAP_SYNC_LEASE_EXPIRED',SafeMessage=N'Lease vencido recuperado.',
  NextAttemptAtUtc=CASE WHEN AttemptCount<MaxAttempts THEN @UtcNow END,WorkerInstance=NULL,OwnerToken=NULL,
  LockedAtUtc=NULL,RenewedAtUtc=NULL,LockExpiresAtUtc=NULL,UpdatedAt=@UtcNow
 OUTPUT inserted.Id,inserted.SapSyncExecutionId,inserted.Status INTO @Changed
 WHERE Status='Processing' AND LockExpiresAtUtc<@UtcNow;
 INSERT dbo.AuditSapSyncExecutionChanges(SapSyncExecutionId,SapSyncExecutionDetailId,Action,PreviousStatus,NewStatus,Reason)
 SELECT SapSyncExecutionId,Id,'ExpiredLockRecovered','Processing',NewStatus,N'Lease vencido recuperado.' FROM @Changed;
 SELECT COUNT(1) FROM @Changed;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PATCH_SAPSYNCEXECUTIONDETALLELIBERARVENCIDO
 @DetailId bigint,@Reason nvarchar(500),@RequestedByUserId int=NULL,@RequestedByUserName nvarchar(120)=NULL,
 @ExpectedRowVersion varbinary(8)
AS
BEGIN
 SET NOCOUNT ON; DECLARE @ExecutionId bigint,@Changed int=0;
 IF NULLIF(LTRIM(RTRIM(@Reason)),N'') IS NULL THROW 51158,'Release reason is required.',1;
 SELECT @ExecutionId=SapSyncExecutionId FROM dbo.SapSyncExecutionDetails WHERE Id=@DetailId;
 UPDATE dbo.SapSyncExecutionDetails SET Status=CASE WHEN AttemptCount>=MaxAttempts THEN 'DeadLetter' ELSE 'RetryScheduled' END,
  ResultCode='SAP_SYNC_LEASE_RELEASED',SafeMessage=@Reason,NextAttemptAtUtc=SYSUTCDATETIME(),WorkerInstance=NULL,
  OwnerToken=NULL,LockedAtUtc=NULL,RenewedAtUtc=NULL,LockExpiresAtUtc=NULL,UpdatedAt=SYSUTCDATETIME()
 WHERE Id=@DetailId AND Status='Processing' AND LockExpiresAtUtc<SYSUTCDATETIME() AND RowVersion=@ExpectedRowVersion;
 SET @Changed=@@ROWCOUNT;
 IF @Changed=1 INSERT dbo.AuditSapSyncExecutionChanges(SapSyncExecutionId,SapSyncExecutionDetailId,Action,PreviousStatus,NewStatus,Reason,UserId,UserName)
 SELECT @ExecutionId,@DetailId,'ExpiredLockReleased','Processing',Status,@Reason,@RequestedByUserId,@RequestedByUserName FROM dbo.SapSyncExecutionDetails WHERE Id=@DetailId;
 SELECT @DetailId Id,CASE WHEN @Changed=1 THEN 'Updated' ELSE 'ConcurrencyConflict' END ResultCode,RowVersion
 FROM dbo.SapSyncExecutionDetails WHERE Id=@DetailId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SAPSYNCEXECUTIONREINTENTOMANUAL
 @ParentExecutionUid uniqueidentifier,@ClientRequestId uniqueidentifier,@Reason nvarchar(500),
 @RequestedByUserId int=NULL,@RequestedByUserName nvarchar(120)=NULL,@ExpectedRowVersion varbinary(8)
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 DECLARE @ParentId bigint,@NewId bigint;
 IF NULLIF(LTRIM(RTRIM(@Reason)),N'') IS NULL THROW 51158,'Retry reason is required.',1;
 IF EXISTS(SELECT 1 FROM dbo.SapSyncExecutions WHERE ExecutionUid=@ClientRequestId)
 BEGIN SELECT Id,ExecutionUid,'Existing' ResultCode,RowVersion FROM dbo.SapSyncExecutions WHERE ExecutionUid=@ClientRequestId; RETURN; END;
 BEGIN TRAN;
 SELECT @ParentId=Id FROM dbo.SapSyncExecutions WITH(UPDLOCK,HOLDLOCK) WHERE ExecutionUid=@ParentExecutionUid
  AND RowVersion=@ExpectedRowVersion AND Status IN('Failed','CompletedWithErrors');
 IF @ParentId IS NULL BEGIN ROLLBACK; SELECT CAST(NULL AS bigint) Id,CAST(NULL AS uniqueidentifier) ExecutionUid,'RetryNotAllowed' ResultCode,CAST(NULL AS varbinary(8)) RowVersion; RETURN; END;
 INSERT dbo.SapSyncExecutions(ExecutionUid,RunGroupId,CorrelationId,SapSyncProfileId,SapSyncProfileEntityId,ProfileCode,ProfileName,CompanyId,CompanyCode,EntityCode,Direction,TriggerType,ParentExecutionId,Status,BatchSize,MaxAttempts,ExecutionOrder,TimeoutMinutes,ScheduleType,TimeZoneId,ProfileSnapshotJson,EffectiveParametersJson,RequestedByUserId,RequestedByUserName)
 SELECT @ClientRequestId,RunGroupId,NEWID(),SapSyncProfileId,SapSyncProfileEntityId,ProfileCode,ProfileName,CompanyId,CompanyCode,EntityCode,Direction,'Retry',Id,'RetryScheduled',BatchSize,MaxAttempts,ExecutionOrder,TimeoutMinutes,ScheduleType,TimeZoneId,ProfileSnapshotJson,EffectiveParametersJson,@RequestedByUserId,@RequestedByUserName FROM dbo.SapSyncExecutions WHERE Id=@ParentId;
 SET @NewId=SCOPE_IDENTITY();
 INSERT dbo.SapSyncExecutionDetails(SapSyncExecutionId,SourceRecordKey,SourceVersion,Action,Status,AttemptCount,MaxAttempts,NextAttemptAtUtc,ApprovedSnapshotType,ApprovedSnapshotJson,SnapshotHash)
 SELECT @NewId,SourceRecordKey,SourceVersion,Action,'RetryScheduled',0,MaxAttempts,SYSUTCDATETIME(),ApprovedSnapshotType,ApprovedSnapshotJson,SnapshotHash FROM dbo.SapSyncExecutionDetails WHERE SapSyncExecutionId=@ParentId AND Status IN('Failed','DeadLetter') AND ApprovedSnapshotJson IS NOT NULL;
 IF @@ROWCOUNT=0 BEGIN ROLLBACK; SELECT CAST(NULL AS bigint) Id,CAST(NULL AS uniqueidentifier) ExecutionUid,'NoRetryableDetails' ResultCode,CAST(NULL AS varbinary(8)) RowVersion; RETURN; END;
 INSERT dbo.AuditSapSyncExecutionChanges(SapSyncExecutionId,Action,PreviousStatus,NewStatus,Reason,UserId,UserName)
 VALUES(@NewId,'ManualRetryCreated',NULL,'RetryScheduled',@Reason,@RequestedByUserId,@RequestedByUserName);
 COMMIT; SELECT Id,ExecutionUid,'Created' ResultCode,RowVersion FROM dbo.SapSyncExecutions WHERE Id=@NewId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SAPSYNCTECHNICALLOGCREAR
 @CompanyId int,@CompanyCode nvarchar(50),@EntityCode nvarchar(80),@Direction varchar(20),@Operation varchar(40),
 @Status varchar(30),@CorrelationId uniqueidentifier,@WorkerInstance nvarchar(120)=NULL,@AttemptCount int,
 @QueueItemId bigint=NULL,@LocalEntityId bigint=NULL,@SapEntityId nvarchar(120)=NULL,@SapDocEntry int=NULL,@SapDocNum int=NULL,
 @RequestJson nvarchar(max)=NULL,@ResponseJson nvarchar(max)=NULL,@ErrorCode nvarchar(120)=NULL,@ErrorMessage nvarchar(1000)=NULL,
 @DurationMs bigint=NULL,@StartedAtUtc datetime2(0),@FinishedAtUtc datetime2(0)=NULL
AS
BEGIN
 SET NOCOUNT ON;
 INSERT dbo.SapSyncTechnicalLog(CompanyId,CompanyCode,EntityCode,Direction,Operation,Status,CorrelationId,WorkerInstance,
  AttemptCount,QueueItemId,LocalEntityId,SapEntityId,SapDocEntry,SapDocNum,RequestJson,ResponseJson,ErrorCode,ErrorMessage,
  DurationMs,StartedAtUtc,FinishedAtUtc)
 VALUES(@CompanyId,@CompanyCode,@EntityCode,@Direction,@Operation,@Status,@CorrelationId,@WorkerInstance,@AttemptCount,
  @QueueItemId,@LocalEntityId,@SapEntityId,@SapDocEntry,@SapDocNum,@RequestJson,@ResponseJson,@ErrorCode,@ErrorMessage,
  @DurationMs,@StartedAtUtc,@FinishedAtUtc);
 SELECT CAST(SCOPE_IDENTITY() AS bigint);
END;
GO

IF NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260731.158')
 INSERT dbo.SchemaHistory(Version,Description) VALUES(N'20260731.158',N'Fase 10.5: historial y reintentos SAP');
GO
