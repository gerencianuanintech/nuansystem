using DevExpress.Utils;

namespace NuanSystem.WinForms.Controls.Grids;

public sealed class NuanGridColumnDefinition
{
    public string FieldName { get; set; } = string.Empty;

    public string Caption { get; set; } = string.Empty;

    public int VisibleIndex { get; set; }

    public int Width { get; set; } = 100;

    public bool Visible { get; set; } = true;

    public NuanGridColumnFormat Format { get; set; } = NuanGridColumnFormat.Text;

    public HorzAlignment Alignment { get; set; } = HorzAlignment.Default;

    public bool AllowFilter { get; set; } = true;

    public bool AllowSort { get; set; } = true;
}
