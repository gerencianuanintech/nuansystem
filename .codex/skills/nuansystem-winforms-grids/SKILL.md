---
name: nuansystem-winforms-grids
description: Configure, extend, or review NuanSystem DevExpress grids, including BaseGridCrudListForm, NuanDataGridControl, GridControl/GridView, columns, formats, paging, selection, status badges, summaries, export, personalization, events, and performance. Use for any frontend task that adds or changes a data grid.
---

# NuanSystem WinForms Grids

## Authority and discovery

Run `$nuansystem-framework-discovery`, reuse its core record, and inspect:

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
- Keep pagination controls inside `NuanDataGridControl`; forms must not add their own first, previous, next, last, page-size, page-count, or total-count controls.
- For server-paged APIs, bind the returned slice through `SetPagedData` and handle `PageRequested` only to request and rebind the target page.
- In `BaseGridCrudListForm`, use the inherited `EnableServerFind` callback only when the feature API implements global text filtering. The base owns Find normalization, debounce, event wiring, and disposal; the callback applies the typed filter, resets the page, and reloads.
- Keep server-side paging/filtering when data volume or the existing API requires it.
- Reset or reconcile the current page when filters change.
- Preserve filter/search intent across refresh when the existing form pattern does.
- Do not load an unbounded dataset merely to use client filtering.
- Show loading, empty, filtered-empty, error, and stale states clearly.

## Personalization

- Use the same stable `FormKey` as navigation/security.
- Use stable `GridName` when a form contains multiple grids.
- For administrative master lists, register every persisted projection column in the grid: business fields, relationship identities, `Id`, `GlobalId`, creation/update/delete audit fields, and logical-deletion state. Keep technical/audit columns hidden by default unless explicitly approved as visible.
- Do not depend on `PopulateColumns()` to discover the contract. Explicitly add missing columns before customization so “Seleccionar columnas” remains complete when the result set is empty.
- Verificar la cadena completa `SELECT/procedimiento SQL -> DTO de Application -> modelo del cliente -> GridView.Columns.AddField`. Una columna persistida omitida en cualquiera de esos cuatro puntos se considera un defecto, aunque el listado muestre correctamente las columnas principales.
- Probar la personalización con cero filas y con datos. El selector debe conservar todas las columnas persistidas autorizadas; las columnas técnicas y de auditoría deben existir pero iniciar ocultas.
- Mantener una prueba contractual que enumere las columnas persistidas esperadas y falle cuando el DTO, el modelo frontend o la declaración explícita de la grilla omita una de ellas.
- Exclude computed display-only properties, secrets, tokens, raw integration payloads, and fields absent from the authorized API projection.
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

### Embedded grid operation bar

For a grid inside an edit, detail, operational, or document form, render its local operations as a compact link-style bar immediately above the grid and align the group to the right.

- Use borderless, transparent `SimpleButton` controls with Segoe UI 9 pt, 28 px height, a 16 x 16 operation icon on the left, 4 px icon/text spacing, and no filled card or panel background.
- Use the established labels and semantic colors when applicable: `Agregar` with the green plus icon, `Editar` with the blue pencil icon, `Quitar` with the red minus icon, and `Marcar principal` with the green approval/check icon.
- Use the matching semantic operation icon and blue or established state color for additional actions such as abrir, descargar, actualizar, or limpiar.
- Reuse `OperationButtonIcons` and `Assets/Icons/Operaciones`; do not draw glyphs, use Unicode symbols, or add feature-local copies of corporate operation icons.
- Keep unavailable operations visible but disabled when the form-operation permission contract requires discoverability. Do not rely on frontend state as authorization.
- Stop title separator lines before the action group; never draw a line behind button text or icons.
- Reserve filled `NuanActionButton` controls for primary form or workflow actions such as Guardar, Cancelar, Aprobar, or Procesar. Do not use those 100 x 36 buttons inside an embedded grid operation bar.

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
- [ ] Todas las columnas persistidas autorizadas recorren SQL, DTO, modelo y `AddField`, y siguen disponibles con una fuente vacía.
- [ ] Export and sensitive-field behavior are verified.
- [ ] Permissions, empty/error/busy states, and performance are addressed.
- [ ] Build, tests, Designer, and representative runtime checks are reported truthfully.


