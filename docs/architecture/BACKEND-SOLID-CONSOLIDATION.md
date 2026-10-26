# Consolidación SOLID del backend — fase 1

Fecha: 2026-10-22. Estado: protecciones de arquitectura y baseline local; no certifica producción.

## Alcance y decisiones

Se conserva el monolito modular con Clean Architecture. La fase 1 solo añade pruebas y documentación; no modifica contratos, lógica comercial, paquetes, SQL, configuración productiva ni procesos de integración. El plan rector es `docs/superpowers/plans/2026-10-22-backend-solid-consolidation.md`.

El checkout ya contiene cambios del usuario sobre `codex/auxiliary-master-generator`, base `e97124a1e2863f066c1fa55ea992c78179f01623`. El baseline debe incluir esos cambios. Se trabaja de forma aditiva en este checkout, sin stash, reset, cambio de rama ni commits del trabajo ajeno. Las huellas SHA-256 de archivos modificados y no rastreados existentes se registran en `artifacts/backend-solid-phase1/preserved-files.json`; se comparan al terminar. No se creó un worktree de HEAD que omitiera esos cambios. Esta decisión limita el aislamiento físico y exige verificar las huellas.

## Discovery Record

- Tipo: infraestructura de pruebas y documentación; no cambia comportamiento de negocio.
- Referencias: instrucciones AGENTS, README, ARCHITECTURE, ARQUITECTURA-COMERCIAL, FASE-2-MULTIEMPRESA, FASE-10-SAP, MASTER-BRANCH-STANDALONE-SAP y SRI-DOCUMENTS-WORKER; Constitución/Kernel y catálogo de transacciones/conexiones revisados en el contexto de esta tarea.
- Reutilización: proyecto xUnit existente, reflection/XML del runtime, registro real de Application/Infrastructure/Persistence/SapIntegration y Generic Host ya disponible. No añade librerías de análisis de arquitectura ni otro contenedor DI.
- Riesgo: ejecución accidental de integración externa o alteración de trabajo ajeno; mitigado mediante filtros explícitos, configuración de prueba en memoria y comprobación de archivos.
- Frescura remota: no validada, revisión del checkout local.

## Matriz de referencias de proyecto permitidas

Los nombres de esta tabla omiten el prefijo `NuanSystem.`. Los proyectos externos que se agreguen a una capa deben revisarse; la prueba no acepta automáticamente una referencia nueva.

| Proyecto | Referencias de proyecto permitidas |
|---|---|
| Domain | Ninguna |
| Shared | Ninguna |
| Application | Domain, Shared |
| Persistence | Application, Domain, Shared |
| Infrastructure | Application, Domain, Shared |
| SapIntegration | Application, Shared |
| Api | Application, Infrastructure, Persistence, SapIntegration, Shared |
| SyncWorker | Application, Infrastructure, Persistence, SapIntegration, Shared |
| MasterBranchSyncWorker | Application, Infrastructure, Persistence, Shared |
| SriWorker | Application, Infrastructure, Persistence, Shared |

API y workers son raíces de composición: pueden referenciar adaptadores para registrar dependencias. Eso no autoriza a Application a consumir implementaciones concretas de esos adaptadores.

Las pruebas combinan referencias de proyecto y paquetes declaradas en los proyectos, referencias de assemblies compilados y tipos reales de parámetros de los constructores de handlers MediatR. Incluyen controles negativos para verificar que las reglas detectan referencias y dependencias prohibidas; no dependen únicamente de buscar cadenas en archivos fuente. La lectura XML no evalúa imports MSBuild ni paquetes transitivos completos; el assembly complementa la inspección para referencias efectivamente utilizadas.

### Deuda explícita y fronteras del gate

- `Domain/Tenancy/Company` aún contiene configuración física y `SapIntegrationMode`: deuda semántica de fase 8. Que no referencie un assembly SAP no resuelve esta deuda.
- `ITransactionRunner` y contratos de repositorio aún exponen ADO.NET: deuda de fase 7, permitida en esta fase para no romper transacciones.
- `ISapSyncEntityHandler` aún exige import/export: deuda de fase 4.
- Application contiene varios módulos dentro de un assembly. El gate protege capas, no acredita aislamiento completo entre módulos ni ausencia de toda dependencia en cuerpos de métodos.
- Las referencias externas usadas mediante reflection/cadenas en tiempo de ejecución requieren revisión adicional; las pruebas no acreditan portabilidad o seguridad de producción.

