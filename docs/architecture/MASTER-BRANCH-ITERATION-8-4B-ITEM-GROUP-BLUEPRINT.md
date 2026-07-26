# Fase 8.4B-2 — ItemGroup Matriz-Sucursal

## Estado

- Fecha: 2026-07-26.
- Estado: implementación y migraciones versionadas; despliegue SQL y validación runtime pendientes.
- Worker: deshabilitado por defecto.
- Sucursal piloto futura: Remigio, sujeta a autorización separada.

Este estado no autoriza ejecutar los scripts, activar el relay ni aplicar eventos.

## Contrato aprobado

1. `GlobalId` es la única identidad entre tenants.
2. `Code` conserva unicidad local, incluso para registros eliminados lógicamente.
3. Una colisión de código con otro `GlobalId` es terminal y no adopta el registro existente.
4. Create, update, disable y eliminación lógica guardan el maestro y `LocalOutbox` en la misma transacción tenant.
5. El relay promueve el mismo `EventId` de forma idempotente hacia Master.
6. `ItemGroup` precede a `ItemFamily`, y ambas preceden a `Item`.
7. SAP y SRI quedan fuera del alcance.

## Componentes

- Productor tenant: `ItemGroupLocalOutboxWriter` y `ItemGroupSyncEventFactory`.
- Fuente Full: `ItemGroupSyncFullEntitySource`.
- Aplicador: `ItemGroupSyncEventApplier` y `ItemGroupSyncApplyRepository`.
- Migración tenant: `129_tenant_item_group_transactional_outbox.sql`.
- Registro Master: `130_master_item_group_sync_registration.sql`.

## Quality gates pendientes

- respaldos verificados de las bases autorizadas;
- scripts 129 y 130 ejecutados dos veces;
- prueba de commit atómico y rollback tenant;
- promoción repetida e idempotente;
- colisión terminal y reserva del código eliminado;
- aplicación controlada DEMO → Remigio;
- limpieza completa de fixtures;
- build y suite completos posteriores al despliegue.
