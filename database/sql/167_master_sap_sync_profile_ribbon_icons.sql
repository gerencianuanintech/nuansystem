/*
    Migracion 167 - Iconos Ribbon para acciones de Perfiles SAP.

    Target: NuanSystem_Master.
    Prerequisite: 160_master_sap_sync_winforms_navigation.sql.

    Asigna recursos SVG corporativos a Validar, Activar y Desactivar. No cambia
    permisos, grants, menus, formularios ni comportamiento funcional.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME() <> N'NuanSystem_Master'
    THROW 51167, 'Migration 167 must run only in NuanSystem_Master.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL
   OR OBJECT_ID(N'dbo.SecurityOperations', N'U') IS NULL
    THROW 51167, 'Security operations schema is required before migration 167.', 1;
IF NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260731.160')
    THROW 51167, 'Migration 160 is required before migration 167.', 1;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @Icons table
    (
        Code nvarchar(80) PRIMARY KEY,
        IconLarge nvarchar(260) NOT NULL,
        IconSmall nvarchar(260) NOT NULL
    );

    INSERT @Icons(Code, IconLarge, IconSmall)
    VALUES
        (N'ACTION.SAP_SYNC_PROFILES.VALIDATE', N'Ribbon/validar_cuadro_check_32.svg', N'Ribbon/validar_cuadro_check_16.svg'),
        (N'ACTION.SAP_SYNC_PROFILES.ACTIVATE', N'Ribbon/activar_toggle_on_32.svg', N'Ribbon/activar_toggle_on_16.svg'),
        (N'ACTION.SAP_SYNC_PROFILES.DEACTIVATE', N'Ribbon/desactivar_toggle_off_32.svg', N'Ribbon/desactivar_toggle_off_16.svg');

    IF EXISTS
    (
        SELECT 1
        FROM @Icons source
        LEFT JOIN dbo.SecurityOperations target
            ON target.Code = source.Code
           AND target.IsActive = 1
           AND target.IsDeleted = 0
        WHERE target.Id IS NULL
    )
        THROW 51167, 'A required SAP profile operation is missing.', 1;

    UPDATE target
    SET IconLarge = source.IconLarge,
        IconSmall = source.IconSmall,
        UpdatedByUserName = N'Sistema',
        UpdatedAt = SYSUTCDATETIME()
    FROM dbo.SecurityOperations target
    INNER JOIN @Icons source ON source.Code = target.Code;

    IF NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260804.167')
        INSERT dbo.MasterSchemaHistory(Version, Description)
        VALUES (N'20260804.167', N'Iconos Ribbon para acciones de Perfiles SAP');

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
