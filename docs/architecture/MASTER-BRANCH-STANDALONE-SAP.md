# Arquitectura objetivo Master/Sucursal, ERP independiente y SAP opcional

Este documento complementa `docs/ARCHITECTURE.md`, `docs/ARQUITECTURA-COMERCIAL.md`, `docs/FASE-2-MULTIEMPRESA.md` y `docs/FASE-10-SAP.md`. No reemplaza esas fases: fija la arquitectura objetivo para operar NuanSystem como ERP independiente, con integracion SAP Business One opcional por empresa.

## Principios

- NuanSystem es el sistema operacional principal y debe funcionar sin SAP.
- SAP Business One es un destino/origen opcional de integracion, activado por empresa.
- El modelo comercial de NuanSystem no se diseña alrededor de SAP.
- `Domain` no contiene tipos, estados, nombres de campos ni reglas propias de SAP.
- WinForms consume la API; nunca abre conexiones directas a Master, sucursales ni SAP.
- Toda replicacion entre Master y sucursal usa Outbox/Inbox, no escrituras cruzadas directas.

## Topologia de datos

`NuanSystem_Master` es la base central de gobierno:

- Empresas, sucursales y relacion empresa-sucursal.
- Usuarios, roles, permisos y acceso por empresa/sucursal.
- Capacidades comerciales y parametros globales por empresa.
- Configuracion de conexiones tenant.
- Configuracion SAP opcional por empresa.
- Configuracion SRI centralizada.
- Estado de sincronizacion, rutas logicas, nodos y politicas de replicacion.

Cada base de sucursal contiene operacion local:

- Catalogos necesarios para operar.
- Inventario, ventas, compras, caja, documentos y auditoria local.
- Outbox local con eventos producidos por la sucursal.
- Inbox local con comandos o datos recibidos desde Master.
- Estado de sincronizacion e idempotencia por mensaje.

## Modos de operacion

### ERP independiente

Es el modo base y obligatorio. La empresa usa NuanSystem para administrar catalogos, inventario, ventas, compras, caja, documentos, seguridad, SRI y reportes sin SAP.

En este modo:

- `SapIntegrationMode = None`.
- No se requiere Service Layer, DI API ni AddOn SAP.
- Los documentos comerciales mantienen su ciclo de vida local.
- SRI funciona con documentos generados o importados por NuanSystem.

### ERP con SAP opcional

Una empresa puede activar SAP si su operacion lo requiere.

En este modo:

- SAP se configura por empresa en Master.
- El backend selecciona Service Layer o DI API segun configuracion.
- Los envios a SAP pasan por `Application` y `NuanSystem.SapIntegration`.
- Los errores y reintentos se registran en `SapSyncLog`.
- La desactivacion de SAP no debe romper operaciones locales.

## Relacion Master/Sucursal

Master gobierna configuracion y consolida informacion. La sucursal ejecuta la operacion diaria con tolerancia a desconexion cuando el caso lo permita.

Flujo conceptual:

```text
Sucursal API/DB
  -> Outbox sucursal
  -> Sync Worker/API
  -> Inbox Master
  -> Procesamiento Master
  -> Outbox Master
  -> Inbox sucursal
  -> Aplicacion local idempotente
```

No se permite que una sucursal escriba directamente en tablas operativas de otra sucursal. La comunicacion siempre se expresa como mensajes versionados.

## Outbox/Inbox

Outbox registra cambios confirmados por la misma transaccion local que produjo el evento de negocio. Inbox registra mensajes recibidos antes de aplicarlos.

La infraestructura base de Fase 4 ya crea los objetos tecnicos para preparar sincronizacion:

