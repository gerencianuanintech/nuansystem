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

## 5. Standard CRUD relationship graph

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

### Confirmed boundary — Transportistas (pre-implementation)

`Transportistas` is an independent administrative master. It is not a `BusinessPartner`, supplier subtype, supplier-class-filtered view, or specialization of the `TRA / Transporte` seed. The seed and existing BusinessPartners implementation are not ownership evidence for this feature.

```text
Transportistas
  -> own entity/contract
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

Persist the code, not the display text or selected index. Do not provide related-create, clear, or free-text behavior for the type. The backend must reject any code outside `04`, `05`, and `06`; the database contract must enforce the same closed set. Keep identifier normalization and type-specific format validation in the Transportistas Application/backend contract, not only in WinForms.

The list grid must include `IdentificationTypeCode`/its resolved display label and `IdentificationNumber`. The edit form must place the identification-type combo immediately before the identification editor, with both controls declared explicitly in `CarrierEditForm.Designer.cs`.

## 6. Operational relationship graph

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

## 7. Change impact traversal

For a changed node, traverse:

1. **Upstream owners** — who constructs/configures it?
2. **Downstream consumers** — who derives from/calls/embeds it?
3. **Parallel contracts** — Designer, interface, DTO, SQL result, tests, docs.
4. **Cross-cutting edges** — tenant, auth, permissions, audit, errors.
5. **External edges** — SAP/BEAS/worker/outbox/retry.

Record the search terms and resulting paths. Do not rely solely on this static graph.

## 8. Graph maintenance rules

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

## 9. Known scope of Iteration 1

This initial graph is intentionally strongest for the WinForms framework because frontend consistency is the first implementation priority. Backend, SQL, SAP, BEAS, Android, and domain-specific graphs require subsequent repository-backed catalog iterations. Their absence here is a documented pending expansion, not permission to invent their architecture.
