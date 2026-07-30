# Fase 10.1 — Plan de validación Bodegas SAP → DEMO

## Estado

- Fecha: 2026-07-30.
- Estado: Fase 10.2 SQL real aprobada y cerrada; las validaciones SAP → DEMO posteriores continúan pendientes y requieren autorización independiente.
- Documento arquitectónico: [SAP-SYNC-PROFILES-BLUEPRINT.md](../architecture/SAP-SYNC-PROFILES-BLUEPRINT.md).
- Fuente: SAP Business One Service Layer.
- Destino único: empresa `DEMO`, base `NuanSystem_DEMO`.
- Worker autorizado por diseño: `NuanSystem.SyncWorker`.
- Fuera de alcance: `NuanSystem.MasterBranchSyncWorker`, Remigio, Cañaris, SRI, SQL/runtime real durante Fase 10.1.

Este documento define qué debe probarse en Fases 10.3–10.9 y conserva el cierre saneado de Fase 10.2. No declara que SAP, Service Layer, SRI, API, WinForms o workers hayan sido ejecutados.

## Evidencia saneada del cierre 10.2

| Requisito autorizado | Estado aprobado | Evidencia conservada |
|---|---|---|
| Respaldo Master y DEMO | Validated | Respaldos `COPY_ONLY WITH CHECKSUM` creados y `RESTORE VERIFYONLY WITH CHECKSUM` conformes. |
| Script Master `152` dos veces | Validated | Solo `NuanSystem_Master`; una versión `20260730.152`. |
| Script tenant `153` dos veces | Validated | Solo `NuanSystem_DEMO`; una versión `20260730.153`; Remigio y Cañaris no fueron tocados. |
| Objetos y contratos Dapper | Validated | Tablas, procedimientos, índices, claves, checks, defaults, auditoría, materialización y locks renovables conformes. |
| Migración legado | Validated | Perfil, entidades y agenda Manual inactivos; sin dual-write; fallback habilitado y dos ciclos exitosos requeridos. |
| Seguridad | Validated | Doce permisos únicos, concedidos únicamente a `ADMIN`; sin formularios ni menús. |
| Idempotencia y concurrencia | Validated | `ExecutionUid`, `RowVersion`, locks vencidos, snapshots tipados y transiciones conformes. |
| Limpieza y regresión | Validated | Snapshots finales conformes, cero fixtures residuales, build y pruebas conformes. |
| Servicios externos/runtime | Not executed | Sin SAP, Service Layer, SRI, API, WinForms ni workers. |

No se conservan aquí rutas de respaldo, hashes completos, cadenas de conexión, credenciales, sesiones ni payloads. Todos los perfiles y agendas nuevos permanecieron inactivos al cierre.

## Objetivo de validación

Demostrar, con evidencia separada por capa, que un perfil SAP independiente puede ejecutar una lectura Full de Bodegas desde Service Layer hacia `NuanSystem_DEMO`, procesar cada registro de forma idempotente y segura, conservar la identidad/campos locales, exigir aprobación ante desactivación SAP y producir historial/retry/heartbeat sin tocar Matriz–Sucursal.

## Baseline verificable

### Implementado actualmente

- `SapServiceLayerWarehouseReader` lee todas las páginas de `Warehouses`, valida HTTPS/configuración, inicia/cierra sesión y mapea `Inactive`/`Locked`.
- `SapWarehouseImportService` previsualiza e importa por registro.
- `CreateWarehouseCommandHandler` genera `GlobalId` si no se provee.
- `UpdateWarehouseCommandHandler` preserva el `GlobalId` recargado desde base.
- La identidad externa de Bodega es `SAP_B1` + `WarehouseCode`.
- Un `Code` local sin vínculo SAP es `Conflict`; no se adopta.
- SAP inactiva frente a local activa no cambia `IsActive`.
- Las pruebas existentes incluyen Full paginado, conflicto, preservación de estado, campos aprobados y segundo ciclo idempotente.

### No implementado actualmente

