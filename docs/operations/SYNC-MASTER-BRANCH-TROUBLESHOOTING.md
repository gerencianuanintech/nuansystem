# Troubleshooting Sync Master/Sucursal

Esta guia resume sintomas frecuentes, causas probables y acciones seguras. No habilita cambios manuales de payload, identidad ni estados fuera de los endpoints autorizados.

## 1. Evento queda `Pending`

Causas probables:

- Worker apagado.
- `SkeletonMode=true` + `ObserveOnly`.
- No hay targets para la sucursal.
- `SyncEntityConfiguration` deshabilitada.
- `SyncDistributionRules` no aplican.
- Empresa o sucursal con `SyncEnabled = false`.

Acciones:

- Revisar logs del worker.
- Consultar `GET /api/sync/dashboard`.
- Consultar `GET /api/sync/outbox/{id}/targets`.
- Validar configuracion de empresa, entidad y reglas.
- Confirmar si `ObserveOnly` es intencional.

## 2. Evento queda `InProcess`

Causas probables:

- Worker detenido durante procesamiento.
- Lock tecnico vigente.
- Lock tecnico vencido.
- Error de infraestructura mientras el evento estaba reclamado.

Acciones:

- Revisar `LockedBy`, `LockedAt` y `LockExpiresAt`.
- Esperar vencimiento si el lock esta vigente.
- Usar Monitor Sync o `POST /api/sync/outbox/{id}/release-expired-lock` solo si el lock vencio y el usuario tiene `SYNC.OUTBOX.RELEASE_LOCK`.
- No liberar locks vigentes.
- Revisar auditoria antes de reintentar.

## 3. Evento queda `Error`

Causas probables:

- Fallo de conexion con sucursal.
- Payload invalido o incompatible con aplicador.
- Aplicador no habilitado.
- Constraint en destino.
- `GlobalId` duplicado o datos destino inconsistentes.

Acciones:

- Revisar `LastErrorMessage`.
- Revisar `GET /api/sync/audit`.
- Revisar targets con `GET /api/sync/outbox/{id}/targets`.
- Corregir la causa tecnica o de datos.
- Usar Monitor Sync o `POST /api/sync/outbox/{id}/retry` cuando el evento sea reprocesable y el usuario tenga `SYNC.OUTBOX.RETRY`.
- No editar `PayloadJson` para forzar reproceso.

## 4. Evento queda `DeadLetter`

Causas probables:

- `MaxAttempts` agotado.
- Error definitivo o clasificado como no reprocesable.
- Datos destino requieren correccion manual controlada.

Acciones:

- Revisar `LastErrorMessage`, auditoria y logs.
- Corregir causa raiz.
- Usar Monitor Sync o `POST /api/sync/outbox/{id}/retry-deadletter` con motivo obligatorio y permiso `SYNC.OUTBOX.RETRY_DEADLETTER`.
- Monitorear que vuelva a `Pending` y luego a `Applied` o `Error`.
- Detener el worker o volver a `SkeletonMode=true` + `ObserveOnly` si el problema amenaza datos productivos.
- No reintentar masivamente sin analisis.

## 5. Duplicados

Causas probables:

- `GlobalId` incorrecto.
- Aplicacion manual previa en sucursal.
- Datos antiguos sin `GlobalId` correcto.
- Indice unico por `GlobalId` ausente o no aplicado.

Acciones:

- No cambiar `GlobalId` sin analisis.
- Revisar `SyncInbox` por `EventId`.
- Revisar indice unico de `GlobalId`.
- Comparar registros Master/sucursal por `GlobalId` y `Code`.
- Documentar correccion de datos antes de reintentar.

## 6. Item sincronizo pero no hay stock

Esto es correcto por diseno.

El aplicador actual de `Item` sincroniza solo maestro de articulo. No sincroniza stock, kardex, movimientos, lotes, series, vencimientos, costos, precios por lista, bodegas ni disponibilidad. Esos dominios pertenecen a fases separadas porque afectan inventario operativo, costos y consistencia transaccional.

## 7. `SapCode` vacio

Esto es correcto para clientes Standalone y para entidades sin referencia SAP.

- `SapCode` es referencia externa opcional.
- `SapCode` no es identidad de sincronizacion.
- La sincronizacion usa `GlobalId`.
- `SapCode` vacio no bloquea Sync Master/Sucursal.
- SAP no participa en el flujo Master/Sucursal.

