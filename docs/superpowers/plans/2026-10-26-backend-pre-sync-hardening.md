# Backend Pre-Sync Hardening Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers-dev:subagent-driven-development (recommended) or superpowers-dev:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Completar el endurecimiento del backend independiente de NuanSystem antes de desarrollar sincronizacion Master/Sucursal o exportacion ERP hacia SAP.

**Architecture:** Mantener el monolito modular y la direccion `Api -> Application -> Domain`, con Persistence e Infrastructure implementando contratos de Application. El trabajo se divide en cuatro entregas independientes: integridad del workflow de compras, sello de seguridad eficiente, frontera segura de errores y descomposicion de componentes tocados; las capacidades de sincronizacion permanecen deshabilitadas o explicitamente no disponibles.

**Tech Stack:** .NET 10, ASP.NET Core Minimal API, MediatR, FluentValidation, Dapper, SQL Server y xUnit existentes en el repositorio. No agregar paquetes, servicios externos, cache distribuida ni microservicios.

**Spec:** Auditoria arquitectonica del 2026-10-26; `docs/architecture/BACKEND-SOLID-STATUS.md`; `docs/operations/BACKEND-SOLID-VALIDATION.md`; `docs/ARCHITECTURE.md`; `docs/ARQUITECTURA-COMERCIAL.md`.

## Global Constraints

- NuanSystem debe seguir funcionando como ERP independiente sin SAP.
- Quedan fuera de alcance: Master/Sucursal, `SyncEventPublisher`, `SyncOutboxRepository`, LocalOutbox, Inbox, workers de sincronizacion, exportadores ERP->SAP, importadores SAP, watermarks, locks y pruebas SQL de Fase 7.
- No declarar como implementada ninguna operacion SAP o de sincronizacion que retorne `NotImplemented` o no tenga procesador ejecutable.
- No cambiar reglas monetarias, calculos, numeracion ni datos historicos de ordenes de compra.
- La API obtiene identidad y empresa desde el contexto autenticado; nunca desde valores de auditoria enviados por el cliente.
- Las transiciones de documentos se validan contra estado autoritativo y se escriben atomicamente con su auditoria funcional.
- No registrar contrasenas, tokens, cookies, cadenas de conexion, payloads completos ni mensajes externos sin sanitizar.
- Cada incremento debe compilar y aprobar sus pruebas antes de comenzar el siguiente.
- Los scripts SQL se preparan de forma idempotente, pero solo se ejecutan sobre bases autorizadas y respaldadas.
- Preservar los cambios locales existentes; implementar en worktree aislado cuando comience la ejecucion.

## Review Focus

1. Dos aprobadores actuando simultaneamente: solo una transicion puede ganar y no se duplican respuestas de aprobacion.
2. Cambio de clave, rol o permiso mientras existe un JWT: el token anterior deja de ser valido en la siguiente peticion.
3. Mensaje de SQL/SAP que contiene secretos: la respuesta y la auditoria almacenan solo informacion saneada y correlacionada.
4. Empresa sin SAP o exportador: solicitar `sync-sap` no cambia estado, no agrega un falso pendiente y devuelve un error estable.
5. Refactor de archivos: no modifica rutas, permisos, contratos HTTP, estados ni resultados observables.

---

## Scope and delivery order

| Incremento | Resultado util independiente | Dependencia |
|---|---|---|
| 0 | Baseline, ADR y frontera que impide presentar sincronizacion incompleta | Ninguna |
| 1 | Workflow manual de compras consistente y auditable | 0 |
| 2 | Revocacion JWT explicita con una consulta liviana por peticion | 0 |
| 3 | Errores desacoplados de SQL/SAP y diagnosticos saneados | 0 |
| 4 | Archivos focalizados y cierre de calidad | 1, 2 y 3 |

Los incrementos 1, 2 y 3 pueden revisarse y liberarse por separado. El incremento 4 solo reorganiza codigo ya protegido por pruebas.

