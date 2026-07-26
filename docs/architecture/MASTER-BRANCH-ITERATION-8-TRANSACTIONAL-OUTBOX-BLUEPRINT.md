# Iteración 8 — Límite transaccional de Sync Matriz–Sucursal

## Estado y autoridad

- **Fecha:** 2026-07-25.
- **Alcance actual:** límite transaccional, relay e incorporación progresiva de
  maestros.
- **Estado:** Fases 8.1–8.3, 8.4A y ItemFamily 8.4B-1 implementadas y
  validadas. ItemGroup, Item payload v2/UnitOfMeasure y Warehouse están
  implementados en código con migraciones versionadas, deshabilitados y
  pendientes de despliegue SQL y runtime autorizados.
- **Autoridad:** Constitución > Kernel > catálogos > skill
  `nuansystem-master-branch-sync` > implementación.

Esta iteración cierra el pendiente documentado entre la persistencia de un
maestro en la base tenant y la creación de `SyncOutbox` en Master. No habilita
workers, no activa perfiles, no aplica eventos en sucursales y no involucra SAP
ni SRI. La existencia de código o migraciones no concede autorización para
ejecutarlos.

## Discovery Record

**Outcome:** garantizar que un cambio replicable confirmado nunca se pierda por
una caída de Master o por un fallo ocurrido después del commit tenant.

**Work type:** integración/sincronización y evolución de persistencia.

**Domain:** replicación interna NuanSystem Matriz–Sucursal.

**Explicit decisions and exclusions:**

- La base tenant que contiene el maestro es la fuente de verdad de la mutación.
- `LocalOutbox` es la intención durable local; `SyncOutbox` es la cola central
  de distribución.
- SAP, SRI, `NuanSystem.SyncWorker` y `NuanSystem.SriWorker` quedan excluidos.
- No se usará MSDTC ni una transacción distribuida.
- No se abrirá una conexión a Master dentro de una transacción tenant.
- No se devolverá error de CRUD después de haber confirmado el maestro solo
  porque Master esté temporalmente inaccesible.
- `GlobalId` conserva la identidad entre bases y `EventId` conserva la identidad
  del evento durante su promoción.
- La implementación inicial se limita a `BusinessPartner`. `Item`, `Warehouse`
  y cualquier otra entidad requieren promoción independiente después del piloto.

**Affected layers:** Application, Persistence, SQL tenant, SQL Master,
`NuanSystem.MasterBranchSyncWorker`, pruebas y documentación.

**Risk:** alto. Cambia el límite de commit, reintento e idempotencia de una
sincronización multi-base.

**Evidence inspected:**

- `Application/Features/BusinessPartners/Commands/*CommandHandler.cs` — el CRUD
  persiste, vuelve a leer y publica actualmente después del commit tenant.
- `Application/Features/GeneralInventory/Warehouses/Commands/WarehouseSyncPublisher.cs`
  — confirma el mismo patrón de publicación directa para bodegas.
- `Application/Features/Sync/Services/SyncEventPublisher.cs` — crea primero
  `SyncOutbox` y después evalúa/crea targets mediante operaciones separadas.
- `Persistence/Connections/TenantConnectionFactory.cs` y
  `Persistence/Connections/MasterConnectionFactory.cs` — tenant y Master usan
  conexiones independientes.
- `Application/Abstractions/Data/ITransactionRunner.cs` y
  `Persistence/Transactions/SqlTransactionRunner.cs` — existe un runner
  transaccional tenant reutilizable.
- `database/sql/065_tenant_sync_inbox_local_outbox.sql` — `LocalOutbox` ya
  existe, tiene `EventId` único, payload JSON, estado, intentos y retry, pero no
  posee todavía un contrato operativo de claim/promoción.
- `database/sql/064_master_sync_outbox_inbox.sql` — `SyncOutbox` ya protege
  `EventId` con índice único.
- `docs/architecture/MASTER-BRANCH-STANDALONE-SAP.md` — declara que el outbox
  debe escribirse en la misma transacción local y registra el límite pendiente.

**Selected pattern:** transactional outbox local con relay idempotente hacia
Master.

**Permitted reuse boundary:**

