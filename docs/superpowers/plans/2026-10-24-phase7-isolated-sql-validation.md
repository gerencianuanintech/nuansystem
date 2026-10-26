# Validación SQL aislada del piloto transaccional — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers-dev:executing-plans to implement this plan task-by-task. Las casillas registran trabajo futuro. Este documento NO autoriza conexiones SQL, DML/DDL, SAP, creación o eliminación de bases. La ejecución requiere un manifiesto de entorno revisado y autorización expresa para los destinos y operaciones indicados.

**Goal:** Obtener evidencia SQL Server de atomicidad, aislamiento y recuperación del piloto BusinessPartners para cerrar su gate de fase7, sin ampliar aún la migración a otros verticales.

**Architecture:** Conservar IBusinessPartnerUnitOfWork/IBusinessPartnerTransaction y su puente a ITransactionRunner. Ejecutar handlers, SqlTransactionRunner, BusinessPartnerRepository, BusinessPartnerLocalOutboxWriter y LocalSyncOutboxRepository reales contra dos tenants exclusivos de pruebas, con conexiones observadoras independientes. No iniciar Program, inicializadores, API, workers, relay ni procesos externos.

**Tech Stack:** .NET10, xUnit, Microsoft.Data.SqlClient y Dapper existentes; SQL Server con la versión/compatibilidad del destino documentadas. Sin instalar herramientas o paquetes globales.

**Spec:** `docs/architecture/BACKEND-SOLID-PHASE-7-TRANSACTION-SESSION.md`, fase7 de `docs/superpowers/plans/2026-10-22-backend-solid-consolidation.md` y `docs/operations/BACKEND-SOLID-PHASE-7-VALIDATION.md`.

Fecha: 2026-10-24. Estado: **plan preparado; SQL no ejecutado**. Último baseline local documentado:1263 pruebas aprobadas,0 fallos, compilación limpia, en `docs/operations/BACKEND-SOLID-WINFORMS-VALIDATION.md`. No se repitió ese baseline para redactar este plan.

## Restricciones globales

- Preservar el checkout y cambios actuales; no usar reset/stash ni descartar archivos. Fijar manifiesto de fuente/binarios antes del ensayo; HEAD solo no identifica el código sin commit.
- SAP, SRI, Matriz–Sucursal, relay, workers y HTTP externos permanecen fuera. Los escenarios escriben solo fixtures sintéticos en las dos bases aisladas autorizadas.
- Ningún nombre predeterminado, cadena de desarrollo, `appsettings` productivo o resolución real desde Master puede seleccionar el destino de prueba.
- No modificar tablas/procedimientos productivos ni reescribir scripts históricos. Si aparece un defecto, registrar una corrección separada; no cambiar la expectativa para aprobar.
- No envolver todo el test en una transacción exterior que siempre se revierta: ocultaría errores de commit. La transacción bajo prueba la abre el runner real; el observador comprueba el estado durable desde otra conexión.
- No borrar datos, colas ni auditoría al terminar. La eliminación posterior de bases desechables requiere autorización independiente; conservarlas y aislarlas ante un fallo.
- Mantener ITransactionRunner y sobrecargas ADO.NET. Aprobar este piloto no autoriza migrar automáticamente otros módulos ni retirar compatibilidad.

## Evidencia de descubrimiento

