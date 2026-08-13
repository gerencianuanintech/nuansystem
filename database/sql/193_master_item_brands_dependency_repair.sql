/*
    Reparacion forward-only de la dependencia Item -> ItemBrands creada por 192.

    Item todavia no contiene BrandId ni publica BrandGlobalId en su contrato de
    sincronizacion. La relacion se conserva fisicamente como historial, pero se
    marca eliminada hasta que exista una dependencia funcional real.

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
    THROW 51193, 'Migration 193 must run only in NuanSystem_Master.', 1;
IF OBJECT_ID(N'dbo.SyncEntityDefinitions', N'U') IS NULL
   OR OBJECT_ID(N'dbo.SyncEntityDefinitionDependencies', N'U') IS NULL
    THROW 51193, 'Sync entity definitions and dependencies are required.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL
    THROW 51193, 'MasterSchemaHistory is required.', 1;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @ItemDefinitionId int =
        (SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code=N'Item' AND IsDeleted=0);
    DECLARE @ItemBrandsDefinitionId int =
        (SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code=N'ItemBrands' AND IsDeleted=0);

    IF @ItemDefinitionId IS NULL OR @ItemBrandsDefinitionId IS NULL
        THROW 51193, 'Item and ItemBrands definitions from migration 192 are required.', 1;

    UPDATE dbo.SyncEntityDefinitionDependencies
    SET IsDeleted=1,
        DeletedAt=COALESCE(DeletedAt,SYSUTCDATETIME()),
        DeletedByUserName=COALESCE(DeletedByUserName,N'Sistema'),
        UpdatedAt=SYSUTCDATETIME(),
        UpdatedByUserName=N'Sistema'
    WHERE EntityDefinitionId=@ItemDefinitionId
      AND DependsOnEntityDefinitionId=@ItemBrandsDefinitionId
      AND IsDeleted=0;

    IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260812.193')
        INSERT dbo.MasterSchemaHistory(Version,Description)
        VALUES(N'20260812.193',
               N'Retira Item -> ItemBrands hasta que Items tenga BrandId y BrandGlobalId sincronizable');

    COMMIT;
END TRY
BEGIN CATCH
    IF XACT_STATE()<>0 ROLLBACK;
    THROW;
END CATCH;
GO
