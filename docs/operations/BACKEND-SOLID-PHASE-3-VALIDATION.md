# Evidencia de consolidación backend — fase 3

Fecha: 2026-10-23. Estado: implementada y validada localmente; se conservan tres fallos previos de WinForms. Alcance: fase 3 del plan `docs/superpowers/plans/2026-10-22-backend-solid-consolidation.md`.

Decisiones: `docs/architecture/BACKEND-SOLID-PHASE-3-ERROR-SEMANTICS.md`.

## Entrega y semántica

| Componente | Cambio |
|---|---|
| Application/Common/Models/FailureKind.cs | Categorías propias de Application, sin HTTP |
| Application/Common/Models/Result.cs | Propiedad nullable no posicional y sobrecarga explícita; constructor/deconstrucción conservados |
| Api/Extensions/ResultExtensions.cs | Traducción explícita a 400/404/409/403/503/500, conservando envelope |
| Purchasing/PurchaseOrders/Queries/PurchaseOrderQueryHandlers.cs | Consulta por Id inexistente → 404 |
| Purchasing/PurchaseOrders/Commands/PurchaseOrderCommandHandlers.cs | Ausencia inicial → 404, conflicto → 409, denegación de serie → 403, lectura fallida posterior a escritura → 500 |
| SapSync/Queries/TestSapServiceLayerConnectionQuery.cs | Tester con resultado negativo → 503 seguro; éxito → 200; excepción/cancelación siguen propagándose |

Los paths Application/Api de la tabla parten de `src/Backend/NuanSystem.*` según la capa. No se modifica Domain, Persistence, SQL, endpoints, autenticación, cliente WinForms ni sus formularios. No se cambian permisos, numeración, reglas comerciales, operaciones de persistencia o publicación.

`Failure(message, errors)` sigue siendo Validation/400. El constructor existente sin categoría también conserva 400. La categoría no se infiere del texto. Los otros módulos mantienen su comportamiento anterior hasta migrarlos explícitamente.

Unexpected y valores de enum desconocidos producen 500 con mensaje y código `UNEXPECTED_ERROR` seguros, descartando mensaje/errores originales y sin devolver Value. El mensaje indica consultar el estado de la operación; no recomienda repetir una escritura posiblemente confirmada. Otros tipos de fallo conservan los mensajes/códigos de negocio seguros suministrados por sus handlers.

La forma HTTP conserva los campos `success`, `message`, `data`, `errors`. Las excepciones inesperadas siguen bajo el middleware existente; no se capturan todas las excepciones como 400 o 503. No se modifica la respuesta 401 de autenticación. La importación manual de proveedores sigue devolviendo 200 con resumen parcial explícito de fase 2.

## Compatibilidad WinForms comprobada

Se reutilizan sin cambios `NuanApiClient`, `ApiClientException`, `ApiClientErrorMessageFormatter`, clientes tipados de compras/SAP y el ViewModel SAP.

Las pruebas ejecutan el IResult real sobre DefaultHttpContext y el serializador ASP.NET en memoria. Un HttpMessageHandler simulado devuelve esa respuesta al cliente WinForms real, sin listener ni arranque de aplicación.

Se verifican estados 400/404/409/403/503, código/mensaje/campo de ApiError, sesión y empresa sin cambios, legacy 400, éxito 200, error inesperado sin secretos, consulta tipada de compras y SAP 503 con restablecimiento de IsBusy.

## Evidencia RED → GREEN

1. El primer build detectó la ausencia esperada del contrato FailureKind. Se agregó el contrato mínimo, todavía sin cambiar el mapper/handlers.
2. Con 32 pruebas nuevas compiladas: 20 fallos esperados y 12 aprobadas. Evidencia: `artifacts/backend-solid-phase3/red/phase3-red.trx`.
3. Tras implementar mapper y consumidores, 162 pruebas dirigidas aprobadas: arquitectura, compras, cliente, proveedores y configuración SAP. Evidencia: `targeted/phase3-targeted.trx`.
4. Se añadió una regresión sobre el mensaje post-escritura: falló porque recomendaba reintentar. Se corrigió el mensaje y la corrida final verifica esa prueba junto con las anteriores. RED: `red/phase3-guidance-red.trx`.

