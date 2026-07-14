# Checklist de Despliegue Sync Master/Sucursal

Este checklist guia un despliegue controlado de Sync Master/Sucursal. No reemplaza los respaldos, controles de cambio ni procedimientos internos de produccion.

## 1. Precondiciones

- [ ] `dotnet build NuanSystem.sln --no-restore` ejecuta limpio.
- [ ] `dotnet test NuanSystem.sln --no-build` ejecuta limpio.
- [ ] Backup reciente de `NuanSystem_Master`.
- [ ] Backup reciente de cada tenant/sucursal participante.
- [ ] Scripts versionados aplicados en ambiente de prueba.
- [ ] Empresas y sucursales configuradas en Master.
- [ ] `GlobalId` validado en entidades replicables.
- [ ] Permisos Sync asignados a roles operativos.
- [ ] Ventana de despliegue aprobada.

## 2. Scripts

| Script | Base | Validacion |
|---|---|---|
| `database/sql/062_master_tenant_configuration.sql` | Master | Empresas, features, integraciones y ownership. |
| `database/sql/063_tenant_global_ids_and_external_refs.sql` | Master para `Users`/`CompanyParameters` si existen; tenant/sucursal para entidades y catalogos operativos | `GlobalId`, referencias externas e indices idempotentes. |
| `database/sql/064_master_sync_outbox_inbox.sql` | Master | `SyncOutbox`, targets, auditoria, configuraciones y permisos. |
| `database/sql/065_tenant_sync_inbox_local_outbox.sql` | Tenant/sucursal | `SyncInbox`, `LocalOutbox` y auditoria local. |

Checklist:

- [ ] Ejecutar scripts en el orden indicado.
- [ ] Confirmar que cada script es idempotente en una segunda ejecucion de prueba.
- [ ] Validar indices unicos por `EventId` y `GlobalId` donde aplique.
- [ ] Validar que `SapCode` y referencias externas sigan siendo nullable.
- [ ] Validar que `GlobalId` exista y no se modifique manualmente.
- [ ] Confirmar que no cambiaron claves primarias.
- [ ] Registrar fecha, base y responsable de ejecucion.

## 3. Configuracion Master/Sucursal

- [ ] `CompanyOperationMode` correcto: `Standalone`, `SapIntegrated` o `Hybrid`.
- [ ] Empresa Master con `IsMaster = true`.
- [ ] Sucursales con `IsMaster = false`.
- [ ] `ParentCompanyId` apunta al Master correcto.
- [ ] `BranchCode` unico y estable.
- [ ] `SyncEnabled` activado solo donde aplique.
- [ ] `SyncEntityConfigurations` habilita solo entidades del piloto.
- [ ] `SyncDistributionRules` apunta a sucursales esperadas.
- [ ] `TenantFeatures` incluye capacidades necesarias.
- [ ] `EntityOwnershipConfigurations` define source of truth y direccion.
- [ ] SAP, si existe, se mantiene como integracion opcional y no como requisito Sync.

## 4. Configuracion Worker

- [ ] Instalar `NuanSystem.MasterBranchSyncWorker`.
- [ ] Validar `appsettings.json` y appsettings de ambiente.
- [ ] `ConnectionStrings__SqlServerAdmin` configurado como secreto del ambiente o del servicio Windows.
- [ ] `Security__EncryptionKey` configurado como secreto del ambiente o del servicio Windows.
- [ ] `SqlConnectionPolicy__Encrypt` definido segun politica del ambiente.
- [ ] `SqlConnectionPolicy__TrustServerCertificate` definido segun politica del ambiente.
- [ ] Valores por defecto seguros validados: `Encrypt=true` y `TrustServerCertificate=false`.
- [ ] `SkeletonMode = true`.
- [ ] `SkeletonModeBehavior = ObserveOnly`.
- [ ] `EnabledEntityAppliers` contiene solo entidades planeadas.
- [ ] Connection strings de Master y sucursales resueltas por configuracion segura.
- [ ] Usuario del servicio con permisos minimos necesarios.
- [ ] Credenciales fuera de codigo fuente y logs.
- [ ] Carpeta de logs creada y con permisos del servicio.
- [ ] Retencion de logs definida.
- [ ] Servicio Windows configurado para reinicio controlado.

## 5. Prueba en ObserveOnly

- [ ] Iniciar worker.
- [ ] Confirmar en logs que esta en `SkeletonMode=true` + `ObserveOnly`.
- [ ] Crear o identificar evento de prueba.
- [ ] Validar que no llama `ClaimPendingAsync`.
- [ ] Validar que `SyncOutbox` no cambia estado.
- [ ] Validar dashboard y summary por API.
- [ ] Confirmar que no se escribio `SyncInbox` en sucursal.

## 6. Prueba ClaimAndRelease

- [ ] Cambiar temporalmente a `SkeletonModeBehavior = ClaimAndRelease`.
- [ ] Reiniciar worker.
- [ ] Crear evento de prueba no productivo.
- [ ] Validar claim tecnico.
- [ ] Validar release de lock.
- [ ] Confirmar que el evento vuelve a `Pending`.
- [ ] Revisar auditoria `DryRun`.
- [ ] Confirmar que no se aplicaron entidades.
- [ ] Volver a `ObserveOnly` o avanzar a piloto real aprobado.

## 7. Piloto Real `SkeletonMode=false`

