# Prueba piloto Sync Warehouse Master/Sucursal

Este documento cierra la prueba piloto controlada de sincronizacion Master -> Sucursal para el maestro de bodegas (`Warehouse`). La prueba valida el flujo minimo funcional de alta, actualizacion e inactivacion logica usando `SyncOutbox`, `SyncOutboxTargets`, `SyncInbox`, `SyncAudit` y `NuanSystem.MasterBranchSyncWorker`.

## Objetivo

Validar que una bodega creada en el Master se replica hacia una sucursal por `GlobalId`, sin depender de SAP y sin sincronizar stock, kardex, costos ni movimientos.

La prueba cubrio:

- Creacion de bodega.
- Actualizacion de datos maestros.
- Inactivacion logica.
- Idempotencia por `EventId`.
- No duplicidad por `GlobalId` ni por `Code`.
- Auditoria tecnica del proceso.

## Arquitectura de bases

- `NuanSystem_Master`: base de control para empresas, sucursales, configuracion Sync, `SyncOutbox`, `SyncOutboxTargets` y `SyncAudit`.
- Base operativa Master `DEMO`: base tenant donde vive el maestro operativo de bodegas de la empresa Master.
- Base operativa sucursal `NuanSystem_SYNC_WH_BRANCH_TEST`: base tenant destino donde se aplica el evento en `Warehouses` y se registra `SyncInbox`.

La prueba confirma la separacion esperada: `NuanSystem_Master` gobierna configuracion y eventos, mientras las bases operativas contienen los datos de negocio.

## Configuracion requerida

### Companies

- Empresa Master:
  - `CompanyId = 1`
  - `Code = DEMO`
  - `IsMaster = 1`
  - `IsActive = 1`
  - `SyncEnabled = 1`

- Sucursal:
  - `BranchCompanyId = 2`
  - `Code = SYNC-WH-BRANCH-TEST`
  - `BranchCode = WH-TEST`
  - `ParentCompanyId = 1`
  - `IsMaster = 0`
  - `IsActive = 1`
  - `SyncEnabled = 1`
  - `DatabaseName = NuanSystem_SYNC_WH_BRANCH_TEST`

### SyncEntityConfigurations

Configuracion esperada en `NuanSystem_Master`:

- `CompanyId = 1`
- `EntityName = Warehouse`
- `IsEnabled = 1`
- `Direction = MasterToBranch`
- `ConflictPolicy = MasterWins`
- `BatchSize = 100`
- `MaxAttempts = 3`

### SyncDistributionRules

Regla esperada:

- `CompanyId = 1`
- `EntityName = Warehouse`
- `BranchCompanyId = 2`
- `RuleType = All`
- `IsEnabled = 1`

### SyncOutboxTargets

Cada evento replicable de `Warehouse` debe materializar un target para `BranchCompanyId = 2` con estado inicial `Pending`.

## Datos usados

- `Code = BOD-SYNC-002`
- `GlobalId = 838f3baf-9d73-412b-9d46-c6d3d3de623e`
- Nombre inicial: `Bodega Sync Real`
- Nombre actualizado: `Bodega Sync Real Actualizada`
- Ciudad actualizada: `Quito`
- Provincia actualizada: `Pichincha`
- Estado final: `IsActive = 0`

## Flujo probado

### Create

La bodega se creo en Master usando el flujo real de aplicacion. El publicador genero `SyncOutbox` y target para la sucursal.

Resultado esperado:

- Evento `Created` en `Pending`.
- Target para `BranchCompanyId = 2` en `Pending`.
- No existe la bodega aun en sucursal antes de ejecutar el worker.
- Al ejecutar `SkeletonMode=false`, el evento queda `Applied`.
- La bodega existe en sucursal por el mismo `GlobalId`.

Resultado real:

- `SyncOutboxId = 10005`
- `Operation = Created`
- `Status = Applied`
- Sucursal creo `BOD-SYNC-002` con el mismo `GlobalId`.
- `SyncInbox` registro el `EventId` como `Applied`.
- `SyncAudit` registro el claim y aplicacion.

### Update

Se actualizo la bodega en Master usando el flujo real de aplicacion.

Cambios probados:

- `Name = Bodega Sync Real Actualizada`
- `City = Quito`
- `Province = Pichincha`
- `AllowsProduction = true`
- `Description = Prueba update Master Sucursal Warehouse`

Resultado esperado:

- Nuevo evento `Updated` distinto de los eventos de creacion.
- Target para `BranchCompanyId = 2`.
- Al ejecutar `SkeletonMode=false`, la sucursal queda con los valores actualizados.
- No se cambia `GlobalId`.

Resultado real:

- `SyncOutboxId = 10006`
- `Operation = Updated`
- `Status = Applied`
- Sucursal actualizo la bodega por `GlobalId`.
- No hubo duplicados por `GlobalId` ni por `Code`.

