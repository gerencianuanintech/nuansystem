# Plan de validación — Iteración 8 transactional outbox

## Propósito

Validar el límite tenant `LocalOutbox` → Master `SyncOutbox` sin habilitar
entidades adicionales ni confundir promoción con aplicación en sucursal.

Fuente arquitectónica:
[MASTER-BRANCH-ITERATION-8-TRANSACTIONAL-OUTBOX-BLUEPRINT.md](../architecture/MASTER-BRANCH-ITERATION-8-TRANSACTIONAL-OUTBOX-BLUEPRINT.md).

## Separación de gates

### Gate A — Código y contratos

- Build completo sin errores ni advertencias.
- Pruebas unitarias de handler, writer, relay y promoción.
- Pruebas contractuales de SQL, DI, configuración segura y ausencia de SAP/SRI.
- Confirmar que `BusinessPartner` usa un solo camino de publicación.

### Gate B — SQL autorizado

Requiere autorización que nombre las bases.

- Backup/restore policy acordada antes de migrar.
- Script tenant ejecutado dos veces.
- Script Master ejecutado dos veces.
- Historia de schema sin duplicados.
- Objetos, índices y constraints confiables.
- Claims concurrentes exclusivos.
- Lease vencido recuperable.
- Completion por owner.
- Retry y `DeadLetter` acotados.
- Promoción repetida con mismo contenido: idempotente.
- Mismo `EventId` con contenido distinto: conflicto.

### Gate C — Transacción tenant

Usar un fixture creado para la prueba; no reutilizar datos protegidos.

1. Forzar fallo antes de commit: no existe maestro ni `LocalOutbox`.
2. Confirmar éxito: existe maestro y exactamente una intención.
3. Forzar fallo del insert de intención: se revierte el maestro.
4. Create/update/delete lógico conservan operación y snapshot correctos.
5. Dos solicitudes equivalentes no comparten `EventId`.

### Gate D — Master no disponible

1. Confirmar que el CRUD tenant termina con éxito durable.
2. `LocalOutbox` queda elegible y sin lock residual.
3. Retry aumenta de forma acotada y registra error saneado.
4. Restablecer Master y promover el mismo `EventId`.
5. Confirmar una fila `SyncOutbox`, targets únicos y cierre local.

### Gate E — Crash window

Simular commit Master exitoso antes de actualizar `LocalOutbox`.

- El segundo intento recupera `SyncOutbox` por `EventId`.
- No duplica targets, decisiones ni auditoría incompatible.
- Cierra la intención local como `Applied`.

### Gate F — Runtime controlado

Requiere autorización separada.

- Solo una empresa Master y una entidad piloto.
- `SkeletonMode` y relay parten deshabilitados.
- `BusinessPartner` es la única entidad del piloto.
- Una sola sucursal autorizada.
- Cero llamadas SAP y SRI.
- Capturar estados LocalOutbox, SyncOutbox, target, SyncInbox y auditoría.
- Detener procesos y retirar fixtures al terminar.

## Evidencia mínima

```text
Branch/commit:
Bases autorizadas:
Configuración efectiva saneada:
Conteos iniciales/finales:
EventId del fixture:
GlobalId del fixture:
Transiciones LocalOutbox:
Resultado promoción Master:
Targets:
Resultado sucursal:
Auditorías:
Procesos finales:
Git final:
```

No imprimir payload completo, conexiones, tokens, claves, certificados ni datos
personales reales.

## Criterios de aborto

Detener sin avanzar al gate siguiente cuando:

- una segunda ejecución SQL no es idempotente;
- maestro e intención local divergen;
- hay dos owners de un lease vigente;
- una colisión de payload se acepta como idempotente;
- aparecen targets duplicados;
- el relay llama SAP o SRI;
- se habilita otra entidad/empresa/sucursal;
- la limpieza no puede preservar evidencia y datos ajenos.

## Estado

Gate A y el alcance autorizado de Gate B para Fase 8.1 quedaron aprobados. Los
gates C a F del piloto `BusinessPartner` se ejecutaron posteriormente bajo la
autorización específica de Fases 8.2 y 8.3 documentada al final.

