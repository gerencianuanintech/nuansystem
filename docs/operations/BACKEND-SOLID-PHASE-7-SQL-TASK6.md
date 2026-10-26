# Fase 7: preparación de ejecución controlada y cierre — tarea 6

Fecha: 2026-10-24. **Estado actual: NO LISTO PARA SQL. Fase7 abierta.** Se preparó el procedimiento y se implementó su evaluación offline; no se ejecutó SAP, no se abrieron conexiones de datos ni se instaló/preparó ninguna base.

## Alcance y prerrequisitos pendientes

Se reutilizan el manifiesto/guardas de tarea1, fixture de tarea2 y escenarios S01–S18 de tareas3–5. Los cambios de esta tarea quedan en infraestructura de pruebas y documentación. No hay un lanzador SQL, inicializadores, Fact SQL incompletos, resolución de secretos ni cambios de producto.

Antes de habilitar una ejecución deben quedar implementados y revisados:

1. Preflight real de `IPhase7SqlDestinationVerifier`: cotejo de servidor/versión, destinos, empresa, RunId/propósito, permisos/principales, compatibilidad/collation/RCSI y hashes efectivos contra el manifiesto aprobado, en la misma conexión usada por el caso.
2. Inventario y preparación autorizada del laboratorio: objetos efectivos024/028/042/065/124/126 y dependencias, SchemaHistory, FK/lookups sintéticos, marcadores de base, triggers propios revisados por texto/hash. Si falta algo, detener y preparar su instalación como tarea separada.
3. Lectores SQL de snapshots completos, filtrados por fixture, incluyendo filas eliminadas, hijas, auditoría, Outbox y SyncAudit, con parámetros, orden determinista, timeout/token y observador independiente. No reconstruir resultados desde el comando ni usar NOLOCK.
4. Clase ejecutable `BusinessPartnerTransactionSqlTests`, traits Category=SqlServerIntegration y Scope=Phase7Isolated, producción de evidencias y correspondencia revisada con el catálogo propuesto. Esa clase todavía no existe; los helpers implementados no equivalen a una suite SQL ejecutable.
5. Ensayo específico del ROLLBACK interno por registro no encontrado en las definiciones efectivas, tanto legacy como piloto: caracterizar resultado/excepción y demostrar que no confirma una escritura anterior parcial.
6. Revisión de autorizaciones separadas de preparación/ejecución, destino y ventana UTC, referencias a secretos y paquete de evidencias. El permiso de esta tarea es únicamente offline.

## Catálogo de aceptación propuesto

`Phase7SqlExecutionCatalog` fija21 grupos: S01–S18, Equivalence, R01.Legacy y R01.Pilot. **21 grupos no son21 operaciones SQL**: cada uno exige todos sus subcasos. Por ejemplo, S05–S11 incluyen alta/edición/baja; S04 incluye tres configuraciones de sync y SAP=None con sync activo; S18 incluye contextos retenidos tras éxito/fallo, cambio de empresa, anidamiento, solapamiento y reutilización.

S01–S03 requieren legacy y piloto. Equivalence compara contenido comercial, relaciones, auditoría y eventos entre fixtures equivalentes, sin igualar GUID/IDENTITY/fechas generadas. R01 se exige además de S09, no se considera implícito en sus errores genéricos.

El contrato propuesto usa21 métodos agregados, con nombres completos bajo `NuanSystem.Application.Tests.Features.BusinessPartners.BusinessPartnerTransactionSqlTests`: S01…S18, Equivalence, R01_Legacy y R01_Pilot. Cada método debe acreditar sus subchecks, marcándolos Passed solo tras las comprobaciones correspondientes. No copiar los valores Passed del generador sintético de pruebas como productor de evidencia real.

La correspondencia de nombres se debe confirmar al implementar la suite. Si se usan theories o se divide el catálogo en más métodos, revisar el catálogo, el evaluador, el número esperado y su SHA256 **antes** de aprobar la ejecución. No bajar el umbral para aceptar el TRX obtenido.

## Procedimiento operativo futuro

