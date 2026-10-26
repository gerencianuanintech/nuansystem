# SAP Sync Quality Hardening Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans para implementar este plan tarea por tarea. No activar servicios ni ejecutar pilotos externos como parte de la preparación documental.

**Goal:** Consolidar SAP → NuanSystem en un circuito durable, idempotente, recuperable y observable, con contratos SOLID y activación gradual por empresa y entidad.

**Architecture:** Conservar Clean Architecture, los adaptadores SAP, los repositorios y los workers existentes. Separar capacidades de importación, exportación, programación y recuperación; persistir el despacho antes de consumir una agenda y ejecutar trabajo reclamado mediante leases. La distribución Matriz–Sucursal mantiene su propio Outbox/Inbox y worker.

**Tech Stack:** C#, .NET y dependencias de las versiones fijadas por el repositorio, ASP.NET Core, MediatR, Dapper, SQL Server, WinForms/DevExpress, Service Layer y lectores HANA existentes. No introducir un broker ni nuevas dependencias de resiliencia sin justificar la necesidad y comprobar compatibilidad al ejecutar.

**Spec:** Requisitos de calidad y SOLID acordados en esta conversación; `docs/ARCHITECTURE.md`, `docs/architecture/MASTER-BRANCH-STANDALONE-SAP.md`, `docs/architecture/SAP-SYNC-PROFILES-BLUEPRINT.md` y `docs/architecture/SAP-NUANSYSTEM-BIDIRECTIONAL-FOUNDATION.md`. Este último contiene decisiones objetivo, no prueba de implementación.

## Global Constraints

- SAP opcional por empresa; Domain independiente de SAP, SQL, SRI y frontend.
- WinForms solo consume la API mediante el cliente centralizado.
- Master de gobierno contiene configuración; el tenant central contiene operación comercial.
- No abrir una transacción distribuida Master/tenant/SAP ni mantener SQL abierto durante llamadas SAP.
- Reutilizar ejecución, detalles, locks, snapshots y recuperación existentes; no crear otro motor ni otra cola SAP de salida.
- Toda nueva capacidad queda inactiva hasta superar su piloto. Preservar configuraciones activas existentes durante la migración.
- Un reintento no duplica entidades ni eventos. No prometer entrega exactamente una vez: asegurar aplicación idempotente con posibles reentregas.
- No modificar identidad de socios, reglas de propiedad, stock, contabilidad, SRI o distribución interna incidentalmente.
- Salida ERP → SAP y BranchToMaster quedan fuera de este plan; conservan su planificación independiente.
- No correr migraciones, consultas SAP, workers ni despliegues reales durante la elaboración del plan.
- Antes de implementar, releer instrucciones aplicables, inventariar cambios del usuario y crear aislamiento de trabajo. No revertir cambios ajenos.

## Review Focus

1. Caída después de reservar agenda y antes de crear ejecución: el trabajo permanece durable y recuperable; tarea 3.
2. Lease vencido mientras el propietario antiguo sigue trabajando: ninguna confirmación tardía puede sobrescribir al propietario vigente; tareas 3 y 4.
3. Dos empresas con igual clave SAP: sesiones, circuitos, filas y permisos permanecen aislados; tareas 4, 6 y 8.
4. Varias filas con la misma marca temporal y fallo parcial: no se salta ninguna fila al avanzar el checkpoint; tarea 5.
5. Reinicio después del commit local y antes de confirmar el detalle: la repetición no crea otra entidad ni otro evento; tareas 4 y 5.

## Discovery Record

- Outcome: plan de endurecimiento, sin implementación operativa.
- Work type: integración/sincronización/worker, riesgo alto al ejecutar; entrega actual documental.
- Evidence: `SapSyncScheduler`, `SapSyncScheduledExecutionPreparer`, `SapSyncExecutionLeaseCoordinator`, `SapSyncExecutionRetryService`, `SapSyncRetryPolicy`, `SapWarehouseExecutionProcessor`, registro Application y transporte SAP.
- Selected pattern: ejecución y recuperación de Bodegas como referencia de ciclo de vida; no reutilizar su modelo de negocio para otras entidades.
- Existing assets: contratos Application, reloj `ISystemClock`, repositorios SQL, locks renovables, snapshots, logs, heartbeat, pantallas de ejecución.
- Confirmed gaps: scheduler valida handlers pero ejecutor usa procesadores distintos; interfaz obliga import/export; retry por coincidencia de textos; cobertura de procesadores programados/retry limitada a Warehouses/Countries/Provinces/Cities.
- Investigate before changing: atomicidad exacta de reserva SQL, publicación transaccional, fencing de escrituras y recuperación tras fallo. Son riesgos por comprobar, no defectos confirmados en todos los caminos.
- Transport already has configurable timeout and session/TLS safeguards: extenderlos, no duplicarlos.
- Remote freshness: no validada; revisión del checkout local únicamente.

