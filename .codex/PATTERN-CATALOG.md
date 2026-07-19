# NuanSystem Pattern Catalog

## 1. Purpose

This catalog maps recurring NuanSystem problems to approved solution shapes. It answers “which lifecycle applies?” before individual files or controls are chosen.

A pattern is selected only after executing Framework Discovery. A nearby implementation is evidence, not permission to copy defects or cross domain boundaries.

## 2. Classification tree

```text
Requested behavior
  -> changes stock, money, prices, cash, document/external/sync state?
       Yes -> Operational / Document / Integration pattern
       No  -> administers master or configuration data?
                Yes -> Standard CRUD or Simple Catalog
                No  -> presents metrics? Dashboard/Monitor
                        -> guides finite steps? Wizard/Dialog
                        -> changes shared infrastructure? Framework Evolution
                        -> otherwise document the gap and select deliberately
```

## 3. Pattern P1 — Standard CRUD maintenance

### Use when

The feature administers master/configuration data and does not itself execute stock, money, pricing, cash, document, or external-system transitions.

### Vertical shape

```text
Entity/contract
  -> Application commands and queries
  -> Validation
  -> Repository contract and persistence
  -> API endpoints
  -> Typed frontend service
  -> List form
  -> Edit/consult form
  -> Form/operation permissions
  -> Menu registration when navigable
  -> Tests and SQL contract
```

Apply `$nuansystem-backend-architecture` and `$nuansystem-backend-crud` to the backend portion, then load endpoint, validation, persistence, tenant-security, SQL, and testing skills for the affected layers.

### Frontend defaults

- List derives from `BaseGridCrudListForm`.
- Edit/consult derives from `BaseEditForm`.
- Standard actions use `NuanActionButton`.
- Related catalogs use `NuanLookupEdit` when its contract fits.
- Transport uses a typed client over `INuanApiClient`.
- Designer-backed controls are declared explicitly in `.Designer.cs`.

### Reference families

- Geography: `CitiesForm` / `CityEditForm`, `CountriesForm` / `CountryEditForm`.
- General supplier catalogs: `SupplierGroupsForm` and related edit form family.
- Security: `RolesForm` / `RoleEditForm`.
- Financial catalogs: `BranchesForm` / `BranchEditForm`.
- Validated independent master: `CarriersForm` / `CarrierEditForm`, with its own backend, SQL, security, typed client, ViewModel, forms, audit, and tests and no BusinessPartners ownership.

Select the closest domain and lifecycle; do not choose solely by name.

Reference selection does not assign domain ownership. A form or vertical slice from another feature may be used to learn CRUD lifecycle, layout, permissions, or validation structure, but its entity, aggregate, table, repository, endpoints, synchronization, and domain-specific form remain out of scope unless the requirements explicitly establish that relationship.

### Required decisions

- company/global scope;
- logical versus physical delete;
- unique code/name rules;
- audit fields;
- permission keys;
- pagination/filter behavior;
- lookup dependencies;
- SQL procedure/contract conventions.

### Antipatterns

- frontend-only persisted field;
- endpoint-first design without use-case/repository inspection;
- local CRUD toolbar;
- inline SQL where stored procedures are required;
- business invariants enforced only in `BaseEditForm`.
- converting an independent master into a specialization of the reference entity to avoid creating its own vertical contract.

## 4. Pattern P2 — Simple auxiliary catalog

### Use when

The entity is small but independently administrable and referenced by other features.

### Rules

A “small table” is not an excuse to embed maintenance inside another module. If users administer it, it needs an explicit contract, permissions, persistence, API, and UI path appropriate to its scope.

Use the Standard CRUD pattern with a reduced field set, not a shortcut architecture.

### Lookup creation flow

```text
User selects Create on NuanLookupEdit
  -> verify create permission
  -> open the approved edit form
  -> persist through API
  -> reload lookup source
  -> select the newly created record
  -> preserve cancel/no-change behavior
```

## 5. Pattern P3 — Operational use case

### Use when

The action affects stock, money, pricing, cash, purchasing, auditable workflow state, cancellation/reversal, or external state.

### Required model

```text
Actor + command
  -> load authoritative state
  -> validate permissions/capabilities/invariants
  -> calculate server-side result
  -> concurrency/idempotency protection
  -> transaction boundary
  -> persist state and audit/outbox
  -> commit
  -> observable result
  -> retry/reversal/recovery behavior
```

Apply `$nuansystem-operational-usecase` with the specialized backend skills. A CRUD handler is not an acceptable substitute for posting, receiving, transferring, approving, canceling, reversing, or synchronizing.

### Frontend role

The frontend gathers intent, presents authoritative responses, controls interaction state, and handles validation/errors. It does not decide final stock, total, price, balance, document state, or synchronization success.

### Required decisions

