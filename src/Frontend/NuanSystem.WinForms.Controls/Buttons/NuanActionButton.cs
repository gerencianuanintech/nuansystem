using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace NuanSystem.WinForms.Controls.Buttons;

[ToolboxItem(true)]
public class NuanActionButton : SimpleButton
{
    private const int DefaultWidth = 100;
    private const int DefaultHeight = 36;
    private const int DefaultIconSize = 24;
    private static readonly Font ButtonFont = new("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);

    private NuanActionButtonKind buttonKind = NuanActionButtonKind.Primary;
    private Color normalBackColor = Color.FromArgb(37, 99, 235);
    private Color hoverBackColor = Color.FromArgb(29, 78, 216);
    private Color pressedBackColor = Color.FromArgb(30, 64, 175);
    private Color normalForeColor = Color.White;
    private int iconSize = DefaultIconSize;
    private bool useDefaultSize;
    private bool autoApplyKindStyle = true;
    private string? iconNameOverride;

    public NuanActionButton()
    {
        Size = new Size(DefaultWidth, DefaultHeight);
        Text = "Aceptar";
        ButtonStyle = BorderStyles.UltraFlat;
        LookAndFeel.Style = LookAndFeelStyle.Flat;
        LookAndFeel.UseDefaultLookAndFeel = false;
        ImageOptions.Location = ImageLocation.MiddleLeft;
        ImageOptions.ImageToTextAlignment = ImageAlignToText.None;
        ImageOptions.ImageToTextIndent = 0;
        ApplyKindStyle(updateSize: false);
    }

    [Category("NuanSystem")]
    [Description("Define el estilo visual del boton.")]
    [DefaultValue(NuanActionButtonKind.Primary)]
    public NuanActionButtonKind ButtonKind
    {
        get => buttonKind;
        set
        {
            if (buttonKind == value)
            {
                return;
            }

            buttonKind = value;
            if (autoApplyKindStyle)
            {
                ApplyKindStyle(updateSize: useDefaultSize);
            }
        }
    }

    [Category("NuanSystem")]
    [Description("Texto visible del boton.")]
    [DefaultValue("Aceptar")]
    public string ButtonText
    {
        get => Text;
        set
        {
            Text = value ?? string.Empty;
            ApplyVisualStyle();
        }
    }

    [Category("NuanSystem")]
    [Description("Color de fondo normal.")]
    public Color NormalBackColor
    {
        get => normalBackColor;
        set
        {
            normalBackColor = value;
            ApplyVisualStyle();
        }
    }

    [Category("NuanSystem")]
    [Description("Color de fondo al pasar el mouse.")]
    public Color HoverBackColor
    {
        get => hoverBackColor;
        set
        {
            hoverBackColor = value;
            ApplyVisualStyle();
        }
    }

    [Category("NuanSystem")]
    [Description("Color de fondo al presionar.")]
    public Color PressedBackColor
    {
        get => pressedBackColor;
        set
        {
            pressedBackColor = value;
            ApplyVisualStyle();
        }
    }

    [Category("NuanSystem")]
    [Description("Color del texto e icono.")]
    public Color NormalForeColor
    {
        get => normalForeColor;
        set
        {
            normalForeColor = value;
            ApplyVisualStyle();
            ApplyKindIcon();
        }
    }

    [Category("NuanSystem")]
    [Description("Tamano del icono SVG.")]
    [DefaultValue(DefaultIconSize)]
    public int IconSize
    {
        get => iconSize;
        set
        {
            iconSize = Math.Max(12, value);
            ImageOptions.SvgImageSize = new Size(iconSize, iconSize);
        }
    }

    [Category("NuanSystem")]
    [Description("Restaura el tamano estandar 100 x 36 al aplicar el estilo.")]
    [DefaultValue(false)]
    public bool UseDefaultSize
    {
        get => useDefaultSize;
        set
        {
            useDefaultSize = value;
            if (useDefaultSize)
            {
                Size = new Size(DefaultWidth, DefaultHeight);
            }
        }
    }

    [Category("NuanSystem")]
    [Description("Aplica automaticamente colores, texto e icono al cambiar ButtonKind.")]
    [DefaultValue(true)]
    public bool AutoApplyKindStyle
    {
        get => autoApplyKindStyle;
        set => autoApplyKindStyle = value;
    }

    [Category("NuanSystem")]
    [Description("Nombre del icono SVG en Assets/Icons/Operaciones que reemplaza al icono del estilo.")]
    [DefaultValue(null)]
    public string? IconNameOverride
    {
        get => iconNameOverride;
        set
        {
            iconNameOverride = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
            ApplyKindIcon();
        }
    }

    public void ApplyKindStyle()
    {
        ApplyKindStyle(updateSize: useDefaultSize);
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();
        ApplyVisualStyle();
        ApplyKindIcon();
    }

    private void ApplyKindStyle(bool updateSize)
    {
        var style = NuanActionButtonStyle.FromKind(buttonKind);
        ButtonText = style.Text;
        normalBackColor = style.BackColor;
        hoverBackColor = style.HoverBackColor;
        pressedBackColor = style.PressedBackColor;
        normalForeColor = style.ForeColor;
        if (updateSize)
        {
            Size = new Size(DefaultWidth, DefaultHeight);
        }

        ApplyVisualStyle();
        ApplyKindIcon();
    }

    private void ApplyVisualStyle()
    {
        ButtonStyle = BorderStyles.UltraFlat;
        LookAndFeel.Style = LookAndFeelStyle.Flat;
        LookAndFeel.UseDefaultLookAndFeel = false;

        Appearance.BackColor = normalBackColor;
        Appearance.BorderColor = normalBackColor;
        Appearance.ForeColor = normalForeColor;
        Appearance.Font = ButtonFont;
        Appearance.Options.UseBackColor = true;
        Appearance.Options.UseBorderColor = true;
        Appearance.Options.UseForeColor = true;
        Appearance.Options.UseFont = true;

        AppearanceHovered.BackColor = hoverBackColor;
        AppearanceHovered.BorderColor = hoverBackColor;
        AppearanceHovered.ForeColor = normalForeColor;
        AppearanceHovered.Options.UseBackColor = true;
        AppearanceHovered.Options.UseBorderColor = true;
        AppearanceHovered.Options.UseForeColor = true;

        AppearancePressed.BackColor = pressedBackColor;
        AppearancePressed.BorderColor = pressedBackColor;
        AppearancePressed.ForeColor = normalForeColor;
        AppearancePressed.Options.UseBackColor = true;
        AppearancePressed.Options.UseBorderColor = true;
        AppearancePressed.Options.UseForeColor = true;
    }

    private void ApplyKindIcon([CallerFilePath] string callerFilePath = "")
    {
        var style = NuanActionButtonStyle.FromKind(buttonKind);
        var iconName = iconNameOverride ?? style.IconName;
        ImageOptions.SvgImage = string.IsNullOrWhiteSpace(iconName)
            ? null
            : LoadOperationIcon(iconName, normalForeColor, callerFilePath);
        ImageOptions.SvgImageSize = new Size(iconSize, iconSize);
        ImageOptions.Location = ImageLocation.MiddleLeft;
        ImageOptions.ImageToTextAlignment = ImageAlignToText.None;
        ImageOptions.ImageToTextIndent = 0;
    }

    private bool ShouldSerializeIconNameOverride() => !string.IsNullOrWhiteSpace(iconNameOverride);

    private static SvgImage? LoadOperationIcon(string fileName, Color color, string callerFilePath)
    {
        try
        {
            var iconPath = ResolveIconPath(fileName, callerFilePath);
            return File.Exists(iconPath) ? LoadRecoloredSvg(iconPath, color) : null;
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
        catch (ArgumentException)
        {
            return null;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
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

    private bool ShouldSerializeNormalBackColor() => normalBackColor != NuanActionButtonStyle.FromKind(buttonKind).BackColor;

    private bool ShouldSerializeHoverBackColor() => hoverBackColor != NuanActionButtonStyle.FromKind(buttonKind).HoverBackColor;

    private bool ShouldSerializePressedBackColor() => pressedBackColor != NuanActionButtonStyle.FromKind(buttonKind).PressedBackColor;

    private bool ShouldSerializeNormalForeColor() => normalForeColor != Color.White;
}
