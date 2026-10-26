# Fase 6: validacion del calculo de compras

Fecha: 2026-10-23. Alcance: fase 6 del plan de consolidacion del backend.

## Resultado

El calculo numerico reside en `Domain/Purchasing/PurchaseOrderAmountCalculator.cs`, con entradas/resultados numericos sin DTO de persistencia ni SAP/HTTP/SQL. Application conserva el filtrado de lineas y adapta resultados al contrato existente. El codigo SAP 22 queda centralizado en `Features/SapSync/Mapping/SapPurchaseOrderMapping.cs`, conservando el valor persistido/default del DTO y el mensaje SAP anterior.

Como decision separada se eliminan los defaults inventados de numeracion: crear exige codigo de serie autorizada y numero reservado; modificar exige identificadores existentes. Un documento sin numeracion se rechaza con `PURCHASE_ORDER_NUMBERING_MISSING`/Conflict antes de escribir; no se renumera.

- Compilacion de solucion Release: 0 errores y 0 advertencias.
- 14 casos de caracterizacion aprobados sobre el calculador anterior y sobre el nuevo.
- 79 pruebas enfocadas de compras aprobadas.
- 29 casos nuevos aprobados: caracterizacion 14, reglas Domain/adaptador 9 y numeracion en handlers 6.
- Regresion completa local: 1208 ejecutadas, 1205 aprobadas y los mismos 3 fallos visuales WinForms de fase 5.
- Revision independiente de solo lectura: sin hallazgos importantes.

Los casos caracterizan descuentos, impuesto cero/mixto, base imponible proporcional, lineas vacias/filtradas, precision y clamp, redondeo ToEven a seis decimales, overflow, moneda/tipo de cambio como metadata, identidad, auditoria y adaptacion. No se cambiaron importes, conversion de moneda, truncamientos ni asignacion del descuento de cabecera a las lineas.

## RED/GREEN y ejecucion

Antes de extraer se compilaron y ejecutaron los 14 casos contra el adaptador original. Los cuatro casos de edicion/colecciones sin numeracion fallaron antes del endurecimiento, porque la implementacion inventaba identificadores. Despues aprobaron junto con los demas casos. Las pruebas dedicadas de creacion verifican que una serie sin codigo no reserve numero ni escriba y que una serie valida use el numero reservado en vez de los valores enviados por el cliente.

```powershell
$env:NUANSYSTEM_RUN_SQL_INTEGRATION_TESTS='0'
$env:NUAN_RUN_SAP_PO_PILOT='0'
dotnet build NuanSystem.sln -c Release --no-restore --disable-build-servers -m:1 --verbosity quiet
dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj -c Release --no-build --no-restore --filter 'FullyQualifiedName~Features.Purchasing.PurchaseOrders' --logger 'trx;LogFileName=phase6-final-targeted.trx' --results-directory artifacts/backend-solid-phase6/targeted-final --verbosity minimal
dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj -c Release --no-build --no-restore --filter 'Category!=SqlServerIntegration&FullyQualifiedName!~SapPurchaseOrderPilotIntegrationTests' --logger 'trx;LogFileName=phase6-final.trx' --results-directory artifacts/backend-solid-phase6/final --verbosity minimal
```

Los gates solo afectan el proceso hijo. La suite excluye expresamente integracion SQL externa y piloto SAP. Las pruebas nuevas son puras o usan repositorios/servicios de numeracion simulados. No se inicia API/worker ni se contactan SAP o bases de datos. Build/test fuera del sandbox permiten SDK Windows y comunicacion local con testhost; no se instalan/restauran paquetes.

La suite global permanece fallida por los mismos nombres exactos que en fase 5:

- `SapSyncWinFormsContractTests.SapForms_UseCorporatePagingOperationalAndLookupContracts`.
- `SapSyncWinFormsContractTests.Designers_AreExplicitAndKeepSapCaptions`.
- `SapSyncWinFormsContractTests.SapProfileEditor_UsesCorporateActionButtonSizeAndFooterSpacing`.

No se editaron ni deshabilitaron esos tests. El resumen cuenta todos los nodos TRX, incluidos los casos con nombre visible repetido.

## Preservacion

Rama/HEAD conservados: `codex/auxiliary-master-generator`, `e97124a1e2863f066c1fa55ea992c78179f01623`.

Snapshot: 145 archivos previamente modificados/no rastreados en src/tests/docs/database; 144 conservan SHA-256 exacto y ninguno falta. El unico archivo de ese conjunto editado es `PurchaseOrderCommandHandlers.cs`: conserva las mejoras previas de errores HTTP y agrega validacion de numeracion/referencia al mapeo SAP. Hay copias anteriores de los cuatro archivos existentes afectados en `artifacts/backend-solid-phase6/before/`.

Se agregaron el calculador/tipos Domain, mapeo SAP, acceso a internos de Application para las pruebas y dos archivos de pruebas. No se modificaron procedimientos, repositorios, permisos, rutas, contratos HTTP ni frontend. No hubo stash, reset, commit o cambio de rama.

## Checklist aplicable

| Gate | Estado y evidencia |
|---|---|
| R1/R2 Alcance y descubrimiento | Validated: tres consumidores del adaptador, caracterizacion anterior, ADR y decision de numeracion separados. |
| R3 Arquitectura | Validated: Domain sin dependencias externas; adaptacion Application y mapeo SAP separado. Guardas de arquitectura/composicion incluidas en regresion. |
| R4 Operacion | Validated en memoria: resultados financieros conservados, permisos/workflow sin cambios, guards previos a reserva/escritura. Atomicidad real SQL: Not validated. |
| R5 Seguridad/tenant | Validated por inspeccion/regresion del alcance: contexto y autorizacion existentes intactos. Aislamiento real de filas: Not validated. |
| R6/R7 UI/Designer | Not applicable: sin cambios. |
| R8 API | Validated por inspeccion: DTO/default SAP compatibles; error de numeracion usa Result Conflict y contrato de errores existente. |
| R9 Persistencia | Validated estaticamente: SQL 046 conserva DEFAULT/CHECK 22 y contrato Dapper sin cambios. SQL real/migraciones: Not validated/no ejecutadas. |
| R10 Integracion | Validated estaticamente: codigo 22 fuera del calculo puro, mensaje anterior conservado; sin SAP requerido para calcular. SAP real: Not validated. |
| R11/R12 Verificacion y entrega | Validated con limite conocido: build limpio, 79 enfocadas y 29 nuevas aprobadas; global conserva 3 fallos previos. Revision independiente y diff sin problemas de espacios. |

## Evidencia y limites

ADR: `docs/architecture/BACKEND-SOLID-PHASE-6-PURCHASE-CALCULATION.md`. Artefactos locales ignorados por Git: `artifacts/backend-solid-phase6/baseline.json`, `before/`, `before-tests/phase6-before.trx`, `red/phase6-red.trx`, `targeted-final/phase6-final-targeted.trx`, `build-final.log`, `final/phase6-final.trx` y `final/summary.json`.

No se ejecuta SAP ni se modifican bases de datos. Sin renumeracion ni recalculo de documentos historicos. Se conservan las normalizaciones heredadas USD/1 y los truncamientos del adaptador; no se corrigen reglas financieras adicionales ni se introduce redondeo dependiente de moneda. Estas pruebas no acreditan integracion real, rendimiento o disponibilidad de produccion. Las fases 7 y siguientes quedan fuera de esta entrega.
