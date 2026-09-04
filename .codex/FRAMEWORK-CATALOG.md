# NuanSystem Framework Catalog

## 1. Purpose

This catalog is the authoritative inventory of reusable NuanSystem engineering components. It supports discovery and prevents parallel infrastructure.

Authority: `ENGINEERING-CONSTITUTION.md` > `ENGINEERING-KERNEL.md` > this catalog > specialized skills > local implementation.

## 2. How to use the catalog

Before creating a base class, shared control, helper, service, wrapper, or framework convention:

1. Search this catalog.
2. Read the referenced source file.
3. Inspect at least one representative consumer.
4. Prefer public contracts over copying internals.
5. Record reuse, extension, or justified gap.
6. Update this catalog when a shared contract is added, deprecated, or materially changed.

Status values:

- **Active/preferred** — default for covered needs.
- **Active/specialized** — use only for its documented scenario.
- **Legacy** — preserve where present; do not copy into new work.
- **Deprecated** — migrate away when scope permits.

## 3. Frontend base forms

### 3.1 `BaseGridCrudListForm`

- **Location:** `src/Frontend/NuanSystem.WinForms.Forms/Common/BaseGridCrudListForm.cs`
- **Status:** Active/preferred for standard grid-based CRUD lists.
- **Responsibility:** Supplies the corporate CRUD list lifecycle through a designer-owned `NuanDataGridControl`.
- **Contract observed:** `BaseGridCrudListForm.Designer.cs` creates `NuanDataGridControl`; the base form exposes its inner `GridControl` and `GridView`, supports typed data binding and selection, implements edit, copy, delete, consult, and history hooks, and provides column configuration/personalization plus Excel/PDF/JSON/XML export. When hosted by `MainForm`, CRUD actions and the initial refresh fail closed until the active `FormKey` operation access is loaded; a denied or unavailable operation response never falls back to coarse session permissions. The Ribbon refreshes operation access whenever an open tab becomes active, without forcing a data reload, shows only operations the active form can execute, and hides groups with no visible actions. When legacy aliases coexist, the explicit UI operations `NEW` and `EDIT` take precedence over the API/process aliases `CREATE` and `UPDATE`, including when the UI operation is denied. `Consultar` is created with its corporate SVG and also uses it as a client fallback when security metadata has no valid icon. Remote CRUD sources opt into the contained grid's server paging through `EnableServerPaging`, handle `NuanGrid.PageRequested`, and bind each response with `SetPagedGridData`; the legacy local paging path remains the default for existing consumers. A server-filtered list may additionally call `EnableServerFind`, passing its async reload callback; the base owns Find normalization, debounce, DevExpress event wiring, and disposal, while the feature callback applies the text to its typed API filter, resets the page, and reloads. This capability remains opt-in so forms without a backend search contract retain their existing behavior.
- **Use when:** a maintenance list has standard CRUD commands, selection, permissions, export, and configurable columns.
- **Do not use when:** the screen is an operational transaction, document editor, dashboard, or workflow whose lifecycle is not CRUD.
- **Extension rule:** derive a feature form and override documented hooks such as grid configuration and CRUD operations. Do not fork the base lifecycle.
- **Representative consumers:**
  - `src/Frontend/NuanSystem.WinForms.Forms/Definitions/General/Countries/CountriesForm.cs`
  - `src/Frontend/NuanSystem.WinForms.Forms/GeneralSupplier/SupplierGroups/SupplierGroupsForm.cs`
  - `src/Frontend/NuanSystem.WinForms.Forms/Security/Roles/RolesForm.cs`
- **Antipatterns:** parallel CRUD toolbar, duplicate export engine, local grid personalization service, or direct database/API orchestration inside the form.

### 3.2 `BaseEditForm`

