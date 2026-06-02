---
name: nuansystem-enterprise-typography
description: Apply NuanSystem official WinForms/DevExpress typography with Segoe UI fonts, ERP-density sizing, XtraForm, LabelControl, input editors, SimpleButton, GridControl/GridView, PanelControl sections, RibbonControl, AccordionControl, and a Visual Studio Designer-compatible AppTypography class. Use when creating or reviewing frontend UI styling, forms, grids, buttons, labels, or typography consistency.
---

# NuanSystem Enterprise Typography

## Official Font

Use `Segoe UI` for NuanSystem UI.

- Base form font: `Segoe UI 9F` or `Segoe UI 9.75F` Regular.
- Dense ERP forms: use `9F`.
- Forms that need more legibility: use `9.75F`.
- Main title: `Segoe UI Semibold 14F` Bold.
- Section title: `Segoe UI Semibold 11F` Bold.
- Field labels: `Segoe UI 9F` Regular.
- Input controls: `Segoe UI 9F` or `9.75F` Regular.
- Standard single-line editors in dense ERP forms: height `22 px`; vertically stacked field rows use a `26 px` top-to-top cadence.
- Buttons: `Segoe UI Semibold 9F` Bold.
- Grid headers: `Segoe UI Semibold 9F` Bold.
- Grid rows: `Segoe UI 9F` Regular.
- Helper text: `Segoe UI 8.5F` Regular.

## Do Not Introduce

Do not introduce these fonts in new or modified NuanSystem UI:

- Comic Sans.
- Impact.
- Brush Script.
- Papyrus.
- Times New Roman.
- Courier New for normal forms.

`Consolas` is allowed only for logs, SQL, JSON, code, and technical diagnostics.

## DevExpress Rules

- `XtraForm`: set `Font` to `AppTypography.BaseFont` or `AppTypography.BaseReadableFont`.
- `LabelControl`: use `LabelFont` for field labels, `TitleFont` for main titles, and `SectionFont` for section headers.
- `TextEdit`, `LookUpEdit`, `GridLookUpEdit`, `DateEdit`, `MemoEdit`, `SpinEdit`: use `InputFont`.
- `TextEdit`, `SearchLookUpEdit`, `LookUpEdit`, `DateEdit`, and `ComboBoxEdit` used as standard single-line fields must be `Size = new Size(width, 22)` in `.Designer.cs`; when stacked vertically, place the next field 26 px lower on the Y axis.
- `SimpleButton`: use `ButtonFont`; keep captions short and action-oriented.
- `GridControl/GridView`: use `GridRowFont` for rows and `GridHeaderFont` for column headers.
- Every DevExpress `GridView` must explicitly set row and header fonts. This includes main `GridControl` views and popup views used by `SearchLookUpEdit` or `GridLookUpEdit`.
- Grid rows must use `Segoe UI 9F` Regular.
- Grid header captions must use `Segoe UI Semibold 9F` Bold.
- Grid typography must be assigned in `.Designer.cs` for designer-backed forms so the Visual Studio Designer shows the same result as runtime.
- Avoid `GroupControl`/group panels in operational document edit forms unless explicitly requested. Prefer `PanelControl` plus a `LabelControl` section title using `SectionFont`.
- `RibbonControl` or `AccordionControl`: keep fonts aligned to `BaseFont`; do not mix decorative fonts for navigation.

Required `GridView` pattern:

```csharp
gridView.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
gridView.Appearance.HeaderPanel.Options.UseFont = true;
gridView.Appearance.Row.Font = new Font("Segoe UI", 9F);
gridView.Appearance.Row.Options.UseFont = true;
```

When a `SearchLookUpEdit` uses a popup `GridView`, apply the same pattern:

```csharp
searchLookUpEdit.Properties.PopupView = lookupGridView;
lookupGridView.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
lookupGridView.Appearance.HeaderPanel.Options.UseFont = true;
lookupGridView.Appearance.Row.Font = new Font("Segoe UI", 9F);
lookupGridView.Appearance.Row.Options.UseFont = true;
lookupGridView.OptionsView.ShowGroupPanel = false;
```

## AppTypography

Keep this helper compatible with the Visual Studio Designer. Use explicit properties and avoid runtime-only layout factories inside `.Designer.cs`.

```csharp
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;

namespace NuanSystem.WinForms.Common;

public static class AppTypography
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
    }
}
```

## Designer Guidance

- Put typography assignments in `.Designer.cs` when the Visual Studio Designer must show the final appearance.
- Use `AppTypography` from runtime code only when the local form family already applies styling helpers that preserve designer layout.
- Do not use typography helpers that resize or reposition controls unexpectedly.
- Do not mix font families within the same form unless the control displays technical text that explicitly allows `Consolas`.
