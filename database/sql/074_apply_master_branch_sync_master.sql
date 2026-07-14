:ON ERROR EXIT
/*
    Ejecutar con SQLCMD desde la carpeta database/sql.

    Objetivo:
    - Llevar NuanSystem_Master al estado requerido por el modulo administrativo
      de sincronizacion Maestro-Sucursal.
    - Incluye configuracion, routing, ejecuciones, seguridad WinForms y hardening.

    Ejemplo:
    sqlcmd -S <servidor> -U <usuario> -P <password> -d NuanSystem_Master -b -i 074_apply_master_branch_sync_master.sql
*/

SET NOCOUNT ON;
GO

IF DB_NAME() <> N'NuanSystem_Master'
BEGIN
    THROW 51074, 'Este script debe ejecutarse en NuanSystem_Master.', 1;
END;
GO

PRINT N'Aplicando 069_sync_master_branch_configuration.sql';
:r 069_sync_master_branch_configuration.sql

PRINT N'Aplicando 070_sync_master_branch_routing.sql';
:r 070_sync_master_branch_routing.sql

PRINT N'Aplicando 071_sync_profile_execution.sql';
:r 071_sync_profile_execution.sql

PRINT N'Aplicando 072_sync_configuration_winforms_security.sql';
:r 072_sync_configuration_winforms_security.sql

PRINT N'Aplicando 073_sync_master_branch_hardening.sql';
:r 073_sync_master_branch_hardening.sql

PRINT N'Validando objetos Master Sync';
SET NOCOUNT ON;
GO

DECLARE @MissingObjects TABLE (ObjectName sysname NOT NULL);

INSERT INTO @MissingObjects (ObjectName)
SELECT missing.ObjectName
FROM
(
    VALUES
        (N'dbo.SyncProfiles', N'U'),
        (N'dbo.SyncProfileBranches', N'U'),
        (N'dbo.SyncProfileEntities', N'U'),
        (N'dbo.SyncProfileEntityBranches', N'U'),
        (N'dbo.SyncSchedules', N'U'),
        (N'dbo.SyncProfileExecutions', N'U'),
        (N'dbo.SyncProfileExecutionDetails', N'U'),
        (N'dbo.SP_NA_GET_SYNCPROFILEPAGINAR', N'P'),
        (N'dbo.SP_NA_GET_SYNCCONFIGURATIONCOMPANYLOOKUPS', N'P'),
        (N'dbo.SP_NA_GET_SYNCPROFILEBUSCARPORID', N'P'),
        (N'dbo.SP_NA_POST_SYNCPROFILECREAR', N'P'),
        (N'dbo.SP_NA_PUT_SYNCPROFILEACTUALIZAR', N'P'),
        (N'dbo.SP_NA_CREATE_SYNCPROFILEEXECUTION', N'P')
) AS missing(ObjectName, ObjectType)
WHERE OBJECT_ID(missing.ObjectName, missing.ObjectType) IS NULL;

IF EXISTS (SELECT 1 FROM @MissingObjects)
BEGIN
    SELECT ObjectName AS MissingObject
    FROM @MissingObjects
    ORDER BY ObjectName;

    THROW 51075, 'Faltan objetos requeridos del modulo Sync Maestro-Sucursal.', 1;
END;

PRINT N'Modulo Sync Maestro-Sucursal aplicado correctamente en Master.';
GO
