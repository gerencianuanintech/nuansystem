SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.PurchaseOrderWarehouseRoutes',N'U') IS NULL
BEGIN
 CREATE TABLE dbo.PurchaseOrderWarehouseRoutes
 (
  Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PurchaseOrderWarehouseRoutes PRIMARY KEY,
  SourceCompanyId int NOT NULL, WarehouseCode nvarchar(50) NOT NULL, BranchCompanyId int NOT NULL,
  IsActive bit NOT NULL CONSTRAINT DF_PurchaseOrderWarehouseRoutes_IsActive DEFAULT 1,
  CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_PurchaseOrderWarehouseRoutes_CreatedAt DEFAULT SYSUTCDATETIME(),
  CreatedBy nvarchar(150) NULL,
  CONSTRAINT FK_PurchaseOrderWarehouseRoutes_Source FOREIGN KEY(SourceCompanyId) REFERENCES dbo.Companies(Id),
  CONSTRAINT FK_PurchaseOrderWarehouseRoutes_Branch FOREIGN KEY(BranchCompanyId) REFERENCES dbo.Companies(Id),
  CONSTRAINT UQ_PurchaseOrderWarehouseRoutes UNIQUE(SourceCompanyId,WarehouseCode)
 );
END;
GO

IF EXISTS(SELECT 1 FROM dbo.Companies WHERE Id=1 AND Code=N'DEMO')
AND EXISTS(SELECT 1 FROM dbo.Companies WHERE Id=1002 AND Code=N'DEMO-REMIGIO')
AND NOT EXISTS(SELECT 1 FROM dbo.PurchaseOrderWarehouseRoutes WHERE SourceCompanyId=1 AND WarehouseCode=N'20')
 INSERT dbo.PurchaseOrderWarehouseRoutes(SourceCompanyId,WarehouseCode,BranchCompanyId,CreatedBy) VALUES(1,N'20',1002,N'Fase7Pilot');
IF EXISTS(SELECT 1 FROM dbo.Companies WHERE Id=1 AND Code=N'DEMO')
AND EXISTS(SELECT 1 FROM dbo.Companies WHERE Id=1003 AND Code=N'DEMO-CANARIS')
AND NOT EXISTS(SELECT 1 FROM dbo.PurchaseOrderWarehouseRoutes WHERE SourceCompanyId=1 AND WarehouseCode=N'11')
 INSERT dbo.PurchaseOrderWarehouseRoutes(SourceCompanyId,WarehouseCode,BranchCompanyId,CreatedBy) VALUES(1,N'11',1003,N'Fase7Pilot');
GO

IF OBJECT_ID(N'dbo.SchemaHistory',N'U') IS NOT NULL
 EXEC(N'IF NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N''20260718.102'')
 INSERT dbo.SchemaHistory(Version,Description) VALUES(N''20260718.102'',N''Rutas Master de ordenes por bodega'');');
GO

-- Ejecutar el siguiente bloque en cada tenant que reciba ordenes.
-- La tabla se crea tambien desde 103_tenant_purchase_order_sync.sql para instalaciones automatizadas.
