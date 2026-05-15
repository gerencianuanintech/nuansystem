# Lookup Controls Checklist

Use this when a form has `LookUpEdit`, `SearchLookUpEdit`, or any selector backed by another table/maintenance.

## Display

- Show at least `Codigo` and `Nombre` columns for business tables.
- Include description or status columns only when they help the user distinguish records.
- Use a readable display member. Prefer a `DisplayText` property like `"CODE - Name"` when available.
- Keep the value member aligned with the API contract: usually `Id`, sometimes `Code` if the backend expects a code.
- Set `NullText = ""` unless the form needs placeholder text.

## Related Create Action

- Provide a way to add a missing related option when the selector points to another maintenance table.
- Use a nearby plus button or a lookup editor button with a clear tooltip/caption.
- Show or enable the create action only when the user has create access for that related maintenance.
- Check create access through `ApiSession.HasPermission(...)`, `CrudOperationPermissions`, or loaded form-operation access for the related `FormKey`.
- If the user lacks create access, hide the create action or keep it disabled; do not open the related maintenance in create mode.

## After Create

- Open the related maintenance's create/edit form using the established factory/service pattern.
- On successful save, reload the lookup data source.
- Select the newly created option when the related form or service returns its Id/code.
- Preserve the current form state and validation messages while refreshing lookup data.

## Backend Requirements

- Ensure the API provides lookup data with code and name fields.
- If the related maintenance does not exist yet, use `$nuansystem-backend-crud` and `$nuansystem-sql-standards` to create the missing backend and SQL support.