| Componente actual | Evidencia que condiciona el ensayo |
|---|---|
| `Persistence/Transactions/BusinessPartnerUnitOfWork.cs` | Un contexto por callback, misma empresa por referencia, caducidad y rechazo de unidades anidadas/solapadas; comandos secuenciales. |
| `Persistence/Transactions/SqlTransactionRunner.cs` | Abre/commit/rollback; cancelación previa y antes de commit; conserva error original con fallo de rollback en `Exception.Data`. No tiene retry automático. |
| `Persistence/Repositories/BusinessPartnerRepository.cs` | Las sobrecargas transaccionales deben utilizar la pareja recibida; SP de crear/actualizar/eliminar y consulta por id existentes. |
| `Application/Features/BusinessPartners/Commands/BusinessPartnerLocalOutboxWriter.cs` | Publica solo si `IsMaster && SyncEnabled`; requiere GlobalId; genera un EventId nuevo por intención. |
| `Persistence/Repositories/Sync/LocalSyncOutboxRepository.cs`, `CreateAsync` | Inserta LocalOutbox y después SyncAudit dentro de la transacción recibida; esta ruta no necesita Master. |
| SQL024,042,126 | Hay procedimientos envolventes y transacciones internas. Validar las definiciones efectivas, no asumir que la numeración de scripts identifica un esquema completo. |
| SQL065 y124 | Fundamentos de cola local/auditoría y evolución del relay. Sus workers/procedimientos de claim/promoción no se ejecutan. |
| `SqlServerIntegrationFactAttribute` | Gate actual `NUANSYSTEM_RUN_SQL_INTEGRATION_TESTS`; debe reutilizarse con filtros adicionales exclusivos. |

Objetos críticos: `BusinessPartners`, sus tablas hijas realmente utilizadas por el DTO, `BusinessPartnerPurchaseSettings`, `AuditCatalogChanges`, `LocalOutbox`, `SyncAudit` y procedimientos `SP_NA_POST_BUSINESSPARTNERS_CREAR`, `SP_NA_PUT_BUSINESSPARTNERS_ACTUALIZAR`, `SP_NA_DELETE_BUSINESSPARTNERS_ELIMINAR` y consulta por id. Los wrappers `*_LEGACY_ACCOUNTING_DIMENSIONS` deben existir cuando la definición efectiva los requiera.

`AuditCatalogChanges.EntityName` usa **BusinessPartners**; la intención Outbox usa **BusinessPartner**. No mezclar ambos filtros al medir deltas. La auditoría de errores fuera de la transacción comercial no tiene obligación de desaparecer con el rollback; este ensayo mide la auditoría comercial y SyncAudit ligados a la operación.

## Review Focus

1. Un COMMIT interno del procedimiento no puede hacer durable el socio antes del commit exterior: pruebaS08.
2. Un error en SyncAudit después de insertar LocalOutbox debe revertir socio, hijas, auditoría e intención: pruebaS07.
3. La cancelación puede dejar un comando pendiente o una transacción no confirmable: pruebasS10/S11, sin clasificar como aprobada una espera vencida.
4. Las dos empresas pueden compartir código/identificación externa; sus datos y sesiones no pueden contaminarse, incluso con pooling: pruebasS12/S13.
5. Repetir un alta, repetir una edición y perder la confirmación de commit son situaciones distintas: pruebasS14–S17, sin prometer exactamente-una-vez donde no existe clave idempotente.

## Entorno y autorización previa

**Destinos propuestos, todavía no creados:** una instancia SQL Server exclusiva de laboratorio y dos bases con nombres `NuanSystem_Phase7_<RunId>_A` / `_B`. El nombre es solo una etiqueta, no la prueba de aislamiento. No usar DEMO, Remigio, Cañaris ni una base real existente como fixture.

El manifiesto local, sin secretos, debe registrar: RunId completo, operador/autorización, instancia exacta, dos nombres e identidades de bases, marcador de laboratorio, versión/compatibilidad/collation, configuración RCSI, hash de esquema/procedimientos, principal ejecutor y permisos, operaciones DML/DDL autorizadas, ventanas y límites de duración, y referencias a secretos efímeros fuera del repositorio. Resolver alias/instancia y comprobar identidad real antes de escribir; una coincidencia de prefijo no basta. El provisionador debe registrar SERVERPROPERTY(ServerName), DB_NAME(), DB_ID() y dos propiedades extendidas de base: NuanSystem.Phase7.RunId=<RunId completo> y NuanSystem.Phase7.Purpose=BackendTransactionValidation. La fixture compara todos esos valores con el manifiesto aprobado antes de DML; cualquier discrepancia bloquea el ensayo.

