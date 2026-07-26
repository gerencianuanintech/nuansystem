/*
    Repara el registro de ItemGroups como entidad Matriz-Sucursal deshabilitada
    y conserva ItemGroups -> ItemFamilies -> Item.

    Ejecutar solo en NuanSystem_Master.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SyncEntityDefinitions', N'U') IS NULL
    THROW 51130, 'SyncEntityDefinitions is required before migration 130.', 1;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SyncEntityDefinitions WHERE Code = N'ItemGroups')
BEGIN
    INSERT INTO dbo.SyncEntityDefinitions
    (
        Code, Name, Description, DefaultExecutionOrder, SupportsIncremental,
        SupportsInsert, SupportsUpdate, SupportsDeactivate,
        DefaultKeyField, DefaultModifiedAtField, IsSystem, IsActive, CreatedByUserName
    )
    VALUES
    (
        N'ItemGroups', N'Grupos de articulos',
        N'Catalogo con LocalOutbox transaccional y aplicacion por GlobalId sin adopcion por codigo.',
        205, 1, 1, 1, 1, N'Code', N'UpdatedAt', 1, 1, N'Sistema'
    );
END
ELSE
BEGIN
    UPDATE dbo.SyncEntityDefinitions
    SET Name = N'Grupos de articulos',
        Description = N'Catalogo con LocalOutbox transaccional y aplicacion por GlobalId sin adopcion por codigo.',
        DefaultExecutionOrder = 205,
        SupportsIncremental = 1,
        SupportsInsert = 1,
        SupportsUpdate = 1,
        SupportsDeactivate = 1,
        DefaultKeyField = N'Code',
        DefaultModifiedAtField = N'UpdatedAt',
        IsSystem = 1,
        IsActive = 1,
        IsDeleted = 0,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserName = N'Sistema'
    WHERE Code = N'ItemGroups';
END;
GO

IF OBJECT_ID(N'dbo.SyncEntityDefinitionDependencies', N'U') IS NOT NULL
BEGIN
    DECLARE @ItemGroupsDefinitionId int =
        (SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code = N'ItemGroups' AND IsDeleted = 0);
    DECLARE @ItemFamiliesDefinitionId int =
        (SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code = N'ItemFamilies' AND IsDeleted = 0);
    DECLARE @ItemDefinitionId int =
        (SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code = N'Item' AND IsDeleted = 0);

    IF @ItemGroupsDefinitionId IS NULL
        THROW 51130, 'ItemGroups definition is required for migration 130.', 1;

    IF @ItemFamiliesDefinitionId IS NOT NULL
       AND NOT EXISTS
       (
           SELECT 1
           FROM dbo.SyncEntityDefinitionDependencies
           WHERE EntityDefinitionId = @ItemFamiliesDefinitionId
             AND DependsOnEntityDefinitionId = @ItemGroupsDefinitionId
             AND IsDeleted = 0
       )
    BEGIN
        INSERT INTO dbo.SyncEntityDefinitionDependencies
        (
            EntityDefinitionId, DependsOnEntityDefinitionId, CreatedByUserName
        )
        VALUES
        (
            @ItemFamiliesDefinitionId, @ItemGroupsDefinitionId, N'Sistema'
        );
    END;

    IF @ItemDefinitionId IS NOT NULL
       AND NOT EXISTS
       (
           SELECT 1
           FROM dbo.SyncEntityDefinitionDependencies
           WHERE EntityDefinitionId = @ItemDefinitionId
             AND DependsOnEntityDefinitionId = @ItemGroupsDefinitionId
             AND IsDeleted = 0
       )
    BEGIN
        INSERT INTO dbo.SyncEntityDefinitionDependencies
        (
            EntityDefinitionId, DependsOnEntityDefinitionId, CreatedByUserName
        )
        VALUES
        (
            @ItemDefinitionId, @ItemGroupsDefinitionId, N'Sistema'
        );
    END;
END;
GO

IF OBJECT_ID(N'dbo.SyncEntityConfigurations', N'U') IS NOT NULL
BEGIN
    INSERT INTO dbo.SyncEntityConfigurations
    (
        CompanyId, EntityName, IsEnabled, Direction, ConflictPolicy, BatchSize, MaxAttempts
    )
    SELECT company.Id, N'ItemGroups', CONVERT(bit, 0), N'MasterToBranch', N'MasterWins', 100, 3
    FROM dbo.Companies AS company
    WHERE company.IsMaster = 1
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SyncEntityConfigurations AS existing
          WHERE existing.CompanyId = company.Id
            AND existing.EntityName = N'ItemGroups'
      );
END;
GO

IF OBJECT_ID(N'dbo.EntityOwnershipConfigurations', N'U') IS NOT NULL
BEGIN
    INSERT INTO dbo.EntityOwnershipConfigurations
    (
        CompanyId, EntityName, SourceOfTruth, SyncDirection, IsEnabled
    )
    SELECT company.Id, N'ItemGroups', 0, 4, CONVERT(bit, 0)
    FROM dbo.Companies AS company
    WHERE company.IsMaster = 1
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.EntityOwnershipConfigurations AS existing
          WHERE existing.CompanyId = company.Id
            AND existing.EntityName = N'ItemGroups'
      );
END;
GO

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL
    THROW 51130, 'MasterSchemaHistory is required before recording migration 130.', 1;

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.MasterSchemaHistory
    WHERE Version = N'20260726.130'
)
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description)
    VALUES
    (
        N'20260726.130',
        N'Registra ItemGroups transaccional y dependencias con ItemFamilies e Item'
    );
END;
GO