### Task 0: Baseline, ADR and unavailable-capability boundary

**Files:**
- Create: `docs/architecture/BACKEND-PRE-SYNC-HARDENING.md`
- Create: `tests/NuanSystem.Application.Tests/Architecture/PreSyncCapabilityBoundaryTests.cs`
- Modify: `src/Backend/NuanSystem.Application/Features/Purchasing/PurchaseOrders/Commands/PurchaseOrderCommandHandlers.cs`
- Test: `tests/NuanSystem.Application.Tests/Features/Purchasing/PurchaseOrders/PurchaseOrderWorkflowHandlerTests.cs`

**Interfaces:**
- Consumes: `Result<T>`, `FailureKind.Unavailable`, `SyncPurchaseOrderSapCommand`.
- Produces: codigo estable `SAP_EXPORT_NOT_AVAILABLE`; ninguna escritura de estado o log SAP cuando no existe exportador ejecutable.

- [ ] **Step 1: Record the baseline and exclusions**

  Documentar en el ADR las dependencias permitidas, los cuatro incrementos incluidos y la lista explicita de componentes de sincronizacion excluidos.

- [ ] **Step 2: Write the failing capability test**

  Agregar `SyncSap_WhenExporterIsUnavailable_DoesNotMutateOrderOrLogPending` y comprobar `FailureKind.Unavailable`, error `SAP_EXPORT_NOT_AVAILABLE`, cero llamadas a `AddSapLogAsync` y cero llamadas a `UpdateStatusIfCurrentAsync`.

- [ ] **Step 3: Run the targeted test and confirm the current false-pending behavior**

  Run: `dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj --filter "FullyQualifiedName~PurchaseOrderWorkflowHandlerTests.SyncSap_WhenExporterIsUnavailable"`

  Expected before implementation: FAIL because the current handler writes `Pending` and `SapPending`.

- [ ] **Step 4: Add the minimal availability guard**

  Mantener el contrato HTTP actual, pero resolver la disponibilidad mediante una abstraccion de Application registrada en composicion. La implementacion de esta etapa devuelve no disponible; no crear exportador, Outbox ni worker. Evaluar el guard antes de cualquier escritura.

- [ ] **Step 5: Add an architecture test for the deferred boundary**

  Verificar que no exista registro de `ISapSyncExporter` productivo y que la operacion no se anuncie como ejecutable mientras esa coleccion este vacia.

- [ ] **Step 6: Verify and commit**

  Run: `dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj --filter "FullyQualifiedName~PurchaseOrderWorkflowHandlerTests|FullyQualifiedName~PreSyncCapabilityBoundaryTests"`

  Commit: `docs/architecture/BACKEND-PRE-SYNC-HARDENING.md`, guard y pruebas con mensaje `fix: guard unavailable sap export capability`.

### Task 1: Atomic manual purchase-order workflow

**Files:**
- Modify: `src/Backend/NuanSystem.Application/Features/Purchasing/PurchaseOrders/Commands/PurchaseOrderWorkflowCommands.cs`
- Modify: `src/Backend/NuanSystem.Application/Features/Purchasing/PurchaseOrders/Commands/PurchaseOrderCommandHandlers.cs`
- Modify: `src/Backend/NuanSystem.Application/Abstractions/Data/IPurchaseOrderRepository.cs`
- Modify: `src/Backend/NuanSystem.Persistence/Repositories/Purchasing/PurchaseOrderRepository.cs`
- Create: `database/sql/237_tenant_purchase_order_approval_workflow.sql`
- Modify: tenant database initializer that registers ordered scripts
- Modify: `src/Backend/NuanSystem.Api/Endpoints/PurchaseOrderEndpoints.cs`
- Test: `tests/NuanSystem.Application.Tests/Features/Purchasing/PurchaseOrders/PurchaseOrderWorkflowPolicyTests.cs`
- Test: `tests/NuanSystem.Application.Tests/Features/Purchasing/PurchaseOrders/PurchaseOrderWorkflowHandlerTests.cs`
- Create: `tests/NuanSystem.Application.Tests/Features/Purchasing/PurchaseOrders/PurchaseOrderApprovalPersistenceContractTests.cs`

