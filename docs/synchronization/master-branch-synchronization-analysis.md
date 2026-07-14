# Auditoria y diseno tecnico: configuracion de sincronizacion Maestro-Sucursal

## 1. Arquitectura actual identificada

NuanSystem esta organizado como una solucion modular con backend .NET, API REST, cliente WinForms DevExpress, scripts SQL manuales y workers separados. La arquitectura real encontrada coincide en sus capas principales con `README.md` y `docs/ARCHITECTURE.md`:

- `src/Backend/NuanSystem.Api`: entrada HTTP, middleware, autenticacion/autorizacion, endpoints minimal API y composicion en `Program.cs`.
- `src/Backend/NuanSystem.Application`: CQRS con MediatR, commands, queries, handlers, validators, DTOs, contratos de repositorio y servicios de aplicacion.
- `src/Backend/NuanSystem.Domain`: modelo de dominio liviano, especialmente tenancy en `Domain/Tenancy`.
- `src/Backend/NuanSystem.Persistence`: Dapper, fabricas de conexion, repositorios, inicializadores y acceso a Master/Tenant.
- `src/Backend/NuanSystem.Infrastructure`: servicios transversales como seguridad/cifrado.
- `src/Backend/NuanSystem.SapIntegration`: integracion SAP aislada y opcional.
- `src/Backend/NuanSystem.Shared`: constantes, respuestas compartidas, contratos de autenticacion y enums compartidos de Sync.
- `src/Backend/NuanSystem.MasterBranchSyncWorker`: worker dedicado a sincronizacion Master/Sucursal.
- `src/Backend/NuanSystem.SyncWorker`: workers SAP, separado de Master/Sucursal.
- `src/Frontend/NuanSystem.WinForms*`: aplicacion WinForms, formularios, ViewModels, clientes HTTP y controles.
- `database/sql`: scripts SQL versionados manuales. No existe `database/sqlserver`.
- `tests/NuanSystem.Application.Tests`: pruebas de Application y contratos de Sync/Worker.

La persistencia real no usa EF Core ni migraciones de EF. No se encontraron `DbContext`, `Migration`, `IEntityTypeConfiguration` ni paquetes EF en el barrido. El patron vigente es Dapper + stored procedures para CRUD administrativo y SQL inline acotado para infraestructura Sync/monitoring.

## 2. Proyectos involucrados

Para una implementacion futura del modulo administrativo de configuracion Master-Sucursal, los proyectos involucrados serian:

- `NuanSystem.Domain`: tipos puros si se requiere modelar invariantes estables de configuracion, sin SQL, SAP, SRI ni WinForms.
- `NuanSystem.Application`: DTOs, commands, queries, validators, contratos de repositorio y servicios de validacion de configuracion.
- `NuanSystem.Persistence`: repositorios Dapper contra Master, stored procedures/scripts en `database/sql`, evaluacion de reglas y lectura de empresas/sucursales.
- `NuanSystem.Api`: endpoints minimal API del modulo de configuracion, protegidos por permisos/form operations.
- `NuanSystem.Shared`: nuevos enums/constantes compartidas solo si deben ser consumidos por API y WinForms.
- `NuanSystem.WinForms.Services`: cliente HTTP del modulo.
- `NuanSystem.WinForms.ViewModels`: estado de listado/edicion/configuracion.
- `NuanSystem.WinForms.Forms`: pantallas DevExpress administrativas.
- `NuanSystem.MasterBranchSyncWorker`: consumo posterior de la configuracion para schedules/checkpoints/ejecucion. No debe recibir logica de formulario.
- `database/sql`: scripts master/tenant y seed de seguridad.
- `tests/NuanSystem.Application.Tests`: pruebas de contratos, validaciones y comportamiento de configuracion.

## 3. Entidades existentes reutilizables

### Tenancy, empresa y sucursal

La entidad real de empresa es `NuanSystem.Domain/Tenancy/Company.cs`. Incluye:

- `Id`
- `Code`
- `CommercialName`
- datos de conexion tenant
- `SapIntegrationMode`
- `OperationMode`
- `IsMaster`
- `ParentCompanyId`
- `BranchCode`
- `SyncEnabled`

El contexto activo real es `ICompanyContext` + `CompanyConnectionInfo`, donde el identificador confiable es `CompanyId` y el codigo enviado desde WinForms es `X-Company-Code`. La resolucion se implementa en `SqlServerCompanyResolver`, consultando `dbo.Companies` y `dbo.UserCompanies`.

No existe una entidad `Tenant` separada. En el codigo actual, el concepto tenant se representa por `Company` + `CompanyConnectionInfo` + conexion dinamica.

No existe una entidad Master `Branch` dedicada para relacion Maestro-Sucursal. Existen dos conceptos distintos:

- `Company.BranchCode` y `ParentCompanyId` en Master, agregados por `database/sql/062_master_tenant_configuration.sql`.
- Catalogo financiero tenant `Branches` en formularios `FinancialCatalogs/Branches`, que no representa por si mismo una sucursal tenant sincronizable.

Por tanto, para sincronizacion Maestro-Sucursal la relacion debe anclarse en `CompanyId` de Master y `BranchCompanyId` como empresa-sucursal destino. `BranchId` no debe usarse como clave principal de sincronizacion en esta version porque el modelo real de sucursal sincronizable es otra fila de `dbo.Companies`, no el catalogo financiero tenant.

### Conexion y base de datos

La base Master se accede con `IMasterConnectionFactory`/`MasterConnectionFactory`. Las bases tenant/sucursal se acceden con `ITenantConnectionFactory` cuando hay empresa activa, o resolviendo una empresa destino desde Master para el worker.

Las credenciales se protegen con `ISecretProtector`. Cualquier modulo de configuracion debe evitar exponer passwords, connection strings o secretos en DTOs de lectura.

### Usuario, permisos y auditoria

Usuarios, roles, permisos, menus, formularios y operaciones viven en Master. Se reutilizan:

- `dbo.Users`
- `dbo.Roles`
- `dbo.Permissions`
- `dbo.UserCompanies`
- `dbo.SecurityMenus`
- `dbo.SecurityForms`
- `dbo.SecurityOperations`
- `dbo.SecurityRoleMenus`
- `dbo.SecurityRoleFormOperations`
- `AuditSecurityChanges`

El patron de auditoria administrativa usa columnas `CreatedByUserId`, `CreatedByUserName`, `CreatedAt`, `UpdatedByUserId`, `UpdatedByUserName`, `UpdatedAt`, `IsDeleted`, `DeletedByUserId`, `DeletedByUserName`, `DeletedAt` cuando la tabla es de mantenimiento. Tambien existen auditorias especificas como `SyncAudit` para eventos de sincronizacion.

### Jobs y tareas programadas

No hay entidad generica `Job` o `Schedule` para todo el sistema. Existen infraestructuras puntuales:

- `NuanSystem.MasterBranchSyncWorker` con `BackgroundService`, opciones `MasterBranchSyncWorkerOptions`, locks, reintentos y `SyncOutbox`.
- `NuanSystem.SyncWorker` para SAP.
- Servicios SAP como `SapSyncJobRunner`, `SapSyncLockService`, `SapSyncWatermarkService`, `WorkerHeartbeatService`.

Para esta etapa, `SynchronizationSchedule`, `SynchronizationExecution`, `SynchronizationExecutionDetail`, `SynchronizationError` y `SynchronizationCheckpoint` deben disenarse como parte del modulo Master/Sucursal, no como si existiera un scheduler generico reutilizable completo.

## 4. Patrones de referencia

### Backend administrativo

Referencia principal: `ConfigurationCompanies`.

- Endpoints en `ConfigurationCompanyEndpoints`.
- Commands/queries en `Application/Features/ConfigurationCompanies`.
- Repositorio contrato en `Application/Abstractions/Data`.
- Implementacion Dapper en `Persistence/Repositories/ConfigurationCompanyRepository`.
- Stored procedures en `database/sql/011_master_configuration_companies.sql`.
- Autorizacion con `RequireFormOperation("configuration-companies", "...")`.
- Auditoria desde `ClaimsPrincipal` mediante `EndpointContextHelper.GetAuditUser`.

### Sync existente

Referencia principal: `Application/Features/Sync`, `Persistence/Repositories/Sync`, `Api/Endpoints/SyncEndpoints.cs`, `NuanSystem.MasterBranchSyncWorker`.

Ya existen:

- `SyncEntityConfigurations`
- `SyncDistributionRules`
- `SyncOutbox`
- `SyncOutboxTargets`
- `SyncAudit`
- `SyncInbox`
- `LocalOutbox`
- `ISyncEventPublisher`
- `ISyncRuleEvaluator`
- `IReplicableEntityMetadataProvider`
- aplicadores para `BusinessPartner`, `Item` y `Warehouse`
- monitor API/WinForms para dashboard, outbox, targets, audit y acciones manuales acotadas

El modulo nuevo debe administrar configuracion, no reemplazar estas tablas tecnicas sin una migracion planificada.

### CQRS, validadores y Result

Commands y queries implementan `ICommand<T>`/`IQuery<T>`. Handlers devuelven `Result<T>`. Validators usan FluentValidation y se registran por assembly. `ValidationBehavior` y `LoggingBehavior` estan registrados en Application.