- valid state transitions;
- transaction boundary;
- concurrency token/strategy;
- idempotency key or duplicate protection;
- audit content;
- partial failure behavior;
- retry versus reversal;
- external integration timing;
- capability flags.

### Antipatterns

- treating the process as generic CRUD;
- trusting UI totals/status;
- multiple uncoordinated repository writes;
- success returned before durable state/outbox;
- destructive “cancel” without reversal semantics.

## 6. Pattern P4 — Business document/master-detail

### Use when

The feature owns a header, detail lines, totals, states, and a lifecycle such as draft, post, cancel, or reverse.

### Shape

```text
Header + lines input
  -> normalize and validate identifiers/quantities
  -> reload prices/taxes/stock/rules
  -> calculate totals server-side
  -> validate state transition
  -> atomic persistence
  -> audit and integration event/outbox
  -> return authoritative document
```

### UI shape

Use an explicit designer-backed structure for header, detail grid, totals, actions, and status. A document editor does not inherit CRUD bases merely because it can be saved.

### Quality focus

Line identity, rounding, taxes, totals, duplicate submission, state transitions, cancellation/reversal, and integration recovery.

## 7. Pattern P5 — Dashboard or monitor

### Use when

The feature summarizes state and supports inspection rather than authoritative mutation.

### Preferred components

- `NuanKpiCardControl` for KPI summaries.
- `NuanDataGridControl` for detailed monitored rows where its capabilities fit.
- Typed frontend services over `INuanApiClient`.
- Explicit loading, empty, degraded, and error states.

### Reference

`src/Frontend/NuanSystem.WinForms.Forms/Sync/SyncMonitorForm.Designer.cs` demonstrates corporate KPI and grid components in a monitor surface.

### Antipatterns

- expensive synchronous work on the UI thread;
- KPI values computed from incomplete client pages;
- silently stale data;
- custom painted cards/grids duplicating corporate controls.

## 8. Pattern P6 — Dialog, selector, or wizard

### Dialog/selector

Use for a bounded decision that returns a clear result and can be cancelled without side effects. Reuse corporate lookups and buttons. Do not hide a multi-step business transaction inside a modal.

### Wizard

Use when steps have real sequencing, validation boundaries, and review/confirmation. Define:

- state owned by the wizard;
- validation per step;
- back/cancel semantics;
- final server-side command;
- recovery after failure.

A sequence of tabs is not automatically a wizard.

## 9. Pattern P7 — API/client contract

### Backend

Endpoint shape follows the Application use case, not the screen layout. Preserve validation, authorization, tenant context, cancellation, consistent errors, and response contracts.

### Frontend

Create or extend a typed feature client that depends on `INuanApiClient`. Forms consume the feature service/client; they do not create `HttpClient`, attach JWT/company headers, or deserialize transport errors ad hoc.

### Contract change tree

```text
Contract change
  -> additive and backward compatible?
       Yes -> update DTO/client/tests and verify consumers
       No  -> identify all consumers, version/migrate deliberately, validate end-to-end
```

## 10. Pattern P8 — Integration and synchronization

### Use when

Data crosses NuanSystem and SAP Business One, BEAS, or another external system.

### Shape

```text
Authoritative local transaction
  -> durable integration intent/outbox
  -> tenant-aware worker/process
  -> mapping and external validation
  -> idempotent external call
  -> success/failure state and audit
  -> retry/backoff or manual recovery
```

### Required decisions

- source of truth;
- mapping version;
- external identifier;
- idempotency;
- session lifecycle;
- transient/permanent error classification;
- retry limits;
- observability;
- reconciliation.

### Antipatterns

- external calls directly from WinForms;
- losing local transaction because SAP is unavailable;
- infinite retry without visibility;
- marking synchronized before confirmed success;
- tenant/company context inferred from client input alone.

## 11. Pattern P9 — Shared framework evolution

### Use when

Changing a base form, corporate control, visual resource, transport, shared result, or cross-cutting service.

### Mandatory process

1. Search and map all consumers.
2. State the missing capability in the current public contract.
3. Prefer backward-compatible configuration.
4. Define Designer/serialization impact for controls.
5. Define migration and compatibility behavior.
6. Update Framework Catalog and Knowledge Graph.
7. Validate representative consumers from different domains.

### Antipatterns

- changing defaults to fix one screen;
- feature-specific behavior in a shared component;
- breaking public properties/events without migration;
- adding a second framework instead of evolving the first.

## 12. Pattern selection record

Before implementation, record:

```text
Selected pattern:
Why it applies:
Reference implementation(s):
Differences from reference:
Framework components:
Affected layers:
Risk:
Validation:
```

## 13. Pattern completion gate

A pattern is correctly applied only when:

- classification matches the business effect;
- all vertical layers are classified;
- corporate components are reused where applicable;
- state/transaction/security semantics are explicit;
- representative evidence is named;
- applicable review gates pass.
