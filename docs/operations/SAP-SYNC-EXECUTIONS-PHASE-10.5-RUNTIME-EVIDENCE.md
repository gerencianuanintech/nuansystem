# Evidencia runtime — Fase 10.5 Historial, detalle y reintentos SAP

Fecha: 2026-07-31
Rama: `refactor/codex-skills-v10-sap-profiles`
HEAD desplegado: `cd66125bdd162d483c9dd6ebe491f27061fc8a5b`
Tenants: `NuanSystem_DEMO`, `NuanSystem_DEMO_REMIGIO` y `NuanSystem_DEMO_CANARIS`

## Alcance y seguridad

Se desplegaron y validaron las migraciones tenant
[`153_tenant_sap_sync_execution_history.sql`](../../database/sql/153_tenant_sap_sync_execution_history.sql)
y [`158_tenant_sap_sync_execution_operations.sql`](../../database/sql/158_tenant_sap_sync_execution_operations.sql).
La configuración local se leyó como JSON tipado y los secretos se mantuvieron únicamente
en memoria. Las conexiones usaron `Encrypt=true` y `TrustServerCertificate=false`.

Todos los workers permanecieron deshabilitados. Solo se inició temporalmente la API para
los gates HTTP y se detuvo al finalizar. No hubo llamadas a SAP Business One, Service
Layer o SRI, ni procesamiento externo.

## Respaldos verificados

Antes de las migraciones tenant se crearon respaldos `COPY_ONLY WITH CHECKSUM` y se
aprobaron mediante `RESTORE VERIFYONLY WITH CHECKSUM`:

- `NuanSystem_DEMO_Phase105_20260731_203646.bak`
- `NuanSystem_DEMO_REMIGIO_Phase105_20260731_203646.bak`
- `NuanSystem_DEMO_CANARIS_Phase105_20260731_203646.bak`

Durante el gate API se confirmó que las credenciales tenant cifradas de las tres empresas
habían quedado desactualizadas después de una rotación local previa. Con autorización
separada se creó y verificó
`NuanSystem_Master_Phase105_CredentialRepair_20260731_205644.bak`, y se actualizó
exclusivamente `Companies.DatabasePasswordEncrypted` para `DEMO`, `DEMO-REMIGIO` y
`DEMO-CANARIS`, usando la misma `Security:EncryptionKey`. Un fingerprint sin secretos
confirmó que ningún otro dato funcional de esas empresas cambió. Las tres conexiones
almacenadas aprobaron posteriormente con TLS estricto.

## Despliegue idempotente

La migración 153 ya existía una vez en DEMO. Se ejecutó dos veces exclusivamente en
Remigio y Cañaris. La migración 158 se ejecutó dos veces en los tres tenants.

| Evidencia final por tenant | Resultado |
|---|---:|
| Versión 153 | 1 |
| Versión 158 | 1 |
| Tablas de historial SAP | 3 |
| Procedimientos de 153 | 10 |
| Procedimientos operativos de 158 | 7 |
| Ejecuciones residuales | 0 |
| Detalles residuales | 0 |
| Locks activos | 0 |

Los índices y constraints quedaron presentes y confiables. Los datos iniciales se
preservaron y `DBCC CHECKCONSTRAINTS WITH ALL_CONSTRAINTS` no reportó violaciones.

## Gates Dapper y operativos

Los fixtures fueron identificables con prefijo `P105-`, se limitaron a DEMO y se
eliminaron al finalizar.

| Gate | Resultado |
|---|---|
| Listado de ejecuciones y detalle paginado mediante Dapper real | Aprobado |
| Proyección pública sin snapshots ni hashes internos | Aprobado |
| Reintento manual repetido | Idempotente; mismo reintento |
| Solicitud de cancelación | Aprobada con resultado contractual |
| Dos claims competidores | Exactamente un ganador |
| Recuperación de lease vencido | Aprobada |
| Liberación manual de lease vencido | Aprobada |
| Límite de intentos y transición a `DeadLetter` | Aprobado |
| Auditoría de transiciones | Aprobada |
| Aislamiento Remigio/Cañaris | Aprobado; sin fixtures cruzados |
| Limpieza | Aprobada; cero residuos |

Durante la validación se corrigieron tres contratos antes del cierre: la versión
prerrequisito de 153 en el script 158, la cobertura contractual de esa dependencia y el
resultado `CancellationRequested` devuelto por el procedimiento de cancelación.

## API y permisos

La API se inició únicamente sobre loopback y se detuvo en el mismo gate. Los JWT se
generaron temporalmente en memoria con el `SecurityStamp` vigente y nunca se imprimieron
ni persistieron.

| Escenario | Resultado |
|---|---:|
| API readiness | HTTP 200 |
| Sin autenticación | HTTP 401 |
| Usuario autenticado sin `SAP.SYNC.EXECUTIONS.VIEW` | HTTP 403 |
| Usuario con permiso exacto y tenant DEMO | HTTP 200 |
| Respuesta sin snapshots/hashes internos | Aprobado |

## Build y pruebas

| Gate | Resultado |
|---|---|
| `git diff --check` | Aprobado |
| `dotnet build NuanSystem.sln --no-restore` | 0 errores, 0 advertencias |
| `dotnet test NuanSystem.sln --no-build --no-restore` | 694 superadas, 0 fallidas, 5 omitidas |

Las cinco omisiones son diagnósticos SQL condicionados existentes; los gates SQL y
Dapper autorizados para esta fase se ejecutaron separadamente y aprobaron.

## Cierre

- La Fase 10.5 queda cerrada para el alcance local autorizado.
- No quedaron fixtures, ejecuciones, detalles, locks ni procesos NuanSystem activos.
- Perfiles, entidades, agendas y workers permanecieron deshabilitados.
- No se llamó SAP, Service Layer o SRI.
- No se realizó push, PR ni integración a `master`.
- La Fase 10.6 no fue iniciada.
