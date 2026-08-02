# Operacion Sync Master/Sucursal

Este documento describe la operacion diaria de la sincronizacion Master/Sucursal. Complementa `docs/architecture/MASTER-BRANCH-STANDALONE-SAP.md`; no reemplaza la arquitectura rectora ni habilita nuevas entidades.

## Proposito

Sync Master/Sucursal sincroniza datos entre `NuanSystem_Master` y bases tenant/sucursal usando Outbox/Inbox, `GlobalId`, idempotencia por `EventId` y auditoria. La sincronizacion es independiente de SAP Business One y debe funcionar para clientes Standalone, SapIntegrated e Hybrid.

Reglas base:

- `Id` sigue siendo local de cada base.
- `GlobalId` identifica la misma entidad entre Master y sucursal.
- `SapCode` y referencias externas son opcionales.
- SAP no participa en Sync Master/Sucursal.
- WinForms consulta la API; no se conecta directo a bases.

## Componentes

| Componente | Ubicacion | Proposito |
|---|---|---|
| `SyncOutbox` | Master | Registra eventos publicados desde entidades replicables. |
| `SyncOutboxTargets` | Master | Declara sucursales destino y estado por target. |
| `SyncInbox` | Sucursal | Garantiza idempotencia por `EventId` antes de aplicar. |
| `LocalOutbox` | Tenant | Intencion durable que comparte transaccion con el maestro y luego es promovida idempotentemente a Master. |
| `SyncAudit` | Master/Sucursal | Registra cambios de estado, acciones tecnicas y acciones manuales. |
| `NuanSystem.MasterBranchSyncWorker` | Worker backend | Reclama eventos, consulta targets y aplica entidades cuando corresponde. |
| `ISyncEventPublisher` | Application | Publicador directo legado para entidades aun no migradas. |
| Writers `LocalOutbox` | Application/Persistence | Construyen la intencion local dentro de la misma transaccion tenant. |
| Relay `LocalOutbox` | Backend worker | Promueve por `EventId` hacia `SyncOutbox` sin transaccion distribuida. |
| Aplicador `BusinessPartner` | Worker/Application/Persistence | Aplica terceros por `GlobalId`. |
| Aplicador `Item` | Worker/Application/Persistence | Aplica maestro de articulo por `GlobalId`. |
| Aplicador `Warehouse` | Worker/Application/Persistence | Aplica maestro de bodega por `GlobalId`. |

## Separacion SAP y SRI

Sync Master/Sucursal no invoca SAP Business One, no usa `NuanSystem.SyncWorker` y no depende de `SapSyncLog`. El worker `NuanSystem.SyncWorker` corresponde a sincronizaciones SAP previas; Master/Sucursal usa `NuanSystem.MasterBranchSyncWorker`.

Sync Master/Sucursal tampoco procesa SRI, no descarga XML y no procesa documentos electronicos. El modulo SRI y su Worker Service son componentes independientes.

## Modos del Worker

### `SkeletonMode=true` + `ObserveOnly`

Modo seguro por defecto.

- No reclama eventos.
- No cambia estados.
- No marca `Ignored`.
- No consume eventos productivos.
- No aplica entidades.
- Solo registra log tecnico si corresponde.
- Recomendado para despliegue inicial y validacion de configuracion.

### `SkeletonMode=true` + `ClaimAndRelease`

Dry-run tecnico.

- Reclama eventos.
- No aplica entidades reales.
- Libera el lock.
- Devuelve el evento a `Pending`.
- Puede registrar auditoria `DryRun`.
- Puede generar ruido operativo/auditoria de prueba.
- No debe dejarse corriendo continuamente sin monitoreo.

### `SkeletonMode=true` + `ClaimAndIgnore`

Comportamiento explicito y excepcional.

- Reclama eventos.
- Marca eventos como `Ignored`.
- No aplica entidades reales.
- No recomendado para produccion.
- Usar solo con intencion operativa clara y auditoria revisada.

### `SkeletonMode=false`

Modo real.

- Reclama eventos pendientes o reprocesables.
- Aplica entidades habilitadas en `EnabledEntityAppliers`.
- Usa `SyncInbox` para idempotencia.
- Debe activarse solo en piloto controlado, por sucursal y entidad.

