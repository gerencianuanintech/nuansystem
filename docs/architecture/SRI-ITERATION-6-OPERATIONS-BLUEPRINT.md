# Iteracion 6 - Endurecimiento operativo del SRI Worker

## Estado y autoridad

- **Estado del documento:** decisiones bloqueantes aprobadas e implementacion tecnica inicial; no constituye aprobacion productiva.
- **Alcance:** Discovery, arquitectura y contrato operativo de `NuanSystem.SriWorker`.
- **Implementacion runtime:** contratos y plantillas implementados; validacion runtime bloqueada por el defecto de idempotencia SQL descrito abajo.
- **SQL, servicios Windows y canales externos:** `120` completo su primer pase en Master y fallo en el segundo; `121` no fue ejecutado. El forward repair `122` esta implementado pero no ejecutado. No se creo cuenta/servicio/certificado ni canal externo.
- **Autoridad:** `ENGINEERING-CONSTITUTION` > `ENGINEERING-KERNEL` > catalogos y grafo > skills aplicables > implementacion.

Este blueprint parte de la Iteracion 5 validada. No autoriza nuevas llamadas al SRI, no amplia el piloto de consulta de autorizaciones, no cambia la retencion actual y no permite habilitar permanentemente el worker. Los procedimientos operativos propuestos viven en [`../operations/SRI-WORKER-OPERATIONS.md`](../operations/SRI-WORKER-OPERATIONS.md); la evidencia historica de Fases 5.3/5.4 permanece en [SRI-WORKER-DEPLOYMENT.md](SRI-WORKER-DEPLOYMENT.md).

### Defecto de idempotencia y estado parcialmente desplegado

La validacion controlada del 2026-07-22 confirmo un defecto real en la segunda ejecucion de `120`: sus `ALTER COLUMN` incondicionales intentaban alterar `WorkerInstance` cuando `UX_WorkerHeartbeat_LogicalIdentity` ya dependia de esa columna, produciendo SQL Server 5074. El primer pase habia completado sus 11 lotes y registrado una sola version `20260721.120` en Master; preservo dos heartbeats SAP y no creo heartbeat SRI. DEMO conservo `20260721.121` ausente y su evidencia documental protegida intacta.

La correccion hace que `120` compare tipo, longitud y nullability mediante catalogo antes de cada alteracion. Si las columnas de identidad o el indice realmente requieren reparacion, elimina y recrea el indice unico filtrado dentro de una transaccion con `XACT_ABORT`; defaults y checks dependientes se preservan o restauran sin debilitar el contrato. El nuevo `122_master_worker_heartbeat_operations_idempotency_fix.sql` aplica la misma reparacion forward-safe a instalaciones sin `120`, completas o parcialmente aplicadas, vuelve a establecer procedimientos/permisos y registra una sola version `20260722.122` sin eliminar `120`.

Esta tarea valida codigo y contratos, no runtime SQL. El gate permanece **bloqueado** hasta revisar la correccion y reanudar en una ventana autorizada desde el segundo pase de `120`, seguido de dos pases de `122` y solo entonces `121`.

## Clasificacion de evidencia

| Etiqueta | Significado en este documento |
|---|---|
| Evidencia existente | Contrato o comportamiento comprobado en repositorio o en la evidencia saneada de Iteracion 5. |
| Diseno propuesto | Arquitectura candidata; requiere implementacion y validacion posterior. |
| Decision aprobada | Decision ya autorizada en los contratos de Iteracion 5. |
| Pendiente | Requiere informacion o aprobacion del propietario. |
| Fuera de alcance | No se implementa ni valida en Iteracion 6 salvo nueva autorizacion. |

## Discovery Record

**Outcome:** definir el endurecimiento operativo y los gates que permitirian evaluar `NuanSystem.SriWorker` como servicio productivo.

**Work type:** integracion/worker operativo, observabilidad, seguridad, despliegue y continuidad.

**Domain:** consulta y persistencia de comprobantes autorizados SRI; independiente de SAP y de Sync Master/Sucursal.

**Explicit domain decisions and exclusions:**

- La base Master gobierna configuracion y habilitacion; cada tenant conserva cola, intentos, XML y auditoria.
- Solo el SRI Worker puede realizar el procesamiento remoto aprobado.
- El piloto sigue limitado a consulta de autorizacion por clave; emision, firma, envio, anulacion y scraping estan excluidos.
- El XML autorizado no se elimina automaticamente mientras no exista politica aprobada.
- No se reutilizan colas, handlers ni estados de SAP o Sync Master/Sucursal.

**Affected layers:** Application/Persistence compartidos, hosting SRI Worker, Master y tenant SQL forward-only, API/monitor, seguridad, observabilidad, despliegue y documentacion.

**Risk:** alto para la implementacion futura; bajo para el cambio documental actual.

**Evidence inspected:**