### Transacciones

Existe `ITransactionRunner`, pero solo para transacciones tenant mediante `ITenantConnectionFactory`. En Sync Master actual, algunas acciones manuales abren transaccion directamente contra Master dentro del repositorio. Para configuracion Master, se debe definir si se agrega un runner Master o se mantiene transaccion localizada en repositorio siguiendo el patron de SyncOutbox.

### Paginacion

El monitor Sync usa filtros `Page` y `PageSize`, normalizados hasta 500, y SQL Server `OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY`. En WinForms hay controles con paginacion (`NuanDataGridControl`) y bases CRUD con grilla.

### Autorizacion y permisos

Conviven dos patrones:

- `RequirePermission(PermissionCodes.SyncOutboxView)` para permisos granulares Sync.
- `RequireFormOperation(formKey, operation)` para formularios administrativos.

Para configuracion de sincronizacion conviene usar `RequireFormOperation("synchronization-profiles", "refresh/create/update/delete/consult")` y, si hay acciones sensibles futuras, permisos granulares especificos.

### Frontend WinForms

Referencias:

- Listado CRUD: `ConfigurationCompaniesForm`, `BranchesForm`, `ItemGroupsForm`, `WarehousesForm`.
- Edicion: `ConfigurationCompanyEditForm`, `BranchEditForm`, `WarehouseEditForm`.
- Monitor Sync no CRUD: `SyncMonitorForm`, `SyncOutboxDetailForm`, `SyncRetryDeadLetterReasonDialog`.
- Cliente HTTP: `SyncMonitorClient`, `ConfigurationCompanyClient`.
- Manejo de errores: `UiExceptionHandler`/`GlobalUiExceptionHandler`, `ApiClientException`, mensajes `XtraMessageBox`.
- Menu/navegacion: `ShellViewModel` y factories en `Program.cs`.

## 5. Propuesta de modelo de dominio

La primera version debe soportar solo `MasterToBranch`. No debe incluir sincronizacion bidireccional, conflictos avanzados, scripts personalizados, SQL de usuario, documentos transaccionales, BusinessPartners completos ni adjuntos fisicos.

### Ubicacion general

El modelo de configuracion debe vivir en Master porque gobierna empresas, sucursales, reglas y rutas logicas. Los efectos operativos se materializan en las tablas Sync existentes y en el worker.

### Entidades propuestas

`SynchronizationProfile`

- Perfil administrativo de sincronizacion para una empresa maestra.
- Se relaciona con `MasterCompanyId` hacia `dbo.Companies.Id`.
- Debe exigir `Companies.IsMaster = 1` y `SyncEnabled = 1` para perfiles activos.
- Campos sugeridos: `Id`, `Code`, `Name`, `Description`, `MasterCompanyId`, `Direction = MasterToBranch`, `IsEnabled`, `ConflictPolicy`, `DefaultBatchSize`, `DefaultMaxAttempts`, auditoria y soft delete.
- No debe aceptar SQL, scripts ni mapeos avanzados.

`SynchronizationProfileBranch`

- Sucursales destino habilitadas para un perfil.
- Se relaciona con `SynchronizationProfileId` y `BranchCompanyId`.
- `BranchCompanyId` referencia `dbo.Companies.Id`, no el catalogo financiero `Branches`.
- Debe validar que `BranchCompany.ParentCompanyId = MasterCompanyId`, `IsMaster = 0`, `SyncEnabled = 1` e `IsActive = 1`.
- Campos sugeridos: estado activo, prioridad opcional, ventana horaria opcional simple, auditoria.

`SynchronizationEntity`

- Catalogo administrable de entidades sincronizables por perfil.
- En esta version debe mapearse a nombres reales soportados por la infraestructura: inicialmente `BusinessPartner` limitado, `Item` maestro y `Warehouse` maestro, segun `IReplicableEntityMetadataProvider` y aplicadores existentes.
- Puede ser global por perfil o por master company.
- Campos sugeridos: `Id`, `ProfileId`, `EntityName`, `DisplayName`, `Direction = MasterToBranch`, `IsEnabled`, `BatchSize`, `MaxAttempts`, `ConflictPolicy`.
- Debe excluir documentos, stock, kardex, caja, compras, ventas, adjuntos fisicos y mapeo de campos.

`SynchronizationEntityBranch`

- Matriz entidad-sucursal para habilitar o deshabilitar entidades por branch.
- Se relaciona con `SynchronizationEntityId` y `SynchronizationProfileBranchId`.
- Debe traducirse a reglas de distribucion hacia `SyncDistributionRules` o reemplazarlas solo si se decide migrar el modelo tecnico.
- Campos sugeridos: `IsEnabled`, `RuleType = All` o reglas simples permitidas, `RuleValue` controlado por sistema.

`SynchronizationSchedule`

- Programacion administrativa del perfil.
- Primera version: habilitado/deshabilitado, intervalo simple y ventana opcional. No scheduler avanzado.
- Debe alimentar opciones/criterios del worker, no ejecutar trabajo desde API.
- Campos sugeridos: `ProfileId`, `IsEnabled`, `IntervalSeconds`, `WindowStartLocalTime`, `WindowEndLocalTime`, `TimeZoneId`, `RunOnStartup`, auditoria.

`SynchronizationExecution`

- Cabecera historica de corrida del worker o ejecucion planificada.
- Debe registrar ejecuciones reales del worker cuando este consuma perfiles/schedules.
- Campos sugeridos: `ProfileId`, `StartedAt`, `FinishedAt`, `Status`, `WorkerInstance`, `TotalEvents`, `AppliedCount`, `ErrorCount`, `DeadLetterCount`, `TraceId`.
- En la infraestructura actual, parte del estado vive en `SyncOutbox`/`SyncAudit`; esta tabla seria para agrupar corridas.

`SynchronizationExecutionDetail`

- Detalle por entidad/sucursal/evento dentro de una ejecucion.
- Puede referenciar `SyncOutbox.Id`, `SyncOutboxTargets.Id`, `EntityName`, `BranchCompanyId`, estado y tiempos.
- No debe duplicar payloads completos salvo referencia controlada.

`SynchronizationError`

- Registro normalizado de errores de sincronizacion por perfil/ejecucion/evento.
- Debe guardar codigo seguro, mensaje seguro, detalle tecnico limitado, `TraceId`, `Retryable`, `CreatedAt`.
- No debe almacenar connection strings, passwords, tokens, payload sensible completo ni stack traces para usuarios.

`SynchronizationCheckpoint`

- Marca de progreso por perfil, entidad y sucursal.
- Para MasterToBranch basado en outbox puede guardar ultimo `SyncOutbox.Id`/`EventId` aplicado por entidad-sucursal.
- Debe coexistir con idempotencia de `SyncInbox.EventId`; no reemplaza los indices unicos de Inbox/Outbox.

## 6. Propuesta de persistencia

Ubicacion propuesta:

- Scripts Master en `database/sql/069_master_synchronization_configuration.sql` o siguiente numero disponible.
- Seed de seguridad/menu/form operations en script separado si el patron del repo lo exige.
- Repositorio en `src/Backend/NuanSystem.Persistence/Repositories/Sync` o `Repositories/Synchronization`.
- Contrato en `src/Backend/NuanSystem.Application/Abstractions/Sync` si se considera parte de Sync, o `Abstractions/Data` si se trata como mantenimiento administrativo.

Decision recomendada: usar `Application/Features/Sync/Configuration` o `Application/Features/SynchronizationProfiles` y mantener contratos en `Abstractions/Sync`, porque la configuracion alimenta infraestructura Sync existente.

Compatibilidad con tablas existentes:

- `SynchronizationEntity` puede mapear a `SyncEntityConfigurations` en una primera implementacion si el modelo administrativo no requiere mas campos.
- `SynchronizationEntityBranch` puede mapear a `SyncDistributionRules` y `SyncOutboxTargets`.
- `SynchronizationProfile` y `SynchronizationProfileBranch` no existen actualmente; deben agregarse si se necesita agrupar reglas por perfil y varias sucursales.
- `SynchronizationSchedule`, `SynchronizationExecution`, `SynchronizationExecutionDetail`, `SynchronizationError` y `SynchronizationCheckpoint` tampoco existen como tablas genericas.

Reglas SQL:

- Usar scripts idempotentes en `database/sql`.
- Usar stored procedures `SP_NA_GET/POST/PUT/DELETE...` para CRUD administrativo.
- Mantener auditoria estandar y soft delete.
- Validar relaciones contra `dbo.Companies`.
- Prohibir SQL de usuario y campos de script personalizado.
- No modificar `SyncOutbox.PayloadJson` desde pantallas administrativas.

## 7. Propuesta de endpoints

Modulo propuesto: `SynchronizationConfigurationEndpoints` o extension dentro de `SyncEndpoints` si se mantiene el agrupamiento por `/api/sync`.

Rutas sugeridas:

- `GET /api/sync/configuration/profiles`
- `GET /api/sync/configuration/profiles/{id:int}`
- `POST /api/sync/configuration/profiles`
- `PUT /api/sync/configuration/profiles/{id:int}`
- `DELETE /api/sync/configuration/profiles/{id:int}`
- `GET /api/sync/configuration/profiles/{id:int}/branches`
- `PUT /api/sync/configuration/profiles/{id:int}/branches`
- `GET /api/sync/configuration/profiles/{id:int}/entities`
- `PUT /api/sync/configuration/profiles/{id:int}/entities`
- `GET /api/sync/configuration/eligible-master-companies`
- `GET /api/sync/configuration/eligible-branch-companies?masterCompanyId=...`
- `GET /api/sync/configuration/replicable-entities`
- `GET /api/sync/configuration/profiles/{id:int}/schedule`
- `PUT /api/sync/configuration/profiles/{id:int}/schedule`

Autorizacion:

- FormKey sugerido: `sync-configuration` o `synchronization-profiles`.
- Operaciones: `refresh`, `consult`, `create`, `update`, `delete`.
- Permisos granulares opcionales futuros: `SYNC.CONFIGURATION.VIEW`, `SYNC.CONFIGURATION.MANAGE`.

Los endpoints deben ser thin, llamar a MediatR y devolver `Result<T>` con `ToHttpResult()`.

## 8. Propuesta de formularios

Ubicacion propuesta:

- `src/Frontend/NuanSystem.WinForms.Forms/Sync/Configuration/SynchronizationProfilesForm.cs`
- `SynchronizationProfileEditForm.cs`
- Dialogos auxiliares para seleccion de sucursales y entidades si la edicion se vuelve extensa.

Cliente/ViewModel:

- `src/Frontend/NuanSystem.WinForms.Services/Sync/Configuration/SynchronizationConfigurationClient.cs`
- modelos bajo `Services/Sync/Configuration/Models`
- `src/Frontend/NuanSystem.WinForms.ViewModels/Sync/Configuration/SynchronizationProfilesViewModel.cs`

Patron visual:

- Listado con `BaseGridCrudListForm`.
- Edicion con `BaseEditForm`.
- Tabs/secciones internas para datos generales, sucursales, entidades y schedule simple.
- Lookups a empresas master y branch usando endpoints de elegibles.
- Integracion en `ShellViewModel` y factory de `Program.cs`.
- Errores mediante manejadores UI existentes; nunca mostrar detalles SQL/secretos.

No se debe mezclar con `SyncMonitorForm`: el monitor actual es operativo/observabilidad; el nuevo modulo es configuracion administrativa.

## 9. Propuesta de ejecucion en segundo plano

Infraestructura reutilizable existente:

- `NuanSystem.MasterBranchSyncWorker` como `BackgroundService`.
- `MasterBranchSyncWorkerOptions` con `Enabled`, `BatchSize`, `LockMinutes`, delays, `SkeletonMode`, `EnabledEntityAppliers`.
- Repositorios `ISyncOutboxRepository`, `ISyncAuditRepository`, `ISyncInboxRepository`.
- Dispatcher `ISyncEventApplier` y aplicadores por entidad.
- Estados `Pending`, `InProcess`, `Applied`, `Error`, `Ignored`, `DeadLetter`.
- Locks por `LockedBy`, `LockedAt`, `LockExpiresAt`.

Brecha actual:

- El worker lee opciones de configuracion de archivo y tablas Sync tecnicas, pero no existe todavia un schedule administrativo por perfil.
- No existe agrupacion de ejecuciones por perfil.
- No existe checkpoint administrativo por perfil/entidad/sucursal.

Diseno recomendado:

- Mantener el worker como unico ejecutor de fondo.
- El modulo administrativo solo persiste configuracion.
- En etapa posterior, el worker debe cargar perfiles habilitados desde Master y decidir si procesa segun `SynchronizationSchedule`.
- `SynchronizationExecution*` debe poblarse desde worker, no desde WinForms.
- `SyncInbox.EventId` y los indices unicos siguen siendo la garantia de idempotencia.

## 10. Riesgos tecnicos

- La documentacion objetivo y el codigo real ya tienen infraestructura Sync parcial; crear nuevas tablas sin mapearlas a `SyncEntityConfigurations`/`SyncDistributionRules` puede duplicar conceptos.
- `BranchId` es ambiguo: existe catalogo financiero tenant, pero Sync usa `BranchCompanyId`. Usar el `BranchId` incorrecto romperia aislamiento multiempresa.
- No hay `ITransactionRunner` para Master; configuraciones multi-tabla necesitan una decision transaccional explicita.
- `SyncOutbox` Master y CRUD tenant aun no comparten transaccion distribuida; ya esta documentado como pendiente.
- `SkeletonMode` del worker puede hacer creer al usuario que la sincronizacion esta activa aunque solo observe.
- Las entidades piloto tienen alcance limitado; BusinessPartners completo, documentos y adjuntos estan excluidos.
- Deben evitarse secretos en payload, DTOs, logs y errores.
- Scripts SQL estan en `database/sql`, no en `database/sqlserver`; moverlos sin decision global crearia inconsistencia.

## 11. Dependencias

- Tablas Master `Companies`, `UserCompanies`, seguridad y permisos.
- Scripts `062_master_tenant_configuration.sql`, `064_master_sync_outbox_inbox.sql`, `065_tenant_sync_inbox_local_outbox.sql`, `066_master_sync_monitor_security.sql`, `068_master_sync_warehouse_entity.sql`.
- `ISecretProtector` para conexiones.
- `IMasterConnectionFactory` y resolucion segura de empresas.
- `IReplicableEntityMetadataProvider` para entidades soportadas.
- `ISyncRuleEvaluator` para distribucion a sucursales.
- `NuanApiClient` y `ApiSession` en WinForms.
- `BaseGridCrudListForm`, `BaseEditForm`, `ShellViewModel`, form operations y grid personalization.

## 12. Decisiones pendientes

- Confirmar si el nombre de modulo sera `Sync Configuration`, `Synchronization Profiles` o `Master Branch Synchronization`.
- Decidir si `SynchronizationProfile` es una nueva tabla o si la primera version administra directamente `SyncEntityConfigurations`/`SyncDistributionRules`.
- Definir si se agrega `IMasterTransactionRunner`.
- Definir FormKey final y permisos iniciales.
- Definir si schedules se almacenan desde la primera version o quedan solo como diseno hasta que el worker los consuma.
- Definir lista inicial exacta de entidades habilitables: recomendacion segura: `Item` maestro y `Warehouse` maestro; `BusinessPartner` solo si se mantiene el alcance limitado ya documentado.
- Definir si `ConflictPolicy` queda fijo en `MasterWins` para esta version.
- Definir si `SynchronizationExecution*` sera obligatorio para V1 o si se reutiliza `SyncAudit` hasta una etapa posterior.

## 13. Archivos que deberan crearse por etapa

### Etapa futura: modelo y contratos

- `src/Backend/NuanSystem.Application/Features/Sync/Configuration/Dtos/*.cs`
- `src/Backend/NuanSystem.Application/Features/Sync/Configuration/Commands/*.cs`
- `src/Backend/NuanSystem.Application/Features/Sync/Configuration/Queries/*.cs`
- `src/Backend/NuanSystem.Application/Features/Sync/Configuration/Validators/*.cs`
- `src/Backend/NuanSystem.Application/Abstractions/Sync/ISynchronizationConfigurationRepository.cs`

### Etapa futura: persistencia

- `src/Backend/NuanSystem.Persistence/Repositories/Sync/SynchronizationConfigurationRepository.cs`
- registro en `PersistenceServiceRegistration.cs`
- `database/sql/069_master_synchronization_configuration.sql`
- script de seed de menus/forms/operations/permisos si no se incluye en el mismo archivo

### Etapa futura: API

- `src/Backend/NuanSystem.Api/Endpoints/SynchronizationConfigurationEndpoints.cs`
- registro en `Program.cs`
- constantes de permisos en `PermissionCodes.cs` si se usan permisos granulares

### Etapa futura: WinForms

- `src/Frontend/NuanSystem.WinForms.Services/Sync/Configuration/*`
- `src/Frontend/NuanSystem.WinForms.ViewModels/Sync/Configuration/*`
- `src/Frontend/NuanSystem.WinForms.Forms/Sync/Configuration/SynchronizationProfilesForm.cs`
- `src/Frontend/NuanSystem.WinForms.Forms/Sync/Configuration/SynchronizationProfileEditForm.cs`
- integracion en `ShellViewModel`
- factory y cliente en `src/Frontend/NuanSystem.WinForms/Program.cs`

### Etapa futura: worker

- servicio de lectura de perfiles/schedules en `NuanSystem.MasterBranchSyncWorker`
- persistencia de `SynchronizationExecution*` si se aprueba
- pruebas de schedule/checkpoint/ejecucion

### Etapa futura: pruebas

- pruebas de validadores de configuracion
- pruebas de repositorio/contrato con scripts cuando aplique
- pruebas de permisos/endpoints
- pruebas de que solo `MasterToBranch` es aceptado
- pruebas de exclusion de SQL/scripts personalizados

## 14. Funcionalidades excluidas de la primera version

