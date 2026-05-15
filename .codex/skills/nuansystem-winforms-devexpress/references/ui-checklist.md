# WinForms UI Checklist

## New Form Intake

- Ask for the destination folder/module name before creating a new form unless the user already provided it.
- Ask how the form should appear in the menu unless the user already provided parent menu, label, `FormKey`, and operations.
- Create the form and menu/security registration together when requested.

## Services

- Add an interface in `NuanSystem.WinForms.Services`.
- Implement calls through `INuanApiClient`.
- Use `GetAsync`, `PostAsync`, `PutAsync`, and `DeleteAsync` with route paths only.
- Keep request/response records in a `Models` folder.

## ViewModels

- Inherit existing base view model classes where nearby modules do.
- Keep selected item, list state, loading state, and validation messages outside the form where possible.
- Use async methods and pass `CancellationToken` when the existing pattern supports it.

## Forms

- Use DevExpress controls and approved prefixes.
- Apply `FormStyler` and `BrandResources`.
- Wire buttons to base form operations where available.
- Use `RunWithBusyStateAsync` for async CRUD operations.
- Use consult/read-only mode for view-only actions.
- Keep the form folder, view model folder, service folder, and menu `FormKey` aligned.

## Lookup/Search Lookup Controls

- For `LookUpEdit` or `SearchLookUpEdit` bound to another table, show `Codigo` and `Nombre` columns.
- Add a create-related option only when the user has create permission for that related maintenance.
- Refresh the lookup and select the new record after successful related creation.

## Validation

- Build the touched frontend project when practical.
- Manually inspect designer/code-built layouts for text clipping, missing anchoring, and inconsistent fonts.
