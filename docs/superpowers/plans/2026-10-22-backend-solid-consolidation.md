# Backend SOLID Consolidation Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers-dev:executing-plans para ejecutar por incrementos revisables. Este documento autoriza preparación documental, no activación de servicios o cambios de datos externos.

**Goal:** Consolidar el monolito modular y Clean Architecture de NuanSystem mediante contratos precisos, errores seguros, reglas puras y pruebas ejecutables de sus fronteras.

**Architecture:** Mantener API → Application → Domain y adaptadores Persistence/Infrastructure/SapIntegration. Aplicar cambios compatibles por vertical, comenzando con proveedores SAP, contratos de sincronización y compras. Conservar transacciones, aislamiento por empresa, auditoría y publicación Outbox.

**Tech Stack:** Versiones .NET, ASP.NET Core, MediatR, FluentValidation, Dapper, SQL Server y xUnit ya fijadas en el repositorio. No instalar herramientas, actualizar paquetes ni introducir microservicios para esta consolidación.

**Spec:** Hallazgos de la revisión estática del backend en esta conversación del 2026-10-22; `docs/ARCHITECTURE.md`, `docs/ARQUITECTURA-COMERCIAL.md`, `docs/architecture/MASTER-BRANCH-STANDALONE-SAP.md`, `docs/architecture/SAP-NUANSYSTEM-BIDIRECTIONAL-FOUNDATION.md`. Coordinar, sin duplicar tareas, con `2026-10-19-sap-sync-quality-hardening.md` de este directorio.

## Global Constraints

- Revalidar hallazgos sobre el checkout al comenzar; existen cambios locales del usuario en API, Application, Persistence y SAP. Preservarlos y trabajar con aislamiento al implementar.
- Antes de cambios transversales, leer las referencias obligatorias de AGENTS.md y registrar decisiones en `docs/architecture/BACKEND-SOLID-CONSOLIDATION.md`.
- SAP sigue opcional. Domain no depende de SAP/SRI, proveedores SQL, Dapper, HTTP ni frontend.
- No cambiar roles, identidad, códigos o unicidad de socios como efecto de separar interfaces.
- No modificar fórmulas, redondeos, tratamiento de líneas o valores históricos al extraer cálculo: caracterizar primero y tratar correcciones como cambios explícitos.
- No introducir repositorios genéricos, interfaces por cada clase o un nuevo framework de sincronización sin necesidad demostrada.
- Migrar contratos consumidos por WinForms con compatibilidad o entrega coordinada; el frontend usa INuanApiClient.
- No hacer llamadas SAP dentro de transacciones SQL. No debilitar atomicidad de datos + auditoría + Outbox.
- Ninguna migración, piloto SAP, worker o despliegue productivo se ejecuta por la elaboración del plan.

## Review Focus

1. Cancelación dentro de un lote: se propaga como cancelación, sin convertirla en error por registro ni éxito global; fase 2.
2. Cambiar de 400 a 404/409/503: clientes existentes conservan lectura de errores y las respuestas no filtran detalles; fase 3.
3. Capacidad configurada sin procesador real o registro DI duplicado: rechazo previo a reservar trabajo; fase 4.
4. Fallo entre guardar datos y publicar Outbox: rollback conjunto o recuperación idempotente según contrato; fase 7.
5. Empresa sin SAP y registros históricos: extracción de modelos no cambia identidades, configuración efectiva ni comportamiento; fases 5, 6 y 8.

## Orden y criterio de ejecución

Fase 1 precede a todas. Fases 2 → 3 cierran errores; fase 4 consolida SAP; fases 5 y 6 son entregas separadas; fase 7 requiere las pruebas de fase 1 y descubrimiento de consumidores; fase 8 va después de inventariar consumidores; fase 9 cierra cada incremento liberado. Dividir cada fase en PRs por vertical cuando afecte múltiples módulos.