### Disabled

Se inactivo logicamente la bodega en Master usando el flujo real de aplicacion.

Cambio probado:

- `IsActive = false`

Resultado esperado:

- Nuevo evento `Disabled`.
- Target para `BranchCompanyId = 2`.
- Al ejecutar `SkeletonMode=false`, la sucursal queda con `IsActive = 0`.
- No hay delete fisico.

Resultado real:

- `SyncOutboxId = 20002`
- `EventId = ba2de4b3-cddf-4873-ac1d-22f0af76d019`
- `Operation = Disabled`
- `Status = Applied`
- `AttemptCount = 1`
- `ProcessedAt = 2026-07-11 13:16:53`
- Target `20002` quedo `Applied`.
- Sucursal quedo con `IsActive = 0`.
- No hubo delete fisico.

## Eventos de la prueba

| SyncOutboxId | Operation | Status | Observacion |
|---:|---|---|---|
| 10005 | Created | Applied | Creacion aplicada correctamente en sucursal. |
| 10006 | Updated | Applied | Actualizacion aplicada correctamente en sucursal. |
| 20002 | Disabled | Applied | Inactivacion logica aplicada correctamente en sucursal. |
| 10004 | Created | Ignored | Evento duplicado funcional reemplazado por `10005`. |

No se usaron los eventos `10003`, `10004`, `10005` ni `10006` para la fase de inactivacion; el evento real de inactivacion fue `20002`.

## Validaciones SQL usadas

### Evento SyncOutbox

```sql
SELECT
    Id,
    EventId,
    EntityName,
    EntityCode,
    EntityGlobalId,
    Operation,
    Status,
    AttemptCount,
    LockedBy,
    LockedAt,
    LockExpiresAt,
    ProcessedAt,
    LastErrorMessage
FROM dbo.SyncOutbox
WHERE Id = @SyncOutboxId;
```

### Target

```sql
SELECT
    Id,
    OutboxId,
    BranchCompanyId,
    Status,
    AttemptCount,
    AppliedAt,
    LastErrorMessage
FROM dbo.SyncOutboxTargets
WHERE OutboxId = @SyncOutboxId;
```

### Bodega en sucursal

```sql
SELECT
    Id,
    GlobalId,
    Code,
    Name,
    IsActive,
    UpdatedAt
FROM NuanSystem_SYNC_WH_BRANCH_TEST.dbo.Warehouses
WHERE GlobalId = '838f3baf-9d73-412b-9d46-c6d3d3de623e'
   OR Code = 'BOD-SYNC-002';
```

### Evidencia de no duplicidad

```sql
SELECT COUNT(*) AS WarehouseCountByGlobalId
FROM NuanSystem_SYNC_WH_BRANCH_TEST.dbo.Warehouses
WHERE GlobalId = '838f3baf-9d73-412b-9d46-c6d3d3de623e';

SELECT COUNT(*) AS WarehouseCountByCode
FROM NuanSystem_SYNC_WH_BRANCH_TEST.dbo.Warehouses
WHERE Code = 'BOD-SYNC-002';
```

Resultado real:

- `WarehouseCountByGlobalId = 1`
- `WarehouseCountByCode = 1`

### SyncInbox

```sql
SELECT
    Id,
    EventId,
    SourceCompanyId,
    EntityName,
    EntityGlobalId,
    Operation,
    Status,
    AttemptCount,
    MaxAttempts,
    ReceivedAt,
    AppliedAt,
    ErrorMessage,
    LastErrorMessage
FROM NuanSystem_SYNC_WH_BRANCH_TEST.dbo.SyncInbox
WHERE EventId = @EventId;
```

Resultado real para `20002`:

- `Id = 10002`
- `SourceCompanyId = 1`
- `EntityName = Warehouse`
- `Operation = Disabled`
- `Status = Applied`
- `AppliedAt = 2026-07-11 13:16:53`
- Sin `ErrorMessage`.
- Sin `LastErrorMessage`.

### SyncAudit

```sql
SELECT *
FROM dbo.SyncAudit
WHERE EventId = @EventId
ORDER BY CreatedAt;
```

Resultado real para `20002`:

- `Claimed`: `Pending -> InProcess`.
- `Applied`: target de `BranchCompanyId = 2`.
- `Applied`: evento aplicado en todos los targets aplicables.

## Restricciones de alcance

La prueba no cubre ni habilita:

- Stock.
- Saldos.
- Kardex.
- Costos.
- Movimientos.
- Transferencias.
- Ubicaciones internas avanzadas.
- Lotes.
- Series.
- SAP.
- SRI.

`Warehouse` se valido solo como maestro de bodega.

## Problemas encontrados

