/*
    Fase 10.6 - Registro forward-only de la capacidad SAP de Bodegas.
    Prerrequisitos:
      - 152_master_sap_sync_profiles.sql.
      - 157_master_sap_sync_scheduler_session_options.sql.

    Alcance: registra Warehouses como handler implementado exclusivamente para
    SAP -> ERP en modo Full. No crea ni activa perfiles, entidades o agendas.
*/
USE [NuanSystem_Master];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.MasterSchemaHistory
    WHERE Version = N'20260731.157'
)
    THROW 51159, 'SAP scheduler migration 157 is required.', 1;
GO

BEGIN TRANSACTION;

UPDATE dbo.SapSyncHandlerCapabilities
SET DisplayName = N'Bodegas',
    SupportsSapToErp = 1,
    SupportsErpToSap = 0,
    SupportsFull = 1,
    SupportsIncremental = 0,
    IsImplemented = 1,
    IsActive = 1,
    UpdatedByUserName = N'Sistema',
    UpdatedAt = SYSUTCDATETIME()
WHERE EntityCode = N'Warehouses'
  AND
  (
      DisplayName <> N'Bodegas'
      OR SupportsSapToErp <> 1
      OR SupportsErpToSap <> 0
      OR SupportsFull <> 1
      OR SupportsIncremental <> 0
      OR IsImplemented <> 1
      OR IsActive <> 1
  );

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.SapSyncHandlerCapabilities
    WHERE EntityCode = N'Warehouses'
)
BEGIN
    INSERT dbo.SapSyncHandlerCapabilities
    (
        EntityCode,
        DisplayName,
        SupportsSapToErp,
        SupportsErpToSap,
        SupportsFull,
        SupportsIncremental,
        IsImplemented,
        IsActive,
        CreatedByUserName
    )
    VALUES
    (
        N'Warehouses',
        N'Bodegas',
        1,
        0,
        1,
        0,
        1,
        1,
        N'Sistema'
    );
END;

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.MasterSchemaHistory
    WHERE Version = N'20260731.159'
)
BEGIN
    INSERT dbo.MasterSchemaHistory(Version, Description)
    VALUES
    (
        N'20260731.159',
        N'Registra Bodegas para sincronizacion SAP a ERP en modo Full'
    );
END;

COMMIT TRANSACTION;
GO
