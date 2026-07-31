# Evidencia runtime — Fase 10.4 Scheduler y heartbeat SAP

Fecha: 2026-07-31  
Rama: `refactor/codex-skills-v10-sap-profiles`  
HEAD desplegado: `52a6f822d14359353473018ababca5af7d69f115`  
Base objetivo: `NuanSystem_Master`

## Alcance y seguridad

Se validó exclusivamente la reparación forward-only
[`157_master_sap_sync_scheduler_session_options.sql`](../../database/sql/157_master_sap_sync_scheduler_session_options.sql).
La configuración local se deserializó mediante un modelo JSON restringido y se mantuvo en memoria.
Solo se comprobó presencia de conexión, autenticación SQL y la política efectiva
`Encrypt=true` / `TrustServerCertificate=false`; no se imprimieron valores, fragmentos,
longitudes ni fingerprints de secretos. No se leyó ni utilizó configuración SAP o SRI.

No se iniciaron API, WinForms ni workers. No hubo llamadas a SAP Business One,
Service Layer o SRI. `NuanSystem_DEMO` se consultó únicamente para confirmar conteos;
Remigio y Cañaris no fueron consultados ni modificados.

## Baseline y respaldo

| Evidencia | Inicial | Final |
|---|---:|---:|
| `20260730.155` en `MasterSchemaHistory` | 1 | 1 |
| `20260731.156` en `MasterSchemaHistory` | 1 | 1 |
| `20260731.157` en `MasterSchemaHistory` | 0 | 1 |
| Perfiles SAP activos | 0 | 0 |
| Entidades de perfil activas | 0 | 0 |
| Agendas SAP activas | 0 | 0 |
| Ejecuciones SAP en DEMO | 0 | 0 |
| Detalles SAP en DEMO | 0 | 0 |
| Locks SAP en DEMO | 0 | 0 |
| Heartbeats históricos `SapSync` | 2 | 2 |
| Heartbeats recientes | 0 | 0 |
| Heartbeats activos | 0 | 0 |
| Heartbeats antiguos detenidos/deshabilitados | 2 | 2 |
| Fixtures residuales | 0 | 0 |

Se creó `NuanSystem_Master_Phase104_157_20260731_172419.bak` mediante
`COPY_ONLY WITH CHECKSUM`. `RESTORE VERIFYONLY WITH CHECKSUM` aprobó antes de
ejecutar la migración.

Los fingerprints internos, calculados únicamente sobre columnas funcionales
permitidas, confirmaron preservación de perfiles, entidades, agendas, capacidades,
compatibilidad, configuración legacy y los dos heartbeats históricos. Los valores de
los fingerprints no se persistieron ni se incluyeron en esta evidencia.

## Despliegue idempotente

El script 157 se ejecutó dos veces en `NuanSystem_Master`:

| Pase | Resultado | Versión 157 |
|---|---|---:|
| 1 | Correcto | 1 |
| 2 | Correcto | 1 |

La segunda ejecución preservó la misma definición efectiva y no duplicó objetos ni
historial. Los scripts 155 y 156 permanecieron intactos, verificados por diff y por
sus pruebas contractuales de contenido normalizado.

## Contrato runtime del procedimiento

`dbo.SP_NA_PATCH_SAPSYNCSCHEDULERESERVAR` existe una sola vez. SQL Server reportó
`uses_ansi_nulls=1` y `uses_quoted_identifier=1`.

| Orden | Parámetro | Tipo | Default |
|---:|---|---|---|
| 1 | `@ScheduleId` | `bigint` | obligatorio |
| 2 | `@ExpectedRowVersion` | `varbinary(8)` | obligatorio |
| 3 | `@UtcNow` | `datetime2(0)` | obligatorio |
| 4 | `@ObservedNextExecutionAtUtc` | `datetime2(0)` | `NULL` |
| 5 | `@ScheduledAtUtc` | `datetime2(0)` | `NULL` |
| 6 | `@NextExecutionAtUtc` | `datetime2(0)` | obligatorio |