- Perfil SAP y agenda por entidad.
- Handler `ISapSyncEntityHandler` registrado para Bodegas.
- Historial `SapSyncExecutions`/`SapSyncExecutionDetails`.
- Estado persistido `ApprovalRequired`.
- Retry real por registro de Bodega.
- Propagación efectiva de `BatchSize`/`MaxAttempts` desde perfil.
- Lease renovable y vínculo lock/ejecución.
- Formularios/permisos SAP Profiles/Executions.
- Implementación del resultado `Skipped/SAP_WAREHOUSE_INACTIVE` para bodega SAP nueva inactiva.

## Precondiciones obligatorias para una futura ejecución

No se ejecutará ninguna prueba SQL/runtime hasta que todas estén satisfechas:

1. Rama/commit inmutable, working tree limpio y artefactos identificables.
2. Aprobación explícita del propietario para:
   - bases exactas;
   - llamada SAP exacta;
   - ventana;
   - worker/API que se iniciará;
   - fixtures si fueran imprescindibles;
   - habilitación exacta de Bodegas en la fase autorizada;
3. Backups verificados de `NuanSystem_Master` y `NuanSystem_DEMO`.
4. `NuanSystem.MasterBranchSyncWorker` apagado.
5. `LocalSyncOutboxRelay`/relay Matriz–Sucursal apagado.
6. Remigio y Cañaris excluidos de perfiles, rutas y validación.
7. `NuanSystem.SyncWorker` apagado durante migración/inspección SQL y habilitado solo en la ventana aprobada.
8. Credenciales en configuración protegida/ignorada; ninguna credencial en comandos, capturas o documentos.
9. TLS estricto. `ServiceLayer:IgnoreSslErrors` debe permanecer `false` salvo diagnóstico Development explícitamente aprobado.
10. Perfil SAP de Bodegas inicialmente Manual e inactivo.
11. Query de inventario baseline sin datos sensibles: conteos, hashes/identidades permitidas y configuración.
12. Plan de rollback probado estáticamente y responsables nombrados.

## Matriz de entornos y exclusiones

| Componente | Estado esperado |
|---|---|
| SAP B1 Service Layer | Solo lectura `Warehouses`; Login/GET/Logout. |
| `NuanSystem_Master` | Configuración SAP/perfil/agenda y heartbeat. |
| `NuanSystem_DEMO` | Único tenant con ejecución y mutación Warehouse. |
| Remigio | No consultar, no migrar, no escribir, no crear targets. |
| Cañaris | No consultar, no migrar, no escribir, no crear targets. |
| `NuanSystem.SyncWorker` | Único host SAP; controlado y luego apagado/restaurado. |
| `NuanSystem.MasterBranchSyncWorker` | Siempre apagado durante este plan. |
| SRI Worker/proveedor | No invocado. |

## Dataset de prueba

Preferir datos reales existentes y no destructivos. Si el propietario autoriza fixtures, usar prefijo único, trazable y eliminable, sin reutilizar códigos productivos.

Casos mínimos:

| Caso | SAP | DEMO antes | Resultado esperado |
|---|---|---|---|
| W1 | nueva activa | no existe por identidad ni Code | `Created`, GlobalId nuevo, activa. |
| W2 | vinculada, cambió nombre/dirección | existe por `SapCode` | `Updated`, mismo GlobalId, campos locales preservados. |
| W3 | vinculada, idéntica | existe | `Unchanged`, cero write. |
| W4 | mismo Code sin vínculo SAP | existe solo por Code | `Conflict`, no adopción, cero write. |
| W5 | SAP inactiva | DEMO vinculada activa | `ApprovalRequired`, DEMO sigue activa. |
| W6 | nueva inactiva | no existe | `Skipped`, código seguro `SAP_WAREHOUSE_INACTIVE`, cero create. |
| W7 | código/nombre inválido | no aplicable | `Skipped`/terminal seguro. |
| W8 | un registro provoca fallo SQL transitorio simulado | otros válidos | éxitos conservados; registro `RetryScheduled`. |
| W9 | segundo Full | estado posterior a W1–W8 | sin duplicados; W1/W2 `Unchanged` si no cambió SAP. |

