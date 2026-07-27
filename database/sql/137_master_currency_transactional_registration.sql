/*
    Iteracion 8.5 - Registro Master de Currency transaccional.

    No habilita perfiles, rutas, ownership ni workers existentes o nuevos.
    Conserva la dependencia PriceList -> Currencies.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SyncEntityDefinitions', N'U') IS NULL
    THROW 51137, 'SyncEntityDefinitions is required before migration 137.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL
    THROW 51137, 'MasterSchemaHistory is required before migration 137.', 1;
GO

IF EXISTS (SELECT 1 FROM dbo.SyncEntityDefinitions WHERE Code = N'Currencies')
BEGIN
    UPDATE dbo.SyncEntityDefinitions
    SET Name = N'Monedas',
        Description = N'Catalogo con LocalOutbox transaccional, ExternalSystem/ExternalCode y conflicto terminal por codigo sin adopcion automatica.',
        DefaultExecutionOrder = 40,
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
    WHERE Code = N'Currencies';
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
        N'Currencies', N'Monedas',
        N'Catalogo con LocalOutbox transaccional, ExternalSystem/ExternalCode y conflicto terminal por codigo sin adopcion automatica.',
        40, 1, 1, 1, 1, N'Code', N'UpdatedAt', 1, 1, N'Sistema'
    );
END;
GO

IF OBJECT_ID(N'dbo.SyncEntityDefinitionDependencies', N'U') IS NOT NULL
BEGIN
    DECLARE @CurrencyDefinitionId int =
        (SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code = N'Currencies' AND IsDeleted = 0);
    DECLARE @PriceListDefinitionId int =
        (SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code = N'PriceList' AND IsDeleted = 0);

    IF @CurrencyDefinitionId IS NULL
        THROW 51137, 'Currencies definition is required for migration 137.', 1;

    IF @PriceListDefinitionId IS NOT NULL
       AND NOT EXISTS
       (
           SELECT 1
           FROM dbo.SyncEntityDefinitionDependencies
           WHERE EntityDefinitionId = @PriceListDefinitionId
             AND DependsOnEntityDefinitionId = @CurrencyDefinitionId
             AND IsDeleted = 0
       )
    BEGIN
        INSERT INTO dbo.SyncEntityDefinitionDependencies
        (
            EntityDefinitionId, DependsOnEntityDefinitionId, CreatedByUserName
        )
        VALUES
        (
            @PriceListDefinitionId, @CurrencyDefinitionId, N'Sistema'
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
    SELECT
        company.Id, N'Currencies', CONVERT(bit, 0), N'MasterToBranch', N'MasterWins', 100, 3
    FROM dbo.Companies AS company
    WHERE company.IsMaster = 1
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SyncEntityConfigurations AS existing
          WHERE existing.CompanyId = company.Id
            AND existing.EntityName = N'Currencies'
      );
END;
GO

IF OBJECT_ID(N'dbo.EntityOwnershipConfigurations', N'U') IS NOT NULL
BEGIN
    INSERT INTO dbo.EntityOwnershipConfigurations
    (
        CompanyId, EntityName, SourceOfTruth, SyncDirection, IsEnabled
    )
    SELECT
        company.Id, N'Currencies', 0, 4, CONVERT(bit, 0)
    FROM dbo.Companies AS company
    WHERE company.IsMaster = 1
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.EntityOwnershipConfigurations AS existing
          WHERE existing.CompanyId = company.Id
            AND existing.EntityName = N'Currencies'
      );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.MasterSchemaHistory
    WHERE Version = N'20260727.137'
)
BEGIN
    INSERT INTO dbo.MasterSchemaHistory(Version, Description)
    VALUES
    (
        N'20260727.137',
        N'Registra Currency transaccional y conserva dependencia PriceList'
    );
END;
GO