## Entidades soportadas actualmente

| Entidad | `EntityName` | Identidad | Operaciones | Restricciones |
|---|---|---|---|---|
| BusinessPartner | `BusinessPartner` | `GlobalId` | Create, update, disable/delete logico | `Code` es referencia funcional. `SapCardCode` no es identidad, `BusinessPartnerSapMapping` no participa y SAP no es obligatorio. |
| Item | `Item` | `GlobalId` | Create, update, disable/delete logico del maestro | No sincroniza stock, precios, costos, kardex, movimientos de inventario, lotes, series, vencimientos ni bodegas. `SapCode` es nullable, opcional y no es identidad. |
| ItemGroup | `ItemGroups` | `GlobalId` | Create, update, disable/delete logico | Migraciones 129/130 y piloto DEMO a Remigio validados; colision por codigo terminal, sin adopcion. Configuracion permanente y worker continúan deshabilitados. |
| ItemFamily | `ItemFamilies` | `GlobalId` | Create, update, disable/delete logico | Depende de ItemGroup; piloto DEMO a Remigio validado. |
| UnitOfMeasure | `UnitOfMeasure` | `GlobalId` | Create, update, disable/delete logico | Piloto DEMO a Remigio validado junto con Item payload v2; colision por codigo terminal, sin adopcion. |
| Warehouse | `Warehouse` | `GlobalId` | Create, update, disable/delete logico del maestro | SQL y piloto DEMO a Remigio validados. No sincroniza stock, saldos, costos ni kardex. |
| Carrier | `Carrier` | `GlobalId` | Create, update, disable/delete logico | Mantenimiento Transportistas independiente; SQL y piloto DEMO a Remigio validados, con colision terminal sin adopcion. |

## Flujo Operativo

1. Un registro replicable se crea, actualiza o desactiva en el tenant Matriz.
2. Para entidades migradas, el caso de uso persiste maestro y `LocalOutbox` en una sola transaccion tenant.
3. El relay promueve el mismo `EventId` a `SyncOutbox`; entidades legadas aun pueden usar publicacion directa.
4. Las reglas de distribucion crean o mantienen targets en `SyncOutboxTargets` dentro del commit Master.
5. El worker, segun su modo, observa, reclama/libera o procesa el evento.
6. En modo real, por cada target se registra `SyncInbox` en la sucursal.
7. El aplicador usa `GlobalId` para upsert o desactivacion idempotente.
8. `SyncOutbox`, targets, `SyncInbox` y `SyncAudit` registran resultado operativo.

## Estados

| Estado | Significado | Lo reclama el worker | Retry manual | Accion manual | Auditoria esperada |
|---|---|---:|---:|---|---|
| `Pending` | Evento pendiente de procesamiento. | Si, salvo `ObserveOnly`. | No. | Ninguna. | Creacion/retorno a pendiente cuando aplique. |
| `InProcess` | Evento reclamado con lock tecnico. | No mientras el lock este vigente. | No. | `release-expired-lock` solo si el lock vencio. | `Claimed`. |
| `Applied` | Evento aplicado correctamente o cerrado con exito operativo. | No. | No. | Ninguna. | `Applied`. |
| `Error` | Fallo reprocesable. | Si cumple ventana de reintento. | Si. | `retry` devuelve a `Pending`; `release-expired-lock` si tiene lock vencido. | `Failed` y `Retried` si hay accion manual. |
| `DeadLetter` | Fallo definitivo o MaxAttempts agotado. | No. | Si, con motivo. | `retry-deadletter` devuelve a `Pending`. | `DeadLetter` y `RetriedFromDeadLetter` si hay accion manual. |
| `Ignored` | Evento ignorado por regla o modo explicito. | No. | No. | Ninguna en esta fase. | `Ignored`. |

## Acciones Manuales Disponibles

