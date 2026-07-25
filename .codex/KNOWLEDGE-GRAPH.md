# NuanSystem Engineering Knowledge Graph

## 1. Purpose

This graph records high-value relationships that Codex must inspect before changing NuanSystem. It is a curated navigation and impact map, not a substitute for repository search.

Every node must reference a real path or symbol. Every shared-framework change requires a fresh consumer search because this document may lag behind source code.

## 2. Authority and use

Authority: Constitution > Kernel > Catalogs/this graph > Skills > implementation.

Use the graph to:

- identify likely dependencies and consumers;
- choose representative patterns;
- expand the affected-layer map;
- assess shared-component risk;
- discover which catalog/skill to load.

Do not infer that an unlisted relationship does not exist.

## 3. Core engineering graph

```text
ENGINEERING-CONSTITUTION.md
  -> ENGINEERING-KERNEL.md
       -> nuansystem-framework-discovery/SKILL.md
       -> FRAMEWORK-CATALOG.md
       -> PATTERN-CATALOG.md
       -> KNOWLEDGE-GRAPH.md
       -> REVIEW-CHECKLIST.md
            -> specialized skills
                 -> implementation
```

## 4. Frontend framework graph

### 4.1 Standard CRUD list

```text
BaseGridCrudListForm
  location: src/Frontend/NuanSystem.WinForms.Forms/Common/BaseGridCrudListForm.cs
  exposes/coordinates:
    -> GridControl and GridView
    -> typed data and selection helpers
    -> edit/copy/delete/consult/history lifecycle
    -> column configuration and personalization
    -> Excel/PDF/JSON/XML export
  representative consumers:
    -> Geography/Cities/CitiesForm.cs
    -> GeneralSupplier/SupplierGroups/SupplierGroupsForm.cs
    -> Security/Roles/RolesForm.cs
  governed by:
    -> FRAMEWORK-CATALOG.md §3.1
    -> PATTERN-CATALOG.md P1
```

Impact rule: changing base selection, command lifecycle, personalization, export, or grid defaults is high risk and requires a repository-wide derived-type/consumer search.

### 4.2 Standard edit/consult

```text
BaseEditForm
  location: src/Frontend/NuanSystem.WinForms.Forms/Common/BaseEditForm.cs
  owns:
    -> FormValidator lifecycle
    -> ValidateForm / BuildRequest / PersistAsync hooks
    -> Save and UI exception handling
    -> read-only consult mode
  designer dependency:
    -> Common/BaseEditForm.Designer.cs
       -> NuanActionButton
  representative consumers:
    -> Geography/Countries/CountryEditForm.cs
    -> FinancialCatalogs/Branches/BranchEditForm.cs
    -> Security/Roles/RoleEditForm.cs
```

Impact rule: changing save, read-only, disposal, validation, or button behavior requires representative consumers from multiple domains.

### 4.3 Corporate action button

```text
NuanActionButton
  location: src/Frontend/NuanSystem.WinForms.Controls/Buttons/NuanActionButton.cs
  related:
    -> NuanActionButtonKind.cs
    -> NuanActionButtonStyle.cs
  used by:
    -> Common/BaseEditForm.Designer.cs
    -> Sync/SyncMonitorForm.Designer.cs
    -> other designer-backed action surfaces
```

Impact rule: public property/default/icon/size changes may alter many Designer surfaces. Verify serialization and representative forms.

### 4.4 Corporate lookup

```text
NuanLookupEdit
  location: src/Frontend/NuanSystem.WinForms.Controls/Lookups/NuanLookupEdit.cs
  provides:
    -> clear action/event
    -> create action/event
    -> permission-aware enablement hooks
  representative consumers:
    -> GeneralInventory/ItemGroups/ItemGroupEditForm.Designer.cs
    -> Security/Users/UserEditForm.Designer.cs
  feature flow:
    -> typed catalog client
    -> create permission
    -> related edit dialog
    -> source refresh
    -> select new record
```

Impact rule: button collection, events, default enablement, or Designer properties are shared-control changes.

### 4.5 Corporate data grid

