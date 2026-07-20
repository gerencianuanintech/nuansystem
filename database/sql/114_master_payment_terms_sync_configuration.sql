/*
    Completa el registro Master de BusinessPartnerPaymentTerms omitido por 113.
    No activa la entidad, perfiles, sucursales ni workers automáticamente.
*/
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME() <> N'NuanSystem_Master'
    THROW 51114, 'Este script debe ejecutarse en NuanSystem_Master.', 1;
GO

IF OBJECT_ID(N'dbo.SyncEntityConfigurations', N'U') IS NULL
    THROW 51114, 'No existe SyncEntityConfigurations. Ejecute primero 064.', 1;
GO

INSERT INTO dbo.SyncEntityConfigurations
(
    CompanyId, EntityName, IsEnabled, Direction, ConflictPolicy, BatchSize, MaxAttempts
)
SELECT
    company.Id,
    N'BusinessPartnerPaymentTerms',
    CONVERT(bit, 0),
    N'MasterToBranch',
    N'MasterWins',
    100,
    3
FROM dbo.Companies AS company
WHERE company.IsMaster = 1
  AND company.IsDeleted = 0
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.SyncEntityConfigurations AS existing
      WHERE existing.CompanyId = company.Id
        AND existing.EntityName = N'BusinessPartnerPaymentTerms'
  );
GO

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260720.114')
BEGIN
    INSERT dbo.MasterSchemaHistory(Version, Description)
    VALUES(N'20260720.114', N'Completa configuracion Master de BusinessPartnerPaymentTerms');
END;
GO