| Accion | Endpoint | Permiso | Regla |
|---|---|---|---|
| Retry Error -> Pending | `POST /api/sync/outbox/{id}/retry` | `SYNC.OUTBOX.RETRY` | Solo eventos `Error`. |
| Retry DeadLetter -> Pending | `POST /api/sync/outbox/{id}/retry-deadletter` | `SYNC.OUTBOX.RETRY_DEADLETTER` | Solo `DeadLetter`; motivo obligatorio. |
| Release expired lock | `POST /api/sync/outbox/{id}/release-expired-lock` | `SYNC.OUTBOX.RELEASE_LOCK` | Solo locks vencidos en `InProcess` o `Error`. |

Restricciones:

- No retry de `Applied`.
- No retry de `Pending`.
- No release de lock vigente.
- No edicion manual de `PayloadJson`.
- No edicion de `EntityGlobalId`.
- No cambio manual de `EntityName`.
- No ejecucion directa del worker desde API.
- No endpoints `apply`, `run`, `process`, `dispatch`, `claim`, `sync-now` ni `reprocess`.

## Permisos

- `SYNC.OUTBOX.VIEW`: consulta dashboard, summary, outbox, detalle y targets.
- `SYNC.AUDIT.VIEW`: consulta auditoria Sync.
- `SYNC.OUTBOX.RETRY`: reintento manual desde `Error`.
- `SYNC.OUTBOX.RETRY_DEADLETTER`: reintento manual desde `DeadLetter` con motivo.
- `SYNC.OUTBOX.RELEASE_LOCK`: liberacion de locks vencidos.

## Monitoreo

Endpoints disponibles:

- `GET /api/sync/dashboard`
- `GET /api/sync/summary`
- `GET /api/sync/outbox`
- `GET /api/sync/outbox/{id}`
- `GET /api/sync/outbox/{id}/targets`
- `GET /api/sync/audit`

Filtros operativos recomendados:

- `Status`
- `EntityName`
- `EventId`
- `EntityGlobalId`
- `BranchCompanyId`
- `CreatedFrom`
- `CreatedTo`
- `DeadLetterOnly`
- `HasErrors`

El dashboard y listados no deben cargar `PayloadJson` masivo. El detalle puede mostrar payload para diagnostico autorizado.

### Monitor WinForms

El cliente WinForms expone `Administracion > Sincronizacion > Monitor Sync` con `FormKey = sync-monitor`.

- La pantalla consume endpoints `GET` para monitoreo y solo los tres `POST` manuales controlados para acciones autorizadas.
- El dashboard muestra totales por estado, resumen por entidad/sucursal y ultimos eventos/errores.
- El listado de `SyncOutbox` permite filtrar por estado, entidad, fechas, errores y `DeadLetter`.
- El detalle puede mostrar `PayloadJson`, targets y auditoria asociada si el usuario tiene permiso.
- Sin `SYNC.AUDIT.VIEW`, la pestana y acciones de auditoria no se muestran.
- `SYNC.OUTBOX.RETRY` muestra reintento solo para eventos `Error`.
- `SYNC.OUTBOX.RETRY_DEADLETTER` muestra reintento DeadLetter solo para `DeadLetter` y exige motivo obligatorio.
- `SYNC.OUTBOX.RELEASE_LOCK` muestra liberacion solo para `InProcess` o `Error` con `LockExpiresAt` vencido.
- Luego de una accion exitosa, el detalle, targets, auditoria, listado y dashboard se refrescan.
- La pantalla no expone apply, run, process, dispatch, claim, sync-now, reprocess ni ejecucion directa del worker.
- La pantalla no permite editar `PayloadJson`, `EntityGlobalId` ni `EntityName`.

## Reglas de Seguridad

- No activar `SkeletonMode=false` sin validar targets y permisos.
- No activar `Item` esperando stock, precios o costos; el alcance actual es solo maestro.
- No activar `Warehouse` esperando stock, saldos, costos, kardex, ubicaciones internas avanzadas, lotes, series ni transferencias; el alcance actual es solo maestro.
- No usar `SapCode` como identidad.
- No editar `PayloadJson` manualmente.
- No cambiar `GlobalId` sin analisis de datos e impacto de idempotencia.
- No ejecutar scripts sin backup de Master y sucursal.
- No guardar credenciales en texto plano ni logs.
- No usar SAP como requisito para sincronizar Master/Sucursal.
