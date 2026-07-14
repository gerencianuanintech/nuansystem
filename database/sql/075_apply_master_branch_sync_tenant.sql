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
        (N'dbo.SyncAudit', N'U')
) AS missing(ObjectName, ObjectType)
WHERE OBJECT_ID(missing.ObjectName, missing.ObjectType) IS NULL;

IF EXISTS (SELECT 1 FROM @MissingObjects)
BEGIN
    SELECT ObjectName AS MissingObject
    FROM @MissingObjects
    ORDER BY ObjectName;

    THROW 51077, 'Faltan objetos tenant requeridos para Sync Maestro-Sucursal.', 1;
END;

PRINT N'Infraestructura tenant Sync aplicada correctamente.';
GO
