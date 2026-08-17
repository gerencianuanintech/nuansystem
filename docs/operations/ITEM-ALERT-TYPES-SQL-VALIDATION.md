# Tipos de alerta de artículos — evidencia SQL

Fecha: 2026-08-14 16:42 America/Guayaquil.

## Alcance autorizado

- `221_tenant_item_alert_types_master.sql` en todos los tenants SQL Server activos registrados en Master.
- `222_master_definitions_inventory_item_alert_types_navigation.sql` en `NuanSystem_Master`.
- No se activaron sincronización, SAP, SRI ni workers.

## Destinos

- `NuanSystem_Master`.
- `NuanSystem_DEMO`.
- `NuanSystem_DEMO_REMIGIO`.
- `NuanSystem_DEMO_CANARIS`.
- `NuanSystem_SYNC_WH_BRANCH_TEST`.

## Recuperación

Antes de las migraciones se crearon respaldos `COPY_ONLY WITH CHECKSUM` y se validaron con `RESTORE VERIFYONLY`:

- `/var/opt/mssql/data/NuanSystem_Master_ItemAlertTypes221222_20260814_164249.bak`.
- `/var/opt/mssql/data/NuanSystem_DEMO_ItemAlertTypes221222_20260814_164249.bak`.
- `/var/opt/mssql/data/NuanSystem_DEMO_REMIGIO_ItemAlertTypes221222_20260814_164249.bak`.
- `/var/opt/mssql/data/NuanSystem_DEMO_CANARIS_ItemAlertTypes221222_20260814_164249.bak`.
- `/var/opt/mssql/data/NuanSystem_SYNC_WH_BRANCH_TEST_ItemAlertTypes221222_20260814_164249.bak`.

## Resultados

- El validador estático de lotes SQL aprobó ambos scripts.
- `221` se ejecutó dos veces sin error en los cuatro tenants.
- `222` se ejecutó dos veces sin error en Master.
- Cada tenant conserva una sola fila `20260814.221`, 17 columnas, 8 procedimientos, 3 índices únicos contando la PK y 3 restricciones `CHECK`.
- Master conserva una sola fila `20260814.222`, 2 permisos activos y sus 2 concesiones a `ADMIN`.
- El formulario y menú `item-alert-types` están activos.
- Existen 12 operaciones aplicables y 12 concesiones de operación para `ADMIN`.
- Una prueba CRUD dentro de una transacción aprobó create/update/delete y produjo 9 filas de auditoría por tenant; el `ROLLBACK` dejó cero fixtures.
- El lookup se ejecutó correctamente aun sin registros.

## Pendiente de validación de aplicación

La autorización runtime con los permisos nuevos requiere cerrar sesión e ingresar nuevamente para emitir un JWT actualizado. No se usaron ni registraron credenciales de usuario en esta evidencia.
