/*
    Alinea Warehouse en Master sin habilitar configuraciones existentes ni nuevas.
    Ejecutar solo en NuanSystem_Master.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO
IF OBJECT_ID(N'dbo.SyncEntityDefinitions',N'U') IS NULL
    THROW 51134,'SyncEntityDefinitions is required before migration 134.',1;
GO
IF EXISTS(SELECT 1 FROM dbo.SyncEntityDefinitions WHERE Code=N'Warehouse')
    UPDATE dbo.SyncEntityDefinitions
    SET Name=N'Bodegas',Description=N'Contrato corporativo minimo con LocalOutbox transaccional y conflicto terminal por codigo.',
        DefaultExecutionOrder=220,SupportsIncremental=1,SupportsInsert=1,SupportsUpdate=1,
        SupportsDeactivate=1,DefaultKeyField=N'Code',DefaultModifiedAtField=N'UpdatedAt',
        IsSystem=1,IsActive=1,IsDeleted=0,UpdatedAt=SYSUTCDATETIME(),UpdatedByUserName=N'Sistema'
    WHERE Code=N'Warehouse';
ELSE
    INSERT dbo.SyncEntityDefinitions
    (Code,Name,Description,DefaultExecutionOrder,SupportsIncremental,SupportsInsert,SupportsUpdate,
     SupportsDeactivate,DefaultKeyField,DefaultModifiedAtField,IsSystem,IsActive,CreatedByUserName)
    VALUES(N'Warehouse',N'Bodegas',N'Contrato corporativo minimo con LocalOutbox transaccional y conflicto terminal por codigo.',
           220,1,1,1,1,N'Code',N'UpdatedAt',1,1,N'Sistema');
GO
IF OBJECT_ID(N'dbo.SyncEntityConfigurations',N'U') IS NOT NULL
    INSERT dbo.SyncEntityConfigurations(CompanyId,EntityName,IsEnabled,Direction,ConflictPolicy,BatchSize,MaxAttempts)
    SELECT Id,N'Warehouse',0,N'MasterToBranch',N'MasterWins',100,3
    FROM dbo.Companies c WHERE c.IsMaster=1
      AND NOT EXISTS(SELECT 1 FROM dbo.SyncEntityConfigurations x WHERE x.CompanyId=c.Id AND x.EntityName=N'Warehouse');
GO
IF OBJECT_ID(N'dbo.EntityOwnershipConfigurations',N'U') IS NOT NULL
    INSERT dbo.EntityOwnershipConfigurations(CompanyId,EntityName,SourceOfTruth,SyncDirection,IsEnabled)
    SELECT Id,N'Warehouse',0,4,0 FROM dbo.Companies c WHERE c.IsMaster=1
      AND NOT EXISTS(SELECT 1 FROM dbo.EntityOwnershipConfigurations x WHERE x.CompanyId=c.Id AND x.EntityName=N'Warehouse');
GO
IF OBJECT_ID(N'dbo.MasterSchemaHistory',N'U') IS NULL
    THROW 51134,'MasterSchemaHistory is required before recording migration 134.',1;
IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260726.134')
    INSERT dbo.MasterSchemaHistory(Version,Description)
    VALUES(N'20260726.134',N'Registra contrato transaccional minimo de Warehouse deshabilitado por defecto');
GO
