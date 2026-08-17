---
name: nuansystem-winforms-lookups
description: Implement or review NuanSystem lookup and selector behavior, including binding, stable values, dependent filters, related creation, permissions, refresh, and selection. Use only when a task adds or changes a lookup contract or selector lifecycle; do not load it merely because an unchanged form contains a lookup.
---

# NuanSystem WinForms Lookups

## Authority and discovery

Run `$nuansystem-framework-discovery`, reuse its core record, and inspect:

- `src/Frontend/NuanSystem.WinForms.Controls/Lookups/NuanLookupEdit.cs`
- `.codex/skills/nuansystem-winforms-devexpress/references/lookup-controls.md`
- a same-domain lookup consumer and its typed client
- related maintenance permissions and edit form

Use `$nuansystem-winforms-designer` for serialized editors and popup views.

## Control selection

```text
Standard related catalog with clear/create behavior?
  -> NuanLookupEdit.
Small fixed enum/list with no related maintenance?
  -> direct closed LookUpEdit using the approved fixed-catalog contract.
Large searchable dataset or multi-column discovery need?
  -> inspect approved SearchLookUpEdit/GridLookUpEdit pattern.
No fitting pattern?
  -> document the gap before creating another lookup wrapper.
```

Do not add an external plus button when `NuanLookupEdit` already provides the corporate create action.

## Fixed closed catalog contract

Use a direct `LookUpEdit` only for a small approved set that has no independent maintenance or remote lifecycle.

- Declare the editor in `.Designer.cs` with an explicit combo button and `TextEditStyle = TextEditStyles.DisableTextEditor`.
- Bind a typed local list with separate stable code and visible text.
- Set `ValueMember` to the persisted code and `DisplayMember` to readable text; never persist `SelectedIndex` or the caption.
- Do not expose free text, clear, related-create, refresh, or an API lookup endpoint unless the product contract later introduces that lifecycle.
- Enforce the same closed set in backend validation and the database when it is a persisted business contract.
- Use `CarrierEditForm` as the approved example: SRI codes `04`, `05`, and `06` are local fixed values and remain independent from BusinessPartners.

## Binding contract

- Use a stable API value member, normally `Id` and sometimes `Code` only when the contract requires it.
- Show at least business code and name when both exist.
- Use readable display text such as `CODE - Name` when the local model provides it.
- Keep display and value semantics separate.
- Use `NullText = ""` unless a documented placeholder is required.
- Avoid binding persistence entities directly to UI.
- Preserve selection across refresh when the selected value still exists.

## Related creation flow

```text
User invokes Create
  -> check related FormKey/create permission
  -> open approved related edit form in create mode
  -> save through typed API client
  -> close only on successful persistence
  -> reload lookup source
  -> select returned Id/code
  -> preserve parent form state and validation
```

- Disable or hide create when permission is absent.
- Disable create in parent consult/read-only mode.
- Do not infer success merely because the dialog closed.
- If the related form cannot return identity, refresh and locate by an approved stable key; do not guess.

## Clear behavior

- Enable clear only when the field is optional or the workflow supports removing the relation.
- Update bound state and validation consistently after clear.
- Do not permit UI clear when the backend contract requires a value.
- In consult/read-only mode, clear remains disabled.

## Dependent lookups

For country/province/city, warehouse/branch, or other dependency chains:

1. Load parent options.
2. Clear invalid child selection when parent changes.
3. Cancel/ignore stale async child loads.
4. Load child options using the stable parent identifier.
5. Preserve valid selection during edit loading.
6. Keep backend validation authoritative.

Do not filter only by display text or mutable labels.

## Search/popup grids

- Show only useful identification/status columns.
- Apply `AppTypography` to popup headers and rows.
- Disable editing in lookup popup views.
- Use server-side search/paging for large catalogs when the API supports it.
- Keep filters company-aware and permission-aware.
- Do not expose inactive/unauthorized options unless the use case explicitly needs historical display.

## Async and errors

- Use typed clients over `INuanApiClient`.
- Pass cancellation where supported.
- Avoid repeated loads for identical dependencies.
- Show an empty/error state without leaving stale valid-looking options.
- Do not block the UI thread with remote lookup loading.
- Preserve existing selected historical values when edit/consult requires them, even if they are no longer generally selectable, using the established domain policy.

## Representative evidence

- `Definitions/Inventory/ItemGroups/ItemGroupEditForm.Designer.cs`.
- `Security/Users/UserEditForm.Designer.cs`.
- `Sync/Configuration/SyncProfileBranchDialog.Designer.cs`.
- `Sync/Configuration/SyncProfileEntityDialog.Designer.cs`.

## Antipatterns

- Duplicated plus/clear buttons and handlers.
- Creation without related-maintenance permission.
- Refresh without selecting the created record.
- Binding by mutable display text.
- Direct SQL/API transport in the control/form.
- N+1 API calls per displayed lookup row.
- Leaving an invalid child value after parent changes.
- Popup views with inconsistent fonts or editable columns.
- A fixed catalog that accepts free text or persists its visible caption/index.

## Completion checklist

- [ ] Correct selector/control and stable value contract were selected.
- [ ] Fixed catalogs are closed in the Designer and enforced consistently by frontend, backend, and database.
- [ ] Code/name display and null semantics are explicit.
- [ ] Create/clear actions respect permission, read-only mode, refresh, and identity.
- [ ] Dependent and async behavior handles stale/invalid selections.
- [ ] Backend/API requirements and company scope were verified.
- [ ] Designer, build, tests, and runtime behavior are reported truthfully.


