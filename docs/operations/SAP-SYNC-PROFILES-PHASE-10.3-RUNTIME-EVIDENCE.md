# Evidencia runtime de perfiles SAP — Fase 10.3

Fecha de ejecución: 2026-07-30 (`America/Guayaquil`).

Alcance ejecutado:

- `NuanSystem_Master` para backup, migración 154 y contratos de perfiles SAP.
- `NuanSystem_DEMO` únicamente para comprobar en modo lectura la migración 153 y la ausencia de ejecuciones SAP.
- empresa lógica piloto `DEMO`.
- API iniciada temporalmente con `SyncProfileExecutionWorker:Enabled=false`.

Quedaron fuera de alcance SAP Business One, Service Layer, SRI, WinForms,
`NuanSystem.SyncWorker`, `NuanSystem.MasterBranchSyncWorker`,
`NuanSystem.SriWorker`, Remigio y Cañaris.

## Gate inicial

| Requisito | Estado | Evidencia saneada |
|---|---|---|
| Rama y HEAD | Validado | `refactor/codex-skills-v10-sap-profiles` en `415b3b4408e498964ac4d74de736f86a69a4ba5c`. |
| Working tree | Validado | Limpio antes del despliegue. |
| Procesos | Validado | Cero procesos API, WinForms o workers. |
| Configuración Local | Validado | Lectura JSON tipada; conexión SQL, `Security:EncryptionKey` y clave JWT presentes. No se registraron valores. |
| TLS SQL | Validado | `Encrypt=true`, `TrustServerCertificate=false` y sesión SQL cifrada. |
| Migración 152 | Validado | Una versión `20260730.152` en Master. |
| Migración 154 inicial | Validado | Cero versiones y cero procedimientos de acceso antes del despliegue. |
| Migración 153 | Validado | Una versión `20260730.153` en DEMO; tenant consultado en modo lectura. |
| Fixtures previos | Validado | Cero perfiles, usuarios o roles con el prefijo reservado de la validación. |

## Backup y despliegue

El respaldo `NuanSystem_Master_Phase103_20260731_013019.bak` fue creado con
`COPY_ONLY WITH CHECKSUM` y aprobó `RESTORE VERIFYONLY WITH CHECKSUM`.

`154_master_sap_sync_profile_api_hardening.sql` se ejecutó dos veces,
exclusivamente en `NuanSystem_Master`.

| Validación | Resultado |
|---|---|
| Historia `20260730.154` | Una fila después de ambos pases. |
| `SP_NA_GET_SAPSYNCPROFILEEMPRESASACCESIBLES` | Un procedimiento. |
| `SP_NA_PUT_SAPSYNCPROFILEACTUALIZAR` | Un procedimiento con resultado `CompanyImmutable` y sin asignación de `CompanyId`. |
| Parámetros Dapper | `@UserId int` y `@CompanyId int = NULL`, verificados en metadata real. |
| Objetos duplicados | Ninguno. |
| Permisos SAP nuevos | Doce antes y doce después; sin duplicados. |
| Grants no ADMIN | Cero antes y cero después del despliegue. |
| Menús, formularios y operaciones | Snapshots inicial y final equivalentes. |
| Perfiles y agendas activados por migración | Ninguno. |
| `SapSyncEntitySettings` | Cuatro filas antes y cuatro después; snapshot equivalente. |
| `DBCC CHECKCONSTRAINTS` | Sin violaciones en perfiles, entidades y agendas SAP. |

## Dapper y SQL real

La materialización real de `SapSyncProfileCompanyAccessDto` validó:

- consulta sin `CompanyId`: solo la empresa asignada al usuario;
- filtro DEMO: `IsUserAuthorized=true`;
- filtro de empresa existente no asignada: fila presente con
  `IsUserAuthorized=false`;
- empresa inexistente: cero filas.

Un fixture dentro de transacción comprobó:

- creación inicial;
- actualización conservando la empresa;
- respuesta `CompanyImmutable` al cambiar `CompanyId`;
- `CompanyId`, `RowVersion`, entidades, agendas y auditoría sin cambios ante
  el rechazo.

La transacción fue revertida y el snapshot posterior confirmó cero fixtures.

## Validación API

