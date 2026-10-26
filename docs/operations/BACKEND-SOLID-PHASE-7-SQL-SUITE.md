# Fase 7: suite SQL ejecutable — integración validada offline

Fecha: 2026-10-25. **Suite implementada; NO AUTORIZADA NI VALIDADA EN SQL. Fase7 abierta.**

## Alcance y descubrimiento

Continuación de tareas1–6, preflight y snapshots. Se reutilizan los contratos y escenarios existentes, sin cambiar producto, Domain, Application, API, WinForms, SQL desplegable, SAP o workers. Se aplican las skills de descubrimiento, pruebas, persistencia, ejecución del plan, verificación y optimización de contexto. Referencias: catálogo de21 grupos, fixture, escenarios nominales/fallos/confiabilidad/recuperación, lectores y definiciones024/028/042/065/124/126. No se instalan herramientas ni se preparan bases.

Se añaden `BusinessPartnerTransactionSqlTests`, entrada con guardas, cargador, runtime y pruebas offline. Dos ajustes necesarios de infraestructura previa: normalización de prefijos alfabéticos del RunId conforme a los handlers, y conservación de referencias a scopes independientes para exportar también sus trazas. Se guardan copias previas de los archivos afectados y hashes del resto.

## Contrato ejecutable

La clase tiene exactamente21 Facts: S01…S18, Equivalence, R01_Legacy y R01_Pilot, con Category=SqlServerIntegration y Scope=Phase7Isolated. Coincide con nombres y hash del catálogo vigente; no se rebajaron contadores ni criterios del evaluador. Colección sin paralelismo con otras colecciones; la concurrencia de S15 es explícita dentro del caso. El filtro restringido sigue siendo el definido en tarea6.

La discovery solo consulta los dos gates. Si cualquiera está apagado, los Facts se omiten; sus cuerpos vuelven a comprobarlos antes de crear el runtime. La regresión offline excluye la categoría SQL por completo. Una corrida con omitidos o sin resultados sigue siendo inválida para aceptación.

| Grupos | Integración |
|---|---|
| S01–S03 / Equivalence | Secuencia completa alta/edición/baja con legacy enA y piloto enB, mismo código/identificación, hijos y notas sintéticos, limpieza de Email, conservación de GlobalId, auditoría e intenciones. Compara equivalencia y GUID generados independientes. Cada grupo utiliza un código distinto. |
| S04 | Cuatro secuencias: no Master, sync deshabilitado, ambos deshabilitados y sync activo con SAP=None. |
| S05–S11 | Alta/edición/baja por separado para cada fallo; semilla confirmada para edición/baja, snapshots físicos antes/después, clasificación primaria/secundaria y cancelación con barrera. |
| S09 | Después de las escrituras y enqueue del CRUD solicitado, intenta un alta adicional en el mismo contexto con IdentificationTypeId negativo confirmado inexistente. Debe fallar dentro del procedimiento real por FK547 y revertir todas las escrituras previas. No modifica procedimientos. |
| S12 | Dos readers/destinos separados, mismo código/identificación y nombres diferentes; no reutiliza un reader deA paraB. |
| S13–S16 | Pooling real y SPID observado; repetición de alta; dos unidades con barrera de inserción; repetición de actualización con hijos reales y deltas de auditoría/intenciones observados. |
| S17–S18 | Cierre solo del propietario antes de commit, sin reintento, y contextos inválidos/anidados/solapados/cambio de empresa/reutilización. |
| R01 legacy/piloto | Dentro de una misma sesión: alta previa, lectura y enqueue; luego actualización directa por contrato transaccional de un id confirmado inexistente. Caracteriza el ROLLBACK interno y exige snapshot físico sin cambios. |

