# NuanSystem Engineering Review Checklist

## 1. Purpose

This checklist executes the quality gates required by the Engineering Constitution. Apply only relevant gates, but classify every gate as:

- **Validated**
- **Not validated**
- **Not applicable**
- **Blocked**

A checkbox without evidence is not validation.

## 2. Evidence header

```text
Request:
Branch:
Scope reviewed:
Selected pattern:
Risk:
Reference implementations:
Validation environment:
```

## 3. Gate R1 — Scope and completeness

- [ ] The requested outcome is implemented, not merely planned.
- [ ] Explicit exclusions are respected.
- [ ] The diff contains no unrelated cleanup or user work.
- [ ] No TODO, fake data, empty handler, swallowed error, or disabled rule remains silently.
- [ ] Every affected layer is marked Change, Verified unchanged, Not applicable, or Blocked.
- [ ] New/modified files use established naming and folders.

**Evidence:** changed-file list and diff inspection.

## 4. Gate R2 — Discovery and pattern

- [ ] Framework Discovery was executed before new design.
- [ ] Exact symbols, shared/common areas, same-domain examples, tests, and docs were searched.
- [ ] At least one representative implementation was inspected; two when available for medium/high risk.
- [ ] The selected pattern is named and justified.
- [ ] Alternatives and material differences are recorded.
- [ ] Explicit user-approved domain ownership, independence, and exclusions were preserved.
- [ ] The selected reference states what may be reused and does not redefine the requested aggregate.
- [ ] No numeric confidence/similarity score was invented.

**Evidence:** paths/symbols inspected and selection record.

## 5. Gate R3 — Architecture and reuse

- [ ] Dependency direction remains valid.
- [ ] Frontend does not access SQL, HANA, SAP, DI API, or Service Layer directly.
- [ ] Business truth remains in backend/domain/application layers.
- [ ] No parallel base form, helper, wrapper, service, repository abstraction, result type, or pipeline was added without an approved gap.
- [ ] Exact/configurable components were reused before extension.
- [ ] Shared extension is backward compatible or has a migration plan.
- [ ] Framework Catalog and Knowledge Graph were updated if a shared contract changed.

**Evidence:** dependency/diff review and consumer search.

## 6. Gate R4 — Domain and operational safety

- [ ] CRUD versus operational classification is correct.
- [ ] Independent masters have their own vertical contract; no unrelated aggregate, table, API, sync flow, ViewModel, typed client, or concrete form was reused by convenience.
- [ ] Authoritative state is reloaded before operational decisions.
- [ ] Stock, money, price, totals, balances, and document state are not trusted from UI input.
- [ ] State transitions are explicit.
- [ ] Transaction boundary is explicit.
- [ ] Concurrency and duplicate submission are addressed.
- [ ] Idempotency/retry/reversal/recovery behavior is defined where relevant.
- [ ] Audit behavior is present and meaningful.
- [ ] Capability/configuration rules are used instead of hard-coded business-type branches.

**Evidence:** use-case/handler/domain/transaction tests or inspection.

## 7. Gate R5 — Security and multi-company

- [ ] Authentication is required where appropriate.
- [ ] Backend authorization exists; UI visibility alone is not security.
- [ ] Form, operation, and field permissions are preserved.
- [ ] Endpoint policy codes exist in `Permissions`, approved roles have `RolePermissions`, and form-operation grants are not mistaken for API permission grants.
- [ ] Runtime authorization after a new permission grant was tested with a freshly issued token.
- [ ] Active company/tenant context is propagated through approved infrastructure.
- [ ] Queries and writes are isolated by the correct company scope.
- [ ] Audit identity and company are authoritative.
- [ ] No JWT, company header, connection string, or secret is hard-coded.

**Evidence:** endpoint/policy/session/persistence inspection and security tests.

## 8. Gate R6 — Frontend framework

- [ ] Standard CRUD list uses `BaseGridCrudListForm` or the deviation is justified.
- [ ] Standard edit/consult uses `BaseEditForm` or the deviation is justified.
- [ ] Record-level CRUD history uses `RecordHistoryForm` and its refresh/filter/error lifecycle rather than a local message or duplicated history UI.
- [ ] Standard actions use `NuanActionButton`.
- [ ] Related catalog lookup uses `NuanLookupEdit` when its contract fits.
- [ ] A direct `LookUpEdit` is used only for a small fixed catalog with no related maintenance; it persists a stable code and is closed against free text.
- [ ] Reusable grid requirements use `NuanDataGridControl` when appropriate.
- [ ] KPI summaries use `NuanKpiCardControl`.
- [ ] Colors/logo use `BrandResources`.
- [ ] Typography uses `AppTypography`.
- [ ] `FormStyler` is not used to build or reposition layout.
- [ ] Feature clients use `INuanApiClient`/`NuanApiClient`; forms do not create `HttpClient`.