- Master: `SyncEntityConfigurations`, `SyncDistributionRules`, `SyncOutbox`, `SyncOutboxTargets` y `SyncAudit` mediante `database/sql/064_master_sync_outbox_inbox.sql`.
- Tenant/sucursal: `SyncInbox`, `LocalOutbox` y `SyncAudit` mediante `database/sql/065_tenant_sync_inbox_local_outbox.sql`.
- Application define contratos genericos SAP-free para eventos, auditoria, evaluacion de reglas, serializacion y aplicacion futura.
- Persistence registra repositorios base para crear/consultar Outbox, Inbox y auditoria.
- API expone endpoints protegidos de monitoreo en `/api/sync/dashboard`, `/api/sync/summary`, `/api/sync/outbox`, `/api/sync/outbox/{id}`, `/api/sync/outbox/{id}/targets` y `/api/sync/audit`, filtrados por la empresa activa resuelta desde `X-Company-Code`.
- Fase 4.1 endurece la idempotencia concurrente de `SyncOutbox`, `SyncInbox` y `SyncOutboxTargets`: ante duplicados por `EventId` o por `OutboxId` + `BranchCompanyId`, los repositorios recuperan el registro existente y mantienen los indices unicos como proteccion final.
- Los endpoints de monitoreo usan permisos granulares `SYNC.OUTBOX.VIEW` y `SYNC.AUDIT.VIEW`.
- Fase 4.2 agrega contratos y servicios de publicacion transaccional futura: `ISyncEventPublisher`, `ISyncEventPayloadFactory` e `IReplicableEntityMetadataProvider`. El publicador valida `SyncEnabled`, `IsMaster`, configuracion de entidad y direccion antes de crear `SyncOutbox`.
- La fabrica de payload genera JSON estable con `GlobalId`, `Code` cuando existe y remueve campos sensibles como passwords, tokens, credenciales o connection strings.
- Fase 4.3 conecta `BusinessPartners` como primera entidad piloto: crear, actualizar y eliminar logicamente un tercero publica `SyncOutbox` con `EntityName = BusinessPartner`, `EntityGlobalId = BusinessPartner.GlobalId` y `EntityCode = BusinessPartner.Code` cuando la empresa activa y la configuracion permiten `MasterToBranch`.
- Fase 4.4 crea `NuanSystem.MasterBranchSyncWorker` como worker esqueleto separado del worker SAP. Reclama eventos `SyncOutbox` pendientes/error reprocesable con `LockedBy`, `LockedAt` y `LockExpiresAt`, libera locks vencidos, consulta targets existentes y registra auditoria tecnica `Claimed`, `Ignored`, `Applied` o `Failed`.
- Fase 4.5 agrega `DeadLetter` como estado final operativo para eventos fallidos definitivos o que agotaron `MaxAttempts`. El worker registra auditoria `DeadLetter`, limpia locks tecnicos y no vuelve a reclamar esos eventos.
- Fase 4.6 agrega el primer aplicador real y limitado para `BusinessPartner`. El worker usa `GlobalId` como identidad de sincronizacion hacia la sucursal, registra `SyncInbox` por `EventId`, aplica upsert/desactivacion idempotente y no usa `Id` local ni `SapCardCode`.
- Fase 4.7 agrega el segundo aplicador real para `Item`, limitado al maestro del articulo. Publica y aplica `EntityName = Item` por `GlobalId`, usa `SyncInbox` para idempotencia por `EventId` y trata `SapCode` solo como referencia externa opcional.
- Fase 4.8 agrega monitoreo operativo de lectura: dashboard, resumen, busqueda de `SyncOutbox`, detalle con `PayloadJson`, targets y auditoria. Los listados y dashboard no cargan `PayloadJson` masivo. No agrega retry, reprocess, apply, run, dispatch ni claim desde API.
- Fase 4.9 agrega acciones manuales controladas sobre `SyncOutbox`: retry de `Error`, retry de `DeadLetter` con motivo obligatorio y liberacion de locks vencidos. Cada accion exige permiso especifico, valida estado actual, no cambia `PayloadJson`, `EntityName` ni `EntityGlobalId`, y registra auditoria transaccional.

BusinessPartners publica el evento despues de persistir y volver a leer la entidad. El CRUD tenant y el `SyncOutbox` Master aun no comparten una transaccion distribuida; queda como pendiente cerrar ese limite transaccional antes de masificar la estrategia a otras entidades.

