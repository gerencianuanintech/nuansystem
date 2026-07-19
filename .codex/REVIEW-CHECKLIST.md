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
- [ ] Active company/tenant context is propagated through approved infrastructure.
- [ ] Queries and writes are isolated by the correct company scope.
- [ ] Audit identity and company are authoritative.
- [ ] No JWT, company header, connection string, or secret is hard-coded.

**Evidence:** endpoint/policy/session/persistence inspection and security tests.

## 8. Gate R6 — Frontend framework

- [ ] Standard CRUD list uses `BaseGridCrudListForm` or the deviation is justified.
- [ ] Standard edit/consult uses `BaseEditForm` or the deviation is justified.
- [ ] Standard actions use `NuanActionButton`.
- [ ] Related catalog lookup uses `NuanLookupEdit` when its contract fits.
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
- “Not validated” is never equivalent to “Validated.”
