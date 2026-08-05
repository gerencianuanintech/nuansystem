/*
    Registra Países como capacidad SAP -> ERP Full.
    Ejecutar solo en NuanSystem_Master después de 152 y 157.
    No crea perfiles, agendas ni activa ejecuciones.
*/
USE [NuanSystem_Master];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME() <> N'NuanSystem_Master'
    THROW 51170, 'Migration 170 must run only in NuanSystem_Master.', 1;
IF OBJECT_ID(N'dbo.SapSyncHandlerCapabilities', N'U') IS NULL
    THROW 51170, 'Migration 152 is required before migration 170.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL
    THROW 51170, 'MasterSchemaHistory is required before migration 170.', 1;
GO

BEGIN TRANSACTION;

UPDATE dbo.SapSyncHandlerCapabilities
SET DisplayName = N'Países',
    SupportsSapToErp = 1,
    SupportsErpToSap = 0,
    SupportsFull = 1,
    SupportsIncremental = 0,
    IsImplemented = 1,
    IsActive = 1,
    UpdatedByUserName = N'Sistema',
    UpdatedAt = SYSUTCDATETIME()
WHERE EntityCode = N'Countries';

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.SapSyncHandlerCapabilities
    WHERE EntityCode = N'Countries'
)
BEGIN
    INSERT dbo.SapSyncHandlerCapabilities
    (
        EntityCode, DisplayName,
        SupportsSapToErp, SupportsErpToSap,
        SupportsFull, SupportsIncremental,
        IsImplemented, IsActive,
        CreatedByUserName
    )
    VALUES
    (
        N'Countries', N'Países',
        1, 0,
        1, 0,
        1, 1,
        N'Sistema'
    );
END;

IF NOT EXISTS
(
    SELECT 1 FROM dbo.MasterSchemaHistory
    WHERE Version = N'20260804.170'
)
BEGIN
    INSERT dbo.MasterSchemaHistory(Version, Description)
    VALUES(N'20260804.170', N'Registra Países para sincronización SAP a ERP Full sin filtros');
END;

COMMIT TRANSACTION;
GO
