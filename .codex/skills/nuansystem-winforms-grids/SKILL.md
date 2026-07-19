---
name: nuansystem-winforms-grids
description: Configure, extend, or review NuanSystem DevExpress grids, including BaseGridCrudListForm, NuanDataGridControl, GridControl/GridView, columns, formats, paging, selection, status badges, summaries, export, personalization, events, and performance. Use for any frontend task that adds or changes a data grid.
---

# NuanSystem WinForms Grids

## Authority and discovery

Follow the engineering core and run `$nuansystem-framework-discovery`. Inspect:

- `src/Frontend/NuanSystem.WinForms.Forms/Common/BaseGridCrudListForm.cs`
- `src/Frontend/NuanSystem.WinForms.Forms/Common/BaseGridCrudListForm.Designer.cs`
- `src/Frontend/NuanSystem.WinForms.Controls/Grids/NuanDataGridControl.cs`
- the closest same-domain grid and its DTO/model

Use `$nuansystem-winforms-designer` for serialized grid structure and `$nuansystem-winforms-layout` for sizing.

## Grid selection

```text
Standard CRUD list?
  -> derive from BaseGridCrudListForm; use its internal NuanDataGridControl.
Feature/dashboard/detail grid outside CRUD base?
  -> use NuanDataGridControl when its contract fits.
Specialized DevExpress view/editor requirement not covered?
  -> use direct GridControl/GridView only with documented evidence.
```

Never add a second `NuanDataGridControl` to a standard CRUD list without a distinct, justified detail-grid requirement.

## Data and selection

- Use typed `SetGridData`/`SetData<T>` paths.
- Use typed focused/selected row helpers.
- Choose single versus multi-select deliberately.
- Enable selection checkboxes only when users perform real batch actions.
- Keep row double-click consistent with consult/edit permissions.
- Do not infer authoritative business state from a stale selected row; backend revalidates mutations.

## Column contract

For every visible column define deliberately:

- field/property name;
- caption;
- visible order;
- width or resizing behavior;
- alignment;
- display format;
- sort/filter behavior;
- editability/read-only behavior;
- null/empty representation;
- summary/status behavior where applicable.

Hide internal IDs, tenant keys, audit internals, tokens, raw integration payloads, and technical fields unless the screen explicitly requires them.

For coded fixed catalogs, display the approved readable label and the business value users need to identify the record. Keep the raw code available only when it has user-facing meaning or is needed for filtering/export; do not expose persistence-only codes accidentally.

## Formatting

- Use `AppTypography` for headers, rows, footer, and filter presentation.
- Right-align numeric values and summaries.
- Use consistent date/time, percentage, quantity, price, currency, and status formats from the closest domain pattern.
- Use status badge providers through `NuanDataGridControl` when applicable; do not duplicate row-paint logic.
- Do not use color as the only status signal.

## Paging, search, and filters

- Use the established paging contract and page size.
- Keep server-side paging/filtering when data volume or the existing API requires it.
- Reset or reconcile the current page when filters change.
- Preserve filter/search intent across refresh when the existing form pattern does.
- Do not load an unbounded dataset merely to use client filtering.
- Show loading, empty, filtered-empty, error, and stale states clearly.

## Personalization

- Use the same stable `FormKey` as navigation/security.
- Use stable `GridName` when a form contains multiple grids.
- Preserve saved settings when adding compatible columns.
- Treat column rename/removal as a compatibility change.
- Use shared customization paths; do not add feature-local column-setting stores.

## Export

- Reuse inherited/control export support.
- Export only visible/authorized data and approved columns.
- Preserve user/company metadata and logo through existing export contracts.
- Do not export hidden sensitive fields.
- Distinguish exporting the current page from the complete filtered result; never imply one is the other.

## Events and actions

- Keep focused-row, selection, and double-click handlers small and UI-focused.
- Route CRUD actions through base lifecycle hooks.
- Check permissions before enabling row/batch actions; backend remains authoritative.
- Avoid event recursion when binding, refreshing, or changing focused rows.
- Unsubscribe owned runtime handlers when required.

## Performance

- Avoid repeated full rebinding when a targeted refresh works with the existing pattern.
- Avoid expensive per-row service/API calls.
- Precompute display data server-side or in the ViewModel when appropriate.
- Keep custom drawing lightweight.
- Use cancellation/busy state for remote refresh.
- Inspect large-data behavior before enabling summaries, auto-width, or unbounded best-fit operations.

## Representative evidence

- `BaseGridCrudListForm` for CRUD selection, export, history, and personalization.
- `SyncMonitorForm.Designer.cs` for monitor grids and KPIs.
- `SyncOutboxDetailForm.Designer.cs` for a feature detail grid.
- `CitiesForm.cs`, `SupplierGroupsForm.cs`, and `RolesForm.cs` for CRUD-derived grids.
- `CarriersForm.cs` for an independent CRUD grid that presents identification type and identification number without inheriting BusinessPartners UI.

## Antipatterns

- Raw `DataGridView`.
- Parallel paging/export/personalization infrastructure.
- Columns generated accidentally from DTO internals.
- Local hard-coded corporate fonts/colors.
- Business mutations from cell formatting/custom draw.
- Unbounded client data loads.
- Claiming export scope or authorization without verifying it.

## Completion checklist

- [ ] Correct grid lifecycle/control was selected.
- [ ] Data, selection, columns, formats, filters, and paging are explicit.
- [ ] Personalization keys align with `FormKey` and `GridName`.
- [ ] Export and sensitive-field behavior are verified.
- [ ] Permissions, empty/error/busy states, and performance are addressed.
- [ ] Build, tests, Designer, and representative runtime checks are reported truthfully.