- Sincronizacion bidireccional.
- Resolucion avanzada de conflictos.
- Scripts personalizados.
- SQL ingresado por el usuario.
- Sincronizacion de documentos transaccionales.
- Sincronizacion completa de BusinessPartners.
- Sincronizacion de adjuntos fisicos.
- Disenador avanzado de mapeo de campos.
- Ejecucion manual tipo `apply`, `run`, `process`, `dispatch`, `claim` o edicion de payload.
- Reproceso masivo avanzado.
- Integracion directa con SAP, SRI o WinForms hacia bases de datos.

## Etapa 4 - Flujo operativo identificado

La clase que genera eventos `SyncOutbox` es `SyncEventPublisher` en `src/Backend/NuanSystem.Application/Features/Sync/Services/SyncEventPublisher.cs`. Recibe solicitudes desde publicadores existentes como `BusinessPartnerSyncPublisher`, `ItemSyncPublisher` y `WarehouseSyncPublisher`, valida metadata con `IReplicableEntityMetadataProvider`, serializa payload seguro con `SyncEventPayloadFactory` y persiste el evento mediante `ISyncOutboxRepository.CreateAsync`.

La creacion de `SyncOutboxTargets` ocurre en el mismo `SyncEventPublisher` despues de persistir el outbox. Antes de esta etapa, los destinos salian de `ISyncRuleEvaluator`/`SyncRuleEvaluator`, que consultaba `SyncDistributionRules`. En Etapa 4 la resolucion pasa a `ISyncRoutingService`/`ISyncRoutingRepository` y al SP `SP_NA_GET_SYNCROUTINGTARGETS`, que consulta perfiles activos `MasterToBranch`, `Incremental` y `MasterWins`. La insercion de targets sigue reutilizando `SyncOutboxRepository.CreateTargetAsync`.

Los destinos se resolvian originalmente en `SyncRuleEvaluator` por `CompanyId`, `EntityName`, `RuleType`, `RuleValue` y sucursales `Companies` activas con `ParentCompanyId` igual a la empresa maestra. La nueva resolucion usa `SyncProfiles`, `SyncProfileBranches`, `SyncProfileEntities` y `SyncProfileEntityBranches`, manteniendo como clave real `CompanyId` de origen y `BranchCompanyId` destino.

El worker `NuanSystem.MasterBranchSyncWorker` esta referenciado en `NuanSystem.sln`, pero `git ls-files "*MasterBranchSyncWorker*"` no devolvio archivos trackeados; por eso en esta etapa queda sin modificaciones. La auditoria de codigo muestra que el worker consume `SyncOutbox`/`SyncOutboxTargets`, usa opciones propias de appsettings y resuelve empresas destino desde Master para aplicar eventos en bases sucursal. No existe todavia scheduler administrativo persistido consumido por el worker.

La prevencion de duplicados existe en `SyncOutboxTargets` con la restriccion unica `UQ_SyncOutboxTargets_Outbox_Branch` sobre `OutboxId` y `BranchCompanyId`. `SyncOutboxRepository.CreateTargetAsync` tambien es idempotente: si el target ya existe devuelve su `Id`; si hay error de indice unico por carrera, recupera el target existente. Etapa 4 no modifica historicos ni reescribe targets previos.

Los `EntityCode` operativos se identifican por los productores existentes y por los aplicadores reales: `BusinessPartner`, `Item` y `Warehouse`. El catalogo administrativo inicial tambien contiene catalogos como `Countries`, `Provinces` y `Cities`, pero esos codigos no tienen productor/aplicador Master-Branch en el codigo actual; se permiten como borrador inactivo con advertencias y se bloquean al activar perfiles.

Los reintentos se manejan en `SyncOutboxRepository`: `ClaimPendingAsync` reclama eventos en estado `Pending` o `Error` con `AttemptCount < MaxAttempts`; `MarkErrorAsync` y `MarkTargetErrorAsync` calculan `NextRetryAt`; cuando se agotan intentos se marca `DeadLetter`. La configuracion efectiva de Etapa 4 alimenta `MaxAttempts` de targets a partir de `MaxRetries` efectivo, manteniendo la semantica existente de targets.

La configuracion reutilizable para routing es: perfil activo, direccion `MasterToBranch`, modo `Incremental`, estrategia `MasterWins`, sucursal activa, entidad activa, matriz entidad-sucursal habilitada y empresa/sucursal con `SyncEnabled`. La prioridad efectiva queda: `BatchSize` matriz, entidad, sucursal, perfil; `MaxRetries` sucursal, perfil; `RetryDelaySeconds` y `TimeoutMinutes` desde perfil; flags desde entidad.

No se agrega `SyncProfileId` ni `SyncProfileEntityId` a `SyncOutboxTargets` en esta etapa. La razon es mantener la tabla tecnica estable, evitar alteracion de historicos y reutilizar la idempotencia existente por `OutboxId + BranchCompanyId`. Si en una etapa posterior se requiere trazabilidad exacta del perfil aplicado al momento de generar el target, debe evaluarse una columna nullable `int` o una tabla de auditoria de routing.

## Etapa 2 implementada

La Etapa 2 agrego solo configuracion administrativa y persistencia Dapper. No se crearon endpoints, formularios, workers, EF Core, migraciones EF ni librerias nuevas.

Archivos creados o modificados en esta etapa:

- `database/sql/069_sync_master_branch_configuration.sql`
- `src/Backend/NuanSystem.Application/Abstractions/Sync/ISyncProfileRepository.cs`
- `src/Backend/NuanSystem.Application/Features/Sync/Configuration/SyncMasterBranchEntityCatalog.cs`
- `src/Backend/NuanSystem.Application/Features/Sync/Configuration/Dtos/SyncProfileConfigurationDtos.cs`
- `src/Backend/NuanSystem.Persistence/Repositories/Sync/SyncProfileRepository.cs`
- `src/Backend/NuanSystem.Persistence/DependencyInjection/PersistenceServiceRegistration.cs`
- `tests/NuanSystem.Application.Tests/Features/Sync/SyncConfigurationContractTests.cs`
- `docs/synchronization/master-branch-synchronization-analysis.md`

Decisiones aplicadas:

- Se usaron `int IDENTITY` y referencias `CompanyId`/`BranchCompanyId`, porque `dbo.Companies.Id` y los catalogos revisados son `int`; no se introdujeron `TenantId`, `MasterTenantId`, `BranchTenantId` ni `BranchId`.
- No se agrego `ConnectionProfileId`, porque no existe un perfil de conexion reutilizable en el modelo real; las conexiones viven en `dbo.Companies`.
- `Code` quedo unico por empresa maestra mediante `UX_SyncProfiles_Company_Code_Active`.
- La matriz entidad-sucursal incluye `SyncProfileId` y FKs compuestas para impedir combinaciones entre perfiles distintos.
- `SyncSchedules` permite maximo un schedule activo por perfil y usa `America/Guayaquil` como zona horaria por defecto.
- El catalogo inicial queda limitado a `Countries`, `Provinces`, `Cities`, `Currencies`, `BusinessPartnerPaymentTerms`, `SupplierGroups`, `SupplierClasses`, `EconomicActivities`, `Zones` y `SupplyMethods`.
- La persistencia usa procedimientos almacenados y carga el agregado completo con `QueryMultipleAsync`; las colecciones se envian como JSON generado por backend, no SQL ingresado por usuario.

Elementos excluidos tambien en Etapa 2:

- `SyncExecutions`, `SyncExecutionDetails`, `SyncErrors` y `SyncCheckpoints`.
- Nuevos `SyncOutbox`, `SyncOutboxTargets`, `SyncInbox` o `SyncAudit`.
- Cambios en `MasterBranchSyncWorker`, endpoints API, monitor WinForms o aplicadores piloto.

## Etapa 3 implementada

La Etapa 3 agrego casos de uso de Application y API REST para administrar la configuracion Maestro-Sucursal. No se implemento ejecucion manual, scheduler, cambios en `NuanSystem.MasterBranchSyncWorker`, generacion de `SyncOutbox`, lectura operativa de `SyncInbox`, formularios WinForms ni nuevas tablas operativas.

Casos de uso implementados:

- `GetSyncProfilesQuery`: listado paginado con `Search`, `CompanyId`, `IsActive`, `ExecutionMode`, `PageNumber` y `PageSize`.
- `GetSyncProfileByIdQuery`: detalle del agregado con perfil, empresa, sucursales, entidades, matriz y schedule.
- `GetSyncConfigurationCatalogQuery`: catalogos para frontend sin datos de conexion.
- `CreateSyncProfileCommand`.
- `UpdateSyncProfileCommand`.
- `ActivateSyncProfileCommand`.
- `DeactivateSyncProfileCommand`.
- `DeleteSyncProfileCommand`.
- `ValidateSyncProfileCommand`.
- `ValidatePersistedSyncProfileCommand`.

DTOs publicos creados:

- `SyncProfileListItemDto`
- `SyncProfileApiDetailDto`
- `SyncProfileBranchDto`
- `SyncProfileEntityDto`
- `SyncEntityBranchDto`
- `SyncScheduleDto`
- `SaveSyncProfileRequest`
- `SaveSyncProfileBranchRequest`
- `SaveSyncProfileEntityRequest`
- `SaveSyncEntityBranchRequest`
- `SaveSyncScheduleRequest`
- `SyncConfigurationCatalogDto`
- `SyncEntityCatalogItemDto`
- `CompanyLookupDto`
- `LookupItemDto`
- `SyncProfileValidationResultDto`
- `SyncValidationMessageDto`
- `PagedResultDto<T>`