**Interfaces:**
- Consumes: current single-level, manually initiated approval workflow and `PurchaseOrderApprovalDto`.
- Produces: `TransitionWorkflowAsync(PurchaseOrderWorkflowTransitionData data, CancellationToken cancellationToken)` returning the updated order or a conflict result; transition and approval audit commit together.

- [ ] **Step 1: Characterize the existing state machine**

  Add tests for Draft/Rejected -> PendingApproval, PendingApproval -> Approved/Rejected, stale-state conflict and forbidden series operation. Preserve current allowed transitions exactly.

- [ ] **Step 2: Define the transition data contract**

  Add `PurchaseOrderWorkflowTransitionData` with order id, expected statuses, next status, action, approval level `1`, optional observation, trusted user id and trusted user name. Do not include SAP state.

- [ ] **Step 3: Extend command contracts compatibly**

  Add optional `Observation` to approve/reject requests and commands. Existing callers without observation remain valid. Sending to approval records a pending level-1 approval.

- [ ] **Step 4: Write failing handler tests**

  Assert that send creates one Pending approval; approve/reject update that same pending approval with user, `RespondedAt`, status and observation; a stale transition returns Conflict without partial audit.

- [ ] **Step 5: Add the idempotent SQL workflow procedure**

  Script `237` must create or alter one stored procedure that locks the header row, validates expected status, updates the header and inserts/updates `PurchaseOrderApprovals` in one transaction. Add uniqueness preventing more than one open approval for the same order and level without deleting history.

- [ ] **Step 6: Implement the repository operation**

  Map the procedure result to a stable result distinguishing Updated, NotFound and Conflict. Application must not know procedure names or `SqlException`.

- [ ] **Step 7: Replace the TODO path in the handlers**

  Route send/approve/reject through `TransitionWorkflowAsync`; remove the TODO only after approval metadata and atomicity are covered. Keep permissions and company context unchanged.

- [ ] **Step 8: Verify SQL contract and behavior**

  Run targeted unit/contract tests. Execute script twice only in an authorized disposable tenant; prove the second pass is idempotent and two concurrent approval attempts produce one winner.

- [ ] **Step 9: Commit**

  Commit with message `feat: complete purchase order approval audit`.

**Deferred business decision:** Automatic `ConfirmPurchaseOrderCommand`, amount thresholds, multi-level rules and approver selection require approved business rules. This task deliberately completes the current manual single-level workflow without inventing those policies.

### Task 2: Explicit and efficient user security stamp

**Files:**
- Create: `database/sql/238_master_user_security_stamp.sql`
- Modify: `src/Backend/NuanSystem.Persistence/Services/SqlServerMasterDatabaseInitializer.cs`
- Modify: `src/Backend/NuanSystem.Persistence/Security/UserSecurityStateService.cs`
- Modify: `src/Backend/NuanSystem.Persistence/Security/SqlServerUserCredentialRepository.cs`
- Modify: user, role assignment and role-permission persistence paths discovered under `NuanSystem.Persistence/Repositories`
- Test: `tests/NuanSystem.Application.Tests/Features/Auth/Commands/ChangePasswordCommandHandlerTests.cs`
- Create: `tests/NuanSystem.Application.Tests/Architecture/UserSecurityStampContractTests.cs`
- Create: `tests/NuanSystem.Application.Tests/Infrastructure/UserSecurityStampSqlContractTests.cs`

**Interfaces:**
- Consumes: `IUserSecurityStateService.GetSecurityStampAsync(int, CancellationToken)` and the existing JWT claim.
- Produces: persisted opaque `SecurityStamp`; a valid-user check plus stamp retrieval in one query; stamp rotation for every credential/authorization mutation.

