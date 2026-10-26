# Fase 7: empaquetado de evidencias y controles globales

Fecha: 2026-10-25. **Implementación y validación exclusivamente offline. Fase7 abierta; ninguna corrida SQL real autorizada con esta entrega.**

## Alcance y decisiones

Se reutilizan el catálogo de21 grupos, sus17 controles globales/14 artefactos, manifiesto/aprobación, verificador, lectores y suite ejecutable de las tareas anteriores. Se amplía infraestructura de pruebas y documentación; no se cambia producto, SQL desplegable, SAP, workers ni bases. Skills aplicadas: descubrimiento, pruebas, persistencia, ejecución de planes, verificación y optimización de contexto. Las lecturas rectoras y catálogos se reutilizan de la consolidación y se contrastan con los contratos actuales.

La decisión central es separar **integridad del paquete**, **coherencia de resultados** y **autenticidad**. Tener archivos o checks declarados Passed no certifica una ejecución real. El paquete calcula huellas sobre bytes reales y verifica vínculos; el evaluador exige los casos/TRX y controles completos. La autenticidad y el cierre global siguen requiriendo revisión independiente: `EvidenceAuthenticityVerified` y `GlobalPhase7Closed` permanecen false, incluso en ejemplos sintéticos completos.

## Candidato congelado y nuevo contrato de inputs

`Phase7SqlCandidate.Freeze` recibe raíz absoluta y listas explícitas aprobadas de fuentes/binarios. No recorre automáticamente el repositorio ni incluye configuración/secretos. Acepta fuentes .cs/.csproj/.props/.targets/.sql/.md y binarios .dll/.exe/.pdb; rechaza appsettings, .env y archivos de claves. Los índices ordenados contienen ruta relativa, longitud y SHA256 de bytes originales, sin RunId, fecha ni rutas absolutas. Por eso su hash es comparable entre dos corridas nuevas. El descriptor local sí conserva raíz y fecha UTC para comprobar los archivos; esa raíz local no se incorpora al índice portable.

El descriptor se genera **después de compilar y antes del baseline offline**, y la regresión posterior debe ejecutarse sin rebuild/restauración. Cada archivo se vuelve a verificar antes de SQL y al empaquetar. Se debe revisar la cobertura de las listas: una huella demuestra los archivos seleccionados, no que el operador haya seleccionado toda la dependencia del candidato. Incluir el assembly ejecutor `NuanSystem.Application.Tests.dll` es obligatorio y se contrasta con el archivo realmente cargado antes de comenzar la suite.

`Phase7SqlSuiteInputs` añade tres campos obligatorios: `SourceInventorySha256`, `BinaryInventorySha256` y `OfflineBaselineSha256`. El archivo aprobado de inputs debe contenerlos; no se rellenan con hashes observados para corregir discrepancias. El catálogo y sus21 nombres no cambian.

Dos rutas explícitas nuevas para el futuro proceso SQL:

- `NUANSYSTEM_PHASE7_CANDIDATE_PATH`: descriptor JSON local producido por Freeze, UTF-8 sin BOM y hasta1MiB.
- `NUANSYSTEM_PHASE7_OFFLINE_BASELINE_PATH`: TRX offline original, hasta16MiB, cuyo SHA está aprobado en los inputs.

Primero se validan manifiesto, inputs, candidato, binarios y baseline; después se resuelven credenciales y se compone la fixture. No se habilitan gates ni se abren conexiones por generar/verificar un paquete.

## Capturas de la suite

`Phase7SqlSuiteEvidenceCapture` se integra al cargador y runtime:

- Archiva índices del candidato y baseline original, y un recibo de inicio con RunId/PID/UTC y huellas de fuente, binarios, catálogo, baseline y assembly ejecutor.
- Conserva las cuatro primeras observaciones verificadas A/B × escritor/observador, incluyendo inventario efectivo y definiciones de procedimientos. Las verificaciones posteriores siguen fallando ante cambios; no se reemplazan los recibos originales.
- Cada grupo conserva sus observaciones de lookups y tres triggers revisados paraA/B. Las definiciones se comparan contra los hashes aprobados antes de escribir.
- Cada scope recibe un EvidenceScopeId estable; cada grupo archiva `case-timing.json` con secuencia real, tiempos y árbol de trazas anterior, además del árbol final en `sessions.json`. Se valida continuidad antes/después entre grupos según esa secuencia, no según el orden del catálogo, y se exige cobertura de escrituras, estados, scopes independientes y observers que concuerde con los snapshots/preflight. Un paquete anterior sin estos recibos no se completa con datos inventados.
- Antes de completar el último grupo lee y guarda snapshots físicos finales deA/B desde observers autorizados. Nunca se realizan estas lecturas con gates apagados.
- Al terminar o fallar registra estado de gates, grupos terminados, contadores y límites. Un intento de barrera externa queda contado aunque alguien capture su excepción. Una falla posterior deja un latch persistente; no sobrescribe el primer cierre.

Cada conexión del escritor/observer recibe un ApplicationName exclusivo del RunId, rol y propósito. Pooling continúa según el caso; el monitor puede identificar las cuatro familias sin confundir SPID reciclados. La captura y su cierre no crean clientes SAP ni workers. Los fallos al escribir evidencia no convierten un caso en éxito ni sustituyen el error primario; una evidencia faltante/inconsistente bloquea los controles posteriores.

## Monitor independiente después del proceso

`Phase7SqlMonitorReader` recibe una `SqlConnection` **ya abierta** sobre master y una `Phase7SqlMonitorApproval` independiente con RunId, principal, referencia y ventana UTC. No tiene fábrica, resolución de secretos, apertura, kill, cleanup ni preparación de bases. Rechaza proveedores falsos, transacciones ambientales, sysadmin, impersonación y principales de aplicación/observers. Coteja servidor/versión, login original y permiso de visibilidad.

