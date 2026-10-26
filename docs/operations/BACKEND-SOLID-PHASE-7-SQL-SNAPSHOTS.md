# Fase 7: lectores de snapshots SQL y pruebas offline

Fecha: 2026-10-25. **NO LISTO PARA SQL; fase 7 abierta.** No se abrieron conexiones, ejecutaron consultas ni SAP.

## Alcance y descubrimiento

Se amplía exclusivamente infraestructura de pruebas. Se reutilizan `IPhase7SqlDestinationVerifier`, el observador independiente de `Phase7SqlFixture`, `Phase7SqlRollbackSnapshot`, `Phase7NominalSnapshot` y `Phase7SqlReliabilitySnapshot`. Referencias revisadas: escenarios de tareas3–5, repositorio de socios, DTOs y definiciones024/028/042/065/124/126. Las reglas rectoras y catálogos se reutilizan de la consolidación y del preflight precedente. No hay cambios de arquitectura de producto, migraciones, permisos, configuración ni DI.

## Lectores implementados

`Phase7SqlSnapshotReaders` se configura con manifiesto/destino y código de caso perteneciente al RunId. Exige un verificador explícito, una conexión observadora ya abierta y presupuesto de comando válido. Ejecuta el preflight en esa misma conexión sin transacción antes de leer. La fixture sigue siendo responsable de autorización independiente, gates, ventana, presupuesto global y exclusión de operaciones de escritura activas. Se debe invocar siempre mediante `tenant.ObserveAsync`, nunca mediante conexiones descubiertas por el lector.

Métodos compatibles con los delegates existentes:

- `ReadRollbackAsync(connection, timeout, token)`: evidencia física exacta de las16 tablas requeridas, todas sus columnas, filas vigentes/eliminadas y filas huérfanas, ordenadas por la clave física. Conserva timestamps, ids y columnas adicionales. No normaliza identidades ni oculta escrituras parciales.
- `ReadNominalAsync(connection, id, timeout, token)`: socio del código de caso e id exacto, incluyendo padre eliminado; valores comerciales persistidos, hijos vigentes, notas, auditoría, intenciones y SyncAudit. No usa el procedimiento de consulta de producto, que excluye padres eliminados.
- `ReadReliabilityAsync(connection, timeout, token)`: socios del código de caso y sus evidencias, incluyendo casos vacíos. Detecta eventos del código o GlobalId relacionados, y rechaza CompanyId/EntityName incompatibles. Las intenciones sin SyncAudit se conservan como tales; no se fabrican filas faltantes.

Las vistas DTO muestran valores almacenados. Los nombres/etiquetas derivados de catálogos (por ejemplo, PaymentTermName o CustomerAccountName) quedan fuera de esta proyección; los ids, códigos y valores almacenados se preservan. No usar estas vistas para validar nombres de lookups de la API. Los registros físicos son la evidencia completa para rollback. Las vistas por caso no sustituyen esa evidencia: una fila huérfana sin padre/GlobalId conocido puede no pertenecer a su proyección comercial.

Los hijos bancarios, retenciones, mappings y adjuntos usan sus claves físicas `BusinessPartner…Id`; solo la proyección DTO las presenta como Id. Se evita sobrescribir CreatedAt/UpdatedAt del padre con timestamps de filas auxiliares. Si falta una de las cinco filas escalares obligatorias, no se completa con defaults del comando: se rechaza la proyección. JSON mal formado, tablas faltantes/adicionales, claves duplicadas, identidades inválidas o respuestas superiores a8Mi caracteres también se rechazan. Los errores de lectura/proyección se sanean y la cancelación solicitada se propaga.

## Decisión de alcance físico

El snapshot de rollback captura las16 tablas completas de la **base de laboratorio dedicada al RunId**, y no únicamente un JOIN al socio del caso. Un JOIN podría ocultar un SyncAudit parcial sin Outbox o hijos huérfanos tras un fallo. Esta lectura amplia permite detectar también efectos colaterales en otros casos/seeds del laboratorio. Está limitada al destino aislado aprobado: no se debe usar en bases reales, compartidas o con datos productivos. Los casos deben usar datos sintéticos.

`Phase7SqlSnapshotSqlReader` construye un SELECT desde una lista cerrada de tablas y claves físicas. No interpola valores de entrada. Destino, DatabaseId, RunId, propósito y CompanyId se envían como parámetros; la consulta exige coincidencia de marcadores de base y TRANCOUNT/XACT_STATE=0. Si el guard no coincide, no retorna snapshot y el lector falla. No crea conexiones ni transacciones, modifica filas, cambia aislamiento, instala objetos o utiliza NOLOCK. Requiere `SqlConnection` ya abierta.

La captura usa un único comando de lectura con subconsultas JSON. **No promete aislamiento snapshot bajo READ COMMITTED.** Debe realizarse después de que terminen todas las operaciones del caso, con workers/relay y escritores externos detenidos; no observar mientras la barrera concurrente siga activa. La fixture impide escrituras activas propias, pero la ausencia de escritores externos sigue siendo un requisito del laboratorio. La consistencia efectiva con RCSI activado/desactivado y la consulta contra cada versión aprobada requieren validación SQL posterior autorizada.

## Integración futura y límites

Crear un lector por caso y tenant, suministrarle el verificador del preflight y pasar sus métodos como delegates a los helpers nominales, de fallos/recuperación y confiabilidad existentes. Los observers son independientes del escritor y solo se usan cuando el handler terminó o el fallo/cancelación drenó. La lectura de rollback debe usar un token/budget de observación independiente del token cancelado de la operación.

Las pruebas de esta tarea inyectan JSON sintético mediante un puerto de lectura; no abren conexiones y no ejecutan el SELECT. El test del adaptador SQL predeterminado le entrega un proveedor falso y comprueba rechazo sin crear comandos. Esto valida selección, conservación, proyección, errores y límites offline, **no prueba sintaxis/mapeo SQL real ni atomicidad del producto**.

Siguen pendientes la suite SQL ejecutable, inventario/provisión y revisión del laboratorio, validación real del preflight/lectores, ensayo específico del rollback interno y dos corridas de aceptación. Ninguna evidencia offline autoriza ejecutar SQL o cerrar la fase7.

Artefactos locales de preservación, compilación y pruebas: `artifacts/backend-solid-phase7-sql-snapshots/`. No se modifican archivos preexistentes ni se cambia el plan histórico para presentar validación real como terminada.

## Resultado offline

Solución Release compilada sin restauración:0 errores y0 advertencias. Regresión: **1564 pruebas aprobadas,0 fallidas y0 omitidas**, incluidas35 nuevas de snapshots. Se excluyeron suites SQL/SAP y sus gates permanecieron en0. Las221 rutas del baseline conservan sus hashes. La prueba específica de claves físicas reprodujo el fallo antes de corregirlo y pasó después.

Revisión independiente sin hallazgos importantes. Se incorporaron las recomendaciones menores de sanear también los filtros/IsDeleted mal tipados y comprobar la cancelación después del parseado/proyección, con cobertura offline. No hay corridas SQL reales ni evidencia de cierre de fase.
