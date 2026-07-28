/*
    Migracion 151 - Reparacion del contrato bigint del resumen del Monitor SRI.

    La migracion 150 agrego el alcance opcional por ImportId, pero recreo las
    sumas condicionales como int. Este forward repair conserva el filtro por
    carga y devuelve los cinco agregados como bigint para Dapper.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
    THROW 51151, 'SchemaHistory is required before migration 151.', 1;
IF OBJECT_ID(N'dbo.SriDocumentQueue', N'U') IS NULL
    THROW 51151, 'SriDocumentQueue is required before migration 151.', 1;
IF OBJECT_ID(N'dbo.SriTxtImportRows', N'U') IS NULL
    THROW 51151, 'SriTxtImportRows is required before migration 151.', 1;
IF NOT EXISTS
(
    SELECT 1
    FROM dbo.SchemaHistory
    WHERE Version=N'20260728.150'
)
    THROW 51151, 'Migration 150 is required before migration 151.', 1;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SRIDOCUMENTMONITOR_RESUMEN
    @ImportId bigint = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @ImportId IS NOT NULL AND @ImportId <= 0
        THROW 51151, 'ImportId must be greater than zero.', 1;

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

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.SchemaHistory
    WHERE Version=N'20260728.151'
)
BEGIN
    INSERT dbo.SchemaHistory(Version,Description)
    VALUES(N'20260728.151',N'Repara contrato bigint del resumen SRI con alcance por importacion TXT');
END;
GO