El monitor está preparado en código, pero **no se ejecutó aquí**. En una futura tarea deberá autorizarse aparte su sesión de solo lectura y sus permisos: visibilidad de DMV del servidor y SELECT sobre las16 tablas de las dos bases aisladas. No se amplían los permisos del escritor/observer. Referencias oficiales: [sys.dm_exec_sessions](https://learn.microsoft.com/en-us/sql/relational-databases/system-dynamic-management-views/sys-dm-exec-sessions-transact-sql), [sys.dm_tran_database_transactions](https://learn.microsoft.com/en-us/sql/relational-databases/system-dynamic-management-objects/sys-dm-tran-database-transactions-transact-sql). Las exigencias de visibilidad varían por versión; la consulta y permisos reales están pendientes de ensayo autorizado.

Desde el proceso padre, después de comprobar que el hijo terminó, el monitor consulta sesiones con los ApplicationNames exactos, transacciones de usuario deA/B, identidad/marcadores de ambas bases y snapshots completos posteriores. Conservación de datos exige **hash exacto del snapshot final y contadores por tabla**, no solo igual número de filas. No cambia de base con USE ni modifica estado. Los nombres usados para lecturas entre bases se derivan exclusivamente del RunId y roles cerrados; los valores se parametrizan.

`Witness` observa un objeto Process que el padre ya posee; no lo crea, espera ni termina. La declaración del inspector sobre SyncWorker, MasterBranchSyncWorker, SriWorker y Relay debe cubrir todo el intervalo de la corrida. Esta declaración no es un detector remoto de servicios ni una firma. La observación SQL debe ser posterior al fin del TRX y a la observación de salida del hijo; sin proceso terminado, exit0, monitor autorizado y cero sesiones/transacciones, NoOrphanSessions no pasa.

## Los17 controles

| Control | Comprobación |
|---|---|
| ApprovedManifest | Aprobación independiente, hash exacto del manifiesto, RunId y tiempos. |
| DestinationPreflight | Cuatro observaciones A/B escritor/observer, identidad, permisos, destino y sesión. |
| EffectiveSchemaAndProcedures | Huellas recalculadas de inventario/definiciones efectivas contra el manifiesto. |
| FaultTriggerDefinitions / LookupSeeds |42 recibos, uno por grupo/rol, triggers habilitados y huellas correctas; lookup explícito e ids negativos/ausentes comprobados. |
| OfflineBaseline | TRX original íntegro, resultados/counters exitosos, sin SQL/SAP piloto según nombres y definiciones conocidas, vínculos de ids y orden candidato→baseline→inicio. Admite ejecuciones repetidas de un mismo testId cuando VSTest emite una sola definición, preservando executionId únicos. El comando operativo debe usar el filtro offline documentado. |
| ArtifactHashes | Lectura de bytes, huellas, vínculos de manifiesto/índices/catálogo/TRX, copia y nueva verificación del resultado/ZIP. |
| DurableSnapshots | Evidencias de los21 grupos y finalesA/B con las16 tablas completas, sin claves JSON duplicadas. |
| SessionTraces / ConnectionCounts | Base/id/SPID/TRANCOUNT/XACT_STATE, una entrada por ejecución, contadores acordes, scopes independientes y ausencia de reescritura del historial acumulado. |
| ErrorDiagnostics | Clasificaciones y barreras reales por archivo, rollback3915 caracterizado, cierre del propietario, carrera y repetición; no pasa por mera presencia de archivos. |
| NoOrphanSessions | Aprobación del monitor, salida del proceso y observación posterior con cero sesiones/transacciones. |
| NoExternalCalls / SapDisabled | Contadores/límites del cierre, no una inferencia de que no hay logs. |
| WorkersAndRelayDisabled | Cero workers de esta composición y testimonio independiente que cubre la corrida. |
| GatesDisabledAtEnd | Ambos gates apagados, RunId/PID concordantes y los21 grupos terminados sin duplicados. |
| DataPreserved | Ambas bases/marcadores conservados y hashes exactos de snapshots finales concordantes con el monitor. |

Falta de evidencia produce NotRun; contradicción produce Failed. Ambas bloquean revisión favorable. Las afirmaciones del runtime y del inspector siguen siendo datos que se deben autenticar por medios independientes: un protocolo coherente puede fabricarse, como muestran expresamente las pruebas positivas.

En S15 se conservan los códigos del resultado de negocio y la colección completa de números SQL. Se exige exactamente un ganador limpio; el perdedor debe ser un conflicto de código/identificación permitido o un SqlException2601/2627/1205 sin timeout. Un objeto vacío, rechazo sin código, error arbitrario o timeout no pasa. Las observaciones de estado exigidas por S12–S18 y los scopes nuevos de S13/S15/S17/S18 no pueden omitirse aunque los contadores totales parezcan válidos.

## Construcción y lectura del paquete

`Phase7SqlEvidencePackager.Build` recibe raíz de corrida, destino nuevo, ruta relativa del TRX SQL, aprobación confiable, candidato congelado y observaciones/testigo/aprobación independiente del monitor. Es una API interna del harness; no es un endpoint ni un inicializador. Puede empaquetar una corrida incompleta para diagnóstico, con controles pendientes/fallidos y revisión bloqueada. No genera pruebas SQL ni resultados ficticios para completar faltantes.

Produce carpeta, ZIP, `run-report.json`, índice de sellado y los14 artefactos del catálogo: Manifest, SourceInventory, Binaries, CaseCatalog, Schema, Procedures, Trx, Snapshots, Sessions, Connections, Errors, Boundaries, Environment y SessionTermination. Conserva archivos individuales y fuentes/binarios seleccionados. Los índices de bundles enlazan rutas, longitud y SHA; se verifican contra sus archivos al leer el ZIP. Los bytes originales del TRX, incluido BOM, se preservan. El catálogo usa exactamente el texto canónico cuyo SHA ya exige el evaluador.

`VerifyZip` verifica integridad local sin extraer. `ArchiveSha256` se devuelve como huella fija de salida y puede conservarse en un canal confiable. `ReadIntegrityVerifiedReport(zip, expectedSha)` exige esa huella antes de importar un reporte para `Phase7SqlClosureEvaluator.Assess([primero, segundo])`. Una huella o sello enviados únicamente junto al archivo no acreditan por sí mismos quién lo produjo. No hay cierre automático de fase ni autenticación de evidencia.

Se rechazan rutas absolutas/traversal, colisiones de mayúsculas, reparse points/junctions, enlaces ZIP, entradas duplicadas, destinos existentes, sobrescrituras y bundles incoherentes. Se limita archivo a32MiB, SQL TRX a1MiB, baseline a16MiB y paquete a128MiB; la metadata del directorio central ZIP se valida antes de materializar sus entradas, con hasta4096 archivos. No se extraen ZIP, borran parciales ni se modifica la evidencia fuente. Los diagnósticos siguen un contrato saneado; las listas de fuentes y los datos del laboratorio deben revisarse como sintéticos y sin secretos antes de exportar.

## Validación y límites de esta entrega

Las pruebas usan archivos locales, JSON/TRX **fabricados**, conexiones simuladas o SqlConnection creadas pero cerradas. Los positivos fabricados demuestran integridad/coherencia, nunca ejecución real, permisos efectivos o atomicidad SQL. No se accedió a SAP, no se abrieron conexiones, no se resolvieron credenciales reales y no se modificaron bases.

Se preservan copias de los archivos preexistentes modificados, baseline de hashes, logs y TRX en `artifacts/backend-solid-phase7-sql-evidence/`. Quedan pendientes la revisión/autorización del laboratorio y del monitor, el ensayo real de las consultas y permisos, las dos corridas aisladas y la revisión independiente de sus paquetes auténticos. La fase7 permanece abierta.

## Resultado final offline

- Solución Release sin restauración:0 errores y0 advertencias.
- **1685 pruebas aprobadas,0 fallidas y0 omitidas;63 nuevas**, baseline1622. Todas las suites SQL y SAP piloto se excluyeron y sus gates permanecieron en0.
- 232 rutas preexistentes inventariadas:225 mantienen sus hashes y7 recibieron exclusivamente ajustes de integración, con copias previas verificadas. Rama y HEAD conservados; no se revirtieron cambios del usuario.
- Revisión independiente con dos hallazgos importantes corregidos en un único pase: las regresiones reprodujeron inicialmente la aceptación de observers ausentes y de un perdedor con timeout. Después aprobaron, junto a pruebas de pérdida de estados/scopes/historial, ejecución en orden inverso al catálogo y clasificación de los conflictos permitidos.
- Se verificaron ZIP/hash/traversal/duplicados/enlaces, pin de huella de archivo, fuentes alteradas, respuestas incompletas, errores con contenido inesperado, retención por hash exacto, cancelación y baseline con testId repetidos/relabelling de resultados.
- Las pruebas positivas de paquetes y de dos corridas son **fabricaciones de protocolo**; ninguna es una corrida SQL real. Conexiones SQL abiertas0, Facts SQL ejecutados0, credenciales reales resueltas0, SAP ejecutado0. Autenticidad y cierre globalfalse.

## Capturas persistidas del monitor

`BuildFromMonitorCapture` permite empaquetar directamente una captura del ejecutor después de verificar su índice mediante una huella externa aprobada, los cinco documentos y sus seis entradas de corrida. Conserva esos bytes en el ZIP y verifica también su coherencia al leerlo. La API Build anterior continúa disponible. Ver [integración de capturas](BACKEND-SOLID-PHASE-7-SQL-MONITOR-PACKAGING.md). Las capturas offline mantienen su origen y sus bloqueos; los negativos no se convierten en resultados aprobados.