R01 no pasa por el handler de edición porque este detecta el id inexistente antes de invocar el SP. Reutiliza el contrato `UpdateBusinessPartnerData`, con identidad de auditoría explícita. Resultados caracterizados: SQL266/3902/3903/**3915** en la colección completa de errores, o retorno false seguido de InvalidOperationException del proveedor/runner al usar una transacción completada. Rechaza timeout, deadlock y lock-timeout aunque acompañen otro diagnóstico aceptado. Requiere haber alcanzado la llamada de actualización y demostrar rollback físico. Un éxito silencioso, deadline, excepción arbitraria o discrepancia de snapshot falla.

El wrapper126 usa INSERT…EXEC y el legado028 intenta ROLLBACK en el camino missing-id. SQL3915 corresponde a **rechazo de ese ROLLBACK interno**, no a confirmación de que se ejecutó correctamente; se registra `InsertExecRollbackRejected` y se exige que la sesión/runner igualmente revierta las escrituras previas. Referencia: [Microsoft Learn, error3915](https://learn.microsoft.com/en-us/sql/relational-databases/errors-events/database-engine-events-and-errors-3000-to-3999). La caracterización observada en SQL sigue pendiente y no se corrigió ningún procedimiento en esta tarea.

## Carga y autorización futura

**No habilitar gates con esta tarea.** La ejecución futura requiere autorización independiente y proceso hijo dedicado según tarea6. No hay inicializadores ni preparación automática. El runtime solo compone repositorios del piloto, barreras externas y SAP=None.

Entradas explícitas del proceso futuro:

- `NUANSYSTEM_PHASE7_MANIFEST_PATH`: archivo absoluto del manifiesto aprobado.
- `NUANSYSTEM_PHASE7_APPROVAL_PATH`: archivo absoluto de `Phase7SqlApproval`, obtenido de una fuente independiente confiable. Es coherencia de autorización, no una firma criptográfica ni prueba automática de aprobación humana.
- `NUANSYSTEM_PHASE7_SUITE_INPUTS_PATH`: archivo absoluto de `Phase7SqlSuiteInputs`.
- Las cuatro referencias de credenciales ya permitidas por el manifiesto: TENANT_A/B y OBSERVER_A/B. No hay descubrimiento, appsettings ni fallback a conexiones de producto.

Los JSON auxiliares rechazan propiedades desconocidas/duplicadas y campos obligatorios omitidos. Se limita su lectura a64KiB. Primero se validan manifiesto, aprobación e inputs; después se resuelven únicamente las referencias autorizadas. Se validan las cadenas completas y se construyen fábricas de conexiones cerradas. La apertura futura ocurre exclusivamente a través de las guardas de la fixture. SAP pilot gate activo impide cargar la suite.

`Phase7SqlSuiteInputs` contiene RunId, ExecutionAuthorizationId, ManifestSha256, CatalogSha256, IdentificationTypeIdA/B, MissingPartnerId, MissingIdentificationTypeId, FaultTriggerSha256A/B y BoundaryAttestation. Los ids de identificación deben existir, estar activos y ser iguales en ambas bases para que la equivalencia preserve FK de negocio. No se supone Id=1. MissingPartnerId positivo y MissingIdentificationTypeId negativo deben no existir.

Cada diccionario de triggers exige exactamente los tres nombres `TR_P7_{runId:N}_Audit`, `_Outbox` y `_SyncAudit`, con SHA256 efectivo según el protocolo UTF-16LE del preflight. Se verifican sus definiciones y estado habilitado desde cada observador antes de cualquier escritura del caso, además del esquema aprobado. La suite no los instala ni corrige hashes automáticamente. El inventario y la revisión independientes deben confirmar que esos hashes corresponden a los triggers aislados autorizados de tarea4.

BoundaryAttestation debe ser `SYNTHETIC_LAB_ONLY_SAP_NONE_WORKERS_OFF_RELAY_OFF`. **Es una declaración del operador**, no un detector de procesos remotos. La ausencia de escritores externos sigue siendo prerrequisito. No usar datos reales ni bases compartidas.

## Presupuesto, fallos y evidencias

Un lease por proceso congela RunId/configuración (incluido el texto de aprobación), impide repetir casos o cambiar de corrida y mantiene un presupuesto monotónico común. La fixture aprobada se crea una sola vez y conserva su reloj real, ventana, gates y lifetime original. Cada caso siguiente debe tener exactamente la misma configuración y reutiliza esa fixture, comprobando vigencia y presupuesto restante; no se exige de nuevo todo el presupuesto inicial. Cada runtime enlaza el presupuesto restante con los tokens de escenarios/fixture. Las fábricas construyen conexiones nuevas y aplican pooling según el caso activo; los observers siempre usan pooling=false. No se comparten conexiones/transacciones entre casos.

El primer fallo detiene casos posteriores, dispone la fixture y pone ambos gates SQL del proceso en0. Al completar los21 grupos también dispone la fixture y los pone en0. Esto afecta solo al proceso hijo: no modifica configuración global. Ante terminación abrupta, finalizar el hijo y revisar sesiones desde el procedimiento operativo autorizado; no existe una garantía de finally frente a caída del proceso. Las trazas exportadas por caso incluyen el historial acumulado del proceso y scopes independientes, diferenciados por ExecutionId.

La salida se crea bajo `artifacts/phase7-sql-executions/{runId:N}` del directorio de trabajo del proceso. `execution.claim` y los archivos usan CreateNew: no sobrescribe ni borra evidencia, ni continúa automáticamente una corrida anterior. Archiva manifiesto/aprobación/inputs sin valores de credenciales. Por grupo conserva snapshots físicos candidatos, resultado del caso, trazas de scopes raíz/independientes, contadores, deltas y diagnósticos sin mensajes/inner exceptions del proveedor ni connection strings. Ante fallo deja diagnóstico y no genera aprobación del caso. Un JSON de snapshot capturado antes de su comprobación es evidencia candidata, no una certificación por sí mismo.

Los checks del grupo solo se marcan Passed cuando termina satisfactoriamente toda su ruta y las aserciones de los escenarios. No se copian reportes sintéticos ni se marcan controles globales de corrida como aprobados. El TRX externo de VSTest sigue siendo obligatorio.

**Esta suite no produce automáticamente un paquete completo de cierre**: siguen pendientes congelación/revisión de fuente y binarios, inventarios efectivos, comprobador de hashes de archivos, verificación de ausencia de sesiones huérfanas, productor/revisión de los17 runchecks y de los14 artefactos, y las dos corridas reales. Los archivos por caso permiten alimentar ese paquete; no sustituyen controles independientes. El evaluador mantiene `GlobalPhase7Closed=false` y `EvidenceAuthenticityVerified=false`.

## Validación de esta entrega

Se ejecutan exclusivamente pruebas offline de guardas, catálogo/despacho, configuración, referencias sintéticas, conexiones creadas pero cerradas, presupuesto/repetición/detención, inyección de error y contrato R01; además de la regresión existente de escenarios/verificador/snapshots. La presencia de métodos SQL no constituye evidencia de ejecución SQL. Los resultados de compilación, TRX, preservación y revisión se guardan en `artifacts/backend-solid-phase7-sql-suite/`.

Pendiente validar en laboratorio autorizado: sintaxis y permisos efectivos de las consultas, definiciones reales de triggers/procedimientos, efecto de rollback interno, pooling/SPID, cancelación de SqlClient, estado de sesiones y evidencia integrada. Esta tarea no ejecuta SAP, no abre conexiones de datos y no cambia bases.

## Resultado final de esta entrega

- Solución Release sin restauración:0 errores y0 advertencias.
- **1622 pruebas offline aprobadas,0 fallidas y0 omitidas;58 nuevas**, frente a baseline1564. SQL/SAP excluidos y gates reales en0.
- Discovery con filtro restringido: exactamente21 Facts SQL reconocidos, **0 ejecutados**. No se usó ese resultado como aceptación SQL.
- Preservación:225 archivos previamente modificados/no registrados inventariados;221 mantienen sus hashes y4 recibieron únicamente ajustes necesarios de fixture/readers/pruebas, con copias previas en `before/`. Rama y HEAD conservados.
- Revisión independiente: dos hallazgos importantes corregidos. SQL3915 se reprodujo con `SqlException.Errors` sintéticos, pasó de fallo a aprobación y conserva el snapshot exacto obligatorio. La prueba del presupuesto compara el rechazo de una nueva admisión completa con la continuación válida de la fixture original:11min de ventana/10min de presupuesto inicial, avance2min y8min restantes, sin congelar el reloj operativo. Se cubre también el cambio de pooling sin reconsultar credenciales.
- Ninguna conexión SQL abierta, consulta ejecutada, credencial real resuelta, SAP invocado o base modificada. No se cerró fase7 ni se creó un reporte global de aceptación.

El siguiente trabajo pendiente es preparar/revisar el laboratorio y completar el empaquetado y controles globales de evidencia bajo autorización separada; después podrán plantearse las dos corridas reales. La existencia de esta suite no habilita por sí sola esos pasos.