## Entregas y dependencias

1 → 2 → 3 → 4 → 5 → piloto Bodegas. Tarea 6 después de 4; tarea 7 después de 5 y 6; tareas 8 y 9 cierran la operación. Cada tarea entrega un incremento verificable. Las tareas 5/7 se subdividen por entidad al ejecutarlas: no aprobar un lote de entidades sin gates individuales.

### Tarea 1: Fijar decisiones y objetivos verificables

**Files:** crear `docs/architecture/SAP-SYNC-QUALITY-HARDENING.md`; ampliar este plan con referencias al ADR aprobado.

**Interfaces:** produce decisiones sobre ubicación del despacho durable, política de recuperación y objetivos de operación. No cambia contratos públicos.

- [ ] Registrar inventario por entidad: lector, importador, programador, retry, modo Full/Incremental, propietario de datos y evidencia real.
- [ ] Revisar SQL `049`, `050`, `152`, `153`, `155`, `158` y migraciones posteriores efectivamente aplicables; identificar procedimientos que reservan agendas y reclaman trabajo.
- [ ] Documentar despacho durable en Master de gobierno, creado en la misma transacción que consume el vencimiento de agenda; ejecución funcional permanece en tenant. Usar objetos existentes si satisfacen el contrato; ampliar esquema solo si falta esa responsabilidad.
- [ ] Definir una clave estable del despacho por agenda, vencimiento UTC y revisión de configuración, además de `ExecutionUid` estable entre reentregas.
- [ ] Acordar dataset y objetivos: empresas simultáneas, filas por entidad, retraso máximo, ventana de recuperación, retención y RPO/RTO de restauración. Hasta acordarlos, pruebas de capacidad son caracterización y no cumplimiento de SLA.
- [ ] Revisar aprobación funcional previa de socios y prohibir consolidar supuestos legacy contrarios a la fundación bidireccional.

**Aceptación:** ADR trazable y matriz de capacidades revisados; objetivos medibles establecidos antes de certificar capacidad o disponibilidad.

### Tarea 2: Segregar contratos y alinear capacidades

**Files:** modificar `src/Backend/NuanSystem.Application/Abstractions/SapSync/ISapSyncEntityHandler.cs`, `Features/SapSync/Services/SapSyncScheduler.cs`, `Services/SapSyncOrchestrator.cs`, `Profiles/Services/SapSyncProfileValidationService.cs` y `DependencyInjection/ApplicationServiceRegistration.cs`; crear interfaces import/export y catálogo de capacidades dentro de `Abstractions/SapSync`/`Features/SapSync` si discovery confirma que no existen equivalentes.

**Interfaces propuestas:** `ISapSyncImporter.ImportFromSapAsync(SapSyncExecutionContext, CancellationToken)`, `ISapSyncExporter.ExportToSapAsync(SapSyncExecutionContext, CancellationToken)` retornan `Task<SapSyncExecutionResult>` y exponen `EntityCode`. `ISapSyncCapabilityRegistry.Supports(string entityCode, SapSyncDirection direction, string syncMode, bool scheduled, bool retry)` retorna `bool`; la implementación cruza capacidad configurada con contratos ejecutables, sin convertir la configuración SQL en autoridad sobre código inexistente.

- [ ] Añadir tests a `tests/NuanSystem.Application.Tests/Features/SapSync/SapSyncSchedulerTests.cs`: handler sin procesador programado es rechazado antes de reservar agenda; entidad sin exportador no ofrece salida; modos no soportados no se anuncian.
- [ ] Ejecutar el filtro correspondiente y confirmar que las nuevas aserciones fallan por la brecha prevista.
- [ ] Separar import/export, migrar consumidores y preservar compatibilidad transitoria explícita si hace falta. Registrar duplicados de procesador como error de composición.
- [ ] Hacer que validación del perfil, scheduler y ejecución utilicen la misma definición efectiva de capacidades.
- [ ] Ejecutar pruebas Scheduler, Profile, Handler y compilación afectada; registrar resultados y revisar el diff.

**Aceptación:** una entidad solo puede programarse o exponerse cuando existe la implementación necesaria; ningún importador necesita simular exportación.

### Tarea 3: Despacho durable y recuperación de agenda

