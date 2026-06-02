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

    public static void ApplyToControl(Control control)
    {
        control.Font = control switch
        {
            Button or SimpleButton => ButtonFont,
            TextBoxBase or System.Windows.Forms.ComboBox or NumericUpDown or DateTimePicker => InputFont,
            Label or LabelControl => LabelFont,
            DataGridView => GridRowFont,
            _ => BaseFont
        };

        if (control is LabelControl label)
        {
            ApplyLabel(label);
        }
        else if (control is SimpleButton button)
        {
            ApplyButton(button);
        }
        else if (control is BaseEdit editor)
        {
            editor.Properties.Appearance.Font = InputFont;
            editor.Properties.Appearance.Options.UseFont = true;
        }
        else if (control is DataGridView grid)
        {
            grid.ColumnHeadersDefaultCellStyle.Font = GridHeaderFont;
            grid.DefaultCellStyle.Font = GridRowFont;
        }
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
        button.Font = ButtonFont;
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

