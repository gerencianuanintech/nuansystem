# Fase 7: validacion del piloto transaccional

Fecha: 2026-10-23. Alcance: piloto BusinessPartners, compatible con el contrato transaccional existente.

## Implementacion y estado

Las tres escrituras de socios dependen de IBusinessPartnerUnitOfWork y reciben IBusinessPartnerTransaction, sin conexiones/transacciones ADO.NET. Persistence adapta las operaciones al mismo repositorio, escritor LocalOutbox y pareja conexion/transaccion del runner existente. Se conservan DTOs, mensajes, auditoria, GlobalId, orden de llamadas, aislamiento scoped y el comportamiento de duplicados existente.

El contexto caduca al salir del callback y verifica empresa/cancelacion antes y despues de cada llamada. La unidad scoped rechaza anidamiento/solapamiento de ExecuteAsync. El callback debe ejecutar/esperar comandos secuencialmente: no se ofrece paralelismo de comandos sobre la misma conexion. No se usan TransactionScope, AsyncLocal, estado global ni transacciones distribuidas.

ITransactionRunner y sus sobrecargas ADO.NET se mantienen intactos como contrato; los demas verticales no se migran. La validacion SQL aislada requerida por el plan no se ejecuto por la restriccion expresa de no usar bases. Esta entrega cierra el piloto compatible, no la migracion global ni la demostracion de atomicidad real SQL.

SqlTransactionRunner conserva diagnostico original cuando rollback falla, no abre con cancelacion previa y comprueba cancelacion antes de commit. Un error de commit no provoca retry automatico. El fallo secundario de rollback se conserva internamente en Exception.Data con clave NuanSystem.Transaction.RollbackException, sin agregarlo al contrato HTTP.

## Resultado de pruebas

- Solucion Release: 0 errores y 0 advertencias.
- Tres regresiones del runner fallaron antes de corregirlo: cancelacion previa, cancelacion antes de commit y rollback que sustituye el error original.
- Primera pasada enfocada: 35 aprobadas. Se agregaron cinco casos relevantes de commit/cancelacion/disposal; la suite final contiene 40 casos del grupo enfocado, todos aprobados.
- 21 casos nuevos aprobados: contexto/unidad 13, runner 6, composicion scoped 1 y repeticion de alta 1.
- Regresion local completa: 1229 ejecutadas, 1226 aprobadas y los mismos tres fallos visuales WinForms que fase 6.
- Revision independiente de solo lectura: sin hallazgos importantes; condicion de uso secuencial documentada.

Las pruebas usan SqlTransactionRunner real con ITenantConnectionFactory, IDbConnection e IDbTransaction simulados, y repositorios/escritor simulados. Verifican que se solicita commit/rollback y que los participantes reciben los mismos objetos, no que SQL Server haya revertido filas o auditoria reales. La prueba de alta repetida demuestra la conservacion del camino de validacion de duplicados con su fixture; no prueba deduplicacion durable bajo concurrencia.

Los fallos globales preexistentes son exactamente:

- `SapSyncWinFormsContractTests.SapForms_UseCorporatePagingOperationalAndLookupContracts`.
- `SapSyncWinFormsContractTests.Designers_AreExplicitAndKeepSapCaptions`.
- `SapSyncWinFormsContractTests.SapProfileEditor_UsesCorporateActionButtonSizeAndFooterSpacing`.

La suite global sigue fallida. No se editaron ni deshabilitaron esos casos.

## Ejecucion segura

```powershell
$env:NUANSYSTEM_RUN_SQL_INTEGRATION_TESTS='0'
$env:NUAN_RUN_SAP_PO_PILOT='0'
dotnet build NuanSystem.sln -c Release --no-restore --disable-build-servers -m:1 --verbosity quiet
dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj -c Release --no-build --no-restore --filter 'FullyQualifiedName~BusinessPartnerUnitOfWorkTests|FullyQualifiedName~SqlTransactionRunnerTests|FullyQualifiedName~BusinessPartnerSyncPublishingTests|FullyQualifiedName~BackendCompositionTests'
dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj -c Release --no-build --no-restore --filter 'Category!=SqlServerIntegration&FullyQualifiedName!~SapPurchaseOrderPilotIntegrationTests' --logger 'trx;LogFileName=phase7-final.trx' --results-directory artifacts/backend-solid-phase7/final --verbosity minimal
```

