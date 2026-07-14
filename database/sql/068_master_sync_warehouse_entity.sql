/*
    Fase 4.10: registro de Warehouse como entidad replicable Master/Sucursal.

    Reglas:
    - Registra la entidad en configuracion Sync sin activarla automaticamente.
    - La activacion del piloto debe hacerse por empresa/sucursal de forma controlada.
    - No toca SAP, SRI, stock, kardex, costos ni movimientos.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF OBJECT_ID(N'dbo.SyncEntityConfigurations', N'U') IS NOT NULL
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
    SELECT
        company.Id,
        N'Warehouse',
        CONVERT(bit, 0),
        N'MasterToBranch',
        N'MasterWins',
        100,
        3
    FROM dbo.Companies AS company
    WHERE company.IsMaster = 1
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.SyncEntityConfigurations AS existing
          WHERE existing.CompanyId = company.Id
            AND existing.EntityName = N'Warehouse'
      );
END;
GO

IF OBJECT_ID(N'dbo.EntityOwnershipConfigurations', N'U') IS NOT NULL
BEGIN
    INSERT INTO dbo.EntityOwnershipConfigurations
    (
        CompanyId,
        EntityName,
        SourceOfTruth,
        SyncDirection,
        IsEnabled
    )
    SELECT
        company.Id,
        N'Warehouse',
        0,
        4,
        CONVERT(bit, 0)
    FROM dbo.Companies AS company
    WHERE company.IsMaster = 1
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.EntityOwnershipConfigurations AS existing
          WHERE existing.CompanyId = company.Id
            AND existing.EntityName = N'Warehouse'
      );
END;
GO

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NOT NULL
    AND NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260710.02')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description)
    VALUES (N'20260710.02', N'Fase 4.10: Warehouse registrado como entidad replicable Master/Sucursal');
END;
GO
