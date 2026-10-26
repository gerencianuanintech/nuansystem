# Fase 7: empresa, concurrencia, repetición y recuperación — tarea 5

Fecha: 2026-10-24. Implementación y validación offline. No se ejecutó SAP ni se abrieron conexiones de datos. No se modificaron bases, scripts de producto, dependencias ni configuración global.

## Descubrimiento y decisiones

Se conservan las decisiones de tareas1–4: fixture con manifiesto/aprobación, preflight obligatorio, probe privado, handlers/UoW/runner/writer reales y observador independiente. Los adaptadores de preflight y snapshots SQL siguen pendientes. Se inspeccionaron el contrato del piloto, CompanyContext, pruebas transaccionales cercanas y procedimientos024/028, además de los catálogos de transacción y gates de revisión aplicables.

El cambio afecta únicamente tests y documentación. La simulación de persistencia, unicidad y pool es local al harness de pruebas, no un servicio nuevo del backend. No se alteran Domain, Application, Persistence, API, WinForms, integraciones ni workers.

Decisiones específicas: scopes independientes sobre el mismo destino para S15; ninguna carrera dentro de un callback/handle compartido; barreras antes del INSERT, no pausas arbitrarias; cierre exclusivamente del propietario para S17; controles de empresa accesibles solo durante el callback de prueba. Se preserva la semántica actual de eventos por operación, sin introducir idempotencia de edición o retries.

## Escenarios implementados

| Caso | Implementación | Evidencia local |
|---|---|---|
| S12 | Dos empresas con el mismo Code/identificación y nombres distintos; alta, lectura y eventos vinculados a su empresa/GlobalId. | Datos y payloads separados; los ids locales pueden coincidir. Se rechaza CompanyId ajeno en el evento. |
| S13 | Nuevos scopes, Pooling=true, lectura de SPID/base/id/TRANCOUNT/XACT_STATE y los cuatro marcadores de fallo antes del callback. | Reutilización de sesión física simulada y rechazo de estado residual. Tener pooling activado no sustituye observar SPID reutilizado. |
| S14 | Mismo comando de alta dos veces. Segundo Result contiene BusinessPartnerCodeAlreadyExists. | Snapshot exacto sin cambios tras el duplicado; una identidad/Created/SyncAudit. La unidad permite una lectura posterior. |
| S15 | Dos unidades/scopes independientes pasan validaciones de duplicado y esperan una barrera justo antes de CreateAsync. | Un ganador y un conflicto esperado; una fila/evento/auditoría INSERT. Falta de unicidad, error inesperado o deadline hacen fallar el caso. |
| S16 | Mismo comando de edición aplicado dos veces después del alta. | Dos eventos Updated con ids distintos, misma identidad y valores comerciales; evidencia de deltas Outbox/SyncAudit/auditoría y conservación del historial previo. |
| S17 | Cierre de la conexión propietaria tras AfterEnqueue y antes del commit; excepción identificable inmediata. | Snapshot de rollback idéntico; error primario/secundario conservado; un callback y una conexión. Se rechaza un adaptador que repite el callback. |
| S18 | Contexto retenido después de éxito/fallo, cambio de empresa durante callback, anidamiento y solapamiento de ExecuteAsync. | Rechazo sin consulta de negocio ni conexión adicional para el intento inválido; scope restaurado y unidad reutilizable. |

## Fixture y observación

`CreateIndependentScope` conserva el destino aprobado, factories, verificador y presupuesto de la fixture; crea otro probe, contexto y control de actividad. No comparte sesión/transacción activa. Las factories reales deben devolver una conexión cerrada nueva por petición, aunque el pool reutilice una sesión física.

`CreateReliabilityScope` compone los repositorios/writer reales a través del mismo camino de preflight. Comprueba el pooling solicitado en SqlConnection, consulta higiene de sesión dentro de la transacción exterior y guarda las muestras por scope. Se exige base/id correctos, SPID positivo, TRANCOUNT=1, XACT_STATE=1 y ausencia de P7_RunId/P7_CompanyId/P7_CaseCode/P7_FaultPoint. Los contadores acumulados de escritura se separan del observador.

S13 exige evidencia de SPID repetido entre scopes limpios del mismo destino. Si el pool no reutiliza una sesión en el ensayo, se informa PHYSICAL_SESSION_REUSE_NOT_OBSERVED y no se declara ese camino probado. La consulta de higiene SQL está implementada, pero no se ejecutó aquí.

Los wrappers OnFixture de S12–S18 usan el observador independiente, timeout/token del manifiesto y lectores SQL proporcionados explícitamente. Esos lectores deben devolver snapshots completos del caso, separados de los resultados de los comandos, en orden determinista y sin NOLOCK. Los casos de alta/repetición/concurrencia exigen un fixture inicialmente vacío; preparar códigos únicos por RunId/caso y resolver FK mediante los lookups autorizados, sin asumir el id1 usado solo en memoria.

## Concurrencia y límites