- `src/Backend/NuanSystem.SriWorker/Program.cs` - Generic Host, `UseWindowsService`, Serilog, validacion de opciones, timeout de apagado y DI existentes.
- `SriBackgroundWorker` y `SriWorkerProcessor` - bucle acotado, cancelacion, concurrencia, resolucion de tenants, recuperacion de leases y procesamiento fuera de transacciones SQL.
- `SriAuthorizationProvider` - endpoints oficiales restringidos, HTTPS, timeout, respuesta acotada, validacion XML y redaccion de errores.
- `SriWorkerRepository` y scripts `115`-`119` - tenant ownership, claims atomicos, intentos, retry/DeadLetter, XML inmutable, auditoria y monitor protegido.
- `NuanSystem.SyncWorker` - referencia de hosting como Windows Service y heartbeat; su dominio SAP no se reutiliza.
- `NuanSystem.MasterBranchSyncWorker` - referencia de modo seguro, locks vencidos, shutdown y runbooks; su outbox y aplicadores no se reutilizan.
- `WorkerHeartbeat`, `WorkerHeartbeatService` y `WorkerHeartbeatRepository` - infraestructura existente en Master, actualmente ubicada bajo contratos `SapSync` y sin lectura API operativa.
- `MasterDatabaseHealthCheck` y endpoints `/health/live`/`health/ready` de la API - referencia de liveness/readiness, no health del proceso SRI.
- `AesSecretProtector`, `MasterConnectionFactory` y `SqlConnectionPolicyOptions` - configuracion protegida actual y enforcement de TLS SQL.
- Documentos de Iteracion 5 y runbooks Sync/SAP - evidencia, limites, despliegue por gates, rollback no destructivo y diagnostico.

**Selected pattern:** extender de forma forward-safe el hosting y el registro operativo existentes; mantener queue/attempt/audit tenant como fuente de verdad y Master como plano de control observable.

**Permitted reuse boundary:** Generic Host, Windows Service integration, Serilog, opciones validadas, `CancellationToken`, politica SQL TLS, resolucion protegida de tenants, tabla genericamente nombrada `WorkerHeartbeat` y patrones de runbook. No se reutilizan contratos `SapSync`, `SyncOutbox`, locks SAP/Sync ni monitores de esos dominios.

**Alternatives rejected:**

- Heartbeat solo en logs: no permite consulta consistente, alertas por ausencia ni vista multiinstancia.
- Heartbeat duplicado en cada tenant: fragmenta el plano de control y obliga a consultar todas las bases para saber si el servicio vive.
- Nuevo pipeline de observabilidad paralelo: duplica `WorkerHeartbeat`, health API, Serilog y monitor existentes.
- Mutex de sistema como garantia global: solo protege un host y no sirve para alta disponibilidad multi-host.

**Implementado:** contrato de heartbeat desacoplado de SAP, vista health segura, resumen tenant, eventos operativos, lifecycle/gate/mutex, configuracion externa, plantillas parametrizadas y reparacion SQL idempotente `120`/`122`. La reparacion aun no fue ejecutada; siguen pendientes revalidacion SQL, despliegue de `121`/SCM, pruebas runtime, certificados y canales externos.

**Differences/constraints:** el heartbeat existente solo contiene instancia, empresa, ultimo beat, estado, trabajo y version; la unicidad actual es solo por instancia. El SRI Worker no emite heartbeat, no expone health propio, no renueva leases durante una llamada y no tiene pipeline productivo validado.

**Confidence:** alta para el inventario y la separacion de responsabilidades; media para la topologia productiva porque host, dominio, identidad, HA, secretos y soporte aun no estan decididos.

**Validation required:** gates de servicio, minimo privilegio, secrets, TLS, health/heartbeat, alertas, multiinstancia, lease/retry/DeadLetter, backup/restore, upgrade/rollback, redaccion, build/tests y runbook.

### Skills aplicados

Se aplicaron `nuansystem-framework-discovery`, `nuansystem-commercial-architecture`, `nuansystem-operational-usecase`, `nuansystem-sql-standards`, `nuansystem-backend-architecture`, `nuansystem-sri-document-queue` y `nuansystem-sri-worker`. El skill solicitado `nuansystem-observability` no existe en el repositorio; se usaron como sustitutos declarados `nuansystem-api-error-logging` y `nuansystem-deployment-production`. Esta sustitucion no equivale a cargar el skill ausente.

### Hallazgos obligatorios del framework