## Ejecución autorizada de Fase 8.1 — 2026-07-25

### Alcance ejecutado

- `125_master_sync_outbox_promotion.sql` se ejecutó dos veces en
  `NuanSystem_Master`.
- `124_tenant_local_outbox_relay.sql` se ejecutó dos veces en
  `NuanSystem_DEMO`, `NuanSystem_DEMO_REMIGIO` y
  `NuanSystem_DEMO_CANARIS`.
- Se crearon cuatro respaldos `COPY_ONLY WITH CHECKSUM` y los cuatro aprobaron
  `RESTORE VERIFYONLY WITH CHECKSUM`.
- La conexión usó `Encrypt=true` y `TrustServerCertificate=false`.
- Ningún worker fue iniciado y el relay permaneció deshabilitado.

### Evidencia de objetos e idempotencia

| Base | Versión | Columnas lease | Índice | Procedimientos |
|---|---:|---:|---:|---:|
| `NuanSystem_Master` | `20260725.125` = 1 | N/A | 1 | N/A |
| `NuanSystem_DEMO` | `20260725.124` = 1 | 3 | 1 | 5 |
| `NuanSystem_DEMO_REMIGIO` | `20260725.124` = 1 | 3 | 1 | 5 |
| `NuanSystem_DEMO_CANARIS` | `20260725.124` = 1 | 3 | 1 | 5 |

Tamaños de respaldo reportados por SQL Server:

- Master: 33.656.832 bytes.
- DEMO: 27.357.184 bytes.
- Remigio: 16.871.424 bytes.
- Cañaris: 16.871.424 bytes.

### Validación funcional reversible

En cada tenant se validó dentro de una transacción posteriormente revertida:

- claim con ownership y lease;
- completion como `Applied`;
- retry acotado hasta `DeadLetter`;
- conflicto terminal de `EventId`;
- recuperación de lease vencido;
- auditoría local;
- conteos de `LocalOutbox` y `SyncAudit` idénticos antes y después del rollback.

Resultado final:

- build Release: 0 errores y 0 advertencias;
- pruebas: 490 aprobadas, 5 diagnósticas omitidas y 0 fallidas;
- procesos worker: 0;
- `MasterBranchSyncWorker.Enabled=false`;
- `LocalOutboxRelay.Enabled=false`;
- Git limpio antes del registro documental.

### Límites pendientes

Esta ejecución no valida todavía:

- dos conexiones reclamando simultáneamente;
- caída entre commit Master y cierre tenant;
- promoción runtime mediante `SyncOutboxPromotionRepository`;
- indisponibilidad real de Master;
- handler transaccional de `BusinessPartner`;
- aplicación en sucursal.

Esos escenarios pertenecen a las Fases 8.2 y 8.3 y requieren una autorización
independiente. No se habilitó ningún worker ni se realizaron llamadas SAP/SRI.

## Ejecución autorizada de Fases 8.2 y 8.3 — 2026-07-25

### Alcance y bases

- Rama: `refactor/codex-skills-v8-transactional-outbox`.
- Entidad piloto única: `BusinessPartner`.
- Bases con escritura autorizada: `NuanSystem_Master` y
  `NuanSystem_DEMO`.
- `NuanSystem_DEMO_REMIGIO` y `NuanSystem_DEMO_CANARIS`: solo lectura.
- `MasterBranchSyncWorker` iniciado únicamente de forma temporal.
- Relay habilitado solo mediante variables del proceso.
- `SkeletonMode=ObserveOnly`.
- SAP y SRI excluidos.

Antes de las pruebas se crearon y verificaron respaldos de Master y DEMO. Las
conexiones y claves locales se utilizaron únicamente en memoria y no forman
parte de la evidencia ni de Git.

### Reparación forward-only de DEMO

La primera ejecución funcional reveló un contrato tenant histórico incompleto
para `BusinessPartner`. Se creó
`126_tenant_business_partner_purchase_contract.sql` y se desplegó dos veces
solo en DEMO:

| Evidencia | Resultado |
|---|---:|
| `SchemaHistory` `20260725.126` | 1 |
| Columnas de `BusinessPartnerPurchaseSettings` | 17 |
| Parámetros create | 107 |
| Parámetros update | 108 |
| Proyecciones con identidad global | 2 |
| BusinessPartners preservados durante despliegue | 11 |

