/*
    Habilitacion manual idempotente del piloto Warehouse Sync Master -> Sucursal.

    Reglas:
    - Ejecutar en NuanSystem_Master.
    - No crea bodegas.
    - No crea SyncOutbox ni SyncOutboxTargets.
    - No toca SAP, SRI, stock, kardex, costos ni movimientos.
    - Puede ejecutarse varias veces sin duplicar configuraciones ni reglas.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;

USE [NuanSystem_Master];

DECLARE @MasterCompanyId int = NULL; -- TODO: colocar Id de la empresa Master.
DECLARE @BranchCompanyId int = NULL; -- TODO: colocar Id de la empresa sucursal destino.

DECLARE @EntityName nvarchar(120) = N'Warehouse';
DECLARE @Direction nvarchar(30) = N'MasterToBranch';
DECLARE @ConflictPolicy nvarchar(30) = N'MasterWins';
DECLARE @BatchSize int = 100;
DECLARE @MaxAttempts int = 3;
DECLARE @RuleType nvarchar(50) = N'All';
DECLARE @RuleValue nvarchar(200) = NULL;

IF @MasterCompanyId IS NULL
BEGIN
    THROW 51001, 'Debe asignar @MasterCompanyId antes de ejecutar el script.', 1;
END;

IF @BranchCompanyId IS NULL
BEGIN
    THROW 51002, 'Debe asignar @BranchCompanyId antes de ejecutar el script.', 1;
END;

IF OBJECT_ID(N'dbo.Companies', N'U') IS NULL
BEGIN
    THROW 51003, 'No existe dbo.Companies en NuanSystem_Master.', 1;
END;

IF OBJECT_ID(N'dbo.SyncEntityConfigurations', N'U') IS NULL
BEGIN
    THROW 51004, 'No existe dbo.SyncEntityConfigurations en NuanSystem_Master.', 1;
END;

IF OBJECT_ID(N'dbo.SyncDistributionRules', N'U') IS NULL
BEGIN
    THROW 51005, 'No existe dbo.SyncDistributionRules en NuanSystem_Master.', 1;
END;

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Companies AS company
    WHERE company.Id = @MasterCompanyId
      AND company.IsMaster = 1
      AND company.IsDeleted = 0
)
BEGIN
    THROW 51006, '@MasterCompanyId no existe o no corresponde a una empresa Master activa en metadata.', 1;
END;

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Companies AS branch
    WHERE branch.Id = @BranchCompanyId
      AND branch.IsMaster = 0
      AND branch.IsDeleted = 0
)
BEGIN
    THROW 51007, '@BranchCompanyId no existe o no corresponde a una sucursal.', 1;
END;

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Companies AS branch
    WHERE branch.Id = @BranchCompanyId
      AND branch.ParentCompanyId = @MasterCompanyId
      AND branch.IsDeleted = 0
)
BEGIN
    THROW 51008, 'La sucursal indicada no pertenece al @MasterCompanyId informado.', 1;
END;

BEGIN TRANSACTION;

UPDATE dbo.Companies
SET SyncEnabled = 1,
    UpdatedAt = SYSUTCDATETIME(),
    UpdatedByUserName = N'manual-enable-warehouse-sync-pilot'
WHERE Id IN (@MasterCompanyId, @BranchCompanyId)
  AND SyncEnabled = 0;

IF EXISTS
(
    SELECT 1
    FROM dbo.SyncEntityConfigurations
    WHERE CompanyId = @MasterCompanyId
      AND EntityName = @EntityName
)
BEGIN
    UPDATE dbo.SyncEntityConfigurations
    SET IsEnabled = 1,
        Direction = @Direction,
        ConflictPolicy = @ConflictPolicy,
        BatchSize = @BatchSize,
        MaxAttempts = @MaxAttempts,
        UpdatedAt = SYSUTCDATETIME()
    WHERE CompanyId = @MasterCompanyId
      AND EntityName = @EntityName;
END;
ELSE
BEGIN
    INSERT INTO dbo.SyncEntityConfigurations
    (
        CompanyId,
        EntityName,
        IsEnabled,
        Direction,
        ConflictPolicy,
        BatchSize,
        MaxAttempts
    )
    VALUES
    (
        @MasterCompanyId,
        @EntityName,
        CONVERT(bit, 1),
        @Direction,
        @ConflictPolicy,
        @BatchSize,
        @MaxAttempts
    );
END;

IF EXISTS
(
    SELECT 1
    FROM dbo.SyncDistributionRules
    WHERE CompanyId = @MasterCompanyId
      AND EntityName = @EntityName
      AND BranchCompanyId = @BranchCompanyId
)
BEGIN
    UPDATE dbo.SyncDistributionRules
    SET RuleType = @RuleType,
        RuleValue = @RuleValue,
        IsEnabled = 1,
        UpdatedAt = SYSUTCDATETIME()
    WHERE CompanyId = @MasterCompanyId
      AND EntityName = @EntityName
      AND BranchCompanyId = @BranchCompanyId;
END;
ELSE
BEGIN
    INSERT INTO dbo.SyncDistributionRules
    (
        CompanyId,
        EntityName,
        BranchCompanyId,
        RuleType,
        RuleValue,
        IsEnabled
    )
    VALUES
    (
        @MasterCompanyId,
        @EntityName,
        @BranchCompanyId,
        @RuleType,
        @RuleValue,
        CONVERT(bit, 1)
    );
END;

COMMIT TRANSACTION;

SELECT
    MasterCompanyId = @MasterCompanyId,
    BranchCompanyId = @BranchCompanyId,
    EntityName = @EntityName,
    SyncEnabled = CONVERT(bit, 1),
    Direction = @Direction,
    ConflictPolicy = @ConflictPolicy,
    RuleType = @RuleType,
    RuleValue = @RuleValue;