Se crearon identidades y roles mínimos temporales. Todos los grants fueron
aplicados antes de emitir JWT nuevos mediante el login real. Los JWT, claves,
contraseñas, `SecurityStamp` y valores de configuración permanecieron
exclusivamente en memoria.

| Caso | Resultado |
|---|---|
| `/health` | HTTP 200. |
| Anónimo | HTTP 401. |
| Usuario autenticado sin permiso | HTTP 403. |
| `SAP.SYNC.READ`, `SAP.SYNC.MANAGE` y `SYNC.CONFIGURATION.*` | HTTP 403 en perfiles SAP. |
| Permiso exacto `VIEW` | Catálogo, listado paginado y detalle: HTTP 200. |
| Permisos `CREATE`, `EDIT`, `DELETE`, `VALIDATE`, `ACTIVATE` | Cada acción autorizada únicamente por su permiso correspondiente. |
| Creación | HTTP 201; perfil, entidad y agenda inicialmente inactivos. |
| Update con `RowVersion` vigente | HTTP 200 y empresa preservada. |
| `RowVersion` obsoleto | HTTP 409. |
| Cambio de `CompanyId` | HTTP 409 con `SAP_SYNC_PROFILE_COMPANY_IMMUTABLE`; detalle y `RowVersion` preservados. |
| Código duplicado | HTTP 409. |
| Validación estática | HTTP 200 y configuración válida después del update. |
| Activación / desactivación | HTTP 200 en ambas transiciones. |
| Eliminación lógica | HTTP 200. |
| `PurchaseOrders` | HTTP 400. |
| Dirección `Both` | HTTP 400. |
| `POST .../execute` | HTTP 404; endpoint inexistente. |
| Respuestas de perfiles | Sin conexiones, claves, contraseñas, sesiones SAP ni configuración sensible. |
| `SapSyncExecutions` en DEMO | Cero antes y cero después. |

La API fue detenida al finalizar. El log capturado en memoria confirmó que el
hosted service de ejecuciones Matriz–Sucursal estuvo deshabilitado.

## Conteos y limpieza

| Superficie | Inicial | Final |
|---|---:|---:|
| Versión Master 152 | 1 | 1 |
| Versión Master 154 | 0 | 1 |
| Versión tenant DEMO 153 | 1 | 1 |
| Perfiles SAP | 1 | 1 |
| Perfiles SAP activos | 0 | 0 |
| `SapSyncEntitySettings` | 4 | 4 |
| Permisos SAP de perfiles/ejecuciones | 12 | 12 |
| Grants no ADMIN permanentes | 0 | 0 |
| `SapSyncExecutions` DEMO | 0 | 0 |
| Fixtures de perfiles, usuarios y roles | 0 | 0 |

Los conteos y checksums saneados de entidades, agendas, auditoría de perfiles,
usuarios, roles, asignaciones, grants, operaciones, formularios y menús fueron
equivalentes antes y después de la limpieza. Los harnesses temporales no forman
parte del repositorio.

## Build y pruebas

| Gate | Resultado |
|---|---|
| `dotnet build NuanSystem.sln --no-restore` | Correcto, 0 errores y 0 advertencias. |
| Perfiles SAP dirigidos | 42 aprobadas, 0 omitidas. |
| Regresión SAP y Matriz–Sucursal | 360 aprobadas, 5 diagnósticos SQL opt-in omitidos. |
| Suite completa | 636 aprobadas, 5 diagnósticos SQL opt-in omitidos. |
| Proceso final | Cero procesos NuanSystem. |

Los cinco diagnósticos opt-in omitidos pertenecen al conjunto histórico de
diagnósticos SQL. El despliegue 154, Dapper real y la API sí fueron validados
en runtime por harnesses controlados y posteriormente eliminados.

## Riesgos residuales

- `RESTORE VERIFYONLY` verifica la legibilidad y checksums del respaldo, pero no
  sustituye una restauración integral en una instancia aislada.
- Los fixtures temporales fueron eliminados y los conteos regresaron al estado
  inicial; como en cualquier prueba real con tablas `IDENTITY`, pueden quedar
  saltos no funcionales en las secuencias de identidad.
- La validación no autoriza habilitar workers, ejecutar SAP, desplegar en otras
  empresas ni iniciar la Fase 10.4.