La firma normalizada y la metadata de parámetros permanecieron iguales al contrato
previo. Se conservaron `SELECT @@ROWCOUNT`, la comparación de `RowVersion`, la
revalidación de `NextExecutionAtUtc` y las guardas de perfil, entidad y agenda activos.

El procedimiento establece antes del `UPDATE` las opciones requeridas por índices
filtrados: `ANSI_PADDING ON`, `ANSI_WARNINGS ON`, `ARITHABORT ON`,
`CONCAT_NULL_YIELDS_NULL ON` y `NUMERIC_ROUNDABORT OFF`. La prueba invocó el
repositorio con `ARITHABORT OFF` en la sesión cliente y la reserva aprobó, demostrando
que [`SapSyncScheduleRepository`](../../src/Backend/NuanSystem.Persistence/Repositories/SapSync/SapSyncScheduleRepository.cs)
no necesita ejecutar `SET` ni SQL inline.

## Gates Dapper y scheduler

Los fixtures usaron códigos con prefijo `__CODEX_PHASE104_157_` y una transacción
revertida. Los candidatos legacy preexistentes se mantuvieron fuera de las aserciones
de fixtures, sin modificarlos.

| Gate | Resultado |
|---|---|
| Materialización `Profile` | Aprobado |
| Materialización `LegacyFallback`, incluidos nullables y `RowVersion` nulo | Aprobado |
| `SapToErp` y `ErpToSap`; capacidades `true`/`false` | Aprobado |
| Paginación keyset estable | Aprobado |
| Fairness entre tres empresas y varias entidades | Aprobado |
| Ausencia de starvation en el recorrido paginado | Aprobado |
| Exclusión de perfil, entidad y agenda inactivos | Aprobado |
| Exclusión de agenda `Manual` | Aprobado |
| Agendas `Interval` y `Daily` | Aprobado |
| Reserva real mediante `SapSyncScheduleRepository.TryReserveAsync` | Aprobado |
| `RowVersion` obsoleto | Rechazado sin cambio |
| Dos competidores con el mismo `RowVersion` | Exactamente un ganador |
| `NextExecutionAtUtc` observado distinto | Rechazado; estado preservado |
| Perfil, entidad o agenda inactivos al reservar | Rechazados |
| Fallback legacy | Solo lectura; sin dual-write |
| SQL Server 1934 | No reapareció |
| Rollback y limpieza | Aprobado; cero fixtures |
| `DBCC CHECKCONSTRAINTS WITH ALL_CONSTRAINTS` | Cero violaciones |

## Build y pruebas

| Comando/gate | Resultado |
|---|---|
| `git diff --check` | Aprobado |
| Pruebas dirigidas scheduler SAP | 53 superadas, 0 fallidas |
| Regresión SAP y Matriz–Sucursal | 413 superadas, 0 fallidas, 5 diagnósticos SQL condicionados omitidos |
| `dotnet build NuanSystem.sln --no-restore` | Aprobado; 0 advertencias, 0 errores |
| `dotnet test NuanSystem.sln --no-build --no-restore` | 689 superadas, 0 fallidas, 5 omitidas |

Las cinco omisiones corresponden a diagnósticos SQL condicionados existentes; el gate
runtime autorizado de 157 se ejecutó separadamente y aprobó.

## Cierre y riesgos pendientes

- El scheduler y la reparación de sesión quedan validados en el entorno local autorizado.
- Los dos heartbeats históricos permanecen exactamente preservados, antiguos e inactivos.
- No quedaron perfiles, entidades o agendas activos, ni ejecuciones, detalles, locks o fixtures.
- El respaldo fue verificado, pero `VERIFYONLY` no sustituye una prueba completa de restauración.
- No se validó conectividad externa SAP/SRI por restricción expresa.
- No se inició la Fase 10.5, no hubo push, PR ni integración a `master`.
