# Correccion WinForms y revalidacion del backend

Fecha: 2026-10-24. Resultado actual: compilacion limpia y suite local completa aprobada. Sin ejecucion SAP, acceso/modificacion de bases ni despliegue.

## Correccion aplicada

Los tres fallos tenian una causa comun en SapSyncProfileEditForm.Designer.cs: el editor con tarjetas introdujo layout mediante helpers y desplazo la cabecera/pie respecto al contrato visual aprobado. Se corrigio el formulario, sin modificar, omitir ni debilitar pruebas.

- Se conservaron los 48 controles actuales, sus nombres, seis tarjetas selectoras, acciones, jerarquia y comportamiento de seleccion/mapeo/programacion.
- Los helpers de layout se sustituyeron por propiedades explicitas dentro de InitializeComponent, con Name y tabulacion por contenedor.
- Se restauro la cabecera de dos columnas (empresa160/50, codigo700/50, nombre160/78, descripcion700/78), el titulo Entidades y programacion y el pie heredado (cancelar936/672, guardar1042/672).
- Los botones heredados conservan el tamano corporativo100x36 de BaseEditForm; no se modifica la base ni sus cambios previos. Las cuatro acciones del editor usan NuanActionButton y las tarjetas siguen como selectores SimpleButton.
- Se corrigieron solapamientos entre modo/lote, intervalo/hora y el pie al reducir la ventana. El minimo1160x752 preserva las seis tarjetas y las acciones.
- Se balancearon18 pares BeginInit/EndInit y3 pares SuspendLayout/ResumeLayout. Se mantuvieron disposicion y recursos UTF-8 sin mojibake.

El archivo de comportamiento SapSyncProfileEditForm.cs y su .resx son byte a byte identicos a las copias previas. No se cambiaron eventos, ViewModel, clientes HTTP, permisos, menus, contratos backend, SQL ni SAP. Las referencias fueron BaseEditForm, el Designer anterior aprobado y el patron corporativo de controles/formularios.

## RED/GREEN y regresion

Antes del cambio: 13 pruebas de SapSyncWinFormsContractTests,10 aprobadas y3 fallidas. Despues:13/13 aprobadas. Los casos corregidos son:

- Designers_AreExplicitAndKeepSapCaptions.
- SapForms_UseCorporatePagingOperationalAndLookupContracts.
- SapProfileEditor_UsesCorporateActionButtonSizeAndFooterSpacing.

Resultados finales:

| Verificacion | Resultado |
|---|---|
| Solucion Release | 0 errores,0 advertencias. |
| Suites dirigidas backend/HTTP/WinForms | 296 aprobadas,0 fallos. |
| Regresion local completa | 1263 aprobadas,0 fallos,0 omitidas. |
| Casos modificados/agregados para pasar | Ninguno: mismo total de fase9 y pruebas anteriores intactas. |
| Revision independiente | Sin hallazgos importantes tras corregir los solapamientos observados. |

```powershell
$env:NUANSYSTEM_RUN_SQL_INTEGRATION_TESTS='0'
$env:NUAN_RUN_SAP_PO_PILOT='0'
dotnet build NuanSystem.sln -c Release --no-restore --disable-build-servers -m:1 --verbosity quiet
dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj -c Release --no-build --no-restore --filter 'FullyQualifiedName~Architecture|FullyQualifiedName~BackendIntegratedHttpTests|FullyQualifiedName~SapSupplierImport|FullyQualifiedName~SapSyncProcessorRegistryTests|FullyQualifiedName~SapSyncSchedulerTests|FullyQualifiedName~SapSyncProfileApplicationTests|FullyQualifiedName~Features.Purchasing.PurchaseOrders|FullyQualifiedName~Features.BusinessPartners|FullyQualifiedName~ApiClientHttpErrorCompatibilityTests|FullyQualifiedName~SapSyncWinFormsContractTests' --logger 'trx;LogFileName=winforms-directed.trx' --results-directory artifacts/backend-solid-winforms-fix/directed --verbosity minimal
dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj -c Release --no-build --no-restore --filter 'Category!=SqlServerIntegration&FullyQualifiedName!~SapPurchaseOrderPilotIntegrationTests' --logger 'trx;LogFileName=winforms-final.trx' --results-directory artifacts/backend-solid-winforms-fix/final --verbosity minimal
```

Los gates solo afectan el proceso hijo. SQL externo y piloto SAP se excluyen expresamente. La suite conserva el ensayo HTTP completamente en memoria con dobles; no se contactan sistemas externos. No se instalan/restauran paquetes ni se cambia configuracion global.

## Comprobacion visual aislada

Se ejecuto el constructor sin dependencias del editor en un proceso STA separado, invisible/fuera de pantalla. No se ejecuto Program ni se inyectaron ViewModel, cliente, empresa o datos. Se dibujaron capturas con los assemblies reales de WinForms/DevExpress. El analizador de limites revisa los48 controles propios y2 botones heredados, agrupados por padre:

| Caso | Cliente renderizado | Controles | Fuera de limites | Solapamientos |
|---|---|---:|---:|---:|
| Normal | 1160x720 | 50 | 0 | 0 |
| Minimo | 1158x720 | 50 | 0 | 0 |
| Ampliado | 1360x820 | 50 | 0 | 0 |

Se inspeccionaron las tres capturas. La primera pasada detecto el cruce de informacion/tarjetas al reducir altura y se ajusto MinimumSize; la pasada final no presenta cruces. El probe temporal se compilo con Roslyn y referencias ya instaladas, sin nuevo proyecto/dependencias. Su compilacion directa emitio CS1701 por unificacion de referencias DevExpress.NET8 con.NET10, y ejecuto correctamente; esas advertencias no aparecen en la compilacion de solucion, que esta limpia.

Esta comprobacion acredita instanciacion, disposicion y render a96DPI, no un ciclo de abrir/guardar en Visual Studio Designer. Designer IDE, otros DPI, estados con datos/permisos y accion de guardar contra API real: Not validated en esta entrega. El codigo de esos comportamientos se conservo sin cambios.

## Preservacion y evidencia

Rama/HEAD conservados: codex/auxiliary-master-generator, e97124a1e2863f066c1fa55ea992c78179f01623. Snapshot de181 archivos previos modificados/no rastreados:177 huellas SHA-256 identicas y ninguno falta. Los4 archivos editados dentro del alcance son el Designer y tres registros documentales (ARCHITECTURE, BACKEND-SOLID-STATUS y BACKEND-SOLID-VALIDATION). No se alteran pruebas, backend, SQL, catalogos internos ni los demas cambios del usuario. Sin stash/reset/commit/cambio de rama.

Evidencia local ignorada por Git, en artifacts/backend-solid-winforms-fix/:

- baseline.json y before/: huellas y copias anteriores.
- red/winforms-red.trx y green/winforms-green.trx.
- build-final.log.
- directed/winforms-directed.trx.
- final/winforms-final.trx y final/summary.json.
- preview/layout.json y capturas [normal](../../artifacts/backend-solid-winforms-fix/preview/default.png), [minima](../../artifacts/backend-solid-winforms-fix/preview/minimum.png), [ampliada](../../artifacts/backend-solid-winforms-fix/preview/large.png).

Los contadores globales usan todos los nodos TRX, incluidos nombres visibles repetidos. El gate de contratos visuales queda resuelto sin exenciones. La liberacion de produccion sigue pendiente de SQL aislado del piloto transaccional, JWT/transporte y comprobaciones del entorno de destino descritas en BACKEND-SOLID-VALIDATION; no se confunden con la suite local ahora aprobada.