**Evidence:** source and Designer inspection.

## 9. Gate R7 — WinForms UX and Designer safety

- [ ] Designer-backed controls and layout are explicit in `.Designer.cs`.
- [ ] `InitializeComponent` remains serialization-friendly.
- [ ] `BeginInit`/`EndInit`, `SuspendLayout`/`ResumeLayout`, disposal, and component ownership are balanced.
- [ ] Runtime code binds data/behavior rather than reconstructing layout.
- [ ] Compact CRUD rows follow the approved 28 px top-to-top cadence (6 px visible gap for 22 px editors), unless a documented form-family exception applies.
- [ ] A post-Designer semantic diff preserves required editor properties and contains only intentional `.resx`/serialization changes.
- [ ] Naming follows the existing form family.
- [ ] Read-only/consult mode covers all editable actions.
- [ ] Loading, empty, error, and disabled states are visible.
- [ ] Create-from-lookup checks permission, refreshes, and selects the created record.
- [ ] Grid selection, double-click, paging, export, and personalization behavior remain consistent where applicable.
- [ ] Keyboard/tab order, anchoring/docking, minimum resolution, and localization/text are inspected.
- [ ] Visual Studio Designer was opened when the environment permits.

**Evidence:** Designer inspection, build, screenshots/manual check where available.

## 10. Gate R8 — API and frontend services

- [ ] Endpoints represent use cases rather than form internals.
- [ ] Request/response contracts are coherent and not duplicated.
- [ ] Validation and consistent errors are preserved.
- [ ] Cancellation is propagated where supported.
- [ ] Typed feature clients depend on approved transport.
- [ ] No manual auth/company headers or ad hoc transport error parsing was introduced.
- [ ] Contract changes identify and validate all consumers.

**Evidence:** endpoint/client/DTO tests and search results.

## 11. Gate R9 — Persistence and SQL

- [ ] Repository contract matches Application needs.
- [ ] Established stored-procedure conventions are followed where required.
- [ ] SQL is parameterized and company-scoped.
- [ ] Script is idempotent where deployment requires it.
- [ ] Keys, foreign keys, uniqueness, defaults, indexes, audit, and logical deletion are explicit.
- [ ] Result column names/types match Dapper/DTO contracts.
- [ ] Transaction ownership is correct.
- [ ] Data migration avoids silent loss and has recovery considerations.
- [ ] SQL Server/HANA syntax is not mixed.

**Evidence:** scripts/procedures inspected or executed and persistence tests.

## 12. Gate R10 — Integration and synchronization

- [ ] Source of truth is named.
- [ ] Mapping and external identifiers are explicit.
- [ ] Integration intent is durable where required.
- [ ] Calls are tenant-aware and idempotent.
- [ ] Transient and permanent errors are distinguished.
- [ ] Retry is bounded and observable.
- [ ] Success is recorded only after confirmed external success.
- [ ] Manual recovery/reconciliation exists for terminal failure.
- [ ] Sensitive external details are not exposed to UI/logs.

**Evidence:** worker/outbox/mapping tests or trace inspection.

## 13. Gate R11 — Validation

### Build

- [ ] Relevant project/solution build executed.
- [ ] New warnings/errors caused by the change are resolved.
- [ ] If not executed, the reason is explicit.

### Tests

- [ ] Targeted unit tests executed.
- [ ] Relevant integration/contract tests executed.
- [ ] Regression tests added for a defect or contract change.
- [ ] If not executed, the reason is explicit.

### Static/manual checks

- [ ] Referenced paths and symbols exist.
- [ ] Final diff reviewed.
- [ ] Designer inspected/opened when applicable.
- [ ] SQL/integration checks performed when applicable.
- [ ] Failure and cancellation paths inspected.

Record exact commands or inspection targets and outcomes.

## 14. Gate R12 — Delivery evidence

- [ ] Summary states actual completed changes.
- [ ] Changed files are listed accurately.
- [ ] Validation is separated from assumptions.
- [ ] Blockers and residual risks are explicit.
- [ ] Pending work is not hidden.
- [ ] No PR/merge/deployment is claimed unless it occurred.
- [ ] Branch and commits are named when publishing work.

## 15. Review result template

```text
Gate results:
- R1 Scope: Validated | ...
- R2 Discovery: Validated | ...
- R3 Architecture/reuse: Validated | ...
- R4 Domain/operational: Not applicable | ...
- R5 Security/tenant: Validated | ...
- R6 Frontend framework: Validated | ...
- R7 Designer/UX: Not applicable | ...
- R8 API/services: Not applicable | ...
- R9 Persistence/SQL: Not applicable | ...
- R10 Integration/sync: Not applicable | ...
- R11 Validation: Validated | ...
- R12 Delivery evidence: Validated | ...

Residual risks:
Blocked checks:
Decision: Accept | Revise | Blocked
```