- **Location:** `src/Frontend/NuanSystem.WinForms.Forms/Common/BaseEditForm.cs`
- **Status:** Active/preferred for standard CRUD edit/consult forms.
- **Responsibility:** Supplies validation, save lifecycle, persistence hook, UI exception handling, and read-only consult mode.
- **Contract observed:** `Validator`, `ValidateForm`, `BuildRequest`, `PersistAsync`, `Save`, warning/error helpers, and `BeginReadOnlyMode`.
- **Use when:** creating or editing a standard master/catalog record.
- **Do not use when:** the screen requires an operational transaction boundary, complex document workflow, wizard state machine, or monitor-only experience.
- **Extension rule:** override validation/request/persistence hooks; keep business validation authoritative in the backend.
- **Representative consumers:**
  - `src/Frontend/NuanSystem.WinForms.Forms/Geography/Countries/CountryEditForm.cs`
  - `src/Frontend/NuanSystem.WinForms.Forms/FinancialCatalogs/Branches/BranchEditForm.cs`
  - `src/Frontend/NuanSystem.WinForms.Forms/Security/Roles/RoleEditForm.cs`
- **Antipatterns:** duplicate save/read-only framework, form-owned SQL/SAP access, or authoritative business rules in UI validation.

### 3.3 `RecordHistoryForm`

- **Location:** `src/Frontend/NuanSystem.WinForms.Forms/Audit/RecordHistoryForm.cs` and its Designer.
- **Status:** Active/preferred for record-level audit history opened from CRUD forms.
- **Responsibility:** Displays audit changes in a read-only grid with refresh, action/user filters, previous/new values, user formatting, and corporate presentation.
- **Contract:** Accept a title, record description, and an async loader returning `SecurityChangeItem` values.
- **Use when:** an inherited History action displays field-level changes for a selected record.
- **Representative consumers:** `SettingsForm`, `SecurityDocumentSeriesForm`, `BusinessPartnersForm`, and `CarriersForm`.
- **Antipatterns:** history in `XtraMessageBox`, feature-local history grids/forms, arbitrary 20-row truncation, or loading before the shared form owns refresh/error behavior.

## 4. Corporate controls

### 4.1 `NuanActionButton`

- **Location:** `src/Frontend/NuanSystem.WinForms.Controls/Buttons/NuanActionButton.cs`
- **Status:** Active/preferred for standard actions.
- **Responsibility:** Corporate action button built on `SimpleButton` with semantic kind, text, icon, colors, size, and automatic style behavior.
- **Use when:** an action maps to an existing `NuanActionButtonKind` or corporate action style.
- **Do not use when:** a specialized third-party control is technically required and no action-button contract fits; document the gap.
- **Extension rule:** configure `ButtonKind` and public styling properties before subclassing.
- **Representative consumers:**
  - `src/Frontend/NuanSystem.WinForms.Forms/Common/BaseEditForm.Designer.cs`
  - `src/Frontend/NuanSystem.WinForms.Forms/Sync/SyncMonitorForm.Designer.cs`
- **Antipatterns:** raw `SimpleButton` for a standard action, hard-coded colors/icons, or a parallel button helper.

### 4.2 `NuanLookupEdit`

- **Location:** `src/Frontend/NuanSystem.WinForms.Controls/Lookups/NuanLookupEdit.cs`
- **Status:** Active/preferred for corporate lookups requiring clear/create/edit affordances.
- **Responsibility:** `LookUpEdit` with managed clear/create/edit buttons and `ClearButtonClick` / `CreateButtonClick` / `EditButtonClick` events.
- **Contract observed:** `CreateButtonEnabled`, `EditButtonEnabled`, `ClearButtonEnabled`, and `RefreshButtons`.
- **Use when:** selecting a related catalog and optionally creating, editing, or clearing the relation.
- **Do not use when:** a read-only label, fixed enum editor, or specialized search/grid lookup is required and the control cannot meet it.
- **Extension rule:** configure the control and wire permission-aware events; refresh the source and preserve/select the created or edited record.
- **Representative consumers:**
  - `src/Frontend/NuanSystem.WinForms.Forms/GeneralInventory/ItemGroups/ItemGroupEditForm.Designer.cs`
  - `src/Frontend/NuanSystem.WinForms.Forms/Security/Users/UserEditForm.Designer.cs`
- **Antipatterns:** ad hoc plus button beside every lookup, duplicate clear-button logic, or creation without permission checks.

### 4.3 Direct closed `LookUpEdit`