```text
NuanDataGridControl
  location: src/Frontend/NuanSystem.WinForms.Controls/Grids/NuanDataGridControl.cs
  wraps:
    -> DevExpress GridControl
    -> DevExpress GridView
  coordinates:
    -> paging
    -> find panel
    -> selection/multi-select
    -> column definitions/customization
    -> status badges
    -> export
  representative consumers:
    -> Sync/SyncMonitorForm.Designer.cs
    -> Sync/SyncOutboxDetailForm.Designer.cs
  related standard CRUD path:
    -> BaseGridCrudListForm
```

Decision edge: use the base CRUD form for a complete CRUD-list lifecycle; use `NuanDataGridControl` for a reusable feature grid when its independent control contract is needed. Do not layer both automatically.

### 4.6 KPI and monitor surface

```text
SyncMonitorForm.Designer.cs
  -> NuanKpiCardControl
       location: Controls/Kpi/NuanKpiCardControl.cs
  -> NuanDataGridControl
  -> NuanActionButton
  -> typed frontend services
```

This is the initial representative monitor/dashboard pattern, not a universal domain template.

### 4.7 Visual foundation

```text
BrandResources
  location: Forms/Common/BrandResources.cs
  -> brand colors, logo, rounded geometry

AppTypography
  location: Forms/Common/AppTypography.cs
  -> form/control/title/label/button/grid typography

FormStyler
  location: Forms/Common/FormStyler.cs
  -> uses AppTypography
  -> applies form/panel presentation
  -X must not construct layout
```

Representative `FormStyler` consumers:

- `Forms/Audit/AuditLogsForm.cs`
- `Forms/Auth/ChangePasswordForm.cs`

Impact rule: corporate color/font defaults are cross-application visual changes and require broad UI review.

### 4.8 Frontend transport

```text
Feature Form/ViewModel
  -> typed feature client
       examples:
         -> Services/Companies/CompanyClient.cs
         -> Services/Security/Menus/MenuClient.cs
       -> INuanApiClient
            -> NuanApiClient
                 location: Services/Http/NuanApiClient.cs
                 -> HttpClient (injected)
                 -> ApiSession
                 -> authenticated/company-aware API request
                      -> backend endpoint
```

Forbidden edges:

```text
Form -X-> new HttpClient
Form -X-> SQL Server/HANA
Form -X-> SAP DI API/Service Layer
Feature client -X-> manual JWT/company header
```

## 5. Backend framework graph

### 5.1 Application dispatch and result

```text
Minimal API endpoint
  -> ISender
       -> ICommand<T> / IQuery<T>
            -> handler
                 -> Result<T>
                      -> ToHttpResult()
```

Impact rule: changing messaging or result contracts requires inspecting handlers, pipeline behaviors, endpoint mapping, frontend error deserialization, and tests.

### 5.2 Tenant data path

```text
Authorization: Bearer
  + X-Company-Code
  -> CompanyContextMiddleware
       -> validated user/company access
       -> trusted ICompanyContext
            -> ITenantConnectionFactory
                 -> tenant-scoped repository/procedure
```

Forbidden edge: request DTO/company header `-X->` direct trusted connection selection.

### 5.3 Repository and SQL path

```text
Application handler
  -> Application repository interface
       -> Persistence Dapper repository
            -> CommandDefinition + CancellationToken
            -> stored procedure
                 -> constraints/audit/logical delete/transaction
```

Representative paths:

- `Application/Features/Geography` -> `Persistence/Repositories/Geography/GeographyRepository.cs`.
- `Application/Features/BusinessPartners` -> `Persistence/Repositories/BusinessPartnerRepository.cs`.
- `Application/Features/Purchasing/PurchaseOrders` -> `Persistence/Repositories/Purchasing/PurchaseOrderRepository.cs`.

### 5.4 Transaction path

```text
Application operational handler
  -> ITransactionRunner
       -> SqlTransactionRunner
            -> one tenant connection + transaction
            -> transaction-aware repository writes
            -> commit or rollback
```

Remote SAP/HTTP processing must not run inside the open SQL transaction. Persist durable intent/outbox and process externally when required.

### 5.5 Backend authorization and audit

```text
Endpoint
  -> RequirePermission / RequireFormOperation
  -> ClaimsPrincipal.GetAuditUser()
  -> command
  -> handler/repository
  -> audit columns/history source
```

Impact rule: a new action requires aligned permission/FormKey/action values in backend, Master security data, frontend navigation/action visibility, and allowed/denied tests.