Gates solo del proceso hijo; SQL externo/piloto SAP excluidos. Sin inicio de API/workers operativos, llamadas SAP, acceso a bases, instalacion/restauracion ni cambios globales. La ejecucion local fuera del sandbox permite comunicacion de testhost y consulta del SDK Windows. Las pruebas de composicion construyen un Host sin iniciarlo y mantienen barreras de IO.

## Preservacion y revision

Rama/HEAD conservados: `codex/auxiliary-master-generator`, `e97124a1e2863f066c1fa55ea992c78179f01623`.

Snapshot: 155 archivos modificados/no rastreados existentes en src/tests/docs/database. 152 conservan huella SHA-256 exacta y ninguno falta. Los tres editados dentro de ese conjunto son BackendCompositionTests.cs, BusinessPartnerSyncPublishingTests.cs y PersistenceServiceRegistration.cs; se preservaron las mejoras de fases anteriores. Copias previas de los siete archivos existentes afectados en `artifacts/backend-solid-phase7/before/`.

Inventario legacy previo en `legacy-consumers.txt`. No se retiran sobrecargas todavia usadas, no se cambian repositorios/procedimientos/SQL, SAP, Domain, API ni frontend. Sin stash, reset, commit ni cambio de rama. Comprobacion de espacios del diff sin errores.

| Gate | Estado y evidencia |
|---|---|
| R1/R2 Alcance y descubrimiento | Validated: inventario, piloto unico y ADR; no habia sesion equivalente. |
| R3 Arquitectura | Validated: Application expresa operacion sin ADO en sus handlers piloto; Persistence conserva los detalles. Composicion completa/scopes aprobados. |
| R4 Operacion | Validated en memoria para propagacion, orden, rollback solicitado y diagnostico. Atomicidad SQL/auditoria real: Not validated. |
| R5 Empresa/seguridad | Validated en memoria para cambio de contexto, lifetime, scoped DI y cancelacion. Aislamiento de filas real: Not validated; permisos existentes sin cambios. |
| R6/R7 UI | Not applicable: sin cambios. |
| R8 API | Validated por inspeccion: contratos HTTP/rutas/DTO/resultados conservados. |
| R9 Persistencia | Validated por revision: mismos metodos y objetos ADO para el dato y Outbox; SQL sin cambios. SQL aislado: Not validated/no ejecutado. |
| R10 Integracion | Validated para publicacion local con dobles. SAP y entrega/replicacion reales: Not validated, fuera de alcance. |
| R11/R12 Evidencia | Validated con limites: build limpio, 21 nuevas pasan, 40 enfocadas pasan en suite final; global conserva tres fallos previos y el gate SQL pendiente. |

## Evidencia y trabajo posterior

ADR: `docs/architecture/BACKEND-SOLID-PHASE-7-TRANSACTION-SESSION.md`. Artefactos ignorados por Git: `artifacts/backend-solid-phase7/baseline.json`, `before/`, `legacy-consumers.txt`, `red/phase7-red.trx`, `targeted/phase7-targeted.trx`, `build-final.log`, `final/phase7-final.trx` y `final/summary.json`.

Antes de migrar los siguientes verticales o retirar compatibilidad: demostrar en SQL aislado autorizado rollback de dato/auditoria/Outbox, aislamiento de empresas, concurrencia, idempotencia e identidad. No se afirma disponibilidad, rendimiento ni atomicidad SQL de produccion a partir de estos dobles. No se inician fases 8 y siguientes.