**Files:** modificar `Abstractions/SapSync/ISapSyncScheduleRepository.cs`, `Features/SapSync/Scheduling/SapSyncSchedulingContracts.cs`, `Features/SapSync/Services/SapSyncScheduler.cs`, `Persistence/Repositories/SapSync/SapSyncScheduleRepository.cs`, `SyncWorker/Workers/SapSyncWorker.cs`; migraciones Master nuevas en `database/sql` con numeración libre verificada al implementar.

**Interfaces:** reemplazar de forma compatible la reserva booleana por una operación de reserva + registro de despacho que retorna el contexto durable o resultado de concurrencia. Añadir operaciones de claim/ack/release al repositorio propietario del despacho. Firmas y columnas finales se fijan en el ADR de tarea 1 antes de editar SQL.

- [ ] Añadir pruebas de integración SQL: dos reservas concurrentes generan un despacho; rollback no consume agenda; caída posterior al commit deja el despacho pendiente; reinicio usa el mismo `ExecutionUid`.
- [ ] Confirmar fallos relevantes en un SQL de pruebas aislado, sin SAP.
- [ ] Persistir reserva y despacho en una transacción Master. Guardar el contexto autorizado de empresa/perfil/entidad y no secretos.
- [ ] Reclamar despacho con lease; resolver tenant; crear/recuperar ejecución idempotente; confirmar despacho solo cuando el estado durable permite continuar o termina de forma explícita.
- [ ] Conservar y reintentar fallos de resolución tenant sin perder trabajo; limitar intentos y exponer estado terminal.
- [ ] Verificar reinicio después de reserva, después de crear ejecución y después del commit tenant antes del ack Master; repetir migración y comprobar unicidad.

**Aceptación:** ningún trabajo aceptado desaparece ante caída del proceso; no hay transacción cruzada entre bases.

### Tarea 4: Errores tipados, retry y propiedad de ejecución

**Files:** modificar `Features/SapSync/Services/SapSyncRetryPolicy.cs`, `Executions/SapSyncExecutionRetryService.cs`, `Services/SapSyncExecutionLeaseCoordinator.cs`, contratos y repositorios de ejecución/lock; ampliar transportes en `SapIntegration/ServiceLayer` y `SapIntegration/Hana` según puntos de error reales. Crear pruebas `SapSyncFailureClassificationTests.cs` y `SapSyncRecoveryConcurrencyTests.cs` en el directorio de tests SAP.

**Interfaces propuestas:** contrato Application `SapSyncFailure` con código estable, clase y retraso remoto opcional; clasificador convierte detalles externos a ese contrato. La política calcula retry/dead-letter con `ISystemClock` y fuente de jitter inyectable; no expone excepciones del proveedor a WinForms.

- [ ] Tests: timeout/429/503 transitorios; autenticación requiere corrección; validación/conflicto no se reintentan automáticamente; texto que contiene HANA no convierte un error permanente en transitorio.
- [ ] Tests: cancelación solicitada no consume un intento como error; max attempts exacto; backoff acotado con jitter verificable y `Retry-After` válido respetado por el presupuesto.
- [ ] Implementar clasificación tipada y compatibilidad temporal con códigos existentes; eliminar dependencia del texto para las rutas migradas.
- [ ] Hacer cumplir token de propietario/fencing en cada escritura de control reclamada; propagar cancelación por pérdida de lease y comprobar idempotencia/versiones en la mutación comercial.
- [ ] Probar propietario vencido frente a nuevo propietario, recuperación de locks y shutdown durante SAP/SQL, incluyendo fallos del almacenamiento de resultados.
- [ ] Ejecutar pruebas de retry, lease y procesadores existentes; registrar resultados.

**Aceptación:** errores terminales no entran en bucles; el trabajador antiguo no confirma trabajo del nuevo; el fallo de registrar resultado nunca se presenta como éxito.

### Tarea 5: Piloto endurecido Bodegas e integridad de checkpoints

**Files:** modificar `Features/SapSync/Warehouses/Services/SapWarehouseExecutionProcessor.cs`, `SapWarehouseRecordProcessor.cs`, repositorios/procedimientos asociados y pruebas `Features/SapSync/Warehouses`; revisar `Services/SapSyncWatermarkService.cs` y rutas incrementales existentes sin habilitar modos nuevos por defecto.

**Interfaces:** preservar `ISapSyncScheduledExecutionProcessor.ProcessAsync(SapSyncScheduledExecutionContext, CancellationToken)` y `ISapSyncExecutionRetryProcessor`; extender contratos de persistencia únicamente para cerrar transacciones/idempotencia comprobadas.

