/*
    Registra ItemGroups como entidad operativa Maestro-Sucursal y declara que
    Item depende de este catalogo. No activa perfiles existentes.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF OBJECT_ID(N'dbo.SyncEntityDefinitions', N'U') IS NOT NULL
BEGIN
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
            N'Catalogo maestro con productor incremental, fuente Full y aplicador idempotente por GlobalId.',
            205, 1, 1, 1, 1, N'Code', N'UpdatedAt', 1, 1, N'Sistema'
        );
    END
    ELSE
    BEGIN
        UPDATE dbo.SyncEntityDefinitions
        SET Name = N'Grupos de articulos',
            Description = N'Catalogo maestro con productor incremental, fuente Full y aplicador idempotente por GlobalId.',
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

    IF OBJECT_ID(N'dbo.SyncEntityDefinitionDependencies', N'U') IS NOT NULL
    BEGIN
        DECLARE @ItemDefinitionId int = (SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code = N'Item' AND IsDeleted = 0);
        DECLARE @ItemGroupsDefinitionId int = (SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code = N'ItemGroups' AND IsDeleted = 0);

        IF @ItemDefinitionId IS NOT NULL AND @ItemGroupsDefinitionId IS NOT NULL
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
END;
GO

IF OBJECT_ID(N'dbo.SyncEntityConfigurations', N'U') IS NOT NULL
BEGIN
    INSERT INTO dbo.SyncEntityConfigurations
    (
        CompanyId, EntityName, IsEnabled, Direction, ConflictPolicy, BatchSize, MaxAttempts
    )
    SELECT company.Id, N'ItemGroups', CONVERT(bit, 0), N'MasterToBranch', N'MasterWins', 500, 3
    FROM dbo.Companies AS company
    WHERE company.IsMaster = 1
      AND NOT EXISTS
      (
          SELECT 1 FROM dbo.SyncEntityConfigurations AS existing
          WHERE existing.CompanyId = company.Id AND existing.EntityName = N'ItemGroups'
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
          SELECT 1 FROM dbo.EntityOwnershipConfigurations AS existing
          WHERE existing.CompanyId = company.Id AND existing.EntityName = N'ItemGroups'
      );
END;
GO

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260717.098')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description)
    VALUES (N'20260717.098', N'ItemGroups registrado como entidad operativa Maestro-Sucursal');
END;
GO