La migración restableció la tabla de compras, alineó create/update con Dapper,
preservó los wrappers de dimensiones contables y agregó `GlobalId`,
`ExternalSystem` y `ExternalCode` a list/get-by-id cuando faltaban.

### Matriz de fallos y recuperación

| Escenario | Estado | Evidencia saneada |
|---|---|---|
| Create/update/delete lógico | Validado | 3 maestros, 5 eventos únicos y `GlobalId` coincidente |
| Rollback tenant | Validado | fallo controlado dejó 0 maestro y 0 `LocalOutbox` |
| Master no disponible | Validado | 4 eventos locales permanecieron durables y elegibles |
| Claims concurrentes | Validado | dos relays: 1 claim combinado y 1 owner activo |
| Lease vencido | Validado | 4 leases expirados fueron liberados y reclamados |
| Promoción inicial | Validado | 4 locales Applied y 4 filas Master con `EventId` único |
| Promoción repetida | Validado | 1 fila local y 1 fila Master, sin duplicado |
| Colisión de `EventId` | Validado | local `DeadLetter`; payload original Master preservado |
| Crash después del commit Master | Validado | primer intento Error; segundo Applied; Master=1 |
| Routing en `ObserveOnly` | Validado | eventos Master Pending; ninguna sucursal aplicada |
| SAP/SRI | Validado | cero invocaciones |

Durante el relay se corrigieron dos defectos reales:

1. el host de `MasterBranchSyncWorker` no registraba los servicios de
   Application necesarios para resolver el servicio de promoción;
2. el record posicional `LocalSyncOutboxDto` no podía materializar de forma
   segura el `SELECT item.*` después de agregar columnas de lease.

El segundo defecto cuenta con una prueba Dapper real que materializa el orden
físico del esquema SQL.

### Limpieza y snapshot final

Se retiraron exclusivamente:

- 3 BusinessPartners `I8BP83*`;
- 5 eventos `LocalOutbox`;
- 5 eventos `SyncOutbox` y sus targets, decisiones y auditorías asociadas;
- los triggers temporales de rollback y crash.

Snapshot final:

| Base | LocalOutbox | Elegibles | Locks | Fixtures |
|---|---:|---:|---:|---:|
| `NuanSystem_DEMO` | 0 | 0 | 0 | 0 |
| `NuanSystem_DEMO_REMIGIO` | 0 | 0 | 0 | 0 |
| `NuanSystem_DEMO_CANARIS` | 0 | 0 | 0 | 0 |

Master quedó con 0 fixtures, 0 elegibles y 0 locks del piloto. La migración 126
permanece instalada una sola vez en DEMO.

### Validaciones de código

- pruebas dirigidas de transactional outbox y `BusinessPartner`: 16 aprobadas;
- build Release completo: 0 errores y 0 advertencias;
- suite completa: 493 aprobadas, 5 diagnósticas omitidas y 0 fallidas;
- `git diff --check`: aprobado.

### Límite de aprobación

Las Fases 8.2 y 8.3 quedan aprobadas únicamente para el piloto
`BusinessPartner`. No se inició la Fase 8.4, no se migraron `Item` ni
`Warehouse`, no se realizó push, PR ni integración a `master`.

## Plan de validación de Fase 8.4A — Item

La implementación de código fue aprobada y completada. El contrato completo se
define en
[MASTER-BRANCH-ITERATION-8-4-ITEM-OUTBOX-BLUEPRINT.md](../architecture/MASTER-BRANCH-ITERATION-8-4-ITEM-OUTBOX-BLUEPRINT.md).

Cuando se autorice, se repetirá la matriz aprobada de BusinessPartner con estas
diferencias:

- el commit tenant debe incluir núcleo Item, colecciones guardadas por el
  procedimiento, `ItemMasterData` e intención local;
- el payload debe conservar su alcance maestro limitado;
- las pruebas runtime se ejecutarán con `ObserveOnly`;
- no se aplicará el evento en Remigio ni Cañaris;
- la aplicación real en sucursal pertenece a 8.4B y requiere aprobación
  separada.