- **Status:** Approved low-level pattern for a small fixed catalog; not a replacement for `NuanLookupEdit`.
- **Use when:** values are an approved local/contractual set with no related maintenance, remote loading, create, clear, or refresh lifecycle.
- **Contract:** serialize the combo button and `TextEditStyle = DisableTextEditor`, bind typed code/display values, and persist the stable code rather than caption/index.
- **Representative consumer:** `src/Frontend/NuanSystem.WinForms.Forms/Carriers/CarrierEditForm.Designer.cs` for SRI identification-type codes `04`, `05`, and `06`.
- **Antipatterns:** free text, duplicated related-create controls, persistence by caption/index, or frontend-only enforcement of a persisted closed set.

### 4.3.1 `NuanToggleSwitch`

- **Location:** `src/Frontend/NuanSystem.WinForms.Controls/Editors/NuanToggleSwitch.cs`
- **Status:** Active/preferred for editable or read-only Boolean fields presented as Sí/No.
- **Responsibility:** Extends the DevExpress `ToggleSwitch` interaction and accessibility contract with a uniform compact rendering, explicit Sí/No state text, configurable active/inactive/text/thumb colors, and a `Checked` compatibility property.
- **Use when:** a form presents a Boolean business field whose state must remain immediately legible.
- **Extension rule:** assign the active, inactive, and text colors from the authoritative form palette such as `BrandResources`; use the inherited `ReadOnly`, `IsOnChanged`, keyboard, and focus behavior rather than adding a second interaction layer.
- **Representative consumer:** `src/Frontend/NuanSystem.WinForms.Forms/Definitions/Inventory/ItemGroups/ItemGroupEditForm.Designer.cs` for Activo and Grupo del sistema.
- **Antipatterns:** raw per-form toggle styling, hard-coded feature brand colors, checkbox captions that hide the Boolean state, or custom click/keyboard handling outside the control.

### 4.4 `NuanDataGridControl`

- **Location:** `src/Frontend/NuanSystem.WinForms.Controls/Grids/NuanDataGridControl.cs`
- **Status:** Active/preferred for reusable feature grids.
- **Responsibility:** Corporate grid user control with local or server-driven pagination, find panel, multi-selection, selection checkboxes, column configuration, status badges, export, and column customization.
- **Contract observed:** exposes inner `GridControl`/`GridView`; `SetData<T>`, focused/selected row helpers, `ConfigureColumns`, `SetStatusBadgeProvider`, `ApplyStandardGridStyle`, and `ExportVisibleColumns`.
- **Use when:** a feature needs a reusable grid surface outside the inherited standard CRUD list or needs the control's paging/search/selection contract.
- **Do not use when:** `BaseGridCrudListForm` already owns the complete standard CRUD list lifecycle and already contains this control internally; do not add a second `NuanDataGridControl` without evidence.
- **Representative consumers:**
  - `src/Frontend/NuanSystem.WinForms.Forms/Sync/SyncMonitorForm.Designer.cs`
  - `src/Frontend/NuanSystem.WinForms.Forms/Sync/SyncOutboxDetailForm.Designer.cs`
- **Server paging contract:** Bind an already paged result with `SetPagedData`; handle `PageRequested` only to execute the remote query and bind the returned page. First/previous/next/last controls, page-size selection, page count, and total count remain owned by `NuanDataGridControl`.
- **Antipatterns:** new grid wrapper, feature-local pagination/export framework, form-owned previous/next buttons, or bypassing its public contract to reproduce behavior.

### 4.5 `NuanKpiCardControl`

- **Location:** `src/Frontend/NuanSystem.WinForms.Controls/Kpi/NuanKpiCardControl.cs`
- **Status:** Active/specialized for KPI summaries.
- **Responsibility:** Corporate KPI card with title, value, description, icon, colors, border, shadow, style presets, and automatic numeric font fitting that preserves complete long counters without ellipsis.
- **Use when:** a dashboard/monitor presents a compact metric summary.
- **Do not use when:** displaying ordinary field values or editable data.
- **Representative consumers:** `src/Frontend/NuanSystem.WinForms.Forms/Sync/SyncMonitorForm.Designer.cs` and `src/Frontend/NuanSystem.WinForms.Forms/SriTxtImports/SriTxtImportForm.Designer.cs`.
- **Antipatterns:** custom painted KPI panels or copied KPI styling.