## 16. Acceptance rule

- **Accept** only when all applicable gates are Validated and residual risk is understood.
- **Revise** when any applicable gate fails.
- **Blocked** when required evidence cannot be obtained and safe completion is impossible.

## 17. Iteration 4 integration evidence

For SAP or synchronization work, R10 also requires:

- [ ] SAP transport and Matriz-Sucursal replication are separate stages.
- [ ] Company/session isolation and credential redaction are verified.
- [ ] Entity registration agrees with handler/producer/applier, DI, SQL, settings, and tests.
- [ ] Locks have owner, timeout, release, and expired-lock recovery.
- [ ] Watermarks advance only after durable success.
- [ ] Per-target and aggregate replication states cannot contradict each other.
- [ ] Retryable, ignored, and terminal/DeadLetter outcomes are bounded and distinct.
- [ ] Skeleton, disabled, `NotImplemented`, and pilot paths are not called production-complete.
- [ ] Manual recovery requires permission, reason, and audit.

External SAP connectivity not exercised must be reported `Not validated`.
- “Not validated” is never equivalent to “Validated.”

## 18. Iteration 5 SRI evidence

For SRI queue or worker work, R4, R5, R9, R10, R11, and R12 additionally require:

- [ ] Pilot direction, provider/environment, idempotency, XML storage, retention, privacy, and document relationship are approved or explicitly blocked.
- [ ] SRI, SAP, and Matriz-Sucursal use separate queues, workers, and ownership boundaries.
- [ ] Durable enqueue commits before remote work and enforces tenant isolation plus database uniqueness.
- [ ] State transitions are explicit, atomic, legal, and concurrency protected.
- [ ] Claims have owner/expiry/recovery; two workers cannot process one job concurrently.
- [ ] Remote calls occur outside SQL transactions; retry is bounded and DeadLetter/manual recovery is visible and audited.
- [ ] Secrets, certificates, XML, payload, and taxpayer data follow redaction/access/retention policy.
- [ ] Permission changes were checked with a renewed JWT where runtime authorization is claimed.
- [ ] XML integrity and duplicate prevention were validated at the selected storage boundary.
- [ ] A real approved non-production SRI round trip exists before end-to-end success is claimed.
- [ ] Protected XML download returns bytes with `application/xml`, safe filename and `Cache-Control: no-store`.
- [ ] Every successful download is audited without XML/full access key and without changing `Authorized` or duplicating the document.
- [ ] Monitor projections omit full access keys and server paths; WinForms uses typed `INuanApiClient` only.
- [ ] Scripts `118`/`119`, renewed permissions, tenant isolation, API download and Designer opening are reported separately from automated evidence.

At Iteration 5 blueprint stage, implementation gates must be reported `Not validated`, not inferred from architecture documents.

Phase 5.5 passed these gates on 2026-07-21: scripts `118`/`119` were deployed idempotently, renewed-token permission profiles and tenant isolation were exercised, two protected downloads preserved one document and added two audits, and the Designer was reviewed. Use `docs/operations/SRI-DOCUMENT-MONITOR.md` as the detailed sanitized evidence; do not infer broader SRI operations from this approval.

## 19. Iteration 6 WorkerHeartbeat forward-repair gate

- [ ] `120` compara tipo, longitud y nullability antes de cada `ALTER COLUMN`; una segunda o tercera ejecucion no altera metadata correcta.
- [ ] La reparacion de `WorkerType`, `HostName` o `WorkerInstance` detecta primero la necesidad y protege `UX_WorkerHeartbeat_LogicalIdentity` dentro de una transaccion con `XACT_ABORT`.
- [ ] El indice final es unico, no clusterizado, con claves `WorkerType`, `HostName`, `WorkerInstance` y filtro `WorkerInstance IS NOT NULL`.
- [ ] Defaults y checks dependientes se conservan cuando son correctos y se restauran con el mismo contrato cuando una alteracion real los requiere.
- [ ] `122` funciona como forward repair sin borrar `20260721.120`, filas heartbeat, identidad `InstanceName`, permisos u operaciones.
- [ ] El inicializador Master ejecuta `122` inmediatamente despues de `120`.
- [ ] Pruebas contractuales cubren instalacion previa a `120`, segundo/tercer pase, estado parcial, historia unica, metadata, indice, defaults/checks, SAP y ausencia de SQL destructivo contra datos.
- [ ] Una revalidacion SQL autorizada confirma el segundo pase de `120` y dos pases de `122` antes de ejecutar `121`.

