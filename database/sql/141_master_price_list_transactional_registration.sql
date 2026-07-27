/*
    Iteracion 8.6 - Registro Master de PriceList.

    Mantiene perfiles, rutas, ownership y workers deshabilitados.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SyncEntityDefinitions', N'U') IS NULL
    THROW 51141, 'SyncEntityDefinitions is required before migration 141.', 1;
IF OBJECT_ID(N'dbo.SyncEntityDefinitionDependencies', N'U') IS NULL
    THROW 51141, 'SyncEntityDefinitionDependencies is required before migration 141.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL
    THROW 51141, 'MasterSchemaHistory is required before migration 141.', 1;
GO

IF EXISTS (SELECT 1 FROM dbo.SyncEntityDefinitions WHERE Code = N'PriceList')
BEGIN
    UPDATE dbo.SyncEntityDefinitions
    SET Name = N'Listas de precios',
        Description = N'Catalogo Pricing con LocalOutbox transaccional, Currency por GlobalId y conflicto terminal sin adopcion.',
        DefaultExecutionOrder = 230,
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
    WHERE Code = N'PriceList';
END
ELSE
BEGIN
    INSERT INTO dbo.SyncEntityDefinitions
    (
        Code, Name, Description, DefaultExecutionOrder, SupportsIncremental,
        SupportsInsert, SupportsUpdate, SupportsDeactivate,
        DefaultKeyField, DefaultModifiedAtField, IsSystem, IsActive, CreatedByUserName
    )
    VALUES
    (
        N'PriceList', N'Listas de precios',
        N'Catalogo Pricing con LocalOutbox transaccional, Currency por GlobalId y conflicto terminal sin adopcion.',
        230, 1, 1, 1, 1, N'Code', N'UpdatedAt', 1, 1, N'Sistema'
    );
END;
GO

DECLARE @PriceListDefinitionId int =
    (SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code = N'PriceList' AND IsDeleted = 0);
DECLARE @CurrencyDefinitionId int =
    (SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code = N'Currencies' AND IsDeleted = 0);
DECLARE @ItemDefinitionId int =
    (SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code = N'Item' AND IsDeleted = 0);

IF @PriceListDefinitionId IS NULL OR @CurrencyDefinitionId IS NULL
    THROW 51141, 'PriceList and Currencies definitions are required for migration 141.', 1;

IF @ItemDefinitionId IS NOT NULL
BEGIN
    UPDATE dbo.SyncEntityDefinitionDependencies
    SET IsDeleted = 1,
        DeletedAt = COALESCE(DeletedAt, SYSUTCDATETIME()),
        DeletedByUserName = N'Sistema',
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserName = N'Sistema'
    WHERE EntityDefinitionId = @PriceListDefinitionId
      AND DependsOnEntityDefinitionId = @ItemDefinitionId
      AND IsDeleted = 0;
END;

IF EXISTS
(
    SELECT 1 FROM dbo.SyncEntityDefinitionDependencies
    WHERE EntityDefinitionId = @PriceListDefinitionId
      AND DependsOnEntityDefinitionId = @CurrencyDefinitionId
      AND IsDeleted = 1
)
BEGIN
    UPDATE dbo.SyncEntityDefinitionDependencies
    SET IsDeleted = 0, DeletedAt = NULL, DeletedByUserName = NULL,
        UpdatedAt = SYSUTCDATETIME(), UpdatedByUserName = N'Sistema'
    WHERE EntityDefinitionId = @PriceListDefinitionId
      AND DependsOnEntityDefinitionId = @CurrencyDefinitionId;
END
ELSE IF NOT EXISTS
(
    SELECT 1 FROM dbo.SyncEntityDefinitionDependencies
    WHERE EntityDefinitionId = @PriceListDefinitionId
      AND DependsOnEntityDefinitionId = @CurrencyDefinitionId
      AND IsDeleted = 0
)
BEGIN
    INSERT INTO dbo.SyncEntityDefinitionDependencies
    (
        EntityDefinitionId, DependsOnEntityDefinitionId, CreatedByUserName
    )
    VALUES (@PriceListDefinitionId, @CurrencyDefinitionId, N'Sistema');
END;
GO

IF OBJECT_ID(N'dbo.SyncEntityConfigurations', N'U') IS NOT NULL
BEGIN
    INSERT INTO dbo.SyncEntityConfigurations
    (
        CompanyId, EntityName, IsEnabled, Direction, ConflictPolicy, BatchSize, MaxAttempts
    )
    SELECT company.Id, N'PriceList', CONVERT(bit, 0), N'MasterToBranch', N'MasterWins', 100, 3
    FROM dbo.Companies AS company
    WHERE company.IsMaster = 1
      AND NOT EXISTS
      (
          SELECT 1 FROM dbo.SyncEntityConfigurations AS existing
          WHERE existing.CompanyId = company.Id AND existing.EntityName = N'PriceList'
      );
END;
GO

IF OBJECT_ID(N'dbo.EntityOwnershipConfigurations', N'U') IS NOT NULL
BEGIN
    INSERT INTO dbo.EntityOwnershipConfigurations
    (
        CompanyId, EntityName, SourceOfTruth, SyncDirection, IsEnabled
    )
    SELECT company.Id, N'PriceList', 0, 4, CONVERT(bit, 0)
    FROM dbo.Companies AS company
    WHERE company.IsMaster = 1
      AND NOT EXISTS
      (
          SELECT 1 FROM dbo.EntityOwnershipConfigurations AS existing
          WHERE existing.CompanyId = company.Id AND existing.EntityName = N'PriceList'
      );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260727.141')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory(Version, Description)
    VALUES (N'20260727.141', N'Registra PriceList transaccional con dependencia exclusiva Currency');
END;
GO