| Pregunta | Evidencia existente | Brecha/decision |
|---|---|---|
| Como se alojan workers | Generic Host + `BackgroundService`; los tres workers backend usan hosting separado. | Conservar separacion de dominio. |
| Soporte Windows Service | `UseWindowsService` ya existe en SRI, SAP Sync y Master/Branch Sync. | Faltan instalador, ACL, identidad y evidencia SCM del SRI. |
| Heartbeat | `WorkerHeartbeat` en Master y servicios bajo `SapSync`. | SRI no escribe heartbeat; evolucion compartida pendiente. |
| Errores | Provider clasifica HTTP, timeout, transporte, contrato, negocio e integridad; loop registra excepcion no controlada. | Falta taxonomia operacional comun, deduplicacion y alertas. |
| Estados operativos | API/WinForms SRI consultan cola, intentos, auditoria y XML protegido. | No muestran lifecycle/health de la instancia. |
| Secrets | `appsettings.Local.json` ignorado, variables/command line y `AesSecretProtector`; configuracion sensible tenant protegida. | Proveedor productivo, rotacion y recovery pendientes. |
| Tenants habilitados | Master filtra empresas activas con feature/integracion SRI y ambiente `Test`/`Production`. | Falta snapshot/health seguro y allowlist operacional. |
| Leases vencidos | Script `117` recupera atomicamente, cierra intento y audita. | Falta alerta, metrica y prueba durante shutdown/HA. |
| Retry/DeadLetter | Backoff exponencial con jitter, `NextAttemptAt`, max attempts y DeadLetter persistidos. | Falta dashboard host-level, umbrales y escalamiento. |
| Graceful shutdown | Tokens llegan a delay, procesador, Dapper y HTTP; host espera 30 s. | Falta estado `Stopping`, prueba de drain y politica de lease largo. |
| Logs/retencion | SRI escribe archivos diarios normales/error, 30 archivos, y consola. | Paths, ACL, capacidad, Event Log y retencion aprobada pendientes. |
| Health/monitor | API tiene liveness/readiness de Master; SRI monitor cubre documentos. | No existe health propio del worker ni endpoint agregado. |
| Plantillas reutilizables | Runbooks Sync/SAP y plantilla de polling SRI sin trabajo. | Automatizacion de servicio debe crearse solo tras decisiones. |

## Mapa de capas

| Capa | Estado de esta fase | Evidencia/accion futura |
|---|---|---|
| Domain | Verificada sin cambios | SRI sigue fuera de `Domain`. |
| Application | Cambio implementado | Contrato/evaluador operacional generico fuera de `SapSync`. |
| Persistence | Cambio implementado | Repositorio compartido por procedimientos y resumen tenant seguro. |
| API | Cambio implementado | Health SRI protegido y sin secretos. |
| Database Master | Cambio y forward repair; runtime bloqueado | `120` esta aplicado una vez; su segundo pase fallo. `122` repara idempotencia sin borrar historia y espera revision/ejecucion. |
| Database tenant | Verificada sin cambios | `121` no fue ejecutado; cola/XML/auditoria siguen autoritativos. |
| Worker | Cambio implementado | Heartbeat, lifecycle, gate, mutex, shutdown y eventos. |
| Frontend | Cambio implementado | Pestaña health en el monitor existente; solo API. |
| Security | Cambio implementado estaticamente | Permiso, config externa/TLS y ACL en plantillas; runtime pendiente. |
| Tests | Cambio | Regresion contractual cubre metadata, segundo/tercer pase, indice, defaults/checks, historia, SAP e inicializador; SQL real sigue pendiente. |
| Documentacion | Cambio | Blueprint, runbook y plantillas. |

## Infraestructura reutilizable y brechas

| Necesidad | Infraestructura existente | Decision de diseno |
|---|---|---|
| Windows Service | `UseWindowsService` y paquete `Microsoft.Extensions.Hosting.WindowsServices` | Reutilizar; parametrizar identidad operacional y empaquetado en una fase posterior. |
| Apagado | `HostOptions.ShutdownTimeout=30s`, tokens propagados a loop, HTTP y Dapper | Reutilizar; agregar estado `Stopping`, dejar de reclamar y demostrar drain/lease recovery. |
| Logs | Serilog console + archivos diarios, errores separados, 30 archivos | Reutilizar; mover paths/retencion a configuracion no secreta y probar ACL/rotacion/espacio. |
| Heartbeat | `dbo.WorkerHeartbeat` en Master, usado solo por SAP | Evolucionar el contrato compartido; no crear un heartbeat SRI paralelo. |
| Health API | `/health/live`, `/health/ready` y `MasterDatabaseHealthCheck` en API | Reutilizar el patron; health del worker se deriva de heartbeat, no de un servidor HTTP dentro del worker. |
| Tenants | `ISriWorkerCompanyRepository` filtra empresa, feature, integracion y ambiente | Reutilizar como autoridad; reportar solo identificadores y conteos seguros. |
| TLS SQL | `SqlConnectionPolicy` sobre `SqlConnectionStringBuilder` | Mantener `Encrypt=true` y `TrustServerCertificate=false` en QA/produccion. |
| Secrets | `appsettings.Local.json` ignorado, variables de entorno y `AesSecretProtector` | Desarrollo actual reutilizable; produccion requiere proveedor/identidad aprobados. |
| Claims/leases | Procedimientos de script `117` | Reutilizar; probar dos instancias, parada durante descarga y recuperacion vencida. |
| Retry/DeadLetter | Backoff persistido, jitter determinista y estados tenant | Reutilizar; agregar visibilidad, umbrales y runbook, no otro scheduler. |
| Auditoria | `SriDocumentAttempts` y `AuditSriDocumentChanges` | Reutilizar; eventos operativos del host no deben contaminar auditoria documental. |