### 4.5.1 `NuanOperationalKpiCardControl`

- **Location:** `src/Frontend/NuanSystem.WinForms.Controls/Kpi/NuanOperationalKpiCardControl.cs`
- **Status:** Active/specialized for operational KPI summaries.
- **Responsibility:** Independent option-3 KPI card with icon medallion, title, fitted value, unit, and status badge; numeric value/unit groups align right, text values align left, and the unit shares the value font and accent color. It owns presentation only and never calculates business state.
- **Use when:** an operational or master-data summary needs the larger option-3 visual language and its compact responsive form.
- **Do not use when:** the execution/monitor card contract of `NuanKpiCardControl` is required, or the value is editable.
- **Representative consumer:** `src/Frontend/NuanSystem.WinForms.Forms/InventoryItems/ItemEditForm.Designer.cs` for the item operational and commercial summary.
- **Antipatterns:** adding operational properties back into `NuanKpiCardControl`, computing authoritative health/status inside the control, or copying its paint logic into forms.

## 5. Visual resources

### 5.1 `BrandResources`

- **Location:** `src/Frontend/NuanSystem.WinForms.Forms/Common/BrandResources.cs`
- **Status:** Active/authoritative for brand colors and logo loading in the Forms project.
- **Responsibility:** Corporate background, surface, primary, text, border, semantic state, customer/supplier accent colors, logo loading, and rounded geometry.
- **Use when:** a form or shared form component needs established brand resources.
- **Do not use when:** a domain status has a distinct centralized palette; extend the authoritative resource rather than hard-code locally.
- **Antipatterns:** literal corporate colors, duplicate logo loading, or competing palette classes.

### 5.2 `AppTypography`

- **Location:** `src/Frontend/NuanSystem.WinForms.Forms/Common/AppTypography.cs`
- **Status:** Active/authoritative for WinForms typography.
- **Responsibility:** Segoe UI font definitions and application helpers for forms, controls, titles, labels, buttons, and grids.
- **Use when:** setting or applying typography in the Forms project.
- **Do not use when:** the Designer already serializes an intentional framework-compatible font and no runtime application is needed.
- **Antipatterns:** Arial/Tahoma defaults, feature-local `Font` constants, or duplicated typography helpers.

### 5.3 `FormStyler`

- **Location:** `src/Frontend/NuanSystem.WinForms.Forms/Common/FormStyler.cs`
- **Status:** Active for established form-level presentation application.
- **Responsibility:** Applies base form styling, typography, inherited panel backgrounds, and panel titles using `AppTypography`.
- **Use when:** an existing form pattern uses it for runtime presentation normalization.
- **Do not use when:** it would create, move, size, dock, anchor, or otherwise hide visual layout from the Designer.
- **Representative consumers:**
  - `src/Frontend/NuanSystem.WinForms.Forms/Audit/AuditLogsForm.cs`
  - `src/Frontend/NuanSystem.WinForms.Forms/Auth/ChangePasswordForm.cs`
- **Antipatterns:** expanding it into a runtime layout builder or creating a competing form styler.

## 6. Frontend HTTP infrastructure

### 6.1 `INuanApiClient` / `NuanApiClient`

- **Locations:**
  - `src/Frontend/NuanSystem.WinForms.Services/Http/INuanApiClient.cs`
  - `src/Frontend/NuanSystem.WinForms.Services/Http/NuanApiClient.cs`
- **Status:** Active/authoritative transport path for frontend feature clients.
- **Responsibility:** Registered HTTP transport with `ApiSession`, generic GET/POST/PUT/DELETE
  methods, streamed multipart file upload through `PostFileAsync`, file download, availability
  check, authenticated session behavior, and company-aware communication.
- **Use when:** a typed frontend service client communicates with NuanSystem API.
- **Do not use when:** accessing SQL or SAP; those calls belong behind backend/integration boundaries.
- **Extension rule:** add a typed feature client that depends on `INuanApiClient`; use
  `PostFileAsync` for bounded multipart uploads instead of exposing `HttpClient`. Change the
  transport only for cross-cutting requirements.
