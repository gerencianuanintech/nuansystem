# Monitor de documentos SRI

## Estado

**Fase 5.5 y sus ampliaciones de paginacion, Ribbon y alcance por importacion implementadas,
desplegadas y validadas.** La evidencia de descarga protegida corresponde a la ejecucion controlada
del 2026-07-21; el smoke final del 2026-07-30 fue exclusivamente de lectura y no autoriza repetir
descargas, llamadas al SRI ni modificaciones sobre filas protegidas.

## Discovery record

- Arquitectura revisada: Constitution/Kernel, Framework/Pattern Catalogs, Knowledge Graph, Review Checklist y contratos SRI de Iteracion 5.
- Estructuras existentes reutilizadas: `SriDocumentQueue`, `SriDocumentAttempts`, `SriAuthorizedDocuments` y `AuditSriDocumentChanges`; no se creo otra tabla de documentos ni auditoria.
- Referencia frontend: patron P5 Dashboard/Monitor de `SyncMonitorForm`, exclusivamente para ciclo visual y composicion. No se reutilizaron `SyncOutbox`, sus endpoints ni sus workers.
- Framework reutilizado: MediatR, `Result<T>`, repositorio Dapper tenant, permisos existentes, `INuanApiClient`, `NuanDataGridControl`, `NuanKpiCardControl`, `NuanActionButton`, `FormStyler` y navegacion dinamica por `FormKey`.
- Gap confirmado: el transporte JSON no podia preservar un archivo binario; se agrego `GetFileAsync` al cliente central sin crear `HttpClient` en la vertical.
- Scripts seleccionados: `118` tenant y `119` Master, siguientes numeros disponibles. Fueron desplegados y verificados idempotentemente en los esquemas autorizados.

## Alcance

`SriDocumentMonitorForm` consulta documentos registrados, detalle, intentos y auditoria, y descarga un XML previamente persistido por `NuanSystem.SriWorker`. No genera, firma, autoriza, anula, reintenta ni consulta al SRI.

## Seguridad

- `SRI.DOCUMENTS.VIEW`: menu, resumen, lista e intentos.
- `SRI.DOCUMENTS.VIEW_PAYLOAD`: detalle seguro y auditoria.
- `SRI.DOCUMENTS.DOWNLOAD_XML`: accion y endpoint de descarga.
- La empresa proviene del contexto autenticado y el repositorio abre solamente la base tenant activa.
- La pantalla no presenta XML, claves de acceso completas ni rutas del servidor.

## Descarga

Seleccione un documento `Authorized` con XML disponible, pulse **Descargar XML** y elija una ubicacion en `SaveFileDialog`. El cliente conserva los bytes y no abre el archivo. Cada exito agrega una auditoria `DownloadXml`; repetirlo no duplica el documento ni cambia el estado.

## Evidencia de despliegue y runtime

### Cierre del monitor — 2026-07-30

- `149_master_sri_document_monitor_ribbon_operations.sql` registra Actualizar, Consultar, Filtro y
  Descargar XML en el Ribbon corporativo, conservando la autorizacion API como gate independiente.
- `150_tenant_sri_document_monitor_import_scope.sql` agrega el filtro opcional `ImportId` a los
  procedimientos del monitor; el modo global no cambia.
- `151_tenant_sri_document_monitor_summary_bigint_repair.sql` conserva `Total`, `Pending`,
  `Querying`, `Authorized` y `Errors` como valores SQL `bigint` materializables por Dapper.
- El modo global mostro 4.048 documentos, 2.721 pendientes, 2 autorizados y 0 errores, con
  50 registros por pagina. El filtro Authorized mostro las colas autorizadas existentes sin
  descargar sus XML.
- Abierto desde Importaciones TXT SRI, el mismo formulario mostro el titulo
  `Monitor SRI - Carga 17` y exclusivamente 1.322 filas `Staged` vinculadas, en 27 paginas.
- Las acciones Ribbon conservaron el contexto y los permisos del monitor. Descargar XML solo fue
  visible para una seleccion autorizada y no fue ejecutada.
- Los cuatro KPI del monitor y los seis KPI de Importaciones TXT SRI se mostraron completos. El
  ajuste del importador es local y no modifica el contrato compartido de `NuanKpiCardControl`.
- API health y Swagger respondieron HTTP 200; las rutas SRI quedaron agrupadas exclusivamente bajo
  `SRI`. No hubo escrituras SQL, workers, llamadas al SRI, llamadas a SAP ni nuevos archivos XML.
- Build Release: 0 errores y 0 advertencias. Suite: 594 aprobadas, 5 diagnosticas omitidas y
  0 fallidas. La revision visual fue aprobada por el propietario.

