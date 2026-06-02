using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using System.Runtime.CompilerServices;
using System.Text;

namespace NuanSystem.WinForms.Forms.Common;

internal static class OperationButtonIcons
{
    private const int DefaultIconSize = 32;
    private const int DefaultButtonHeight = 36;
    private static readonly Color SaveBackColor = Color.FromArgb(0, 184, 148);
    private static readonly Color SaveForeColor = Color.White;
    private static readonly Color SaveHoverBackColor = Color.FromArgb(0, 160, 128);
    private static readonly Color CancelBackColor = Color.FromArgb(99, 110, 114);
    private static readonly Color CancelHoverBackColor = Color.FromArgb(78, 87, 90);

    public static void ApplySaveCancel(
        SimpleButton saveButton,
        SimpleButton cancelButton,
        [CallerFilePath] string callerFilePath = "")
    {
        var saveBounds = saveButton.Bounds;
        var cancelBounds = cancelButton.Bounds;

        ApplySvg(cancelButton, "cancelar_32.svg", Color.White, callerFilePath);
        ApplySvg(saveButton, "diskette_32.svg", SaveForeColor, callerFilePath);

        saveButton.Bounds = saveBounds;
        cancelButton.Bounds = cancelBounds;
    }

    public static void ApplySave(SimpleButton button, [CallerFilePath] string callerFilePath = "")
    {
        button.Height = DefaultButtonHeight;
        button.Appearance.BackColor = SaveBackColor;
        button.Appearance.ForeColor = SaveForeColor;
        button.AppearanceHovered.BackColor = SaveHoverBackColor;
        button.AppearanceHovered.ForeColor = SaveForeColor;
        button.Appearance.Options.UseBackColor = true;
        button.Appearance.Options.UseForeColor = true;
        button.AppearanceHovered.Options.UseBackColor = true;
        button.AppearanceHovered.Options.UseForeColor = true;
        button.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        button.LookAndFeel.UseDefaultLookAndFeel = false;
        ApplySvg(button, "diskette_32.svg", SaveForeColor, callerFilePath);
    }

    public static SvgImage? LoadOperationIcon(
        string fileName,
        Color color,
        [CallerFilePath] string callerFilePath = "")
    {
        var iconPath = ResolveIconPath(fileName, callerFilePath);
        return File.Exists(iconPath) ? LoadRecoloredSvg(iconPath, color) : null;
    }

    private static void ApplyCancelStyle(SimpleButton button)
    {
        button.Height = DefaultButtonHeight;
        button.Appearance.BackColor = CancelBackColor;
        button.Appearance.ForeColor = Color.White;
        button.AppearanceHovered.BackColor = CancelHoverBackColor;
        button.AppearanceHovered.ForeColor = Color.White;
        button.Appearance.Options.UseBackColor = true;
        button.Appearance.Options.UseForeColor = true;
        button.AppearanceHovered.Options.UseBackColor = true;
        button.AppearanceHovered.Options.UseForeColor = true;
        button.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        button.LookAndFeel.UseDefaultLookAndFeel = false;
    }

    private static void PlaceCancelBeforeSave(SimpleButton saveButton, SimpleButton cancelButton)
    {
        if (saveButton.Parent != cancelButton.Parent || cancelButton.Left <= saveButton.Left)
        {
            return;
        }

        var saveLocation = saveButton.Location;
        saveButton.Location = cancelButton.Location;
        cancelButton.Location = saveLocation;
    }

    private static void ApplySvg(SimpleButton button, string fileName, Color? iconColor = null, string callerFilePath = "")
    {
        var iconPath = ResolveIconPath(fileName, callerFilePath);
        if (!File.Exists(iconPath))
        {
            return;
        }

        button.ImageOptions.SvgImage = iconColor.HasValue
            ? LoadRecoloredSvg(iconPath, iconColor.Value)
            : SvgImage.FromFile(iconPath);
        button.ImageOptions.SvgImageSize = new Size(DefaultIconSize, DefaultIconSize);
    }

    private static string ResolveIconPath(string fileName, string callerFilePath)
    {
        var relativePath = Path.Combine("Assets", "Icons", "Operaciones", fileName);
        if (!string.IsNullOrWhiteSpace(callerFilePath))
        {
            var callerDirectory = new DirectoryInfo(Path.GetDirectoryName(callerFilePath) ?? string.Empty);
            while (callerDirectory is not null)
            {
                var callerProjectPath = Path.Combine(callerDirectory.FullName, relativePath);
                if (File.Exists(callerProjectPath))
                {
                    return callerProjectPath;
                }

                callerDirectory = callerDirectory.Parent;
            }
        }

        var outputPath = Path.Combine(AppContext.BaseDirectory, relativePath);
        if (File.Exists(outputPath))
        {
            return outputPath;
        }

        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var projectPath = Path.Combine(
                directory.FullName,
                "src",
                "Frontend",
                "NuanSystem.WinForms.Forms",
                relativePath);
            if (File.Exists(projectPath))
            {
                return projectPath;
            }

            var localPath = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(localPath))
            {
                return localPath;
            }

            directory = directory.Parent;
        }

        return outputPath;
    }

    private static SvgImage LoadRecoloredSvg(string iconPath, Color color)
    {
        var svg = File.ReadAllText(iconPath, Encoding.UTF8);
        var colorValue = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        svg = svg.Replace("#0F6E56", colorValue, StringComparison.OrdinalIgnoreCase)
            .Replace("#00B894", colorValue, StringComparison.OrdinalIgnoreCase);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(svg));
        return SvgImage.FromStream(stream);
    }
}
