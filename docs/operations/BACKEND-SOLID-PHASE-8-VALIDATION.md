# Fase 8: validacion de empresa y configuracion tecnica

Fecha: 2026-10-24. Alcance: fase 8 de consolidacion, sin ejecutar SAP ni modificar bases.

## Resultado

- Company conserva solo identidad, estado y organizacion empresarial. Se retiraron siete propiedades tecnicas sin consumidores productivos del modelo.
- Se reutilizan CompanyConnectionInfo y los DTO/configuraciones existentes. No se duplican modelos administrativos ni contratos de resolucion.
- CompanyDatabaseConfiguration es interno de Persistence y construye conexiones SQL Server sin abrirlas. El resolver conserva SQL, filtros UserCompanies/activos, parametros y proyecciones de compatibilidad.
- ConnectionString sigue disponible para el contexto/factory interno, pero se omite en JSON y ToString. Los DTO HTTP administrativos/autenticacion/SAP existentes no cambian; las passwords siguen fuera de sus respuestas.
- Cifrado, puerto, TLS y seleccion por CompanyId se conservan. MySQL sigue no implementado y el modo sin SAP no requiere un cliente SAP.

Compilacion de solucion Release: 0 errores y 0 advertencias. Pruebas enfocadas: 39 aprobadas. Casos nuevos: 24 aprobados (8 guardas arquitectonicas y 16 de limites/configuracion). Regresion completa: 1253 ejecutadas, 1250 aprobadas y los mismos 3 fallos previos de WinForms. Revision independiente de solo lectura: sin hallazgos importantes.

## Verificacion

Ocho guardas fallaron sobre el modelo original (RED) y aprobaron despues (GREEN). Las pruebas nuevas verifican forma JSON empresarial/autenticacion, ausencia de secretos en tipos de respuesta, redaccion de la conexion, construccion con puerto/TLS y password descifrada, rechazo de cifrado invalido/motor no soportado, funcionamiento sin SAP, dos empresas con la misma base SAP externa y middleware autorizado/denegado.

La primera pasada enfocada detecto dos errores de fixture, corregidos sin cambiar produccion: reconfigurar un sustituto que ya lanzaba y reutilizar un CompanyContext entre solicitudes de empresas distintas. La pasada final usa un contexto por solicitud y respeta su guard existente.

```powershell
$env:NUANSYSTEM_RUN_SQL_INTEGRATION_TESTS='0'
$env:NUAN_RUN_SAP_PO_PILOT='0'
dotnet build NuanSystem.sln -c Release --no-restore --disable-build-servers -m:1 --verbosity quiet
dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj -c Release --no-build --no-restore --filter 'FullyQualifiedName~CompanyConfigurationBoundaryTests|FullyQualifiedName~Business_company_does_not|FullyQualifiedName~SyncInfrastructureContractTests' --logger 'trx;LogFileName=phase8-targeted-final.trx' --results-directory artifacts/backend-solid-phase8/targeted-final --verbosity minimal
dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj -c Release --no-build --no-restore --filter 'Category!=SqlServerIntegration&FullyQualifiedName!~SapPurchaseOrderPilotIntegrationTests' --logger 'trx;LogFileName=phase8-final.trx' --results-directory artifacts/backend-solid-phase8/final --verbosity minimal
```

Los gates solo afectan el proceso hijo. Se excluyen expresamente SQL externo y piloto SAP. Las pruebas de middleware/repositorio/descifrado usan dobles. Una prueba construye una SqlConnection con datos ficticios y confirma State=Closed; nunca se llama Open. No se inicia API/workers ni se contacta SAP o DB. Build/test local fuera del sandbox permite SDK Windows y comunicacion de testhost; sin instalaciones/restauraciones ni configuracion global.

La suite global permanece fallida por estos nombres exactos, iguales a fase 7:

- `SapSyncWinFormsContractTests.SapForms_UseCorporatePagingOperationalAndLookupContracts`.
- `SapSyncWinFormsContractTests.Designers_AreExplicitAndKeepSapCaptions`.
- `SapSyncWinFormsContractTests.SapProfileEditor_UsesCorporateActionButtonSizeAndFooterSpacing`.

No se modifican ni deshabilitan esos tests. Los totales cuentan todos los nodos TRX, incluidos nombres visibles repetidos.

## Preservacion y compatibilidad

Rama/HEAD conservados: `codex/auxiliary-master-generator`, `e97124a1e2863f066c1fa55ea992c78179f01623`.

Snapshot de 165 archivos modificados/no rastreados previos en src/tests/docs/database: 164 conservan SHA-256 exacto y ninguno falta. El unico editado de ese conjunto es BackendDependencyTests.cs para agregar guardas de Company. Copias previas de los cinco archivos existentes afectados en `artifacts/backend-solid-phase8/before/`.

Se verifico igualdad exacta del resolver anterior al invertir exclusivamente la extraccion del builder y sus tres llamadas. SQL y parametros permanecen identicos, incluida autorizacion UserCompanies. La prueba estatica de politica TLS ahora inspecciona el builder extraido y verifica que el resolver lo utilice; no se debilitan sus reglas.

No se cambian schema, scripts, repositorios administrativos, cifrado persistido, endpoints, permisos, API DTO ni frontend. Los enums DatabaseEngine/SapIntegrationMode conservan ubicacion legacy por compatibilidad; Company ya no depende de ellos. Sin stash/reset/commit/cambio de rama. Diff sin errores de espacios.

## Checklist aplicable y limites

| Gate | Estado y evidencia |
|---|---|
| R1/R2 Descubrimiento/alcance | Validated: Company sin consumidores productivos, contratos existentes reutilizados, ADR y busqueda de serializacion/resolucion/configuracion. |
| R3 Arquitectura | Validated: Company empresarial y configuracion interna Persistence; 8 guardas nuevas y regresion de dependencias/composicion aprobadas. |
| R4 Operacion | Validated por inspeccion: sin cambios de datos, transacciones, numeracion o documentos. |
| R5 Tenant/secretos | Validated en memoria: autorizacion por usuario, contextos separados, configuracion SAP por CompanyId y serializacion sin passwords/conexion. Autorizacion/aislamiento SQL real: Not validated. |
| R6/R7 WinForms | Not applicable: sin cambios UI/Designer. |
| R8 API | Validated para esquema de respuestas e inspeccion de rutas/permisos sin cambios. HTTP autenticado en servidor real: Not validated. |
| R9 Persistencia | Validated estaticamente y con builder sin apertura: SQL/parametros/cifrado/TLS conservados. SQL real: Not validated/no ejecutado. |
| R10 SAP | Validated con dobles para aislamiento/configuracion; SAP real y DI API multiplataforma: Not validated/no afirmado. |
| R11/R12 Evidencia | Validated con limite conocido: build limpio, 39 enfocadas/24 nuevas pasan; global conserva 3 fallos previos y revision independiente sin hallazgos. |

ADR: `docs/architecture/BACKEND-SOLID-PHASE-8-COMPANY-CONFIGURATION.md`. Artefactos locales ignorados por Git: `artifacts/backend-solid-phase8/baseline.json`, `before/`, `red/phase8-red.trx`, `targeted-final/phase8-targeted-final.trx`, `build-final.log`, `final/phase8-final.trx` y `final/summary.json`.

Las pruebas no acreditan conectividad, credenciales reales, cifrado con secretos productivos, rendimiento o disponibilidad. La fase 7 sigue como piloto con su gate SQL pendiente; esta entrega no inicia fase 9 ni libera cambios a produccion.
