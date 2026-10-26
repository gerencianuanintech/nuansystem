# Evidencia de consolidación del backend — fase 1

Fecha: 2026-10-22. Alcance autorizado: fase 1 de `docs/superpowers/plans/2026-10-22-backend-solid-consolidation.md`. No incluye correcciones de fases 2–9, llamadas SAP, cambios SQL, arranque de API ni workers operativos.

## Estado y preservación

- Rama: `codex/auxiliary-master-generator`; HEAD inicial `e97124a1e2863f066c1fa55ea992c78179f01623`.
- Cambios previos: 76 archivos existentes modificados/no rastreados registrados con SHA-256 en `artifacts/backend-solid-phase1/preserved-files.json`. Incluyen backend, frontend, documentación y SQL.
- Solo se añaden dos archivos de tests y dos documentos. No se edita el proyecto de pruebas ni código productivo, paquetes, scripts o configuración.
- Registro de progreso: esta evidencia. No se hace commit, stash, reset, cambio de rama ni manipulación del trabajo ajeno.
- Decisión: baseline del checkout real con cambios del usuario, en lugar de otro worktree de HEAD que los excluya. Se verifica preservación por huellas al cierre.
- SDK fijado: 10.0.302 con `latestFeature`; SDK seleccionado: 10.0.400. No se instala ni restaura software.

## Entregables

- `tests/NuanSystem.Application.Tests/Architecture/BackendDependencyTests.cs`: referencias de diez proyectos, paquetes y assemblies de las capas centrales, constructores de todos los handlers MediatR y controles negativos de las reglas.
- `tests/NuanSystem.Application.Tests/Architecture/BackendCompositionTests.cs`: registro DI real en Generic Host no iniciado, resolución de handlers y procesadores, scopes de tenant/repositorio y barreras de IO verificadas.
- `docs/architecture/BACKEND-SOLID-CONSOLIDATION.md`: decisiones, matriz de dependencias, inventario de consumidores, pruebas externas y deuda explícita.
- Este documento: baseline, comandos, resultados, límites y excepciones.

## Ejecución segura

Se inspeccionaron los usos de variables de entorno, SqlConnection y configuración externa en el proyecto de pruebas. Los accesos SQL externos identificados están en `MasterBranchSyncConnectionDiagnosticsTests`, con atributo `SqlServerIntegrationFact` y categoría `SqlServerIntegration`. El piloto mutante está en `SapPurchaseOrderPilotIntegrationTests` y se excluye completo, aunque su gate ya esté desactivado.

En cada proceso de prueba:

```powershell
$env:NUANSYSTEM_RUN_SQL_INTEGRATION_TESTS='0'
$env:NUAN_RUN_SAP_PO_PILOT='0'
```

Son valores del proceso ejecutado, no configuración global. No se leen appsettings o secretos en el nuevo host de composición y no se llama `StartAsync`. Algunos tests existentes ejecutan servicios con mocks/transportes simulados: eso no es activar un worker operativo.

## Baseline existente

Comando después de compilar, excluyendo los nuevos tests para medir el estado previo:

```powershell
dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj --no-build --no-restore --filter 'Category!=SqlServerIntegration&FullyQualifiedName!~SapPurchaseOrderPilotIntegrationTests&FullyQualifiedName!~NuanSystem.Application.Tests.Architecture' --logger 'trx;LogFileName=baseline.trx' --results-directory artifacts/backend-solid-phase1/baseline --verbosity minimal
```

Resultado: 1.047 ejecutadas; 1.044 aprobadas; 3 fallidas. Código de salida 1. No se presenta la suite completa como verde.

Fallos en `Features/SapSync/SapSyncWinFormsContractTests.cs`, sobre archivos Designer que ya estaban modificados al iniciar:

| Test | Expectativa estática que no coincide |
|---|---|
| SapProfileEditor_UsesCorporateActionButtonSizeAndFooterSpacing | `btnCancelar.Location = new Point(936, 672)` |
| Designers_AreExplicitAndKeepSapCaptions | texto `Entidades y programacion` |
| SapForms_UseCorporatePagingOperationalAndLookupContracts | `codeEdit.Location = new Point(700, 50)` |

