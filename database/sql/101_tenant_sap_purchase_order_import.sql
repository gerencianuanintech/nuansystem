SET NOCOUNT ON;
SET XACT_ABORT ON;

IF OBJECT_ID(N'dbo.PurchaseOrderHeaders', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'dbo.PurchaseOrderHeaders', N'GlobalId') IS NULL
        ALTER TABLE dbo.PurchaseOrderHeaders ADD GlobalId uniqueidentifier NULL;
    IF COL_LENGTH(N'dbo.PurchaseOrderHeaders', N'SapUpdatedAt') IS NULL
        ALTER TABLE dbo.PurchaseOrderHeaders ADD SapUpdatedAt datetime2(0) NULL;
    IF COL_LENGTH(N'dbo.PurchaseOrderHeaders', N'SapVersion') IS NULL
        ALTER TABLE dbo.PurchaseOrderHeaders ADD SapVersion bigint NOT NULL CONSTRAINT DF_PurchaseOrderHeaders_SapVersion DEFAULT 0;
    IF COL_LENGTH(N'dbo.PurchaseOrderHeaders', N'RoutingStatus') IS NULL
        ALTER TABLE dbo.PurchaseOrderHeaders ADD RoutingStatus nvarchar(30) NOT NULL CONSTRAINT DF_PurchaseOrderHeaders_RoutingStatus DEFAULT N'Pending';
    IF COL_LENGTH(N'dbo.PurchaseOrderHeaders', N'RoutedBranchCompanyId') IS NULL
        ALTER TABLE dbo.PurchaseOrderHeaders ADD RoutedBranchCompanyId int NULL;
    IF COL_LENGTH(N'dbo.PurchaseOrderHeaders', N'RoutingDecisionAt') IS NULL
        ALTER TABLE dbo.PurchaseOrderHeaders ADD RoutingDecisionAt datetime2(0) NULL;
    IF COL_LENGTH(N'dbo.PurchaseOrderHeaders', N'RoutingDecisionBy') IS NULL
        ALTER TABLE dbo.PurchaseOrderHeaders ADD RoutingDecisionBy nvarchar(150) NULL;
    IF COL_LENGTH(N'dbo.PurchaseOrderHeaders', N'RoutingReason') IS NULL
        ALTER TABLE dbo.PurchaseOrderHeaders ADD RoutingReason nvarchar(500) NULL;
END;
GO

IF OBJECT_ID(N'dbo.PurchaseOrderHeaders', N'U') IS NOT NULL
BEGIN
    UPDATE dbo.PurchaseOrderHeaders SET GlobalId=NEWID() WHERE GlobalId IS NULL;
    IF EXISTS(SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID(N'dbo.PurchaseOrderHeaders') AND name=N'GlobalId' AND is_nullable=1)
        ALTER TABLE dbo.PurchaseOrderHeaders ALTER COLUMN GlobalId uniqueidentifier NOT NULL;
    IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.PurchaseOrderHeaders') AND name=N'UX_PurchaseOrderHeaders_GlobalId')
        CREATE UNIQUE INDEX UX_PurchaseOrderHeaders_GlobalId ON dbo.PurchaseOrderHeaders(GlobalId);
    IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.PurchaseOrderHeaders') AND name=N'UX_PurchaseOrderHeaders_SapDocEntry')
        CREATE UNIQUE INDEX UX_PurchaseOrderHeaders_SapDocEntry ON dbo.PurchaseOrderHeaders(SapDocEntry) WHERE SapDocEntry IS NOT NULL AND IsDeleted=0;
END;

IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260718.101')
    INSERT dbo.SchemaHistory(Version,Description) VALUES(N'20260718.101',N'Identidad, version y estado de enrutamiento de ordenes SAP');
