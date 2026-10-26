# Fase 7: fixture y trazabilidad de sesión — tarea 2

Fecha: 2026-10-24. Alcance implementado y validado **offline**. No se ejecutó SAP, no se abrieron conexiones de datos y no se aplicaron scripts ni semillas.

## Componentes

- `Phase7SqlFixture`: recibe el JSON y la aprobación independiente de la tarea 1, dos pares explícitos de factories escritura/observador y un verificador obligatorio de destinos. No busca conexiones, lee secretos, carga configuración de la aplicación ni arranca servicios. En uso normal lee los dos gates reales; las pruebas inyectan valores locales sin cambiar esos gates.
- Cada tenant compone `BusinessPartnerUnitOfWork`, `BusinessPartnerRepository`, `BusinessPartnerLocalOutboxWriter`, `SyncEventPayloadFactory` y `LocalSyncOutboxRepository` reales. El contexto usa la empresa del manifiesto, SAP=None, IsMaster=true y SyncEnabled=true. Su ConnectionString queda vacía: los repositorios reciben exclusivamente la factory explícita del laboratorio.
- `Phase7SqlSessionProbe` decora `SqlTransactionRunner` real. Conserva los mismos handles, exige una petición de conexión por ejecución y registra ExecutionId, checkpoint, SPID, base, DatabaseId, TRANCOUNT y XACT_STATE. Entrada y BeforeCommit son automáticos; las pruebas futuras deben llamar CheckpointAsync tras cada paso crítico. Se exige SPID estable, base/id aprobados, TRANCOUNT=1 y XACT_STATE=1. Una anomalía medida impide confirmar aunque el callback la capture. El runner conserva sus reglas de rollback y error original.
- `ObserveAsync` abre una conexión independiente, sin transacción exterior, exige TRANCOUNT=0/XACT_STATE=0, verifica el destino y entrega timeout/token al lector de snapshots. Se permite después de completar la ejecución, incluido rollback; se rechaza durante la operación del mismo tenant. Los contadores de observación y escritura son independientes.
- `Phase7SqlLedger` genera `P7_<RunId8>_<Caso>`, distingue A/B y registra Id/GlobalId/EventId sin suponer secuencias IDENTITY. `ResolveLookup` exige exactamente un candidato positivo leído del destino autorizado: cero o varios candidatos detienen el caso. No inserta semillas.

El probe ejecutable es privado en la fixture: se exponen únicamente sus muestras, contador y checkpoints. No hay service provider general, Program, inicializadores, hosted workers, relay, SAP, SRI ni HTTP. Las dependencias Master y CompanyResolver del outbox son barreras que lanzan al invocarse.

## Barreras y contrato de preparación

La construcción valida el manifiesto sin I/O. Antes de crear conexiones y antes de checkpoints se comprueban nuevamente gates, vigencia y presupuesto de ejecución. Un token enlazado limita toda la ejecución; el observador tiene además el límite de comando. Las factories deben devolver conexiones cerradas. Para SqlConnection se comprueban servidor/base/principal declarados, contraseña presente, TLS obligatorio y certificado validado, sin attach, failover alternativo, MARS, enlistamiento ambiental ni credenciales persistentes. El timeout de conexión debe ser positivo y no superar el de comando. Este laboratorio utiliza autenticación SQL con login igual al principal declarado; no toma autenticación integrada de la máquina.

`IPhase7SqlDestinationVerifier` es un **puerto obligatorio del harness de ejecución futura**, sin implementación permisiva por defecto. Debe cotejar en la misma sesión, mediante lecturas acotadas, servidor/versión, base/id, RunId/propósito, empresa, compatibilidad/collation/RCSI, hashes efectivos, SchemaHistory, principal y permisos reales antes de devolver control al caso. Las factories y este adaptador son código confiable del harness; la interfaz no prueba identidad por sí misma. La validación de cadenas solo restringe el destino solicitado y no sustituye esa comprobación online.

**Todavía no se entrega un adaptador SQL de preflight ni una entrada ejecutable que resuelva secretos y lance casos reales.** El doble usado en las pruebas solo comprueba el enrutamiento y rechazo. Incorporar el adaptador y cerrar su contrato de fingerprints es un requisito pendiente antes de habilitar cualquier ejecución SQL, no una propiedad acreditada por estas pruebas.

También queda pendiente, con preparación autorizada, cerrar el inventario efectivo de objetos/semillas y sus hashes según 024 → 042 → 126 para BusinessPartners y 065 → 124 para LocalOutbox, incluidos sus prerrequisitos. El orden expresa dependencias del vertical, no una instrucción de instalar scripts automáticamente. Si faltan objetos, procedimientos legacy envueltos, catálogos FK o marcadores del laboratorio, detener; su instalación requiere una tarea separada. No asumir que el hash de un archivo equivale al hash de la definición desplegada.

El observador debe usar su principal SELECT y consultas parametrizadas con el timeout/token recibidos. No usar NOLOCK. READ COMMITTED puede bloquear y RCSI devolver versiones anteriores; esta infraestructura no acredita atomicidad por una lectura previa al commit. No se han observado todavía filas durables ni ejecutado S01–S18. Los registros BeforeCommit tampoco prueban commit durable.

## Decisiones de implementación

- Se conserva el trabajo existente en su ubicación: trasladarlo a un checkout limpio excluiría las fases previas sin confirmar.
- Se implementa la fixture sin habilitar conectividad implícita. El adaptador obligatorio y las factories explícitas permiten probarla con dobles ahora; la verificación de destinos reales y el cierre de dependencias quedan visibles como pendientes.
- No se modifica Application ni Persistence para instrumentar pruebas. Los handles permanecen únicamente en infraestructura de pruebas y en los contratos legacy existentes.

## Evidencia local

Compilación Release de `NuanSystem.sln` con `--no-restore`: 0 errores, 0 advertencias.

Regresión offline: **1361 aprobadas, 0 fallidas, 0 omitidas**; baseline anterior 1328 más 33 casos nuevos. Se excluyeron Category=SqlServerIntegration y SapPurchaseOrderPilotIntegrationTests y se fijaron a0 los gates SQL general, SQL fase7 y SAP piloto en el proceso.

Cobertura nueva: handles/sesión, drift de SPID/base/estado/conteo, rechazo anidado y de segunda conexión, anomalías capturadas, rollback y reutilización, revocación/caducidad/cancelación, preflight obligatorio, composición del piloto real, separación A/B, observador independiente y exclusión durante escritura, cadenas resueltas en memoria, ledger/lookups y barreras externas.

Artefactos locales ignorados: `artifacts/backend-solid-phase7-sql-task2/`: baseline de hashes, plan previo, fallo de compilación inicial por tipos ausentes, logs de compilación y TRX final. No hay evidencia de SQL real; la fase7 continúa pendiente de validación integrada contra laboratorio.