- El 2026-07-25 se aplico dos veces en `NuanSystem_DEMO`,
  `NuanSystem_DEMO_REMIGIO` y `NuanSystem_DEMO_CANARIS` el forward repair
  `123_tenant_sri_document_monitor_summary_bigint_fix.sql`. Cada tenant quedo con exactamente una
  version `20260725.123` y un procedimiento; la metadata SQL confirmo `bigint` para `Total`,
  `Pending`, `Querying`, `Authorized` y `Errors`.
- El procedimiento corregido se materializo mediante Dapper contra
  `SriDocumentMonitorSummaryDto` con el resultado real `4/0/0/1/0`. El Monitor SRI abrio sin el
  error interno anterior, mostro los KPI `4/0/1/0` y repitio la consulta con **Actualizar** sin
  excepcion. Remigio y Canaris materializaron mediante Dapper `0/0/0/0/0`, conservaron estables
  los conteos protegidos y recibieron respaldos `COPY_ONLY` verificados antes del despliegue. No
  se inicio el worker, no se realizaron llamadas al SRI y Master no fue modificado.
- La regresion Dapper tambien materializa el resumen vacio como cinco valores `long` en cero. No
  se eliminaron ni aislaron datos reales para producir ese escenario.
- Una solicitud real sin JWT, con la empresa `DEMO` indicada y TLS estricto, obtuvo HTTP 401 en
  `/api/sri/documents/monitor/summary`. Los perfiles HTTP 403/200 y el aislamiento entre tenants
  no se repitieron porque este forward repair no modifica endpoints, autenticacion, autorizacion,
  contexto de empresa ni seleccion de conexion; se conserva la evidencia runtime de Fase 5.5.
- Se tomaron respaldos `COPY_ONLY` de Master y de los tres tenants autorizados y todos superaron `RESTORE VERIFYONLY WITH CHECKSUM`; ningun respaldo forma parte del repositorio.
- `118_tenant_sri_document_monitor_and_download.sql` se ejecuto dos veces en `NuanSystem_DEMO`, `NuanSystem_DEMO_REMIGIO` y `NuanSystem_DEMO_CANARIS`. Cada base conserva una sola version `20260721.118`, cinco procedimientos y un solo indice de auditoria del monitor.
- `119_master_sri_document_monitor_security.sql` se ejecuto dos veces en `NuanSystem_Master`. Quedaron una sola version `20260721.119`, un formulario, un menu, tres operaciones y las asignaciones ADMIN esperadas.
- Todas las conexiones de despliegue y runtime mantuvieron `Encrypt=true`, `TrustServerCertificate=false` y validacion completa del certificado.
- Un JWT renovado sin formulario ni permisos obtuvo HTTP 403. El perfil de consulta obtuvo HTTP 200 en el monitor y HTTP 403 al descargar. El perfil con los tres permisos obtuvo HTTP 200 al descargar.
- La misma identidad autenticada consulto Remigio y Canaris sin observar el documento de `NuanSystem_DEMO`; ambos tenants conservaron cero filas operativas SRI.
- QueueId `10004` se descargo dos veces desde la API como `application/xml`, con `Cache-Control: no-store`, nombre seguro `sri-10004.xml` y 10 027 bytes. El SHA-256 recalculado coincidio con el persistido y mantuvo exactamente 32 bytes; el valor completo no se documenta.
- Las dos descargas agregaron exactamente dos auditorias `DownloadXml`, con transicion `Authorized -> Authorized`, usuario y `TraceId`. Permanecieron un solo intento, un solo `SriAuthorizedDocuments`, el mismo documento, el mismo estado `Authorized` y cero locks activos.
- Los casos controlados devolvieron HTTP 404 para documento inexistente, HTTP 400 para estado no autorizado y HTTP 400 para contenido ausente. El fixture de contenido ausente fue eliminado.
- Los tres usuarios y tres roles temporales fueron eliminados. No quedaron credenciales, JWT, XML descargados, logs de prueba ni configuraciones temporales.
- `SriDocumentMonitorForm` arranco contra la API local, mostro disponibilidad y fue abierto en Visual Studio Designer. Se revisaron layout, `AutoScaleMode.Font`, tamaño minimo, docking/redimensionamiento, tema corporativo y serializacion sin detectar defectos funcionales.
- La API, WinForms y `NuanSystem.SriWorker` quedaron detenidos. El worker nunca fue levantado y hubo cero llamadas al SRI.
- La validacion final obtuvo build con 0 errores y 0 advertencias, y 441 pruebas superadas, 5 omitidas y 0 fallidas.

La instancia local de `devenv` que quedo sin ventana principal fue una observacion operativa del entorno y no un defecto del producto.

## Limites conservados

Esta validacion acredita consulta y descarga del XML previamente persistido. No implementa ni valida generacion, firma, envio, autorizacion, anulacion, eliminacion/retencion automatica, procesamiento SAP, consulta del SRI desde WinForms ni descarga de documentos reales pertenecientes a otros tenants.
