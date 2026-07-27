# Fase 8.5 — Currency transaccional Matriz-Sucursal

## Estado

- **Fecha de discovery:** 2026-07-27.
- **Rama:** `refactor/codex-skills-v8-5-currency`.
- **Estado:** discovery completado; implementación, SQL y runtime no autorizados.
- **Predecesores:** BusinessPartner 8.2/8.3, ItemGroup, ItemFamily,
  UnitOfMeasure, Item payload v2 y Warehouse 8.4C validados.
- **Siguiente dependencia propuesta:** PriceList, únicamente después de validar
  Currency.

## Discovery Record

**Outcome:** migrar exclusivamente `Currencies` desde publicación directa a
Master hacia `LocalOutbox` transaccional tenant, conservando Full source y
aplicación en sucursal.

**Work type:** sincronización operacional de catálogo administrativo.

**Domain:** FinancialCatalogs / Currency.

**Explicit domain decisions and exclusions:**

- Currency es un catálogo independiente y requisito previo de PriceList.
- La Matriz es la fuente de verdad para el flujo NuanSystem
  `MasterToBranch`.
- `GlobalId` identifica la misma moneda entre bases; `CurrencyId` es local.
- SAP puede aportar referencias externas, pero no gobierna el transporte
  Matriz-Sucursal.
- SAP, SRI, PriceList, documentos, tasas de cambio y conversión monetaria
  quedan fuera de esta fase.
- No se habilitan perfiles, rutas, relay ni workers por scripts.
- El cambio compartido no puede convertir otros FinancialCatalogs en
  productores.

**Affected layers:** Application, Persistence, SQL tenant, SQL Master,
MasterBranchSyncWorker, pruebas y documentación. API y WinForms se verifican;
su ampliación funcional requiere una decisión independiente.

**Risk:** alto. El CRUD actual persiste en tenant y luego publica directamente
a Master. Un fallo de Master puede devolver error después de que la moneda ya
quedó guardada. El aplicador actual además adopta por `Code`, comportamiento
incompatible con la política vigente de colisión terminal.

## Evidencia inspeccionada

- `Application/Features/FinancialCatalogs/Catalogs/Commands/*FinancialCatalogCommandHandler.cs`
  — los handlers genéricos crean, actualizan o eliminan primero y después
  invocan `CurrencySyncPublisher`.
- `Application/Features/FinancialCatalogs/Catalogs/Commands/CurrencySyncPublisher.cs`
  — publicación incremental directa mediante `ISyncEventPublisher`; solo se
  activa para `catalogKey=currencies`.
- `Persistence/Repositories/FinancialCatalogs/FinancialCatalogRepository.cs`
  — repositorio compartido por monedas, bancos, listas de precios y otros
  catálogos; abre una conexión independiente por operación.
- `Persistence/Repositories/Sync/SyncFullEntitySources.cs`
  — `CurrencyFullEntitySource` ya produce payload completo Full.
- `MasterBranchSyncWorker/Services/CurrencySyncEventApplier.cs`
  — dispatcher operativo para Created, Updated, Disabled y Deleted.
- `Persistence/Repositories/Sync/CurrencySyncApplyRepository.cs`
  — usa `SyncInbox` y transacción en sucursal, pero adopta automáticamente una
  fila preexistente por `Code`.
- `Application/Features/Sync/Configuration/SyncMasterBranchEntityCatalog.cs`
  — Currency está operativa en orden 40; PriceList depende de Currency.
- `database/sql/090_tenant_currency_master_branch_sync.sql`
  — contrato tenant anterior, con índice de código filtrado por
  `IsDeleted=0`.
- `database/sql/091_master_currency_sync_registration.sql`
  — registro Master deshabilitado por defecto.
- `database/sql/129_tenant_item_group_transactional_outbox.sql` y
  `133_tenant_warehouse_transactional_outbox.sql`
  — referencias aprobadas para CRUD + `LocalOutbox` en una transacción.
- `database/sql/135_tenant_warehouse_tombstone_code_reservation.sql`
  — referencia vigente para reservar códigos después de eliminación lógica.
- `tests/.../CurrencySyncPublishingTests.cs` y
  `CurrencySyncEventApplierTests.cs`
  — cubren publicación y aplicación básica, pero todavía afirman adopción
  legacy por código.
- `WinForms.Forms/FinancialCatalogs/Currencies/*`
  — formulario compartido visualmente; administra código, nombre, descripción
  y estado. No expone símbolo ni moneda base.

## Patrón seleccionado

Reutilizar el patrón ItemGroup/Warehouse:

