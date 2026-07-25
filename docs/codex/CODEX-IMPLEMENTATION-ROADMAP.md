# Roadmap Codex para arquitectura objetivo

Este roadmap ordena la implementacion de la arquitectura Master/Sucursal, SAP opcional y SRI Worker.

## Fase A - Modelo Master/Sucursal

- Crear entidades y tablas Master para sucursales, nodos de sincronizacion y politicas.
- Extender resolucion de contexto para incluir sucursal cuando el endpoint lo requiera.
- Mantener compatibilidad con la empresa activa actual por `X-Company-Code`.
- Agregar permisos por empresa/sucursal.

Prompt sugerido: `docs/codex/prompts/01-master-branch-tenant-model.md`.

## Fase B - Capacidades e integraciones por empresa

- Consolidar capacidades por empresa y sucursal.
- Separar parametros de negocio, SAP, SRI y sincronizacion.
- Garantizar que SAP pueda estar desactivado sin afectar la operacion.

Prompt sugerido: `docs/codex/prompts/02-tenant-features-integrations.md`.

## Fase C0 - Identificadores globales y referencias externas

- Preparar entidades replicables con `GlobalId` estable sin reemplazar `Id` local.
- Agregar referencias opcionales `ExternalSystem`, `ExternalCode` y `SapCode` solo donde aplique.
- Mantener creacion Standalone sin SAP y sin exigir codigos externos.
- Crear script idempotente `database/sql/063_tenant_global_ids_and_external_refs.sql`.
- Exponer campos en DTOs de entidades maestras y catalogos preparados.
- Dejar fuera Outbox/Inbox, workers, colas SRI y SAP IntegrationOutbox.

Entidades cubiertas: terceros, items, almacenes, listas de precio, usuarios, parametros por empresa y catalogos administrativos tenant existentes.

Prompt base: `docs/codex/prompts/03-sync-outbox-inbox.md`, limitado a preparacion de identificadores.

## Fase C - Sincronizacion Outbox/Inbox

- Crear contratos y tablas Outbox/Inbox en Master y sucursal. Implementado como infraestructura base con `database/sql/064_master_sync_outbox_inbox.sql` y `database/sql/065_tenant_sync_inbox_local_outbox.sql`.
- Registrar scripts en inicializadores Master/Tenant.
- Crear contratos Application y enums Shared SAP-free para eventos, reglas, auditoria, payload y aplicacion futura.
- Crear repositorios base `ISyncOutboxRepository`, `ISyncInboxRepository` e `ISyncAuditRepository`.
- Exponer monitoreo protegido por permisos `SYNC.OUTBOX.VIEW` y `SYNC.AUDIT.VIEW` en `/api/sync/dashboard`, `/api/sync/summary`, `/api/sync/outbox`, `/api/sync/outbox/{id}`, `/api/sync/outbox/{id}/targets` y `/api/sync/audit`.
- Fase C.1: endurecer idempotencia concurrente en repositorios para duplicados por `EventId` y por `OutboxId` + `BranchCompanyId`.
- Mantener documentado que `NuanSystem.SyncWorker` es el worker SAP existente y que la sincronizacion Master/Sucursal vive en el proyecto separado `NuanSystem.MasterBranchSyncWorker`.
- Fase C.2: crear publicacion base con `ISyncEventPublisher`, `ISyncEventPayloadFactory` e `IReplicableEntityMetadataProvider`. El publicador solo crea `SyncOutbox` si la empresa es Master, tiene `SyncEnabled`, la entidad esta configurada/habilitada y la direccion permite `MasterToBranch`.
- Fase C.2 no conecta handlers reales de `BusinessPartners`, `Items` ni otros catalogos; solo deja la infraestructura lista para hacerlo despues.
- Fase C.3: conectar `BusinessPartners` como entidad piloto al publisher en create, update y delete logico. El evento usa `GlobalId` como `EntityGlobalId`, `Code` como `EntityCode` y un payload sin campos SAP directos.
- Deuda detectada en C.3: el limite entre persistencia tenant y `SyncOutbox`
  Master no era atomico. Iteracion 8 / Fase C.13 define su correccion antes de
  promover mas entidades.