- Reutilizar `LocalOutbox`, `SyncOutbox`, `SyncOutboxTargets`, `SyncAudit`,
  `ITransactionRunner`, metadata, payload factory, routing, locks, retry y
  `NuanSystem.MasterBranchSyncWorker`.
- Extender la persistencia para aceptar conexión/transacción existentes y crear
  una unidad atómica tenant.
- Crear un runner transaccional Master solo si el diseño detallado demuestra que
  no puede lograrse el commit de outbox, decisiones, targets y auditoría mediante
  un procedimiento idempotente existente.
- No reutilizar SAP outbox, SRI queue ni sus workers.

**Alternatives rejected:**

- **Escritura dual directa tenant + Master:** permite maestro confirmado sin
  evento o respuesta de error después de haber guardado.
- **MSDTC/transacción distribuida:** acopla servidores y operación, complica
  recuperación y no es requisito del producto.
- **Llamada a Master dentro de la transacción tenant:** prolonga locks y hace
  depender el CRUD de disponibilidad remota.
- **Reconstruir eventos solo por escaneo periódico de maestros:** no conserva
  operación, snapshot ni causalidad confiables.
- **Publicar en memoria después del commit:** pierde eventos ante caída del
  proceso.

**Confidence:** alta para el patrón y el defecto arquitectónico; media para la
forma exacta del contrato de repositorio hasta completar un prototipo con los
procedimientos reales de `BusinessPartner`.

## Invariantes

1. Maestro e intención local se confirman o revierten juntos.
2. Una transacción tenant nunca abre Master ni realiza llamadas externas.
3. El relay conserva el mismo `EventId`; no genera otro al reintentar.
4. La promoción Master es idempotente por `EventId`.
5. `SyncOutbox`, decisiones, targets y auditoría quedan consistentes en un único
   commit Master.
6. Un crash después del commit Master y antes de cerrar `LocalOutbox` es
   recuperable: el siguiente intento encuentra el evento existente y completa
   el cierre local.
7. Solo un relay posee un lease local vigente.
8. Retry es acotado; agotamiento termina en `DeadLetter` visible y auditable.
9. Un evento sin configuración o dirección aplicable termina `Ignored` con
   motivo; no desaparece silenciosamente.
10. El éxito del CRUD significa “maestro e intención durable confirmados”, no
    “todas las sucursales aplicaron”.

## Arquitectura objetivo

```text
Command handler
  -> ITransactionRunner (tenant)
       -> guardar/actualizar/desactivar maestro
       -> volver a leer estado confirmado dentro de la transacción
       -> construir snapshot SAP-free
       -> insertar LocalOutbox con EventId estable
  -> COMMIT tenant
  -> responder éxito del CRUD

NuanSystem.MasterBranchSyncWorker (relay, deshabilitado por defecto)
  -> resolver tenants Master habilitados
  -> reclamar LocalOutbox con lease
  -> evaluar metadata/configuración en Master
  -> transacción Master
       -> crear o recuperar SyncOutbox por EventId
       -> registrar decisiones de routing
       -> crear/recuperar targets
       -> registrar auditoría
  -> COMMIT Master
  -> marcar LocalOutbox Applied/Ignored
  -> worker normal distribuye SyncOutbox a sucursales
```

El relay y el aplicador de sucursal son etapas distintas. Promover a Master no
equivale a aplicar el registro en una sucursal.

## Contratos propuestos

### Escritura tenant

Crear un contrato local, por ejemplo `ILocalSyncOutboxWriter`, que reciba la
conexión y transacción existentes. No debe resolver metadata Master ni decidir
targets.

Datos mínimos:

```text
EventId
CompanyId
EntityName
EntityGlobalId
EntityCode
Operation
PayloadJson
MaxAttempts
CreatedAt
```

El `EventId` se genera antes de iniciar la unidad de trabajo y viaja durante todo
el proceso.

### Claim local

La migración tenant debe agregar:

- `LockedBy`, `LockedAt`, `LockExpiresAt`;
- `rowversion` o control equivalente para completar con ownership;
- procedimientos para claim, release de lease vencido, completar, programar
  retry y mover a `DeadLetter`;
- índices para estados elegibles y expiración;
- auditoría local sin payload ni secretos.

### Promoción Master

Crear un contrato de promoción que acepte el `EventId` original. Debe devolver:

```text
Promoted
Existing
Ignored
Conflict
```

`Existing` es éxito idempotente solo si identidad, entidad, operación y payload
coinciden con el evento ya persistido. Una colisión del mismo `EventId` con otro
contenido es `Conflict` terminal y auditable.

### Compatibilidad

`ISyncEventPublisher.PublishAsync` sigue siendo válido para ejecuciones
administrativas/full que ya operan desde Master. Los CRUD migrados no deben
usar simultáneamente publicación directa y `LocalOutbox`.

## Fases de implementación

### 8.1 — Infraestructura sin activación

- Migración tenant para lease, retry, auditoría y procedimientos de
  `LocalOutbox`.
- Contratos Application y repositorio Dapper.
- Promoción idempotente Master con `EventId` aportado.
- Configuración del relay deshabilitada por defecto.
- Ningún handler conectado.

### 8.2 — Piloto `BusinessPartner`

- Adaptar create/update/delete lógico para una unidad transaccional tenant.
- Persistir snapshot e intención local en el mismo commit.
- Retirar la publicación directa únicamente de esos tres handlers.
- Mantener `Item` y `Warehouse` sin cambios durante el piloto.

### 8.3 — Validación de fallos

- rollback tenant;
- Master no disponible;
- dos relays concurrentes;
- lease vencido;
- crash después del commit Master;
- promoción repetida;
- colisión de `EventId`;
- metadata deshabilitada;
- routing sin targets;
- retry y `DeadLetter`;
- cero llamadas SAP/SRI.

### 8.4 — Promoción controlada

Solo después de aprobar 8.3:

1. migrar `Item`;
2. validar de nuevo;
3. migrar `Warehouse`;
4. validar de nuevo;
5. evaluar otras entidades en cambios independientes.

## Matriz de capas

| Capa | Estado actual | Acción de implementación |
|---|---|---|
| Domain | Verificar sin cambios | Ninguna dependencia de infraestructura |
| Application | Cambio futuro | Unidad de trabajo y contratos local/promoción |
| Persistence | Cambio futuro | Métodos transaccionales y repositorios outbox |
| API | Verificar sin cambios | El CRUD conserva su contrato funcional |
| SQL tenant | Cambio futuro | Lease, procedimientos, índices y auditoría |
| SQL Master | Cambio futuro | Promoción idempotente y commit consistente |
| Worker | Cambio futuro | Relay local→Master, deshabilitado por defecto |
| Frontend | No aplicable | No se agrega UI en 8.1–8.3 |
| Security | Verificar | Contexto tenant y permisos existentes |
| Tests | Cambio futuro | Contratos, concurrencia, fallos y SQL real |
| Documentation | Cambio actual | Blueprint, roadmap y plan de validación |

## Quality gates

- El commit tenant contiene maestro + `LocalOutbox`.
- La caída de Master no revierte ni falsea el éxito del CRUD.
- La recuperación no duplica `SyncOutbox` ni targets.
- La colisión de `EventId` no se trata como idempotencia.
- Los leases tienen propietario, expiración y recuperación.
- Los errores quedan visibles; no hay retry infinito.
- `BusinessPartner` no usa ambos caminos de publicación.
- Worker, perfiles y entidades permanecen deshabilitados por defecto.
- Build y pruebas pasan sin depender de SAP/SRI.
- La prueba SQL real requiere autorización separada y bases identificadas.

## Riesgos pendientes

- Los repositorios CRUD actuales abren sus propias conexiones; deben evolucionar
  sin duplicar lógica ni romper procedimientos existentes.
- El payload se construye después de releer el estado persistido; esa lectura
  debe ocurrir dentro de la misma transacción.
- La consistencia Master incluye routing/targets, no solo la fila `SyncOutbox`.
- `LocalOutbox` fue creado como reserva y necesita un contrato operativo
  forward-only.
- La activación runtime exige un piloto independiente; esta iteración documental
  no la autoriza.

## Criterio de salida documental

La iteración puede pasar a implementación cuando el propietario apruebe:

1. `LocalOutbox` como límite transaccional definitivo;
2. `BusinessPartner` como único piloto inicial;
3. relay dentro de `NuanSystem.MasterBranchSyncWorker`;
4. worker/relay deshabilitado por defecto;
5. migraciones forward-only e idempotentes;
6. prueba SQL y runtime separadas de la implementación.

