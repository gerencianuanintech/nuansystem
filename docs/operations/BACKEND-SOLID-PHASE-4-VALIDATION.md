# Fase 4: validacion de contratos y capacidades SAP

Fecha: 2026-10-23. Alcance: fase 4 de `2026-10-22-backend-solid-consolidation.md`.

## Resultado

- Solucion Release: compilacion correcta, 0 errores y 0 advertencias.
- Regresion local: 1173 ejecutadas, 1170 aprobadas, 3 fallidas preexistentes.
- 34 casos nuevos aprobados: registro 11, despacho direccional 9, cancelacion de importadores reales 7, scheduler 3, preparador 3 y coherencia perfil/catalogo 1.
- Cinco pruebas anteriores de exportacion pendiente se migraron a comprobar que los importadores no implementan el contrato de exportacion. Se actualizaron los consumidores y las comprobaciones de registro DI.
- Revision independiente de solo lectura: sin hallazgos importantes.

Los nombres exactos de los tres fallos coinciden con el TRX de fase 3:

- `SapSyncWinFormsContractTests.SapForms_UseCorporatePagingOperationalAndLookupContracts`.
- `SapSyncWinFormsContractTests.Designers_AreExplicitAndKeepSapCaptions`.
- `SapSyncWinFormsContractTests.SapProfileEditor_UsesCorporateActionButtonSizeAndFooterSpacing`.

La suite global permanece fallida; no se deshabilitaron ni corrigieron esas pruebas visuales fuera de alcance. Los tres casos de politica SQL de Cities que tienen el mismo nombre visible en TRX se cuentan individualmente: los totales proceden de los nodos de resultados completos.

## Ejecucion segura

Se usaron estas comprobaciones locales, sin iniciar API, workers operativos ni servicios externos:

```powershell
$env:NUANSYSTEM_RUN_SQL_INTEGRATION_TESTS='0'
$env:NUAN_RUN_SAP_PO_PILOT='0'
dotnet build NuanSystem.sln -c Release --no-restore --disable-build-servers -m:1 --verbosity quiet
dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj -c Release --no-build --no-restore --filter 'Category!=SqlServerIntegration&FullyQualifiedName!~SapPurchaseOrderPilotIntegrationTests' --logger 'trx;LogFileName=phase4-final.trx' --results-directory artifacts/backend-solid-phase4/final --verbosity minimal
```

Los gates solo afectan el proceso hijo. Se excluyen SQL externo y piloto SAP incluso si sus gates estan apagados. Build y VSTest se ejecutaron localmente fuera del sandbox para permitir acceso al SDK Windows y comunicacion testhost, sin instalar ni restaurar paquetes.

Antes de implementar, los nuevos contratos/registro hicieron fallar la compilacion de las pruebas por tipos inexistentes (`red-build.log`). Tras implementar y migrar consumidores, 322 pruebas SAP iniciales aprobaron. Despues se ampliaron los casos de cancelacion, despacho y capacidades y se verifico la solucion completa y la regresion anterior. No se repitieron comprobaciones exitosas sin cambios que lo justificaran.

Las pruebas nuevas verifican:

- Importador sin exportador ni procesador programado no anuncia exportacion ni perfiles ejecutables.
- La configuracion no puede agregar direcciones o modos inexistentes ni habilitar un procesador desactivado en metadata.
- Duplicados, modos invalidos y procesadores programados/reintentos incompatibles se rechazan al construir el registro.
- Scheduler no reserva ni emite ejecuciones sin procesador, incluido fallback legacy.
- Preparador rechaza Both, exportacion y modo Incremental no implementado antes de invocar el procesador.
- Both nunca se convierte silenciosamente en importacion. El despacho importa/exporta exclusivamente por su contrato, conserva resultados/logs y libera el lock.
- Siete importadores de la composicion real propagan cancelacion previa sin intentos en las barreras de HTTP/SQL.

## Preservacion y evidencia

Rama y HEAD permanecen `codex/auxiliary-master-generator`, `e97124a1e2863f066c1fa55ea992c78179f01623`. Se conservaron los cambios anteriores sin stash, reset ni commits.

`artifacts/backend-solid-phase4/baseline.json` registra SHA-256 de 95 archivos modificados/no rastreados previos en src/tests/docs/database. 94 permanecen identicos. El unico cambio dentro de ese conjunto es ampliar `BackendCompositionTests.cs`, necesario para probar la nueva composicion; su contenido anterior queda en `before/`. No falta ningun archivo del snapshot. Las copias previas adicionales permiten revisar los 38 archivos C# existentes afectados por esta fase, incluidos los dos contratos/handlers obsoletos retirados.

Evidencia local ignorada por Git:

- `artifacts/backend-solid-phase4/build-final.log`.
- `artifacts/backend-solid-phase4/final/phase4-final.trx` y `summary.json`.
- `artifacts/backend-solid-phase4/targeted/phase4-targeted.trx`.
- `artifacts/backend-solid-phase4/before/` y `baseline.json`.

La comprobacion de espacios del cambio paso. La documentacion de arquitectura y matriz efectiva esta en `docs/architecture/BACKEND-SOLID-PHASE-4-SAP-CONTRACTS.md`.

## Limites y compatibilidad

Sin ejecucion SAP ni cambios en bases de datos, scripts SQL, configuracion global o frontend. Las rutas y DTO de WinForms se conservan. El catalogo y la validacion ahora restringen los perfiles a Warehouses, Countries, Provinces y Cities Full, que tienen procesadores reales. Los perfiles anteriores de Suppliers, Items y PaymentTerms no se eliminan ni modifican: ya no se aceptan como ejecutables por el scheduler. Sus comandos de importacion siguen disponibles.

No se implementan exportaciones, outbox SAP, procesadores nuevos, reintento generico ni fases posteriores. La validacion en memoria y con dobles no acredita conectividad real, transacciones SQL, rendimiento ni disponibilidad de produccion.