## Modelo propuesto de Windows Service

### Identidad tecnica

| Elemento | Propuesta | Estado |
|---|---|---|
| Nombre tecnico SCM | `NuanSystem.SriWorker` | Propuesto. |
| Nombre visible | `NuanSystem SRI Worker` | Ya configurado como nombre visible; validar instalacion. |
| Proyecto/binario | `src/Backend/NuanSystem.SriWorker` / `NuanSystem.SriWorker.exe` | Evidencia existente. |
| Directorio | `%ProgramFiles%\NuanSystem\SriWorker\releases\<version>` | Propuesto; host y volumen pendientes. |
| Datos operativos | `%ProgramData%\NuanSystem\SriWorker\logs` y `diagnostics` | Propuesto; sin XML ni secretos. |
| Inicio | Automatico retrasado | Propuesto. |
| Timeout de apagado | 30 s inicial; revisar contra timeout proveedor y lease | Existente/propuesto, pendiente de prueba. |
| Habilitacion | Persistida en `false`; activacion por configuracion segura y cambio aprobado | Decision aprobada del piloto. |

Los paquetes se conservan por version. La instalacion activa apunta a una version concreta; actualizar o hacer rollback cambia el binario configurado solo mientras el servicio esta detenido. No se usa `git` en el host productivo y no se sobrescribe el unico artefacto recuperable.

### Cuenta del servicio

| Opcion | Ventajas | Riesgos/limites | Ajuste probable |
|---|---|---|---|
| `LocalService` | Privilegio local muy bajo; sin password administrado | Identidad de red anonima; dificil acceso SQL remoto y a recursos protegidos | Solo si SQL/recursos permiten el modelo; no recomendada por defecto. |
| `NetworkService` | Privilegio local bajo; usa identidad de maquina en dominio | Comparte identidad con otros servicios y requiere ACL/SQL para `DOMINIO\HOST$` | Viable en dominio para despliegues simples. |
| Cuenta local dedicada | Aislamiento y ACL claras | Password/rotacion; para SQL remoto suele requerir SQL authentication u otra gestion de secreto | Fallback para host fuera de dominio. |
| Cuenta de dominio dedicada | Identidad de red estable y permisos SQL integrados | Password y rotacion operativa; riesgo de privilegios excesivos | Viable con administracion central. |
| gMSA | Rotacion automatica, no interactiva, identidad integrada y ACL granular | Requiere Active Directory, hosts habilitados y soporte operativo | Recomendacion condicionada si el entorno lo soporta. |

No se selecciona una cuenta definitiva sin conocer dominio, host, SQL y politica corporativa. Preferencia: gMSA; fallback: cuenta dedicada de minimo privilegio. Nunca `LocalSystem` ni una cuenta administrativa.

### Configuracion y secrets por ambiente

- Versionar solo defaults seguros y configuracion no sensible; `Enabled=false` permanece en el artefacto.
- Desarrollo puede usar `appsettings.Local.json` ignorado, sin promoverlo al servidor.
- QA/produccion no crean `appsettings.Local.json`; usan Integrated Security y/o el proveedor de secretos aprobado para conexion Master y `Security:EncryptionKey`.
- Variables de entorno son una opcion transitoria, no la seleccion automatica: su scope, ACL, backup y rotacion deben documentarse.
- Los argumentos de linea de comandos no transportan secretos porque pueden quedar visibles en procesos/historial.
- Rotacion debe admitir dos versiones durante una ventana o un procedimiento de re-cifrado aprobado; perder `Security:EncryptionKey` impide resolver configuracion tenant.
- La configuracion efectiva se diagnostica solo por presencia, origen y flags no sensibles; nunca por valor.

### Permisos minimos

- Iniciar como servicio; negar inicio interactivo cuando la plataforma lo permita.
- Lectura/ejecucion sobre la release activa; escritura solo en logs/diagnosticos autorizados.
- Sin escritura en el repositorio, directorio de binarios o almacenes de certificados.
- Red saliente solo a SQL autorizado, DNS/CRL/OCSP necesarios y hosts HTTPS oficiales del SRI.
- SQL Master: lectura de empresas/configuracion habilitada y escritura exclusiva del contrato operativo futuro.
- SQL tenant: `EXECUTE` unicamente sobre procedimientos SRI aprobados y lectura de proyecciones necesarias; sin `db_owner` ni DDL.
- Lectura de almacenes de confianza de maquina. No requiere clave privada de firma en el piloto actual.
- Lectura del secreto mediante el proveedor aprobado; ningun permiso para exportar claves privadas.

### Instancias, recuperacion y apagado

