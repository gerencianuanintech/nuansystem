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

PRINT N'Aplicando 079_sync_profile_entity_catalog_alignment.sql';
:r 079_sync_profile_entity_catalog_alignment.sql

PRINT N'Aplicando 080_sync_entity_definitions.sql';
:r 080_sync_entity_definitions.sql

PRINT N'Aplicando 081_sync_entity_definition_api_security.sql';
:r 081_sync_entity_definition_api_security.sql

PRINT N'Aplicando 082_sync_entity_definition_winforms.sql';
:r 082_sync_entity_definition_winforms.sql

PRINT N'Aplicando 084_master_country_sync_registration.sql';
:r 084_master_country_sync_registration.sql

PRINT N'Aplicando 086_master_province_sync_registration.sql';
:r 086_master_province_sync_registration.sql

PRINT N'Aplicando 088_master_city_sync_registration.sql';
:r 088_master_city_sync_registration.sql

PRINT N'Aplicando 091_master_currency_sync_registration.sql';
:r 091_master_currency_sync_registration.sql

PRINT N'Aplicando 092_sync_routing_by_target_branch.sql';
:r 092_sync_routing_by_target_branch.sql

PRINT N'Aplicando 093_sync_distribution_policies.sql';
:r 093_sync_distribution_policies.sql

PRINT N'Aplicando 098_master_item_group_sync_registration.sql';
:r 098_master_item_group_sync_registration.sql

PRINT N'Aplicando 128_master_item_family_sync_registration.sql';
:r 128_master_item_family_sync_registration.sql

PRINT N'Aplicando 099_master_sync_dependency_engine.sql';
:r 099_master_sync_dependency_engine.sql

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
        (N'dbo.SyncEntityDefinitions', N'U'),
        (N'dbo.SyncEntityDefinitionDependencies', N'U'),
        (N'dbo.AuditSyncConfigurationChanges', N'U'),
        (N'dbo.SyncSchedules', N'U'),
        (N'dbo.SyncProfileExecutions', N'U'),
        (N'dbo.SyncProfileExecutionDetails', N'U'),
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
        (N'dbo.SP_NA_CREATE_SYNCPROFILEEXECUTION', N'P')
        ,(N'dbo.SP_NA_GET_SYNCDISTRIBUTIONPOLICYBYMATRIXID', N'P')
        ,(N'dbo.SP_NA_PUT_SYNCDISTRIBUTIONPOLICYACTUALIZAR', N'P')
        ,(N'dbo.SP_NA_POST_SYNCDISTRIBUTIONDECISIONREGISTRAR', N'P')
        ,(N'dbo.SP_NA_GET_SYNCDISTRIBUTIONRULETARGETS', N'P')
) AS missing(ObjectName, ObjectType)
WHERE OBJECT_ID(missing.ObjectName, missing.ObjectType) IS NULL;

IF EXISTS (SELECT 1 FROM @MissingObjects)
BEGIN
    SELECT ObjectName AS MissingObject
    FROM @MissingObjects
    ORDER BY ObjectName;

    THROW 51075, 'Faltan objetos requeridos del modulo Sync Maestro-Sucursal.', 1;
END;

DECLARE @ProfileUpdateDefinition nvarchar(max) = OBJECT_DEFINITION(OBJECT_ID(N'dbo.SP_NA_PUT_SYNCPROFILEACTUALIZAR'));
DECLARE @RoutingDefinition nvarchar(max) = OBJECT_DEFINITION(OBJECT_ID(N'dbo.SP_NA_GET_SYNCROUTINGTARGETS'));

IF @ProfileUpdateDefinition NOT LIKE N'%dbo.SyncEntityDefinitions%'
BEGIN
    THROW 51076, 'SP_NA_PUT_SYNCPROFILEACTUALIZAR no valida contra el catalogo administrable de entidades.', 1;
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_SyncProfileEntities_EntityDefinition'
      AND parent_object_id = OBJECT_ID(N'dbo.SyncProfileEntities')
      AND is_disabled = 0
      AND is_not_trusted = 0
)
BEGIN
    THROW 51077, 'SyncProfileEntities no esta relacionado correctamente con SyncEntityDefinitions.', 1;
END;

IF @RoutingDefinition NOT LIKE N'%@RequireTargetBranchMatch%'
   OR @RoutingDefinition NOT LIKE N'%branchCompany.BranchCode = @NormalizedTargetBranchCode%'
   OR @RoutingDefinition NOT LIKE N'%matrix.DistributionMode%'
BEGIN
    THROW 51078, 'SP_NA_GET_SYNCROUTINGTARGETS no contiene el filtro por sucursal destino.', 1;
END;

PRINT N'Modulo Sync Maestro-Sucursal aplicado correctamente en Master.';
GO
