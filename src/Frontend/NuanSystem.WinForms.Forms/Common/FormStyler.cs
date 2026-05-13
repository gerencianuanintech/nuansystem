namespace NuanSystem.WinForms.Forms.Common;

internal static class FormStyler
{
    public static readonly Font TitleFont = new("Segoe UI", 16F, FontStyle.Bold);
    public static readonly Font LabelFont = new("Segoe UI", 9F, FontStyle.Regular);

    public static void ApplyBase(Form form)
    {
        form.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
        form.BackColor = BrandResources.Background;
        form.StartPosition = FormStartPosition.CenterScreen;
    }
}