- SCM evita dos procesos con el mismo servicio en un host; el control real multi-host sigue siendo el lease tenant.
- `WorkerInstance` debe ser estable y unico por host/instancia; no usar solo un nombre generico compartido.
- Para singleton, Master mantiene allowlist de una instancia y alerta si aparece otra. Para HA, se autorizan instancias distintas y se conserva claim atomico.
- Recuperacion SCM propuesta: reinicio tras 60 s en primer/segundo fallo y 300 s en fallos posteriores; reset de contador a 24 h. Valores pendientes de aprobacion.
- Un fatal de arranque devuelve codigo no cero y registra un mensaje seguro en log/Event Log.
- Al recibir stop: marcar `Stopping`, impedir nuevos claims, cancelar trabajo cooperativamente, esperar hasta el timeout y dejar cualquier lease inconcluso para recuperacion vencida.
- No liberar por fuerza un lease que aun pueda estar ejecutando una llamada; la recuperacion se realiza por el procedimiento atomico existente.

## Modelo de health y heartbeat

### Ubicacion recomendada

Persistir una fila por instancia en `NuanSystem_Master`, evolucionando de forma forward-only `dbo.WorkerHeartbeat`. El contrato de Application debe moverse en una fase posterior desde `SapSync` hacia un namespace operacional compartido, con compatibilidad para el worker SAP. No renombrar ni eliminar la tabla ejecutada.

La cola, intentos, documentos y auditorias siguen en tenant. El heartbeat solo guarda resumen seguro; no guarda XML, claves de acceso, connection strings, certificados ni excepciones completas.

Campos candidatos para expandir el registro: `WorkerType`, `InstanceName`, `HostName`, `DeploymentId`, `WorkerVersion`, `Enabled`, `LifecycleState`, `StartedAt`, `LastBeatAt`, `LastCycleStartedAt`, `LastCycleCompletedAt`, `LastSuccessfulCycleAt`, `LastSuccessfulProviderAt`, `LastCycleDurationMs`, `LastSafeErrorCode`, `LastSafeErrorMessage`, `EnabledCompanyCount`, conteos agregados de `Pending`, `RetryScheduled`, `DeadLetter`, locks activos/vencidos y `UpdatedAt`/`RowVersion`.

Un detalle por tenant se consulta bajo permiso desde las proyecciones tenant o, si la escala lo exige, mediante un snapshot hijo aprobado. No se crea automaticamente una segunda tabla de heartbeat.

### Estados

| Estado | Regla propuesta |
|---|---|
| `Healthy` | Beat reciente, ultimo ciclo completo dentro del objetivo, dependencias requeridas disponibles y sin umbral critico. |
| `Degraded` | Servicio vivo pero con cola envejecida, retries altos, dependencia intermitente o error recuperable. |
| `Unhealthy` | Beat ausente, arranque/configuracion invalida, Master inaccesible, TLS fallido persistente o condicion critica. |
| `Disabled` | Instancia viva con procesamiento explicitamente deshabilitado. |
| `Unknown` | No existe evidencia suficiente, version no reconocida o registro nunca observado. |

### Umbrales iniciales aprobados para el piloto

| Indicador | Warning/Degraded | Error/Unhealthy |
|---|---:|---:|
| Heartbeat esperado | cada 30 s | - |
| Edad del heartbeat | > 90 s | > 180 s |
| Edad maxima `Pending` | > 10 min | > 30 min |
| `RetryScheduled` | >= 5 o crecimiento por 3 ciclos | >= 20 o crecimiento por 10 min |
| `DeadLetter` nuevos | >= 1 | >= 5 en 15 min |
| Lease vencido | >= 1 recuperado | repeticion en 3 ciclos o crecimiento |
| Ultimo ciclo exitoso | > 5 min con worker habilitado | > 15 min |
| Certificado SQL/SRI observado | <= 30 dias | <= 14 dias o vencido |
| Almacenamiento libre | < 20 % | < 10 % |

Estos valores son configurables y aprobados como baseline del piloto; deben calibrarse con evidencia real antes de produccion.

### Vista operativa requerida

La API/monitor futuro debe poder mostrar, sin datos sensibles: servicio iniciado/detenido inferido, ultimo heartbeat, ciclo exitoso/error seguro/duracion, version e instancia, empresas habilitadas, colas por ambiente/estado, retries, DeadLetter, locks activos/vencidos, intentos recientes, ultima consulta satisfactoria al proveedor y estado degradado. Liveness de API no sustituye health del worker.

## Metricas y observabilidad

Metricas logicas propuestas, independientes del backend de metricas:

- `sri_worker_cycle_duration_ms` y resultado por ciclo.
- `sri_worker_jobs_claimed_total`, `completed_total` por outcome y `provider_calls_total` por ambiente/outcome.
- `sri_worker_provider_duration_ms`, timeouts y fallos TLS/transporte.
- `sri_queue_depth`, edad maxima, retries y DeadLetter por tenant/ambiente.
- `sri_active_leases`, `expired_leases_recovered_total` y conflictos de lease.
- `sri_xml_persisted_bytes`, rechazo por limite y conflicto SHA-256.
- `sri_worker_last_success_timestamp` y version/deployment.

