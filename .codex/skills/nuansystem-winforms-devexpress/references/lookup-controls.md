# Lookup Controls Compatibility Reference

Use `$nuansystem-winforms-lookups` as the authoritative workflow. This reference remains for older links.

## Selection

- Prefer `NuanLookupEdit` for related catalogs requiring corporate clear/create behavior.
- Use direct `LookUpEdit`, `SearchLookUpEdit`, or `GridLookUpEdit` only when the corporate control does not cover the established selector behavior.
- Do not add a parallel plus/clear button when `NuanLookupEdit` fits.
- Inspect the closest same-domain lookup, related edit form, typed client, and permissions.

## Display and value

- Show business code and name when both exist.
- Use a readable display member and stable API value member.
- Keep display text separate from identity.
- Use empty `NullText` unless a documented placeholder is required.
- Apply corporate typography to popup views.

## Related creation

- Check create permission for the related maintenance FormKey.
- Disable creation in parent consult/read-only mode.
- Open the approved related edit form.
- Reload the lookup only after confirmed persistence.
- Select the returned stable Id/code.
- Preserve parent form state and validation.

## Dependencies and validation

- Clear invalid child selections when a parent lookup changes.
- Ignore stale async loads and preserve valid edit selections.
- Keep backend validation and company isolation authoritative.
- Validate create/clear permissions, refresh/selection, null semantics, dependent filters, Designer behavior, build, and errors.
