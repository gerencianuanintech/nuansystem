---
name: nuansystem-winforms-layout
description: Design, modify, or review NuanSystem WinForms and DevExpress visual layout, spacing, alignment, hierarchy, anchoring, docking, sections, tabs, headers, content, footers, typography, and resizing. Use for new forms or any change to control position, size, grouping, density, or responsive behavior.
---

# NuanSystem WinForms Layout

## Authority and evidence

Run `$nuansystem-framework-discovery`, reuse its core record, load `$nuansystem-winforms-designer`, and inspect:

- `docs/estandar-visual-winforms.md`
- `docs/FRONTEND-DEVEXPRESS-NOMENCLATURA.md`
- `.codex/skills/nuansystem-winforms-devexpress/references/enterprise-typography.md`
- the closest approved form and its base/Designer files
- `src/Frontend/NuanSystem.WinForms.Forms/Carriers/CarrierEditForm.Designer.cs` at commit `bc7e73f6` for the approved compact CRUD vertical rhythm

Do not invent universal pixel values that are not established. Prefer the closest form family. For compact CRUD edit forms, use 22 px editor height and a 28 px vertical top-to-top cadence, leaving 6 px of visible vertical space between consecutive single-line editors.

## Layout workflow

1. Classify the form with `$nuansystem-winforms-forms`.
2. Inspect base-form inherited regions before placing controls.
3. Identify header/title, content, navigation/tabs, grid/detail, status, and action regions.
4. Choose the closest same-lifecycle reference.
5. Define hierarchy, alignment, sizing, anchoring/docking, and tab order in `.Designer.cs`.
6. Apply corporate typography and resources without moving layout at runtime.
7. Validate minimum size, resize behavior, clipping, overlap, keyboard flow, and read-only state.

## Hierarchy rules

- Keep the primary title visually stronger than section titles.
- Group fields by business meaning, not storage table order.
- Keep primary actions visible and consistent with the selected form lifecycle.
- Keep destructive actions separated and clearly identified through corporate action kinds.
- Avoid excessive boxed sections and nested containers.
- Preserve a predictable left-to-right, top-to-bottom reading order.
- Align labels, editor starts, editor widths, and numeric values consistently.

## CRUD layout

- Use the inherited regions of `BaseGridCrudListForm` and `BaseEditForm`.
- For designer-backed maintenance forms, prefer direct placement on the form or `XtraTabPage` when that is the established family pattern.
- Do not add `Panel`, `PanelControl`, `TableLayoutPanel`, `FlowLayoutPanel`, or layout helpers merely to recreate an existing CRUD arrangement.
- Use tabs only for meaningful groups that cannot fit clearly in the primary view.

## Operational/document layout

- Do not force operational workflows into CRUD geometry.
- Separate header, detail lines, totals/status, and posting actions visibly.
- Use flat `PanelControl` sections with explicit titles only when the operational reference pattern uses them.
- Keep authoritative totals and statuses visibly distinct from editable input.
- Keep duplicate-submit-sensitive primary actions stable during busy state.

## Dashboard/monitor layout

- Use `NuanKpiCardControl` for comparable summary metrics.
- Use `NuanDataGridControl` for monitored rows when its behavior fits.
- Keep filters, refresh state, last-update/error indicators, KPIs, and detail grid visually distinct.
- Preserve usable layout when KPI text grows or the form resizes.

## Density and typography

- Use `AppTypography` and `BrandResources`.
- Use Segoe UI according to the existing typography reference.
- Standard dense single-line editors use 22 px height.
- Compact CRUD field rows use a 28 px top-to-top cadence: `nextY = currentY + 28`.
- With a 22 px single-line editor, this produces 6 px of visible vertical separation. Do not interpret 28 px as the empty gap.
- Keep each label vertically aligned with its editor. In the approved `CarrierEditForm` reference, label Y is editor Y + 3 px.
- Continue the 28 px row origin sequence when control types remain single-line. After a multiline editor, calculate the next control from the multiline editor's actual bottom plus the locally approved section gap; do not force it into a single-line row.
- Do not compress compact CRUD rows back to 26 px or introduce arbitrary 30/31 px steps unless the selected form family documents an exception.
- Right-align quantities, counters, percentages, prices, costs, totals, dimensions, and other numeric business values.
- Do not introduce Tahoma, Arial, Times New Roman, or feature-local font constants.
- Do not use runtime typography helpers to resize or reposition controls.

## Sizing and resizing

- Set explicit `MinimumSize` when the screen has a minimum usable geometry.
- Use `Anchor` for controls that retain distances to form edges.
- Use `Dock` for true region ownership, not as a shortcut for unclear geometry.
- Ensure grids/detail areas grow while action areas remain usable.
- Check long Spanish labels, validation messages, high-DPI scaling, and standard Windows font scaling when practical.
- Preserve tab order after moving or adding controls.

## Container decision

```text
Standard CRUD family already defines the region?
  -> place controls in that region; do not add a container.
Operational/dashboard section has an approved container pattern?
  -> use that pattern explicitly in Designer.
Need scrolling for a justified long surface?
  -> inspect nearby XtraScrollableControl/LayoutControl usage first.
No evidence?
  -> keep the simplest explicit hierarchy and document the decision.
```

## Antipatterns

- Building layout through `BuildHeader`, `AddField`, factories, loops, or LINQ.
- Repairing Designer geometry from `Load`/`Shown` events.
- Arbitrary margins, fonts, colors, or control heights.
- Unnecessary nested panels/group boxes.
- Overlap, clipping, negative coordinates, broken z-order, or unreachable actions.
- Tabs used as a substitute for clear information architecture.
- A visually attractive screen that violates the form's business lifecycle.

## Validation checklist

- [ ] Closest approved layout and base form were inspected.
- [ ] Hierarchy and geometry are explicit in `.Designer.cs`.
- [ ] Corporate typography/resources and documented density are used.
- [ ] Compact CRUD rows use the approved 28 px top-to-top cadence and 6 px visible single-line gap, unless a documented family exception applies.
- [ ] Resize, minimum size, clipping, overlap, tab order, and numeric alignment were checked.
- [ ] Read-only, busy, empty, error, and permission states remain usable.
- [ ] Build and Designer opening are reported separately and truthfully.


