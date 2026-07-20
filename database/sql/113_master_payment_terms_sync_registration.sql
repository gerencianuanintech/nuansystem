/* Activa contratos de Condiciones de Pago SAP->Matriz->Sucursal sin habilitar workers. */
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SapSyncEntitySettings',N'U') IS NULL OR OBJECT_ID(N'dbo.SyncEntityDefinitions',N'U') IS NULL
    THROW 51113, 'Falta instalar la infraestructura SAP Sync o Master-Branch Sync.', 1;
GO

INSERT dbo.SapSyncEntitySettings(CompanyId,EntityCode,Direction,IsEnabled,BatchSize,MaxRetryCount,ExecutionOrder)
SELECT c.Id,N'PaymentTerms',N'SapToErp',1,100,3,5
FROM dbo.Companies c
INNER JOIN dbo.SapCompanySettings s ON s.CompanyId=c.Id AND s.IsEnabled=1 AND s.IntegrationMode<>0
WHERE c.IsActive=1 AND c.IsMaster=1 AND c.SapIntegrationMode<>0
AND NOT EXISTS(SELECT 1 FROM dbo.SapSyncEntitySettings x WHERE x.CompanyId=c.Id AND x.EntityCode=N'PaymentTerms' AND x.Direction=N'SapToErp');
GO

UPDATE dbo.SyncEntityDefinitions
SET Description=N'Condiciones de pago con importacion SAP Full, fuente Full y aplicador idempotente por GlobalId.',
    SupportsIncremental=1,SupportsInsert=1,SupportsUpdate=1,SupportsDeactivate=1,
    DefaultExecutionOrder=50,DefaultKeyField=N'Code',DefaultModifiedAtField=N'UpdatedAt',IsActive=1,
    UpdatedByUserName=N'Script 113',UpdatedAt=SYSUTCDATETIME()
WHERE Code=N'BusinessPartnerPaymentTerms';
GO

IF OBJECT_ID(N'dbo.MasterSchemaHistory',N'U') IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260719.113')
    INSERT dbo.MasterSchemaHistory(Version,Description) VALUES(N'20260719.113',N'Activa contratos PaymentTerms SAP y Master-Branch');
GO