- **Representative consumers:**
  - `src/Frontend/NuanSystem.WinForms.Services/Companies/CompanyClient.cs`
  - `src/Frontend/NuanSystem.WinForms.Services/Security/Menus/MenuClient.cs`
- **Antipatterns:** `new HttpClient()` in forms/feature clients, manual JWT or company headers, duplicated serialization/error handling, or direct backend resource access.

## 7. Backend application and persistence framework

### 7.1 Messaging abstractions

- **Location:** `src/Backend/NuanSystem.Application/Abstractions/Messaging`.
- **Status:** Active/authoritative for Application commands, queries, and handlers.
- **Responsibility:** `ICommand<T>`, `IQuery<T>`, and handler contracts dispatched through MediatR.
- **Use when:** implementing a backend use case or read model.
- **Boundary:** Application contracts must not depend on HTTP, Dapper, claims, WinForms, or provider-specific types.
- **Representative consumers:** Geography commands/queries and PurchaseOrders workflow handlers.

### 7.2 `Result<T>` and API errors

- **Locations:** `src/Backend/NuanSystem.Application/Common/Models` and `src/Backend/NuanSystem.Shared/Responses`.
- **Status:** Active/authoritative for expected business, validation, and not-found outcomes.
- **Responsibility:** Stable success/failure contracts and field-aware `ApiError` values mapped by the API.
- **Use when:** a handler can produce an expected non-success outcome.
- **Do not use when:** hiding unexpected infrastructure exceptions or fabricating success.
- **Current HTTP behavior:** `ResultExtensions.ToHttpResult()` returns HTTP 200 for success and HTTP 400 for every failed `Result<T>`; do not assume 404/409 mapping without a deliberate shared-contract migration.
- **Antipatterns:** new result type, endpoint-local error schema, raw SQL/SAP exception returned to clients.

### 7.3 Trusted company context and tenant connection

- **Locations:**
  - `src/Backend/NuanSystem.Application/Abstractions/Tenancy`
  - `src/Backend/NuanSystem.Api/Middleware/CompanyContextMiddleware.cs`
  - `src/Backend/NuanSystem.Persistence/Connections/TenantConnectionFactory.cs`
- **Status:** Active/authoritative for company-scoped backend work.
- **Responsibility:** Validate user/company access and resolve the active tenant connection.
- **Use when:** reading or writing tenant data.
- **Boundary:** request DTOs and frontend headers do not become trusted company context by themselves.
- **Provider status:** `TenantConnectionFactory` implements SQL Server. `DatabaseEngine.MySql` exists but currently throws `NotSupportedException`; enum presence is not provider support.

### 7.4 `ITransactionRunner` / `SqlTransactionRunner`

- **Locations:**
  - `src/Backend/NuanSystem.Application/Abstractions/Data/ITransactionRunner.cs`
  - `src/Backend/NuanSystem.Persistence/Transactions/SqlTransactionRunner.cs`
- **Status:** Active for multi-write tenant transactions.
- **Responsibility:** Execute Application-defined units of work on one tenant connection/transaction with commit/rollback.
- **Use when:** multiple local writes must succeed or fail together.
- **Do not use when:** wrapping a remote SAP/HTTP call inside an open SQL transaction.

### 7.5 Endpoint authorization

- **Location:** `src/Backend/NuanSystem.Api/Extensions/EndpointAuthorizationExtensions.cs`.
- **Status:** Active/authoritative for `RequirePermission` and `RequireFormOperation`.
- **Responsibility:** Enforce backend permissions and form operations after authentication/company context.
- **Boundary:** frontend menu/button visibility is not authorization.
- **Representative consumers:** Geography and BusinessPartner endpoints.

### 7.6 Dapper stored-procedure repositories

- **Locations:** `src/Backend/NuanSystem.Persistence/Repositories`.
- **Status:** Active SQL Server persistence pattern.
- **Responsibility:** Map Application repository contracts to tenant/master connections and stored procedures using `CommandDefinition` and cancellation.
- **Representative consumers:** `GeographyRepository`, `BusinessPartnerRepository`, and `PurchaseOrderRepository`.
- **Boundary:** procedure names, Dapper, `CommandType`, and SQL-provider details remain in Persistence.