Validacion implementada:

- Cabecera: codigo, nombre, duplicados, empresa maestra, direccion `MasterToBranch`, estrategia `MasterWins`, modos soportados y rangos numericos.
- Sucursales: al menos una activa, no igual al maestro, existencia/acceso, pertenencia al maestro, duplicados y rangos por sucursal.
- Entidades: al menos una activa, duplicados, codigos dentro de `SyncMasterBranchEntityCatalog`, orden, modo, dependencias, batch y bloqueo de campos con SQL libre o expresiones ejecutables.
- Matriz entidad-sucursal: referencias dentro del mismo perfil, duplicados, combinaciones habilitadas, entidad sin sucursal y sucursal sin entidad.
- Programacion: tipos `Manual`, `Interval`, `Daily`, forma esperada por tipo y zona horaria valida para .NET.
- Advertencias: modo `Full`, lote alto, `AllowDeactivate`, intervalo muy frecuente, sucursal con pocas entidades y perfil inactivo.

Endpoints creados:

- `GET /api/sync/configuration/profiles`
- `GET /api/sync/configuration/profiles/{id:int}`
- `GET /api/sync/configuration/catalog`
- `POST /api/sync/configuration/profiles`
- `PUT /api/sync/configuration/profiles/{id:int}`
- `DELETE /api/sync/configuration/profiles/{id:int}`
- `POST /api/sync/configuration/profiles/validate`
- `POST /api/sync/configuration/profiles/{id:int}/validate`
- `POST /api/sync/configuration/profiles/{id:int}/activate`
- `POST /api/sync/configuration/profiles/{id:int}/deactivate`

Permisos registrados:

- `SYNC.CONFIGURATION.VIEW`
- `SYNC.CONFIGURATION.CREATE`
- `SYNC.CONFIGURATION.EDIT`
- `SYNC.CONFIGURATION.DELETE`
- `SYNC.CONFIGURATION.ACTIVATE`
- `SYNC.CONFIGURATION.VALIDATE`

Auditoria:

- Create/update/delete/activate/deactivate envian usuario auditor a persistencia y poblan columnas auditables de `SyncProfiles`, hijos y `SyncSchedules`.
- `SyncProfileCreated`, `SyncProfileUpdated`, `SyncProfileActivated`, `SyncProfileDeactivated`, `SyncProfileDeleted` y `SyncProfileValidated` se registran de forma segura en `AuditSecurityChanges` cuando la tabla existe.
- La validacion no persiste el request ni datos sensibles; solo registra resultado `Valid`/`Invalid` y devuelve errores/advertencias estructuradas.

Decision sobre eliminacion:

- Si `ISyncProfileRepository.HasOperationalHistoryAsync` detecta historial relacionado en `SyncOutbox` o `SyncAudit`, la eliminacion se bloquea y el usuario debe desactivar el perfil.
- Si no existe historial operativo, `SP_NA_DELETE_SYNCPROFILEELIMINAR` aplica eliminacion logica transaccional sobre perfil, ramas, entidades, matriz y schedule.

SQL/persistencia agregada:

- `SP_NA_GET_SYNCPROFILEPAGINAR`
- `SP_NA_GET_SYNCCONFIGURATIONCOMPANYLOOKUPS`
- `SP_NA_DELETE_SYNCPROFILEELIMINAR`
- `SP_NA_POST_SYNCPROFILEAUDITREGISTRAR`
- Seed idempotente de permisos `SYNC.CONFIGURATION.*` y asignacion al rol `ADMIN`.

Limitaciones y pendientes para etapa posterior:

- `LastExecutionAt` y `NextExecutionAt` se devuelven `null`; quedaran disponibles cuando exista scheduler/lectura operativa confiable.
- No se verifico Swagger en runtime porque no se levanto la API durante esta etapa.
- No se integro el worker con `SyncSchedules`.
- No se crean ejecuciones, errores normalizados ni checkpoints.
- No se agrega UI WinForms.
- Los DTOs usan `int` en lugar de `Guid` porque el modelo real (`dbo.Companies.Id`, tablas de configuracion y catalogos) usa `int`.

## Archivos revisados

- `README.md`
- `docs/ARCHITECTURE.md`
- `docs/ARQUITECTURA-COMERCIAL.md`
- `docs/FASE-2-MULTIEMPRESA.md`
- `docs/FASE-5-MEDIATR.md`
- `docs/FASE-10-SAP.md`
- `docs/architecture/MASTER-BRANCH-STANDALONE-SAP.md`
- `docs/architecture/SRI-DOCUMENTS-WORKER.md`
- `src/Backend/NuanSystem.Domain/Tenancy/Company.cs`
- `src/Backend/NuanSystem.Domain/Tenancy/TenantFeature.cs`
- `src/Backend/NuanSystem.Domain/Tenancy/TenantIntegration.cs`
- `src/Backend/NuanSystem.Domain/Tenancy/EntitySyncDirection.cs`
- `src/Backend/NuanSystem.Application/Abstractions/Tenancy/ICompanyContext.cs`
- `src/Backend/NuanSystem.Application/Abstractions/Tenancy/CompanyConnectionInfo.cs`
- `src/Backend/NuanSystem.Persistence/Tenancy/CompanyContext.cs`
- `src/Backend/NuanSystem.Persistence/Tenancy/SqlServerCompanyResolver.cs`
- `src/Backend/NuanSystem.Persistence/Connections/MasterConnectionFactory.cs`
- `src/Backend/NuanSystem.Persistence/Connections/TenantConnectionFactory.cs`
- `src/Backend/NuanSystem.Application/Abstractions/Data/ITransactionRunner.cs`
- `src/Backend/NuanSystem.Persistence/Transactions/SqlTransactionRunner.cs`
- `src/Backend/NuanSystem.Application/Abstractions/Sync/*`
- `src/Backend/NuanSystem.Application/Features/Sync/*`
- `src/Backend/NuanSystem.Persistence/Repositories/Sync/*`
- `src/Backend/NuanSystem.Api/Endpoints/SyncEndpoints.cs`
- `src/Backend/NuanSystem.Api/Endpoints/ConfigurationCompanyEndpoints.cs`
- `src/Backend/NuanSystem.Shared/Constants/PermissionCodes.cs`
- `src/Backend/NuanSystem.MasterBranchSyncWorker/Program.cs`
- `src/Backend/NuanSystem.MasterBranchSyncWorker/Workers/MasterBranchSyncWorker.cs`
- `src/Backend/NuanSystem.MasterBranchSyncWorker/Services/MasterBranchSyncWorkerProcessor.cs`
- `src/Backend/NuanSystem.MasterBranchSyncWorker/Options/MasterBranchSyncWorkerOptions.cs`
- `src/Frontend/NuanSystem.WinForms.Services/Sync/SyncMonitorClient.cs`
- `src/Frontend/NuanSystem.WinForms.Services/Sync/Models/SyncMonitorModels.cs`
- `src/Frontend/NuanSystem.WinForms.ViewModels/Sync/*`
- `src/Frontend/NuanSystem.WinForms.Forms/Sync/SyncMonitorForm.cs`
- `src/Frontend/NuanSystem.WinForms.Forms/Sync/SyncOutboxDetailForm.cs`
- `src/Frontend/NuanSystem.WinForms.Forms/ConfigurationCompanies/ConfigurationCompaniesForm.cs`
- `src/Frontend/NuanSystem.WinForms.Services/ConfigurationCompanies/ConfigurationCompanyClient.cs`
- `src/Frontend/NuanSystem.WinForms.Forms/FinancialCatalogs/Branches/BranchesForm.cs`
- `src/Frontend/NuanSystem.WinForms/Program.cs`
- `database/sql/001_master_database.sql`
- `database/sql/062_master_tenant_configuration.sql`
- `database/sql/064_master_sync_outbox_inbox.sql`
- `database/sql/065_tenant_sync_inbox_local_outbox.sql`
- `database/sql/066_master_sync_monitor_security.sql`
- `database/sql/068_master_sync_warehouse_entity.sql`
- `database/sql/069_sync_master_branch_configuration.sql`
- `tests/NuanSystem.Application.Tests/Features/Sync/*`

## Conclusiones

- La solucion ya tiene una base tecnica fuerte para Sync Master/Sucursal: outbox, inbox, targets, audit, worker dedicado, monitor API/WinForms, acciones manuales controladas y aplicadores piloto.
- Falta el modulo administrativo de configuracion solicitado: perfiles, matriz perfil-sucursal, schedule administrativo, ejecuciones agrupadas, errores normalizados y checkpoints administrativos.
- Para Maestro-Sucursal, la clave correcta en el modelo real es `CompanyId`/`BranchCompanyId`. `TenantId` no existe como entidad separada y `BranchId` pertenece a catalogos tenant, no a la topologia de sucursales sincronizables.
- La primera version debe restringirse a `MasterToBranch` y a entidades maestras ya soportadas/limitadas.
- Existe infraestructura reutilizable para workers/jobs, pero no un scheduler administrativo generico completo.

