# Fase 8.4C — Warehouse Matriz-Sucursal

## Estado

- Fecha: 2026-07-26.
- Estado: implementación y despliegue SQL validados; runtime pendiente.
- Worker: deshabilitado por defecto.
- Sucursal piloto futura: Remigio, sujeta a autorización separada.

Este estado no autoriza ejecutar los scripts, activar el relay ni aplicar eventos.

## Contrato aprobado

1. El CRUD y la interfaz local permanecen completos.
2. El contrato corporativo replicado se limita a `GlobalId`, `Code`, `Name`, `IsActive`, referencias externas y auditoría temporal.
3. Campos locales adicionales de la sucursal se preservan durante la aplicación.
4. Create, update, cambio de estado y eliminación lógica guardan `Warehouse` y `LocalOutbox` en la misma transacción tenant.
5. `GlobalId` es inmutable.
6. Los códigos de tombstones continúan reservados.
7. Una colisión de código con otro `GlobalId` es terminal y no produce adopción automática.
8. Stock, kardex, asignaciones de usuario, SAP y SRI quedan fuera del alcance.

## Componentes

- Productor tenant: `WarehouseLocalOutboxWriter` y `WarehouseSyncEventFactory`.
- Fuente Full: `WarehouseSyncFullEntitySource`.
- Aplicador: `WarehouseSyncEventApplier` y `WarehouseSyncApplyRepository`.
- Migración tenant: `133_tenant_warehouse_transactional_outbox.sql`.
- Registro Master: `134_master_warehouse_sync_registration.sql`.

## Despliegue SQL validado

- `134` fue ejecutado dos veces en `NuanSystem_Master`.
- `133` fue ejecutado dos veces en `NuanSystem_DEMO` y
  `NuanSystem_DEMO_REMIGIO`.
- Cada versión quedó registrada exactamente una vez.
- Los conteos Warehouse permanecieron en 35 para DEMO y 4 para Remigio.
- `NuanSystem_DEMO_CANARIS` permaneció en solo lectura y no recibió `133`.
- Master ya tenía una configuración Warehouse habilitada antes de esta
  ejecución. El script 134 no la activó ni la desactivó; worker y relay
  permanecen apagados. Debe revisarse expresamente antes del runtime.

## Quality gates pendientes

- prueba de commit atómico y rollback tenant;
- create, update, disable y delete lógico;
- promoción repetida e idempotente;
- preservación de campos locales;
- colisión terminal, `GlobalId` inmutable y reserva de tombstone;
- aplicación controlada DEMO → Remigio;
- limpieza completa de fixtures;
- build y suite completos posteriores al despliegue.