## 8. Framework selection matrix

| Need | Preferred component | Important boundary |
|---|---|---|
| Standard CRUD list | `BaseGridCrudListForm` | Not for operational workflows |
| Standard CRUD edit/consult | `BaseEditForm` | Backend remains authoritative |
| Standard action button | `NuanActionButton` | Configure semantic kind first |
| Related catalog lookup | `NuanLookupEdit` | Enforce create permission and refresh |
| Small fixed closed catalog | direct `LookUpEdit` | No related maintenance; persist stable code and disable free text |
| Reusable feature grid | `NuanDataGridControl` | Avoid duplicating base CRUD grid lifecycle |
| KPI summary | `NuanKpiCardControl` | Presentation, not editable data |
| Colors/logo | `BrandResources` | No local corporate literals |
| Typography | `AppTypography` | One source of truth |
| Form styling | `FormStyler` | Must not build layout |
| API transport | `INuanApiClient` / `NuanApiClient` | No direct SQL/SAP from frontend |
| Application use case | Messaging abstractions | No HTTP/Dapper/provider types |
| Expected backend outcome | `Result<T>` / `ApiError` | No raw infrastructure errors |
| Tenant data access | Trusted company context + `ITenantConnectionFactory` | Never trust request company directly |
| Multi-write tenant unit | `ITransactionRunner` | No remote calls inside SQL transaction |
| Endpoint authorization | `RequirePermission` / `RequireFormOperation` | UI visibility is not security |
| SQL Server persistence | Dapper stored-procedure repository | Procedure names stay in Persistence |
| Authentication | `SqlServerAuthService` + `JwtTokenService` + JWT bearer validation | Renew token after claim-based permission changes |
| Unexpected API failure | `GlobalExceptionMiddleware` | Safe client response; technical detail stays in server/audit logs |

## 9. Extension gate

Before extending a shared component, document:

- current public contract;
- consumers found;
- requirement not met through configuration;
- compatibility strategy;
- Designer impact when applicable;
- test/build plan;
- rollback or migration need;
- catalog and knowledge-graph updates.

## 10. Catalog maintenance gate

A framework change is incomplete until:

- this catalog reflects the new contract/status;
- representative consumers are accurate;
- deprecated paths are named;
- the Knowledge Graph reflects dependencies;
- applicable skills route to the updated component;
- validation evidence is recorded.

## Iteration 4 integration framework

### SAP transport

`NuanSystem.SapIntegration` owns SAP adapters. `SapIntegrationMode`, `ISapClientFactory`, registered Service Layer clients, DI API, HANA adapters, typed readers, and `ISapDocumentSender` are implemented entry points. Application owns use cases; API owns HTTP/permissions; WinForms never calls SAP directly.

`SapServiceLayerQueryClient` is the shared transport for bounded, read-only
Service Layer queries that fit its contract. It owns company-scoped login,
cookies, validated same-root `odata.nextLink` pagination, page-limit failure,
safe SAP error extraction and best-effort logout. Entity readers own the
relative OData query and mapping. `Warehouses` is the reference separation:
`SapWarehouseQuery` defines the query and `SapWarehouseMapper` translates
the JSON rows. Readers with specialized filters or headers may retain a focused
transport path until the shared contract explicitly supports those needs.

### SAP synchronization runtime

`Application/Features/SapSync` and `NuanSystem.SyncWorker` own scheduled SAP synchronization through company context, entity settings, handlers, lock, log, watermark, retry policy, heartbeat, and bounded loops. `SapOutboxWorker` and `SapSyncJobRunner.RunOutboxAsync` remain incomplete and are not approved export implementations.

### Matriz-Sucursal runtime

`Application/Features/Sync` and `NuanSystem.MasterBranchSyncWorker` own internal replication: catalog/profile -> durable `SyncOutbox` -> routing/policy -> targets -> expiring claim -> entity applier -> target/aggregate status -> `SyncAudit` and manual recovery.

