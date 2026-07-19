---
name: nuansystem-winforms-designer
description: Create or modify NuanSystem WinForms and DevExpress designer-backed forms while preserving Visual Studio Designer serialization, explicit layout, corporate controls, disposal, initialization order, and review evidence.
---

# NuanSystem WinForms Designer

## Purpose

Protect Visual Studio Designer compatibility and keep visual structure editable by the NuanSystem team. Use whenever a task adds, removes, moves, resizes, anchors, docks, groups, or styles controls in a designer-backed form.

## Authority and prerequisites

Before editing:

1. Read `.codex/ENGINEERING-CONSTITUTION.md`.
2. Follow `.codex/ENGINEERING-KERNEL.md`.
3. Run `nuansystem-framework-discovery`.
4. Read the frontend entries in `.codex/FRAMEWORK-CATALOG.md`.
5. Inspect the form's `.cs`, `.Designer.cs`, and `.resx` when present.
6. Inspect the base form and at least one approved similar form.

## Core boundary

```text
.Designer.cs
  -> declares controls
  -> initializes visual properties
  -> establishes hierarchy, docking, anchoring, size, location, tab order
  -> wires stable designer events where the repository pattern does so

Form .cs
  -> loads data
  -> binds state
  -> handles behavior/events
  -> applies permissions/read-only state
  -> coordinates typed services
```

Runtime code must not secretly reconstruct the visual tree of a designer-backed form.

## Corporate controls first

Evaluate before direct controls:

- `NuanActionButton` for standard actions.
- `NuanLookupEdit` for corporate lookup behavior.
- `NuanDataGridControl` for reusable feature grids.
- `NuanKpiCardControl` for KPI cards.
- `BaseGridCrudListForm` and `BaseEditForm` for standard CRUD lifecycles.
- `BrandResources`, `AppTypography`, and `FormStyler` for presentation.

Direct DevExpress controls may be used when they are the established low-level building block and no corporate component covers the requirement. Document the decision; do not duplicate corporate behavior.

## Mandatory Designer rules

### Declarations

- Keep the form `partial`.
- Declare each designer-owned control as an explicit field.
- Use the concrete designer-serializable type.
- Use established control naming from the closest form family.
- Keep `IContainer components` ownership/disposal consistent with generated patterns.
- Do not create controls through local factories, loops, LINQ, reflection, or collection expressions.

### InitializeComponent

- Instantiate controls explicitly.
- Keep component/resource manager initialization conventional.
- Call `SuspendLayout` before bulk layout and `ResumeLayout(false)` / `PerformLayout` as required.
- Balance every `BeginInit` with `EndInit`.
- Balance nested panel/control layout suspension.
- Add controls to the correct parent in deterministic order.
- Set `Name`, `Size`, `Location`, `Dock`, `Anchor`, `TabIndex`, and relevant appearance properties explicitly.
- Preserve base-form inherited controls and modifiers.
- Keep event wiring consistent with nearby Designer files.
- Do not split `InitializeComponent` across hidden helpers.
- For a direct closed `LookUpEdit`, serialize the combo button and `TextEditStyle = TextEditStyles.DisableTextEditor` explicitly; do not rely on design-time defaults.

### Layout

- Define header, content, navigation/tabs, grid/detail area, and action/footer hierarchy explicitly.
- Use docking/anchoring according to the closest approved form.
- Avoid negative coordinates, overlapping controls, unreachable actions, and implicit z-order.
- Preserve minimum supported form size and usable resizing.
- Maintain tab order and keyboard accessibility.
- Do not move, resize, font, or recolor designer-owned controls later from feature logic merely to repair incomplete Designer layout.

### Resources and presentation

- Reuse `BrandResources` and `AppTypography`; do not introduce local corporate constants.
- Use `FormStyler` only for its established presentation contract, never as a layout builder.
- Use embedded resources for icons/images when the project pattern requires Designer serialization.
- Preserve localization/resource application when present.
- Do not dispose shared/static resources from a form.