No se crea deliberadamente un conflicto real con una bodega productiva sin aprobación específica.

## Validaciones unitarias

### Agenda y perfil

- Manual no genera ejecución automática.
- Interval calcula por entidad y no por `Worker:LoopDelaySeconds`.
- Daily convierte `ExecutionTime`/IANA a UTC.
- `NextExecutionAtUtc` avanza una sola vez por disparo.
- agenda inactiva o perfil inactivo no crea ejecución.
- `Both` es rechazado por backend y no se muestra en la UI inicial mientras ambos sentidos no estén implementados.
- orden respeta `ExecutionOrder`.
- `BatchSize`, `MaxAttempts` y timeout llegan al contexto/ejecución.
- más entidades/empresas que el límite no sufren starvation.

### Bodegas

- reader solicita `Warehouses?$orderby=WarehouseCode` y sigue todas las páginas.
- mapea `Inactive` o `Locked` a inactiva.
- logout se intenta en éxito, error y cancelación.
- URL no HTTPS, userinfo o settings incompletos fallan de forma segura.
- no se comparte cookie/sesión entre empresas.
- nueva activa → Create con `SAP_B1`, `ExternalCode`, `SapCode`.
- update preserva GlobalId, Code y campos locales.
- mismo Code sin identidad SAP → Conflict.
- SAP inactiva + DEMO activa → ApprovalRequired y no update de estado.
- ausencia del Full no desactiva.
- segundo ciclo no crea/actualiza.
- excepción de un registro no detiene los siguientes.
- resultado no contiene contraseña, cookie, token, URL con userinfo ni payload Login.

### Estados/retry

- transición ilegal es rechazada.
- transient con intentos disponibles → RetryScheduled.
- terminal/conflict/approval no se reintenta automáticamente.
- agotamiento → DeadLetter.
- retry manual crea ejecución hija y conserva original.
- cabecera calcula Completed/Warnings/Errors/Failed desde detalles.
- cancelación termina el registro actual, no inicia el siguiente y cierra la ejecución como `Cancelled`.

### Locks/heartbeat

- dos claims: solo uno adquiere.
- solo owner renueva/libera.
- lock vigente no se libera manualmente.
- lock vencido se recupera y deja auditoría.
- heartbeat publica lifecycle, ciclo, conteos y lease sin secretos.

## Validaciones de integración Application/Persistence

### Master

1. Crear perfil SAP DEMO inactivo.
2. Agregar Bodegas `SapToErp`, Full, batch y max attempts.
3. Agregar agenda Manual.
4. Consultar detalle y verificar auditoría.
5. Simular edición concurrente con `RowVersion`; una debe fallar.
6. Validar unicidad de código por empresa.
7. Validar que no existan sucursales/distribution matrices en el contrato.
8. Activación bloqueada hasta validar handler/config y comprobar `Skipped/SAP_WAREHOUSE_INACTIVE`.

### Tenant DEMO

1. Crear ejecución snapshot sin FK cruzada a Master.
2. Crear un detalle por `SourceRecordKey`.
3. Repetir insert del mismo registro/ejecución: no duplica.
4. Confirmar transacción independiente por registro.
5. Confirmar rollback de un registro fallido sin revertir éxitos previos.
6. Claim/retry con `READPAST`/`UPDLOCK` o procedimiento equivalente.
7. Lock vence/renueva/recupera con owner.
8. Watermark Full avanza solo al cierre durable permitido.
9. Índices soportan listado paginado y claim.
10. `ApprovedSnapshotJson` es nulo o JSON allowlist válido y `SnapshotHash` es nulo o SHA-256 de 32 bytes.
11. No existen procedimientos de purga; la retención de desarrollo es indefinida.

### Separación

- Ningún repositorio SAP consulta `SyncProfiles`.
- Ningún endpoint SAP usa `SYNC.CONFIGURATION.*`.
- Ningún perfil SAP contiene `BranchCompanyId`, Remigio o Cañaris.
- Ninguna prueba inicia `NuanSystem.MasterBranchSyncWorker`.
- La posible fila `LocalOutbox` creada por el comando Warehouse no se promueve ni genera `SyncOutboxTargets`.
- La generación normal de `LocalOutbox` se conserva; relay y `NuanSystem.MasterBranchSyncWorker` permanecen apagados.

