namespace NuanSystem.WinForms.Forms.Common;

internal static class FormStyler
{
    public static readonly Font TitleFont = AppTypography.TitleFont;
    public static readonly Font LabelFont = AppTypography.LabelFont;

    public static void ApplyBase(Form form)
    {
        if (form is DevExpress.XtraEditors.XtraForm xtraForm)
        {
            AppTypography.ApplyToForm(xtraForm);
        }

        form.Font = AppTypography.BaseFont;
        form.BackColor = BrandResources.Background;
        form.StartPosition = FormStartPosition.CenterScreen;
    }

    public static void ApplyTypography(Control parent)
    {
        foreach (Control control in parent.Controls)
        {
            AppTypography.ApplyToControl(control);

            if (control.HasChildren)
            {
                ApplyTypography(control);
            }
        }
    }

    public static void ApplyPanelBackColor(Control parent, Color backColor)
    {
        foreach (Control control in parent.Controls)
        {
            if (control is DevExpress.XtraEditors.PanelControl panel)
            {
                panel.Appearance.BackColor = backColor;
                panel.Appearance.Options.UseBackColor = true;
            }

            if (control.HasChildren)
            {
                ApplyPanelBackColor(control, backColor);
            }
        }
    }

    public static void ApplyPanelInheritedBackColor(Control parent)
    {
        foreach (Control control in parent.Controls)
        {
            if (control is DevExpress.XtraEditors.PanelControl panel)
            {
                var inheritedBackColor = panel.Parent?.BackColor ?? parent.BackColor;
                panel.Appearance.BackColor = inheritedBackColor;
                panel.Appearance.Options.UseBackColor = true;
            }

            if (control.HasChildren)
            {
                ApplyPanelInheritedBackColor(control);
            }
        }
    }

    public static void ApplyPanelTitles(Control parent, Color titleColor)
    {
        foreach (Control control in parent.Controls)
        {
            if (control is DevExpress.XtraEditors.PanelControl panel
                && !string.IsNullOrWhiteSpace(panel.Text)
                && !panel.Controls.ContainsKey($"{panel.Name}Title"))
            {
                var title = new DevExpress.XtraEditors.LabelControl
                {
                    Name = $"{panel.Name}Title",
                    Text = panel.Text,
                    Location = new Point(13, 10),
                    AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Default
                };

                title.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
                title.Appearance.ForeColor = titleColor;
                title.Appearance.Options.UseFont = true;
                title.Appearance.Options.UseForeColor = true;

                panel.Controls.Add(title);
                title.BringToFront();
            }

            if (control.HasChildren)
            {
                ApplyPanelTitles(control, titleColor);
            }
        }
    }
}