Por tarea: escribir prueba de regresión, comprobar el fallo esperado cuando aplique, implementar el mínimo cambio, ejecutar pruebas pertinentes, revisar diff y registrar evidencia. No ejecutar suites con SAP/SQL real sin entorno autorizado.

## Fase 1 — Baseline y protecciones de arquitectura

**Files:** revisar `src/Backend/*/*.csproj`, `tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj`; crear `tests/NuanSystem.Application.Tests/Architecture/BackendDependencyTests.cs` y el ADR indicado arriba. Reutilizar comprobaciones existentes si discovery encuentra equivalentes.

**Interfaces:** no altera contratos de producción; produce matriz de dependencias permitidas, baseline de fallos y reglas verificables.

- [ ] Registrar versiones, árbol de dependencias y cambios previos. Inventariar consumidores por módulo, sin afirmar independencia física que todavía no existe.
- [ ] Registrar qué pruebas son unitarias, de integración aislada y externas; revisar configuración antes de ejecutarlas.
- [ ] Añadir tests de referencias/assemblies: Domain sin proveedores/transportes; Application sin Persistence/SapIntegration/SqlClient/Dapper; implementaciones externas no se inyectan directamente en handlers.
- [ ] Añadir smoke test de composición DI en host de pruebas, sin conexiones reales; validar scopes de los caminos modificados y presencia de implementaciones.
- [ ] Ejecutar build y baseline unitario; registrar fallos previos separadamente. No usar búsquedas textuales como única prueba de comportamiento.

**Cierre:** baseline reproducible, reglas vigentes verificadas y deuda detectada identificada. Una regla nueva incompatible debe tener excepción acotada y un paso de eliminación en este plan.

## Fase 2 — Excepciones seguras y cancelación

**Files:** modificar `src/Backend/NuanSystem.Application/Features/SapSync/Services/SapSupplierImportService.cs`; extender `tests/NuanSystem.Application.Tests/Features/SapSync/SapSupplierImportServiceTests.cs`; revisar `Api/Middleware/GlobalExceptionMiddleware.cs`, infraestructura de logs y contratos ApiError existentes antes de crear otro componente.

**Interfaces:** preservar contrato de importación y estados actuales; error público con código estable y mensaje seguro, detalle técnico solo en log correlacionado. No introducir dependencia de excepción SQL en Application.

- [ ] Test: excepción de persistencia cuyo mensaje contiene un valor sensible no aparece en DTO/respuesta; existe correlación en diagnóstico.
- [ ] Test: OperationCanceledException de la operación se propaga; no agrega resultado Failed ni retorna lote completo exitoso.
- [ ] Aplicar clasificación reutilizando contratos y logging actuales; manejar resultados parciales explícitamente y conservar decisiones de negocio por registro.
- [ ] Comprobar que importación manual y worker interpretan consistentemente fallo parcial y cancelación. HTTP 200 para una operación por lote puede ser válido si su resumen distingue fallos; no cambiarlo incidentalmente.
- [ ] Ejecutar pruebas de servicio, handler SAP y contrato de respuesta afectado.

**Cierre:** cancelación correcta, sin detalles técnicos públicos y con evidencia diagnóstica suficiente.

## Fase 3 — Semántica de errores y compatibilidad HTTP

**Files:** modificar `Application/Common/Models/Result.cs`, `Api/Extensions/ResultExtensions.cs`, handlers migrados y tests; crear `tests/NuanSystem.Application.Tests/Architecture/ResultHttpMappingTests.cs` si no existe equivalente.

**Interfaces propuestas:** agregar categoría `FailureKind` (Validation, NotFound, Conflict, Forbidden, Unavailable, Unexpected) al contrato Application mediante propiedad no posicional para evitar romper constructores/deconstrucciones. Conservar `Failure(string, errors)` como Validation durante transición. Categorías explícitas generan 400/404/409/403/503/500 en API; Unexpected usa mensaje seguro. Success conserva comportamiento existente.