1. **Preparación offline:** preservar hashes/cambios actuales; compilar la solución y ejecutar regresión sin SQL con los tres gates en0. Resolver los prerrequisitos anteriores y preparar una lista exacta de pruebas con los gates desactivados. Verificar que el filtro selecciona únicamente el catálogo aprobado, sin omitidos inesperados ni casos adicionales.
2. **Congelar candidato y evidencias:** registrar hashes de fuente/binarios/catálogo; aprobar el manifiesto específico de la primera corrida, las cuatro credenciales por referencias y las huellas del laboratorio. No incluir valores de secretos en argumentos, logs, repo o paquetes. El verificador SQL y el comprobador de archivos deben validar las afirmaciones que luego recibe el evaluador.
3. **Ejecutar en un proceso hijo dedicado:** solo después de la autorización de ejecución, los dos gates SQL pueden valer1 dentro de ese proceso; SAP debe seguir en0. No modificar configuración global ni habilitar el gate para toda la suite. El filtro restringido propuesto es `Category=SqlServerIntegration&Scope=Phase7Isolated&FullyQualifiedName~BusinessPartnerTransactionSqlTests`. No se ejecutó aquí y todavía no es utilizable.
4. **Detener ante fallos:** error, skip, caso inconcluso, deadline, preflight discrepante, barrera externa invocada, sesión que no termina o snapshot incompleto impiden aprobar. Esperar terminación/rollback y observar desde otra conexión antes de clasificar el resultado. No reintentar automáticamente ni borrar filas/bases para ocultar efectos.
5. **Cerrar la primera corrida:** desactivar ambos gates en finally dentro del hijo y terminar el proceso. Registrar estado final0, ausencia de sesiones/transacciones de prueba huérfanas y conservación de bases/datos/evidencias. No iniciar relay, workers ni consumidores de Outbox.
6. **Segunda corrida limpia:** nuevo RunId, manifiesto y bases A/B, con fixtures nuevos. Conservar la primera corrida. Usar el mismo candidato de fuente/binarios; registrar y comparar SQL/versiones, compatibilidad/collation/RCSI, permisos, pooling y definiciones efectivas. Si el candidato cambia por una corrección, generar nuevamente las dos corridas de aceptación.
7. **Revisión independiente:** comprobar el contenido y hashes de archivos, resultados y límites; ejecutar el evaluador offline sobre los dos reportes completos. Una evaluación favorable solo permite someter el piloto a decisión de cierre. Documentar hallazgos y conservar compatibilidad si algún criterio falla.

El proceso de ejecución debe terminar aun si falla la prueba. No restaurar una configuración global previa que tuviera gates activos: los cambios deben existir únicamente en el hijo desechable. Pooling=false para marcadores/fallos/cierre de propietario; pooling=true solo en el ensayo explícito de reutilización sin marcadores residuales. No ejecutar SAP/SRI/HTTP/Master ni pruebas de caída del servidor/red bajo este permiso.

## Paquete por RunId

El modelo `Phase7SqlRunEvidence` recibe identidad de corrida/bases, hashes del manifiesto/candidato/catálogo, tiempos UTC, origen declarado, checks por grupo, inventario de artefactos y bytes originales del TRX. Los tiempos del reporte corresponden a start/finish del TRX, convertidos a UTC; la ventana autorizada se coteja además contra el manifiesto en el preflight y revisión.

Inventario requerido, con ruta relativa dentro del paquete y SHA256 no cero:

| Tipo | Contenido que se debe revisar |
|---|---|
| Manifest | Manifiesto aprobado saneado y referencias a autorización independiente. |
| SourceInventory / Binaries / CaseCatalog | Fuente exacta, binarios y catálogo del candidato; sin RunId incorporado en los inventarios estables del candidato. |
| Schema / Procedures | Definiciones efectivas, SchemaHistory, hashes de procedimientos/triggers y dependencias. |
| Trx | Archivo original íntegro con resultados y definiciones de pruebas. |
| Snapshots | Antes/después por caso, en tablas y fixtures pertinentes, incluida evidencia de rollback. |
| Sessions / Connections | Base/SPID/TRANCOUNT/XACT_STATE, pooling, higiene y contadores de escritura/observador separados. |
| Errors | Error primario/secundario y clasificación por caso; vacío explícito cuando no aplique, sin secretos ni datos reales. |
| Boundaries / Environment | Cero intentos externos, SAP/workers/relay deshabilitados, versión SQL y configuración efectiva. |
| SessionTermination | Sesiones de prueba terminadas, ausencia de transacciones huérfanas, gates finales0 y datos conservados. |

Los logs detallados deben ir en archivos controlados, manteniendo el TRX compacto: el evaluador admite hasta1MiB de TRX. El cargador futuro debe entregar bytes originales, no reserializar XML ni usar texto que pierda el BOM; el SHA se calcula sobre esos bytes. El evaluador no lee rutas ni archivos y no resuelve enlaces. La revisión/cargador debe confirmar que todas las rutas permanecen dentro del paquete y que todos los hashes coinciden con sus archivos reales. Los checks declarados no sustituyen esa comprobación.

