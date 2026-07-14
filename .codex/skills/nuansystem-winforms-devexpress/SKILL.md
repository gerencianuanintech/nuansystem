---
name: nuansystem-winforms-devexpress
description: Build or modify NuanSystem WinForms UI with DevExpress, FormStyler, BrandResources, BaseGridCrudListForm/BaseEditForm, operational forms, ViewModels, HTTP service clients, ApiSession permissions, lookup/search lookup controls, menu integration, Visual Studio Designer compatibility, and frontend normalization for a configurable multi-business commercial system. Use for tasks touching src/Frontend, forms, controls, desktop service clients, view models, DevExpress visual standards, new form creation, or adding a form to the dynamic menu/security navigation.
---

# NuanSystem WinForms DevExpress

## Purpose

This is the only primary frontend skill for NuanSystem WinForms + DevExpress work. It governs desktop UI structure, form patterns, API consumption from WinForms, permissions, menu integration, Designer compatibility, and references for specialized details.

Use it for:

- New or modified WinForms DevExpress screens.
- CRUD maintenance list/edit forms.
- Operational desktop workflows.
- Frontend service clients, frontend models, and ViewModels.
- Lookup/search lookup controls and related-record creation from selectors.
- Dynamic menu, `FormKey`, and operation-permission integration.
- Visual consistency and typography decisions through internal references.

## Workflow

1. Classify the requested frontend work as CRUD maintenance, operational workflow, lookup integration, menu/security wiring, service-client change, or visual/Designer cleanup.
2. Read `docs/FRONTEND-DEVEXPRESS-NOMENCLATURA.md`, `docs/estandar-visual-winforms.md`, `docs/ARQUITECTURA-COMERCIAL.md`, and the closest existing form before editing frontend code.
3. Load only the internal references needed for the task:
   - `references/ui-checklist.md` for any new screen.
   - `references/enterprise-typography.md` for typography, font, grid, and AppTypography decisions.
   - `references/designer-compatibility.md` before creating or editing `.Designer.cs`.
   - `references/service-clients.md` before creating or modifying frontend API clients.
   - `references/operational-forms.md` for sales, cash, receiving, transfers, counts, monitors, or other operational workflows.
   - `references/lookup-controls.md` when a form uses `LookUpEdit`, `SearchLookUpEdit`, `GridLookUpEdit`, or table-backed selectors.
   - `references/menu-integration.md` when a form needs a menu entry, `FormKey`, or form-operation permissions.
4. Ask for missing destination folder/module, visible title, menu placement, `FormKey`, operations, and default access only when the user asked for a new form and the values cannot be inferred safely.
5. Follow nearby module patterns first. Prefer shared project bases and helpers over new local abstractions.
6. Build the touched frontend project or solution when practical.

## New Form Intake

Before creating a new form, ensure these decisions are known:

- Folder/module name: the exact folder under `Forms`, `ViewModels`, and `Services`.
- Form class name and screen title.
- Menu placement: parent menu/category and visible menu label.
- `FormKey`: kebab-case key used by navigation, permissions, grid settings, and form-operation authorization.
- Operations: refresh, create, update, delete, consult, copy, history, column customization, and exports as applicable.
- Default access: whether an existing admin role should receive the new menu and operations in seed SQL.

If any of these are missing and the user asked for a new form, pause and ask a concise question before writing files.

## Naming

- Prefix control fields with the approved DevExpress prefixes.
- Name service clients as `{Entity}Client` and interfaces as `I{Entity}Client`.
- Place service models under `Services/{Module}/Models`.
- Place view models under `ViewModels/{Module}` and forms under `Forms/{Module}`.
- Use the user-provided folder/module name exactly when it matches existing folder conventions; otherwise normalize only casing/pluralization to match nearby folders after confirming.

## Frontend Dependency Rules

- WinForms is a desktop client. It must not connect directly to SQL Server, MySQL, SAP Business One, SRI, files used as backend integrations, or any external business system.
- Every business operation must pass through the REST API.
- Forms must not contain business rules. Forms coordinate UI events, call ViewModels/services, and render responses.
- Backend remains responsible for security, tenancy, validation, business rules, persistence, SAP, SRI, and synchronization.
- Frontend permissions are UX constraints only; backend permissions remain authoritative.
- Do not hard-code company selection, tenant connection data, audit users, SAP mode, or capability behavior in WinForms.

## Form Patterns

- Use DevExpress controls for new UI: `XtraForm`, `SimpleButton`, `TextEdit`, `LabelControl`, `GridControl`, `GridView`, `LookUpEdit`, `SearchLookUpEdit`, `SpinEdit`, `CheckEdit`, `MemoEdit`, `DateEdit`, and related editors.
- Apply `Common.FormStyler.ApplyBase(this)` in code-built forms and use `FormStyler`, `BrandResources`, and the typography reference instead of ad-hoc fonts/colors.
- Use ViewModels for UI state, selected item state, busy state, validation state, and screen orchestration when the local module pattern supports it.
- Use `GridControl` + `GridView`; do not introduce `DataGridView` for DevExpress screens.
- Use `XtraMessageBox` or approved shared UI helpers for confirmations and user-facing errors.
- Disable actions during async operations using the existing busy-state patterns.
- Hide, show, enable, or require fields based on company capabilities only as UI reflection of backend/configuration rules.

