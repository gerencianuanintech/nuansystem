---
name: nuansystem-winforms-forms
description: Classify, create, modify, or review complete NuanSystem WinForms forms and screen lifecycles, including CRUD lists/editors, operational workflows, documents, dashboards, monitors, dialogs, selectors, and wizards. Use when adding a screen or deciding its base form, ViewModel, service, state, actions, and validation flow.
---

# NuanSystem WinForms Forms

## Authority

Follow the engineering core and run `$nuansystem-framework-discovery`. Use `$nuansystem-winforms-controls`, `$nuansystem-winforms-designer`, `$nuansystem-winforms-layout`, and the specialized grid/lookup/navigation skills as applicable.

Use `.codex/PATTERN-CATALOG.md` to select the lifecycle before choosing inheritance.

## Classification tree

```text
Changes stock, money, pricing, cash, document, workflow, sync, or external state?
  -> operational/document/integration screen.
Administers master or configuration data?
  -> standard CRUD/catalog screen.
Summarizes and inspects state?
  -> dashboard/monitor.
Collects one bounded decision?
  -> dialog/selector.
Has real sequential steps with per-step validation?
  -> wizard.
```

Do not classify by the presence of a grid or Save button.

## Form intake

Before implementation, determine or infer safely:

- domain and folder;
- form class and visible title;
- lifecycle/pattern;
- base form or explicit reason for none;
- frontend client/ViewModel dependencies;
- `FormKey`, menu placement, operations, and default access when navigable;
- company/capability behavior;
- read-only/consult behavior;
- validation and busy/error states;
- Designer and build validation.

Stop only for product decisions that materially change identity, destructive semantics, fields, permissions, or workflow.

## Standard CRUD list

- Derive from `BaseGridCrudListForm`.
- Reuse inherited `NuanDataGridControl`, CRUD actions, selection, export, history, and column personalization.
- Configure data with `SetGridData`, visible columns in `ConfigureGridColumns`, and selected records through typed selection helpers.
- Use the same `FormKey` for navigation, operation access, and column settings.
- Open a concrete edit form for create/edit/consult rather than a generic end-user catalog editor.

## Standard edit/consult

- Derive from `BaseEditForm`.
- Use its validation, request, persistence, error, and read-only lifecycle.
- Keep layout explicit in `.Designer.cs`.
- Use typed frontend services over `INuanApiClient`.
- Keep UI validation helpful but non-authoritative.
- Ensure consult mode disables all mutation paths, including related-create actions.

## Operational/document screen

- Use a dedicated form shaped around the transaction.
- Keep header/detail/draft/busy/status state in a ViewModel when interactions are non-trivial.
- Send user intent to the API and render authoritative results.
- Do not calculate authoritative stock, tax, price, cost, payment, or accounting values in the form.
- Prevent duplicate posting while async work runs.
- Preserve correctable user input after business validation errors.
- Reflect transaction states and allowed actions clearly.

Load `.codex/skills/nuansystem-winforms-devexpress/references/operational-forms.md`.

## Dashboard/monitor

- Use KPI/grid corporate controls when they fit.
- Model loading, empty, degraded, stale, success, and error states.
- Keep refresh/cancellation predictable.
- Do not compute global KPIs from an incomplete client page.
- Use `SyncMonitorForm` only as a monitor reference, not as a universal dashboard template.

## Dialog/selector

- Return a clear result and support cancellation without side effects.
- Keep the scope bounded; do not hide a multi-step operational transaction in a modal.
- Use corporate actions and lookup/grid controls.
- Preserve owner/form state when opening related maintenance.

## Wizard

- Use only when steps have real order and validation boundaries.
- Define owned state, per-step validation, Back/Next/Cancel behavior, final API command, and failure recovery.
- Do not implement a wizard as tabs with hidden business state.

## Form, ViewModel, and service boundaries

### Form

Own event coordination, binding, focus, visible UI state, and user messages.

### ViewModel

Own selected/list/draft state, busy state, UI validation messages, and screen orchestration when the local pattern supports it.

### Typed client

Own routes and request/response transport mapping through `INuanApiClient`.

### Backend

Own authorization, tenant, business rules, authoritative validation/calculation, persistence, SAP/SRI, and synchronization.

## Failure and async behavior

- Disable mutation actions during in-flight requests.
- Restore actions after success, failure, cancellation, or exception.
- Use shared exception/error presentation.
- Do not show SQL, tokens, connection strings, stack traces, or external credentials.
- Handle form closing/cancellation without leaving inconsistent UI state.

## Representative patterns

- CRUD list: `Geography/Cities/CitiesForm.cs`, `Security/Roles/RolesForm.cs`.
- CRUD edit: `Geography/Countries/CountryEditForm.cs`, `Security/Roles/RoleEditForm.cs`.
- Complex supplier editor: `BusinessPartners/SupplierEditForm.cs`.
- Monitor: `Sync/SyncMonitorForm.Designer.cs`.

## Antipatterns

- Forcing operational work into CRUD inheritance.
- Business rules or direct integrations in forms/ViewModels.
- Generic final-user forms for unrelated administrable catalogs.
- Form-only persisted changes without vertical contract inspection.
- Creating `HttpClient` or headers in forms.
- Starting implementation before form lifecycle and security are known.

## Completion checklist

- [ ] Lifecycle/pattern and reference form are explicit.
- [ ] Base form choice is justified.
- [ ] Form/ViewModel/client/backend responsibilities are preserved.
- [ ] Corporate controls, layout, Designer, grids/lookups, and security skills were applied.
- [ ] Busy, error, empty, consult, and permission states are complete.
- [ ] Affected vertical layers and validation evidence are reported.