No incluir access key, XML, RUC, authorization number, credenciales, URL configurada arbitraria ni connection string como labels. Tenant se representa por identificador interno/codigo autorizado y solo donde la cardinalidad sea controlada.

## Eventos y alertas independientes del canal

Todo evento contiene codigo estable, severidad, worker/instancia, tenant/ambiente opcionales, `TraceId`, timestamp, contador/umbral, mensaje seguro, clave de deduplicacion y estado abierto/resuelto.

| Evento | Severidad inicial | Condicion propuesta |
|---|---|---|
| `SRI_WORKER_STARTED` / `STOPPED` | Information | Cambio de ciclo de vida esperado. |
| `SRI_WORKER_HEARTBEAT_LATE` | Warning/Error | Supera umbral de heartbeat. |
| `SRI_QUEUE_GROWING` | Warning/Error | Profundidad o edad crece sostenidamente. |
| `SRI_RETRY_REPEATED` | Warning | Reintentos repetidos por categoria. |
| `SRI_DEADLETTER_CREATED` | Error/Critical | Nuevo DeadLetter; Critical por volumen. |
| `SRI_LEASE_EXPIRED` | Warning/Error | Lease recuperado o repeticion anomala. |
| `SRI_SQL_CONNECTION_FAILED` | Error/Critical | Master/tenant inaccesible; Critical sostenido. |
| `SRI_TLS_VALIDATION_FAILED` | Critical | Certificado/cadena/hostname no validos. |
| `SRI_PROVIDER_UNAVAILABLE` | Warning/Error | Fallo transitorio o ventana sostenida. |
| `SRI_XML_INTEGRITY_FAILED` | Critical | XML invalido, identidad incompatible o conflicto SHA-256. |
| `SRI_CERTIFICATE_EXPIRING` | Warning/Critical | Vencimiento dentro de umbral o expirado. |
| `SRI_STORAGE_LOW` | Warning/Critical | Espacio bajo umbral. |
| `SRI_BACKUP_FAILED` | Critical | Job o verificacion de backup falla. |
| `SRI_SCHEMA_INCOMPATIBLE` | Critical | Version requerida ausente/incompatible. |

Canales candidatos:

1. Logs estructurados: minimo obligatorio y fuente inmediata.
2. Windows Event Log: inicio/fallo temprano y Critical; requiere source/ACL instalados de forma controlada.
3. API/WinForms: consulta protegida de estado/eventos.
4. Correo, Teams u otro webhook: adaptadores futuros, solo con aprobacion de propietario, destinatarios, secretos, rate limits y escalamiento.

Los canales aprobados inicialmente son logs estructurados, Windows Event Log para arranque/fallo temprano/Critical, API protegida y WinForms. Correo, Teams, Slack, SMS y webhooks quedan fuera de alcance.

## TLS y certificados

| Frontera | Propietario propuesto | Ubicacion/confianza | Renovacion y validacion |
|---|---|---|---|
| Certificado del SQL Server | DBA/Infraestructura | Certificado servidor con SAN correcto; clave privada solo en host SQL | Renovar en ventana DBA; probar cadena, hostname y conexiones Master/tenants. |
| Confianza del cliente SQL | Infraestructura Windows | `LocalMachine` Root/Intermediate del host worker; lectura por el proceso | Distribucion controlada; validar `Encrypt=true`, `TrustServerCertificate=false`. |
| HTTPS oficial SRI | SRI; confianza local administrada por Infraestructura | Cadena publica del sistema operativo; sin certificado cliente | Monitorizar handshake/cadena/expiracion; no pinning improvisado ni bypass. |
| Firma electronica | Propietario tributario/seguridad | Fuera del piloto actual; almacen privado por definir | Fuera de alcance; no instalar, exportar ni conceder acceso al worker actual. |

La renovacion exige inventario, responsable, aviso previo, backup de configuracion no secreta, instalacion controlada, reinicio si corresponde, smoke test sin SRI, validacion TLS/health y rollback. Esta prohibido exportar claves privadas, usar `TrustServerCertificate=true`, aceptar cualquier certificado o desactivar validacion TLS. El certificado local de desarrollo no es recomendacion productiva.

## Retencion de XML y auditorias

**Decision vigente:** XML en base tenant, inmutable, maximo 5 MiB y sin eliminacion automatica ni manual aprobada.