## Composición DI sin operaciones externas

Construir un Generic Host de pruebas con las extensiones reales y `ValidateOnBuild`/`ValidateScopes`, sin `Host.CreateDefaultBuilder`, appsettings, secretos, variables del entorno operativo, `StartAsync`, API Program ni workers. Instanciar contratos representativos y handlers, no ejecutar casos de uso.

Los límites de IO se sustituyen por fábricas de conexión y HTTP que fallan y cuentan intentos. Los tests verifican que esas barreras funcionan y que la resolución normal no intenta conectarse. La identidad de empresa scoped debe ser estable dentro de un scope, distinta entre scopes y no resolverse desde la raíz. Esto prueba composición y lifetime, no aislamiento real de filas en SQL.

Las barreras cubren `IMasterConnectionFactory`, `ITenantConnectionFactory` e `IHttpClientFactory`. No constituyen una prohibición universal de IO: el factory concreto de Master y el factory HANA conservan registros reales; tampoco interceptan `new HttpClient`, conexiones construidas directamente ni IO arbitrario. Se revisaron los constructores actuales y la configuración queda vacía; no se invocan métodos de negocio. Cero intentos en estos contadores acredita únicamente los límites sustituidos.

## Clasificación de pruebas y ejecución segura

| Grupo | Ejemplos | Ejecución en fase 1 |
|---|---|---|
| Unitarias y contratos estáticos | handlers con NSubstitute, validadores, cálculo, inspección SQL/Designer como texto | Sí, después de revisar accesos externos |
| Integración en memoria | DI/Generic Host sin iniciar, HttpMessageHandler falso | Sí; no equivale a integración con proveedor real |
| SQL externo | `SqlServerIntegrationFact`, categoría `SqlServerIntegration` en MasterBranchSyncConnectionDiagnosticsTests | Excluido |
| Piloto SAP/SQL mutante | `SapPurchaseOrderPilotIntegrationTests`, gate `NUAN_RUN_SAP_PO_PILOT` | Clase completa excluida |

Para cada invocación local se establecen los gates en `0` solo en el proceso hijo y se utiliza el filtro `Category!=SqlServerIntegration&FullyQualifiedName!~SapPurchaseOrderPilotIntegrationTests`. No se cambia configuración global. Las pruebas con transportes simulados pueden instanciar lógica de un worker; no se inicia un worker operativo ni se contacta un servicio.

Los tests externos antiguos que hacen `return` al estar deshabilitados no deben contarse como integración aprobada. La exclusión explícita evita esa interpretación.

## Inventario de consumidores representativos

| Área | Entrada y dependencias | Gate de esta fase |
|---|---|---|
| API | Endpoints → ISender → handlers Application | Dirección de proyectos y constructores |
| Socios | handlers → IBusinessPartnerRepository + publicador/auditoría | Resolución DI; no CRUD real |
| Compras | handlers → IPurchaseOrderRepository + numeración/permisos | Resolución DI; no reserva números |
| SAP | scheduler/preparer/retry → contratos + lectores/repositorios | Resolución de colecciones; no lectura SAP |
| Matriz–Sucursal | Application → contratos; worker compone aplicadores | Dependencias de proyecto; no relay/apply |
| SRI | contratos Application y worker dedicado | Dependencias de proyecto; no procesamiento XML remoto |
| Tenancy | ICompanyContext scoped → resolución/conexión en Persistence | Separación de contextos y scopes |

## Evidencia

El registro de comandos, resultados y baseline se mantiene en `docs/operations/BACKEND-SOLID-PHASE-1-VALIDATION.md`. Los logs y TRX locales quedan en `artifacts/backend-solid-phase1/`, ignorado por Git. El SDK fijado es 10.0.302 con `latestFeature`; el SDK instalado seleccionado durante esta sesión es 10.0.400. No se actualiza el SDK ni se restauran paquetes sin necesidad.
