SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

DECLARE @ProfileId int=(SELECT Id FROM dbo.SyncProfiles WHERE CompanyId=1 AND Code=N'DEMO-ITEMS-PILOT' AND IsDeleted=0);
IF @ProfileId IS NULL THROW 51104,'No existe perfil DEMO-ITEMS-PILOT.',1;

UPDATE dbo.SyncProfileBranches
SET IsActive=0,UpdatedByUserName=N'Fase9Pilot',UpdatedAt=SYSUTCDATETIME()
WHERE SyncProfileId=@ProfileId AND BranchCompanyId NOT IN(1002,1003) AND IsDeleted=0;

DECLARE @Entities TABLE(EntityCode nvarchar(100),EntityName nvarchar(200),ExecutionOrder int,SyncMode nvarchar(20),BatchSize int);
INSERT @Entities VALUES
(N'Currencies',N'Monedas',40,N'Full',100),(N'Tax',N'Impuestos',45,N'Full',100),(N'UnitOfMeasure',N'Unidades de medida',50,N'Full',100),
(N'BusinessPartner',N'Proveedores',200,N'Full',100),(N'Warehouse',N'Bodegas',220,N'Full',100),(N'PriceList',N'Listas de precios',230,N'Full',100),
(N'PurchaseOrder',N'Ordenes de compra',300,N'Incremental',50);

MERGE dbo.SyncProfileEntities target USING(SELECT @ProfileId SyncProfileId,* FROM @Entities) source
ON target.SyncProfileId=source.SyncProfileId AND target.EntityCode=source.EntityCode AND target.IsDeleted=0
WHEN MATCHED THEN UPDATE SET EntityName=source.EntityName,ExecutionOrder=source.ExecutionOrder,SyncMode=source.SyncMode,BatchSize=source.BatchSize,
 IsActive=1,AllowInsert=1,AllowUpdate=1,AllowDeactivate=1,UpdatedByUserName=N'Fase9Pilot',UpdatedAt=SYSUTCDATETIME()
WHEN NOT MATCHED THEN INSERT(SyncProfileId,EntityCode,EntityName,ExecutionOrder,SyncMode,KeyField,ModifiedAtField,ActiveField,AllowInsert,AllowUpdate,
 AllowDeactivate,ContinueOnError,BatchSize,IsActive,CreatedByUserName,CreatedAt,IsDeleted)
 VALUES(source.SyncProfileId,source.EntityCode,source.EntityName,source.ExecutionOrder,source.SyncMode,N'Code',N'UpdatedAt',N'IsActive',1,1,1,0,source.BatchSize,1,N'Fase9Pilot',SYSUTCDATETIME(),0);

MERGE dbo.SyncProfileEntityBranches target USING
(
 SELECT @ProfileId SyncProfileId,e.Id SyncProfileEntityId,b.Id SyncProfileBranchId,e.BatchSize
 FROM dbo.SyncProfileEntities e CROSS JOIN dbo.SyncProfileBranches b
 WHERE e.SyncProfileId=@ProfileId AND b.SyncProfileId=@ProfileId AND e.EntityCode IN(SELECT EntityCode FROM @Entities)
 AND e.IsDeleted=0 AND b.IsDeleted=0 AND b.BranchCompanyId IN(1002,1003)
) source
ON target.SyncProfileId=source.SyncProfileId AND target.SyncProfileEntityId=source.SyncProfileEntityId AND target.SyncProfileBranchId=source.SyncProfileBranchId AND target.IsDeleted=0
WHEN MATCHED THEN UPDATE SET IsEnabled=1,BatchSize=source.BatchSize,DistributionMode=N'All',UpdatedByUserName=N'Fase9Pilot',UpdatedAt=SYSUTCDATETIME()
WHEN NOT MATCHED THEN INSERT(SyncProfileId,SyncProfileEntityId,SyncProfileBranchId,IsEnabled,BatchSize,DistributionMode,OnNoMatch,RuleVersion,CreatedByUserName,CreatedAt,IsDeleted)
 VALUES(source.SyncProfileId,source.SyncProfileEntityId,source.SyncProfileBranchId,1,source.BatchSize,N'All',N'KeepInMaster',1,N'Fase9Pilot',SYSUTCDATETIME(),0);

UPDATE matrix SET DistributionMode=N'All',UpdatedByUserName=N'Fase9Pilot',UpdatedAt=SYSUTCDATETIME()
FROM dbo.SyncProfileEntityBranches matrix INNER JOIN dbo.SyncProfileEntities entity ON entity.Id=matrix.SyncProfileEntityId
WHERE matrix.SyncProfileId=@ProfileId AND entity.EntityCode IN(N'ItemGroups',N'Item') AND matrix.IsDeleted=0;

UPDATE matrix SET DistributionMode=N'Selected',UpdatedByUserName=N'Fase9Pilot',UpdatedAt=SYSUTCDATETIME()
FROM dbo.SyncProfileEntityBranches matrix INNER JOIN dbo.SyncProfileEntities entity ON entity.Id=matrix.SyncProfileEntityId
WHERE matrix.SyncProfileId=@ProfileId AND entity.EntityCode=N'Warehouse' AND matrix.IsDeleted=0;

MERGE dbo.SyncDistributionSelections target USING
(
 SELECT matrix.Id SyncProfileEntityBranchId,warehouse.GlobalId EntityGlobalId
 FROM dbo.SyncProfileEntityBranches matrix
 INNER JOIN dbo.SyncProfileEntities entity ON entity.Id=matrix.SyncProfileEntityId AND entity.EntityCode=N'Warehouse'
 INNER JOIN dbo.SyncProfileBranches branch ON branch.Id=matrix.SyncProfileBranchId
 INNER JOIN NuanSystem_DEMO.dbo.Warehouses warehouse ON warehouse.Code=CASE branch.BranchCompanyId WHEN 1002 THEN N'20' WHEN 1003 THEN N'11' END
 WHERE matrix.SyncProfileId=@ProfileId AND matrix.IsDeleted=0 AND branch.BranchCompanyId IN(1002,1003)
) source ON target.SyncProfileEntityBranchId=source.SyncProfileEntityBranchId AND target.EntityGlobalId=source.EntityGlobalId AND target.IsDeleted=0
WHEN NOT MATCHED THEN INSERT(SyncProfileEntityBranchId,EntityGlobalId,CreatedByUserName,CreatedAt,IsDeleted)
 VALUES(source.SyncProfileEntityBranchId,source.EntityGlobalId,N'Fase9Pilot',SYSUTCDATETIME(),0);
GO

IF OBJECT_ID(N'dbo.SchemaHistory',N'U') IS NOT NULL
 EXEC(N'IF NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N''20260718.104'') INSERT dbo.SchemaHistory(Version,Description) VALUES(N''20260718.104'',N''Perfil piloto dependencias y ordenes de compra'');');
GO