Dos autorizaciones separadas:

- **Preparación del laboratorio:** el responsable de SQL crea/provisiona solo esos destinos, esquema, semillas e instrumentación de fallos aprobada. No aplicar todos los scripts del repositorio ni inicializar un Master real por conveniencia. Si usa un clon, debe ser de esquema y datos sintéticos/saneados, sin credenciales o información productiva.
- **Ejecución del ensayo:** principal de aplicación limitado a esos tenants y a EXEC/SELECT/DML necesarios para el flujo existente. Sin permisos sobre otras bases, DDL, linked servers, jobs o administración de servidor. Principal observador separado y de solo lectura; el provisionador no ejecuta los casos como administrador.

La fixture utiliza contextos CompanyConnectionInfo sintéticos por scope. Ambos tenants se ensayan con `IsMaster=true, SyncEnabled=true` para forzar publicación local; se agregan negativos con sync desactivado o `IsMaster=false`. No se necesita base Master: sus factories/resolver se sustituyen por barreras que lanzan antes de abrir conexiones si accidentalmente se invocan. Los repositorios de socio y Outbox y el runner SQL siguen siendo reales.

## Archivos previstos (no creados en esta preparación)

| Archivo propuesto | Responsabilidad |
|---|---|
| `tests/NuanSystem.Application.Tests/Infrastructure/Phase7Sql/Phase7SqlManifest.cs` | Validar manifiesto, autorización y destinos; sin valores por defecto. |
| `.../Phase7Sql/Phase7SqlFixture.cs` | DI acotado, dos tenants, semillas identificables, observador independiente y barreras externas. No provisionar/borrar automáticamente bases. |
| `.../Phase7Sql/Phase7SqlSessionProbe.cs` | Decorar factory/runner existentes, sin cambiar contratos productivos; correlación, conteo de conexiones y sondeo de sesión. |
| `tests/NuanSystem.Application.Tests/Fixtures/Phase7Sql/fault-injection.sql` | DDL de laboratorio revisable, aplicado solo por el provisionador autorizado; nunca registrado como migración productiva. |
| `tests/NuanSystem.Application.Tests/Features/BusinessPartners/Phase7SqlSafetyTests.cs` | Guardas offline, sin habilitar SQL. |
| `.../BusinessPartners/BusinessPartnerTransactionSqlTests.cs` | Matriz SQL con `[SqlServerIntegrationFact]`, categoría `SqlServerIntegration` y scope `Phase7Isolated`. |
| `docs/operations/BACKEND-SOLID-PHASE-7-SQL-VALIDATION.md` | Informe de ejecución futuro, casos, hashes, pruebas y decisión de cierre. |

Las firmas propuestas se concretan sin introducir un segundo runner:

- `Phase7SqlManifestValidator.Validate(json, approval, gates, utcNow)` devuelve una instantánea validada offline o falla cerrado antes de crear conexión. No constituye autorización para conectar ni reemplaza la verificación online de la tarea2. Todas las rutas SQL de la fixture usan esta validación.
- `Phase7SqlFixture.CreateAuthorizedAsync(manifest, token)` valida identidad/esquema y prepara contextos; no crea bases ni aplica migraciones.
- `Phase7SqlSessionProbe` implementa/delega ITransactionRunner y permite `SnapshotAsync(label, token)` únicamente en tests. El contexto de Application sigue sin ADO.NET.
- El observador crea snapshots de filas identificadas por RunId/Code/GlobalId/EventId, incluidos valores y relaciones; no solo COUNT global.

## Tarea1 — Guardas offline y manifiesto