Los tests nuevos se encuentran en:

- `Architecture/ResultHttpMappingTests.cs`.
- `Features/Common/ApiClientHttpErrorCompatibilityTests.cs`.
- `Features/Purchasing/PurchaseOrders/PurchaseOrderErrorSemanticsTests.cs`.
- `Features/SapSync/SapConnectionErrorSemanticsTests.cs`.

Infraestructura de pruebas: `Infrastructure/InMemoryResultHandler.cs`. Paths relativos a `tests/NuanSystem.Application.Tests`.

## Validación final

```powershell
dotnet build NuanSystem.sln -c Release --no-restore --disable-build-servers -m:1 --verbosity quiet
```

Resultado: compilación correcta, 0 advertencias y 0 errores. Log: `artifacts/backend-solid-phase3/build-final.log`.

```powershell
$env:NUANSYSTEM_RUN_SQL_INTEGRATION_TESTS='0'
$env:NUAN_RUN_SAP_PO_PILOT='0'
dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj -c Release --no-build --no-restore --filter 'Category!=SqlServerIntegration&FullyQualifiedName!~SapPurchaseOrderPilotIntegrationTests' --logger 'trx;LogFileName=phase3-final.trx' --results-directory artifacts/backend-solid-phase3/final --verbosity minimal
```

Resultado: **1.139 ejecutadas, 1.136 aprobadas y 3 fallidas**. Las **33 pruebas nuevas** y las **40 protecciones de arquitectura/composición originales** están aprobadas. También pasan las regresiones de fases anteriores, salvo los tres fallos ya conocidos.

El conjunto exacto de nombres fallidos coincide con fase 2:

- `SapSyncWinFormsContractTests.SapForms_UseCorporatePagingOperationalAndLookupContracts`.
- `SapSyncWinFormsContractTests.Designers_AreExplicitAndKeepSapCaptions`.
- `SapSyncWinFormsContractTests.SapProfileEditor_UsesCorporateActionButtonSizeAndFooterSpacing`.

No se altera ni se deshabilita ese trabajo fuera de alcance. El código de salida de la suite global es 1; no se presenta como suite totalmente aprobada. TRX: `artifacts/backend-solid-phase3/final/phase3-final.trx`; resumen: `summary.json`.

Build/runner se ejecutan localmente fuera del sandbox por la consulta del SDK Windows y comunicación VSTest/testhost, manteniendo filtros y gates. No se instalan/restauran paquetes ni se cambia configuración global.

## Preservación y revisión

Rama/HEAD conservados: `codex/auxiliary-master-generator`, `e97124a1e2863f066c1fa55ea992c78179f01623`. Snapshot: 84 archivos modificados/no rastreados existentes, en `artifacts/backend-solid-phase3/preserved-files.json`.

De ellos, 83 mantienen su huella; la query SAP es el único archivo previo editado dentro del alcance. Se reconstruyó su contenido anterior invirtiendo exclusivamente el cambio aprobado y se comprobó coincidencia SHA-256 exacta con el snapshot; copia en `TestSapServiceLayerConnectionQuery.before.cs`. El resto del trabajo de las fases previas permanece intacto. No hay cambios previos alterados fuera de alcance, commits, stash, reset ni cambio de rama.

Revisión independiente de solo lectura de contratos, mapper, handlers, tests y cliente real: sin hallazgos importantes accionables. El ajuste posterior de mensaje se verificó mediante RED/GREEN y regresión completa.

## Límites

- Es una migración inicial de consumidores, no conversión automática de todos los módulos a nuevas categorías.
- No se acredita autenticación HTTP real, UI renderizada, SAP, transacciones SQL, rendimiento ni disponibilidad de producción.
- Un 500 posterior a escritura no prueba rollback. No se añade retry automático.
- Las fases 4 y siguientes permanecen sin iniciar; las declaraciones históricas de mapeo global 400 deben interpretarse con esta migración deliberada y sus pruebas.
