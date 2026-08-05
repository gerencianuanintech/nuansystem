/* Registers Provinces as SAP -> ERP Full capability. Master only; creates no profile or schedule. */
USE [NuanSystem_Master];
GO
SET NOCOUNT ON; SET XACT_ABORT ON;
GO
IF DB_NAME()<>N'NuanSystem_Master' THROW 51174,'Migration 174 must run only in NuanSystem_Master.',1;
IF OBJECT_ID(N'dbo.SapSyncHandlerCapabilities',N'U') IS NULL OR OBJECT_ID(N'dbo.MasterSchemaHistory',N'U') IS NULL THROW 51174,'Master SAP capability foundation is required before 174.',1;
GO
BEGIN TRANSACTION;
UPDATE dbo.SapSyncHandlerCapabilities SET DisplayName=N'Provincias',SupportsSapToErp=1,SupportsErpToSap=0,SupportsFull=1,SupportsIncremental=0,IsImplemented=1,IsActive=1,UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME() WHERE EntityCode=N'Provinces';
IF NOT EXISTS(SELECT 1 FROM dbo.SapSyncHandlerCapabilities WHERE EntityCode=N'Provinces')
 INSERT dbo.SapSyncHandlerCapabilities(EntityCode,DisplayName,SupportsSapToErp,SupportsErpToSap,SupportsFull,SupportsIncremental,IsImplemented,IsActive,CreatedByUserName)
 VALUES(N'Provinces',N'Provincias',1,0,1,0,1,1,N'Sistema');
IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260804.174') INSERT dbo.MasterSchemaHistory(Version,Description) VALUES(N'20260804.174',N'Registra Provincias para sincronizacion SAP a ERP Full sin filtros');
COMMIT;
GO
