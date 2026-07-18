---
name: nuansystem-winforms-controls
description: Select, configure, reuse, extend, or review NuanSystem corporate WinForms and DevExpress controls. Use when adding buttons, lookups, grids, KPI cards, shared editors, base forms, visual helpers, or proposing a new reusable frontend control under src/Frontend.
---

# NuanSystem WinForms Controls

## Authority

Read `.codex/ENGINEERING-CONSTITUTION.md`, follow `.codex/ENGINEERING-KERNEL.md`, run `$nuansystem-framework-discovery`, and inspect the frontend entries in `.codex/FRAMEWORK-CATALOG.md` before changing controls.

Use `$nuansystem-winforms-designer` for `.Designer.cs`, `$nuansystem-winforms-layout` for geometry, `$nuansystem-winforms-grids` for grid behavior, and `$nuansystem-winforms-lookups` for selector behavior.

## Selection workflow

1. Classify the need by behavior, not by requested control name.
2. Inspect the corporate component and its public contract.
3. Inspect one same-lifecycle consumer and one additional consumer for shared changes.
4. Reuse through configuration before subclassing.
5. Extend only for a reusable, backward-compatible gap.
6. Implement locally when the need is screen-specific.
7. Create a new shared control only after documenting searched alternatives, consumers, Designer impact, and validation.

## Corporate defaults

| Need | Default | Boundary |
|---|---|---|
| Standard CRUD list | `BaseGridCrudListForm` | Owns CRUD lifecycle and an internal `NuanDataGridControl` |
| Standard edit/consult | `BaseEditForm` | UI validation is not authoritative business validation |
| Standard action | `NuanActionButton` | Configure `ButtonKind` before custom styling |
| Related catalog selector | `NuanLookupEdit` | Use create/clear events with permissions |
| Reusable feature grid | `NuanDataGridControl` | Do not add a second instance to a standard CRUD list |
| KPI summary | `NuanKpiCardControl` | Presentation only, not editable data |
| Colors and logo | `BrandResources` | No local corporate color constants |
| Typography | `AppTypography` | One font source of truth |
| Form presentation | `FormStyler` | Never use it to create or reposition layout |
| API transport | `INuanApiClient` / `NuanApiClient` | Forms never create `HttpClient` |

## Direct DevExpress controls

Use DevExpress controls as low-level building blocks only when no corporate component covers the behavior or when an established base/control already owns them.

- Prefer `TextEdit`, `MemoEdit`, `SpinEdit`, `DateEdit`, `CheckEdit`, and other DevExpress editors over standard WinForms equivalents.
- Do not use `DataGridView` in DevExpress screens.
- Do not replace `NuanActionButton` with raw `SimpleButton` for standard actions.
- Do not recreate lookup clear/create buttons beside a direct `LookUpEdit` when `NuanLookupEdit` fits.
- Do not create another grid, paging, export, KPI, typography, brand, or HTTP wrapper.

## Base form rules

### `BaseGridCrudListForm`

Derive for standard maintenance lists. Configure data, columns, selection, CRUD operations, history, export, and personalization through its hooks. Do not fork the toolbar or grid lifecycle.

### `BaseEditForm`

Derive for create/edit/consult of administrative records. Override validation/request/persistence hooks. Preserve read-only mode and shared exception handling. Keep authoritative validation and persistence behind the API.

## Extension gate

Before modifying a shared component, record:

- missing public capability;
- configuration alternatives tried;
- all discovered consumers;
- serialization/Designer effect;
- default behavior compatibility;
- migration requirement;
- build, tests, and representative visual checks;
- required Framework Catalog and Knowledge Graph updates.

Do not change a shared default to fix one form. Prefer an opt-in property or local configuration when compatible.

## New control gate

Create a new shared control only when the behavior is needed by multiple screens and cannot be supplied safely by existing components. Define:

- single responsibility;
- public properties/events;
- design-time behavior;
- resource ownership/disposal;
- accessibility and keyboard behavior;
- visual source of truth;
- representative consumers and tests.

Do not create factories, managers, wrappers, or helpers merely to rename DevExpress APIs.

## Evidence examples

- `src/Frontend/NuanSystem.WinForms.Forms/Common/BaseEditForm.Designer.cs` demonstrates `NuanActionButton` in the shared edit lifecycle.
- `src/Frontend/NuanSystem.WinForms.Forms/Common/BaseGridCrudListForm.Designer.cs` demonstrates internal ownership of `NuanDataGridControl`.
- `src/Frontend/NuanSystem.WinForms.Forms/Sync/SyncMonitorForm.Designer.cs` demonstrates KPI, grid, and action controls in a monitor.
- `src/Frontend/NuanSystem.WinForms.Forms/GeneralInventory/ItemGroups/ItemGroupEditForm.Designer.cs` demonstrates `NuanLookupEdit`.

## Antipatterns

- Creating controls before Framework Discovery.
- Copying visual internals instead of using public contracts.
- Feature-specific behavior in a shared control.
- Runtime layout factories for designer-backed controls.
- Hard-coded fonts, brand colors, icons, auth headers, or company headers.
- Direct SQL, SAP, or business rules in controls/forms.
- Claiming shared compatibility after checking only one consumer.

## Completion checklist

- [ ] Corporate candidates were inspected and selection is justified.
- [ ] Public contracts are used without parallel infrastructure.
- [ ] Designer, disposal, permissions, and read-only behavior are preserved.
- [ ] Shared changes include consumer and compatibility evidence.
- [ ] Catalog/graph are updated for shared contract changes.
- [ ] Applicable frontend build/tests/Designer checks are truthfully reported.