Estado al cierre de Iteracion 6: los pases autorizados de `120`, `122` y `121` aprobaron idempotencia, metadata, compatibilidad SAP y preservacion de evidencia. La validacion runtime controlada aprobo SCM/ACL, TLS, JWT, lifecycle, heartbeat, mutex, Event Log, monitor, Designer y update/rollback, con limpieza final completa. Usar `docs/operations/SRI-WORKER-OPERATIONS.md` como fuente de evidencia; no generalizar este resultado a habilitacion productiva permanente.

## .NET 10 release artifact gate

- [ ] Working tree limpio y commit fuente inmutable.
- [ ] API, SyncWorker, MasterBranchSyncWorker, SriWorker y WinForms publicados
      por separado.
- [ ] `Release`, `win-x64`, framework-dependent, sin trimming ni single-file.
- [ ] InformationalVersion, FileVersion y AssemblyVersion verificadas.
- [ ] Release manifest, dependency inventory y SHA-256 por archivo presentes.
- [ ] `appsettings.Local.json`, secretos, certificados, logs, backups y payloads
      sensibles ausentes.
- [ ] Todos los workers y retries publicados permanecen deshabilitados.
- [ ] Pilot1 y pilot2 tienen versiones, commits y manifests distinguibles.
- [ ] Rollback pilot1 -> pilot2 -> pilot1 preserva los hashes originales.
- [ ] No se declara instalación SCM, runtime o promoción productiva si no fue
      ejecutada y evidenciada por separado.

## Iteration 7 production readiness gate

- [ ] D7-01 through D7-10 have an explicit owner and evidence state.
- [ ] .NET 10 and the selected release mode point to executed evidence.
- [ ] Host, gMSA and vault values are production decisions, not pilot defaults.
- [ ] No secret value, credential, token or private key appears in the decision record.
- [ ] Push alerts and support coverage name routing, acknowledge and escalation.
- [ ] RPO/RTO are based on a full coordinated restore, not `VERIFYONLY`.
- [ ] Retention and legal hold are approved before any archive/purge/delete design.
- [ ] DEMO canary names environment, action, window, limits, owners and abort criteria.
- [ ] Remigio, Cañaris and other tenants are excluded from the first canary.
- [ ] `QueueId=10004` is preserved as protected evidence and is not a fixture.
- [ ] `SriWorker:Enabled=false` remains the baseline until an independent activation change.

Development closure approved on 2026-07-25:

- [x] Current computer is development-only and has no permanent SRI Worker installation.
- [x] No Active Directory/gMSA is claimed; production identity remains blocked.
- [x] Development secrets remain in ignored local configuration with strict TLS.
- [x] Development observability is logs, Windows Event Log and WinForms Monitor.
- [x] Development support is business-hours only, without 24x7 claims.
- [x] RPO/RTO and full restore are deferred until a production host exists.
- [x] XML and audit retention is indefinite; automatic and manual deletion remain forbidden.
- [x] Runtime remains singleton and disabled.
- [x] Every tenant and every real SRI call requires independent explicit authorization.
- [x] A future canary is limited to DEMO; Remigio and Cañaris remain excluded.

These checked development gates do not satisfy the unchecked production gates.

## Iteration 8 transactional outbox gate

- [ ] Entity mutation and `LocalOutbox` intent share one tenant transaction.
- [ ] No Master, SAP, SRI or HTTP call runs inside that tenant transaction.
- [ ] `EventId` is stable from local intent through Master promotion.
- [ ] Master promotion is idempotent and detects payload/identity conflicts.
- [ ] `SyncOutbox`, routing decisions, targets and audit cannot commit partially.
- [ ] Local claims have owner, expiry, recovery and bounded retry.
- [ ] A crash after Master commit is recovered without duplicate outbox/targets.
- [ ] The CRUD returns success after durable local commit and does not report a
      false failure solely because Master is unavailable.
- [ ] A migrated handler does not also use direct `ISyncEventPublisher`.
- [ ] `BusinessPartner` is validated before independently migrating `Item`,
      `Warehouse` or another entity.
- [ ] Relay and worker remain disabled by default; SQL/runtime require separate
      authorization and evidence.
- [ ] The entity dependency order is explicit before enabling a profile.
- [ ] Tenant and Master scripts are registered and executed twice only after
      verified backups and explicit database authorization.
- [ ] DI registrations and database initializers include every new writer and
      migration.
- [ ] Code collisions are terminal, tombstones reserve identity, and no
      automatic adoption occurs.
- [ ] Runtime fixtures are identifiable, isolated to authorized tenants and
      removed after validation.