```text
Command handler
  -> ITransactionRunner
  -> CRUD Currency usando connection + transaction
  -> releer Currency dentro de la misma transacción
  -> ICurrencyLocalOutboxWriter
  -> LocalOutbox Pending con EventId estable
  -> commit tenant
  -> LocalSyncOutboxRelay
  -> promoción idempotente a SyncOutbox Master
  -> routing/target
  -> CurrencySyncEventApplier
  -> SyncInbox + apply transaccional por GlobalId
```

La implementación debe conservar los handlers financieros compartidos, pero
el camino transaccional se ejecutará únicamente cuando la clave normalizada
sea `currencies`. Los demás catálogos deben conservar exactamente su
comportamiento actual y nunca crear `LocalOutbox`.

## Componentes que se reutilizan

- `ITransactionRunner`.
- `ILocalSyncOutboxRepository`.
- `ISyncEventPayloadFactory`.
- `LocalSyncOutboxRelay`.
- `CurrencySyncPayload`.
- `CurrencyFullEntitySource`.
- `CurrencySyncEventApplier`.
- `SyncInbox`, `SyncAudit`, locks, retry y DeadLetter existentes.
- Configuración, routing, targets, permisos y monitor Matriz-Sucursal.
- `FinancialCatalogRepository`, extendido de forma compatible para aceptar
  conexión y transacción cuando el catálogo sea Currency.

## Alternativas rechazadas

- **Mantener publicación directa:** permite éxito tenant seguido de fallo
  Master y duplica el camino de Iteración 8.
- **Crear otro repositorio completo de Currency:** duplicaría el catálogo de
  procedimientos y el CRUD financiero.
- **Hacer transaccionales todos los FinancialCatalogs:** ampliaría el alcance
  sin productor, payload ni aplicador aprobados.
- **Adoptar por coincidencia de Code:** puede apropiarse de una moneda local o
  tombstone con otro `GlobalId`.
- **Usar SAP outbox o SRI:** son pipelines ajenos.

## Gaps y cambios previstos

### Application

- Crear `ICurrencyLocalOutboxWriter`.
- Reemplazar `CurrencySyncPublisher` por una fábrica/escritor de evento local,
  o reutilizar su construcción de payload sin publicación directa.
- Ejecutar create, update y eliminación lógica de Currency con
  `ITransactionRunner`.
- Mantener el camino actual para cualquier otra clave de catálogo.
- Definir operación `Disabled` cuando `IsActive=false` y `Deleted` para
  eliminación lógica.

### Persistence

- Agregar overloads transaccionales compatibles en
  `IFinancialCatalogRepository`/`FinancialCatalogRepository`, limitados por los
  handlers a Currency.
- Releer el estado persistido dentro de la misma conexión y transacción.
- Sustituir la adopción por código en `CurrencySyncApplyRepository` por:

```text
GlobalId encontrado -> actualizar esa identidad.
GlobalId ausente y Code libre -> insertar.
GlobalId ausente y Code ocupado, activo o tombstone -> conflicto terminal.
```

- Preservar `SyncInbox` y la idempotencia por `EventId`.

### SQL tenant propuesto — 136

`136_tenant_currency_transactional_outbox.sql` deberá:

- exigir `Currencies`, `LocalOutbox`, `SyncInbox`, `SyncAudit` y
  `SchemaHistory`;
- detenerse ante códigos duplicados existentes;
- reservar `Code` incluyendo tombstones mediante índice único no filtrado;
- actualizar el procedimiento de existencia para consultar activos y
  eliminados;
- alinear procedimientos CRUD con el contrato transaccional;
- crear un procedimiento de aplicación Currency por `GlobalId`;
- devolver un resultado explícito de conflicto de código sin modificar la fila
  existente;
- ser forward-only e idempotente;
- registrar una sola versión;
- no habilitar workers ni perfiles.

### SQL Master propuesto — 137

`137_master_currency_transactional_registration.sql` deberá:

- alinear la definición Currency con LocalOutbox transaccional y colisión
  terminal;
- preservar orden 40;
- preservar `Currency -> PriceList`;
- crear configuraciones faltantes deshabilitadas;
- no cambiar configuraciones o rutas existentes;
- registrar una sola versión en `MasterSchemaHistory`.

### API y frontend

El contrato HTTP actual puede conservarse para el endurecimiento
transaccional. Existe una brecha funcional separada:

- el DTO/payload contiene `Symbol` e `IsBaseCurrency`;
- create/update, request WinForms y formulario no permiten editarlos;
- el formulario usa `SimpleButton` directo, aunque las reglas actuales
  priorizan controles corporativos.

Esta brecha no se incorpora automáticamente a 8.5. Requiere decisión del
propietario para evitar mezclar sincronización con rediseño CRUD/Designer.

## Identidad, dependencias y política