- Fase C.4: crear `NuanSystem.MasterBranchSyncWorker` como worker esqueleto separado de `NuanSystem.SyncWorker` SAP. Reclama eventos `SyncOutbox`, usa lock tecnico (`LockedBy`, `LockedAt`, `LockExpiresAt`), libera locks vencidos, consulta targets existentes y registra auditoria tecnica. `SkeletonMode` queda `true` por defecto con `NoOpSyncEventApplier`, sin aplicar entidades reales.
- Fase C.5: agregar `DeadLetter` como estado final para eventos que agotan `MaxAttempts` o se consideran fallidos definitivos. Los repositorios limpian locks tecnicos, el worker no detiene el servicio ante errores y registra auditoria `DeadLetter`. No se implementa reintento manual ni endpoints de ejecucion.
- Fase C.6: implementar aplicador piloto de `BusinessPartner` hacia sucursales. La identidad de sincronizacion es `GlobalId`, `SyncInbox` asegura idempotencia por `EventId`, el payload es SAP-free y `SkeletonMode` permanece `true` por defecto. `Items`, `Warehouses` y `PriceLists` no se conectan en esta fase.
- Fase C.7: endurecer `SkeletonMode` con `SkeletonModeBehavior`. El default es `ObserveOnly`, que no reclama ni cambia `SyncOutbox`; `ClaimAndRelease` permite dry-run con auditoria `DryRun` y devuelve el evento a `Pending`; `ClaimAndIgnore` conserva el cierre en `Ignored` solo bajo configuracion explicita. `SkeletonMode=false` mantiene el flujo real existente.
- Fase C.8: implementar aplicador piloto de `Item` como maestro de articulo. Publica create/update/delete logico con `EntityName = Item`, aplica en sucursal por `GlobalId`, mantiene idempotencia por `EventId` en `SyncInbox` y permite `SapCode` nullable solo como referencia externa. Quedan fuera stock, kardex, movimientos, lotes, series, vencimientos, costos, precios por lista, bodegas y disponibilidad.
- Fase C.9: implementar monitor operativo de sincronizacion Master/Sucursal. Agrega endpoints GET para dashboard, summary, outbox listado/detalle, targets y auditoria; los listados no cargan `PayloadJson` y el detalle si puede mostrarlo. No se agregan endpoints de retry, reprocess, apply, run, dispatch ni claim en esta fase; las acciones manuales quedan para Fase C.10 con permisos y reglas explicitas.
- Frontend Fase 1: implementar Monitor Sync Master/Sucursal en WinForms DevExpress con dashboard, listado `SyncOutbox`, detalle con payload bajo demanda, targets y auditoria opcional. El cliente consume solo endpoints GET mediante `NuanApiClient`, usa `FormKey = sync-monitor`, se publica en `Administracion > Sincronizacion > Monitor Sync` y no expone acciones manuales mutadoras.
- Fase C.10: implementar retry manual controlado. Agrega solo tres acciones POST: retry de eventos `Error` con `SYNC.OUTBOX.RETRY`, retry de `DeadLetter` con motivo obligatorio y `SYNC.OUTBOX.RETRY_DEADLETTER`, y liberacion de lock vencido con `SYNC.OUTBOX.RELEASE_LOCK`. Todas registran auditoria, no editan payload, no cambian identidad de entidad, no ejecutan worker y no aplican eventos desde API.
- Frontend Fase 2: agregar al detalle del Monitor Sync las tres acciones manuales de Fase C.10. La UI muestra botones solo por permiso, habilita por estado/lock vencido, exige motivo para `DeadLetter`, refresca detalle/listado/dashboard/auditoria tras exito y mantiene prohibidos apply, run, process, dispatch, claim, sync-now, reprocess, edicion de payload y ejecucion del worker.
- Fase C.11 / Fase 4.10: documentar operacion, checklist de despliegue y troubleshooting de Sync Master/Sucursal en `docs/operations/`. La documentacion queda cerrada como guia operativa, distingue `ObserveOnly`, `ClaimAndRelease`, `ClaimAndIgnore` y `SkeletonMode=false`; advierte que `Item` no sincroniza stock, precios ni costos; y confirma que SAP no participa en Sync Master/Sucursal.
- Fase C.12 / Fase 4.10.1: auditar la documentacion operativa contra la implementacion real. Solo corrige documentacion insegura o ambigua; no cambia endpoints, worker, scripts ejecutables, permisos, SAP ni SRI.
- Iteracion 8 / Fase C.13: cerrar el limite transaccional definitivo con `LocalOutbox` tenant y promocion idempotente a `SyncOutbox` Master. La implementacion se divide en infraestructura deshabilitada, piloto exclusivo `BusinessPartner`, validacion de fallos y migracion posterior independiente de `Item`/`Warehouse`. No usa MSDTC, no abre Master dentro de una transaccion tenant y no participa SAP ni SRI. Blueprint: `docs/architecture/MASTER-BRANCH-ITERATION-8-TRANSACTIONAL-OUTBOX-BLUEPRINT.md`.
- Implementar consumo idempotente completo en sucursal.
- Implementar aplicadores reales por entidad, backoff avanzado, resolucion de conflictos y consola avanzada para revision/reproceso masivo de `DeadLetter`.

Prompt sugerido: `docs/codex/prompts/03-sync-outbox-inbox.md`.

## Fase D - Cola SRI

Estado actual: Iteracion 5 en blueprint/contratos. No existe todavia una cola operativa. Ver `docs/architecture/SRI-ITERATION-5-BLUEPRINT.md` y `$nuansystem-sri-document-queue`.

- Crear contratos, tablas y endpoints para encolar documentos SRI.
- Permitir origen NuanSystem, TXT, AddOn SAP y formulario.
- Mantener procesamiento XML fuera de API y WinForms.

Prompt sugerido: `docs/codex/prompts/04-sri-document-queue.md`.

## Fase E - Worker SRI

Estado actual: Iteracion 5 en blueprint/contratos. `NuanSystem.SriWorker` y el proveedor SRI productivo no existen todavia. Ver `docs/architecture/SRI-ITERATION-5-BLUEPRINT.md` y `$nuansystem-sri-worker`.

- Crear Worker Service.
- Procesar cola, descargar XML, registrar intentos y aplicar reintentos.
- Agregar health checks y configuracion productiva.

Prompt sugerido: `docs/codex/prompts/05-sri-worker-service.md`.

## Fase F - SAP opcional

- Llevar configuracion SAP a Master por empresa.
- Validar modo `None`, `ServiceLayer` y `DiApi`.
- Aislar mapeos, logs y errores en `NuanSystem.SapIntegration`.

Prompt sugerido: `docs/codex/prompts/06-sap-optional-integration.md`.

## Fase G - UI WinForms de configuracion

- Crear pantallas para empresa, sucursal, capacidades, SAP, SRI y sincronizacion.
- Consumir API mediante servicios centralizados.
- No incluir logica de negocio ni conexiones directas.

Prompt sugerido: `docs/codex/prompts/07-winforms-configuration-ui.md`.