## Validación SQL futura

Estado en Fase 10.1: **Not executed**.

Cuando se autorice:

1. Inspeccionar numeración vigente y elegir scripts forward-only.
2. Ejecutar script Master dos veces.
3. Ejecutar script tenant únicamente en `NuanSystem_DEMO`, dos veces.
4. No ejecutar scripts en Remigio/Cañaris.
5. Verificar una sola versión en historial.
6. Verificar tablas, columnas, nullability, defaults, checks, FKs locales e índices.
7. Verificar que `SyncProfiles`, `SyncSchedules`, `SyncProfileExecutions` y tablas Matriz–Sucursal no cambiaron.
8. Migrar `SapSyncEntitySettings` dos veces; no duplicar perfil/entidades.
9. Confirmar perfil migrado inactivo y agenda Manual inactiva.
10. Confirmar `PurchaseOrders/Both` desactivado.
11. Confirmar cero secretos en tablas nuevas.
12. Restaurar desde backup en un ensayo autorizado o validar procedimiento de rollback forward-only.

Consultas de evidencia deben proyectar metadata y conteos, no credenciales ni payloads.

## Validación API futura

### Perfiles SAP

- 401 sin token.
- 403 sin permiso SAP Profiles.
- 403 si el usuario no accede a DEMO.
- crear/editar/validar/activar con códigos de error estables.
- no devuelve `SapPasswordEncrypted`, HANA password, cookies ni connection strings.
- `execute` devuelve Accepted + ejecución/correlación.
- doble `ClientRequestId` devuelve la misma intención o conflicto estable, no dos ejecuciones.

### Ejecuciones SAP

- listado/detail paginados por empresa activa.
- permiso View no concede Retry/Cancel/Release.
- retry y release requieren motivo.
- lock vigente no se libera.
- detalle expone clave SAP permitida y mensaje seguro, no snapshot prohibido.
- exportación de grilla usa proyección segura.

### Regresión

- `/api/sap/warehouses/preview` y `/api/sap/warehouses/import` mantienen compatibilidad acordada.
- `/api/sap/sync-logs` sigue operativo durante transición.
- `/api/sync/configuration/*` conserva permisos/contratos Matriz–Sucursal.
- cambio de empresa no reutiliza IDs, perfiles, sesiones SAP ni páginas previas.

## Validación runtime controlada

Estado en Fase 10.1: **Not executed**.

### Secuencia

1. Confirmar backups y servicios apagados.
2. Confirmar hash/commit de binarios.
3. Confirmar perfil Manual inactivo.
4. Iniciar API si es necesario para comando manual, con configuración externa.
5. Iniciar únicamente `NuanSystem.SyncWorker` en modo controlado o ejecutar el mecanismo manual aprobado que encole para ese worker.
6. Activar perfil/entidad solo durante la ventana.
7. Ejecutar preview Full y revisar decisiones sin mutación.
8. Ejecutar Full manual con límite aprobado.
9. Observar `SapSyncExecutions`, detalles y `WorkerHeartbeat`.
10. Ejecutar el segundo Full.
11. Desactivar perfil y worker.
12. Confirmar ausencia de targets/eventos entregados a sucursales.
13. Limpiar solo fixtures aprobados y conservar evidencia no sensible.
14. Restaurar configuración exacta y comprobar servicios apagados.

### Evidencia de éxito

- Login/GET paginado/Logout contra el endpoint aprobado.
- una sola empresa/tenant: DEMO.
- W1–W9 con resultados esperados.
- GlobalId de filas actualizadas no cambia.
- campos locales no aprobados no cambian.
- cero adopciones por Code.
- ApprovalRequired no cambia actividad.
- segundo ciclo sin mutación.
- retry limitado y visible.
- lock/heartbeat coherentes.
- cero secretos en logs/export/evidencia.
- cero ejecución de `NuanSystem.MasterBranchSyncWorker`.
- cero writes/targets en Remigio/Cañaris.

