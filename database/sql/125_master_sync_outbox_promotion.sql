/*
    Iteracion 8.1 - protecciones Master para promocion LocalOutbox.
    La transaccion atomica se ejecuta desde Persistence sobre un unico SqlTransaction.
*/
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id=OBJECT_ID(N'dbo.SyncOutbox')
      AND name=N'IX_SyncOutbox_SourceReference'
)
BEGIN
    CREATE INDEX IX_SyncOutbox_SourceReference
    ON dbo.SyncOutbox(CompanyId,SourceSystem,SourceReference)
    WHERE SourceReference IS NOT NULL;
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM dbo.MasterSchemaHistory
    WHERE Version=N'20260725.125'
)
BEGIN
    INSERT dbo.MasterSchemaHistory(Version,Description)
    VALUES(N'20260725.125',N'Iteracion 8.1: protecciones para promocion atomica de LocalOutbox');
END;
GO
