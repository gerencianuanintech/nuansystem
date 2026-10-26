# Fase 7: caminos nominales y equivalencia — tarea 3

Fecha: 2026-10-24. Implementación y validación offline. No se ejecutó SAP, no se abrieron conexiones de datos ni se modificaron bases.

## Implementación

`Phase7SqlNominalScenarios` ejecuta Create/Update/DeleteBusinessPartnerCommandHandler y sus validadores reales. S01 comprueba alta e identidad; S02 edita datos, vacía un campo y modifica una dirección; S03 verifica baja lógica y conservación de Id/GlobalId. S04 recorre las tres operaciones sin intención local cuando IsMaster=false o SyncEnabled=false. Con ambos true se esperan eventos aunque SAP=None y el modo sea Standalone.

Cada handler completa su propia unidad; no hay transacción de test exterior. Después de su retorno se solicita un snapshot separado. La evidencia exige campos comerciales solicitados, relaciones asociadas al socio, auditoría BusinessPartners del usuario de la operación, eventos BusinessPartner y un SyncAudit Created/Pending asociado a cada EventId. UPDATE puede auditar varios campos: no se presupone una sola fila por edición. Se comprueban conteos acumulados de eventos1/2/3 o0 y ausencia de eventos parciales sin su SyncAudit.

La referencia `Phase7LegacyBusinessPartnerUnitOfWork` adapta los handlers actuales directamente a los overloads ADO de repositorio/writer y a ITransactionRunner. No usa BusinessPartnerUnitOfWork. Es una referencia del **contrato transaccional legacy**, no una reproducción histórica independiente de todos los mapeos de handlers. Ambos caminos comparten deliberadamente los handlers y el escritor reales: las comprobaciones contra los comandos aportan una expectativa externa a esa equivalencia.

La fixture expone CreateLegacyReference y permite configurar IsMaster/SyncEnabled por composición. Ambas variantes pasan por el mismo VerifiedRunner, guardas, preflight obligatorio y probe privado. RunOnFixtureAsync conecta los escenarios al observador independiente de la tarea2. Un decorador dentro del callback captura AfterCreate/AfterUpdate/AfterDelete/AfterEnqueue, además de Entry/BeforeCommit automáticos, sin exponer ADO a Application.

## Comparación y payloads

La comparación incluye DTO comercial completo, relaciones, auditoría, eventos y SyncAudit. Normaliza únicamente identidades generadas de filas/eventos, CompanyId de cada laboratorio y fechas CreatedAt/UpdatedAt. Conserva los identificadores de catálogos comerciales; las dos bases deben tener lookups equivalentes con esos ids o aportar una normalización semántica explícita antes de comparar. No descarta diferencias de cuentas, identificación, importes, flags ni contenido comercial. Los snapshots deben usar orden determinista para filas/hijas/auditoría y eventos en orden de operación.

Antes de normalizar se exige Id/GlobalId constante dentro de cada secuencia. En la ejecución se comprueba además la vinculación de cada evento/payload y su identidad original. Los payloads se inspeccionan recursivamente para detectar campos sensibles e identificadores SAP excluidos. No se prohíben ExternalSystem/ExternalCode: el factory actual los incluye expresamente. El evento Deleted mantiene la semántica vigente del handler, que entrega al writer el DTO leído antes de la baja; no se fuerza artificialmente un payload posterior a ella.

`Phase7NominalCommands` usa constructores explícitos. Exige código, tipo de identificación resuelto y número sintético proporcionados por el llamador. El id1 usado en el doble offline pertenece solo a su catálogo simulado; no es una semilla SQL supuesta. Los datos finales del laboratorio deben usar el ledger/RunId y lookups autorizados de la tarea2.

## Qué está validado

El doble `Phase7NominalOfflineHarness` simula repositorio, auditoría e inserción local de Outbox/SyncAudit en memoria; usa SqlTransactionRunner con conexiones simuladas, ambos adaptadores, handlers, validadores y escritor/payload factory reales. Entrega snapshots separados y comprueba que se solicitan tras el commit simulado. No demuestra durabilidad ni comportamiento de procedimientos SQL.

17 pruebas nuevas: equivalencia nominal con ids/GUID independientes, S04 en tres configuraciones, rechazo de diferencias comerciales/hijas/auditoría/eventos/SyncAudit e identidad, fechas independientes, inspección de campos sensibles anidados y rechazo de un doble defectuoso que retiene un email solicitado como null. También se comprueba el orden de checkpoints previo al commit y la dirección editada.

Compilación Release de NuanSystem.sln sin restaurar: **0 errores, 0 advertencias**. Regresión offline: **1378 aprobadas, 0 fallidas, 0 omitidas** (baseline1361 +17). Gates de SQL general, fase7 y piloto SAP fijados a0; suites SQL y SAP piloto excluidas. Evidencia local en `artifacts/backend-solid-phase7-sql-task3/`, incluido TRX final, logs y baseline de hashes.

## Pendiente antes del cierre SQL

Continúan pendientes el adaptador de preflight de la tarea2, inventario/huellas/semillas del laboratorio y un lector SQL de snapshots que consulte las tablas reales mediante el observador, con filtro de fixture, parámetros, orden determinista y timeout/token. Ese lector debe recuperar también filas de baja lógica, todas las tablas hijas y auditorías; no reconstruir resultados desde el comando o desde un DTO anterior.

RunOnFixtureAsync requiere dicho lector explícitamente; no hay lector permisivo ni entrada que encuentre secretos y lance SQL automáticamente. No se ejecutaron S01–S04 contra SQL, no se acreditaron operaciones durables ni se cierra la fase7. Las tareas de fallos reales, concurrencia y cierre integrado siguen fuera de este cambio.
