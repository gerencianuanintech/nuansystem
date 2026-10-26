# Fase 7: piloto de sesion transaccional en socios

Fecha: 2026-10-23. Proceso operativo local; sin ejecucion SAP/SQL ni cambios de schema.

## Descubrimiento y decision

ITransactionRunner expone ADO.NET y se usa en multiples verticales (inventario en artifacts/backend-solid-phase7/legacy-consumers.txt). No existe una sesion/unidad de trabajo equivalente. BusinessPartners ya tiene metodos de repositorio y LocalOutbox con conexion/transaccion compartidas, mas pruebas de publicacion. Se elige este piloto y se mantienen esos mecanismos internos.

Se agrega IBusinessPartnerUnitOfWork.ExecuteAsync y un contexto IBusinessPartnerTransaction con operaciones de socios y publicacion local, sin IDbConnection/IDbTransaction. Persistence construye un contexto ligado a los mismos objetos ADO.NET que entrega ITransactionRunner. Las tres escrituras de socios usan el puerto nuevo; queries y repositorios no cambian. Se mantiene ITransactionRunner y las sobrecargas ADO.NET para compatibilidad y para el adaptador; no se migran otros verticales.

El contexto solo puede usarse durante el callback. Valida cancelacion y la misma instancia de configuracion de empresa capturada antes de abrir la transaccion, incluida una comprobacion antes de commit. Se invalida al salir incluso con fallo. La unidad scoped rechaza anidamiento/solapamiento a traves del puerto piloto; el contrato legacy no adquiere semantica de anidamiento nueva. No se usa ambient TransactionScope ni estado global/AsyncLocal.

Las consultas/escrituras y publicacion LocalOutbox se delegan a los metodos existentes con exactamente la misma conexion/transaccion. No se llaman servicios externos dentro de la operacion. Los aliases de fase 5 y el procedimiento SAP siguen intactos. No se introduce una transaccion distribuida.

## Errores y cancelacion

Las pruebas previas con el runner real y dobles ADO.NET detectan tres problemas: conexion creada para un token ya cancelado, commit despues de cancelacion del callback y rollback que reemplaza el error original. Se comprueba cancelacion antes de abrir y antes del commit; si rollback falla se conserva el error original y el secundario queda en Exception.Data con clave interna NuanSystem.Transaction.RollbackException. No hay reintento automatico; un fallo de commit conserva su incertidumbre.

## Gate de validacion y compatibilidad

El plan exige evidencia SQL aislada antes de reemplazar el contrato general. La instruccion de esta fase prohibe ejecutar/modificar bases; por eso se conserva ese contrato y no se retiran sus sobrecargas ni se despliega una migracion general. El piloto usa el runner y SQL anteriores y se valida en memoria con dobles. Esa evidencia no demuestra atomicidad de SQL Server, constraints, concurrencia real ni idempotencia durable. La migracion de otros verticales y el retiro de compatibilidad requieren pruebas SQL aisladas autorizadas en una entrega posterior.

No se cambia la politica de identificadores ni idempotencia: se mantienen GlobalId, validacion de duplicados, auditoria y el escritor de eventos existente. No se promete deduplicacion nueva de ediciones repetidas.

## Registro operativo

Actor/empresa: contexto autenticado existente; unidad scoped por operacion. Estado: mismas validaciones de socios y resultados. Datos: mismos DTOs/normalizacion/auditoria. Transaccion: una conexion/transaction del runner por callback; Outbox local en esa misma pareja. Reversion: excepciones/cancelacion hacen rollback, diagnostico original preservado. Repositorio/SQL: sin cambios. API/WinForms/Domain/SAP: sin cambios. Pruebas: composicion, puente ADO.NET real con dobles, scope/empresa/lifetime/anidamiento, cancelacion/rollback, regresion de socios y publicacion.

Se conserva el checkout, rama y cambios existentes. Snapshot y copias anteriores en artifacts/backend-solid-phase7. Sin stash, reset, commits ni cambios de rama. El patron se documenta aqui sin editar el nucleo .codex.

## Contrato de uso y revision

Cada callback del piloto debe esperar secuencialmente sus operaciones y no iniciar trabajo separado ni transferir el contexto a otro callback. El control de concurrencia bloquea ExecuteAsync anidados/solapados; no serializa comandos paralelos dentro de un callback. Los tres handlers migrados esperan cada llamada en orden. Revision independiente de solo lectura: sin hallazgos importantes para ese uso; mantener esta condicion al migrar otros consumidores.

Resultado de esta entrega: piloto encapsulado y compatibilidad conservada. No se declara cerrada la migracion de todos los consumidores ni la prueba de atomicidad real SQL que exige el plan.