- [x] Escribir primero SafetyTests que rechacen manifiesto ausente, destino fuera de allowlist, servidor/bases intercambiados, autorización inexistente, credenciales embebidas en el manifiesto y gate específico desactivado.
- [x] Implementar manifiesto/validador sin conexiones por defecto ni lectura de configuración del producto. Una denegación no crea siquiera una SqlConnection abierta.
- [x] Reutilizar el gate estándar de SqlServerIntegrationFact; añadir `NUANSYSTEM_RUN_PHASE7_SQL_TESTS=1` como condición de la fixture, no como sustituto de autorización. Si el filtro SQL se ejecuta con manifiesto inválido, debe fallar/bloquear con causa, no producir un aprobado por retorno temprano.
- [x] Ejecutar SafetyTests con ambos gates apagados y sin servidor. Entregable: guardas aprobadas sin tocar SQL.

## Tarea2 — Laboratorio y trazabilidad de sesión

- [ ] Con autorización de preparación, cerrar manifiesto de objetos/semillas y orden de dependencias según definiciones efectivas de024/042/065/124/126 y los prerequisitos encontrados. Registrar hashes/SchemaHistory; si falta algo, detener y preparar su instalación separada.
- [x] Implementar generador de datos sintéticos `P7_<RunId8>_<Caso>`, selección inequívoca de lookup y ledger Code/Id/GlobalId/EventId por caso/tenant. Probado offline; resolver las FK reales e insertar las filas queda para los casos SQL autorizados. Los huecos de IDENTITY tras rollback son válidos.
- [x] Implementar tracking de ITenantConnectionFactory y probe delegado sobre SqlTransactionRunner real. El pipeline exige **una** conexión de escritura por unidad; observadores se cuentan aparte. Validación actual con conexiones simuladas.
- [x] Implementar captura de `@@SPID`, `DB_NAME()`, `@@TRANCOUNT`, `XACT_STATE()` y DB_ID en entrada/checkpoints/BeforeCommit, exigiendo sesión/base estable, conteo1 y estado1. Sin handles en Application ni cadenas/passwords en muestras. La captura real en SQL aún no se ha ejecutado.
- [ ] Comprobar desde observador independiente el resultado durable tras commit/rollback. Antes de commit, READ COMMITTED puede bloquear y RCSI puede devolver estado anterior: respetar configuración, usar esperas limitadas y no usar NOLOCK como prueba de atomicidad.
- [x] Componer explícitamente el piloto sin inicializadores, hosted workers, relay ni conexiones Master/SAP/SRI/HTTP; barreras Master/resolver fallan al invocarse. Verificado offline.

Implementación offline de tarea2: [detalle y pendientes de preflight](../../operations/BACKEND-SOLID-PHASE-7-SQL-TASK2.md). Fixture y observador implementados; quedan sin marcar el cierre de objetos/semillas y la comprobación durable real. `IPhase7SqlDestinationVerifier` es obligatorio, sin adaptador SQL por defecto: antes de ejecutar SQL se debe implementar su cotejo completo del manifiesto contra el destino. No se abrió ninguna conexión ni se ejecutó SAP. Regresión local:1361/1361, incluidas33 pruebas nuevas; compilación sin errores/advertencias.

## Tarea3 — Caminos nominales y equivalencia

- [x] ImplementarS01–S04 con handlers/validadores reales, sin transacción de test exterior; escenarios reutilizables conectados a la fixture y validados ahora con persistencia simulada.
- [x] Ejecutar offline una referencia por contratos ADO legacy y otra por el piloto sobre fixtures independientes equivalentes. Comparador de campos comerciales, relaciones, auditoría y eventos, sin exigir GUID/IDENTITY/fechas iguales. Repetición real SQL pendiente.
- [x] Comprobar offline conservación del GlobalId al editar/bajar y payloads sin campos sensibles o identificadores SAP excluidos por el factory. Confirmación durable SQL pendiente.
- [ ] Entregable: tres operaciones durables correctas y negativo de sync sin intención local, con evidencia por tabla y sesión.

