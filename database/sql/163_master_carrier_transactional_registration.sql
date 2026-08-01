/*
    Iteracion 8.8 - Registro Master de Transportistas Matriz-Sucursal.

    Registra la capacidad de gobierno y distribucion. No habilita perfiles,
    rutas, ownership ni workers y no duplica el formulario existente.

    Ejecutar solo en NuanSystem_Master.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME() <> N'NuanSystem_Master'
    THROW 51163, 'Migration 163 must run only in NuanSystem_Master.', 1;
IF OBJECT_ID(N'dbo.SyncEntityDefinitions', N'U') IS NULL
    THROW 51163, 'SyncEntityDefinitions is required before migration 163.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL
    THROW 51163, 'MasterSchemaHistory is required before migration 163.', 1;
GO

IF EXISTS (SELECT 1 FROM dbo.SyncEntityDefinitions WHERE Code = N'Carrier')
BEGIN
    UPDATE dbo.SyncEntityDefinitions
    SET Name = N'Transportistas',
        Description = N'Maestro independiente con LocalOutbox transaccional, GlobalId y conflicto terminal sin adopcion por codigo.',
        DefaultExecutionOrder = 240,
        SupportsIncremental = 1,
        SupportsInsert = 1,
        SupportsUpdate = 1,
        SupportsDeactivate = 1,
        DefaultKeyField = N'Id',
        DefaultModifiedAtField = N'UpdatedAt',
        IsSystem = 1,
        IsActive = 1,
        IsDeleted = 0,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserName = N'Sistema'
    WHERE Code = N'Carrier';
END
ELSE
BEGIN
    INSERT dbo.SyncEntityDefinitions
    (
        Code, Name, Description, DefaultExecutionOrder, SupportsIncremental,
        SupportsInsert, SupportsUpdate, SupportsDeactivate,
        DefaultKeyField, DefaultModifiedAtField, IsSystem, IsActive, CreatedByUserName
    )
    VALUES
    (
        N'Carrier', N'Transportistas',
        N'Maestro independiente con LocalOutbox transaccional, GlobalId y conflicto terminal sin adopcion por codigo.',
        240, 1, 1, 1, 1, N'Id', N'UpdatedAt', 1, 1, N'Sistema'
    );
END;
GO

IF OBJECT_ID(N'dbo.SyncEntityDefinitionDependencies', N'U') IS NOT NULL
BEGIN
    DECLARE @CarrierDefinitionId int =
        (SELECT Id FROM dbo.SyncEntityDefinitions WHERE Code = N'Carrier' AND IsDeleted = 0);

    IF @CarrierDefinitionId IS NULL
        THROW 51163, 'Carrier definition is required for migration 163.', 1;

    UPDATE dbo.SyncEntityDefinitionDependencies
    SET IsDeleted = 1,
        DeletedAt = COALESCE(DeletedAt, SYSUTCDATETIME()),
        DeletedByUserName = N'Sistema',
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserName = N'Sistema'
    WHERE EntityDefinitionId = @CarrierDefinitionId
      AND IsDeleted = 0;
END;
GO

IF OBJECT_ID(N'dbo.SyncEntityConfigurations', N'U') IS NOT NULL
BEGIN
    INSERT dbo.SyncEntityConfigurations
        (CompanyId, EntityName, IsEnabled, Direction, ConflictPolicy, BatchSize, MaxAttempts)
    SELECT company.Id, N'Carrier', CONVERT(bit, 0), N'MasterToBranch', N'MasterWins', 100, 3
    FROM dbo.Companies AS company
    WHERE company.IsMaster = 1
      AND NOT EXISTS
      (
          SELECT 1 FROM dbo.SyncEntityConfigurations AS existing
          WHERE existing.CompanyId = company.Id AND existing.EntityName = N'Carrier'
      );

END;
GO

IF OBJECT_ID(N'dbo.EntityOwnershipConfigurations', N'U') IS NOT NULL
BEGIN
    INSERT dbo.EntityOwnershipConfigurations
        (CompanyId, EntityName, SourceOfTruth, SyncDirection, IsEnabled)
    SELECT company.Id, N'Carrier', 0, 4, CONVERT(bit, 0)
    FROM dbo.Companies AS company
    WHERE company.IsMaster = 1
      AND NOT EXISTS
      (
          SELECT 1 FROM dbo.EntityOwnershipConfigurations AS existing
          WHERE existing.CompanyId = company.Id AND existing.EntityName = N'Carrier'
      );

END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260801.163')
BEGIN
    INSERT dbo.MasterSchemaHistory(Version, Description)
    VALUES
    (
        N'20260801.163',
        N'Registra Transportistas transaccional Matriz-Sucursal deshabilitado por defecto'
    );
END;
GO