Gates pendientes:

| Gate | Estado |
|---|---|
| Discovery y blueprint | Validado |
| Decisiones del propietario | Aprobadas |
| Implementación 8.4A | Completada; commits `e43043d3` y `594341a0` |
| Migración SQL | No requerida según metadatos reales de DEMO |
| Runtime DEMO/Master | Validado: CRUD completo, rollback atómico y promoción idempotente ObserveOnly |
| Aplicación 8.4B en sucursal | No autorizada |
| Warehouse | Fuera de alcance |

La evidencia runtime se encuentra en
[MASTER-BRANCH-ITERATION-8-4-ITEM-OUTBOX-BLUEPRINT.md](../architecture/MASTER-BRANCH-ITERATION-8-4-ITEM-OUTBOX-BLUEPRINT.md).
Los fixtures `I8IT84*` fueron eliminados al terminar y no se aplicaron eventos
en Remigio ni Cañaris.

## Próxima oleada — ItemGroup, Item payload v2/UOM y Warehouse

La implementación y las migraciones están preparadas, pero ninguna de estas
fases está desplegada ni validada en runtime:

| Alcance | Tenant | Master | Estado |
|---|---|---|---|
| ItemGroup | 129 | 130 | SQL y piloto runtime DEMO a Remigio validados |
| Item v2 y UnitOfMeasure | 131 | 132 | SQL y piloto runtime DEMO a Remigio validados |
| Warehouse | 133 | 134 | Código listo; SQL/runtime pendiente |

El orden obligatorio para el piloto es:

1. respaldos verificados de las bases expresamente autorizadas;
2. ejecutar cada script dos veces y comprobar una sola versión;
3. mantener relay y workers deshabilitados;
4. validar ItemGroup antes de ItemFamily/Item;
5. validar UnitOfMeasure antes del payload Item v2;
6. validar Warehouse como flujo independiente;
7. probar rollback atómico, idempotencia, tombstone y colisión terminal;
8. aplicar únicamente DEMO → Remigio cuando exista autorización expresa;
9. limpiar fixtures y confirmar cero locks/procesos.

Cañaris, SAP y SRI permanecen fuera de alcance.

## Despliegue SQL autorizado de la oleada — 2026-07-26

### Respaldos

Se crearon respaldos `COPY_ONLY WITH CHECKSUM` y se verificaron mediante
`RESTORE VERIFYONLY WITH CHECKSUM`:

- `NuanSystem_Master-catalog-wave-20260726-100612.bak`;
- `NuanSystem_DEMO-catalog-wave-20260726-100612.bak`;
- `NuanSystem_DEMO_REMIGIO-catalog-wave-20260726-100612.bak`.

### Scripts y resultado

| Base | Scripts, dos pases cada uno | Resultado |
|---|---|---|
| `NuanSystem_Master` | 130, 132, 134 | Una fila por versión |
| `NuanSystem_DEMO` | 129, 131, 133 | Una fila por versión |
| `NuanSystem_DEMO_REMIGIO` | 129, 131, 133 | Una fila por versión |
| `NuanSystem_DEMO_CANARIS` | Ninguno; solo lectura | Sin versiones 129/131/133 |

Se confirmaron las cinco definiciones Master, las tres dependencias de Item,
los procedimientos de aplicación, la descripción UOM y los índices esperados.
Los conteos de ItemGroup, ItemFamily, Item, UnitOfMeasure, Warehouse y
LocalOutbox permanecieron iguales al snapshot inicial.

Worker, relay, SAP y SRI permanecieron apagados. No hubo aplicación de eventos
ni fixtures. La configuración Warehouse habilitada que ya existía en Master no
fue modificada y debe revisarse antes de cualquier runtime.

Validación de código posterior: build con 0 errores/advertencias y suite con
523 aprobadas, 5 diagnósticas omitidas y 0 fallidas.

## Piloto runtime ItemGroup — 2026-07-26

ItemGroup fue validado con DEMO como Matriz y Remigio como única sucursal
temporal. Aprobaron rollback atómico, create, update, disable, delete lógico,
promoción idempotente, aplicación por `GlobalId`, tombstone y colisión terminal
sin adopción. Cañaris permaneció en solo lectura y recibió cero targets.

