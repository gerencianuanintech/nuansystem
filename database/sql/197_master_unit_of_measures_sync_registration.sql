/*
    Registra UnitOfMeasure como entidad incremental Matriz-Sucursal.

    194 + 196 y el backend deben estar desplegados antes de habilitar cualquier
    perfil. Las configuraciones/ownership nuevas nacen deshabilitadas y las
    existentes no se sobreescriben. Este script no activa perfiles ni workers.
*/

USE [NuanSystem_Master];
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME()<>N'NuanSystem_Master' THROW 51197,'Migration 197 must run only in NuanSystem_Master.',1;
IF OBJECT_ID(N'dbo.SyncEntityDefinitions',N'U') IS NULL THROW 51197,'SyncEntityDefinitions is required.',1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory',N'U') IS NULL THROW 51197,'MasterSchemaHistory is required.',1;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    IF EXISTS(SELECT 1 FROM dbo.SyncEntityDefinitions WHERE Code=N'UnitOfMeasure')
    BEGIN
        UPDATE dbo.SyncEntityDefinitions
        SET Name=N'Unidades de medida',
            Description=N'Maestro con GlobalId, LocalOutbox incremental y referencias externas locales no distribuibles.',
            DefaultExecutionOrder=50,SupportsIncremental=1,SupportsInsert=1,SupportsUpdate=1,SupportsDeactivate=1,
            DefaultKeyField=N'Code',DefaultModifiedAtField=N'UpdatedAt',IsSystem=1,IsActive=1,IsDeleted=0,
            UpdatedAt=SYSUTCDATETIME(),UpdatedByUserName=N'Sistema'
        WHERE Code=N'UnitOfMeasure';
    END
    ELSE
    BEGIN
        INSERT dbo.SyncEntityDefinitions
        (Code,Name,Description,DefaultExecutionOrder,SupportsIncremental,SupportsInsert,SupportsUpdate,SupportsDeactivate,
         DefaultKeyField,DefaultModifiedAtField,IsSystem,IsActive,CreatedByUserName)
        VALUES
        (N'UnitOfMeasure',N'Unidades de medida',
         N'Maestro con GlobalId, LocalOutbox incremental y referencias externas locales no distribuibles.',
         50,1,1,1,1,N'Code',N'UpdatedAt',1,1,N'Sistema');
    END;

    /* Conserva la dependencia Item -> UnitOfMeasure de 132. */
    IF OBJECT_ID(N'dbo.SyncEntityDefinitionDependencies',N'U') IS NOT NULL
    BEGIN
        DECLARE @ItemDefinitionId int=(SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code=N'Item' AND IsDeleted=0);
        DECLARE @UnitDefinitionId int=(SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code=N'UnitOfMeasure' AND IsDeleted=0);
        IF @UnitDefinitionId IS NULL THROW 51197,'UnitOfMeasure definition is required.',1;

        IF @ItemDefinitionId IS NOT NULL
        BEGIN
            IF EXISTS(SELECT 1 FROM dbo.SyncEntityDefinitionDependencies
                      WHERE EntityDefinitionId=@ItemDefinitionId AND DependsOnEntityDefinitionId=@UnitDefinitionId)
                UPDATE dbo.SyncEntityDefinitionDependencies
                SET IsDeleted=0,DeletedAt=NULL,DeletedByUserId=NULL,DeletedByUserName=NULL,
                    UpdatedAt=SYSUTCDATETIME(),UpdatedByUserName=N'Sistema'
                WHERE EntityDefinitionId=@ItemDefinitionId AND DependsOnEntityDefinitionId=@UnitDefinitionId;
            ELSE
                INSERT dbo.SyncEntityDefinitionDependencies
                (EntityDefinitionId,DependsOnEntityDefinitionId,CreatedByUserName,CreatedAt)
                VALUES(@ItemDefinitionId,@UnitDefinitionId,N'Sistema',SYSUTCDATETIME());
        END;
    END;

    IF OBJECT_ID(N'dbo.SyncEntityConfigurations',N'U') IS NOT NULL
    BEGIN
        INSERT dbo.SyncEntityConfigurations(CompanyId,EntityName,IsEnabled,Direction,ConflictPolicy,BatchSize,MaxAttempts)
        SELECT company.Id,N'UnitOfMeasure',CONVERT(bit,0),N'MasterToBranch',N'MasterWins',100,3
        FROM dbo.Companies company
        WHERE company.IsMaster=1
          AND NOT EXISTS(SELECT 1 FROM dbo.SyncEntityConfigurations existing
                         WHERE existing.CompanyId=company.Id AND existing.EntityName=N'UnitOfMeasure');
        /* Intencional: no sobreescribir configuraciones existentes. */
    END;

    IF OBJECT_ID(N'dbo.EntityOwnershipConfigurations',N'U') IS NOT NULL
    BEGIN
        INSERT dbo.EntityOwnershipConfigurations(CompanyId,EntityName,SourceOfTruth,SyncDirection,IsEnabled)
        SELECT company.Id,N'UnitOfMeasure',0,4,CONVERT(bit,0)
        FROM dbo.Companies company
        WHERE company.IsMaster=1
          AND NOT EXISTS(SELECT 1 FROM dbo.EntityOwnershipConfigurations existing
                         WHERE existing.CompanyId=company.Id AND existing.EntityName=N'UnitOfMeasure');
        /* Intencional: no sobreescribir ownership existente. */
    END;

    IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260812.197')
        INSERT dbo.MasterSchemaHistory(Version,Description)
        VALUES(N'20260812.197',N'Habilita contrato incremental de UnitOfMeasure sin activar perfiles ni workers');

    COMMIT;
END TRY
BEGIN CATCH
    IF XACT_STATE()<>0 ROLLBACK;
    THROW;
END CATCH;
GO
