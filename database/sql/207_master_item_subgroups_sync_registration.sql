/*
    Registra ItemSubgroups para sincronizacion Matriz-Sucursal.
    Nace deshabilitado por empresa y depende de ItemFamilies. No activa
    perfiles, rutas o workers y no modifica integraciones SAP.
*/
USE [NuanSystem_Master];
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME()<>N'NuanSystem_Master' THROW 51207,'Migration 207 must run only in NuanSystem_Master.',1;
IF OBJECT_ID(N'dbo.SyncEntityDefinitions',N'U') IS NULL THROW 51207,'SyncEntityDefinitions is required.',1;
IF OBJECT_ID(N'dbo.SyncEntityDefinitionDependencies',N'U') IS NULL THROW 51207,'Sync entity dependencies are required.',1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory',N'U') IS NULL THROW 51207,'MasterSchemaHistory is required.',1;
GO

BEGIN TRY
 BEGIN TRANSACTION;
 IF EXISTS(SELECT 1 FROM dbo.SyncEntityDefinitions WHERE Code=N'ItemSubgroups')
  UPDATE dbo.SyncEntityDefinitions SET Name=N'Subgrupos de articulos',
      Description=N'Maestro dependiente de ItemFamilies con GlobalId, LocalOutbox y aplicacion sin adopcion por codigo.',
      DefaultExecutionOrder=209,SupportsIncremental=1,SupportsInsert=1,SupportsUpdate=1,SupportsDeactivate=1,
      DefaultKeyField=N'GlobalId',DefaultModifiedAtField=N'UpdatedAt',IsSystem=1,IsActive=1,IsDeleted=0,
      UpdatedAt=SYSUTCDATETIME(),UpdatedByUserName=N'Sistema'
  WHERE Code=N'ItemSubgroups';
 ELSE
  INSERT dbo.SyncEntityDefinitions(Code,Name,Description,DefaultExecutionOrder,SupportsIncremental,SupportsInsert,SupportsUpdate,SupportsDeactivate,
      DefaultKeyField,DefaultModifiedAtField,IsSystem,IsActive,CreatedByUserName)
  VALUES(N'ItemSubgroups',N'Subgrupos de articulos',N'Maestro dependiente de ItemFamilies con GlobalId, LocalOutbox y aplicacion sin adopcion por codigo.',
      209,1,1,1,1,N'GlobalId',N'UpdatedAt',1,1,N'Sistema');

 DECLARE @ItemSubgroupsId int=(SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code=N'ItemSubgroups' AND IsDeleted=0);
 DECLARE @ItemFamiliesId int=(SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code=N'ItemFamilies' AND IsDeleted=0);
 IF @ItemSubgroupsId IS NULL OR @ItemFamiliesId IS NULL THROW 51207,'ItemSubgroups and ItemFamilies definitions are required.',1;

 UPDATE dbo.SyncEntityDefinitionDependencies SET IsDeleted=1,DeletedAt=COALESCE(DeletedAt,SYSUTCDATETIME()),
     DeletedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME(),UpdatedByUserName=N'Sistema'
 WHERE EntityDefinitionId=@ItemSubgroupsId AND DependsOnEntityDefinitionId<>@ItemFamiliesId AND IsDeleted=0;
 IF EXISTS(SELECT 1 FROM dbo.SyncEntityDefinitionDependencies WHERE EntityDefinitionId=@ItemSubgroupsId AND DependsOnEntityDefinitionId=@ItemFamiliesId)
  UPDATE dbo.SyncEntityDefinitionDependencies SET IsDeleted=0,DeletedAt=NULL,DeletedByUserId=NULL,DeletedByUserName=NULL,
      UpdatedAt=SYSUTCDATETIME(),UpdatedByUserName=N'Sistema'
  WHERE EntityDefinitionId=@ItemSubgroupsId AND DependsOnEntityDefinitionId=@ItemFamiliesId;
 ELSE
  INSERT dbo.SyncEntityDefinitionDependencies
      (EntityDefinitionId,DependsOnEntityDefinitionId,CreatedByUserName,CreatedAt)
  VALUES(@ItemSubgroupsId,@ItemFamiliesId,N'Sistema',SYSUTCDATETIME());

 IF OBJECT_ID(N'dbo.SyncEntityConfigurations',N'U') IS NOT NULL
  INSERT dbo.SyncEntityConfigurations(CompanyId,EntityName,IsEnabled,Direction,ConflictPolicy,BatchSize,MaxAttempts)
  SELECT company.Id,N'ItemSubgroups',0,N'MasterToBranch',N'MasterWins',100,3 FROM dbo.Companies company
  WHERE company.IsMaster=1 AND NOT EXISTS(SELECT 1 FROM dbo.SyncEntityConfigurations c WHERE c.CompanyId=company.Id AND c.EntityName=N'ItemSubgroups');
 IF OBJECT_ID(N'dbo.EntityOwnershipConfigurations',N'U') IS NOT NULL
  INSERT dbo.EntityOwnershipConfigurations(CompanyId,EntityName,SourceOfTruth,SyncDirection,IsEnabled)
  SELECT company.Id,N'ItemSubgroups',0,4,0 FROM dbo.Companies company
  WHERE company.IsMaster=1 AND NOT EXISTS(SELECT 1 FROM dbo.EntityOwnershipConfigurations o WHERE o.CompanyId=company.Id AND o.EntityName=N'ItemSubgroups');

 IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260813.207')
  INSERT dbo.MasterSchemaHistory(Version,Description)
  VALUES(N'20260813.207',N'Registra ItemSubgroups deshabilitado por defecto y dependiente de ItemFamilies');
 COMMIT;
END TRY
BEGIN CATCH
 IF XACT_STATE()<>0 ROLLBACK;
 THROW;
END CATCH;
GO
