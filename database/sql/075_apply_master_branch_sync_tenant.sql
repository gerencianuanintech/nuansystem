:ON ERROR EXIT
/*
    Ejecutar con SQLCMD en cada base tenant/sucursal que deba recibir eventos
    desde NuanSystem_Master.

    Objetivo:
    - Asegurar infraestructura local SyncInbox/LocalOutbox/SyncAudit.

    Ejemplo:
    sqlcmd -S <servidor> -U <usuario> -P <password> -d NuanSystem_DEMO -b -i 075_apply_master_branch_sync_tenant.sql
*/

SET NOCOUNT ON;
GO

IF DB_NAME() = N'NuanSystem_Master'
BEGIN
    THROW 51076, 'Este script debe ejecutarse en una base tenant/sucursal, no en NuanSystem_Master.', 1;
END;
GO

PRINT N'Aplicando 065_tenant_sync_inbox_local_outbox.sql';
:r 065_tenant_sync_inbox_local_outbox.sql

PRINT N'Aplicando 083_tenant_country_master_branch_sync.sql';
:r 083_tenant_country_master_branch_sync.sql

PRINT N'Aplicando 085_tenant_province_master_branch_sync.sql';
:r 085_tenant_province_master_branch_sync.sql

PRINT N'Aplicando 087_tenant_city_master_branch_sync.sql';
:r 087_tenant_city_master_branch_sync.sql

PRINT N'Aplicando 090_tenant_currency_master_branch_sync.sql';
:r 090_tenant_currency_master_branch_sync.sql

PRINT N'Aplicando 097_tenant_item_group_master_branch_sync.sql';
:r 097_tenant_item_group_master_branch_sync.sql

PRINT N'Aplicando 127_tenant_item_family_master_branch_sync.sql';
:r 127_tenant_item_family_master_branch_sync.sql

PRINT N'Validando objetos tenant Sync';
SET NOCOUNT ON;
GO

DECLARE @MissingObjects TABLE (ObjectName sysname NOT NULL);

INSERT INTO @MissingObjects (ObjectName)
SELECT missing.ObjectName
FROM
(
    VALUES
        (N'dbo.SyncInbox', N'U'),
        (N'dbo.LocalOutbox', N'U'),
        (N'dbo.SyncAudit', N'U'),
        (N'dbo.Countries', N'U'),
        (N'dbo.Provinces', N'U'),
        (N'dbo.Cities', N'U'),
        (N'dbo.Currencies', N'U'),
        (N'dbo.ItemGroups', N'U'),
        (N'dbo.SP_NA_POST_ITEM_GROUP_SYNC_APPLY', N'P'),
        (N'dbo.ItemFamilies', N'U'),
        (N'dbo.SP_NA_POST_ITEM_FAMILY_SYNC_APPLY', N'P')
) AS missing(ObjectName, ObjectType)
WHERE OBJECT_ID(missing.ObjectName, missing.ObjectType) IS NULL;

IF EXISTS (SELECT 1 FROM @MissingObjects)
BEGIN
    SELECT ObjectName AS MissingObject
    FROM @MissingObjects
    ORDER BY ObjectName;

    THROW 51077, 'Faltan objetos tenant requeridos para Sync Maestro-Sucursal.', 1;
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Countries_GlobalId'
      AND object_id = OBJECT_ID(N'dbo.Countries')
)
BEGIN
    THROW 51078, 'Countries no tiene el indice unico requerido para GlobalId.', 1;
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Provinces_GlobalId'
      AND object_id = OBJECT_ID(N'dbo.Provinces')
)
BEGIN
    THROW 51079, 'Provinces no tiene el indice unico requerido para GlobalId.', 1;
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Cities_GlobalId'
      AND object_id = OBJECT_ID(N'dbo.Cities')
)
BEGIN
    THROW 51080, 'Cities no tiene el indice unico requerido para GlobalId.', 1;
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Currencies_GlobalId'
      AND object_id = OBJECT_ID(N'dbo.Currencies')
)
BEGIN
    THROW 51081, 'Currencies no tiene el indice unico requerido para GlobalId.', 1;
END;

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'UX_ItemGroups_GlobalId'
      AND object_id = OBJECT_ID(N'dbo.ItemGroups')
)
BEGIN
    THROW 51082, 'ItemGroups no tiene el indice unico requerido para GlobalId.', 1;
END;

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'UX_ItemFamilies_GlobalId'
      AND object_id = OBJECT_ID(N'dbo.ItemFamilies')
)
BEGIN
    THROW 51083, 'ItemFamilies no tiene el indice unico requerido para GlobalId.', 1;
END;

PRINT N'Infraestructura tenant Sync aplicada correctamente.';
GO