Implementación de tarea3: [detalle, evidencia offline y límites](../../operations/BACKEND-SOLID-PHASE-7-SQL-TASK3.md). Regresión1378/1378, incluidas17 pruebas nuevas; compilación sin errores/advertencias. No se ejecutó SAP ni se abrió SQL. Antes de RunOnFixtureAsync real siguen siendo obligatorios preflight, prerrequisitos y lector SQL independiente de snapshots; la equivalencia offline no sustituye la validación durable.

## Tarea4 — Fallos reales y cancelación

- [x] Implementar renderizador puro de triggers ligados a RunId/empresa/caso, con SHA256 y guardas de destino/marcadores, sin instalador automático ni cambios de migraciones de producto.
- [ ] El provisionador instala únicamente triggers de fallo de laboratorio, con nombres propios, hash conocido y doble condición de RunId de sesión + filas del fixture. Deben ser inocuos sin esos marcadores. No deshabilitar triggers existentes ni alterar procedimientos para el ensayo.
- [x] Implementar S05–S11, hooks del piloto real y comparador de rollback; validar offline los siete puntos para alta/edición/baja con persistencia simulada, sin atribuirles evidencia SQL.
- [ ] Instrumentar fallos en AuditCatalogChanges, LocalOutbox y SyncAudit por separado, usando THROW con código de prueba identificable. EjecutarS05–S09 con los repositorios y SQL reales, no con un escritor que lance antes de llegar a SQL.
- [x] Implementar sesiones de fallo con Pooling=false y read_only=0, limpieza acotada independiente de cancelación y conservación de errores primario/secundario. Probado offline; funcionamiento efectivo de SESSION_CONTEXT pendiente de SQL.
- [x] Implementar barreras explícitas para S10 y helper WAITFOR acotado con ACK del servidor para S11; impedir que deadline/cancelación del usuario/error inesperado aprueben el caso.
- [ ] EjecutarS10/S11 con barreras deterministas después de escrituras reales y antes de commit. Para cancelación durante comando, usar solo un WAITFOR acotado en la conexión de laboratorio y CancellationToken, sin KILL global ni esperas arbitrarias para simular éxito. Vencimiento del límite es fallo/inconcluso, nunca aprobado.
- [ ] Verificar snapshots posteriores desde otra conexión: rollback de alta deja0 filas del fixture; rollback de edición/baja conserva exactamente el estado comercial/hijas previo y0 deltas de auditoría/intención. No exigir desaparición de logs diagnósticos externos al negocio.

Implementación offline de tarea4: [detalle de escenarios, script y límites](../../operations/BACKEND-SOLID-PHASE-7-SQL-TASK4.md). Regresión1410/1410, incluidas32 pruebas nuevas; compilación sin errores/advertencias. No se instaló ningún trigger, no se ejecutó SAP ni se abrió SQL. Continúan pendientes la preparación/preflight/lector SQL y las casillas de ejecución durable real.

## Tarea5 — Empresa, concurrencia, repetición y recuperación