### Criterios de abortar

- tenant resuelto distinto de DEMO;
- perfil contiene sucursal/ruta no esperada;
- relay o worker Matriz–Sucursal activo;
- TLS inseguro no autorizado;
- credencial/payload Login visible en log;
- adopción automática por Code;
- cambio de GlobalId;
- desactivación automática de DEMO;
- retry sin límite;
- lock duplicado o lease sin owner;
- cualquier write en Remigio/Cañaris;
- conteos o estados no reconciliables.

Al abortar: cancelar nuevos claims, permitir cierre seguro del registro en curso, apagar worker, preservar evidencia, restaurar configuración y no “corregir en caliente” datos sin plan autorizado.

## Validación visual futura

### Perfiles SAP

- caption inequívoco “Perfiles SAP”.
- no aparecen pestañas Sucursales/Distribución.
- empresa, entidad, dirección, Full, batch, attempts, order, agenda, hora, zona y actividad visibles.
- próxima/última ejecución en zona local con UTC disponible en detalle.
- Validar/Activar/Desactivar/Ejecutar según permisos y estado.
- read-only completo en Consultar.

### Ejecuciones SAP

- caption inequívoco “Ejecuciones SAP”.
- filtros de perfil/entidad/dirección/estado/trigger/fecha.
- badges para Completed, Warnings, Errors, ApprovalRequired, Retry y DeadLetter.
- detalle paginado por registro.
- mensajes largos legibles y exportación segura.
- retry/cancel/release solo en estados y permisos válidos.

### Matriz–Sucursal

- sus formularios actuales conservan captions, FormKeys, Ribbon y permisos.
- abrir `sync-profiles` nunca muestra SAP.
- abrir `sap-sync-profiles` nunca muestra sucursales.

Validar Designer de Visual Studio, DPI, resolución mínima, teclado, tab order, loading, empty, error y forbidden. Build correcto no sustituye revisión Designer.

## Escaneo de secretos

### Patrones mínimos

Escanear archivos cambiados, logs y evidencia por:

- `password`, `pwd`, `secret`, `token`, `cookie`, `session`;
- `B1SESSION`, `ROUTEID`;
- `connectionstring`, `Data Source=`, `Server=`, `User Id=`;
- `SapPasswordEncrypted`, `HanaPasswordEncrypted`;
- `Authorization:`, `Bearer `;
- claves privadas/certificados;
- payloads de `/Login`.

Los nombres de campos en documentación pueden aparecer para declarar prohibiciones; los valores nunca.

### Reglas

- No adjuntar `appsettings.Local.json`.
- No imprimir variables de entorno.
- No guardar request/response completos de Login.
- Enmascarar host si la política de evidencia lo exige.
- Logs de error visibles usan código/clase y mensaje saneado.
- El resultado por registro solo incluye campos allowlist.

## Pruebas de regresión mínimas

- handlers existentes Suppliers/Items/PaymentTerms conservan comportamiento.
- PurchaseOrders sigue visible como no implementado y no se activa.
- `SapOutboxWorker` no se presenta como operativo.
- `SapRetryWorker` no entra en loop de cambio de estados sin ejecutar trabajo.
- heartbeat SRI continúa compatible con `WorkerHeartbeat`.
- perfiles/ejecuciones Matriz–Sucursal pasan sus tests existentes.
- Warehouse CRUD y LocalOutbox transaccional pasan regresión.
- warehouse Master–Sucursal no se activa durante el piloto SAP.
- seguridad existente `SAP.SYNC.READ/MANAGE` conserva endpoints legado.

## Quality gates por fase

### 10.2

- contratos y ownership Master/tenant explícitos;
- scripts idempotentes y no ejecutados sin autorización;
- migración inactiva/manual;
- Dapper/SQL alineados;
- ningún cambio a tablas `SyncProfiles`.

### 10.3