## Diferencias entre arquitectura esperada y arquitectura real

- Esperado: posible EF Core/configuraciones/migraciones. Real: no se encontro EF Core; se usa Dapper + scripts SQL manuales.
- Esperado: `database/sqlserver`. Real: los scripts viven en `database/sql`.
- Esperado: entidad `Branch` clara para sucursal. Real: Sync usa empresas-sucursal (`dbo.Companies` con `ParentCompanyId`, `BranchCode`, `IsMaster = 0`) y existe ademas un catalogo financiero `Branches` no equivalente.
- Esperado: jobs/schedules genericos. Real: hay workers especificos y opciones por appsettings; no hay `SynchronizationSchedule` persistido.
- Esperado: modulo por implementar desde cero. Real: ya existe infraestructura Sync operativa parcial; el nuevo modulo debe configurarla, no duplicarla.

## Etapa 5 implementada

La Etapa 5 agrega programacion administrativa y ejecucion manual de perfiles Sync como productor de eventos. El flujo implementado queda:

`request administrativo -> validar perfil -> crear SyncProfileExecution Pending -> hosted service toma pendiente -> lee entidades Full soportadas desde empresa maestra -> publica con SyncEventPublisher -> SyncOutbox -> ISyncRoutingService -> SyncOutboxTargets -> NuanSystem.MasterBranchSyncWorker`.

No se escribe directo en sucursales, no se escribe `SyncInbox` desde la API, no se crea otro worker de transporte y no se reemplaza `SyncOutbox`, `SyncOutboxTargets`, `SyncInbox`, `SyncAudit`, `ISyncRoutingService`, `SyncEventPublisher` ni `NuanSystem.MasterBranchSyncWorker`.

### Publicacion y payloads

`SyncEventPublisher` sigue siendo el unico productor tecnico de `SyncOutbox`. Para ejecuciones administrativas se agrego `SyncProfileId` opcional y `CorrelationId` opcional a `SyncPublishRequest`. Cuando existe `SyncProfileId`, el publisher mantiene las validaciones de empresa sincronizable y maestra, pero no exige que la entidad este habilitada por metadata incremental, porque el routing se resuelve explicitamente por perfil Full/Manual.

`SyncEventPayloadFactory` agrega `correlationId` al documento raiz cuando existe. El payload operativo sigue siendo el mismo contrato consumido por los aplicadores existentes:

- `BusinessPartnerSyncPayload`
- `ItemSyncPayload`
- `WarehouseSyncPayload`

No se guardan payloads completos en `SyncProfileExecutions` ni en `SyncProfileExecutionDetails`.

### Operaciones soportadas

La ejecucion Full/Manual usa lectura paginada deterministica por `Code` y publica:

- `SyncOperation.Updated` para registros activos.
- `SyncOperation.Disabled` para registros inactivos.

No se implementa `Deleted` fisico. No se implementa SQL ingresado por usuario, scripts ni mapeo avanzado de campos.

### Persistencia agregada

Se agrego `database/sql/071_sync_profile_execution.sql` con tablas minimas:

- `SyncProfileExecutions`
- `SyncProfileExecutionDetails`

No se agrego tabla separada de errores todavia. Los errores por entidad quedan como contadores y mensaje seguro en el detalle. Se agregaron columnas de agenda a `SyncSchedules`:

- `NextExecutionAt`
- `LastSuccessfulScheduledExecutionAt`

El script tambien redefine de forma compatible `SP_NA_GET_SYNCROUTINGTARGETS` para aceptar `@SyncProfileId int = NULL`. Sin `SyncProfileId`, conserva el comportamiento incremental automatico. Con `SyncProfileId`, limita el routing al perfil indicado y permite `Incremental`, `Full` o `Manual`.

### Endpoints agregados

- `POST /api/sync/configuration/profiles/{id:int}/execute`
- `GET /api/sync/configuration/executions`
- `GET /api/sync/configuration/executions/{id:int}`
- `POST /api/sync/configuration/executions/{id:int}/cancel`
- `POST /api/sync/configuration/executions/{id:int}/retry`

Permisos agregados:

- `SYNC.CONFIGURATION.EXECUTE`
- `SYNC.CONFIGURATION.VIEWEXECUTIONS`
- `SYNC.CONFIGURATION.CANCEL`
- `SYNC.CONFIGURATION.RETRY`

### Programacion

Se agrego `ISyncScheduleCalculator` con estas reglas:

- `Manual`: no produce proxima ejecucion.
- `Interval`: calcula desde la ultima ejecucion programada exitosa; si no existe, usa la fecha de configuracion.
- `Daily`: interpreta `ExecutionTime` en `TimeZoneId` mediante `TimeZoneInfo` y devuelve UTC.

No existia abstraccion de reloj en Application; se agrego `ISystemClock`/`SystemClock` para evitar `DateTime.Now` directo en casos de uso.

### Worker administrativo

No existia un hosted service administrativo reutilizable para producir ejecuciones de perfiles. Si existe infraestructura reutilizable de transporte/aplicacion en `NuanSystem.MasterBranchSyncWorker`, pero esa infraestructura no agenda perfiles administrativos.

Por eso se agrego `SyncProfileExecutionHostedService` en API. Este hosted service solo:

- revisa perfiles Full programados vencidos;
- encola ejecuciones Scheduled;
- procesa ejecuciones Pending;
- publica eventos por `SyncEventPublisher`.

No reclama outbox, no aplica en sucursal, no transporta eventos y no toca `SyncInbox`.

### Entidades Full soportadas

Los lectores Full registrados en Persistence son:

- `BusinessPartnerFullEntitySource`
- `ItemFullEntitySource`
- `WarehouseFullEntitySource`

Cada lector resuelve la empresa maestra por `CompanyId`, exige SQL Server y lee paginas ordenadas por `Code`. El limite efectivo usa `BatchSize` de entidad o perfil, y `MaxRecords` cuando se solicita manualmente.

### Cancelacion y reintento

Cancelar cambia la ejecucion a `Cancelling` o `Cancelled`; no borra outbox, no revierte targets y no modifica sucursales. Reintentar crea una nueva ejecucion con los mismos filtros administrativos basicos.

### Diferencias detectadas en Etapa 5

- No habia scheduler administrativo generico; se creo uno minimo en API como productor, no como transporte.
- El worker Master/Sucursal existente es reutilizable para transporte/aplicacion, pero no debia modificarse en esta etapa.
- La metadata incremental existente bloqueaba Full/Manual; se agrego routing explicito por perfil para no afectar el flujo incremental automatico.
- La base tecnica usa `int` para perfiles, entidades y empresas; se mantuvo `int` en ejecuciones y se uso `CorrelationId` string para trazabilidad externa.

## Etapa 6 implementada

La Etapa 6 agrega el frontend WinForms DevExpress para administrar perfiles y monitorear ejecuciones de sincronizacion Maestro/Sucursal. El cliente consume exclusivamente los endpoints REST bajo `/api/sync/configuration`; no usa conexiones SQL, no ejecuta stored procedures, no escribe `SyncOutbox`/`SyncInbox`, no modifica `NuanSystem.MasterBranchSyncWorker` y no expone payloads completos ni secretos.

### Componentes frontend agregados

- `NuanSystem.WinForms.Services.Sync.SyncConfigurationClient`: cliente HTTP centralizado sobre `INuanApiClient`.
- `NuanSystem.WinForms.Services.Sync.Models.SyncConfigurationModels`: contratos del frontend alineados con los DTOs API.
- `NuanSystem.WinForms.ViewModels.Sync.SyncConfigurationViewModels`: filtros, estado editable, armado de `SaveSyncProfileRequest`, ejecuciones y detalle.
- `NuanSystem.WinForms.Forms.Sync.Configuration.SyncProfileListForm`: listado, filtros, crear, editar, eliminar, activar, desactivar, validar, ejecutar y abrir ejecuciones.
- `NuanSystem.WinForms.Forms.Sync.Configuration.SyncProfileEditForm`: edicion con pestanas General, Sucursales, Entidades, Matriz entidad-sucursal y Programacion.
- `NuanSystem.WinForms.Forms.Sync.Configuration.SyncExecutionListForm`: consulta global o por perfil, cancelacion, reintento y polling de ejecuciones activas cada 7 segundos.
- `NuanSystem.WinForms.Forms.Sync.Configuration.SyncExecutionDetailForm`: resumen seguro y detalle por entidad, con cancelacion/reintento y polling.
- `NuanSystem.WinForms.Forms.Sync.Configuration.ExecuteSyncProfileDialog`: dialogo auxiliar para ejecucion manual con entidades opcionales, clave inicial y limite de registros.

### Navegacion y permisos

Se registraron las factories en `Program.cs`, las claves en `MainForm` y el fallback de navegacion en `ShellViewModel`:

- `sync-profiles`: requiere `SYNC.CONFIGURATION.VIEW`.
- `sync-executions`: requiere `SYNC.CONFIGURATION.VIEWEXECUTIONS`.

El script `database/sql/072_sync_configuration_winforms_security.sql` agrega las entradas dinamicas bajo Administracion -> Integraciones:

- Perfiles de sincronizacion.
- Ejecuciones.

