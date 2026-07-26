/*
    Declara UnitOfMeasures como dependencia conceptual de Item.
    UOM continua en modo Full; esta migracion no habilita CRUD incremental.

    Ejecutar solo en NuanSystem_Master. Script idempotente. No ejecutado por esta rama.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SyncEntityDefinitions', N'U') IS NULL
   OR OBJECT_ID(N'dbo.SyncEntityDefinitionDependencies', N'U') IS NULL
    THROW 51132, 'Sync entity definitions and dependencies are required before migration 132.', 1;
GO

DECLARE @ItemDefinitionId int =
    (SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code = N'Item' AND IsDeleted = 0);
DECLARE @UnitOfMeasuresDefinitionId int =
    (SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code = N'UnitOfMeasures' AND IsDeleted = 0);

IF @ItemDefinitionId IS NULL OR @UnitOfMeasuresDefinitionId IS NULL
    THROW 51132, 'Item and UnitOfMeasures definitions are required for migration 132.', 1;

IF EXISTS
(
    SELECT 1
    FROM dbo.SyncEntityDefinitionDependencies
    WHERE EntityDefinitionId = @ItemDefinitionId
      AND DependsOnEntityDefinitionId = @UnitOfMeasuresDefinitionId
)
BEGIN
    UPDATE dbo.SyncEntityDefinitionDependencies
    SET IsDeleted = 0,
        DeletedAt = NULL,
        DeletedByUserName = NULL,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserName = N'Sistema'
    WHERE EntityDefinitionId = @ItemDefinitionId
      AND DependsOnEntityDefinitionId = @UnitOfMeasuresDefinitionId
      AND IsDeleted = 1;
END
ELSE
BEGIN
    INSERT INTO dbo.SyncEntityDefinitionDependencies
    (
        EntityDefinitionId,
        DependsOnEntityDefinitionId,
        CreatedByUserName
    )
    VALUES
    (
        @ItemDefinitionId,
        @UnitOfMeasuresDefinitionId,
        N'Sistema'
    );
END;
GO

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL
    THROW 51132, 'MasterSchemaHistory is required before recording migration 132.', 1;

IF NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260726.132')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description)
    VALUES (N'20260726.132', N'Declara UnitOfMeasures como dependencia conceptual de Item');
END;
GO