| Alternativa | Auditoria/integridad | Recuperacion/backup | Coste/acceso | Riesgo |
|---|---|---|---|---|
| Indefinida en SQL | SHA-256 y acceso historico inmediatos | Incluida en backup tenant; restauracion simple pero creciente | Coste SQL creciente | Crecimiento sin limite y ventanas de backup mayores. |
| Por anos en SQL | Requiere plazo legal y hold/auditoria | Recuperable dentro del periodo | Predecible | Purga ilegal o prematura si el plazo es incorrecto. |
| Archivo externo inmutable | Conservar hash, URI inmutable y evidencia de exportacion | Requiere backup/replicacion y prueba de lectura | Reduce SQL; acceso mas lento | Perdida de vinculo, proveedor o permisos. |
| SQL con compresion | Mantiene ownership local | Backups potencialmente menores | Complejidad y CPU | Alterar bytes originales invalida evidencia si no se conserva canon/hash. |
| Exportacion y purga controlada | Doble verificacion de bytes/hash antes de purga | Restore debe cubrir archivo y metadatos | Ahorro SQL | Mayor riesgo operacional y de eliminacion accidental. |
| Diferenciada por tipo/tenant | Ajuste contractual | Multiplica matrices de restore | Flexible | Inconsistencia y configuracion erronea. |

La normativa legal/tributaria vigente de Ecuador y cualquier plazo minimo quedan como investigacion legal externa pendiente. No se fija plazo hasta contar con fuente vigente y aprobacion del propietario. Toda politica futura requiere legal hold, segregacion de permisos, dry-run, doble aprobacion, auditoria y restauracion probada antes de permitir purga.

## Backup, recuperacion y continuidad

### Alcance

- `NuanSystem_Master`: empresas, configuracion protegida, permisos y futuro estado operativo/alertas.
- Cada tenant SRI habilitado: `SriDocumentQueue`, `SriDocumentAttempts`, `SriAuthorizedDocuments`, `AuditSriDocumentChanges`, schema history y dependencias de tenancy.
- Binarios/version manifest y configuracion no secreta; los secretos se respaldan mediante su proveedor, no dentro del paquete.
- Logs segun politica operativa; no son sustituto de auditoria SQL.

### Politica propuesta, pendiente de aprobacion

- Full diario con `CHECKSUM`; diferencial cada 6 horas donde el volumen lo justifique.
- Log backup cada 15 minutos para bases en recovery model Full.
- `COPY_ONLY` antes de despliegue o validacion manual para no alterar la cadena normal.
- Backup cifrado, repositorio separado, acceso por minimo privilegio y copia fuera del host.
- `RESTORE VERIFYONLY WITH CHECKSUM` automatizado y restauracion trimestral en entorno aislado.
- RPO objetivo inicial: 15 minutos. RTO objetivo inicial: 4 horas. Ambos requieren aprobacion y capacidad real de infraestructura.

### Escenarios

- Perdida de servicio sin perdida SQL: reinstalar misma version/configuracion, validar heartbeat y dejar que leases vencidos se recuperen.
- Reinicio durante descarga: cancelar cooperativamente; si no completa, el lease vence y se reintenta sin duplicar documento.
- Perdida/corrupcion SQL: detener worker, restaurar Master/tenant a punto coordinado, verificar schema/hash/auditoria antes de habilitar.
- Despliegue fallido: volver a binario/configuracion anteriores; no borrar cola, intentos, XML ni auditoria.
- Backup fallido: elevar Critical, detener cambios de schema y no avanzar un despliegue productivo.

## Pipeline de despliegue y rollback

| Gate | Accion | Evidencia para continuar |
|---:|---|---|
| 1 | Validar commit, firma/hash y manifest de artefactos | Version exacta y origen aprobados. |
| 2 | Compilar y probar Release | Build/tests sin fallos no aceptados. |
| 3 | Respaldar | Backups cifrados, checksum y ubicacion registrados. |
| 4 | Validar scripts pendientes | Orden, idempotencia, compatibilidad y autorizacion por base. |
| 5 | Detener servicio | Estado detenido y sin proceso residual; leases inventariados. |
| 6 | Desplegar binarios versionados | Hashes coinciden; version previa conservada. |
| 7 | Aplicar configuracion segura | No secretos en artefacto/log; ACL e identidad verificadas. |
| 8 | Aplicar SQL autorizado | Solo scripts forward-safe y bases aprobadas. |
| 9 | Iniciar servicio deshabilitado | Inicio limpio y version correcta. |
| 10 | Validar heartbeat | `Disabled`/`Healthy` esperado y dependencias seguras. |
| 11 | Smoke test sin SRI | Sin claims ni llamadas remotas. |
| 12 | Habilitar procesamiento | Solo con autorizacion y tenants/ambientes aprobados. |
| 13 | Monitorear | Ventana, responsables y criterios de aborto activos. |
| 14 | Rollback | Ejecutar si cualquier gate critico falla. |

Rollback:

- **Binarios:** detener, apuntar a release anterior y validar hash/version.
- **Configuracion:** restaurar version no secreta previa; reinyectar secretos desde el proveedor.
- **SQL:** solo forward-fix; no rollback destructivo de scripts ejecutados.
- **Servicio:** volver a `Enabled=false`; conservar evidencia y leases para recuperacion.
- **Certificados:** volver al certificado/cadena anterior solo si sigue valido y esta aprobado.
- **Estado:** no editar manualmente estados ni eliminar evidencia; usar procedimientos autorizados.