El proyecto `NuanSystem.SyncWorker` existente corresponde a workers SAP ya implementados. La sincronizacion Master/Sucursal usa el proyecto separado `NuanSystem.MasterBranchSyncWorker`. Desde Fase 4.7 opera con `SkeletonMode = true` y `SkeletonModeBehavior = ObserveOnly` por defecto: no reclama eventos, no libera locks, no cambia estados, no escribe en tablas de negocio de sucursal, no aplica `BusinessPartners`, `Items`, bodegas ni listas de precio, y no invoca SAP ni SRI.

`SkeletonMode` tiene tres comportamientos operativos:

- `ObserveOnly`: modo seguro por defecto. El worker solo registra log tecnico y no consume `SyncOutbox`.
- `ClaimAndRelease`: reclama eventos para dry-run tecnico, registra auditoria `DryRun` y devuelve el evento a `Pending` liberando el lock.
- `ClaimAndIgnore`: conserva el comportamiento anterior y marca `Ignored`, pero solo debe usarse con configuracion explicita.

La aplicacion real solo ocurre cuando `SkeletonMode = false`, la entidad esta en `EnabledEntityAppliers` y existe un aplicador registrado. `Countries`, `Provinces` y `Cities` pueden habilitarse como catalogos geograficos ordenados por dependencia. `Currencies` puede habilitarse como catalogo de monedas. `Items` puede habilitarse como maestro de articulo, pero no activa sincronizacion de inventario operativo. `Warehouse` puede habilitarse como maestro de bodega, sin stock, kardex, costos ni transferencias. Listas de precio y otros catalogos quedan para fases posteriores.

`ItemGroups` es una dependencia operativa de `Item` y debe ejecutarse primero. Replica identidad `GlobalId`, codigo, nombre, descripcion, cuentas contables configuradas, referencias SAP opcionales, referencias externas y estado. El aplicador adopta por `Code` solamente registros heredados que aun no comparten `GlobalId`; no replica articulos, stock ni movimientos.

El alcance de `Item` en Fase 4.7 es exclusivamente maestro:

- Incluye identidad `GlobalId`, codigo, nombre, descripcion, grupo/familia/unidad cuando existan, flags comerciales basicos, referencias externas y `SapCode` nullable.
- Excluye existencias, kardex, movimientos, lotes, series, vencimientos, costo promedio, costos por bodega, precios por lista y disponibilidad.
- `SapCode` no es identidad de sincronizacion y no es requerido para clientes Standalone.

El alcance de `Warehouse` en Fase 4.10 es exclusivamente maestro:

- Incluye identidad `GlobalId`, codigo, nombre, descripcion, sucursal logica, direccion, ciudad, provincia, pais, contacto responsable, flags operativos, estado y referencias externas.
- Excluye stock, saldos, kardex, movimientos, costos, ubicaciones internas avanzadas, lotes, series y transferencias.
- `SapCode` no es identidad de sincronizacion y no es requerido para clientes Standalone.

El alcance de `Countries` es exclusivamente el catalogo maestro de paises:

- Incluye `GlobalId`, codigo, nombre, ISO2, ISO3, prefijo telefonico y estado.
- Conserva `CountryId` como identidad local de cada tenant y usa `GlobalId` para la replica.
- Durante la adopcion inicial puede reconciliar por `Code` unico para no duplicar catalogos preexistentes.
- Excluye provincias, ciudades, direcciones, geocodificacion y cualquier dato transaccional.

El alcance de `Provinces` es exclusivamente el catalogo maestro de provincias:

- Incluye `GlobalId`, `CountryGlobalId`, codigo, nombre y estado.
- Conserva `ProvinceId` y `CountryId` como identidades locales de cada tenant.
- Resuelve el pais por `CountryGlobalId` y usa `CountryCode` solo para adoptar datos heredados.
- Excluye ciudades, direcciones, geocodificacion y cualquier dato transaccional.

