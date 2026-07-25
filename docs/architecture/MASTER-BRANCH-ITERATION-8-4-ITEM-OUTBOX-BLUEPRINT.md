# Fase 8.4 — Piloto transactional outbox para Item

## Estado

- **Fecha:** 2026-07-25.
- **Tipo:** sincronización/worker y persistencia transaccional.
- **Riesgo:** alto.
- **Estado:** discovery y diseño; implementación no iniciada.
- **Rama:** `refactor/codex-skills-v8-4-item-outbox`.
- **Predecesor aprobado:** Fases 8.1–8.3 para `BusinessPartner`.

Esta fase migra únicamente el productor incremental de `Item` al límite
transaccional tenant `Item + LocalOutbox`. No autoriza cambios sobre
`Warehouse`, SAP, SRI, stock, costos, precios ni documentos.

## Discovery Record

### Resultado esperado

Un create, update, disable o delete lógico de Item confirmado en el tenant debe
dejar una intención durable `LocalOutbox` dentro del mismo commit. La
indisponibilidad de Master no debe revertir el CRUD ni perder el evento.

### Evidencia inspeccionada

- `CreateItemCommandHandler`, `UpdateItemCommandHandler` y
  `DeleteItemCommandHandler` guardan el Item y después publican directamente a
  Master mediante `ISyncEventPublisher`.
- `ItemSyncPublisher` crea un payload maestro limitado con `GlobalId`, código,
  nombre, clasificación, unidad de inventario, un código de barras y
  referencias externas.
- `ItemRepository` abre conexiones propias. Create/update guardan primero el
  procedimiento principal y después `ItemMasterData` mediante un segundo
  procedimiento sin una transacción exterior compartida.
- `ItemSyncEventApplier` y `ItemSyncApplyRepository` ya implementan Inbox,
  idempotencia por `EventId`/`GlobalId` y aplicación transaccional limitada.
- El catálogo declara `ItemGroups -> Item`, pero el aplicador también intenta
  resolver `ItemFamilies` y `UnitOfMeasures`.
- La implementación de BusinessPartner aporta el patrón aprobado:
  `ITransactionRunner`, overloads de repositorio con conexión/transacción,
  writer local y promoción idempotente con el mismo `EventId`.

### Patrón seleccionado

Reutilizar el límite transaccional de Iteración 8:

```text
Handler Item
  -> ITransactionRunner tenant
       -> validar unicidad dentro de la transacción
       -> guardar núcleo + hijos + ItemMasterData
       -> releer snapshot confirmado dentro de la transacción
       -> guardar LocalOutbox con EventId estable
  -> commit tenant
  -> responder éxito

MasterBranchSyncWorker
  -> claim local con lease
  -> promoción idempotente a SyncOutbox Master
  -> cerrar LocalOutbox
```

### Alternativas rechazadas

- Mantener la publicación directa después del commit: conserva la ventana de
  pérdida ya corregida para BusinessPartner.
- Escribir a Master dentro de la transacción tenant: acopla disponibilidad,
  prolonga locks y exige una transacción distribuida.
- Encolar solo el núcleo del Item antes de guardar `ItemMasterData`: puede
  publicar un snapshot distinto del estado confirmado.
- Ampliar simultáneamente el payload a stock, precios, costos o bodegas:
  mezcla datos operativos con un piloto de maestro.

## Límite funcional del payload

La Fase 8.4 conserva el payload maestro existente. No incorpora:

- existencias ni disponibilidad;
- costos o método de costeo operativo;
- precios ni listas de precios;
- saldos por bodega, mínimos, máximos o ubicaciones;
- lotes, seriales o movimientos;
- configuración SAP adicional;
- ficha `MasterData` completa;
- colecciones completas de códigos de barras.

`SapCode` continúa siendo una referencia opcional. La fase no llama SAP y no
convierte SAP en propietario del Item.

## División obligatoria

### 8.4A — Productor transaccional y promoción ObserveOnly

