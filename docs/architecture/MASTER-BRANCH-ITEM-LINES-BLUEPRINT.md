# Líneas de artículos — maestro y sincronización Matriz–Sucursal

## Decisión de producto

`ItemLine` es un maestro tenant independiente de Definiciones > Inventario.
Su contrato funcional contiene únicamente:

- `Code`;
- `Name`;
- `Description` opcional;
- `SortOrder`;
- `IsActive`.

`Id`, `GlobalId`, auditoría y eliminación lógica son campos técnicos. El
maestro no incorpora `IsSystem`, SAP, sistema externo ni códigos externos.

## Propiedad y relación con Artículos

La fuente de verdad de una mutación es la base tenant que contiene el maestro.
La identidad distribuible es `GlobalId`; un código nunca permite adoptar una
identidad distinta.

El contrato actual de `Item` conserva `Line` como texto y su payload de
sincronización no contiene `ItemLineGlobalId`. Por ello:

- `ItemLine` no depende de otra entidad;
- no se registra todavía la dependencia `Item -> ItemLine`;
- la futura normalización de Artículos debe ser una migración independiente,
  con backfill explícito y payload versionado.

## Contratos SQL

Orden forward-only:

1. `201_tenant_item_lines_master.sql` — tabla existente de `044`, `GlobalId`,
   `SortOrder`, restricciones, índices, CRUD, historial, Full y apply.
2. `202_master_definitions_inventory_item_lines_navigation.sql` — navegación
   y seguridad en Configuración > Definiciones > Inventario, conservando las
   identidades legacy y usando `FormKey=item-lines`.
3. `203_master_item_lines_sync_registration.sql` — definición `ItemLine`,
   configuración y ownership deshabilitados por defecto.

Prerrequisitos tenant: `044`, `065`, `106` y `SchemaHistory`. Prerrequisitos
Master: seguridad y jerarquía de Definiciones > Inventario de `185`, además del
catálogo Matriz–Sucursal.

## Flujo incremental

```text
CRUD tenant
  -> transacción tenant
       -> mutación ItemLines
       -> AuditCatalogChanges
       -> LocalOutbox (EntityName=ItemLine)
  -> commit

MasterBranchSyncWorker (deshabilitado por defecto)
  -> promoción idempotente con el mismo EventId
  -> SyncOutbox y targets
  -> apply en sucursal por GlobalId
  -> SyncInbox y auditoría
```

La publicación local no abre Master, SAP, SRI ni HTTP dentro de la transacción.
El apply resuelve por `GlobalId`; una colisión de `Code` con otro `GlobalId` es
terminal y no realiza adopción automática. Los tombstones conservan código e
identidad para impedir reutilización ambigua.

## Modo Full

`SP_NA_GET_ITEM_LINE_SYNC_FULL` pagina por `Id` y entrega filas activas,
inactivas y eliminadas. El payload contiene `GlobalId`, campos funcionales,
estado lógico y fecha efectiva. Full no activa perfiles ni workers.

## Seguridad y navegación

- API: `GENERALINVENTORY.ITEMLINES.READ` y
  `GENERALINVENTORY.ITEMLINES.MANAGE`.
- Ruta: `/api/definitions/inventory/item-lines`.
- Formulario: `item-lines`.
- Menú: `MENU.DEFINITIONS.INVENTORY.ITEMLINES`.

La migración conserva el `Id` del formulario y menú legacy cuando existe, así
como los accesos explícitos de roles. Solo completa las operaciones corporativas
del rol `ADMIN`; no sobrescribe operaciones configuradas para otros roles.

## Activación y despliegue

Los scripts son idempotentes y no crean semillas nuevas. La configuración
`ItemLine` y el ownership nacen con `IsEnabled=0`. La implementación no autoriza:

- desplegar `201`, `202` o `203`;
- habilitar perfiles, relay o worker;
- ejecutar SAP, SRI o pruebas runtime contra bases reales.

Despliegue y validación SQL requieren autorización independiente, bases
identificadas y respaldo verificado. El orden de recuperación es detener antes
de la migración que falle, conservar la historia ya confirmada y corregir con
una migración forward-only; no se reescribe un script ya desplegado.

## Validación requerida antes de activar

- dos pases por script en una base autorizada;
- preservación de filas, `GlobalId`, auditoría y accesos legacy;
- CRUD y `LocalOutbox` en la misma transacción;
- Full paginado;
- apply idempotente por `GlobalId`;
- colisión terminal por código y reserva de tombstone;
- allowlist vacía sin claim ni mutación;
- ausencia de llamadas SAP/SRI;
- restauración final de toda configuración temporal.
