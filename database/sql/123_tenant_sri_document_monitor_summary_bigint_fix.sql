/* Forward repair: alinea el resumen del Monitor SRI con el contrato Dapper de bigint. Tenant only; prerequisite 118. */
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SriDocumentQueue', N'U') IS NULL
    THROW 50001, 'Prerequisite 118 is required: dbo.SriDocumentQueue was not found.', 1;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SRIDOCUMENTMONITOR_RESUMEN
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        COUNT_BIG(1) AS Total,
        COALESCE(SUM(CASE WHEN q.Status=N'Pending' THEN CONVERT(bigint,1) ELSE CONVERT(bigint,0) END),CONVERT(bigint,0)) AS Pending,
        COALESCE(SUM(CASE WHEN q.Status=N'Querying' THEN CONVERT(bigint,1) ELSE CONVERT(bigint,0) END),CONVERT(bigint,0)) AS Querying,
        COALESCE(SUM(CASE WHEN q.Status=N'Authorized' THEN CONVERT(bigint,1) ELSE CONVERT(bigint,0) END),CONVERT(bigint,0)) AS Authorized,
        COALESCE(SUM(CASE WHEN q.Status IN(N'Failed',N'DeadLetter') THEN CONVERT(bigint,1) ELSE CONVERT(bigint,0) END),CONVERT(bigint,0)) AS Errors
    FROM dbo.SriDocumentQueue q;
END;
GO

IF OBJECT_ID(N'dbo.SchemaHistory',N'U') IS NOT NULL
   AND NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260725.123')
    INSERT dbo.SchemaHistory(Version,Description)
    VALUES(N'20260725.123',N'Corrige contrato bigint del resumen del Monitor SRI');
GO
