# WinForms UI Checklist

This compatibility checklist supports older links from the WinForms orchestrator. The specialized skills are authoritative:

- `$nuansystem-winforms-forms` for lifecycle and form boundaries.
- `$nuansystem-winforms-controls` for corporate components.
- `$nuansystem-winforms-designer` for `.Designer.cs`.
- `$nuansystem-winforms-layout` for geometry and density.
- `$nuansystem-winforms-grids` and `$nuansystem-winforms-lookups` for data surfaces.
- `$nuansystem-winforms-navigation-security` for FormKey, menu, permissions, and company security.

## Intake

- Run Framework Discovery before implementation.
- Infer routine folder, naming, title, standard operations, and menu placement from the closest module when evidence is clear.
- Ask only when a missing decision materially changes product identity, destructive semantics, workflow, new top-level navigation, or default access policy.
- Build a vertical affected-layer map; do not assume a new screen is frontend-only.

## Services and ViewModels

- Add/extend typed clients through `INuanApiClient`.
- Keep request/response models in the established module area.
- Keep selected/list/draft, busy, and UI validation state in a ViewModel when the local lifecycle benefits.
- Preserve cancellation and consistent error presentation.

## Forms

- Select the lifecycle before the base form.
- Reuse corporate controls, typography, brand resources, and base forms.
- Keep visual structure in `.Designer.cs` and behavior in the main partial class.
- Complete read-only, busy, empty, error, and permission states.
- Keep folder, service, ViewModel, FormKey, menu, API authorization, and grid personalization aligned.

## Validation

- Build the touched frontend project/solution.
- Run targeted tests.
- Inspect Designer initialization and open Visual Studio Designer when available.
- Validate clipping, resizing, tab order, permissions, company context, loading/errors, and navigation.
- Report every check as Validated, Not validated, Not applicable, or Blocked.
