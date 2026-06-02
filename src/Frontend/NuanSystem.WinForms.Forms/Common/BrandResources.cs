using System.Drawing.Drawing2D;

namespace NuanSystem.WinForms.Forms.Common;

internal static class BrandResources
{
    public static readonly Color Background = Color.White;
    public static readonly Color Surface = Color.FromArgb(247, 248, 252);
    public static readonly Color Primary = Color.FromArgb(0, 184, 148);
    public static readonly Color PrimaryHover = Color.FromArgb(0, 161, 132);
    public static readonly Color PrimarySoft = Color.FromArgb(230, 250, 246);
    public static readonly Color Text = Color.FromArgb(23, 32, 51);
    public static readonly Color MutedText = Color.FromArgb(100, 112, 132);
    public static readonly Color Border = Color.FromArgb(221, 226, 240);
    public static readonly Color SuccessText = Color.FromArgb(22, 163, 74);
    public static readonly Color SuccessBack = Color.FromArgb(236, 253, 245);
    public static readonly Color WarningText = Color.FromArgb(217, 119, 6);
    public static readonly Color WarningBack = Color.FromArgb(255, 251, 235);
    public static readonly Color ErrorText = Color.FromArgb(220, 38, 38);
    public static readonly Color ErrorBack = Color.FromArgb(254, 242, 242);
    public static readonly Color CustomerAccent = Color.FromArgb(0, 86, 210);
    public static readonly Color CustomerAccentSoft = Color.FromArgb(232, 241, 255);
    public static readonly Color SupplierAccent = Color.FromArgb(91, 49, 163);
    public static readonly Color SupplierAccentSoft = Color.FromArgb(243, 238, 255);

    public static Image? LoadLogo(bool horizontal = true)
    {
        var fileName = horizontal
            ? "nuan-intech-logo-horizontal.png"
            : "nuan-intech-logo-compact.png";

        var path = Path.Combine(AppContext.BaseDirectory, "Assets", "Brand", fileName);
        if (!File.Exists(path))
        {
            return null;
        }

        using var stream = File.OpenRead(path);
        using var image = Image.FromStream(stream);
        return new Bitmap(image);
    }

    public static GraphicsPath RoundedRectangle(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
        path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
        path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
        path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
        path.CloseFigure();
        return path;
    }
}
