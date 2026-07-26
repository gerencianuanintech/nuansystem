# Fase 8.4B-2 — ItemGroup Matriz-Sucursal

## Estado

- Fecha: 2026-07-26.
- Estado: implementación, despliegue SQL y piloto runtime DEMO a Remigio
  validados.
- Worker: deshabilitado por defecto.
- Sucursal piloto validada: Remigio.

Este estado no autoriza una activación permanente del relay o del worker.

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

## Despliegue SQL validado

- `130` fue ejecutado dos veces en `NuanSystem_Master`.
- `129` fue ejecutado dos veces en `NuanSystem_DEMO` y
  `NuanSystem_DEMO_REMIGIO`.
- Cada versión quedó registrada exactamente una vez.
- Los conteos de ItemGroup permanecieron en 5 en ambos tenants.
- `NuanSystem_DEMO_CANARIS` permaneció en solo lectura y no recibió `129`.

## Runtime validado

El piloto controlado usó `NuanSystem_DEMO` como Matriz y
`NuanSystem_DEMO_REMIGIO` como única sucursal destino. Antes de la prueba se
crearon y verificaron estos respaldos:

- `NuanSystem_Master-item-group-runtime-20260726-232603.bak`;
- `NuanSystem_DEMO-item-group-runtime-20260726-232603.bak`;
- `NuanSystem_DEMO_REMIGIO-item-group-runtime-20260726-232603.bak`.

La ruta `ItemGroups` del perfil `DEMO-ITEMS-PILOT` estaba registrada como
`Full`, por lo que el relay incremental produjo inicialmente eventos sin
targets. Esos fixtures fueron retirados. Con autorización expresa se cambió
temporalmente la entidad a `Incremental`, se deshabilitó solamente la matriz
de Cañaris y se repitió el piloto hacia Remigio. Al finalizar se restauraron
exactamente `Full` y ambas matrices activas.

Los gates ejecutados aprobaron:

- create, update, disable y eliminación lógica mediante la API;
- maestro y `LocalOutbox` revertidos juntos ante un fallo controlado;
- cinco eventos locales con cinco `EventId` distintos;
- promoción repetida del mismo `EventId` sin duplicar `SyncOutbox`;
- cuatro eventos aplicados en Remigio;
- tombstone aplicado por `GlobalId`;
- una colisión terminal por `Code`, sin adopción automática;
- cero targets e inbox de fixtures en Cañaris;
- limpieza completa de ItemGroup, LocalOutbox, SyncOutbox, targets, inbox y
  auditorías de los fixtures `I8IGRT1*`;
- cero procesos NuanSystem al finalizar;
- build con cero errores y advertencias;
- 523 pruebas superadas, 5 diagnósticas omitidas y 0 fallidas.

Los siete eventos históricos de BusinessPartner, Item y Warehouse que ya
existían en Master conservaron `AttemptCount=MaxAttempts`, no fueron
reclamables y no fueron modificados.

## Estado operativo posterior

El perfil volvió a `SyncMode=Full`, las rutas de Remigio y Cañaris quedaron
activas y todos los workers permanecen deshabilitados. El piloto valida el
contrato runtime, pero no convierte la configuración temporal en activación
permanente.