- API thin/MediatR;
- validación de empresa/handler/dirección;
- dos contratos de seguridad;
- proyecciones sin secretos;
- 401/403/tenant tests.

### 10.4

- agenda por entidad;
- poll global separado;
- no starvation;
- lock owner/renew/expiry/recovery;
- heartbeat completo;
- un solo worker SAP.

### 10.5

- ejecución/detalle por registro;
- estados parciales;
- retry real y limitado;
- manual retry con motivo/auditoría;
- logs allowlist/saneados.

### 10.6

- Full Service Layer;
- nuevas activas creadas;
- updates allowlist;
- GlobalId/local fields preservados;
- no adopción Code;
- ApprovalRequired;
- W6 produce `Skipped/SAP_WAREHOUSE_INACTIVE` sin crear la bodega;
- segundo ciclo idempotente.

### 10.7

- formularios SAP independientes;
- Designer y controles corporativos;
- FormKeys/permisos/Ribbon separados;
- cliente tipado `INuanApiClient`;
- estados UX completos.

### 10.8

- SQL/runtime solo DEMO autorizados;
- backups;
- Service Layer real controlado;
- no secrets;
- workers Matriz–Sucursal apagados;
- evidencia saneada y restauración final.

### 10.9

- criterio de cutover cumplido;
- fallback retirado sin romper endpoints legado acordados;
- rollback/troubleshooting documentados;
- configuración final deshabilitada salvo aprobación de operación permanente.

## Riesgos operativos

| Riesgo | Señal | Respuesta |
|---|---|---|
| Tenant equivocado | CompanyCode/base no es DEMO | abortar antes de SAP/write. |
| Downstream Matriz–Sucursal | relay/worker activo o targets creados | apagar, preservar evidencia, no continuar. |
| Sesión compartida | cookies cruzadas entre compañías | abortar; revisar named client/session factory. |
| Lease expira durante Full | segunda ejecución entra | abortar; corregir renovación antes de repetir. |
| Datos locales sobrescritos | cambian descripción/flags/contacto/GlobalId | abortar y restaurar desde backup/plan. |
| Regresión de bodega nueva inactiva | W6 se crea en DEMO | abortar; exigir `Skipped/SAP_WAREHOUSE_INACTIVE` antes de activar. |
| Retry infinito | attempts exceden máximo | apagar worker y corregir estado/policy. |
| Evidencia con secretos | patrón sensible con valor | restringir acceso, sanear y activar respuesta a incidente. |
| Resultado parcial incoherente | cabecera no cuadra con detalles | no aceptar gate; reconciliar algoritmo. |

## Registro de evidencia esperado

Cada validación futura debe registrar:

- fecha/hora UTC y zona local;
- rama, commit y versión del worker;
- responsables y autorización;
- base/empresa por nombre no sensible;
- comandos exactos sin secretos;
- conteos antes/después;
- IDs/correlaciones seguras;
- resultado de cada gate;
- checks no ejecutados y motivo;
- configuración restaurada;
- servicios finales apagados/estado esperado.

No incluir credenciales, tokens, cookies, conexiones, XML SRI, payload Login ni datos personales innecesarios.

## Estado de validación en Fase 10.1

| Check | Estado | Evidencia |
|---|---|---|
| Discovery de reader/import/worker/perfiles | Validated estáticamente | Archivos citados en el blueprint. |
| Diseño de separación SAP/Matriz–Sucursal | Validated documentalmente | Dos contratos y ownership explícitos. |
| SQL | Not executed | Prohibido en Fase 10.1. |
| SAP Service Layer | Not executed | Prohibido en Fase 10.1. |
| API/WinForms/workers | Not started | Prohibido en Fase 10.1. |
| Runtime DEMO | Not executed | Requiere fase y autorización posteriores. |
| Remigio/Cañaris | Not touched | Fuera de alcance. |
| SRI | Not touched | Fuera de alcance. |

La Fase 10.1 termina con este plan. La autorización posterior permite únicamente los contratos, repositorios, scripts no ejecutados y pruebas estáticas de Fase 10.2; no autoriza SQL real, SAP, API, WinForms ni workers.
