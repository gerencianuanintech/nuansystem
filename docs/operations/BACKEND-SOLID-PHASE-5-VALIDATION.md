# Fase 5: validacion de contratos de socios

Fecha: 2026-10-23. Implementacion de fase 5 del plan de consolidacion, sin ejecutar SAP ni modificar bases de datos.

## Resultado

- CRUD de socios deja de exponer ImportSupplierFromSapAsync.
- ISapSupplierImportRepository conserva la firma y DTO actuales; SapSupplierImportService usa ese contrato para importar y conserva IBusinessPartnerRepository para consultar/matching.
- BusinessPartnerRepository implementa ambos contratos con una sola instancia scoped y dos aliases.
- Cuerpos de metodos de persistencia sin cambios. Una comparacion contra las copias previas, invirtiendo exclusivamente using/declaracion de interfaces, da igualdad completa. La misma comprobacion confirma que la logica del importador solo cambio su dependencia/receptor de llamada.
- Solucion Release: 0 errores y 0 advertencias.
- Pruebas enfocadas: 50 aprobadas.
- Regresion completa local: 1179 ejecutadas, 1176 aprobadas y los mismos 3 fallos preexistentes de contratos visuales WinForms.
- 6 casos nuevos aprobados: ausencia del metodo SAP en CRUD, instancia compartida/scopes y cuatro casos de importacion/preview. Las pruebas anteriores de alta/edicion se ampliaron con identidad retornada y auditoria; baja y Outbox conservan su cobertura transaccional anterior.
- Revision independiente de solo lectura: sin hallazgos.

Fallos preexistentes, con nombres iguales al TRX de fase 4:

- `SapSyncWinFormsContractTests.SapForms_UseCorporatePagingOperationalAndLookupContracts`.
- `SapSyncWinFormsContractTests.Designers_AreExplicitAndKeepSapCaptions`.
- `SapSyncWinFormsContractTests.SapProfileEditor_UsesCorporateActionButtonSizeAndFooterSpacing`.

La suite global permanece fallida. Esas pruebas no se modificaron ni deshabilitaron.

## Comprobaciones

Se agrego primero la prueba que exige ausencia del metodo SAP en el contrato CRUD. Fallo por ese motivo (RED); despues de la extraccion paso junto con toda la cobertura enfocada (GREEN).

```powershell
$env:NUANSYSTEM_RUN_SQL_INTEGRATION_TESTS='0'
$env:NUAN_RUN_SAP_PO_PILOT='0'
dotnet build NuanSystem.sln -c Release --no-restore --disable-build-servers -m:1 --verbosity quiet
dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj -c Release --no-build --no-restore --filter 'FullyQualifiedName~BusinessPartnerSyncPublishingTests|FullyQualifiedName~SapSupplierImportServiceTests|FullyQualifiedName~SapSupplierImportFailureTests|FullyQualifiedName~BackendCompositionTests'
dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj -c Release --no-build --no-restore --filter 'Category!=SqlServerIntegration&FullyQualifiedName!~SapPurchaseOrderPilotIntegrationTests' --logger 'trx;LogFileName=phase5-final.trx' --results-directory artifacts/backend-solid-phase5/final --verbosity minimal
```

Los gates solo afectan el proceso hijo. No se inicia API ni worker operativo. SQL externo y piloto SAP se excluyen expresamente. La composicion usa un Host sin iniciar y barreras HTTP/SQL; las pruebas de importacion y CRUD usan dobles. La ejecucion local fuera del sandbox permite las consultas del SDK Windows y la comunicacion de testhost; no cambia estos limites ni instala/restaura paquetes.

## Preservacion

Rama/HEAD sin cambios: `codex/auxiliary-master-generator`, `e97124a1e2863f066c1fa55ea992c78179f01623`.

Snapshot de 138 archivos modificados/no rastreados previos en src/tests/docs/database: 134 conservan su huella SHA-256; los 4 restantes se editaron dentro del alcance:

- `BackendCompositionTests.cs`: prueba del contrato y aliases scoped.
- `SapSupplierImportFailureTests.cs`: fixture segregado conservando cancelacion y errores seguros.
- `SapSupplierImportService.cs`: parametro y receptor de importacion.
- `PersistenceServiceRegistration.cs`: aliases sobre un concreto scoped.

Ningun archivo previo falta. Copias anteriores de los ocho archivos existentes afectados en `artifacts/backend-solid-phase5/before/`; nuevo contrato en Application/Abstractions/Sap. Sin reset, stash, commit, cambio de rama ni ediciones de SQL/frontend.

## Checklist de revision aplicable

| Gate | Estado y evidencia |
|---|---|
| R1 Alcance | Validated: solo puerto SAP, consumidor, aliases, pruebas y documentacion. |
| R2 Descubrimiento | Validated: mapa de consumidores y decision en ADR de fase 5; no existe puerto equivalente. |
| R3 Arquitectura | Validated: contratos Application, detalles Persistence; mismo patron de repositorio existente. Arista del contrato adicional documentada en ADR, sin cambios del nucleo .codex. |
| R4 Operacion | Validated en memoria e inspeccion: auditoria, identidad, resultados, Outbox y limites transaccionales conservados. Atomicidad SQL real: Not validated. |
| R5 Seguridad/tenant | Validated para composicion scoped y fuente de conexion sin cambios. Aislamiento real de filas/permisos HTTP: Not validated, fuera de esta extraccion. |
| R6/R7 WinForms | Not applicable: no cambios UI/Designer. |
| R8 API/servicios | Validated por inspeccion: rutas, DTO y contratos HTTP sin cambios; consumidores del nuevo puerto migrados. |
| R9 Persistencia | Validated por comparacion de cuerpos y revision del procedimiento como texto. SQL real/migraciones: Not validated/no ejecutadas. |
| R10 Integracion | Validated con dobles: matching, no duplicacion, auditoria, cancelacion y errores. SAP real: Not validated. |
| R11 Verificacion | Validated con limite conocido: build limpio, 50 pruebas enfocadas, 6 nuevas pasan; suite global conserva 3 fallos anteriores. Revision independiente y espacios del diff sin problemas. |
| R12 Evidencia | Validated: ADR, este registro, snapshot, logs y TRX locales. |

## Evidencia y limites

Artefactos ignorados por Git: `artifacts/backend-solid-phase5/baseline.json`, `before/`, `red/phase5-red.trx`, `targeted/phase5-targeted.trx`, `build-final.log`, `final/phase5-final.trx` y `final/summary.json`.

El ADR es `docs/architecture/BACKEND-SOLID-PHASE-5-BUSINESS-PARTNER-CONTRACTS.md`. No hay cambios de identidad, schema, procedimiento, transacciones ni contrato HTTP. Los DTO de importacion permanecen en su ubicacion actual para evitar cambios adicionales. No se inicia fase 6 ni fase 7. Estas comprobaciones no acreditan SAP/SQL real ni disponibilidad o rendimiento de produccion.