## 8. Warehouse sincronizo pero no hay stock

Esto es correcto por diseno.

El aplicador de `Warehouse` sincroniza solo maestro de bodega. No sincroniza stock, saldos, costos, kardex, movimientos, ubicaciones internas avanzadas, lotes, series ni transferencias.

## 9. SkeletonMode no procesa

Esto es correcto si el worker esta en `SkeletonMode=true` + `ObserveOnly`.

Opciones:

- Mantener `ObserveOnly` para despliegue inicial seguro.
- Usar `ClaimAndRelease` para dry-run tecnico con liberacion.
- Usar `SkeletonMode=false` solo en piloto controlado y con entidades habilitadas.
- No usar `ClaimAndIgnore` salvo intencion explicita de cerrar eventos como ignorados.

## 10. Logs y Evidencia

Revisar:

- Logs de `NuanSystem.MasterBranchSyncWorker`.
- `GET /api/sync/dashboard`.
- `GET /api/sync/summary`.
- `GET /api/sync/outbox`.
- `GET /api/sync/outbox/{id}`.
- `GET /api/sync/outbox/{id}/targets`.
- `GET /api/sync/audit`.

Campos clave:

- `EventId`.
- `EntityName`.
- `EntityGlobalId`.
- `Status`.
- `BranchCompanyId`.
- Estado de target.
- `AttemptCount`.
- `MaxAttempts`.
- `NextRetryAt`.
- `LockedBy`.
- `LockExpiresAt`.
- `LastErrorMessage`.

## 11. Warehouse Sync Pilot - problemas encontrados

### Evento sin targets

Sintoma:

- Existe `SyncOutbox` para `EntityName = Warehouse`.
- No existen filas en `SyncOutboxTargets` para el evento.
- El worker no tiene sucursal destino aplicable.

Causas probables:

- `SyncDistributionRules` deshabilitada o incompleta.
- Sucursal inactiva o sin `SyncEnabled`.
- Error tecnico durante la evaluacion de reglas.
- Bug por alias SQL reservado en la consulta de reglas de distribucion.

Acciones:

- Validar `SyncEntityConfigurations` para `Warehouse`.
- Validar `SyncDistributionRules` para `CompanyId`, `EntityName` y `BranchCompanyId`.
- Confirmar que la sucursal esta `IsActive = 1` y `SyncEnabled = 1`.
- Revisar logs y auditoria de publicacion.
- No crear targets manualmente sin procedimiento aprobado.

### Duplicado funcional `10004`

Sintoma:

- Evento `10004` quedo `Pending`.
- El evento correspondia al mismo `Warehouse` que luego fue aplicado correctamente con `10005`.
- No tenia targets ni auditoria.

Accion aplicada en piloto:

- Se marco `10004` como `Ignored` con auditoria manual controlada.
- El motivo fue que `10005` reemplazo funcionalmente el mismo `EntityName`, `EntityCode` y `EntityGlobalId`.

Regla:

- No marcar eventos como `Ignored` sin validar que existe un evento posterior `Applied` para la misma entidad y operacion equivalente.
- No tocar `PayloadJson`, `EntityGlobalId` ni `GlobalId`.

### Falla por `Security:EncryptionKey`

Sintoma:

- El worker no puede resolver configuracion protegida.
- La ejecucion falla antes de aplicar el evento.

Acciones:

- Configurar `Security:EncryptionKey` por variable de entorno, secreto del servicio o mecanismo seguro del ambiente.
- En Windows Service, preferir `Security__EncryptionKey` en el entorno del servicio o en el proveedor de secretos aprobado.
- No escribir la clave en codigo fuente.
- Validar la ejecucion del worker con el mismo usuario del servicio.

### Falla por cifrado SQL

Sintoma:

- La conexion SQL falla por requerimiento de cifrado o certificado no confiable.
- Puede ocurrir tanto al conectar con `NuanSystem_Master` como al resolver la base de sucursal.

Acciones:

- Definir si el ambiente usara `Encrypt=True` con certificado valido.
- La politica normalizada del worker se controla con `SqlConnectionPolicy__Encrypt` y `SqlConnectionPolicy__TrustServerCertificate`.
- Default seguro: `SqlConnectionPolicy__Encrypt=true` y `SqlConnectionPolicy__TrustServerCertificate=false`.
- En piloto local, usar excepciones como `SqlConnectionPolicy__TrustServerCertificate=true` solo por variable de entorno/proceso cuando el ambiente lo requiera.
- No modificar archivos permanentes solo para una prueba local.
- Validar la conexion a Master y a cada sucursal antes de ejecutar `SkeletonMode=false`.