- [x] Implementar S12/S13 con scopes independientes, comparaciones por empresa/payload, higiene de sesión y exigencia de SPID reutilizado; validar ahora con pool/persistencia simulados.
- [ ] EjecutarS12/S13 con dos scopes/tenants y el mismo Code/identificación sintética; usar datos intencionalmente distintos y comprobar lecturas/escrituras de cada uno. Repetir con pooling activado sin marcadores de fallo y verificar que no quedan transacciones o contexto de fallo al reutilizar conexiones.
- [x] Implementar S14 con rechazo del segundo alta y snapshot idéntico; S16 con eventos por edición, deltas observados e historial conservado, permitiendo sustitución legítima de ids hijos sin ocultar cambios comerciales.
- [ ] EjecutarS14 con el mismo comando de alta dos veces: segundo intento rechazado por duplicado, una identidad y un evento de alta. La unidad sigue pudiendo ejecutarse tras fallos.
- [x] Implementar S15 con dos unidades independientes y barrera antes de INSERT, resultados/conflictos registrados, plazo/drain y ningún retry; negativos offline detectan ausencia de unicidad y errores inesperados.
- [ ] EjecutarS15 con dos altas simultáneas desde unidades independientes y barreras explícitas. La base debe proteger la unicidad activa; como máximo una alta/evento exitosos. Registrar conflictos/deadlocks sin asumir un tipo de excepción único ni reintentar automáticamente.
- [ ] CaracterizarS16: una edición repetida puede producir otra intención, porque EventId se genera por operación. Comprobar deltas según escrituras efectivas; no exigir exactamente-una-vez ni añadir silenciosamente una clave idempotente.
- [x] Implementar S17 cierre solo del propietario antes de commit, snapshots exactos y contadores contra reconexión/replay; S18 contexto/empresa/anidamiento/solapamiento y reutilización posterior. Probado offline con runner/UoW reales.
- [ ] EjecutarS17 cerrando solo la conexión propietaria del caso antes de commit: observar rollback, preservar error primario/secundario y confirmar que el callback no se repite. Esto NO reproduce pérdida de respuesta después de commit. Para ese desenlace incierto, documentar conciliación por identidad/evento antes de cualquier retry; pruebas de caída de red/servidor requieren otro alcance autorizado.
- [ ] EjecutarS18: invalidar contexto al salir, rechazar otra empresa o anidamiento/solapamiento de ExecuteAsync y comprobar que no aparece una segunda transacción. No lanzar comandos paralelos dentro de un mismo callback.

Implementación offline de tarea5: [detalle y evidencia](../../operations/BACKEND-SOLID-PHASE-7-SQL-TASK5.md). Regresión1439/1439, incluidas29 pruebas nuevas; compilación sin errores/advertencias. No se ejecutó SAP ni se abrió SQL. Las casillas de ejecución real siguen pendientes del preflight/lectores y laboratorio autorizado; la fase7 continúa abierta.

## Matriz de aceptación SQL

| Caso | Escenario | Evidencia obligatoria |
|---|---|---|
| S01 | Alta | Socio/hijas válidos, GlobalId no vacío, auditoría del usuario y1 intención Created + SyncAudit vinculada. |
| S02 | Edición | Mismo Id/GlobalId; cambios/hijas y auditoría coherentes;1 intención Updated de la operación. |
| S03 | Baja lógica | IsDeleted/IsActive y auditoría correctos; misma identidad;1 intención Deleted. No borrado físico. |
| S04 | Sync desactivado o no matriz; sin SAP | Escritura comercial válida sin intención cuando IsMaster=false o SyncEnabled=false. Con ambos true debe haber intención aun con SAP=None. No inferir supresión solo por OperationMode.Standalone: no es un filtro del writer actual. |
| S05 | Fallo en auditoría comercial | Ningún cambio durable de socio/hijas ni nueva auditoría/intención. |
| S06 | Fallo insertando LocalOutbox | Rollback del cambio comercial y su auditoría; sin intención parcial. |
| S07 | Fallo SyncAudit tras insertar Outbox | Ninguna fila comercial/auditoría/Outbox parcial; confirmar rollback exterior real. |
| S08 | Excepción tras retornar del SP | COMMIT anidado no hace durable el dato; el rollback exterior revierte todo. Conteo transaccional balanceado. |
| S09 | Error SQL/validación del procedimiento | Error original conservado, sesión cerrada/limpia; no transacción huérfana ni escrituras parciales. |
| S10 | Cancelación después de escrituras | OperationCanceledException;0 cambios durables del caso y ningún commit posterior. |
| S11 | Cancelación durante comando | Cancelación efectiva acotada, reversión verificable y sin sesión de prueba bloqueando después. |
| S12 | Dos empresas, mismo código externo | Misma clave de negocio admitida por tenant sin mezcla de datos, conexiones, payloads o CompanyId. |
| S13 | Reutilización de conexiones/scopes | Sin estado/transacción de otra unidad/empresa; pooling y cierre documentados. |
| S14 | Alta secuencial repetida | Una identidad activa/evento Created; segundo resultado de duplicado, sin creación adicional. |
| S15 | Altas concurrentes iguales | Máximo un socio activo y un evento exitoso; el perdedor no deja efectos parciales. |
| S16 | Edición repetida | Identidad conservada; caracterización de eventos por operación, sin garantía idempotente inventada. |
| S17 | Cierre antes de commit | Rollback en el servidor, error primario preservado y callback ejecutado una sola vez. Sin afirmar cobertura de commit incierto. |
| S18 | Contexto inválido/anidado | Rechazo antes de operación inválida; sin conexión/transacción extra ni cambio cruzado. |