These are related but non-interchangeable pipelines. Route through `$nuansystem-sap-business-one`, `$nuansystem-sap-sync-orchestration`, or `$nuansystem-master-branch-sync` as applicable.

### Iteration 8 transactional LocalOutbox catalog

| Entity | Tenant producer | Dependencies | Code/migrations | SQL/runtime |
|---|---|---|---|---|
| BusinessPartner | Proposal/Canonical/ProposalResult LocalOutbox | None | Blocks 1/2 code-complete | Unit/static validated; SQL runtime/pilot pending |
| Item 8.4A | Transactional LocalOutbox | Limited payload | Integrated | Validated in ObserveOnly |
| ItemFamily | Transactional LocalOutbox | ItemGroup | Integrated | Validated DEMO to Remigio |
| ItemGroup | Transactional LocalOutbox | None | Scripts 129/130 deployed | Validated DEMO to Remigio |
| Item v2 | Existing transactional LocalOutbox | ItemGroup, ItemFamily, three UOM identities | Scripts 131/132 deployed | Validated DEMO to Remigio |
| UnitOfMeasure | Full source; controlled applier events used by the pilot | None | Script 132 deployed | Worker/applier validated DEMO to Remigio; full-profile launcher pending |
| Warehouse | Transactional LocalOutbox | None | Scripts 133/134 deployed | Runtime pending |

For every row marked pending, the relay and worker remain disabled. Code-ready
does not imply permission to deploy SQL, create fixtures or apply branch events.

### BusinessPartner bidirectional blocks 1/2

The internal `BusinessPartner` path now reuses the production snapshot and payload factories, central identity/prefix policy, three-way merge/reconciliation, local promotion, closed routing, relay, worker dispatcher and the three directional appliers. The acceptance harness models one central tenant and two branches and covers the twelve approved create/replay/outage/distribution/no-loop/role/conflict/merge/payload/disabled/no-SAP scenarios. Persistence and external infrastructure are the only in-memory boundaries.

Scripts `228`/`229`/`230` and their Dapper contracts are code-complete. Migration `229` accepts only the exact production database or a disposable `NuanSystem_Test_Master_<32hex>` database bound through a same-connection, read-only session-context marker. The opt-in SQL fixture also requires an administrative connection whose initial catalog is `master`, exact generated-name validation, a `RunId` ownership marker and a creation registry before cleanup.

This entry records code/unit/static verification only. SQL runtime, migration execution, readiness review, worker/profile activation, operational pilot, rollback and every SAP action remain pending. The authoritative operational gate is `docs/operations/BUSINESS-PARTNER-BIDIRECTIONAL-PILOT.md`.

## Iteration 5 SRI framework

### Implemented foundation

`TenantFeatureCodes.SriDocuments`, `TenantIntegrationCodes.Sri`, protected tenant integration configuration, Application queue contracts, Dapper repository, protected API endpoints, and scripts `115`/`116` form the implemented Phase 5.2 foundation. Capability and integration remain disabled by default.

### Implemented queue and Phase 5.3 worker baseline

The tenant queue, attempts, audit, Application contracts, endpoints, permissions and optimistic concurrency are implemented and runtime-validated. Scripts `115`/`116` were installed idempotently in Master and the three DEMO tenants; concurrency, `rowversion`, audit, JWT and forbidden access were exercised.

`NuanSystem.SriWorker`, `SriAuthorizationProvider`, `SriWorkerRepository`, and tenant script `117` implement the Phase 5.3 baseline: exact official HTTPS endpoints, bounded claim/lease processing, recovery, persisted retry/dead-letter outcomes, integrity validation, and immutable XML storage with SHA-256 and a 5 MiB limit. Script `117` is deployed idempotently in the three DEMO tenants; concurrent claim, environment isolation, lease recovery/ownership, atomic authorization, repeated response, checksum conflict and oversize rejection passed in real SQL. Phase 5.4 validated the controlled worker lifecycle and one expressly authorized `Production` round trip in `NuanSystem_DEMO`; the worker remains disabled and that evidence does not authorize another call.

