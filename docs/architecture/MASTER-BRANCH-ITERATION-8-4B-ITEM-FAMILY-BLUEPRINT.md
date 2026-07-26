# Fase 8.4B-1 — ItemFamily Matriz-Sucursal

## Estado

- **Fecha:** 2026-07-25.
- **Rama:** `refactor/codex-skills-v8-4b-item-family`.
- **Estado:** implementacion de codigo; despliegue SQL y runtime pendientes de
  autorizacion independiente.
- **Predecesor:** Item 8.4A integrado y validado.
- **Siguiente dependencia:** Item 8.4B no puede aplicar familias hasta cerrar
  esta fase.

## Decisiones aprobadas

1. `ItemFamily` es un maestro independiente y dependiente de `ItemGroup`.
2. La identidad entre tenants es `GlobalId`.
3. La unicidad local continua siendo `(ItemGroupId, Code)`.
4. La Matriz es la fuente de verdad y la direccion es `MasterToBranch`.
5. Una colision de codigo con otro `GlobalId` es terminal y pasa a
   `DeadLetter`; no existe adopcion automatica.
6. Create, update, disable y delete logico escriben `LocalOutbox` en la misma
   transaccion tenant.
7. La fuente Full pagina mediante `ItemGroupCode|ItemFamilyCode`.
8. La configuracion queda deshabilitada por defecto.
9. Remigio es la sucursal piloto futura. DEMO, Cañaris y cualquier otra base
   quedan fuera de una aplicacion real hasta una autorizacion que las nombre.
10. SAP, SRI, Item, Warehouse, stock, precios y costos quedan fuera del alcance.

## Flujo implementado

```text
ItemFamily CRUD en Matriz
  -> ITransactionRunner tenant
       -> persistencia ItemFamily
       -> relectura con ItemGroupGlobalId
       -> LocalOutbox con el mismo commit
  -> relay local deshabilitado por defecto
  -> SyncOutbox Master
  -> dependencia ItemGroups
  -> ItemFamilySyncEventApplier
  -> SyncInbox + SP_NA_POST_ITEM_FAMILY_SYNC_APPLY
  -> Applied o DeadLetter terminal
```

## Contrato de payload

El payload replica exclusivamente:

- `GlobalId`;
- `ItemGroupGlobalId` y codigo del grupo como evidencia;
- codigo, nombre y descripcion;
- estado activo/inactivo;
- `SapFamilyCode`, `SapCode`, `ExternalSystem` y `ExternalCode`;
- fechas de creacion y actualizacion.

No replica identificadores enteros locales, usuarios de auditoria, Items,
existencias, bodegas, precios ni costos.

## Persistencia

### Tenant 127

`127_tenant_item_family_master_branch_sync.sql`:

- garantiza `GlobalId` no nulo y unico;
- conserva referencias externas opcionales;
- repara las proyecciones CRUD para incluir `ItemGroupGlobalId`;
- permite crear ItemFamily con `GlobalId` generado por Application;
- crea el procedimiento idempotente de aplicacion;
- resuelve el grupo solamente por `ItemGroupGlobalId`;
- devuelve conflicto terminal cuando el codigo ya pertenece a otro
  `GlobalId`;
- no adopta registros por codigo.

### Master 128

`128_master_item_family_sync_registration.sql`:

- registra `ItemFamilies` con orden 207;
- declara `ItemGroups -> ItemFamilies -> Item`;
- crea configuracion `MasterToBranch`, `MasterWins`;
- mantiene configuracion y ownership deshabilitados;
- no activa perfiles, rutas ni workers.

## Quality gates de codigo

- mutacion y `LocalOutbox` comparten conexion y transaccion;
- fallo de outbox revierte la mutacion;
- update inactivo publica `Disabled`;
- delete logico publica `Deleted`;
- dependencia ausente es reintentable;
- colision de codigo es terminal sin reintento;
- conflicto terminal cierra el target como `DeadLetter`;
- paginacion Full usa clave compuesta estable;
- inicializadores, instaladores y DI incluyen los contratos;
- build y pruebas completas deben aprobar.

## Gates pendientes de autorizacion

Antes de ejecutar SQL o runtime se requiere una instruccion que nombre
expresamente las bases permitidas. El piloto recomendado es:

1. respaldo verificado de `NuanSystem_Master` y la base tenant de Remigio;
2. script 128 dos veces en Master;
3. script 127 dos veces en Matriz y Remigio, solo si ambas bases son
   expresamente autorizadas;
4. workers deshabilitados durante el despliegue;
5. fixture identificable en Matriz;
6. aplicacion create/update/disable/delete en Remigio;
7. dependencia ausente y colision terminal controladas;
8. segunda promocion idempotente;
9. limpieza exacta y comparacion de snapshots;
10. Cañaris y DEMO en solo lectura si el propietario lo autoriza.

Hasta cerrar esos gates no debe declararse la fase validada en runtime ni
habilitarse `ItemFamilies` en perfiles permanentes.