- [ ] **Step 1: Write contract tests for revocation triggers**

  Cover password change, activation/deactivation, lock/unlock, user-role assignment/removal and role-permission change. The previous stamp must differ after each successful mutation and remain unchanged after a rejected mutation.

- [ ] **Step 2: Add the idempotent Master migration**

  Script `238` adds a non-null opaque stamp to `dbo.Users`, backfills existing users safely and provides procedures/statements to rotate one user or every user assigned to a role. Do not store JWTs or session payloads.

- [ ] **Step 3: Register the migration in the Master initializer**

  Execute `238` after the base security scripts and before consumers require the column. Add a contract test for ordering and repeat execution.

- [ ] **Step 4: Reduce token validation to one query**

  Change `UserSecurityStateService` to retrieve active/lock state and the persisted stamp in one round trip. Remove role and permission aggregation from per-request validation; login still loads claims through the existing auth path.

- [ ] **Step 5: Rotate stamps inside the owning write operations**

  Password and user changes rotate the user stamp in the same database operation. Role membership changes rotate affected users; role permission changes rotate every active user holding that role. A failed write must not rotate it.

- [ ] **Step 6: Add an authentication integration test**

  Issue a token, validate it, perform each supported security mutation, then prove the old token is rejected and a newly issued token succeeds. Use an authorized disposable Master database.

- [ ] **Step 7: Verify and commit**

  Run Auth, SecurityUsers, SecurityRoles and architecture suites. Record query-count evidence showing one Master query per validated token.

  Commit with message `perf: persist and rotate user security stamps`.

### Task 3: Provider-neutral and sanitized error boundary

**Files:**
- Create: `src/Backend/NuanSystem.Application/Common/Exceptions/DependencyFailureException.cs`
- Create: `src/Backend/NuanSystem.Application/Abstractions/Diagnostics/IErrorSanitizer.cs`
- Create: `src/Backend/NuanSystem.Infrastructure/Diagnostics/ErrorSanitizer.cs`
- Modify: `src/Backend/NuanSystem.Api/Middleware/GlobalExceptionMiddleware.cs`
- Modify: `src/Backend/NuanSystem.Application/Common/Exceptions/SapServiceLayerException.cs`
- Modify: SAP adapter catch/throw sites under `src/Backend/NuanSystem.SapIntegration`
- Modify: Persistence boundary only where provider exceptions currently escape expected operations
- Test: create `tests/NuanSystem.Application.Tests/Architecture/GlobalExceptionBoundaryTests.cs`
- Test: extend SAP transport tests under `tests/NuanSystem.Application.Tests/Features/SapSync`

**Interfaces:**
- Consumes: existing `ApiError`, trace id and audit repository.
- Produces: stable dependency categories `Database`, `Sap`, `Network`, `Timeout`; safe public code/message; sanitized diagnostic text. API no longer imports `Microsoft.Data.SqlClient` or knows `SapServiceLayerException`.

- [ ] **Step 1: Write failing redaction tests**

  Include exception text containing `Password=`, `Authorization`, token, cookie and connection string. Assert absence from HTTP response, audit message and audit stack text; assert trace id remains available.

- [ ] **Step 2: Define the provider-neutral exception contract**

  `DependencyFailureException` carries dependency kind, safe error code, retryability and optional sanitized diagnostic context. It must not accept or expose credentials, request headers or full connection strings.

- [ ] **Step 3: Centralize sanitization**

  Implement `IErrorSanitizer.Sanitize(string?)` with bounded output and the existing sensitive-name policy. Reuse it from SAP logging and global error auditing rather than maintaining divergent lists.

- [ ] **Step 4: Translate adapter failures at their boundaries**

  SAPIntegration translates Service Layer failures before crossing into Application/API. Expected Persistence failures translate provider exceptions without moving SQL types into Application. Preserve cancellation.

- [ ] **Step 5: Simplify the middleware**

  Map validation, dependency failure, timeout and unexpected failure to stable envelopes. Store sanitized diagnostics only; development responses may include safe diagnostic codes but never raw exception messages.

