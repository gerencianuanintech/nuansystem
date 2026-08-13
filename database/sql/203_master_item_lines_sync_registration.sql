/*
    Registra ItemLine para sincronizacion Matriz-Sucursal.

    Solo NuanSystem_Master. La entidad nace deshabilitada y sin dependencias:
    Items conserva Line como texto y todavia no publica ItemLineGlobalId.
    No activa perfiles, rutas ni workers y no integra SAP.
*/
USE [NuanSystem_Master];
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME()<>N'NuanSystem_Master' THROW 51203,'Migration 203 must run only in NuanSystem_Master.',1;
IF OBJECT_ID(N'dbo.SyncEntityDefinitions',N'U') IS NULL THROW 51203,'SyncEntityDefinitions is required.',1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory',N'U') IS NULL THROW 51203,'MasterSchemaHistory is required.',1;
GO

BEGIN TRY
 BEGIN TRANSACTION;
 IF EXISTS(SELECT 1 FROM dbo.SyncEntityDefinitions WHERE Code=N'ItemLine')
  UPDATE dbo.SyncEntityDefinitions SET Name=N'Lineas de articulos',Description=N'Maestro independiente con GlobalId, LocalOutbox y aplicacion sin adopcion por codigo.',
      DefaultExecutionOrder=209,SupportsIncremental=1,SupportsInsert=1,SupportsUpdate=1,SupportsDeactivate=1,
      DefaultKeyField=N'GlobalId',DefaultModifiedAtField=N'UpdatedAt',IsSystem=1,IsActive=1,IsDeleted=0,UpdatedAt=SYSUTCDATETIME(),UpdatedByUserName=N'Sistema'
  WHERE Code=N'ItemLine';
 ELSE
  INSERT dbo.SyncEntityDefinitions(Code,Name,Description,DefaultExecutionOrder,SupportsIncremental,SupportsInsert,SupportsUpdate,SupportsDeactivate,
      DefaultKeyField,DefaultModifiedAtField,IsSystem,IsActive,CreatedByUserName)
  VALUES(N'ItemLine',N'Lineas de articulos',N'Maestro independiente con GlobalId, LocalOutbox y aplicacion sin adopcion por codigo.',209,1,1,1,1,N'GlobalId',N'UpdatedAt',1,1,N'Sistema');

 /* ItemLine no depende de otra entidad. Conserva como historial cualquier
    dependencia anterior, pero no inventa Item -> ItemLine mientras Item use texto. */
 IF OBJECT_ID(N'dbo.SyncEntityDefinitionDependencies',N'U') IS NOT NULL
 BEGIN
  DECLARE @ItemLineId int=(SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code=N'ItemLine' AND IsDeleted=0);
  IF @ItemLineId IS NULL THROW 51203,'ItemLine definition is required.',1;
  UPDATE dbo.SyncEntityDefinitionDependencies SET IsDeleted=1,DeletedAt=COALESCE(DeletedAt,SYSUTCDATETIME()),DeletedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME(),UpdatedByUserName=N'Sistema'
  WHERE EntityDefinitionId=@ItemLineId AND IsDeleted=0;
 END;

 IF OBJECT_ID(N'dbo.SyncEntityConfigurations',N'U') IS NOT NULL
  INSERT dbo.SyncEntityConfigurations(CompanyId,EntityName,IsEnabled,Direction,ConflictPolicy,BatchSize,MaxAttempts)
  SELECT company.Id,N'ItemLine',0,N'MasterToBranch',N'MasterWins',100,3 FROM dbo.Companies company
  WHERE company.IsMaster=1 AND NOT EXISTS(SELECT 1 FROM dbo.SyncEntityConfigurations c WHERE c.CompanyId=company.Id AND c.EntityName=N'ItemLine');
 IF OBJECT_ID(N'dbo.EntityOwnershipConfigurations',N'U') IS NOT NULL
  INSERT dbo.EntityOwnershipConfigurations(CompanyId,EntityName,SourceOfTruth,SyncDirection,IsEnabled)
  SELECT company.Id,N'ItemLine',0,4,0 FROM dbo.Companies company
  WHERE company.IsMaster=1 AND NOT EXISTS(SELECT 1 FROM dbo.EntityOwnershipConfigurations o WHERE o.CompanyId=company.Id AND o.EntityName=N'ItemLine');

 IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260813.203')
  INSERT dbo.MasterSchemaHistory(Version,Description) VALUES(N'20260813.203',N'Registra ItemLine Matriz-Sucursal deshabilitado por defecto y sin dependencia Item');
 COMMIT;
END TRY
BEGIN CATCH
 IF XACT_STATE()<>0 ROLLBACK;
 THROW;
END CATCH;
GO