- [ ] Tests de mapeo por categoría y compatibilidad del método Failure existente; no inferir categoría a partir del mensaje en español.
- [ ] Inventariar ApiClientException y consumo WinForms; comprobar parseo de 404/409/503 y forma estable del envelope.
- [ ] Implementar categorías sin introducir HTTP en Application. Mantener 401 en la capa de autenticación y permisos backend efectivos.
- [ ] Migrar primero un vertical de compras y uno SAP; documentar contrato y respuesta parcial de lote antes de ampliar al resto.
- [ ] Ejecutar pruebas del mapper, endpoints y clientes afectados; verificar que un fallo técnico inesperado no se clasifica como validación.

**Cierre:** consumidores distinguen errores sin analizar texto; compatibilidad documentada y validada.

## Fase 4 — Contratos SAP segregados y capacidades coherentes

**Files:** modificar `Application/Abstractions/SapSync/ISapSyncEntityHandler.cs`, `Features/SapSync/Services/SapSyncScheduler.cs`, `SapSyncOrchestrator.cs`, `SapSyncExecutionLeaseCoordinator.cs`, validación de perfiles y `DependencyInjection/ApplicationServiceRegistration.cs`; extender tests Scheduler/Profile/Handler.

**Interfaces propuestas:** `ISapSyncImporter` y `ISapSyncExporter` exponen EntityCode y solo su operación, con las firmas actuales de contexto, CancellationToken y Task<SapSyncExecutionResult>. Mantener contratos específicos de ejecución programada/retry. Un registro efectivo cruza implementación con capacidad configurada; no sustituye configuración por empresa.

- [ ] Test: importador sin exportador no se anuncia bidireccional; handler sin procesador programado no reserva agenda.
- [ ] Test: contratos duplicados o incompatibles se detectan al componer servicios; el mismo contrato ejecutable se usa en validación y despacho.
- [ ] Migrar consumidores por entidad; conservar adaptador temporal solo si tiene consumidores identificados y fecha/paso de retirada.
- [ ] Probar cancelación, estados y resultados válidos de cada implementación registrada mediante pruebas de contrato compartidas.
- [ ] Actualizar capacidades/documentación y coordinar con tarea equivalente del plan SAP del 2026-10-19 para implementarla una sola vez.

**Cierre:** ningún consumidor debe aceptar operaciones que una implementación solo simula con NotImplemented.

## Fase 5 — Contratos de socios por necesidad del consumidor

**Files:** modificar `Application/Abstractions/Data/IBusinessPartnerRepository.cs`, `Persistence/Repositories/BusinessPartnerRepository.cs`, importador SAP, handlers de socios y registro DI; extender pruebas BusinessPartnerSyncPublishing y SapSupplierImportService.

**Interfaces propuestas:** extraer `ISapSupplierImportRepository.ImportSupplierFromSapAsync(BusinessPartnerSapImportData, CancellationToken)` con el retorno actual. Mantener implementación transaccional y SQL vigentes. Dividir consultas/escrituras solo si el mapa de consumidores muestra beneficio; no prescribir una interfaz por método.

- [ ] Mapear consumidores CRUD, lookups, sincronización e importación; verificar interfaces existentes antes de agregar nombres.
- [ ] Test: alta/edición/baja e importación mantienen identidades, resultados, auditoría y eventos esperados.
- [ ] Extraer contrato SAP e inyectarlo solo donde se necesita; una misma implementación puede satisfacer contratos segregados.
- [ ] Revisar igualdad de scopes y transacción al registrar varias interfaces; evitar instancias que pierdan una sesión compartida.
- [ ] Ejecutar regresión de socios y publicación; ninguna migración de identidad ni SQL funcional forma parte de esta extracción.

**Cierre:** CRUD de socios no depende de métodos de integración SAP y el comportamiento observable permanece estable.

## Fase 6 — Reglas de compras independientes de persistencia y SAP