Añadir una prueba específica para el camino de procedimiento que ejecuta ROLLBACK interno al no encontrar un registro: no permitir que el runner confirme otra operación parcialmente. Determinar con el esquema efectivo si ese camino retorna resultado o excepción; compararlo con referencia legacy y documentar antes de aceptar su semántica.

## Tarea6 — Ejecución controlada y decisión de cierre

- [x] Ejecutar guardas y regresión offline, preparar procedimiento/evidencias/criterios de revisión y evaluador sin I/O. Regresión1485/1485, incluidas46 pruebas nuevas; no SQL/SAP.
- [x] Fijar catálogo propuesto de21 grupos agregados (S01–S18, Equivalence, R01.Legacy/Pilot), con subchecks obligatorios y control de TRX/contadores/identidad/hash; rechazar filtro vacío/omitidos y no confundir evidencia sintética con SQL.
- [ ] Repetir guardas y baseline del candidato final inmediatamente antes de la ejecución SQL autorizada. No usar el gate global para correr toda la suite contra un servidor.
- [ ] Una vez implementadas las clases y autorizados los destinos, ejecutar únicamente el filtro propuesto inferior. Los nombres/gate específicos todavía no existen como suite ejecutable en esta preparación.
- [ ] Repetir la matriz completa en dos ejecuciones limpias, con RunId/fixtures nuevos, sin limpiar o borrar la primera para ocultar efectos. Registrar versión SQL, compatibilidad/RCSI, conteos y todos los casos; un skip o caso inconcluso requerido impide cerrar el gate.
- [ ] Desactivar los gates del proceso al acabar, conservar evidencias/bases aisladas y emitir el informe. No promover Outbox ni consumir lo generado con workers.
- [ ] Revisar hallazgos y actualizar ADR de fase7/estado consolidado solo tras evidencia aprobada. Si falla atomicidad, aislamiento o diagnóstico, mantener compatibilidad y detener migraciones de otros módulos.

Comando futuro de alcance limitado, **no ejecutado y no utilizable hasta implementar la suite y aprobar el manifiesto**:

```powershell
# Solo en el proceso del ensayo autorizado; nunca configuracion global.
$env:NUANSYSTEM_RUN_SQL_INTEGRATION_TESTS='1'
$env:NUANSYSTEM_RUN_PHASE7_SQL_TESTS='1' # gate implementado; suite SQL ejecutable todavía pendiente
$env:NUAN_RUN_SAP_PO_PILOT='0'
# Requiere harness/preflight/lectores y resolución controlada de referencias todavía pendientes.
dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj -c Release --no-build --no-restore --filter 'Category=SqlServerIntegration&Scope=Phase7Isolated&FullyQualifiedName~BusinessPartnerTransactionSqlTests' --logger 'trx;LogFileName=phase7-isolated-sql.trx' --results-directory artifacts/backend-solid-phase7-sql/<RunId>
```

El ejecutor debe comprobar que la cantidad de casos ejecutados coincide con la matriz y que no hubo omitidos; un filtro con0 tests no es éxito. La tarea de ejecución debe incluir build de la suite ya implementada y habilitar los gates solo para su proceso, con restauración en finally o proceso hijo desechable. No guardar cadenas en argumentos, salida o repo.

