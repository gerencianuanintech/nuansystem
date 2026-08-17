# Enterprise Typography Reference

## Contents

1. [Official font](#official-font)
2. [Official table](#official-table)
3. [ERP density](#erp-density)
4. [Input controls](#input-controls)
5. [Prohibited fonts](#prohibited-fonts)
6. [GridView rules](#gridview-rules)
7. [AppTypography](#apptypography)
8. [Designer visibility](#designer-visibility)
9. [Operational sections](#operational-sections)
10. [Checklist](#checklist)

Use this reference from `nuansystem-winforms-devexpress` when creating or reviewing NuanSystem WinForms DevExpress typography. Typography is no longer a separate primary skill; it is a specialized frontend reference.

## Official Font

The official WinForms DevExpress font family is `Segoe UI`.

Do not change the official font unless the user explicitly asks for a project-wide visual redesign.

## Official Table

| Element | Font | Size | Style |
| --- | --- | ---: | --- |
| Base form | Segoe UI | 9 pt or 9.75 pt | Regular |
| Dense ERP form | Segoe UI | 9 pt | Regular |
| More legible form | Segoe UI | 9.75 pt | Regular |
| Main title | Segoe UI Semibold | 14 pt | Bold/Semibold |
| Subtitle or descriptive text | Segoe UI | 10 pt | Regular |
| Section title | Segoe UI Semibold | 11 pt | Bold/Semibold |
| Field label | Segoe UI | 9 pt | Regular |
| Input controls | Segoe UI | 9 pt or 9.75 pt | Regular |
| SimpleButton | Segoe UI Semibold | 9 pt | Bold/Semibold |
| GridView header | Segoe UI Semibold | 9 pt | Bold |
| GridView rows | Segoe UI | 9 pt | Regular |
| GridView footer | Segoe UI Semibold | 9 pt | Bold |
| GridView filter row | Segoe UI | 9 pt | Regular |
| RibbonControl, AccordionControl, NavBarControl, BarManager | Segoe UI | 9 pt | Regular |
| Helper text or small notes | Segoe UI | 8.5 pt | Regular |

## ERP Density

- Standard single-line editors in dense ERP forms use `Size = new Size(width, 22)`.
- Treat row spacing as layout geometry governed by `$nuansystem-winforms-layout`; compact CRUD edit forms currently use a 28 px top-to-top cadence, which leaves 6 px visible space between 22 px single-line editors.
- Apply this to `TextEdit`, `SearchLookUpEdit`, `LookUpEdit`, `DateEdit`, and `ComboBoxEdit` unless a local form family has a documented exception.
- Main titles must be visually larger than section titles; section titles must not compete with screen titles.

## Input Controls

Apply input typography to:

- `TextEdit`
- `LookUpEdit`
- `GridLookUpEdit`
- `SearchLookUpEdit`
- `ComboBoxEdit`
- `DateEdit`
- `SpinEdit`
- `MemoEdit`
- `ButtonEdit`
- `CalcEdit`

Pattern:

```csharp
control.Properties.Appearance.Font = AppTypography.InputFont;
control.Properties.Appearance.Options.UseFont = true;
```

## Prohibited Fonts

Do not introduce decorative, informal, or non-enterprise fonts in normal UI:

- Comic Sans
- Impact
- Brush Script
- Papyrus
- Times New Roman
- Courier New

`Consolas` is allowed only for logs, SQL, JSON, code, and technical diagnostics.

## GridView Rules

Every DevExpress `GridView` must explicitly set row and header fonts. This includes main grids and popup views used by `SearchLookUpEdit` or `GridLookUpEdit`.

Required pattern:

```csharp
gridView.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
gridView.Appearance.HeaderPanel.Options.UseFont = true;
gridView.Appearance.Row.Font = new Font("Segoe UI", 9F);
gridView.Appearance.Row.Options.UseFont = true;
```

For popup lookup views:

```csharp
searchLookUpEdit.Properties.PopupView = lookupGridView;
lookupGridView.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
lookupGridView.Appearance.HeaderPanel.Options.UseFont = true;
lookupGridView.Appearance.Row.Font = new Font("Segoe UI", 9F);
lookupGridView.Appearance.Row.Options.UseFont = true;
lookupGridView.OptionsView.ShowGroupPanel = false;
```

Align numeric values to the right in edit fields and grid columns, including summaries, totals, quantities, amounts, percentages, prices, costs, stock fields, dimensions, weights, volumes, days, and counters.

## AppTypography

Create or reuse a central `AppTypography` helper in the shared WinForms UI layer, for example:

```text
src/Frontend/NuanSystem.WinForms.Forms/Common/AppTypography.cs
```

Keep the helper compatible with the Visual Studio Designer. Typography helpers must not resize or reposition controls unexpectedly.

```csharp
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;

namespace NuanSystem.WinForms.Forms.Common;

internal static class AppTypography
{
    public static readonly Font BaseFont = new("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
    public static readonly Font BaseReadableFont = new("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
    public static readonly Font LabelFont = new("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
    public static readonly Font InputFont = new("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
    public static readonly Font ButtonFont = new("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
    public static readonly Font TitleFont = new("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
    public static readonly Font SectionFont = new("Segoe UI Semibold", 11F, FontStyle.Bold, GraphicsUnit.Point);
    public static readonly Font GridHeaderFont = new("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
    public static readonly Font GridRowFont = new("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
    public static readonly Font SmallFont = new("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point);

    public static void ApplyToForm(XtraForm form)
    {
        form.Font = BaseFont;
        form.Appearance.Font = BaseFont;
        form.Appearance.Options.UseFont = true;
    }

    public static void ApplyTitle(LabelControl label)
    {
        label.Appearance.Font = TitleFont;
        label.Appearance.Options.UseFont = true;
    }

    public static void ApplyLabel(LabelControl label)
    {
        label.Appearance.Font = LabelFont;
        label.Appearance.Options.UseFont = true;
    }

    public static void ApplyButton(SimpleButton button)
    {
        button.Appearance.Font = ButtonFont;
        button.Appearance.Options.UseFont = true;
    }

    public static void ApplyGrid(GridView gridView)
    {
        gridView.Appearance.HeaderPanel.Font = GridHeaderFont;
        gridView.Appearance.HeaderPanel.Options.UseFont = true;
        gridView.Appearance.Row.Font = GridRowFont;
        gridView.Appearance.Row.Options.UseFont = true;
        gridView.Appearance.FooterPanel.Font = GridHeaderFont;
        gridView.Appearance.FooterPanel.Options.UseFont = true;
        gridView.Appearance.FilterPanel.Font = GridRowFont;
        gridView.Appearance.FilterPanel.Options.UseFont = true;
    }
}
```

## Designer Visibility

- Put typography assignments in `.Designer.cs` when the Visual Studio Designer must show the final appearance.
- Use `AppTypography` from runtime code only when the local form family already applies styling helpers that preserve Designer layout.
- Do not rely only on `Load` events for typography in designer-backed forms.
- For manual forms, apply fonts field by field to labels, inputs, buttons, and grids.

## Operational Sections

For operational document edit forms, avoid `GroupControl` or boxed group panels unless the user explicitly asks for them. Prefer `PanelControl` sections with explicit `LabelControl` titles using `SectionFont`, so the screen reads as a flat ERP document workflow.

## Checklist

Before delivering a DevExpress form:

- The form uses `Segoe UI`.
- Labels, inputs, buttons, and grids have explicit fonts.
- Section titles do not exceed main title hierarchy.
- Prohibited fonts are absent.
- GridView header and row fonts are explicit, including popup lookup views.
- Numeric fields and columns are right-aligned.
- Designer-backed forms show typography without depending on runtime-only code.
- The touched frontend project compiles when practical.