El alcance de `Cities` es exclusivamente el catalogo maestro de ciudades:

- Incluye `GlobalId`, `CountryGlobalId`, `ProvinceGlobalId`, codigo, nombre y estado.
- Conserva `CityId`, `ProvinceId` y `CountryId` como identidades locales de cada tenant.
- Resuelve pais y provincia por sus identidades globales, valida la jerarquia y usa codigos solo para adoptar datos heredados.
- Excluye direcciones, parroquias, zonas, geocodificacion y cualquier dato transaccional.

El alcance de `Currencies` es exclusivamente el catalogo maestro de monedas:

- Incluye `GlobalId`, codigo ISO/comercial, nombre, simbolo, descripcion, indicador de moneda base, estado y referencias externas opcionales.
- Conserva `CurrencyId` como identidad local de cada tenant y usa `GlobalId` para la replica.
- Durante la adopcion inicial puede reconciliar por `Code` unico para no duplicar USD, EUR u otras monedas existentes.
- Excluye tipos de cambio, historicos de cotizacion, listas de precio, documentos y conversiones monetarias.

Desde Fase 4.9 existen acciones manuales acotadas:

- `POST /api/sync/outbox/{id}/retry`: solo para `Error`, requiere `SYNC.OUTBOX.RETRY`, devuelve el evento a `Pending`, limpia lock tecnico y registra `Retried`.
- `POST /api/sync/outbox/{id}/retry-deadletter`: solo para `DeadLetter`, requiere `SYNC.OUTBOX.RETRY_DEADLETTER`, exige motivo, puede resetear `AttemptCount` y registra `RetriedFromDeadLetter`.
- `POST /api/sync/outbox/{id}/release-expired-lock`: solo para `InProcess` o `Error` con lock vencido, requiere `SYNC.OUTBOX.RELEASE_LOCK`, no libera locks vigentes y registra `LockReleased`.

Estas acciones no aplican el evento, no ejecutan el worker, no aceptan payload arbitrario, no editan `PayloadJson`, no cambian `EntityName`, no cambian `EntityGlobalId` y no modifican targets. Siguen prohibidos endpoints `apply`, `run`, `process`, `dispatch`, `claim`, `sync-now`, `reprocess` y edicion de payload.

La operacion y despliegue de Sync Master/Sucursal se documentan en:

- `docs/operations/SYNC-MASTER-BRANCH-OPERATIONS.md`
- `docs/operations/SYNC-MASTER-BRANCH-DEPLOYMENT-CHECKLIST.md`
- `docs/operations/SYNC-MASTER-BRANCH-TROUBLESHOOTING.md`

Estos documentos son guias operativas. No agregan entidades, no habilitan SAP/SRI y no cambian el comportamiento del worker.

Cada mensaje debe incluir:

- `EventId` global unico.
- Empresa origen y destino cuando aplique.
- `CompanyCode` y, cuando aplique, `BranchCode`.
- `MessageType` versionado.
- `EntityName`, `EntityGlobalId`, `EntityCode` y version local cuando aplique.
- Payload JSON.
- Hash o clave de idempotencia.
- Estado, reintentos, fechas y traza.

Estados vigentes:

- `Pending`
- `InProcess`
- `Applied`
- `Error`
- `Ignored`
- `DeadLetter`

## Propiedad de datos

- Master es propietario de empresas, sucursales, permisos, capacidades globales e integraciones.
- La sucursal es propietaria de transacciones locales originadas en su punto de operacion.
- Catalogos pueden originarse en Master o sucursal segun politica, pero su direccion de sincronizacion debe estar declarada.
- `Id` es local de cada base. `GlobalId` identifica la misma entidad entre Master y sucursales. `Code` sigue siendo codigo funcional.
- Referencias externas como `ExternalSystem`, `ExternalCode` y `SapCode` son opcionales y no convierten SAP en dependencia obligatoria.

## Entidades replicables preparadas