## Evaluación offline y criterios de cierre

`Phase7SqlClosureEvaluator.Assess` no lee entorno, archivos o secretos, no inicia procesos y no abre conexiones. Comprueba:

- Dos corridas ordenadas, RunId/bases/manifiestos/TRX-run-id independientes y mismo candidato de fuente/binarios.
-21 grupos sin duplicados y todos los subchecks Passed. Los17 controles de corrida deben pasar, incluidos autorización/preflight/esquema, verificación de artefactos/snapshots/sesiones/conexiones/errores, ausencia de huérfanos/actividad externa, gates finales0 y datos conservados.
- Inventario completo de14 tipos de artefacto, rutas relativas sin escapes, hashes válidos y vínculos de manifiesto/candidato/catálogo/TRX. Verifica directamente el hash del TRX aportado; la autenticidad y el contenido de otros archivos siguen siendo responsabilidad del verificador/revisor.
- TRX con namespace y estructura esperados, contadores completos, exactamente los21 nombres/definiciones aprobados, todos Passed, ids de prueba/ejecución únicos y enlazados, tiempos consistentes y ausencia de errores. Cero resultados, omitidos, inconclusos, contadores maquillados o pruebas de otra clase no sirven como éxito. Se prohíbe DTD y resolución XML externa.
- Ningún intento de barrera externa, SAP o workers ejecutados. Un reporte etiquetado Offline nunca satisface el origen SQL requerido.

**Límite explícito:** el modelo contiene afirmaciones del futuro harness. Un objeto fabricado puede tener checks Passed y origen SQL; el evaluador comprueba coherencia, no demuestra que SQL se ejecutó ni autentica firmas/personas/archivos. `ReadyForIndependentPilotReview=true` solo indica que el reporte es candidato para revisión. `EvidenceAuthenticityVerified` y `GlobalPhase7Closed` son siempre false. No hay función que cierre fases ni que autorice ejecución.

Para cerrar el **gate SQL del piloto**, la revisión debe aceptar las dos corridas reales, todos los subcasos, atomicidad/aislamiento, integridad negocio-auditoría-Outbox, identidades/repetición, ausencia de sesiones huérfanas y límites del ensayo. Un skip o evidencia requerida inconclusa deja el gate abierto.

Para cerrar la **fase7 global**, se requiere además inventario actualizado de consumidores de contratos legacy, decisión explícita de qué contratos se conservan, plan/revisión por vertical migrado y actualización sustentada del ADR/estado consolidado. El éxito del piloto no autoriza retirar overloads usados, desplegar producción, ejecutar SAP, migrar otros módulos ni borrar bases. Tampoco prueba entrega de eventos, exactamente-una-vez, recuperación de desastre o resultado de commit incierto tras pérdida de respuesta: ese último caso exige conciliación por identidad/evento antes de decidir reintentar.

## Evidencia obtenida en esta tarea

**Validado:** Release completo sin restauración,0 errores y0 advertencias; **1485 pruebas offline aprobadas,0 fallidas,0 omitidas**, baseline1439 +46. Pruebas con reportes/TRX sintéticos: cobertura incompleta, checks omitidos, corridas reutilizadas, hashes/rutas inconsistentes, resultados/contadores/definiciones manipulados, fechas, BOM, XML inválido/DTD/tamaño y actividad externa. Los casos positivos fabricados solo validan el protocolo del evaluador.

Se fijaron a0 NUANSYSTEM_RUN_SQL_INTEGRATION_TESTS, NUANSYSTEM_RUN_PHASE7_SQL_TESTS y NUAN_RUN_SAP_PO_PILOT; se excluyeron las suites SQL y SAP piloto. Revisión independiente sin hallazgos materiales. Artefactos locales en `artifacts/backend-solid-phase7-sql-task6/`: baseline de hashes, plan previo, fallos iniciales esperados, logs de compilación/pruebas y TRX final.

**No ejecutado/no validado:** ninguna corrida SQL, SAP, preparación de bases, instalación de triggers, lectura de secretos, autenticidad de un paquete SQL real ni cierre del piloto/fase global. Reportes reales disponibles para el cierre:0. El estado permanece NO LISTO PARA SQL hasta completar los prerrequisitos.