- [ ] Aprobar cambio con responsables de negocio y soporte.
- [ ] Activar una sola sucursal.
- [ ] Activar inicialmente solo `BusinessPartner`.
- [ ] Crear/actualizar/desactivar un tercero de prueba.
- [ ] Validar `SyncOutbox`, target, `SyncInbox` y registro destino por `GlobalId`.
- [ ] Confirmar que no se usa `SapCardCode`.
- [ ] Activar `Item` solo despues de validar terceros.
- [ ] Validar que `Item` sincroniza solo maestro.
- [ ] Confirmar que no sincroniza stock, precios, costos, kardex, movimientos de inventario, lotes, series, vencimientos ni bodegas.
- [ ] Activar `Warehouse` solo despues de validar entidades previas o en piloto aislado.
- [ ] Validar que `Warehouse` sincroniza solo maestro de bodega.
- [ ] Confirmar que no sincroniza stock, saldos, costos, kardex, ubicaciones internas avanzadas, lotes, series ni transferencias.
- [ ] Confirmar que no se toca SAP.
- [ ] Confirmar que no se toca SRI ni se descargan XML.

## 8. Validacion Post Despliegue

- [ ] Revisar total `Applied`.
- [ ] Revisar eventos `Error`.
- [ ] Revisar eventos `DeadLetter`.
- [ ] Revisar eventos `Ignored`.
- [ ] Revisar auditoria de acciones manuales.
- [ ] Revisar targets pendientes por sucursal.
- [ ] Revisar locks vencidos.
- [ ] Revisar logs del worker.
- [ ] Validar que no existan duplicados por `GlobalId`.
- [ ] Validar que endpoints de monitoreo respondan con permisos correctos.
- [ ] Validar en WinForms que `Retry` solo se habilite para `Error` con `SYNC.OUTBOX.RETRY`.
- [ ] Validar en WinForms que `Retry DeadLetter` solo se habilite para `DeadLetter` con `SYNC.OUTBOX.RETRY_DEADLETTER` y motivo obligatorio.
- [ ] Validar en WinForms que `Release expired lock` solo se habilite para locks vencidos con `SYNC.OUTBOX.RELEASE_LOCK`.
- [ ] Confirmar que `Applied`, `Pending`, locks vigentes y usuarios sin permiso no habilitan acciones.
- [ ] Confirmar que WinForms no edita `PayloadJson`, `EntityGlobalId` ni `EntityName`.
- [ ] Confirmar que WinForms no ejecuta worker ni expone apply, run, process, dispatch, claim, sync-now o reprocess.

## 9. Checklist especifico Warehouse

- [ ] `Warehouse` existe en `SyncEntityConfigurations`.
- [ ] `SyncEntityConfigurations.IsEnabled = 1`.
- [ ] `SyncEntityConfigurations.Direction = MasterToBranch`.
- [ ] Existe regla activa en `SyncDistributionRules` para `EntityName = Warehouse`.
- [ ] La regla apunta al `BranchCompanyId` esperado.
- [ ] La sucursal esta `IsActive = 1`.
- [ ] La sucursal tiene `SyncEnabled = 1`.
- [ ] La sucursal tiene `ParentCompanyId` apuntando al Master correcto.
- [ ] `BranchCode` esta informado y es estable.
- [ ] La tabla `Warehouses` existe en la base operativa Master.
- [ ] La tabla `Warehouses` existe en la base operativa sucursal.
- [ ] La tabla `SyncInbox` existe en la sucursal.
- [ ] La tabla `SyncOutboxTargets` existe en Master.
- [ ] Al crear/actualizar/inactivar una bodega, se genera target para la sucursal.
- [ ] `EnabledEntityAppliers` del worker contiene `Warehouse`.
- [ ] `BatchSize` de piloto permite aislar el evento de prueba.
- [ ] `Security:EncryptionKey` esta configurado en el ambiente del worker.
- [ ] La conexion SQL a `NuanSystem_Master` es valida.
- [ ] La conexion SQL resuelta a la sucursal es valida.
- [ ] La politica `Encrypt`/certificado SQL es valida para el ambiente.
- [ ] En produccion, `SqlConnectionPolicy__Encrypt=true` y `SqlConnectionPolicy__TrustServerCertificate=false`, con certificado SQL confiable.
- [ ] En piloto local, cualquier excepcion temporal de certificado se configura solo por variables de entorno o secreto local, no en archivos versionados.
- [ ] La bodega tiene `GlobalId` no nulo y no `Guid.Empty`.
- [ ] No existen duplicados por `GlobalId` en Master ni en sucursal.
- [ ] No existen duplicados por `Code` para el caso de prueba.
- [ ] La prueba confirma que `Warehouse` no sincroniza stock, saldos, kardex, costos, lotes, series ni transferencias.
- [ ] La prueba confirma que SAP y SRI no participan.

## 10. Rollback Operativo

- [ ] Detener `NuanSystem.MasterBranchSyncWorker`.
- [ ] Volver configuracion a `SkeletonMode=true` + `ObserveOnly`.
- [ ] No borrar `SyncOutbox`.
- [ ] No editar `PayloadJson`.
- [ ] No cambiar `EntityGlobalId`.
- [ ] Deshabilitar reglas para aislar una sucursal si corresponde.
- [ ] Revisar auditoria antes de cualquier reproceso.
- [ ] Restaurar backup solo si hubo dano de datos y con aprobacion formal.
- [ ] Documentar evento, causa, hora de corte y registros afectados.