La ruta se cambió temporalmente de `Full` a `Incremental` y la matriz de
Cañaris se deshabilitó solamente durante el piloto. Ambos valores originales
fueron restaurados al finalizar. Los fixtures `I8IGRT1*` y toda su trazabilidad
fueron eliminados. Los respaldos runtime verificados se conservan.

## Piloto runtime UnitOfMeasure e Item payload v2 — 2026-07-27

La validación utilizó DEMO como Matriz y Remigio como única sucursal destino.
Se verificaron respaldos `COPY_ONLY WITH CHECKSUM` de Master, DEMO y Remigio
antes de crear fixtures. Cañaris permaneció en solo lectura y recibió cero
targets.

Respaldos conservados:

- `NuanSystem_Master-item-uom-v2-runtime-20260727-000256.bak`;
- `NuanSystem_DEMO-item-uom-v2-runtime-20260727-000256.bak`;
- `NuanSystem_DEMO_REMIGIO-item-uom-v2-runtime-20260727-000256.bak`.

UnitOfMeasure aprobó create/upsert, update, disable, tombstone, reproceso del
mismo `EventId` y colisión terminal sin adopción por código. Como el perfil
piloto completo contiene dependencias históricas que impiden una ejecución
administrativa Full aislada, los eventos UOM identificables se publicaron
directamente en `SyncOutbox` con un único target explícito a Remigio; este
piloto valida el worker/applier, no declara validado el launcher Full del
perfil completo.

Item v2 aprobó rollback atómico del agregado y `LocalOutbox`, create, update,
disable, eliminación lógica, promoción repetida del mismo evento, aplicación
en Remigio y colisión terminal. La fila aplicada resolvió por `GlobalId` y
conservó separadamente ItemGroup, ItemFamily y las tres unidades de medida.

Durante el primer create se detectaron perfiles históricos `SYNC-*` con rutas
Item activas hacia `SYNC-WH-BRANCH-TEST`. La corrida se detuvo, el fixture se
retiró y esas matrices se deshabilitaron temporalmente antes de repetir. El
segundo recorrido tuvo exclusivamente Remigio como target. Todas las rutas,
incluidas las históricas, fueron restauradas exactamente al finalizar.

Los fixtures `I8IURT1`, trazas Inbox/Outbox y el trigger temporal de rollback
fueron eliminados. El snapshot final confirmó cero residuos, cero procesos,
perfil Item/UOM nuevamente Full y ausencia de datos de prueba en Cañaris.

## Piloto runtime Warehouse 8.4C — 2026-07-27

Warehouse fue validado con DEMO como Matriz y Remigio como única sucursal
temporal. Aprobaron rollback atómico, create, update, disable, delete lógico,
promoción idempotente, aplicación por `GlobalId`, preservación de campos
locales, colisión terminal sin adopción y tombstone.

El intento inicial reveló cuatro perfiles históricos activos hacia
`SYNC-WH-BRANCH-TEST`. Los fixtures fueron retirados antes de repetir. Con
autorización explícita se deshabilitaron temporalmente únicamente esas cuatro
matrices; `DEMO-ITEMS-PILOT` quedó temporalmente `Incremental`, con distribución
`All` hacia Remigio y Cañaris deshabilitada. Todas las rutas fueron restauradas
exactamente.

La primera prueba de reserva tombstone detectó reutilización indebida del
código eliminado. La migración tenant `135` corrigió el procedimiento de
existencia y sustituyó el índice filtrado por `UX_Warehouses_Code`, único y no
filtrado. Fue desplegada dos veces en DEMO y Remigio con una sola versión,
cero duplicados y rechazo HTTP 400 al intentar reutilizar el código eliminado.
Cañaris permaneció en solo lectura y no recibió `135`.

Respaldos adicionales de la corrección:

- `NuanSystem_DEMO-warehouse-135-20260727-145314.bak`;
- `NuanSystem_DEMO_REMIGIO-warehouse-135-20260727-145314.bak`.