### Disposal

- Dispose `components` in the standard override.
- Unsubscribe/dispose runtime subscriptions and owned resources in the form code where needed.
- Do not double-dispose controls owned by the Designer hierarchy.

## Compatibility restrictions

Avoid language constructs in `.Designer.cs` that the Visual Studio serializer may not emit or preserve, including:

- `var`;
- collection expressions;
- target-typed object creation when it obscures serialized types;
- LINQ;
- loops that create visual controls;
- local functions;
- factories/helpers for control creation or layout;
- conditional creation based on runtime state;
- async code;
- dependency injection or service resolution;
- business logic;
- dynamic resource or permission decisions.

Use plain, explicit, serializer-friendly statements modeled on existing Designer files.

## Inheritance decision tree

```text
Standard grid CRUD list?
  -> derive from BaseGridCrudListForm.
Standard CRUD edit/consult?
  -> derive from BaseEditForm.
Operational/document/dashboard/dialog?
  -> select its pattern; do not force CRUD inheritance.
Existing specialized base in same domain?
  -> inspect its contract and consumers before choosing.
No reliable base?
  -> document discovery gap before creating one.
```

## Change workflow

1. Inspect the complete form triplet and base class.
2. Run Framework Discovery and select reference form.
3. Sketch the intended control hierarchy.
4. Update declarations and initialization explicitly.
5. Update runtime behavior separately.
6. Verify permission/read-only/data-binding implications.
7. Build the affected frontend projects.
8. Inspect the final Designer file for balanced initialization/layout.
9. After opening/saving in Visual Studio Designer, review the semantic diff: preserve intended `.resx` creation but restore required properties the serializer removed or reset.
10. Open the form in Visual Studio Designer when the environment supports it.
11. Execute frontend gates in `.codex/REVIEW-CHECKLIST.md`.

## High-risk shared Designer changes

Changes to these require consumer discovery across domains:

- `BaseEditForm.Designer.cs`;
- base CRUD form designers;
- corporate controls or their defaults;
- typography/brand defaults;
- shared grid/lookup/button serialization behavior.

For shared changes, record consumers, backward compatibility, representative visual checks, and catalog/graph updates.

## Antipatterns

Do not:

- create all controls at runtime for a designer-backed form;
- hide layout inside `BuildLayout`, factories, loops, or extensions;
- use raw `SimpleButton`, `LookUpEdit`, grid wrappers, or KPI panels to bypass a fitting corporate control;
- place API calls, SQL, SAP, totals, or business decisions in `InitializeComponent`;
- manually attach JWT/company headers;
- mutate control hierarchy from async loading;
- fix one screen by changing a shared control default without consumer review;
- claim the Designer opens because the project compiles.
- accept a Designer-generated diff without checking closed-editor behavior, inherited controls, tab order, resources, and established layout cadence.

## Validation evidence

Report separately:

- **Build:** command/project and result.
- **Static Designer inspection:** files and checks performed.
- **Visual Designer:** opened successfully, not validated, or blocked with reason.
- **Runtime/manual:** scenario and result, when performed.
- **Tests:** targeted tests and result.

Build success does not prove Designer success. Static inspection does not prove runtime layout.

## Completion checklist

- [ ] Core engineering documents and discovery skill were followed.
- [ ] Base form and corporate controls were selected deliberately.
- [ ] Designer declarations and initialization are explicit.
- [ ] Layout ownership remains in `.Designer.cs`.
- [ ] Initialization/layout/disposal calls are balanced.
- [ ] A post-Designer semantic diff confirms required editor behavior and intentional `.resx` changes.
- [ ] Corporate typography/resources are reused.
- [ ] Runtime code contains behavior, not hidden layout.
- [ ] Permissions and read-only behavior are addressed.
- [ ] Build and applicable tests were executed or honestly classified.
- [ ] Designer opening was validated or reported accurately.
- [ ] Shared changes include consumer, catalog, and graph review.