La fase de preparacion agrega `GlobalId` y referencias externas opcionales a entidades maestras que pueden viajar entre Master y sucursal. No implementa Outbox/Inbox ni workers.

Entidades iniciales preparadas:

- `BusinessPartners`: `GlobalId`, `ExternalSystem`, `ExternalCode`. SAP se mantiene en `BusinessPartnerSapMapping.SapCardCode` para no duplicar `SapCode`.
- `Items`: `GlobalId`, `ExternalSystem`, `ExternalCode`, `SapCode` opcional.
- `Warehouses`: `GlobalId`, `ExternalSystem`, `ExternalCode`, `SapCode` opcional.
- `Countries`: `GlobalId` y codigo comercial unico por tenant.
- `Provinces`: `GlobalId` y referencia al `GlobalId` de Countries.
- `Cities`: `GlobalId` y referencias a los `GlobalId` de Countries y Provinces.
- `Currencies`: `GlobalId`, `ExternalSystem` y `ExternalCode` opcionales; `Code` se usa solo para adopcion inicial.
- `ItemGroups`: `GlobalId`, referencias externas y referencias SAP opcionales; se distribuye antes de `Items`.
- `PriceLists`: `GlobalId`, `ExternalSystem`, `ExternalCode`, `SapCode` opcional.
- `Users` y `CompanyParameters` en Master.
- Catalogos administrativos tenant existentes: unidades de medida, grupos y familias de items, impuestos, monedas, catalogos auxiliares de terceros/proveedores, geografia, bancos y catalogo operacional.

Entidades excluidas por ahora:

- Documentos transaccionales, stock, kardex, caja, compras y ventas. Se sincronizaran despues de definir Outbox/Inbox y contratos versionados.
- Colas SRI, SAP IntegrationOutbox y workers. Pertenecen a fases posteriores.

## SAP como integracion

SAP no define la arquitectura de tenancy. Una empresa sin SAP y una empresa con SAP comparten el mismo modelo de Master, sucursal, API, documentos y SRI.

Reglas:

- SAP se configura en Master por empresa.
- La integracion SAP vive fuera de `Domain`.
- La API y workers backend son los unicos puntos autorizados para invocar SAP.
- SAP puede consumir documentos locales o publicar referencias externas, pero no sustituye la consistencia local.
- La sincronizacion Master/Sucursal no depende de SAP.

## Alta administrativa de sucursales

El mantenimiento de companias en Master expone de forma explicita `IsMaster`,
`ParentCompanyId`, `BranchCode` y `SyncEnabled`. Una sucursal se registra como una
compania tenant independiente, con base propia, y siempre referencia una compania
maestra activa.

Reglas de ciclo de vida:

- El tipo maestra/sucursal no puede cambiar despues del alta.
- Una maestra no acepta padre ni codigo de sucursal.
- Una sucursal exige padre maestro y `BranchCode` unico dentro de ese padre.
- El alta inicial se realiza con `SyncEnabled = false` hasta provisionar la base,
  inicializar el esquema, configurar politicas y validar una previsualizacion.
- Las sucursales del piloto operan en modo Standalone; no almacenan credenciales SAP.
- La activacion de `SyncEnabled` no sustituye la configuracion del perfil ni de sus
  politicas de distribucion.

## Pendientes de implementacion

- Definir entidades Master para sucursales, nodos de sincronizacion y politicas de direccion.
- Definir el limite transaccional definitivo entre CRUD tenant y `SyncOutbox` Master antes de conectar mas entidades replicables.
- Completar la consola WinForms para administrar politicas de distribucion y sus selecciones.
- Diseñar consola operativa avanzada para revision historica, aprobaciones y reproceso masivo controlado de eventos `DeadLetter`.
- Implementar aplicadores reales para listas de precio y otros catalogos replicables. `Currencies`, `Items` y `Warehouse` ya cuentan con aplicadores limitados al maestro.
- Versionar tipos de mensaje y contratos JSON.
- Completar la UI WinForms para configurar jerarquia de empresa/sucursal y el aprovisionamiento tenant.