El cierre confirmó cero fixtures `I8WHRT1`, cero procesos NuanSystem, workers
deshabilitados, rutas originales restauradas, build sin errores ni advertencias
y 524 pruebas aprobadas, 5 diagnósticas omitidas y 0 fallidas.

## Discovery Currency 8.5 — 2026-07-27

Currency fue seleccionada como siguiente entidad porque ya tenía productor
incremental, fuente Full, aplicador e identidad `GlobalId`, y es dependencia
obligatoria de PriceList. El discovery detectó dos contratos que fueron
corregidos antes del piloto:

- el CRUD guarda en tenant y luego publica directamente a Master;
- el aplicador adopta por `Code` cuando no encuentra `GlobalId`.

La implementación migra solo `currencies` a `LocalOutbox` transaccional,
conserva sin cambios los demás FinancialCatalogs y reemplaza la adopción por
conflicto terminal con reserva de tombstone. Los scripts 136 tenant y 137
Master fueron ejecutados dos veces y validados en Master, DEMO, Remigio y
Cañaris según corresponda. Permanecen deshabilitados por defecto. El runtime
fue validado con DEMO como Matriz y Remigio/Cañaris como sucursales piloto:
CRUD completo, rollback atómico, promoción idempotente, aplicación en ambas
sucursales, tombstone y colisión terminal sin adopción. La configuración
temporal volvió a `Full`, se limpiaron todos los fixtures y quedaron cero
procesos y eventos reclamables. PriceList, SAP y SRI permanecieron fuera de
alcance.

El detalle, decisiones aprobadas, evidencia SQL y matriz runtime están en
[MASTER-BRANCH-ITERATION-8-5-CURRENCY-BLUEPRINT.md](../architecture/MASTER-BRANCH-ITERATION-8-5-CURRENCY-BLUEPRINT.md).

## Piloto runtime PriceList 8.6 — 2026-07-27

PriceList fue validada con DEMO como Matriz y Remigio/Cañaris como sucursales
piloto. Los scripts `140` tenant y `141` Master se ejecutaron dos veces, con
una sola versión final por base y sin habilitar configuraciones permanentes.

Se verificaron cuatro respaldos `COPY_ONLY WITH CHECKSUM`:

- `NuanSystem_Master-price-list-86-20260727-142623.bak`;
- `NuanSystem_DEMO-price-list-86-20260727-142623.bak`;
- `NuanSystem_DEMO_REMIGIO-price-list-86-20260727-142623.bak`;
- `NuanSystem_DEMO_CANARIS-price-list-86-20260727-142623.bak`.

El recorrido aprobó rollback conjunto PriceList/LocalOutbox, conflicto de lista
predeterminada, CRUD completo, tombstone, promoción idempotente, resolución de
Currency por `GlobalId` y colisión terminal sin adopción automática. Se
produjeron cinco eventos y diez targets; cada sucursal aplicó cuatro eventos y
cerró uno en `DeadLetter`, conservando la fila local en conflicto.

La repetición de un `EventId` ya promovido no incrementó SyncOutbox, targets ni
SyncInbox. Todos los fixtures `PL86*` se eliminaron, PriceList volvió a `Full`,
las dos rutas originales quedaron activas y no permanecieron procesos,
ejecuciones o eventos reclamables. SAP y SRI no fueron iniciados.

## Plan de Fase 8.7 — Tax

Tax fue seleccionado después de PriceList porque es una dependencia anterior
de Item y PurchaseOrder y conserva dos contratos incompatibles con Iteración
8: no tiene CRUD incremental transaccional y el aplicador genérico puede
adoptar por código.

Las decisiones funcionales, frontend, payload, scripts reservados `144/145`,
gates SQL y matriz runtime se definen en
[MASTER-BRANCH-ITERATION-8-7-TAX-BLUEPRINT.md](../architecture/MASTER-BRANCH-ITERATION-8-7-TAX-BLUEPRINT.md).

Estado:

- discovery: aprobado;
- decisiones del propietario: aprobadas;
- implementación estática: completada;
- build: 0 errores / 0 advertencias;
- pruebas Tax: 7 aprobadas / 0 fallidas;
- suite completa: 546 aprobadas / 5 diagnósticas omitidas / 0 fallidas;
- SQL real: aprobado;
- runtime Matriz-Sucursal: aprobado;
- SAP y SRI: fuera de alcance.