## 6. Standard CRUD relationship graph

```text
Administrative entity
  -> Domain/entity or shared contract
  -> Application command/query/validator
  -> Repository contract
  -> Persistence implementation and SQL procedure contract
  -> API endpoint and authorization
  -> Typed frontend client over INuanApiClient
  -> BaseGridCrudListForm-derived list
  -> BaseEditForm-derived edit/consult
  -> form/operation permissions
  -> menu entry where navigable
  -> tests
```

### Geography reference family

```text
CitiesForm
  -> BaseGridCrudListForm
  -> CityEditForm
       -> BaseEditForm

CountriesForm
  -> BaseGridCrudListForm
  -> CountryEditForm
       -> BaseEditForm
```

Use this family for simple geographic catalog evidence after confirming the current domain need matches.

### Security reference family

```text
RolesForm
  -> BaseGridCrudListForm
  -> RoleEditForm
       -> BaseEditForm
  -> role/form/operation permission contracts
```

Security catalogs require extra authorization/permission inspection; do not copy them as ordinary CRUD without those edges.

### Implemented boundary — Transportistas

`Transportistas` is an independent administrative master. It is not a `BusinessPartner`, supplier subtype, supplier-class-filtered view, or specialization of the `TRA / Transporte` seed. The seed and existing BusinessPartners implementation are not ownership evidence for this feature.

```text
Transportistas
  -> own tenant table and contracts (without a Domain entity)
       -> Id
       -> Code
       -> Name
       -> IdentificationTypeCode
       -> IdentificationNumber
       -> Description
       -> IsActive
       -> audit and logical deletion
  -> own Application commands, queries, validation, and DTOs
  -> own repository and persistence/SQL contract
  -> own API endpoints and authorization
  -> own typed frontend client and ViewModel
  -> own BaseGridCrudListForm-derived list
  -> own BaseEditForm-derived edit/consult form
  -> own FormKey, menu, operations, audit, and tests

Transportistas -X-> BusinessPartners entity/repository/API/synchronization
Transportistas -X-> SupplierEditForm / BusinessPartnersForm reuse
Transportistas -X-> SupplierClassId or TRA as identity/discriminator
```

Allowed reuse is framework-level only: CRUD lifecycle, corporate base forms and controls, Designer/layout rules, `INuanApiClient`, session/company propagation, permission infrastructure, and visual resources. Any future relationship between a transportista and a business partner must be a separately approved requirement; it must not be inferred during discovery.

The implemented vertical is rooted at `Application/Features/Carriers`, `Persistence/Repositories/CarrierRepository.cs`, `Api/Endpoints/CarrierEndpoints.cs`, tenant audit foundation `106`, tenant feature script `107`, forward tenant hardening `110`, Master security scripts `108`/`109`, forward Master operation hardening `111`, and the frontend `Carriers` folders. It deliberately has no Domain entity, SAP mapping, synchronization event, outbox publisher, or BusinessPartners dependency.

`110` and `111` are required forward repairs for environments where `106`-`109` were already executed. `110` adds nonblank database checks, locks write decisions inside their transactions, verifies affected rows, and maps concurrent unique-code collisions to the repository result contract. `111` limits automatic ADMIN grants to the operations actually supported by the corporate CRUD grid while preserving intentionally modified grants.

#### Transportista identification contract

Identification belongs to the independent `Transportistas` vertical. It must not reference or reuse `BusinessPartnerIdentificationTypes`, BusinessPartners lookups, DTOs, repositories, endpoints, or forms.

Persist these required fields:

- `IdentificationTypeCode` — two-character SRI code.
- `IdentificationNumber` — entered identification value.

The edit form must expose `IdentificationTypeCode` as a non-editable-options combo with exactly:

| Code | Visible value |
|---|---|
| `05` | Cédula |
| `04` | RUC |
| `06` | Pasaporte |

Persist the code, not the display text or selected index. Do not provide related-create, clear, or free-text behavior for the type. The backend must reject any code outside `04`, `05`, and `06`; the database contract must enforce the same closed set. The approved contract requires a non-empty identifier of at most 30 characters and does not infer type-specific checksum validation or uniqueness; either rule requires a separate business decision.