| Decisión | Contrato propuesto |
|---|---|
| EntityName | `Currencies` |
| Identidad distribuida | `GlobalId` |
| Identidad local | `CurrencyId` |
| Código funcional | `Code`, máximo 3 |
| Dirección | `MasterToBranch` |
| Fuente de verdad | Matriz NuanSystem |
| Conflicto | terminal, sin adopción automática |
| Tombstone | reserva permanentemente `Code` |
| Dependencias | ninguna anterior; PriceList depende de Currency |
| Payload | versión actual de `CurrencySyncPayload` |
| Operaciones | Created, Updated, Disabled, Deleted |
| Perfil/relay/worker | deshabilitados por defecto |

## Árboles de decisión

### Productor

```text
catalogKey != currencies
  -> CRUD financiero existente, sin LocalOutbox Currency.

catalogKey == currencies
  -> tenant Master con SyncEnabled?
       No -> CRUD transaccional sin evento.
       Sí -> Currency + LocalOutbox en la misma transacción.
```

### Aplicador

```text
EventId ya Applied
  -> idempotente, sin segunda escritura.

GlobalId existe
  -> actualizar/deshabilitar/eliminar esa fila.

GlobalId no existe
  -> Code libre, incluido tombstone?
       Sí -> insertar con GlobalId recibido.
       No -> conflicto terminal; no adoptar ni modificar.
```

### Fallos

```text
Falla CRUD o LocalOutbox antes del commit
  -> rollback completo.

Master no disponible después del commit tenant
  -> CRUD conserva éxito; LocalOutbox reintenta.

Mismo EventId y mismo contenido promovido
  -> idempotente.

Mismo EventId con identidad/contenido incompatible
  -> Conflict terminal.
```

## Matriz de validación futura

### Automática

- Otros FinancialCatalogs no crean LocalOutbox.
- Currency create/update/disable/delete crean un evento dentro de la
  transacción.
- Fallo del writer revierte Currency.
- Payload conserva GlobalId, Code, Name, Symbol, Description,
  IsBaseCurrency, estado y referencias externas.
- Promoción repetida no duplica evento ni targets.
- Aplicador usa GlobalId.
- Colisión de Code es terminal y no adopta.
- Tombstone impide reutilizar Code.
- Segundo EventId idempotente según contrato.
- SQL 136/137 y sus inicializadores permanecen alineados.

### SQL/runtime, sujeto a autorización independiente

- respaldos verificados de Master, DEMO y la sucursal piloto;
- scripts 136/137 ejecutados dos veces;
- una sola versión y cero objetos duplicados;
- workers deshabilitados durante despliegue;
- fixtures inequívocos;
- create, update, disable y delete lógico;
- rollback atómico;
- Master no disponible;
- promoción idempotente;
- aplicación en una única sucursal;
- tombstone y colisión terminal;
- PriceList no procesada;
- Cañaris solo lectura;
- cero llamadas SAP/SRI;
- restauración exacta de configuración temporal;
- limpieza de fixtures, locks y procesos.

## Riesgos

1. El repositorio genérico puede afectar catálogos no relacionados si los
   overloads o handlers no se aíslan por `catalogKey`.
2. La migración 090 permite reutilizar códigos eliminados y el aplicador adopta
   por código.
3. Currency es referencia de PriceList, BusinessPartner, Item y documentos;
   la eliminación debe respetar restricciones existentes y nunca borrar
   físicamente.
4. `IsBaseCurrency` es una regla empresarial sensible que no está administrada
   por el CRUD actual.
5. El seed USD/EUR y posibles referencias SAP pueden producir colisiones reales
   en una sucursal; deben auditarse antes del piloto.
6. La documentación de entidades implementadas todavía describía Warehouse
   como pendiente de runtime y deberá actualizarse junto con esta fase.

## Decisiones requeridas del propietario

1. Confirmar Remigio como única sucursal piloto y Cañaris en solo lectura.
2. Confirmar que 8.5 se limita al endurecimiento transaccional/sync, dejando
   símbolo y moneda base fuera del CRUD visual.
3. Confirmar que un `Code` existente con otro `GlobalId`, incluso USD/EUR o
   tombstone, será conflicto terminal sin adopción automática.
4. Confirmar que las referencias `ExternalSystem`, `ExternalCode` y `SapCode`
   se preservan/replican cuando existan, sin llamar SAP.
5. Confirmar que PriceList queda fuera hasta cerrar Currency.

## Plan de commits propuesto

1. `docs(sync): define Currency transactional discovery`
2. `feat(currency): write CRUD and local outbox atomically`
3. `fix(currency): reject branch code adoption and reserve tombstones`
4. `feat(sql): add Currency transactional sync contracts`
5. `test(currency): verify transactional publishing and apply conflicts`
6. `docs(sync): record Currency SQL and runtime validation`

Los commits 2 a 6 requieren aprobación posterior. Este discovery no los
autoriza.
