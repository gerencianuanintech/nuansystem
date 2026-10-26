# Fase 7: fallos y cancelación — tarea 4

Fecha: 2026-10-24. Estado: implementación y validación **offline**. Sin SAP, conexiones a bases de datos, instalación de triggers, cambios de esquema ni semillas.

## Descubrimiento y alcance

Cambio de infraestructura de pruebas del piloto BusinessPartners, sin cambios de producto. Se reutilizan SqlTransactionRunner, BusinessPartnerUnitOfWork, los handlers CRUD, BusinessPartnerLocalOutboxWriter y SyncEventPayloadFactory. La extensión vive en tests; Domain/Application/Persistence/API/WinForms y workers permanecen sin cambios.

Referencias inspeccionadas: plan de validación aislada, fixture/probe/checkpoints de tareas2–3, contratos de tablas en024/028/065/126 y runner de producción; catálogo de transacciones, aristas de consolidación y gates R4/R5/R9/R10/R11. Se aplicaron las guías de discovery, persistencia, SQL y testing, optimización de contexto y ejecución/TDD. El inventario efectivo completo de dependencias continúa siendo requisito del laboratorio, no se sustituye por esta lista de referencias.

Decisión: entregar un **renderizador puro** del script, en vez de un instalador automático o una migración en database/sql. Genera nombres/condiciones ligados al RunId y calcula el SHA256 de los bytes UTF8 del resultado. La instalación y aprobación de ese resultado concreto son pasos separados. Ningún componente de esta tarea ejecuta el script generado.

## Escenarios preparados

| Caso | Disparo futuro en laboratorio | Comprobación offline |
|---|---|---|
| S05 | Trigger en AuditCatalogChanges, THROW51505 | Error original y reversión de negocio/hijas sin nueva auditoría/intención. |
| S06 | Trigger en LocalOutbox, THROW51506 | Reversión del negocio y auditoría, sin intención durable. |
| S07 | Trigger en SyncAudit, THROW51507 después de insertar Outbox | Reversión de la intención parcial junto con negocio/auditoría. |
| S08 | Excepción tras retornar Create/Update/Delete del repositorio, antes de encolar | Runner exterior revierte; no se interpreta el COMMIT interno del SP como durabilidad. |
| S09 | Error de validación SQL del procedimiento, elegido explícitamente por el caso | Se conserva la excepción y se exige estado posterior idéntico. |
| S10 | Barrera AfterEnqueue, después de negocio/auditoría/intención | Cancelación del token de operación y retorno completo antes de observar. |
| S11 | Comando WAITFOR de5s en la misma sesión/transacción; ACK del servidor antes de cancelar | Contrato de barrera/token validado con comando simulado; comportamiento de SqlClient/servidor pendiente. |

El doble offline recorre los siete puntos para alta, edición y baja:21 combinaciones nominales de fallo. Usa handlers, runner, unidad y writer reales sobre persistencia en memoria con estado pendiente/confirmado. Sus rollback y auditorías son un modelo de expectativa, **no evidencia de ejecución SQL**.

## Script de laboratorio

`Phase7SqlFaultScript.Render(runId,role,companyId)` devuelve texto y hash; no recibe conexiones. Rechaza RunId vacío, rol distinto de A/B y empresa inválida. El script resultante:

- Exige base exacta `NuanSystem_Phase7_<RunIdN>_<A|B>`, DB_ID>4, propiedades extendidas de base `Phase7.RunId` y `Phase7.Purpose=BackendTransactionValidation`, y ausencia de transacción ambiental de instalación.
- Exige tablas existentes; crea únicamente tres triggers AFTER INSERT con nombres propios que incluyen el RunId completo. Rechaza nombres ya existentes, sin sobrescribir/deshabilitar triggers ni alterar procedimientos. La creación agrupada usa transacción y revierte si falla; el script no es una reparación de objetos existentes.
- Cada trigger retorna sin efecto si no coinciden destino, RunId de sesión y de base, CompanyId y FaultPoint. Además debe coincidir exactamente el CaseCode de sesión con una fila del fixture y su prefijo `P7_<RunId8>_`. No usa LIKE con guiones bajos como comodines.
- AuditCatalogChanges usa EntityName plural BusinessPartners y RecordId asociado al socio; LocalOutbox y SyncAudit usan BusinessPartner singular. SyncAudit enlaza EventId/CompanyId/EntityGlobalId con la fila de Outbox recién insertada. Las comprobaciones consideran conjuntos inserted, no una sola fila supuesta.

Las condiciones funcionan sin marcadores activados; esta propiedad se inspeccionó estáticamente y el renderizado se probó offline. **No se compiló ni ejecutó el T-SQL en SQL Server.** Antes de instalar, el provisionador debe revisar/aprobar el texto/hash concreto, verificar permisos y prerequisitos, comprobar las definiciones desplegadas y actualizar las huellas del manifiesto de ejecución. Las propiedades de laboratorio y sus valores deben entrar en el preflight pendiente; este cambio no las crea.

## Sesión, limpieza y barreras