Tambien asigna al rol `ADMIN` los permisos funcionales ya existentes:

- `SYNC.CONFIGURATION.VIEW`
- `SYNC.CONFIGURATION.CREATE`
- `SYNC.CONFIGURATION.EDIT`
- `SYNC.CONFIGURATION.DELETE`
- `SYNC.CONFIGURATION.ACTIVATE`
- `SYNC.CONFIGURATION.VALIDATE`
- `SYNC.CONFIGURATION.EXECUTE`
- `SYNC.CONFIGURATION.VIEWEXECUTIONS`
- `SYNC.CONFIGURATION.CANCEL`
- `SYNC.CONFIGURATION.RETRY`

### Alcance funcional cubierto

- Listar, filtrar, crear, editar y eliminar perfiles.
- Activar/desactivar perfiles.
- Validar perfiles persistidos y perfiles editados antes de guardar.
- Configurar sucursales, entidades, matriz entidad-sucursal y programacion.
- Ejecutar manualmente un perfil con filtros administrativos.
- Consultar ejecuciones globales y por perfil.
- Ver detalle por entidad sin payloads ni secretos.
- Cancelar y reintentar ejecuciones mediante API.
- Refrescar automaticamente ejecuciones activas con polling y detenerlo al cerrar formularios.

### Pruebas y contratos agregados

Se agrego `tests/NuanSystem.Application.Tests/Features/Sync/SyncConfigurationFrontendContractTests.cs` para validar:

- uso exclusivo de `/api/sync/configuration`;
- registro en `Program`, `MainForm`, `ShellViewModel` y seed SQL;
- ausencia de acceso directo a base de datos, `SyncOutbox`, `SyncInbox`, worker y payloads;
- armado correcto del request desde `SyncProfileEditorState`.

### Diferencias detectadas en Etapa 6

- No existe un proyecto separado de pruebas WinForms; los contratos frontend Sync se validan en `NuanSystem.Application.Tests`, que ya referencia `NuanSystem.WinForms.ViewModels`.
- La navegacion real prioriza menus dinamicos desde seguridad; por eso tambien se agrego fallback local en `ShellViewModel`.
- La UI existente mezcla formularios Designer y formularios construidos por codigo. Para esta etapa se uso construccion programatica DevExpress para reducir archivos generados y mantener el alcance revisable.
- Los permisos de operaciones por formulario del shell son genericos; las acciones especificas de Sync se habilitan adicionalmente con `ApiSession.HasPermission` dentro de los formularios.

## Etapa 7 implementada

La Etapa 7 revisa integralmente el modulo Maestro-Sucursal construido en las etapas anteriores, endurece defectos puntuales y documenta el plan de pruebas. No se agregaron entidades de dominio, no se crearon endpoints nuevos, no se agregaron formularios nuevos, no se introdujeron librerias y no se modifico el worker Master/Sucursal.

### Matriz de revision

| Componente | Estado | Problema | Severidad | Correccion | Archivo | Resultado |
| --- | --- | --- | --- | --- | --- | --- |
| `SyncExecutionListForm` | Corregido | Usaba `Queued` y no contemplaba `Pending`/`Cancelling` como estados activos reales. | Alta | Estados alineados con backend y polling protegido contra controles dispuestos. | `src/Frontend/NuanSystem.WinForms.Forms/Sync/Configuration/SyncExecutionListForm.cs` | Cubierto por test de contrato. |
| `SyncExecutionDetailForm` | Corregido | Mismo desalineamiento de estados y riesgo de actualizar UI despues de `Dispose`. | Alta | `Pending`, `Running`, `Cancelling`; guardas `IsDisposed || Disposing`. | `src/Frontend/NuanSystem.WinForms.Forms/Sync/Configuration/SyncExecutionDetailForm.cs` | Cubierto por test de contrato. |
| `SyncProfileEditForm` | Corregido | Programacion Manual podia enviar `ExecutionTime = 00:00`, invalidando el guardado. | Alta | Campos de programacion normalizados por tipo y UI deshabilita/limpia campos no aplicables. | `src/Frontend/NuanSystem.WinForms.Forms/Sync/Configuration/SyncProfileEditForm.cs` | Cubierto por test de ViewModel. |
| `SyncScheduleEditorState` | Corregido | El request podia transportar intervalo/hora aunque el tipo no correspondiera. | Alta | `ToRequest()` envia intervalo solo en `Interval` y hora solo en `Daily`. | `src/Frontend/NuanSystem.WinForms.ViewModels/Sync/SyncConfigurationViewModels.cs` | Cubierto por test. |
| Seed WinForms Sync | Corregido | El seed de `RolePermissions` asumio columnas inexistentes (`IsDeleted`, `UpdatedByUserName`, `UpdatedAt`). | Alta | Insercion idempotente usando solo `RoleId`, `PermissionId`. | `database/sql/072_sync_configuration_winforms_security.sql` | Cubierto por test de contrato. |
| `SP_NA_CREATE_SYNCPROFILEEXECUTION` | Corregido | La validacion de ejecucion activa y la insercion no eran atomicas. | Alta | Script incremental `073` redefine el SP con transaccion y `UPDLOCK, HOLDLOCK`. | `database/sql/073_sync_master_branch_hardening.sql` | Cubierto por test de contrato. |
| Flujo de arquitectura | Cumple | No se detecto ruta paralela desde WinForms/API hacia sucursales o Inbox. | Media | Se documenta flujo unico configuracion -> ejecucion -> publisher -> outbox -> routing -> targets -> worker -> inbox. | `docs/synchronization/master-branch-synchronization-architecture.md` | Cumple. |
| API/Frontend | Cumple | Riesgo de exponer payloads, secretos o operaciones worker al frontend. | Alta | Test verifica ausencia de `SqlConnection`, `Dapper`, payloads, `SyncInbox`, `SyncOutbox`, rutas worker. | `tests/NuanSystem.Application.Tests/Features/Sync/SyncConfigurationFrontendContractTests.cs` | Cumple. |
| Monitor SyncOutbox | Corregido | El detalle del monitor renderizaba `PayloadJson` completo en WinForms. | Alta | La UI retiene el payload por seguridad y conserva metadatos, targets, auditoria y errores para diagnostico. | `src/Frontend/NuanSystem.WinForms.Forms/Sync/SyncOutboxDetailForm.cs` | Cubierto por test de contrato. |
| Revision visual | Parcial | No se abrio UI real DevExpress en resoluciones/DPI solicitadas dentro de esta ejecucion. | Media | Se hizo revision estatica, build WinForms y plan manual 1366/1920 DPI 100/125. | `docs/synchronization/master-branch-synchronization-test-plan.md` | Pendiente manual. |
| Worker Master/Sucursal | Riesgo aceptado | El proyecto esta en estado no trackeado y bloquea builds completos si esta ejecutandose. | Media | No se modifico; se valida por contrato y se documenta como dependencia operativa. | `src/Backend/NuanSystem.MasterBranchSyncWorker` | Infraestructura reutilizable existe. |

### Flujo validado

El flujo vigente queda asi:

1. WinForms administra perfiles y ejecuciones mediante `SyncConfigurationClient`.
2. La API expone `/api/sync/configuration` y delega en Application.
3. Application valida y persiste configuracion/ejecuciones mediante repositorios Dapper.
4. El hosted service administrativo toma ejecuciones `Pending` y usa `SyncEventPublisher`.
5. `SyncEventPublisher` escribe `SyncOutbox`.
6. `ISyncRoutingService` calcula destinos y escribe `SyncOutboxTargets`.
7. `NuanSystem.MasterBranchSyncWorker` reclama targets y entrega/aplica en `SyncInbox`.

No se encontro acceso directo del frontend a base de datos, stored procedures, `SyncInbox`, `SyncOutboxTargets` ni al worker.

### Seguridad y datos sensibles

- Los formularios y modelos frontend no exponen passwords, cadenas de conexion ni payload JSON completo.
- Las acciones estan separadas por permisos `SYNC.CONFIGURATION.*`.
- Cancelar y reintentar se ejecutan por API y no manipulan colas fisicas desde WinForms.
- Los mensajes de error deben seguir normalizados por la API; la UI solo muestra respuestas funcionales.

### Concurrencia y performance

- Las ejecuciones activas son `Pending`, `Running` y `Cancelling`.
- El polling WinForms usa 7 segundos, no permite solapes por `isRefreshing` y evita actualizar controles dispuestos.
- La reserva de ejecucion con `PreventConcurrentExecutions = 1` queda endurecida en SQL con locks transaccionales.
- Listados y ejecuciones usan paginacion/filtros; la UI no debe cargar payloads completos.

### Pruebas de Etapa 7

Se amplio `SyncConfigurationFrontendContractTests` para cubrir:

- estados reales de ejecucion y ausencia de `Queued`;
- guardas contra `Dispose` en polling;
- normalizacion de schedule Manual;
- seed de `RolePermissions` compatible con el esquema real;
- hardening transaccional de `SP_NA_CREATE_SYNCPROFILEEXECUTION`.

Tambien se creo `docs/synchronization/master-branch-synchronization-test-plan.md` con 40 casos manuales e integrales.

### Documentacion agregada

