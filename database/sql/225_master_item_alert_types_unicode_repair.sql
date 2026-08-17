/*
    Migration 225 - Repairs ItemAlertTypes navigation Unicode text.

    Target: NuanSystem_Master only.
    Prerequisite: 222.
    Uses NCHAR for the accented character so sqlcmd input encoding cannot
    transform the label into mojibake during manual deployments.
*/
USE [NuanSystem_Master];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME()<>N'NuanSystem_Master'
    THROW 51225,'Migration 225 must run only in NuanSystem_Master.',1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory',N'U') IS NULL
    OR OBJECT_ID(N'dbo.SecurityForms',N'U') IS NULL
    OR OBJECT_ID(N'dbo.SecurityMenus',N'U') IS NULL
    OR OBJECT_ID(N'dbo.Permissions',N'U') IS NULL
    THROW 51225,'Master navigation tables are required.',1;
IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260814.222')
    THROW 51225,'Migration 222 is required before migration 225.',1;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @Name nvarchar(160)=N'Tipos de alerta de art'+NCHAR(237)+N'culos';
    DECLARE @Description nvarchar(300)=N'Mantenimiento de '+@Name;

    UPDATE dbo.SecurityForms
    SET Name=@Name,Description=@Description,UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
    WHERE FormKey=N'item-alert-types'
       OR Code=N'FORM.DEFINITIONS.INVENTORY.ItemAlertTypes';

    UPDATE dbo.SecurityMenus
    SET Name=@Name,Description=@Description,UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
    WHERE FormKey=N'item-alert-types'
       OR Code=N'MENU.DEFINITIONS.INVENTORY.ITEMALERTTYPES';

    UPDATE dbo.Permissions
    SET Name=CASE Code
            WHEN N'GENERALINVENTORY.ITEMALERTTYPES.READ' THEN N'Ver '+@Name
            ELSE N'Gestionar '+@Name
        END,
        Description=CASE Code
            WHEN N'GENERALINVENTORY.ITEMALERTTYPES.READ' THEN N'Ver '+@Name
            ELSE N'Gestionar '+@Name
        END,
        UpdatedAt=SYSUTCDATETIME()
    WHERE Code IN(
        N'GENERALINVENTORY.ITEMALERTTYPES.READ',
        N'GENERALINVENTORY.ITEMALERTTYPES.MANAGE');

    IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260816.225')
        INSERT dbo.MasterSchemaHistory(Version,Description)
        VALUES(N'20260816.225',N'Repairs ItemAlertTypes navigation Unicode text');

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
