# Fase 8.4C — Warehouse Matriz-Sucursal

## Estado

- Fecha: 2026-07-26.
- Estado: implementación y migraciones versionadas; despliegue SQL y validación runtime pendientes.
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

## Quality gates pendientes

- respaldos verificados de las bases autorizadas;
- scripts 133 y 134 ejecutados dos veces;
- prueba de commit atómico y rollback tenant;
- create, update, disable y delete lógico;
- promoción repetida e idempotente;
- preservación de campos locales;
- colisión terminal, `GlobalId` inmutable y reserva de tombstone;
- aplicación controlada DEMO → Remigio;
- limpieza completa de fixtures;
- build y suite completos posteriores al despliegue.