### Gate SQL Tax 8.7 — 2026-07-27

- Los respaldos verificados de Master, DEMO, Remigio y Cañaris se conservaron.
- Los scripts 144 y 145 aprobaron dos pases y dejaron una sola versión por
  base.
- Master conserva Tax deshabilitado y sin grants automáticos.
- Los tres tenants conservan dos filas Tax idénticas, sin duplicados, tasas
  fuera de rango, tombstones, nuevos outbox o nuevas auditorías.
- Se detuvo el cierre al comprobar que `CK_Taxes_Rate` solo exige
  `Rate >= 0`.
- La migración correctiva 146 se ejecutó dos veces en DEMO, Remigio y
  Cañaris, con respaldos nuevos previamente verificados.
- Cada tenant conserva una sola versión 146 y `CK_Taxes_Rate` habilitada,
  confiable y cerrada sobre `0..1`.
- Las pruebas reales dentro de transacciones revertidas rechazaron
  `-0.000001` y `1.000001`.
- Los conteos y huellas de Taxes, LocalOutbox, SyncInbox y auditoría Tax
  coincidieron exactamente antes y después. El gate SQL queda aprobado; el
  runtime Matriz–Sucursal fue autorizado y validado posteriormente.

### Gate runtime Tax 8.7 — 2026-07-27

- Se crearon y verificaron respaldos `COPY_ONLY WITH CHECKSUM` de Master, DEMO,
  Remigio y Cañaris antes de las pruebas.
- La API y `MasterBranchSyncWorker` se iniciaron temporalmente con conexiones,
  `EncryptionKey` y JWT solo en memoria. SAP y SRI permanecieron detenidos.
- Se validaron create, update, disable, eliminación lógica, historial, bloqueo
  por dependencia Item y rollback atómico de Tax junto con LocalOutbox.
- La reserva del código después del tombstone rechazó la recreación.
- Cinco eventos LocalOutbox se promovieron una sola vez a cinco eventos Master
  y diez targets. El replay de un `EventId` existente no creó duplicados.
- Remigio y Cañaris aplicaron por `GlobalId` las cuatro transiciones del
  lifecycle y conservaron el tombstone final.
- Una colisión de código con otro `GlobalId` terminó en `DeadLetter` en ambas
  sucursales, sin adopción automática ni alteración de los registros locales.
- Los siete eventos ajenos preexistentes se protegieron mediante locks de fila
  no mutantes. Su conteo y fingerprint fueron idénticos antes y después.
- Se eliminaron fixtures, rol y grants temporales, auditorías, inbox, outbox y
  triggers de fallo. Tax volvió a `Full`, ambas rutas quedaron habilitadas y
  no permanecieron procesos NuanSystem.
- Las huellas y conteos finales de Taxes, Items, LocalOutbox, SyncInbox y
  auditoría coincidieron exactamente con la línea base en los cuatro ámbitos.
- Build completo: 0 errores / 0 advertencias. Suite: 546 aprobadas, 5
  diagnósticas omitidas y 0 fallidas.

## Gates pendientes de Fase 8.8 — Transportistas

La implementación estática usa `Carrier` como código canónico y conserva el
mantenimiento independiente existente. Antes de cualquier piloto deben
aprobarse separadamente:

1. respaldos verificados de Master y de los tenants nombrados;
2. dos ejecuciones de `162` tenant y `163` Master;
3. inventario previo de códigos repetidos, incluidos tombstones;
4. metadata, constraints y materialización Dapper reales;
5. rollback atómico de Carrier junto con LocalOutbox;
6. promoción repetida por el mismo EventId;
7. aplicación DEMO a Remigio por GlobalId;
8. disable, delete lógico y reserva permanente de Code;
9. colisión terminal sin adopción automática;
10. auditoría técnica/funcional, limpieza y restauración exacta de configuración.

Hasta aprobar esos gates, perfiles, rutas, relay y workers deben permanecer
deshabilitados. SAP, SRI y BusinessPartners quedan fuera del alcance.
