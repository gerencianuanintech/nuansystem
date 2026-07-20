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
- **Contract observed:** `BaseGridCrudListForm.Designer.cs` creates `NuanDataGridControl`; the base form exposes its inner `GridControl` and `GridView`, supports typed data binding and selection, implements edit, copy, delete, consult, and history hooks, and provides column configuration/personalization plus Excel/PDF/JSON/XML export.
- **Use when:** a maintenance list has standard CRUD commands, selection, permissions, export, and configurable columns.
- **Do not use when:** the screen is an operational transaction, document editor, dashboard, or workflow whose lifecycle is not CRUD.
- **Extension rule:** derive a feature form and override documented hooks such as grid configuration and CRUD operations. Do not fork the base lifecycle.
- **Representative consumers:**
  - `src/Frontend/NuanSystem.WinForms.Forms/Geography/Cities/CitiesForm.cs`
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
- **Status:** Active/preferred for corporate lookups requiring clear/create affordances.
- **Responsibility:** `LookUpEdit` with managed clear/create buttons and `ClearButtonClick` / `CreateButtonClick` events.
- **Contract observed:** `CreateButtonEnabled`, `ClearButtonEnabled`, and `RefreshButtons`.
- **Use when:** selecting a related catalog and optionally creating or clearing the relation.
- **Do not use when:** a read-only label, fixed enum editor, or specialized search/grid lookup is required and the control cannot meet it.
- **Extension rule:** configure the control and wire permission-aware events; refresh the source and select a newly created record.
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

### 4.4 `NuanDataGridControl`

- **Location:** `src/Frontend/NuanSystem.WinForms.Controls/Grids/NuanDataGridControl.cs`
- **Status:** Active/preferred for reusable feature grids.
- **Responsibility:** Corporate grid user control with pagination, find panel, multi-selection, selection checkboxes, column configuration, status badges, export, and column customization.
- **Contract observed:** exposes inner `GridControl`/`GridView`; `SetData<T>`, focused/selected row helpers, `ConfigureColumns`, `SetStatusBadgeProvider`, `ApplyStandardGridStyle`, and `ExportVisibleColumns`.
- **Use when:** a feature needs a reusable grid surface outside the inherited standard CRUD list or needs the control's paging/search/selection contract.
- **Do not use when:** `BaseGridCrudListForm` already owns the complete standard CRUD list lifecycle and already contains this control internally; do not add a second `NuanDataGridControl` without evidence.
- **Representative consumers:**
  - `src/Frontend/NuanSystem.WinForms.Forms/Sync/SyncMonitorForm.Designer.cs`
  - `src/Frontend/NuanSystem.WinForms.Forms/Sync/SyncOutboxDetailForm.Designer.cs`
- **Antipatterns:** new grid wrapper, feature-local pagination/export framework, or bypassing its public contract to reproduce behavior.

### 4.5 `NuanKpiCardControl`

- **Location:** `src/Frontend/NuanSystem.WinForms.Controls/Kpi/NuanKpiCardControl.cs`
- **Status:** Active/specialized for KPI summaries.
- **Responsibility:** Corporate KPI card with title, value, description, icon, colors, border, shadow, and style presets.
- **Use when:** a dashboard/monitor presents a compact metric summary.
- **Do not use when:** displaying ordinary field values or editable data.
- **Representative consumer:** `src/Frontend/NuanSystem.WinForms.Forms/Sync/SyncMonitorForm.Designer.cs`
- **Antipatterns:** custom painted KPI panels or copied KPI styling.

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
- **Responsibility:** Registered HTTP transport with `ApiSession`, generic GET/POST/PUT/DELETE methods, availability check, authenticated session behavior, and company-aware communication.
- **Use when:** a typed frontend service client communicates with NuanSystem API.
- **Do not use when:** accessing SQL or SAP; those calls belong behind backend/integration boundaries.
- **Extension rule:** add a typed feature client that depends on `INuanApiClient`; change the transport only for cross-cutting requirements.
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

### SAP synchronization runtime

`Application/Features/SapSync` and `NuanSystem.SyncWorker` own scheduled SAP synchronization through company context, entity settings, handlers, lock, log, watermark, retry policy, heartbeat, and bounded loops. `SapOutboxWorker` and `SapSyncJobRunner.RunOutboxAsync` remain incomplete and are not approved export implementations.

### Matriz-Sucursal runtime

`Application/Features/Sync` and `NuanSystem.MasterBranchSyncWorker` own internal replication: catalog/profile -> durable `SyncOutbox` -> routing/policy -> targets -> expiring claim -> entity applier -> target/aggregate status -> `SyncAudit` and manual recovery.

These are related but non-interchangeable pipelines. Route through `$nuansystem-sap-business-one`, `$nuansystem-sap-sync-orchestration`, or `$nuansystem-master-branch-sync` as applicable.

## Iteration 5 SRI framework

### Implemented foundation

`TenantFeatureCodes.SriDocuments`, `TenantIntegrationCodes.Sri`, protected tenant integration configuration, Application queue contracts, Dapper repository, protected API endpoints, and scripts `115`/`116` form the implemented Phase 5.2 foundation. Capability and integration remain disabled by default.

### Implemented queue; remote processing not operative

The tenant queue, attempts, audit, Application contracts, endpoints, permissions and optimistic concurrency are implemented in code. Deployment evidence still requires executing scripts `115` and `116`. XML storage, monitor, provider client, claim loop and `NuanSystem.SriWorker` remain target contracts only.

The first pilot direction is approved: query and download by access key for previously authorized documents. `docs/architecture/SRI-CONSULT-DOWNLOAD-PILOT-CONTRACT.md` owns its functional boundary. Queue implementation does not prove provider, XML download or end-to-end processing.

| Need | Skill/owner | Boundary |
|---|---|---|
| Capture, enqueue, query, reprocess, queue SQL, monitor | `$nuansystem-sri-document-queue` | API/Application persist intent; no remote SRI work |
| Claim, provider call, XML, retry, dead letter, health | `$nuansystem-sri-worker` | Dedicated worker only; no commercial-document ownership |
| Tenant capability and secrets | Master tenant configuration | Disabled by default; sensitive values remain protected |

SRI must not reuse SAP handlers/outbox, `SyncOutbox`, `NuanSystem.SyncWorker`, or `NuanSystem.MasterBranchSyncWorker`. Existing worker patterns are evidence for hosting and reliability techniques only.
