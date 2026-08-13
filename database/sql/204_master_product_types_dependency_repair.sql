/*
    Reparacion forward-only de la dependencia Item -> ProductType creada por 200.

    El payload vigente de Item todavia no publica ProductTypeGlobalId. Mantener
    esa dependencia activa obliga a configurar un maestro que Item no puede
    resolver ni aplicar. La relacion se conserva como historial, pero se marca
    eliminada hasta que el contrato Item incorpore la identidad GlobalId.

    Solo NuanSystem_Master. No modifica definiciones, configuraciones, perfiles,
    ownership ni workers.
*/

USE [NuanSystem_Master];
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME() <> N'NuanSystem_Master'
    THROW 51204, 'Migration 204 must run only in NuanSystem_Master.', 1;
IF OBJECT_ID(N'dbo.SyncEntityDefinitions', N'U') IS NULL
   OR OBJECT_ID(N'dbo.SyncEntityDefinitionDependencies', N'U') IS NULL
    THROW 51204, 'Sync entity definitions and dependencies are required.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL
    THROW 51204, 'MasterSchemaHistory is required.', 1;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @ItemDefinitionId int =
        (SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code=N'Item' AND IsDeleted=0);
    DECLARE @ProductTypeDefinitionId int =
        (SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code=N'ProductType' AND IsDeleted=0);

    IF @ItemDefinitionId IS NULL OR @ProductTypeDefinitionId IS NULL
        THROW 51204, 'Item and ProductType definitions from migration 200 are required.', 1;

    UPDATE dbo.SyncEntityDefinitionDependencies
    SET IsDeleted=1,
        DeletedAt=COALESCE(DeletedAt,SYSUTCDATETIME()),
        DeletedByUserName=COALESCE(DeletedByUserName,N'Sistema'),
        UpdatedAt=SYSUTCDATETIME(),
        UpdatedByUserName=N'Sistema'
    WHERE EntityDefinitionId=@ItemDefinitionId
      AND DependsOnEntityDefinitionId=@ProductTypeDefinitionId
      AND IsDeleted=0;

    IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260813.204')
        INSERT dbo.MasterSchemaHistory(Version,Description)
        VALUES(N'20260813.204',
               N'Retira Item -> ProductType hasta que Item publique ProductTypeGlobalId');

    COMMIT;
END TRY
BEGIN CATCH
    IF XACT_STATE()<>0 ROLLBACK;
    THROW;
END CATCH;
GO