- Agregar overloads transaccionales al repositorio Item.
- Incluir el guardado de `ItemMasterData` dentro de la misma transacción.
- Crear un writer Item que reutilice `ILocalSyncOutboxRepository`.
- Retirar `ISyncEventPublisher` solamente de create/update/delete de Item.
- Releer el Item dentro de la transacción y construir el payload existente.
- Promover a Master con el relay de Iteración 8.
- Mantener `SkeletonMode=ObserveOnly`; no aplicar en Remigio ni Cañaris.

### 8.4B — Aplicación real en una sucursal

No puede iniciarse automáticamente al aprobar 8.4A. Requiere una decisión y
autorización independientes sobre:

- dependencia operativa de `ItemFamilies`;
- dependencia de `UnitOfMeasures`;
- comportamiento cuando una referencia opcional no existe;
- alcance exacto de códigos de barras;
- política de colisión de código frente a identidad `GlobalId`;
- sucursal piloto y limpieza de fixtures.

Hasta resolver esas decisiones, el applier existente se revisa y prueba, pero
no se habilita contra una sucursal real.

## Cambios previstos para 8.4A

| Capa | Estado previsto |
|---|---|
| Domain | Verificar sin cambios |
| Application | Writer Item y handlers transaccionales |
| Persistence | Overloads con conexión/transacción; lectura y `MasterData` en la unidad |
| API | Verificar contrato HTTP sin cambios |
| SQL tenant | Solo migración forward-only si el contrato real lo exige |
| SQL Master | Sin nuevo contrato esperado |
| Worker | Reutilizar relay existente, deshabilitado por defecto |
| Applier sucursal | Verificar sin activar |
| Frontend | No aplicable |
| Tests | Transacción, payload, fallos, idempotencia y regresión |
| Documentación | Blueprint, plan y evidencia |

## Invariantes

1. Núcleo, hijos persistidos, `ItemMasterData` e intención local se confirman o
   revierten juntos.
2. El handler no usa simultáneamente `LocalOutbox` y publicación directa.
3. La relectura para el payload usa la misma conexión y transacción.
4. El `EventId` se conserva durante cada reintento y promoción.
5. La caída de Master no cambia el resultado de un commit tenant exitoso.
6. Create, update, disable y delete lógico conservan `GlobalId`.
7. El payload no contiene stock, costos, precios ni saldos de bodega.
8. `Item` es el único productor nuevo de esta fase.
9. `Warehouse` permanece sin cambios.
10. SAP y SRI permanecen fuera del flujo.

## Gates de implementación 8.4A

### Gate A — Contrato de código

- pruebas que demuestren una única transacción tenant;
- rollback si falla `LocalOutbox`;
- eliminación del publisher directo en los tres handlers;
- payload actual estable y libre de campos operativos;
- build y suite completa.

### Gate B — SQL autorizado

- inspeccionar el contrato real de procedimientos Item en DEMO;
- crear migración solamente si existe un desalineamiento comprobado;
- ejecutar dos veces únicamente en bases nombradas por el propietario;
- no aplicar scripts a Remigio/Cañaris sin autorización.

### Gate C — Runtime DEMO

- fixtures identificables creados mediante API;
- create/update/disable/delete lógico;
- Master no disponible;
- lease vencido;
- promoción repetida;
- conflicto de `EventId`;
- crash posterior al commit Master;
- relay por variable de proceso y `ObserveOnly`;
- limpieza exacta y snapshot final.

## Criterios de aborto

Detener la fase cuando:

- el repositorio no puede incluir `ItemMasterData` en la misma transacción;
- el snapshot se obtiene después del commit;
- aparecen campos operativos en el payload;
- el handler conserva la publicación directa;
- se intenta modificar `Warehouse`;
- se habilita aplicación real en sucursales durante 8.4A;
- una migración encuentra datos o contratos inesperados;
- un fixture no puede identificarse y retirarse con seguridad;
- se inicia SAP o SRI.

## Criterio de salida documental

La implementación 8.4A puede comenzar cuando el propietario apruebe
explícitamente:

1. Item como único piloto;
2. persistencia completa del agregado local dentro de la transacción;
3. conservación del payload maestro limitado existente;
4. ejecución runtime solamente en DEMO y Master;
5. Remigio y Cañaris en solo lectura;
6. relay temporal en `ObserveOnly`;
7. separación de 8.4B y sus decisiones de dependencias.

