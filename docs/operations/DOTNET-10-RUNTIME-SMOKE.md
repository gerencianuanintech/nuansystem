# Smoke runtime de NuanSystem sobre .NET 10

## Estado

- Fecha: 2026-07-25.
- Rama: `refactor/codex-skills-v7-2-1-dotnet10-runtime-smoke`.
- Baseline: `ed8333914704f253249fcb49be2baabdda2ca1f3`.
- Rama de cierre: `refactor/codex-skills-v7-2-2-dotnet10-runtime-closure`.
- Baseline de cierre: `709b9ae63088d6aa2398d105e83e7337263eb07f`.
- Alcance: API local, workers deshabilitados y WinForms contra la empresa piloto `DEMO`.
- Resultado original: **NO-GO**, corregido posteriormente por el forward repair tenant `123`.
- Resultado de cierre de Fase 7.2.2: **GO runtime** para el alcance autenticado y multiempresa
  aprobado.
- Motivo bloqueante original: el resumen del Monitor SRI no podia materializarse con Dapper debido
  a tipos incompatibles entre el procedimiento almacenado y el DTO.

No se ejecutaron scripts SQL, SAP, SRI, servicios Windows ni procesamiento documental. No se
crearon claims, leases o procesos residuales. La API iniciada previamente por Visual Studio se
conservó sin detenerla.

## API

Se inició una instancia HTTP aislada en loopback, con inicialización de Master y hosted services
deshabilitados, únicamente para comprobar el pipeline básico.

| Gate | Resultado |
|---|---|
| `/health` | HTTP 200, `Healthy` |
| `/health/live` | HTTP 200, `Healthy` |
| `/health/ready` sin JWT | HTTP 401 |
| API abierta por Visual Studio | Conservada; no se reinició ni detuvo |
| Instancia HTTP aislada al finalizar | Detenida; puerto liberado |

La instancia aislada registró advertencias de Data Protection porque el contexto de ejecución no
podía usar el key ring DPAPI de la sesión interactiva. Esto no bloqueó los endpoints anónimos y no
se atribuye al proceso iniciado desde Visual Studio.

La autenticación interactiva del cliente WinForms fue satisfactoria para `admin`: devolvió tres
empresas y permitió seleccionar `Empresa Demo`. El login exitoso ejecuta por diseño
`RegisterSuccessfulLoginAsync` y actualiza `Users.LastLoginAt`; por tanto, esa marca de auditoría
es el único efecto persistente conocido de la validación. No se repitió el login después de
detectar el defecto.

Los escenarios autenticados 403/200 no se declararon completos porque el criterio de aborto se
activó al fallar el Monitor SRI.

## Workers deshabilitados

| Host | Configuración efectiva | Resultado |
|---|---|---|
| `NuanSystem.SyncWorker` | `Worker:Enabled=false`, `Retry:Enabled=false` | Inicio y cierre cooperativo; ambos ciclos deshabilitados |
| `NuanSystem.MasterBranchSyncWorker` | `Enabled=false`, `SkeletonMode=true`, `ObserveOnly` | Inicio y cierre cooperativo; sin acceso a datos |
| `NuanSystem.SriWorker` | `Enabled=false`, Event Log deshabilitado | Lifecycle `Disabled` y cierre cooperativo |

Para impedir que el heartbeat deshabilitado del SRI Worker alcanzara Master, la conexión se
sobrescribió temporalmente con un destino loopback no atendido. El fallo de conexión esperado fue
registrado, pero no hubo acceso SQL ni invocación del proveedor SRI.

Al finalizar no quedaron procesos de WinForms, SyncWorker, MasterBranchSyncWorker o SriWorker.

## Build y pruebas

| Gate | Resultado |
|---|---|
| `git diff --check` | Correcto |
| Build Release sin restore | 0 advertencias, 0 errores |
| Tests Release sin build/restore | 473 superadas, 5 diagnósticas omitidas, 0 fallidas; 478 total |

La suite automatizada no detecta actualmente la incompatibilidad Dapper porque sus verificaciones
del resumen son contractuales y no materializan el result set SQL real.

## WinForms y DevExpress

Se ejecutó `NuanSystem.WinForms` Release sobre `net10.0-windows` usando los assemblies DevExpress
25.2 instalados en la máquina.

| Gate | Resultado |
|---|---|
| Inicio del cliente | Validado |
| Estado de API en login | `API activa` |
| Login y selección de empresa | Validado; tres empresas, `Empresa Demo` seleccionada |
| Shell, Ribbon, Accordion y pestañas | Validado visualmente |
| Navegación por permisos | Validada para accesos visibles del usuario administrador |
| Transportistas | Validado en modo consulta |
| Historial de transportista | Validado; formulario corporativo con siete registros |
| Monitor SRI | **Fallido** |
| Cierre del cliente | Validado; cero procesos residuales |

Transportistas mostró el maestro independiente con las columnas `Código`, `Nombre`,
`Tipo de identificación`, `Identificación`, `Descripción` y `Activo`. También quedaron visibles
las operaciones `Consultar`, `Copiar`, `Actualizar`, `Nuevo`, `Editar`, `Eliminar`, `Columnas` e
`Historial`. No se creó, actualizó ni eliminó ningún transportista.