S15 usa dos scopes independientes del mismo tenant. Cada uno tiene su propio runner/probe y el callback hace operaciones secuenciales. La barrera compartida tiene exactamente dos llegadas y no se reutiliza. Se ejecuta después de los checks de Application, antes de los INSERT reales del futuro ensayo SQL.

Se esperan ambos resultados antes del snapshot. El plazo global del caso es de1–30s; ante cancelación se drenan las operaciones hasta2s adicionales. Si no terminan, se informa CONCURRENT_OPERATIONS_NOT_STOPPED y no se observa ni aprueba el caso. El vencimiento y la cancelación del usuario nunca se aceptan como un conflicto exitoso.

Se devuelve Result o excepción de cada intento cuando el caso supera sus comprobaciones. El clasificador SQL admite2601/2627 y1205; no acepta cualquier excepción, timeout o cancelación como duplicado. Los errores inesperados quedan referenciados en Phase7Sql.ConcurrentFailure. No hay reintentos automáticos, ni siquiera al reconocer un deadlock. Se exige exactamente un ganador para este fixture vacío; cero ganadores falla.

## Repetición e historial

S14 compara el snapshot entero sin normalizar ids ni fechas: el segundo alta debe dejarlo intacto.

S16 no exige exactamente-una-vez. Ambas ediciones válidas deben mantener la identidad del socio y agregar una intención/SyncAudit por ejecución exitosa, como hacen los handlers actuales. El informe devuelve snapshots anteriores/posteriores a la repetición y sus deltas, sin inventar filas de auditoría cuando no hubo un campo auditado distinto. En memoria se observa delta Outbox=1, SyncAudit=1 y auditoría=0 para la segunda edición.

Los procedimientos024/028 marcan hijas anteriores como eliminadas e insertan otras durante la edición; sus ids físicos pueden cambiar. Por ello, la comparación comercial de S16 omite únicamente UpdatedAt del padre e Id de cada hija. Exige ids hijos positivos y BusinessPartnerId correcto; conserva Id/GlobalId del padre, CreatedAt, valores, flags, FK comerciales y contenido de las relaciones. Las pruebas rechazan tanto un valor hijo diferente como un padre ajeno. No se aplica esta normalización a snapshots de rollback, que siguen siendo exactos.

Se comprueba que los eventos, SyncAudit y auditorías previos permanecen como prefijo del historial, sin reescribirlos. Las nuevas filas auditadas se caracterizan por su delta real y deben pertenecer al socio.

## Recuperación y alcance de S17/S18

S17 exige Pooling=false. El hook mide AfterEnqueue, cierra exclusivamente su conexión y lanza Phase7SqlOwnerClosedException inmediatamente: no emite un comando posterior que pudiera reabrir implícitamente el handle cerrado. El runner conserva esa excepción y cualquier error secundario de rollback en su Data. El helper cuenta invocaciones del callback; el wrapper de fixture exige una sola petición de conexión y un solo AfterEnqueue. El observador usa otra conexión y verifica el estado exacto posterior.

Esto cubre cierre antes de commit, no pérdida de respuesta después de un commit durable. Ante resultado de commit incierto, conciliar primero por identidad de negocio/GlobalId/EventId y auditorías en el tenant correcto antes de decidir un retry. No se añade una función automática de reenvío ni se simulan caídas de red/servidor.

S18 invalida el contexto solo mientras su operación está activa; no permite preparar una empresa distinta antes de ejecutar una unidad contra el destino aprobado. Restaura la referencia original tras el ensayo. La prueba de solapamiento usa barreras y drena el callback activo incluso si se cancela el usuario. No lanza comandos concurrentes dentro de una transacción.

## Verificación y pendientes

**Validado:** compilación Release completa sin restauración,0 errores y0 advertencias; **1439 pruebas aprobadas,0 fallidas,0 omitidas**, baseline1410 +29 nuevas. Se ejecutaron escenarios con componentes de aplicación reales y persistencia/pool simulados. Incluye negativos de datos cruzados, sesiones sucias, reutilización no observada, unicidad ausente, error/deadlock, deadline, pérdida de eventos, callback repetido y sustitución legítima/ilegítima de filas hijas. Revisión independiente sin hallazgos materiales.

Gates SQL general/fase7 y SAP piloto fijados a0; suites Category=SqlServerIntegration y SapPurchaseOrderPilotIntegrationTests excluidas. Evidencias locales en `artifacts/backend-solid-phase7-sql-task5/`: hashes previos, copias de archivos editados, fallos iniciales esperados, regresión de hijas, logs y TRX final.

**No validado:** pooling/reutilización física real, unicidad/locks/deadlocks del servidor, limpieza real de sesiones, rollback del cierre y snapshots durables SQL. No se ejecutó ninguno de los wrappers OnFixture contra una base. Siguen pendientes preflight, lectores SQL, preparación del laboratorio y ejecución autorizada de la matriz. La tarea5 queda implementada offline; la fase7 no se cierra con estos resultados.