`CreateFailureUnitOfWork` mantiene repositorios reales y el camino privado de verificación de destino antes del callback. Rechaza otro RunId antes de crear conexiones. `Phase7SqlFaultSession` exige SqlConnection/SqlTransaction de la misma sesión abierta, Pooling=false, Enlist=false y MARS=false antes de colocar marcadores. Usa parámetros para RunId/CompanyId/CaseCode/FaultPoint y read_only=0. El CaseCode y prefijo se normalizan a mayúsculas, igual que los handlers, incluidos RunId con letras hexadecimales y collation sensible a mayúsculas.

La limpieza elimina los cuatro marcadores con timeout/token propios de2s, aun cuando el token original esté cancelado. Si falla, conserva la excepción primaria y anota la secundaria en `Phase7Sql.SessionCleanupException`. Una limpieza fallida sin error previo provoca rollback en vez de confirmar. No se confía en rollback para borrar SESSION_CONTEXT; la sesión dedicada sin pooling se desecha por el runner, incluso si el contexto no pudo restablecerse en una transacción abortada o conexión rota.

Los checkpoints capturan estado dentro de la unidad, después del repositorio o encolado. S10 espera cancelación cooperativa tras AfterEnqueue. En S11, el helper SQL ejecuta RAISERROR de severidad10 WITH NOWAIT con un nonce, seguido de WAITFOR acotado; InfoMessage reconoce ese nonce antes de señalar la barrera. Así se exige una señal del batch del servidor, no una pausa arbitraria del cliente. El handler de InfoMessage se desuscribe en finally. El helper exige timeout de comando≥6s; el valor procede del manifiesto. Si el WAITFOR termina normalmente, el caso falla.

El coordinador distingue el token de operación —incluido un token enlazado interior— del token del usuario y del presupuesto global del caso (1–30s). Solo acepta cancelación tras ACK y con el token registrado en la barrera. No acepta timeouts ni cancelaciones como errores S05–S09, aunque un clasificador demasiado amplio los incluya. El vencimiento del presupuesto falla; se cancela y espera hasta2s adicionales de terminación. Si la operación no termina, se informa `PHASE7_SQL_OPERATION_NOT_STOPPED` y no se lee un snapshot ni se acredita cierre/rollback. La investigación de una sesión que no termine sigue requiriendo el procedimiento autorizado del laboratorio, sin KILL global.

S09 requiere un predicado explícito sobre SqlException para el error de procedimiento esperado; no se inventa un código SQL universal. No se confunden errores de setup/preflight con un fallo SQL aprobado.

## Evidencia de rollback

`Phase7SqlRollbackSnapshot` exige las16 tablas del vertical: socio, direcciones/contactos, fiscal, contabilidad, crédito, compras, mapping SAP, bancos, mappings de campos, retenciones, notas, metadatos de adjuntos, AuditCatalogChanges, LocalOutbox y SyncAudit. Congela las filas recibidas. La comparación dentro de un caso es exacta: conserva ids, fechas, flags y valores. No aplica la normalización de equivalencia entre fixtures de la tarea3. La asignación de IDENTITY/sequence no forma parte del snapshot y puede dejar huecos legítimos.

Se observa antes y después de terminar el handler y su rollback, mediante token independiente del token cancelado de la operación. El lector futuro debe filtrar el caso de manera completa, incluir sus posibles efectos parciales y devolver filas en orden determinista, sin NOLOCK. Un alta debe partir sin filas del fixture; una edición/baja debe partir de un estado aprobado y capturado. El helper comprueba igualdad con ese baseline; el harness debe acreditar la preparación inicial.

Si aparecen cambios, falla la validación y conserva el error primario en `Phase7Sql.PrimaryFailure`. Si falla el observador, el caso no se aprueba y también conserva esa referencia. No exige borrar logs diagnósticos externos a las tablas de negocio/intención.

## Validación realizada y límites

**Validado:** compilación Release completa sin restauración, 0 errores y0 advertencias; regresión offline **1410 aprobadas,0 fallidas,0 omitidas** (baseline1378 +32 pruebas). Incluye escenarios21 por operación/punto, fugas deliberadas de estado, errores secundarios, snapshots completos/inmutables, errores inesperados, barrera ausente, tokens enlazados, cancelación del usuario, deadline, campos de destino y sesión dedicada. La revisión independiente no encontró hallazgos materiales.

Se fijaron a0 NUANSYSTEM_RUN_SQL_INTEGRATION_TESTS, NUANSYSTEM_RUN_PHASE7_SQL_TESTS y NUAN_RUN_SAP_PO_PILOT. Se excluyeron Category=SqlServerIntegration y SapPurchaseOrderPilotIntegrationTests. Artefactos locales: `artifacts/backend-solid-phase7-sql-task4/` con hashes previos, copias de archivos modificados, fallos de compilación inicial esperados, logs y TRX final.

**No validado:** instalación/compilación SQL, disparo real de triggers, COMMIT/ROLLBACK anidados efectivos, cancelación SqlClient, estado de servidor tras cancelación y durabilidad desde observador SQL. RunOnFixtureAsync mantiene obligatorios los adaptadores de preflight y lectura SQL de snapshots todavía pendientes de tareas2–3. No se añade una entrada automática que resuelva secretos, instale objetos o ejecute la matriz. La fase7 permanece pendiente de validación SQL autorizada.