The list grid must include `IdentificationTypeCode`/its resolved display label and `IdentificationNumber`. The edit form must place the identification-type combo immediately before the identification editor, with both controls declared explicitly in `CarrierEditForm.Designer.cs`.

## 7. Operational relationship graph

```text
User intent
  -> Frontend form
  -> typed client/API command
  -> authorization + company context
  -> Application operational handler
  -> authoritative reads and invariant validation
  -> transaction/concurrency/idempotency
  -> persistence + audit + optional outbox
  -> integration worker/mapping/retry when applicable
  -> authoritative result/status
  -> frontend presentation
```

Any stock, money, pricing, cash, document, or synchronization edge routes to `PATTERN-CATALOG.md` P3/P4/P8 and the operational skill, not generic CRUD.

## 8. Change impact traversal

For a changed node, traverse:

1. **Upstream owners** — who constructs/configures it?
2. **Downstream consumers** — who derives from/calls/embeds it?
3. **Parallel contracts** — Designer, interface, DTO, SQL result, tests, docs.
4. **Cross-cutting edges** — tenant, auth, permissions, audit, errors.
5. **External edges** — SAP/BEAS/worker/outbox/retry.

Record the search terms and resulting paths. Do not rely solely on this static graph.

## 9. Graph maintenance rules

Update this graph when:

- adding/removing/renaming a shared framework node;
- changing a public contract or default behavior;
- establishing a new approved pattern;
- deprecating a component;
- adding an important cross-layer or external relationship.

A graph update must:

- cite real repository paths;
- distinguish representative examples from exhaustive consumers;
- avoid speculative nodes;
- include impact rules for high-risk shared components;
- remain consistent with Framework and Pattern Catalogs.

## 10. Integration and synchronization graph

### 10.1 SAP transport and ingestion

```text
TenantIntegration / SAP company settings
  -> SapIntegrationMode -> SapIntegrationServiceRegistration
  -> typed Service Layer/HANA reader or ISapClientFactory
  -> Application/Features/SapSync use case
  -> tenant mapping/persistence
  -> /api/sap + SapRead/SapManage
```

### 10.2 SAP scheduled synchronization

```text
NuanSystem.SyncWorker -> active SAP companies -> company context
  -> enabled settings -> SapSyncOrchestrator
  -> lock -> ISapSyncEntityHandler -> log/watermark -> unlock
  -> heartbeat + bounded retry policy
```

`SapOutboxWorker` and `SapSyncJobRunner.RunOutboxAsync` are incomplete nodes. `Both` does not prove complete bidirectional delivery.

### 10.3 Matriz-Sucursal replication

```text
SyncMasterBranchEntityCatalog + SyncProfiles
  -> SyncEventPublisher -> SyncOutbox
  -> routing/distribution -> target per branch
  -> MasterBranchSyncWorkerProcessor
  -> dispatcher/entity applier
  -> target states -> aggregate state
  -> SyncAudit / retry / release lock / DeadLetter
```

Dependencies include Countries -> Provinces -> Cities, ItemGroups -> Item, PriceLists -> Currencies, and PurchaseOrder references. A catalog entry without producer or applier is a capability gap, not an active path.

### 10.4 Condiciones de Pago SAP B1 → Matriz → Sucursal

```text
SAP B1 PaymentTermsTypes
  -> SapServiceLayerPaymentTermReader
  -> SapPaymentTermImportService
  -> SP_NA_POST_BUSINESSPARTNERPAYMENTTERMS_IMPORTARSAP
  -> BusinessPartnerPaymentTerms (SAP_B1 + GroupNumber; stable GlobalId)
  -> SyncOutbox or PaymentTermFullEntitySource
  -> ReferenceCatalogSyncEventApplier
  -> branch BusinessPartnerPaymentTerms
```

This path is operative after forward scripts `112` and `113`. It accepts only exactly representable day-based, single-payment terms; conflicts remain visible, local seeds are not adopted automatically, and snapshot absence does not deactivate rows. Read `docs/architecture/SAP-PAYMENT-TERMS-SYNC.md` for the authoritative contract.

Forbidden: WinForms-to-SAP, MasterBranchSyncWorker-to-SAP session, SAP outbox substituted by `SyncOutbox`, skeleton/NotImplemented called complete, or Transportistas synchronized without an approved requirement.

### 10.5 SRI electronic documents