- [ ] Tests de dos ciclos Full sin cambios: identidad estable, cero duplicados y cero eventos adicionales donde el contrato no exige reconciliación.
- [ ] Tests de caída después del commit comercial antes de registrar detalle: reentrega converge y no duplica eventos; definir claves de publicación con semántica de negocio, no un GUID nuevo por intento.
- [ ] Verificar que guardado de datos y publicación del evento pertenecen a la misma transacción local donde corresponda. Mantener llamadas SAP fuera de SQL.
- [ ] Añadir tests del algoritmo incremental: fechas iguales, paginación, fallo intermedio y reinicio; usar cursor compuesto o ventana solapada idempotente según lector. No avanzar checkpoint por encima de registros que quedarían inaccesibles para recuperación.
- [ ] Limitar memoria y evitar confundir `Chunk` sobre una colección ya cargada con lectura streaming. Preservar capacidades reales de cada API SAP.
- [ ] Ejecutar regresión Bodegas, snapshots, SQL contracts y publicación Matriz–Sucursal sin activar su worker.

**Aceptación:** Bodegas supera interrupción/reinicio/repetición en entorno aislado; se documentan resultados antes de extender la base a otras entidades.

### Tarea 6: Aislamiento de fallos y concurrencia acotada

**Files:** modificar `SapIntegration/DependencyInjection/SapIntegrationServiceRegistration.cs`, clientes/lectores afectados, `SyncWorker/Options/WorkerOptions.cs`, opciones SAP y los tres workers según su comportamiento real. Crear tests de transporte/concurrencia dentro del proyecto de pruebas existente.

**Interfaces:** preservar lectores Application; configurar límites por empresa y globales. No agregar un wrapper HTTP paralelo; extender el cliente registrado y el límite de ejecución existente.

- [ ] Tests: una empresa en timeout no bloquea el progreso de otra; dos empresas con el mismo código SAP no comparten sesiones, filas ni estado de circuito.
- [ ] Establecer un único presupuesto de retry: las lecturas pueden tener retry de transporte acotado; el trabajo durable gestiona recuperación larga. No reintentar escrituras remotas ambiguas sin reconciliación.
- [ ] Añadir circuit breaker por contexto adecuado, límite de llamadas simultáneas y timeout total, conservando TLS y el timeout HTTP existente.
- [ ] Implementar reparto justo y concurrencia real acotada; ninguna opción llamada paralela puede limitarse a truncar indefinidamente la lista de empresas.
- [ ] Tests con dos instancias y más empresas que el límite; todas progresan, la concurrencia no excede el límite y el mismo trabajo no se confirma dos veces.

**Aceptación:** aislamiento entre empresas, ausencia de inanición y límites observados en ejecución. No se acredita escalamiento horizontal hasta superar las pruebas multiinstancia.

### Tarea 7: Incorporar entidades por entregas verticales

**Files:** `Features/SapSync/{Countries,Provinces,Cities}`, `Handlers`, `Services`, lectores y repositorios de cada entidad; crear procesadores de ejecución/retry faltantes siguiendo el patrón probado; actualizar registro Application, capacidades SQL y tests por entidad.

**Interfaces:** cada vertical implementa solo los contratos habilitados por la tarea 2; snapshots versionados y lectura Full/Incremental según capacidad demostrada.

- [ ] Migrar Países y Provincias al endurecimiento común y verificar orden/dependencias.
- [ ] Mantener Ciudades bloqueada para el caso provincial `90`; la corrección del origen o correspondencia requiere una decisión funcional separada, nunca inventar una provincia.
- [ ] Completar Artículos y Condiciones de pago con historial, retry, idempotencia y pruebas independientes.
- [ ] Para Proveedores, verificar primero identidad/propiedad/reconciliación aprobadas en la fundación bidireccional; cualquier migración necesaria es un prerrequisito separado. No automatizar supuestos legacy incompatibles.
- [ ] Completar Ordenes de compra solamente después de sus dependencias; preservar routing, aprobaciones y exclusión de stock/pagos/SRI.
- [ ] Para cada entidad: escribir fallo esperado, implementar incremento, ejecutar tests y piloto aislado, revisar evidencia y habilitar capacidad solo al cerrar su gate.

**Aceptación:** matriz de capacidades coincide con código, SQL, UI y evidencia; cada entidad tiene aprobación independiente.

### Tarea 8: Observabilidad, seguridad y recuperación de operador

**Files:** ampliar logs/heartbeat existentes en `SyncWorker`, `Features/SapSync/Executions`, `Api/Endpoints/SapSyncExecutionEndpoints.cs`, clientes SAP frontend y `WinForms.Forms/Sap/SapSyncExecution*`; crear `docs/operations/SAP-SYNC-RECOVERY-RUNBOOK.md` y configuración de métricas/trazas en Infrastructure si no existe componente reutilizable.

