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
            (N'dbo.SyncEntityDefinitions', N'U'),
            (N'dbo.SyncEntityDefinitionDependencies', N'U'),
            (N'dbo.AuditSyncConfigurationChanges', N'U'),
            (N'dbo.SyncSchedules', N'U'),
            (N'dbo.SyncProfileExecutions', N'U'),
            (N'dbo.SyncProfileExecutionDetails', N'U'),
            (N'dbo.SyncOutbox', N'U'),
            (N'dbo.SyncOutboxTargets', N'U'),
            (N'dbo.SyncDistributionSelections', N'U'),
            (N'dbo.SyncDistributionDecisionLog', N'U'),
            (N'dbo.SP_NA_GET_SYNCPROFILEPAGINAR', N'P'),
            (N'dbo.SP_NA_GET_SYNCCONFIGURATIONCOMPANYLOOKUPS', N'P'),
            (N'dbo.SP_NA_GET_SYNCPROFILEBUSCARPORID', N'P'),
            (N'dbo.SP_NA_POST_SYNCPROFILECREAR', N'P'),
            (N'dbo.SP_NA_PUT_SYNCPROFILEACTUALIZAR', N'P'),
            (N'dbo.SP_NA_GET_SYNCENTITYDEFINITIONPAGINAR', N'P'),
            (N'dbo.SP_NA_GET_SYNCENTITYDEFINITIONBUSCARPORID', N'P'),
            (N'dbo.SP_NA_POST_SYNCENTITYDEFINITIONCREAR', N'P'),
            (N'dbo.SP_NA_PUT_SYNCENTITYDEFINITIONACTUALIZAR', N'P'),
            (N'dbo.SP_NA_DELETE_SYNCENTITYDEFINITIONELIMINAR', N'P'),
            (N'dbo.SP_NA_GET_SYNCENTITYDEFINITIONHISTORIAL', N'P'),
            (N'dbo.SP_NA_CREATE_SYNCPROFILEEXECUTION', N'P')
            ,(N'dbo.SP_NA_GET_SYNCDISTRIBUTIONPOLICYBYMATRIXID', N'P')
            ,(N'dbo.SP_NA_PUT_SYNCDISTRIBUTIONPOLICYACTUALIZAR', N'P')
            ,(N'dbo.SP_NA_POST_SYNCDISTRIBUTIONDECISIONREGISTRAR', N'P')
            ,(N'dbo.SP_NA_GET_SYNCDISTRIBUTIONRULETARGETS', N'P')
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
            (N'SYNC.CONFIGURATION.RETRY'),
            (N'SYNC.ENTITIES.VIEW'),
            (N'SYNC.ENTITIES.CREATE'),
            (N'SYNC.ENTITIES.EDIT'),
            (N'SYNC.ENTITIES.DELETE')
    ) AS required(Code)
    LEFT JOIN dbo.Permissions AS p
        ON p.Code = required.Code;

    SELECT
        required.FormKey,
        CASE WHEN form.Id IS NULL THEN N'Missing' ELSE N'Ok' END AS Status
    FROM (VALUES (N'sync-entities')) AS required(FormKey)
    LEFT JOIN dbo.SecurityForms AS form
        ON form.FormKey = required.FormKey
       AND form.IsDeleted = 0;

    SELECT
        N'sync-entities' AS MenuFormKey,
        CASE WHEN menu.Id IS NULL THEN N'Missing' ELSE N'Ok' END AS Status
    FROM (VALUES (N'sync-entities')) AS required(FormKey)
    LEFT JOIN dbo.SecurityMenus AS menu
        ON menu.FormKey = required.FormKey
       AND menu.IsDeleted = 0;

    DECLARE @ProfileUpdateDefinition nvarchar(max) = OBJECT_DEFINITION(OBJECT_ID(N'dbo.SP_NA_PUT_SYNCPROFILEACTUALIZAR'));
    DECLARE @RoutingDefinition nvarchar(max) = OBJECT_DEFINITION(OBJECT_ID(N'dbo.SP_NA_GET_SYNCROUTINGTARGETS'));

    SELECT
        N'dbo.SP_NA_PUT_SYNCPROFILEACTUALIZAR.EntityCatalog' AS ObjectName,
        N'P' AS ObjectType,
        CASE
            WHEN @ProfileUpdateDefinition LIKE N'%dbo.SyncEntityDefinitions%'
                THEN N'Ok'
            ELSE N'Outdated'
        END AS Status;

    SELECT
        N'dbo.FK_SyncProfileEntities_EntityDefinition' AS ObjectName,
        N'F' AS ObjectType,
        CASE
            WHEN EXISTS
            (
                SELECT 1
                FROM sys.foreign_keys
                WHERE name = N'FK_SyncProfileEntities_EntityDefinition'
                  AND parent_object_id = OBJECT_ID(N'dbo.SyncProfileEntities')
                  AND is_disabled = 0
                  AND is_not_trusted = 0
            ) THEN N'Ok'
            ELSE N'MissingOrDisabled'
        END AS Status;

    SELECT
        N'dbo.SP_NA_GET_SYNCROUTINGTARGETS.TargetBranchFilter' AS ObjectName,
        N'P' AS ObjectType,
        CASE
            WHEN @RoutingDefinition LIKE N'%@RequireTargetBranchMatch%'
             AND @RoutingDefinition LIKE N'%branchCompany.BranchCode = @NormalizedTargetBranchCode%'
             AND @RoutingDefinition LIKE N'%matrix.DistributionMode%'
                THEN N'Ok'
            ELSE N'Outdated'
        END AS Status;
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
            (N'dbo.SyncAudit', N'U'),
            (N'dbo.Currencies', N'U'),
            (N'dbo.ItemFamilies', N'U'),
            (N'dbo.SP_NA_GET_CURRENCIES_LISTAR', N'P'),
            (N'dbo.SP_NA_GET_CURRENCIES_BUSCARPORID', N'P'),
            (N'dbo.SP_NA_POST_ITEM_FAMILY_SYNC_APPLY', N'P')
    ) AS required(ObjectName, ObjectType)
    ORDER BY required.ObjectName;
END;
GO
