/*
    Diagnostico de instalacion Sync Maestro-Sucursal.

    Ejecutar en NuanSystem_Master y en cada tenant/sucursal. No modifica datos.
*/

SET NOCOUNT ON;
GO

DECLARE @IsMaster bit = CASE WHEN DB_NAME() = N'NuanSystem_Master' THEN 1 ELSE 0 END;

SELECT
    DB_NAME() AS DatabaseName,
    CASE WHEN @IsMaster = 1 THEN N'Master' ELSE N'Tenant' END AS DatabaseRole;

IF @IsMaster = 1
BEGIN
    SELECT
        required.ObjectName,
        required.ObjectType,
        CASE WHEN OBJECT_ID(required.ObjectName, required.ObjectType) IS NULL THEN N'Missing' ELSE N'Ok' END AS Status
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
            (N'dbo.SyncOutbox', N'U'),
            (N'dbo.SyncOutboxTargets', N'U'),
            (N'dbo.SP_NA_GET_SYNCPROFILEPAGINAR', N'P'),
            (N'dbo.SP_NA_GET_SYNCCONFIGURATIONCOMPANYLOOKUPS', N'P'),
            (N'dbo.SP_NA_GET_SYNCPROFILEBUSCARPORID', N'P'),
            (N'dbo.SP_NA_POST_SYNCPROFILECREAR', N'P'),
            (N'dbo.SP_NA_PUT_SYNCPROFILEACTUALIZAR', N'P'),
            (N'dbo.SP_NA_CREATE_SYNCPROFILEEXECUTION', N'P')
    ) AS required(ObjectName, ObjectType)
    ORDER BY required.ObjectName;

    SELECT
        p.Code AS PermissionCode,
        CASE WHEN p.Id IS NULL THEN N'Missing' ELSE N'Ok' END AS Status
    FROM
    (
        VALUES
            (N'SYNC.CONFIGURATION.VIEW'),
            (N'SYNC.CONFIGURATION.CREATE'),
            (N'SYNC.CONFIGURATION.EDIT'),
            (N'SYNC.CONFIGURATION.DELETE'),
            (N'SYNC.CONFIGURATION.ACTIVATE'),
            (N'SYNC.CONFIGURATION.VALIDATE'),
            (N'SYNC.CONFIGURATION.EXECUTE'),
            (N'SYNC.CONFIGURATION.VIEWEXECUTIONS'),
            (N'SYNC.CONFIGURATION.CANCEL'),
            (N'SYNC.CONFIGURATION.RETRY')
    ) AS required(Code)
    LEFT JOIN dbo.Permissions AS p
        ON p.Code = required.Code;
END
ELSE
BEGIN
    SELECT
        required.ObjectName,
        required.ObjectType,
        CASE WHEN OBJECT_ID(required.ObjectName, required.ObjectType) IS NULL THEN N'Missing' ELSE N'Ok' END AS Status
    FROM
    (
        VALUES
            (N'dbo.SyncInbox', N'U'),
            (N'dbo.LocalOutbox', N'U'),
            (N'dbo.SyncAudit', N'U')
    ) AS required(ObjectName, ObjectType)
    ORDER BY required.ObjectName;
END;
GO
