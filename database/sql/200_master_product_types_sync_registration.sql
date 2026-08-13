/*
    Registra ProductType para sincronizacion Matriz-Sucursal.

    Solo NuanSystem_Master. ProductType se ejecuta antes de Item. Las nuevas
    configuraciones y ownership nacen deshabilitados. No activa perfiles,
    rutas ni workers y no integra SAP.
*/
USE [NuanSystem_Master];
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME()<>N'NuanSystem_Master' THROW 51200,'Migration 200 must run only in NuanSystem_Master.',1;
IF OBJECT_ID(N'dbo.SyncEntityDefinitions',N'U') IS NULL THROW 51200,'SyncEntityDefinitions is required.',1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory',N'U') IS NULL THROW 51200,'MasterSchemaHistory is required.',1;
GO

BEGIN TRY
 BEGIN TRANSACTION;
 IF EXISTS(SELECT 1 FROM dbo.SyncEntityDefinitions WHERE Code=N'ProductType')
  UPDATE dbo.SyncEntityDefinitions SET Name=N'Tipos de producto',Description=N'Maestro con naturaleza ERP, GlobalId, LocalOutbox y aplicacion sin adopcion por codigo.',
      DefaultExecutionOrder=55,SupportsIncremental=1,SupportsInsert=1,SupportsUpdate=1,SupportsDeactivate=1,
      DefaultKeyField=N'GlobalId',DefaultModifiedAtField=N'UpdatedAt',IsSystem=1,IsActive=1,IsDeleted=0,UpdatedAt=SYSUTCDATETIME(),UpdatedByUserName=N'Sistema'
  WHERE Code=N'ProductType';
 ELSE
  INSERT dbo.SyncEntityDefinitions(Code,Name,Description,DefaultExecutionOrder,SupportsIncremental,SupportsInsert,SupportsUpdate,SupportsDeactivate,
      DefaultKeyField,DefaultModifiedAtField,IsSystem,IsActive,CreatedByUserName)
  VALUES(N'ProductType',N'Tipos de producto',N'Maestro con naturaleza ERP, GlobalId, LocalOutbox y aplicacion sin adopcion por codigo.',55,1,1,1,1,N'GlobalId',N'UpdatedAt',1,1,N'Sistema');

 IF OBJECT_ID(N'dbo.SyncEntityDefinitionDependencies',N'U') IS NOT NULL
 BEGIN
  DECLARE @ProductTypeId int=(SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code=N'ProductType' AND IsDeleted=0);
  DECLARE @ItemId int=(SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code=N'Item' AND IsDeleted=0);
  IF @ProductTypeId IS NULL THROW 51200,'ProductType definition is required.',1;
  UPDATE dbo.SyncEntityDefinitionDependencies SET IsDeleted=1,DeletedAt=COALESCE(DeletedAt,SYSUTCDATETIME()),DeletedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME(),UpdatedByUserName=N'Sistema'
  WHERE EntityDefinitionId=@ProductTypeId AND IsDeleted=0;
  IF @ItemId IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.SyncEntityDefinitionDependencies WHERE EntityDefinitionId=@ItemId AND DependsOnEntityDefinitionId=@ProductTypeId AND IsDeleted=0)
  BEGIN
   IF EXISTS(SELECT 1 FROM dbo.SyncEntityDefinitionDependencies WHERE EntityDefinitionId=@ItemId AND DependsOnEntityDefinitionId=@ProductTypeId)
    UPDATE dbo.SyncEntityDefinitionDependencies SET IsDeleted=0,DeletedAt=NULL,DeletedByUserId=NULL,DeletedByUserName=NULL,UpdatedAt=SYSUTCDATETIME(),UpdatedByUserName=N'Sistema'
    WHERE Id=(SELECT TOP(1) Id FROM dbo.SyncEntityDefinitionDependencies WHERE EntityDefinitionId=@ItemId AND DependsOnEntityDefinitionId=@ProductTypeId ORDER BY Id DESC);
   ELSE INSERT dbo.SyncEntityDefinitionDependencies(EntityDefinitionId,DependsOnEntityDefinitionId,CreatedByUserName,CreatedAt) VALUES(@ItemId,@ProductTypeId,N'Sistema',SYSUTCDATETIME());
  END;
 END;

 IF OBJECT_ID(N'dbo.SyncEntityConfigurations',N'U') IS NOT NULL
  INSERT dbo.SyncEntityConfigurations(CompanyId,EntityName,IsEnabled,Direction,ConflictPolicy,BatchSize,MaxAttempts)
  SELECT company.Id,N'ProductType',0,N'MasterToBranch',N'MasterWins',100,3 FROM dbo.Companies company
  WHERE company.IsMaster=1 AND NOT EXISTS(SELECT 1 FROM dbo.SyncEntityConfigurations c WHERE c.CompanyId=company.Id AND c.EntityName=N'ProductType');
 IF OBJECT_ID(N'dbo.EntityOwnershipConfigurations',N'U') IS NOT NULL
  INSERT dbo.EntityOwnershipConfigurations(CompanyId,EntityName,SourceOfTruth,SyncDirection,IsEnabled)
  SELECT company.Id,N'ProductType',0,4,0 FROM dbo.Companies company
  WHERE company.IsMaster=1 AND NOT EXISTS(SELECT 1 FROM dbo.EntityOwnershipConfigurations o WHERE o.CompanyId=company.Id AND o.EntityName=N'ProductType');

 IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260812.200')
  INSERT dbo.MasterSchemaHistory(Version,Description) VALUES(N'20260812.200',N'Registra ProductType Matriz-Sucursal deshabilitado por defecto y antes de Item');
 COMMIT;
END TRY
BEGIN CATCH
 IF XACT_STATE()<>0 ROLLBACK;
 THROW;
END CATCH;
GO