Desinstalacion futura: deshabilitar, drenar/detener, confirmar cero procesos, exportar manifest y evidencia, eliminar solo el registro SCM y binarios segun retencion, y conservar configuracion protegida, logs requeridos y toda la evidencia SQL. La cuenta, secretos, ACL y certificados se retiran por sus propietarios solo despues de comprobar que ningun otro servicio los consume.

## Quality gates para aptitud productiva

Todos son obligatorios y requieren evidencia ejecutada:

1. Instalacion y arranque como Windows Service.
2. Reinicio automatico ante fallo no controlado.
3. Apagado controlado durante idle, claim y llamada cancelada.
4. Cuenta de minimo privilegio y ACL/SQL verificadas.
5. Secrets fuera del repositorio, paquete y logs; rotacion probada.
6. TLS estricto para Master, tenants y SRI.
7. Heartbeat visible, monotono y multiinstancia.
8. Health `Healthy`, `Degraded`, `Unhealthy`, `Disabled` y `Unknown` probado.
9. Alertas generadas, deduplicadas, resueltas y entregadas por canales aprobados.
10. Dos instancias con claim exclusivo y politica singleton/HA coherente.
11. Lease vencido recuperado sin duplicar intento/documento.
12. Retry, ventana funcional y DeadLetter observables y acotados.
13. Backup con checksum y restauracion completa probada.
14. Actualizacion y rollback de binario/configuracion/servicio probados.
15. Cero secretos, XML o claves completas en logs, heartbeat, eventos y dumps aprobados.
16. Build Release y suites unitarias/contrato/integracion aprobadas.
17. Runbook ejecutado y firmado por Operaciones, Seguridad, DBA y propietario.

Hasta completar todos los gates, el estado es **No apto para produccion**; una prueba de Iteracion 5 no sustituye estas evidencias.

## Matriz de decisiones aprobadas

| # | Decision aprobada | Aplicacion en esta fase |
|---:|---|---|
| 1 | Piloto Windows x64/Windows Server compatible; sin host productivo declarado | Hosting y plantillas, sin instalacion real. |
| 2 | Cuenta local dedicada `NuanSriWorkerSvc`, sin crearla ni conceder logon | Parametro obligatorio y ACL en plantilla. |
| 3 | Singleton piloto; claims/leases compatibles con futuro multiinstancia | Mutex local, identidad compuesta y alerta por segundo heartbeat activo. |
| 4 | Secrets fuera de release/Git bajo ProgramData; vault futuro | Fuente JSON externa, ACL documentada y `AesSecretProtector` existente. |
| 5 | Evolucion forward-safe de `dbo.WorkerHeartbeat` | `120` corregido y forward repair `122`; compatibilidad SAP preservada. Ejecucion real de la reparacion pendiente. |
| 6 | Baseline de health configurable | API/evaluador con 90/180 s, 10/30 min, 5/20, 30/14 dias y 20/10%. |
| 7 | Logs, Event Log critico, API y WinForms | Implementados sin canales externos. |
| 8 | Soporte en horario laboral, Critical visible | Runbook con roles y objetivos no contractuales. |
| 9 | RPO 15 min/RTO 4 h piloto | Documentado; pendiente de restore real. |
| 10 | Retencion indefinida | Sin purge, archive ni endpoints destructivos. |
| 11 | Certificados propiedad DBA/Infra | Solo observacion/umbrales; sin operaciones de certificado. |
| 12 | `NuanSystem_DEMO`, Production existente, worker disabled, cero SRI | Contrato de futura validacion; no ejecutado en esta fase. |
| 13 | Ventana manual off-hours de 60 minutos | Secuencia freeze/backup/deploy/smoke/observe/rollback documentada. |
| 14 | Artefactos versionados, service stopped, config externa y forward SQL | Plantillas install/start/stop/update/rollback/uninstall; no MSI/pipeline. |

## Affected-layer y quality review de esta fase

- **Validado:** Discovery, ownership SRI, infraestructura existente, separacion SAP/Sync, contratos SQL 115-119 y documentacion de Iteracion 5.
- **Diseno propuesto:** servicio, identidad, heartbeat/health, metricas, alertas, TLS, backup, retencion, despliegue y gates.
- **Decision aprobada existente:** TLS estricto, worker deshabilitado por defecto, tenant XML inmutable sin purga, piloto solo de consulta.
- **Pendiente:** revisar y ejecutar el segundo pase corregido de `120`, ejecutar `122` idempotentemente y luego `121`; despues instalacion/ACL/SCM, runtime, permisos con JWT, Designer visual, certificados y upgrade/rollback.
- **Fuera de alcance:** ejecutar scripts, instalar servicio/certificados/cuentas, levantar procesos, consultar SRI, modificar XML o bases, crear canales externos, purge o HA.
