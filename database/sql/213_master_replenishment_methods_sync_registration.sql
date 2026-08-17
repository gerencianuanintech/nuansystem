/* Registra ReplenishmentMethod independiente; no activa perfiles, rutas ni workers. */
USE [NuanSystem_Master];
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO
SET NOCOUNT ON; SET XACT_ABORT ON;
IF DB_NAME()<>N'NuanSystem_Master' THROW 51213,'Migration 213 must run only in NuanSystem_Master.',1;
IF OBJECT_ID(N'dbo.SyncEntityDefinitions',N'U') IS NULL OR OBJECT_ID(N'dbo.MasterSchemaHistory',N'U') IS NULL THROW 51213,'Sync definitions and MasterSchemaHistory are required.',1;
GO
BEGIN TRY
 BEGIN TRANSACTION;
 IF EXISTS(SELECT 1 FROM dbo.SyncEntityDefinitions WHERE Code=N'ReplenishmentMethod') UPDATE dbo.SyncEntityDefinitions SET Name=N'Metodos de reposicion',Description=N'Maestro independiente con GlobalId, LocalOutbox y aplicacion sin adopcion por codigo.',DefaultExecutionOrder=209,SupportsIncremental=1,SupportsInsert=1,SupportsUpdate=1,SupportsDeactivate=1,DefaultKeyField=N'GlobalId',DefaultModifiedAtField=N'UpdatedAt',IsSystem=1,IsActive=1,IsDeleted=0,UpdatedAt=SYSUTCDATETIME(),UpdatedByUserName=N'Sistema' WHERE Code=N'ReplenishmentMethod';
 ELSE INSERT dbo.SyncEntityDefinitions(Code,Name,Description,DefaultExecutionOrder,SupportsIncremental,SupportsInsert,SupportsUpdate,SupportsDeactivate,DefaultKeyField,DefaultModifiedAtField,IsSystem,IsActive,CreatedByUserName) VALUES(N'ReplenishmentMethod',N'Metodos de reposicion',N'Maestro independiente con GlobalId, LocalOutbox y aplicacion sin adopcion por codigo.',209,1,1,1,1,N'GlobalId',N'UpdatedAt',1,1,N'Sistema');
 IF OBJECT_ID(N'dbo.SyncEntityDefinitionDependencies',N'U') IS NOT NULL BEGIN DECLARE @EntityId int=(SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code=N'ReplenishmentMethod' AND IsDeleted=0); UPDATE dbo.SyncEntityDefinitionDependencies SET IsDeleted=1,DeletedAt=COALESCE(DeletedAt,SYSUTCDATETIME()),DeletedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME(),UpdatedByUserName=N'Sistema' WHERE EntityDefinitionId=@EntityId AND IsDeleted=0; END;
 IF OBJECT_ID(N'dbo.SyncEntityConfigurations',N'U') IS NOT NULL INSERT dbo.SyncEntityConfigurations(CompanyId,EntityName,IsEnabled,Direction,ConflictPolicy,BatchSize,MaxAttempts) SELECT company.Id,N'ReplenishmentMethod',0,N'MasterToBranch',N'MasterWins',100,3 FROM dbo.Companies company WHERE company.IsMaster=1 AND NOT EXISTS(SELECT 1 FROM dbo.SyncEntityConfigurations c WHERE c.CompanyId=company.Id AND c.EntityName=N'ReplenishmentMethod');
 IF OBJECT_ID(N'dbo.EntityOwnershipConfigurations',N'U') IS NOT NULL INSERT dbo.EntityOwnershipConfigurations(CompanyId,EntityName,SourceOfTruth,SyncDirection,IsEnabled) SELECT company.Id,N'ReplenishmentMethod',0,4,0 FROM dbo.Companies company WHERE company.IsMaster=1 AND NOT EXISTS(SELECT 1 FROM dbo.EntityOwnershipConfigurations o WHERE o.CompanyId=company.Id AND o.EntityName=N'ReplenishmentMethod');
 IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260813.213') INSERT dbo.MasterSchemaHistory(Version,Description) VALUES(N'20260813.213',N'Registra ReplenishmentMethod independiente y deshabilitado por defecto');
 COMMIT;
END TRY BEGIN CATCH IF XACT_STATE()<>0 ROLLBACK; THROW; END CATCH;
GO