## Avance de Fase 8.1

**Estado de código:** implementado y validado estáticamente; no desplegado.

Se incorporaron:

- contratos Application para escritura transaccional de `LocalOutbox`;
- migración tenant 124 con leases exclusivos, recuperación, retry,
  `DeadLetter` y auditoría local;
- promoción Master idempotente que conserva `EventId` y confirma
  `SyncOutbox`, decisiones, targets y auditoría en una sola transacción;
- migración Master 125 forward-only;
- relay dentro de `NuanSystem.MasterBranchSyncWorker`, deshabilitado por
  defecto;
- pruebas de contrato de transacción, configuración, scripts y promoción.

No se conectó ningún handler de `BusinessPartner`; eso pertenece a Fase 8.2.
No se ejecutaron scripts, workers, SAP, SRI ni pruebas runtime.

## Cierre de Fases 8.2 y 8.3

**Fecha de validación:** 2026-07-25.

El piloto `BusinessPartner` quedó conectado al límite transaccional tenant:

- create, update y eliminación lógica guardan maestro e intención
  `LocalOutbox` dentro de la misma transacción;
- esos tres handlers ya no publican directamente a Master;
- `Item` y `Warehouse` permanecen fuera del cambio;
- el relay se resuelve desde el host
  `NuanSystem.MasterBranchSyncWorker` mediante los servicios de Application;
- el DTO reclamado por Dapper admite materialización por propiedades y no
  depende del orden físico de las columnas agregadas por la migración 124.

Durante la prueba real se detectó que el contrato histórico de
`BusinessPartner` de DEMO no incluía toda la configuración de compras ni las
proyecciones de identidad global. La migración tenant 126 restablece el
contrato, conserva los wrappers de dimensiones contables y fue ejecutada dos
veces exclusivamente en `NuanSystem_DEMO`. La versión
`20260725.126` quedó registrada una sola vez. Remigio y Cañaris permanecieron
en solo lectura.

La validación runtime aprobó:

- create, update y delete lógico con cinco eventos únicos;
- rollback conjunto cuando falla la escritura de `LocalOutbox`;
- indisponibilidad de Master sin pérdida del evento local;
- claim exclusivo entre dos relays;
- recuperación de lease vencido;
- promoción idempotente repetida;
- conflicto terminal cuando el mismo `EventId` contiene otro payload;
- recuperación del crash ocurrido después del commit Master y antes del cierre
  local, sin duplicar `SyncOutbox`;
- ejecución con `SkeletonMode=ObserveOnly`, sin aplicar eventos en sucursales;
- cero llamadas SAP y SRI.

Todos los fixtures `I8BP83*`, eventos Master, targets, auditorías asociadas y
triggers temporales fueron retirados. El snapshot final confirmó cero eventos
elegibles, cero locks y cero fixtures en Master, DEMO, Remigio y Cañaris. La
migración 126 y sus objetos forward-only permanecen instalados en DEMO.

La evidencia de BusinessPartner no autoriza inferir la migración de otra
entidad. Item 8.4A y ItemFamily 8.4B-1 tienen decisiones, contratos y gates
independientes.

## Plan documental de Fase 8.4

El siguiente piloto propuesto es `Item` y está definido en
[MASTER-BRANCH-ITERATION-8-4-ITEM-OUTBOX-BLUEPRINT.md](MASTER-BRANCH-ITERATION-8-4-ITEM-OUTBOX-BLUEPRINT.md).

La propuesta separa:

- **8.4A:** productor Item transaccional y promoción a Master en
  `ObserveOnly`;
- **8.4B-1:** ItemFamily como dependencia operativa, definida en
  [MASTER-BRANCH-ITERATION-8-4B-ITEM-FAMILY-BLUEPRINT.md](MASTER-BRANCH-ITERATION-8-4B-ITEM-FAMILY-BLUEPRINT.md);
- **8.4B:** aplicación real de Item en una sucursal, todavía sujeta a unidades
  de medida, código de barras principal y validación de dependencias.

La existencia del blueprint no autoriza implementación, despliegue ni
activación. `Warehouse` permanece fuera del alcance.
