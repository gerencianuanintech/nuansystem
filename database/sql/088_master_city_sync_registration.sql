/*
    Registra Cities como capacidad operativa dependiente de Countries y Provinces.
    No activa perfiles ni distribuciones existentes.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF OBJECT_ID(N'dbo.SyncEntityDefinitions', N'U') IS NOT NULL
BEGIN
    UPDATE dbo.SyncEntityDefinitions
    SET Description = N'Catalogo geografico dependiente de Countries y Provinces con productor incremental, fuente Full y aplicador idempotente por GlobalId.',
        SupportsIncremental = 1,
        SupportsInsert = 1,
        SupportsUpdate = 1,
        SupportsDeactivate = 1,
        DefaultKeyField = N'Code',
        DefaultModifiedAtField = N'UpdatedAt',
        IsSystem = 1,
        IsActive = 1,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserName = N'Sistema'
    WHERE Code = N'Cities'
      AND IsDeleted = 0;
END;
GO

IF OBJECT_ID(N'dbo.SyncEntityConfigurations', N'U') IS NOT NULL
BEGIN
    INSERT INTO dbo.SyncEntityConfigurations
    (
        CompanyId, EntityName, IsEnabled, Direction, ConflictPolicy, BatchSize, MaxAttempts
    )
    SELECT
        company.Id, N'Cities', CONVERT(bit, 0), N'MasterToBranch', N'MasterWins', 500, 3
    FROM dbo.Companies AS company
    WHERE company.IsMaster = 1
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SyncEntityConfigurations AS existing
          WHERE existing.CompanyId = company.Id
            AND existing.EntityName = N'Cities'
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
        company.Id, N'Cities', 0, 4, CONVERT(bit, 0)
    FROM dbo.Companies AS company
    WHERE company.IsMaster = 1
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.EntityOwnershipConfigurations AS existing
          WHERE existing.CompanyId = company.Id
            AND existing.EntityName = N'Cities'
      );
END;
GO

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NOT NULL
    AND NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260716.088')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description)
    VALUES (N'20260716.088', N'Cities registrado como entidad operativa Maestro-Sucursal');
END;
GO