## Defecto bloqueante

Al abrir Monitor SRI, `GET /api/sri/documents/monitor/summary` produjo una excepción controlada y
el cliente mostró `Ocurrió un error interno procesando la solicitud`.

Evidencia técnica:

```text
InvalidOperationException:
A parameterless default constructor or one matching signature
(System.Int64 Total, System.Int32 Pending, System.Int32 Querying,
 System.Int32 Authorized, System.Int32 Errors)
is required for SriDocumentMonitorSummaryDto materialization.
```

El contrato C# declara:

```text
SriDocumentMonitorSummaryDto(
    long Total,
    long Pending,
    long Querying,
    long Authorized,
    long Errors)
```

El procedimiento `SP_NA_GET_SRIDOCUMENTMONITOR_RESUMEN` devuelve:

- `COUNT_BIG(1)` como `Int64`;
- cada `SUM(CASE ... THEN 1 ELSE 0 END)` como `Int32`.

Dapper intenta usar el constructor posicional exacto y rechaza esa combinación. Las pruebas
existentes verifican presencia y seguridad textual del contrato, pero no materializan el result
set real del procedimiento.

## Corrección recomendada

Debe abrirse una fase correctiva separada con autorización SQL explícita:

1. agregar una nueva migración tenant idempotente;
2. hacer que las cuatro sumas devuelvan `bigint` y cero para una cola vacía, preservando el
   contrato público `long`;
3. añadir una prueba de integración de materialización Dapper contra el esquema real;
4. ejecutar el script dos veces en el tenant piloto autorizado;
5. repetir únicamente resumen, listado, salud del worker y renderizado del Monitor SRI;
6. completar después los gates autenticados 403/200 pendientes.

No se recomienda reducir el DTO a `int`, porque los contadores públicos y `COUNT_BIG` ya expresan
la intención de soportar volúmenes superiores a `Int32`.

## Criterio de reanudación

El blocker de materializacion y apertura del Monitor SRI quedo resuelto el 2026-07-25 mediante:

- `123_tenant_sri_document_monitor_summary_bigint_fix.sql`, ejecutado dos veces en
  `NuanSystem_DEMO`, `NuanSystem_DEMO_REMIGIO` y `NuanSystem_DEMO_CANARIS`, con una unica version
  `20260725.123` por tenant;
- metadata SQL real con cinco columnas `bigint` en los tres tenants;
- materializacion Dapper real contra `SriDocumentMonitorSummaryDto`;
- apertura y actualizacion visual del Monitor SRI con KPI `4/0/1/0`, sin iniciar workers ni llamar
  al SRI.

## Cierre autenticado de Fase 7.2.2

El 2026-07-25 se completo el gate autenticado contra la API .NET 10 ya iniciada por Visual Studio,
sin reiniciarla ni detenerla. La validacion uso un usuario activo existente y dos JWT efimeros
generados exclusivamente en memoria: uno sin permisos y otro con
`SRI.DOCUMENTS.VIEW`. No se ejecuto login, no se modificaron usuarios, roles, permisos o datos y
no se imprimieron ni persistieron tokens, claves o cadenas de conexion.

| Gate | Resultado |
|---|---|
| Runtime del validador | .NET `10.0.10` |
| TLS SQL efectivo | `Encrypt=true`, `TrustServerCertificate=false` |
| Sin autenticacion, empresa `DEMO` | HTTP 401 |
| JWT vigente sin permiso, empresa `DEMO` | HTTP 403 |
| JWT vigente con permiso, empresa `DEMO` | HTTP 200 |
| Empresa inexistente/no disponible | HTTP 403 |
| Resumen `DEMO` | `Total=4`, `Pending=0`, `Querying=0`, `Authorized=1`, `Errors=0` |
| Resumen `DEMO-REMIGIO` | `0/0/0/0/0` |
| Resumen `DEMO-CANARIS` | `0/0/0/0/0` |
| Build Release sin restore | 0 advertencias, 0 errores |
| Tests Release sin build/restore | 478 superadas, 5 diagnosticas omitidas, 0 fallidas; 483 total |

Los tres codigos de empresa fueron resueltos por el middleware real para el mismo usuario y cada
respuesta fue materializada desde su base tenant. Los conteos distintos entre Matriz y sucursales
acreditan que el contexto de empresa no reutilizo datos de otro tenant.

No se iniciaron WinForms ni workers durante este cierre; sus conteos finales fueron cero. La API
preexistente con PID `45612` se conservo activa. El flujo ejecutado fue exclusivamente de consulta:
no encolo documentos, no creo claims, leases, intentos o auditorias y no invoco SAP ni el proveedor
SRI.

Con esta evidencia quedan cerrados los pendientes de Fase 7.2.1 y 7.2.2. El siguiente gate no es
otro smoke funcional: corresponde definir artefactos versionados `win-x64`, manifests, hashes y
rollback pilot1/pilot2 antes de cualquier promocion.