Preparación de tarea6: [procedimiento operativo, catálogo y criterios de cierre](../../operations/BACKEND-SOLID-PHASE-7-SQL-TASK6.md). Estado actual NO LISTO PARA SQL: faltan preflight completo, preparación/huellas/lookups, lectores SQL y clase ejecutable ajustada al catálogo. El evaluador solo comprueba evidencia declarada para revisión independiente; nunca autentica ejecución ni cierra fase7. Se preservaron los cambios y no se abrió SQL ni se ejecutó SAP.

## Evidencia mínima y cierre

Paquete por RunId: autorización/manifiesto saneado; hashes de fuente/binarios/esquema/procedimientos; TRX y resultados por caso; snapshots antes/después filtrados por fixture; trazas de base/SPID/conteo/estado; tabla de errores primario/secundario; conteo de conexiones; cero intentos de barreras externas; configuración de pooling/RCSI; lista de sesiones de prueba terminadas y datos de laboratorio conservados. No incluir passwords, connection strings, datos reales ni tokens.

**Cerrar el gate SQL del piloto únicamente si:** todos los casos requeridos pasan en las dos ejecuciones, los efectos son atómicos y aislados, hay integridad socio/auditoría/Outbox, no hay transacciones/sesiones de prueba huérfanas, identidad y repetición cumplen el contrato real y la revisión acepta los límites de commit incierto. La comprobación no prueba recuperación de desastre, entrega de eventos o exactamente-una-vez.

**Cerrar fase7 global requiere además:** inventario actualizado de consumidores, decisión explícita de si se conservan contratos legacy y plan/revisión por cada vertical que se migre. No retirar sobrecargas aún usadas. La autorización de este laboratorio no autoriza producción, nuevas migraciones, pruebas de otros módulos, SAP, ni borrar las bases al terminar.

## Autorrevisión del plan

Cobertura revisada: nominales, fallos de auditoría/Outbox/SyncAudit, procedimientos anidados, cancelación, empresa, pooling, concurrencia, repetición y diagnóstico tienen tarea/caso asignado. Los archivos/contratos nuevos se señalan como propuestos; los símbolos actuales se distinguen de ellos. No hay tareas de modificación productiva o limpieza destructiva encubiertas. No se ejecutaron SQL, tests, build, SAP o provisionamiento al redactarlo; solo se inspeccionó fuente y se creó este documento.


## Registro de ejecución — tarea1, 2026-10-24

Tarea1 implementada exclusivamente como infraestructura de pruebas: modelo/validador/gates en Infrastructure/Phase7Sql, SafetyTests offline y plantilla inerte en Fixtures/Phase7Sql/manifest.example.json. La autorización es un objeto independiente aportado por un llamador confiable y vinculado al SHA-256 del texto UTF-8; no se acepta desde el JSON. No se verifica firma humana, identidad SQL ni permisos efectivos en esta tarea.

Decisión: separar modelo y validador para que no haya factories ni funciones de conexión en la validación. Se reutiliza el parser del gate estándar cambiando únicamente IsEnabled de private a internal, sin alterar su lógica. Los casos positivos de pruebas simulan ambos gates con valores inmutables; los gates reales del proceso se mantienen en0. Se exige 1..3600 segundos por ejecución y 1..120 por comando, incluido todo el presupuesto dentro de la ventana UTC aprobada.

La plantilla deliberadamente no pasa validación: RunId vacío, IDs0, hashes cero y ventana vencida. El resultado válido siempre indica RequiresOnlineVerification=true. Las tareas2–6, el laboratorio, la fixture SQL y la inyección de fallos no se implementan ni ejecutan en este incremento. Contrato y evidencia: docs/operations/BACKEND-SOLID-PHASE-7-SQL-TASK1.md.