The first pilot direction is approved: query and download by access key for previously authorized documents. `docs/architecture/SRI-CONSULT-DOWNLOAD-PILOT-CONTRACT.md` owns its functional boundary. Phase 5.5 adds tenant script `118`, Master script `119`, safe monitor projections, protected byte download, per-access audit, typed frontend transport and `SriDocumentMonitorForm`. These contracts are implemented, deployed and validated with real permissions, tenant isolation, API download, integrity/audit checks and Visual Studio Designer review. Detailed sanitized evidence lives in `docs/operations/SRI-DOCUMENT-MONITOR.md`.

The SRI TXT Import vertical extends the same queue without adding another worker or XML store.
`NuanSystem.Application/SyncSRI/Features/SriTxtImports`, `SriTxtImportRepository`, tenant scripts
`138`/`142`, Master scripts `139`/`143`/`147`/`148`, and `/api/sri/txt-imports` implement bounded
upload/validation, normalized tenant detail, server paging, queue preparation as `Staged`, explicit
audited `Staged -> Pending`, maintenance navigation and corporate Ribbon actions. The worker claim
remains limited to `Pending` and `RetryScheduled`. The access key is not serialized and is not
duplicated in import detail. SQL, API, permission separation, WinForms, tenant isolation and the
final smoke path are runtime-validated; SAP and purchase reconciliation are deferred.

Monitor forward scripts `149`/`150`/`151` register corporate Ribbon operations, add optional
`ImportId` scope and preserve the five summary aggregates as SQL `bigint` values compatible with
Dapper. The global and scoped monitors, paging, masked projections and compact KPI layouts passed
the 2026-07-30 smoke test without downloading XML or invoking the worker/provider.

### Iteration 6 operational contracts

`Application/Features/Operations` owns the shared `WorkerHeartbeat` contract and health evaluator; SAP Sync consumes it without owning it. Master scripts `120`/`122` evolve the existing table forward-only while preserving legacy `InstanceName` writes, and tenant script `121` exposes the SRI queue summary required by heartbeat. Their authorized repeated execution passed with stable history, metadata, security and SAP heartbeats. `NuanSystem.SriWorker` reports lifecycle and exact informational release version, closes claims before shutdown, rejects a duplicate local identity and emits safe structured/Event Log events. The protected health endpoint and `SriDocumentMonitorForm` expose safe projections. The controlled pilot validated SCM installation/cleanup, TLS, JWT, monitor, Visual Studio Designer and pilot1 -> pilot2 -> pilot1 update/rollback. See `docs/operations/SRI-WORKER-OPERATIONS.md`; this evidence does not authorize permanent production enablement.

| Need | Skill/owner | Boundary |
|---|---|---|
| Capture, enqueue, query, reprocess, queue SQL, monitor | `$nuansystem-sri-document-queue` | API/Application persist intent; no remote SRI work |
| Claim, provider call, XML, retry, dead letter, health | `$nuansystem-sri-worker` | Dedicated worker only; no commercial-document ownership |
| Tenant capability and secrets | Master tenant configuration | Disabled by default; sensitive values remain protected |

SRI must not reuse SAP handlers/outbox, `SyncOutbox`, `NuanSystem.SyncWorker`, or `NuanSystem.MasterBranchSyncWorker`. Existing worker patterns are evidence for hosting and reliability techniques only.

## .NET 10 release artifact framework

`tools/release` owns the reusable Phase 7.3 packaging contract:

- `Publish-NuanSystemRelease.ps1` publishes API, the three workers and WinForms
  separately as `Release/win-x64`, framework-dependent artifacts;
- `Test-NuanSystemRelease.ps1` verifies the exact file set, SHA-256, entry-point
  versions, safe configuration and forbidden content;
- `Set-NuanSystemActiveRelease.ps1` selects an immutable release through an
  external pointer and supports artifact-level rollback without recompilation.

Published releases exclude local settings, secrets, certificates, logs,
backups and SRI payloads. All workers remain disabled. This framework packages
and verifies binaries only; it does not own SCM installation, external
configuration, process startup or production promotion. The operational
contract is `docs/operations/DOTNET-10-RELEASE-ARTIFACTS.md`.
