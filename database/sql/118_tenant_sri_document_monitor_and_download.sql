/* Fase 5.5: monitor seguro y descarga auditada de XML autorizado. Tenant only. */
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.AuditSriDocumentChanges') AND name=N'IX_AuditSriDocumentChanges_Queue_CreatedAt')
    CREATE INDEX IX_AuditSriDocumentChanges_Queue_CreatedAt ON dbo.AuditSriDocumentChanges(QueueId, CreatedAt DESC);
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SRIDOCUMENTMONITOR_RESUMEN
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT_BIG(1) Total,
           SUM(CASE WHEN q.Status=N'Pending' THEN 1 ELSE 0 END) Pending,
           SUM(CASE WHEN q.Status=N'Querying' THEN 1 ELSE 0 END) Querying,
           SUM(CASE WHEN q.Status=N'Authorized' THEN 1 ELSE 0 END) Authorized,
           SUM(CASE WHEN q.Status IN(N'Failed',N'DeadLetter') THEN 1 ELSE 0 END) Errors
    FROM dbo.SriDocumentQueue q;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SRIDOCUMENTMONITOR_LISTAR
    @Environment nvarchar(20)=NULL,@Status nvarchar(30)=NULL,@DocumentTypeCode char(2)=NULL,
    @SourceType nvarchar(30)=NULL,@CreatedFrom datetime2(0)=NULL,@CreatedTo datetime2(0)=NULL,
    @Search nvarchar(200)=NULL,@Page int=1,@PageSize int=50
AS
BEGIN
    SET NOCOUNT ON;
    SET @Page=CASE WHEN @Page<1 THEN 1 ELSE @Page END;
    SET @PageSize=CASE WHEN @PageSize<1 THEN 50 WHEN @PageSize>200 THEN 200 ELSE @PageSize END;
    SELECT q.Id QueueId,q.Environment,q.DocumentTypeCode,q.SourceType,q.SourceReference,q.BranchCode,q.Status,
           q.AttemptCount,q.CreatedAt,d.AuthorizationAt,CAST(CASE WHEN d.Id IS NULL THEN 0 ELSE 1 END AS bit) HasXml,
           COUNT_BIG(1) OVER() TotalCount
    FROM dbo.SriDocumentQueue q
    LEFT JOIN dbo.SriAuthorizedDocuments d ON d.QueueId=q.Id
    WHERE (@Environment IS NULL OR q.Environment=@Environment)
      AND (@Status IS NULL OR q.Status=@Status)
      AND (@DocumentTypeCode IS NULL OR q.DocumentTypeCode=@DocumentTypeCode)
      AND (@SourceType IS NULL OR q.SourceType=@SourceType)
      AND (@CreatedFrom IS NULL OR q.CreatedAt>=@CreatedFrom)
      AND (@CreatedTo IS NULL OR q.CreatedAt<DATEADD(day,1,@CreatedTo))
      AND (@Search IS NULL OR q.SourceReference LIKE N'%' + @Search + N'%' OR q.BranchCode LIKE N'%' + @Search + N'%')
    ORDER BY q.CreatedAt DESC,q.Id DESC
    OFFSET (@Page-1)*@PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SRIDOCUMENTMONITOR_BUSCARPORID @QueueId bigint
AS
BEGIN
    SET NOCOUNT ON;
    SELECT q.Id QueueId,q.Environment,q.DocumentTypeCode,q.SourceType,q.SourceReference,q.BranchCode,q.Status,
           q.AttemptCount,q.MaxAttempts,q.CreatedAt,q.UpdatedAt,q.CompletedAt,q.LastErrorCode,
           d.Id DocumentId,d.AuthorizationNumber,d.AuthorizationAt,d.ProviderEnvironment,d.IssuerRuc,
           d.ContentType,d.SizeBytes,CONVERT(varchar(64),d.Sha256,2) Sha256Hex,d.StoredAt,
           CAST(CASE WHEN d.Id IS NULL THEN 0 ELSE 1 END AS bit) HasXml
    FROM dbo.SriDocumentQueue q LEFT JOIN dbo.SriAuthorizedDocuments d ON d.QueueId=q.Id WHERE q.Id=@QueueId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SRIDOCUMENTMONITOR_AUDITORIA @QueueId bigint
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id,QueueId,Action,PreviousStatus,NewStatus,Reason,UserId,UserName,TraceId,CreatedAt
    FROM dbo.AuditSriDocumentChanges WHERE QueueId=@QueueId ORDER BY CreatedAt DESC,Id DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SRIDOCUMENTAUTORIZADO_DESCARGAR
    @QueueId bigint,@AuditUserId int=NULL,@AuditUserName nvarchar(150)=NULL,@TraceId uniqueidentifier
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    BEGIN TRANSACTION;
    DECLARE @Status nvarchar(30),@DocumentId bigint,@Content varbinary(max),@ContentType nvarchar(100),@SizeBytes int;
    SELECT @Status=q.Status,@DocumentId=d.Id,@Content=d.XmlContent,@ContentType=d.ContentType,@SizeBytes=d.SizeBytes
    FROM dbo.SriDocumentQueue q WITH(UPDLOCK,HOLDLOCK)
    LEFT JOIN dbo.SriAuthorizedDocuments d ON d.QueueId=q.Id WHERE q.Id=@QueueId;
    IF @Status IS NULL BEGIN COMMIT; SELECT 0 ResultCode; RETURN; END;
    IF @Status<>N'Authorized' BEGIN COMMIT; SELECT -3 ResultCode; RETURN; END;
    IF @DocumentId IS NULL OR @Content IS NULL OR DATALENGTH(@Content)=0 BEGIN COMMIT; SELECT -4 ResultCode; RETURN; END;
    INSERT dbo.AuditSriDocumentChanges(QueueId,Action,PreviousStatus,NewStatus,Reason,UserId,UserName,TraceId)
    VALUES(@QueueId,N'DownloadXml',N'Authorized',N'Authorized',N'Descarga autorizada de XML persistido.',@AuditUserId,@AuditUserName,@TraceId);
    COMMIT;
    SELECT 1 ResultCode,@DocumentId DocumentId,@QueueId QueueId,@Content XmlContent,@ContentType ContentType,@SizeBytes SizeBytes;
END;
GO

IF OBJECT_ID(N'dbo.SchemaHistory',N'U') IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260721.118')
    INSERT dbo.SchemaHistory(Version,Description) VALUES(N'20260721.118',N'Monitor SRI y descarga protegida con auditoria por acceso');
GO
