SET NOCOUNT ON;
SET XACT_ABORT ON;

IF OBJECT_ID(N'dbo.PurchaseOrderRoutingAudit',N'U') IS NULL
BEGIN
 CREATE TABLE dbo.PurchaseOrderRoutingAudit
 (
  Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_PurchaseOrderRoutingAudit PRIMARY KEY,
  PurchaseOrderId int NOT NULL, PreviousStatus nvarchar(30) NULL, NewStatus nvarchar(30) NOT NULL,
  BranchCompanyId int NULL, Reason nvarchar(500) NOT NULL, CreatedByUserId int NULL, CreatedByUserName nvarchar(150) NULL,
  CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_PurchaseOrderRoutingAudit_CreatedAt DEFAULT SYSUTCDATETIME(),
  CONSTRAINT FK_PurchaseOrderRoutingAudit_Order FOREIGN KEY(PurchaseOrderId) REFERENCES dbo.PurchaseOrderHeaders(Id)
 );
END;

IF OBJECT_ID(N'dbo.SchemaHistory',N'U') IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260718.103')
 INSERT dbo.SchemaHistory(Version,Description) VALUES(N'20260718.103',N'Auditoria y aplicacion de rutas de ordenes');
