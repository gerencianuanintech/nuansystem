# Cities — LocalOutbox transaccional y aplicación jerárquica

## Estado y alcance

- Fecha: 2026-08-04.
- Alcance: catálogo tenant `Cities`, CRUD administrativo y distribución Matriz–Sucursal.
- Migración: `175_tenant_city_transactional_outbox.sql`.
- Estado SQL/runtime: preparado y validable estáticamente; no desplegado.
- Fuera de alcance: lector/importador SAP de ciudades, workers activos, perfiles activos y cambios WinForms.

## Decisiones

1. La identidad entre bases es `City.GlobalId`.
2. Los padres se resuelven exclusivamente por `CountryGlobalId` y `ProvinceGlobalId`.
3. La provincia debe pertenecer al país del payload.
4. El CRUD y la aplicación rechazan la reasignación de una ciudad existente a otro país o provincia; moverla requiere un flujo explícito y una decisión de identidad.
5. `(ProvinceId, Code)` reserva el código incluso después de eliminación lógica.
6. Las referencias externas son opcionales y no sustituyen `GlobalId`.
7. Una referencia externa presente se protege por `(ProvinceId, ExternalSystem, ExternalCode)`.
8. El CRUD y la intención `LocalOutbox` se confirman o revierten en una única transacción tenant.
9. El Full incluye tombstones de ciudades cuyos padres continúan disponibles.
10. No existe adopción por código, nombre o referencia externa, ni truncado silencioso.

## Flujo

```text
Create/Update/Delete City
  -> ITransactionRunner tenant
     -> GeographyRepository (misma conexión/transacción)
     -> lectura del DTO confirmado
     -> CityLocalOutboxWriter (mismo EventId durable en LocalOutbox)
  -> commit tenant

Relay deshabilitado por defecto
  -> SyncOutbox Master
  -> CitySyncEventApplier
  -> SP_NA_POST_CITY_SYNC_APPLY_EVENT
  -> resolución Country/Province por GlobalId
  -> aplicación City por GlobalId + SyncInbox/SyncAudit
```

## Resultados terminales

- `SYNC_CITY_CODE_CONFLICT`: otro `GlobalId` posee el código dentro de la provincia, incluido tombstone.
- `SYNC_CITY_EXTERNAL_CONFLICT`: otro `GlobalId` posee la referencia externa dentro de la provincia.
- `SYNC_CITY_HIERARCHY_CONFLICT`: la provincia del payload no pertenece al país indicado.
- `SYNC_CITY_PARENT_CONFLICT`: el `GlobalId` de City ya pertenece a otros padres.
- Payload, operación o identidades inválidas también son terminales antes de persistencia.

Una dependencia Country/Province aún no disponible produce fallo reintentable; no crea ni adopta el padre. El control de intentos, backoff y DeadLetter pertenece al worker común.

## Compatibilidad y despliegue

La migración es forward-only e idempotente. Antes de desplegar debe comprobar que no existen duplicados históricos de código o referencia externa. No habilita perfiles, relay ni workers. La existencia de la migración no autoriza su ejecución.

La integración SAP de Cities requiere una decisión separada sobre fuente, metadata e identidad externa estable; este contrato no presupone que SAP Business One exponga un catálogo estándar de ciudades.
