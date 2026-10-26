# Evidencia de consolidación backend — fase 2

Fecha: 2026-10-23. Estado: implementada y validada localmente, con tres fallos previos de WinForms documentados. No se ejecutó SAP, SQL, API ni workers operativos.

## Alcance y preservación

Plan: fase 2 de `docs/superpowers/plans/2026-10-22-backend-solid-consolidation.md`. Decisiones: `docs/architecture/BACKEND-SOLID-PHASE-2-SUPPLIER-ERRORS.md`.

Rama y HEAD conservados: `codex/auxiliary-master-generator`, `e97124a1e2863f066c1fa55ea992c78179f01623`. Se registraron huellas de 80 archivos modificados/no rastreados existentes en `artifacts/backend-solid-phase2/preserved-files.json` y una copia previa del importador en `SapSupplierImportService.before.cs`.

Se modifican únicamente dos archivos productivos:

- `Application/Features/SapSync/Services/SapSupplierImportService.cs`: cancelación explícita, excepción por fila segura, logger opcional y correlación compartida con Inbox.
- `Application/Features/SapSync/Dtos/SapSupplierImportResultDto.cs`: propiedades opcionales `ErrorCode` y `CorrelationId` sobre el mismo DTO existente, omitidas cuando null.

Se añaden `SapSupplierImportFailureTests.cs` y los dos documentos de fase 2. No se modifica SQL, registro DI, middleware, mapper HTTP, cliente WinForms ni reglas de identidad. La edición previa del usuario que genera códigos mediante `SapBusinessPartnerIdentityGenerator` se conserva, comprobando el delta contra la copia del inicio y no atribuyendo el diff completo de Git a esta entrega.

De los 80 archivos previos registrados, 79 mantienen su huella; el importador es el único modificado dentro del alcance. No hay archivos previos alterados fuera de alcance. No se hacen commits, stash, reset, cambio de rama ni instalación/restauración de dependencias.

## Comportamiento resultante

1. Las excepciones inesperadas al persistir una fila producen `Failed`, `SUPPLIER_IMPORT_FAILED` y una referencia de correlación, y permiten continuar con otras filas.
2. No se copia el mensaje técnico ni la excepción completa a resultado público, Inbox, log de resumen o diagnóstico. El logger recibe únicamente código, tipo de excepción, referencia y empresa cuando está disponible.
3. Toda `OperationCanceledException` se propaga. Los checkpoints observan cancelación antes de iniciar, después de leer watermark, entre filas, después de persistencia y antes de devolver resumen/avanzar watermark.
4. Los fallos fuera del límite de fila —lector, carga local o log final— siguen propagándose; no se inventa un resultado exitoso ni se avanza el watermark tras esos errores.
5. Manual conserva HTTP 200 con resumen parcial explícito y mensaje indicando errores. El handler worker devuelve Failed cuando hay filas fallidas. Ninguno devuelve un resultado final si recibe cancelación.
6. El constructor/deconstrucción y las cinco propiedades JSON previas del resultado por fila se conservan. Las propiedades adicionales se incluyen en fallos; no aparecen como null en resultados exitosos.

## Pruebas antes de implementar

Primera corrida RED con mocks y producción anterior: 12 pruebas, 10 fallidas y 2 aprobadas. Fallos esperados: cancelación capturada, trabajo realizado con token ya cancelado, exposición de excepción, ausencia de código/referencia y avance después de cancelación tardía.

Después del primer cambio pasaron 66 pruebas dirigidas de proveedores y arquitectura. Se añadieron siete casos de borde: cancelación entre lectura de watermark y siguiente lectura, propagación por ambos handlers, correlación vacía y fallos de infraestructura externos al registro.

La segunda corrida RED tuvo 19 pruebas: 18 aprobadas y un fallo esperado en cancelación inmediatamente después de leer watermark. Se añadió el checkpoint antes de consultar SAP; la corrida final verifica las 19 aprobadas.

TRX: `artifacts/backend-solid-phase2/red/phase2-red.trx`, `red/phase2-boundary-red.trx` y `targeted/phase2-targeted.trx`.

## Compilación final

```powershell
dotnet build NuanSystem.sln -c Release --no-restore --disable-build-servers -m:1 --verbosity quiet
```

Resultado: compilación correcta, cero advertencias y cero errores. Log: `artifacts/backend-solid-phase2/build-final.log`.

## Regresión final

Gates establecidos solo en el proceso de pruebas:

```powershell
$env:NUANSYSTEM_RUN_SQL_INTEGRATION_TESTS='0'
$env:NUAN_RUN_SAP_PO_PILOT='0'
dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj -c Release --no-build --no-restore --filter 'Category!=SqlServerIntegration&FullyQualifiedName!~SapPurchaseOrderPilotIntegrationTests' --logger 'trx;LogFileName=phase2-final.trx' --results-directory artifacts/backend-solid-phase2/final --verbosity minimal
```

Resultado: 1.106 ejecutadas, 1.103 aprobadas, 3 fallidas. Las 19 nuevas y las 40 de arquitectura/composición de fase 1 están aprobadas. No se presenta la suite global como verde.

Se comparó el conjunto exacto de fallos con `artifacts/backend-solid-phase1/final/phase1-final.trx`; son los mismos tres tests de `SapSyncWinFormsContractTests`:

- `SapForms_UseCorporatePagingOperationalAndLookupContracts`.
- `Designers_AreExplicitAndKeepSapCaptions`.
- `SapProfileEditor_UsesCorporateActionButtonSizeAndFooterSpacing`.

No se corrigen ni se deshabilitan esas pruebas: comprueban texto/posiciones de archivos WinForms ya modificados y están fuera del alcance de esta fase.

Resultados en `artifacts/backend-solid-phase2/final/phase2-final.trx`, `final-tests.log` y `summary.json`. Build y runner se ejecutaron localmente fuera del sandbox para permitir consulta del SDK Windows y comunicación VSTest/testhost, conservando filtros y gates. Esto no habilitó servicios ni cambió configuración global.

## Revisión y límites

Revisión independiente de solo lectura del delta: sin problemas importantes accionables. No ejecutó pruebas ni servicios; el coordinador verificó build, TRX y huellas.

- Cancelar no revierte filas ya confirmadas ni garantiza recuperación automática de Inbox Processing; ese circuito sigue bajo el plan de endurecimiento SAP.
- No se cambia el middleware global, la clasificación HTTP ni el motor de reintentos. Los errores anteriores a la fila conservan su ruta global de error existente.
- El diagnóstico deliberadamente omite excepción completa y payload para evitar registrar secretos. La fuente de fallo se identifica por tipo y correlación, no por mensajes originales.
- Los tests son con dependencias sustituidas y no certifican SAP real, transacciones SQL, rendimiento ni aislamiento de filas.
- La fase 3 y siguientes permanecen sin iniciar.
