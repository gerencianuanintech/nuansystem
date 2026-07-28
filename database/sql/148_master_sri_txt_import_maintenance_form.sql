/*
    Migracion 148 - Clasificacion de mantenimiento de SRI TXT Import.

    Corrige exclusivamente el tipo del formulario sri-txt-imports para que sea
    administrable desde Accesos a Formularios de Mantenimiento. No concede menus, operaciones
    ni permisos API adicionales.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME() <> N'NuanSystem_Master'
    THROW 51148, 'Migration 148 must run only in NuanSystem_Master.', 1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NULL
    THROW 51148, 'MasterSchemaHistory is required before migration 148.', 1;
IF OBJECT_ID(N'dbo.SecurityForms', N'U') IS NULL
    THROW 51148, 'SecurityForms is required before migration 148.', 1;
GO

BEGIN TRY
BEGIN TRANSACTION;

DECLARE @FormId int =
(
    SELECT TOP (1) Id
    FROM dbo.SecurityForms
    WHERE FormKey = N'sri-txt-imports'
      AND IsDeleted = 0
);

IF @FormId IS NULL
    THROW 51148, 'The sri-txt-imports form is required before migration 148.', 1;

UPDATE dbo.SecurityForms
SET FormType = 1,
    UpdatedByUserName = N'Sistema',
    UpdatedAt = SYSUTCDATETIME()
WHERE Id = @FormId
  AND FormType <> 1;

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.MasterSchemaHistory
    WHERE Version = N'20260728.148'
)
BEGIN
    INSERT dbo.MasterSchemaHistory(Version, Description)
    VALUES
    (
        N'20260728.148',
        N'Clasifica Importaciones TXT SRI como formulario de mantenimiento'
    );
END;

UPDATE dbo.MasterSchemaHistory
SET Description = N'Clasifica Importaciones TXT SRI como formulario de mantenimiento'
WHERE Version = N'20260728.148'
  AND Description <> N'Clasifica Importaciones TXT SRI como formulario de mantenimiento';

COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
