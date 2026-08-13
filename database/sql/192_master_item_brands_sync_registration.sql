/*
    Registra ItemBrands para sincronizacion Matriz-Sucursal.

    Solo NuanSystem_Master. No habilita perfiles, ownership, rutas ni workers.
    ItemBrands se ejecuta antes de Item y se registra como dependencia de Item
    cuando la tabla de dependencias esta disponible.
*/

USE [NuanSystem_Master];
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME() <> N'NuanSystem_Master' THROW 51192, 'Migration 192 must run only in NuanSystem_Master.', 1;
IF OBJECT_ID(N'dbo.SyncEntityDefinitions', N'U') IS NULL THROW 51192, 'SyncEntityDefinitions is required.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL THROW 51192, 'MasterSchemaHistory is required.', 1;
GO

IF EXISTS(SELECT 1 FROM dbo.SyncEntityDefinitions WHERE Code=N'ItemBrands')
BEGIN
    UPDATE dbo.SyncEntityDefinitions
    SET Name=N'Marcas de articulos',
        Description=N'Maestro independiente con LocalOutbox, GlobalId y referencias SAP/externas locales no distribuibles.',
        DefaultExecutionOrder=208,SupportsIncremental=1,SupportsInsert=1,SupportsUpdate=1,SupportsDeactivate=1,
        DefaultKeyField=N'Code',DefaultModifiedAtField=N'UpdatedAt',IsSystem=1,IsActive=1,IsDeleted=0,
        UpdatedAt=SYSUTCDATETIME(),UpdatedByUserName=N'Sistema'
    WHERE Code=N'ItemBrands';
END
ELSE
BEGIN
    INSERT dbo.SyncEntityDefinitions
    (Code,Name,Description,DefaultExecutionOrder,SupportsIncremental,SupportsInsert,SupportsUpdate,SupportsDeactivate,
     DefaultKeyField,DefaultModifiedAtField,IsSystem,IsActive,CreatedByUserName)
    VALUES
    (N'ItemBrands',N'Marcas de articulos',
     N'Maestro independiente con LocalOutbox, GlobalId y referencias SAP/externas locales no distribuibles.',
     208,1,1,1,1,N'Code',N'UpdatedAt',1,1,N'Sistema');
END;
GO

IF OBJECT_ID(N'dbo.SyncEntityDefinitionDependencies', N'U') IS NOT NULL
BEGIN
    DECLARE @ItemBrandsDefinitionId int=(SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code=N'ItemBrands' AND IsDeleted=0);
    DECLARE @ItemDefinitionId int=(SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code=N'Item' AND IsDeleted=0);
    IF @ItemBrandsDefinitionId IS NULL THROW 51192, 'ItemBrands definition is required.', 1;

    /* ItemBrands no depende de otra entidad. Elimina dependencias activas
       heredadas sin tocar el historial fisico. */
    UPDATE dbo.SyncEntityDefinitionDependencies
    SET IsDeleted=1,DeletedAt=COALESCE(DeletedAt,SYSUTCDATETIME()),DeletedByUserName=N'Sistema',
        UpdatedAt=SYSUTCDATETIME(),UpdatedByUserName=N'Sistema'
    WHERE EntityDefinitionId=@ItemBrandsDefinitionId AND IsDeleted=0;

    IF @ItemDefinitionId IS NOT NULL AND NOT EXISTS
    (
        SELECT 1 FROM dbo.SyncEntityDefinitionDependencies
        WHERE EntityDefinitionId=@ItemDefinitionId AND DependsOnEntityDefinitionId=@ItemBrandsDefinitionId AND IsDeleted=0
    )
    BEGIN
        /* Reactiva una relacion historica si existe; inserta solo si no existe. */
        IF EXISTS
        (
            SELECT 1 FROM dbo.SyncEntityDefinitionDependencies
            WHERE EntityDefinitionId=@ItemDefinitionId AND DependsOnEntityDefinitionId=@ItemBrandsDefinitionId
        )
            UPDATE dbo.SyncEntityDefinitionDependencies
            SET IsDeleted=0,DeletedAt=NULL,DeletedByUserId=NULL,DeletedByUserName=NULL,
                UpdatedAt=SYSUTCDATETIME(),UpdatedByUserName=N'Sistema'
            WHERE Id=
            (
                SELECT TOP(1) Id FROM dbo.SyncEntityDefinitionDependencies
                WHERE EntityDefinitionId=@ItemDefinitionId AND DependsOnEntityDefinitionId=@ItemBrandsDefinitionId
                ORDER BY Id DESC
            );
        ELSE
            INSERT dbo.SyncEntityDefinitionDependencies
            (EntityDefinitionId,DependsOnEntityDefinitionId,CreatedByUserName,CreatedAt)
            VALUES(@ItemDefinitionId,@ItemBrandsDefinitionId,N'Sistema',SYSUTCDATETIME());
    END;
END;
GO

IF OBJECT_ID(N'dbo.SyncEntityConfigurations', N'U') IS NOT NULL
BEGIN
    INSERT dbo.SyncEntityConfigurations(CompanyId,EntityName,IsEnabled,Direction,ConflictPolicy,BatchSize,MaxAttempts)
    SELECT company.Id,N'ItemBrands',CONVERT(bit,0),N'MasterToBranch',N'MasterWins',100,3
    FROM dbo.Companies company
    WHERE company.IsMaster=1
      AND NOT EXISTS(SELECT 1 FROM dbo.SyncEntityConfigurations c
                     WHERE c.CompanyId=company.Id AND c.EntityName=N'ItemBrands');
    /* Intencional: no sobreescribir configuraciones existentes. */
END;
GO

IF OBJECT_ID(N'dbo.EntityOwnershipConfigurations', N'U') IS NOT NULL
BEGIN
    INSERT dbo.EntityOwnershipConfigurations(CompanyId,EntityName,SourceOfTruth,SyncDirection,IsEnabled)
    SELECT company.Id,N'ItemBrands',0,4,CONVERT(bit,0)
    FROM dbo.Companies company
    WHERE company.IsMaster=1
      AND NOT EXISTS(SELECT 1 FROM dbo.EntityOwnershipConfigurations o
                     WHERE o.CompanyId=company.Id AND o.EntityName=N'ItemBrands');
    /* Intencional: no sobreescribir ownership existente. */
END;
GO

IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260812.192')
    INSERT dbo.MasterSchemaHistory(Version,Description)
    VALUES(N'20260812.192',N'Registra ItemBrands Matriz-Sucursal deshabilitado por defecto y antes de Item');
GO
