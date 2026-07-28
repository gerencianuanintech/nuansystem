/*
    Migracion 150 - Alcance del Monitor SRI por importacion TXT.

    Cuando @ImportId es NULL conserva la consulta global del tenant.
    Cuando se informa, devuelve exclusivamente colas vinculadas a esa carga
    mediante SriTxtImportRows.QueueId. No devuelve claves de acceso ni payloads.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
    THROW 51150, 'SchemaHistory is required before migration 150.', 1;
IF OBJECT_ID(N'dbo.SriDocumentQueue', N'U') IS NULL
    THROW 51150, 'SriDocumentQueue is required before migration 150.', 1;
IF OBJECT_ID(N'dbo.SriAuthorizedDocuments', N'U') IS NULL
    THROW 51150, 'SriAuthorizedDocuments is required before migration 150.', 1;
IF OBJECT_ID(N'dbo.SriTxtImports', N'U') IS NULL
    THROW 51150, 'SriTxtImports is required before migration 150.', 1;
IF OBJECT_ID(N'dbo.SriTxtImportRows', N'U') IS NULL
    THROW 51150, 'SriTxtImportRows is required before migration 150.', 1;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SRIDOCUMENTMONITOR_RESUMEN
    @ImportId bigint = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @ImportId IS NOT NULL AND @ImportId <= 0
        THROW 51150, 'ImportId must be greater than zero.', 1;

    SELECT COUNT_BIG(1) AS Total,
           COALESCE(SUM(CASE WHEN q.Status=N'Pending' THEN CONVERT(bigint,1) ELSE CONVERT(bigint,0) END),CONVERT(bigint,0)) AS Pending,
           COALESCE(SUM(CASE WHEN q.Status=N'Querying' THEN CONVERT(bigint,1) ELSE CONVERT(bigint,0) END),CONVERT(bigint,0)) AS Querying,
           COALESCE(SUM(CASE WHEN q.Status=N'Authorized' THEN CONVERT(bigint,1) ELSE CONVERT(bigint,0) END),CONVERT(bigint,0)) AS Authorized,
           COALESCE(SUM(CASE WHEN q.Status IN(N'Failed',N'DeadLetter') THEN CONVERT(bigint,1) ELSE CONVERT(bigint,0) END),CONVERT(bigint,0)) AS Errors
    FROM dbo.SriDocumentQueue q
    WHERE @ImportId IS NULL
       OR EXISTS
          (
              SELECT 1
              FROM dbo.SriTxtImportRows r
              WHERE r.ImportId=@ImportId
                AND r.QueueId=q.Id
          );
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SRIDOCUMENTMONITOR_LISTAR
    @Environment nvarchar(20)=NULL,@Status nvarchar(30)=NULL,@DocumentTypeCode char(2)=NULL,
    @SourceType nvarchar(30)=NULL,@CreatedFrom datetime2(0)=NULL,@CreatedTo datetime2(0)=NULL,
    @Search nvarchar(200)=NULL,@Page int=1,@PageSize int=50,@ImportId bigint=NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @ImportId IS NOT NULL AND @ImportId <= 0
        THROW 51150, 'ImportId must be greater than zero.', 1;

    SET @Page=CASE WHEN @Page<1 THEN 1 ELSE @Page END;
    SET @PageSize=CASE WHEN @PageSize<1 THEN 50 WHEN @PageSize>200 THEN 200 ELSE @PageSize END;

    SELECT q.Id QueueId,q.Environment,q.DocumentTypeCode,q.SourceType,q.SourceReference,q.BranchCode,q.Status,
           q.AttemptCount,q.CreatedAt,d.AuthorizationAt,CAST(CASE WHEN d.Id IS NULL THEN 0 ELSE 1 END AS bit) HasXml,
           COUNT_BIG(1) OVER() TotalCount
    FROM dbo.SriDocumentQueue q
    LEFT JOIN dbo.SriAuthorizedDocuments d ON d.QueueId=q.Id
    WHERE (@ImportId IS NULL OR EXISTS
          (
              SELECT 1
              FROM dbo.SriTxtImportRows r
              WHERE r.ImportId=@ImportId
                AND r.QueueId=q.Id
          ))
      AND (@Environment IS NULL OR q.Environment=@Environment)
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

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.SchemaHistory
    WHERE Version=N'20260728.150'
)
BEGIN
    INSERT dbo.SchemaHistory(Version,Description)
    VALUES(N'20260728.150',N'Monitor SRI filtrado por importacion TXT');
END;
GO