**Files:** modificar `Application/Features/Purchasing/PurchaseOrders/PurchaseOrderCalculator.cs`, DTOs y handlers implicados; crear tipos puros bajo `Domain/Purchasing` según descubrimiento; crear tests de cálculo en `tests/NuanSystem.Application.Tests/Features/Purchasing/PurchaseOrders`.

**Interfaces:** calculador puro recibe valores de líneas/descuento y devuelve importes y reglas de redondeo explícitas, sin DTO de Persistence ni estado SAP. Application transforma su resultado a PurchaseOrderPersistData. Elegir nombres/signaturas finales en el ADR después de caracterizar entradas actuales; evitar crear un agregado completo sin necesidad.

- [ ] Caracterizar resultados actuales: descuentos de línea/cabecera, base imponible, líneas vacías, impuesto cero, precisión, moneda y límites; conservar resultados durante la extracción.
- [ ] Extraer cálculo y valores puros; mantener orquestación, permisos y consultas en Application.
- [ ] Trasladar código objeto SAP 22 a mapeo de integración, verificando persistencia histórica y consumidores.
- [ ] Tratar OC-2026/OC-000001 y defaults como decisión separada: comprobar caminos alcanzables, usar numeración/configuración autorizada y rechazar ausencia cuando corresponda. No renumerar documentos históricos.
- [ ] Si se decide corregir truncamientos, normalización o reglas existentes, crear tests y entrega separados de la extracción para hacer visible el cambio funcional.
- [ ] Ejecutar cálculo, workflow, numeración y contratos de persistencia.

**Cierre:** cálculo sin SQL/SAP/HTTP, sin cambios monetarios accidentales y documentos históricos intactos.

## Fase 7 — Encapsular la sesión transaccional

**Files:** revisar `Application/Abstractions/Data/ITransactionRunner.cs`, variantes transaccionales de repositorios, `Persistence/Transactions/SqlTransactionRunner.cs`, conexiones y publicadores Outbox. Modificar primero un solo vertical, preferentemente socios con publicación transaccional ya comprobada.

**Interfaces objetivo:** Application declara una operación transaccional sin IDbConnection/IDbTransaction; Persistence mantiene conexión y transacción compartidas en el scope de la operación. Antes de crear tipos, comprobar si sesión/unidad de trabajo equivalente ya existe. Mantener explícitas transacciones anidadas, rollback y ámbito tenant.

- [ ] Inventariar todos los consumidores y establecer pruebas SQL aisladas antes de cambiar el contrato.
- [ ] Test: fallo en auditoría o Outbox revierte el dato comercial; repetición no duplica eventos ni identidad.
- [ ] Test: repositorios participantes usan la misma transacción/tenant; scope de otra empresa no puede reutilizarla; cancelación y fallo de rollback conservan diagnóstico original.
- [ ] Implementar el piloto con compatibilidad temporal. Si no demuestra menor acoplamiento sin debilitar seguridad/atomicidad, conservar contrato actual y registrar decisión.
- [ ] Migrar los demás consumidores por vertical; retirar sobrecargas ADO.NET solo cuando no queden usos.

**Cierre:** transacciones comprobadas y menor exposición técnica; no hay llamadas externas dentro de la transacción ni dependencia de una transacción distribuida.

## Fase 8 — Separar empresa de configuración técnica

**Files:** modificar `Domain/Tenancy/Company.cs` y consumidores confirmados; revisar `Application/Abstractions/Tenancy`, contratos de configuración, resolver y repositorios Master. Reutilizar CompanyConnectionInfo/configuración existente si corresponde.

**Interfaces:** modelo empresarial sin credenciales ni datos físicos de conexión; configuración técnica no pública para resolución de empresa y adaptadores. La forma de tablas Master no tiene que cambiar para separar modelos.