Variables requeridas para el worker:

```text
ConnectionStrings__SqlServerAdmin=Server=<host>,<port>;Database=NuanSystem_Master;User Id=<worker-user>;Password=<secret>;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=False
Security__EncryptionKey=<secret>
SqlConnectionPolicy__Encrypt=true
SqlConnectionPolicy__TrustServerCertificate=false
```

Matriz recomendada:

| Ambiente | Encrypt | TrustServerCertificate | Nota |
|---|---:|---:|---|
| Desarrollo local | true | true solo si no hay certificado confiable | Usar secreto local o variable de proceso. |
| QA/Staging | true | false | Validar certificado antes del piloto. |
| Produccion | true | false | Obligatorio usar certificado SQL confiable. |

### Bloqueo por diferencia de usuario/proceso

Sintoma:

- Los diagnosticos en `testhost.exe` abren correctamente la conexion Master.
- El proceso real `NuanSystem.MasterBranchSyncWorker` falla al abrir la misma conexion con error de cifrado SQL.
- La configuracion efectiva muestra `Encrypt=True` y `TrustServerCertificate=True`.

Causa probable:

- Diferencia de contexto entre el usuario/proceso que ejecuta las pruebas y el usuario/proceso que ejecuta el worker.
- En la prueba controlada, `testhost.exe` bajo el usuario Windows real `proye` conecto correctamente, mientras el worker ejecutado desde Codex bajo `CodexSandboxOffline` fallo antes de `ReleaseExpiredLocksAsync`.
- Esto apunta al contexto de usuario, entorno, certificados o capacidad TLS disponible para ese proceso, no a la logica de Warehouse Sync.

Acciones:

- Ejecutar la prueba final desde una consola normal del usuario Windows real o desde la cuenta de servicio definitiva.
- Usar `docs/operations/templates/run-master-branch-worker-local-proye.example.ps1` como plantilla de ejecucion manual controlada.
- Validar el resultado con `docs/operations/templates/validate-warehouse-sync-final-001.sql`.
- No usar `Encrypt=False` como solucion.
- Si se usara Windows Service, validar que la cuenta del servicio tenga el mismo acceso/capacidad TLS/certificados que la cuenta interactiva que conecta correctamente.

### `SyncInbox` sin `EntityCode` fisico

Sintoma:

- Consultas a `SyncInbox` fallan si intentan seleccionar `EntityCode`.

Causa:

- `SyncInbox` almacena `EventId`, `EntityName`, `EntityGlobalId`, `Operation` y `PayloadJson`; no tiene columna fisica `EntityCode`.

Acciones:

- Consultar `SyncInbox` por `EventId` y `EntityGlobalId`.
- Usar `PayloadJson` para diagnostico autorizado del codigo funcional.
- No cambiar el esquema solo para replicar el valor si no hay decision de arquitectura.

### Diferencia entre `NuanSystem_Master` y base operativa `DEMO`

Sintoma:

- Confusion al buscar bodegas en `NuanSystem_Master`.

Causa:

- `NuanSystem_Master` gobierna empresas, configuracion y eventos Sync.
- La bodega operativa vive en la base tenant de la empresa Master `DEMO`.
- La sucursal vive en su propia base operativa.

Acciones:

- Consultar configuracion y `SyncOutbox` en `NuanSystem_Master`.
- Consultar `Warehouses` del Master en la base operativa `DEMO`.
- Consultar `Warehouses` y `SyncInbox` en la base operativa de sucursal.

## Acciones Prohibidas en Diagnostico

- No editar `PayloadJson`.
- No cambiar `EntityGlobalId`.
- No cambiar `EntityName`.
- No usar `SapCode` como llave de correccion.
- No liberar locks vigentes.
- No hacer retry de `Applied` o `Pending`.
- No hacer retry normal de `DeadLetter`; usar la accion especifica con motivo.
- No hacer retry DeadLetter de eventos `Error`.
- No ejecutar acciones masivas desde WinForms.
- No ejecutar el worker desde API.
- No borrar `SyncOutbox`, `SyncInbox` ni auditoria para "limpiar" errores.
- No activar `SkeletonMode=false` directo en produccion sin piloto controlado.
- No usar `ClaimAndIgnore` por defecto.
