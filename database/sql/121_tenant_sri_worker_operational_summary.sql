/* Iteracion 6: resumen operacional seguro del SRI Worker. Tenant only; prerequisite 117. */
SET NOCOUNT ON;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SRIWORKER_RESUMENOPERATIVO
AS
BEGIN
 SET NOCOUNT ON;
 SELECT
  COALESCE(SUM(CASE WHEN Status=N'Pending' THEN CONVERT(bigint,1) ELSE 0 END),0) PendingCount,
  COALESCE(SUM(CASE WHEN Status=N'RetryScheduled' THEN CONVERT(bigint,1) ELSE 0 END),0) RetryScheduledCount,
  COALESCE(SUM(CASE WHEN Status=N'DeadLetter' THEN CONVERT(bigint,1) ELSE 0 END),0) DeadLetterCount,
  COALESCE(SUM(CASE WHEN Status=N'DeadLetter' AND CompletedAt>=DATEADD(minute,-15,SYSUTCDATETIME()) THEN CONVERT(bigint,1) ELSE 0 END),0) RecentDeadLetterCount,
  COALESCE(SUM(CASE WHEN Status=N'Querying' AND LockExpiresAt>SYSUTCDATETIME() THEN CONVERT(bigint,1) ELSE 0 END),0) ActiveLeaseCount,
  COALESCE(SUM(CASE WHEN Status=N'Querying' AND LockExpiresAt<=SYSUTCDATETIME() THEN CONVERT(bigint,1) ELSE 0 END),0) ExpiredLeaseCount,
  MIN(CASE WHEN Status=N'Pending' THEN CreatedAt END) OldestPendingAtUtc
 FROM dbo.SriDocumentQueue;
END;
GO
IF OBJECT_ID(N'dbo.SchemaHistory',N'U') IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260721.121')
 INSERT dbo.SchemaHistory(Version,Description) VALUES(N'20260721.121',N'Resumen operacional seguro del SRI Worker');
GO
