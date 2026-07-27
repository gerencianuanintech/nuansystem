# Fase 8.4C — Warehouse Matriz-Sucursal

## Estado

- Fecha: 2026-07-27.
- Estado: implementación, despliegue SQL y runtime DEMO -> Remigio validados.
- Worker: deshabilitado por defecto.
- Sucursal piloto validada: Remigio.

Este cierre no habilita permanentemente el relay ni los workers.

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
- Corrección de reserva tombstone:
  `135_tenant_warehouse_tombstone_code_reservation.sql`.
- Registro Master: `134_master_warehouse_sync_registration.sql`.

## Despliegue SQL validado

- `134` fue ejecutado dos veces en `NuanSystem_Master`.
- `133` fue ejecutado dos veces en `NuanSystem_DEMO` y
  `NuanSystem_DEMO_REMIGIO`.
- Cada versión quedó registrada exactamente una vez.
- Los conteos Warehouse permanecieron en 35 para DEMO y 4 para Remigio.
- `NuanSystem_DEMO_CANARIS` permaneció en solo lectura y no recibió `133`.
- `135` fue ejecutado dos veces en `NuanSystem_DEMO` y
  `NuanSystem_DEMO_REMIGIO`; quedó una sola versión `20260727.135`, el índice
  único no filtrado `UX_Warehouses_Code` y cero códigos duplicados.
- Cañaris permaneció en solo lectura y no recibió `135`.
- Master ya tenía una configuración Warehouse habilitada antes de esta
  ejecución. El script 134 no la activó ni la desactivó; worker y relay
  permanecen apagados.

## Validación runtime

El piloto utilizó DEMO como Matriz y Remigio como única sucursal temporal.
Se verificaron respaldos `COPY_ONLY WITH CHECKSUM` de Master, DEMO y Remigio
antes del recorrido completo, y respaldos adicionales de DEMO y Remigio antes
de desplegar `135`.

Aprobaron:

- commit atómico y rollback de Warehouse + `LocalOutbox`;
- create, update, disable y delete lógico;
- promoción repetida e idempotente con el mismo `EventId`;
- aplicación controlada DEMO -> Remigio por `GlobalId`;
- preservación de descripción, dirección, responsables y banderas locales de
  Remigio;
- colisión terminal por código sin adopción automática;
- tombstone aplicado y código permanentemente reservado;
- cero targets y cero fixtures en Cañaris;
- limpieza completa de `I8WHRT1`, Inbox, Outbox y auditoría;
- restauración exacta de todas las rutas temporales;
- build con 0 errores y 0 advertencias;
- 524 pruebas aprobadas, 5 diagnósticas omitidas y 0 fallidas.

Durante el primer recorrido se detectaron cuatro perfiles históricos activos
(`SYNC-001`, `SYNC-002`, `SYNC-003` y `SYNC-005`) que también distribuían
Warehouse hacia `SYNC-WH-BRANCH-TEST`. El recorrido se detuvo, los fixtures se
retiraron y, con autorización explícita, esas cuatro matrices se deshabilitaron
temporalmente. El perfil `DEMO-ITEMS-PILOT` operó como `Incremental`, con
distribución `All` exclusivamente hacia Remigio y Cañaris deshabilitada.
Todos los valores originales fueron restaurados al finalizar.

El gate de reserva tombstone detectó que el procedimiento CRUD de `133`
ignoraba filas eliminadas. La migración `135` reemplazó el índice filtrado por
un índice único permanente y corrigió la búsqueda por código. Después del
despliegue, crear, eliminar lógicamente e intentar reutilizar el mismo código
produjo HTTP 400, sin crear una segunda fila.