- `docs/synchronization/master-branch-synchronization-architecture.md`
- `docs/synchronization/master-branch-synchronization-test-plan.md`

### Riesgos pendientes

- Falta ejecutar revision visual manual en DevExpress real con 1366x768/1920x1080 y DPI 100/125.
- No hay `rowversion` ni token explicito de concurrencia optimista en edicion de perfiles; por ahora se mitiga con validaciones y auditoria, pero conviene evaluarlo antes de uso multiadministrador intensivo.
- El build/test completo puede fallar si el worker no trackeado esta ejecutandose y mantiene ensamblados bloqueados.
- La compatibilidad futura MySQL sigue pendiente porque los scripts actuales son SQL Server-first.

### Diferencias entre arquitectura esperada y real

- No hay EF Core ni migraciones EF; la persistencia real es Dapper + SQL versionado manual.
- No hay proyecto de pruebas WinForms separado; los contratos frontend se prueban desde `NuanSystem.Application.Tests`.
- El scheduler administrativo implementado es un hosted service propio en API, no Hangfire, Quartz ni un worker nuevo.
- La navegacion WinForms combina menus dinamicos de seguridad con fallback local en `ShellViewModel`.

### Checklist final

| Item | Estado | Observacion |
| --- | --- | --- |
| No agregar sincronizacion bidireccional | Cumple | Solo `MasterToBranch`. |
| No agregar resolucion avanzada de conflictos | Cumple | Se mantiene `MasterWins`. |
| No crear entidades nuevas en Etapa 7 | Cumple | Solo documentacion, tests y hardening puntual. |
| No crear endpoints nuevos en Etapa 7 | Cumple | Sin endpoints nuevos. |
| No crear formularios nuevos en Etapa 7 | Cumple | Solo ajustes en formularios existentes. |
| No introducir librerias | Cumple | Sin paquetes nuevos. |
| Frontend sin acceso directo a SQL/SP | Cumple | Verificado por contrato. |
| Frontend sin secretos/payloads | Cumple | Verificado por contrato. |
| Polling cada 7 segundos | Cumple | Formularios de ejecucion usan timer de 7s. |
| Polling solo para activos | Cumple | `Pending`, `Running`, `Cancelling`. |
| Polling sin solape | Cumple | Guarda `isRefreshing`. |
| No actualizar UI tras `Dispose` | Cumple | Guardas `IsDisposed || Disposing`. |
| Seed WinForms compatible | Cumple | `RolePermissions` corregido. |
| Reserva concurrente de ejecucion | Cumple | Script `073`. |
| Revision visual real | Pendiente | Requiere ejecucion manual en Windows/DevExpress. |
| Infraestructura jobs/workers reutilizable | Cumple | Existe hosted service administrativo y worker Master/Sucursal reutilizable. |

## Etapa 8 - Formularios compatibles con Visual Studio Designer

La Etapa 8 convierte los formularios WinForms DevExpress de configuracion Maestro-Sucursal que construian su interfaz en runtime al patron estandar `FormName.cs` + `FormName.Designer.cs` + `FormName.resx`. No se cambiaron endpoints, DTOs, permisos, routing, scheduler, stored procedures ni worker.

### Auditoria previa

| Formulario | Metodo que construia UI | Controles principales | Eventos asociados | Datos dinamicos | Elementos que permanecen en runtime |
| --- | --- | --- | --- | --- | --- |
| `SyncProfileListForm` | `BuildLayout`, `ConfigureGrid` | filtros, botones CRUD/Sync, `GridControl`, `GridView`, columnas | Enter en busqueda, acciones CRUD, activar, validar, ejecutar, doble click | perfiles, filtros, permisos | carga paginada, permisos, acciones, navegacion |
| `SyncProfileEditForm` | `BuildLayout`, `BuildGeneralTab`, `BuildBranchesTab`, `BuildEntitiesTab`, `BuildMatrixTab`, `BuildScheduleTab`, `BuildGridPanel`, `ConfigureGrids` | tabs, editores, lookups, grillas de sucursales/entidades/matriz, schedule, footer | guardar, validar, agregar/quitar, cambios de grilla, schedule type | catalogos, datasources, matriz | catalogos API, bind/pull state, validacion, guardado, matriz de datos |
| `SyncExecutionListForm` | `BuildLayout`, `ConfigureGrid` | filtros, acciones, grilla, timer | refrescar, detalle, cancelar, reintentar, doble click, tick | ejecuciones, permisos, polling | polling, carga, acciones API, filtros |
| `SyncExecutionDetailForm` | `BuildLayout`, `ConfigureGrid` | resumen, acciones, grilla de detalle, timer | refrescar, cancelar, reintentar, tick | detalle de ejecucion, permisos | polling, carga detalle, acciones API |
| `ExecuteSyncProfileDialog` | `BuildLayout` | editores de entidades/clave/maximo, botones | click ejecutar | request construido desde inputs | construccion del request |

No se encontraron `SyncBranchEditDialog`, `SyncEntityEditDialog` ni `SyncEntityBranchEditDialog` en el modulo de configuracion.

### Formularios refactorizados

- `SyncProfileListForm`
- `SyncProfileEditForm`
- `SyncExecutionListForm`
- `SyncExecutionDetailForm`
- `ExecuteSyncProfileDialog`

Cada uno tiene ahora constructor sin parametros para Designer y constructor productivo para DI cuando aplica. El constructor sin parametros ejecuta `InitializeComponent()` y configuracion visual segura; no carga API, base de datos, sesion, permisos ni polling.

### Metodos runtime eliminados

Se eliminaron de los archivos principales:

- `BuildLayout`
- `BuildGeneralTab`
- `BuildBranchesTab`
- `BuildEntitiesTab`
- `BuildMatrixTab`
- `BuildScheduleTab`
- `BuildGridPanel`
- `ConfigureGrid`
- `ConfigureGrids`
- `ConfigureEditors`

La estructura visual paso a `InitializeComponent()` en los `.Designer.cs`. La logica funcional queda en los `.cs`.

### Elementos que permanecen dinamicos

- Carga de catalogos de empresas, sucursales, entidades y tipos.
- Datasource de grillas y lookups.
- Permisos de usuario por `ApiSession`.
- Guardado, validacion, ejecucion manual, cancelacion y reintento.
- Polling cada 7 segundos en ejecuciones.
- Estado de campos de programacion segun tipo.
- Matriz de datos entidad-sucursal.

### Estrategia Designer

- Constructor sin parametros para Visual Studio Designer.
- Constructor productivo con ViewModel/cliente/sesion llama a `: this()`.
- Campos inyectados son nullable y se exponen mediante propiedades que lanzan error solo si se usa el formulario productivo sin DI.
- `OnShown` retorna sin ejecutar runtime si `IsInDesignMode()` o si el ViewModel es `null`.
- Los timers se declaran en Designer con `Enabled = false`; se inician solo en runtime luego de la carga inicial.

### Deteccion de modo diseno

Los formularios usan el patron:

```csharp
LicenseManager.UsageMode == LicenseUsageMode.Designtime
    || DesignMode
    || Site?.DesignMode == true
```

### Eventos revisados

Los eventos se conectan una sola vez desde los constructores productivos o configuracion runtime. No se dejan eventos que ejecuten API en el constructor sin parametros. El Designer contiene estructura visual, columnas estaticas y propiedades iniciales.

### Pruebas agregadas

`SyncConfigurationFrontendContractTests` valida:

- existencia de `.Designer.cs` y `.resx`;
- formularios `partial`;
- llamada a `InitializeComponent()`;
- constructor compatible con Designer;
- ausencia de `BuildLayout`, `BuildUi`, `CreateControls`, `CreateTabs`;
- ausencia de creacion de controles principales en `.cs`;
- ausencia de SQL, Dapper y `HttpClient` en Designer;
- timers deshabilitados en Designer y arranque solo fuera de modo diseno.

### Resultados de compilacion

- `dotnet build src\Frontend\NuanSystem.WinForms\NuanSystem.WinForms.csproj -v minimal`: correcto.
- `dotnet build src\Backend\NuanSystem.Application\NuanSystem.Application.csproj -v minimal`: correcto.
- `dotnet build src\Backend\NuanSystem.Api\NuanSystem.Api.csproj -v minimal`: correcto.
- `dotnet test tests\NuanSystem.Application.Tests\NuanSystem.Application.Tests.csproj -v minimal -p:BuildProjectReferences=false`: correcto, 253 pruebas.

El build completo de tests sin workaround sigue bloqueado por el proceso `dotnet.exe (51044)` del worker Master/Sucursal, igual que en la Etapa 7.

### Apertura en Visual Studio Designer

Pendiente de verificacion manual en Visual Studio. Desde esta sesion se verifico por compilacion y contratos de estructura; no se inicio Visual Studio ni se abrio el diseñador grafico.

### Limitaciones

- La verificacion real del Designer debe hacerse manualmente con Visual Studio, API detenida, base no disponible y usuario no autenticado.
- La revision visual 1366x768/1920x1080 y DPI 100/125 queda en el plan de pruebas manual.
- El editor conserva la funcionalidad existente; no se agregaron tabs funcionales nuevos para validacion persistente o historial de ejecuciones dentro del editor porque eso seria nueva funcionalidad.