No se corrigen posiciones ni se debilitan esos tests: son trabajo previo y están fuera de fase 1. TRX y log: `artifacts/backend-solid-phase1/baseline/` y `baseline.log`.

## Pruebas del gate

Se introdujo primero el detector de referencias prohibidas sin implementar su clasificación y se ejecutaron los controles negativos: 8 fallos esperados y 25 aprobadas. Después se implementó la política y pasaron las 37 pruebas iniciales, incluidas las cuatro de composición. Los tres chequeos adicionales de paquetes y la limpieza de warnings se incluyen en la corrida final Release.

Evidencia RED: `artifacts/backend-solid-phase1/red/architecture-red.trx`.
Evidencia GREEN inicial: `artifacts/backend-solid-phase1/green/architecture-green.trx`.

## Compilación

```powershell
dotnet build NuanSystem.sln -c Release --no-restore --disable-build-servers -m:1 --verbosity quiet
```

Resultado: aprobado, 0 advertencias y 0 errores; log `artifacts/backend-solid-phase1/build-release.log`.

La ejecución inicial dentro del sandbox falló al consultar el registro del SDK Windows (`MSB4184`) para WinForms. La repetición local fuera del sandbox superó la compilación, sin instalar/restaurar dependencias ni cambiar el registro.

El runner de tests también requirió ejecución fuera del sandbox: dentro, VSTest no conectó con testhost y abortó sin ejecutar tests. No se confundió ese bloqueo con un fallo del código. Diagnóstico en `artifacts/backend-solid-phase1/testhost-diag.log`; gates y filtros se conservaron fuera del sandbox.

## Corrida final

```powershell
dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj -c Release --no-build --no-restore --filter 'Category!=SqlServerIntegration&FullyQualifiedName!~SapPurchaseOrderPilotIntegrationTests' --logger 'trx;LogFileName=phase1-final.trx' --results-directory artifacts/backend-solid-phase1/final --verbosity minimal
```

Resultado: 1.087 ejecutadas, 1.084 aprobadas y los mismos 3 fallos previos de WinForms; sin fallos nuevos. Las 40 pruebas nuevas de arquitectura/composición están aprobadas. Código de salida global 1 por los fallos del baseline; no se presenta como suite global aprobada.

Se comparó el conjunto exacto de nombres de pruebas fallidas entre baseline y corrida final: sin diferencias. Se compararon las huellas de los 76 archivos existentes: 0 alterados. HEAD y rama se conservaron. Resumen verificable: `artifacts/backend-solid-phase1/summary.json`.

Estado de fase 1: implementada y verificada con baseline conocido de tres fallos ajenos al alcance. Las fases siguientes permanecen sin iniciar. No hay cambios productivos ni de base de datos que desplegar.

## Revisión y alcance de garantías

Revisión independiente de los nuevos archivos: sin hallazgos bloqueantes. Se incorporan sus límites: las barreras no interceptan todo IO posible y el XML no evalúa imports MSBuild. El detalle está en el ADR.

| Gate | Evidencia / límite |
|---|---|
| Scope / preservación | nuevos archivos solamente, comparación SHA-256 |
| Arquitectura | referencias XML, assemblies y constructores + pruebas de DI |
| Reutilización | xUnit/Generic Host/registros existentes; sin nuevos paquetes |
| Negocio / SQL / SAP | no aplicable a cambios productivos; no se ejecutan operaciones reales |
| Seguridad / tenancy | scopes probados; autorización real y aislamiento de filas no validados aquí |
| UI | sin cambios; tres fallos estáticos previos documentados |
| Recuperación operacional / carga | no validada; pertenece al plan SAP y fases posteriores |

No se acredita todavía cumplimiento completo de SOLID ni cierre de los hallazgos de fases 2–8. El gate evita regresiones de fronteras y deja un baseline reproducible para continuar.