**Interfaces:** mantener contratos actuales; agregar campos compatibles para error estable, despacho/ejecución/correlación y recuperación permitida. Toda acción mutante requiere autorización backend y auditoría, no solo un botón oculto.

- [ ] Tests de acceso a empresa ajena y acciones sin permiso; credenciales/cookies/payloads sensibles ausentes de logs y respuestas.
- [ ] Instrumentar pendientes, edad del pendiente más antiguo, lag de datos, duración, throughput, retry, dead-letter y leases perdidos. Evitar etiquetas de métrica por registro o ejecución de alta cardinalidad.
- [ ] Separar heartbeat de proceso de éxito funcional de la entidad; alertar sobre retraso, fallos terminales y worker sin actividad esperada según objetivos acordados.
- [ ] Mostrar estados comprensibles, motivo y acción permitida; reintento manual con motivo, sin permitir reescribir arbitrariamente snapshots.
- [ ] Establecer retención y limpieza por lotes con protección de trabajo pendiente, auditoría necesaria y política de datos sensibles.
- [ ] Verificar trazabilidad de una ejecución desde agenda hasta registro, retry y resultado; comprobar que una alerta llega al canal operativo configurado en el piloto autorizado.

**Aceptación:** el operador identifica el fallo y recupera trabajo sin editar tablas; permisos e historial prueban quién hizo qué.

### Tarea 9: Validación operacional y despliegue gradual

**Files:** crear `docs/operations/SAP-SYNC-QUALITY-VALIDATION.md`; actualizar runbooks SAP y documentación de despliegue. Registrar migraciones y binarios exactos usados.

- [ ] Ejecutar baseline y pruebas dirigidas: `dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj --filter FullyQualifiedName~Sap`.
- [ ] Ejecutar suite completa y build Release de la solución identificada en el checkout; un fallo previo se documenta, nunca se presenta como aprobado.
- [ ] Ejecutar SQL aislado e idempotencia de migraciones; probar reinicio de API/worker, caída de SAP, caída tenant, caída Master y expiración de lease.
- [ ] Ejecutar carga con el dataset y objetivos acordados en tarea 1; registrar p95, lag, throughput, memoria y conexiones. No inventar cifras de capacidad.
- [ ] Ensayar restauración de respaldo y reconciliación. Distinguir cero pérdidas por caída del proceso de posibles pérdidas por restauración según RPO acordado.
- [ ] Preparar migraciones compatibles y estrategia de rollback: deshabilitar nueva reclamación, drenar o preservar pendientes y volver a binarios compatibles; no borrar colas ni deshacer datos de negocio automáticamente.
- [ ] Piloto Bodegas en una empresa autorizada, seguido de oleadas por entidad/empresa. No habilitar globalmente por superar una sola entidad.
- [ ] Actualizar catálogo, grafo, capacidades, evidencia saneada y manual del operador; revisar retención, supervisión y soporte.

**Aceptación:** pruebas con evidencia, objetivos cumplidos, recuperación ensayada, alertas verificadas y liberación gradual aprobada. Ningún proceso de producción se activa por elaborar o aprobar el documento de plan.

## Atributos y límites de la entrega

- SOLID/mantenibilidad/extensibilidad: tarea 2 y contratos por entidad, sin framework genérico innecesario.
- Resiliencia/recuperabilidad/confiabilidad: tareas 3–6 y pruebas de fallo.
- Escalabilidad/eficiencia: tarea 6 y medición tarea 9.
- Disponibilidad: supervisión y recuperación verificadas; redundancia API/SQL y failover son un subproyecto de despliegue si el objetivo acordado lo requiere.
- Seguridad/auditoría/observabilidad/usabilidad: tarea 8 y gates por entidad.
- Flexibilidad/interoperabilidad: capacidades y contratos de adaptadores; no anunciar soportes SAP que no se prueban.
- Portabilidad: preservar fronteras; este plan no migra WinForms a otra plataforma, SQL Server a otro motor ni DI API fuera de Windows.
- Sostenibilidad: migraciones compatibles, regresión, retención y runbooks; revisar dependencias en cada actualización.

## Estado de esta entrega

Plan redactado y revisado contra evidencia local. No se modificó código de aplicación, no se ejecutaron tests, SQL, SAP ni workers. Firmas señaladas como propuestas son nuevas; las firmas de reserva/claim se concretan en el ADR previo a su implementación. No hay estimación de calendario hasta cerrar la tarea 1 y dimensionar los verticales de la tarea 7.