- [ ] Inventariar serialización API, resolución tenant, configuración SAP y cifrado; crear pruebas de compatibilidad y ausencia de secretos en respuestas.
- [ ] Extraer servidor, base, usuario, contraseña cifrada y selección técnica SAP del modelo Domain hacia los contratos propietarios adecuados.
- [ ] Preservar autorización por empresa, cifrado, códigos y configuración efectiva. No prometer portabilidad MySQL o DI API multiplataforma por separar tipos.
- [ ] Probar modo sin SAP y tenants con el mismo código externo; extender regla arquitectónica de fase 1 para impedir reintroducción de detalles.

**Cierre:** Domain conserva conceptos empresariales, resolución/configuración funcionan igual y no se amplía exposición de secretos.

## Fase 9 — Validación integrada y liberación gradual

**Files:** actualizar ADR, `docs/ARCHITECTURE.md`, catálogos/grafo aplicables y crear `docs/operations/BACKEND-SOLID-VALIDATION.md` con evidencia saneada.

- [ ] Ejecutar suites dirigidas por fase y suite completa autorizada; separar tests externos de pruebas locales.
- [ ] Ejecutar `dotnet build NuanSystem.sln -c Release --no-restore` cuando existan assets válidos; si faltan, registrar bloqueo y acordar restauración de dependencias sin instalar herramientas globales.
- [ ] Probar API/cliente real en ensayo: errores, permisos, tenant, cancelación, importación y operación de compras.
- [ ] Para cambios SQL, validar scripts idempotentes y compatibilidad con binarios anteriores; si no hay migración, no agregar una innecesaria.
- [ ] Liberar por incrementos con binarios/contratos identificados; prever rollback de código compatible sin borrar colas ni revertir documentos de negocio.
- [ ] Retirar adaptadores y excepciones temporales; revisar reglas de dependencias y documentación contra estado final.

**Cierre:** cambios relevantes verificados, consumidores compatibles, sin capacidades pendientes presentadas como completas y con recuperación documentada.

## Mapa SOLID y atributos

| Principio | Aplicación | Evidencia de cierre |
|---|---|---|
| S | separar cálculo, mapeo, errores y configuración | fases 2, 3, 6, 8 con pruebas focalizadas |
| O | agregar implementaciones por contratos/capacidades | fase 4 sin modificar el scheduler por cada entidad |
| L | comportamientos sustituibles, cancelación y resultados coherentes | contratos ejecutados contra cada procesador en fase 4 |
| I | import/export y contrato de importación de socios separados | fases 4–5, consumidores sin métodos irrelevantes |
| D | Application/Domain independientes de adaptadores | fases 1, 7–8 y reglas de referencias |

SOLID no acredita por sí solo disponibilidad, rendimiento o seguridad. Carga, failover, ejecución durable, alertas y recuperación operacional se completan con el plan SAP y el despliegue correspondiente. No asignar porcentajes de cumplimiento ni SLA sin medición.

## Estado y control del plan

Entrega documental basada en revisión estática representativa, no auditoría exhaustiva. No se ejecutaron tests/build/SQL/SAP ni se modificó código productivo al redactarlo. Reestimar cada fase después del baseline: priorizar 1–4; 7 es la de mayor impacto transversal. Las nuevas firmas señaladas como objetivo se concretan en el ADR antes de implementación; no se presentan como componentes existentes.


## Actualizacion de ejecucion — 2026-10-24

El estado documental inicial anterior es historico. Las fases 1–6 y 8 tienen implementacion y evidencia local por fase. La fase 7 entrega un piloto BusinessPartners compatible; validacion SQL y migracion de otros verticales siguen pendientes. La fase 9 integra comprobaciones locales, ensayo HTTP sin sockets, catalogos/grafo y un procedimiento de liberacion/recuperacion; no ejecuta SAP/DB ni despliega.

El cierre local no equivale a liberacion: permanecen los tres fallos visuales WinForms previos, el gate SQL del piloto y ensayos reales de JWT/transporte/produccion. No se retira compatibilidad necesaria ni se habilitan capacidades no implementadas. Estado consolidado: docs/architecture/BACKEND-SOLID-STATUS.md. Evidencia actual: docs/operations/BACKEND-SOLID-VALIDATION.md.
