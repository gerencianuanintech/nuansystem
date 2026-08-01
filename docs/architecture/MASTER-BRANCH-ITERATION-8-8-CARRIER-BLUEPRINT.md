# Fase 8.8 — Transportistas Matriz–Sucursal

## Estado y autoridad

- **Alcance:** maestro administrativo independiente `Carrier`.
- **Fuente autorizada:** tenant de la empresa Matriz; piloto previsto en `NuanSystem_DEMO`.
- **Destino inicial recomendado:** Remigio; Cañaris requiere incorporación posterior aprobada.
- **Autoridad:** Constitución > Kernel > catálogos > skill `nuansystem-master-branch-sync` > implementación.
- **Estado:** implementación estática completada; sin despliegue ni activación.

Transportistas continúa siendo un vertical propio. No es un `BusinessPartner`,
proveedor, subtipo SAP ni una vista filtrada de terceros.

## Decisiones aprobadas

1. El tenant Matriz es propietario de la mutación; Master solo gobierna y distribuye.
2. El código técnico de entidad es `Carrier` y el nombre visible es `Transportistas`.
3. `GlobalId` es la única identidad compartida entre bases; `Id` permanece local.
4. Se admiten producción incremental y fuente Full, ambas deshabilitadas por defecto.
5. La colisión de `Code` con otro `GlobalId` es terminal y no adopta registros.
6. El código continúa reservado después del tombstone.
7. `IdentificationNumber` conserva el contrato vigente y no se vuelve único.
8. No se agregan campos SAP ni relaciones con `BusinessPartner`.
9. La sucursal registra su propio historial funcional y auditoría técnica.
10. Los datos preexistentes se inspeccionan antes del piloto; cualquier alineación
    de `GlobalId` exige aprobación explícita.
11. Se reutilizan `CarriersForm`, `CarrierEditForm` y el historial corporativo.
12. El piloto runtime y el despliegue SQL requieren autorizaciones separadas.

## Contrato funcional

Campos replicables:

```text
GlobalId
Code
Name
IdentificationTypeCode
IdentificationNumber
Description
IsActive
IsDeleted
CreatedAt
UpdatedAt
```

No viajan el `Id` local, usuarios de auditoría, filas de historial, secretos,
credenciales, referencias SAP ni datos de `BusinessPartner`.

Los códigos de identificación continúan cerrados a `04` RUC, `05` Cédula y
`06` Pasaporte.

## Límite transaccional

```text
Create / Update / Delete lógico
  -> ITransactionRunner tenant
       -> persistir Carrier
       -> releer el snapshot confirmado
       -> registrar AuditCatalogChanges
       -> insertar LocalOutbox con EventId nuevo
  -> commit tenant

LocalOutbox relay, deshabilitado por defecto
  -> promoción idempotente por EventId
  -> SyncOutbox + decisiones + targets en Master
  -> aplicación de sucursal por GlobalId
```

Una caída de Master no invalida el CRUD ya confirmado. El relay conserva el
mismo `EventId` en cada reintento y nunca abre Master dentro de la transacción
tenant.

## Operaciones

- `Inserted`: crea la misma identidad global en la sucursal.
- `Updated`: actualiza el registro encontrado por `GlobalId`.
- `Disabled`: conserva el registro con `IsActive = 0`.
- `Deleted`: conserva tombstone con `IsActive = 0` e `IsDeleted = 1`.

El mismo `EventId` es idempotente. El mismo `EventId` con identidad, operación
o payload diferente es un conflicto terminal.

## Aplicación en sucursal

La aplicación usa una transacción por evento:

1. registra o recupera `SyncInbox` por `EventId`;
2. valida que payload y `EntityGlobalId` coincidan;
3. busca exclusivamente por `GlobalId`;
4. rechaza un `Code` ocupado por otra identidad, incluso si es tombstone;
5. aplica el estado e inserta auditoría funcional con actor
   `MasterBranchSyncWorker`;
6. completa Inbox y auditoría técnica en el mismo commit.

No existe adopción por código, identificación o referencia a terceros.

## Fuente Full

La fuente Full pagina de forma estable y publica únicamente filas del tenant
Matriz. Full sirve para carga inicial y recuperación controlada, no para
revivir tombstones ni reconciliar códigos automáticamente.

## Dependencias y orden

`Carrier` no tiene dependencias actuales. Una referencia futura desde compras,
guías o documentos deberá aprobar un contrato independiente por `GlobalId`.

## Frontend

No se crea un segundo mantenimiento. El usuario continúa trabajando con:

- `CarriersForm` para listado, CRUD e historial;
- `CarrierEditForm` para edición;
- perfiles y monitor genéricos de Matriz–Sucursal para configuración y soporte.

## Quality gates de código

- Carrier + `LocalOutbox` se guardan en un único commit tenant.
- Los handlers no publican directamente a Master.
- Payload y fuente Full no exponen `Id` local ni auditoría personal.
- El aplicador usa `GlobalId` y una colisión por código es terminal.
- El tombstone reserva el código.
- Productor, fuente, catálogo, aplicador y DI existen antes de declarar la entidad operativa.
- Configuración y workers permanecen deshabilitados por defecto.
- Build y pruebas no requieren SAP, SRI ni SQL real.

## Entregables implementados

- `162_tenant_carrier_transactional_outbox.sql`: incorpora `GlobalId`, reserva
  tombstones por código, adapta el CRUD al límite transaccional, agrega fuente
  Full y aplicación atómica con Inbox y auditorías.
- `163_master_carrier_transactional_registration.sql`: registra `Carrier` como
  capacidad independiente y deshabilitada por defecto, sin dependencias ni
  concesiones automáticas.
- Los inicializadores tenant y Master conocen ambas migraciones en ese orden.
- Productor incremental, fuente Full, payload v1, aplicador y registros DI
  comparten el código canónico `Carrier`.

## Gates posteriores con autorización independiente

1. respaldos verificados de las bases nombradas;
2. scripts tenant/Master ejecutados dos veces;
3. metadata, Dapper y constraints reales;
4. inventario de códigos y `GlobalId` preexistentes;
5. rollback atómico Carrier/LocalOutbox;
6. promoción repetida e indisponibilidad de Master;
7. aplicación DEMO → Remigio;
8. disable, tombstone y reserva de código;
9. colisión terminal sin adopción;
10. limpieza de fixtures, restauración de configuración y cero procesos.

## Exclusiones

- SAP, Service Layer, DI API y `NuanSystem.SyncWorker`.
- SRI y `NuanSystem.SriWorker`.
- BusinessPartners y proveedores.
- Documentos, guías, compras y conciliación.
- Activación permanente de perfiles, rutas, relay o appliers.