## CRUD Maintenance Forms

- For standard grid-based CRUD listings, prefer `BaseGridCrudListForm`, as used by `InventoryItems/ItemsForm` and `GeneralInventory/ItemGroups/ItemGroupsForm`.
- Standard CRUD list forms should reuse base search, pagination, selection, audit footer, export, column customization, and CRUD action behavior.
- Configure grid data with `SetGridData(...)`, visible columns in `ConfigureGridColumns()`, selected records with `SelectedGridItem<T>()`, and column personalization with the same `FormKey`.
- Use `BaseEditForm` for create/edit/consult flows when applicable.
- Administrable auxiliary masters must have concrete user-facing forms, service clients, models, ViewModels when used locally, `FormKey`, menu/security entries, and CRUD permissions. Do not collapse multiple administrable masters into a single generic final user form.

## Operational Forms

- Do not force operational workflows into CRUD list/edit forms.
- Sales, cash, purchase receiving, inventory transfers, counts, adjustments, operational monitors, and similar workflows need dedicated screens shaped around the business process.
- Load `references/operational-forms.md` before designing or modifying these screens.

## API Consumption

- Forms must never create `HttpClient` directly.
- API calls must go through module service clients backed by `NuanApiClient` or the approved centralized HTTP client.
- Individual feature clients know route paths and request/response models only.
- `Authorization` and `X-Company-Code` are injected centrally by `NuanApiClient`; forms and module clients must not add those headers manually.
- Put service clients under `NuanSystem.WinForms.Services` and service models under the module `Models` folder.
- Load `references/service-clients.md` before adding or changing frontend API clients.

## Permissions And Menu

- Respect permissions through `ApiSession`, `CrudOperationPermissions`, and operation access returned by backend security.
- Do not add a form without considering its dynamic menu entry, `FormKey`, operation permissions, grid settings key, and navigation path.
- When a new screen must appear in the menu, load `references/menu-integration.md` and keep `SecurityForms`, `SecurityMenus`, `SecurityRoleMenus`, form operations, and frontend factories aligned.
- Related-record creation from lookup controls must be permission-controlled for the related maintenance.

## Lookup Controls

- For table-backed `LookUpEdit`, `SearchLookUpEdit`, or `GridLookUpEdit`, show at least code and name columns when the data has those business identifiers.
- Keep the selected value aligned with the stable Id/code expected by the API.
- Provide an inline create action for related records only when useful and allowed by permissions.
- After creating a related lookup record, refresh the lookup data and select the new value when possible.
- Load `references/lookup-controls.md` for selector-specific rules.

## Designer Compatibility

- Forms must remain compatible with the Visual Studio WinForms Designer.
- Visual layout belongs in `.Designer.cs` for designer-backed forms, using explicit field declarations and classic `InitializeComponent` assignments.
- Keep runtime logic, events, permissions, API calls, lookup loading, validation, and request mapping in the main `.cs` partial class.
- Do not use C# collection expressions in `InitializeComponent`; use `new Control[] { ... }` and equivalent classic arrays.
- Do not build visual layout through runtime helper/factory methods that hide controls from the Designer.
- Load `references/designer-compatibility.md` before editing `.Designer.cs` files.

## Encoding Safety

- Preserve Spanish accented text in WinForms files. Do not leave mojibake caused by double-encoded UTF-8 or replacement characters.
- Prefer `apply_patch` for manual edits. Avoid bulk rewrites with PowerShell `Set-Content` unless the encoding is explicitly controlled.
- If a mechanical rewrite is necessary on Windows, read and write with .NET UTF-8 explicitly, preferably `new UTF8Encoding(false)`, so `.cs` and `.Designer.cs` files remain UTF-8 without BOM and accents stay valid.
- After any broad text rewrite in WinForms files, run the project mojibake scan for double-encoded UTF-8 markers and replacement characters, then fix matches before compiling.
- If accented text is already corrupted, repair the text first, then compile. Do not continue editing layout while the file contains mojibake.

## References

- Load `references/ui-checklist.md` before implementing a new screen.
- Load `references/enterprise-typography.md` when typography, grid fonts, AppTypography, prohibited fonts, or numeric alignment are relevant.
- Load `references/designer-compatibility.md` before editing `.Designer.cs`.
- Load `references/service-clients.md` before adding or changing WinForms API clients.
- Load `references/operational-forms.md` before designing or modifying operational workflows.
- Load `references/lookup-controls.md` when a form uses `LookUpEdit`, `SearchLookUpEdit`, or a selector backed by another table.
- Load `references/menu-integration.md` when a new form needs a menu entry or form-operation permissions.
- Use `$nuansystem-backend-crud` when an API endpoint or DTO must be added.