### Bug alias SQL rule

Durante la preparacion se encontro un fallo por uso del alias SQL `rule` en la evaluacion de reglas de distribucion. En SQL Server, ese alias puede entrar en conflicto y provocar error de sintaxis. El resultado operativo fue un evento sin targets.

Correccion permanente:

- `SyncRuleEvaluator` usa el alias seguro `distRule` para `dbo.SyncDistributionRules`.
- La query mantiene los filtros por `CompanyId`, `EntityName`, `IsEnabled`, sucursal activa, `SyncEnabled` y `ParentCompanyId`.
- Los tests contractuales bloquean la reintroduccion de `AS rule` y los tests de `SyncEventPublisher` validan publicacion de targets para `Warehouse` sin duplicarlos.

### Ajuste de Encrypt y TrustServerCertificate

El ambiente de prueba requirio `Encrypt=False;TrustServerCertificate=True` por proceso para la conexion SQL. No se modificaron archivos permanentes.

Recomendacion:

- Definir la politica de cifrado por ambiente.
- Asegurar certificados validos si `Encrypt=True` sera obligatorio en produccion.
- Validar tanto la conexion a `NuanSystem_Master` como la conexion resuelta a cada sucursal.

### Security:EncryptionKey

El worker requiere `Security:EncryptionKey` para resolver datos protegidos de configuracion. En la prueba se uso la clave validada por variable de entorno/proceso.

Recomendacion:

- Configurar la clave mediante secreto de ambiente, no en codigo.
- Validar el worker como servicio Windows con la misma clave.

### Bloqueo por diferencia de usuario/proceso

Durante la prueba final limpia de `BOD-SYNC-FINAL-001` se comparo el comportamiento de conexion SQL entre procesos:

- `testhost.exe` ejecutando los diagnosticos bajo el usuario Windows real `proye` pudo abrir la conexion Master.
- El worker real ejecutado desde Codex bajo el usuario `CodexSandboxOffline` fallo al abrir la conexion Master con error de cifrado SQL.
- La politica efectiva llegaba correctamente al worker: `Encrypt=True` y `TrustServerCertificate=True`.
- El bloqueo probable esta en el contexto de usuario/proceso/entorno, no en la logica de Sync Warehouse.

Recomendacion:

- Ejecutar la prueba final desde una consola normal del usuario Windows real o desde la cuenta de servicio definitiva.
- Usar la plantilla `docs/operations/templates/run-master-branch-worker-local-proye.example.ps1` para una ejecucion manual controlada.
- Validar despues con `docs/operations/templates/validate-warehouse-sync-final-001.sql`.
- No usar `Encrypt=False` como solucion.
- Si el worker se instalara como servicio Windows, validar que la cuenta del servicio tenga el mismo acceso y capacidad TLS/certificados que la consola donde la conexion funciona.

### AttemptCount incrementado por fallos tecnicos

Algunos intentos tecnicos previos incrementaron `AttemptCount`, aun cuando el problema fue de infraestructura o configuracion. Para la prueba real se genero un evento limpio y se evito reutilizar eventos con reintentos agotados.

Recomendacion:

- No usar eventos de dry-run o diagnostico como prueba final.
- Documentar cada intento que cambie `AttemptCount`.
- Usar acciones operativas de retry solo cuando aplique.

### SyncInbox sin EntityCode fisico

`SyncInbox` en sucursal no tiene una columna fisica `EntityCode`. El codigo funcional viaja dentro de `PayloadJson`; la identidad tecnica de replicacion es `EntityGlobalId`.

Recomendacion:

- No asumir `EntityCode` en consultas de `SyncInbox`.
- Buscar por `EventId` y `EntityGlobalId`.
- Consultar `PayloadJson` solo en diagnostico autorizado.

## Recomendaciones antes de produccion

- Corregir el bug de alias SQL en la evaluacion de reglas.
- Validar certificados o politica de cifrado SQL por ambiente.
- Configurar `Security:EncryptionKey` como secreto operativo.
- Ejecutar build y tests antes de cada piloto.
- Ejecutar el piloto primero en `ObserveOnly`, luego `ClaimAndRelease`, y solo despues `SkeletonMode=false`.
- Activar `EnabledEntityAppliers=Warehouse` solo para la ventana controlada.
- Validar que la sucursal este `IsActive=1` y `SyncEnabled=1`.
- Validar que `SyncDistributionRules` materializa targets antes de correr el worker real.
- Validar no duplicidad por `GlobalId` y `Code` antes y despues.
- No activar stock, kardex, costos ni transferencias como parte de `Warehouse`.
- Mantener SAP y SRI fuera del flujo Master/Sucursal.
- Documentar cada evento de prueba con `SyncOutboxId`, `EventId`, estado, target, `SyncInbox` y auditoria.