- [ ] **Step 6: Verify and commit**

  Run `GlobalExceptionBoundaryTests`, SAP transport tests, `ResultHttpMappingTests` and API composition tests.

  Commit with message `refactor: isolate and sanitize dependency failures`.

### Task 4: Focused-file decomposition without behavior change

**Files:**
- Split: `src/Backend/NuanSystem.Application/Features/Purchasing/PurchaseOrders/Commands/PurchaseOrderCommandHandlers.cs`
- Split: `src/Backend/NuanSystem.Api/Middleware/GlobalExceptionMiddleware.cs`
- Update: `.codex/FRAMEWORK-CATALOG.md` and `.codex/KNOWLEDGE-GRAPH.md` only if shared contracts changed
- Test: existing purchase, auth, error and architecture suites

**Interfaces:**
- Consumes: behavior completed by Tasks 1-3.
- Produces: one handler per file or cohesive workflow file; middleware restricted to pipeline control; mapper and audit writer separately testable.

- [ ] **Step 1: Freeze observable behavior with current tests**

  Run relevant suites before moving types and save the exact baseline counts.

- [ ] **Step 2: Split purchase handlers by use case**

  Create focused files for Create, Update, Delete, Workflow, attachments and related documents. Preserve namespaces, public type names and DI discovery.

- [ ] **Step 3: Split exception response mapping from audit writing**

  Keep `GlobalExceptionMiddleware` responsible for catch/control flow. Move response classification and sanitized audit construction to focused internal services without changing envelopes.

- [ ] **Step 4: Add architecture limits for touched components**

  Add tests that prevent Application from referencing provider namespaces, API from referencing `SqlException`/SAP transport exceptions, and the workflow TODO from returning.

- [ ] **Step 5: Run complete local verification**

  Run:

  - `dotnet build NuanSystem.sln --no-restore --verbosity minimal`
  - `dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj --no-build --filter "FullyQualifiedName!~Phase7Sql&FullyQualifiedName!~BusinessPartnerTransactionSqlTests"`
  - targeted authorized SQL tests for scripts `237` and `238`

  Expected: zero build warnings/errors; all non-laboratory tests pass; external SQL evidence reported separately.

- [ ] **Step 6: Review and commit**

  Confirm no changes under synchronization workers, Outbox/Inbox repositories or SAP export implementation. Commit with message `refactor: decompose backend hardening components`.

## Deferred synchronization backlog

This plan does not implement or refactor the following. They become a separate design and implementation plan when synchronization is authorized:

- Atomic Master Outbox publication, routing decisions and targets.
- Migration of remaining aggregates to LocalOutbox.
- Master/Sucursal relay, claims, recovery and reconciliation.
- ERP->SAP exporters and `SapSyncJobRunner.RunOutboxAsync`.
- Splitting `ISyncOutboxRepository` and `SyncOutboxRepository`.
- SAP/full synchronization load, failover and operational drills.
- MySQL support for synchronization repositories.

## Estimated sequence

- Increment 0: 1-2 engineering days.
- Increment 1: 5-7 engineering days plus the authorized SQL concurrency check.
- Increment 2: 4-6 engineering days plus Master integration testing.
- Increment 3: 3-5 engineering days.
- Increment 4: 2-3 engineering days.

Expected total for one engineer: approximately 3-4 weeks, excluding approval of business rules, environment access and release windows. Do not schedule by calendar date until Increment 0 revalidates the dirty worktree and available database fixtures.

## Completion criteria

- Purchase approvals are atomic, concurrency-safe and auditable.
- Unavailable SAP export never creates a false `SapPending` state.
- JWT revocation remains immediate while token validation uses one lightweight Master query.
- API/Application no longer depend on SQL/SAP exception types for public error behavior.
- Stored diagnostics are sanitized and correlated.
- Build and non-laboratory suite pass; SQL checks are evidenced separately.
- No synchronization implementation, worker activation, database deployment or production claim is included.