```text
TenantFeatureCodes.SriDocuments + TenantIntegrationCodes.Sri
  -> protected Master tenant configuration (implemented; disabled by default)
  -> capturer submits approved minimum contract [API implemented]
  -> Application/API enqueue/query/cancel/reprocess [implemented]
  -> tenant SRI queue + attempts + audit [deployed and runtime-validated]
  -> tenant authorized XML + SHA-256 store [script 117 deployed and SQL-validated in three pilot tenants]
  -> NuanSystem.SriWorker claim/lease/recovery [implemented and runtime-validated for the authorized Production pilot]
  -> official offline authorization provider + XML integrity processing [implemented; one authorized Production round trip validated]
  -> protected API query/reprocess [implemented]
  -> protected XML download + per-access audit [implemented, deployed and runtime-validated in Phase 5.5]
  -> WinForms monitor through typed NuanApiClient [implemented and visually validated]
```

The SRI graph has no edge to `SyncOutbox`, SAP outbox, `NuanSystem.SyncWorker`, or `NuanSystem.MasterBranchSyncWorker`. Capturers stop after durable enqueue. Only the dedicated SRI worker performs remote processing. See `docs/architecture/SRI-ITERATION-5-BLUEPRINT.md`.

The approved pilot traverses only `Environment + AccessKey -> authorization lookup -> immutable authorized XML`. Generation, signing, submission, cancellation, and portal scraping have no edge in this pilot. Queue/worker/XML runtime, one expressly authorized Production round trip and the Phase 5.5 protected download/monitor are validated. See `docs/architecture/SRI-CONSULT-DOWNLOAD-PILOT-CONTRACT.md` and `docs/operations/SRI-DOCUMENT-MONITOR.md`.

Iteration 6 adds implemented and runtime-validated operational contracts:

```text
NuanSystem.SriWorker Generic Host + UseWindowsService [validated pilot]
  -> shared Master WorkerHeartbeat evolution [120/122 idempotent; SAP compatibility preserved]
  -> tenant operational summary [121 idempotent in DEMO]
  -> protected health + WinForms projection [JWT and runtime validated]
  -> lifecycle/gate/mutex/safe events [runtime validated]
  -> Windows Service identity, external config, TLS and ACL [temporary installation/cleanup validated]
  -> informational release version [pilot1/pilot2 exact values validated]
  -> deployment update and binary rollback [pilot1 -> pilot2 -> pilot1 validated]
```

The design is governed by `docs/architecture/SRI-ITERATION-6-OPERATIONS-BLUEPRINT.md`; sanitized runtime evidence is summarized in `docs/operations/SRI-WORKER-OPERATIONS.md`. `WorkerHeartbeat` remains the single shared Master surface and its Application/Persistence contracts live under `Operations`; SAP compatibility is retained. The approval is limited to the controlled pilot and must not be generalized to permanent production enablement, other tenants or new SRI actions.

### 10.6 .NET 10 release artifacts

```text
clean Git commit
  -> Publish-NuanSystemRelease
  -> API + SyncWorker + MasterBranchSyncWorker + SriWorker + WinForms
  -> dependency inventory + release manifest + per-file SHA-256
  -> Test-NuanSystemRelease
  -> immutable pilot1 / immutable pilot2
  -> external active-release pointer
  -> pilot1 -> pilot2 -> pilot1 rollback
```

This graph has no edge to SQL migration, SCM installation, worker enablement,
SAP, SRI or production promotion. Published workers are disabled and local
configuration remains external. See
`docs/operations/DOTNET-10-RELEASE-ARTIFACTS.md`.

## 11. Iteration scope

The graph covers the Iteration 1 core, Iteration 2 WinForms framework, Iteration 3 backend contracts, Iteration 4 repository-backed SAP/Matriz-Sucursal boundaries, Iteration 5 through Phase 5.5, and the controlled runtime validation of Iteration 6. BEAS, Android, permanent production enablement of the SRI worker, and deeper domain graphs remain future work.

`Transportistas` is the validated Iteration 2 pilot: solution build, automated tests, tenant/master SQL execution, renewed-token authorization, runtime CRUD, closed identification selector, Designer serialization, and approved compact vertical spacing were exercised. Its evidence promotes only the documented reusable framework patterns; its business identity remains independent.
